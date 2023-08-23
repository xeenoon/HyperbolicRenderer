using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SharpDX.DirectWrite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace GameUI
{
    internal class ShapeBatcher
    {
        GraphicsDevice graphicsDevice;

        List<VertexPositionColor> vertices = new List<VertexPositionColor>();
        List<int> indices = new List<int>();

        private VertexBuffer vBuffer;
        private IndexBuffer iBuffer;

        BasicEffect effect;

        public ShapeBatcher(GraphicsDevice graphicsDevice, Texture2D drawingtexture)
        {
            this.graphicsDevice = graphicsDevice;
            this.shapetexture = drawingtexture;
        }
        Texture2D shapetexture;

        public void Render()
        {

            effect.World = Matrix.Identity;
            effect.View = Matrix.CreateLookAt(new Vector3(0, 0, -10), new Vector3(0, 0, -9), Vector3.Up);
            effect.Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver2, graphicsDevice.Viewport.AspectRatio, 0.1f, 1000f);
            effect.VertexColorEnabled = true;

            effect.CurrentTechnique.Passes[0].Apply();

            graphicsDevice.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, vertices.ToArray(), 0, vertices.Count, indices.ToArray(), 0, indices.Count/3);
        }

        public void Draw(Vector3[] points, Color color)
        {
            int vertexstart = vertices.Count();
            //int indicestart = indices.Count();
            //Start at point[0] and step around clockwize, create triangles
            vertices.Add(new VertexPositionColor(new Vector3(points[0].X, points[0].Y, 0), color));
            vertices.Add(new VertexPositionColor(new Vector3(points[1].X, points[1].Y, 0), color));

            for (int i = 2; i < points.Length; ++i)
            {
                vertices.Add(new VertexPositionColor(new Vector3(points[i].X, points[i].Y, 0), color));

                //Add the triangle to the list
                indices.AddRange(new int[3] { vertexstart, vertexstart + (i - 1), vertexstart + i });
            }
            //indices.AddRange(new int[3] { 0, points.Length - 1, points.Length - 2 });
            // vertices.Add(new VertexPosition(new Vector3(points[points.Length-1].X, points[points.Length-1].Y, 0)));

            effect = new BasicEffect(graphicsDevice);

            vBuffer = new VertexBuffer(graphicsDevice, typeof(VertexPositionColor), vertices.Count, BufferUsage.WriteOnly);
            iBuffer = new IndexBuffer(graphicsDevice, IndexElementSize.ThirtyTwoBits, indices.Count, BufferUsage.None);

            vBuffer.SetData(vertices.ToArray());
            iBuffer.SetData(indices.ToArray());
        }
    }
}
