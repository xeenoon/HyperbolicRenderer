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
        public float _rotation=0;
        public const float _rotationSpeed = 3f;

        public Ship(Texture2D tex, Vector2 pos, Game1 game) : base(tex, pos, game)
        {
        }

        public void Update()
        {
            _rotation += InputManager.direction.X * _rotationSpeed * game.totalseconds;
            Vector2 direction = new((float)Math.Sin(_rotation), -(float)Math.Cos(_rotation));
            position += InputManager.direction.Y * direction * speed * game.totalseconds;
        }

        public override void Draw()
        {
            game.spriteBatch.Draw(texture, position, null, Color.White, _rotation, origin, 0.25f, SpriteEffects.None, 1);
        }
    }
}
