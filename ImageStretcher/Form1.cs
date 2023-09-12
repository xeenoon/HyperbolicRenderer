using HyperbolicRenderer;
using ManagedCuda.BasicTypes;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using AnimatedGif;

namespace ImageStretcher
{
    public partial class Form1 : Form
    {
        System.Timers.Timer timer = new System.Timers.Timer(10);
        TimeScalar scalar;
        ImageDeformer deformer;
        public Form1()
        {
            InitializeComponent();
            timer.Elapsed += new System.Timers.ElapsedEventHandler(Update);
            timer.Start();
            image = (Bitmap)pictureBox1.Image.Clone();
            scalar = new TimeScalar(new PointF(image.Width / 2, image.Height / 2));
            pictureBox1.Image = null;
            pictureBox1.Invalidate();
            deformer = new ImageDeformer(image);
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
                deformer.DeformImageToPolygon(scalar.TransformPoint, new Point(0, 0), result);
                e.Graphics.DrawImage(result, new Point(0, 0));
                result.Dispose();
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
                    deformer.GC_pacifier.Dispose();
                    deformer = new ImageDeformer(image);
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

        private void button3_Click(object sender, EventArgs e)
        {
            int resolution;
            if (int.TryParse(textBox4.Text, out resolution))
            {
                Stopwatch s = new Stopwatch();
                s.Start();
                Bitmap result = new Bitmap(pictureBox1.Width, pictureBox1.Height);
                for (int i = 0; i < 1000; ++i)
                {
                    deformer.DeformImageToPolygon(scalar.TransformPoint, new Point(0, 0), result, resolution);
                }
                s.Stop();
                MessageBox.Show("Did 1000 operations, averaging: " + (s.ElapsedMilliseconds / 1000f).ToString());
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            TimeScalar scalar = new TimeScalar(new PointF(image.Width / 2, image.Height / 2), false);
            scalar.period = this.scalar.period;
            scalar.amplitude = this.scalar.amplitude;
            scalar.speed = this.scalar.speed;
            //const float timeamt = 2;
            int frames = (int)((scalar.period * 4) / (Math.PI * 2)) * 33;

            Bitmap[] GIFbitmaps = new Bitmap[frames];
            string path = SelectFolder();
            if (path != "")
            {
                using (var gif = AnimatedGif.AnimatedGif.Create(path + @"\gif.gif", 33))
                {
                    for (int i = 0; i < frames; ++i)
                    {
                        scalar.time += ((2 * Math.PI) / (33f));
                        GIFbitmaps[i] = new Bitmap(image.Width, image.Height);
                        deformer.DeformImageToPolygon(scalar.TransformPoint, new Point(0, 0), GIFbitmaps[i]);
                        gif.AddFrame(GIFbitmaps[i], delay: 33, quality: GifQuality.Default);
                    }
                }
            }
            MessageBox.Show("Finished exporting");
        }
        private string SelectFolder()
        {
            using (FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog())
            {
                // Set the initial folder (optional)
                //folderBrowserDialog.SelectedPath = @"C:\users"; // Change to your desired initial folder

                // Show the FolderBrowserDialog and check if the user clicked the "OK" button
                if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
                {
                    // Get the selected folder path
                    return folderBrowserDialog.SelectedPath;
                }
            }

            // Return an empty string if the user cancels the dialog
            return string.Empty;
        }
    }
}