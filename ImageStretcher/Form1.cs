using HyperbolicRenderer;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace ImageStretcher
{
    public partial class Form1 : Form
    {
        System.Timers.Timer timer = new System.Timers.Timer(10);
        TimeScalar scalar;
        public Form1()
        {
            InitializeComponent();
            timer.Elapsed += new System.Timers.ElapsedEventHandler(Update);
            timer.Start();
            image = (Bitmap)pictureBox1.Image.Clone();
            scalar = new TimeScalar(new PointF(image.Width / 2, image.Height / 2));
            pictureBox1.Image = null;
            pictureBox1.Invalidate();
        }

        private void Update(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (started)
            {
                pictureBox1.Invalidate();
            }
        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            if (image != null)
            {
                Bitmap result = new Bitmap(pictureBox1.Width, pictureBox1.Height);
                ImageDeformer deformer = new ImageDeformer(image);
                deformer.DeformImageToPolygon(scalar.TransformPoint, new Point(0, 0), result);
                e.Graphics.DrawImage(result, new Point(0, 0));
            }
        }
        Bitmap image;
        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.ShowDialog(this);
            string name = ofd.FileName;
            if (File.Exists(name))
            {
                string extension = name.Split(".")[1];
                if (extension == "png" || extension == "jpg")
                {
                    var temp = (Bitmap)Image.FromFile(name);
                    image = temp.Clone(new Rectangle(0, 0, temp.Width, temp.Height), System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                    scalar = new TimeScalar(new PointF(image.Width / 2, image.Height / 2));
                    pictureBox1.Invalidate();
                }
            }
        }
        bool started = false;
        double lasttime = 0;
        private void button2_Click(object sender, EventArgs e)
        {
            started = !started;
            if (started)
            {
                scalar.time = lasttime;
                button2.BackColor = Color.Red;
                button2.Text = "Stop";
            }
            else
            {
                button2.BackColor = Color.FromArgb(0, 255, 0);
                button2.Text = "Start time";
                lasttime = scalar.time;
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            float outfloat;
            switch (((Control)sender).Name)
            {
                case "textBox1": //speed
                    if (float.TryParse(textBox1.Text, out outfloat))
                    {
                        scalar.speed = outfloat;
                    }
                    break;
                case "textBox2": //period
                    if (float.TryParse(textBox2.Text, out outfloat))
                    {
                        if (outfloat % 1 != 0)
                        {
                            textBox2.ForeColor = Color.Red;
                        }
                        else
                        {
                            textBox2.ForeColor = Color.Black;
                        }
                        scalar.period = outfloat;
                    }
                    break;
                case "textBox3": //amplitude
                    if (float.TryParse(textBox3.Text, out outfloat))
                    {
                        if (outfloat > 0.3f)
                        {
                            textBox3.ForeColor = Color.Red;
                        }
                        else
                        {
                            textBox3.ForeColor = Color.Black;
                        }
                        scalar.amplitude = outfloat;
                    }
                    break;
            }
        }
    }
}