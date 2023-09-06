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


        public Map(int pointcount, int radius)
        {
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
            /*
            adjustedshapes.Clear();
            for (int i = 0; i < shapes.Count; i++)
            {
                Shape shape = shapes[i];
                adjustedshapes.Add(new Shape(new PointF[shape.points.Count()], shape.centre));

                PointF centre = new PointF(shape.centre.X + offsetx, shape.centre.Y + offsety);
                List<PointF> closestpoints = new List<PointF>();
                closestpoints = volume[unadjustedvolume.FindIndex(v => centre.InPolygon(v.points))].points.ToList();

                PointF newcentre = StretchShapePoint(offsetx, offsety, closestpoints, centre);

                PointF[] newpoints = new PointF[shape.points.Count()];

                for (int i1 = 0; i1 < shape.points.Length; i1++)
                {
                    PointF point = shape.points[i1];
                    newpoints[i1] = new PointF(point.X + (newcentre.X - centre.X), point.Y + (newcentre.Y - centre.Y));
                }

                closestpoints = volume.Where(v => newcentre.InPolygon(v.points)).FirstOrDefault().points.ToList(); //Recalculate incase we moved squares

                //Find the point that is closest to it, and scale towards that one
                float exactxiterpoint = centre.X / squaresize;
                float exactyiterpoint = centre.Y / squaresize;

                exactyiterpoint = Math.Max(exactyiterpoint, 0);
                exactxiterpoint = Math.Max(exactxiterpoint, 0);

                //Find the centre of the shape
                for (int j = 0; j < shape.points.Length; j++)
                {
                    PointF point = newpoints[j];
                    adjustedshapes[i].points[j] = StretchShapePoint(offsetx, offsety, closestpoints, point);
                }
            }*/
        }

        private PointF StretchShapePoint(float offsetx, float offsety, List<PointF> closestpoints, PointF point)
        {
            point = new PointF(point.X + offsetx, point.Y + offsety);

            //Find the point that is closest to it, and scale towards that one
            List<float> distances = new List<float>
                    {
                        1/(float)closestpoints[0].DistanceTo(point),
                        1/(float)closestpoints[1].DistanceTo(point),
                        1/(float)closestpoints[2].DistanceTo(point),
                        1/(float)closestpoints[3].DistanceTo(point),
                    };
            float sum = distances.Sum();
            for (int i1 = 0; i1 < distances.Count; i1++)
            {
                float distance = distances[i1];
                distances[i1] = (distance / sum); //Distances becom a percentage of the factor that they will change the point by
            }
            double radius = 10;
            List<PointF> moveeffects = new List<PointF>
                    {
                        new PointF((((closestpoints[0].X - point.X)))*distances[0], (((closestpoints[0].Y - point.Y)) ) * (distances[0])),
                        new PointF((((closestpoints[1].X - point.X)))*distances[1], (((closestpoints[1].Y - point.Y)) ) * (distances[1])),
                        new PointF((((closestpoints[2].X - point.X)))*distances[2], (((closestpoints[2].Y - point.Y)) ) * (distances[2])),
                        new PointF((((closestpoints[3].X - point.X)))*distances[3], (((closestpoints[3].Y - point.Y)) ) * (distances[3]))
                    };
            PointF moveamount = new PointF(moveeffects.Sum(s => s.X), moveeffects.Sum(s => s.Y));
            return new PointF(moveamount.X + point.X, moveamount.Y + point.Y);
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

            float ay = (relativepoint.Y);
            float ax = (relativepoint.X);

            ay += (float)scalar.Y * squaresize;
            ax += (float)scalar.X * squaresize;

            return new PointF(ax, ay);
        }
        public PointF SinScale(PointF relativepoint, bool showdebug = false, int debugidx = 0)
        {
            double turningtime = 100;
            float y_scale = float.MaxValue;
            float x_scale = float.MaxValue;

            //Wrap bounds around shapelines to only deal with relavent ones
            List<Line> lines = shapelines;
            foreach (var line in lines)
            {
                PointF linedistance = relativepoint.DistanceTo(line);

                if (Math.Abs(linedistance.Y) < Math.Abs(y_scale) && (linedistance.Y != 0 || (points.Count() % 4 != 2)))
                {
                    y_scale = linedistance.Y;
                }

                if (Math.Abs(linedistance.X) < Math.Abs(x_scale) && (linedistance.X != 0 || (points.Count() % 4!=2)))
                {
                    x_scale = linedistance.X;
                }
            }

            if (showdebug)
            {
                sideconnections[debugidx] = new Line(relativepoint, new PointF(x_scale + relativepoint.X, y_scale + relativepoint.Y)); //debugdata
            }


            float y_distancetocentre = relativepoint.Y - radius;
            float x_distancetocentre = relativepoint.X - radius;

            x_scale *= (float)SmootheCutoff(x_distancetocentre, turningtime);
            y_scale *= (float)SmootheCutoff(y_distancetocentre, turningtime);

            //Sin = 1 at y_distancetocentre = 0

            x_scale = (float)Math.Sin((x_scale) / (radius / 10)) / 2;
            y_scale = (float)Math.Sin((y_scale) / (radius / 10)) / 2;

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

            return result;
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
                return new PointF(0, 0);
            }

            PointF height = heights[xloc, yloc];
           // if (height.IsEmpty)
           // {
           //     height = SinScale(relativepoint);
           //     heights[xloc, yloc] = height; //Update the array in-case we need this one in the future
           //     return height;
           // }

            return height;
        }
    }
}
