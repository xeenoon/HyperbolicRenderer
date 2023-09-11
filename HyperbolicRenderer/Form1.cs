using System.Data.SqlTypes;
using System.Diagnostics;
using System.Net.Http.Headers;
using System.Windows.Forms;

namespace HyperbolicRenderer
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            asteroidBitmap = (Bitmap)(pictureBox1.Image.Clone());
            pictureBox1.Image = null;
        }
        int sides = 4;
        float scale = 0.77f;
        Bitmap asteroidBitmap;

        Map m;
        int mapradius;

        PointF[] colliderpoints = new PointF[110] { new PointF(66, 146), new PointF(61, 146), new PointF(58, 144), new PointF(54, 143), new PointF(50, 141), new PointF(47, 139), new PointF(42, 140), new PointF(37, 141), new PointF(32, 143), new PointF(27, 146), new PointF(22, 148), new PointF(17, 148), new PointF(14, 145), new PointF(11, 143), new PointF(8, 139), new PointF(6, 136), new PointF(3, 132), new PointF(1, 129), new PointF(0, 125), new PointF(0, 120), new PointF(0, 115), new PointF(2, 113), new PointF(4, 109), new PointF(4, 107), new PointF(4, 102), new PointF(4, 97), new PointF(6, 95), new PointF(6, 91), new PointF(6, 89), new PointF(5, 85), new PointF(6, 82), new PointF(7, 79), new PointF(8, 76), new PointF(10, 72), new PointF(12, 69), new PointF(14, 66), new PointF(17, 63), new PointF(21, 59), new PointF(24, 57), new PointF(26, 53), new PointF(28, 51), new PointF(29, 48), new PointF(32, 45), new PointF(34, 41), new PointF(36, 40), new PointF(39, 37), new PointF(41, 34), new PointF(44, 31), new PointF(48, 28), new PointF(50, 27), new PointF(53, 25), new PointF(57, 21), new PointF(60, 19), new PointF(63, 18), new PointF(67, 15), new PointF(70, 14), new PointF(73, 13), new PointF(77, 10), new PointF(80, 9), new PointF(84, 8), new PointF(88, 7), new PointF(92, 5), new PointF(96, 4), new PointF(100, 3), new PointF(104, 2), new PointF(108, 1), new PointF(112, 1), new PointF(115, 0), new PointF(120, 0), new PointF(125, 0), new PointF(130, 0), new PointF(135, 0), new PointF(138, 1), new PointF(142, 2), new PointF(145, 2), new PointF(149, 4), new PointF(152, 5), new PointF(156, 8), new PointF(161, 13), new PointF(165, 18), new PointF(163, 23), new PointF(158, 26), new PointF(155, 31), new PointF(155, 36), new PointF(155, 41), new PointF(159, 46), new PointF(161, 51), new PointF(161, 56), new PointF(160, 61), new PointF(157, 66), new PointF(152, 71), new PointF(148, 76), new PointF(145, 81), new PointF(144, 86), new PointF(143, 91), new PointF(142, 96), new PointF(139, 101), new PointF(134, 105), new PointF(129, 109), new PointF(125, 114), new PointF(120, 117), new PointF(115, 118), new PointF(110, 123), new PointF(105, 126), new PointF(100, 128), new PointF(96, 132), new PointF(92, 137), new PointF(87, 142), new PointF(82, 145), new PointF(77, 145), };
        ImageDeformer imageDeformer;
        PointF fixedoffset = new PointF(0, 0);
        private void button1_Click(object sender, EventArgs e)
        {
            fixedoffset = new PointF(pictureBox1.Width/2, pictureBox1.Width/2);
            int.TryParse(textBox1.Text, out sides);
            float.TryParse(textBox2.Text, out scale);
            int inputsize;
            int.TryParse(textBox4.Text, out inputsize);
            Map.extracells = inputsize;
            mapradius = pictureBox1.Height / 2;
            if (sides == -1 || scale == -1)
            {
                return;
            }
            xchange = 0;
            ychange = 0;
            firstdraw = true;
            m = new Map(sides, mapradius, fixedoffset);
            m.shapes.Add(new Shape(colliderpoints, new PointF(88, 74)));
            imageDeformer = new ImageDeformer((Bitmap)asteroidBitmap.Clone());
            m.GenerateVolume(scale, xchange, ychange, infinitemovement);
            m.BakeHeights(10);

            timeScalar = new TimeScalar(new PointF(asteroidBitmap.Width / 2 + xchange, asteroidBitmap.Height / 2 + ychange));

            pictureBox1.Refresh();
        }
        TimeScalar timeScalar;
        bool firstdraw = true;
        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            Stopwatch s = new Stopwatch();
            s.Start();

            Bitmap tempimage = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            if (imageDeformer != null)
            {
                imageDeformer.DeformImageToPolygon(AdjustFunc, new Point((int)xchange, (int)ychange), tempimage);
            }
            e.Graphics.DrawImage(tempimage, 0, 0);
            s.Stop();
            return;
            //Graphics graphics = e.Graphics;
            Graphics graphics = Graphics.FromImage(tempimage);
            if (m == null)
            {
                return;
            }

            if (firstdraw)
            {
                xchange = -m.squaresize * (Map.extracells / 2);
                ychange = -m.squaresize * (Map.extracells / 2);
                firstdraw = false;
            }

            double gentime = 0;
            m.GenerateVolume(scale, xchange, ychange, infinitemovement);
            s.Stop();

            gentime += s.ElapsedTicks;

            double finaldraw;
            s.Restart();

            if (sides == -1)
            {
                return;
            }
            graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            if (showdebugdata && showgrid)
            {
                for (int y = 0; y < m.volumewidth; ++y)
                {
                    graphics.DrawLine(new Pen(Color.Orange), new PointF(0, y * m.squaresize), new PointF(mapradius * 2, y * m.squaresize));
                }
                for (int x = 0; x < m.volumewidth; ++x)
                {
                    graphics.DrawLine(new Pen(Color.Orange), new PointF(x * m.squaresize, 0), new PointF(x * m.squaresize, mapradius * 2));
                }
            }

            graphics.FillPolygon(new Pen(Color.DarkBlue).Brush, m.points);

            for (int i = 0; i < m.volume.Count; i++)
            {
                Trapezium trapezium = m.volume[i];
                //Scale with a red to green to blue gradient
                double scalingfactor = ((((i % Math.Sqrt(m.volume.Count)) * (i / (float)Math.Sqrt(m.volume.Count))) / (float)(m.volume.Count)));

                if (scalingfactor < 0 || scalingfactor > 1)
                {
                    continue;
                }

                double red = 0;
                double green = 0;
                double blue = 0;
                if (scalingfactor < 0.33f)
                {
                    blue = 255 - ((scalingfactor) * 255);
                    red = scalingfactor * 3 * 255;
                }
                else if (scalingfactor < 0.66f)
                {
                    red = 255 - ((scalingfactor - 0.33f) * 3 * 255);
                    green = (scalingfactor - 0.33f) * 3 * 255;
                }
                else
                {
                    green = 255 - ((scalingfactor - 0.66f) * 3 * 255);
                    blue = (scalingfactor - 0.66f) * 3 * 255;
                }


                Color result = Color.FromArgb((int)red, (int)green, (int)blue);


                if (showdebugdata && straighlines)
                {
                    trapezium.Draw(graphics, false, Color.White, m, false);
                }
                else if (showdebugdata && !showbackground)
                {
                    trapezium.Draw(graphics, true, result, m);
                    trapezium.Draw(graphics, true, Color.White, m, false);
                }
                else
                {
                    trapezium.Draw(graphics, true, Color.White, m, false);
                }
            }

            foreach (var shape in m.adjustedshapes)
            {
                //Draw the image onto the shape
                //   graphics.DrawPolygon(new Pen(Color.Red), shape.points);
                //
                //   PointF[] oldpoints = new PointF[shape.points.Length];
                //   for (int i = 0; i < colliderpoints.Length; ++i) 
                //   {
                //       oldpoints[i] = new PointF(xchange + colliderpoints[i].X, ychange + colliderpoints[i].Y);
                //   }
                //   graphics.DrawPolygon(new Pen(Color.Orange), oldpoints);
                //shape.Draw(graphics, Color.Brown, m);
            }

            if (showdebugdata)
            {
                for (int i = 0; i < m.oldconnections.Length; i++)
                {
                    PointF connection = m.connections[i];
                    PointF oldconnection = m.oldconnections[i];
                    if (showpointmovement)
                    {
                        graphics.DrawLine(new Pen(Color.Green, 2), m.connections[i], m.oldconnections[i]);
                    }
                    if (showclosestedge)
                    {
                        graphics.DrawLine(new Pen(Color.Magenta, 1), m.sideconnections[i].start, m.sideconnections[i].end);
                        graphics.FillEllipse(new Pen(Color.Magenta).Brush, m.sideconnections[i].end.X - 2, m.sideconnections[i].end.Y - 2, 4, 4);
                    }
                    if (showmodifiedpoints)
                    {
                        graphics.FillEllipse(new Pen(Color.Red).Brush, connection.X - 2, connection.Y - 2, 4, 4);
                    }
                    if (showgridpoints)
                    {
                        graphics.FillEllipse(new Pen(Color.Orange).Brush, oldconnection.X - 2, oldconnection.Y - 2, 4, 4);
                    }
                }
                foreach (var point in m.debugpoints)
                {
                    graphics.FillEllipse(new Pen(Color.SaddleBrown).Brush, point.X - 2, point.Y - 2, 4, 4);
                }
            }

            if (!(showdebugdata && !showbackground))
            {

                using (var path = new System.Drawing.Drawing2D.GraphicsPath())
                {
                    path.AddPolygon(m.points);

                    // Uncomment this to invert:
                    path.AddRectangle(new Rectangle(0, 0, pictureBox1.Width, pictureBox1.Height));

                    using (var brush = new SolidBrush(Color.Black))
                    {
                        graphics.FillPath(brush, path);
                    }
                }
            }
            e.Graphics.DrawImage(tempimage, new PointF(0, 0));

            s.Stop();
            finaldraw = s.ElapsedMilliseconds;

            longestgen = Math.Max(longestgen, gentime);
            longestdraw = Math.Max(longestdraw, Trapezium.elapseddrawtime);
            longesttrig = Math.Max(longesttrig, Trapezium.elapsedtrigtime);
            longestfinaldraw = Math.Max(longestfinaldraw, finaldraw);


            label7.Text = "DEBUG INFO: \nTrig: " + Trapezium.elapsedtrigtime.ToString() + " ticks\nDraw: " + Trapezium.elapseddrawtime.ToString() + " ticks\nGen: " + gentime.ToString() + " ticks\nFinaldraw: " + finaldraw.ToString() +
                " ms\n\nMaxtrig: " + longesttrig + " ticks\nMaxdraw: " + longestdraw + " ticks\nMaxgen: " + longestgen + " ticks\nMaxfinaldraw: " + longestfinaldraw.ToString() + " ms";

            Trapezium.elapseddrawtime = 0;
            Trapezium.elapsedtrigtime = 0;

        }
        Point AdjustFunc(Point input)
        {
            float offsetx = xchange + fixedoffset.X;
            float offsety = ychange + fixedoffset.Y;

            PointF newinput = new PointF(input.X + offsetx, input.Y + offsety);
            PointF output = m.StretchPoint(newinput);

            output.X -= fixedoffset.X;
            output.Y -= fixedoffset.Y;

            return new Point((int)output.X, (int)output.Y);
        }
        static double longesttrig = 0;
        static double longestdraw = 0;
        static double longestgen = 0;
        static double longestfinaldraw = 0;

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
            label7.Visible = checkBox1.Checked;

            showdebugdata = checkBox1.Checked;
            pictureBox1.Invalidate();
        }
        bool showgridpoints = false;
        bool showgrid = false;
        bool showmodifiedpoints = false;
        bool showpointmovement = false;
        bool showclosestedge = false;
        bool straighlines = false;
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
        float xchange = 0;
        float ychange = 0;
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
                case Keys.ShiftKey:
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
            int yadd = 0;
            int xadd = 0;
            foreach (var c in keys)
            {
                switch (c)
                {
                    case 'w':
                        if (shift)
                        {
                            yadd = -3;
                        }
                        else
                        {
                            yadd = -1;
                        }
                        break;
                    case 'a':
                        if (shift)
                        {
                            xadd = -3;
                        }
                        else
                        {
                            xadd = -1;
                        }
                        break;
                    case 's':
                        if (shift)
                        {
                            yadd = 3;
                        }
                        else
                        {
                            yadd = 1;
                        }
                        break;
                    case 'd':
                        if (shift)
                        {
                            xadd = 3;
                        }
                        else
                        {
                            xadd = 1;
                        }
                        break;
                }
            }
            if (m == null)
            {
                return;
            }
            if (m.points.Count() % 2 == 0 || !invertodd)
            {
                xchange += xadd * speedmodifier;
                ychange += yadd * speedmodifier;
            }
            else
            {
                xchange += xadd * speedmodifier;
                ychange -= yadd * speedmodifier;
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
            if (e.KeyCode == Keys.ShiftKey)
            {
                shift = false;
                return;
            }

            keydown = false;
            keys = "";
        }
        bool infinitemovement;
        private void checkBox9_CheckedChanged(object sender, EventArgs e)
        {
            infinitemovement = checkBox9.Checked;
            pictureBox1.Invalidate();
        }
        float speedmodifier = 1;

        private void button2_Click(object sender, EventArgs e)
        {
            float.TryParse(textBox3.Text, out speedmodifier);
            pictureBox1.Invalidate();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }


        bool invertodd = false;
        private void checkBox10_CheckedChanged(object sender, EventArgs e)
        {
            invertodd = checkBox10.Checked;
        }

        private void checkBox1_CheckedChanged_1(object sender, EventArgs e)
        {

        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            pictureBox1.Width = Height;
            pictureBox1.Height = Height;
        }
    }
}