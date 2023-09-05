using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ImageStretcher
{
    internal class ImageDeformer
    {
        public Bitmap originalimage;
        public PointF[] polygonPoints;
        public Size drawsize;

        public ImageDeformer(Bitmap originalimage, PointF[] polygonPoints, Size drawsize)
        {
            this.originalimage = originalimage;
            this.polygonPoints = polygonPoints;
            this.drawsize = drawsize;
        }

        public Bitmap DeformImageToPolygon(Func<PointF, PointF> DeformFunction, PointF[] newpolygon)
        {
            int width = originalimage.Width;
            int height = originalimage.Height;

            Bitmap resultBitmap = new Bitmap(originalimage.Width, originalimage.Height);

            //const int offset = 5;
            //Rectangle drawarea = new Rectangle(drawsize.Width / (2 * offset), drawsize.Width / (2 * offset), drawsize.Width * (offset - 1) / offset, drawsize.Height * (offset - 1) / offset);
            const int resolution = 10;
            // Lock the source bitmap for faster pixel access
            using (BMP inputdata = new BMP(originalimage))
            {
                using (BMP outputdata = new BMP(resultBitmap))
                {
                    //Stopwatch s = new Stopwatch();
                    //s.Start();
                    //for (int i = 0; i < 100; ++i)
                    //{
                    for (int y = 0; y < height; y+=10)
                    {
                        for (int x = 0; x < width; x+=10)
                        {
                            PointF blockcentre = new PointF(x, y);
                            Color oldcolor = inputdata.GetPixel(x, y);
                            // Calculate the displacement for this pixel based on its distance from the polygon edges
                            PointF newtransform = DeformFunction(blockcentre);
                            var newtransformX = (int)(newtransform.X - blockcentre.X);
                            var newtransformY = (int)(newtransform.Y - blockcentre.Y);

                            // Ensure the new position is within bounds
                            newtransform = new PointF(Math.Max(0, Math.Min(newtransform.X, width - 1)), Math.Max(0, Math.Min(newtransform.Y, height - 1)));

                            //Map the new point to the drawsize
                            for (int newx = x-5; newx < x+5; ++newx)
                            {
                                for (int newy = y - 5; newy < y + 5; ++newy)
                                {
                                    if (newx >= originalimage.Width || newx < 0 || newy >= originalimage.Height || newy < 0)
                                    {
                                        continue;
                                    }
                                    outputdata.SetPixel(newx + newtransformX, newy + newtransformY, oldcolor);
                                }
                            }
                        }
                    }
                    //}
                    //s.Stop();
                    //var elapsed = s.ElapsedMilliseconds;
                }
            }

            return resultBitmap;
        }

        public static Bitmap RemoveWhitePixelsAndSmooth(Bitmap image, int maxPixelDistance)
        {
            Bitmap result = new Bitmap(image);
            using (var resultbmp = new BMP(result)) 
            {
                for (int x = 0; x < image.Width; x++)
                {
                    for (int y = 0; y < image.Height; y++)
                    {
                        Color pixelColor = image.GetPixel(x, y);

                        if (IsWhite(pixelColor))
                        {
                            Color averageColor = CalculateAverageColor(image, x, y, maxPixelDistance);
                            resultbmp.SetPixel(x, y, averageColor);
                        }
                    }
                }
            }
            return result;
        }

        private static bool IsWhite(Color color)
        {
            return color.R == 255 && color.G == 255 && color.B == 255;
        }

        private static Color CalculateAverageColor(Bitmap image, int x, int y, int maxPixelDistance)
        {
            int totalR = 0, totalG = 0, totalB = 0;
            int count = 0;

            for (int xOffset = -maxPixelDistance; xOffset <= maxPixelDistance; xOffset++)
            {
                for (int yOffset = -maxPixelDistance; yOffset <= maxPixelDistance; yOffset++)
                {
                    int newX = x + xOffset;
                    int newY = y + yOffset;

                    if (newX >= 0 && newX < image.Width && newY >= 0 && newY < image.Height)
                    {
                        Color neighborColor = image.GetPixel(newX, newY);
                        if (!IsWhite(neighborColor))
                        {
                            totalR += neighborColor.R;
                            totalG += neighborColor.G;
                            totalB += neighborColor.B;
                            count++;
                        }
                    }
                }
            }

            if (count > 0)
            {
                int avgR = totalR / count;
                int avgG = totalG / count;
                int avgB = totalB / count;
                return Color.FromArgb(avgR, avgG, avgB);
            }
            else
            {
                // If there are no nearby non-white pixels, return a fallback color (e.g., black)
                return Color.Black;
            }
        }
    }
}
