using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace MyWinFormsApp
{
    public class InputSupportForm : Form
    {
        private TextBox filterTextBox;
        private ListBox itemListBox;
        private readonly string[] _allItems;
        public string SelectedValue { get; private set; }

        public InputSupportForm()
        {
            _allItems = HospitalData.Load().GetAll().ToArray();
            InitializeComponent();
            UpdateListBox("");

            this.ActiveControl = filterTextBox;
        }

        private void InitializeComponent()
        {
            this.Text = "入力支援";
            this.Width = 300;
            this.Height = 400;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.StartPosition = FormStartPosition.CenterParent;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            filterTextBox = new TextBox
            {
                Location = new Point(10, 10),
                Width = 260,
                Height = 25,
                Font = new Font("Segoe UI", 10),
                ImeMode = ImeMode.Hiragana
            };
            filterTextBox.TextChanged += FilterTextBox_TextChanged;
            filterTextBox.KeyDown += FilterTextBox_KeyDown;

            itemListBox = new ListBox
            {
                Location = new Point(10, 45),
                Width = 260,
                Height = 300,
                Font = new Font("Segoe UI", 10)
            };
            itemListBox.DoubleClick += ItemListBox_DoubleClick;
            itemListBox.KeyDown += ItemListBox_KeyDown;

            this.Controls.Add(filterTextBox);
            this.Controls.Add(itemListBox);
        }

        private void FilterTextBox_TextChanged(object sender, EventArgs e)
        {
            UpdateListBox(filterTextBox.Text);
        }

        private void FilterTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Down)
            {
                itemListBox.Focus();
                if (itemListBox.Items.Count > 0)
                    itemListBox.SelectedIndex = 0;
            }
            else if (e.KeyCode == Keys.Enter)
            {
                ConfirmSelection();
            }
        }

        private void UpdateListBox(string filter)
        {
            itemListBox.BeginUpdate();
            itemListBox.Items.Clear();
            var filtered = string.IsNullOrWhiteSpace(filter)
                ? _allItems
                : _allItems.Where(i => i.IndexOf(filter, StringComparison.OrdinalIgnoreCase) >= 0).ToArray();
            itemListBox.Items.AddRange(filtered);
            if (itemListBox.Items.Count > 0)
                itemListBox.SelectedIndex = 0;
            itemListBox.EndUpdate();
        }

        private void ItemListBox_DoubleClick(object sender, EventArgs e)
        {
            ConfirmSelection();
        }

        private void ItemListBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                ConfirmSelection();
            }
            else if (e.KeyCode == Keys.Up && itemListBox.SelectedIndex == 0)
            {
                filterTextBox.Focus();
            }
        }

        private void ConfirmSelection()
        {
            if (itemListBox.SelectedItem != null)
            {
                SelectedValue = itemListBox.SelectedItem.ToString();
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
        }
    }
}
