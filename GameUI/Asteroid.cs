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
    public class Asteroid : Sprite
    {
        double speed;
        public Vector direction;
        double rotation;
        double rotationspeed;
        public bool disappear;
        public Collider collider;

        public Asteroid(Texture2D tex, Vector2 pos, Vector direction, double speed) : base(tex, pos)
        {
            this.speed = speed;
            this.direction = direction;
            rotation = GameManager.RandomFloat(0, 50);
            rotationspeed = GameManager.RandomFloat(0, 5);

            collider = new Collider(new List<Vector2>() { new Vector2(pos.X - 10, pos.Y - 10), new Vector2(pos.X + 10, pos.Y - 10), new Vector2(pos.X + 10, pos.Y + 10), new Vector2(pos.X - 10, pos.Y + 10) }, OnCollision, pos);
        }
        public bool OnCollision()
        {
            return false;
        }
        public override void Draw()
        {
            Game1.game.spriteBatch.Draw(texture, position, null, Color.White, (float)rotation, origin, 1f, SpriteEffects.None, 1);
            Game1.game.batcher.Draw(collider.points.ToArray(), Color.White);
        }
        public void Update()
        {
            rotation += (rotationspeed * Game1.game.looptime);
            position = new Vector2 (position.X + (float)(speed*Game1.game.looptime*direction.i), position.Y + (float)(speed * Game1.game.looptime * direction.j));
            if (position.X < -100 || position.Y < -100 || position.X > (Game1.game.width + 100) || position.Y > (Game1.game.height+100))
            {
                disappear = true;
            }
        }
    }
}
