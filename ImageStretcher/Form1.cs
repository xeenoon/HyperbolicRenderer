using System.Diagnostics;
using System.Drawing.Imaging;
using AnimatedGif;

namespace ImageStretcher
{
    public partial class Form1 : Form
    {
        System.Timers.Timer UpdateTimer = new System.Timers.Timer(10);
        PointTransformer scalar;
        PolygonMenu menu;
        public Form1()
        {
            InitializeComponent();
            UpdateTimer.Elapsed += new System.Timers.ElapsedEventHandler(Update);
            UpdateTimer.Start();
            image = (Bitmap)pictureBox1.Image.Clone();
            pictureBox1.Image = null;
            pictureBox1.Invalidate();
            menu = new PolygonMenu(polygonMenu, addPolygonButton);
            scalar = new PointTransformer(new PointF(image.Width / 2, image.Height / 2), image.Width, menu);
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
                Bitmap result = new Bitmap(pictureBox1.Width, pictureBox1.Height);
                ImageDeformer.DeformImageToPolygon(scalar.TransformPoint, new Point(offset.X, offset.Y), image, result, resolution);

                e.Graphics.DrawImage(result, 0, 0, result.Width, result.Height);

                foreach (var polygon in menu.menuItems.Where(m => m.visiblepolygon).Select(m => m.polygonpoints))
                {
                    PointF[] offsetpolygon = new PointF[polygon.Count];
                    for (int i = 0; i < polygon.Count; i++)
                    {
                        PointF point = polygon[i];
                        PointF offsetedpoint = new PointF(point.X + offset.X, point.Y + offset.Y);
                        e.Graphics.FillEllipse(new Pen(Color.Blue).Brush, new Rectangle((int)(offsetedpoint.X - 2), (int)(offsetedpoint.Y - 2), 4, 4));
                        offsetpolygon[i] = offsetedpoint;
                    }
                    if (offsetpolygon.Count() >= 3)
                    {
                        e.Graphics.DrawPolygon(new Pen(Color.Black), offsetpolygon.ToArray());
                    }
                }

                result.Dispose();
            }
        }
        Bitmap image;
        private void ImportImage(object sender, EventArgs e)
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
                    scalar = new PointTransformer(new PointF(image.Width / 2, image.Height / 2), image.Width, menu);
                    pictureBox1.Invalidate();
                }
            }
        }
        bool started = false;
        double lasttime = 0;
        private void StartStopButton(object sender, EventArgs e)
        {
            started = !started;
            if (started)
            {
                scalar.time = lasttime;
                timeButton.BackColor = Color.Red;
                timeButton.Text = "Stop";
                scalar.Restart();
            }
            else
            {
                timeButton.BackColor = Color.FromArgb(0, 255, 0);
                timeButton.Text = "Start time";
                lasttime = scalar.time;
                scalar.Pause();
            }
        }

        int resolution = 2;
        Point offset = new Point(0, 0);
        private void MenuItemTextChanged(object sender, EventArgs e)
        {
            float outfloat;
            switch (((Control)sender).Name)
            {
                case "offsetTextbox":
                    if (offsetTextbox.Text.Contains(','))
                    {
                        int.TryParse(offsetTextbox.Text.Split(',')[0], out int x);
                        int.TryParse(offsetTextbox.Text.Split(',')[1], out int y);

                        offset.X = x;
                        offset.Y = y;
                    }
                    break;
            }
            pictureBox1.Invalidate();
        }
        private void Restart(object sender, EventArgs e)
        {
            scalar.time = 0;
            pictureBox1.Invalidate();
        }
        private void Benchmark(object sender, EventArgs e)
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
        private void ExportGIF(object sender, EventArgs e)
        {
            PointTransformer scalar = new PointTransformer(new PointF(image.Width / 2, image.Height / 2), image.Width, menu, false);
            scalar.speed = this.scalar.speed;
            //const float timeamt = 2;
            int frames = (int)((menu.menuItems.Max(m => m.period) * 4) / (Math.PI * 2)) * 33;
            //choose the largest period, if the user specifies periods that dont line up, its their problem

            Bitmap[] GIFbitmaps = new Bitmap[frames];
            string path = SelectFolder();
            if (path != "")
            {
                using (var gif = AnimatedGif.AnimatedGif.Create(path + @"\gif.gif", 33))
                {
                    for (int i = 0; i < frames; ++i)
                    {
                        scalar.time += ((2 * Math.PI) / (31.4f));
                        Bitmap temp = new Bitmap(image.Width + offset.X * 2, image.Height + offset.X * 2);
                        var data = ImageDeformer.DeformImageToPolygon(scalar.TransformPoint, new Point(offset.X, offset.Y), image, temp, 2, true);
                        GIFbitmaps[i] = temp.Clone(new Rectangle(data.left, data.top, data.right - data.left, data.bottom - data.top), PixelFormat.Format32bppRgb);
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
        private void ExportFramesClick(object sender, EventArgs e)
        {
            ExportPanelSettings.Visible = true;
        }
        private void ExportFrames()
        {
            PointTransformer scalar = new PointTransformer(new PointF(image.Width / 2, image.Height / 2), image.Width, menu, false);
            scalar.speed = this.scalar.speed;
            //const float timeamt = 2;
            int frames = (int)((menu.menuItems.Max(m => m.period) * 4) / (Math.PI * 2)) * 33;
            //choose the largest period, if the user specifies periods that dont line up, its their problem

            string path = SelectFolder();
            if (path != "")
            {
                Bitmap[] GIFbitmaps = new Bitmap[frames];
                for (int i = 0; i < frames; ++i)
                {
                    scalar.time += ((2 * Math.PI) / (31.4f));
                    var temp = new Bitmap(image.Width + offset.X * 2, image.Height + offset.Y * 2);
                    var data = ImageDeformer.DeformImageToPolygon(scalar.TransformPoint, new Point(offset.X, offset.Y), image, temp, 2, true);
                    GIFbitmaps[i] = temp.Clone(new Rectangle(data.left, data.top, data.right - data.left, data.bottom - data.top), PixelFormat.DontCare);
                    GIFbitmaps[i].Save(string.Format("{0}\\{1}_{2}.png", path, animationname, i));
                }
                MessageBox.Show("Finished exporting");
            }
        }
        Bitmap[] frames;
        private void ImportAnimation(object sender, EventArgs e)
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

                ImportSettingsPanel.Visible = true; //Show the menu
            }
        }
        int delay;
        bool restartanimation;
        bool animating;
        private void AnimationImportClick(object sender, EventArgs e)
        {
            if (!int.TryParse(delayTextbox.Text, out delay))
            {
                MessageBox.Show("Invalid FPS");
            }
            else
            {
                delayTextbox.Text = "";
                restartanimation = looptypeDropdown.SelectedIndex == 0;
                ImportSettingsPanel.Visible = false;
                animating = true;
            }
        }
        private void Form1_Load(object sender, EventArgs e)
        {

        }

        string animationname;
        private void finalExportButton_Click(object sender, EventArgs e)
        {
            animationname = framenameTextbox.Text;
            ExportFrames();
            framenameTextbox.Text = "";
            ExportPanelSettings.Visible = false;
        }

        private void exportSettingsCloseButton_Click(object sender, EventArgs e)
        {
            ExportPanelSettings.Visible = false;
        }

        private void importSettingsCloseButton_Click(object sender, EventArgs e)
        {
            ImportSettingsPanel.Visible = false;
        }

        private void addPolygonButton_Click(object sender, EventArgs e)
        {
            new PolygonMenuItem(menu, Repaint);
            polygonMenu.ScrollControlIntoView(addPolygonButton);
        }
        bool Repaint()
        {
            pictureBox1.Invalidate();
            return false;
        }
        private void AddPoint(object sender, EventArgs e)
        {
            if (menu.selecteditem != null)
            {
                Point clickpos = pictureBox1.PointToClient(Cursor.Position);
                clickpos.X -= offset.X;
                clickpos.Y -= offset.Y;
                if (ModifierKeys == Keys.Shift)
                {
                    PointF closestpoint;
                    if (menu.selecteditem.polygonpoints.Any(m => m.DistanceTo(clickpos) < 4))
                    {
                        closestpoint = menu.selecteditem.polygonpoints.Where(m => m.DistanceTo(clickpos) < 4).FirstOrDefault();
                        menu.selecteditem.polygonpoints.Remove(closestpoint);
                    }
                }
                else
                {
                    menu.selecteditem.AddPoint(clickpos);
                }
                pictureBox1.Invalidate();
            }
        }
    }
}