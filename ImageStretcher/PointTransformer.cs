using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageStretcher
{
    public class PointTransformer
    {
        PointF centre;
        System.Timers.Timer timer = new System.Timers.Timer();

        public double time = 0;
        public double period = 2;
        public double amplitude = 0.05;
        public double speed = 1;
        public PointTransformer(PointF centre, bool usetimer = true)
        {
            this.centre = centre;
            if (usetimer)
            {
                timer.Interval = 100;
                timer.Elapsed += new System.Timers.ElapsedEventHandler(Update);
                timer.Start();
            }
        }
        public void Update(object sender, System.Timers.ElapsedEventArgs e)
        {
            time += 0.5f;
        }
        public Point JelloTransformPoint(Point input)
        {
            PointF adjustedpoint = new PointF((input.X - centre.X), (input.Y - centre.Y));

            //Based on time, points will be scaled based on their angle to the centre
            double angle = Math.Atan(adjustedpoint.Y / adjustedpoint.X) + Math.PI / 2;
            float heightmultiplier = (float)((Math.Sin((angle * period * 2) + (time * speed))) * amplitude) + 1;
            adjustedpoint.X *= heightmultiplier;
            adjustedpoint.Y *= heightmultiplier;

            adjustedpoint.X += centre.X;
            adjustedpoint.Y += centre.Y;

            return new Point((int)adjustedpoint.X, (int)adjustedpoint.Y);
        }

        public Point RightDeform(Point input)
        {
            PointF adjustedpoint = new PointF((input.X - centre.X), (input.Y - centre.Y));

            if (adjustedpoint.X > 0 && adjustedpoint.Y < 0) //Only move points on rhs
            {
                double change = (time % 20);

                if (change > 10)
                {
                    change = 10 - (change - 10);
                }
                change *= 0.05f;
                adjustedpoint.X *= (float)(1 + change);
            }

            adjustedpoint.X += centre.X;
            adjustedpoint.Y += centre.Y;

            return new Point((int)adjustedpoint.X, (int)adjustedpoint.Y);
        }
    }
}