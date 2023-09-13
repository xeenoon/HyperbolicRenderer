namespace ImageStretcher
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            pictureBox1 = new PictureBox();
            button1 = new Button();
            button2 = new Button();
            label1 = new Label();
            textBox1 = new TextBox();
            textBox2 = new TextBox();
            textBox3 = new TextBox();
            label2 = new Label();
            label3 = new Label();
            button3 = new Button();
            textBox4 = new TextBox();
            label4 = new Label();
            button4 = new Button();
            button5 = new Button();
            button6 = new Button();
            button7 = new Button();
            panel1 = new Panel();
            button8 = new Button();
            label7 = new Label();
            comboBox1 = new ComboBox();
            textBox5 = new TextBox();
            label6 = new Label();
            label5 = new Label();
            label8 = new Label();
            textBox6 = new TextBox();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            panel1.SuspendLayout();
            SuspendLayout();
            // 
            // pictureBox1
            // 
            pictureBox1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            pictureBox1.Image = (Image)resources.GetObject("pictureBox1.Image");
            pictureBox1.Location = new Point(12, 12);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(668, 426);
            pictureBox1.TabIndex = 0;
            pictureBox1.TabStop = false;
            pictureBox1.Paint += pictureBox1_Paint;
            // 
            // button1
            // 
            button1.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            button1.Location = new Point(707, 12);
            button1.Name = "button1";
            button1.Size = new Size(104, 23);
            button1.TabIndex = 1;
            button1.Text = "Import image";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // button2
            // 
            button2.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            button2.BackColor = Color.Lime;
            button2.FlatAppearance.BorderSize = 0;
            button2.FlatStyle = FlatStyle.Popup;
            button2.Location = new Point(716, 200);
            button2.Name = "button2";
            button2.Size = new Size(104, 23);
            button2.TabIndex = 2;
            button2.Text = "Start time";
            button2.UseVisualStyleBackColor = false;
            button2.Click += button2_Click;
            // 
            // label1
            // 
            label1.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            label1.AutoSize = true;
            label1.Location = new Point(728, 69);
            label1.Name = "label1";
            label1.Size = new Size(39, 15);
            label1.TabIndex = 3;
            label1.Text = "Speed";
            // 
            // textBox1
            // 
            textBox1.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            textBox1.Location = new Point(773, 66);
            textBox1.Name = "textBox1";
            textBox1.Size = new Size(100, 23);
            textBox1.TabIndex = 4;
            textBox1.Text = "1";
            textBox1.TextChanged += textBox1_TextChanged;
            // 
            // textBox2
            // 
            textBox2.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            textBox2.Location = new Point(773, 99);
            textBox2.Name = "textBox2";
            textBox2.Size = new Size(100, 23);
            textBox2.TabIndex = 5;
            textBox2.Text = "2";
            textBox2.TextChanged += textBox1_TextChanged;
            // 
            // textBox3
            // 
            textBox3.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            textBox3.Location = new Point(773, 132);
            textBox3.Name = "textBox3";
            textBox3.Size = new Size(100, 23);
            textBox3.TabIndex = 6;
            textBox3.Text = "0.05";
            textBox3.TextChanged += textBox1_TextChanged;
            // 
            // label2
            // 
            label2.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            label2.AutoSize = true;
            label2.Location = new Point(726, 104);
            label2.Name = "label2";
            label2.Size = new Size(41, 15);
            label2.TabIndex = 7;
            label2.Text = "Period";
            // 
            // label3
            // 
            label3.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            label3.AutoSize = true;
            label3.Location = new Point(704, 136);
            label3.Name = "label3";
            label3.Size = new Size(63, 15);
            label3.TabIndex = 8;
            label3.Text = "Amplitude";
            // 
            // button3
            // 
            button3.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            button3.Location = new Point(708, 347);
            button3.Name = "button3";
            button3.Size = new Size(84, 23);
            button3.TabIndex = 9;
            button3.Text = "Benchmark";
            button3.UseVisualStyleBackColor = true;
            button3.Click += button3_Click;
            // 
            // textBox4
            // 
            textBox4.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            textBox4.Location = new Point(774, 318);
            textBox4.Name = "textBox4";
            textBox4.Size = new Size(100, 23);
            textBox4.TabIndex = 10;
            textBox4.Text = "10";
            // 
            // label4
            // 
            label4.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            label4.AutoSize = true;
            label4.Location = new Point(708, 321);
            label4.Name = "label4";
            label4.Size = new Size(63, 15);
            label4.TabIndex = 11;
            label4.Text = "Resolution";
            // 
            // button4
            // 
            button4.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            button4.Location = new Point(716, 230);
            button4.Name = "button4";
            button4.Size = new Size(104, 23);
            button4.TabIndex = 12;
            button4.Text = "Export GIF";
            button4.UseVisualStyleBackColor = true;
            button4.Click += button4_Click;
            // 
            // button5
            // 
            button5.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            button5.Location = new Point(716, 259);
            button5.Name = "button5";
            button5.Size = new Size(104, 23);
            button5.TabIndex = 13;
            button5.Text = "Export Frames";
            button5.UseVisualStyleBackColor = true;
            button5.Click += button5_Click;
            // 
            // button6
            // 
            button6.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            button6.Location = new Point(716, 288);
            button6.Name = "button6";
            button6.Size = new Size(104, 23);
            button6.TabIndex = 14;
            button6.Text = "Export MP4";
            button6.UseVisualStyleBackColor = true;
            button6.Click += button6_Click;
            // 
            // button7
            // 
            button7.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            button7.Location = new Point(707, 37);
            button7.Name = "button7";
            button7.Size = new Size(104, 23);
            button7.TabIndex = 15;
            button7.Text = "Import frames";
            button7.UseVisualStyleBackColor = true;
            button7.Click += button7_Click;
            // 
            // panel1
            // 
            panel1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            panel1.Controls.Add(button8);
            panel1.Controls.Add(label7);
            panel1.Controls.Add(comboBox1);
            panel1.Controls.Add(textBox5);
            panel1.Controls.Add(label6);
            panel1.Controls.Add(label5);
            panel1.Location = new Point(357, 171);
            panel1.Name = "panel1";
            panel1.Size = new Size(200, 141);
            panel1.TabIndex = 16;
            panel1.Visible = false;
            // 
            // button8
            // 
            button8.Location = new Point(55, 108);
            button8.Name = "button8";
            button8.Size = new Size(75, 23);
            button8.TabIndex = 5;
            button8.Text = "Import";
            button8.UseVisualStyleBackColor = true;
            button8.Click += button8_Click;
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Location = new Point(3, 78);
            label7.Name = "label7";
            label7.Size = new Size(78, 15);
            label7.TabIndex = 4;
            label7.Text = "Loop settings";
            // 
            // comboBox1
            // 
            comboBox1.FormattingEnabled = true;
            comboBox1.Items.AddRange(new object[] { "Restart", "Reverse at end" });
            comboBox1.Location = new Point(87, 77);
            comboBox1.Name = "comboBox1";
            comboBox1.Size = new Size(100, 23);
            comboBox1.TabIndex = 3;
            // 
            // textBox5
            // 
            textBox5.Location = new Point(87, 48);
            textBox5.Name = "textBox5";
            textBox5.Size = new Size(100, 23);
            textBox5.TabIndex = 2;
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Location = new Point(19, 51);
            label6.Name = "label6";
            label6.Size = new Size(63, 15);
            label6.TabIndex = 1;
            label6.Text = "Delay (ms)";
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Font = new Font("Segoe UI", 15.75F, FontStyle.Bold, GraphicsUnit.Point);
            label5.Location = new Point(0, 9);
            label5.Name = "label5";
            label5.Size = new Size(200, 30);
            label5.TabIndex = 0;
            label5.Text = "Animation settings";
            // 
            // label8
            // 
            label8.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            label8.AutoSize = true;
            label8.Location = new Point(733, 167);
            label8.Name = "label8";
            label8.Size = new Size(34, 15);
            label8.TabIndex = 17;
            label8.Text = "Scale";
            // 
            // textBox6
            // 
            textBox6.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            textBox6.Location = new Point(773, 164);
            textBox6.Name = "textBox6";
            textBox6.Size = new Size(100, 23);
            textBox6.TabIndex = 18;
            textBox6.Text = "1";
            textBox6.TextChanged += textBox6_TextChanged;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(888, 450);
            Controls.Add(textBox6);
            Controls.Add(label8);
            Controls.Add(panel1);
            Controls.Add(button7);
            Controls.Add(button6);
            Controls.Add(button5);
            Controls.Add(button4);
            Controls.Add(label4);
            Controls.Add(textBox4);
            Controls.Add(button3);
            Controls.Add(label3);
            Controls.Add(label2);
            Controls.Add(textBox3);
            Controls.Add(textBox2);
            Controls.Add(textBox1);
            Controls.Add(label1);
            Controls.Add(button2);
            Controls.Add(button1);
            Controls.Add(pictureBox1);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "Form1";
            Text = "Jello Machine";
            Load += Form1_Load;
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private PictureBox pictureBox1;
        private Button button1;
        private Button button2;
        private Label label1;
        private TextBox textBox1;
        private TextBox textBox2;
        private TextBox textBox3;
        private Label label2;
        private Label label3;
        private Button button3;
        private TextBox textBox4;
        private Label label4;
        private Button button4;
        private Button button5;
        private Button button6;
        private Button button7;
        private Panel panel1;
        private Label label6;
        private Label label5;
        private Label label7;
        private ComboBox comboBox1;
        private TextBox textBox5;
        private Button button8;
        private Label label8;
        private TextBox textBox6;
    }
}