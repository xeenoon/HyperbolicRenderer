using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageStretcher
{
    internal static class PointManager
    {
        static List<PointF> movedpoints = new List<PointF>();
        public static void GrahamsAlgorithm(PointF last, List<PointF> points, ref List<PointF> result)
        {
            if (result.Count() == 0)
            {
                movedpoints.Clear();
                result = new List<PointF>();
            }
            movedpoints.Add(last);
            result.Add(last);

            List<PointF> nearbypoints = new List<PointF>();
            double newresolution = 1;
            while (nearbypoints.Count() == 0)
            {
                nearbypoints = points.Where(p => p.DistanceTo(last) < newresolution && !movedpoints.Contains(p)).ToList();
                newresolution++;
                const int RECURSIONLIMIT = 1000;
                if (newresolution > RECURSIONLIMIT)
                {
                    return;
                }
            }

            double smallestangle = double.MaxValue;
            PointF closestpoint = new PointF();
            foreach (var point in nearbypoints)
            {
                double angle = GetAngle(last, point);
                if (angle < smallestangle)
                {
                    smallestangle = angle;
                    closestpoint = point;
                }
            }
            if (result.Count() > points.Count() * 0.6f && last.DistanceTo(points[0]) < points.Min(p => p != points[0] ? p.DistanceTo(points[0]) : double.MaxValue))
            {
                return;
            }
            GrahamsAlgorithm(closestpoint, points, ref result);
        }
        private static double GetAngle(PointF start, PointF end)
        {
            float ychange = -(end.Y - start.Y);
            float xchange = (end.X - start.X);

            double angle = Math.Atan(Math.Abs(ychange / xchange));

            //Check each quadrant
            if (ychange >= 0 && xchange >= 0) //NE stays the same
            {

            }
            else if (ychange < 0 && xchange > 0) //SE
            {
                angle = Math.PI + angle;
            }
            else if (ychange < 0 && xchange < 0) //SW
            {
                angle = Math.PI + angle;
            }
            else if (ychange > 0 && xchange < 0) //NW
            {
                angle = Math.PI - angle;
            }

            if (ychange == 0 && xchange > 0)
            {
                angle = Math.PI / 2;
            }
            else if (ychange == 0 && xchange < 0)
            {
                angle = 3 * Math.PI / 2;
            }
            else if (xchange == 0 && ychange < 0)
            {
                angle = Math.PI;
            }

            return angle;
        }
    }
}
