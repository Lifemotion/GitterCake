namespace gitter.Framework.Options
{
    partial class EssentialOptionsPage
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
			if(disposing && (components != null))
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
            this.panel1 = new System.Windows.Forms.Panel();
            this._levelStandard = new System.Windows.Forms.RadioButton();
            this._levelSimple = new System.Windows.Forms.RadioButton();
            this.groupSeparator1 = new gitter.Framework.Controls.GroupSeparator();
            this._pnlRestartRequiredWarning = new System.Windows.Forms.Panel();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.label1 = new System.Windows.Forms.Label();
            this._levelAdvanced = new System.Windows.Forms.RadioButton();
            this.panel1.SuspendLayout();
            this._pnlRestartRequiredWarning.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.Controls.Add(this._levelAdvanced);
            this.panel1.Controls.Add(this._levelStandard);
            this.panel1.Controls.Add(this._levelSimple);
            this.panel1.Location = new System.Drawing.Point(3, 20);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(388, 70);
            this.panel1.TabIndex = 0;
            // 
            // _levelStandard
            // 
            this._levelStandard.AutoSize = true;
            this._levelStandard.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this._levelStandard.Location = new System.Drawing.Point(3, 25);
            this._levelStandard.Name = "_levelStandard";
            this._levelStandard.Size = new System.Drawing.Size(78, 20);
            this._levelStandard.TabIndex = 1;
            this._levelStandard.Text = Properties.Resources.StrStandard;
            this._levelStandard.UseVisualStyleBackColor = true;
            // 
            // _levelSimple
            // 
            this._levelSimple.AutoSize = true;
            this._levelSimple.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this._levelSimple.Location = new System.Drawing.Point(3, 3);
            this._levelSimple.Name = "_levelSimple";
            this._levelSimple.Size = new System.Drawing.Size(67, 20);
            this._levelSimple.TabIndex = 0;
            this._levelSimple.Text = Properties.Resources.StrSimple;
            this._levelSimple.UseVisualStyleBackColor = true;
            // 
            // groupSeparator1
            // 
            this.groupSeparator1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupSeparator1.Location = new System.Drawing.Point(0, 0);
            this.groupSeparator1.Name = "groupSeparator1";
            this.groupSeparator1.Size = new System.Drawing.Size(388, 19);
            this.groupSeparator1.TabIndex = 1;
            this.groupSeparator1.Text = Properties.Resources.StrApplicationMode;
            // 
            // _pnlRestartRequiredWarning
            // 
            this._pnlRestartRequiredWarning.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._pnlRestartRequiredWarning.Controls.Add(this.pictureBox1);
            this._pnlRestartRequiredWarning.Controls.Add(this.label1);
            this._pnlRestartRequiredWarning.Location = new System.Drawing.Point(3, 282);
            this._pnlRestartRequiredWarning.Name = "_pnlRestartRequiredWarning";
            this._pnlRestartRequiredWarning.Size = new System.Drawing.Size(385, 20);
            this._pnlRestartRequiredWarning.TabIndex = 4;
            this._pnlRestartRequiredWarning.Visible = false;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::gitter.Framework.Properties.Resources.ImgLogWarning;
            this.pictureBox1.Location = new System.Drawing.Point(0, 3);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(16, 16);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(18, 4);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(256, 15);
            this.label1.TabIndex = 1;
            this.label1.Text = Properties.Resources.StrApplicationMustBeRestartedToApply;// "Application must be restarted to apply changes";
            // 
            // _levelAdvanced
            // 
            this._levelAdvanced.AutoSize = true;
            this._levelAdvanced.Checked = true;
            this._levelAdvanced.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this._levelAdvanced.Location = new System.Drawing.Point(3, 47);
            this._levelAdvanced.Name = "_levelAdvanced";
            this._levelAdvanced.Size = new System.Drawing.Size(84, 20);
            this._levelAdvanced.TabIndex = 2;
            this._levelAdvanced.Text = Properties.Resources.StrAdvanced;
            this._levelAdvanced.UseVisualStyleBackColor = true;
            // 
            // EssentialOptionsPage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this._pnlRestartRequiredWarning);
            this.Controls.Add(this.groupSeparator1);
            this.Controls.Add(this.panel1);
            this.Name = "EssentialOptionsPage";
            this.Size = new System.Drawing.Size(391, 305);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this._pnlRestartRequiredWarning.ResumeLayout(false);
            this._pnlRestartRequiredWarning.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.RadioButton _levelStandard;
		private System.Windows.Forms.RadioButton _levelSimple;
        private Controls.GroupSeparator groupSeparator1;
		private System.Windows.Forms.Panel _pnlRestartRequiredWarning;
		private System.Windows.Forms.PictureBox pictureBox1;
		private System.Windows.Forms.Label label1;
        private System.Windows.Forms.RadioButton _levelAdvanced;
	}
}
