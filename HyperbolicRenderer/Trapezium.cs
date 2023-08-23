using Microsoft.Xna.Framework;
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
        public Trapezium()
        {

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
        public void Draw(Graphics graphics, bool curved, System.Drawing.Color color, Map map, bool fill=true)
        {
            if (top_left.X > map.radius * 2 || top_right.X < 0 || top_left.Y < 0 || bottom_left.Y > map.radius * 2)
            {
                return;
            }

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

            List<PointF> finalpoints = new List<PointF>();
            foreach (PointF p in polygonpoints)
            {
                if (p.X > 0 && p.X < map.radius*2 && p.Y > 0 && p.Y < map.radius*2)
                {
                    finalpoints.Add(p);
                }
            }

            if (finalpoints.Count() >= 3)
            {
                if (fill)
                {
                    graphics.FillPolygon(new Pen(color).Brush, finalpoints.ToArray());
                }
                else
                {
                    graphics.DrawPolygon(new Pen(color), finalpoints.ToArray());
                }
            }
        }

        public Vector3[] GetPoints(Map map)
        {
            if (top_left.X > map.radius*2 || top_right.X < 0 || top_left.Y < 0 || bottom_left.Y > map.radius*2)
            {
                return Array.Empty<Vector3>();
            }

            List<Vector3> vertices = new List<Vector3>();

            vertices.AddRange(Shape.SinCurvePoints(top_left.ToVector(), top_right.ToVector(), map));
            vertices.AddRange(Shape.SinCurvePoints(top_right.ToVector(), bottom_right.ToVector(), map));
            vertices.AddRange(Shape.SinCurvePoints(bottom_left.ToVector(), bottom_right.ToVector(), map).Reverse());
            vertices.AddRange(Shape.SinCurvePoints(top_left.ToVector(), bottom_left.ToVector(), map).Reverse());


            List<Vector3> finalpoints = new List<Vector3>();
            foreach (Vector3 p in vertices)
            {
                if (p.X > 0 && p.X < map.radius * 2 && p.Y > 0 && p.Y < map.radius * 2)
                {
                    finalpoints.Add(p);
                }
            }

            return finalpoints.ToArray();

           // return finalpoints.ToArray();
        }
    }
}
