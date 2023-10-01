using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageStretcher
{
    public class ZoomManager
    {
        public List<GraphicsObject> displaydata = new List<GraphicsObject>();
        public int canvaswidth;
        public int canvasheight;
        public Point topleft;
        public float zoom;
        public int realwidth
        {
            get
            {
                return (int)((1 / zoom) * realwidth);
            }
        }
        public int realheight
        {
            get
            {
                return (int)((1 / zoom) * realheight);
            }
        }

        public void DrawCanvas(Graphics g)
        {
            foreach (var graphicsobject in displaydata)
            {
                int newleft  = (int)(graphicsobject.x * zoom);
                int newright = (int)(graphicsobject.y * zoom);
                g.DrawImage(graphicsobject.data, graphicsobject.x, graphicsobject.y, graphicsobject.width, graphicsobject.height);
            }
        }
    }
    public class GraphicsObject
    {
        public int x;
        public int y;
        public int width;
        public int height;
        public Bitmap data;

        public GraphicsObject(int x, int y, int width, int height, Bitmap data)
        {
            this.x = x;
            this.y = y;
            this.width = width;
            this.height = height;
            this.data = data;
        }
    }
}
