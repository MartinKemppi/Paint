using System.Drawing.Drawing2D;

namespace Paint
{
    public partial class Paint : Form
    {
        private MenuStrip Menu;
        private ToolStrip TlSp;
        private Panel pan;
        private ColorDialog crdiag;
        private OpenFileDialog ofd;
        private SaveFileDialog sfd;
        private PictureBox pb;
        private ImageList il;
        private TrackBar tkb;
        private ComboBox CB;
        private List<Image> History;
        private int historyCounter;
        private Bitmap drawingSurface;
        private bool drawing = false;
        private bool erasing = false;
        private Point lastPoint;        
        private Color historyColor;
        public Pen currentPen;

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
            ToolStripMenuItem colorMenuItem = new ToolStripMenuItem("Värv");
            ToolStripMenuItem solidMenuItem = new ToolStripMenuItem("Tugev");
            ToolStripMenuItem dotMenuItem = new ToolStripMenuItem("Punktidega");
            ToolStripMenuItem dashdotdotMenuItem = new ToolStripMenuItem("KriipsTäppTäpp");

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

            aboutMenuItem.ShortcutKeys = Keys.F1;

            aboutMenuItem.Click += AboutMenuItem_Click;

            Menu.Items.Add(fileMenuItem);
            Menu.Items.Add(editMenuItem);
            Menu.Items.Add(helpMenuItem);

            //HISTORY
            History = new List<Image>();

            //PICTUREBOX
            pb = new PictureBox();
            pb.Location = new Point(100,50);
            pb.Size = new Size(650, 650);
            pb.SizeMode = PictureBoxSizeMode.Zoom;
            pb.BorderStyle = BorderStyle.Fixed3D;

            // TRACKBAR
            tkb = new TrackBar();
            tkb.Height = 50;
            tkb.Width = 200;
            tkb.Minimum = 1;
            tkb.Maximum = 100;
            tkb.Value = 40;
            
            tkb.Scroll += TrackBar_Scroll;

            // PANEL
            pan = new Panel();
            pan.Location = new Point(pb.Location.X, pb.Location.Y + pb.Height);
            pan.Size = new Size(pb.Width, 50);            

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

            //FORMILE LISATUD
            this.Controls.Add(Menu);
            this.Controls.Add(pb);
            this.Controls.Add(pan);
            this.Controls.Add(tkb);

            pan.Controls.Add(tkb);
        }
        private void OpenMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Open");
        }

        private void SaveMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Save");
        }

        private void ExitMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
        private void AboutMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("About");
        }
        private void Pb_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                drawing = true;
                lastPoint = e.Location;
            }
        }
        private void Pb_MouseMove(object sender, MouseEventArgs e)
        {
            if (drawing || erasing)
            {
                using (Graphics g = Graphics.FromImage(drawingSurface))
                {
                    if (erasing)
                    {
                        using (Pen eraserPen = new Pen(Color.White, tkb.Value))
                        {
                            eraserPen.StartCap = LineCap.Round;
                            eraserPen.EndCap = LineCap.Round;
                            g.SmoothingMode = SmoothingMode.AntiAlias;
                            g.DrawLine(eraserPen, lastPoint, e.Location);
                        }
                    }
                    else
                    {
                        using (Pen pen = new Pen(currentPen.Color, tkb.Value))
                        {
                            pen.StartCap = LineCap.Round;
                            pen.EndCap = LineCap.Round;
                            g.SmoothingMode = SmoothingMode.AntiAlias;
                            g.DrawLine(pen, lastPoint, e.Location);
                        }
                    }
                }
                lastPoint = e.Location;
                pb.Invalidate();
            }
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
            if (pb.Image != null)
            {
                using (Graphics g = Graphics.FromImage(pb.Image))
                {
                    using (Pen pen = new Pen(Color.Black, tkb.Value))
                    {
                        pen.DashStyle = style;
                        if (erasing)
                        {
                            pen.Color = pb.BackColor;
                        }
                        g.DrawLine(pen, lastPoint, lastPoint);
                    }
                }
                pb.Invalidate();
                SaveToHistory();
            }
        }
        private void SetDotStyle()
        {
            SetPenStyle(DashStyle.Dot);
        }

        private void SetSolidStyle()
        {
            SetPenStyle(DashStyle.Solid);
        }

        private void SetDashDotDotStyle()
        {
            SetPenStyle(DashStyle.DashDotDot);
        }
        private void TrackBar_Scroll(object sender, EventArgs e)
        {
            currentPen.Width = tkb.Value;
        }
        private void NewMenuItem_Click(object sender, EventArgs e)
        {
            History.Clear();
            historyCounter = 0;
            Bitmap pic = new Bitmap(750, 500);
            pb.Image = pic;
            History.Add(new Bitmap(pb.Image));
        }
        private void picDrawingSurface_MouseUp(object sender, MouseEventArgs e)
        {            
            SaveToHistory();

            drawing = false;
        }
    }
}