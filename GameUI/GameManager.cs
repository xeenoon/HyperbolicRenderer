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
        private Ship _ship;

        public GameManager(Game1 game)
        {
            _ship = new(game.Content.Load<Texture2D>("ship"), new(0, 0), game);
        }

        public void Update()
        {
            InputManager.Update();
            _ship.Update();
        }

        public void Draw()
        { 
            _ship.Draw();
        }
    }
}
