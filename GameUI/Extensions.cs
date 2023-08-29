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
    }
}
