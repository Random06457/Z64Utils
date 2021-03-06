namespace Z64.Forms
{
    partial class RenderSettingsForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RenderSettingsForm));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label3 = new System.Windows.Forms.Label();
            this.value_gridScale = new System.Windows.Forms.NumericUpDown();
            this.checkBox_showAxis = new System.Windows.Forms.CheckBox();
            this.checkBox_showGrid = new System.Windows.Forms.CheckBox();
            this.checkBox_renderTextures = new System.Windows.Forms.CheckBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.value_gridScale)).BeginInit();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.value_gridScale);
            this.groupBox1.Controls.Add(this.checkBox_showAxis);
            this.groupBox1.Controls.Add(this.checkBox_showGrid);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(174, 85);
            this.groupBox1.TabIndex = 6;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Render Control Settings";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 56);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(56, 13);
            this.label3.TabIndex = 3;
            this.label3.Text = "Grid Scale";
            // 
            // value_gridScale
            // 
            this.value_gridScale.Location = new System.Drawing.Point(65, 53);
            this.value_gridScale.Maximum = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
            this.value_gridScale.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.value_gridScale.Name = "value_gridScale";
            this.value_gridScale.Size = new System.Drawing.Size(103, 20);
            this.value_gridScale.TabIndex = 2;
            this.value_gridScale.Value = new decimal(new int[] {
            5000,
            0,
            0,
            0});
            this.value_gridScale.ValueChanged += new System.EventHandler(this.UpdateSettings);
            // 
            // checkBox_showAxis
            // 
            this.checkBox_showAxis.AutoSize = true;
            this.checkBox_showAxis.Checked = true;
            this.checkBox_showAxis.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox_showAxis.Location = new System.Drawing.Point(9, 17);
            this.checkBox_showAxis.Name = "checkBox_showAxis";
            this.checkBox_showAxis.Size = new System.Drawing.Size(75, 17);
            this.checkBox_showAxis.TabIndex = 1;
            this.checkBox_showAxis.Text = "Show Axis";
            this.checkBox_showAxis.UseVisualStyleBackColor = true;
            this.checkBox_showAxis.CheckedChanged += new System.EventHandler(this.UpdateSettings);
            // 
            // checkBox_showGrid
            // 
            this.checkBox_showGrid.AutoSize = true;
            this.checkBox_showGrid.Checked = true;
            this.checkBox_showGrid.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox_showGrid.Location = new System.Drawing.Point(9, 35);
            this.checkBox_showGrid.Name = "checkBox_showGrid";
            this.checkBox_showGrid.Size = new System.Drawing.Size(75, 17);
            this.checkBox_showGrid.TabIndex = 0;
            this.checkBox_showGrid.Text = "Show Grid";
            this.checkBox_showGrid.UseVisualStyleBackColor = true;
            this.checkBox_showGrid.CheckedChanged += new System.EventHandler(this.UpdateSettings);
            // 
            // checkBox_renderTextures
            // 
            this.checkBox_renderTextures.AutoSize = true;
            this.checkBox_renderTextures.Checked = true;
            this.checkBox_renderTextures.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox_renderTextures.Location = new System.Drawing.Point(23, 30);
            this.checkBox_renderTextures.Name = "checkBox_renderTextures";
            this.checkBox_renderTextures.Size = new System.Drawing.Size(105, 17);
            this.checkBox_renderTextures.TabIndex = 4;
            this.checkBox_renderTextures.Text = "Render Textures";
            this.checkBox_renderTextures.UseVisualStyleBackColor = true;
            this.checkBox_renderTextures.CheckedChanged += new System.EventHandler(this.UpdateSettings);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.checkBox_renderTextures);
            this.groupBox2.Location = new System.Drawing.Point(12, 103);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(174, 61);
            this.groupBox2.TabIndex = 7;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Renderer Settings";
            // 
            // RenderSettingsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(198, 174);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "RenderSettingsForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Render Settings";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.value_gridScale)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.CheckBox checkBox_renderTextures;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.NumericUpDown value_gridScale;
        private System.Windows.Forms.CheckBox checkBox_showAxis;
        private System.Windows.Forms.CheckBox checkBox_showGrid;
        private System.Windows.Forms.GroupBox groupBox2;
    }
}