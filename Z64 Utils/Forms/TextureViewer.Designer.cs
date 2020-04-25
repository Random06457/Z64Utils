namespace Z64.Forms
{
    partial class TextureViewer
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TextureViewer));
            this.comboBoxAddressType = new System.Windows.Forms.ComboBox();
            this.comboBoxTexFmt = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.valueW = new System.Windows.Forms.NumericUpDown();
            this.valueH = new System.Windows.Forms.NumericUpDown();
            this.textBoxTexAddr = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.textBoxTlutAddr = new System.Windows.Forms.TextBox();
            this.textureBox1 = new Z64.Forms.TextureBox();
            ((System.ComponentModel.ISupportInitialize)(this.valueW)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.valueH)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.textureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // comboBoxAddressType
            // 
            this.comboBoxAddressType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxAddressType.FormattingEnabled = true;
            this.comboBoxAddressType.Items.AddRange(new object[] {
            "VRAM",
            "VROM"});
            this.comboBoxAddressType.Location = new System.Drawing.Point(17, 27);
            this.comboBoxAddressType.Name = "comboBoxAddressType";
            this.comboBoxAddressType.Size = new System.Drawing.Size(59, 21);
            this.comboBoxAddressType.TabIndex = 1;
            this.comboBoxAddressType.SelectedIndexChanged += new System.EventHandler(this.UpdateTexture);
            // 
            // comboBoxTexFmt
            // 
            this.comboBoxTexFmt.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxTexFmt.FormattingEnabled = true;
            this.comboBoxTexFmt.Items.AddRange(new object[] {
            "VRAM",
            "VROM"});
            this.comboBoxTexFmt.Location = new System.Drawing.Point(315, 26);
            this.comboBoxTexFmt.Name = "comboBoxTexFmt";
            this.comboBoxTexFmt.Size = new System.Drawing.Size(121, 21);
            this.comboBoxTexFmt.TabIndex = 2;
            this.comboBoxTexFmt.SelectedIndexChanged += new System.EventHandler(this.UpdateTexture);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(354, 10);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(39, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "Format";
            // 
            // valueW
            // 
            this.valueW.Location = new System.Drawing.Point(446, 26);
            this.valueW.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.valueW.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.valueW.Name = "valueW";
            this.valueW.Size = new System.Drawing.Size(92, 20);
            this.valueW.TabIndex = 5;
            this.valueW.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.valueW.ValueChanged += new System.EventHandler(this.UpdateTexture);
            // 
            // valueH
            // 
            this.valueH.Location = new System.Drawing.Point(555, 26);
            this.valueH.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.valueH.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.valueH.Name = "valueH";
            this.valueH.Size = new System.Drawing.Size(92, 20);
            this.valueH.TabIndex = 6;
            this.valueH.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.valueH.ValueChanged += new System.EventHandler(this.UpdateTexture);
            // 
            // textBoxTexAddr
            // 
            this.textBoxTexAddr.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBoxTexAddr.Location = new System.Drawing.Point(82, 27);
            this.textBoxTexAddr.Name = "textBoxTexAddr";
            this.textBoxTexAddr.Size = new System.Drawing.Size(81, 20);
            this.textBoxTexAddr.TabIndex = 0;
            this.textBoxTexAddr.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textBox_KeyDown);
            this.textBoxTexAddr.Validated += new System.EventHandler(this.UpdateTexture);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(540, 30);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(12, 13);
            this.label2.TabIndex = 7;
            this.label2.Text = "x";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(530, 10);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(27, 13);
            this.label3.TabIndex = 8;
            this.label3.Text = "Size";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(79, 10);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(84, 13);
            this.label4.TabIndex = 9;
            this.label4.Text = "Texture Address";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(169, 10);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(76, 13);
            this.label5.TabIndex = 11;
            this.label5.Text = "TLUT Address";
            // 
            // textBoxTlutAddr
            // 
            this.textBoxTlutAddr.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBoxTlutAddr.Location = new System.Drawing.Point(172, 26);
            this.textBoxTlutAddr.Name = "textBoxTlutAddr";
            this.textBoxTlutAddr.Size = new System.Drawing.Size(81, 20);
            this.textBoxTlutAddr.TabIndex = 10;
            this.textBoxTlutAddr.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textBox_KeyDown);
            this.textBoxTlutAddr.Validated += new System.EventHandler(this.UpdateTexture);
            // 
            // textureBox1
            // 
            this.textureBox1.AlphaTileSize = 10;
            this.textureBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textureBox1.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
            this.textureBox1.Location = new System.Drawing.Point(12, 54);
            this.textureBox1.Name = "textureBox1";
            this.textureBox1.Size = new System.Drawing.Size(793, 520);
            this.textureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.textureBox1.TabIndex = 3;
            this.textureBox1.TabStop = false;
            // 
            // TextureViewer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(817, 586);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.textBoxTlutAddr);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.valueH);
            this.Controls.Add(this.valueW);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.textureBox1);
            this.Controls.Add(this.comboBoxTexFmt);
            this.Controls.Add(this.comboBoxAddressType);
            this.Controls.Add(this.textBoxTexAddr);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "TextureViewer";
            this.Text = "Texture Viewer";
            ((System.ComponentModel.ISupportInitialize)(this.valueW)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.valueH)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.textureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.ComboBox comboBoxAddressType;
        private System.Windows.Forms.ComboBox comboBoxTexFmt;
        private TextureBox textureBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown valueW;
        private System.Windows.Forms.NumericUpDown valueH;
        private System.Windows.Forms.TextBox textBoxTexAddr;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox textBoxTlutAddr;
    }
}