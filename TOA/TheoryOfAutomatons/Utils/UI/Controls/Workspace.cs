using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TheoryOfAutomatons.Utils.UI.Controls
{
    public partial class Workspace : UserControl
    {
        private float scaleFactor = 1.0f;
        private float minScaleFactor = 0.05f;
        private Bitmap workspaceBitmap;
        private Graphics workspaceGraphics;

        private VScrollBar vScrollBar;
        private HScrollBar hScrollBar;

        private int zoomStepPercentage = 1;

        private int scaledWidth;
        private int scaledHeight;
        private int visibleWidth;
        private int visibleHeight;

        /// <summary>
        /// Текущий коэффициент масштабирования.
        /// </summary>
        [Category("Layout")]
        [Description("Текущий коэффициент масштабирования.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public float ScaleFactor
        {
            get => scaleFactor;
            set
            {
                if (value < minScaleFactor)
                    scaleFactor = minScaleFactor;
                else if (value >= 1.0f)
                    scaleFactor = 1.0f;
                else
                    scaleFactor = value;

                RecalculateSizes();
                UpdateScrollBars();
                this.Invalidate();
            }
        }

        /// <summary>
        /// Минимальный коэффициент масштабирования.
        /// </summary>
        [Category("Layout")]
        [Description("Минимальный коэффициент масштабирования.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public float MinScaleFactor
        {
            get => minScaleFactor;
            set
            {
                if (value < 0.05f)
                    minScaleFactor = 0.05f;
                else if (value >= 1.0f)
                    minScaleFactor = 1.0f;
                else
                    minScaleFactor = value;

                if (scaleFactor < minScaleFactor)
                    scaleFactor = minScaleFactor;

                RecalculateSizes();
                UpdateScrollBars();
                this.Invalidate();
            }
        }

        /// <summary>
        /// Шаг изменения масштаба.
        /// </summary>
        [Category("Layout")]
        [Description("Шаг изменения масштаба в процентах при прокрутке колесом мыши.")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int ZoomStepPercentage
        {
            get => zoomStepPercentage;
            set
            {
                if (value >= 1 && value <= 10)
                    zoomStepPercentage = value;
            }
        }

        /// <summary>
        /// Объект Graphics для использования.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Graphics GraphicsObject
        {
            get => workspaceGraphics;
            set
            {
                if (value != null)
                    workspaceGraphics = value;
            }
        }

        public Workspace()
        {
            InitializeComponent();
            InitializeWorkspace();
        }

        private void InitializeWorkspace()
        {
            CreateBitmap();
            this.DoubleBuffered = true;

            this.MouseWheel += Workspace_MouseWheel;
            this.MouseDown += Workspace_MouseDown;
            this.MouseUp += Workspace_MouseUp;
            this.Resize += Workspace_Resize;
        }

        private void CreateBitmap()
        {
            workspaceBitmap = new Bitmap(this.Size.Width, this.Size.Height);
            workspaceGraphics = Graphics.FromImage(workspaceBitmap);
            workspaceGraphics.Clear(this.BackColor);
        }



        private void Workspace_Resize(object sender, EventArgs e)
        {
            CreateBitmap();
            RecalculateSizes();
            UpdateScrollBars();
            this.Invalidate();
        }

        private void Workspace_MouseWheel(object sender, MouseEventArgs e)
        {
            if (ModifierKeys.HasFlag(Keys.Control))
            {
                float zoomChange = zoomStepPercentage / 100.0f;
                ScaleFactor += (e.Delta > 0) ? zoomChange : -zoomChange;
            }
        }

        private void Workspace_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && ModifierKeys.HasFlag(Keys.Control))
            {
                this.Cursor = Cursors.SizeAll;
            }
        }

        private void Workspace_MouseUp(object sender, MouseEventArgs e)
        {
            this.Cursor = Cursors.Default;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            if (workspaceBitmap == null)
                return;

            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;

            // Получаем текущие значения прокрутки
            int scrollX = hScrollBar.Value;
            int scrollY = vScrollBar.Value;

            // Растягиваем изображение
            Rectangle srcRect = new Rectangle((int)(scrollX / scaleFactor), (int)(scrollY / scaleFactor),
                (int)(visibleWidth / scaleFactor), (int)(visibleHeight / scaleFactor));

            // Ограничиваем область источника битмапа
            srcRect.Width = Math.Min(srcRect.Width, workspaceBitmap.Width - srcRect.X);
            srcRect.Height = Math.Min(srcRect.Height, workspaceBitmap.Height - srcRect.Y);

            Rectangle destRect = new Rectangle(0, 0, visibleWidth, visibleHeight);

            g.DrawImage(workspaceBitmap, destRect, srcRect, GraphicsUnit.Pixel);
        }



        private void UpdateScrollBars()
        {
            if (workspaceBitmap == null)
                return;

            // Обновляем максимумы и видимые области полос прокрутки
            hScrollBar.Maximum = scaledWidth > visibleWidth ? scaledWidth - 1 : 0;
            hScrollBar.LargeChange = visibleWidth;
            hScrollBar.Enabled = scaledWidth > visibleWidth;

            vScrollBar.Maximum = scaledHeight > visibleHeight ? scaledHeight - 1 : 0;
            vScrollBar.LargeChange = visibleHeight;
            vScrollBar.Enabled = scaledHeight > visibleHeight;
        }

        private void RecalculateSizes()
        {
            scaledWidth = (int)(workspaceBitmap.Width * scaleFactor);
            scaledHeight = (int)(workspaceBitmap.Height * scaleFactor);
            visibleWidth = this.ClientSize.Width - (vScrollBar.Visible ? vScrollBar.Width : 0);
            visibleHeight = this.ClientSize.Height - (hScrollBar.Visible ? hScrollBar.Height : 0);
        }

        public void SetScale(float newScaleFactor)
        {
            if (newScaleFactor < minScaleFactor)
                newScaleFactor = minScaleFactor;

            scaleFactor = newScaleFactor;
            UpdateScrollBars();
            this.Invalidate();
        }

        public void ResetScale()
        {
            scaleFactor = 1.0f;
            UpdateScrollBars();
            this.Invalidate();
        }

        public void ClearWorkspace(Color color)
        {
            if (workspaceGraphics != null)
            {
                workspaceGraphics.Clear(color);
                this.Invalidate();
            }
        }

        #region Код, автоматически созданный конструктором компонентов

        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.vScrollBar = new System.Windows.Forms.VScrollBar();
            this.hScrollBar = new System.Windows.Forms.HScrollBar();
            this.SuspendLayout();
            // 
            // vScrollBar
            // 
            this.vScrollBar.Dock = System.Windows.Forms.DockStyle.Right;
            this.vScrollBar.Location = new System.Drawing.Point(324, 0);
            this.vScrollBar.Name = "vScrollBar";
            this.vScrollBar.Size = new System.Drawing.Size(17, 247);
            this.vScrollBar.TabIndex = 0;
            // 
            // hScrollBar
            // 
            this.hScrollBar.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.hScrollBar.Location = new System.Drawing.Point(0, 247);
            this.hScrollBar.Name = "hScrollBar";
            this.hScrollBar.Size = new System.Drawing.Size(341, 17);
            this.hScrollBar.TabIndex = 1;
            // 
            // Workspace
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(96)))), ((int)(((byte)(96)))), ((int)(((byte)(96)))));
            this.Controls.Add(this.vScrollBar);
            this.Controls.Add(this.hScrollBar);
            this.Name = "Workspace";
            this.Size = new System.Drawing.Size(341, 264);
            this.ResumeLayout(false);

        }

        #endregion
    }
}
