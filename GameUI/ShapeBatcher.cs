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
    public class ShapeBatcher
    {
        GraphicsDevice graphicsDevice;

        internal List<VertexPositionColor> vertices = new List<VertexPositionColor>();
        public List<MoveableShape> shapes = new List<MoveableShape>();
        internal List<int> indices = new List<int>();

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
        public void AddShape(Vector2[] points, Color color)
        {
            stopwatch.Restart();
            if (points.Count() <= 2)
            {
                return;
            }

            int vertexstart = vertices.Count();

            //Start at points[0] and step around clockwize, create triangles
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
        public int AddMoveableShape(Vector2[] points, Color color, Vector2 centre) //Returns the index of the shape
        {
            int vertexstart = vertices.Count; //inclusive
            AddShape(points, color);
            int vertexend = vertices.Count; //exclusive

            shapes.Add(new MoveableShape(vertexstart, vertexend, centre, this));
            return shapes.Count-1;
        }
    }

    public class MoveableShape
    {
        public int vertexstart; 
        public int vertexend;   //Index of vertices, inclusive lower bound, exclusive upper bound for easy iteration
                                //Since we dont add vertices, we dont need an index of indices
        public Vector2 location;
        public ShapeBatcher batcher;

        public MoveableShape(int vertexstart, int vertexend, Vector2 location, ShapeBatcher batcher)
        {
            this.vertexstart = vertexstart;
            this.vertexend = vertexend;
            this.location = location;
            this.batcher = batcher;
        }

        public void Move(Vector2 newlocation)
        {
            Vector3 change = new Vector3((newlocation - location).X, (newlocation - location).Y, 0);
            if (change == Vector3.Zero)
            {
                return;
            }
            for (int i = vertexstart; i < vertexend; ++i)
            {
                Vector3 oldlocation = batcher.vertices[i].Position;
                batcher.vertices[i] = new VertexPositionColor(oldlocation + change, batcher.vertices[i].Color); //Move all the vertices
            }
            location = newlocation;
        }
    }
}
