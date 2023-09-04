using HyperbolicRenderer;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameUI
{
    public class EyeEnemy : Ship
    {
//      MoveableShape graphicalcollider;
        public static Texture2D[] frames = new Texture2D[10];

        //centre -64, -62
        Vector2[] colliderpoints = new Vector2[50] { new Vector2(25, 47), new Vector2(15, 51), new Vector2(6, 58), new Vector2(-3, 59), new Vector2(-10, 56), new Vector2(-16, 52), new Vector2(-23, 49), new Vector2(-29, 45), new Vector2(-38, 44), new Vector2(-47, 45), new Vector2(-54, 41), new Vector2(-55, 37), new Vector2(-56, 31), new Vector2(-55, 26), new Vector2(-52, 20), new Vector2(-55, 13), new Vector2(-59, 7), new Vector2(-63, 1), new Vector2(-61, -6), new Vector2(-56, -12), new Vector2(-53, -18), new Vector2(-53, -25), new Vector2(-55, -31), new Vector2(-55, -35), new Vector2(-56, -41), new Vector2(-49, -45), new Vector2(-42, -46), new Vector2(-34, -47), new Vector2(-28, -47), new Vector2(-21, -51), new Vector2(-14, -54), new Vector2(-9, -59), new Vector2(-2, -62), new Vector2(5, -60), new Vector2(12, -54), new Vector2(19, -52), new Vector2(26, -48), new Vector2(32, -47), new Vector2(40, -46), new Vector2(47, -46), new Vector2(54, -42), new Vector2(55, -32), new Vector2(50, -22), new Vector2(56, -12), new Vector2(61, -4), new Vector2(59, 6), new Vector2(53, 16), new Vector2(54, 26), new Vector2(55, 36), new Vector2(48, 44), };
        
        public EyeEnemy(Vector2 pos) : base(frames[0], pos)
        {
            collider = new Collider(colliderpoints, OnCollision, pos, "EYEENEMY");
            //graphicalcollider = Game1.game.batcher.AddMoveableShape(colliderpoints.Copy().ToArray(), Color.White, Vector2.Zero);
            //graphicalcollider.Move(position);

            engineEmitData = new EngineEmitData(this, Color.Lavender, Color.LightPink, 12, 6.2f, 3);

            maxspeed = 300;
            _rotationSpeed = 1;
            boostable = false;
        }
        double rotationdirection;
        public override void Update() //Allows for player movement
        {
            Vector traveldirection = new Vector(new System.Drawing.PointF(position.X, position.Y), new System.Drawing.PointF(Game1.player.position.X, Game1.player.position.Y)).GetUnitVector();

            double mouseangle = (traveldirection.angle + Math.PI / 2);
            double rotationamount = (_rotationSpeed * Game1.game.looptime);
            double rotationchange = 0;

            if (Math.Abs(rotationdirection - mouseangle) > (rotationamount) || rotationdirection < 0) //Stops snappy movement
            {
                if (mouseangle < rotationdirection && rotationdirection - mouseangle > Math.PI)
                {
                    rotationdirection -= Math.Tau;
                }
                if ((mouseangle - rotationdirection <= Math.PI && rotationdirection < mouseangle))
                {
                    if (rotationdirection + rotationamount <= mouseangle)
                    {
                        rotationchange = rotationamount;
                    }

                }
                else
                {
                    if ((rotationdirection - rotationamount) + Math.Tau >= mouseangle)
                    {
                        rotationchange = -rotationamount;

                    }
                    else
                    {
                        rotationdirection += Math.Tau;
                    }
                }
            }
            rotationdirection += rotationchange;
            rotation += rotationamount;

            var cockpitdirection = new Vector(rotationdirection - Math.PI / 2).GetUnitVector();
            double distanceaway = texture.Height * 0.5f;
            Vector shippoint = cockpitdirection * (distanceaway);
            Vector2 shippointposition = new Vector2((float)(position.X + shippoint.i), (float)(position.Y + shippoint.j));

            const double reloadtime = 0.6;
            if (Game1.game.totalseconds - lasttime >= reloadtime)
            {
                for (int i = 0; i < 4; ++i)
                {
                    double bulletdirection = (rotation - Math.PI / 4) + ((Math.PI / 2) * i);
                    var cannondirection = new Vector(bulletdirection).GetUnitVector();
                    double cannondistanceaway = Math.Sqrt(Math.Pow(texture.Height, 2) + Math.Pow(texture.Width, 2)) * 0.5f;
                    Vector cannonpoint = cannondirection * cannondistanceaway;
                    Vector2 cannonpointposition = new Vector2((float)(position.X + cannonpoint.i), (float)(position.Y + cannonpoint.j));
                    Game1.projectiles.Add(new Bullet(Game1.bullettexture, cannonpointposition, new Vector(bulletdirection).GetUnitVector(), 7));
                }

                lasttime = Game1.game.totalseconds;
            }

            Vector shipengine = (cockpitdirection * -1 * 0.7f) * distanceaway;
            emitpositions[0] = new Vector2((float)(position.X + shipengine.i), (float)(position.Y + shipengine.j));


            Vector mousedirection = new Vector(rotationdirection - Math.PI / 2);
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
        public bool disappear;
        double lasttime = Game1.game.totalseconds;
        double timebetweenframes;
        int framedrawidx = 0;
        bool framedirection = true;
        bool blinking = false;
        public override void Draw()
        {
            if (!disappear)
            {
                timebetweenframes += Game1.game.drawlooptime;
                if (timebetweenframes > 1)
                {
                    timebetweenframes -= 1;
                    if (GameManager.RandomDouble() > 0.0) //Blind 10% of the time
                    {
                        blinking = true;
                        framedirection = true;
                    }
                }
                if (timebetweenframes > 0.05f && blinking) //Change frames every second
                {
                    if (framedrawidx == 9)
                    {
                        framedirection = !framedirection;
                    }
                    if (framedrawidx == 1 && !framedirection) //Going backwards, at 1
                    {
                        blinking = false;
                    }
                    if (framedirection)
                    {
                        ++framedrawidx;
                    }
                    else
                    {
                        --framedrawidx;
                    }
                    timebetweenframes = 0;
                }
                Game1.game.spriteBatch.Draw(frames[framedrawidx], position, null, Color.White, (float)rotation, origin, 1f, SpriteEffects.None, 1);
            }
            else
            {

            }
        }
        public override bool OnCollision(string tag)
        {
            if (tag == "BULLET")
            {
                disappear = true;
            }
            return false;
        }

        public override void Dispose()
        {
            disappear = true;
            collider.Dispose();
            if (engineEmitData != null)
            {
                ParticleManager.particleEmitters.RemoveRange(engineEmitData.enginehandlers);
            }
        }
    }
}
