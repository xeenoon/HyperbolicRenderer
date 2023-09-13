using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
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
        public Point WaveDeform(Point input)
        {
            PointF adjustedpoint = new PointF((input.X - centre.X), (input.Y - centre.Y));

            //Based on time, points will be scaled based on their angle to the centre
            double angle = Math.Atan(adjustedpoint.Y / adjustedpoint.X) + Math.PI / 2;
            float heightmultiplier = (float)((Math.Sin((angle * period * 2) + (time * speed)) * amplitude) + (1+amplitude));
            
            adjustedpoint.X *= heightmultiplier;
            adjustedpoint.Y *= heightmultiplier;

            adjustedpoint.X += centre.X;
            adjustedpoint.Y += centre.Y;

            return new Point((int)adjustedpoint.X, (int)adjustedpoint.Y);
        }


        public static PointF[] bobsrightarm = new PointF[7] { new PointF(246, 6), new PointF(194, 8), new PointF(185, 12), new PointF(184, 17), new PointF(181, 18), new PointF(213, 76), new PointF(233, 114), };
        public static PointF[] bobsleftarm = new PointF[12] { new PointF(73, 131), new PointF(64, 131), new PointF(34, 139), new PointF(45, 173), new PointF(53, 182), new PointF(70, 197), new PointF(86, 201), new PointF(93, 184), new PointF(87, 178), new PointF(79, 171), new PointF(80, 165), new PointF(83, 159), };
        public Point WaveArms(Point input)
        {
            PointF adjustedpoint = new PointF((input.X - centre.X), (input.Y - centre.Y));

            if (input.InPolygon(bobsrightarm) || input.InPolygon(bobsleftarm)) //Only move points on rhs
            {
                double angle = Math.Atan(adjustedpoint.Y / adjustedpoint.X) + Math.PI / 2;
                float heightmultiplier = (float)((Math.Sin((angle * period * 2) + (time * speed))) * amplitude * 2) + 1;
                adjustedpoint.X *= heightmultiplier;
                adjustedpoint.Y *= heightmultiplier;
            }

            adjustedpoint.X += centre.X;
            adjustedpoint.Y += centre.Y;

            return new Point((int)adjustedpoint.X, (int)adjustedpoint.Y);
        }
    }
}