using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameUI
{
    public static class InputManager
    {
        private static MouseState lastMouseState;
        public static bool moving = false;
        public static bool boosting = false;
        public static Vector2 MousePosition
        {
            get
            {
                return Mouse.GetState().Position.ToVector2();
            }
        }
        public static bool mouseClicked;

        static double lasttime = Game1.game.totalseconds;

        public static void Update()
        {
            var keyboardState = Keyboard.GetState();
            boosting = false;
            moving = false;
            if (keyboardState.IsKeyDown(Keys.Space))
            {
                moving = true;
            }

            if (keyboardState.IsKeyDown(Keys.LeftShift))
            {
                boosting = true;
            }


            const double reloadtime = 0.25;
            if (Game1.game.totalseconds - lasttime >= reloadtime && Mouse.GetState().LeftButton == ButtonState.Pressed)
            {
                lasttime = Game1.game.totalseconds;
                Game1.projectiles.Add(new Bullet(Game1.bullettexture, Game1.player.position));
            }

            mouseClicked = (Mouse.GetState().LeftButton == ButtonState.Pressed) && (lastMouseState.LeftButton == ButtonState.Released);
            lastMouseState = Mouse.GetState();
        }
    }
}
