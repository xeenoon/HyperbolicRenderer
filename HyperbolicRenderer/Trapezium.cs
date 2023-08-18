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


                polygonpoints.AddRange(Shape.SinCurvePoints(top_left, top_right, map));
                polygonpoints.AddRange(Shape.SinCurvePoints(top_right, bottom_right, map));
                polygonpoints.AddRange(Shape.SinCurvePoints(bottom_left, bottom_right, map).Reverse());
                polygonpoints.AddRange(Shape.SinCurvePoints(top_left, bottom_left, map).Reverse());
            }
            else
            {
                s.Restart();
                Shape.DrawLine(top_left, top_right, true, map.radius * 2, image, color);
                Shape.DrawLine(top_right, bottom_right, false, map.radius * 2, image, color);
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
    }
}
