namespace HyperbolicRenderer
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            int sides = -1;
            int size = -1;
            int.TryParse(textBox1.Text, out sides);
            int.TryParse(textBox2.Text, out sides);
            if (sides == -1 || size == -1)
            {
                return;
            }

            //Generate geometry
        }
    }
}