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
                Location = new Point(5,15),
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
        public FrameCollection(Panel bgpanel)
        {
            this.bgpanel = bgpanel;
            bgpanel.AutoScroll = false;
            bgpanel.VerticalScroll.Enabled = false;
            bgpanel.VerticalScroll.Visible = false;
            bgpanel.VerticalScroll.Maximum = 0;
            bgpanel.HorizontalScroll.Enabled = false;
            bgpanel.HorizontalScroll.Visible = false;
            bgpanel.HorizontalScroll.Maximum = 0;

            scrollBar = new ScrollBar(10, bgpanel);
        }

        public void GenerateFrames(Bitmap[] frames)
        {
            bgpanel.Controls.Clear();
            bgpanel.Controls.Add(new Frame().backgroundpanel);

            scrollBar = new ScrollBar(10, bgpanel);

            for (int i = 0; i < frames.Length; ++i)
            {
                Frame frame = new Frame();
                var bg = frame.backgroundpanel;
                bgpanel.Controls.Add(bg);
                bg.Location = new Point(bg.Location.X + 90 * (i+1), bg.Location.Y);
                frame.name.Text = i.ToString();
                frame.preview.Image = frames[i];
                frame.preview.SizeMode = PictureBoxSizeMode.StretchImage;
            }
            scrollBar.SetOffset();
            bgpanel.Invalidate();
        }
    }
}
