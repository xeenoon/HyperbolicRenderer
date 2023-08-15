namespace HyperbolicRenderer
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            pictureBox1 = new PictureBox();
            textBox1 = new TextBox();
            button1 = new Button();
            label1 = new Label();
            label2 = new Label();
            textBox2 = new TextBox();
            checkBox1 = new CheckBox();
            checkBox2 = new CheckBox();
            checkBox3 = new CheckBox();
            checkBox4 = new CheckBox();
            checkBox5 = new CheckBox();
            checkBox6 = new CheckBox();
            checkBox7 = new CheckBox();
            checkBox8 = new CheckBox();
            checkBox9 = new CheckBox();
            label3 = new Label();
            label4 = new Label();
            label5 = new Label();
            label6 = new Label();
            textBox3 = new TextBox();
            button2 = new Button();
            checkBox10 = new CheckBox();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            SuspendLayout();
            // 
            // pictureBox1
            // 
            pictureBox1.Location = new Point(12, 29);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(400, 400);
            pictureBox1.TabIndex = 0;
            pictureBox1.TabStop = false;
            pictureBox1.Paint += pictureBox1_Paint;
            // 
            // textBox1
            // 
            textBox1.Location = new Point(518, 65);
            textBox1.Name = "textBox1";
            textBox1.Size = new Size(100, 23);
            textBox1.TabIndex = 1;
            textBox1.Text = "4";
            // 
            // button1
            // 
            button1.Location = new Point(475, 123);
            button1.Name = "button1";
            button1.Size = new Size(75, 23);
            button1.TabIndex = 2;
            button1.Text = "Generate";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            button1.KeyDown += Form1_KeyDown;
            button1.KeyUp += Form1_KeyUp;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(475, 69);
            label1.Name = "label1";
            label1.Size = new Size(34, 15);
            label1.TabIndex = 3;
            label1.Text = "Sides";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(475, 99);
            label2.Name = "label2";
            label2.Size = new Size(34, 15);
            label2.TabIndex = 4;
            label2.Text = "Scale";
            // 
            // textBox2
            // 
            textBox2.Location = new Point(518, 96);
            textBox2.Name = "textBox2";
            textBox2.Size = new Size(100, 23);
            textBox2.TabIndex = 5;
            textBox2.Text = "0.77";
            // 
            // checkBox1
            // 
            checkBox1.AutoSize = true;
            checkBox1.Location = new Point(657, 65);
            checkBox1.Name = "checkBox1";
            checkBox1.Size = new Size(87, 19);
            checkBox1.TabIndex = 6;
            checkBox1.Text = "Debug data";
            checkBox1.UseVisualStyleBackColor = true;
            checkBox1.Click += checkBox1_CheckedChanged;
            checkBox1.KeyDown += Form1_KeyDown;
            checkBox1.KeyUp += Form1_KeyUp;
            // 
            // checkBox2
            // 
            checkBox2.AutoSize = true;
            checkBox2.Checked = true;
            checkBox2.CheckState = CheckState.Checked;
            checkBox2.ForeColor = Color.FromArgb(255, 128, 0);
            checkBox2.Location = new Point(677, 90);
            checkBox2.Name = "checkBox2";
            checkBox2.Size = new Size(81, 19);
            checkBox2.TabIndex = 11;
            checkBox2.Text = "Gridpoints";
            checkBox2.UseVisualStyleBackColor = true;
            checkBox2.Visible = false;
            checkBox2.Click += checkBox2_CheckedChanged;
            checkBox2.KeyDown += Form1_KeyDown;
            checkBox2.KeyUp += Form1_KeyUp;
            // 
            // checkBox3
            // 
            checkBox3.AutoSize = true;
            checkBox3.Checked = true;
            checkBox3.CheckState = CheckState.Checked;
            checkBox3.ForeColor = Color.Red;
            checkBox3.Location = new Point(677, 138);
            checkBox3.Name = "checkBox3";
            checkBox3.Size = new Size(105, 19);
            checkBox3.TabIndex = 12;
            checkBox3.Text = "Modified Point";
            checkBox3.UseVisualStyleBackColor = true;
            checkBox3.Visible = false;
            checkBox3.Click += checkBox3_CheckedChanged;
            checkBox3.KeyDown += Form1_KeyDown;
            checkBox3.KeyUp += Form1_KeyUp;
            // 
            // checkBox4
            // 
            checkBox4.AutoSize = true;
            checkBox4.Checked = true;
            checkBox4.CheckState = CheckState.Checked;
            checkBox4.ForeColor = Color.Green;
            checkBox4.Location = new Point(677, 162);
            checkBox4.Name = "checkBox4";
            checkBox4.Size = new Size(115, 19);
            checkBox4.TabIndex = 13;
            checkBox4.Text = "Point Movement";
            checkBox4.UseVisualStyleBackColor = true;
            checkBox4.Visible = false;
            checkBox4.Click += checkBox4_CheckedChanged;
            checkBox4.KeyDown += Form1_KeyDown;
            checkBox4.KeyUp += Form1_KeyUp;
            // 
            // checkBox5
            // 
            checkBox5.AutoSize = true;
            checkBox5.Checked = true;
            checkBox5.CheckState = CheckState.Checked;
            checkBox5.ForeColor = Color.Magenta;
            checkBox5.Location = new Point(677, 186);
            checkBox5.Name = "checkBox5";
            checkBox5.Size = new Size(93, 19);
            checkBox5.TabIndex = 14;
            checkBox5.Text = "Closest Edge";
            checkBox5.UseVisualStyleBackColor = true;
            checkBox5.Visible = false;
            checkBox5.Click += checkBox5_CheckedChanged;
            checkBox5.KeyDown += Form1_KeyDown;
            checkBox5.KeyUp += Form1_KeyUp;
            // 
            // checkBox6
            // 
            checkBox6.AutoSize = true;
            checkBox6.Checked = true;
            checkBox6.CheckState = CheckState.Checked;
            checkBox6.ForeColor = Color.Black;
            checkBox6.Location = new Point(677, 210);
            checkBox6.Name = "checkBox6";
            checkBox6.Size = new Size(97, 19);
            checkBox6.TabIndex = 15;
            checkBox6.Text = "Straight Lines";
            checkBox6.UseVisualStyleBackColor = true;
            checkBox6.Visible = false;
            checkBox6.Click += checkBox6_CheckedChanged;
            checkBox6.KeyDown += Form1_KeyDown;
            checkBox6.KeyUp += Form1_KeyUp;
            // 
            // checkBox7
            // 
            checkBox7.AutoSize = true;
            checkBox7.Checked = true;
            checkBox7.CheckState = CheckState.Checked;
            checkBox7.ForeColor = Color.FromArgb(255, 128, 0);
            checkBox7.Location = new Point(677, 114);
            checkBox7.Name = "checkBox7";
            checkBox7.Size = new Size(48, 19);
            checkBox7.TabIndex = 16;
            checkBox7.Text = "Grid";
            checkBox7.UseVisualStyleBackColor = true;
            checkBox7.Visible = false;
            checkBox7.Click += checkBox7_CheckedChanged;
            checkBox7.KeyDown += Form1_KeyDown;
            checkBox7.KeyUp += Form1_KeyUp;
            // 
            // checkBox8
            // 
            checkBox8.AutoSize = true;
            checkBox8.Checked = true;
            checkBox8.CheckState = CheckState.Checked;
            checkBox8.ForeColor = Color.Black;
            checkBox8.Location = new Point(677, 235);
            checkBox8.Name = "checkBox8";
            checkBox8.Size = new Size(90, 19);
            checkBox8.TabIndex = 17;
            checkBox8.Text = "Background";
            checkBox8.UseVisualStyleBackColor = true;
            checkBox8.Visible = false;
            checkBox8.Click += checkBox8_CheckedChanged;
            checkBox8.KeyDown += Form1_KeyDown;
            checkBox8.KeyUp += Form1_KeyUp;
            // 
            // checkBox9
            // 
            checkBox9.AutoSize = true;
            checkBox9.Location = new Point(475, 186);
            checkBox9.Name = "checkBox9";
            checkBox9.Size = new Size(124, 19);
            checkBox9.TabIndex = 18;
            checkBox9.Text = "Infinite movement";
            checkBox9.UseVisualStyleBackColor = true;
            checkBox9.CheckedChanged += checkBox9_CheckedChanged;
            checkBox9.KeyDown += Form1_KeyDown;
            checkBox9.KeyUp += Form1_KeyUp;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Font = new Font("Segoe UI", 12F, FontStyle.Bold, GraphicsUnit.Point);
            label3.Location = new Point(657, 38);
            label3.Name = "label3";
            label3.Size = new Size(61, 21);
            label3.TabIndex = 19;
            label3.Text = "Debug";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Font = new Font("Segoe UI", 12F, FontStyle.Bold, GraphicsUnit.Point);
            label4.Location = new Point(475, 38);
            label4.Name = "label4";
            label4.Size = new Size(95, 21);
            label4.TabIndex = 20;
            label4.Text = "Generation";
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Font = new Font("Segoe UI", 12F, FontStyle.Bold, GraphicsUnit.Point);
            label5.Location = new Point(475, 158);
            label5.Name = "label5";
            label5.Size = new Size(93, 21);
            label5.TabIndex = 21;
            label5.Text = "Movement";
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Location = new Point(475, 229);
            label6.Name = "label6";
            label6.Size = new Size(39, 15);
            label6.TabIndex = 22;
            label6.Text = "Speed";
            // 
            // textBox3
            // 
            textBox3.Location = new Point(520, 225);
            textBox3.Name = "textBox3";
            textBox3.Size = new Size(99, 23);
            textBox3.TabIndex = 23;
            textBox3.Text = "1";
            // 
            // button2
            // 
            button2.Location = new Point(476, 252);
            button2.Name = "button2";
            button2.Size = new Size(75, 23);
            button2.TabIndex = 24;
            button2.Text = "Apply";
            button2.UseVisualStyleBackColor = true;
            button2.Click += button2_Click;
            button2.KeyDown += Form1_KeyDown;
            button2.KeyUp += Form1_KeyUp;
            // 
            // checkBox10
            // 
            checkBox10.AutoSize = true;
            checkBox10.Location = new Point(475, 206);
            checkBox10.Name = "checkBox10";
            checkBox10.Size = new Size(80, 19);
            checkBox10.TabIndex = 25;
            checkBox10.Text = "Invert odd";
            checkBox10.UseVisualStyleBackColor = true;
            checkBox10.CheckedChanged += checkBox10_CheckedChanged;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(checkBox10);
            Controls.Add(button2);
            Controls.Add(textBox3);
            Controls.Add(label6);
            Controls.Add(label5);
            Controls.Add(label4);
            Controls.Add(label3);
            Controls.Add(checkBox9);
            Controls.Add(checkBox8);
            Controls.Add(checkBox7);
            Controls.Add(checkBox6);
            Controls.Add(checkBox5);
            Controls.Add(checkBox4);
            Controls.Add(checkBox3);
            Controls.Add(checkBox2);
            Controls.Add(checkBox1);
            Controls.Add(textBox2);
            Controls.Add(label2);
            Controls.Add(label1);
            Controls.Add(button1);
            Controls.Add(textBox1);
            Controls.Add(pictureBox1);
            Name = "Form1";
            Text = "Form1";
            Load += Form1_Load;
            KeyDown += Form1_KeyDown;
            KeyUp += Form1_KeyUp;
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private PictureBox pictureBox1;
        private TextBox textBox1;
        private Button button1;
        private Label label1;
        private Label label2;
        private TextBox textBox2;
        private CheckBox checkBox1;
        private CheckBox checkBox2;
        private CheckBox checkBox3;
        private CheckBox checkBox4;
        private CheckBox checkBox5;
        private CheckBox checkBox6;
        private CheckBox checkBox7;
        private CheckBox checkBox8;
        private CheckBox checkBox9;
        private Label label3;
        private Label label4;
        private Label label5;
        private Label label6;
        private TextBox textBox3;
        private Button button2;
        private CheckBox checkBox10;
    }
}