using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Intrinsics.X86;
using System.Text;
using System.Threading.Tasks;

namespace HyperbolicRenderer
{
    public class Vector
    {
        public double i;
        public double j;

        public PointF A;
        public PointF B; //Where vector = ->
        internal double angle //Is in radians
        {
            //theta = arctan(j/i)
            get
            {
                return Math.Atan(j / i);
            }
        }

        //               AB
        private YMC_VectorLine vectorLine;
        public Vector(double i, double j)
        {
            this.i = i;
            this.j = j;
            CreateVectorline();
        }

        public static Vector operator *(Vector a, double s)
        {
            return new Vector(a.i * s, a.j * s);
        }
        public static Vector operator /(Vector a, double s)
        {
            return new Vector(a.i / s, a.j / s);
        }
        public static Vector operator +(Vector a, Vector b)
        {
            return new Vector(a.i + b.i, a.j + b.j);
        }
        public static Vector operator -(Vector a, Vector b)
        {
            return new Vector(a.i - b.i, a.j - b.j);
        }

        public Vector(PointF A, PointF B)
        {
            this.A = A;
            this.B = B;

            i = B.X - A.X;
            j = B.Y - A.Y;

            CreateVectorline();
        }

        private void CreateVectorline()
        {
            //Generate ymc vectorline
            //Convert into a y = mx + c equation
            //Step one, convert into ai + bj + t(ci + dj)
            //AB = A + t(b-a)
            var b_minus_a = new PointF((float)i, (float)j);
            //r = (a+ct)i + (b+dt)j

            //a+ct = x
            //x - a = ct
            //t = x-a/c

            //y = b + d((x-a)/c)
            //y = d (x-a)/c

            vectorLine = new YMC_VectorLine(A.X, A.Y, b_minus_a.X, b_minus_a.Y);
        }

        public Vector GetPerpindicular()
        {
            //Find a new vector where i*a + j*b == 0
            return new Vector(-j, i);
        }
        public Vector GetUnitVector()
        {
            //Unit vector = v/|v|
            double magnitude = Math.Sqrt(i * i + j * j);
            return new Vector(i / magnitude, j / magnitude);
        }

        public bool PointOnLine(PointF point)
        {
            //Convert into a y = mx + c equation
            //Step one, convert into ai + bj + t(ci + dj)

            //Can only be done if two seperate points are given, where our 'point' is within the bounds
            RectangleF bounds = new RectangleF(new PointF(A.X < B.X ? A.X : B.X, A.Y < B.Y ? A.Y : B.Y), new SizeF(Math.Abs(B.X - A.X), Math.Abs(B.Y - A.Y)));
            if (A == B || !bounds.Contains(point)) //A, B start as 0,0 so if no points are given still returns false
            {
                return false;
            }

            return vectorLine.PointOnLine(point.X, point.Y);
        }

        internal double GetPoint(double x)
        {
            return vectorLine.SubstituteX(x);
        }

        internal PointF Intersection(Vector rt)
        {
            //Convert to cartesian and compare
            var line1 = vectorLine;
            var line2 = rt.vectorLine;

            double a = line1.a;
            double b = line1.b;
            double c = line1.c;
            double d = line1.d;
            double e = line2.a;
            double f = line2.b;
            double g = line2.c;
            double h = line2.d;

            double ch = c * h;
            double dg = d * g;
            
            if (i != 0 && rt.i != 0) 
            {
                //Formula:
                //data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAOQAAAAzCAYAAACHbKQaAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAJcEhZcwAADsMAAA7DAcdvqGQAAAmXSURBVHhe7Z0HTFRNEIAHNCp2ERHssRJ7x4aKiIqQKIqIXRKR2MVG7LEkxsRYUGKPvcVojF1sJBqNJfauqGgUjYIVjHX/nWXvvzu4xz3u7sGV+RLCm91Hbtj35s3s7L45t8zMTAYEQdgF7vI3QRB2gBvjyGOCIAoZ8pAEYUeQQRKEHUEGSRB2BBkkQdgRZJAEYUeQQRKEHUEGqUBGRgaMHz8eGjZsCOvXr5etBUd6ejrExcVBo0aNYOnSpbLVNcCVuHnz5kHHjh1h2LBhsrXwQb0WL14MTZo0gbCwMNlqW8ggFfD09ISFCxdCamoqNG3aVLYWHBUrVoT4+Hh4+fIldO7cWba6Bm5ubjBz5kwoXbo0dO/eXbYWPmvWrIHfv39DUlISdOvWTbbaFtoYkAfXr1+HNm3awLdv38TNUdC8ePFCeOgvX75AsWLFZKvrUKVKFbh8+TLUrFlTthQef/78AR8fH7h16xZUq1ZNttoel/CQa9euhR49esDw4cOFt/H395c9udm3bx/07dsXhgwZApGRkVC3bl1FY3z//j1ERUXBqFGjIDg4GIYOHSpCrFevXskzTLNhwwa4f/++lJS5ceMGtGzZEpYtWwa1atWCdu3awbt372RvblatWgWhoaHiBsYb2RRa64yo0WPr1q0wbtw4aN++PVy9elW26nnz5g38/ftXHKO+lSpVsnnorkZPHegVMWpRY4yWjjHi9AY5d+5cOH78OBw5cgS2b98uQg6cm5gCL9DmzZuFUe7atQtq164NzZo1k725wUFv3bo1bNq0CQICAiAtLQ127NgBNWrUkGeYBg3tw4cPUlLm5s2b8PPnT3HTPHjwQHhKNE5TTJ06VXjRo0ePinkn3tCm0FpnNXosWbJEGHdiYiL07NkTdu7cKXv0XLt2TYSuqNu6detgwIABcODAAdlrPWrHC8H7Z9KkSeLeCQ8PF7mFvLB0jAUYsjorPORjJUuWZNyryBbGuIGx/fv3S0nP27dvmYeHh/gbHYGBgYzPI6VkDJ6Hw8eNRMjcg7CgoCBxbI7Y2Fh2/vx5KSnTu3dvxj2JlBiLi4tj3NNLSc+9e/dY48aNpaSM1jqr0SMlJYWVLVtWXBPuScT5p06dkr16Zs2axSZMmCAlxhYsWMAiIiKkZB1qx8sQ7uEU7wVDrBljxOE8JNcZTp48qfiDMb4O9IiYFKhcubKQcR7w8OFDaNGihZANQY+I4ROGhjrwXCUP+enTJyhTpgzwm0vIGHYpZQRz6vz69WtxvimdDUEPyW8cKYF4omPYlBP0MBiSm0NrndXocejQIfE/YIJk/vz5IiIx9TfoIfF66ECP2rx5cynpyc/9oEPteBny5MkTqFevnpSUyc8Ym8KpDZI/CcUcUMfjx4+hePHiIhTNCRqf4YDjwOJ8Tckgsb1q1apw+vRpOHPmjJhbjBgxQvYaY8nNjZ+NcxFM6uh49OgRNGjQQEp6UHfUxRxa66xGD7wmGH4uWrRIzO3btm0re4zBhJrh/44GimFgTiwxSLXjZcjTp09VGWR+xtgk/B9yWkJCQozCnt27dzN/f38pGTN48GAWExMjJSbCs3LlyknJNHzewTp16sQuXbokW9ShJvzj8xbGbwApMZaens74k9copNYRHBwswllDsrKy5JExWuqsRo/o6GjWv39/KWXz9etXeZQNv/mZu7s7+/Hjh5A/fvwowkDDqYc15Ge8EN3n68JQc1g6xohTJ3Uw3Dt79qxIjOBTERMi3MhkrzF47rlz58S5z549g9mzZ4vMXl6cOHFCeOBWrVrJFtuB4aphuh+TEPzhYhRS68ClGUw8fP/+XSReMOnA58Sy1xgtdVajB3q55ORksfEBkyTLly+HY8eOyd5s0BvWqVMHSpQoIeS7d++K37ow0FryM14Ihst4LdR+vlVjLA3TKcEnaocOHRgPG8QTkRsn8/b2ZmPHjmV8TiDPygY9UEBAAPPz82MzZsxgPDxjvr6+wsNeuXJFnqXn8+fPLDw8nPH5KeMXi23ZskX2mEeNtzl8+DDjcyYWFhYmPHdiYqLsyU1GRoZI9vj4+LDQ0FDGQ3PZY4zWOqvRAz0RJmfwmvTq1YvxOaXs0TNlyhTWp08fKTHGDYJ5eXmxyMhIszqoQe146UhISBCfrQZrxhhxaoPUirS0NGG4fF7F+FNWZN8wQzt58mR5hv3hiDrbCwMHDmR8PiglZWwxxmSQFoDeCr2pIbiUgk9Fe8URdS5Mfv36xZKSktidO3dYVFSUbM0bW4yxS+zUsTXVq1eH27dvi8yjDjzu16+flOwPR9S5MMGxweWK1atXw7Zt22Rr3thijGkvq4XwcETsHMGN56VKlRKTeB6aQNGiReUZ9ocj6uxoWDvGFhskbjnCtxEw83Tx4kUoUqTI/2tLqJDhoi5BEOqwykPiQjs+CS5cuCAW0jGVjLsUYmJijFL2BEGow+qQddCgQVC/fn2xqwQ3ASuBa04hISFSUgZ3ZyQkJEgpN35+fvKIIBwLXEtGZ5UXVhvk3r17xdYgfD2lS5cusjU3+DEpKSlSUgYXg3G7EUG4IlYbJL6jhuUWcGeJ4d5DgiDyj1UGiVuP8H1D3JYWFBQE06dPlz25wZAVNxWbAzdPu1oNGYLQYZFB4i52fIse12jQePbs2SNe6tW9VGoK/BhcozEHporV7KonCKcEDTK/xMfHM09PT/H2BJKamir2JnIPyJKTk0UbQRD5x+o5JEEQtoO2zjkJ+FwtyFqmhV231lkhg3QSCrqWaWHXrXVWyCCdCA8PD/Eyb9euXWWLtjx//hyysrJEJW/CNpBBOhDm6ohiKUOta5kWRN1aV4YM0kFQU0dU61qmBVW31pUhg3QAsKYLbk0cM2aMMDjcxB8RESF79aBBoteaM2cOlC9fXpS+t9UmfzQmnKNiBXOs3If8+/dP0SDxO0mwds7o0aOF7Ovrq7hGTeghg3QA1NYRRYPUqpaplnVrCT1kkA6A2jqiWtcy1apuLWEAbgwg7Bs1dUS1rmVakHVrXRnykA6Amjqi6A21rGVqb3VrnRUySAdg2rRpIjmDIWN0dDRMnDhRGJ8hOcNVb29v8PLygpEjR4rkirXExsaK72vEOenGjRth5cqVkJmZKXTB+WFO8Ju6sAAyGiW+wI5fP0eYh/ayEjYH55aBgYFi/lihQgVRnRy/gg6NesWKFfIswhTkIQmbc/DgQRHOYiIHX6fDd2bxm8jwNT0ib8ggCZtDNWAth0JWQhOoBqxlkEEShB1BIStB2BFkkARhNwD8BwBzKEv7gACPAAAAAElFTkSuQmCC

                // x = (bcg + ceh - cfg - adg)/(ch-dg)
                double bcg = b * c * g;
                double ceh = c * e * h;
                double cfg = c * f * g;
                double adg = a * d * g;


                double x = (bcg + ceh - cfg - adg) / (ch - dg);
                return new PointF((float)x, (float)line1.SubstituteX(x));
            }
            else
            {
                //Formula:
                //data:image/png;base64,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

                // y = (-adh+bdh-dfg)/ch-dg

                double y = rt.A.Y;

                return new PointF((float)line1.SubstituteY(y), (float)y);
            }
        }
    }

    public class YMC_VectorLine
    {
        public double a;
        public double b;
        public double c;
        public double d; //y = b + d((x-a)/c)
                         //x = (-bc + ad + cy)/d

        public YMC_VectorLine(double a, double b, double c, double d)
        {
            this.a = a;
            this.b = b;
            this.c = c;
            this.d = d;
        }

        public bool PointOnLine(double x, double y)
        {
            //y = b + d((x-a)/c)
            return b + d * ((x - a) / c) == y;
        }
        public double SubstituteX(double x)
        {
            return b + d * ((x - a) / c);
        }
        public double SubstituteY(double y)
        {
            return (-b*c + a*d + c*y) / d;
        }
    }
}
