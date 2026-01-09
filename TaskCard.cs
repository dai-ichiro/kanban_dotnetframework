using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace MyWinFormsApp
{
    public class TaskCard : Panel
    {
        private Label titleLabel;
        private Label assigneeLabel;
        private TableLayoutPanel timestampsPanel;
        private Button editButton;
        private Button deleteButton;
        private Panel bottomContainer;
        private Panel buttonPanel;

        [System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)]
        public TaskItem Task { get; set; }
        public event EventHandler EditClicked;
        public event EventHandler DeleteClicked;

        public TaskCard(TaskItem task)
        {
            Task = task;
            InitializeComponent();
            UpdateDisplay();
        }

        private void InitializeComponent()
        {
            this.BorderStyle = BorderStyle.FixedSingle;
            this.BackColor = Color.White;
            this.Padding = new Padding(10, 6, 10, 6);
            this.MinimumSize = new Size(200, 160);
            this.Height = 165; // 高さを維持

            // --- 1. タイトル (2行分) ---
            titleLabel = new Label
            {
                AutoSize = false,
                Dock = DockStyle.Top,
                Height = 50,
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                TextAlign = ContentAlignment.TopLeft,
                BackColor = Color.Transparent,
                Cursor = Cursors.Hand,
                AutoEllipsis = true
            };
            titleLabel.MouseDown += TaskCard_MouseDown;

            // --- 2. 担当者 ---
            assigneeLabel = new Label
            {
                AutoSize = false,
                Dock = DockStyle.Top,
                Height = 22,
                Font = new Font("Segoe UI", 9.5F, FontStyle.Regular),
                TextAlign = ContentAlignment.MiddleLeft,
                BackColor = Color.Transparent
            };
            assigneeLabel.MouseDown += TaskCard_MouseDown;

            // --- 3. ボトムコンテナ (ボタン縦並び + 時間表示広め) ---
            bottomContainer = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 65,
                BackColor = Color.Transparent
            };

            // 【左側】ボタンパネル (縦並びにするために幅を狭く)
            buttonPanel = new Panel
            {
                Dock = DockStyle.Left,
                Width = 65,
                BackColor = Color.Transparent
            };

            editButton = new Button
            {
                Text = "編集",
                Size = new Size(58, 24),
                Location = new Point(0, 5),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.White,
                Font = new Font("Segoe UI", 8)
            };
            editButton.Click += (s, e) => EditClicked?.Invoke(this, EventArgs.Empty);

            deleteButton = new Button
            {
                Text = "削除",
                Size = new Size(58, 24),
                Location = new Point(0, 34),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(240, 128, 128),
                Font = new Font("Segoe UI", 8)
            };
            deleteButton.Click += (s, e) => DeleteClicked?.Invoke(this, EventArgs.Empty);

            buttonPanel.Controls.Add(editButton);
            buttonPanel.Controls.Add(deleteButton);

            // 【右側】タイムスタンプパネル (領域を最大化)
            timestampsPanel = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                RowCount = 1,
                ColumnCount = 3,
                BackColor = Color.Transparent,
                Padding = new Padding(2, 5, 0, 0)
            };
            timestampsPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33F));
            timestampsPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33F));
            timestampsPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.34F));

            bottomContainer.Controls.Add(timestampsPanel);
            bottomContainer.Controls.Add(buttonPanel);

            this.Controls.Add(bottomContainer);
            this.Controls.Add(assigneeLabel);
            this.Controls.Add(titleLabel);

            this.MouseDown += TaskCard_MouseDown;
        }

        private void TaskCard_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                DoDragDrop(this, DragDropEffects.Move);
            }
        }

        public void UpdateDisplay()
        {
            titleLabel.Text = $"#{Task.TaskNumber} {Task.Title}";
            assigneeLabel.Text = string.IsNullOrEmpty(Task.Assignee) ? "" : $"担当: {Task.Assignee}";

            switch (Task.Priority)
            {
                case TaskPriority.High:
                    this.BackColor = Color.FromArgb(255, 230, 230);
                    break;
                case TaskPriority.Medium:
                    this.BackColor = Color.FromArgb(255, 248, 225);
                    break;
                case TaskPriority.Low:
                default:
                    this.BackColor = Color.White;
                    break;
            }

            while (timestampsPanel.Controls.Count > 0)
            {
                var c = timestampsPanel.Controls[0];
                timestampsPanel.Controls.RemoveAt(0);
                c.Dispose();
            }

            timestampsPanel.Controls.Add(CreateTimestampLabel("作成", Task.CreatedDate), 0, 0);

            if (Task.Status == TaskStatus.InProgress || Task.Status == TaskStatus.Done)
            {
                var started = Task.MoveHistory
                    .Where(h => h.ToStatus == TaskStatus.InProgress)
                    .OrderByDescending(h => h.MovedDate)
                    .FirstOrDefault();

                if (started != null)
                {
                    timestampsPanel.Controls.Add(CreateTimestampLabel("着手", started.MovedDate), 1, 0);
                }
            }

            if (Task.Status == TaskStatus.Done)
            {
                var completed = Task.MoveHistory
                    .Where(h => h.ToStatus == TaskStatus.Done)
                    .OrderByDescending(h => h.MovedDate)
                    .FirstOrDefault();

                if (completed != null)
                {
                    timestampsPanel.Controls.Add(CreateTimestampLabel("完了", completed.MovedDate), 2, 0);
                }
            }
        }

        private Label CreateTimestampLabel(string label, DateTime date)
        {
            return new Label
            {
                Text = $"{label}\n{date:yyyy/MM/dd}\n{date:HH:mm}",
                AutoSize = false,
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 7.8F, FontStyle.Regular),
                TextAlign = ContentAlignment.TopLeft,
                ForeColor = Color.FromArgb(100, 100, 100)
            };
        }
    }
}
