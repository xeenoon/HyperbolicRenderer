using DirectShowLib.BDA;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ImageStretcher
{
    public class Frame
    {
        public Panel backgroundpanel;
        public PictureBox preview;
        public Label name;
        public Frame() 
        {
            backgroundpanel = new Panel()
            {
                Size = new Size(80, 100),
                BackColor = Color.White,
                Location = new Point(10,15),
            };

            preview = new PictureBox();
            name = new Label();
            backgroundpanel.Controls.Add(name);
            backgroundpanel.Controls.Add(preview);

            preview.Size = new Size(80,80);
            name.AutoSize = false;
            name.Location = new Point(0,80);
            name.Size = new Size(80,20);
            name.Text = "Master";
            name.TextAlign = ContentAlignment.MiddleCenter;
        }
    }
    internal class FrameCollection
    {
        Panel bgpanel;
        ScrollBar scrollBar;
        Form parent;
        public FrameCollection(Panel bgpanel, Form parent)
        {
            this.parent = parent;
            this.bgpanel = bgpanel;
            bgpanel.AutoScroll = false;
            bgpanel.VerticalScroll.Enabled = false;
            bgpanel.VerticalScroll.Visible = false;
            bgpanel.VerticalScroll.Maximum = 0;
            bgpanel.HorizontalScroll.Minimum = int.MaxValue;

            bgpanel.HorizontalScroll.Enabled = false;
            bgpanel.HorizontalScroll.Visible = false;
            bgpanel.HorizontalScroll.Maximum = int.MaxValue;
            bgpanel.HorizontalScroll.Minimum = 0;
            bgpanel.AutoScroll = true;

            scrollBar = new ScrollBar(10, bgpanel, parent);
        }

        public void GenerateFrames(Bitmap[] frames)
        {
            bgpanel.Controls.Clear();
            bgpanel.Controls.Add(new Frame().backgroundpanel);

            scrollBar = new ScrollBar(10, bgpanel, parent);

            int farright = 100;
            for (int i = 0; i < frames.Length; ++i)
            {
                Frame frame = new Frame();
                var bg = frame.backgroundpanel;
                bgpanel.Controls.Add(bg);
                bg.Location = new Point(bg.Location.X + 90 * (i+1), bg.Location.Y);
                frame.name.Text = i.ToString();
                frame.preview.Image = frames[i];
                frame.preview.SizeMode = PictureBoxSizeMode.StretchImage;
                farright = frame.backgroundpanel.Right;
            }
            Panel buffer = new Panel();
            buffer.Location = new Point(farright, 0);
            buffer.Size = new Size(10, 100);
            buffer.Name = "BUFFER";
            bgpanel.Controls.Add(buffer);
            scrollBar.SetOffset();
            bgpanel.Invalidate();
        }
    }
}
