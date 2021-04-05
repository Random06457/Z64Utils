namespace Z64.Forms
{
    partial class SegmentEditForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SegmentEditForm));
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage_address = new System.Windows.Forms.TabPage();
            this.addressValue = new System.Windows.Forms.TextBox();
            this.tabPage_file = new System.Windows.Forms.TabPage();
            this.button1 = new System.Windows.Forms.Button();
            this.tabPage_empty = new System.Windows.Forms.TabPage();
            this.label1 = new System.Windows.Forms.Label();
            this.okBtn = new System.Windows.Forms.Button();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.tabControl1.SuspendLayout();
            this.tabPage_address.SuspendLayout();
            this.tabPage_file.SuspendLayout();
            this.SuspendLayout();
            // 
            // comboBox1
            // 
            this.comboBox1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Items.AddRange(new object[] {
            "Empty",
            "Address",
            "ROM FS",
            "File",
            "Ident Matrices",
            "Null Bytes",
            "Empty Dlist"});
            this.comboBox1.Location = new System.Drawing.Point(21, 22);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(108, 21);
            this.comboBox1.TabIndex = 0;
            this.comboBox1.SelectedIndexChanged += new System.EventHandler(this.comboBox1_SelectedIndexChanged);
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage_address);
            this.tabControl1.Controls.Add(this.tabPage_file);
            this.tabControl1.Controls.Add(this.tabPage_empty);
            this.tabControl1.Location = new System.Drawing.Point(26, 45);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(99, 53);
            this.tabControl1.TabIndex = 1;
            // 
            // tabPage_address
            // 
            this.tabPage_address.BackColor = System.Drawing.SystemColors.Control;
            this.tabPage_address.Controls.Add(this.addressValue);
            this.tabPage_address.Location = new System.Drawing.Point(4, 22);
            this.tabPage_address.Name = "tabPage_address";
            this.tabPage_address.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage_address.Size = new System.Drawing.Size(91, 27);
            this.tabPage_address.TabIndex = 0;
            this.tabPage_address.Text = "address";
            // 
            // addressValue
            // 
            this.addressValue.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.addressValue.ForeColor = System.Drawing.SystemColors.ControlText;
            this.addressValue.Location = new System.Drawing.Point(5, 3);
            this.addressValue.Name = "addressValue";
            this.addressValue.Size = new System.Drawing.Size(80, 20);
            this.addressValue.TabIndex = 5;
            this.addressValue.Text = "00000000";
            this.addressValue.TextChanged += new System.EventHandler(this.addressValue_TextChanged);
            // 
            // tabPage_file
            // 
            this.tabPage_file.BackColor = System.Drawing.SystemColors.Control;
            this.tabPage_file.Controls.Add(this.button1);
            this.tabPage_file.Location = new System.Drawing.Point(4, 22);
            this.tabPage_file.Name = "tabPage_file";
            this.tabPage_file.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage_file.Size = new System.Drawing.Size(91, 27);
            this.tabPage_file.TabIndex = 1;
            this.tabPage_file.Text = "file";
            // 
            // button1
            // 
            this.button1.ForeColor = System.Drawing.Color.Black;
            this.button1.Location = new System.Drawing.Point(8, 2);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 0;
            this.button1.Text = "Select File";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // tabPage_empty
            // 
            this.tabPage_empty.Location = new System.Drawing.Point(4, 22);
            this.tabPage_empty.Name = "tabPage_empty";
            this.tabPage_empty.Size = new System.Drawing.Size(91, 27);
            this.tabPage_empty.TabIndex = 2;
            this.tabPage_empty.Text = "Empty";
            this.tabPage_empty.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(53, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(44, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Source:";
            // 
            // okBtn
            // 
            this.okBtn.Enabled = false;
            this.okBtn.Location = new System.Drawing.Point(38, 98);
            this.okBtn.Name = "okBtn";
            this.okBtn.Size = new System.Drawing.Size(75, 23);
            this.okBtn.TabIndex = 3;
            this.okBtn.Text = "Ok";
            this.okBtn.UseVisualStyleBackColor = true;
            this.okBtn.Click += new System.EventHandler(this.okBtn_Click);
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // SegmentEditForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(150, 123);
            this.Controls.Add(this.okBtn);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.comboBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SegmentEditForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Edit Segment";
            this.tabControl1.ResumeLayout(false);
            this.tabPage_address.ResumeLayout(false);
            this.tabPage_address.PerformLayout();
            this.tabPage_file.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage_address;
        private System.Windows.Forms.TabPage tabPage_file;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox addressValue;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button okBtn;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.TabPage tabPage_empty;
    }
}