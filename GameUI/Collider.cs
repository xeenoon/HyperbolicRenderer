using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameUI
{
    public class Collider
    {
        public List<Vector2> points = new List<Vector2>();
        public Vector2 location;
        double longestdistance = double.NegativeInfinity;
        public Func<bool> OnCollision;
        public static List<Collider> colliders = new List<Collider>();

        public Collider(List<Vector2> points, Func<bool> OnCollision, Vector2 centre)
        {
            this.points = points;
            this.OnCollision = OnCollision;
            location = centre;

            foreach (var point in points)
            {
                longestdistance = Math.Max(longestdistance, Vector2.Distance(centre, point));
            }
        }

        struct Collision
        {
            internal Collider a;
            internal Collider b;

            public Collision(Collider a, Collider b)
            {
                this.a = a;
                this.b = b;
            }
        }
        public static void Update()
        {
            //Potential collisions will only occur if the centres of each object are in the range of:
            //obj1.longestdistance + obj2.longestdistance
            //Since no points are further away from the centre, remove all colliders which are too far away

            List<Collision> collisions = new List<Collision>();

            foreach (var collider in colliders)
            {
                foreach (var potentialcollision in colliders)
                {
                    if (Vector2.Distance(potentialcollision.location, collider.location) < potentialcollision.longestdistance + collider.longestdistance)
                    {
                        if (!collisions.Any(c=>c.a==potentialcollision && c.b == collider))
                        {
                            foreach (Vector2 point in collider.points)
                            {
                                if (point.InPolygon(potentialcollision.points.ToArray()))
                                {
                                    //There is a collision, add it
                                    collisions.Add(new Collision(collider, potentialcollision));
                                    collider.OnCollision();
                                    potentialcollision.OnCollision();
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
