using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace ImageStretcher
{
    internal static class Extensions
    {
        public static double DistanceTo(this PointF p, PointF destination)
        {
            if (p.X == destination.X && p.Y == destination.Y)
            {
                return 0.0001;
            }
            return Math.Sqrt(Math.Pow(destination.X - p.X, 2) + Math.Pow(destination.Y - p.Y, 2));
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
        public static bool InPolygon(this Point point, PointF[] polygon)
        {
            int polygonLength = polygon.Length, i = 0;
            bool inside = false;
            // x, y for tested point.
            float pointX = point.X, pointY = point.Y;
            // start / end point for the current polygon segment.
            float startX, startY, endX, endY;
            PointF endPoint = polygon[polygonLength - 1];
            endX = endPoint.X;
            endY = endPoint.Y;
            while (i < polygonLength)
            {
                startX = endX; startY = endY;
                endPoint = polygon[i++];
                endX = endPoint.X; endY = endPoint.Y;
                //
                inside ^= (endY > pointY ^ startY > pointY) /* ? pointY inside [startY;endY] segment ? */
                          && /* if so, test if it is under the segment */
                          ((pointX - endX) < (pointY - endY) * (startX - endX) / (startY - endY));
            }
            return inside;
        }

        public static PointF[] ScalePolygon(this PointF[] polygon, int scale)
        {
            PointF[] result = new PointF[polygon.Count()];
            for (int i = 0; i < polygon.Count(); ++i)
            {
                PointF centre = polygon[i];
                PointF left;
                if (i == 0)
                {
                    left = polygon[polygon.Count() - 1];
                }
                else
                {
                    left = polygon[i - 1];
                }

                PointF right;
                if (i == polygon.Count() - 1)
                {
                    right = polygon[0];
                }
                else
                {
                    right = polygon[i + 1];
                }

                //Let the origin = left
                Vector c = new Vector(right, left);
                var perpindicdular = c.GetPerpindicular().GetUnitVector() * scale;
                result[i] = new PointF((float)(centre.X + perpindicdular.i), (float)(centre.Y + perpindicdular.j));
            }
            return result;
        }

        public static string IterateString(this List<PointF> points)
        {
            string result = "";
            foreach(var point in points)
            {
                result += string.Format("({0},{1}),", point.X, point.Y);
            }
            return result;
        }
    }
}
