namespace Z64.Forms
{
    partial class DisasmForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DisasmForm));
            this.textBox_bytes = new System.Windows.Forms.TextBox();
            this.label_disas = new System.Windows.Forms.Label();
            this.textBox_disassembly = new System.Windows.Forms.TextBox();
            this.label_bytes = new System.Windows.Forms.Label();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolStripButton1 = new System.Windows.Forms.ToolStripButton();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // textBox_bytes
            // 
            this.textBox_bytes.Location = new System.Drawing.Point(12, 41);
            this.textBox_bytes.Multiline = true;
            this.textBox_bytes.Name = "textBox_bytes";
            this.textBox_bytes.Size = new System.Drawing.Size(898, 186);
            this.textBox_bytes.TabIndex = 0;
            this.textBox_bytes.TextChanged += new System.EventHandler(this.textBox1_TextChanged);
            // 
            // label_disas
            // 
            this.label_disas.AutoSize = true;
            this.label_disas.Location = new System.Drawing.Point(9, 233);
            this.label_disas.Name = "label_disas";
            this.label_disas.Size = new System.Drawing.Size(65, 13);
            this.label_disas.TabIndex = 9;
            this.label_disas.Text = "Disassembly";
            // 
            // textBox_disassembly
            // 
            this.textBox_disassembly.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox_disassembly.Font = new System.Drawing.Font("Consolas", 8.25F);
            this.textBox_disassembly.Location = new System.Drawing.Point(12, 249);
            this.textBox_disassembly.Multiline = true;
            this.textBox_disassembly.Name = "textBox_disassembly";
            this.textBox_disassembly.ReadOnly = true;
            this.textBox_disassembly.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.textBox_disassembly.Size = new System.Drawing.Size(898, 280);
            this.textBox_disassembly.TabIndex = 8;
            this.textBox_disassembly.WordWrap = false;
            // 
            // label_bytes
            // 
            this.label_bytes.AutoSize = true;
            this.label_bytes.ForeColor = System.Drawing.Color.Green;
            this.label_bytes.Location = new System.Drawing.Point(12, 25);
            this.label_bytes.Name = "label_bytes";
            this.label_bytes.Size = new System.Drawing.Size(26, 13);
            this.label_bytes.TabIndex = 11;
            this.label_bytes.Text = "Hex";
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButton1});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(922, 25);
            this.toolStrip1.TabIndex = 12;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // toolStripButton1
            // 
            this.toolStripButton1.Image = global::Z64.Properties.Resources.tools;
            this.toolStripButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton1.Name = "toolStripButton1";
            this.toolStripButton1.Size = new System.Drawing.Size(137, 22);
            this.toolStripButton1.Text = "Disassembly Settings";
            this.toolStripButton1.Click += new System.EventHandler(this.toolStripButton1_Click);
            // 
            // DisasmForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(922, 541);
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.label_bytes);
            this.Controls.Add(this.label_disas);
            this.Controls.Add(this.textBox_disassembly);
            this.Controls.Add(this.textBox_bytes);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "DisasmForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Disassembly";
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textBox_bytes;
        private System.Windows.Forms.Label label_disas;
        private System.Windows.Forms.TextBox textBox_disassembly;
        private System.Windows.Forms.Label label_bytes;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton toolStripButton1;
    }
}