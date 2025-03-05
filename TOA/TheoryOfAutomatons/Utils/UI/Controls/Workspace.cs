using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TheoryOfAutomatons.Utils.UI.Controls
{
    [ToolboxItem(true)]
    public partial class Workspace : UserControl
    {
        private Panel WorkspacePanel;
        private HScrollBar HScrollBar;
        private VScrollBar VScrollBar;
        public PictureBox WorkspacePB;
        private TrackBar ZoomTrackBar;
        private ToolTip WorkspaceToolTip;
        private ContextMenuStrip WorkspaceContextMenu;
        private ToolStripMenuItem fitToWindow;
        private ToolStripMenuItem resetZoom;


        #region Image handling properties

        private Image originalImage;
        private Bitmap displayImage;
        private float zoomFactor = 1.0f;
        private const float MinZoom = 0.1f;
        private const float MaxZoom = 10.0f;
        private const float ZoomStep = 0.1f;
        private PointF panOffset = new PointF(0, 0);
        private Point lastMousePosition;
        private bool isPanning = false;

        #endregion



        #region Публичные свойства

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Image Image
        {
            get => originalImage;
            set
            {
                if (originalImage != null)
                {
                    originalImage.Dispose();
                    originalImage = null;
                }
                originalImage = value;
                UpdateDisplayImage();
                AdjustScrollbars();
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public float ZoomFactor
        {
            get => zoomFactor;
            set
            {
                value = Math.Clamp(value, MinZoom, MaxZoom);
                if (zoomFactor != value)
                {
                    zoomFactor = value;
                    ZoomTrackBar.Value = (int)(zoomFactor * 100);
                    UpdateDisplayImage();
                    AdjustScrollbars();
                }
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Graphics ImageGraphics
        {
            get
            {
                if (originalImage == null)
                    return null;

                return Graphics.FromImage(originalImage);
            }
        }

        #endregion



        public Workspace()
        {
            InitializeComponent();
            SetupGraphicsQuality();
            UpdateDisplayImage();
            AdjustScrollbars();
            RefreshImage();
        }



        #region Методы настройки отображения

        private void SetupGraphicsQuality()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint |
                    ControlStyles.UserPaint |
                    ControlStyles.OptimizedDoubleBuffer, true);
        }

        private void UpdateDisplayImage()
        {
            if (originalImage == null)
            {
                WorkspacePB.Image = null;
                originalImage = new Bitmap(this.Size.Width, this.Size.Height, PixelFormat.Format32bppArgb);
            }

            int newWidth = (int)(originalImage.Width * zoomFactor);
            int newHeight = (int)(originalImage.Height * zoomFactor);

            if (newWidth <= 0 || newHeight <= 0)
                return;

            // Create a new bitmap with the zoomed size
            displayImage?.Dispose();
            displayImage = new Bitmap(newWidth, newHeight, PixelFormat.Format32bppArgb);;

            // Set high quality rendering
            using (Graphics g = Graphics.FromImage(displayImage))
            {
                if (g.SmoothingMode != SmoothingMode.AntiAlias)
                    g.SmoothingMode = SmoothingMode.AntiAlias;
                if (g.TextRenderingHint != TextRenderingHint.AntiAlias)
                    g.TextRenderingHint = TextRenderingHint.AntiAlias;
                if (g.PixelOffsetMode != PixelOffsetMode.HighQuality)
                    g.PixelOffsetMode = PixelOffsetMode.HighQuality;
                if (g.InterpolationMode != InterpolationMode.HighQualityBicubic)
                    g.InterpolationMode = InterpolationMode.HighQualityBicubic;

                g.DrawImage(originalImage, new Rectangle(0, 0, newWidth, newHeight));
            }

            WorkspacePB.Size = new Size(newWidth, newHeight);
            WorkspacePB.Image = displayImage;
            Invalidate();
        }

        private void AdjustScrollbars()
        {
            if (originalImage == null)
                return;

            int imageWidth = (int)(originalImage.Width * zoomFactor);
            int imageHeight = (int)(originalImage.Height * zoomFactor);

            // Configure horizontal scrollbar
            if (imageWidth > WorkspacePanel.Width)
            {
                HScrollBar.Minimum = 0;
                HScrollBar.Maximum = imageWidth;
                HScrollBar.LargeChange = WorkspacePanel.Width;
                HScrollBar.SmallChange = WorkspacePanel.Width / 10;
                HScrollBar.Enabled = true;
            }
            else
            {
                HScrollBar.Value = 0;
                HScrollBar.Enabled = false;
            }

            // Configure vertical scrollbar
            if (imageHeight > WorkspacePB.Height)
            {
                VScrollBar.Minimum = 0;
                VScrollBar.Maximum = imageHeight;
                VScrollBar.LargeChange = WorkspacePB.Height;
                VScrollBar.SmallChange = WorkspacePB.Height / 10;
                VScrollBar.Enabled = true;
            }
            else
            {
                VScrollBar.Value = 0;
                VScrollBar.Enabled = false;
            }
        }

        #endregion



        #region Преобразование координат и проверки

        public Point ScreenToReal(Point screenPoint)
        {
            if (originalImage == null) return Point.Empty;

            // Convert screen coordinates to real image coordinates
            int imageX = screenPoint.X - WorkspacePB.Left;
            int imageY = screenPoint.Y - WorkspacePB.Top;

            // Convert to original image coordinates
            int realX = (int)(imageX / zoomFactor);
            int realY = (int)(imageY / zoomFactor);

            return new Point(realX, realY);
        }

        public Point RealToScreen(Point realPoint)
        {
            if (originalImage == null) return Point.Empty;

            // Convert real image coordinates to screen coordinates
            int screenX = (int)(realPoint.X * zoomFactor) + WorkspacePB.Left;
            int screenY = (int)(realPoint.Y * zoomFactor) + WorkspacePB.Top;

            return new Point(screenX, screenY);
        }

        private bool IsCloserToHorizontalEdge(Point mousePosition)
        {
            // Calculate distances to edges
            int distToLeftEdge = mousePosition.X;
            int distToRightEdge = WorkspacePanel.Width - mousePosition.X;
            int distToTopEdge = mousePosition.Y;
            int distToBottomEdge = WorkspacePanel.Height - mousePosition.Y;

            int minHorizontalDist = Math.Min(distToLeftEdge, distToRightEdge);
            int minVerticalDist = Math.Min(distToTopEdge, distToBottomEdge);

            return minHorizontalDist < minVerticalDist;
        }

        public bool IsPointInWorkspace(Point realPoint)
        {
            return realPoint.X >= 0 && realPoint.X < originalImage.Width &&
                   realPoint.Y >= 0 && realPoint.Y < originalImage.Height;
        }

        #endregion



        #region Утилиты

        private void ZoomToPoint(float newZoom, Point zoomCenter)
        {
            if (originalImage == null || Math.Abs(newZoom - zoomFactor) < 0.001f)
                return;

            Point imagePoint = new Point(
                zoomCenter.X - WorkspacePB.Left,
                zoomCenter.Y - WorkspacePB.Top
            );

            float realX = imagePoint.X / zoomFactor;
            float realY = imagePoint.Y / zoomFactor;

            zoomFactor = Math.Clamp(newZoom, MinZoom, MaxZoom);
            ZoomTrackBar.Value = (int)(zoomFactor * 100);
            UpdateDisplayImage();

            int newImageX = (int)(realX * zoomFactor);
            int newImageY = (int)(realY * zoomFactor);

            WorkspacePB.Left = Math.Clamp(
                zoomCenter.X - newImageX,
                WorkspacePanel.Width - WorkspacePB.Width,
                0
            );

            WorkspacePB.Top = Math.Clamp(
                zoomCenter.Y - newImageY,
                WorkspacePanel.Height - WorkspacePB.Height,
                0
            );

            AdjustScrollbars();
            SyncScrollbarsWithPosition();
        }

        public void FitToWindow()
        {
            if (originalImage == null || WorkspacePanel.Width <= 0 || WorkspacePanel.Height <= 0)
                return;

            float widthRatio = (float)WorkspacePanel.Width / originalImage.Width;
            float heightRatio = (float)WorkspacePanel.Height / originalImage.Height;

            // Use the smaller ratio to ensure the entire image fits
            ZoomFactor = Math.Min(widthRatio, heightRatio);

            // Center the image
            CenterImage();
        }

        private void CenterImage()
        {
            if (originalImage == null) return;

            WorkspacePB.Left = Math.Clamp(
                (WorkspacePanel.Width - WorkspacePB.Width) / 2,
                WorkspacePanel.Width - WorkspacePB.Width,
                0
            );

            WorkspacePB.Top = Math.Clamp(
                (WorkspacePanel.Height - WorkspacePB.Height) / 2,
                WorkspacePanel.Height - WorkspacePB.Height,
                0
            );

            SyncScrollbarsWithPosition();
        }

        private void SyncScrollbarsWithPosition()
        {
            if (HScrollBar.Enabled)
                HScrollBar.Value = Math.Clamp(-WorkspacePB.Left, HScrollBar.Minimum, HScrollBar.Maximum - HScrollBar.LargeChange);

            if (VScrollBar.Enabled)
                VScrollBar.Value = Math.Clamp(-WorkspacePB.Top, VScrollBar.Minimum, VScrollBar.Maximum - VScrollBar.LargeChange);
        }
        #endregion



        #region Методы перерисовки

        public void ResetZoom()
        {
            ZoomFactor = 1.0f;
            CenterImage();
        }

        public void RefreshImage()
        {
            if (displayImage != null)
            {
                WorkspacePB.Invalidate();
            }
        }

        #endregion



        #region Обработчики событий

        private void DoScrollbarsChange(object sender, EventArgs e)
        {
            if (sender == HScrollBar)
            {
                WorkspacePB.Left = -HScrollBar.Value;
            }
            else if (sender == VScrollBar)
            {
                WorkspacePB.Top = -VScrollBar.Value;
            }
        }

        private void zoomTrackBar_ValueChanged(object sender, EventArgs e)
        {
            float newZoom = ZoomTrackBar.Value / 100f;
            if (Math.Abs(newZoom - zoomFactor) > 0.001f)
            {
                ZoomToPoint(newZoom, new Point(WorkspacePanel.Width / 2, WorkspacePanel.Height / 2));
            }
        }

        private void vScrollBar1_ValueChanged(object sender, EventArgs e)
        {
            DoScrollbarsChange(sender, e);
        }

        private void hScrollBar1_ValueChanged(object sender, EventArgs e)
        {
            DoScrollbarsChange(sender, e);
        }

        private void containerPB_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                isPanning = true;
                lastMousePosition = e.Location;
                Cursor = Cursors.Hand;
            }
        }

        private void containerPB_MouseMove(object sender, MouseEventArgs e)
        {
            Point currentPosition = e.Location;

            // Update tooltip to show pixel info
            if (originalImage != null)
            {
                Point realPoint = ScreenToReal(currentPosition);
                if (realPoint.X >= 0 && realPoint.X < originalImage.Width &&
                    realPoint.Y >= 0 && realPoint.Y < originalImage.Height)
                {
                    WorkspaceToolTip.SetToolTip(WorkspacePB, $"X: {realPoint.X}, Y: {realPoint.Y}");
                }
            }

            // Handle panning
            if (isPanning)
            {
                int deltaX = currentPosition.X - lastMousePosition.X;
                int deltaY = currentPosition.Y - lastMousePosition.Y;

                int newX = WorkspacePB.Left + deltaX;
                int newY = WorkspacePB.Top + deltaY;

                // Apply constraints
                newX = Math.Min(0, Math.Max(newX, WorkspacePanel.Width - WorkspacePB.Width));
                newY = Math.Min(0, Math.Max(newY, WorkspacePanel.Height - WorkspacePB.Height));

                WorkspacePB.Left = newX;
                WorkspacePB.Top = newY;

                // Update scrollbars
                if (HScrollBar.Enabled)
                    HScrollBar.Value = Math.Min(HScrollBar.Maximum - HScrollBar.LargeChange, Math.Max(HScrollBar.Minimum, -newX));

                if (VScrollBar.Enabled)
                    VScrollBar.Value = Math.Min(VScrollBar.Maximum - VScrollBar.LargeChange, Math.Max(VScrollBar.Minimum, -newY));

                lastMousePosition = currentPosition;
            }
        }

        private void containerPB_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                isPanning = false;
                Cursor = Cursors.Default;
            }
        }

        private void containerPB_MouseWheel(object sender, MouseEventArgs e)
        {
            if (originalImage == null) return;

            if (ModifierKeys.HasFlag(Keys.Control))
            {
                float zoomDelta = e.Delta > 0 ? ZoomStep : -ZoomStep;
                ZoomToPoint(zoomFactor + zoomDelta, e.Location);
            }
            else
            {
                int scrollValue = e.Delta > 0 ? -HScrollBar.SmallChange : HScrollBar.SmallChange;

                if (IsCloserToHorizontalEdge(e.Location))
                    HScrollBar.Value = Math.Clamp(HScrollBar.Value + scrollValue, HScrollBar.Minimum, HScrollBar.Maximum - HScrollBar.LargeChange);
                else
                    VScrollBar.Value = Math.Clamp(VScrollBar.Value + scrollValue, VScrollBar.Minimum, VScrollBar.Maximum - VScrollBar.LargeChange);
            }
        }

        private void containerPB_Paint(object sender, PaintEventArgs e)
        {
            if (displayImage == null)
                return;

            e.Graphics.SmoothingMode = SmoothingMode.HighQuality;
            e.Graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
            e.Graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
            e.Graphics.CompositingQuality = CompositingQuality.HighQuality;
            e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
        }

        private void fitToWindow_Click(object sender, EventArgs e)
        {
            FitToWindow();
        }

        private void resetZoom_Click(object sender, EventArgs e)
        {
            ResetZoom();
        }

        #endregion





        public void DisposeImageGraphics()
        {
            var graphics = ImageGraphics;
            graphics?.Dispose();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                DisposeImageGraphics();
                displayImage?.Dispose();
                originalImage?.Dispose();
                WorkspaceToolTip?.Dispose();
                WorkspaceContextMenu?.Dispose();
            }
            base.Dispose(disposing);
        }



        #region Код, автоматически созданный конструктором компонентов

        private System.ComponentModel.IContainer components = null;

        private void InitializeComponent()
        {
            components = new Container();
            WorkspacePanel = new Panel();
            ZoomTrackBar = new TrackBar();
            HScrollBar = new HScrollBar();
            VScrollBar = new VScrollBar();
            WorkspacePB = new PictureBox();
            WorkspaceContextMenu = new ContextMenuStrip(components);
            fitToWindow = new ToolStripMenuItem();
            resetZoom = new ToolStripMenuItem();
            WorkspaceToolTip = new ToolTip(components);
            WorkspacePanel.SuspendLayout();
            ((ISupportInitialize)ZoomTrackBar).BeginInit();
            ((ISupportInitialize)WorkspacePB).BeginInit();
            WorkspaceContextMenu.SuspendLayout();
            SuspendLayout();
            // 
            // WorkspacePanel
            // 
            WorkspacePanel.BackColor = Color.FromArgb(32, 32, 32);
            WorkspacePanel.BorderStyle = BorderStyle.Fixed3D;
            WorkspacePanel.Controls.Add(ZoomTrackBar);
            WorkspacePanel.Controls.Add(HScrollBar);
            WorkspacePanel.Controls.Add(VScrollBar);
            WorkspacePanel.Controls.Add(WorkspacePB);
            WorkspacePanel.Dock = DockStyle.Fill;
            WorkspacePanel.Location = new Point(0, 0);
            WorkspacePanel.Name = "WorkspacePanel";
            WorkspacePanel.Size = new Size(500, 500);
            WorkspacePanel.TabIndex = 0;
            // 
            // ZoomTrackBar
            // 
            ZoomTrackBar.Dock = DockStyle.Bottom;
            ZoomTrackBar.LargeChange = 25;
            ZoomTrackBar.Location = new Point(0, 434);
            ZoomTrackBar.Maximum = 1000;
            ZoomTrackBar.Minimum = 10;
            ZoomTrackBar.Name = "ZoomTrackBar";
            ZoomTrackBar.Size = new Size(479, 45);
            ZoomTrackBar.SmallChange = 10;
            ZoomTrackBar.TabIndex = 3;
            ZoomTrackBar.TickFrequency = 25;
            ZoomTrackBar.Value = 100;
            ZoomTrackBar.ValueChanged += zoomTrackBar_ValueChanged;
            // 
            // HScrollBar
            // 
            HScrollBar.Dock = DockStyle.Bottom;
            HScrollBar.Location = new Point(0, 479);
            HScrollBar.Name = "HScrollBar";
            HScrollBar.Size = new Size(479, 17);
            HScrollBar.TabIndex = 2;
            HScrollBar.ValueChanged += hScrollBar1_ValueChanged;
            // 
            // VScrollBar
            // 
            VScrollBar.Dock = DockStyle.Right;
            VScrollBar.Location = new Point(479, 0);
            VScrollBar.Name = "VScrollBar";
            VScrollBar.Size = new Size(17, 496);
            VScrollBar.TabIndex = 1;
            VScrollBar.ValueChanged += vScrollBar1_ValueChanged;
            // 
            // WorkspacePB
            // 
            WorkspacePB.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            WorkspacePB.BackColor = Color.FromArgb(96, 96, 96);
            WorkspacePB.BorderStyle = BorderStyle.FixedSingle;
            WorkspacePB.ContextMenuStrip = WorkspaceContextMenu;
            WorkspacePB.Location = new Point(3, 3);
            WorkspacePB.Name = "WorkspacePB";
            WorkspacePB.Size = new Size(473, 422);
            WorkspacePB.SizeMode = PictureBoxSizeMode.AutoSize;
            WorkspacePB.TabIndex = 0;
            WorkspacePB.TabStop = false;
            WorkspacePB.Paint += containerPB_Paint;
            WorkspacePB.MouseDown += containerPB_MouseDown;
            WorkspacePB.MouseMove += containerPB_MouseMove;
            WorkspacePB.MouseUp += containerPB_MouseUp;
            WorkspacePB.MouseWheel += containerPB_MouseWheel;
            // 
            // WorkspaceContextMenu
            // 
            WorkspaceContextMenu.Items.AddRange(new ToolStripItem[] { fitToWindow, resetZoom });
            WorkspaceContextMenu.Name = "contextMenu";
            WorkspaceContextMenu.Size = new Size(214, 70);
            WorkspaceContextMenu.Text = "Меню рабочего поля";
            // 
            // fitToWindow
            // 
            fitToWindow.Name = "fitToWindow";
            fitToWindow.Size = new Size(213, 22);
            fitToWindow.Text = "Сделать по размеру окна";
            fitToWindow.Click += fitToWindow_Click;
            // 
            // resetZoom
            // 
            resetZoom.Name = "resetZoom";
            resetZoom.Size = new Size(213, 22);
            resetZoom.Text = "К реальному размеру";
            resetZoom.Click += resetZoom_Click;
            // 
            // Workspace
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(96, 96, 96);
            Controls.Add(WorkspacePanel);
            Margin = new Padding(4, 3, 4, 3);
            Name = "Workspace";
            Size = new Size(500, 500);
            WorkspacePanel.ResumeLayout(false);
            WorkspacePanel.PerformLayout();
            ((ISupportInitialize)ZoomTrackBar).EndInit();
            ((ISupportInitialize)WorkspacePB).EndInit();
            WorkspaceContextMenu.ResumeLayout(false);
            ResumeLayout(false);

        }

        #endregion
    }
}