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
            lblValue = new Label();
            txtValue = new TextBox();
            lblDescription = new Label();
            txtDescription = new TextBox();
            btnOK = new Button();
            btnCancel = new Button();
            SuspendLayout();
            // 
            // lblValue
            // 
            lblValue.Location = new Point(0, 0);
            lblValue.Name = "lblValue";
            lblValue.Size = new Size(100, 23);
            lblValue.TabIndex = 0;
            // 
            // txtValue
            // 
            txtValue.Location = new Point(0, 0);
            txtValue.Name = "txtValue";
            txtValue.Size = new Size(100, 23);
            txtValue.TabIndex = 1;
            // 
            // lblDescription
            // 
            lblDescription.Location = new Point(0, 0);
            lblDescription.Name = "lblDescription";
            lblDescription.Size = new Size(100, 23);
            lblDescription.TabIndex = 2;
            // 
            // txtDescription
            // 
            txtDescription.Location = new Point(0, 0);
            txtDescription.Name = "txtDescription";
            txtDescription.Size = new Size(100, 23);
            txtDescription.TabIndex = 3;
            // 
            // btnOK
            // 
            btnOK.Location = new Point(0, 0);
            btnOK.Name = "btnOK";
            btnOK.Size = new Size(75, 23);
            btnOK.TabIndex = 4;
            btnOK.Click += BtnOK_Click;
            // 
            // btnCancel
            // 
            btnCancel.Location = new Point(0, 0);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new Size(75, 23);
            btnCancel.TabIndex = 5;
            btnCancel.Click += BtnCancel_Click;
            // 
            // AddItemForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(32, 32, 32);
            ClientSize = new Size(284, 161);
            Controls.Add(lblValue);
            Controls.Add(txtValue);
            Controls.Add(lblDescription);
            Controls.Add(txtDescription);
            Controls.Add(btnOK);
            Controls.Add(btnCancel);
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
