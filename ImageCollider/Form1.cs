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

        bool repaintrequired = false;
        bool autogenerate = true;
        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            if (!repaintrequired)
            {
                return;
            }
            repaintrequired = false;
            if (image != null)
            {
                e.Graphics.DrawImage(image, 0, 0, pictureBox1.Width - 20, pictureBox1.Height - 20);
            }
            // return;
            if (autogenerate)
            {
                AutoGenerateImage(e.Graphics);
            }
            else
            {
                ManualGenerateImage(e.Graphics);
            }
        }
        class Vertex
        {
            public PointF position;
            public int index;

            public Vertex(PointF position, int index)
            {
                this.position = position;
                this.index = index;
            }
        }
        List<Vertex> userdefinedpoints = new List<Vertex>();
        bool customindices;
        private void ManualGenerateImage(Graphics graphics)
        {
            if (image == null)
            {
                return;
            }

            const int pointsize = 10;
            List<PointF> polygon = new List<PointF>();
            if (userdefinedpoints.Count >= 3 && !customindices)
            {
                GrahamsAlgorithm(userdefinedpoints.OrderBy(p => p.position.Y).First().position, userdefinedpoints.Select(u => u.position).ToList(), ref polygon);
                graphics.DrawPolygon(new Pen(Color.Orange), polygon.ToArray());
            }
            if (userdefinedpoints.Count == 2)
            {
                graphics.DrawLine(new Pen(Color.Orange), userdefinedpoints[0].position, userdefinedpoints[1].position);
            }

            if (customindices && polygon.Count >= 3) //Order as set by the user
            {
                foreach (PointF p in userdefinedpoints.OrderBy(v => v.index).Where(v => v.index != -1).Select(v => v.position))
                {
                    polygon.Add(p); //Add the point at its index
                }
                graphics.DrawPolygon(new Pen(Color.Orange), polygon.ToArray());
            }

            foreach (Vertex v in userdefinedpoints)
            {
                PointF p = v.position;
                if (showmarkers)
                {
                    graphics.FillEllipse(new Pen(Color.Blue).Brush, p.X - (pointsize / 2), p.Y - (pointsize / 2), pointsize, pointsize);
                    if (polygon.Contains(p))
                    {
                        var index = polygon.IndexOf(v.position);
                        if (customindices)
                        {
                            index = v.index;
                        }
                        graphics.DrawString(index.ToString(), new Font("Arial", 10), new Pen(Color.Black).Brush, p.X + 15, p.Y);
                    }
                }
            }

            //We have a list of points scaled to the current screen, now descale them to the client size
            float xscale = (float)image.Width / (float)pictureBox1.Width;
            float yscale = (float)image.Height / (float)pictureBox1.Height;


            string data = string.Format("{0}[] colliderpoints = new {0}[{1}]{{", comboBox1.SelectedItem, polygon.Count);
            foreach (PointF p in polygon)
            {
                PointF adjustedpoint = new PointF((float)(Math.Round(xscale * p.X) + centre.X), (float)(Math.Round(yscale * p.Y) + centre.Y));
                data += string.Format("new {0}({1},{2}),", comboBox1.SelectedItem, adjustedpoint.X, adjustedpoint.Y);
            }
            data += "};";
            textBox1.Text = data;
        }

        private void AutoGenerateImage(Graphics g)
        {
            if (image == null)
            {
                return;
            }

            float largestxscale = (float)pictureBox1.Width / (float)image.Width;
            float largestyscale = (float)pictureBox1.Height / (float)image.Height;

            float finalscale = (float)Math.Min(largestyscale, largestxscale);

            float new_width = finalscale * image.Width;
            float new_height = finalscale * image.Height;

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
            if (points.Count() == 0)
            {
                return;
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
            userdefinedpoints.Clear();
            for (int i = 0; i < highdefpoints.Count; i++)
            {
                PointF p = highdefpoints[i];
                if (i % resolution == 0) //Incrementally remove points to decrease resolution
                {
                    PointF adjustedpoint = new PointF(centre.X + p.X, centre.Y + p.Y);

                    polygonpoints.Add(p);
                    data += string.Format("new {0}({1},{2}),", comboBox1.SelectedItem, adjustedpoint.X, adjustedpoint.Y);
                    userdefinedpoints.Add(new Vertex(p, 0));
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

            g.DrawImage(result, 0, 0, pictureBox1.Width - 20, pictureBox1.Height - 20);
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
            centre = new PointF(x, y);


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
            double newresolution = 1;
            while (nearbypoints.Count() == 0)
            {
                nearbypoints = points.Where(p => p.DistanceTo(last) < newresolution && !movedpoints.Contains(p)).ToList();
                newresolution++;
                if (newresolution >= pictureBox1.Width)
                {
                    return;
                }
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
            if (result.Count() > points.Count() * 0.6f && last.DistanceTo(points[0]) < points.Min(p => p != points[0] ? p.DistanceTo(points[0]) : double.MaxValue))
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
        
        int selectedvertex = -1;
        private void pictureBox1_Click(object sender, EventArgs e)
        {
            if (autogenerate)
            {
                return;
            }

            //Add manual mouseclick positions to the points
            var mpos = pictureBox1.PointToClient(Cursor.Position);

            if (ModifierKeys == Keys.Shift) //Deleting points
            {
                Vertex closestpoint = userdefinedpoints.Where(u => u.position.DistanceTo(mpos) < 10).OrderBy(p => p.position.DistanceTo(mpos)).FirstOrDefault();
                if (closestpoint != null)
                {
                    userdefinedpoints.Remove((Vertex)closestpoint);
                    //Find all indexes above the closest points index and decrease it
                    for (int i = 0; i < userdefinedpoints.Count; ++i)
                    {
                        if (userdefinedpoints[i].index >= closestpoint.index)
                        {
                            userdefinedpoints[i] = new Vertex(userdefinedpoints[i].position, userdefinedpoints[i].index - 1);
                        }
                    }
                }
            }
            else if (ModifierKeys == Keys.Control && customindices)
            {
                Vertex closestpoint = userdefinedpoints.Where(u => u.position.DistanceTo(mpos) < 10).OrderBy(p => p.position.DistanceTo(mpos)).FirstOrDefault();
                if (closestpoint != null)
                {
                    //potentially change the index
                    textBox4.Text = ((Vertex)closestpoint).index.ToString();
                    selectedvertex = userdefinedpoints.IndexOf(((Vertex)(closestpoint)));
                    panel1.Visible = true;
                }
            }
            else
            {
                userdefinedpoints.Add(new Vertex(mpos, userdefinedpoints.Count));
            }
            repaintrequired = true;
            pictureBox1.Invalidate();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            customindices = checkBox1.Checked;
            if (customindices && !autogenerate && userdefinedpoints.Count() != 0)
            {
                List<PointF> orderedpoints = new List<PointF>();
                GrahamsAlgorithm(userdefinedpoints.OrderBy(p => p.position.Y).First().position, userdefinedpoints.Select(u => u.position).ToList(), ref orderedpoints);
                //Find the correct indices of each point and update the userdefinedpoints
                for (int i = 0; i < userdefinedpoints.Count; ++i)
                {
                    int index = userdefinedpoints[i].index;

                    if (index >= orderedpoints.Count || index == -1) //unselected vertice with an index out of the range
                    {
                        userdefinedpoints[i] = new Vertex(userdefinedpoints[index].position, -1); //deselct the point
                    }
                    else
                    {
                        //Swap it out for the new one
                        userdefinedpoints[i] = new Vertex(orderedpoints[i], index);
                    }
                }
            }
            repaintrequired = true;
            pictureBox1.Invalidate();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (panel1.Visible && selectedvertex != -1)
            {
                int newidx;
                if (int.TryParse(textBox4.Text, out newidx))
                {
                    if (userdefinedpoints.Any(u => u.index == newidx)) //overwriting?
                    {
                        //Increase the index of all other values
                        for (int i = 0; i < userdefinedpoints.Count; ++i)
                        {
                            Vertex v = userdefinedpoints[i];
                            if (v.index >= newidx && i != selectedvertex)
                            {
                                userdefinedpoints[i] = new Vertex(userdefinedpoints[i].position, v.index + 1);
                            }
                        }
                    }
                    //Get all the values infront of the moved point, and decrease the index
                    for (int i = 0; i < userdefinedpoints.Count; ++i)
                    {
                        if (userdefinedpoints[i].index >= userdefinedpoints[selectedvertex].index)
                        {
                            userdefinedpoints[i] = new Vertex(userdefinedpoints[i].position, userdefinedpoints[i].index - 1);
                        }
                    }

                    userdefinedpoints[selectedvertex] = new Vertex(userdefinedpoints[selectedvertex].position, newidx);
                    panel1.Visible = false;
                    selectedvertex = -1;
                    repaintrequired = true;
                    pictureBox1.Invalidate();
                }
                else
                {
                    MessageBox.Show("Invalid value given");
                }
            }
        }
        bool showmarkers = true;

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            showmarkers = checkBox2.Checked;
            repaintrequired = true;
            pictureBox1.Invalidate();
        }

        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            repaintrequired = true;
            autogenerate = !checkBox3.Checked;
            checkBox2.Checked = checkBox3.Checked;

            pictureBox1.Invalidate();
        }
    }
}