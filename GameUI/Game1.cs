using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Globalization;
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
        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }
        int _width = 0;
        int _height = 0;
        protected override void Initialize()
        {
            batcher = new ShapeBatcher(GraphicsDevice, Content.Load<Texture2D>("download"));
            _width = Window.ClientBounds.Width;
            _height = Window.ClientBounds.Height;


            graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
            graphics.HardwareModeSwitch = false;

            graphics.IsFullScreen = true;
            graphics.ApplyChanges();
            IsMouseVisible = true;
            GraphicsDevice.RasterizerState = RasterizerState.CullNone;


            base.Initialize();
        }

        protected override void LoadContent()
        {
            List<Shape> drawingshapes = new List<Shape>
            {
                Shape.CreateShape(50, 4.5f, new Vector3(0, -2, 0), Color.Black),
                Shape.CreateShape(50, 4.4f, new Vector3(0, -2, 0), Color.White),
                Shape.CreateShape(50, 3f, new Vector3(0, 3, 0), Color.Black),
                Shape.CreateShape(50, 2.9f, new Vector3(0, 3, 0), Color.White),
                Shape.CreateShape(50, 2f, new Vector3(0, 7, 0), Color.Black),
                Shape.CreateShape(50, 1.9f, new Vector3(0, 7, 0), Color.White),


                Shape.CreateShape(50, 1.5f, new Vector3(0, 7f, 0), Color.Black),
                Shape.CreateShape(50, 1.45f, new Vector3(0, 7.2f, 0), Color.White),

                Shape.CreateShape(50, 0.25f, new Vector3(1, 8f, 0), Color.Black),
                Shape.CreateShape(50, 0.25f, new Vector3(-1, 8f, 0), Color.Black),
                Shape.CreateShape(3, 0.5f, new Vector3(0,7,0), Color.Orange),

                Shape.CreateShape(3, 0.5f, new Vector3(-0.4f,5,0), Color.Red, 0),
                Shape.CreateShape(3, 0.5f, new Vector3(0.4f,5,0), Color.Red, Math.PI),
            };

            foreach (var shape in drawingshapes)
            {
                batcher.Draw(shape.points, shape.color);
            }
        }

        protected override void UnloadContent()
        {
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();


            base.Update(gameTime);
        }


        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Microsoft.Xna.Framework.Color.CornflowerBlue);
            batcher.Render();
            base.Draw(gameTime);
        }
    }
}