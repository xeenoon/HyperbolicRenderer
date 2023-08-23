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

        public ShapeBatcher(GraphicsDevice graphicsDevice)
        {
            effect = new BasicEffect(graphicsDevice);
            this.graphicsDevice = graphicsDevice;



        }
        public double buffercopytime = 0;
        public void Render()
        {
            if (vertices.Count() == 0)
            {
                return;
            }

            vBuffer = new VertexBuffer(graphicsDevice, typeof(VertexPositionColor), vertices.Count, BufferUsage.WriteOnly);
            iBuffer = new IndexBuffer(graphicsDevice, IndexElementSize.ThirtyTwoBits, indices.Count, BufferUsage.None);

            vBuffer.SetData(vertices.ToArray());
            iBuffer.SetData(indices.ToArray());
            
            effect.World = Matrix.Identity;
            Matrix projection = Matrix.CreateOrthographicOffCenter(0, graphicsDevice.Viewport.Width, graphicsDevice.Viewport.Height, 0, 0, 1);

            effect.Projection = projection;

            effect.VertexColorEnabled = true;

            effect.CurrentTechnique.Passes[0].Apply();

            stopwatch.Restart();

            graphicsDevice.Indices = iBuffer;
            graphicsDevice.SetVertexBuffer(vBuffer);

            graphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, vertices.Count, 0, indices.Count / 3);



            stopwatch.Stop();
            buffercopytime += stopwatch.ElapsedMilliseconds;
        }
        public double drawtime = 0;
        System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
        public void Draw(Vector3[] points, Color color)
        {
            stopwatch.Restart();
            if (points.Count() <= 2)
            {
                return;
            }

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
            stopwatch.Stop();
            drawtime += stopwatch.ElapsedMilliseconds;
        }
    }
}
