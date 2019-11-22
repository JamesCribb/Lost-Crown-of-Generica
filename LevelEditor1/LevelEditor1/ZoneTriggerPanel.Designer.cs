namespace LevelEditor1
{
    partial class ZoneTriggerPanel
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.ZoneLabel = new System.Windows.Forms.Label();
            this.ZoneTB = new System.Windows.Forms.TextBox();
            this.LevelXLabel = new System.Windows.Forms.Label();
            this.LevelXNUD = new System.Windows.Forms.NumericUpDown();
            this.LevelYNUD = new System.Windows.Forms.NumericUpDown();
            this.LevelYLabel = new System.Windows.Forms.Label();
            this.PlayerXNUD = new System.Windows.Forms.NumericUpDown();
            this.PlayerXLabel = new System.Windows.Forms.Label();
            this.PlayerYNUD = new System.Windows.Forms.NumericUpDown();
            this.PlayerYLabel = new System.Windows.Forms.Label();
            this.SaveButton = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.LevelXNUD)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.LevelYNUD)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.PlayerXNUD)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.PlayerYNUD)).BeginInit();
            this.SuspendLayout();
            // 
            // ZoneLabel
            // 
            this.ZoneLabel.AutoSize = true;
            this.ZoneLabel.Location = new System.Drawing.Point(16, 22);
            this.ZoneLabel.Name = "ZoneLabel";
            this.ZoneLabel.Size = new System.Drawing.Size(32, 13);
            this.ZoneLabel.TabIndex = 0;
            this.ZoneLabel.Text = "Zone";
            // 
            // ZoneTB
            // 
            this.ZoneTB.Location = new System.Drawing.Point(75, 19);
            this.ZoneTB.Name = "ZoneTB";
            this.ZoneTB.Size = new System.Drawing.Size(120, 20);
            this.ZoneTB.TabIndex = 1;
            // 
            // LevelXLabel
            // 
            this.LevelXLabel.AutoSize = true;
            this.LevelXLabel.Location = new System.Drawing.Point(16, 47);
            this.LevelXLabel.Name = "LevelXLabel";
            this.LevelXLabel.Size = new System.Drawing.Size(43, 13);
            this.LevelXLabel.TabIndex = 2;
            this.LevelXLabel.Text = "Level X";
            // 
            // LevelXNUD
            // 
            this.LevelXNUD.Location = new System.Drawing.Point(75, 45);
            this.LevelXNUD.Maximum = new decimal(new int[] {
            640,
            0,
            0,
            0});
            this.LevelXNUD.Name = "LevelXNUD";
            this.LevelXNUD.Size = new System.Drawing.Size(120, 20);
            this.LevelXNUD.TabIndex = 3;
            // 
            // LevelYNUD
            // 
            this.LevelYNUD.Location = new System.Drawing.Point(75, 71);
            this.LevelYNUD.Maximum = new decimal(new int[] {
            640,
            0,
            0,
            0});
            this.LevelYNUD.Name = "LevelYNUD";
            this.LevelYNUD.Size = new System.Drawing.Size(120, 20);
            this.LevelYNUD.TabIndex = 5;
            // 
            // LevelYLabel
            // 
            this.LevelYLabel.AutoSize = true;
            this.LevelYLabel.Location = new System.Drawing.Point(16, 73);
            this.LevelYLabel.Name = "LevelYLabel";
            this.LevelYLabel.Size = new System.Drawing.Size(43, 13);
            this.LevelYLabel.TabIndex = 4;
            this.LevelYLabel.Text = "Level Y";
            // 
            // PlayerXNUD
            // 
            this.PlayerXNUD.Location = new System.Drawing.Point(75, 97);
            this.PlayerXNUD.Maximum = new decimal(new int[] {
            640,
            0,
            0,
            0});
            this.PlayerXNUD.Name = "PlayerXNUD";
            this.PlayerXNUD.Size = new System.Drawing.Size(120, 20);
            this.PlayerXNUD.TabIndex = 7;
            // 
            // PlayerXLabel
            // 
            this.PlayerXLabel.AutoSize = true;
            this.PlayerXLabel.Location = new System.Drawing.Point(16, 99);
            this.PlayerXLabel.Name = "PlayerXLabel";
            this.PlayerXLabel.Size = new System.Drawing.Size(46, 13);
            this.PlayerXLabel.TabIndex = 6;
            this.PlayerXLabel.Text = "Player X";
            // 
            // PlayerYNUD
            // 
            this.PlayerYNUD.Location = new System.Drawing.Point(75, 123);
            this.PlayerYNUD.Maximum = new decimal(new int[] {
            640,
            0,
            0,
            0});
            this.PlayerYNUD.Name = "PlayerYNUD";
            this.PlayerYNUD.Size = new System.Drawing.Size(120, 20);
            this.PlayerYNUD.TabIndex = 9;
            // 
            // PlayerYLabel
            // 
            this.PlayerYLabel.AutoSize = true;
            this.PlayerYLabel.Location = new System.Drawing.Point(16, 125);
            this.PlayerYLabel.Name = "PlayerYLabel";
            this.PlayerYLabel.Size = new System.Drawing.Size(46, 13);
            this.PlayerYLabel.TabIndex = 8;
            this.PlayerYLabel.Text = "Player Y";
            // 
            // SaveButton
            // 
            this.SaveButton.Location = new System.Drawing.Point(19, 172);
            this.SaveButton.Name = "SaveButton";
            this.SaveButton.Size = new System.Drawing.Size(75, 23);
            this.SaveButton.TabIndex = 10;
            this.SaveButton.Text = "Save";
            this.SaveButton.UseVisualStyleBackColor = true;
            // 
            // ZoneTriggerPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.SaveButton);
            this.Controls.Add(this.PlayerYNUD);
            this.Controls.Add(this.PlayerYLabel);
            this.Controls.Add(this.PlayerXNUD);
            this.Controls.Add(this.PlayerXLabel);
            this.Controls.Add(this.LevelYNUD);
            this.Controls.Add(this.LevelYLabel);
            this.Controls.Add(this.LevelXNUD);
            this.Controls.Add(this.LevelXLabel);
            this.Controls.Add(this.ZoneTB);
            this.Controls.Add(this.ZoneLabel);
            this.Name = "ZoneTriggerPanel";
            this.Size = new System.Drawing.Size(221, 212);
            ((System.ComponentModel.ISupportInitialize)(this.LevelXNUD)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.LevelYNUD)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.PlayerXNUD)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.PlayerYNUD)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label ZoneLabel;
        private System.Windows.Forms.TextBox ZoneTB;
        private System.Windows.Forms.Label LevelXLabel;
        private System.Windows.Forms.NumericUpDown LevelXNUD;
        private System.Windows.Forms.NumericUpDown LevelYNUD;
        private System.Windows.Forms.Label LevelYLabel;
        private System.Windows.Forms.NumericUpDown PlayerXNUD;
        private System.Windows.Forms.Label PlayerXLabel;
        private System.Windows.Forms.NumericUpDown PlayerYNUD;
        private System.Windows.Forms.Label PlayerYLabel;
        private System.Windows.Forms.Button SaveButton;
    }
}
