using Microsoft.VisualBasic.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyperbolicRenderer
{
    public class Trapezium
    {
        public PointF top_left;
        public PointF bottom_left;
        public PointF top_right;
        public PointF bottom_right;

        public Trapezium(PointF top_left, PointF bottom_left, PointF top_right, PointF bottom_right)
        {
            this.top_left = top_left;
            this.bottom_left = bottom_left;
            this.top_right = top_right;
            this.bottom_right = bottom_right;
        }
        public static double elapseddrawtime;
        public static double elapsedtrigtime;
        Stopwatch s = new Stopwatch();
        internal void Draw(BMP image, Graphics outerlayer, bool curved, Color color, int mapsize, bool fill=true)
        {
            List<PointF> polygonpoints = new List<PointF>();
            if (curved)
            {
                var topdistance = top_right.X - top_left.X;
                var bottomdistance = bottom_right.X - bottom_left.X;
                var rightdistance = bottom_right.Y - top_right.Y;
                var leftdistance = bottom_left.Y - top_left.Y;

                double t_m = (top_right.Y - top_left.Y) / topdistance;
                double t_c = top_right.Y - t_m * top_right.X;


                double b_m = (bottom_right.Y - bottom_left.Y) / bottomdistance;
                double b_c = bottom_right.Y - b_m * bottom_right.X;

                double r_m = rightdistance / (bottom_right.X - top_right.X);
                double r_c = bottom_right.Y - r_m * bottom_right.X;
                
                double l_m = leftdistance / (bottom_left.X - top_left.X);
                double l_c = bottom_left.Y - l_m * bottom_left.X;

                if (fill)
                {
                    if (top_left.X > top_right.X)
                    {

                    }

                    polygonpoints.AddRange(CurvePoints(topdistance, t_m, t_c, Side.Top, (int)top_left.X, mapsize));
                    polygonpoints.AddRange(CurvePoints(rightdistance, r_m, r_c, Side.Right, (int)top_right.Y, mapsize));
                    polygonpoints.AddRange(CurvePoints(bottomdistance, b_m, b_c, Side.Bottom, (int)bottom_left.X, mapsize).Reverse());
                    polygonpoints.AddRange(CurvePoints(leftdistance, l_m, l_c, Side.Left, (int)top_left.Y, mapsize).Reverse());
                }
                else
                {
                    DrawCurve(topdistance, t_m, t_c, true, mapsize, image, color);
                    DrawCurve(rightdistance, r_m, r_c, false, mapsize, image, color);
                }
            }
            else
            {
                s.Restart();
                DrawLine(top_left, top_right, true, mapsize, image, color);
                DrawLine(top_right, bottom_right, false, mapsize, image, color);
                s.Stop();
                elapseddrawtime += s.ElapsedTicks;
            }

            if (fill && polygonpoints.Count() != 0)
            {
                outerlayer.FillPolygon(new Pen(color).Brush, polygonpoints.ToArray());
            }
        }
        private void DrawCurve(double distance, double m, double c, bool horizontal, double mapsize, BMP image, Color color)
        {
            double a = Math.PI / (distance);
            
            for (float i = 0; i < distance; ++i)
            {
                s.Restart();
                double sin_height = Math.Sin(a * i); //Expressed as a percentage of the new height, pi/2 gets to next period to curve upwards
                int workingvar;
                if (horizontal)
                {
                    workingvar = (int)(i + top_left.X);
                }
                else
                {
                    workingvar = (int)(i + top_right.Y);
                }

                double normalheight;
                if (horizontal)
                {
                    normalheight = m*workingvar + c;
                }
                else
                {
                    normalheight = (workingvar - c) / m; //Find the height if it was a straight line
                }

                if (bottom_right.X - top_right.X == 0 && !horizontal) //Check fofr pure vertical lines
                {
                    normalheight = top_right.X;
                }

                //Use pythag to get distance to centre

                double axisdist = normalheight - (mapsize / 2);

                double scalingfactor = Math.Abs(axisdist) / (mapsize / 2);
                scalingfactor = Math.Log(scalingfactor + 1) * 0.5f;
                scalingfactor = axisdist > 0 ? scalingfactor : -scalingfactor;

                int curveheight = (int)(sin_height * (distance) * scalingfactor + normalheight);
                if (curveheight >= mapsize || curveheight < 0 || workingvar >= mapsize || workingvar < 0)
                {
                    continue;
                }
                s.Stop();
                elapsedtrigtime += s.ElapsedTicks;
                s.Restart();
                if (horizontal)
                {
                    image.SetPixel(workingvar, curveheight, color);
                }
                else
                {
                    image.SetPixel(curveheight, workingvar, color);
                }
                s.Stop();
                elapseddrawtime += s.ElapsedTicks;
            }
        }
        enum Side
        {
            Top,
            Right,
            Bottom,
            Left
        }
        private PointF[] CurvePoints(double distance, double m, double c, Side side, int startidx, double mapsize)
        {
            if (startidx > mapsize-distance || startidx < 0)
            {
                return new PointF[0];
            }
            PointF[] polygonpoints = new PointF[(int)Math.Ceiling(distance)];
            double a = Math.PI / (distance);
            
            s.Restart();
            for (float i = 0; i < distance; ++i)
            {
                double sin_height = Math.Sin(a * i); //Expressed as a percentage of the new height, pi/2 gets to next period to curve upwards
                int workingvar = (int)(i + startidx);


                double normalheight;
                if (side == Side.Top || side == Side.Bottom)
                {
                    normalheight = m * workingvar + c;
                }
                else
                {
                    normalheight = (workingvar - c) / m; //Find the height if it was a straight line
                }


                if (side == Side.Right && double.IsNaN(normalheight))
                {
                    normalheight = top_right.X;
                }
                else if (side == Side.Left && double.IsNaN(normalheight))
                {
                    normalheight = top_left.X;
                }

                //Use pythag to get distance to centre

                double axisdist = normalheight - (mapsize / 2);

                double scalingfactor = Math.Abs(axisdist) / (mapsize / 2);
                scalingfactor = Math.Log(scalingfactor + 1) * 0.5f;
                scalingfactor = axisdist > 0 ? scalingfactor : -scalingfactor;

                int curveheight = (int)(sin_height * (distance) * scalingfactor + normalheight);
                curveheight = (int)Math.Min(curveheight, mapsize-1);
                workingvar = (int)Math.Min(workingvar, mapsize-1);
                workingvar = (int)Math.Max(workingvar, 0);
                curveheight = (int)Math.Max(curveheight, 0);

                if (side == Side.Top || side == Side.Bottom)
                {
                    polygonpoints[(int)i] = new PointF(workingvar, curveheight);
                }
                else
                {
                    polygonpoints[(int)i] = new PointF(curveheight, workingvar);
                }
            }
            s.Stop();
            elapsedtrigtime += s.ElapsedTicks;

            return polygonpoints;
        }
        private void DrawLine(PointF start, PointF end, bool horizontal, double mapsize, BMP image, Color color)
        {
            if (start.X > end.X && horizontal) //Reversed pointers given?
            {
                //Switch them
                PointF placeholder = new PointF(end.X, end.Y);
                end = new PointF(start.X, start.Y);
                start = new PointF(placeholder.X, placeholder.Y);
            }
            double distance;
            double m;
            double c;
            if (horizontal)
            {
                distance = end.X - start.X;
                m = (end.Y - start.Y) / distance;
                c = end.Y - m * end.X;
            }
            else
            {
                distance = end.Y - start.Y;
                m = distance / (end.X - start.X);
                c = end.Y - m * end.X;
            }
            for (float i = 0; i < distance; ++i)
            {
                int workingvar;

                int resultheight;
                if (horizontal)
                {
                    workingvar = (int)(i + start.X);
                    resultheight = (int)(m * workingvar + c); //Find the height if it was a straight line
                }
                else
                {
                    workingvar = (int)(i+start.Y);
                    resultheight = (int)((workingvar - c) / m); //Find the height if it was a straight line

                    if (end.X - start.X == 0)
                    {
                        resultheight = (int)start.X;
                    }
                }
                if (workingvar >= mapsize || workingvar < 0 || resultheight >= mapsize || resultheight < 0)
                {
                    continue;
                }
                if (horizontal)
                {
                    image.SetPixel(workingvar, resultheight, color);
                }
                else
                {
                    image.SetPixel(resultheight, workingvar, color);
                }
            }
        }
    }
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
        public PointF[] points;
        public List<Trapezium> volume = new List<Trapezium>();
        public float radius;
        public PointF[] connections;
        public PointF[] oldconnections;
        public Line[] sideconnections;
        public List<PointF[]> shapes = new List<PointF[]>();
        public List<PointF[]> adjustedshapes = new List<PointF[]>();
        List<Line> shapelines = new List<Line>();


    public Map(int points, float radius)
        {
            this.points = new PointF[points];
            this.radius = radius;
        }
        public PointF[] CreateShape(int points, float radius)
        {
            PointF[] result = new PointF[points];
            double radiansstepsize = Math.Tau / points;
            for (int i = 0; i < points; ++i)
            {
                //Draw a ray from the centre until it hits the edge of the square
                //Make this a vector
                double angle = (3 * (Math.PI / 2)) + (radiansstepsize * i);

                float x = (float)(Math.Cos(angle) * radius + radius);

                float y = (float)(Math.Sin(angle) * radius + radius);
                result[i] = new PointF(x, y);
            }
            return result;
        }
        public void GenerateShape()
        {
            points = CreateShape(points.Length, radius);

            for (int i = 0; i < points.Count(); i++)
            {
                shapelines.Add(new Line(points[i], points[i + 1 >= points.Count() ? 0 : i + 1]));
            }
        }
        public void AddShape(PointF[] points, PointF position)
        {
            PointF[] resultshape = new PointF[points.Length];
            for (int i = 0; i < points.Length; i++)
            {
                PointF point = points[i];
                resultshape[i] = new PointF(point.X + position.X, point.Y + position.Y);
            }
            shapes.Add(resultshape);
        }
        public int volumewidth = 0;
        public float squaresize = 0;
        public static int extracells = 4;

        public void GenerateVolume(float scale, float offsetx, float offsety, bool infinitevolume) //Scale of 1 will have squares of 20% size, 0.5 = 10% size...
        {
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
            adjustedshapes.Clear();
            for (int i = 0; i < shapes.Count; i++)
            {
                PointF[]? shape = shapes[i];
                adjustedshapes.Add(new PointF[shape.Length]);
                for (int j = 0; j < shape.Length; j++)
                {
                    PointF point = shape[j];
                    point = new PointF(point.X + offsetx, point.Y + offsety);
                    //Stretch the point
                    PointF stretchedpoint = StretchPoint(point, offsetx, offsety);
                    adjustedshapes[i][j] = stretchedpoint;
                }
            }

            for (int x = 0; x < volumewidth - 1; x++)
            {
                for (int y = 0; y < volumewidth - 1; y++)
                {
                    int ay = y * volumewidth; //Adjusted y

                    volume.Add(new Trapezium(connections[x + ay], connections[x + (ay + volumewidth)], connections[(x + 1) + ay], connections[(x + 1) + (ay + volumewidth)]));
                }
            }
        }
        public PointF StretchPoint(PointF relativepoint, float offsetx, float offsety)
        {
            float neg_x_scale = float.MinValue;
            float pos_x_scale = float.MaxValue;

            float neg_y_scale = float.MinValue;
            float pos_y_scale = float.MaxValue;

            float y_scalemodifier = 1;
            float x_scalemodifier = 1;

            Line closestline = new Line();

            foreach (var line in shapelines)
            {
                PointF distance = relativepoint.DistanceTo(line);

                if (distance.Y == 0)
                {
                    y_scalemodifier = 0.5f;
                }
                if (distance.X == 0)
                {
                    x_scalemodifier = 0.5f;
                }
                if (distance.Y > neg_y_scale && distance.Y < 0 && distance.Y != 0)
                {
                    neg_y_scale = distance.Y;
                }
                if (distance.Y < pos_y_scale && distance.Y > 0 && distance.Y != 0)
                {
                    pos_y_scale = distance.Y;
                }

                if (distance.X > neg_x_scale && distance.X < 0 && distance.X != 0)
                {
                    neg_x_scale = distance.X;
                }
                if (distance.X < pos_x_scale && distance.X > 0 && distance.X != 0)
                {
                    pos_x_scale = distance.X;
                }
            }

            float y_scale;
            float x_scale;
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

            float y_distancetocentre = Math.Abs(relativepoint.Y - radius);
            float turningtime = squaresize / 2f;
            const double cutoff = 0.7f;
            if (y_distancetocentre < turningtime)
            {
                //y_distancetocentre = y_scale < 0 ? -y_distancetocentre : y_distancetocentre;
                //Begin to approach a linear straight line at the centre
                if (neg_y_scale + pos_y_scale == 0)
                {
                    y_scale = 0;
                }
                else
                {
                    //Should equal zero when distancetocentre == 0
                    //Should equal 1 when distancetocentre == squaresize/5
                    if (y_distancetocentre >= turningtime * cutoff)
                    {
                        double a = 0.7f / Math.Pow(turningtime - (turningtime * cutoff), 2);
                        y_scale *= (float)(a * Math.Pow(y_distancetocentre - (turningtime * cutoff), 2) + 0.3f);
                        //y_scale *= (float)Math.Pow(Math.E, y_distancetocentre - (squaresize / 5));
                    }
                    else
                    {
                        //((0.7)/(3 a s)) x+1-((0.7)/(3 a s)) 
                        y_scale *= (float)((0.3f / (cutoff * turningtime)) * y_distancetocentre);
                        //y_scale *= (float)(y_distancetocentre * (Math.Pow(Math.E, ((squaresize / 5) * (1f / 6f)))/ ((squaresize / 5) * (5f / 6f))));
                    }
                    //y_scale *= (float)(Math.Log((y_distancetocentre+1f)/((squaresize/5f)/2f)));
                }
            }
            float x_distancetocentre = Math.Abs(relativepoint.X - radius);

            if (x_distancetocentre < turningtime)
            {
                //x_distancetocentre = x_scale < 0 ? -x_distancetocentre : x_distancetocentre;
                //Begin to approach a linear straight line at the centre
                if (neg_x_scale + pos_x_scale == 0)
                {
                    x_scale = 0;
                }
                else
                {
                    //Should equal zero when distancetocentre == 0
                    //Should equal 1 when distancetocentre == squaresize/5
                    if (x_distancetocentre >= turningtime * cutoff)
                    {
                        double a = 0.7f / Math.Pow(turningtime - (turningtime * cutoff), 2);
                        x_scale *= (float)(a * Math.Pow(x_distancetocentre - (turningtime * cutoff), 2) + 0.3f);
                        //x_scale *= (float)Math.Pow(Math.E, x_distancetocentre - (squaresize / 5));
                    }
                    else
                    {
                        //((0.7)/(3 a s)) x+1-((0.7)/(3 a s)) 
                        x_scale *= (float)((0.3f / (cutoff * turningtime)) * x_distancetocentre);
                        //x_scale *= (float)(x_distancetocentre * (Math.Pow(Math.E, ((squaresize / 5) * (1f / 6f)))/ ((squaresize / 5) * (5f / 6f))));
                    }
                    //x_scale *= (float)(Math.Log((x_distancetocentre+1f)/((squaresize/5f)/2f)));
                }
            }

            y_scale *= y_scalemodifier;
            x_scale *= x_scalemodifier;
            //sideconnections[x + y * volumewidth] = new Line(relativepoint, new PointF(x_scale + relativepoint.X, y_scale + relativepoint.Y)); //debugdata

            x_scale = (float)Math.Sin(x_scale / 20) / 2;
            y_scale = (float)Math.Sin(y_scale / 20) / 2;
            const float limiter = 0.5f;
            if (x_scale >= limiter)
            {
                x_scale = limiter;
            }
            if (x_scale <= -limiter)
            {
                x_scale = -limiter;
            }
            if (y_scale >= limiter)
            {
                y_scale = limiter;
            }
            if (y_scale <= -limiter)
            {
                y_scale = -limiter;
            }
            if (Math.Abs(relativepoint.X - radius) < squaresize && Math.Abs(relativepoint.Y - radius) < squaresize)
            {
                x_scale /= 2;
                y_scale /= 2;
            }

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
            ay += y_scale;
            ax += x_scale;
            ay *= squaresize;
            ax *= squaresize;

            return new PointF(ax, ay);
        }
    }
}
