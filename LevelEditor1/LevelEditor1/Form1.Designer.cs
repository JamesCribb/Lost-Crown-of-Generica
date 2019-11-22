namespace LevelEditor1
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.LevelEditorPanel = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.TileListFLP = new System.Windows.Forms.FlowLayoutPanel();
            this.LmbPB = new System.Windows.Forms.PictureBox();
            this.RmbPB = new System.Windows.Forms.PictureBox();
            this.NewSaveButton = new System.Windows.Forms.Button();
            this.LoadFileButton = new System.Windows.Forms.Button();
            this.ObjectListFLP = new System.Windows.Forms.FlowLayoutPanel();
            this.MobileListFLP = new System.Windows.Forms.FlowLayoutPanel();
            this.OverwriteSaveButton = new System.Windows.Forms.Button();
            this.LayerSelectCB = new System.Windows.Forms.ComboBox();
            this.zoneTriggerPanel = new LevelEditor1.ZoneTriggerPanel();
            this.InteractibleListFLP = new System.Windows.Forms.FlowLayoutPanel();
            this.InteractiblePanel = new LevelEditor1.InteractiblePanel();
            ((System.ComponentModel.ISupportInitialize)(this.LmbPB)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.RmbPB)).BeginInit();
            this.SuspendLayout();
            // 
            // LevelEditorPanel
            // 
            this.LevelEditorPanel.Location = new System.Drawing.Point(392, 16);
            this.LevelEditorPanel.Name = "LevelEditorPanel";
            this.LevelEditorPanel.Size = new System.Drawing.Size(640, 640);
            this.LevelEditorPanel.TabIndex = 0;
            this.LevelEditorPanel.Paint += new System.Windows.Forms.PaintEventHandler(this.LevelEditorPanel_Paint);
            this.LevelEditorPanel.MouseClick += new System.Windows.Forms.MouseEventHandler(this.LevelEditorPanel_MouseClick);
            this.LevelEditorPanel.MouseMove += new System.Windows.Forms.MouseEventHandler(this.LevelEditorPanel_MouseMove);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(14, 12);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(29, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "LMB";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(52, 12);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(31, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "RMB";
            // 
            // TileListFLP
            // 
            this.TileListFLP.Location = new System.Drawing.Point(13, 119);
            this.TileListFLP.Name = "TileListFLP";
            this.TileListFLP.Size = new System.Drawing.Size(357, 537);
            this.TileListFLP.TabIndex = 4;
            // 
            // LmbPB
            // 
            this.LmbPB.Location = new System.Drawing.Point(13, 29);
            this.LmbPB.Name = "LmbPB";
            this.LmbPB.Size = new System.Drawing.Size(32, 32);
            this.LmbPB.TabIndex = 5;
            this.LmbPB.TabStop = false;
            // 
            // RmbPB
            // 
            this.RmbPB.Location = new System.Drawing.Point(51, 29);
            this.RmbPB.Name = "RmbPB";
            this.RmbPB.Size = new System.Drawing.Size(32, 32);
            this.RmbPB.TabIndex = 6;
            this.RmbPB.TabStop = false;
            // 
            // NewSaveButton
            // 
            this.NewSaveButton.Location = new System.Drawing.Point(239, 16);
            this.NewSaveButton.Name = "NewSaveButton";
            this.NewSaveButton.Size = new System.Drawing.Size(131, 23);
            this.NewSaveButton.TabIndex = 7;
            this.NewSaveButton.Text = "Save as New Level";
            this.NewSaveButton.UseVisualStyleBackColor = true;
            this.NewSaveButton.Click += new System.EventHandler(this.SaveButton_Click);
            // 
            // LoadFileButton
            // 
            this.LoadFileButton.Location = new System.Drawing.Point(239, 74);
            this.LoadFileButton.Name = "LoadFileButton";
            this.LoadFileButton.Size = new System.Drawing.Size(131, 23);
            this.LoadFileButton.TabIndex = 8;
            this.LoadFileButton.Text = "Load";
            this.LoadFileButton.UseVisualStyleBackColor = true;
            this.LoadFileButton.Click += new System.EventHandler(this.LoadFileButton_Click);
            // 
            // ObjectListFLP
            // 
            this.ObjectListFLP.Location = new System.Drawing.Point(13, 119);
            this.ObjectListFLP.Name = "ObjectListFLP";
            this.ObjectListFLP.Size = new System.Drawing.Size(282, 537);
            this.ObjectListFLP.TabIndex = 5;
            // 
            // MobileListFLP
            // 
            this.MobileListFLP.Location = new System.Drawing.Point(13, 119);
            this.MobileListFLP.Name = "MobileListFLP";
            this.MobileListFLP.Size = new System.Drawing.Size(357, 537);
            this.MobileListFLP.TabIndex = 6;
            // 
            // OverwriteSaveButton
            // 
            this.OverwriteSaveButton.Enabled = false;
            this.OverwriteSaveButton.Location = new System.Drawing.Point(239, 45);
            this.OverwriteSaveButton.Name = "OverwriteSaveButton";
            this.OverwriteSaveButton.Size = new System.Drawing.Size(131, 23);
            this.OverwriteSaveButton.TabIndex = 12;
            this.OverwriteSaveButton.Text = "Overwrite Existing Save";
            this.OverwriteSaveButton.UseVisualStyleBackColor = true;
            this.OverwriteSaveButton.Click += new System.EventHandler(this.OverwriteSaveButton_Click);
            // 
            // LayerSelectCB
            // 
            this.LayerSelectCB.FormattingEnabled = true;
            this.LayerSelectCB.Items.AddRange(new object[] {
            "Tile Layer",
            "Prop Layer",
            "Mobile Layer",
            "ZoneTrigger Layer",
            "Interactible Layer"});
            this.LayerSelectCB.Location = new System.Drawing.Point(101, 16);
            this.LayerSelectCB.Name = "LayerSelectCB";
            this.LayerSelectCB.Size = new System.Drawing.Size(121, 21);
            this.LayerSelectCB.TabIndex = 15;
            // 
            // zoneTriggerPanel
            // 
            this.zoneTriggerPanel.Location = new System.Drawing.Point(15, 121);
            this.zoneTriggerPanel.Name = "zoneTriggerPanel";
            this.zoneTriggerPanel.Size = new System.Drawing.Size(221, 212);
            this.zoneTriggerPanel.TabIndex = 14;
            // 
            // InteractibleListFLP
            // 
            this.InteractibleListFLP.Location = new System.Drawing.Point(13, 119);
            this.InteractibleListFLP.Name = "InteractibleListFLP";
            this.InteractibleListFLP.Size = new System.Drawing.Size(357, 230);
            this.InteractibleListFLP.TabIndex = 7;
            // 
            // InteractiblePanel
            // 
            this.InteractiblePanel.Location = new System.Drawing.Point(15, 370);
            this.InteractiblePanel.Name = "InteractiblePanel";
            this.InteractiblePanel.Size = new System.Drawing.Size(232, 248);
            this.InteractiblePanel.TabIndex = 16;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1055, 673);
            this.Controls.Add(this.InteractiblePanel);
            this.Controls.Add(this.InteractibleListFLP);
            this.Controls.Add(this.LayerSelectCB);
            this.Controls.Add(this.zoneTriggerPanel);
            this.Controls.Add(this.OverwriteSaveButton);
            this.Controls.Add(this.MobileListFLP);
            this.Controls.Add(this.ObjectListFLP);
            this.Controls.Add(this.LoadFileButton);
            this.Controls.Add(this.NewSaveButton);
            this.Controls.Add(this.RmbPB);
            this.Controls.Add(this.LmbPB);
            this.Controls.Add(this.TileListFLP);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.LevelEditorPanel);
            this.DoubleBuffered = true;
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "Form1";
            this.Text = "Level Editor 0.1";
            ((System.ComponentModel.ISupportInitialize)(this.LmbPB)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.RmbPB)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel LevelEditorPanel;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.FlowLayoutPanel TileListFLP;
        private System.Windows.Forms.PictureBox LmbPB;
        private System.Windows.Forms.PictureBox RmbPB;
        private System.Windows.Forms.Button NewSaveButton;
        private System.Windows.Forms.Button LoadFileButton;
        private System.Windows.Forms.FlowLayoutPanel ObjectListFLP;
        private System.Windows.Forms.FlowLayoutPanel MobileListFLP;
        private System.Windows.Forms.Button OverwriteSaveButton;
        private ZoneTriggerPanel zoneTriggerPanel;
        private System.Windows.Forms.ComboBox LayerSelectCB;
        private System.Windows.Forms.FlowLayoutPanel InteractibleListFLP;
        private InteractiblePanel InteractiblePanel;
    }
}

