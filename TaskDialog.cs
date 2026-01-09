using System;
using System.Drawing;
using System.Windows.Forms;

namespace MyWinFormsApp
{
    public class TaskDialog : Form
    {
        private Label titleLabel;
        private TextBox titleTextBox;
        private Label assigneeLabel;
        private TextBox assigneeTextBox;
        private Label priorityLabel;
        private ComboBox priorityComboBox;
        private Button okButton;
        private Button cancelButton;

        public string TaskTitle { get; private set; } = string.Empty;
        public string TaskAssignee { get; private set; } = string.Empty;
        public TaskPriority TaskPriority { get; private set; } = TaskPriority.Medium;

        private int? _taskNumber;

        public TaskDialog(string title = "", string assignee = "", TaskPriority priority = TaskPriority.Medium, int? taskNumber = null)
        {
            TaskTitle = title;
            TaskAssignee = assignee;
            TaskPriority = priority;
            _taskNumber = taskNumber;
            InitializeComponent();
            if (titleTextBox != null)
                titleTextBox.Text = title;
            if (assigneeTextBox != null)
                assigneeTextBox.Text = assignee;
            if (priorityComboBox != null)
                priorityComboBox.SelectedIndex = (int)priority;
        }

        private void InitializeComponent()
        {
            if (string.IsNullOrEmpty(TaskTitle))
            {
                this.Text = _taskNumber.HasValue ? $"新しいタスク (#{_taskNumber})" : "新しいタスク";
            }
            else
            {
                this.Text = _taskNumber.HasValue ? $"タスクを編集 (#{_taskNumber})" : "タスクを編集";
            }
            this.Width = 540;
            this.Height = 350;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.StartPosition = FormStartPosition.CenterParent;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            titleLabel = new Label
            {
                Text = "タイトル:",
                Location = new Point(20, 20),
                Width = 100,
                Height = 20
            };

            titleTextBox = new TextBox
            {
                Location = new Point(20, 45),
                Width = 480,
                Height = 140,
                Multiline = true,
                ScrollBars = ScrollBars.Vertical,
                AcceptsReturn = true,
                Font = new Font("Segoe UI", 11),
                ImeMode = ImeMode.Hiragana
            };

            assigneeLabel = new Label
            {
                Text = "担当:",
                Location = new Point(20, 200),
                Width = 100,
                Height = 20
            };

            assigneeTextBox = new TextBox
            {
                Location = new Point(20, 225),
                Width = 200,
                Height = 25,
                Font = new Font("Segoe UI", 11),
                ImeMode = ImeMode.Hiragana
            };

            priorityLabel = new Label
            {
                Text = "優先度:",
                Location = new Point(240, 200),
                Width = 100,
                Height = 20
            };

            priorityComboBox = new ComboBox
            {
                Location = new Point(240, 225),
                Width = 150,
                Height = 25,
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            priorityComboBox.Items.Add("低");
            priorityComboBox.Items.Add("中");
            priorityComboBox.Items.Add("高");

            okButton = new Button
            {
                Text = "OK",
                DialogResult = DialogResult.OK,
                Location = new Point(320, 260),
                Width = 80,
                Height = 30
            };
            okButton.Click += OkButton_Click;

            cancelButton = new Button
            {
                Text = "キャンセル",
                DialogResult = DialogResult.Cancel,
                Location = new Point(410, 260),
                Width = 90,
                Height = 30
            };

            this.Controls.Add(titleLabel);
            this.Controls.Add(titleTextBox);
            this.Controls.Add(assigneeLabel);
            this.Controls.Add(assigneeTextBox);
            this.Controls.Add(priorityLabel);
            this.Controls.Add(priorityComboBox);
            this.Controls.Add(okButton);
            this.Controls.Add(cancelButton);

            this.AcceptButton = okButton;
            this.CancelButton = cancelButton;
        }

        private void OkButton_Click(object sender, EventArgs e)
        {
            TaskTitle = titleTextBox.Text.Trim();
            TaskAssignee = assigneeTextBox.Text.Trim();
            TaskPriority = (TaskPriority)priorityComboBox.SelectedIndex;

            if (string.IsNullOrWhiteSpace(TaskTitle))
            {
                MessageBox.Show("タイトルを入力してください。", "入力エラー",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                this.DialogResult = DialogResult.None;
            }
        }
    }
}
