using HyperbolicRenderer;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SharpDX.Direct2D1.Effects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameUI
{
    public class Asteroid : Sprite
    {
        double speed;
        public Vector direction;
        double rotation;
        double rotationspeed;
        public bool disappear;
        public Collider collider;
        public MoveableShape graphicalcollider;

        //Centre -83, -75
        public static Vector2[] colliderpoints = new Vector2[55] { new Vector2(-8, 71), new Vector2(2, 68), new Vector2(11, 59), new Vector2(20, 52), new Vector2(30, 45), new Vector2(40, 41), new Vector2(49, 32), new Vector2(58, 23), new Vector2(61, 13), new Vector2(63, 3), new Vector2(72, -7), new Vector2(78, -17), new Vector2(77, -27), new Vector2(72, -37), new Vector2(73, -47), new Vector2(82, -55), new Vector2(75, -65), new Vector2(67, -71), new Vector2(60, -73), new Vector2(53, -75), new Vector2(44, -75), new Vector2(34, -75), new Vector2(26, -74), new Vector2(19, -73), new Vector2(11, -71), new Vector2(3, -68), new Vector2(-5, -65), new Vector2(-12, -61), new Vector2(-19, -57), new Vector2(-25, -54), new Vector2(-31, -49), new Vector2(-38, -44), new Vector2(-43, -40), new Vector2(-48, -34), new Vector2(-53, -29), new Vector2(-56, -22), new Vector2(-61, -16), new Vector2(-69, -10), new Vector2(-72, -5), new Vector2(-76, 3), new Vector2(-77, 10), new Vector2(-78, 15), new Vector2(-78, 22), new Vector2(-79, 30), new Vector2(-81, 37), new Vector2(-83, 43), new Vector2(-82, 52), new Vector2(-78, 59), new Vector2(-73, 66), new Vector2(-68, 72), new Vector2(-58, 72), new Vector2(-48, 67), new Vector2(-38, 65), new Vector2(-30, 67), new Vector2(-23, 70), }.Reverse().ToArray();
        
        public Asteroid(Texture2D tex, Vector2 pos, Vector direction, double speed) : base(tex, pos)
        {
            this.speed = speed;
            this.direction = direction;
            //rotation = GameManager.RandomFloat(0, 50);
            rotationspeed = GameManager.RandomFloat(0, 5);

            graphicalcollider = Game1.game.batcher.AddMoveableShape(colliderpoints.Copy().ToArray(), Color.White, Vector2.Zero);
            collider = new Collider(colliderpoints, OnCollision, pos, "ASTEROID");
            graphicalcollider.Move(position);
        }
        public bool OnCollision(string tag)
        {
            if (tag == "PLAYER")
            {
                graphicalcollider.color = Color.Red;
                return true;
            }
            return false;
        }
        public override void Draw()
        {
            Game1.game.spriteBatch.Draw(texture, position, null, Color.White, (float)rotation, origin, 1f, SpriteEffects.None, 1);
        }
        public void Update()
        {
            rotation += (rotationspeed * Game1.game.looptime);
            position = new Vector2 (position.X + (float)(speed*Game1.game.looptime*direction.i), position.Y + (float)(speed * Game1.game.looptime * direction.j));
            if (position.X < -100 || position.Y < -100 || position.X > (Game1.game.width + 100) || position.Y > (Game1.game.height+100))
            {
                disappear = true;
            }
            collider.Rotate(rotationspeed * Game1.game.looptime);
            collider.Move(position);
            graphicalcollider.Move(position);
            graphicalcollider.Rotate(rotationspeed * Game1.game.looptime);
        }

        public override void Dispose()
        {
            Game1.asteroids.Remove(this);
            collider.Dispose();
            Game1.game.batcher.shapes.Remove(graphicalcollider);
        }
    }
}
