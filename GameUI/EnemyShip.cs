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
    public class EnemyShip : Ship
    {
        Vector2[] colliderpoints = new Vector2[58] { new Vector2(47, 61), new Vector2(43, 57), new Vector2(44, 55), new Vector2(40, 53), new Vector2(40, 54), new Vector2(35, 59), new Vector2(31, 60), new Vector2(26, 62), new Vector2(23, 61), new Vector2(20, 59), new Vector2(17, 57), new Vector2(13, 52), new Vector2(13, 54), new Vector2(10, 59), new Vector2(7, 64), new Vector2(3, 66), new Vector2(0, 64), new Vector2(0, 60), new Vector2(0, 56), new Vector2(0, 54), new Vector2(1, 52), new Vector2(1, 50), new Vector2(1, 48), new Vector2(2, 46), new Vector2(3, 43), new Vector2(4, 38), new Vector2(5, 34), new Vector2(6, 30), new Vector2(7, 26), new Vector2(8, 22), new Vector2(9, 18), new Vector2(12, 16), new Vector2(14, 19), new Vector2(17, 22), new Vector2(18, 18), new Vector2(19, 13), new Vector2(21, 11), new Vector2(22, 8), new Vector2(24, 3), new Vector2(27, 0), new Vector2(30, 2), new Vector2(33, 7), new Vector2(35, 12), new Vector2(36, 18), new Vector2(38, 23), new Vector2(41, 21), new Vector2(42, 16), new Vector2(46, 16), new Vector2(47, 21), new Vector2(48, 26), new Vector2(50, 31), new Vector2(52, 36), new Vector2(52, 41), new Vector2(52, 46), new Vector2(54, 51), new Vector2(54, 56), new Vector2(55, 61), new Vector2(54, 66), };
        public EnemyShip(Texture2D tex, Vector2 pos) : base(tex, pos)
        {
            collider = new Collider(colliderpoints, OnCollision, pos, "ENEMY");
        }

        public override void Update() //Allows for player movement
        {
            Vector traveldirection = new Vector(new System.Drawing.PointF(position.X, position.Y), new System.Drawing.PointF(Game1.player.position.X, Game1.player.position.Y)).GetUnitVector();

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

            var cockpitdirection = new Vector(Game1.enemy.rotation - Math.PI / 2).GetUnitVector();
            double distanceaway = Game1.enemy.texture.Height * 0.5f;
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
