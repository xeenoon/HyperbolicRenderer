using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageStretcher
{
    internal class LoadingBar
    {
        public double percentloaded;
        public void Draw(Graphics g, int width, int height, int offset) 
        {
            width -= offset*2;
            g.FillRectangle(new Pen(Color.LightGray).Brush, new Rectangle(offset, 0, width, height));
            g.FillRectangle(new Pen(Color.Green).Brush, new Rectangle(offset, 0, (int)(width*percentloaded), height));
        }
    }
}
