using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageStretcher
{
    internal class PolygonMenuItem
    {
        public const int BG_HEIGHT = 30;
        public static string[] transformoptions = new string[2] { "Jello", "Rotate" };

        public Panel background = new Panel() { Size = new Size(300, BG_HEIGHT), BackColor = Color.White, BorderStyle = BorderStyle.FixedSingle };
        Label polygonlabel = new Label();
        ComboBox dropdown = new ComboBox();
        PictureBox visiblebutton = new PictureBox();
        PictureBox closebutton = new PictureBox();
        PolygonMenu menu;
        public PolygonMenuItem(PolygonMenu menu)
        {
            this.menu = menu;
            background.Controls.Add(polygonlabel);
            background.Controls.Add(dropdown);
            background.Controls.Add(visiblebutton);
            background.Controls.Add(closebutton);

            polygonlabel.Location = new Point(3, 6);
            polygonlabel.Text = "Unnamed";
            polygonlabel.AutoSize = true;
           
            dropdown.Location = new Point(95, 2);
            dropdown.Items.AddRange(transformoptions);
            dropdown.SelectedIndex = 0;
            
            visiblebutton.Location = new Point(225, 3);
            visiblebutton.Size = new Size(40, 20);
            visiblebutton.Image = Resources.VisibleIcon;
            visiblebutton.SizeMode = PictureBoxSizeMode.StretchImage;
            visiblebutton.Click += new EventHandler(ChangeVisibility);


            closebutton.Location = new Point(274, 3);
            closebutton.Size = new Size(20, 20);
            closebutton.Image = Resources.CloseButtonIcon;
            closebutton.SizeMode = PictureBoxSizeMode.StretchImage;
            closebutton.Click += new EventHandler(Delete);

            menu.AddItem(this);
        }

        public void Delete(object sender, EventArgs e)
        {
            menu.RemoveItem(this);
        }

        bool visible = true;
        public void ChangeVisibility(object sender, EventArgs e)
        {
            visible = !visible;
            if (visible)
            {
                visiblebutton.Image = Resources.VisibleIcon;
            }
            else
            {
                visiblebutton.Image = Resources.InvisibleIcon;
            }
        }
    }
    internal class PolygonMenu
    {
        Panel background;
        Button addbutton;
        public List<PolygonMenuItem> menuItems = new List<PolygonMenuItem>();

        public PolygonMenu(Panel background, Button addbutton)
        {
            this.background = background;
            this.addbutton = addbutton;
        }

        public void AddItem(PolygonMenuItem menuItem)
        {
            menuItems.Add(menuItem);
            background.Controls.Add(menuItem.background);
            RecalculatePositons();
        }
        public void RemoveItem(PolygonMenuItem menuItem)
        {
            menuItems.Remove(menuItem);
            background.Controls.Remove(menuItem.background);
            RecalculatePositons();
        }
        public void RecalculatePositons()
        {
            background.AutoScroll = false;
            for (int i = 0; i < menuItems.Count; ++i)
            {
                menuItems[i].background.Location = new Point(0, i * PolygonMenuItem.BG_HEIGHT);
            }
            addbutton.Location = new Point(3, menuItems.Count * PolygonMenuItem.BG_HEIGHT + 3);
            background.AutoScroll = true;
        }
    }
}
