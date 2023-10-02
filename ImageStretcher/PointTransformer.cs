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
        public Point TransformPoint(Point input)
        {
            if (time == 0)
            {
                return input;
            }
            if(polygonMenu != null && polygonMenu.menuItems.Count >= 1)
            {
                foreach (PolygonMenuItem menuItem in polygonMenu.menuItems)
                {
                    if (menuItem.polygonpoints.Count() >= 3 && input.InPolygon(menuItem.polygonpoints.ToArray()))
                    {
                        if (menuItem.bakedjello == null)
                        {
                            BakeHeights(menuItem.period, menuItem.amplitude, menuItem.offset, 
                                out menuItem.bakedjello, 
                                out menuItem.bakedrotations);
                        }
                        switch (menuItem.stretchType)
                        {
                            case StretchType.Jello:
                                return MakeJello(input, menuItem);
                            case StretchType.RotateLeft:
                                return RotateLeft(input, menuItem.period, menuItem.amplitude, menuItem.offset);
                            case StretchType.RotateRight:
                                return RotateRight(input, menuItem.period, menuItem.amplitude, menuItem.offset);
                            case StretchType.Horizontal:
                                double maxy = menuItem.polygonpoints.Max(p => p.Y);
                                maxy -= centre.Y;

                                double miny = menuItem.polygonpoints.Min(p => p.Y);
                                miny -= centre.Y;
                                return HorizontalWave(input, maxy, miny, menuItem.period, menuItem.amplitude, menuItem.offset);
                            case StretchType.Vertical:
                                double maxx = menuItem.polygonpoints.Max(p => p.X);
                                maxx -= centre.X;

                                double minx = menuItem.polygonpoints.Min(p => p.X);
                                minx -= centre.X;
                                return VerticalWave(input, maxx, minx, menuItem.period, menuItem.amplitude, menuItem.offset);
                        }
                    }
                }
            }
            return input;
        }

        const float scale = 100;
        public void BakeHeights(int period, double amplitude, double offset, out double[] bakedjello, out double[] bakedrotations)
        {
            int length = (int)((scale) * Math.Tau);

            bakedjello = new double[length];
            bakedrotations = new double[length];

            for (int i = 0; i < length; ++i) //Go through all possible angles
            {
                bakedjello[i] = (Math.Sin(((i / scale) * period * 2) + offset) * amplitude) + (1 + amplitude);
            }
            for (int i = 0; i < length; ++i) //Go through all possible angles
            {
                bakedrotations[i] = Math.Sin(((i / scale) * period * 2) + offset) * amplitude;
            }
        }
        public Point MakeJello(Point input, PolygonMenuItem menuItem)
        {
            PointF adjustedpoint = new PointF((input.X - centre.X), (input.Y - centre.Y));

            //Based on time, points will be scaled based on their angle to the centre
            double angle = Math.Atan(adjustedpoint.Y / adjustedpoint.X) + Math.PI / 2;
            if (double.IsNaN(angle))
            {
                angle = 0;
            }
            int idx = (int)((angle + time/4) * scale);
            while (idx >= Math.Floor(scale * Math.Tau))
            {
                idx -= (int)Math.Floor(scale * Math.Tau);
            }
            float heightmultiplier = (float)(menuItem.bakedjello[idx]);

            adjustedpoint.X *= heightmultiplier;
            adjustedpoint.Y *= heightmultiplier;

            adjustedpoint.X += centre.X;
            adjustedpoint.Y += centre.Y;

            return new Point((int)adjustedpoint.X, (int)adjustedpoint.Y);
        }
        public Point RotateLeft(Point input, int period, double amplitude, double offset)
        {
            PointF adjustedpoint = new PointF((input.X - centre.X), (input.Y - centre.Y));
            //period defines speed
            //amplitude defines rotation amount
            amplitude = Math.Min(Math.PI/2, amplitude);
            double angle = PointManager.GetAngle(centre, input);
            float heightmultiplier = (float)((Math.Sin((angle * period * 2) + time + offset) * amplitude));
            angle += heightmultiplier;
            double radius = Math.Sqrt(adjustedpoint.X * adjustedpoint.X + adjustedpoint.Y * adjustedpoint.Y);

            adjustedpoint.X = (float)(Math.Cos(angle) * radius);
            adjustedpoint.Y = (float)(Math.Sin(angle) * radius);

            adjustedpoint.X += centre.X;
            adjustedpoint.Y += centre.Y;

            return new Point((int)adjustedpoint.X, (int)adjustedpoint.Y);
        }
        public Point RotateRight(Point input, int period, double amplitude, double offset)
        {
            PointF adjustedpoint = new PointF((input.X - centre.X), (input.Y - centre.Y));
            //period defines speed
            //amplitude defines rotation amount
            amplitude = Math.Min(Math.PI / 2, amplitude);
            double angle = PointManager.GetAngle(centre, input);
            float heightmultiplier = -(float)((Math.Sin((angle * period * 2) + time + offset) * amplitude));
            angle += heightmultiplier;
            double radius = Math.Sqrt(adjustedpoint.X * adjustedpoint.X + adjustedpoint.Y * adjustedpoint.Y);

            adjustedpoint.X = (float)(Math.Cos(angle) * radius);
            adjustedpoint.Y = (float)(Math.Sin(angle) * radius);

            adjustedpoint.X += centre.X;
            adjustedpoint.Y += centre.Y;

            return new Point((int)adjustedpoint.X, (int)adjustedpoint.Y);
        }
        public Point HorizontalWave(Point input, double highestpoint, double lowestpoint, int period, double amplitude, double offset)
        {
            PointF adjustedpoint = new PointF((input.X - centre.X), (input.Y - centre.Y));

            double edgedistance;
            double halfway = (highestpoint + lowestpoint) / 2;
            if (adjustedpoint.Y > halfway)
            {
                //Stretch slower upwards
                edgedistance = Math.Pow(Math.Max((highestpoint - adjustedpoint.Y), 0), 2) / 100f;
            }
            else
            {
                //Stretch slower downwards
                edgedistance = Math.Pow(Math.Max((adjustedpoint.Y - lowestpoint), 0), 2) / 100f;
            }

            float heightmultiplier = (float)(Math.Sin(time + offset) * amplitude * edgedistance + 1 + amplitude);
            adjustedpoint.X += heightmultiplier;

            adjustedpoint.X += centre.X;
            adjustedpoint.Y += centre.Y;

            return new Point((int)adjustedpoint.X, (int)adjustedpoint.Y);
        }
        public Point VerticalWave(Point input, double highestpoint, double lowestpoint, int period, double amplitude, double offset)
        {
            PointF adjustedpoint = new PointF((input.X - centre.X), (input.Y - centre.Y));

            double edgedistance;
            double halfway = (highestpoint + lowestpoint) / 2;
            if (adjustedpoint.X > halfway)
            {
                //Stretch slower upwards
                edgedistance = Math.Pow(Math.Max((highestpoint - adjustedpoint.X), 0), 2) / 100f;
            }
            else
            {
                //Stretch slower downwards
                edgedistance = Math.Pow(Math.Max((adjustedpoint.X - lowestpoint), 0), 2) / 100f;
            }

            adjustedpoint.Y += (float)(Math.Sin(time + offset) * amplitude * edgedistance + 1 + amplitude);

            adjustedpoint.X += centre.X;
            adjustedpoint.Y += centre.Y;

            return new Point((int)adjustedpoint.X, (int)adjustedpoint.Y);
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