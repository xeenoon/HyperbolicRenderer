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
        public int width;
        public PointTransformer(PointF centre, int width, bool usetimer = true)
        {
            this.centre = centre;
            if (usetimer)
            {
                timer.Interval = 100;
                timer.Elapsed += new System.Timers.ElapsedEventHandler(Update);
                timer.Start();
            }

            this.width = width;
        }
        public void Update(object sender, System.Timers.ElapsedEventArgs e)
        {
            time += 0.5f;
        }
        public Point TransformPoint(Point input)
        {
            return MakeJello(input);
            //return WaveArms(input);
            //return WaveHead(input);
        }

        public Point MakeJello(Point input)
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




        public static PointF[] bobsrightarm = new PointF[16] { new PointF(206, 3), new PointF(199, 6), new PointF(188, 11), new PointF(187, 22), new PointF(200, 38), new PointF(200, 50), new PointF(212, 71), new PointF(218, 98), new PointF(224, 120), new PointF(264, 114), new PointF(256, 80), new PointF(248, 60), new PointF(239, 36), new PointF(230, 18), new PointF(222, 11), new PointF(213, 4), }; public static PointF[] bobsleftarm = new PointF[11] { new PointF(70, 132), new PointF(54, 138), new PointF(48, 151), new PointF(52, 168), new PointF(60, 179), new PointF(68, 186), new PointF(73, 195), new PointF(84, 204), new PointF(93, 190), new PointF(92, 187), new PointF(94, 178), }; 

        public Point WaveArms(Point input)
        {
            PointF adjustedpoint = new PointF((input.X - centre.X), (input.Y - centre.Y));
            double angle = Math.Atan(adjustedpoint.Y / adjustedpoint.X);

            if (input.InPolygon(bobsrightarm))
            {
                //smaller angles turn more
                //shorter angles turn less

                double radius = adjustedpoint.DistanceTo(new PointF(0, 0));
                float heightmultiplier = (float)((Math.Sin((angle * period * 2) + (time * speed)) * amplitude) + (1 + amplitude));
                angle *= heightmultiplier;

                var xscale = (float)Math.Cos(angle);
                var yscale = (float)Math.Sin(angle);

                //transform more for more distances
                adjustedpoint.X = (float)(xscale * radius);
                adjustedpoint.Y = (float)(yscale * radius);
            }



            adjustedpoint.X += centre.X;
            adjustedpoint.Y += centre.Y;

            return new Point((int)adjustedpoint.X, (int)adjustedpoint.Y);
        }
        public static PointF[] bobshead = new PointF[21] { new PointF(28, 3), new PointF(87, 7), new PointF(158, 7), new PointF(277, 7), new PointF(324, 14), new PointF(238, 170), new PointF(210, 188), new PointF(197, 212), new PointF(170, 221), new PointF(137, 219), new PointF(112, 199), new PointF(93, 183), new PointF(86, 178), new PointF(82, 173), new PointF(83, 165), new PointF(85, 158), new PointF(77, 149), new PointF(74, 137), new PointF(72, 132), new PointF(64, 121), new PointF(61, 99), };
        public Point WaveHead(Point input)
        {
            PointF adjustedpoint = new PointF((input.X - centre.X), (input.Y - centre.Y));

            if (input.InPolygon(bobshead))
            {
                const double lowestpoint = 100;
                double bottomdist = Math.Pow(Math.Max((lowestpoint - adjustedpoint.Y),0),2) / 100;

                adjustedpoint.X += (float)(Math.Sin(time*speed)*amplitude*bottomdist + 1 + amplitude);
            }

            adjustedpoint.X += centre.X;
            adjustedpoint.Y += centre.Y;

            return new Point((int)adjustedpoint.X, (int)adjustedpoint.Y);
        }
    }
}