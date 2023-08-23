using HyperbolicRenderer;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SharpDX.Direct2D1.Effects;
using System;
using System.Collections.Generic;
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
        protected override void LoadContent()
        {
            /*List<Shape> drawingshapes = new List<Shape>
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

            */ //Snowman
            int mapsize = (int)(height/2);
            int xoffset = (width - height)/2;
            m = new Map(4, mapsize);
            Map.extracells = 10;
            m.GenerateVolume(0.769f, 0, 0, false);
            m.BakeHeights(10);

            for (int i = 0; i < m.volume.Count; i++)
            {
                Trapezium trapezium = m.volume[i];
                //Scale with a red to green to blue gradient
                double scalingfactor = ((((i % Math.Sqrt(m.volume.Count)) * (i / (float)Math.Sqrt(m.volume.Count))) / (float)(m.volume.Count)));

                if (scalingfactor < 0 || scalingfactor > 1)
                {
                    continue;
                }

                double red = 0;
                double green = 0;
                double blue = 0;
                if (scalingfactor < 0.33f)
                {
                    blue = 255 - ((scalingfactor) * 255 * 3);
                    red = scalingfactor * 3 * 255;
                }
                else if (scalingfactor < 0.66f)
                {
                    red = 255 - ((scalingfactor - 0.33f) * 3 * 255);
                    green = (scalingfactor - 0.33f) * 3 * 255;
                }
                else
                {
                    green = 255 - ((scalingfactor - 0.66f) * 3 * 255);
                    blue = (scalingfactor - 0.66f) * 3 * 255;
                }


                Color result = new Color((int)red, (int)green, (int)blue);
                var points = trapezium.GetPoints(m);
                if (points.Count() <= 2)
                {
                    continue;
                }
                Vector3[] vertices = new Vector3[points.Count()];
                for (int i1 = 0; i1 < points.Length; i1++)
                {
                    System.Drawing.PointF point = points[i1];
                    vertices[i1] = new Vector3((point.X) + xoffset, (point.Y), 0);
                }
                batcher.Draw(vertices, result);
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