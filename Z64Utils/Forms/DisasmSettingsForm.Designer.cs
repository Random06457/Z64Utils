namespace Z64.Forms
{
    partial class DisasmSettingsForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DisasmSettingsForm));
            this.previewTextBox = new System.Windows.Forms.TextBox();
            this.checkBoxStatic = new System.Windows.Forms.CheckBox();
            this.checkBoxShowAddr = new System.Windows.Forms.CheckBox();
            this.checkBoxRelativeAddr = new System.Windows.Forms.CheckBox();
            this.checkBoxMultiCmdMacro = new System.Windows.Forms.CheckBox();
            this.checkBoxAddrLiteral = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // previewTextBox
            // 
            this.previewTextBox.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.previewTextBox.Location = new System.Drawing.Point(12, 133);
            this.previewTextBox.Multiline = true;
            this.previewTextBox.Name = "previewTextBox";
            this.previewTextBox.ReadOnly = true;
            this.previewTextBox.Size = new System.Drawing.Size(290, 48);
            this.previewTextBox.TabIndex = 0;
            // 
            // checkBoxStatic
            // 
            this.checkBoxStatic.AutoSize = true;
            this.checkBoxStatic.Location = new System.Drawing.Point(27, 93);
            this.checkBoxStatic.Name = "checkBoxStatic";
            this.checkBoxStatic.Size = new System.Drawing.Size(108, 17);
            this.checkBoxStatic.TabIndex = 1;
            this.checkBoxStatic.Text = "Static Commands";
            this.checkBoxStatic.UseVisualStyleBackColor = true;
            // 
            // checkBoxShowAddr
            // 
            this.checkBoxShowAddr.AutoSize = true;
            this.checkBoxShowAddr.Location = new System.Drawing.Point(27, 17);
            this.checkBoxShowAddr.Name = "checkBoxShowAddr";
            this.checkBoxShowAddr.Size = new System.Drawing.Size(94, 17);
            this.checkBoxShowAddr.TabIndex = 2;
            this.checkBoxShowAddr.Text = "Show Address";
            this.checkBoxShowAddr.UseVisualStyleBackColor = true;
            // 
            // checkBoxRelativeAddr
            // 
            this.checkBoxRelativeAddr.AutoSize = true;
            this.checkBoxRelativeAddr.Location = new System.Drawing.Point(27, 36);
            this.checkBoxRelativeAddr.Name = "checkBoxRelativeAddr";
            this.checkBoxRelativeAddr.Size = new System.Drawing.Size(106, 17);
            this.checkBoxRelativeAddr.TabIndex = 3;
            this.checkBoxRelativeAddr.Text = "Relative Address";
            this.checkBoxRelativeAddr.UseVisualStyleBackColor = true;
            // 
            // checkBoxMultiCmdMacro
            // 
            this.checkBoxMultiCmdMacro.AutoSize = true;
            this.checkBoxMultiCmdMacro.Location = new System.Drawing.Point(27, 55);
            this.checkBoxMultiCmdMacro.Name = "checkBoxMultiCmdMacro";
            this.checkBoxMultiCmdMacro.Size = new System.Drawing.Size(190, 17);
            this.checkBoxMultiCmdMacro.TabIndex = 4;
            this.checkBoxMultiCmdMacro.Text = "Recognize Multi Command Macros";
            this.checkBoxMultiCmdMacro.UseVisualStyleBackColor = true;
            // 
            // checkBoxAddrLiteral
            // 
            this.checkBoxAddrLiteral.AutoSize = true;
            this.checkBoxAddrLiteral.Location = new System.Drawing.Point(27, 74);
            this.checkBoxAddrLiteral.Name = "checkBoxAddrLiteral";
            this.checkBoxAddrLiteral.Size = new System.Drawing.Size(145, 17);
            this.checkBoxAddrLiteral.TabIndex = 5;
            this.checkBoxAddrLiteral.Text = "Show Address As Literals";
            this.checkBoxAddrLiteral.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(15, 118);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(48, 13);
            this.label1.TabIndex = 6;
            this.label1.Text = "Preview:";
            // 
            // DisasmSettingsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(314, 188);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.checkBoxAddrLiteral);
            this.Controls.Add(this.checkBoxMultiCmdMacro);
            this.Controls.Add(this.checkBoxRelativeAddr);
            this.Controls.Add(this.checkBoxShowAddr);
            this.Controls.Add(this.checkBoxStatic);
            this.Controls.Add(this.previewTextBox);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "DisasmSettingsForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Disassembly Settings";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.DisasmSettingsForm_FormClosed);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox previewTextBox;
        private System.Windows.Forms.CheckBox checkBoxStatic;
        private System.Windows.Forms.CheckBox checkBoxShowAddr;
        private System.Windows.Forms.CheckBox checkBoxRelativeAddr;
        private System.Windows.Forms.CheckBox checkBoxMultiCmdMacro;
        private System.Windows.Forms.CheckBox checkBoxAddrLiteral;
        private System.Windows.Forms.Label label1;
    }
}