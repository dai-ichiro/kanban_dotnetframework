using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Text.Json;
using System.Text.Encodings.Web;
using System.Text.Unicode;

namespace MyWinFormsApp
{
    public partial class Form1 : Form
    {
        private List<TaskItem> tasks;
        private KanbanColumn todoColumn;
        private KanbanColumn inProgressColumn;
        private KanbanColumn doneColumn;
        private Button addTaskButton;
        private TableLayoutPanel mainLayout;
        private string dataFilePath;

        public Form1()
        {
            InitializeComponent();
            tasks = new List<TaskItem>();
            var exeDirectory = AppDomain.CurrentDomain.BaseDirectory;
            dataFilePath = Path.Combine(exeDirectory, "tasks.json");
            InitializeKanbanBoard();
            LoadTasks();
            RefreshBoard();
        }

        private void InitializeKanbanBoard()
        {
            this.Text = "カンバン タスク管理 (.NET Framework 4.8)";
            this.Size = new Size(1200, 800);
            this.MinimumSize = new Size(800, 600);

            addTaskButton = new Button
            {
                Text = "新しいタスクを追加",
                Dock = DockStyle.Top,
                Height = 50,
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                BackColor = Color.FromArgb(76, 175, 80),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            addTaskButton.Click += AddTaskButton_Click;

            mainLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 3,
                RowCount = 1,
                Padding = new Padding(0),
                Margin = new Padding(0)
            };

            mainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33F));
            mainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33F));
            mainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.34F));
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

            todoColumn = new KanbanColumn(TaskStatus.Todo, "TODO", Color.FromArgb(158, 158, 158));
            inProgressColumn = new KanbanColumn(TaskStatus.InProgress, "進行中", Color.FromArgb(33, 150, 243));
            doneColumn = new KanbanColumn(TaskStatus.Done, "完了", Color.FromArgb(76, 175, 80));

            todoColumn.TaskMoved += Column_TaskMoved;
            inProgressColumn.TaskMoved += Column_TaskMoved;
            doneColumn.TaskMoved += Column_TaskMoved;

            mainLayout.Controls.Add(todoColumn, 0, 0);
            mainLayout.Controls.Add(inProgressColumn, 1, 0);
            mainLayout.Controls.Add(doneColumn, 2, 0);

            this.Controls.Add(mainLayout);
            this.Controls.Add(addTaskButton);
        }

        private void AddTaskButton_Click(object sender, EventArgs e)
        {
            int maxNumber = 0;
            if (tasks.Count > 0)
            {
                maxNumber = tasks.Max(t => t.TaskNumber);
            }
            int nextNumber = maxNumber + 1;

            using (var dialog = new TaskDialog(taskNumber: nextNumber))
            {
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    var task = new TaskItem(dialog.TaskTitle);
                    task.Assignee = dialog.TaskAssignee;
                    task.Priority = dialog.TaskPriority;
                    task.TaskNumber = nextNumber;

                    tasks.Add(task);
                    SaveTasks();
                    RefreshBoard();
                }
            }
        }

        private void Column_TaskMoved(object sender, TaskCard taskCard)
        {
            SaveTasks();
            RefreshBoard();
        }

        private void RefreshBoard()
        {
            todoColumn.ClearTasks();
            inProgressColumn.ClearTasks();
            doneColumn.ClearTasks();

            foreach (var task in tasks)
            {
                if (task.Status == TaskStatus.Deleted) continue;

                var taskCard = new TaskCard(task);
                taskCard.EditClicked += (s, e) => EditTask(task);
                taskCard.DeleteClicked += (s, e) => DeleteTask(task);

                switch (task.Status)
                {
                    case TaskStatus.Todo:
                        todoColumn.AddTask(taskCard);
                        break;
                    case TaskStatus.InProgress:
                        inProgressColumn.AddTask(taskCard);
                        break;
                    case TaskStatus.Done:
                        doneColumn.AddTask(taskCard);
                        break;
                }
            }
        }

        private void EditTask(TaskItem task)
        {
            using (var dialog = new TaskDialog(task.Title, task.Assignee, task.Priority, task.TaskNumber))
            {
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    task.Title = dialog.TaskTitle;
                    task.Assignee = dialog.TaskAssignee;
                    task.Priority = dialog.TaskPriority;
                    SaveTasks();
                    RefreshBoard();
                }
            }
        }

        private void DeleteTask(TaskItem task)
        {
            var result = MessageBox.Show(
                $"タスク '{task.Title}' を削除しますか?",
                "削除の確認",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );

            if (result == DialogResult.Yes)
            {
                var oldStatus = task.Status;
                task.Status = TaskStatus.Deleted;
                task.MoveHistory.Add(new TaskMoveHistory
                {
                    FromStatus = oldStatus,
                    ToStatus = TaskStatus.Deleted,
                    MovedDate = DateTime.Now
                });

                SaveTasks();
                RefreshBoard();
            }
        }

        private void SaveTasks()
        {
            try
            {
                var directory = Path.GetDirectoryName(dataFilePath);
                if (directory != null && !Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                var json = JsonSerializer.Serialize(tasks, new JsonSerializerOptions
                {
                    WriteIndented = true,
                    Encoder = JavaScriptEncoder.Create(UnicodeRanges.All)
                });
                File.WriteAllText(dataFilePath, json);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"保存中にエラーが発生しました: {ex.Message}", "エラー",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadTasks()
        {
            try
            {
                if (File.Exists(dataFilePath))
                {
                    var json = File.ReadAllText(dataFilePath);
                    var loadedTasks = JsonSerializer.Deserialize<List<TaskItem>>(json);
                    if (loadedTasks != null)
                    {
                        tasks = loadedTasks.Where(t => t.Status != TaskStatus.Deleted).ToList();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"読み込み中にエラーが発生しました: {ex.Message}", "エラー",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
