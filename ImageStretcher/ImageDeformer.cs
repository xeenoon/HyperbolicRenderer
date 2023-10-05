using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Reflection.Metadata;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Rebar;

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
        public static unsafe DeformData DeformImageToPolygon(Func<Point, PointF> DeformFunction, Point offset, Bitmap originalimage, Bitmap resultBitmap, List<PointF[]> polygons, bool overridescale = false)
        {
            int width = originalimage.Width;
            int height = originalimage.Height;

            Bitmap greenscreen = new Bitmap(width, height);
            var greenscreendata = greenscreen.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadWrite, PixelFormat.Format32bppPArgb);
            memset((byte*)greenscreendata.Scan0, 1, greenscreendata.Stride * greenscreendata.Height); //1 as alpha value should be negligible
            greenscreen.UnlockBits(greenscreendata);
            var graphics = Graphics.FromImage(greenscreen);
            graphics.CompositingMode = CompositingMode.SourceOver;
            graphics.DrawImage(originalimage, new Point(0,0));
            
            var imagedata = greenscreen.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadWrite, PixelFormat.Format32bppPArgb);
            DeformData deformData = new DeformData();
            
            Bitmap temp = new Bitmap(resultBitmap.Width, resultBitmap.Height);
            
            BitmapData outputData = temp.LockBits(new Rectangle(0, 0, resultBitmap.Width, resultBitmap.Height), ImageLockMode.WriteOnly, PixelFormat.Format32bppPArgb);

            int numRows = height + 2;
            int numCols = width + 2; //Add 2 to store elements behind and infront

            int numElements = (numRows) * (numCols);

            float* xCoordinates = (float*)Marshal.AllocHGlobal(sizeof(float) * numElements);
            float* yCoordinates = (float*)Marshal.AllocHGlobal(sizeof(float) * numElements);

            List<int> outputpixels = new List<int>();
            
            for (int row = 0; row < numRows; row++)
            {
                for (int col = 0; col < numCols; col++)
                {
                    int index = row * (numCols) + col;

                    Point blockcentre = new Point(col, row);
                    PointF newtransform = DeformFunction(blockcentre);

                    if ((newtransform != new PointF(int.MinValue, int.MinValue) 
                        && row >= 1 && row <= numRows - 2
                        && col >= 1 && col <= numCols - 2))

                        //&& (col >= 1 && newtransform.X - xCoordinates[index - 1] == 1))
                    {
                        outputpixels.Add(row * numCols + col);
                    }
                    xCoordinates[index] = newtransform.X;
                    yCoordinates[index] = newtransform.Y;
                }
            }
            
            // Now, use the precomputed deformation values
            foreach (var index in outputpixels)
            {
                int row = index / numCols;
                int col = index % numCols;

                int newtransformx = (int)xCoordinates[index];
                int newtransformy = (int)yCoordinates[index];

                float leftdist = newtransformx - xCoordinates[index - 1];
                float rightdist = xCoordinates[index + 1] - newtransformx;
                float topdist = newtransformy - yCoordinates[index - numCols];
                float downdist = yCoordinates[index + numCols] - newtransformy;

               if (leftdist < 0 || rightdist < 0 || topdist < 0 || downdist < 0)
               {
                   continue;
               }

                int finalxresolution = (int)Math.Ceiling((leftdist + rightdist) / 2f);
                int finalyresolution = (int)Math.Ceiling((topdist + downdist) / 2f);

                // Ensure the new position is within bounds
                if (newtransformx + offset.X < 0 ||
                    newtransformy + offset.Y < 0 ||
                    newtransformx + offset.X > resultBitmap.Width - (finalxresolution) ||
                    newtransformy + offset.Y > resultBitmap.Height - (finalyresolution) ||
                    finalxresolution <= 0 ||
                    finalyresolution <= 0 ||
                    finalxresolution >= int.MaxValue ||
                    finalyresolution >= int.MaxValue ||
                    row >= imagedata.Height ||
                    col >= imagedata.Width
                    )
                {
                    continue;
                }
                deformData.left = Math.Min(deformData.left, newtransformx + offset.X);
                deformData.right = Math.Max(deformData.right, newtransformx + offset.X);
                deformData.top = Math.Min(deformData.top, newtransformy + offset.Y);
                deformData.bottom = Math.Max(deformData.bottom, newtransformy + offset.Y);

                //Resize the section to fit, and copy it into the result
                BilinearResizeCopy(imagedata,
                    col, row,
                    outputData,
                    new Rectangle(newtransformx + offset.X, newtransformy + offset.Y, finalxresolution, finalyresolution));
            }
            //Patch holes in the image
            for (int x = deformData.left; x < deformData.right; ++x)
            {
                for (int y = deformData.top; y < deformData.bottom; ++y)
                {
                    byte* position = ((byte*)outputData.Scan0) + x * 4 + y * outputData.Stride;
                    const int alphaoffset = 3;
                    int alpha = *(position + alphaoffset);
                    if (position[0] == 0 && position[1] == 0 && position[2] == 0 && position[3] == 0) //On a transparent pixel?
                    {
                        const int maxdist = 50;

                        //Cast rays left right up and down to determine if we are a hole
                        int uproof = -1;
                        for (int up = 0; up < maxdist && up < y - deformData.top && uproof == -1; ++up)
                        {
                            byte* newlocation = position - (up * outputData.Stride) + alphaoffset;
                            if (*newlocation != 0) //Hit a non-transparent pixel
                            {
                                uproof = up; //Ray hit a wall
                            }
                        }
                        if (uproof == -1) { continue; }

                        int downroof = -1;
                        for (int down = 0; down < maxdist && down < deformData.bottom - y && downroof == -1; ++down)
                        {
                            byte* newlocation = (position + down * outputData.Stride) + alphaoffset;
                            if (*newlocation != 0) //Hit a non-transparent pixel
                            {
                                downroof = down; //Ray hit a wall
                            }
                        }
                        if (downroof == -1) { continue; }

                        int rightroof = -1;
                        for (int right = 0; right < maxdist && right < deformData.right - x && rightroof == -1; ++right)
                        {
                            byte* newlocation = (position + right * 4) + alphaoffset;
                            if (*newlocation != 0) //Hit a non-transparent pixel
                            {
                                rightroof = right; //Ray hit a wall
                            }
                        }
                        if (rightroof == -1) { continue; }

                        int leftroof = -1; ;
                        for (int left = 0; left < maxdist && left < x - deformData.left && leftroof == -1; ++left)
                        {
                            byte* newlocation = (position - left * 4) + alphaoffset;
                            if (*newlocation != 0) //Hit a non-transparent pixel
                            {
                                leftroof = left; //Ray hit a wall
                            }
                        }
                        if (leftroof == -1) { continue; }

                        //All checks passed? Change the color
                        
                        var newcolor = BlendColors(position - (uproof * outputData.Stride),
                                                      position + (downroof * outputData.Stride),
                                                      position - (leftroof * 4),
                                                      position + (rightroof * 4));
                        Buffer.MemoryCopy(newcolor, position, 4, 4);
                    }
                }
            }

            Marshal.FreeHGlobal((IntPtr)xCoordinates);
            Marshal.FreeHGlobal((IntPtr)yCoordinates);
            temp.UnlockBits(outputData);
            greenscreen.UnlockBits(imagedata);
            //Draw the image
            graphics = Graphics.FromImage(resultBitmap);
            graphics.DrawImage(originalimage, new Point(offset.X, offset.Y));
            foreach (var polygon in polygons)
            {
                if (polygon.Count() <= 2)
                {
                    continue;
                }
                PointF[] newpolygon = new PointF[polygon.Count()];
                for (int i = 0; i < polygon.Length; i++)
                {
                    PointF p = polygon[i];
                    newpolygon[i] = new PointF(p.X + offset.X, p.Y + offset.Y);
                }
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.FillPolygon(new Pen(Color.Transparent).Brush, newpolygon);
                graphics.CompositingMode = CompositingMode.SourceOver;
            }
            graphics.DrawImage(temp.Clone(new Rectangle(deformData.left, deformData.top, deformData.right - deformData.left, deformData.bottom - deformData.top), PixelFormat.Format32bppPArgb), 
                new Point(deformData.left, deformData.top));
            
            if (deformData.left > offset.X) //Didn't draw left side
            {
                //Make edge the unchanged pixels
                deformData.left = offset.X;
            }
            if (deformData.right < offset.X + originalimage.Width)
            {
                deformData.right = offset.X + originalimage.Width;
            }
            if (deformData.top > offset.Y)
            {
                deformData.top = offset.Y;
            }
            if (deformData.bottom < offset.Y + originalimage.Height)
            {
                deformData.bottom = offset.Y + originalimage.Height;
            }

            return deformData;
        }

        static unsafe byte* BlendColors(byte* color1, byte* color2, byte* color3, byte* color4) //TODO: scale with distance for texture
        {
            byte* result = (byte*)Marshal.AllocHGlobal(4);

            const double weight = 0.25f;
            // Blend the colors based on weights
            result[3] = (byte)(weight * ((color1[3] + color2[3] + color3[3]) + color4[3]));
            result[2] = (byte)(weight * (color1[2] + color2[2] + color3[2] + color4[2]));
            result[1] = (byte)(weight * ((color1[1] + color2[1] + color3[1]) + color4[1]));
            result[0] = (byte)(weight * ((color1[0] + color2[0] + color3[0]) + color4[0])); //Color order doesn't matter

            return result; //REMEMBER TO FREE!!!
        }


        static unsafe double CalculateRGBDistance(byte* color1, byte* color2)
        {
            // Calculate the Euclidean distance between two colors in RGB space
            int deltaRed = color1[2] - color2[2];
            int deltaGreen = color1[1] - color2[1];
            int deltaBlue = color1[0] - color2[0];

            return Math.Sqrt(deltaRed * deltaRed + deltaGreen * deltaGreen + deltaBlue * deltaBlue);
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

        private static unsafe void BilinearResizeCopy(BitmapData input, int xinput, int yinput, BitmapData dest, Rectangle destrect)
        {
            const int bytesPerPixel = 4;

            // Calculate the coordinates of the four neighboring pixels in the source image
            int x1 = xinput-1;
            int y1 = yinput-1;
            int x2 = x1 + 1;
            int y2 = y1 + 1;

            // Fractional parts for interpolation
            double xFraction = xinput - x1;
            double yFraction = yinput - y1;

            for (int y = 0; y < destrect.Height; ++y)
            {
                for (int x = 0; x < destrect.Width; ++x)
                {
                    // Calculate the coordinates of the corresponding pixels in the source image
                    int srcX1 = x1 + x;
                    int srcY1 = y1 + y;
                    int srcX2 = x2 + x;
                    int srcY2 = y2 + y;

                    // Clamp the coordinates to stay within the bounds of the source image
                    srcX1 = Math.Max(0, Math.Min(input.Width - 1, srcX1));
                    srcY1 = Math.Max(0, Math.Min(input.Height - 1, srcY1));
                    srcX2 = Math.Max(0, Math.Min(input.Width - 1, srcX2));
                    srcY2 = Math.Max(0, Math.Min(input.Height - 1, srcY2));

                    // Calculate the weight for each neighboring pixel
                    double weight1 = (1.0 - xFraction) * (1.0 - yFraction);
                    double weight2 = xFraction * (1.0 - yFraction);
                    double weight3 = (1.0 - xFraction) * yFraction;
                    double weight4 = xFraction * yFraction;

                    // Pointer to the first pixel of the source and destination bitmaps
                    byte* srcPointer1 = (byte*)(input.Scan0 + (srcX1 * bytesPerPixel) + (srcY1 * input.Stride));
                    byte* srcPointer2 = (byte*)(input.Scan0 + (srcX2 * bytesPerPixel) + (srcY1 * input.Stride));
                    byte* srcPointer3 = (byte*)(input.Scan0 + (srcX1 * bytesPerPixel) + (srcY2 * input.Stride));
                    byte* srcPointer4 = (byte*)(input.Scan0 + (srcX2 * bytesPerPixel) + (srcY2 * input.Stride));
                    byte* destPointer = (byte*)(dest.Scan0 + (destrect.X * bytesPerPixel) + (destrect.Y * dest.Stride));

                    // Calculate the interpolated color values for the destination pixel
                    for (int i = 0; i < bytesPerPixel; i++)
                    {
                        double interpolatedValue = (1.0 - xFraction) * ((1.0 - yFraction) * srcPointer1[i] + yFraction * srcPointer3[i]) +
                                           xFraction * ((1.0 - yFraction) * srcPointer2[i] + yFraction * srcPointer4[i]);

                        // Calculate the color difference between neighboring pixels and add depth
                        double depthValue = interpolatedValue - srcPointer1[i];
                        const double depthfactor = 1f;
                        int newValue = (int)(interpolatedValue + depthfactor * depthValue);
                        newValue = Math.Max(0, Math.Min(255, newValue)); // Clamp to the 0-255 range
                        destPointer[y * dest.Stride + x * bytesPerPixel + i] = (byte)newValue;
                    }
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