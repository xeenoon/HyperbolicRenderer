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
            importButton = new Button();
            timeButton = new Button();
            restartButton = new Button();
            exportGIFButton = new Button();
            exportFramesButton = new Button();
            importFramesButton = new Button();
            ImportSettingsPanel = new Panel();
            importSettingsCloseButton = new PictureBox();
            animationImportButton = new Button();
            loopLabel = new Label();
            looptypeDropdown = new ComboBox();
            delayTextbox = new TextBox();
            delayLabel = new Label();
            animationsettingsLabel = new Label();
            offsetLabel = new Label();
            offsetTextbox = new TextBox();
            ExportPanelSettings = new Panel();
            exportSettingsCloseButton = new PictureBox();
            finalExportButton = new Button();
            framenameTextbox = new TextBox();
            framenameLabel = new Label();
            ExportSettingsTitleLabel = new Label();
            polygonMenu = new Panel();
            addPolygonButton = new Button();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            ImportSettingsPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)importSettingsCloseButton).BeginInit();
            ExportPanelSettings.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)exportSettingsCloseButton).BeginInit();
            polygonMenu.SuspendLayout();
            SuspendLayout();
            // 
            // pictureBox1
            // 
            pictureBox1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            pictureBox1.Image = (Image)resources.GetObject("pictureBox1.Image");
            pictureBox1.Location = new Point(-2, 0);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(705, 503);
            pictureBox1.TabIndex = 0;
            pictureBox1.TabStop = false;
            pictureBox1.Click += AddPoint;
            pictureBox1.Paint += pictureBox1_Paint;
            // 
            // importButton
            // 
            importButton.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            importButton.Location = new Point(816, 15);
            importButton.Name = "importButton";
            importButton.Size = new Size(104, 23);
            importButton.TabIndex = 1;
            importButton.Text = "Import image";
            importButton.UseVisualStyleBackColor = true;
            importButton.Click += ImportImage;
            // 
            // timeButton
            // 
            timeButton.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            timeButton.BackColor = Color.Lime;
            timeButton.FlatAppearance.BorderSize = 0;
            timeButton.FlatStyle = FlatStyle.Popup;
            timeButton.Location = new Point(814, 73);
            timeButton.Name = "timeButton";
            timeButton.Size = new Size(107, 23);
            timeButton.TabIndex = 2;
            timeButton.Text = "Start time";
            timeButton.UseVisualStyleBackColor = false;
            timeButton.Click += StartStopButton;
            // 
            // restartButton
            // 
            restartButton.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            restartButton.Location = new Point(927, 73);
            restartButton.Name = "restartButton";
            restartButton.Size = new Size(107, 23);
            restartButton.TabIndex = 9;
            restartButton.Text = "Reset";
            restartButton.UseVisualStyleBackColor = true;
            restartButton.Click += Restart;
            // 
            // exportGIFButton
            // 
            exportGIFButton.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            exportGIFButton.Location = new Point(927, 44);
            exportGIFButton.Name = "exportGIFButton";
            exportGIFButton.Size = new Size(107, 23);
            exportGIFButton.TabIndex = 12;
            exportGIFButton.Text = "Export GIF";
            exportGIFButton.UseVisualStyleBackColor = true;
            exportGIFButton.Click += ExportGIF;
            // 
            // exportFramesButton
            // 
            exportFramesButton.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            exportFramesButton.Location = new Point(816, 44);
            exportFramesButton.Name = "exportFramesButton";
            exportFramesButton.Size = new Size(107, 23);
            exportFramesButton.TabIndex = 13;
            exportFramesButton.Text = "Export Frames";
            exportFramesButton.UseVisualStyleBackColor = true;
            exportFramesButton.Click += ExportFramesClick;
            // 
            // importFramesButton
            // 
            importFramesButton.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            importFramesButton.Location = new Point(926, 15);
            importFramesButton.Name = "importFramesButton";
            importFramesButton.Size = new Size(104, 23);
            importFramesButton.TabIndex = 15;
            importFramesButton.Text = "Import frames";
            importFramesButton.UseVisualStyleBackColor = true;
            importFramesButton.Click += ImportAnimation;
            // 
            // ImportSettingsPanel
            // 
            ImportSettingsPanel.Anchor = AnchorStyles.None;
            ImportSettingsPanel.BackColor = SystemColors.ActiveCaption;
            ImportSettingsPanel.Controls.Add(importSettingsCloseButton);
            ImportSettingsPanel.Controls.Add(animationImportButton);
            ImportSettingsPanel.Controls.Add(loopLabel);
            ImportSettingsPanel.Controls.Add(looptypeDropdown);
            ImportSettingsPanel.Controls.Add(delayTextbox);
            ImportSettingsPanel.Controls.Add(delayLabel);
            ImportSettingsPanel.Controls.Add(animationsettingsLabel);
            ImportSettingsPanel.Location = new Point(424, 187);
            ImportSettingsPanel.Name = "ImportSettingsPanel";
            ImportSettingsPanel.Size = new Size(213, 141);
            ImportSettingsPanel.TabIndex = 16;
            ImportSettingsPanel.Visible = false;
            // 
            // importSettingsCloseButton
            // 
            importSettingsCloseButton.Image = (Image)resources.GetObject("importSettingsCloseButton.Image");
            importSettingsCloseButton.Location = new Point(177, 2);
            importSettingsCloseButton.Name = "importSettingsCloseButton";
            importSettingsCloseButton.Size = new Size(20, 20);
            importSettingsCloseButton.SizeMode = PictureBoxSizeMode.StretchImage;
            importSettingsCloseButton.TabIndex = 24;
            importSettingsCloseButton.TabStop = false;
            importSettingsCloseButton.Click += importSettingsCloseButton_Click;
            // 
            // animationImportButton
            // 
            animationImportButton.Location = new Point(55, 108);
            animationImportButton.Name = "animationImportButton";
            animationImportButton.Size = new Size(75, 23);
            animationImportButton.TabIndex = 5;
            animationImportButton.Text = "Import";
            animationImportButton.UseVisualStyleBackColor = true;
            animationImportButton.Click += AnimationImportClick;
            // 
            // loopLabel
            // 
            loopLabel.AutoSize = true;
            loopLabel.Location = new Point(3, 78);
            loopLabel.Name = "loopLabel";
            loopLabel.Size = new Size(78, 15);
            loopLabel.TabIndex = 4;
            loopLabel.Text = "Loop settings";
            // 
            // looptypeDropdown
            // 
            looptypeDropdown.FormattingEnabled = true;
            looptypeDropdown.Items.AddRange(new object[] { "Restart", "Reverse at end" });
            looptypeDropdown.Location = new Point(87, 77);
            looptypeDropdown.Name = "looptypeDropdown";
            looptypeDropdown.Size = new Size(100, 23);
            looptypeDropdown.TabIndex = 3;
            // 
            // delayTextbox
            // 
            delayTextbox.Location = new Point(87, 48);
            delayTextbox.Name = "delayTextbox";
            delayTextbox.Size = new Size(100, 23);
            delayTextbox.TabIndex = 2;
            // 
            // delayLabel
            // 
            delayLabel.AutoSize = true;
            delayLabel.Location = new Point(19, 51);
            delayLabel.Name = "delayLabel";
            delayLabel.Size = new Size(63, 15);
            delayLabel.TabIndex = 1;
            delayLabel.Text = "Delay (ms)";
            // 
            // animationsettingsLabel
            // 
            animationsettingsLabel.AutoSize = true;
            animationsettingsLabel.Font = new Font("Segoe UI", 14.25F, FontStyle.Bold, GraphicsUnit.Point);
            animationsettingsLabel.Location = new Point(26, 9);
            animationsettingsLabel.Name = "animationsettingsLabel";
            animationsettingsLabel.Size = new Size(149, 25);
            animationsettingsLabel.TabIndex = 0;
            animationsettingsLabel.Text = "Import settings";
            // 
            // offsetLabel
            // 
            offsetLabel.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            offsetLabel.AutoSize = true;
            offsetLabel.Location = new Point(548, 9);
            offsetLabel.Name = "offsetLabel";
            offsetLabel.Size = new Size(39, 15);
            offsetLabel.TabIndex = 19;
            offsetLabel.Text = "Offset";
            // 
            // offsetTextbox
            // 
            offsetTextbox.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            offsetTextbox.Location = new Point(593, 6);
            offsetTextbox.Name = "offsetTextbox";
            offsetTextbox.Size = new Size(100, 23);
            offsetTextbox.TabIndex = 20;
            offsetTextbox.Text = "0,0";
            offsetTextbox.TextChanged += MenuItemTextChanged;
            // 
            // ExportPanelSettings
            // 
            ExportPanelSettings.Anchor = AnchorStyles.None;
            ExportPanelSettings.BackColor = SystemColors.ActiveCaption;
            ExportPanelSettings.Controls.Add(exportSettingsCloseButton);
            ExportPanelSettings.Controls.Add(finalExportButton);
            ExportPanelSettings.Controls.Add(framenameTextbox);
            ExportPanelSettings.Controls.Add(framenameLabel);
            ExportPanelSettings.Controls.Add(ExportSettingsTitleLabel);
            ExportPanelSettings.Location = new Point(424, 203);
            ExportPanelSettings.Name = "ExportPanelSettings";
            ExportPanelSettings.Size = new Size(213, 100);
            ExportPanelSettings.TabIndex = 22;
            ExportPanelSettings.Visible = false;
            // 
            // exportSettingsCloseButton
            // 
            exportSettingsCloseButton.Image = (Image)resources.GetObject("exportSettingsCloseButton.Image");
            exportSettingsCloseButton.Location = new Point(177, 3);
            exportSettingsCloseButton.Name = "exportSettingsCloseButton";
            exportSettingsCloseButton.Size = new Size(20, 20);
            exportSettingsCloseButton.SizeMode = PictureBoxSizeMode.StretchImage;
            exportSettingsCloseButton.TabIndex = 23;
            exportSettingsCloseButton.TabStop = false;
            exportSettingsCloseButton.Click += exportSettingsCloseButton_Click;
            // 
            // finalExportButton
            // 
            finalExportButton.Location = new Point(55, 68);
            finalExportButton.Name = "finalExportButton";
            finalExportButton.Size = new Size(75, 23);
            finalExportButton.TabIndex = 3;
            finalExportButton.Text = "Export";
            finalExportButton.UseVisualStyleBackColor = true;
            finalExportButton.Click += finalExportButton_Click;
            // 
            // framenameTextbox
            // 
            framenameTextbox.Location = new Point(87, 39);
            framenameTextbox.Name = "framenameTextbox";
            framenameTextbox.Size = new Size(100, 23);
            framenameTextbox.TabIndex = 2;
            // 
            // framenameLabel
            // 
            framenameLabel.AutoSize = true;
            framenameLabel.Location = new Point(6, 42);
            framenameLabel.Name = "framenameLabel";
            framenameLabel.Size = new Size(78, 15);
            framenameLabel.TabIndex = 1;
            framenameLabel.Text = "Frame names";
            // 
            // ExportSettingsTitleLabel
            // 
            ExportSettingsTitleLabel.AutoSize = true;
            ExportSettingsTitleLabel.Font = new Font("Segoe UI", 14.25F, FontStyle.Bold, GraphicsUnit.Point);
            ExportSettingsTitleLabel.Location = new Point(29, 9);
            ExportSettingsTitleLabel.Name = "ExportSettingsTitleLabel";
            ExportSettingsTitleLabel.Size = new Size(147, 25);
            ExportSettingsTitleLabel.TabIndex = 0;
            ExportSettingsTitleLabel.Text = "Export settings";
            // 
            // polygonMenu
            // 
            polygonMenu.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Right;
            polygonMenu.AutoScroll = true;
            polygonMenu.BackColor = SystemColors.Control;
            polygonMenu.BorderStyle = BorderStyle.FixedSingle;
            polygonMenu.Controls.Add(addPolygonButton);
            polygonMenu.Location = new Point(709, 102);
            polygonMenu.Name = "polygonMenu";
            polygonMenu.Size = new Size(325, 401);
            polygonMenu.TabIndex = 24;
            // 
            // addPolygonButton
            // 
            addPolygonButton.Location = new Point(3, 3);
            addPolygonButton.Name = "addPolygonButton";
            addPolygonButton.Size = new Size(99, 23);
            addPolygonButton.TabIndex = 1;
            addPolygonButton.Text = "Add polygon";
            addPolygonButton.UseVisualStyleBackColor = true;
            addPolygonButton.Click += addPolygonButton_Click;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1035, 515);
            Controls.Add(polygonMenu);
            Controls.Add(ExportPanelSettings);
            Controls.Add(offsetTextbox);
            Controls.Add(offsetLabel);
            Controls.Add(ImportSettingsPanel);
            Controls.Add(importFramesButton);
            Controls.Add(exportFramesButton);
            Controls.Add(exportGIFButton);
            Controls.Add(restartButton);
            Controls.Add(timeButton);
            Controls.Add(importButton);
            Controls.Add(pictureBox1);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "Form1";
            Text = "Jello Machine";
            Load += Form1_Load;
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            ImportSettingsPanel.ResumeLayout(false);
            ImportSettingsPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)importSettingsCloseButton).EndInit();
            ExportPanelSettings.ResumeLayout(false);
            ExportPanelSettings.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)exportSettingsCloseButton).EndInit();
            polygonMenu.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private PictureBox pictureBox1;
        private Button importButton;
        private Button timeButton;
        private Button restartButton;
        private Button exportGIFButton;
        private Button exportFramesButton;
        private Button importFramesButton;
        private Panel ImportSettingsPanel;
        private Label delayLabel;
        private Label animationsettingsLabel;
        private Label loopLabel;
        private ComboBox looptypeDropdown;
        private TextBox delayTextbox;
        private Button animationImportButton;
        private Label offsetLabel;
        private TextBox offsetTextbox;
        private Panel ExportPanelSettings;
        private Button finalExportButton;
        private TextBox framenameTextbox;
        private Label framenameLabel;
        private Label ExportSettingsTitleLabel;
        private PictureBox exportSettingsCloseButton;
        private PictureBox importSettingsCloseButton;
        private Panel polygonMenu;
        private Button addPolygonButton;
    }
}