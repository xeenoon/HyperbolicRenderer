using HyperbolicRenderer;
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
        public static Vector2 MousePosition;

        private static MouseState _lastMouseState;
        public static bool HasClicked;

        public static bool mouseClicked;

        static double lasttime = Game1.game.totalseconds;

        public static void Update()
        {
            var mouseState = Mouse.GetState();

            HasClicked = mouseState.LeftButton == ButtonState.Pressed && _lastMouseState.LeftButton == ButtonState.Released;
            MousePosition = mouseState.Position.ToVector2();

            _lastMouseState = mouseState;

            var keyboardState = Keyboard.GetState();
            moving = false;
            if (keyboardState.IsKeyDown(Keys.Space))
            {
                moving = true;
            }

            if (!keyboardState.IsKeyDown(Keys.LeftShift) && boosting) //Just lifted key?
            {
                boosting = false;
                Game1.player.UpdateBoostParticles();
            }
            boosting = false;
            if (keyboardState.IsKeyDown(Keys.LeftShift))
            {
                boosting = true;
                Game1.player.UpdateBoostParticles();
            }


            const double reloadtime = 0.25;
            if (Game1.game.totalseconds - lasttime >= reloadtime && Mouse.GetState().LeftButton == ButtonState.Pressed)
            {
                lasttime = Game1.game.totalseconds;

                var cockpitdirection = new Vector(Game1.player.rotation - Math.PI / 2).GetUnitVector();
                double distanceaway = Game1.player.texture.Height * 0.5f;
                Vector shippointend = cockpitdirection * (distanceaway + Game1.bullettexture.Height);
                Vector2 shippointendposition = new Vector2((float)(Game1.player.position.X + shippointend.i), (float)(Game1.player.position.Y + shippointend.j));

                Game1.projectiles.Add(new Bullet(Game1.bullettexture, shippointendposition, new Vector(Game1.player.rotation - Math.PI / 2).GetUnitVector(), 20));
            }

            mouseClicked = (Mouse.GetState().LeftButton == ButtonState.Pressed) && (lastMouseState.LeftButton == ButtonState.Released);
            lastMouseState = Mouse.GetState();
        }
    }
}
