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

        public ScrollBar(int height, Panel parent, Form main)
        {
            this.height = height;
            this.width = parent.Width;
            this.maxwidth = parent.Width;
            this.parent = parent;
            scrollviewarea = new PictureBox();
            scrollviewarea.Size = new Size(parent.Width, height);
            scrollviewarea.Image = new Bitmap(parent.Width, height);
            main.Controls.Add(scrollviewarea);
            scrollviewarea.Location = parent.Location;
            scrollviewarea.BringToFront();

            scrollviewarea.MouseDown += new MouseEventHandler(MouseDown);
            scrollviewarea.MouseUp += new MouseEventHandler(MouseUp);
            scrollviewarea.MouseMove += new MouseEventHandler(MouseMove);

            Draw();
        }
        float scrollstepsize;
        public void SetOffset()
        {
            int highestpixel = parent.Controls.Cast<Control>().ToList().Max(c=>c.Right) + height; //Find the furtherest right, with a buffer of 5
            if (highestpixel > maxwidth)
            {
                //Scale width
                scrollstepsize = (float)(highestpixel) / (maxwidth-height);
                width = (int)(maxwidth * (maxwidth / (float)highestpixel));
            }
            Draw();
        }
        public void Draw()
        {
            var graphics = Graphics.FromImage(scrollviewarea.Image);
            graphics.Clear(Color.Black);
            graphics.FillEllipse  (new Pen(Color.Gray).Brush, offset, 0, 10, height);
            graphics.FillEllipse  (new Pen(Color.Gray).Brush, width - 20 + offset, 0, 10, height);
            graphics.FillRectangle(new Pen(Color.Gray).Brush, offset + height/2, 0, width-20, height);
            scrollviewarea.Invalidate();
        }
        bool mousedown;

        public void MouseDown(object sender, EventArgs e)
        {
            mousedown = true;
            MouseMove(sender, e);
        }
        int moveticks = 0;
        public void MouseMove(object sender, EventArgs e)
        {
            if (mousedown)
            {
                ++moveticks;
                var cursorpos = scrollviewarea.PointToClient(Cursor.Position);
                cursorpos.X -= width / 2;
                offset = Math.Min(maxwidth - width + height / 2, Math.Max(height / 2, cursorpos.X));

                Draw();
                
                parent.HorizontalScroll.Value = (int)((offset-height/2) * scrollstepsize);
                if (parent.HorizontalScroll.Visible == true || parent.AutoScroll == true)
                {
                    parent.HorizontalScroll.Visible = false;
                }
            }
        }
        public void MouseUp(object sender, EventArgs e)
        {
            mousedown = false;
        }
    }
}
