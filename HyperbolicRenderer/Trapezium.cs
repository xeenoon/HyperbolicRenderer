using System.Diagnostics;

namespace HyperbolicRenderer
{
    public class Trapezium : CurvedShape
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
                var rightdistance = bottom_right.Y - top_right.Y;
                var topdistance = top_right.X - top_left.X;

                double t_m = (top_right.Y - top_left.Y) / topdistance;
                double t_c = top_right.Y - t_m * top_right.X;

                double r_m = rightdistance / (bottom_right.X - top_right.X);
                double r_c = bottom_right.Y - r_m * bottom_right.X;

                if (fill)
                {
                    polygonpoints.AddRange(CurvePoints(top_left, top_right, mapsize));
                    polygonpoints.AddRange(CurvePoints(top_right, bottom_right, mapsize));
                    polygonpoints.AddRange(CurvePoints(bottom_left, bottom_right, mapsize).Reverse());
                    polygonpoints.AddRange(CurvePoints(top_left, bottom_left, mapsize).Reverse());
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
        
    }
    public class CurvedShape
    {
        enum Side
        {
            MostlyHorizontal,
            MostlyVertical,
        }
        internal PointF[] CurvePoints(PointF start, PointF end, double mapsize)
        {
            if (start.X < 0 || start.X > mapsize || start.Y < 0 || start.Y > mapsize)
            {
                return new PointF[0];
            }

            Side side;
            double distance;
            double m;
            double c;
            int startidx;

            if (end.X - start.X > end.Y-start.Y)
            {
                startidx = (int)start.X;
                side = Side.MostlyHorizontal;
                distance = end.X - start.X;
                m = (end.Y - start.Y) / distance;
                c = end.Y - m * end.X;
            }
            else
            {
                startidx = (int)start.Y;
                side = Side.MostlyVertical;
                distance = end.Y - start.Y;
                m = distance / (end.X - start.X);
                c = end.Y - m * end.X;
            }

            PointF[] polygonpoints = new PointF[(int)Math.Ceiling(distance)];
            double a = Math.PI / (distance);

            for (float i = 0; i < distance; ++i)
            {
                double sin_height = Math.Sin(a * i); //Expressed as a percentage of the new height, pi/2 gets to next period to curve upwards
                int workingvar = (int)(i + startidx);


                double normalheight;
                if (side == Side.MostlyHorizontal)
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

                //Use pythag to get distance to centre

                double axisdist = normalheight - (mapsize / 2);

                double scalingfactor = Math.Abs(axisdist) / (mapsize / 2);
                scalingfactor = Math.Log(scalingfactor + 1) * 0.5f;
                scalingfactor = axisdist > 0 ? scalingfactor : -scalingfactor;

                int curveheight = (int)(sin_height * (distance) * scalingfactor + normalheight);
                curveheight = (int)Math.Min(curveheight, mapsize - 1);
                workingvar = (int)Math.Min(workingvar, mapsize - 1);
                workingvar = (int)Math.Max(workingvar, 0);
                curveheight = (int)Math.Max(curveheight, 0);

                if (side == Side.MostlyHorizontal)
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
        internal void DrawLine(PointF start, PointF end, bool horizontal, double mapsize, BMP image, Color color)
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
