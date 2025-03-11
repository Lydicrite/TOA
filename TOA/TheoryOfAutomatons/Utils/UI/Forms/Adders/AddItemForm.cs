using System;
using System.Drawing;
using System.Windows.Forms;
using TheoryOfAutomatons.Utils.UI.Controls;
using System.ComponentModel;

namespace TheoryOfAutomatons.Utils.UI.Forms.Adders
{
    internal partial class AddItemForm : Form
    {
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public object Value { get; private set; }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string Description { get; private set; }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public SupportedTypes SelectedType { get; private set; }

        public AddItemForm(SupportedTypes type = SupportedTypes.Char)
        {
            SelectedType = type;
            InitializeComponent();

            btnOk.Click += btnOK_Click;
            btnCancel.Click += btnCancel_Click;
        }

        private void btnOK_Click(object sender, EventArgs e)
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

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }




        private Label lblValue;
        private TextBox txtValue;
        private TextBox txtDescription;
        private Label lblDescription;
        private Button btnOk;
        private Button btnCancel;

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
            lblValue = new Label();
            txtValue = new TextBox();
            txtDescription = new TextBox();
            lblDescription = new Label();
            btnOk = new Button();
            btnCancel = new Button();
            SuspendLayout();
            // 
            // lblValue
            // 
            lblValue.AutoSize = true;
            lblValue.Location = new Point(12, 15);
            lblValue.Name = "lblValue";
            lblValue.Size = new Size(63, 15);
            lblValue.TabIndex = 0;
            lblValue.Text = "Значение:";
            // 
            // txtValue
            // 
            txtValue.BackColor = Color.FromArgb(46, 46, 46);
            txtValue.BorderStyle = BorderStyle.FixedSingle;
            txtValue.ForeColor = Color.Gainsboro;
            txtValue.Location = new Point(81, 12);
            txtValue.Name = "txtValue";
            txtValue.Size = new Size(191, 23);
            txtValue.TabIndex = 1;
            // 
            // txtDescription
            // 
            txtDescription.BackColor = Color.FromArgb(46, 46, 46);
            txtDescription.BorderStyle = BorderStyle.FixedSingle;
            txtDescription.ForeColor = Color.Gainsboro;
            txtDescription.Location = new Point(81, 41);
            txtDescription.Name = "txtDescription";
            txtDescription.Size = new Size(191, 23);
            txtDescription.TabIndex = 3;
            // 
            // lblDescription
            // 
            lblDescription.AutoSize = true;
            lblDescription.Location = new Point(12, 44);
            lblDescription.Name = "lblDescription";
            lblDescription.Size = new Size(65, 15);
            lblDescription.TabIndex = 2;
            lblDescription.Text = "Описание:";
            // 
            // btnOk
            // 
            btnOk.BackColor = Color.FromArgb(46, 46, 46);
            btnOk.FlatAppearance.BorderColor = Color.FromArgb(64, 64, 64);
            btnOk.FlatAppearance.MouseDownBackColor = Color.Gray;
            btnOk.FlatAppearance.MouseOverBackColor = Color.DimGray;
            btnOk.FlatStyle = FlatStyle.Flat;
            btnOk.Location = new Point(12, 126);
            btnOk.Name = "btnOk";
            btnOk.Size = new Size(75, 23);
            btnOk.TabIndex = 4;
            btnOk.Text = "Добавить";
            btnOk.UseVisualStyleBackColor = false;
            // 
            // btnCancel
            // 
            btnCancel.BackColor = Color.FromArgb(46, 46, 46);
            btnCancel.FlatAppearance.BorderColor = Color.FromArgb(64, 64, 64);
            btnCancel.FlatAppearance.MouseDownBackColor = Color.Gray;
            btnCancel.FlatAppearance.MouseOverBackColor = Color.DimGray;
            btnCancel.FlatStyle = FlatStyle.Flat;
            btnCancel.Location = new Point(197, 126);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new Size(75, 23);
            btnCancel.TabIndex = 5;
            btnCancel.Text = "Отмена";
            btnCancel.UseVisualStyleBackColor = false;
            // 
            // AddItemForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(32, 32, 32);
            ClientSize = new Size(284, 161);
            Controls.Add(btnCancel);
            Controls.Add(btnOk);
            Controls.Add(txtDescription);
            Controls.Add(lblDescription);
            Controls.Add(txtValue);
            Controls.Add(lblValue);
            ForeColor = Color.Gainsboro;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "AddItemForm";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Добавить элемент";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
    }
}
