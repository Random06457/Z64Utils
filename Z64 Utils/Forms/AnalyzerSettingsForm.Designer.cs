namespace Z64.Forms
{
    partial class AnalyzerSettingsForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AnalyzerSettingsForm));
            this.textBoxOpCodeList = new System.Windows.Forms.TextBox();
            this.buttonNoRestriction = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.buttonNormal = new System.Windows.Forms.Button();
            this.buttonRestrivtive = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.textBoxPatterns = new System.Windows.Forms.TextBox();
            this.buttonOK = new System.Windows.Forms.Button();
            this.labelOpCodeListError = new System.Windows.Forms.Label();
            this.labelPatternError = new System.Windows.Forms.Label();
            this.linkLabel1 = new System.Windows.Forms.LinkLabel();
            this.SuspendLayout();
            // 
            // textBoxOpCodeList
            // 
            this.textBoxOpCodeList.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBoxOpCodeList.Location = new System.Drawing.Point(15, 87);
            this.textBoxOpCodeList.Multiline = true;
            this.textBoxOpCodeList.Name = "textBoxOpCodeList";
            this.textBoxOpCodeList.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.textBoxOpCodeList.Size = new System.Drawing.Size(458, 107);
            this.textBoxOpCodeList.TabIndex = 0;
            this.textBoxOpCodeList.WordWrap = false;
            this.textBoxOpCodeList.TextChanged += new System.EventHandler(this.textBoxOpCodeList_TextChanged);
            // 
            // buttonNoRestriction
            // 
            this.buttonNoRestriction.Location = new System.Drawing.Point(111, 36);
            this.buttonNoRestriction.Name = "buttonNoRestriction";
            this.buttonNoRestriction.Size = new System.Drawing.Size(85, 23);
            this.buttonNoRestriction.TabIndex = 1;
            this.buttonNoRestriction.Text = "No restriction";
            this.buttonNoRestriction.UseVisualStyleBackColor = true;
            this.buttonNoRestriction.Click += new System.EventHandler(this.buttonNoRestriction_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 71);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(309, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Improbable OpCodes (The analyze will consider them as invalid):";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(202, 17);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(85, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Default Settings:";
            // 
            // buttonNormal
            // 
            this.buttonNormal.Location = new System.Drawing.Point(202, 36);
            this.buttonNormal.Name = "buttonNormal";
            this.buttonNormal.Size = new System.Drawing.Size(85, 23);
            this.buttonNormal.TabIndex = 4;
            this.buttonNormal.Text = "Normal";
            this.buttonNormal.UseVisualStyleBackColor = true;
            this.buttonNormal.Click += new System.EventHandler(this.buttonNormal_Click);
            // 
            // buttonRestrivtive
            // 
            this.buttonRestrivtive.Location = new System.Drawing.Point(293, 36);
            this.buttonRestrivtive.Name = "buttonRestrivtive";
            this.buttonRestrivtive.Size = new System.Drawing.Size(85, 23);
            this.buttonRestrivtive.TabIndex = 5;
            this.buttonRestrivtive.Text = "Restrictive";
            this.buttonRestrivtive.UseVisualStyleBackColor = true;
            this.buttonRestrivtive.Click += new System.EventHandler(this.buttonRestrivtive_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(14, 231);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(444, 13);
            this.label3.TabIndex = 7;
            this.label3.Text = "Patterns (The analyzer will consider the OpCodes that don\'t follow certain patter" +
    "ns as invalid):";
            // 
            // textBoxPatterns
            // 
            this.textBoxPatterns.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBoxPatterns.Location = new System.Drawing.Point(15, 247);
            this.textBoxPatterns.Multiline = true;
            this.textBoxPatterns.Name = "textBoxPatterns";
            this.textBoxPatterns.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.textBoxPatterns.Size = new System.Drawing.Size(458, 189);
            this.textBoxPatterns.TabIndex = 6;
            this.textBoxPatterns.WordWrap = false;
            this.textBoxPatterns.TextChanged += new System.EventHandler(this.textBoxPatterns_TextChanged);
            // 
            // buttonOK
            // 
            this.buttonOK.Location = new System.Drawing.Point(207, 457);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new System.Drawing.Size(75, 23);
            this.buttonOK.TabIndex = 8;
            this.buttonOK.Text = "OK";
            this.buttonOK.UseVisualStyleBackColor = true;
            this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
            // 
            // labelOpCodeListError
            // 
            this.labelOpCodeListError.AutoSize = true;
            this.labelOpCodeListError.ForeColor = System.Drawing.Color.Red;
            this.labelOpCodeListError.Location = new System.Drawing.Point(14, 197);
            this.labelOpCodeListError.Name = "labelOpCodeListError";
            this.labelOpCodeListError.Size = new System.Drawing.Size(16, 13);
            this.labelOpCodeListError.TabIndex = 9;
            this.labelOpCodeListError.Text = "...";
            // 
            // labelPatternError
            // 
            this.labelPatternError.AutoSize = true;
            this.labelPatternError.ForeColor = System.Drawing.Color.Red;
            this.labelPatternError.Location = new System.Drawing.Point(15, 438);
            this.labelPatternError.Name = "labelPatternError";
            this.labelPatternError.Size = new System.Drawing.Size(16, 13);
            this.labelPatternError.TabIndex = 10;
            this.labelPatternError.Text = "...";
            // 
            // linkLabel1
            // 
            this.linkLabel1.AutoSize = true;
            this.linkLabel1.Location = new System.Drawing.Point(446, 439);
            this.linkLabel1.Name = "linkLabel1";
            this.linkLabel1.Size = new System.Drawing.Size(27, 13);
            this.linkLabel1.TabIndex = 11;
            this.linkLabel1.TabStop = true;
            this.linkLabel1.Text = "help";
            this.linkLabel1.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel1_LinkClicked);
            // 
            // AnalyzerSettingsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(489, 486);
            this.Controls.Add(this.linkLabel1);
            this.Controls.Add(this.labelPatternError);
            this.Controls.Add(this.labelOpCodeListError);
            this.Controls.Add(this.buttonOK);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.textBoxPatterns);
            this.Controls.Add(this.buttonRestrivtive);
            this.Controls.Add(this.buttonNormal);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.buttonNoRestriction);
            this.Controls.Add(this.textBoxOpCodeList);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AnalyzerSettingsForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Analyzer Settings";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textBoxOpCodeList;
        private System.Windows.Forms.Button buttonNoRestriction;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button buttonNormal;
        private System.Windows.Forms.Button buttonRestrivtive;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox textBoxPatterns;
        private System.Windows.Forms.Button buttonOK;
        private System.Windows.Forms.Label labelOpCodeListError;
        private System.Windows.Forms.Label labelPatternError;
        private System.Windows.Forms.LinkLabel linkLabel1;
    }
}