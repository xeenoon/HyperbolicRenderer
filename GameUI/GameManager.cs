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
        static Random r;
        public GameManager()
        {
            r = new Random();
            Game1.player = new(Game1.game.Content.Load<Texture2D>("Shipmodel"), new(0, 0));
        }

        public static float RandomFloat(float min, float max)
        {
            return (float)(r.NextDouble() * (max - min)) + min;
        }
        public static float RandomInt(int min, int max)
        {
            return r.Next(min,max);
        }
        public static double RandomDouble()
        {
            return r.NextDouble();
        }
        public void Update()
        {
            InputManager.Update();
            Game1.player.Update();
            foreach (var bullet in Game1.projectiles)
            {
                bullet.Update();
            }
            foreach (var asteroid in Game1.asteroids)
            {
                asteroid.Update();
            }
            ParticleManager.Update();
            Collider.Update();
        }

        public void Draw()
        {
            foreach (var bullet in Game1.projectiles)
            {
                bullet.Draw();
            }
            foreach (var asteroid in Game1.asteroids)
            {
                asteroid.Draw();
            }

            foreach (var asteroid in Game1.asteroids.Where(a=>a.disappear).ToList())
            {
                asteroid.Dispose();
            }
            Game1.player.Draw();
            ParticleManager.Draw();
        }
    }
}
