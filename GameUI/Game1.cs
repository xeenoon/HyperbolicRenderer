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
        int _width = 0;
        int _height = 0;
        protected override void Initialize()
        {
            game = this;
            gameManager = new GameManager();
            batcher = new ShapeBatcher(GraphicsDevice);
            _width = Window.ClientBounds.Width;
            _height = Window.ClientBounds.Height;

            width = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            height = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;

            graphics.PreferredBackBufferWidth = width;
            graphics.PreferredBackBufferHeight = height;
            graphics.HardwareModeSwitch = false;

            graphics.IsFullScreen = true;
            graphics.ApplyChanges();
            IsMouseVisible = true;
            GraphicsDevice.RasterizerState = RasterizerState.CullNone;

            base.Initialize();
        }
        Map m;
        int width;
        int height; 
        Stopwatch s = new Stopwatch();
        Texture2D background;
        public static Texture2D bullettexture;
        public static Ship player;
        public static List<Bullet> projectiles = new List<Bullet>();
        public static Game1 game;
        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            background = Content.Load<Texture2D>("spaceimage");
            bullettexture = Content.Load<Texture2D>("Fireball");

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
        public double looptime;
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.DarkBlue);


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