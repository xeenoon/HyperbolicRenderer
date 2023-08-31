using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameUI
{
    public class Sprite : IDisposable
    {
        public readonly Texture2D texture;
        public readonly Vector2 origin;
        public Vector2 position;

        public Sprite(Texture2D tex, Vector2 pos)
        {
            texture = tex;
            position = pos;
            origin = new(tex.Width / 2, tex.Height / 2);
        }

        public virtual void Draw()
        {
            Game1.game.spriteBatch.Draw(texture, position, null, Color.White, 0, origin, 1, SpriteEffects.None, 1);
        }
        public virtual void Dispose()
        {

        }
    }
}
