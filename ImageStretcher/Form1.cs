using System.Diagnostics;
using System.Drawing.Imaging;
using System.IO;
using System.Security.Cryptography.Pkcs;
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

            SaveFileDialog saveFileDialogue = new SaveFileDialog();
            saveFileDialogue.ShowDialog();
            if (saveFileDialogue.FileName != "")
            {

                if (!saveFileDialogue.FileName.Contains('.') || saveFileDialogue.FileName.Split('.')[1] != "gif") //TODO add this into the fileopen dialogue
                {
                    MessageBox.Show("Must save as GIF");
                    return;
                }
                if (!File.Exists(saveFileDialogue.FileName))
                {
                    var file = File.Create(saveFileDialogue.FileName);
                    file.Dispose();
                }
                using (var gif = AnimatedGif.AnimatedGif.Create(saveFileDialogue.FileName, 33))
                {
                    var frames = GetFrames();
                    foreach (var bmp in frames)
                    {
                        gif.AddFrame(bmp, delay: (int)(33 / scalar.speed), quality: GifQuality.Default);

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
            //choose the largest period, if the user specifies periods that dont line up, its their problem

            string path = SelectFolder();
            if (path != "")
            {
                Bitmap[] array = GetFrames();
                for (int i = 0; i < array.Length; i++)
                {
                    Bitmap? bmp = array[i];
                    bmp.Save(string.Format("{0}\\{1}_{2}.png", path, animationname, i));
                }

                MessageBox.Show("Finished exporting");
            }
        }
        public Bitmap[] GetFrames()
        {
            int frames = (int)((menu.menuItems.Max(m => m.period) * 4) / (Math.PI * 2)) * 33;

            Bitmap[] GIFbitmaps = new Bitmap[frames];
            Bitmap[] tempbitmaps = new Bitmap[(int)frames];
            int minleft = int.MaxValue;
            int mintop = int.MaxValue;
            int maxright = int.MinValue;
            int maxbottom = int.MinValue;
            for (int i = 0; i < frames; ++i)
            {
                scalar.time += ((2 * Math.PI) / (31.4f));
                tempbitmaps[i] = new Bitmap(pictureBox1.Width, pictureBox1.Height);
                var data = ImageDeformer.DeformImageToPolygon(scalar.TransformPoint, new Point(offset.X, offset.Y), image, tempbitmaps[i], 2, true);
                minleft = Math.Min(minleft, data.left);
                mintop = Math.Min(mintop, data.top);
                maxright = Math.Max(maxright, data.right);
                maxbottom = Math.Max(maxbottom, data.bottom);
            }
            for (int i = 0; i < tempbitmaps.Length; i++)
            {
                Bitmap temp = tempbitmaps[i];
                GIFbitmaps[i] = temp.Clone(new Rectangle(minleft, mintop, maxright - minleft, maxbottom - mintop), PixelFormat.DontCare);
            }
            return GIFbitmaps;
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

        private void SaveButtonClick(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialogue = new SaveFileDialog();
            saveFileDialogue.ShowDialog();
            if (saveFileDialogue.FileName != "")
            {
                if (!File.Exists(saveFileDialogue.FileName))
                {
                    var file = File.Create(saveFileDialogue.FileName);
                    file.Dispose();
                }
                File.WriteAllText(saveFileDialogue.FileName, GetFileData());
            }
        }
        public string GetFileData()
        {
            //File printed in this format:
            /*
             * ModuleName
             * {
             * TransformOptions,
             * Period,
             * Amplitude,
             * Offset,
             * [(point1.x, point1.y),(point2.x,point2.y)]
             * };
             */
            string result = "";
            foreach (var menuitem in menu.menuItems)
            {
                result += string.Format("{0}{{{1},{2},{3},{4},[{5}]}};",
                    menuitem.polygonlabel.Text,
                    menuitem.stretchType.ToString(),
                    menuitem.period,
                    menuitem.amplitude,
                    menuitem.offset,
                    menuitem.polygonpoints.IterateString());
            }

            return result;
        }
        public void ParseFileData(string filepath)
        {
            //File printed in this format:
            /*
             * ModuleName
             * {
             * TransformOptions,
             * Period,
             * Amplitude,
             * Offset,
             * [(point1.x, point1.y),(point2.x,point2.y)]
             * };
             */
            string filedata = File.ReadAllText(filepath);
            foreach (var module in filedata.Split(';'))
            {
                if (module == "")
                {
                    continue;
                }
                string name = module.Split('{')[0];
                string data = module.Substring(name.Length + 1);
                data = data.Substring(0, data.Length - 1); //Remove the curly brackets
                string[] datas = data.Split(",");
                StretchType stretchType = Enum.Parse<StretchType>(datas[0]);
                int period;
                int.TryParse(datas[1], out period);
                double amplitude;
                double.TryParse(datas[2], out amplitude);
                double offset;
                double.TryParse(datas[3], out offset);

                string pointdata = data.Substring(datas[0].Length + datas[1].Length + datas[2].Length + datas[3].Length + 5);
                pointdata = pointdata.Substring(0, pointdata.Length - 2);
                string[] points = pointdata.Split("),(");
                List<PointF> polygonpoints = new List<PointF>();
                foreach (var pointstr in points)
                {
                    string[] point = pointstr.Replace("(", "").Replace(")", "").Split(',');

                    float.TryParse(point[0], out float x);
                    float.TryParse(point[1], out float y);

                    polygonpoints.Add(new PointF(x, y));
                }

                var menuitem = new PolygonMenuItem(menu, Repaint);
                menuitem.polygonlabel.Text = name;
                menuitem.dropdown.SelectedIndex = (int)stretchType;
                menuitem.period = period;
                menuitem.periodTextbox.Text = period.ToString();
                menuitem.amplitude = amplitude;
                menuitem.amplitudeTextbox.Text = amplitude.ToString();
                menuitem.offset = offset;
                menuitem.offsetTextbox.Text = offset.ToString();
                menuitem.polygonpoints = polygonpoints;

                polygonMenu.ScrollControlIntoView(addPolygonButton);
            }
            pictureBox1.Invalidate();
        }

        private void ImportSettingsButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.ShowDialog();
            if (openFileDialog.FileName != "" && File.Exists(openFileDialog.FileName))
            {
                ParseFileData(openFileDialog.FileName);
            }
        }
    }
}