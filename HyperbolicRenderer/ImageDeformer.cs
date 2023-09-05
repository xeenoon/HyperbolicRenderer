using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.Linq;
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
                    newtransform.X += offset.X;
                    newtransform.Y += offset.Y;
                    // Ensure the new position is within bounds

                    if (newtransform.X < 0 || newtransform.Y < 0 || newtransform.X > resultBitmap.Width - (resolution / 2) || newtransform.Y > resultBitmap.Height - (resolution / 2))
                    {
                        continue;
                    }

                    //newtransform = new PointF(Math.Max(0, Math.Min(newtransform.X, resultBitmap.Width - 1) - (resolution / 2)), Math.Max(0, Math.Min(newtransform.Y, resultBitmap.Height - 1) - (resolution / 2)));
                    //Map the new point to the drawsize
                    CopyRectangles(inputData, outputData,
                        new Rectangle((int)(blockcentre.X - (resolution / 2)), (int)(blockcentre.Y) - (resolution / 2), resolution, resolution),
                        new Rectangle((int)(newtransform.X), (int)(newtransform.Y), resolution, resolution));

                }
            }
            //}
            //s.Stop();
            //var elapsed = s.ElapsedMilliseconds;
            resultBitmap.UnlockBits(outputData);
            originalimage.UnlockBits(inputData);
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
