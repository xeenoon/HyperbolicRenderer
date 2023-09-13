using System.Diagnostics;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using AnimatedGif;
using Splicer.Renderer;
using Splicer.Timeline;
using Splicer.WindowsMedia;

namespace ImageStretcher
{
    public partial class Form1 : Form
    {
        System.Timers.Timer timer = new System.Timers.Timer(10);
        PointTransformer scalar;
        public Form1()
        {
            InitializeComponent();
            timer.Elapsed += new System.Timers.ElapsedEventHandler(Update);
            timer.Start();
            image = (Bitmap)pictureBox1.Image.Clone();
            scalar = new PointTransformer(new PointF(image.Width / 2, image.Height / 2), image.Width);
            pictureBox1.Image = null;
            pictureBox1.Invalidate();
        }
        double lastlooptime;
        int frameidx = 0;
        bool direction = true;
        private void Update(object sender, System.Timers.ElapsedEventArgs e)
        {
            lastlooptime += 10;
            if (lastlooptime > delay && animating)
            {
                lastlooptime = 0;

                if (direction)
                {
                    frameidx++;
                }
                else
                {
                    frameidx--;
                }
                if (frameidx == frames.Length)
                {
                    if (restartanimation)
                    {
                        frameidx = 0;
                    }
                    else
                    {
                        direction = false;
                        frameidx -= 2;
                    }
                }
                else if (frameidx == 0)
                {
                    direction = true;
                }

                image = frames[frameidx];
                scalar.centre = new PointF(image.Width / 2, image.Height / 2);
            }
            if (started)
            {
                pictureBox1.Invalidate();
            }
        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            if (image != null)
            {
                Bitmap result = new Bitmap(image.Width, image.Height);
                ImageDeformer.DeformImageToPolygon(scalar.TransformPoint, new Point(0, 0), image, result, resolution);
                e.Graphics.DrawImage(result, 0, 0, image.Width * imagescale, image.Height * imagescale);
                //  e.Graphics.DrawPolygon(new Pen(Color.Orange), PointTransformer.bobsleftarm);
                //  e.Graphics.DrawPolygon(new Pen(Color.Orange), PointTransformer.bobsrightarm);
                //  e.Graphics.DrawPolygon(new Pen(Color.Orange), PointTransformer.bobshead);
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
                    animating = false;
                    var temp = (Bitmap)Image.FromFile(name);
                    image = temp.Clone(new Rectangle(0, 0, temp.Width, temp.Height), System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                    scalar = new PointTransformer(new PointF(image.Width / 2, image.Height / 2), image.Width);
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
            Stopwatch s = new Stopwatch();
            s.Start();
            Bitmap result = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            for (int i = 0; i < 1000; ++i)
            {
                ImageDeformer.DeformImageToPolygon(scalar.TransformPoint, new Point(0, 0), image, result, resolution);
            }
            s.Stop();
            MessageBox.Show("Did 1000 operations, averaging: " + (s.ElapsedMilliseconds / 1000f).ToString() + "ms per frame");
        }

        private void button4_Click(object sender, EventArgs e)
        {
            PointTransformer scalar = new PointTransformer(new PointF(image.Width / 2, image.Height / 2), image.Width, false);
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
                        scalar.time += ((2 * Math.PI) / (31.4f));
                        GIFbitmaps[i] = new Bitmap(image.Width, image.Height);
                        ImageDeformer.DeformImageToPolygon(scalar.TransformPoint, new Point(0, 0), image, GIFbitmaps[i], 2, true);
                        gif.AddFrame(GIFbitmaps[i], delay: (int)(33 / scalar.speed), quality: GifQuality.Default);
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

        private void button5_Click(object sender, EventArgs e)
        {
            PointTransformer scalar = new PointTransformer(new PointF(image.Width / 2, image.Height / 2), image.Width, false);
            scalar.period = this.scalar.period;
            scalar.amplitude = this.scalar.amplitude;
            scalar.speed = this.scalar.speed;

            string path = SelectFolder();
            int frames = (int)((scalar.period * 4) / (Math.PI * 2)) * 33;
            if (path != "")
            {
                Bitmap[] GIFbitmaps = new Bitmap[frames];
                for (int i = 0; i < frames; ++i)
                {
                    scalar.time += ((2 * Math.PI) / (31.4f));
                    GIFbitmaps[i] = new Bitmap(image.Width, image.Height);
                    ImageDeformer.DeformImageToPolygon(scalar.TransformPoint, new Point(0, 0), image, GIFbitmaps[i]);
                    GIFbitmaps[i].Save(path + @"\" + i.ToString() + ".png");
                }
                MessageBox.Show("Finished exporting");
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            PointTransformer scalar = new PointTransformer(new PointF(image.Width / 2, image.Height / 2), image.Width, false);
            scalar.period = this.scalar.period;
            scalar.amplitude = this.scalar.amplitude;
            scalar.speed = this.scalar.speed;

            string path = SelectFolder();
            int frames = (int)((scalar.period * 4) / (Math.PI * 2)) * 33;
            if (path != "")
            {
                string outputFile = path + @"\vid.mp4";

                Bitmap[] GIFbitmaps = new Bitmap[frames];

                for (int i = 0; i < frames; ++i)
                {
                    scalar.time += ((2 * Math.PI) / (31.4f));
                    GIFbitmaps[i] = new Bitmap(image.Width, image.Height);
                    ImageDeformer.DeformImageToPolygon(scalar.TransformPoint, new Point(0, 0), image, GIFbitmaps[i]);
                }
                bool success = false;
                do
                {
                    success = CreateVideo(GIFbitmaps.ToList(), outputFile, 10 * scalar.speed);
                } while (!success);
                MessageBox.Show("Finished exporting");
            }
        }

        public bool CreateVideo(List<Bitmap> bitmaps, string outputFile, double fps)
        {
            int width = 640;
            int height = 480;
            if (bitmaps == null || bitmaps.Count == 0) return false;
            try
            {
                using (ITimeline timeline = new DefaultTimeline(fps))
                {
                    IGroup group = timeline.AddVideoGroup(32, width, height);
                    ITrack videoTrack = group.AddTrack();

                    int i = 0;
                    double miniDuration = 1.0 / fps;
                    foreach (var bmp in bitmaps)
                    {
                        IClip clip = videoTrack.AddImage(bmp, 0, i * miniDuration, (i + 1) * miniDuration);
                    }
                    timeline.AddAudioGroup();
                    IRenderer renderer = new WindowsMediaRenderer(timeline, outputFile, WindowsMediaProfiles.HighQualityVideo);
                    renderer.Render();
                }
            }
            catch { return false; }
            return true;
        }
        Bitmap[] frames;
        private void button7_Click(object sender, EventArgs e)
        {
            string folder = SelectFolder();
            if (folder != "")
            {
                Dictionary<int, Bitmap> animationframes = new Dictionary<int, Bitmap>();
                foreach (string path in Directory.EnumerateFiles(folder))
                {
                    int fileno;
                    if (int.TryParse(path.Split("\\").Last().Split('.')[0], out fileno))
                    {
                        if (animationframes.ContainsKey(fileno))
                        {
                            MessageBox.Show("Invalid animation sequence, frame + " + fileno.ToString() + " is repeated");
                        }
                        animationframes.Add(fileno, (Bitmap)Image.FromFile(path));
                    }
                }
                frames = animationframes.OrderBy(a => a.Key).Select(a => a.Value).ToArray();

                panel1.Visible = true; //Show the menu
            }
        }
        int delay;
        bool restartanimation;
        bool animating;
        private void button8_Click(object sender, EventArgs e)
        {
            if (!int.TryParse(textBox5.Text, out delay))
            {
                MessageBox.Show("Invalid FPS");
            }
            else
            {
                textBox5.Text = "";
                restartanimation = comboBox1.SelectedIndex == 0;
                panel1.Visible = false;
                animating = true;
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
        float imagescale = 1;
        private void textBox6_TextChanged(object sender, EventArgs e)
        {
            float newscale;
            if (float.TryParse(textBox6.Text, out newscale))
            {
                imagescale = newscale;
            }
        }
        int resolution = 2;
        private void textBox4_TextChanged(object sender, EventArgs e)
        {
            float tempresolution;
            if (float.TryParse(textBox4.Text, out tempresolution))
            {
                if (tempresolution > 1 || tempresolution == 0)
                {
                    textBox4.ForeColor = Color.Red;
                }
                else
                {
                    textBox4.ForeColor = Color.Black;
                    resolution = (int)(2 / tempresolution);
                }
            }
        }
    }
}