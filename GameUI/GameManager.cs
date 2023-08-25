using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameUI
{
    public class GameManager
    {

        public GameManager()
        {
            Game1.player = new(Game1.game.Content.Load<Texture2D>("Shipmodel"), new(0, 0));
        }

        public void Update()
        {
            InputManager.Update();
            Game1.player.Update();
            foreach (var bullet in Game1.projectiles)
            {
                bullet.Update();
            }
        }

        public void Draw()
        {
            Game1.player.Draw();
            foreach (var bullet in Game1.projectiles)
            {
                bullet.Draw();
            }
        }
    }
}
