using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameUI
{
    public class Sprite
    {
        public readonly Texture2D texture;
        public readonly Vector2 origin;
        public Vector2 position;
        public int speed;
        public Game1 game;

        public Sprite(Texture2D tex, Vector2 pos, Game1 game)
        {
            texture = tex;
            position = pos;
            speed = 300;
            origin = new(tex.Width / 2, tex.Height / 2);
            this.game = game;
        }

        public virtual void Draw()
        {
            game.spriteBatch.Draw(texture, position, null, Color.White, 0, origin, 1, SpriteEffects.None, 1);
        }
    }
}
