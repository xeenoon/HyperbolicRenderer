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
                MessageBox.Show("Did 1000 operations, averaging: " + (s.ElapsedMilliseconds / 1000f).ToString() + "ms per frame");
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
                        scalar.time += ((2 * Math.PI) / (31.4f));
                        GIFbitmaps[i] = new Bitmap(image.Width, image.Height);
                        deformer.DeformImageToPolygon(scalar.TransformPoint, new Point(0, 0), GIFbitmaps[i]);
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
            TimeScalar scalar = new TimeScalar(new PointF(image.Width / 2, image.Height / 2), false);
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
                    deformer.DeformImageToPolygon(scalar.TransformPoint, new Point(0, 0), GIFbitmaps[i]);
                    GIFbitmaps[i].Save(path + @"\" + i.ToString() + ".png");
                }
                MessageBox.Show("Finished exporting");
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            TimeScalar scalar = new TimeScalar(new PointF(image.Width / 2, image.Height / 2), false);
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
                    deformer.DeformImageToPolygon(scalar.TransformPoint, new Point(0, 0), GIFbitmaps[i]);
                }
                bool success = false;
                do
                {
                    success  = CreateVideo(GIFbitmaps.ToList(), outputFile, 10 * scalar.speed);
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
    }
    public class TimeScalar
    {
        PointF centre;
        System.Timers.Timer timer = new System.Timers.Timer();

        public double time = 0;
        public double period = 2;
        public double amplitude = 0.05;
        public double speed = 1;
        public TimeScalar(PointF centre, bool usetimer = true)
        {
            this.centre = centre;
            if (usetimer)
            {
                timer.Interval = 100;
                timer.Elapsed += new System.Timers.ElapsedEventHandler(Update);
                timer.Start();
            }
        }
        public void Update(object sender, System.Timers.ElapsedEventArgs e)
        {
            time += 0.5f;
        }
        public System.Drawing.Point TransformPoint(System.Drawing.Point input)
        {
            PointF adjustedpoint = new PointF((input.X - centre.X), (input.Y - centre.Y));

            //Based on time, points will be scaled based on their angle to the centre
            double angle = Math.Atan(adjustedpoint.Y / adjustedpoint.X) + Math.PI / 2;
            float heightmultiplier = (float)((Math.Cos((angle * period * 2) + (time * speed))) * amplitude) + 1;
            adjustedpoint.X *= heightmultiplier;
            adjustedpoint.Y *= heightmultiplier;

            adjustedpoint.X += centre.X;
            adjustedpoint.Y += centre.Y;

            return new System.Drawing.Point((int)adjustedpoint.X, (int)adjustedpoint.Y);
        }
    }
}