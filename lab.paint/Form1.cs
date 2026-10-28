using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace lab.paint
{
    public partial class Form1 : Form
    {
        private readonly Bitmap canvas = new Bitmap(1000, 600);
        private readonly PictureBox drawingArea = new PictureBox();
        private readonly Button pencilButton = new Button();
        private readonly Button eraserButton = new Button();
        private readonly NumericUpDown brushSize = new NumericUpDown();

        private bool drawing;
        private bool erasing;
        private Point previousPoint;
        private Color drawingColor = Color.Black;

        public Form1()
        {
            InitializeComponent();

            Text = "Миний Paint";
            ClientSize = new Size(1000, 660);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            StartPosition = FormStartPosition.CenterScreen;

            FlowLayoutPanel toolbar = new FlowLayoutPanel
            {
                Dock = DockStyle.Top,
                Height = 60,
                Padding = new Padding(8),
                BackColor = Color.Gainsboro
            };

            pencilButton.Text = "Харандаа";
            pencilButton.Size = new Size(110, 35);
            pencilButton.Click += (sender, e) =>
            {
                erasing = false;
                UpdateTool();
            };

            eraserButton.Text = "Баллуур";
            eraserButton.Size = new Size(110, 35);
            eraserButton.Click += (sender, e) =>
            {
                erasing = true;
                UpdateTool();
            };

            Button colorButton = new Button
            {
                Text = "Өнгө сонгох",
                Size = new Size(120, 35)
            };

            colorButton.Click += (sender, e) =>
            {
                using (ColorDialog dialog = new ColorDialog())
                {
                    dialog.Color = drawingColor;

                    if (dialog.ShowDialog() == DialogResult.OK)
                    {
                        drawingColor = dialog.Color;
                        erasing = false;
                        UpdateTool();
                    }
                }
            };

            Label sizeLabel = new Label
            {
                Text = "Зузаан:",
                AutoSize = true,
                Margin = new Padding(10, 10, 3, 0)
            };

            brushSize.Minimum = 1;
            brushSize.Maximum = 60;
            brushSize.Value = 5;
            brushSize.Width = 60;
            brushSize.Margin = new Padding(3, 7, 10, 0);

            Button clearButton = new Button
            {
                Text = "Бүгдийг арилгах",
                Size = new Size(150, 35)
            };

            clearButton.Click += (sender, e) =>
            {
                using (Graphics g = Graphics.FromImage(canvas))
                {
                    g.Clear(Color.White);
                }

                drawingArea.Invalidate();
            };

            toolbar.Controls.AddRange(new Control[]
            {
                pencilButton,
                eraserButton,
                colorButton,
                sizeLabel,
                brushSize,
                clearButton
            });

            using (Graphics g = Graphics.FromImage(canvas))
            {
                g.Clear(Color.White);
            }

            drawingArea.Location = new Point(0, 60);
            drawingArea.Size = new Size(1000, 600);
            drawingArea.BackColor = Color.White;
            drawingArea.Image = canvas;
            drawingArea.Cursor = Cursors.Cross;

            drawingArea.MouseDown += DrawingArea_MouseDown;
            drawingArea.MouseMove += DrawingArea_MouseMove;
            drawingArea.MouseUp += DrawingArea_MouseUp;
            drawingArea.MouseCaptureChanged += (sender, e) =>
            {
                if (!drawingArea.Capture)
                    drawing = false;
            };

            Controls.Add(drawingArea);
            Controls.Add(toolbar);

            FormClosed += (sender, e) =>
            {
                drawingArea.Image = null;
                canvas.Dispose();
            };

            UpdateTool();
        }

        private void UpdateTool()
        {
            pencilButton.BackColor =
                erasing ? SystemColors.Control : Color.LightSkyBlue;

            eraserButton.BackColor =
                erasing ? Color.LightSkyBlue : SystemColors.Control;
        }

        private void DrawingArea_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left)
                return;

            drawing = true;
            previousPoint = e.Location;
            drawingArea.Capture = true;

            float size = (float)brushSize.Value;
            Color color = erasing ? Color.White : drawingColor;

            using (Graphics g = Graphics.FromImage(canvas))
            using (SolidBrush brush = new SolidBrush(color))
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.FillEllipse(
                    brush,
                    e.X - size / 2,
                    e.Y - size / 2,
                    size,
                    size
                );
            }

            drawingArea.Invalidate();
        }

        private void DrawingArea_MouseMove(object sender, MouseEventArgs e)
        {
            if (!drawing)
                return;

            DrawLine(e.Location);
        }

        private void DrawingArea_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left || !drawing)
                return;

            DrawLine(e.Location);
            drawing = false;
            drawingArea.Capture = false;
        }

        private void DrawLine(Point currentPoint)
        {
            Color color = erasing ? Color.White : drawingColor;

            using (Graphics g = Graphics.FromImage(canvas))
            using (Pen pen = new Pen(color, (float)brushSize.Value))
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;
                pen.StartCap = LineCap.Round;
                pen.EndCap = LineCap.Round;

                g.DrawLine(pen, previousPoint, currentPoint);
            }

            previousPoint = currentPoint;
            drawingArea.Invalidate();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
        }
    }
}