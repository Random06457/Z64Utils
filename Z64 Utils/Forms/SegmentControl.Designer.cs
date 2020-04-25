namespace Z64.Forms
{
    partial class SegmentControl
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
            this.indexLabel = new System.Windows.Forms.Label();
            this.importFileButton = new System.Windows.Forms.Button();
            this.dmaFileButton = new System.Windows.Forms.Button();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.addressValue = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // indexLabel
            // 
            this.indexLabel.AutoSize = true;
            this.indexLabel.Location = new System.Drawing.Point(0, 5);
            this.indexLabel.Name = "indexLabel";
            this.indexLabel.Size = new System.Drawing.Size(25, 13);
            this.indexLabel.TabIndex = 1;
            this.indexLabel.Text = "00 :";
            // 
            // importFileButton
            // 
            this.importFileButton.Location = new System.Drawing.Point(149, 1);
            this.importFileButton.Name = "importFileButton";
            this.importFileButton.Size = new System.Drawing.Size(64, 23);
            this.importFileButton.TabIndex = 2;
            this.importFileButton.Text = "Import File";
            this.importFileButton.UseVisualStyleBackColor = true;
            this.importFileButton.Click += new System.EventHandler(this.importFileButton_Click);
            // 
            // dmaFileButton
            // 
            this.dmaFileButton.ForeColor = System.Drawing.Color.Black;
            this.dmaFileButton.Location = new System.Drawing.Point(216, 1);
            this.dmaFileButton.Name = "dmaFileButton";
            this.dmaFileButton.Size = new System.Drawing.Size(62, 23);
            this.dmaFileButton.TabIndex = 3;
            this.dmaFileButton.Text = "DMA File";
            this.dmaFileButton.UseVisualStyleBackColor = true;
            this.dmaFileButton.Click += new System.EventHandler(this.dmaFileButton_Click);
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // addressValue
            // 
            this.addressValue.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.addressValue.ForeColor = System.Drawing.Color.Green;
            this.addressValue.Location = new System.Drawing.Point(27, 2);
            this.addressValue.Name = "addressValue";
            this.addressValue.Size = new System.Drawing.Size(117, 20);
            this.addressValue.TabIndex = 4;
            this.addressValue.Text = "00000000";
            this.addressValue.Validating += new System.ComponentModel.CancelEventHandler(this.addressValue_Validating);
            this.addressValue.Validated += new System.EventHandler(this.addressValue_Validated);
            // 
            // SegmentControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.addressValue);
            this.Controls.Add(this.dmaFileButton);
            this.Controls.Add(this.importFileButton);
            this.Controls.Add(this.indexLabel);
            this.MaximumSize = new System.Drawing.Size(280, 25);
            this.MinimumSize = new System.Drawing.Size(280, 25);
            this.Name = "SegmentControl";
            this.Size = new System.Drawing.Size(280, 25);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label indexLabel;
        private System.Windows.Forms.Button importFileButton;
        private System.Windows.Forms.Button dmaFileButton;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.TextBox addressValue;
    }
}
