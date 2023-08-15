using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyperbolicRenderer
{
    public class Trapezium
    {
        public PointF top_left;
        public PointF bottom_left;
        public PointF top_right;
        public PointF bottom_right;

        public Trapezium(PointF top_left, PointF bottom_left, PointF top_right, PointF bottom_right)
        {
            this.top_left = top_left;
            this.bottom_left = bottom_left;
            this.top_right = top_right;
            this.bottom_right = bottom_right;
        }
        public static double elapseddrawtime;
        public static double elapsedtrigtime;
        internal void Draw(BMP image, bool curved, Color color, int mapsize)
        {
            float modifier;
            if (top_right.Y > 200)
            {
                modifier = 0.505f;
            }
            else
            {
                modifier = 0.495f;
            }
            if (curved)
            {
                //Create a sin wave where ther period/2 == topright-topleft
                //This will get the first crest

                // y = sin(ax) where period = 2pi/a
                // (2pi/a)/2 = topright - topleft
                // 2pi/a = 2*(topright-topleft)
                // a = 2pi / 2(topright-topleft)
                // a = pi/(topright-topleft)

                var topdistance = top_right.X - top_left.X;

                double a = Math.PI / (topdistance);

                Stopwatch s = new Stopwatch();
                for (float i = 0; i < topdistance; ++i)
                {
                    s.Restart();
                    double sin_height = Math.Sin(a * i); //Expressed as a percentage of the new height, pi/2 gets to next period to curve upwards
                    int x = (int)(i + top_left.X);

                    double m = (top_right.Y - top_left.Y) / topdistance;

                    //y = mx + c

                    //top_right.Y = m*top_right.X + c1
                    //c = top_right.Y - m*top_right.X

                    double c = top_right.Y - m * top_right.X;

                    double normalheight = m * x + c; //Find the height if it was a straight line


                    double ydist = normalheight - (mapsize / 2);
                    double xdist = x - (mapsize / 2);
                    double scalingfactor = Math.Abs(ydist) / (mapsize / 2);
                    scalingfactor = Math.Log(scalingfactor + 1) * 0.5f;
                    scalingfactor = ydist > 0 ? scalingfactor : -scalingfactor;

                    int y = (int)(sin_height*(topdistance) * scalingfactor + normalheight);
                    if (x >= mapsize || x < 0 || y >= mapsize || y < 0)
                    {
                        continue;
                    }
                    s.Stop();
                    elapsedtrigtime += s.ElapsedTicks;
                    s.Restart();
                    image.SetPixel(x, y, color);
                    s.Stop();
                    elapseddrawtime += s.ElapsedTicks;
                }
            }
            else
            {
                //graphics.DrawLine(pen,top_left, top_right);
            }
            if (top_right.X > 200)
            {
                modifier = 0.505f;
            }
            else
            {
                modifier = 0.495f;
            }
            if (curved)
            {
                //Sin wave math same as above

                var sidedistance = bottom_right.Y - top_right.Y;

                double a = Math.PI / (sidedistance);
                Stopwatch s = new Stopwatch();

                for (float i = 0; i < sidedistance; ++i)
                {
                    s.Restart();
                    double sin_height = Math.Sin(a * i); //Expressed as a percentage of the new height, pi/2 gets to next period to curve upwards
                    int y = (int)(i + top_right.Y);

                    double m = sidedistance / (bottom_right.X - top_right.X);
                    //y = mx + c

                    //bottom_right.Y = m*bottom_right.X + c1
                    //c = bottom_right.Y - m*bottom_right.X

                    double c = bottom_right.Y - m * bottom_right.X;

                    double normalheight = (y - c)/m; //Find the height if it was a straight line

                    if (bottom_right.X - top_right.X == 0)
                    {
                        normalheight = top_right.X;
                    }

                    //Use pythag to get distance to centre
                    double ydist = y - (mapsize / 2);
                    double xdist = normalheight - (mapsize / 2);
                    
                    double scalingfactor = Math.Abs(xdist)/(mapsize/2);
                    scalingfactor = Math.Log(scalingfactor + 1) * 0.5f;
                    scalingfactor = xdist > 0 ? scalingfactor : -scalingfactor;

                    int x = (int)(sin_height * (sidedistance) * scalingfactor + normalheight);
                    if (x >= mapsize || x < 0 || y >= mapsize || y < 0)
                    {
                        continue;
                    }
                    s.Stop();
                    elapsedtrigtime += s.ElapsedTicks;
                    s.Restart();
                    image.SetPixel(x, y, color);
                    s.Stop();
                    elapseddrawtime += s.ElapsedTicks;
                }
            }
            else
            {
                //graphics.DrawLine(pen, top_right, bottom_right);
            }

        }
    }
    public struct Line
    {
        public PointF start;
        public PointF end;

        public Line(PointF start, PointF end)
        {
            this.start = start;
            this.end = end;
        }
    }
    public class Map
    {
        public PointF[] points;
        public List<Trapezium> volume = new List<Trapezium>();
        public float radius;
        public PointF[] connections;
        public PointF[] oldconnections;
        public Line[] sideconnections;

        public Map(int points, float radius)
        {
            this.points = new PointF[points];
            this.radius = radius;
        }
        public void GenerateShape()
        {
            double radiansstepsize = Math.Tau / points.Length;
            for (int i = 0; i < points.Length; ++i)
            {
                //Draw a ray from the centre until it hits the edge of the square
                //Make this a vector
                double angle = (3 * (Math.PI / 2)) + (radiansstepsize * i);

                float x = (float)(Math.Cos(angle) * radius + radius);

                float y = (float)(Math.Sin(angle) * radius + radius);
                points[i] = new PointF(x, y);
            }
        }
        public int volumewidth = 0;
        public float squaresize = 0;
        public void GenerateVolume(float scale, float offsetx, float offsety, bool infinitevolume) //Scale of 1 will have squares of 20% size, 0.5 = 10% size...
        {
            squaresize = radius * 0.2f * scale;
            if (infinitevolume)
            {
                while (offsetx > squaresize)
                {
                    offsetx -= squaresize;
                }
                while (offsetx < -squaresize)
                {
                    offsetx += squaresize;
                }
                while (offsety > squaresize)
                {
                    offsety -= squaresize;
                }
                while (offsety < -squaresize)
                {
                    offsety += squaresize;
                }
            }

            volumewidth = (int)Math.Ceiling((radius * 2) / squaresize) + 1;
            connections = new PointF[(volumewidth) * (volumewidth)];
            oldconnections = new PointF[(volumewidth) * (volumewidth)]; //debugdata
            sideconnections = new Line[(volumewidth) * (volumewidth)]; //debugdata
            volume.Clear();

            List<Line> shapelines = new List<Line>();
            for (int i = 0; i < points.Count(); i++)
            {
                shapelines.Add(new Line(points[i], points[i + 1 >= points.Count() ? 0 : i + 1]));
            }

            for (int y = 0; y < (volumewidth); ++y)
            {
                for (int x = 0; x < (volumewidth); ++x)
                {
                    PointF relativepoint = new PointF(x * squaresize + offsetx, y * squaresize + offsety);

                    float neg_x_scale = float.MinValue;
                    float pos_x_scale = float.MaxValue;

                    float neg_y_scale = float.MinValue;
                    float pos_y_scale = float.MaxValue;

                    float y_scalemodifier = 1;
                    float x_scalemodifier = 1;

                    Line closestline = new Line();

                    foreach (var line in shapelines)
                    {
                        PointF distance = relativepoint.DistanceTo(line);

                        if (distance.Y == 0)
                        {
                            y_scalemodifier = 0.5f;
                        }
                        if (distance.X == 0)
                        {
                            x_scalemodifier = 0.5f;
                        }
                        if (distance.Y > neg_y_scale && distance.Y < 0 && distance.Y != 0)
                        {
                            neg_y_scale = distance.Y;
                        }
                        if (distance.Y < pos_y_scale && distance.Y > 0 && distance.Y != 0)
                        {
                            pos_y_scale = distance.Y;
                        }

                        if (distance.X > neg_x_scale && distance.X < 0 && distance.X != 0)
                        {
                            neg_x_scale = distance.X;
                        }
                        if (distance.X < pos_x_scale && distance.X > 0 && distance.X != 0)
                        {
                            pos_x_scale = distance.X;
                        }
                    }

                    float y_scale;
                    float x_scale;
                    if (Math.Abs(neg_y_scale) < pos_y_scale)
                    {
                        y_scale = neg_y_scale;
                    }
                    else
                    {
                        y_scale = pos_y_scale;
                    }

                    if (Math.Abs(neg_x_scale) < pos_x_scale)
                    {
                        x_scale = neg_x_scale;
                    }
                    else
                    {
                        x_scale = pos_x_scale;
                    }

                    float y_distancetocentre = Math.Abs(relativepoint.Y - radius);
                    float turningtime = squaresize/2f;
                    const double cutoff = 0.7f;
                    if (y_distancetocentre < turningtime)
                    {
                        //y_distancetocentre = y_scale < 0 ? -y_distancetocentre : y_distancetocentre;
                        //Begin to approach a linear straight line at the centre
                        if (neg_y_scale + pos_y_scale == 0)
                        {
                            y_scale = 0;
                        }
                        else
                        {
                            //Should equal zero when distancetocentre == 0
                            //Should equal 1 when distancetocentre == squaresize/5
                            if (y_distancetocentre >= turningtime * cutoff)
                            {
                                double a = 0.7f/Math.Pow(turningtime-(turningtime*cutoff),2);
                                y_scale *= (float)(a*Math.Pow(y_distancetocentre-(turningtime*cutoff),2)+0.3f);
                                //y_scale *= (float)Math.Pow(Math.E, y_distancetocentre - (squaresize / 5));
                            }
                            else
                            {
                                //((0.7)/(3 a s)) x+1-((0.7)/(3 a s)) 
                                y_scale *= (float)((0.3f/(cutoff * turningtime)) * y_distancetocentre);
                                //y_scale *= (float)(y_distancetocentre * (Math.Pow(Math.E, ((squaresize / 5) * (1f / 6f)))/ ((squaresize / 5) * (5f / 6f))));
                            }
                            //y_scale *= (float)(Math.Log((y_distancetocentre+1f)/((squaresize/5f)/2f)));
                        }
                    }
                    float x_distancetocentre = Math.Abs(relativepoint.X - radius);

                    if (x_distancetocentre < turningtime)
                    {
                        //x_distancetocentre = x_scale < 0 ? -x_distancetocentre : x_distancetocentre;
                        //Begin to approach a linear straight line at the centre
                        if (neg_x_scale + pos_x_scale == 0)
                        {
                            x_scale = 0;
                        }
                        else
                        {
                            //Should equal zero when distancetocentre == 0
                            //Should equal 1 when distancetocentre == squaresize/5
                            if (x_distancetocentre >= turningtime * cutoff)
                            {
                                double a = 0.7f / Math.Pow(turningtime - (turningtime * cutoff), 2);
                                x_scale *= (float)(a * Math.Pow(x_distancetocentre - (turningtime * cutoff), 2) + 0.3f);
                                //x_scale *= (float)Math.Pow(Math.E, x_distancetocentre - (squaresize / 5));
                            }
                            else
                            {
                                //((0.7)/(3 a s)) x+1-((0.7)/(3 a s)) 
                                x_scale *= (float)((0.3f / (cutoff * turningtime)) * x_distancetocentre);
                                //x_scale *= (float)(x_distancetocentre * (Math.Pow(Math.E, ((squaresize / 5) * (1f / 6f)))/ ((squaresize / 5) * (5f / 6f))));
                            }
                            //x_scale *= (float)(Math.Log((x_distancetocentre+1f)/((squaresize/5f)/2f)));
                        }
                    }

                    y_scale *= y_scalemodifier;
                    x_scale *= x_scalemodifier;
                    sideconnections[x + y * volumewidth] = new Line(relativepoint, new PointF(x_scale + relativepoint.X, y_scale + relativepoint.Y)); //debugdata

                    x_scale = (float)Math.Sin(x_scale / 20) / 2;
                    y_scale = (float)Math.Sin(y_scale / 20) / 2;
                    const float limiter = 0.5f;
                    if (x_scale >= limiter)
                    {
                        x_scale = limiter;
                    }
                    if (x_scale <= -limiter)
                    {
                        x_scale = -limiter;
                    }
                    if (y_scale >= limiter)
                    {
                        y_scale = limiter;
                    }
                    if (y_scale <= -limiter)
                    {
                        y_scale = -limiter;
                    }
                    if (Math.Abs(x * squaresize + offsetx - radius) < squaresize && Math.Abs(y * squaresize + offsety - radius) < squaresize)
                    {
                        x_scale /= 2;
                        y_scale /= 2;
                    }
                    float ay = y;
                    float ax = x;
                    if (offsety != 0) //Removes infinity errors
                    {
                        ay += (offsety / squaresize);
                    }
                    if (offsetx != 0) //Removes infinity errors
                    {
                        ax += (offsetx / squaresize);
                    }
                    ay += y_scale;
                    ax += x_scale;

                    connections[x + y * (volumewidth)] = new PointF(ax * squaresize, ay * squaresize);
                    oldconnections[x + y * (volumewidth)] = relativepoint;
                }
            }

            for (int x = 0; x < volumewidth - 1; x++)
            {
                for (int y = 0; y < volumewidth - 1; y++)
                {
                    int ay = y * volumewidth; //Adjusted y

                    volume.Add(new Trapezium(connections[x + ay], connections[x + (ay + volumewidth)], connections[(x + 1) + ay], connections[(x + 1) + (ay + volumewidth)]));
                }
            }
        }
    }
}
