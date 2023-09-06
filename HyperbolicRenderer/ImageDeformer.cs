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
        BitmapData imagedata;
        Bitmap GC_pacifier; //This has to exist or GC will have a temper tantrum and delete it
        public ImageDeformer(Bitmap originalimage)
        {
            GC_pacifier = (Bitmap)originalimage.Clone();
            imagedata = GC_pacifier.LockBits(new Rectangle(0, 0, originalimage.Width, originalimage.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppPArgb);
        }

        bool asyncrunning = false;
        public unsafe void DeformImageToPolygon(Func<PointF, PointF> DeformFunction, Point offset, Bitmap resultBitmap)
        {
            while (asyncrunning) { }
            asyncrunning = true;

            int width = imagedata.Width;
            int height = imagedata.Height;

            BitmapData outputData = resultBitmap.LockBits(new Rectangle(0, 0, resultBitmap.Width, resultBitmap.Height), ImageLockMode.WriteOnly, PixelFormat.Format32bppPArgb);

            const int sectionwidth = 2;
            const int sectionradius = sectionwidth/2;

            int numRows = ((height - sectionradius) / sectionwidth) + 2;
            int numCols = ((width - sectionradius) / sectionwidth) + 2; //Add 2 to store elements behind and infront

            int numElements = (numRows) * (numCols);

            int* xCoordinates = (int*)Marshal.AllocHGlobal(sizeof(int) * numElements);
            int* yCoordinates = (int*)Marshal.AllocHGlobal(sizeof(int) * numElements);

            for (int row = 0; row < numRows; row++)
            {
                for (int col = 0; col < numCols; col++)
                {
                    int index = row * (numCols) + col;

                    PointF blockcentre = new PointF((col * sectionwidth) - (sectionradius), (row * sectionwidth) - (sectionradius));
                    PointF newtransform = DeformFunction(blockcentre);

                    xCoordinates[index] = (int)newtransform.X;
                    yCoordinates[index] = (int)newtransform.Y;
                }
            }

            // Now, use the precomputed deformation values
            for (int row = 1; row < numRows - 1; row++)
            {
                for (int col = 1; col < numCols - 1; col++)
                {
                    int index = row * numCols + col;

                    int blockcentrex = ((col - 1) * sectionwidth);
                    int blockcentrey = ((row - 1) * sectionwidth);

                    int newtransformx = xCoordinates[index];
                    int newtransformy = yCoordinates[index];

                    double xchangeratio = ((xCoordinates[index + 1] - xCoordinates[index - 1]) / sectionwidth);
                    double ychangeratio = ((yCoordinates[index + numCols] - yCoordinates[index - numCols]) / sectionwidth);

                    int finalxresolution = (int)(sectionwidth * xchangeratio);
                    int finalyresolution = (int)(sectionwidth * ychangeratio);

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
                    BitmapData sourceData = ResizeBitmap(imagedata, new Rectangle(blockcentrex, blockcentrey, sectionwidth, sectionwidth), finalxresolution, finalyresolution);
                    CopyRectangles(sourceData,
                                   outputData,
                                   new Rectangle(0, 0, finalxresolution, finalyresolution),
                                   new Rectangle(newtransformx - (finalxresolution / 2), newtransformy - (finalyresolution / 2), finalxresolution, finalyresolution));
                    Marshal.FreeHGlobal(sourceData.Scan0);
                }
            }

            Marshal.FreeHGlobal((IntPtr)xCoordinates);
            Marshal.FreeHGlobal((IntPtr)yCoordinates);
            resultBitmap.UnlockBits(outputData);
            asyncrunning = false;
        }
        void TestResize()
        {
            for (int i = 0; i < 1000; ++i)
            {
                var b = ResizeBitmap(imagedata, new Rectangle(0, 0, 4, 4), 2, 2);
                Marshal.FreeHGlobal(b.Scan0);
            }
        }
        public unsafe BitmapData ResizeBitmap(BitmapData sourceData, Rectangle areafrom, int newwidth, int newheight)
        {
            const int bytesPerPixel = 4;

            // Lock the source and destination bitmaps in memory
            BitmapData destData = new BitmapData
            {
                Width = newwidth,
                Height = newheight,
                Stride = newwidth * bytesPerPixel, // Assuming 32bpp ARGB format
                PixelFormat = PixelFormat.Format32bppArgb
            };
            destData.Scan0 = Marshal.AllocHGlobal(destData.Stride * newheight);
            
            // Calculate the scaling factors for width and height
            float scaleX = (float)areafrom.Width / newwidth;
            float scaleY = (float)areafrom.Height / newheight;

            if (scaleX == 1 && scaleY == 1)
            {
                CopyRectangles(sourceData, destData, areafrom, new Rectangle(0,0,newwidth, newheight));
                return destData;
            }

            // Pointer to the first pixel of the source and destination bitmaps
            byte* srcPointer = (byte*)sourceData.Scan0 + (areafrom.X * bytesPerPixel) + (areafrom.Y * sourceData.Stride);
            byte* destPointer = (byte*)destData.Scan0;

            for (int y = 0; y < newheight; y++)
            {
                for (int x = 0; x < newwidth; x++)
                {
                    // Calculate the corresponding position in the source bitmap
                    int srcX = (int)(x * scaleX);
                    int srcY = (int)(y * scaleY);

                    // Calculate the byte offsets for the source and destination pixels
                    long srcOffset = (srcY * sourceData.Stride) + (srcX * bytesPerPixel);
                    long destOffset = (y * destData.Stride) + (x * bytesPerPixel);

                    // Use MemoryCopy to copy the pixel data
                    Buffer.MemoryCopy(srcPointer + srcOffset, destPointer + destOffset, bytesPerPixel, bytesPerPixel);
                }
            }
            return destData;
        }
        private void CopyRectangles(BitmapData sourceData, BitmapData destinationData, Rectangle sourceRect, Rectangle destinationRect)
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