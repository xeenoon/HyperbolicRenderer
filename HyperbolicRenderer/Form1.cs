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
            int.TryParse(textBox2.Text, out size);
            if (sides == -1 || size == -1)
            {
                return;
            }
            pictureBox1.Invalidate();
        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            int size = pictureBox1.Width;
            if (sides == -1)
            {
                return;
            }
            PointF[] points = new PointF[sides];

            double radiansstepsize = Math.Tau / sides;
            float radius = pictureBox1.Width / 2;
            for (int i = 0; i < sides; ++i)
            {
                //Draw a ray from the centre until it hits the edge of the square
                //Make this a vector
                double angle = Math.PI - (radiansstepsize * i);

                float x = (float)(Math.Cos(angle) * radius + radius);
                float y = (float)(Math.Sin(angle) * radius + radius);
                points[i] = new PointF(x, y);
            }
            e.Graphics.FillPolygon(new Pen(Color.Green).Brush, points);
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