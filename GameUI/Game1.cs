using HyperbolicRenderer;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SharpDX.Direct3D9;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Net.Http;

namespace GameUI
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        public ShapeBatcher batcher;

        public SpriteBatch spriteBatch;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }
        protected override void Initialize()
        {
            game = this;
            batcher = new ShapeBatcher(GraphicsDevice);
            
            width = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            height = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;



            graphics.PreferredBackBufferWidth = width;
            graphics.PreferredBackBufferHeight = height;
            graphics.HardwareModeSwitch = false;

            graphics.IsFullScreen = true;
            graphics.ApplyChanges();
            IsMouseVisible = true;
            GraphicsDevice.RasterizerState = RasterizerState.CullNone;


            InitializeParticles();

            base.Initialize();
        }

        public StaticEmitter _staticEmitter = new(new(300, 300));
        void InitializeParticles()
        {
        }

        public static int width;
        public static int height; 
        Stopwatch s = new Stopwatch();
        Texture2D background;
        public static Texture2D bullettexture;
        public static Texture2D large_asteroidtexture;
        public static Texture2D medium_asteroidtexture;
        public static Texture2D small_asteroidtexture;
        public static Texture2D enemyship_texture;
        public static PlayerShip player;

        public static List<EnemyShip> basicenemies = new List<EnemyShip>();
        public static List<EyeEnemy> eyeenemies = new List<EyeEnemy>();

        public static List<Asteroid> asteroids = new List<Asteroid>();
        public static List<Bullet> projectiles = new List<Bullet>();
        public static Game1 game;
        public static Map map;
        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            background = Content.Load<Texture2D>("spaceimage");
            bullettexture = Content.Load<Texture2D>("Fireball");
            large_asteroidtexture = Content.Load<Texture2D>("Asteroid1");
            medium_asteroidtexture = Content.Load<Texture2D>("Asteroid2");
            small_asteroidtexture = Content.Load<Texture2D>("Asteroid3");

            enemyship_texture = Content.Load<Texture2D>("enemyship");

            EyeEnemy.frames[0] = Content.Load<Texture2D>("bosseyeframe_1");
            EyeEnemy.frames[1] = Content.Load<Texture2D>("bosseyeframe_2");
            EyeEnemy.frames[2] = Content.Load<Texture2D>("bosseyeframe_3");
            EyeEnemy.frames[3] = Content.Load<Texture2D>("bosseyeframe_4");
            EyeEnemy.frames[4] = Content.Load<Texture2D>("bosseyeframe_5");
            EyeEnemy.frames[5] = Content.Load<Texture2D>("bosseyeframe_6");
            EyeEnemy.frames[6] = Content.Load<Texture2D>("bosseyeframe_7");
            EyeEnemy.frames[7] = Content.Load<Texture2D>("bosseyeframe_8");
            EyeEnemy.frames[8] = Content.Load<Texture2D>("bosseyeframe_9");
            EyeEnemy.frames[9] = Content.Load<Texture2D>("bosseyeframe_10");

            player = new PlayerShip(Game1.game.Content.Load<Texture2D>("Shipmodel"), new Vector2(width / 2, height / 2));

            int mapsize = (int)(height / 2);
            Map.extracells = 100;
            map = new Map(4, mapsize, new System.Drawing.PointF(0,0));
            map.GenerateVolume(0.769f, mapsize / 4, 0, false);
            map.BakeHeights(10);
        }

        protected override void UnloadContent()
        {
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            totalseconds += gameTime.ElapsedGameTime.TotalSeconds;
            looptime = gameTime.ElapsedGameTime.TotalSeconds;
            GameManager.Update();
            base.Update(gameTime);
        }

        double rendertime;
        public double totalseconds;
        private double lastasteroidtime;
        public double looptime;
        public double drawlooptime;
        protected override void Draw(GameTime gameTime)
        {
            drawlooptime = gameTime.ElapsedGameTime.TotalSeconds;
            GraphicsDevice.Clear(Color.DarkBlue);
            spriteBatch.Begin();
            spriteBatch.Draw(background, new Vector2(0, 0), null, Color.White, 0, new Vector2(0, 0), 1.5f, SpriteEffects.None, 1);
            GameManager.Draw();
            spriteBatch.End();

            batcher.Render();

            rendertime += s.ElapsedMilliseconds;

            base.Draw(gameTime);
        }
        public static Vector2 AdjustFunc(Vector2 input, Vector2 offset)
        {
            float offsetx = width / 4;

            Vector2 newinput = new Vector2(input.X + offsetx + offset.X, input.Y + offset.Y);
            Vector2 output = map.StretchPoint(new System.Drawing.PointF(newinput.X, newinput.Y)).ToVector2();
            output.X -= offsetx;
            output.X -= offset.X;
            output.Y -= offset.Y;

            return new Vector2((int)output.X, (int)output.Y);
        }
        internal void Reset()
        {
            foreach (var asteroid in asteroids)
            {
                asteroid.disappear = true;
            }
            foreach (var enemy in basicenemies)
            {
                enemy.Dispose();
            }
            basicenemies.Clear();

            foreach (var enemy in eyeenemies)
            {
                enemy.Dispose();
            }
            eyeenemies.Clear();

            foreach (var enemy in basicenemies)
            {
                enemy.Dispose();
            }
            basicenemies.Clear();
            foreach (var bullet in projectiles)
            {
                bullet.Dispose();
            }
            foreach (var particle in ParticleManager.particles)
            {
                particle._lifespanLeft = 0;
            }
            var amount = Collider.colliders.Count();
            GameManager.lastasteroidtime = 0;
            GameManager.lastenemytime = 0;

            ParticleManager.particleEmitters.RemoveRange(player.engineEmitData.enginehandlers);


            player.position = new Vector2(width / 2, height / 2);
        }
    }
}