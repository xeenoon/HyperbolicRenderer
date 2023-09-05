using System.Diagnostics;

namespace ImageStretcher
{
    public partial class Form1 : Form
    {
        Bitmap asteroidimage;
        PointF[] colliderpoints = new PointF[110] { new PointF(66, 146), new PointF(61, 146), new PointF(58, 144), new PointF(54, 143), new PointF(50, 141), new PointF(47, 139), new PointF(42, 140), new PointF(37, 141), new PointF(32, 143), new PointF(27, 146), new PointF(22, 148), new PointF(17, 148), new PointF(14, 145), new PointF(11, 143), new PointF(8, 139), new PointF(6, 136), new PointF(3, 132), new PointF(1, 129), new PointF(0, 125), new PointF(0, 120), new PointF(0, 115), new PointF(2, 113), new PointF(4, 109), new PointF(4, 107), new PointF(4, 102), new PointF(4, 97), new PointF(6, 95), new PointF(6, 91), new PointF(6, 89), new PointF(5, 85), new PointF(6, 82), new PointF(7, 79), new PointF(8, 76), new PointF(10, 72), new PointF(12, 69), new PointF(14, 66), new PointF(17, 63), new PointF(21, 59), new PointF(24, 57), new PointF(26, 53), new PointF(28, 51), new PointF(29, 48), new PointF(32, 45), new PointF(34, 41), new PointF(36, 40), new PointF(39, 37), new PointF(41, 34), new PointF(44, 31), new PointF(48, 28), new PointF(50, 27), new PointF(53, 25), new PointF(57, 21), new PointF(60, 19), new PointF(63, 18), new PointF(67, 15), new PointF(70, 14), new PointF(73, 13), new PointF(77, 10), new PointF(80, 9), new PointF(84, 8), new PointF(88, 7), new PointF(92, 5), new PointF(96, 4), new PointF(100, 3), new PointF(104, 2), new PointF(108, 1), new PointF(112, 1), new PointF(115, 0), new PointF(120, 0), new PointF(125, 0), new PointF(130, 0), new PointF(135, 0), new PointF(138, 1), new PointF(142, 2), new PointF(145, 2), new PointF(149, 4), new PointF(152, 5), new PointF(156, 8), new PointF(161, 13), new PointF(165, 18), new PointF(163, 23), new PointF(158, 26), new PointF(155, 31), new PointF(155, 36), new PointF(155, 41), new PointF(159, 46), new PointF(161, 51), new PointF(161, 56), new PointF(160, 61), new PointF(157, 66), new PointF(152, 71), new PointF(148, 76), new PointF(145, 81), new PointF(144, 86), new PointF(143, 91), new PointF(142, 96), new PointF(139, 101), new PointF(134, 105), new PointF(129, 109), new PointF(125, 114), new PointF(120, 117), new PointF(115, 118), new PointF(110, 123), new PointF(105, 126), new PointF(100, 128), new PointF(96, 132), new PointF(92, 137), new PointF(87, 142), new PointF(82, 145), new PointF(77, 145), };
        
        
        public Form1()
        {
            InitializeComponent();
            asteroidimage = (Bitmap)pictureBox1.Image.Clone();
            pictureBox1.Image = null;
            pictureBox1.Invalidate();
        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            LockUnlockBitsExample(e);
            return;
            Bitmap temp = new Bitmap(asteroidimage.Width, asteroidimage.Height);
            Graphics tempgraphics = Graphics.FromImage(temp);
            //tempgraphics.DrawPolygon(new Pen(Color.Orange), colliderpoints);

            List<PointF> adjustedpoints = new List<PointF>();
            foreach (PointF p in colliderpoints)
            {
                adjustedpoints.Add(DeformFunc(p));
            }
            tempgraphics.DrawPolygon(new Pen(Color.Orange), colliderpoints.ToArray());
            tempgraphics.DrawPolygon(new Pen(Color.Red), adjustedpoints.ToArray());

            Size maxsize = new Size(pictureBox1.Width, pictureBox1.Height);
            ImageDeformer imageDeformer = new ImageDeformer(asteroidimage, colliderpoints, maxsize);
            var b = imageDeformer.DeformImageToPolygon(DeformFunc, adjustedpoints.ToArray());

            const int offset = 5;
            e.Graphics.DrawImage(b, 0, 0, pictureBox1.Width, pictureBox1.Height);
            e.Graphics.DrawImage(temp, 0, 0, pictureBox1.Width, pictureBox1.Height);
        }


        private void LockUnlockBitsExample(PaintEventArgs e)
        {
            Stopwatch s = new Stopwatch();
            s.Start();
            // Create a new bitmap.
            Bitmap bmp = asteroidimage;
            Bitmap writebmp = new Bitmap(bmp.Width, bmp.Height);

            // Lock the bitmap's bits.  
            Rectangle rect = new Rectangle(0, 0, bmp.Width, bmp.Height);
            System.Drawing.Imaging.BitmapData bmpData = bmp.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            System.Drawing.Imaging.BitmapData writeData = writebmp.LockBits(rect, System.Drawing.Imaging.ImageLockMode.WriteOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            // Get the address of the first line.
            IntPtr readptr = bmpData.Scan0;
            IntPtr writeptr = writeData.Scan0;

            // Declare an array to hold the bytes of the bitmap.
            const int readlength = 10;

            for (int x = 0; x < bmp.Width*4; x+=readlength)
            {
                if ((x/readlength) % 2 == 0)
                {
                    for (int y = 0; y < (bmp.Height); ++y)
                    {

                        int finalptrlocation = ((y * bmpData.Stride) + x);

                        byte[] rgbValues = new byte[readlength]; //stepsize of readlength

                        // Copy the RGB values into the array.
                        System.Runtime.InteropServices.Marshal.Copy(readptr + finalptrlocation, rgbValues, 0, readlength);

                        // Copy the RGB values back to the bitmap
                        System.Runtime.InteropServices.Marshal.Copy(rgbValues, 0, writeptr + (finalptrlocation), readlength);
                    }
                }
            }

            // Unlock the bits.
            bmp.UnlockBits(bmpData);
            writebmp.UnlockBits(writeData);
            s.Stop();
            var elapsed = s.ElapsedMilliseconds;
            // Draw the modified image.
            e.Graphics.DrawImage(writebmp, 0, 0, pictureBox1.Width, pictureBox1.Height);
        }

        public PointF DeformFunc(PointF p)
        {
            PointF result = new PointF(p.X, p.Y);
            if (p.Y > asteroidimage.Height / 2)
            {
                result.Y -= (p.Y - asteroidimage.Height / 2) * 0.4f;
                if (p.X > asteroidimage.Width / 2)
                {
                    result.X -= (p.X - asteroidimage.Width / 2) * 0.2f;
                }
                else if (p.X < asteroidimage.Width / 2)
                {
                   // result.X -= ((asteroidimage.Width / 2) - p.X) * 0.1f;
                }
            }
            return result;
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