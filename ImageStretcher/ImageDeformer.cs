using System;
using System.Collections.Generic;
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

        public ImageDeformer(Bitmap originalimage, PointF[] polygonPoints)
        {
            this.originalimage = originalimage;
            this.polygonPoints = polygonPoints;
        }

        public Bitmap DeformImageToPolygon(PointF[] newPolygonPoints)
        {
            int width = originalimage.Width;
            int height = originalimage.Height;

            Bitmap resultBitmap = new Bitmap(width, height);

            // Lock the source bitmap for faster pixel access
            using (BMP inputdata = new BMP(originalimage)) 
            {
                using (BMP outputdata = new BMP(resultBitmap))
                {
                    for (int y = 0; y < height; y++)
                    {
                        for (int x = 0; x < width; x++)
                        {
                            PointF currentPixel = new PointF(x, y);
                            if (!currentPixel.InPolygon(polygonPoints))
                            {
                               continue;
                            }

                            Color oldcolor = inputdata.GetPixel(x, y);
                            // Calculate the displacement for this pixel based on its distance from the polygon edges
                            PointF displacement = CalculateDisplacement(currentPixel, polygonPoints, newPolygonPoints);

                            // Apply the displacement to the pixel's position with subpixel precision
                            int newX = (int)Math.Round(x + displacement.X);
                            int newY = (int)Math.Round(y + displacement.Y);

                            // Ensure the new position is within bounds
                            newX = Math.Max(0, Math.Min(newX, width - 1));
                            newY = Math.Max(0, Math.Min(newY, height - 1));

                            outputdata.SetPixel(newX, newY, oldcolor);
                        }

                    }
                }
            }

            GraphicsPath gp = new GraphicsPath();
            gp.AddPolygon(newPolygonPoints.ToArray());

            Bitmap finalresult = new Bitmap(originalimage.Width, originalimage.Height);
            using (Graphics G = Graphics.FromImage(finalresult))
            {
                G.Clip = new Region(gp);   // restrict drawing region
                G.DrawImage(resultBitmap, 0, 0);   // draw clipped
            }
            gp.Dispose();

            return finalresult;
        }
        private PointF CalculateDisplacement(PointF pixel, PointF[] oldPoints, PointF[] newPoints)
        {
            PointF displacement = new PointF(0, 0);
            double diagonaldistance = Math.Sqrt(originalimage.Height*originalimage.Height + originalimage.Width*originalimage.Width);

            for (int i = 0; i < oldPoints.Length; i++)
            {
                PointF oldPoint = oldPoints[i];
                
                PointF startpoint = oldPoints[i];
                PointF endpoint = i == oldPoints.Length-1 ? oldPoints[0] : oldPoints[i+1];
                Line line = new Line(startpoint, endpoint);

                PointF newPoint = newPoints[i];

                // Calculate the distance between the pixel and the line
                double distance = pixel.DistanceTo(line).Magnitude();

                //float magnitude = (float)Math.Sqrt(distance.X*distance.X + distance.Y*distance.Y);

                // Make the pixel's movement inversely proportional to the distance

                double linelength = oldPoint.DistanceTo(newPoint);
                
                double scalingfactor = linelength * 0.012;

                scalingfactor = Math.Min(1, scalingfactor);

                double factor = (-1 * (Math.Pow(originalimage.Width, -0.5) * Math.Sqrt(distance*2)) + 1) * 0.05f; // Adding 1 to avoid division by zero
                factor = Math.Max(0, factor);
                if (distance > diagonaldistance / 10)
                {
                    //continue;
                }
                // Calculate the displacement based on the difference between old and new positions
                float deltaX = newPoint.X - oldPoint.X;
                float deltaY = newPoint.Y - oldPoint.Y;

                displacement.X += (float)(deltaX * factor);
                displacement.Y += (float)(deltaY * factor);
            }

            return displacement;
        }

        private float CalculateDistance(PointF point1, PointF point2)
        {
            float dx = point2.X - point1.X;
            float dy = point2.Y - point1.Y;
            return (float)Math.Sqrt(dx * dx + dy * dy);
        }
    }
}
