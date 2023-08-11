namespace HyperbolicRenderer
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        int areasize = 50000;
        int sides = -1;
        int size = -1;
        
        private void button1_Click(object sender, EventArgs e)
        {
            int.TryParse(textBox1.Text, out sides);
            int.TryParse(textBox2.Text, out sides);
            if (sides == -1 || size == -1)
            {
                return;
            }
            pictureBox1.Invalidate();
        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            PointF[] points = new PointF[sides];

            //Generate corners of polygon

            //4 sides to a picturebox, so sides placed on first 3 sides should be sides/4
            for (int i = 0; i < sides/4; ++i)
            {
                points[i] = new PointF(, 0); //For 2 total iterations
            }
        }
    }
    public class Trapezium
    {
        public Point top_left;
        public Point bottom_left;
        public Point top_right;
        public Point bottom_right;

        public Trapezium(Point top_left, Point bottom_left, Point top_right, Point bottom_right)
        {
            this.top_left = top_left;
            this.bottom_left = bottom_left;
            this.top_right = top_right;
            this.bottom_right = bottom_right;
        }
    }
}


/*
 * width = 100

Sides = 4

100 / 2 = 50

Sides = 5

100 / (3) = 33
100 / (1.5) = 66

...

Sides = 8

100 / 3 = 33
100 / 0.5 = 66

Sides = 9

100 / x = 25
100 / 2 = 50
100 / 1 = 75

let x = ((sides-1) / 4) + 1

100 / x = 25
100 / 2 = 50
100 / 1 = 75
 */