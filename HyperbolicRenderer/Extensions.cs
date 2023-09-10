using ManagedCuda.BasicTypes;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace HyperbolicRenderer
{
    public static class Extensions
    {
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
        public static PointF DistanceTo(this PointF p, Line line)
        {
            var A = p.X - line.start.X;
            var B = p.Y - line.start.Y;
            var C = line.end.X - line.start.X;
            var D = line.end.Y - line.start.Y;

            var dot = A * C + B * D;
            var len_sq = C * C + D * D;
            float param = -1;
            if (len_sq != 0) //in case of 0 length line
                param = dot / len_sq;

            float xx, yy;

            if (param < 0)
            {
                xx = line.start.X;
                yy = line.start.Y;
            }
            else if (param > 1)
            {
                xx = line.end.X;
                yy = line.end.Y;
            }
            else
            {
                xx = line.start.X + param * C;
                yy = line.start.Y + param * D;
            }

            var dx = p.X - xx;
            var dy = p.Y - yy;
            return new PointF(dx, dy);
        }
        public static double DistanceTo(this PointF p, PointF destination)
        {
            if (p.X == destination.X && p.Y == destination.Y)
            {
                return 0.0001;
            }
            return Math.Sqrt(Math.Pow(destination.X - p.X,2) + Math.Pow(destination.Y - p.Y, 2));
        }
        public static double Magnitude(this PointF p)
        {
            return Math.Sqrt(p.X * p.X + p.Y * p.Y);
        }

        public static PointF ToPoint(this Microsoft.Xna.Framework.Vector3 pos)
        {
            return new PointF(pos.X, pos.Y);
        }
        public static Microsoft.Xna.Framework.Vector3 ToVector(this PointF point)
        {
            return new Microsoft.Xna.Framework.Vector3(point.X, point.Y, 0);
        }
    }
}
