using HyperbolicRenderer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageStretcher
{
    internal static class Extensions
    {
        public static PointF DistanceTo(this PointF p, Line line)
        {
            Vector normalvector = new Vector(line.start, line.end);
            Vector perpindicular = normalvector.GetPerpindicular();
            perpindicular.A = p;
            perpindicular.CreateVectorline();

            PointF intersection = normalvector.Intersection(perpindicular, p);
            intersection = new PointF(intersection.X - p.X, intersection.Y - p.Y);
            return intersection;
        }
        public static bool InPolygon(this PointF testPoint, PointF[] polygon)
        {
            bool result = false;
            int j = polygon.Length - 1;
            for (int i = 0; i < polygon.Length; i++)
            {
                if (polygon[i].Y < testPoint.Y && polygon[j].Y >= testPoint.Y ||
                    polygon[j].Y < testPoint.Y && polygon[i].Y >= testPoint.Y)
                {
                    if (polygon[i].X + (testPoint.Y - polygon[i].Y) /
                       (polygon[j].Y - polygon[i].Y) *
                       (polygon[j].X - polygon[i].X) < testPoint.X)
                    {
                        result = !result;
                    }
                }
                j = i;
            }
            return result;
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
}
