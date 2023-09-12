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
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            SuspendLayout();
            // 
            // pictureBox1
            // 
            pictureBox1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            pictureBox1.Image = (Image)resources.GetObject("pictureBox1.Image");
            pictureBox1.Location = new Point(12, 12);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(656, 426);
            pictureBox1.TabIndex = 0;
            pictureBox1.TabStop = false;
            pictureBox1.Paint += pictureBox1_Paint;
            // 
            // button1
            // 
            button1.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            button1.Location = new Point(687, 12);
            button1.Name = "button1";
            button1.Size = new Size(104, 23);
            button1.TabIndex = 1;
            button1.Text = "Upload image";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // button2
            // 
            button2.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            button2.BackColor = Color.Lime;
            button2.FlatAppearance.BorderSize = 0;
            button2.FlatStyle = FlatStyle.Popup;
            button2.Location = new Point(682, 148);
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
            label1.Location = new Point(695, 46);
            label1.Name = "label1";
            label1.Size = new Size(39, 15);
            label1.TabIndex = 3;
            label1.Text = "Speed";
            // 
            // textBox1
            // 
            textBox1.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            textBox1.Location = new Point(740, 43);
            textBox1.Name = "textBox1";
            textBox1.Size = new Size(100, 23);
            textBox1.TabIndex = 4;
            textBox1.Text = "1";
            textBox1.TextChanged += textBox1_TextChanged;
            // 
            // textBox2
            // 
            textBox2.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            textBox2.Location = new Point(742, 76);
            textBox2.Name = "textBox2";
            textBox2.Size = new Size(100, 23);
            textBox2.TabIndex = 5;
            textBox2.Text = "2";
            textBox2.TextChanged += textBox1_TextChanged;
            // 
            // textBox3
            // 
            textBox3.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            textBox3.Location = new Point(742, 109);
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
            label2.Location = new Point(695, 81);
            label2.Name = "label2";
            label2.Size = new Size(41, 15);
            label2.TabIndex = 7;
            label2.Text = "Period";
            // 
            // label3
            // 
            label3.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            label3.AutoSize = true;
            label3.Location = new Point(674, 113);
            label3.Name = "label3";
            label3.Size = new Size(63, 15);
            label3.TabIndex = 8;
            label3.Text = "Amplitude";
            // 
            // button3
            // 
            button3.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            button3.Location = new Point(674, 295);
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
            textBox4.Location = new Point(740, 266);
            textBox4.Name = "textBox4";
            textBox4.Size = new Size(100, 23);
            textBox4.TabIndex = 10;
            textBox4.Text = "10";
            // 
            // label4
            // 
            label4.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            label4.AutoSize = true;
            label4.Location = new Point(674, 269);
            label4.Name = "label4";
            label4.Size = new Size(63, 15);
            label4.TabIndex = 11;
            label4.Text = "Resolution";
            // 
            // button4
            // 
            button4.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            button4.Location = new Point(682, 178);
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
            button5.Location = new Point(682, 207);
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
            button6.Location = new Point(682, 236);
            button6.Name = "button6";
            button6.Size = new Size(104, 23);
            button6.TabIndex = 14;
            button6.Text = "Export MP4";
            button6.UseVisualStyleBackColor = true;
            button6.Click += button6_Click;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(855, 450);
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
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
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
    }
}