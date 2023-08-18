using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyperbolicRenderer
{
    public class Shape
    {
        public PointF[] points;
        public PointF centre;
        public double radius;
        
        public Shape(PointF[] points, PointF centre, float radius)
        {
            this.points = points;
            this.centre = centre;
            this.radius = radius;
        }
        public Shape(PointF[] points, PointF centre)
        {
            this.points = points;
            this.centre = centre;
        }


        public static Shape CreateShape(int points, float radius, PointF position)
        {
            PointF[] result = new PointF[points];
            double radiansstepsize = Math.Tau / points;
            for (int i = 0; i < points; ++i)
            {
                //Draw a ray from the centre until it hits the edge of the square
                //Make this a vector
                double angle = (3 * (Math.PI / 2)) + (radiansstepsize * i);

                float x = (float)(Math.Cos(angle) * radius);

                float y = (float)(Math.Sin(angle) * radius);
                result[i] = new PointF(x + position.X, y + position.Y);
            }
            return new Shape(result, position, radius);
        }
    }
}
