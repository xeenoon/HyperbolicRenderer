using System.Diagnostics.Contracts;
using System.DirectoryServices.ActiveDirectory;
using System.Drawing;
using System.Reflection;
using System.Security.Permissions;

namespace ImageCollider
{
    public partial class Form1 : Form
    {
        Bitmap image;
        float resolution = 1;
        PointF centre = new PointF(0, 0);

        public Form1()
        {
            InitializeComponent();
            comboBox1.SelectedIndex = 0;
        }

        struct PointAngle
        {
            public PointF position;
            public double angle;

            public PointAngle(PointF position, double angle)
            {
                this.position = position;
                this.angle = angle;
            }
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
                    image = temp.Clone(new Rectangle(0, 0, temp.Width, temp.Height), System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                    repaintrequired = true;
                    pictureBox1.Invalidate();
                }
            }
        }

        public List<RectangleF> imageAreas = new List<RectangleF>();
        bool repaintrequired = false;
        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            if (!repaintrequired)
            {
                return;
            }
            repaintrequired = false;
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

            List<PointAngle> points = new List<PointAngle>();
            PointF startpoint = new PointF(-1, -1);
            using (BMP lookup = new BMP(image))
            {
                for (float y = image.Height - 1; y > -1; --y)
                {
                    for (float x = 0; x < image.Width; ++x)
                    {
                        if (x >= image.Width || y >= image.Height)
                        {
                            continue;
                        }
                        Color color = lookup.GetPixel((int)x, (int)y);
                        if (color.A > 0 && color.A < 255)
                        {
                            if (startpoint.X == -1 && startpoint.Y == -1) //unset point
                            {
                                startpoint = new PointF(x, y);
                                continue;
                            }


                            double angle = GetAngle(startpoint, new PointF(x, y));

                            points.Add(new PointAngle(new PointF(x, y), angle));
                        }
                    }
                }
            }
            //Since all angles are positive, they can be easily ordered
            points = points.OrderBy(p => p.angle).ToList();

            Bitmap result = new Bitmap(image.Width, image.Height);

            Graphics graphics = Graphics.FromImage(result);
            string data = "";

            List<PointF> polygonpoints = new List<PointF>();
            List<PointF> highdefpoints = new List<PointF>();

            PointF lastpoint = startpoint;


            GrahamsAlgorithm(points[0].position, points.Select(p => p.position).ToList(), ref highdefpoints);
            highdefpoints.Reverse();
            for (int i = 0; i < highdefpoints.Count; i++)
            {
                PointF p = highdefpoints[i];
                if (i % resolution == 0) //Incrementally remove points to decrease resolution
                {
                    PointF adjustedpoint = new PointF(centre.X + p.X, centre.Y + p.Y);
                    
                    polygonpoints.Add(p);
                    data += string.Format("new {0}({1},{2}),", comboBox1.SelectedItem, adjustedpoint.X, adjustedpoint.Y);
                }
            }

            data = data.Insert(0, string.Format("{0}[] colliderpoints = new {0}[{1}]{{", comboBox1.SelectedItem, polygonpoints.Count));
            data += "};";
            if (polygonpoints.Count <= 2)
            {
                MessageBox.Show("Resolution is too low");
                return;
            }
            graphics.DrawPolygon(new Pen(Color.Orange), polygonpoints.ToArray());

            textBox1.Text = data;

            e.Graphics.DrawImage(image, 0, 0, pictureBox1.Width - 20, pictureBox1.Height - 20);
            e.Graphics.DrawImage(result, 0, 0, pictureBox1.Width - 20, pictureBox1.Height - 20);
        }

        private static double GetAngle(PointF start, PointF end)
        {
            float ychange = -(end.Y - start.Y);
            float xchange = (end.X - start.X);

            double angle = Math.Atan(Math.Abs(ychange / xchange));

            //Check each quadrant
            if (ychange >= 0 && xchange >= 0) //NE stays the same
            {

            }
            else if (ychange < 0 && xchange > 0) //SE
            {
                angle = Math.PI + angle;
            }
            else if (ychange < 0 && xchange < 0) //SW
            {
                angle = Math.PI + angle;
            }
            else if (ychange > 0 && xchange < 0) //NW
            {
                angle = Math.PI - angle;
            }

            if (ychange == 0 && xchange > 0)
            {
                angle = Math.PI / 2;
            }
            else if (ychange == 0 && xchange < 0)
            {
                angle = 3 * Math.PI / 2;
            }
            else if (xchange == 0 && ychange < 0)
            {
                angle = Math.PI;
            }

            return angle;
        }

        private void Generate_Click(object sender, EventArgs e)
        {
            float userinput = -1f;
            if (float.TryParse(textBox2.Text, out userinput))
            {
                if (userinput > 1)
                {
                    MessageBox.Show("Resolution cannot be larger than 1");
                }
                resolution = (int)(1 / (userinput));
            }

            string[] centredata = textBox3.Text.Split(',');
            if (centredata.Count() != 2)
            {
                MessageBox.Show("Invalid centre");
                return;
            }
            float x;
            float y;
            float.TryParse(centredata[0], out x);
            float.TryParse(centredata[1], out y);
            centre = new PointF(x,y);
            

            repaintrequired = true;
            pictureBox1.Invalidate();
        }
        List<PointF> movedpoints = new List<PointF>();
        public void GrahamsAlgorithm(PointF last, List<PointF> points, ref List<PointF> result)
        {
            if (result.Count() == 0)
            {
                movedpoints.Clear();
                result = new List<PointF>();
            }
            movedpoints.Add(last);
            result.Add(last);

            List<PointF> nearbypoints = new List<PointF>();
            double newresolution = 2;
            while (nearbypoints.Count() == 0)
            {
                nearbypoints = points.Where(p => p.DistanceTo(last) < newresolution && !movedpoints.Contains(p)).ToList();
                newresolution++;
            }

            double smallestangle = double.MaxValue;
            PointF closestpoint = new PointF();
            foreach (var point in nearbypoints)
            {
                double angle = GetAngle(last, point);
                if (angle < smallestangle)
                {
                    smallestangle = angle;
                    closestpoint = point;
                }
            }
            if (result.Count() > (points.Count()) * 0.6f && last.DistanceTo(points[0]) < 10)
            {
                return;
            }
            GrahamsAlgorithm(closestpoint, points, ref result);
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            repaintrequired = true;
            pictureBox1.Invalidate();
        }
    }
}