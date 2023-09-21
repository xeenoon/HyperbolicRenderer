using DirectShowLib.BDA;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageStretcher
{
    public class Frame
    {
        public Panel backgroundpanel;
        PictureBox preview;
        Label name;
        public Frame() 
        {
            backgroundpanel = new Panel()
            {
                Size = new Size(80, 100),
                BackColor = Color.White,
                Location = new Point(5,5),
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

        public FrameCollection(Panel bgpanel)
        {
            this.bgpanel = bgpanel;
        }

        public void GenerateFrames()
        {
            Frame frame = new Frame();
            bgpanel.Controls.Add(frame.backgroundpanel);
        }
    }
}
