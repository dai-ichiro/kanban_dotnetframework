using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace MyWinFormsApp
{
    public class KanbanColumn : Panel
    {
        private Label headerLabel;
        private FlowLayoutPanel tasksPanel;

        [System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)]
        public TaskStatus Status { get; set; }
        public event EventHandler<TaskCard> TaskMoved;

        public KanbanColumn(TaskStatus status, string title, Color headerColor)
        {
            Status = status;
            InitializeComponent(title, headerColor);
        }

        private void InitializeComponent(string title, Color headerColor)
        {
            this.BorderStyle = BorderStyle.FixedSingle;
            this.BackColor = Color.WhiteSmoke;
            this.Dock = DockStyle.Fill;
            this.AllowDrop = true;

            headerLabel = new Label
            {
                Text = title,
                Dock = DockStyle.Top,
                Height = 40,
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                BackColor = headerColor,
                ForeColor = Color.White
            };

            tasksPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                Padding = new Padding(10),
                AllowDrop = true
            };

            this.Controls.Add(tasksPanel);
            this.Controls.Add(headerLabel);

            this.DragEnter += KanbanColumn_DragEnter;
            this.DragDrop += KanbanColumn_DragDrop;
            this.DragOver += KanbanColumn_DragEnter;
            tasksPanel.DragEnter += KanbanColumn_DragEnter;
            tasksPanel.DragDrop += KanbanColumn_DragDrop;
            tasksPanel.DragOver += KanbanColumn_DragEnter;
            tasksPanel.Resize += TasksPanel_Resize;
            headerLabel.AllowDrop = true;
            headerLabel.DragEnter += KanbanColumn_DragEnter;
            headerLabel.DragDrop += KanbanColumn_DragDrop;
            headerLabel.DragOver += KanbanColumn_DragEnter;
        }

        private void TasksPanel_Resize(object sender, EventArgs e)
        {
            int cardWidth = tasksPanel.ClientSize.Width - tasksPanel.Padding.Left - tasksPanel.Padding.Right;
            if (cardWidth < 250) cardWidth = 250;

            foreach (TaskCard card in tasksPanel.Controls.OfType<TaskCard>())
            {
                card.Width = cardWidth;
            }
        }

        private void KanbanColumn_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(TaskCard)))
            {
                e.Effect = DragDropEffects.Move;
            }
        }

        private void KanbanColumn_DragDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetData(typeof(TaskCard)) is TaskCard taskCard)
            {
                if (taskCard.Task.Status != this.Status)
                {
                    taskCard.Task.MoveHistory.Add(new TaskMoveHistory
                    {
                        FromStatus = taskCard.Task.Status,
                        ToStatus = this.Status,
                        MovedDate = DateTime.Now
                    });
                }
                taskCard.Task.Status = this.Status;
                TaskMoved?.Invoke(this, taskCard);
            }
        }

        public void AddTask(TaskCard taskCard)
        {
            int cardWidth = tasksPanel.ClientSize.Width - tasksPanel.Padding.Left - tasksPanel.Padding.Right;
            if (cardWidth < 250) cardWidth = 250;
            taskCard.Width = cardWidth;
            tasksPanel.Controls.Add(taskCard);
        }

        public void RemoveTask(TaskCard taskCard)
        {
            tasksPanel.Controls.Remove(taskCard);
        }

        public void ClearTasks()
        {
            tasksPanel.Controls.Clear();
        }

        public List<TaskCard> GetTaskCards()
        {
            return tasksPanel.Controls.OfType<TaskCard>().ToList();
        }
    }
}
