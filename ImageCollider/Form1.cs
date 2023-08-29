namespace ImageCollider
{
    public partial class Form1 : Form
    {
        Bitmap image;
        float resolution = 10;

        public Form1()
        {
            InitializeComponent();
        }

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
                    image = temp.Clone(new Rectangle(0,0,temp.Width, temp.Height), System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                    pictureBox1.Invalidate();
                }
            }
        }

        public List<RectangleF> imageAreas = new List<RectangleF>();

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            imageAreas.Clear();
            if (image == null)
            {
                return;
            }

            float largestxscale = (float)pictureBox1.Width / (float)image.Width;
            float largestyscale = (float)pictureBox1.Height / (float)image.Height;
          
            float finalscale = (float)Math.Min(largestyscale, largestxscale);
          
            float new_width = finalscale * image.Width;
            float new_height = finalscale * image.Height;

            //e.Graphics.DrawImage(image, 0, 0, new_width, new_height);


            float cellsize = Math.Min(image.Width, image.Height) / resolution;

            for (int x = 0; x < image.Width / cellsize; ++x)
            {
                for (int y = 0; y < image.Height / cellsize; ++y)
                {

                    imageAreas.Add(new RectangleF(x * cellsize, y * cellsize, cellsize, cellsize));
                }
            }

            Bitmap result = new Bitmap(image.Width, image.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            using (BMP writebmp = new BMP(result))
            {
                using (BMP lookup = new BMP(image))
                {
                    foreach (var area in imageAreas)
                    {
                        for (float x = area.Left; x < area.Right; ++x)
                        {
                            for (float y = area.Top; y < area.Bottom; ++y)
                            {
                                if (x >= image.Width || y >= image.Height)
                                {
                                    continue;
                                }
                                Color color = lookup.GetPixel((int)x, (int)y);
                                if (color.A > 0 && color.A < 255)
                                {
                                    writebmp.SetPixel((int)x, (int)y, Color.Red);
                                }
                            }
                        }
                    }
                }
            }

            e.Graphics.DrawImage(result, 0, 0, new_width, new_height);

        }

        private void Generate_Click(object sender, EventArgs e)
        {
            float.TryParse(textBox2.Text, out resolution);
            pictureBox1.Invalidate();
        }
    }
}