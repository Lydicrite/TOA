using System;
using System.Drawing;
using System.Windows.Forms;
using TheoryOfAutomatons.Utils.UI.Controls;

namespace TheoryOfAutomatons.Utils.UI.Forms.Adders
{
    internal partial class AddItemForm : Form
    {
        public object Value { get; private set; }
        public string Description { get; private set; }
        public SupportedTypes SelectedType { get; private set; }

        public AddItemForm(SupportedTypes type = SupportedTypes.Char)
        {
            SelectedType = type;
            InitializeComponent();
        }

        private void BtnOK_Click(object sender, EventArgs e)
        {
            try
            {
                switch (SelectedType)
                {
                    case SupportedTypes.Int:
                        Value = int.Parse(txtValue.Text);
                        break;
                    case SupportedTypes.Char:
                        if (txtValue.Text.Length != 1)
                            throw new FormatException("Значение должно быть одним символом.");
                        Value = txtValue.Text[0];
                        break;
                    case SupportedTypes.String:
                        Value = txtValue.Text;
                        break;
                    default:
                        throw new NotSupportedException("Неподдерживаемый тип.");
                }

                Description = txtDescription.Text;
                this.DialogResult = DialogResult.OK;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка ввода: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }




        #region Windows Form Designer generated code

        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;
        private Label lblValue;
        private Label lblDescription;
        private TextBox txtValue;
        private TextBox txtDescription;
        private Button btnOK;
        private Button btnCancel;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Text = "Добавить элемент";
            this.Size = new Size(300, 200);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.StartPosition = FormStartPosition.CenterParent;

            lblValue = new Label() { Text = "Значение:", Left = 10, Top = 20, Width = 80 };
            txtValue = new TextBox() { Left = 100, Top = 20, Width = 150 };

            lblDescription = new Label() { Text = "Описание:", Left = 10, Top = 60, Width = 80 };
            txtDescription = new TextBox() { Left = 100, Top = 60, Width = 150 };

            btnOK = new Button() { Text = "OK", Left = 50, Top = 100, Width = 80 };
            btnOK.Click += BtnOK_Click;

            btnCancel = new Button() { Text = "Отмена", Left = 150, Top = 100, Width = 80 };
            btnCancel.Click += BtnCancel_Click;

            this.Controls.Add(lblValue);
            this.Controls.Add(txtValue);
            this.Controls.Add(lblDescription);
            this.Controls.Add(txtDescription);
            this.Controls.Add(btnOK);
            this.Controls.Add(btnCancel);
        }

        #endregion
    }
}
