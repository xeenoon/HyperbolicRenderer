using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameUI
{
    internal class ShapeBatcher
    {
        GraphicsDevice graphicsDevice;

        List<VertexPosition> vertices = new List<VertexPosition>();
        List<int> indices = new List<int>();

        private VertexBuffer vBuffer;
        private IndexBuffer iBuffer;

        BasicEffect effect;

        public ShapeBatcher(GraphicsDevice graphicsDevice)
        {
            this.graphicsDevice = graphicsDevice;
        }

        public void Render()
        {
            effect.World = Matrix.Identity;
            effect.View = Matrix.CreateLookAt(new Vector3(0, 0, -10), new Vector3(0, 0, -9), Vector3.Up);
            effect.Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver2, graphicsDevice.Viewport.AspectRatio, 0.1f, 1000f);
            effect.CurrentTechnique.Passes[0].Apply();

            graphicsDevice.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, vertices.ToArray(), 0, vertices.Count, indices.ToArray(), 0, indices.Count/3);
        }

        public void Draw(PointF[] points)
        {
            //Start at point[0] and step around clockwize, create triangles
          // vertices.Add(new VertexPosition(new Vector3(points[0].X, points[0].Y, 0)));
          // vertices.Add(new VertexPosition(new Vector3(points[1].X, points[1].Y, 0)));
          //
          // for (int i = 2; i < points.Length - 1; ++i)
          // {
          //     vertices.Add(new VertexPosition(new Vector3(points[i].X, points[i].Y, 0)));
          //
          //     //Add the triangle to the list
          //     indices.AddRange(new int[3] { 0, i-1, i });
          // }
          // indices.AddRange(new int[3] { 0, points.Length - 1, points.Length - 2 });
          //
           effect = new BasicEffect(graphicsDevice);
            
            indices.Add((short)(0));
            indices.Add((short)(1));
            indices.Add((short)(2));
            
            indices.Add((short)(0));
            indices.Add((short)(2));
            indices.Add((short)(3));
            
            
            
            Vector3 point1 = new Vector3(-1, -1, 0.0f) + new Vector3(0, 5, 0);
            vertices.Add(new VertexPosition(point1));
            vertices.Add(new VertexPosition(new Vector3(1, -1, 0.0f) + new Vector3(0, 5, 0)));
            vertices.Add(new VertexPosition(new Vector3(1, 1, 0.0f) + new Vector3(0, 5, 0)));
            vertices.Add(new VertexPosition(new Vector3(-1, 1, 0.0f) + new Vector3(0, 5, 0)));


            vBuffer = new VertexBuffer(graphicsDevice, VertexPosition.VertexDeclaration, 4, BufferUsage.None);
            iBuffer = new IndexBuffer(graphicsDevice, IndexElementSize.ThirtyTwoBits, 6, BufferUsage.None);

            vBuffer.SetData(vertices.ToArray());
            iBuffer.SetData(indices.ToArray());
        }
    }
}
