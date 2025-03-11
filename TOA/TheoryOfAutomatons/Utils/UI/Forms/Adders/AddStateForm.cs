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

            btnOk.Click += BtnOK_Click;
            btnCancel.Click += BtnCancel_Click;
        }

        private void BtnOK_Click(object sender, EventArgs e)
        {
            try
            {
                StateDescription = txtDescription.Text;
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

        private Button btnCancel;
        private Button btnOk;
        private TextBox txtDescription;
        private Label lblDescription;




        #region Windows Form Designer generated code

        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

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
            btnCancel = new Button();
            btnOk = new Button();
            txtDescription = new TextBox();
            lblDescription = new Label();
            SuspendLayout();
            // 
            // btnCancel
            // 
            btnCancel.BackColor = Color.FromArgb(46, 46, 46);
            btnCancel.FlatAppearance.BorderColor = Color.FromArgb(64, 64, 64);
            btnCancel.FlatAppearance.MouseDownBackColor = Color.Gray;
            btnCancel.FlatAppearance.MouseOverBackColor = Color.DimGray;
            btnCancel.FlatStyle = FlatStyle.Flat;
            btnCancel.ForeColor = Color.Gainsboro;
            btnCancel.Location = new Point(197, 85);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new Size(75, 23);
            btnCancel.TabIndex = 11;
            btnCancel.Text = "Отмена";
            btnCancel.UseVisualStyleBackColor = false;
            // 
            // btnOk
            // 
            btnOk.BackColor = Color.FromArgb(46, 46, 46);
            btnOk.FlatAppearance.BorderColor = Color.FromArgb(64, 64, 64);
            btnOk.FlatAppearance.MouseDownBackColor = Color.Gray;
            btnOk.FlatAppearance.MouseOverBackColor = Color.DimGray;
            btnOk.FlatStyle = FlatStyle.Flat;
            btnOk.ForeColor = Color.Gainsboro;
            btnOk.Location = new Point(12, 85);
            btnOk.Name = "btnOk";
            btnOk.Size = new Size(75, 23);
            btnOk.TabIndex = 10;
            btnOk.Text = "Добавить";
            btnOk.UseVisualStyleBackColor = false;
            // 
            // txtDescription
            // 
            txtDescription.BackColor = Color.FromArgb(46, 46, 46);
            txtDescription.BorderStyle = BorderStyle.FixedSingle;
            txtDescription.ForeColor = Color.Gainsboro;
            txtDescription.Location = new Point(81, 12);
            txtDescription.Name = "txtDescription";
            txtDescription.Size = new Size(191, 23);
            txtDescription.TabIndex = 9;
            // 
            // lblDescription
            // 
            lblDescription.AutoSize = true;
            lblDescription.ForeColor = Color.Gainsboro;
            lblDescription.Location = new Point(12, 15);
            lblDescription.Name = "lblDescription";
            lblDescription.Size = new Size(65, 15);
            lblDescription.TabIndex = 8;
            lblDescription.Text = "Описание:";
            // 
            // AddStateForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(32, 32, 32);
            ClientSize = new Size(284, 114);
            Controls.Add(btnCancel);
            Controls.Add(btnOk);
            Controls.Add(txtDescription);
            Controls.Add(lblDescription);
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
