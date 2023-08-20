using Microsoft.VisualBasic.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
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
        public void GenerateShape()
        {

        }
        public void AddShape(Shape shape)
        {
            shapes.Add(shape);
        }
        public int volumewidth = 0;
        public float squaresize = 0;
        public static int extracells = 4;
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

                PointF centre = new PointF(shape.centre.X + offsetx, shape.centre.Y + offsety);
                List<PointF> closestpoints = new List<PointF>();
                closestpoints = volume[unadjustedvolume.FindIndex(v=>centre.InPolygon(v.points))].points.ToList();
                
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
            }
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
            float turningtime = squaresize;
            int debugidx = (int)((relativepoint.X - offsetx) / squaresize + ((relativepoint.Y - offsety) / squaresize) * volumewidth);
            PointF scalar = SinScale(turningtime, relativepoint, true, debugidx);

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

        public PointF SinScale(double turningtime, PointF relativepoint, bool showdebug = false, int debugidx = 0)
        {
            float y_scale;
            float x_scale;

            float neg_x_scale = float.MinValue;
            float pos_x_scale = float.MaxValue;

            float neg_y_scale = float.MinValue;
            float pos_y_scale = float.MaxValue;

            foreach (var line in shapelines)
            {
                PointF linedistance = relativepoint.DistanceTo(line);

                if (linedistance.Y > neg_y_scale && linedistance.Y < 0 && linedistance.Y != 0)
                {
                    neg_y_scale = linedistance.Y;
                }
                if (linedistance.Y < pos_y_scale && linedistance.Y > 0 && linedistance.Y != 0)
                {
                    pos_y_scale = linedistance.Y;
                }

                if (linedistance.X > neg_x_scale && linedistance.X < 0 && linedistance.X != 0)
                {
                    neg_x_scale = linedistance.X;
                }
                if (linedistance.X < pos_x_scale && linedistance.X > 0 && linedistance.X != 0)
                {
                    pos_x_scale = linedistance.X;
                }
            }

            if (Math.Abs(neg_y_scale) < pos_y_scale)
            {
                y_scale = neg_y_scale;
            }
            else
            {
                y_scale = pos_y_scale;
            }

            if (Math.Abs(neg_x_scale) < pos_x_scale)
            {
                x_scale = neg_x_scale;
            }
            else
            {
                x_scale = pos_x_scale;
            }
            if (showdebug)
            {
                sideconnections[debugidx] = new Line(relativepoint, new PointF(x_scale + relativepoint.X, y_scale + relativepoint.Y)); //debugdata
            }


            float y_distancetocentre = relativepoint.Y - radius;
            float x_distancetocentre = relativepoint.X - radius;

            x_scale *= (float)SmootheCutoff(x_distancetocentre, turningtime);
            y_scale *= (float)SmootheCutoff(y_distancetocentre, turningtime);

            x_scale = (float)Math.Sin((x_scale) / (radius / 10)) / 2;
            y_scale = (float)Math.Sin((y_scale) / (radius / 10)) / 2;

            return new PointF(x_scale, y_scale);
        }

        private static double SmootheCutoff(double distance, double turningtime)
        {
            double result = 1;
            const double cutoff = 0.6f;
            if (Math.Abs(distance) < turningtime)
            {
                //Should equal zero when distancetocentre == 0
                //Should equal 1 when distancetocentre == turningtime
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
        bool baked = false;
        internal void BakeHeights()
        {
            heights = new PointF[radius*2, radius*2];
            for (int x = 0; x < radius*2; ++x)
            {
                for (int y = 0; y < radius * 2; ++y)
                {
                    heights[x, y] = SinScale(0.6f, new PointF(x,y));
                }
            }
            baked = true;
        }
        public PointF GetBakedHeights(PointF relativepoint)
        {
            if (!baked)
            {
                return new PointF(0,0);
            }
            double xloc = relativepoint.X;
            while (xloc < 0)
            {
                xloc += radius * 2;
            }
            while (xloc >= radius * 2)
            {
                xloc -= radius * 2;
            }

            double yloc = relativepoint.Y;
            while (yloc < 0)
            {
                yloc += radius * 2;
            }
            while (yloc >= radius * 2)
            {
                yloc -= radius * 2;
            }
            return heights[(int)xloc, (int)yloc];
        }
    }
}
