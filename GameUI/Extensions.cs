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
    }
}
