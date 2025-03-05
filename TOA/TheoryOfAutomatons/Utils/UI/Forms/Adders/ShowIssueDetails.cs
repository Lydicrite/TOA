using System;
using System.Drawing;
using System.Windows.Forms;
using TheoryOfAutomatons.Utils.UI.Controls;
using System.ComponentModel;

namespace TheoryOfAutomatons.Utils.UI.Forms.Adders
{
    internal partial class ShowIssueDetails : Form
    {
        private string IssueInfo;
        public ShowIssueDetails(string info)
        {
            InitializeComponent();
            IssueInfo = info;
            infoRTB.Text = IssueInfo;
        }

  
        #region Windows Form Designer generated code

        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;
        public RichTextBox infoRTB;

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
            infoRTB = new RichTextBox();
            SuspendLayout();
            // 
            // infoRTB
            // 
            infoRTB.BackColor = Color.Black;
            infoRTB.Dock = DockStyle.Fill;
            infoRTB.ForeColor = Color.Gainsboro;
            infoRTB.Location = new Point(0, 0);
            infoRTB.Margin = new Padding(4, 3, 4, 3);
            infoRTB.Name = "infoRTB";
            infoRTB.ReadOnly = true;
            infoRTB.Size = new Size(784, 561);
            infoRTB.TabIndex = 1;
            infoRTB.Text = "";
            // 
            // ShowIssueDetails
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(32, 32, 32);
            ClientSize = new Size(784, 561);
            Controls.Add(infoRTB);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            Name = "ShowIssueDetails";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Данные о проблеме";
            ResumeLayout(false);
        }

        #endregion
    }
}
