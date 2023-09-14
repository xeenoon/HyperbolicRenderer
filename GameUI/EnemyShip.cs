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
        const string tag = "ENEMY";
        Vector2[] colliderpoints = new Vector2[149] { new Vector2(23, 33), new Vector2(22, 32), new Vector2(21, 31), new Vector2(19, 29), new Vector2(20, 29), new Vector2(19, 28), new Vector2(18, 26), new Vector2(17, 25), new Vector2(15, 23), new Vector2(15, 22), new Vector2(16, 22), new Vector2(15, 21), new Vector2(14, 20), new Vector2(12, 19), new Vector2(14, 19), new Vector2(13, 19), new Vector2(11, 21), new Vector2(9, 23), new Vector2(7, 25), new Vector2(5, 27), new Vector2(3, 27), new Vector2(2, 27), new Vector2(-5, 28), new Vector2(-2, 28), new Vector2(-3, 27), new Vector2(-4, 26), new Vector2(-6, 27), new Vector2(-7, 26), new Vector2(-8, 25), new Vector2(-9, 24), new Vector2(-10, 23), new Vector2(-11, 22), new Vector2(-12, 20), new Vector2(-15, 18), new Vector2(-14, 18), new Vector2(-14, 19), new Vector2(-16, 21), new Vector2(-17, 23), new Vector2(-18, 25), new Vector2(-20, 27), new Vector2(-20, 29), new Vector2(-22, 31), new Vector2(-24, 33), new Vector2(-25, 32), new Vector2(-26, 31), new Vector2(-28, 31), new Vector2(-28, 29), new Vector2(-28, 27), new Vector2(-28, 26), new Vector2(-27, 24), new Vector2(-28, 23), new Vector2(-27, 23), new Vector2(-27, 21), new Vector2(-28, 20), new Vector2(-27, 20), new Vector2(-26, 18), new Vector2(-28, 18), new Vector2(-28, 16), new Vector2(-27, 16), new Vector2(-26, 16), new Vector2(-26, 14), new Vector2(-27, 13), new Vector2(-26, 13), new Vector2(-26, 12), new Vector2(-26, 10), new Vector2(-25, 10), new Vector2(-25, 8), new Vector2(-24, 6), new Vector2(-24, 4), new Vector2(-24, 1), new Vector2(-23, 1), new Vector2(-23, -1), new Vector2(-23, -2), new Vector2(-22, -4), new Vector2(-22, -7), new Vector2(-21, -7), new Vector2(-21, -9), new Vector2(-21, -10), new Vector2(-20, -12), new Vector2(-20, -14), new Vector2(-20, -15), new Vector2(-19, -18), new Vector2(-18, -18), new Vector2(-16, -18), new Vector2(-15, -18), new Vector2(-14, -16), new Vector2(-14, -14), new Vector2(-12, -12), new Vector2(-11, -12), new Vector2(-11, -13), new Vector2(-10, -15), new Vector2(-10, -18), new Vector2(-9, -18), new Vector2(-9, -21), new Vector2(-8, -21), new Vector2(-8, -22), new Vector2(-7, -25), new Vector2(-6, -25), new Vector2(-6, -26), new Vector2(-5, -29), new Vector2(-4, -29), new Vector2(-4, -30), new Vector2(-3, -32), new Vector2(-1, -34), new Vector2(0, -34), new Vector2(1, -33), new Vector2(3, -31), new Vector2(4, -29), new Vector2(5, -27), new Vector2(6, -25), new Vector2(7, -23), new Vector2(8, -21), new Vector2(8, -19), new Vector2(8, -16), new Vector2(9, -14), new Vector2(9, -12), new Vector2(11, -11), new Vector2(12, -13), new Vector2(13, -13), new Vector2(13, -16), new Vector2(14, -16), new Vector2(14, -17), new Vector2(16, -18), new Vector2(18, -18), new Vector2(19, -16), new Vector2(19, -14), new Vector2(19, -12), new Vector2(20, -10), new Vector2(20, -8), new Vector2(21, -6), new Vector2(21, -4), new Vector2(22, -2), new Vector2(22, 0), new Vector2(24, 2), new Vector2(24, 4), new Vector2(24, 6), new Vector2(24, 8), new Vector2(24, 10), new Vector2(24, 12), new Vector2(25, 14), new Vector2(25, 16), new Vector2(26, 18), new Vector2(26, 20), new Vector2(26, 22), new Vector2(27, 24), new Vector2(27, 26), new Vector2(27, 28), new Vector2(27, 30), new Vector2(26, 32), }; 
        //MoveableShape graphicalcollider;
        public EnemyShip(Texture2D tex, Vector2 pos) : base(tex, pos)
        {
            collider = new Collider(colliderpoints, OnCollision, pos, tag);
            //graphicalcollider = Game1.game.batcher.AddMoveableShape(colliderpoints.Copy().ToArray(), Color.White, Vector2.Zero);
            //graphicalcollider.Move(position);

            engineEmitData = new EngineEmitData(this, Color.DarkOrange, Color.LightYellow, 6, 0.3f, 1);

            maxspeed = 300;
            boostable = false;
        }
        public bool disappear;
        double lasttime = Game1.game.totalseconds;
        public override bool OnCollision(string tag)
        {
            if (tag == "BULLET:PLAYER")
            {
                disappear = true;
            }
            if (tag == "ASTEROID")
            {
                disappear = true;
            }
            return false;
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

            var cockpitdirection = new Vector(rotation - Math.PI / 2).GetUnitVector();
            double distanceaway = texture.Height * 0.5f;
            Vector shippoint = cockpitdirection * distanceaway;
            Vector shippointend = cockpitdirection * (distanceaway + Game1.bullettexture.Height);
            Vector2 shippointposition = new Vector2((float)(position.X + shippoint.i), (float)(position.Y + shippoint.j));
            Vector2 shippointendposition = new Vector2((float)(position.X + shippointend.i), (float)(position.Y + shippointend.j));

            const double reloadtime = 0.6;
            if (Game1.game.totalseconds - lasttime >= reloadtime)
            {
                lasttime = Game1.game.totalseconds;
                Game1.projectiles.Add(new Bullet(Game1.bullettexture, shippointendposition, new Vector(rotation - Math.PI / 2).GetUnitVector(), 7, tag));
            }

            Vector shipengine = (cockpitdirection * -1) * distanceaway;
            emitpositions[0] = new Vector2((float)(position.X + shipengine.i), (float)(position.Y + shipengine.j));


            Vector mousedirection = new Vector(rotation - Math.PI / 2);
            double distancetoend = Vector2.Distance(Game1.player.position, shippointposition);

            AutoDecelerate(distancetoend);
            UpdateParticles();

            double extraspeed = acceleration * Game1.game.looptime * (InputManager.boosting ? 2 : 1);
            if (!forcestop)
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

            collider.Move(position);
            collider.Rotate(rotationchange);
            //graphicalcollider.Move(position);
            //graphicalcollider.Rotate(rotationchange);
        }
        public override void Draw()
        {
            if (!disappear)
            {
                Game1.game.spriteBatch.Draw(texture, position, null, Color.White, (float)rotation, new Vector2(texture.Width / 2, texture.Height / 2), 1f, SpriteEffects.None, 1);
            }
            else
            {

            }
        }
        internal static void SpawnRandom()
        {
            //Generate random place on edge
            float edge = GameManager.RandomInt(0, 4); //0:left, 1:top, 2:right, 3:bottom
            Vector2 startposition = new Vector2(0, 0);
            double xdirection = GameManager.RandomDouble() - 0.5;
            double ydirection = GameManager.RandomDouble() - 0.5;


            if (edge == 0)
            {
                startposition.Y = GameManager.RandomFloat(0, Game1.height);
            }
            else if (edge == 1)
            {
                startposition.X = GameManager.RandomFloat(0, Game1.width);
            }
            if (edge == 2)
            {
                startposition.X = Game1.width;
                startposition.Y = GameManager.RandomFloat(0, Game1.height);
            }
            else if (edge == 3)
            {
                startposition.Y = Game1.height;
                startposition.X = GameManager.RandomFloat(0, Game1.width);
            }
            double shiptype = GameManager.RandomDouble();
            if (shiptype < 0.5)
            {
                Game1.basicenemies.Add(new EnemyShip(Game1.enemyship_texture, startposition));
            }
            else
            {
                Game1.eyeenemies.Add(new EyeEnemy(startposition));
            }
        }
        public override void Dispose()
        {
            disappear = true;
            collider.Dispose();
            ParticleManager.particleEmitters.RemoveRange(engineEmitData.enginehandlers);

            //foreach (var particle in ParticleManager.particles)
            //{
            //    particle._lifespanLeft = 0;
            //}
        }
    }
}
