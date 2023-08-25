using HyperbolicRenderer;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SharpDX.Direct2D1;
using SharpDX.Direct3D9;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameUI
{
    public class Ship : Sprite
    {
        public double rotation = 0;
        public const double _rotationSpeed = 3f;
        const double maxspeed = 600;
        double speed;
        float acceleration = 150f;
        bool forcestop;

        public Ship(Texture2D tex, Vector2 pos) : base(tex, pos)
        {
        }
        public void Update()
        {
            Vector traveldirection = new Vector(new System.Drawing.PointF(position.X, position.Y), new System.Drawing.PointF(InputManager.MousePosition.X, InputManager.MousePosition.Y)).GetUnitVector();

            double mouseangle = (traveldirection.angle + Math.PI / 2);
            double rotationamount = (_rotationSpeed * Game1.game.looptime);

            if (Math.Abs(rotation - mouseangle) > (rotationamount) || rotation < 0) //Stops snappy movement
            {
                if (mouseangle < rotation && rotation - mouseangle > Math.PI)
                {
                    rotation -= Math.Tau;
                }
                if ((mouseangle - rotation < Math.PI && rotation < mouseangle))
                {
                    if (rotation + rotationamount <= mouseangle)
                    {
                        rotation += rotationamount;
                    }
                    else
                    {
                        rotation = mouseangle;
                    }
                }
                else
                {
                    if ((rotation - rotationamount) + Math.Tau >= mouseangle)
                    {
                        rotation -= rotationamount;
                    }
                    else
                    {
                        rotation = mouseangle; //Reset the circle
                    }
                }
            }


            if (InputManager.moving || speed > 0)
            {
                Vector mousedirection = new Vector(rotation - Math.PI / 2);
                double acceleration = this.acceleration;


                var cockpitdirection = new Vector(Game1.player.rotation - Math.PI / 2).GetUnitVector();
                double distanceaway = Game1.player.texture.Height * 0.5f;
                Vector shippoint = cockpitdirection * distanceaway;


                double distancetoend = Vector2.Distance(InputManager.MousePosition, new Vector2((float)(position.X + shippoint.i), (float)(position.Y + shippoint.j)));
                //v^2 = u^2 + 2as
                //0 = speed^2 + 2*-150*4*distance
                //distance = ((speed^2))/1200
                if (distancetoend <= speed * speed / 1200)
                {
                    forcestop = true;
                }
                else if(distancetoend >= 5) //Dont restart if we just finished stopping
                {
                    forcestop = false;
                }

                double extraspeed = acceleration * Game1.game.looptime * (InputManager.boosting ? 2 : 1);
                if (InputManager.moving && !forcestop)
                {
                    if (speed + extraspeed < maxspeed)
                    {
                        speed += extraspeed;
                    }
                    else
                    {
                        speed = maxspeed;
                    }
                }
                else
                {
                    //Decelerate 4 times as fast
                    extraspeed *= 4;
                    if (speed - extraspeed > 0)
                    {
                        speed -= extraspeed;
                    }
                    else
                    {
                        speed = 0;
                    }
                }

                position = new Vector2((float)((mousedirection.i * speed * Game1.game.looptime) + position.X), (float)((mousedirection.j * speed * Game1.game.looptime) + position.Y));
            }
        }
        private static bool TurnDirection(double myangle, double turntowards)
        {
            if (turntowards - myangle < Math.PI && myangle < turntowards)
            {
                return true; //Turn right
            }
            else
            {
                return false;
            }
        }

        public override void Draw()
        {
            Game1.game.spriteBatch.Draw(texture, position, null, Color.White, (float)rotation, origin, 1f, SpriteEffects.None, 1);
        }
    }
    public class Bullet : Sprite
    {
        public Vector destinationdirection;

        public Bullet(Texture2D tex, Vector2 position) : base(tex, position)
        {
            destinationdirection = new Vector(Game1.player.rotation - Math.PI/2).GetUnitVector();
            double distanceaway = Game1.player.texture.Width * 0.5f;
            Vector startadd = destinationdirection * distanceaway;
            this.position.X += (float)(startadd.i);
            this.position.Y += (float)(startadd.j);
        }
        public void Update()
        {
            const int speed = 15;
            position.X += (float)(destinationdirection.i) * speed;
            position.Y += (float)(destinationdirection.j) * speed;
        }
        public override void Draw()
        {
            Game1.game.spriteBatch.Draw(texture, position, null, Color.White, 0, origin, 0.02f, SpriteEffects.None, 1);
        }
    }
}
