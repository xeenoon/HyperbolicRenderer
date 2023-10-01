namespace ImageStretcher
{
    partial class AnimationEditor
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AnimationEditor));
            canvas = new PictureBox();
            importButton = new Button();
            generateButton = new Button();
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
            saveButton = new Button();
            ImportSettingsButton = new Button();
            frameViewer = new Panel();
            startstopButton = new PictureBox();
            panel2 = new Panel();
            zoomoutButton = new PictureBox();
            zoominButton = new PictureBox();
            loadingpanel = new Panel();
            loadingbar = new PictureBox();
            loadingLabel = new Label();
            ((System.ComponentModel.ISupportInitialize)canvas).BeginInit();
            ImportSettingsPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)importSettingsCloseButton).BeginInit();
            ExportPanelSettings.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)exportSettingsCloseButton).BeginInit();
            polygonMenu.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)startstopButton).BeginInit();
            panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)zoomoutButton).BeginInit();
            ((System.ComponentModel.ISupportInitialize)zoominButton).BeginInit();
            loadingpanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)loadingbar).BeginInit();
            SuspendLayout();
            // 
            // canvas
            // 
            canvas.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            canvas.Image = (Image)resources.GetObject("canvas.Image");
            canvas.Location = new Point(52, 6);
            canvas.Name = "canvas";
            canvas.Size = new Size(1520, 921);
            canvas.TabIndex = 0;
            canvas.TabStop = false;
            canvas.Click += AddPoint;
            canvas.Paint += pictureBox1_Paint;
            // 
            // importButton
            // 
            importButton.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            importButton.Location = new Point(1685, 15);
            importButton.Name = "importButton";
            importButton.Size = new Size(104, 23);
            importButton.TabIndex = 1;
            importButton.Text = "Import image";
            importButton.UseVisualStyleBackColor = true;
            importButton.Click += ImportImage;
            // 
            // generateButton
            // 
            generateButton.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            generateButton.BackColor = SystemColors.ActiveCaption;
            generateButton.FlatAppearance.BorderSize = 0;
            generateButton.FlatStyle = FlatStyle.Popup;
            generateButton.Location = new Point(1683, 73);
            generateButton.Name = "generateButton";
            generateButton.Size = new Size(107, 23);
            generateButton.TabIndex = 2;
            generateButton.Text = "Generate frames";
            generateButton.UseVisualStyleBackColor = false;
            generateButton.Click += Generate;
            // 
            // restartButton
            // 
            restartButton.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            restartButton.Location = new Point(1796, 73);
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
            exportGIFButton.Location = new Point(1796, 44);
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
            exportFramesButton.Location = new Point(1685, 44);
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
            importFramesButton.Location = new Point(1795, 15);
            importFramesButton.Name = "importFramesButton";
            importFramesButton.Size = new Size(104, 23);
            importFramesButton.TabIndex = 15;
            importFramesButton.Text = "Import frames";
            importFramesButton.UseVisualStyleBackColor = true;
            importFramesButton.Click += ImportAnimation;
            // 
            // ImportSettingsPanel
            // 
            ImportSettingsPanel.BackColor = SystemColors.ActiveCaption;
            ImportSettingsPanel.Controls.Add(importSettingsCloseButton);
            ImportSettingsPanel.Controls.Add(animationImportButton);
            ImportSettingsPanel.Controls.Add(loopLabel);
            ImportSettingsPanel.Controls.Add(looptypeDropdown);
            ImportSettingsPanel.Controls.Add(delayTextbox);
            ImportSettingsPanel.Controls.Add(delayLabel);
            ImportSettingsPanel.Controls.Add(animationsettingsLabel);
            ImportSettingsPanel.Location = new Point(859, 450);
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
            offsetLabel.Location = new Point(1417, 9);
            offsetLabel.Name = "offsetLabel";
            offsetLabel.Size = new Size(39, 15);
            offsetLabel.TabIndex = 19;
            offsetLabel.Text = "Offset";
            // 
            // offsetTextbox
            // 
            offsetTextbox.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            offsetTextbox.Location = new Point(1462, 6);
            offsetTextbox.Name = "offsetTextbox";
            offsetTextbox.Size = new Size(100, 23);
            offsetTextbox.TabIndex = 20;
            offsetTextbox.Text = "0,0";
            offsetTextbox.TextChanged += MenuItemTextChanged;
            // 
            // ExportPanelSettings
            // 
            ExportPanelSettings.BackColor = SystemColors.ActiveCaption;
            ExportPanelSettings.Controls.Add(exportSettingsCloseButton);
            ExportPanelSettings.Controls.Add(finalExportButton);
            ExportPanelSettings.Controls.Add(framenameTextbox);
            ExportPanelSettings.Controls.Add(framenameLabel);
            ExportPanelSettings.Controls.Add(ExportSettingsTitleLabel);
            ExportPanelSettings.Location = new Point(859, 466);
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
            polygonMenu.Location = new Point(1578, 102);
            polygonMenu.Name = "polygonMenu";
            polygonMenu.Size = new Size(325, 927);
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
            // saveButton
            // 
            saveButton.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            saveButton.Location = new Point(1575, 44);
            saveButton.Name = "saveButton";
            saveButton.Size = new Size(104, 23);
            saveButton.TabIndex = 25;
            saveButton.Text = "Save settings";
            saveButton.UseVisualStyleBackColor = true;
            saveButton.Click += SaveButtonClick;
            // 
            // ImportSettingsButton
            // 
            ImportSettingsButton.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            ImportSettingsButton.Location = new Point(1575, 15);
            ImportSettingsButton.Name = "ImportSettingsButton";
            ImportSettingsButton.Size = new Size(104, 23);
            ImportSettingsButton.TabIndex = 26;
            ImportSettingsButton.Text = "Import settings";
            ImportSettingsButton.UseVisualStyleBackColor = true;
            ImportSettingsButton.Click += ImportSettingsButton_Click;
            // 
            // frameViewer
            // 
            frameViewer.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            frameViewer.BackColor = Color.Black;
            frameViewer.Location = new Point(0, 921);
            frameViewer.Name = "frameViewer";
            frameViewer.Size = new Size(1567, 175);
            frameViewer.TabIndex = 27;
            // 
            // startstopButton
            // 
            startstopButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            startstopButton.Location = new Point(1537, 891);
            startstopButton.Name = "startstopButton";
            startstopButton.Size = new Size(30, 30);
            startstopButton.SizeMode = PictureBoxSizeMode.StretchImage;
            startstopButton.TabIndex = 28;
            startstopButton.TabStop = false;
            startstopButton.Click += startstopButton_Click;
            startstopButton.Paint += startstopButton_Paint;
            // 
            // panel2
            // 
            panel2.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
            panel2.BackColor = Color.FromArgb(50, 117, 168);
            panel2.Controls.Add(zoomoutButton);
            panel2.Controls.Add(zoominButton);
            panel2.Location = new Point(0, 0);
            panel2.Name = "panel2";
            panel2.Size = new Size(50, 921);
            panel2.TabIndex = 29;
            // 
            // zoomoutButton
            // 
            zoomoutButton.Image = (Image)resources.GetObject("zoomoutButton.Image");
            zoomoutButton.Location = new Point(0, 48);
            zoomoutButton.Name = "zoomoutButton";
            zoomoutButton.Size = new Size(50, 50);
            zoomoutButton.SizeMode = PictureBoxSizeMode.StretchImage;
            zoomoutButton.TabIndex = 1;
            zoomoutButton.TabStop = false;
            zoomoutButton.Click += zoomoutButton_Click;
            // 
            // zoominButton
            // 
            zoominButton.Image = (Image)resources.GetObject("zoominButton.Image");
            zoominButton.Location = new Point(0, 0);
            zoominButton.Name = "zoominButton";
            zoominButton.Size = new Size(50, 50);
            zoominButton.SizeMode = PictureBoxSizeMode.StretchImage;
            zoominButton.TabIndex = 0;
            zoominButton.TabStop = false;
            zoominButton.Click += ZoomInClick;
            // 
            // loadingpanel
            // 
            loadingpanel.BackColor = SystemColors.ActiveCaption;
            loadingpanel.Controls.Add(loadingbar);
            loadingpanel.Controls.Add(loadingLabel);
            loadingpanel.Location = new Point(814, 503);
            loadingpanel.Name = "loadingpanel";
            loadingpanel.Size = new Size(297, 78);
            loadingpanel.TabIndex = 30;
            loadingpanel.Visible = false;
            // 
            // loadingbar
            // 
            loadingbar.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            loadingbar.BackColor = SystemColors.ButtonHighlight;
            loadingbar.Location = new Point(0, 43);
            loadingbar.Name = "loadingbar";
            loadingbar.Size = new Size(297, 29);
            loadingbar.TabIndex = 1;
            loadingbar.TabStop = false;
            loadingbar.Paint += loadingbar_Paint;
            // 
            // loadingLabel
            // 
            loadingLabel.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            loadingLabel.Font = new Font("Segoe UI", 14.25F, FontStyle.Bold, GraphicsUnit.Point);
            loadingLabel.Location = new Point(0, 9);
            loadingLabel.Name = "loadingLabel";
            loadingLabel.Size = new Size(297, 23);
            loadingLabel.TabIndex = 0;
            loadingLabel.Text = "Loading Deformations";
            loadingLabel.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // AnimationEditor
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1904, 1041);
            Controls.Add(loadingpanel);
            Controls.Add(panel2);
            Controls.Add(startstopButton);
            Controls.Add(frameViewer);
            Controls.Add(ImportSettingsButton);
            Controls.Add(saveButton);
            Controls.Add(polygonMenu);
            Controls.Add(ExportPanelSettings);
            Controls.Add(offsetTextbox);
            Controls.Add(offsetLabel);
            Controls.Add(ImportSettingsPanel);
            Controls.Add(importFramesButton);
            Controls.Add(exportFramesButton);
            Controls.Add(exportGIFButton);
            Controls.Add(restartButton);
            Controls.Add(generateButton);
            Controls.Add(importButton);
            Controls.Add(canvas);
            DoubleBuffered = true;
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "AnimationEditor";
            Text = "Jello Machine";
            Load += Form1_Load;
            Resize += AnimationEditor_Resize;
            ((System.ComponentModel.ISupportInitialize)canvas).EndInit();
            ImportSettingsPanel.ResumeLayout(false);
            ImportSettingsPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)importSettingsCloseButton).EndInit();
            ExportPanelSettings.ResumeLayout(false);
            ExportPanelSettings.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)exportSettingsCloseButton).EndInit();
            polygonMenu.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)startstopButton).EndInit();
            panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)zoomoutButton).EndInit();
            ((System.ComponentModel.ISupportInitialize)zoominButton).EndInit();
            loadingpanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)loadingbar).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        public PictureBox canvas;
        private Button importButton;
        private Button generateButton;
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
        private Button saveButton;
        private Button ImportSettingsButton;
        private Panel frameViewer;
        private PictureBox startstopButton;
        private Panel panel2;
        private PictureBox zoominButton;
        private PictureBox zoomoutButton;
        private Panel loadingpanel;
        private Label loadingLabel;
        private PictureBox loadingbar;
    }
}