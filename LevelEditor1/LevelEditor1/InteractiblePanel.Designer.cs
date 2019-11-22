namespace LevelEditor1
{
    partial class InteractiblePanel
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
            this.SaveButton = new System.Windows.Forms.Button();
            this.InteractionTextLabel = new System.Windows.Forms.Label();
            this.InteractionTextRTB = new System.Windows.Forms.RichTextBox();
            this.FileNameLabel = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // SaveButton
            // 
            this.SaveButton.Location = new System.Drawing.Point(20, 231);
            this.SaveButton.Name = "SaveButton";
            this.SaveButton.Size = new System.Drawing.Size(75, 23);
            this.SaveButton.TabIndex = 21;
            this.SaveButton.Text = "Save";
            this.SaveButton.UseVisualStyleBackColor = true;
            // 
            // InteractionTextLabel
            // 
            this.InteractionTextLabel.AutoSize = true;
            this.InteractionTextLabel.Location = new System.Drawing.Point(17, 42);
            this.InteractionTextLabel.Name = "InteractionTextLabel";
            this.InteractionTextLabel.Size = new System.Drawing.Size(84, 13);
            this.InteractionTextLabel.TabIndex = 11;
            this.InteractionTextLabel.Text = "Interaction Text:";
            // 
            // InteractionTextRTB
            // 
            this.InteractionTextRTB.Location = new System.Drawing.Point(20, 67);
            this.InteractionTextRTB.Name = "InteractionTextRTB";
            this.InteractionTextRTB.Size = new System.Drawing.Size(194, 149);
            this.InteractionTextRTB.TabIndex = 22;
            this.InteractionTextRTB.Text = "";
            // 
            // FileNameLabel
            // 
            this.FileNameLabel.AutoSize = true;
            this.FileNameLabel.Location = new System.Drawing.Point(17, 14);
            this.FileNameLabel.Name = "FileNameLabel";
            this.FileNameLabel.Size = new System.Drawing.Size(51, 13);
            this.FileNameLabel.TabIndex = 23;
            this.FileNameLabel.Text = "FileName";
            // 
            // InteractiblePanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.FileNameLabel);
            this.Controls.Add(this.InteractionTextRTB);
            this.Controls.Add(this.SaveButton);
            this.Controls.Add(this.InteractionTextLabel);
            this.Name = "InteractiblePanel";
            this.Size = new System.Drawing.Size(232, 276);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button SaveButton;
        private System.Windows.Forms.Label InteractionTextLabel;
        private System.Windows.Forms.RichTextBox InteractionTextRTB;
        private System.Windows.Forms.Label FileNameLabel;
    }
}
