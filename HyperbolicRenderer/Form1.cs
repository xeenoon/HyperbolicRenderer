using System.Windows.Forms;

namespace HyperbolicRenderer
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        int sides = -1;
        int size = -1;

        Map m;

        private void button1_Click(object sender, EventArgs e)
        {
            int.TryParse(textBox1.Text, out sides);
            int.TryParse(textBox2.Text, out size);
            if (sides == -1 || size == -1)
            {
                return;
            }
            m = new Map(sides, pictureBox1.Width / 2f);
            m.GenerateShape();
            m.GenerateVolume(0.775f);
            pictureBox1.Invalidate();
        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            int size = pictureBox1.Width;
            if (sides == -1)
            {
                return;
            }

            e.Graphics.FillPolygon(new Pen(Color.DarkBlue).Brush, m.points);
            foreach (var trapezium in m.volume)
            {
                e.Graphics.DrawPolygon(new Pen(Color.White), new PointF[4] { trapezium.top_left, trapezium.bottom_left, trapezium.bottom_right, trapezium.top_right });
            }
            foreach(var connection in m.connections)
            {
                //e.Graphics.FillEllipse(new Pen(Color.Red).Brush, connection.X-2, connection.Y-2, 4, 4);
            }

            using (var path = new System.Drawing.Drawing2D.GraphicsPath())
            {
                path.AddPolygon(m.points);

                // Uncomment this to invert:
                 path.AddRectangle(pictureBox1.ClientRectangle);

                using (var brush = new SolidBrush(Color.Black))
                {
                    e.Graphics.FillPath(brush, path);
                }
            }
        }
    }
    public class Trapezium
    {
        public PointF top_left;
        public PointF bottom_left;
        public PointF top_right;
        public PointF bottom_right;

        public Trapezium(PointF top_left, PointF bottom_left, PointF top_right, PointF bottom_right)
        {
            this.top_left = top_left;
            this.bottom_left = bottom_left;
            this.top_right = top_right;
            this.bottom_right = bottom_right;
        }
    }
    public class Map
    {
        public PointF[] points;
        public List<Trapezium> volume = new List<Trapezium>();
        public float radius;
        public PointF[] connections;

        public Map(int points, float radius)
        {
            this.points = new PointF[points];
            this.radius = radius;
        }
        public void GenerateShape()
        {
            double radiansstepsize = Math.Tau / points.Length;
            for (int i = 0; i < points.Length; ++i)
            {
                //Draw a ray from the centre until it hits the edge of the square
                //Make this a vector
                double angle = 3 * (Math.PI / 2) - (radiansstepsize * i);

                float x = (float)(Math.Cos(angle) * radius + radius);
                float y = (float)(Math.Sin(angle) * radius + radius);
                points[i] = new PointF(x, y);
            }
        }
        public void GenerateVolume(float scale) //Scale of 1 will have squares of 20% size, 0.5 = 10% size...
        {
            float squaresize = radius * 0.2f * scale;
            /*volume.Add(new Trapezium(new PointF(radius - squaresize / 2f, radius - squaresize / 2f),
                                     new PointF(radius - squaresize / 2f, radius + squaresize / 2f),
                                     new PointF(radius + squaresize / 2f, radius - squaresize / 2f),
                                     new PointF(radius + squaresize / 2f, radius + squaresize / 2f)
                                     ));*/

            int volumewidth = (int)Math.Ceiling((radius * 2) / squaresize) + 1;
            connections = new PointF[(volumewidth) * (volumewidth)];
            float relativeradius = (radius / squaresize);

            for (int y = 0; y < (volumewidth); ++y)
            {
                for (int x = 0; x < (volumewidth); ++x)
                {
                    float x_scale = float.MaxValue;
                    float y_scale = float.MaxValue;
                    if (x==7)
                    {

                    }
                    foreach (var point in points)
                    {
                        //Use herons formula to find the area of the bounded area of the triangle
                        //basevector = point
                        PointF a = new PointF((x*squaresize) - (radius), (y*squaresize) - (radius)); //From 0,0 to the current point
                        PointF b = new PointF((point.X) - ((x*squaresize)),(point.Y) - ((y*squaresize))); //From point to the current point

                        //Area = sqrt(s(s - a)(s - b)(s - c))
                        //Where s = perimetre / 2

                        float mag_a = (float)Math.Sqrt(a.X*a.X + a.Y*a.Y);
                        float mag_c = radius;
                        float mag_b = (float)Math.Sqrt(b.X*b.X + b.Y*b.Y);

                        float s = (mag_c + mag_a + mag_b)/2;

                        float area = (float)Math.Sqrt(s * (s-mag_a) * (s-mag_b) * (s-mag_c));

                        //From the area the height can be determined as:
                        //a = 1/2 * b * h
                        //h = 2a / b

                        float height = 2 * area / mag_c;
                        //The height is the shortest distance and therefore the scaling factor
                        if (mag_b < radius / 2)
                        {
                            height = mag_b;
                        }
                        else
                        {
                            height = (float)Math.Sqrt(mag_b * mag_b - (radius / 2) * (radius / 2));
                        }
                        //if (height > radius * 0.75f)
                        //{
                        //    height = radius * 0.75f;
                        //}
                        if (a.X > 0)
                        {
                            if (height < Math.Abs(x_scale))
                            {
                                x_scale = -height;
                            }
                        }
                        else
                        {
                            if (height < Math.Abs(x_scale))
                            {
                                x_scale = height;
                            }
                        }
                        if (a.Y > 0)
                        {
                            if (height < Math.Abs(y_scale))
                            {
                                y_scale = -height;
                            }
                        }
                        else
                        {
                            if (height < Math.Abs(y_scale))
                            {
                                y_scale = height;
                            }
                        }
                    }
                    if (x_scale >= 100)
                    {

                    }
                    x_scale = (float)Math.Tanh((x_scale) / 200)/2f;
                    y_scale = (float)Math.Tanh((y_scale) / 200)/2f;
                    if (x_scale >= 1)
                    {

                    }
                    float ay = y;
                    float ax = x;
                    ay += y_scale;
                    ax += x_scale;
                    if (ay == float.NegativeInfinity)
                    {

                    }
                    connections[x + y * (volumewidth)] = new PointF(ax * squaresize, ay * squaresize);
                }
            }

            for (int x = 0; x < volumewidth - 1; x++)
            {
                for (int y = 0; y < volumewidth - 1; y++)
                {
                    int ay = y * volumewidth; //Adjusted y
                    volume.Add(new Trapezium(connections[x + ay], connections[x + (ay + volumewidth)], connections[(x + 1) + ay], connections[(x + 1) + (ay + volumewidth)]));
                }
            }
        }
    }
}