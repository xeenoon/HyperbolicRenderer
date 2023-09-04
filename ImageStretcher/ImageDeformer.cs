using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageStretcher
{
    internal class ImageDeformer
    {
        public static Bitmap DeformImageToPolygon(Bitmap image, PointF[] polygonPoints, PointF[] newPolygonPoints)
        {
            int width = image.Width;
            int height = image.Height;

            Bitmap resultBitmap = new Bitmap(width, height);

            // Lock the source bitmap for faster pixel access
            BitmapData sourceData = image.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            BitmapData resultData = resultBitmap.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);

            int bytesPerPixel = 4; // Assuming 32bppArgb format
            int stride = sourceData.Stride;

            unsafe
            {
                byte* sourcePointer = (byte*)sourceData.Scan0;
                byte* resultPointer = (byte*)resultData.Scan0;

                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        int pixelOffset = y * stride + x * bytesPerPixel;

                        // Get the color of the current pixel in the source image
                        byte blue = sourcePointer[pixelOffset];
                        byte green = sourcePointer[pixelOffset + 1];
                        byte red = sourcePointer[pixelOffset + 2];
                        byte alpha = sourcePointer[pixelOffset + 3];

                        // Calculate the displacement for this pixel based on its distance from the polygon edges
                        PointF currentPixel = new PointF(x, y);
                        if (!currentPixel.InPolygon(polygonPoints))
                        {
                            continue;
                        }
                        PointF displacement = CalculateDisplacement(currentPixel, polygonPoints, newPolygonPoints);

                        // Apply the displacement to the pixel's position with subpixel precision
                        int newX = (int)Math.Round(x + displacement.X);
                        int newY = (int)Math.Round(y + displacement.Y);

                        // Ensure the new position is within bounds
                        newX = Math.Max(0, Math.Min(newX, width - 1));
                        newY = Math.Max(0, Math.Min(newY, height - 1));

                        // Calculate the new pixel offset
                        int newPixelOffset = newY * stride + newX * bytesPerPixel;

                        // Set the color of the pixel in the result image
                        resultPointer[newPixelOffset] = blue;
                        resultPointer[newPixelOffset + 1] = green;
                        resultPointer[newPixelOffset + 2] = red;
                        resultPointer[newPixelOffset + 3] = alpha;
                    }
                }
            }

            // Unlock the source bitmap
            image.UnlockBits(sourceData);
            resultBitmap.UnlockBits(resultData);

            return resultBitmap;
        }
        private static PointF CalculateDisplacement(PointF pixel, PointF[] oldPoints, PointF[] newPoints)
        {
            PointF displacement = new PointF(0, 0);

            for (int i = 0; i < oldPoints.Length; i++)
            {
                PointF oldPoint = oldPoints[i];
                
                PointF startpoint = oldPoints[i];
                PointF endpoint = i == oldPoints.Length-1 ? oldPoints[0] : oldPoints[i+1];
                Line line = new Line(startpoint, endpoint);

                PointF newPoint = newPoints[i];

                // Calculate the distance between the pixel and the line
                PointF distance = pixel.DistanceTo(line);

                float magnitude = (float)Math.Sqrt(distance.X*distance.X + distance.Y*distance.Y);

                // Make the pixel's movement inversely proportional to the distance
                float factor = 0.2f / (magnitude + 1); // Adding 1 to avoid division by zero

                // Calculate the displacement based on the difference between old and new positions
                float deltaX = newPoint.X - oldPoint.X;
                float deltaY = newPoint.Y - oldPoint.Y;

                displacement.X += deltaX * factor;
                displacement.Y += deltaY * factor;
            }

            return displacement;
        }

        private static float CalculateDistance(PointF point1, PointF point2)
        {
            float dx = point2.X - point1.X;
            float dy = point2.Y - point1.Y;
            return (float)Math.Sqrt(dx * dx + dy * dy);
        }
    }
}
