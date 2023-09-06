using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata;
using System.Runtime.InteropServices;
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

        public unsafe void DeformImageToPolygon(Func<PointF, PointF> DeformFunction, Point offset, Bitmap resultBitmap)
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

            //Bitmap smallportion = new Bitmap(resolution, resolution);
            //BitmapData portionData = smallportion.LockBits(new Rectangle(0, 0, smallportion.Width, smallportion.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppPArgb);

            //Stopwatch s = new Stopwatch();
            //s.Start();

            //for (int i = 0; i < 100000; ++i)
            //{
            int numRows = ((height - (resolution / 2)) / resolution) + 2;
            int numCols = ((width - (resolution / 2)) / resolution) + 2; //Add 2 to store elements behind and infront

            int numElements = (numRows) * (numCols);

            int* xCoordinates;
            int* yCoordinates;

            xCoordinates = (int*)Marshal.AllocHGlobal(sizeof(int) * numElements);
            yCoordinates = (int*)Marshal.AllocHGlobal(sizeof(int) * numElements);

            for (int row = 0; row < numRows; row++)
            {
                for (int col = 0; col < numCols; col++)
                {
                    int index = row * (numCols) + col;

                    PointF blockcentre = new PointF((col * resolution) - (resolution / 2), (row * resolution) - (resolution / 2));
                    PointF newtransform = DeformFunction(blockcentre);

                    xCoordinates[index] = (int)newtransform.X;
                    yCoordinates[index] = (int)newtransform.Y;
                }
            }

            // Now, you use the precomputed deformation values inside your nested for loops
            for (int row = 1; row < numRows - 1; row++)
            {
                for (int col = 1; col < numCols - 1; col++)
                {
                    int index = row * numCols + col;

                    int blockcentrex = ((col - 1) * resolution);
                    int blockcentrey = ((row - 1) * resolution);

                    int newtransformx = xCoordinates[index];
                    int newtransformy = yCoordinates[index];

                    double xchangeratio = ((xCoordinates[index + 1] - xCoordinates[index - 1]) / resolution);
                    double ychangeratio = ((yCoordinates[index + numCols] - yCoordinates[index - numCols]) / resolution);

                    //if |newtransform| < ||blockcentre:
                    //scale INWARDS
                    //else
                    //scale OUTWARDS
                    var finalxresolution = (resolution * xchangeratio);
                    var finalyresolution = (resolution * ychangeratio);

                    newtransformx += offset.X;
                    newtransformy += offset.Y;

                    // Ensure the new position is within bounds
                    if (newtransformx < finalxresolution / 2 ||
                        newtransformy < finalyresolution / 2 ||
                        newtransformx > resultBitmap.Width - (finalxresolution / 2) ||
                        newtransformy > resultBitmap.Height - (finalyresolution / 2) ||

                        finalxresolution <= 0 ||
                        finalyresolution <= 0)
                    {
                        continue;
                    }
                    //Instead of increasing the size of the area being drawn, scale the old image
                    BitmapData sourceData = ResizeBitmap(inputData, new Rectangle((int)(blockcentrex), (int)(blockcentrey), resolution, resolution), (int)finalxresolution, (int)finalyresolution);
                    CopyRectangles(sourceData,
                                   outputData,
                                   new Rectangle(0, 0, (int)finalxresolution, (int)finalyresolution),
                                   new Rectangle((int)(newtransformx - (finalxresolution / 2)), (int)(newtransformy - (finalyresolution / 2)), (int)finalxresolution, (int)finalyresolution));
                    Marshal.FreeHGlobal(sourceData.Scan0);
                }
            }

            Marshal.FreeHGlobal((IntPtr)xCoordinates);
            Marshal.FreeHGlobal((IntPtr)yCoordinates);
            //}

            //s.Stop();
            //var elapsed = s.ElapsedMilliseconds;
            //MessageBox.Show(elapsed.ToString());
            resultBitmap.UnlockBits(outputData);
            originalimage.UnlockBits(inputData);
        }
        public unsafe static BitmapData ResizeBitmap(BitmapData sourceData, Rectangle areafrom, int newwidth, int newheight)
        {
            // Lock the source and destination bitmaps in memory
            BitmapData destData = new BitmapData
            {
                Width = newwidth,
                Height = newheight,
                Stride = newwidth * 4, // Assuming 32bpp ARGB format
                PixelFormat = PixelFormat.Format32bppArgb
            };
            destData.Scan0 = Marshal.AllocHGlobal(destData.Stride * newheight);

            // Calculate the scaling factors for width and height
            float scaleX = (float)areafrom.Width / newwidth;
            float scaleY = (float)areafrom.Height / newheight;

            // Pointer to the first pixel of the source and destination bitmaps
            byte* srcPointer = (byte*)sourceData.Scan0 + (areafrom.X * 4) + (areafrom.Y * 4 * sourceData.Width);
            byte* destPointer = (byte*)destData.Scan0;

            int pixelSize = 4; // Each pixel is represented by 4 bytes (32bpp ARGB)

            for (int y = 0; y < newheight; y++)
            {
                for (int x = 0; x < newwidth; x++)
                {
                    // Calculate the corresponding position in the source bitmap
                    int srcX = (int)(x * scaleX);
                    int srcY = (int)(y * scaleY);

                    // Calculate the byte offsets for the source and destination pixels
                    long srcOffset = (srcY * sourceData.Stride) + (srcX * pixelSize);
                    long destOffset = (y * destData.Stride) + (x * pixelSize);

                    // Use MemoryCopy to copy the pixel data
                    Buffer.MemoryCopy(srcPointer + srcOffset, destPointer + destOffset, pixelSize, pixelSize);
                }
            }
            return destData;
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
