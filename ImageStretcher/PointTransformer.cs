using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace ImageStretcher
{
    public class PointTransformer
    {
        public PointF centre;
        System.Timers.Timer timer = new System.Timers.Timer();

        public double time = 0;
        public int width;
        public PolygonMenu polygonMenu;

        public PointTransformer(PointF centre, int width, PolygonMenu polygonMenu, bool usetimer = true)
        {
            this.polygonMenu = polygonMenu;
            this.centre = centre;
            Pause();
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
            time += increment;
        }
        public PointF TransformPoint(Point input)
        {
            if (time == 0)
            {
                return new Point(int.MinValue, int.MinValue);
            }
            if (polygonMenu != null && polygonMenu.menuItems.Count >= 1)
            {
                foreach (PolygonMenuItem menuItem in polygonMenu.menuItems)
                {
                    if (menuItem.polygonpoints.Count() >= 3)
                    {
                        if (input.InPolygon(menuItem.smallpoints)) //Scale normally
                        {
                            return TransformPolygon(menuItem, input);
                        }
                        else if (input.InPolygon(menuItem.polygonpoints.ToArray())) //Smoothe to edges
                        {
                            double mindistance = double.MaxValue;
                            for (int i = 0; i < menuItem.polygonpoints.Count(); ++i) 
                                //Can be optimized to only select ones where distancesquared < 100
                            {
                                PointF centre = menuItem.polygonpoints[i];
                                PointF left;
                                if (i == 0)
                                {
                                    left = menuItem.polygonpoints[menuItem.polygonpoints.Count() - 1];
                                }
                                else
                                {
                                    left = menuItem.polygonpoints[i - 1];
                                }

                                PointF right;
                                if (i == menuItem.polygonpoints.Count() - 1)
                                {
                                    right = menuItem.polygonpoints[0];
                                }
                                else
                                {
                                    right = menuItem.polygonpoints[i + 1];
                                }

                                mindistance = Math.Min(mindistance, input.DistanceToLine(centre, left));
                                mindistance = Math.Min(mindistance, input.DistanceToLine(centre, right)); 
                                //Calculate all the distances, select smallest one
                            }
                            if (mindistance > PolygonMenuItem.scalesize)
                            {

                            }
                            return TransformPolygon(menuItem, input, mindistance / PolygonMenuItem.scalesize);
                        }
                    }
                }
            }
            return new Point(int.MinValue,int.MinValue);
        }

        PointF TransformPolygon(PolygonMenuItem menuItem, Point input, double smoothe = 1)
        {
            if (menuItem.bakedjello == null)
            {
                BakeHeights(menuItem.period, menuItem.amplitude, menuItem.offset,
                    out menuItem.bakedjello);
            }
            switch (menuItem.stretchType)
            {
                case StretchType.Jello:
                    return MakeJello(input, menuItem, smoothe);
                case StretchType.RotateLeft:
                    return RotateLeft(input, menuItem.period, menuItem.amplitude, menuItem.offset, smoothe);
                case StretchType.RotateRight:
                    return RotateRight(input, menuItem.period, menuItem.amplitude, menuItem.offset, smoothe);
                case StretchType.Horizontal:
                    return HorizontalWave(input, menuItem.period, menuItem.amplitude, menuItem.offset, smoothe);
                case StretchType.Vertical:
                    return VerticalWave(input, menuItem.period, menuItem.amplitude, menuItem.offset, smoothe);
            }
            return input;
        }

        const float scale = 1000;
        public void BakeHeights(int period, double amplitude, double offset, out double[] bakedjello)
        {
            int length = (int)((scale) * Math.Tau);

            bakedjello = new double[length];

            for (int i = 0; i < length; ++i) //Go through all possible angles
            {
                bakedjello[i] = (Math.Sin(((i / scale) * period * 2) + offset) * amplitude) + (1 + amplitude);
            }
        }
        public PointF MakeJello(Point input, PolygonMenuItem menuItem, double smoothe)
        {
            PointF adjustedpoint = new PointF((input.X - centre.X), (input.Y - centre.Y));

            //Based on time, points will be scaled based on their angle to the centre
            double angle = Math.Atan(adjustedpoint.Y / adjustedpoint.X) + Math.PI / 2;
            if (double.IsNaN(angle))
            {
                angle = 0;
            }
            int idx = (int)((angle + (time/4)) * scale);
            while (idx >= Math.Floor(scale * Math.Tau))
            {
                idx -= (int)Math.Floor(scale * Math.Tau);
            }
            float heightmultiplier = (float)(menuItem.bakedjello[idx]);

            double dx = adjustedpoint.X - adjustedpoint.X * heightmultiplier;
            double dy = adjustedpoint.Y - adjustedpoint.Y * heightmultiplier;

            dx *= smoothe;
            dy *= smoothe;

            adjustedpoint.X += (float)dx;
            adjustedpoint.Y += (float)dy;

            adjustedpoint.X += centre.X;
            adjustedpoint.Y += centre.Y;

            return new PointF(adjustedpoint.X, adjustedpoint.Y);
        }
        public PointF RotateLeft(Point input, int period, double amplitude, double offset, double smoothe)
        {
            PointF adjustedpoint = new PointF((input.X - centre.X), (input.Y - centre.Y));
            //period defines speed
            //amplitude defines rotation amount
            amplitude = Math.Min(Math.PI/2, amplitude);
            double angle = PointManager.GetAngle(centre, input);

            float heightmultiplier = (float)((Math.Sin((angle * period * 2) + time + offset) * amplitude));

            angle += heightmultiplier;
            double radius = Math.Sqrt(adjustedpoint.X * adjustedpoint.X + adjustedpoint.Y * adjustedpoint.Y);

            double dx = 0;
            double dy = 0;

            if (!double.IsNaN(Math.Cos(angle)))
            {
                dx = Math.Cos(angle) * radius - adjustedpoint.X;
            }
            if (!double.IsNaN(Math.Sin(angle)))
            {
                dy = Math.Sin(angle) * radius - adjustedpoint.Y;
            }

            dx *= smoothe;
            dy *= smoothe;

            adjustedpoint.X += (float)dx;
            adjustedpoint.Y += (float)dy;

            adjustedpoint.X += centre.X;
            adjustedpoint.Y += centre.Y;

            return new PointF(adjustedpoint.X, adjustedpoint.Y);
        }
        public PointF RotateRight(Point input, int period, double amplitude, double offset, double smoothe)
        {
            PointF adjustedpoint = new PointF((input.X - centre.X), (input.Y - centre.Y));
            //period defines speed
            //amplitude defines rotation amount
            amplitude = Math.Min(Math.PI / 2, amplitude);
            double angle = PointManager.GetAngle(centre, input);
            float heightmultiplier = -(float)((Math.Sin((angle * period * 2) + time + offset) * amplitude));
            angle += heightmultiplier;
            double radius = Math.Sqrt(adjustedpoint.X * adjustedpoint.X + adjustedpoint.Y * adjustedpoint.Y);

            double dx = 0;
            double dy = 0;

            if (!double.IsNaN(Math.Cos(angle)))
            {
                dx = Math.Cos(angle) * radius - adjustedpoint.X;
            }
            if (!double.IsNaN(Math.Sin(angle)))
            {
                dy = Math.Sin(angle) * radius - adjustedpoint.Y;
            }

            dx *= smoothe;
            dy *= smoothe;

            adjustedpoint.X += (float)dx;
            adjustedpoint.Y += (float)dy;

            adjustedpoint.X += centre.X;
            adjustedpoint.Y += centre.Y;

            return new PointF(adjustedpoint.X, adjustedpoint.Y);
        }
        public PointF HorizontalWave(Point input, int period, double amplitude, double offset, double smoothe)
        {
            PointF adjustedpoint = new PointF((input.X - centre.X), (input.Y - centre.Y));

            float heightmultiplier = (float)(Math.Sin(time + offset) * amplitude + 1 + amplitude * smoothe);
            adjustedpoint.X += heightmultiplier;

            adjustedpoint.X += centre.X;
            adjustedpoint.Y += centre.Y;

            return new PointF(adjustedpoint.X, adjustedpoint.Y);
        }
        public PointF VerticalWave(Point input, int period, double amplitude, double offset, double smoothe)
        {
            PointF adjustedpoint = new PointF((input.X - centre.X), (input.Y - centre.Y));

            adjustedpoint.Y += (float)(Math.Sin(time + offset) * amplitude + 1 + amplitude * smoothe);

            adjustedpoint.X += centre.X;
            adjustedpoint.Y += centre.Y;

            return new PointF(adjustedpoint.X, adjustedpoint.Y);
        }
        double increment = 0.5;
        internal void Pause()
        {
            increment = 0;
        }

        internal void Restart()
        {
            increment = 0.5f;
        }
    }
}