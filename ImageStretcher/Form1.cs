namespace ImageStretcher
{
    public partial class Form1 : Form
    {
        Bitmap asteroidimage;
        PointF[] colliderpoints = new PointF[26] { new PointF(36, 141), new PointF(16, 147), new PointF(5, 135), new PointF(0, 119), new PointF(4, 106), new PointF(6, 90), new PointF(7, 77), new PointF(15, 65), new PointF(26, 54), new PointF(34, 42), new PointF(44, 32), new PointF(57, 22), new PointF(71, 13), new PointF(85, 7), new PointF(101, 3), new PointF(116, 0), new PointF(136, 1), new PointF(150, 5), new PointF(165, 19), new PointF(155, 37), new PointF(161, 57), new PointF(147, 77), new PointF(141, 97), new PointF(124, 115), new PointF(104, 126), new PointF(86, 143), };
        public Form1()
        {
            InitializeComponent();
            asteroidimage = (Bitmap)pictureBox1.Image.Clone();
            pictureBox1.Image = null;
            pictureBox1.Invalidate();
        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            Bitmap temp = new Bitmap(asteroidimage.Width, asteroidimage.Height);
            Graphics tempgraphics = Graphics.FromImage(temp);
            //tempgraphics.DrawPolygon(new Pen(Color.Orange), colliderpoints);

            List<PointF> adjustedpoints = new List<PointF>();
            foreach (PointF p in colliderpoints)
            {
                PointF toadd = new PointF(p.X, p.Y);
                if (p.X > asteroidimage.Width/2)
                {
                    toadd.X -= 10;
                }
                adjustedpoints.Add(toadd);
            }
            tempgraphics.DrawPolygon(new Pen(Color.Red), adjustedpoints.ToArray());

            var b = ImageDeformer.DeformImageToPolygon(asteroidimage, colliderpoints, adjustedpoints.ToArray());
            e.Graphics.DrawImage(b, 0, 0, pictureBox1.Width, pictureBox1.Height);
            e.Graphics.DrawImage(temp, 0, 0, pictureBox1.Width, pictureBox1.Height);
        }

        private void pictureBox2_Paint(object sender, PaintEventArgs e)
        {
            Bitmap temp = new Bitmap(asteroidimage.Width, asteroidimage.Height);
            Graphics tempgraphics = Graphics.FromImage(temp);
            tempgraphics.DrawPolygon(new Pen(Color.Orange), colliderpoints);
            e.Graphics.DrawImage(asteroidimage, 0, 0, pictureBox1.Width, pictureBox1.Height);
            e.Graphics.DrawImage(temp, 0, 0, pictureBox1.Width, pictureBox1.Height);
        }
    }
}