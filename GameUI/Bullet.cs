using HyperbolicRenderer;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace GameUI
{
    public class Bullet : Sprite
    {
        public Vector destinationdirection;

        public Bullet(Texture2D tex, Vector2 position, Vector direction) : base(tex, position)
        {
            this.destinationdirection = direction;
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
            if (!disappear)
            {
                Game1.game.spriteBatch.Draw(texture, position, null, Color.White, 0, origin, 0.02f, SpriteEffects.None, 1);
            }
        }
        public bool disappear;
        public override void Dispose()
        {
            disappear = true;
            base.Dispose();
        }
    }
}
