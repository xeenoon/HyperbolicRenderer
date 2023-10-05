using System.Diagnostics;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.Cryptography.Pkcs;
using System.Security.Policy;
using AnimatedGif;

namespace ImageStretcher
{
    public partial class AnimationEditor : Form
    {
        PointTransformer scalar;
        FrameCollection framecollection;
        PolygonMenu menu;
        public float zoom = 1;
        public int newwidth
        {
            get
            {
                return (int)((1f / zoom) * canvas.Width);
            }
        }
        public int newheight
        {
            get
            {
                return (int)((1f / zoom) * canvas.Width);
            }
        }
        public AnimationEditor()
        {
            InitializeComponent();
            image = (Bitmap)canvas.Image.Clone();
            canvas.Image = null;
            canvas.Invalidate();
            menu = new PolygonMenu(polygonMenu, addPolygonButton);
            scalar = new PointTransformer(new PointF(image.Width / 2, image.Height / 2), image.Width, menu);

            framecollection = new FrameCollection(frameViewer, this);

            frames = new Bitmap[1];
            frames[0] = image;
            framecollection.GenerateFrames(frames);
            Resize += framecollection.Resize;
            this.WindowState = FormWindowState.Maximized;
            startstopButton.Invalidate();
        }
        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            int newwidth;
            int newheight;
            if (playanimation)
            {
                newwidth = (int)(displayimage.Width * zoom);
                newheight = (int)(displayimage.Height * zoom);

                e.Graphics.DrawImage(displayimage, animationoffset.X + offset.X, animationoffset.Y + offset.Y, newwidth, newheight);
            }
            else
            {
                var image = framecollection.selectedframe.frameimg;

                newwidth = (int)(image.Width * zoom);
                newheight = (int)(image.Height * zoom);

                if (framecollection.selectedframe != framecollection.master)
                {
                    e.Graphics.DrawImage(image, animationoffset.X + offset.X, animationoffset.Y + offset.Y, newwidth, newheight);
                }
                else
                {
                    e.Graphics.DrawImage(image, offset.X, offset.Y, newwidth, newheight);
                }
            }
            if (erasing)
            {
                e.Graphics.DrawRectangle(new Pen(Color.Black),
                    eraserlocation.X - erasersize, eraserlocation.Y - erasersize, erasersize, erasersize);
            }
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
        }
        Bitmap image
        {
            get
            {
                return img;
            }
            set
            {
                asyncimage = new Bitmap(value);
                img = value;
            }
        }
        Bitmap img;
        Bitmap asyncimage; //For use on other threads, is a copy of image

        private void ImportImage(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.ShowDialog(this);
            string name = ofd.FileName;
            if (File.Exists(name))
            {
                string extension = name.Split(".")[1];
                if (extension == "png" || extension == "jpg" || extension == "jpeg")
                {
                    animating = false;
                    var temp = (Bitmap)Image.FromFile(name);
                    image = temp.Clone(new Rectangle(0, 0, temp.Width, temp.Height), System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                    scalar = new PointTransformer(new PointF(image.Width / 2, image.Height / 2), image.Width, menu);

                    Bitmap[] originalimage = new Bitmap[1];
                    originalimage[0] = image;
                    framecollection.GenerateFrames(originalimage);

                    offset = new Point(canvas.Width / 2 - image.Width / 2, canvas.Height / 2 - image.Height / 2);
                    offsetTextbox.Text = string.Format("{0},{1}", offset.X, offset.Y);

                    canvas.Invalidate();
                }
            }
        }
        LoadingBar bar = new LoadingBar();
        private void Generate(object sender, EventArgs e)
        {
            //Benchmark(null, null);
            playanimation = false;
            startstopButton.Refresh(); //Force update with .Refresh instead of .Invalidate
            framecollection.selectedframe = framecollection.master; //Reset animation
            frames = new Bitmap[1];
            frames[0] = image;
            bar.percentloaded = 0;
            loadingpanel.Visible = true;
            Refresh();//Force update to display label
            Task.Run(() =>
            {
                frames = frames.Concat(GetFrames()).ToArray();
                loadingpanel.Invoke(() =>
                {
                    framecollection.GenerateFrames(frames);
                    frames = frames.TakeLast(frames.Count() - 1).ToArray(); //remove first frame
                    MessageBox.Show("Finished animating");
                    loadingpanel.Visible = false;
                });
            });
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
            canvas.Invalidate();
        }
        private void SmootheFrames(object sender, EventArgs e)
        {
            for (int i = 0; i < frames.Length; i++)
            {
                Bitmap? frame = frames[i];
                //Smoothe the frames
                frames[i] = ApplyCustomAntiAliasing(frame);
            }
            for (int i = 0; i < framecollection.frames.Count; i++)
            {
                Frame? box = framecollection.frames[i];
                box.preview.Image = frames[i];
                box.frameimg = frames[i];
                box.preview.Invalidate();
            }
        }
        static Bitmap ApplyCustomAntiAliasing(Bitmap inputImage)
        {
            int width = inputImage.Width;
            int height = inputImage.Height;

            // Create an output bitmap with the same resolution as the input
            Bitmap outputImage = new Bitmap(width, height);

            // Lock the bits of the input and output bitmaps
            BitmapData inputData = inputImage.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            BitmapData outputData = outputImage.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);

            unsafe
            {
                byte* inputPtr = (byte*)inputData.Scan0.ToPointer();
                byte* outputPtr = (byte*)outputData.Scan0.ToPointer();

                int stride = inputData.Stride;

                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        inputPtr = (byte*)inputData.Scan0 + y * stride + x * 4;
                        outputPtr = (byte*)outputData.Scan0 + y * stride + x * 4;

                        // Initialize color accumulators
                        int totalRed = 0;
                        int totalGreen = 0;
                        int totalBlue = 0;
                        int pixelCount = 0;
                        const int blur = 1;
                        for (int dx = -blur; dx <= blur; dx++)
                        {
                            for (int dy = -blur; dy <= blur; dy++)
                            {
                                int newX = x + dx;
                                int newY = y + dy;

                                // Check bounds to avoid out-of-range pixels
                                if (newX >= 0 && newX < width && newY >= 0 && newY < height)
                                {
                                    byte* neighborPixel = inputPtr + dy * stride + dx * 4;

                                    // Accumulate color values
                                    totalBlue += neighborPixel[0];
                                    totalGreen += neighborPixel[1];
                                    totalRed += neighborPixel[2];

                                    // Increment the pixel count
                                    pixelCount++;
                                }
                            }
                        }

                        // Calculate the averaged color
                        outputPtr[0] = (byte)(totalBlue / pixelCount);
                        outputPtr[1] = (byte)(totalGreen / pixelCount);
                        outputPtr[2] = (byte)(totalRed / pixelCount);
                        outputPtr[3] = inputPtr[3]; // Alpha channel stays same
                    }
                }
            }

            // Unlock the bits
            inputImage.UnlockBits(inputData);
            outputImage.UnlockBits(outputData);

            return outputImage;
        }
        private void Benchmark(object sender, EventArgs e)
        {
            Stopwatch s = new Stopwatch();
            s.Start();
            Bitmap result = new Bitmap(canvas.Width, canvas.Height);
            for (int i = 0; i < 1000; ++i)
            {
                scalar.time = 1; //Test value
                ImageDeformer.DeformImageToPolygon(scalar.TransformPoint, new Point(0, 0), image, result, menu.menuItems.Select(m => m.polygonpoints.ToArray()).ToList());
            }
            s.Stop();
            MessageBox.Show("Did 1000 operations, averaging: " + (s.ElapsedMilliseconds / 1000f).ToString() + "ms per frame");
        }
        private void ExportGIF(object sender, EventArgs e)
        {
            playanimation = false;
            startstopButton.Invalidate();

            PointTransformer scalar = new PointTransformer(new PointF(image.Width / 2, image.Height / 2), image.Width, menu, false);

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
                using (var gif = AnimatedGif.AnimatedGif.Create(saveFileDialogue.FileName, 16))
                {
                    foreach (var bmp in frames)
                    {
                        gif.AddFrame(bmp, delay = 33, GifQuality.Default);

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
        Point animationoffset;
        DeformData deformdata;
        public Bitmap[] GetFrames()
        {
            if (menu.menuItems.Count == 0)
            {
                return new Bitmap[0];
            }
            int frames = (int)((menu.menuItems.Max(m => m.period) * 4) / (Math.PI * 2)) * 31;

            Bitmap[] GIFbitmaps = new Bitmap[frames];
            Bitmap[] tempbitmaps = new Bitmap[(int)frames];
            int minleft = int.MaxValue;
            int mintop = int.MaxValue;
            int maxright = int.MinValue;
            int maxbottom = int.MinValue;
            List<Bitmap> readcopies = new List<Bitmap>(); //Allow image data to be readable from multiple threads
            int frame_i = 0;
            while (frame_i < frames)
            {
                try
                {
                    readcopies.Add(new Bitmap(asyncimage)); //Avoids concurrent access errors
                    ++frame_i;
                }
                catch { } //Try again
            }

            Parallel.For(0, frames, i =>
            {
                PointTransformer newscalar = new PointTransformer(scalar.centre, scalar.width, scalar.polygonMenu);
                newscalar.time = (i + 1) * ((2 * Math.PI) / (31.4f));
                tempbitmaps[i] = new Bitmap(canvas.Width, canvas.Height);
                var data = ImageDeformer.DeformImageToPolygon(newscalar.TransformPoint, new Point(offset.X, offset.Y), readcopies[i], tempbitmaps[i], menu.menuItems.Select(m => m.polygonpoints.ToArray()).ToList(), true);
                readcopies[i].Dispose();
                bar.percentloaded += (1f / frames);
                Invoke(() => loadingbar.Refresh());
                minleft = Math.Max(Math.Min(minleft, data.left), 0);
                mintop = Math.Max(Math.Min(mintop, data.top), 0);
                maxright = Math.Min(Math.Max(maxright, data.right), canvas.Width);
                maxbottom = Math.Min(Math.Max(maxbottom, data.bottom), canvas.Height);
            });
            this.deformdata = new DeformData();
            deformdata.left = minleft;
            deformdata.right = maxright;
            deformdata.top = mintop;
            deformdata.bottom = maxbottom;

            animationoffset = new Point(minleft - offset.X, mintop - offset.Y);
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
            canvas.Invalidate();
            return false;
        }
        private void CanvasClick(object sender, EventArgs e)
        {
            if (erasing && framecollection.selectedframe != framecollection.master) //Dont allow editing of base image
            {
                Rectangle erasedarea = new Rectangle(eraserlocation.X - erasersize - deformdata.left,
                                                     eraserlocation.Y - erasersize - deformdata.top,
                                                     erasersize, erasersize);
                Graphics g = Graphics.FromImage(framecollection.selectedframe.preview.Image);
                g.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceCopy;
                g.FillRectangle(new Pen(Color.Transparent).Brush, erasedarea);
                g.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceOver;
            }
            else if (menu.selecteditem != null)
            {
                Point clickpos = canvas.PointToClient(Cursor.Position);
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
                canvas.Invalidate();
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
            canvas.Invalidate();
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

        private void AnimationEditor_Resize(object sender, EventArgs e)
        {
            offset = new Point(canvas.Width / 2 - image.Width / 2, canvas.Height / 2 - image.Height / 2);
            offsetTextbox.Text = string.Format("{0},{1}", offset.X, offset.Y);

            canvas.Invalidate();
        }

        bool playanimation = false;
        private void startstopButton_Click(object sender, EventArgs e)
        {
            playanimation = !playanimation;
            startstopButton.Invalidate();
        }

        private void startstopButton_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            e.Graphics.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            e.Graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;

            if (playanimation)
            {

                e.Graphics.DrawImage(Resources.stopicon, 0, 0, startstopButton.Width, startstopButton.Height);
                if (frames != null)
                {
                    firstanimationframe = true;
                    Animate();
                }
            }
            else
            {
                e.Graphics.DrawImage(Resources.playicon, 0, 0, startstopButton.Width, startstopButton.Height);
                if (animationtimer != null)
                {
                    animationtimer.Stop();
                }
            }
        }
        System.Timers.Timer animationtimer;
        bool firstanimationframe = true;
        int animationframeidx = 0;
        Bitmap displayimage;
        public void Animate(object sender = null, System.Timers.ElapsedEventArgs e = null)
        {
            if (firstanimationframe)
            {
                animationtimer = new System.Timers.Timer(16);
                animationtimer.Elapsed += Animate;
                firstanimationframe = false;
                animationtimer.Start();
            }
            animationframeidx++;
            if (animationframeidx >= frames.Count())
            {
                animationframeidx -= frames.Count();
            }
            try
            {
                displayimage = frames[animationframeidx];
            }
            catch { } //Fix multithreading issues
            canvas.Invalidate();
        }

        private void ZoomInClick(object sender, EventArgs e)
        {
            zoom += 0.1f;
            canvas.Invalidate();
        }

        private void zoomoutButton_Click(object sender, EventArgs e)
        {
            zoom -= 0.1f;
            canvas.Invalidate();
        }

        private void loadingbar_Paint(object sender, PaintEventArgs e)
        {
            bar.Draw(e.Graphics, loadingbar.Width, loadingbar.Height, 0);
        }

        bool erasing = false;
        private void EraserClick(object sender, EventArgs e)
        {
            if (erasing)
            {
                eraserToolboxItem.BackColor = Color.Transparent;
            }
            else
            {
                eraserToolboxItem.BackColor = Color.Gray;
            }
            erasing = !erasing;
            canvas.Invalidate();
        }
        Point eraserlocation = new Point(0, 0);
        int erasersize = 2;
        private void canvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (erasing)
            {
                eraserlocation = canvas.PointToClient(Cursor.Position);
                canvas.Invalidate();
            }
        }

        private void AnimationEditor_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Oemplus)
            {
                erasersize++;
                canvas.Invalidate();
            }
            if (e.KeyCode == Keys.OemMinus)
            {
                erasersize--;
                canvas.Invalidate();
            }
        }
    }
}