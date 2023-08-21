using System.Diagnostics;
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
            int inputsize;
            int.TryParse(textBox4.Text, out inputsize);
            Map.extracells = inputsize + 4;
            if (sides == -1 || scale == -1)
            {
                return;
            }
            xchange = 0;
            ychange = 0;
            firstdraw = true;
            m = new Map(sides, pictureBox1.Width);
            m.GenerateVolume(scale, xchange, ychange, infinitemovement);
            m.BakeHeights(10);

            MessageBox.Show(totaldata);
            pictureBox1.Invalidate();
        }
        bool firstdraw = true;
        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            Bitmap tempimage = new Bitmap(pictureBox1.Width*2, pictureBox1.Height*2);
            Graphics tempgraphics = Graphics.FromImage(tempimage);
            if (m == null)
            {
                return;
            }
            Stopwatch s = new Stopwatch();
            s.Start();
            
            if (firstdraw)
            {
                xchange = -m.squaresize * (Map.extracells / 2);
                ychange = -m.squaresize * (Map.extracells / 2);
                firstdraw = false;
            }

            double gentime = 0;
            m.GenerateShape();
            m.GenerateVolume(scale, xchange, ychange, infinitemovement);

            s.Stop();

            gentime += s.ElapsedTicks;

            double finaldraw;
            s.Restart();

            if (sides == -1)
            {
                return;
            }
            tempgraphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            if (showdebugdata && showgrid)
            {
                for (int y = 0; y < m.volumewidth; ++y)
                {
                    tempgraphics.DrawLine(new Pen(Color.Orange), new PointF(0, y * m.squaresize), new PointF(pictureBox1.Width * 2, y * m.squaresize));
                }
                for (int x = 0; x < m.volumewidth; ++x)
                {
                    tempgraphics.DrawLine(new Pen(Color.Orange), new PointF(x * m.squaresize, 0), new PointF(x * m.squaresize, pictureBox1.Width * 2));
                }
            }

            tempgraphics.FillPolygon(new Pen(Color.DarkBlue).Brush, m.points);

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
                    trapezium.Draw(tempgraphics, false, Color.White, m, false);
                }
                else if (showdebugdata && !showbackground)
                {
                    trapezium.Draw(tempgraphics, true, result, m);
                    trapezium.Draw(tempgraphics, true, Color.White, m, false);
                }
                else
                {
                    //trapezium.Draw(e.Graphics, true, result, m);
                    trapezium.Draw(tempgraphics, true, Color.White, m, false);
                }
                //e.Graphics.DrawPolygon(new Pen(Color.White), new PointF[4] { trapezium.top_left, trapezium.bottom_left, trapezium.bottom_right, trapezium.top_right });
            }

            foreach (var shape in m.adjustedshapes)
            {
                shape.Draw(tempgraphics, Color.Brown, m);
            }

            if (showdebugdata)
            {
                for (int i = 0; i < m.oldconnections.Length; i++)
                {
                    PointF connection = m.connections[i];
                    PointF oldconnection = m.oldconnections[i];
                    if (showpointmovement)
                    {
                        tempgraphics.DrawLine(new Pen(Color.Green, 2), m.connections[i], m.oldconnections[i]);
                    }
                    if (showclosestedge)
                    {
                        tempgraphics.DrawLine(new Pen(Color.Magenta, 1), m.sideconnections[i].start, m.sideconnections[i].end);
                        tempgraphics.FillEllipse(new Pen(Color.Magenta).Brush, m.sideconnections[i].end.X - 2, m.sideconnections[i].end.Y - 2, 4, 4);
                    }
                    if (showmodifiedpoints)
                    {
                        tempgraphics.FillEllipse(new Pen(Color.Red).Brush, connection.X - 2, connection.Y - 2, 4, 4);
                    }
                    if (showgridpoints)
                    {
                        tempgraphics.FillEllipse(new Pen(Color.Orange).Brush, oldconnection.X - 2, oldconnection.Y - 2, 4, 4);
                    }
                }
                foreach (var point in m.debugpoints)
                {
                    tempgraphics.FillEllipse(new Pen(Color.SaddleBrown).Brush, point.X - 2, point.Y - 2, 4, 4);
                }
            }

            if (!(showdebugdata && !showbackground))
            {

                using (var path = new System.Drawing.Drawing2D.GraphicsPath())
                {
                    path.AddPolygon(m.points);

                    // Uncomment this to invert:
                    path.AddRectangle(new Rectangle(0,0,pictureBox1.Width*2, pictureBox1.Height*2));

                    using (var brush = new SolidBrush(Color.Black))
                    {
                        tempgraphics.FillPath(brush, path);
                    }
                }
            }
            e.Graphics.DrawImage(tempimage, new PointF(-pictureBox1.Width / 2, -pictureBox1.Width / 2));
            //e.Graphics.DrawImage(tempimage, new Rectangle(0, 0, pictureBox1.Width, pictureBox1.Height));

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