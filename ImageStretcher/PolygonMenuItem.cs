using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ImageStretcher
{
    public enum StretchType
    {
        Jello,
        RotateLeft,
        RotateRight,
        Horizontal,
        Vertical,
    }
    public class PolygonMenuItem
    {
        public const int BG_HEIGHT = 130;
        public static string[] transformoptions = new string[5] { "Jello", "RotateLeft", "RotateRight", "Horizontal", "Vertical" };
        public StretchType stretchType = StretchType.Jello;

        public Panel background = new Panel() { Size = new Size(300, BG_HEIGHT), BackColor = Color.White, BorderStyle = BorderStyle.FixedSingle };
        public TextBox polygonlabel = new TextBox();
        public ComboBox dropdown = new ComboBox();
        public PictureBox visiblebutton = new PictureBox();
        public PictureBox closebutton = new PictureBox();
        public PolygonMenu menu;
        public Label periodLabel = new Label();
        public TextBox periodTextbox = new TextBox();


        public Label offsetLabel = new Label();
        public TextBox offsetTextbox = new TextBox();

        public Label amplitudeLabel = new Label();
        public TextBox amplitudeTextbox = new TextBox();

        public double amplitude=0.05;
        public int period = 2;
        public double offset = 0;

        public List<PointF> polygonpoints;
        public PointF[] smallpoints;
        Func<bool> Paint;

        //Baked heights for specific settings
        public double[] bakedjello;

        public PolygonMenuItem(PolygonMenu menu, Func<bool> PaintFunc)
        {
            this.Paint = PaintFunc;
            this.menu = menu;
            polygonpoints = new List<PointF>();

            background.Controls.Add(polygonlabel);
            background.Controls.Add(dropdown);
            background.Controls.Add(visiblebutton);
            background.Controls.Add(closebutton);
            background.Controls.Add(amplitudeLabel);
            background.Controls.Add(amplitudeTextbox);
            background.Controls.Add(periodLabel);
            background.Controls.Add(periodTextbox);
            background.Controls.Add(offsetLabel);
            background.Controls.Add(offsetTextbox);

            background.Click += new EventHandler(Select);

            polygonlabel.Location = new Point(3, 6);
            polygonlabel.BorderStyle = BorderStyle.None;
            polygonlabel.BackColor = Color.White;
            polygonlabel.Text = "Unnamed";
            polygonlabel.Size = new Size(90, 30);
            polygonlabel.Font = new Font("Arial", 12, FontStyle.Bold);
            polygonlabel.AutoSize = true;
           
            dropdown.Location = new Point(95, 2);
            dropdown.Items.AddRange(transformoptions);
            dropdown.SelectedIndex = 0;
            dropdown.DropDownStyle = ComboBoxStyle.DropDownList;
            dropdown.SelectedIndexChanged += StretchTypeChanged;

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

            amplitudeLabel.Location = new Point(3, 40);
            amplitudeLabel.AutoSize = true;
            amplitudeLabel.Text = "Amplitude";

            amplitudeTextbox.Location = new Point(80, 35);
            amplitudeTextbox.Size = new Size(80,20);
            amplitudeTextbox.Text = "0.05";
            amplitudeTextbox.TextChanged += new EventHandler(AmplitudeTextChange);

            periodLabel.Location = new Point(3, 70);
            periodLabel.AutoSize = true;
            periodLabel.Text = "Period";

            periodTextbox.Location = new Point(80, 65);
            periodTextbox.Size = new Size(80, 20);
            periodTextbox.Text = "2";
            periodTextbox.TextChanged += new EventHandler(PeriodTextChange);

            offsetLabel.Location = new Point(3, 100);
            offsetLabel.AutoSize = true;
            offsetLabel.Text = "Offset";

            offsetTextbox.Location = new Point(80, 95);
            offsetTextbox.Size = new Size(80, 20);
            offsetTextbox.Text = "0";
            offsetTextbox.TextChanged += new EventHandler(OffsetTextChange);


            menu.AddItem(this);
        }


        public void Delete(object sender, EventArgs e)
        {
            menu.RemoveItem(this);
            Paint();
        }

        public bool visiblepolygon = true;
        public void ChangeVisibility(object sender, EventArgs e)
        {
            visiblepolygon = !visiblepolygon;
            if (visiblepolygon)
            {
                visiblebutton.Image = Resources.VisibleIcon;
            }
            else
            {
                visiblebutton.Image = Resources.InvisibleIcon;
            }
            Paint();
        }
        public void Select(object sender, EventArgs e)
        {
            menu.SelectItem(this);
        }
        public const int scalesize = 50;
        internal void AddPoint(Point clickpos)
        {
            polygonpoints.Add(clickpos);
            List<PointF> temp = new List<PointF>();
            PointManager.GrahamsAlgorithm(polygonpoints[0], polygonpoints, ref temp);
            polygonpoints.Clear();
            polygonpoints.AddRange(temp);

            smallpoints = polygonpoints.ToArray().ScalePolygon(scalesize);
        }

        void PeriodTextChange(object sender, EventArgs e)
        {
            if (int.TryParse(periodTextbox.Text, out int temp))
            {
                period = temp;
            }
            bakedjello = null;
        }
        void OffsetTextChange(object sender, EventArgs e)
        {
            if (int.TryParse(periodTextbox.Text, out int temp))
            {
                offset = temp;
            }
            bakedjello = null;
        }
        void AmplitudeTextChange(object sender, EventArgs e)
        {
            if (double.TryParse(amplitudeTextbox.Text, out double temp))
            {
                amplitude = temp;
            }
            bakedjello = null;
        }
        void StretchTypeChanged(object sender, EventArgs e)
        {
            stretchType = Enum.Parse<StretchType>((string)dropdown.SelectedItem);
        }
    }
    public class PolygonMenu
    {
        Panel background;
        Button addbutton;
        public PolygonMenuItem selecteditem;
        public List<PolygonMenuItem> menuItems = new List<PolygonMenuItem>();

        public PolygonMenu(Panel background, Button addbutton)
        {
            this.background = background;
            this.addbutton = addbutton;
        }
        public void SelectItem(PolygonMenuItem polygonMenuItem)
        {
            if (selecteditem != null)
            {
                selecteditem.background.BackColor = Color.White;
                selecteditem.polygonlabel.BackColor = Color.White;
                if (polygonMenuItem == selecteditem)
                {
                    selecteditem = null;
                    return;
                }
            }
            polygonMenuItem.background.BackColor = Color.LightGray;
            polygonMenuItem.polygonlabel.BackColor = Color.LightGray;
            selecteditem = polygonMenuItem;
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
