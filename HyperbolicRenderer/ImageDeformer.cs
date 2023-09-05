using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace HyperbolicRenderer
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

        public void DeformImageToPolygon(Func<PointF, PointF> DeformFunction, Point offset, Bitmap resultBitmap)
        {
            int width = originalimage.Width;
            int height = originalimage.Height;

            //Bitmap resultBitmap = new Bitmap(originalimage.Width, originalimage.Height);
            BitmapData inputData = originalimage.LockBits(new Rectangle(0, 0, originalimage.Width, originalimage.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppPArgb);
            BitmapData outputData = resultBitmap.LockBits(new Rectangle(0, 0, resultBitmap.Width, resultBitmap.Height), ImageLockMode.WriteOnly, PixelFormat.Format32bppPArgb);

            //const int offset = 5;
            //Rectangle drawarea = new Rectangle(drawsize.Width / (2 * offset), drawsize.Width / (2 * offset), drawsize.Width * (offset - 1) / offset, drawsize.Height * (offset - 1) / offset);
            const int resolution = 4;
            // Lock the source bitmap for faster pixel access

            //Stopwatch s = new Stopwatch();
            //s.Start();
            //for (int i = 0; i < 100; ++i)
            //{
            for (int y = (resolution / 2); y < height; y += resolution)
            {
                for (int x = (resolution / 2); x < width; x += resolution)
                {
                    PointF blockcentre = new PointF(x, y);
                    //Color oldcolor = inputdata.GetPixel(x, y);
                    // Calculate the displacement for this pixel based on its distance from the polygon edges
                    PointF newtransform = DeformFunction(blockcentre);

                    //If the distance between newtransform and blockcentre > resolution, scale the drawing

                    float nextxstride = Math.Abs(newtransform.X - DeformFunction(new PointF(x + resolution, y)).X);
                    float lastxstride = Math.Abs(newtransform.X - DeformFunction(new PointF(x - resolution, y)).X);

                    float nextystride = Math.Abs(newtransform.Y - DeformFunction(new PointF(x, y + resolution)).Y);
                    float lastystride = Math.Abs(newtransform.Y - DeformFunction(new PointF(x, y - resolution)).Y);

                    double xchangeratio = Math.Max(((nextxstride + lastxstride) / resolution), 0);
                    double ychangeratio = Math.Max(((nextystride + lastystride) / resolution), 0);

                    //double xchangeratio = Math.Abs((newtransform.X-(width / 2)) / (blockcentre.X-(width / 2)));
                    //double ychangeratio = Math.Abs((newtransform.Y-(height / 2)) / (blockcentre.Y-(height / 2)));
                    
                    //if |newtransform| < ||blockcentre:
                    //scale INWARDS
                    //else
                    //scale OUTWARDS
                    var finalxresolution = (resolution * xchangeratio);
                    var finalyresolution = (resolution * ychangeratio);

                    newtransform.X += offset.X;
                    newtransform.Y += offset.Y;

                    // Ensure the new position is within bounds
                    if (newtransform.X < finalxresolution / 2 || 
                        newtransform.Y < finalyresolution / 2 || 
                        newtransform.X > resultBitmap.Width - (finalxresolution / 2) || 
                        newtransform.Y > resultBitmap.Height - (finalyresolution / 2) ||

                        blockcentre.X < finalxresolution / 2 ||
                        blockcentre.Y < finalyresolution / 2 ||
                        blockcentre.X > width - (finalxresolution / 2) ||
                        blockcentre.Y > height - (finalyresolution / 2) ||
                        
                        finalxresolution <= 0 ||
                        finalyresolution <= 0
                        )
                    {
                        continue;
                    }

                    //Instead of increasing the size of the area being drawn, scale the old image
                    Bitmap smallportion = new Bitmap(resolution, resolution);
                    BitmapData portionData = smallportion.LockBits(new Rectangle(0, 0, smallportion.Width, smallportion.Height), ImageLockMode.WriteOnly, PixelFormat.Format32bppPArgb);
                    CopyRectangles(inputData, portionData,
                        new Rectangle((int)(blockcentre.X - (resolution / 2)), (int)(blockcentre.Y) - (resolution / 2), resolution, resolution),
                        new Rectangle(0,0, resolution, resolution)
);
                    smallportion.UnlockBits(portionData);
                    smallportion = ResizeBitmap(smallportion, (int)finalxresolution, (int)finalyresolution);
                    portionData = smallportion.LockBits(new Rectangle(0, 0, smallportion.Width, smallportion.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppPArgb);

                    CopyRectangles(portionData, outputData,
                        new Rectangle(0, 0, (int)finalxresolution, (int)finalyresolution),
                        new Rectangle((int)(newtransform.X - (finalxresolution / 2)), (int)(newtransform.Y - (finalyresolution / 2)), (int)finalxresolution, (int)finalyresolution));

                }
            }
            //}
            //s.Stop();
            //var elapsed = s.ElapsedMilliseconds;
            resultBitmap.UnlockBits(outputData);
            originalimage.UnlockBits(inputData);
        }
        public Bitmap ResizeBitmap(Bitmap bmp, int width, int height)
        {
            Bitmap result = new Bitmap(width, height);
            using (Graphics g = Graphics.FromImage(result))
            {
                g.DrawImage(bmp, 0, 0, width, height);
            }

            return result;
        }
        private static void CopyRectangles(BitmapData sourceData, BitmapData destinationData, Rectangle sourceRect, Rectangle destinationRect)
        {
            unsafe
            {
                const int bytesPerPixel = 4;

                int sourceStride = sourceData.Stride;
                int destinationStride = destinationData.Stride;

                byte* sourcePtr = (byte*)sourceData.Scan0.ToPointer();
                byte* destinationPtr = (byte*)destinationData.Scan0.ToPointer();

                int sourceStartY = sourceRect.Y;
                int sourceStartX = sourceRect.X * bytesPerPixel;
                int sourceWidthInBytes = sourceRect.Width * bytesPerPixel;
                int sourceHeight = sourceRect.Height;

                int destinationStartY = destinationRect.Y;
                int destinationStartX = destinationRect.X * bytesPerPixel;
                int destinationHeight = destinationRect.Height;

                // Ensure the source and destination rectangles have the same width
                if (sourceWidthInBytes != destinationRect.Width * bytesPerPixel)
                {
                    throw new ArgumentException("Source and destination rectangles must have the same width");
                }

                // Loop through each row in the source and destination rectangles
                for (int y = 0; y < sourceHeight && y < destinationHeight; ++y)
                {
                    int sourceOffset = ((sourceStartY + y) * sourceStride) + sourceStartX;
                    int destinationOffset = ((destinationStartY + y) * destinationStride) + destinationStartX;

                    // Calculate the number of bytes to copy for this row
                    int bytesToCopy = (destinationRect.Width * bytesPerPixel);

                    // Use Buffer.MemoryCopy to copy the data for this row
                    Buffer.MemoryCopy(sourcePtr + sourceOffset,
                        destinationPtr + destinationOffset,
                        bytesToCopy,
                        bytesToCopy);
                }
            }
        }
    }
}
