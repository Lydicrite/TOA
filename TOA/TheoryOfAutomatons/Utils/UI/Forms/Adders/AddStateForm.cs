using System;
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;

namespace TheoryOfAutomatons.Utils.UI.Forms.Adders
{
    internal partial class AddStateForm : Form
    {
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
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
            lblDescr = new Label();
            txtDescr = new TextBox();
            btnOK = new Button();
            btnCancel = new Button();
            SuspendLayout();
            // 
            // lblDescr
            // 
            lblDescr.Location = new Point(0, 0);
            lblDescr.Name = "lblDescr";
            lblDescr.Size = new Size(100, 23);
            lblDescr.TabIndex = 0;
            // 
            // txtDescr
            // 
            txtDescr.Location = new Point(0, 0);
            txtDescr.Name = "txtDescr";
            txtDescr.Size = new Size(100, 23);
            txtDescr.TabIndex = 1;
            // 
            // btnOK
            // 
            btnOK.Location = new Point(0, 0);
            btnOK.Name = "btnOK";
            btnOK.Size = new Size(75, 23);
            btnOK.TabIndex = 2;
            btnOK.Click += BtnOK_Click;
            // 
            // btnCancel
            // 
            btnCancel.Location = new Point(0, 0);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new Size(75, 23);
            btnCancel.TabIndex = 3;
            btnCancel.Click += BtnCancel_Click;
            // 
            // AddStateForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(32, 32, 32);
            ClientSize = new Size(284, 161);
            Controls.Add(lblDescr);
            Controls.Add(txtDescr);
            Controls.Add(btnOK);
            Controls.Add(btnCancel);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "AddStateForm";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Добавить состояние Автомата";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
    }
}
