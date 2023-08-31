using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Runtime.CompilerServices;
using Microsoft.Xna.Framework;

namespace GameUI
{
    internal static class Extensions
    {
        public static PointF ToPoint(this Vector3 pos)
        {
            return new PointF(pos.X, pos.Y);
        }
        public static Vector3 ToVector(this PointF point)
        {
            return new Vector3(point.X, point.Y, 0);
        }

        public static bool InPolygon(this Vector2 testPoint, Vector2[] polygon)
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

        public static List<Vector2> Copy(this IEnumerable<Vector2> s)
        {
            List<Vector2> result = new List<Vector2>();
            foreach (Vector2 v in s)
            {
                result.Add(new Vector2(v.X, v.Y));
            }
            return result;
        }

        public static Vector2 Rotate(this Vector2 v, float centrex, float centrey, double angle)
        {
            double s = Math.Sin(angle);
            double c = Math.Cos(angle);

            // translate point back to origin:
            v.X -= centrex;
            v.Y -= centrey;

            // rotate point
            double xnew = v.X * c - v.Y * s;
            double ynew = v.X * s + v.Y * c;

            // translate point back:
            v.X = (float)(xnew + centrex);
            v.Y = (float)(ynew + centrey);
            return v;
        }
        public static Vector3 Rotate(this Vector3 v, float centrex, float centrey, double angle)
        {
            double s = Math.Sin(angle);
            double c = Math.Cos(angle);

            // translate point back to origin:
            v.X -= centrex;
            v.Y -= centrey;

            // rotate point
            double xnew = v.X * c - v.Y * s;
            double ynew = v.X * s + v.Y * c;

            // translate point back:
            v.X = (float)(xnew + centrex);
            v.Y = (float)(ynew + centrey);
            return v;
        }
    }
}
