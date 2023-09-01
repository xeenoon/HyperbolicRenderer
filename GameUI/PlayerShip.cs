using HyperbolicRenderer;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameUI
{
    public class PlayerShip : Ship
    {
        //Centre -34,-27 
        //Resolution 0.3

        public static Vector2[] colliderpoints = new Vector2[74] { new Vector2(3, 17), new Vector2(2, 15), new Vector2(3, 14), new Vector2(1, 13), new Vector2(-1, 13), new Vector2(-4, 14), new Vector2(-5, 12), new Vector2(-7, 15), new Vector2(-7, 18), new Vector2(-7, 21), new Vector2(-7, 24), new Vector2(-10, 26), new Vector2(-11, 24), new Vector2(-12, 22), new Vector2(-14, 21), new Vector2(-14, 18), new Vector2(-16, 17), new Vector2(-18, 16), new Vector2(-21, 16), new Vector2(-24, 16), new Vector2(-26, 15), new Vector2(-29, 15), new Vector2(-32, 15), new Vector2(-34, 14), new Vector2(-34, 10), new Vector2(-33, 9), new Vector2(-32, 6), new Vector2(-31, 5), new Vector2(-29, 4), new Vector2(-27, 2), new Vector2(-25, 1), new Vector2(-24, 0), new Vector2(-22, -3), new Vector2(-20, -4), new Vector2(-19, -5), new Vector2(-17, -6), new Vector2(-16, -7), new Vector2(-14, -10), new Vector2(-12, -11), new Vector2(-10, -13), new Vector2(-9, -14), new Vector2(-9, -16), new Vector2(-9, -18), new Vector2(-8, -19), new Vector2(-6, -20), new Vector2(-6, -24), new Vector2(-5, -25), new Vector2(-4, -26), new Vector2(-1, -26), new Vector2(0, -25), new Vector2(1, -22), new Vector2(4, -19), new Vector2(4, -16), new Vector2(5, -13), new Vector2(8, -10), new Vector2(10, -9), new Vector2(13, -6), new Vector2(16, -3), new Vector2(18, -2), new Vector2(21, 1), new Vector2(24, 4), new Vector2(26, 5), new Vector2(28, 8), new Vector2(29, 11), new Vector2(29, 14), new Vector2(26, 15), new Vector2(23, 15), new Vector2(20, 16), new Vector2(17, 16), new Vector2(14, 16), new Vector2(11, 17), new Vector2(9, 20), new Vector2(7, 23), new Vector2(5, 26), };

        public PlayerShip(Texture2D tex, Vector2 pos) : base(tex, pos)
        {
            collider = new Collider(colliderpoints, OnCollision, pos, "PLAYER");
            startengineemitcolor = Color.DarkBlue;
            endengineemitcolor = Color.LightBlue;
        }

        public override void Update() //Allows for player movement
        {
            Vector traveldirection = new Vector(new System.Drawing.PointF(position.X, position.Y), new System.Drawing.PointF(InputManager.MousePosition.X, InputManager.MousePosition.Y)).GetUnitVector();

            double mouseangle = (traveldirection.angle + Math.PI / 2);
            double rotationamount = (_rotationSpeed * Game1.game.looptime);
            double rotationchange = 0;

            if (Math.Abs(rotation - mouseangle) > (rotationamount) || rotation < 0) //Stops snappy movement
            {
                if (mouseangle < rotation && rotation - mouseangle > Math.PI)
                {
                    rotation -= Math.Tau;
                }
                if ((mouseangle - rotation <= Math.PI && rotation < mouseangle))
                {
                    if (rotation + rotationamount <= mouseangle)
                    {
                        rotationchange = rotationamount;
                    }

                }
                else
                {
                    if ((rotation - rotationamount) + Math.Tau >= mouseangle)
                    {
                        rotationchange = -rotationamount;

                    }
                    else
                    {
                        rotation += Math.Tau;
                    }
                }
            }
            rotation += rotationchange;

            var cockpitdirection = new Vector(Game1.player.rotation - Math.PI / 2).GetUnitVector();
            double distanceaway = Game1.player.texture.Height * 0.5f;
            Vector shippoint = cockpitdirection * distanceaway;
            Vector shipengine = (cockpitdirection * -1) * distanceaway;
            backend = new Vector2((float)(position.X + shipengine.i), (float)(position.Y + shipengine.j));

            if (InputManager.moving || speed > 0)
            {
                Vector mousedirection = new Vector(rotation - Math.PI / 2);
                double distancetoend = Vector2.Distance(InputManager.MousePosition, new Vector2((float)(position.X + shippoint.i), (float)(position.Y + shippoint.j)));

                AutoDecelerate(distancetoend);
                UpdateParticles();

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
            else
            {
                ParticleManager.particleEmitters.Remove(enginehandler);
                wasmoving = false;
            }

            collider.Move(position);
            collider.Rotate(rotationchange);
        }
    }
}
