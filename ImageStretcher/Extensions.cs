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
