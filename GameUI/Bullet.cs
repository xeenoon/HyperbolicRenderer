using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Diagnostics;

namespace GameUI
{
    public class Bullet : Sprite
    {
        public Vector destinationdirection;
        public float speed;
        public string team;
        //centre -7,-7
        public Vector2[] colliderpoints = new Vector2[4] { new Vector2(-1, 4), new Vector2(-7, 0), new Vector2(-4, -7), new Vector2(4, -4), };
        Collider collider;
        //MoveableShape graphicalcollider;
        public Bullet(Texture2D tex, Vector2 position, Vector direction, float speed, string team) : base(tex, position)
        {
            this.destinationdirection = direction;

            collider = new Collider(colliderpoints, OnCollision, position, "BULLET:"+team);
            this.speed = speed;
            this.team = team;

            //graphicalcollider = Game1.game.batcher.AddMoveableShape(colliderpoints.Copy().ToArray(), Color.White, Vector2.Zero);
            //graphicalcollider.Move(position);
        }
        public bool OnCollision(string tag)
        {
            if (tag == "PLAYER")
            {
                GameManager.Stop();
                return true;
            }
            if (tag == "ASTEROID_Lava")
            {
                disappear = true;
            }
            if (tag.Contains("BULLET"))
            {
                disappear = true;
            }
            return false;
        }
        public void Update()
        {
            position.X += (float)(destinationdirection.i) * speed;
            position.Y += (float)(destinationdirection.j) * speed;

            if (position.X < 0 || position.Y < 0 || position.X > Game1.width || position.Y > Game1.height)
            {
                Dispose();
            }
            collider.Move(position);
            //graphicalcollider.Move(position);
        }
        public override void Draw()
        {
            if (!disappear)
            {
                Game1.game.spriteBatch.Draw(texture, position, null, Color.White, 0, origin, 1f, SpriteEffects.None, 1);
            }
        }
        public bool disappear;
        public override void Dispose()
        {
            disappear = true;
            collider.Dispose();
        }
    }
}
