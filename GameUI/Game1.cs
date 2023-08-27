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
        ShapeBatcher batcher;

        public SpriteBatch spriteBatch;
        public GameManager gameManager;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }
        protected override void Initialize()
        {
            game = this;
            gameManager = new GameManager();
            batcher = new ShapeBatcher(GraphicsDevice);
            width = Window.ClientBounds.Width;
            height = Window.ClientBounds.Height;

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

        Map m;
        public int width;
        public int height; 
        Stopwatch s = new Stopwatch();
        Texture2D background;
        public static Texture2D bullettexture;
        public static Texture2D asteroidtexture;
        public static Ship player;
        public static List<Asteroid> asteroids = new List<Asteroid>();
        public static List<Bullet> projectiles = new List<Bullet>();
        public static Game1 game;
        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            background = Content.Load<Texture2D>("spaceimage");
            bullettexture = Content.Load<Texture2D>("Fireball");
            asteroidtexture = Content.Load<Texture2D>("Asteroid1");

            return;
            int mapsize = (int)(height/2);
            int xoffset = (width - height)/2;
            m = new Map(4, mapsize);
            int extrasize = 100;
            Map.extracells = extrasize;
            m.GenerateVolume(0.769f, 0, 0, false);
            //xoffset += (int)(m.squaresize * 5);
            m.GenerateVolume(0.769f, -m.squaresize * (extrasize/2), -m.squaresize * (extrasize/2), false);
            m.BakeHeights(10);
            s.Start();
            for (int i = 0; i < m.volume.Count; i++)
            {
                Trapezium outertrapezium = m.volume[i];
                const float bordersize = 2;
                Trapezium innertrapezium = new()
                {
                    top_left = new System.Drawing.PointF(outertrapezium.top_left.X + bordersize, outertrapezium.top_left.Y + bordersize),
                    top_right = new System.Drawing.PointF(outertrapezium.top_right.X - bordersize, outertrapezium.top_right.Y + bordersize),
                    bottom_left = new System.Drawing.PointF(outertrapezium.bottom_left.X + bordersize, outertrapezium.bottom_left.Y - bordersize),
                    bottom_right = new System.Drawing.PointF(outertrapezium.bottom_right.X - bordersize, outertrapezium.bottom_right.Y - bordersize)
                };


                var outerpoints = outertrapezium.GetPoints(m);
                var innerpoints = innertrapezium.GetPoints(m);
                if (outerpoints.Count() <= 2)
                {
                    continue;
                }
                
                batcher.Draw(outerpoints, new Color(0, 84, 22));
                batcher.Draw(innerpoints, Color.DarkBlue);
            }
            s.Stop();
            rendertime += s.ElapsedMilliseconds;
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
            gameManager.Update();
            base.Update(gameTime);
        }

        double rendertime;
        public double totalseconds;
        private double lastasteroidtime;
        public double looptime;
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.DarkBlue);

            if (totalseconds - lastasteroidtime > 1) //1 asteroid per second
            {
                lastasteroidtime = totalseconds;
                //Generate random place on edge
                float edge = GameManager.RandomInt(0, 4); //0:left, 1:top, 2:right, 3:bottom
                Vector2 startposition = new Vector2(0, 0);
                if (edge == 0)
                {
                    startposition.Y = GameManager.RandomFloat(0, height);
                }
                else if (edge == 1) 
                {
                    startposition.X = GameManager.RandomFloat(0, width);
                }
                if (edge == 2)
                {
                    startposition.X = width;
                    startposition.Y = GameManager.RandomFloat(0, height);
                }
                else if (edge == 3)
                {
                    startposition.Y = height;
                    startposition.X = GameManager.RandomFloat(0, width);
                }
                asteroids.Add(new Asteroid(asteroidtexture, startposition, new Vector(GameManager.RandomDouble()-0.5, GameManager.RandomDouble()-0.5).GetUnitVector(), GameManager.RandomFloat(200,500)));
            }

            spriteBatch.Begin();
            spriteBatch.Draw(background, new Vector2(0, 0), null, Color.White, 0, new Vector2(0, 0), 1.5f, SpriteEffects.None, 1);
            gameManager.Draw();
            spriteBatch.End();


            s.Restart();
            batcher.Render();
            s.Stop();
            rendertime += s.ElapsedMilliseconds;

            base.Draw(gameTime);
        }
    }
}