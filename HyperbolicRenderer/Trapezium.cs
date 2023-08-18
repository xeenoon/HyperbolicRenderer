using System.Diagnostics;

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
        public List<PointF> polygonpoints = new List<PointF>();
        public PointF[] points
        {
            get
            {
                return new PointF[4] {top_left, top_right, bottom_right, bottom_left};
            }
        }
        internal void Draw(BMP image, Graphics outerlayer, bool curved, Color color, Map map, bool fill=true)
        {
            polygonpoints.Clear();
            if (curved)
            {
                var rightdistance = bottom_right.Y - top_right.Y;
                var topdistance = top_right.X - top_left.X;

                double t_m = (top_right.Y - top_left.Y) / topdistance;
                double t_c = top_right.Y - t_m * top_right.X;

                double r_m = rightdistance / (bottom_right.X - top_right.X);
                double r_c = bottom_right.Y - r_m * bottom_right.X;


                polygonpoints.AddRange(CurvedShape.SinCurvePoints(top_left, top_right, map));
                polygonpoints.AddRange(CurvedShape.SinCurvePoints(top_right, bottom_right, map));
                polygonpoints.AddRange(CurvedShape.SinCurvePoints(bottom_left, bottom_right, map).Reverse());
                polygonpoints.AddRange(CurvedShape.SinCurvePoints(top_left, bottom_left, map).Reverse());
            }
            else
            {
                s.Restart();
                CurvedShape.DrawLine(top_left, top_right, true, map.radius * 2, image, color);
                CurvedShape.DrawLine(top_right, bottom_right, false, map.radius * 2, image, color);
                s.Stop();
                elapseddrawtime += s.ElapsedTicks;
            }

            if (polygonpoints.Count() >= 3)
            {
                if (fill)
                {
                    outerlayer.FillPolygon(new Pen(color).Brush, polygonpoints.ToArray());
                }
                else
                {
                    outerlayer.DrawPolygon(new Pen(color), polygonpoints.ToArray());
                }
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
        
    }
    public class CurvedShape
    {
        PointF[] originalpoints;
        PointF[] adjustedpoints;
        public CurvedShape(PointF[] points)
        {
            originalpoints = points;
        }
        public void Draw(Graphics graphics, Color color, int mapsize, List<Trapezium> trapeziums)
        {
            for (int i = 0; i < originalpoints.Count(); ++i)
            {
                PointF point = originalpoints[i];
                Trapezium background = null;
                foreach (var trapezium in trapeziums)
                {
                    if (point.InPolygon(trapezium.polygonpoints.ToArray()))
                    {
                        background = trapezium;
                        break;
                    }
                }

                List<Line> lines = new List<Line>() { new Line(background.top_left, background.top_right) , new Line(background.top_right, background.bottom_right), new Line(background.bottom_right, background.bottom_left), new Line(background.bottom_left, background.top_left)};
                double mindistance = double.MaxValue;
                Line closestline = lines[0];
                foreach (var line in lines)
                {
                    double distance = point.DistanceTo(line).Magnitude();
                    if (distance < mindistance)
                    {
                        mindistance = distance;
                        closestline = line;
                    }
                }
                var end = closestline.end;
                var start = closestline.start;

                //var curve = AdjustPoint(closestline.start, closestline.end, mapsize, point);
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

                if (i == distance-1)
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
