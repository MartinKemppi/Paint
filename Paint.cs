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
        private Label label_XY, label_KL, label_Size;
        private Button rectangleButton, ellipseButton, triangleButton, btnPen, btnColor, btnSize;
        private DrawingMode currentDrawingMode = DrawingMode.Pen;
        private Rectangle lastRectangle;
        private Point[] lastTrianglePoints;
        private ToolStripMenuItem dotMenuItem;
        private ToolStripMenuItem solidMenuItem;
        private ToolStripMenuItem dashdotdotMenuItem;
        private enum DrawingMode
        {
            Pen,
            Rectangle,
            Ellipse,
            Triangle
        }
        public Paint()
        {
            //FORM
            this.Height = 800;
            this.Width = 800;
            this.Text = "Paint";
            
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
            
            //MENU HELP
            ToolStripMenuItem helpMenuItem = new ToolStripMenuItem("Abi");
            ToolStripMenuItem aboutMenuItem = new ToolStripMenuItem("Sellest");

            helpMenuItem.DropDownItems.Add(aboutMenuItem);                        

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

            drawingSurface = new Bitmap(pb.Width, pb.Height);
            pb.Image = drawingSurface;

            // TRACKBAR
            tkb = new TrackBar();
            tkb.Height = 50;
            tkb.Width = 200;
            tkb.Minimum = 1;
            tkb.Maximum = 100;
            tkb.Value = 40;                        

            //PANEL
            pan = new Panel();
            pan.Location = new Point(pb.Location.X, pb.Location.Y + pb.Height + 5);
            pan.Size = new Size(pb.Width,100);
            pan.Visible = true;
            
            //LABEL XY
            label_XY = new Label();
            label_XY.AutoSize = true;
            label_XY.Location = new Point(10, pb.Location.Y + pb.Height);

            //LABEL KL
            label_KL = new Label();
            label_KL.AutoSize = true;
            label_KL.Location = new Point(10, label_XY.Location.Y + 20);
            UpdateSizeLabel();

            //LABEL SUURUS e SIZE
            label_Size = new Label();
            label_Size.AutoSize = true;
            label_Size.Location = new Point(10, label_KL.Location.Y + 20);
            label_Size.Text = tkb.Value.ToString()+" px";

            //PEN
            currentPen = new Pen(Color.Black, tkb.Value);
            currentPen.StartCap = LineCap.Round;
            currentPen.EndCap = LineCap.Round;            
            
            //BUTTON RISTKYLIK
            rectangleButton = new Button();
            rectangleButton.Width = 40;
            rectangleButton.Height = 40;
            rectangleButton.Image = Image.FromFile("rectangle_icon.png");
            rectangleButton.Location = new Point(10, pb.Location.Y);
                       
            //BUTTON ELLIPS
            ellipseButton = new Button();
            ellipseButton.Width = 40;
            ellipseButton.Height = 40;
            ellipseButton.Image = Image.FromFile("ellipse_icon.png");
            ellipseButton.Location = new Point(10, rectangleButton.Location.Y + rectangleButton.Height + 5);                       

            //BUTTON KOLMNURK
            triangleButton = new Button();
            triangleButton.Width = 40;
            triangleButton.Height = 40;
            triangleButton.Image = Image.FromFile("triangle_icon.png");
            triangleButton.Location = new Point(rectangleButton.Location.X + rectangleButton.Width, rectangleButton.Location.Y);
            lastTrianglePoints = new Point[3];

            //BUTTON PLIATS
            btnPen = new Button();
            btnPen.Width = 40;
            btnPen.Height = 40;
            btnPen.Image = Image.FromFile("pencil_icon.png");
            btnPen.Location = new Point(ellipseButton.Location.X + ellipseButton.Width, triangleButton.Location.Y + triangleButton.Height +5);
            
            //BUTTON VARV
            btnColor = new Button();
            btnColor.Width = 80;
            btnColor.Height = 80;
            btnColor.Image = Image.FromFile("color_paint_icon.png");
            btnColor.Location = new Point(10, btnPen.Location.Y + btnPen.Height +5);            

            //FORMILE LISATUD
            this.Controls.Add(Menu);
            this.Controls.Add(pb);
            this.Controls.Add(pan);
            this.Controls.Add(tkb);
            this.Controls.Add(label_XY);
            this.Controls.Add(label_KL);
            this.Controls.Add(label_Size);
            this.Controls.Add(rectangleButton);
            this.Controls.Add(ellipseButton);
            this.Controls.Add(triangleButton);
            this.Controls.Add(btnPen);
            this.Controls.Add(btnColor);

            //NUPPUD
            newMenuItem.ShortcutKeys = Keys.Control | Keys.N;
            openMenuItem.ShortcutKeys = Keys.F3;
            saveMenuItem.ShortcutKeys = Keys.F2;
            exitMenuItem.ShortcutKeys = Keys.Alt | Keys.X;
            aboutMenuItem.ShortcutKeys = Keys.F1;
            undoMenuItem.ShortcutKeys = Keys.Control | Keys.Z;
            redoMenuItem.ShortcutKeys = Keys.Control | Keys.Y;

            //FUNKTSIOONID            
            newMenuItem.Click += NewMenuItem_Click;
            openMenuItem.Click += OpenMenuItem_Click;
            saveMenuItem.Click += SaveMenuItem_Click;
            exitMenuItem.Click += ExitMenuItem_Click;
            dotMenuItem.Click += SetDotStyle;
            solidMenuItem.Click += SetSolidStyle;
            dashdotdotMenuItem.Click += SetDashDotDotStyle;
            pb.MouseDown += Pb_MouseDown;
            pb.MouseMove += Pb_MouseMove;
            pb.MouseUp += Pb_MouseUp;
            undoMenuItem.Click += undoToolStripMenuItem_Click;
            redoMenuItem.Click += redoToolStripMenuItem_Click;
            colorMenuItem.Click += ColorMenuItem_Click;
            tkb.Scroll += TrackBar_Scroll;
            aboutMenuItem.Click += AboutMenuItem_Click;
            pb.MouseClick += Pb_MouseClick;
            rectangleButton.Click += RectangleButton_Click;
            ellipseButton.Click += EllipseButton_Click;
            triangleButton.Click += TriangleButton_Click;
            btnPen.Click += BtnPen_Click;
            btnColor.Click += ColorMenuItem_Click;

            //IKOONID
            newMenuItem.Image = Image.FromFile("new_icon.png");
            openMenuItem.Image = Image.FromFile("open_icon.png");
            saveMenuItem.Image = Image.FromFile("save_icon.png");
            exitMenuItem.Image = Image.FromFile("exit_icon.png");
            undoMenuItem.Image = Image.FromFile("undo_icon.png");
            redoMenuItem.Image = Image.FromFile("redo_icon.png");
            aboutMenuItem.Image = Image.FromFile("about_icon.png");
            penMenuItem.Image = Image.FromFile("pen_icon.png");
            colorMenuItem.Image = Image.FromFile("color_icon.png");

            //ETTEPANEK
            solidMenuItem.Checked = true;
            btnPen.Select();
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
        private void UpdateSizeLabel()
        {
            label_KL.Text = $"{pb.Width} L x {pb.Height} K";
        }
        private void OpenMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Salvestame joonis?", "Salvestamine joonist", MessageBoxButtons.YesNoCancel);

            if (result == DialogResult.Yes)
            {
                SaveMenuItem_Click(sender, e);
            }
            else if (result == DialogResult.Cancel)
            {
                return;
            }

            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Image Files|*.png;*.jpg;*.jpeg;*.bmp|All Files|*.*";

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    Image loadedImage = Image.FromFile(openFileDialog.FileName);

                    drawingSurface = new Bitmap(loadedImage.Width, loadedImage.Height);
                    pb.Image = drawingSurface;

                    pb.Width = loadedImage.Width;
                    pb.Height = loadedImage.Height;

                    using (Graphics g = Graphics.FromImage(drawingSurface))
                    {
                        g.DrawImage(loadedImage, Point.Empty);
                    }

                    History.Clear();
                    History.Add(new Bitmap(pb.Image));
                    historyCounter = 0;

                    loadedImage.Dispose();
                }
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
        private void ExitMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
        private void AboutMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("See programm on moeldud joonistamiseks.\nSiin saate kasutada edasi/tagasi funktsiooni.\nVarvid varvimiseks");
        }       
        private void DrawRectangle(Point location)
        {
            using (Graphics g = Graphics.FromImage(drawingSurface))
            {
                using (Pen pen = new Pen(currentPen.Color, currentPen.Width))
                {
                    pen.StartCap = LineCap.Round;
                    pen.EndCap = LineCap.Round;
                    pen.DashStyle = currentPen.DashStyle;

                    int width = location.X - lastPoint.X;
                    int height = location.Y - lastPoint.Y;
                    Rectangle rectangle = new Rectangle(lastPoint.X, lastPoint.Y, width, height);

                    g.DrawRectangle(pen, rectangle);
                }
            }
            pb.Invalidate();
        }
        private void DrawEllipse(Point location)
        {
            using (Graphics g = Graphics.FromImage(drawingSurface))
            {
                using (Pen pen = new Pen(currentPen.Color, currentPen.Width))
                {
                    pen.StartCap = LineCap.Round;
                    pen.EndCap = LineCap.Round;
                    pen.DashStyle = currentPen.DashStyle;

                    int width = location.X - lastPoint.X;
                    int height = location.Y - lastPoint.Y;
                    Rectangle ellipseBounds = new Rectangle(lastPoint.X, lastPoint.Y, width, height);

                    g.DrawEllipse(pen, ellipseBounds);
                }
            }
            pb.Invalidate();
        }
        private void DrawTriangle(Point location)
        {
            using (Graphics g = Graphics.FromImage(pb.Image))
            {
                using (Pen pen = new Pen(currentPen.Color, currentPen.Width))
                {
                    pen.StartCap = LineCap.Round;
                    pen.EndCap = LineCap.Round;
                    pen.DashStyle = currentPen.DashStyle;

                    Point[] trianglePoints = new Point[]
                    {
                new Point(lastPoint.X + (location.X - lastPoint.X) / 2, lastPoint.Y),
                new Point(location.X, location.Y),
                new Point(lastPoint.X, location.Y)
                    };
                    g.DrawPolygon(pen, trianglePoints);
                }
            }
            pb.Invalidate();
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
        private void Pb_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                drawing = true;
                erasing = false;
                lastPoint = e.Location;

                if (currentDrawingMode == DrawingMode.Rectangle || currentDrawingMode == DrawingMode.Ellipse)
                {
                    lastRectangle = new Rectangle(lastPoint, new Size(0, 0));
                }
                else if (currentDrawingMode == DrawingMode.Triangle)
                {
                    lastTrianglePoints = new Point[3] { lastPoint, lastPoint, lastPoint };
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
                if (currentDrawingMode == DrawingMode.Pen)
                {
                    Color drawColor = (e.Button == MouseButtons.Left) ? currentPen.Color : pb.BackColor;
                    DrawWithPen(drawColor, e.Location);
                }
                else if (currentDrawingMode == DrawingMode.Rectangle)
                {
                    DrawRectangle(e.Location);
                }
                else if (currentDrawingMode == DrawingMode.Ellipse)
                {
                    DrawEllipse(e.Location);
                }
                else if (currentDrawingMode == DrawingMode.Triangle)
                {
                    DrawTriangle(e.Location);
                }
            }

            lastPoint = e.Location;
        }
        private void DrawWithPen(Color color, Point location)
        {
            using (Graphics g = Graphics.FromImage(pb.Image))
            {
                using (Pen pen = new Pen(color, currentPen.Width))
                {
                    pen.StartCap = LineCap.Round;
                    pen.EndCap = LineCap.Round;
                    pen.DashStyle = currentPen.DashStyle;
                    g.SmoothingMode = SmoothingMode.AntiAlias;
                    g.DrawLine(pen, lastPoint, location);
                }
            }

            pb.Invalidate();
        }
        private void SetDotStyle(object sender, EventArgs e)
        {
            currentPen.DashStyle = DashStyle.DashDot;

            solidMenuItem.Checked = false;
            dotMenuItem.Checked = true;
            dashdotdotMenuItem.Checked = false;
        }
        private void SetSolidStyle(object sender, EventArgs e)
        {
            currentPen.DashStyle = DashStyle.Solid;

            solidMenuItem.Checked = true;
            dotMenuItem.Checked = false;
            dashdotdotMenuItem.Checked = false;
        }
        private void SetDashDotDotStyle(object sender, EventArgs e)
        {
            currentPen.DashStyle = DashStyle.DashDotDot;

            solidMenuItem.Checked = false;
            dotMenuItem.Checked = false;
            dashdotdotMenuItem.Checked = true;
        }
        private void TrackBar_Scroll(object sender, EventArgs e)
        {
            currentPen.Width = tkb.Value;            
            label_Size.Text = $"{tkb.Value.ToString()} px";
        }
        private void NewMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Salvestame joonis?", "Salvestamine joonist", MessageBoxButtons.YesNoCancel);

            if (result == DialogResult.Yes)
            {
                SaveMenuItem_Click(sender, e);
            }
            else if (result == DialogResult.Cancel)
            {
                return;
            }
            using (SizeForm sizeForm = new SizeForm())
            {
                DialogResult sizeResult = sizeForm.ShowDialog();

                if (sizeResult == DialogResult.OK)
                {
                    pb.Width = sizeForm.NewWidth;
                    pb.Height = sizeForm.NewHeight; ;

                    drawingSurface = new Bitmap(pb.Width, pb.Height);
                    using (Graphics g = Graphics.FromImage(pb.Image))
                    {
                        g.Clear(Color.White);
                    }

                    pb.Image = drawingSurface;

                    History.Clear();
                    historyCounter = 0;

                    History.Add(new Bitmap(pb.Image));
                }
                else if (sizeResult == DialogResult.Cancel)
                {
                    // Vaikimisi e Default XY pb-le jääb sama
                }
            }
            UpdateSizeLabel();
            pan.Location = new Point(pb.Location.X, pb.Location.Y + pb.Height + 5);
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