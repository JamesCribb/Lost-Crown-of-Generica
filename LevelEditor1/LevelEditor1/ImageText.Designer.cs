namespace LevelEditor1
{
    partial class ImageText
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
            this.ImagePB = new System.Windows.Forms.PictureBox();
            this.ImageLabel = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.ImagePB)).BeginInit();
            this.SuspendLayout();
            // 
            // ImagePB
            // 
            this.ImagePB.Location = new System.Drawing.Point(4, 4);
            this.ImagePB.Name = "ImagePB";
            this.ImagePB.Size = new System.Drawing.Size(32, 32);
            this.ImagePB.TabIndex = 0;
            this.ImagePB.TabStop = false;
            // 
            // ImageLabel
            // 
            this.ImageLabel.AutoSize = true;
            this.ImageLabel.Location = new System.Drawing.Point(42, 14);
            this.ImageLabel.Name = "ImageLabel";
            this.ImageLabel.Size = new System.Drawing.Size(146, 13);
            this.ImageLabel.TabIndex = 1;
            this.ImageLabel.Text = "Image Description Goes Here";
            // 
            // ImageText
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.ImageLabel);
            this.Controls.Add(this.ImagePB);
            this.Name = "ImageText";
            this.Size = new System.Drawing.Size(198, 41);
            ((System.ComponentModel.ISupportInitialize)(this.ImagePB)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox ImagePB;
        private System.Windows.Forms.Label ImageLabel;
    }
}
