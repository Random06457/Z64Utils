﻿namespace Z64.Forms
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
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // indexLabel
            // 
            this.indexLabel.AutoSize = true;
            this.indexLabel.Location = new System.Drawing.Point(0, 6);
            this.indexLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.indexLabel.Name = "indexLabel";
            this.indexLabel.Size = new System.Drawing.Size(25, 15);
            this.indexLabel.TabIndex = 1;
            this.indexLabel.Text = "00 :";
            // 
            // importFileButton
            // 
            this.importFileButton.Location = new System.Drawing.Point(35, 1);
            this.importFileButton.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.importFileButton.Name = "importFileButton";
            this.importFileButton.Size = new System.Drawing.Size(75, 27);
            this.importFileButton.TabIndex = 2;
            this.importFileButton.Text = "Edit";
            this.importFileButton.UseVisualStyleBackColor = true;
            this.importFileButton.Click += new System.EventHandler(this.importFileButton_Click);
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.label1.Location = new System.Drawing.Point(112, 7);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(25, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "...";
            // 
            // SegmentControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.label1);
            this.Controls.Add(this.importFileButton);
            this.Controls.Add(this.indexLabel);
            this.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.MaximumSize = new System.Drawing.Size(327, 29);
            this.MinimumSize = new System.Drawing.Size(327, 29);
            this.Name = "SegmentControl";
            this.Size = new System.Drawing.Size(327, 29);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label indexLabel;
        private System.Windows.Forms.Button importFileButton;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.Label label1;
    }
}
