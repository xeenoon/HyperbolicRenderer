using HyperbolicRenderer;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace GameUI
{
    public class Shape
    {
        public Vector3[] points;
        public List<Vector3> polygonpoints;
        public Vector3 centre;
        public double radius;
        public Color color;

        public Shape(Vector3[] points, Vector3 centre, float radius, Color color)
        {
            this.points = points;
            this.centre = centre;
            this.radius = radius;
            this.color = color;
        }
        public Shape(Vector3[] points, Vector3 centre)
        {
            this.points = points;
            this.centre = centre;
        }


        public static Shape CreateShape(int points, float radius, Vector3 position, Color color, double offset = (3 * (Math.PI / 2)))
        {
            Vector3[] result = new Vector3[points];
            double radiansstepsize = Math.Tau / points;
            for (int i = 0; i < points; ++i)
            {
                //Draw a ray from the centre until it hits the edge of the square
                //Make this a vector
                double angle = offset + (radiansstepsize * i);

                float x = (float)(Math.Cos(angle) * radius);

                float y = (float)(Math.Sin(angle) * radius);
                result[i] = new Vector3(x + position.X, y + position.Y, 0);
            }
            return new Shape(result, position, radius, color);
        }
    }
}
