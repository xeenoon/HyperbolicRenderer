using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Reflection.Metadata;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ImageStretcher
{
    public struct DeformData
    {
        public int left = int.MaxValue;
        public int right = int.MinValue;
        public int top = int.MinValue;
        public int bottom = int.MaxValue;

        public DeformData()
        {
            left = int.MaxValue;
            right = int.MinValue;
            top = int.MaxValue;
            bottom = int.MinValue;
        }
    }
    public class ImageDeformer
    {
        public static BitmapData imagedata;
        public static Bitmap GC_pacifier; //This has to exist or GC will have a temper tantrum and delete it

        public static unsafe DeformData DeformImageToPolygon(Func<Point, Point> DeformFunction, Point offset, Bitmap originalimage, Bitmap resultBitmap, bool overridescale = false)
        {
            GC_pacifier = (Bitmap)originalimage.Clone();
            imagedata = GC_pacifier.LockBits(new Rectangle(0, 0, originalimage.Width, originalimage.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppPArgb);
            DeformData deformData = new DeformData();

            int width = imagedata.Width;
            int height = imagedata.Height;

            BitmapData outputData = resultBitmap.LockBits(new Rectangle(0, 0, resultBitmap.Width, resultBitmap.Height), ImageLockMode.WriteOnly, PixelFormat.Format32bppPArgb);

            int numRows = height + 2;
            int numCols = width + 2; //Add 2 to store elements behind and infront
            
            int numElements = (numRows) * (numCols);

            int* xCoordinates = (int*)Marshal.AllocHGlobal(sizeof(int) * numElements);
            int* yCoordinates = (int*)Marshal.AllocHGlobal(sizeof(int) * numElements);
            for (int row = 0; row < numRows; row++)
            {
                for (int col = 0; col < numCols; col++)
                {
                    int index = row * (numCols) + col;

                    Point blockcentre = new Point(col, row);
                    Point newtransform = DeformFunction(blockcentre);

                    xCoordinates[index] = newtransform.X;
                    yCoordinates[index] = newtransform.Y;
                }
            }

            // Now, use the precomputed deformation values
            for (int row = 1; row < numRows - 1; row++)
            {
                for (int col = 1; col < numCols - 1; col++)
                {
                    int index = row * numCols + col;

                    int newtransformx = xCoordinates[index];
                    int newtransformy = yCoordinates[index];

                    int leftdist = newtransformx - xCoordinates[index - 1];
                    int rightdist = xCoordinates[index + 1] - newtransformx;
                    int topdist = newtransformy - yCoordinates[index - numCols];
                    int downdist = yCoordinates[index + numCols] - newtransformy;

                    if (leftdist < 0 || rightdist < 0 || topdist < 0 || downdist < 0)
                    {
                        continue;
                    }

                    int finalxresolution = (leftdist + rightdist);
                    int finalyresolution = (topdist + downdist);

                    // Ensure the new position is within bounds
                    if (newtransformx + offset.X < 0 ||
                        newtransformy + offset.Y < 0 ||
                        newtransformx + offset.X > resultBitmap.Width - (finalxresolution) ||
                        newtransformy + offset.Y > resultBitmap.Height - (finalyresolution) ||
                        finalxresolution <= 0 ||
                        finalyresolution <= 0 ||
                        finalxresolution >= int.MaxValue ||
                        finalyresolution >= int.MaxValue
                        )
                    {
                        continue;
                    }
                    deformData.left   = Math.Min(deformData.left, newtransformx + offset.X);
                    deformData.right  = Math.Max(deformData.right, newtransformx + offset.X);
                    deformData.top    = Math.Min(deformData.top, newtransformy + offset.Y);
                    deformData.bottom = Math.Max(deformData.bottom, newtransformy + offset.Y);

                    //Resize the section to fit, and copy it into the result
                    SmootheResizeCopy(imagedata,
                        col, row,
                        outputData,
                        new Rectangle(newtransformx + offset.X, newtransformy + offset.Y, finalxresolution, finalyresolution));
                }
            }
            Marshal.FreeHGlobal((IntPtr)xCoordinates);
            Marshal.FreeHGlobal((IntPtr)yCoordinates);
            resultBitmap.UnlockBits(outputData);

            GC_pacifier.Dispose();
            return deformData;
        }

        [DllImport("msvcrt.dll", CallingConvention = CallingConvention.Cdecl, SetLastError = false)]
        public static extern unsafe IntPtr memset(void* dest, int c, int count);
        private static unsafe void SmootheResizeCopy(BitmapData input, int xinput, int yinput, BitmapData dest, Rectangle destrect)
        {
            const int bytesPerPixel = 4;

            // Pointer to the first pixel of the source and destination bitmaps
            byte* srcPointer = (byte*)(input.Scan0 + (xinput * bytesPerPixel) + (yinput * input.Stride));
            byte* destPointer = (byte*)(dest.Scan0 + (destrect.X * bytesPerPixel) + (destrect.Y * dest.Stride));

            for (int y = 0; y < destrect.Height; ++y)
            {
                for (int x = 0; x < destrect.Width; ++x) //Really really really inefficient
                {
                    Buffer.MemoryCopy(srcPointer, destPointer + y * dest.Stride + x * bytesPerPixel, bytesPerPixel, bytesPerPixel);
                }
            }
        }
        private static unsafe void CopyRectangles(byte* sourcePtr, int sourceStride, byte* destinationPtr, int destinationStride, Rectangle sourceRect, Rectangle destinationRect)
        {
            const int bytesPerPixel = 4;

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
        public static unsafe void CopyRectangles(BitmapData input, BitmapData output, Rectangle sourcerectangle, Rectangle destinationRect)
        {
            CopyRectangles((byte*)input.Scan0.ToPointer(), input.Stride, (byte*)output.Scan0.ToPointer(), output.Stride, sourcerectangle, destinationRect);
        }
    }
}