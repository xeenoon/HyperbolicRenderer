using System.Windows.Forms;

namespace HyperbolicRenderer
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        int sides = 4;
        float scale = 0.77f;

        Map m;

        private void button1_Click(object sender, EventArgs e)
        {
            int.TryParse(textBox1.Text, out sides);
            float.TryParse(textBox2.Text, out scale);
            if (sides == -1 || scale == -1)
            {
                return;
            }
            xchange = 0;
            ychange = 0;
            pictureBox1.Invalidate();
        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            m = new Map(sides, pictureBox1.Width / 2f);
            m.GenerateShape();
            m.GenerateVolume(scale, xchange, ychange);

            int size = pictureBox1.Width;
            if (sides == -1)
            {
                return;
            }
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            if (showdebugdata && showgrid)
            {
                for (int y = 0; y < m.volumewidth; ++y)
                {
                    e.Graphics.DrawLine(new Pen(Color.Orange), new PointF(0, y * m.squaresize), new PointF(pictureBox1.Width, y * m.squaresize));
                }
                for (int x = 0; x < m.volumewidth; ++x)
                {
                    e.Graphics.DrawLine(new Pen(Color.Orange), new PointF(x * m.squaresize, 0), new PointF(x * m.squaresize, pictureBox1.Width));
                }
            }
            e.Graphics.FillPolygon(new Pen(Color.DarkBlue).Brush, m.points);
            foreach (var trapezium in m.volume)
            {
                if (showdebugdata && straighlines)
                {
                    trapezium.Draw(e.Graphics, false, Color.Black, 1);
                }
                else if (showdebugdata && !showbackground)
                {
                    trapezium.Draw(e.Graphics, true, Color.Black, 1);
                }
                else
                {
                    trapezium.Draw(e.Graphics, true, Color.White, 1);
                }
                //e.Graphics.DrawPolygon(new Pen(Color.White), new PointF[4] { trapezium.top_left, trapezium.bottom_left, trapezium.bottom_right, trapezium.top_right });
            }
            for (int i = 0; i < m.oldconnections.Length; i++)
            {
                if (showdebugdata)
                {
                    PointF connection = m.connections[i];
                    PointF oldconnection = m.oldconnections[i];
                    if (showpointmovement)
                    {
                        e.Graphics.DrawLine(new Pen(Color.Green, 2), m.connections[i], m.oldconnections[i]);
                    }
                    if (showclosestedge)
                    {
                        e.Graphics.DrawLine(new Pen(Color.Magenta, 1), m.sideconnections[i].start, m.sideconnections[i].end);
                        e.Graphics.FillEllipse(new Pen(Color.Magenta).Brush, m.sideconnections[i].end.X - 2, m.sideconnections[i].end.Y - 2, 4, 4);
                    }
                    if (showmodifiedpoints)
                    {
                        e.Graphics.FillEllipse(new Pen(Color.Red).Brush, connection.X - 2, connection.Y - 2, 4, 4);
                    }
                    if (showgridpoints)
                    {
                        e.Graphics.FillEllipse(new Pen(Color.Orange).Brush, oldconnection.X - 2, oldconnection.Y - 2, 4, 4);
                    }
                }
            }

            if (showdebugdata && !showbackground)
            {
                return;
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
        bool showdebugdata;
        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            checkBox2.Visible = checkBox1.Checked;
            checkBox3.Visible = checkBox1.Checked;
            checkBox4.Visible = checkBox1.Checked;
            checkBox5.Visible = checkBox1.Checked;
            checkBox6.Visible = checkBox1.Checked;
            checkBox7.Visible = checkBox1.Checked;
            checkBox8.Visible = checkBox1.Checked;

            showdebugdata = checkBox1.Checked;
            pictureBox1.Invalidate();
        }
        bool showgridpoints = true;
        bool showgrid = true;
        bool showmodifiedpoints = true;
        bool showpointmovement = true;
        bool showclosestedge = true;
        bool straighlines = true;
        bool showbackground = true;

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            showgridpoints = checkBox2.Checked;
            pictureBox1.Invalidate();
        }

        private void checkBox7_CheckedChanged(object sender, EventArgs e)
        {
            showgrid = checkBox7.Checked;
            pictureBox1.Invalidate();
        }

        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            showmodifiedpoints = checkBox3.Checked;
            pictureBox1.Invalidate();
        }

        private void checkBox4_CheckedChanged(object sender, EventArgs e)
        {
            showpointmovement = checkBox4.Checked;
            pictureBox1.Invalidate();
        }

        private void checkBox5_CheckedChanged(object sender, EventArgs e)
        {
            showclosestedge = checkBox5.Checked;
            pictureBox1.Invalidate();
        }

        private void checkBox6_CheckedChanged(object sender, EventArgs e)
        {
            straighlines = checkBox6.Checked;
            pictureBox1.Invalidate();
        }

        private void checkBox8_CheckedChanged(object sender, EventArgs e)
        {
            showbackground = checkBox8.Checked;
            pictureBox1.Invalidate();
        }
        bool keydown = false;
        string keys = ""; //Change to flags enum later
        bool shift = false;
        System.Timers.Timer movetimer = new System.Timers.Timer();
        bool runningtimer = false;
        float xchange = 1;
        float ychange = 1;
        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            keydown = true;
            switch (e.KeyCode)
            {
                case Keys.W:
                    if (!keys.Contains("w"))
                    {
                        keys += "w";
                    }
                    break;
                case Keys.A:
                    if (!keys.Contains("a"))
                    {
                        keys += "a";
                    }
                    break;
                case Keys.S:
                    if (!keys.Contains("s"))
                    {
                        keys += "s";
                    }
                    break;
                case Keys.D:
                    if (!keys.Contains("d"))
                    {
                        keys += "d";
                    }
                    break;
                case Keys.Shift:
                    shift = true;
                    break;
            }
            if (!runningtimer)
            {
                runningtimer = true;
                movetimer = new System.Timers.Timer(100);
                movetimer.AutoReset = false;
                movetimer.Elapsed += new System.Timers.ElapsedEventHandler(MovetimeTick);
                movetimer.Start();
            }
        }
        private void MovetimeTick(object sender, System.Timers.ElapsedEventArgs e)
        {
            foreach (var c in keys)
            {
                switch (c)
                {
                    case 'w':
                        if (shift)
                        {
                            ychange -= 3;
                        }
                        else
                        {
                            ychange--;
                        }
                        break;
                    case 'a':
                        if (shift)
                        {
                            xchange -= 3;
                        }
                        else
                        {
                            xchange--;
                        }
                        break;
                    case 's':
                        if (shift)
                        {
                            ychange += 3;
                        }
                        else
                        {
                            ychange++;
                        }
                        break;
                    case 'd':
                        if (shift)
                        {
                            xchange += 3;
                        }
                        else
                        {
                            xchange++;
                        }
                        break;
                }
            }
            pictureBox1.Invalidate();
            if (keydown)
            {
                movetimer.Start();
            }
            else
            {
                runningtimer = false;
            }
        }
        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            shift = false;
            keydown = false;
            keys = "";
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

        internal void Draw(Graphics graphics, bool curved, Color color, int thickness)
        {
            Pen pen = new Pen(color, thickness);
            graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;


            float modifier;
            if (top_right.Y > 200)
            {
                modifier = 0.505f;
            }
            else
            {
                modifier = 0.495f;
            }
            if (curved)
            {
                graphics.DrawCurve(pen, new PointF[3] { top_left, new PointF((top_left.X + top_right.X) / 2, (top_left.Y + top_right.Y) * modifier), top_right });
            }
            else
            {
                graphics.DrawCurve(pen, new PointF[2] { top_left, top_right });
            }
            if (top_right.X > 200)
            {
                modifier = 0.505f;
            }
            else
            {
                modifier = 0.495f;
            }
            if (curved)
            {
                graphics.DrawCurve(pen, new PointF[3] { top_right, new PointF((top_right.X + bottom_right.X) * modifier, (bottom_right.Y + top_right.Y) / 2), bottom_right });
            }
            else
            {
                graphics.DrawCurve(pen, new PointF[2] { top_right, bottom_right });
            }

        }
    }
    public struct Line
    {
        public PointF start;
        public PointF end;

        public Line(PointF start, PointF end)
        {
            this.start = start;
            this.end = end;
        }
    }
    public class Map
    {
        public PointF[] points;
        public List<Trapezium> volume = new List<Trapezium>();
        public float radius;
        public PointF[] connections;
        public PointF[] oldconnections;
        public Line[] sideconnections;

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
                double angle = (3 * (Math.PI / 2)) + (radiansstepsize * i);

                float x = (float)(Math.Cos(angle) * radius + radius);

                float y = (float)(Math.Sin(angle) * radius + radius);
                points[i] = new PointF(x, y);
            }
        }
        public int volumewidth = 0;
        public float squaresize = 0;
        public void GenerateVolume(float scale, float offsetx, float offsety) //Scale of 1 will have squares of 20% size, 0.5 = 10% size...
        {
            squaresize = radius * 0.2f * scale;

            volumewidth = (int)Math.Ceiling((radius * 2) / squaresize) + 1;
            connections = new PointF[(volumewidth) * (volumewidth)];
            oldconnections = new PointF[(volumewidth) * (volumewidth)]; //debugdata
            sideconnections = new Line[(volumewidth) * (volumewidth)]; //debugdata
            volume.Clear();

            List<Line> shapelines = new List<Line>();
            for (int i = 0; i < points.Count(); i++)
            {
                shapelines.Add(new Line(points[i], points[i + 1 >= points.Count() ? 0 : i + 1]));
            }

            for (int y = 0; y < (volumewidth); ++y)
            {
                for (int x = 0; x < (volumewidth); ++x)
                {
                    PointF relativepoint = new PointF(x * squaresize + offsetx, y * squaresize + offsety);

                    float x_scale = float.MaxValue;
                    float y_scale = float.MaxValue;
                    float y_scalemodifier = 1;

                    Line closestline = new Line();

                    foreach (var line in shapelines)
                    {
                        PointF distance = relativepoint.DistanceTo(line);

                        if (distance.Y == 0)
                        {
                            y_scalemodifier = 0.5f;
                        }

                        if (Math.Abs(distance.Y) < Math.Abs(y_scale) && distance.Y != 0)
                        {
                            y_scale = distance.Y;
                            closestline = line;
                        }
                        if (Math.Abs(distance.X) < Math.Abs(x_scale))
                        {
                            x_scale = distance.X;
                            closestline = line;
                        }
                    }

                    if (!relativepoint.InPolygon(points))
                    {

                        Vector linevector = new Vector(closestline.start, closestline.end);
                        Vector perpindicular = linevector.GetPerpindicular().GetUnitVector();
                        perpindicular = new Vector(relativepoint, new PointF((float)(perpindicular.i + relativepoint.X), (float)(perpindicular.j + relativepoint.Y)));

                        PointF intersection = linevector.Intersection(perpindicular);

                        //connections[x + y * (volumewidth)] = intersection;
                        //continue;
                    }

                    y_scale *= y_scalemodifier;
                    sideconnections[x + y * volumewidth] = new Line(relativepoint, new PointF(x_scale + relativepoint.X, y_scale + relativepoint.Y)); //debugdata
                    
                    x_scale = (float)Math.Sin(x_scale / 20) / 2;
                    y_scale = (float)Math.Sin(y_scale / 20) / 2;
                    const float limiter = 0.45f;
                    if (x_scale >= limiter)
                    {
                        x_scale = limiter;
                    }
                    if (x_scale <= -limiter)
                    {
                        x_scale = -limiter;
                    }
                    if (y_scale >= limiter)
                    {
                        y_scale = limiter;
                    }
                    if (y_scale <= -limiter)
                    {
                        y_scale = -limiter;
                    }
                    if (Math.Abs(x * squaresize + offsetx - radius) < squaresize && Math.Abs(y * squaresize + offsety - radius) < squaresize)
                    {
                        x_scale /= 2;
                        y_scale /= 2;
                    }
                    float ay=y;
                    float ax=x;
                    if (offsety != 0) //Removes infinity errors
                    {
                        ay += (offsety / squaresize);
                    }
                    if(offsetx != 0) //Removes infinity errors
                    { 
                        ax += (offsetx / squaresize);
                    }
                    ay += y_scale;
                    ax += x_scale;

                    connections[x + y * (volumewidth)] = new PointF(ax * squaresize, ay * squaresize);
                    oldconnections[x + y * (volumewidth)] = relativepoint;
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