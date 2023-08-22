using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameUI
{
    internal class ShapeBatcher
    {
        GraphicsDevice graphicsDevice;

        VertexPosition[] vertices = new VertexPosition[4];
        int[] indices = new int[6];

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

            graphicsDevice.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, vertices, 0, 4, indices, 0, 2);
        }

        public void Draw()
        {
            effect = new BasicEffect(graphicsDevice);

            indices[0] = (short)(0);
            indices[1] = (short)(1);
            indices[2] = (short)(2);

            indices[3] = (short)(0);
            indices[4] = (short)(2);
            indices[5] = (short)(3);




            vertices[0].Position = new Vector3(-1, -1, 0.5f) + new Vector3(0, 5, 0);
            vertices[1].Position = new Vector3(1, -1, 0.5f) + new Vector3(0, 5, 0);
            vertices[2].Position = new Vector3(1, 1, 0.5f) + new Vector3(0, 5, 0);
            vertices[3].Position = new Vector3(-1, 1, 0.5f) + new Vector3(0, 5, 0);


            vBuffer = new VertexBuffer(graphicsDevice, VertexPosition.VertexDeclaration, 4, BufferUsage.None);
            iBuffer = new IndexBuffer(graphicsDevice, IndexElementSize.ThirtyTwoBits, 6, BufferUsage.None);

            vBuffer.SetData(vertices);
            iBuffer.SetData(indices);
        }
    }
}
