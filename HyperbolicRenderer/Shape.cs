using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyperbolicRenderer
{
    public class Shape
    {
        public PointF[] points;
        public List<PointF> polygonpoints;
        public PointF centre;
        public double radius;
        
        public Shape(PointF[] points, PointF centre, float radius)
        {
            this.points = points;
            this.centre = centre;
            this.radius = radius;
        }
        public Shape(PointF[] points, PointF centre)
        {
            this.points = points;
            this.centre = centre;
        }


        public static Shape CreateShape(int points, float radius, PointF position)
        {
            PointF[] result = new PointF[points];
            double radiansstepsize = Math.Tau / points;
            for (int i = 0; i < points; ++i)
            {
                //Draw a ray from the centre until it hits the edge of the square
                //Make this a vector
                double angle = (3 * (Math.PI / 2)) + (radiansstepsize * i);

                float x = (float)(Math.Cos(angle) * radius);

                float y = (float)(Math.Sin(angle) * radius);
                result[i] = new PointF(x + position.X, y + position.Y);
            }
            return new Shape(result, position, radius);
        }

        public void Draw(Graphics graphics, Color color, Map map)
        {
            polygonpoints = new List<PointF>();
            for (int i = 0; i < points.Count(); ++i)
            {
                PointF start = points[i];
                PointF end;
                if (i >= points.Count() - 1)
                {
                    end = points[0];
                }
                else
                {
                    end = points[i + 1];
                }
                if (i > points.Count() / 4 && i < points.Count() * (3f/4f))
                {
                    //Switch the points and reverse
                    polygonpoints.AddRange(SinCurvePoints(end, start, map).Reverse());
                }
                else
                {
                    polygonpoints.AddRange(SinCurvePoints(start, end, map));
                }
            }
            if (polygonpoints.Count() >= 3)
            {
                graphics.FillPolygon(new Pen(color).Brush, polygonpoints.ToArray());
            }
        }
        public static PointF[] SinCurvePoints(PointF start, PointF end, Map map)
        {
            double mapsize = map.radius * 2;
            if (start.X < 0 || start.X > mapsize || start.Y < 0 || start.Y > mapsize)
            {
                return new PointF[0];
            }

            bool horizontal;
            double distance;
            double m;
            double c;
            int startidx;

            if (end.X - start.X > end.Y - start.Y)
            {
                startidx = (int)start.X;
                horizontal = true;
                distance = end.X - start.X;
                m = (end.Y - start.Y) / distance;
                c = end.Y - m * end.X;
            }
            else
            {
                startidx = (int)start.Y;
                horizontal = false;
                distance = end.Y - start.Y;
                m = distance / (end.X - start.X);
                c = end.Y - m * end.X;
            }

            PointF[] polygonpoints = new PointF[(int)Math.Ceiling(distance)];
            double a = Math.PI / (distance);
            distance = Math.Ceiling(distance);
            for (float i = 0; i < distance; ++i)
            {
                int workingvar = (int)(i + startidx);


                double normalheight;
                if (horizontal)
                {
                    normalheight = m * workingvar + c;
                }
                else
                {
                    normalheight = (workingvar - c) / m; //Find the height if it was a straight line
                }


                if (double.IsNaN(normalheight))
                {
                    normalheight = start.X;
                }
                PointF workingpoint;
                if (horizontal)
                {
                    workingpoint = new PointF(workingvar, (float)normalheight);
                }
                else
                {
                    workingpoint = new PointF((float)normalheight, workingvar);
                }
                workingpoint = map.SinScale(0.6f, workingpoint);


                double sin_height;
                if (horizontal)
                {
                    sin_height = workingpoint.Y;
                }
                else
                {
                    sin_height = workingpoint.X;
                }


                //f: y=-((1)/(25)) (x-5)^(2)+1
                double scaleamount = 3;
                double axisdist = Math.Abs(normalheight - map.radius);
                double y = (-(scaleamount / Math.Pow(distance / 2, 2)) * Math.Pow((i - (distance / 2)), (2))) + scaleamount;

                sin_height *= y;


                //Use pythag to get distance to centre

                sin_height = axisdist < 0 ? -sin_height : sin_height;

                double scalingfactor = Math.Abs(axisdist) / (mapsize / 2);
                scalingfactor = Math.Log(scalingfactor + 1) * 0.5f;
                scalingfactor = axisdist > 0 ? scalingfactor : -scalingfactor;

                int curveheight = (int)(sin_height * (distance) * scalingfactor + normalheight);
                curveheight = (int)Math.Min(curveheight, mapsize - 1);
                workingvar = (int)Math.Min(workingvar, mapsize - 1);
                workingvar = (int)Math.Max(workingvar, 0);
                curveheight = (int)Math.Max(curveheight, 0);

                if (i == distance - 1)
                {
                    curveheight = (int)normalheight;
                }

                if (horizontal)
                {
                    polygonpoints[(int)i] = new PointF(workingvar, curveheight);
                }
                else
                {
                    polygonpoints[(int)i] = new PointF(curveheight, workingvar);
                }
            }

            return polygonpoints;
        }
        public static void DrawLine(PointF start, PointF end, bool horizontal, double mapsize, BMP image, Color color)
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
                    workingvar = (int)(i + start.Y);
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
}
