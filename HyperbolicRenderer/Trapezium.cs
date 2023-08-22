﻿using System.Diagnostics;

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
        internal void Draw(Graphics graphics, bool curved, Color color, Map map, bool fill=true)
        {
            graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            polygonpoints.Clear();
            if (curved)
            {
                polygonpoints.AddRange(Shape.SinCurvePoints(top_left, top_right, map));
                polygonpoints.AddRange(Shape.SinCurvePoints(top_right, bottom_right, map));
                polygonpoints.AddRange(Shape.SinCurvePoints(bottom_left, bottom_right, map).Reverse());
                polygonpoints.AddRange(Shape.SinCurvePoints(top_left, bottom_left, map).Reverse());
            }
            else
            {
                s.Restart();
                Shape.DrawLine(top_left, top_right, true, map.radius * 2, color, graphics);
                Shape.DrawLine(top_right, bottom_right, false, map.radius * 2, color, graphics);
                s.Stop();
                elapseddrawtime += s.ElapsedTicks;
            }

            if (polygonpoints.Count() >= 3)
            {
                if (fill)
                {
                    graphics.FillPolygon(new Pen(color).Brush, polygonpoints.ToArray());
                }
                else
                {
                    graphics.DrawPolygon(new Pen(color), polygonpoints.ToArray());
                }
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
