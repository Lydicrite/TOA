using System;
using System.Drawing;
using System.Windows.Forms;

namespace TheoryOfAutomatons.Utils.UI.Forms.Adders
{
    internal partial class AddStateForm : Form
    {
        public object StateDescription { get; private set; }

        public AddStateForm()
        {
            InitializeComponent();
        }

        private void BtnOK_Click(object sender, EventArgs e)
        {
            try
            {
                StateDescription = txtDescr.Text;
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
        private Label lblDescr;
        private TextBox txtDescr;
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
            this.Text = "Добавить состояние Автомата";
            this.Size = new Size(300, 200);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.StartPosition = FormStartPosition.CenterParent;

            lblDescr = new Label() { Text = "Описание:", Left = 10, Top = 20, Width = 80 };
            txtDescr = new TextBox() { Left = 100, Top = 20, Width = 150 };

            btnOK = new Button() { Text = "OK", Left = 50, Top = 60, Width = 80 };
            btnOK.Click += BtnOK_Click;

            btnCancel = new Button() { Text = "Отмена", Left = 150, Top = 60, Width = 80 };
            btnCancel.Click += BtnCancel_Click;

            this.Controls.Add(lblDescr);
            this.Controls.Add(txtDescr);
            this.Controls.Add(btnOK);
            this.Controls.Add(btnCancel);
        }

        #endregion
    }
}
