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
    public static class GameManager
    {
        static Random r = new Random();

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
        public static double lastasteroidtime;
        public static double lastenemytime;
        public static double crashtime = 0;
        public static void Update()
        {
            if (stop)
            {
                if (crashtime == 0)
                {
                    crashtime = Game1.game.totalseconds;
                }
                if (Game1.game.totalseconds - crashtime >= 0) //5 seconds for respawn
                {
                    Game1.game.Reset();
                    crashtime = 0;
                    stop = false;
                }
                return;
            }

            if (Game1.game.totalseconds - lastasteroidtime > 0.4) //1 asteroid per second 0.4 seconds
            {
                lastasteroidtime = Game1.game.totalseconds;
                Asteroid.CreateRandomAsteroid();
            }

            if (Game1.game.totalseconds - lastenemytime > 5) //1 enemy every 5 seconds
            {
                lastenemytime = Game1.game.totalseconds;
                EnemyShip.SpawnRandom();
            }


            InputManager.Update();
            Game1.player.Update();
            foreach (var bullet in Game1.projectiles)
            {
                bullet.Update();
            }
            foreach (var enemy in Game1.enemies)
            {
                enemy.Update();
            }
            foreach (var asteroid in Game1.asteroids)
            {
                asteroid.Update();
            }
            ParticleManager.Update();
            Collider.Update();
        }

        public static void Draw()
        {
            foreach (var bullet in Game1.projectiles)
            {
                bullet.Draw();
            }
            foreach (var asteroid in Game1.asteroids)
            {
                asteroid.Draw();
            }
            foreach (var enemy in Game1.enemies)
            {
                enemy.Draw();
            }

            foreach (var asteroid in Game1.asteroids.Where(a=>a.disappear).ToList())
            {
                asteroid.Dispose();
            }
            foreach (var enemy in Game1.enemies.Where(e => e.disappear).ToList())
            {
                enemy.Dispose();
                Game1.enemies.Remove(enemy);
            }
            Game1.player.Draw();
            ParticleManager.Draw();
        }
        static bool stop;
        internal static void Stop()
        {
            stop = true;
        }
    }
}
