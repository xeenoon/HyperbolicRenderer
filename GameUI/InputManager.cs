﻿using Microsoft.Xna.Framework;
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
        public static Vector2 direction;
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

            direction = Vector2.Zero;
            if (keyboardState.IsKeyDown(Keys.W) || keyboardState.IsKeyDown(Keys.Up))
            {
                direction.Y++;
            }
            if (keyboardState.IsKeyDown(Keys.A) || keyboardState.IsKeyDown(Keys.Left))
            {
                direction.X--;
            }
            if (keyboardState.IsKeyDown(Keys.S) || keyboardState.IsKeyDown(Keys.Down))
            {
                direction.Y-=0.2f;
            }
            if (keyboardState.IsKeyDown(Keys.D) || keyboardState.IsKeyDown(Keys.Right))
            {
                direction.X++;
            }
            if (keyboardState.IsKeyDown(Keys.LeftShift))
            {
                if (direction.Y > 0) //Positive
                {
                    direction.Y *= 1.5f;
                }
            }

            const double reloadtime = 0.25;
            if (Game1.game.totalseconds - lasttime >= reloadtime && keyboardState.IsKeyDown(Keys.Space))
            {
                lasttime = Game1.game.totalseconds;
                Game1.projectiles.Add(new Bullet(Game1.bullettexture, Game1.player.position));
            }

            mouseClicked = (Mouse.GetState().LeftButton == ButtonState.Pressed) && (lastMouseState.LeftButton == ButtonState.Released);
            lastMouseState = Mouse.GetState();
        }
    }
}
