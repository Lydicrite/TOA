using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
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
        private VScrollBar _vScrollBar;
        private HScrollBar _hScrollBar;

        private const float DEFAULT_MIN_SCALE = 0.05f;
        private const float DEFAULT_MAX_SCALE = 5.0f;

        private float _scaleFactor = 1.0f;
        private float _minScale = DEFAULT_MIN_SCALE;
        private float _maxScale = DEFAULT_MAX_SCALE;
        private Bitmap _workspaceBitmap;
        private Graphics _workspaceGraphics;
        private Matrix _transformCache;

        private int _zoomStepPercentage = 5;
        private bool _isDragging;
        private Point _lastDragPosition;

        private float _dpiScaleX = 1.0f;
        private float _dpiScaleY = 1.0f;
        private ToolTip _zoomToolTip = new ToolTip();



        #region Properties

        [Category("Layout"), Description("Current zoom scale factor")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public float ScaleFactor
        {
            get => _scaleFactor;
            set
            {
                var newScale = Math.Clamp(value, _minScale, _maxScale);
                if (Math.Abs(newScale - _scaleFactor) < 0.001f) return;

                _scaleFactor = newScale;
                UpdateZoomTooltip();
                UpdateTransformCache();
                UpdateWorkspace();
            }
        }

        [Category("Layout"), Description("Minimum allowed zoom scale")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public float MinScale
        {
            get => _minScale;
            set => _minScale = Math.Clamp(value, 0.01f, _maxScale);
        }

        [Category("Layout"), Description("Maximum allowed zoom scale")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public float MaxScale
        {
            get => _maxScale;
            set => _maxScale = Math.Max(value, _minScale);
        }

        [Category("Appearance"), Description("Enable grid display")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public bool ShowGrid { get; set; }

        [Category("Appearance"), Description("Grid cell size in pixels")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public int GridSize { get; set; } = 20;

        [Category("Appearance"), Description("Grid color")]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public Color GridColor { get; set; } = Color.LightGray;

        [Browsable(false)]
        public Graphics WorkspaceGraphics => _workspaceGraphics;

        #endregion



        public Workspace()
        {
            InitializeComponent();

            this.AutoScroll = false;

            SetStyle(ControlStyles.OptimizedDoubleBuffer |
                    ControlStyles.AllPaintingInWmPaint |
                    ControlStyles.UserPaint, true);

            SetupScrollBars();
            InitializeGraphics();
            SubscribeEvents();

            DoubleBuffered = true;
            ResizeRedraw = true;
        }





        #region Initialization

        private void SetupScrollBars()
        {
            _vScrollBar = new VScrollBar { Dock = DockStyle.Right };
            _hScrollBar = new HScrollBar { Dock = DockStyle.Bottom };

            Controls.Add(_vScrollBar);
            Controls.Add(_hScrollBar);

            _vScrollBar.Scroll += (_, _) => Invalidate();
            _hScrollBar.Scroll += (_, _) => Invalidate();
        }

        private void InitializeGraphics()
        {
            CreateWorkspaceBitmap();
            _workspaceGraphics.SmoothingMode = SmoothingMode.AntiAlias;
            _workspaceGraphics.TextRenderingHint = TextRenderingHint.AntiAlias;
            _workspaceGraphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
            UpdateTransformCache();
        }

        private void SubscribeEvents()
        {
            MouseWheel += OnMouseWheel;
            MouseDown += OnMouseDown;
            MouseUp += OnMouseUp;
            MouseMove += OnMouseMove;
            Resize += OnResize;
        }

        #endregion





        #region Event Handlers

        private void OnMouseWheel(object sender, MouseEventArgs e)
        {
            if (ModifierKeys != Keys.Control) return;

            var zoomFactor = 1 + _zoomStepPercentage / 100f * Math.Sign(e.Delta);
            ScaleFactor *= zoomFactor;
        }

        private void OnMouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && ModifierKeys == Keys.Control)
            {
                _isDragging = true;
                _lastDragPosition = e.Location;
                Cursor = Cursors.SizeAll;
            }
        }

        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            if (!_isDragging) return;

            var dx = e.X - _lastDragPosition.X;
            var dy = e.Y - _lastDragPosition.Y;

            SmoothScroll(_hScrollBar, Math.Clamp(_hScrollBar.Value - dx, 0, _hScrollBar.Maximum));
            SmoothScroll(_vScrollBar, Math.Clamp(_vScrollBar.Value - dy, 0, _vScrollBar.Maximum));

            _lastDragPosition = e.Location;
        }

        private void OnMouseUp(object sender, MouseEventArgs e) => ResetDragState();

        private void OnResize(object sender, EventArgs e)
        {
            CreateWorkspaceBitmap();
            UpdateScrollBars();
            UpdateTransformCache();
            Invalidate();
        }

        #endregion





        #region Drawing Logic

        public Point TransformScreenToVirtual(Point screenPoint)
        {
            // Учет прокрутки и масштаба
            return new Point(
                (int)((screenPoint.X + _hScrollBar.Value) / _scaleFactor),
                (int)((screenPoint.Y + _vScrollBar.Value) / _scaleFactor)
            );
        }

        public Point TransformVirtualToScreen(Point virtualPoint)
        {
            return new Point(
                (int)(virtualPoint.X * _scaleFactor - _hScrollBar.Value),
                (int)(virtualPoint.Y * _scaleFactor - _vScrollBar.Value)
            );
        }

        protected override void OnCreateControl()
        {
            base.OnCreateControl();
            using var g = CreateGraphics();
            _dpiScaleX = g.DpiX / 96.0f;
            _dpiScaleY = g.DpiY / 96.0f;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            if (_workspaceBitmap == null) return;

            var g = e.Graphics;

            // Сохраняем оригинальную трансформацию
            var originalTransform = g.Transform;
            g.Transform = _transformCache;
            g.InterpolationMode = InterpolationMode.NearestNeighbor;

            // Отрисовка основного содержимого
            var destRect = new RectangleF(
                -_hScrollBar.Value / _scaleFactor,
                -_vScrollBar.Value / _scaleFactor,
                _workspaceBitmap.Width,
                _workspaceBitmap.Height);
            g.DrawImage(_workspaceBitmap, destRect);

            // Восстанавливаем трансформацию
            g.Transform = originalTransform;

            // Отрисовка сетки (без трансформации)
            if (ShowGrid)
            {
                using var gridPen = new Pen(GridColor, 0.5f);
                float scaledGridSize = GridSize * _scaleFactor;

                // Учет прокрутки и масштаба для позиционирования сетки
                float startX = _hScrollBar.Value % scaledGridSize;
                float startY = _vScrollBar.Value % scaledGridSize;

                for (float x = startX; x < ClientSize.Width; x += scaledGridSize)
                    g.DrawLine(gridPen, x, 0, x, ClientSize.Height);

                for (float y = startY; y < ClientSize.Height; y += scaledGridSize)
                    g.DrawLine(gridPen, 0, y, ClientSize.Width, y);
            }
        }

        #endregion





        #region Improved Features

        public void InvalidateRegion(RectangleF region)
        {
            var scaledRegion = new Rectangle(
                (int)((region.X * _scaleFactor) - _hScrollBar.Value),
                (int)((region.Y * _scaleFactor) - _vScrollBar.Value),
                (int)(region.Width * _scaleFactor),
                (int)(region.Height * _scaleFactor));

            Invalidate(scaledRegion);
        }

        private void UpdateTransformCache()
        {
            _transformCache?.Dispose();
            _transformCache = new Matrix();
            _transformCache.Translate(-_hScrollBar.Value, -_vScrollBar.Value);
            _transformCache.Scale(_scaleFactor, _scaleFactor);
        }

        private void UpdateZoomTooltip()
        {
            _zoomToolTip.SetToolTip(this, $"Zoom: {_scaleFactor * 100:0}%");
        }

        private void SmoothScroll(ScrollBar scrollBar, int targetValue)
        {
            if (scrollBar.Value == targetValue) return;

            Timer animationTimer = new Timer { Interval = 10 };
            int startValue = scrollBar.Value;
            int steps = 15;
            int currentStep = 0;

            animationTimer.Tick += (s, e) =>
            {
                if (currentStep >= steps)
                {
                    animationTimer.Stop();
                    animationTimer.Dispose();
                    return;
                }

                float t = currentStep++ / (float)steps;
                scrollBar.Value = (int)(startValue + (targetValue - startValue) * EaseOutQuad(t));
                Invalidate();
            };

            animationTimer.Start();
        }

        private float EaseOutQuad(float t) => t * (2 - t);

        #endregion





        #region Helpers

        private void CreateWorkspaceBitmap()
        {
            _workspaceGraphics?.Dispose();
            _workspaceBitmap?.Dispose();

            int width = (int)(Width * _dpiScaleX);
            int height = (int)(Height * _dpiScaleY);

            _workspaceBitmap = new Bitmap(Math.Max(1, width), Math.Max(1, height));
            _workspaceGraphics = Graphics.FromImage(_workspaceBitmap);
            _workspaceGraphics.Clear(BackColor);
        }

        private void UpdateScrollBars()
        {
            int contentWidth = (int)(_workspaceBitmap.Width * _scaleFactor);
            int contentHeight = (int)(_workspaceBitmap.Height * _scaleFactor);

            UpdateScrollBar(_hScrollBar, contentWidth, ClientSize.Width);
            UpdateScrollBar(_vScrollBar, contentHeight, ClientSize.Height);
        }

        private static void UpdateScrollBar(ScrollBar scrollBar, int contentSize, int clientSize)
        {
            scrollBar.Enabled = contentSize > clientSize;
            scrollBar.Maximum = Math.Max(0, contentSize - 1);
            scrollBar.LargeChange = Math.Max(1, clientSize);
        }

        private void UpdateWorkspace()
        {
            UpdateScrollBars();
            UpdateTransformCache();
            Invalidate();
        }

        private void ResetDragState()
        {
            _isDragging = false;
            Cursor = Cursors.Default;
        }

        #endregion





        #region Public Methods

        public void ResetZoom() => ScaleFactor = 1.0f;

        public void ClearWorkspace(Color color)
        {
            _workspaceGraphics?.Clear(color);
            Invalidate();
        }

        #endregion

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _workspaceGraphics?.Dispose();
                _workspaceBitmap?.Dispose();
                _transformCache?.Dispose();
                _zoomToolTip?.Dispose();
            }
            base.Dispose(disposing);
        }



        #region Код, автоматически созданный конструктором компонентов

        private System.ComponentModel.IContainer components = null;

        private void InitializeComponent()
        {
            _vScrollBar = new VScrollBar();
            _hScrollBar = new HScrollBar();
            SuspendLayout();
            // 
            // _vScrollBar
            // 
            _vScrollBar.Dock = DockStyle.Right;
            _vScrollBar.Location = new Point(383, 0);
            _vScrollBar.Name = "_vScrollBar";
            _vScrollBar.Size = new Size(17, 283);
            _vScrollBar.TabIndex = 0;
            // 
            // _hScrollBar
            // 
            _hScrollBar.Dock = DockStyle.Bottom;
            _hScrollBar.Location = new Point(0, 283);
            _hScrollBar.Name = "_hScrollBar";
            _hScrollBar.Size = new Size(400, 17);
            _hScrollBar.TabIndex = 1;
            // 
            // Workspace
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(96, 96, 96);
            Margin = new Padding(4, 3, 4, 3);
            Name = "Workspace";
            Size = new Size(400, 300);
            ResumeLayout(false);

        }

        #endregion
    }
}
