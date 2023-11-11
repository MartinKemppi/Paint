using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Drawing.Imaging;

namespace Paint
{
    public partial class Paint : Form
    {
        private MenuStrip Menu;
        private Panel pan;
        private PictureBox pb;
        private TrackBar tkb;
        private List<Image> History;
        private int historyCounter;
        private Bitmap drawingSurface;
        private bool drawing = false;
        private bool erasing = false;
        private Point lastPoint;        
        private Color historyColor;
        public Pen currentPen;
        private Label label_XY;
        private Button rectangleButton, ellipseButton, triangleButton, btnPen;
        private DrawingMode currentDrawingMode;
        private Rectangle lastRectangle;
        private Point[] lastTrianglePoints;

        private ToolStripMenuItem dotMenuItem;
        private ToolStripMenuItem solidMenuItem;
        private ToolStripMenuItem dashdotdotMenuItem;

        private enum DrawingMode
        {
            Pen,
            Line,
            Rectangle,
            Ellipse,
            Triangle
        }

        public Paint()
        {
            //FORM
            this.Height = 800;
            this.Width = 800;
            
            //MENU FAIL
            Menu = new MenuStrip();
            ToolStripMenuItem fileMenuItem = new ToolStripMenuItem("Fail");
            ToolStripMenuItem newMenuItem = new ToolStripMenuItem("Uus");
            ToolStripMenuItem openMenuItem = new ToolStripMenuItem("Ava");
            ToolStripMenuItem saveMenuItem = new ToolStripMenuItem("Salvesta");
            ToolStripMenuItem exitMenuItem = new ToolStripMenuItem("Valju");

            fileMenuItem.DropDownItems.Add(newMenuItem);
            fileMenuItem.DropDownItems.Add(openMenuItem);
            fileMenuItem.DropDownItems.Add(saveMenuItem);
            fileMenuItem.DropDownItems.Add(exitMenuItem);

            newMenuItem.ShortcutKeys = Keys.Control | Keys.N;
            openMenuItem.ShortcutKeys = Keys.F3;
            saveMenuItem.ShortcutKeys = Keys.F2;
            exitMenuItem.ShortcutKeys = Keys.Alt | Keys.X;

            newMenuItem.Click += NewMenuItem_Click;
            openMenuItem.Click += OpenMenuItem_Click;
            saveMenuItem.Click += SaveMenuItem_Click;
            exitMenuItem.Click += ExitMenuItem_Click;

            //MENU EDIT
            ToolStripMenuItem editMenuItem = new ToolStripMenuItem("Redigeeri");
            ToolStripMenuItem undoMenuItem = new ToolStripMenuItem("Tagasi");
            ToolStripMenuItem redoMenuItem = new ToolStripMenuItem("Edasi");
            ToolStripMenuItem penMenuItem = new ToolStripMenuItem("Pliats");
            ToolStripMenuItem styleMenuItem = new ToolStripMenuItem("Stiil");
            ToolStripMenuItem colorMenuItem = new ToolStripMenuItem("Varv");

            solidMenuItem = new ToolStripMenuItem("Tugev");
            dotMenuItem = new ToolStripMenuItem("Punktidega");
            dashdotdotMenuItem = new ToolStripMenuItem("KriipsTappTapp");

            editMenuItem.DropDownItems.Add(undoMenuItem);
            editMenuItem.DropDownItems.Add(redoMenuItem);
            editMenuItem.DropDownItems.Add(penMenuItem);
            penMenuItem.DropDownItems.Add(styleMenuItem);
            penMenuItem.DropDownItems.Add(colorMenuItem);

            styleMenuItem.DropDownItems.Add(solidMenuItem);
            styleMenuItem.DropDownItems.Add(dotMenuItem);
            styleMenuItem.DropDownItems.Add(dashdotdotMenuItem);

            dotMenuItem.Click += SetDotStyle;
            solidMenuItem.Click += SetSolidStyle;
            dashdotdotMenuItem.Click += SetDashDotDotStyle;

            undoMenuItem.ShortcutKeys = Keys.Control | Keys.Z;
            redoMenuItem.ShortcutKeys = Keys.Control | Keys.Y;

            undoMenuItem.Click += undoToolStripMenuItem_Click;
            redoMenuItem.Click += redoToolStripMenuItem_Click;

            colorMenuItem.Click += ColorMenuItem_Click;

            //MENU HELP
            ToolStripMenuItem helpMenuItem = new ToolStripMenuItem("Abi");
            ToolStripMenuItem aboutMenuItem = new ToolStripMenuItem("Sellest");

            helpMenuItem.DropDownItems.Add(aboutMenuItem);

            aboutMenuItem.ShortcutKeys = Keys.F1;

            aboutMenuItem.Click += AboutMenuItem_Click;

            Menu.Items.Add(fileMenuItem);
            Menu.Items.Add(editMenuItem);
            Menu.Items.Add(helpMenuItem);
          
            //HISTORY
            History = new List<Image>();
            historyCounter = -1;

            //PICTUREBOX
            pb = new PictureBox();
            pb.Location = new Point(100,50);
            pb.Size = new Size(650, 650);
            pb.SizeMode = PictureBoxSizeMode.Zoom;
            pb.BorderStyle = BorderStyle.Fixed3D;
            pb.BackColor = Color.White;

            // TRACKBAR
            tkb = new TrackBar();
            tkb.Height = 50;
            tkb.Width = 200;
            tkb.Minimum = 1;
            tkb.Maximum = 100;
            tkb.Value = 40;
            
            tkb.Scroll += TrackBar_Scroll;

            //PANEL
            pan = new Panel();
            pan.Location = new Point(pb.Location.X, pb.Location.Y + pb.Height + 5);
            pan.Size = new Size(pb.Width,50);
            pan.Visible = true;
            
            //LABEL
            label_XY = new Label();
            label_XY.AutoSize = true;
            label_XY.Location = new Point(10, pb.Location.Y + pb.Height + 10);
            
            //PICTUREBOX MOUSE E
            pb.MouseDown += Pb_MouseDown;
            pb.MouseMove += Pb_MouseMove;
            pb.MouseUp += Pb_MouseUp;

            //KUSTUKUMM
            pb.MouseClick += Pb_MouseClick;

            currentPen = new Pen(Color.Black, tkb.Value);
            currentPen.StartCap = LineCap.Round;
            currentPen.EndCap = LineCap.Round;

            drawingSurface = new Bitmap(pb.Width, pb.Height);
            pb.Image = drawingSurface;

            //BUTTON RISTKYLIK
            rectangleButton = new Button();
            rectangleButton.Text = "Ristkülik";
            rectangleButton.Location = new Point(10, pb.Location.Y);
            
            rectangleButton.Click += RectangleButton_Click;

            //BUTTON ELLIPS
            ellipseButton = new Button();
            ellipseButton.Text = "Ellips";
            ellipseButton.Location = new Point(10, pb.Location.Y + 30);
            
            ellipseButton.Click += EllipseButton_Click;

            //BUTTON KOLMNURK
            triangleButton = new Button();
            triangleButton.Text = "Kolmnurk";
            triangleButton.Location = new Point(10, pb.Location.Y + 60);
            lastTrianglePoints = new Point[3];

            triangleButton.Click += TriangleButton_Click;

            //BUTTON PLIATS
            btnPen = new Button();
            btnPen.Text = "Pliats";
            btnPen.Location = new Point(10, pb.Location.Y + 90);  

            btnPen.Click += BtnPen_Click;

            //FORMILE LISATUD
            this.Controls.Add(Menu);
            this.Controls.Add(pb);
            this.Controls.Add(pan);
            this.Controls.Add(tkb);
            this.Controls.Add(label_XY);
            this.Controls.Add(rectangleButton);
            this.Controls.Add(ellipseButton);
            this.Controls.Add(triangleButton);
            this.Controls.Add(btnPen);

            lastTrianglePoints = new Point[3];

            //PANNELI LISATUD
            pan.Controls.Add(tkb);            

            //TINGIMUS
            if (pb.Image == null)
            {
                MessageBox.Show("Looge uus fail");
                return;
            }
        }
        private void OpenMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.gif;*.bmp";

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                pb.Image = new Bitmap(openFileDialog.FileName);

                History.Clear();
                History.Add(new Bitmap(pb.Image));
                historyCounter = 0;
            }
        }

        private void SaveMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "PNG Image|*.png|JPEG Image|*.jpg;*.jpeg|BMP Image|*.bmp|GIF Image|*.gif";

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                string extension = Path.GetExtension(saveFileDialog.FileName);

                switch (extension.ToLower())
                {
                    case ".png":
                        pb.Image.Save(saveFileDialog.FileName, ImageFormat.Png);
                        break;
                    case ".jpg":
                    case ".jpeg":
                        pb.Image.Save(saveFileDialog.FileName, ImageFormat.Jpeg);
                        break;
                    case ".bmp":
                        pb.Image.Save(saveFileDialog.FileName, ImageFormat.Bmp);
                        break;
                    case ".gif":
                        pb.Image.Save(saveFileDialog.FileName, ImageFormat.Gif);
                        break;
                    default:
                        MessageBox.Show("Vale faili formaat");
                        break;
                }
            }
        }        
        private void SetDrawingMode(DrawingMode mode)
        {
            currentDrawingMode = mode;
        }
        private void RectangleButton_Click(object sender, EventArgs e)
        {
            SetDrawingMode(DrawingMode.Rectangle);
        }
        private void EllipseButton_Click(object sender, EventArgs e)
        {
            SetDrawingMode(DrawingMode.Ellipse);
        }
        private void TriangleButton_Click(object sender, EventArgs e)
        {
            currentDrawingMode = DrawingMode.Triangle;
        }
        private void DrawRectangle(Graphics g, Pen pen, Point startPoint, Point endPoint)
        {
            int x = Math.Min(startPoint.X, endPoint.X);
            int y = Math.Min(startPoint.Y, endPoint.Y);
            int width = Math.Abs(startPoint.X - endPoint.X);
            int height = Math.Abs(startPoint.Y - endPoint.Y);

            g.DrawRectangle(pen, x, y, width, height);
        }

        private void DrawEllipse(Graphics g, Pen pen, Point startPoint, Point endPoint)
        {
            int x = Math.Min(startPoint.X, endPoint.X);
            int y = Math.Min(startPoint.Y, endPoint.Y);
            int width = Math.Abs(startPoint.X - endPoint.X);
            int height = Math.Abs(startPoint.Y - endPoint.Y);

            g.DrawEllipse(pen, x, y, width, height);
        }

        private void DrawTriangle(Graphics g, Pen pen, Point point1, Point point2, Point point3)
        {
            Point[] trianglePoints = { point1, point2, point3 };
            g.DrawPolygon(pen, trianglePoints);
        }
        private void ExitMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
        private void AboutMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("See programm on moeldud joonistamiseks.\nSiin saate kasutada edasi/tagasi funktsiooni.\nVarvid varvimiseks");
        }
        private void Pb_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                drawing = true;
                erasing = false;
                lastPoint = e.Location;

                if (currentDrawingMode == DrawingMode.Rectangle)
                {
                    lastRectangle = new Rectangle(lastPoint, new Size(0, 0));
                }
                else if (currentDrawingMode == DrawingMode.Ellipse)
                {
                    lastRectangle = new Rectangle(lastPoint, new Size(0, 0));
                }
            }
            else if (e.Button == MouseButtons.Right)
            {
                erasing = true;
                drawing = false;
                lastPoint = e.Location;
            }
        }
        private void Pb_MouseMove(object sender, MouseEventArgs e)
        {
            UpdateMouseCoordinates(e.X, e.Y);

            if (drawing || erasing)
            {
                Color drawColor;

                switch (currentDrawingMode)
                {
                    case DrawingMode.Pen:
                        drawColor = (e.Button == MouseButtons.Left) ? currentPen.Color : Color.White;
                        break;
                    case DrawingMode.Rectangle:
                    case DrawingMode.Ellipse:
                    case DrawingMode.Triangle:
                        drawColor = currentPen.Color;
                        break;
                    default:
                        drawColor = Color.Black;
                        break;
                }

                using (Graphics g = Graphics.FromImage(drawingSurface))
                {
                    using (Pen pen = new Pen(drawColor, currentPen.Width))
                    {
                        pen.StartCap = LineCap.Round;
                        pen.EndCap = LineCap.Round;
                        pen.DashStyle = currentPen.DashStyle;

                        if (drawing || erasing)
                        {
                            if (currentDrawingMode == DrawingMode.Pen)
                            {
                                g.DrawLine(pen, lastPoint, e.Location);
                            }
                            else if (currentDrawingMode == DrawingMode.Rectangle)
                            {
                                lastRectangle.Width = e.X - lastPoint.X;
                                lastRectangle.Height = e.Y - lastPoint.Y;

                                g.DrawRectangle(pen, lastRectangle);
                            }
                            else if (currentDrawingMode == DrawingMode.Ellipse)
                            {
                                lastRectangle.Width = e.X - lastPoint.X;
                                lastRectangle.Height = e.Y - lastPoint.Y;

                                g.DrawEllipse(pen, lastRectangle);
                            }
                            else if (currentDrawingMode == DrawingMode.Triangle)
                            {
                                Point[] trianglePoints = new Point[]
                                {
                            new Point(lastPoint.X + (e.X - lastPoint.X) / 2, lastPoint.Y),
                            new Point(e.X, e.Y),
                            new Point(lastPoint.X, e.Y)
                                };

                                g.DrawPolygon(pen, trianglePoints);
                            }
                        }
                    }
                }

                lastPoint = e.Location;
                pb.Invalidate();
            }
        }
        private void BtnPen_Click(object sender, EventArgs e)
        {
            currentDrawingMode = DrawingMode.Pen;
        }
        private void Pb_MouseUp(object sender, MouseEventArgs e)
        {
            if (drawing || erasing)
            {
                drawing = false;
                erasing = false;

                SaveToHistory();
            }
        }
        private void Pb_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                erasing = true;
            }
            else if (e.Button == MouseButtons.Left)
            {
                erasing = false;
                drawing = true;
                lastPoint = e.Location;
            }
        }
        private void SaveToHistory()
        {
            Bitmap currentImage = new Bitmap(pb.Image);
            History.RemoveRange(historyCounter + 1, History.Count - historyCounter - 1);
            History.Add(currentImage);
            if (historyCounter + 1 < 10) historyCounter++;
            if (History.Count > 10) History.RemoveAt(0);
        }
        private void SetPenStyle(DashStyle style)
        {
            dotMenuItem.Checked = (style == DashStyle.Dot);
            solidMenuItem.Checked = (style == DashStyle.Solid);
            dashdotdotMenuItem.Checked = (style == DashStyle.DashDotDot);

            currentPen = new Pen((erasing) ? pb.BackColor : currentPen.Color, tkb.Value);
            currentPen.DashStyle = style;

            if (pb.Image != null)
            {
                using (Graphics g = Graphics.FromImage(pb.Image))
                {
                    g.SmoothingMode = SmoothingMode.AntiAlias;
                    g.DrawLine(currentPen, lastPoint, lastPoint);
                }
                pb.Invalidate();
                SaveToHistory();
            }
        }
        private void SetDotStyle(object sender, EventArgs e)
        {
            SetPenStyle(DashStyle.Dot);
        }

        private void SetSolidStyle(object sender, EventArgs e)
        {
            SetPenStyle(DashStyle.Solid);
        }

        private void SetDashDotDotStyle(object sender, EventArgs e)
        {
            SetPenStyle(DashStyle.DashDotDot);
        }

        private void TrackBar_Scroll(object sender, EventArgs e)
        {
            currentPen.Width = tkb.Value;
        }
        private void NewMenuItem_Click(object sender, EventArgs e)
        {
            drawingSurface = new Bitmap(pb.Width, pb.Height);
            using (Graphics g = Graphics.FromImage(drawingSurface))
            {
                g.Clear(Color.White);
            }

            pb.Image = drawingSurface;

            History.Clear();
            historyCounter = 0;

            History.Add(new Bitmap(pb.Image));
        }
        private void picDrawingSurface_MouseUp(object sender, MouseEventArgs e)
        {            
            SaveToHistory();

            drawing = false;
        }
        private void UpdateMouseCoordinates(int x, int y)
        {
            label_XY.Text = $"X: {x}, Y: {y}";
        }
        private void undoToolStripMenuItem_Click(Object sender, EventArgs e)
        {
            if (History.Count > 0 && historyCounter > 0)
            {
                pb.Image = new Bitmap(History[--historyCounter]);
            }
            else
            {
                MessageBox.Show("Ajalugu on tyhi");
            }
        }
        private void redoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (History.Count > 0 && historyCounter < History.Count - 1)
            {
                pb.Image = new Bitmap(History[++historyCounter]);
            }
            else
            {
                MessageBox.Show("Ajalugu on tyhi");
            }
        }
        private void ColorMenuItem_Click(object sender, EventArgs e)
        {
            using (ColorDialog colorDialog = new ColorDialog())
            {
                if (colorDialog.ShowDialog() == DialogResult.OK)
                {
                    currentPen.Color = colorDialog.Color;
                }
            }
        }
    }
}