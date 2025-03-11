using System;
using System.Drawing;
using System.Windows.Forms;
using TheoryOfAutomatons.Automaton;
using TheoryOfAutomatons.Utils.Helpers;

namespace TheoryOfAutomatons
{
    public partial class MainForm : Form
    {
        private float ScaleFactor = 1.0f;
        public Size OriginalSize = new Size(541, 555);
        private Point lastScrollPosition;
        private Point mouseDownPosition;
        private Point _lastScreenPoint;

        public MainForm()
        {
            InitializeComponent();
        }

        internal static AutomatonCreator AutomatonCreator;
        private void Form_Load(object sender, EventArgs e)
        {
            SetStyle
            (
                 ControlStyles.OptimizedDoubleBuffer |
                 ControlStyles.AllPaintingInWmPaint |
                 ControlStyles.UserPaint, true
            );

            AutomatonCreator = new AutomatonCreator
            (
                saveToFileTSB, loadFromFileTSB, clearTSB,
                typeBox,
                inputAlphabet, outputAlphabet,
                addState,
                sequenceTextBox, generateRandomSequence, analyze,
                cirlceDiameterNUD, borderNUD, drawStepDelayNUD, transitionLightPenNUD, transitionBlackPenNUD,
                containerCP, activeBorderCP, inactiveBorderCP, highlightedBorderCP, innerStateCP, transitionLightPenCP, transitionBlackPenCP,
                prohibitIntersectingPaths, developerMode,
                container, this, colorPicker, MainTerminal
            );

            container.MouseMove += Container_MouseMove;
            container.MouseLeave += Container_MouseLeave;
            containerPanel.MouseDown += ContainerPanel_MouseDown;
            containerPanel.MouseMove += ContainerPanel_MouseMove;
            containerPanel.Resize += ContainerPanel_Resize;

            zoomTrackBar.ValueChanged += ZoomTrackBar_ValueChanged;
            clearZoom.Click += ClearZoom_Click;
        }

        private void Container_MouseLeave(object sender, EventArgs e)
        {
            if (labelScreenCoords.Parent.Visible)
            {
                ClearCoordinates();
            }
        }

        private void Container_MouseMove(object sender, MouseEventArgs e)
        {
            if (labelScreenCoords.Parent.Visible)
            {
                UpdateCoordinates(e.Location);
            }
        }

        private void Container_MouseWheel(object sender, MouseEventArgs e)
        {
            if (Control.ModifierKeys == Keys.Control)
            {
                float scaleDelta = e.Delta > 0 ? 1.25f : 0.8f;
                float newScale = Math.Clamp(ScaleFactor * scaleDelta, 0.1f, 10f);
                UpdateZoom(newScale, e.Location);
            }
        }

        private void Container_Paint(object sender, PaintEventArgs e)
        {
            DrawHelper.SetGraphicsParameters(e.Graphics);

            using (var matrix = new System.Drawing.Drawing2D.Matrix())
            {
                matrix.Scale(ScaleFactor, ScaleFactor);
                e.Graphics.Transform = matrix;

                // Ваша логика отрисовки
                using (var pen = new Pen(Color.Black, 1))
                {
                    e.Graphics.DrawRectangle(pen, 0, 0, OriginalSize.Width - 1, OriginalSize.Height - 1);
                }
            }
        }

        private void ZoomTrackBar_ValueChanged(object sender, EventArgs e)
        {
            float newScale = zoomTrackBar.Value / 100f;
            Point? focusPoint = container.PointToClient(Cursor.Position);
            UpdateZoom(newScale, focusPoint);
        }

        private void ClearZoom_Click(object sender, EventArgs e)
        {
            ForceFullCenter();
            zoomTrackBar.Value = 100;
        }

        private void ContainerPanel_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Middle)
            {
                mouseDownPosition = e.Location;
                lastScrollPosition = containerPanel.AutoScrollPosition;
            }
        }

        private void ContainerPanel_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Middle)
            {
                // containerPanel.SuspendLayout();

                var offset = new Point(
                    mouseDownPosition.X - e.X,
                    mouseDownPosition.Y - e.Y
                );

                Point newAutoScrollPos = new Point(
                    Math.Abs(lastScrollPosition.X) + offset.X,
                    Math.Abs(lastScrollPosition.Y) + offset.Y
                );

                if (containerPanel.AutoScrollPosition != newAutoScrollPos)
                    containerPanel.AutoScrollPosition = newAutoScrollPos;

                // containerPanel.ResumeLayout();
            }
        }

        private void ContainerPanel_Resize(object sender, EventArgs e)
        {
            if (!containerPanel.Created) return;
            SmartCenter();
            container.Invalidate();
        }



        private void UpdateCoordinates(Point screenPoint)
        {
            if (screenPoint == _lastScreenPoint) return;
            _lastScreenPoint = screenPoint;

            if (labelScreenCoords.Parent.Visible)
            {
                // Экранные координаты (относительно PictureBox)
                labelScreenCoords.Text = $"Экранные: {_lastScreenPoint.X}, {_lastScreenPoint.Y}";

                // Реальные координаты (с учетом масштаба и прокрутки)
                PointF realPoint = ConvertToRealCoordinates(_lastScreenPoint);
                labelRealCoords.Text = $"Реальные: {realPoint.X:F0}, {realPoint.Y:F0}";
            }
        }

        private void ClearCoordinates()
        {
            labelScreenCoords.Text = "Экранные: -";
            labelRealCoords.Text = "Реальные: -";
        }

        private PointF ConvertToRealCoordinates(Point screenPoint)
        {
            return new PointF(
                (screenPoint.X + containerPanel.HorizontalScroll.Value) / ScaleFactor,
                (screenPoint.Y + containerPanel.VerticalScroll.Value) / ScaleFactor
            );
        }

        private void UpdateZoom(float newScale, Point? focusPoint = null)
        {
            var oldScale = ScaleFactor;
            ScaleFactor = Math.Clamp(newScale, 0.1f, 10f);

            // Рассчитываем новую позицию
            var newSize = new Size(
                (int)(OriginalSize.Width * ScaleFactor),
                (int)(OriginalSize.Height * ScaleFactor)
            );

            // Сохраняем относительную позицию
            if (focusPoint.HasValue && container.ClientRectangle.Contains(focusPoint.Value))
            {
                float ratioX = (focusPoint.Value.X + containerPanel.HorizontalScroll.Value) / (float)container.Width;
                float ratioY = (focusPoint.Value.Y + containerPanel.VerticalScroll.Value) / (float)container.Height;

                container.Size = newSize;

                var newScrollX = (int)(newSize.Width * ratioX - containerPanel.ClientSize.Width * ratioX);
                var newScrollY = (int)(newSize.Height * ratioY - containerPanel.ClientSize.Height * ratioY);

                containerPanel.AutoScrollPosition = new Point(newScrollX, newScrollY);
            }
            else
            {
                container.Size = newSize;
                SmartCenter();
            }

            // Синхронизация элементов управления
            zoomTrackBar.Value = (int)(ScaleFactor * 100);
            container.Invalidate();
        }

        private void SmartCenter(Point? focusPoint = null)
        {
            if (focusPoint.HasValue && container.ClientRectangle.Contains(focusPoint.Value))
            {
                // Центровка относительно точки фокуса
                float ratioX = (float)focusPoint.Value.X / container.Width;
                float ratioY = (float)focusPoint.Value.Y / container.Height;

                var newX = (int)(container.Width * ratioX - containerPanel.ClientSize.Width * ratioX);
                var newY = (int)(container.Height * ratioY - containerPanel.ClientSize.Height * ratioY);

                containerPanel.AutoScrollPosition = new Point(newX, newY);
            }
            else
            {
                // Центровка по середине
                containerPanel.AutoScrollPosition = new Point(
                    Math.Max(0, (container.Width - containerPanel.ClientSize.Width) / 2),
                    Math.Max(0, (container.Height - containerPanel.ClientSize.Height) / 2)
                );
            }
        }

        private void ForceFullCenter()
        {
            ScaleFactor = 1.0f;
            container.Size = OriginalSize;
            SmartCenter();
            container.Invalidate();
        }




        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            if (WindowState != FormWindowState.Minimized)
            {
                SmartCenter();
                container.Invalidate();
            }
        }
        //protected override CreateParams CreateParams
        //{
        //    get
        //    {
        //        var cp = base.CreateParams;
        //        cp.ExStyle |= 0x02000000;
        //        return cp;
        //    }
        //}
    }
}
