using HyperbolicRenderer;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
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
        public float rotation = 0;
        public const float _rotationSpeed = 3f;
        const int speed = 300;

        public Ship(Texture2D tex, Vector2 pos) : base(tex, pos)
        {
        }

        public void Update()
        {
            rotation += InputManager.direction.X * _rotationSpeed * Game1.game.looptime;
            Vector2 direction = new((float)Math.Sin(rotation), -(float)Math.Cos(rotation));
            position += InputManager.direction.Y * direction * speed * Game1.game.looptime;
        }

        public override void Draw()
        {
            Game1.game.spriteBatch.Draw(texture, position, null, Color.White, rotation, origin, 0.25f, SpriteEffects.None, 1);
        }
    }
    public class Bullet : Sprite
    {
        public Vector destinationdirection;

        public Bullet(Texture2D tex, Vector2 position) : base(tex, position)
        {
            destinationdirection = new Vector(Game1.player.rotation - Math.PI/2).GetUnitVector();
            double distanceaway = Game1.player.texture.Width * 0.25f * 0.5f;
            Vector startadd = destinationdirection * distanceaway;
            this.position.X += (float)(startadd.i);
            this.position.Y += (float)(startadd.j);
        }
        public void Update()
        {
            const int speed = 10;
            position.X += (float)(destinationdirection.i) * speed;
            position.Y += (float)(destinationdirection.j) * speed;
        }
        public override void Draw()
        {
            Game1.game.spriteBatch.Draw(texture, position, null, Color.White, 0, origin, 0.02f, SpriteEffects.None, 1);
        }
    }
}
