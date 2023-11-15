using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Paint
{
    public partial class SizeForm : Form
    {
        TextBox korgus, laius;
        Button OK;
        PictureBox pb;
        public int NewWidth
        {
            get { return int.TryParse(laius.Text, out int width) ? width : 0; }
        }
        public int NewHeight
        {
            get { return int.TryParse(korgus.Text, out int height) ? height : 0; }
        }
        public SizeForm()
        {
            this.Height = 150;
            this.Width = 250;
            this.Text = "Suurused";

            korgus = new TextBox();
            korgus.Location = new Point(10, 10);
            korgus.Text = "";
            korgus.Width = 150;
            korgus.PlaceholderText = "sisesta korgus, 650 algusel";
            
            laius = new TextBox();
            laius.Location = new Point(korgus.Location.X, korgus.Location.Y + korgus.Height);
            laius.Text = "";
            laius.Width = 150;
            laius.PlaceholderText = "sisesta laius, 650 algusel";

            OK = new Button();
            OK.Location = new Point(laius.Location.X, laius.Location.Y + laius.Height);
            OK.Text = "OK";
            OK.Width = 150;
            OK.Click += ClickMe;

            pb = new PictureBox();

            this.Controls.Add(korgus);
            this.Controls.Add(laius);
            this.Controls.Add(OK);
            this.Controls.Add(pb);

            if (pb.Image == null)
            {
                return;
            }
        }
        private void ClickMe(object sender, EventArgs e)
        {
            if (int.TryParse(korgus.Text, out int height) && int.TryParse(laius.Text, out int width))
            {
                SetPictureBoxSizes(width, height);
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            else
            {
                MessageBox.Show("Sisesta ainult numbrid");
            }
        }
        private void SetPictureBoxSizes(int width, int height)
        {            
            pb.Width = width;
            pb.Height = height;
        }
    }
}
