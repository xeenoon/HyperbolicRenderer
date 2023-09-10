using Microsoft.VisualBasic.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyperbolicRenderer
{
    public struct Line
    {
        public PointF start;
        public PointF end;

        public Line(PointF start, PointF end)
        {
            this.start = start;
            this.end = end;
        }
    }
    public class Map
    {
        public Shape shape;
        public PointF[] points
        {
            get
            {
                return shape.points;
            }
        }
        public List<Trapezium> volume = new List<Trapezium>();
        public List<Trapezium> unadjustedvolume = new List<Trapezium>();
        public int radius;
        public PointF[] connections;
        public PointF[] oldconnections;
        public Line[] sideconnections;
        public List<Shape> shapes = new List<Shape>();
        public List<Shape> adjustedshapes = new List<Shape>();
        List<Line> shapelines = new List<Line>();
        PointF fixedoffset;

        public Map(int pointcount, int radius, PointF offset)
        {
            fixedoffset = offset;
            shape = Shape.CreateShape(pointcount, radius, new PointF(radius, radius));
            
            for (int i = 0; i < points.Count(); i++)
            {
                shapelines.Add(new Line(points[i], points[i + 1 >= points.Count() ? 0 : i + 1]));
            }
            this.radius = radius;
        }
        public int volumewidth = 0;
        public float squaresize = 0;
        public static int extracells = 0;
        public List<PointF> debugpoints = new List<PointF>();
        public void GenerateVolume(float scale, float offsetx, float offsety, bool infinitevolume) //Scale of 1 will have squares of 20% size, 0.5 = 10% size...
        {
            debugpoints.Clear();
            squaresize = radius * 0.2f * scale;
            if (infinitevolume)
            {
                while (offsetx > 0)
                {
                    offsetx -= squaresize;
                }
                while (offsetx < -squaresize * (extracells / 2))
                {
                    offsetx += squaresize;
                }
                while (offsety > 0)
                {
                    offsety -= squaresize;
                }
                while (offsety < -squaresize * (extracells / 2))
                {
                    offsety += squaresize;
                }
            }

            volumewidth = (int)Math.Ceiling((radius * 2) / squaresize) + 1 + extracells;
            connections = new PointF[(volumewidth) * (volumewidth)];
            oldconnections = new PointF[(volumewidth) * (volumewidth)]; //debugdata
            sideconnections = new Line[(volumewidth) * (volumewidth)]; //debugdata
            volume.Clear();


            for (int y = 0; y < (volumewidth); ++y)
            {
                for (int x = 0; x < (volumewidth); ++x)
                {
                    PointF relativepoint = new PointF(x * squaresize + offsetx, y * squaresize + offsety);
                    PointF p = StretchPoint(relativepoint, offsetx, offsety);

                    connections[x + y * (volumewidth)] = p;
                    oldconnections[x + y * (volumewidth)] = relativepoint;
                }
            }

            for (int x = 0; x < volumewidth - 1; x++)
            {
                for (int y = 0; y < volumewidth - 1; y++)
                {
                    int ay = y * volumewidth; //Adjusted y

                    PointF[] points = new PointF[] { connections[x + ay], connections[x + (ay + volumewidth)], connections[(x + 1) + ay], connections[(x + 1) + (ay + volumewidth)] };
                    PointF[] unadjustedpoints = new PointF[] { oldconnections[x + ay], oldconnections[x + (ay + volumewidth)], oldconnections[(x + 1) + ay], oldconnections[(x + 1) + (ay + volumewidth)] };
                    volume.Add(new Trapezium(points[0], points[1], points[2], points[3]));
                    unadjustedvolume.Add(new Trapezium(unadjustedpoints[0], unadjustedpoints[1], unadjustedpoints[2], unadjustedpoints[3]));
                }
            }

            adjustedshapes.Clear();
            for (int i = 0; i < shapes.Count; i++)
            {
                Shape shape = shapes[i];
                adjustedshapes.Add(new Shape(new PointF[shape.points.Count()], shape.centre));
                for (int pointidx = 0; pointidx < shape.points.Length; pointidx++)
                {
                    PointF p = shape.points[pointidx];
                    p.X += offsetx;
                    p.Y += offsety;
                    p = StretchPoint(p, offsetx, offsety);

                    adjustedshapes[i].points[pointidx] = p;
                }
            }
        }
        public PointF StretchPoint(PointF relativepoint, float offsetx, float offsety)
        {
            int debugidx = (int)((relativepoint.X - offsetx) / squaresize + ((relativepoint.Y - offsety) / squaresize) * volumewidth);
            debugidx = Math.Max(0, debugidx);
            PointF scalar = SinScale(relativepoint, true, debugidx);

            float ay = (relativepoint.Y - offsety) / squaresize;
            float ax = (relativepoint.X - offsetx) / squaresize;
            if (offsety != 0) //Removes infinity errors
            {
                ay += (offsety / squaresize);
            }
            if (offsetx != 0) //Removes infinity errors
            {
                ax += (offsetx / squaresize);
            }
            ay += (float)scalar.Y;
            ax += (float)scalar.X;
            ay *= squaresize;
            ax *= squaresize;

            return new PointF(ax, ay);
        }
        public PointF StretchPoint(PointF relativepoint)
        {
            PointF scalar = GetBakedHeights(relativepoint);

            float ay = relativepoint.Y;
            float ax = relativepoint.X;

            ay += scalar.Y * squaresize;
            ax += scalar.X * squaresize;

            return new PointF(ax, ay);
        }

        public PointF SinScale(PointF relativepoint, bool showdebug = false, int debugidx = 0)
        {
            relativepoint = new PointF(relativepoint.X - fixedoffset.X, relativepoint.Y - fixedoffset.Y);

            double turningtime = squaresize;
            float y_scale = float.MaxValue;
            float x_scale = float.MaxValue;

            //Wrap bounds around shapelines to only deal with relavent ones
            List<Line> lines = shapelines;
            foreach (var line in lines)
            {
                PointF linedistance = relativepoint.DistanceTo(line);

                if (linedistance.Magnitude() < Math.Sqrt((y_scale*y_scale)+(x_scale*x_scale)))
                {
                    y_scale = -linedistance.Y;
                    x_scale = -linedistance.X;
                }
            }

            if (showdebug)
            {
                sideconnections[debugidx] = new Line(relativepoint, new PointF(x_scale + relativepoint.X, y_scale + relativepoint.Y)); //debugdata
            }


            float y_distancetocentre = (relativepoint.Y - radius);
            float x_distancetocentre = (relativepoint.X - radius);

            x_scale *= (float)SmootheCutoff(x_distancetocentre, turningtime);
            y_scale *= (float)SmootheCutoff(y_distancetocentre, turningtime);

            
            x_scale = (float)Math.Sin((Math.PI * x_scale) / (2 * radius));
            y_scale = (float)Math.Sin((Math.PI * y_scale) / (2 * radius));

            return new PointF(x_scale, y_scale);
        }

        private static double SmootheCutoff(double distance, double turningtime)
        {
            double result = 1;
            const double cutoff = 0.3f;
            if (Math.Abs(distance) < turningtime)
            {
                //Should equal zero when distancetocentre == 0
                //Should equal 1 when distancetocentre == turningtime
               // if (distance == 0)
               // {
               //     return 0;
               // }
               // return Math.Abs(distance / turningtime);

                if (Math.Abs(distance) >= turningtime * cutoff)
                {
                    double a = 0.7f / Math.Pow(turningtime - (turningtime * cutoff), 2);
                    result = (float)(a * Math.Pow(Math.Abs(distance) - (turningtime * cutoff), 2) + 0.3f);
                }
                else
                {
                    //((0.7)/(3 a s)) x+1-((0.7)/(3 a s)) 
                    result = (float)((0.3f / (cutoff * turningtime)) * Math.Abs(distance));
                }
            }

            return Math.Abs(result);
        }

        PointF[,] heights;
        public double elapsedtime;
        public void BakeHeights(int threadcount)
        {
            int adjustedradius = (int)((radius + (squaresize * extracells)) * 2);
            heights = new PointF[adjustedradius, adjustedradius];

            Stopwatch s = new Stopwatch();
            s.Start();
            Task.Run(() =>
            {
                Parallel.For(0, threadcount,
                             threadindex =>
                             {
                                 for (int xi = 0; xi < adjustedradius / (threadcount); ++xi)
                                 {
                                     int x = (int)(xi + threadindex * (adjustedradius / (threadcount)));
                                     for (int y = 0; y < adjustedradius; ++y)
                                     {
                                         if (heights[x, y].IsEmpty)
                                         {
                                             heights[x, y] = SinScale(new PointF(x, y)); //Ignore already set heights
                                         }
                                     }
                                 }
                             });
                elapsedtime = s.ElapsedMilliseconds;
            });
        }
        public PointF GetBakedHeights(PointF relativepoint)
        {
            if (heights == null)
            {
                int adjustedradius = (int)((radius + (squaresize * extracells)) * 2);
                heights = new PointF[adjustedradius, adjustedradius];
            }

            var xloc = (int)Math.Round(relativepoint.X);
            var yloc = (int)Math.Round(relativepoint.Y);
            if (heights == null || xloc < 0 || xloc >= heights.GetLength(0) || yloc < 0 || yloc >= heights.GetLength(1))
            {
                return SinScale(new PointF(xloc, yloc));
            }

            PointF height = heights[xloc, yloc];
            if (height.IsEmpty)
            {
                height = SinScale(relativepoint);
                heights[xloc, yloc] = height; //Update the array in-case we need this one in the future
                return height;
            }

            return height;
        }
    }
}
