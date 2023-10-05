using DirectShowLib.BDA;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ImageStretcher
{
    public class Frame
    {
        public Panel backgroundpanel;
        public PictureBox preview;
        public Bitmap frameimg;
        public Label name;
        FrameCollection frames;
        public Frame(FrameCollection frames)
        {
            this.frames = frames;
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
            backgroundpanel.Click += Click;
            preview.Click += Click;
            name.Click += Click;
        }
        public void Click(object sender, EventArgs e)
        {
            if (frames.selectedframe == this)
            {
                frames.selectedframe = frames.master;
                backgroundpanel.BackColor = Color.White;
                frames.master.backgroundpanel.BackColor = Color.LightGray;
            }

            frames.selectedframe.backgroundpanel.BackColor = Color.White;
            frames.selectedframe = this;
            backgroundpanel.BackColor = Color.LightGray;
            frames.parent.canvas.Invalidate();
        }
    }
    public class FrameCollection
    {
        Panel bgpanel;
        ScrollBar scrollBar;
        public AnimationEditor parent;
        public Frame selectedframe = null;
        public Frame master;
        public List<Frame> frames = new List<Frame>();
        public FrameCollection(Panel bgpanel, AnimationEditor parent)
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
            Frame master = new Frame(this);
            master.preview.Image = frames[0];
            master.frameimg = frames[0];
            master.preview.SizeMode = PictureBoxSizeMode.StretchImage;
            master.backgroundpanel.BackColor = Color.LightGray;
            selectedframe = master;
            this.master = master;

            bgpanel.Controls.Add(master.backgroundpanel);

            scrollBar.Dispose();
            scrollBar = new ScrollBar(10, bgpanel, parent);

            int farright = 100;
            for (int i = 1; i < frames.Length; ++i)
            {
                Frame frame = new Frame(this);
                var bg = frame.backgroundpanel;
                bgpanel.Controls.Add(bg);
                bg.Location = new Point(bg.Location.X + 90 * (i), bg.Location.Y);
                frame.name.Text = i.ToString();
                frame.preview.Image = frames[i];
                frame.frameimg = frames[i];
                frame.preview.SizeMode = PictureBoxSizeMode.StretchImage;
                farright = frame.backgroundpanel.Right;
                this.frames.Add(frame);
            }
            Panel buffer = new Panel();
            buffer.Location = new Point(farright, 0);
            buffer.Size = new Size(10, 100);
            buffer.Name = "BUFFER";
            bgpanel.Controls.Add(buffer);
            scrollBar.SetOffset();
            bgpanel.Invalidate();
        }
        public void Resize(object sender, EventArgs e)
        {
            int oldoffset = scrollBar.offset;
            scrollBar.Dispose();
            scrollBar = new ScrollBar(10, bgpanel, parent);
            scrollBar.SetOffset();
            scrollBar.offset = oldoffset;
            scrollBar.Draw();
        }
    }
}
