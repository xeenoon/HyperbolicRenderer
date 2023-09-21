using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Windows.Forms.Control;
using System.Linq;
using System.Windows.Forms;

namespace ImageStretcher
{
    internal class ScrollBar
    {
        public int height;
        public int maxwidth;
        public int width;
        public int offset = 5;
        public Panel parent;
        PictureBox scrollviewarea;

        public ScrollBar(int height, Panel parent)
        {
            this.height = height;
            this.width = parent.Width;
            this.maxwidth = parent.Width;
            this.parent = parent;
            scrollviewarea = new PictureBox();
            scrollviewarea.Size = new Size(parent.Width, height);
            scrollviewarea.Image = new Bitmap(parent.Width, height);
            parent.Controls.Add(scrollviewarea);

            scrollviewarea.MouseDown += new MouseEventHandler(MouseDown);
            scrollviewarea.MouseUp += new MouseEventHandler(MouseUp);
            scrollviewarea.MouseMove += new MouseEventHandler(MouseMove);

            Draw();
        }
        public void SetOffset()
        {
            int highestpixel = parent.Controls.Cast<Control>().ToList().Max(c=>c.Right) + 5; //Find the furtherest right, with a buffer of 5
            if (highestpixel > maxwidth)
            {
                //Scale width
                width = (int)(maxwidth * (maxwidth / (float)highestpixel)) - 5;
            }
            Draw();
        }
        public void Draw()
        {
            var graphics = Graphics.FromImage(scrollviewarea.Image);
            graphics.Clear(Color.Transparent);
            graphics.FillEllipse  (new Pen(Color.Gray).Brush, offset, 0, 10, height);
            graphics.FillEllipse  (new Pen(Color.Gray).Brush, width - 20 + offset, 0, 10, height);
            graphics.FillRectangle(new Pen(Color.Gray).Brush, offset + height/2, 0, width-20, height);
            scrollviewarea.Invalidate();
        }
        int mousedownx;
        bool mousedown;
        public void MouseDown(object sender, EventArgs e)
        {
            mousedown = true;
            var cursorpos = parent.PointToClient(Cursor.Position);
            mousedownx = cursorpos.X;
        }
        int moveticks = 0;
        public void MouseMove(object sender, EventArgs e)
        {
            if (mousedown)
            {
                ++moveticks;
                var cursorpos = parent.PointToClient(Cursor.Position);
                offset = Math.Max(0, cursorpos.X - mousedownx);
                offset = Math.Min(maxwidth - width + height, offset);

                //MouseMove is updated too often
                Draw();
            }
        }
        public void MouseUp(object sender, EventArgs e)
        {
            mousedown = false;
        }
    }
}
