namespace gitter.Git.Gui.Views
{
	partial class DiffView
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
			if(disposing)
			{
				if(_source != null)
				{
					_source.Updated -= OnSourceUpdated;
					_source.Dispose();
				}
				if(components != null)
				{
					components.Dispose();
				}
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
			this._diffViewerHead = new gitter.Git.Gui.Controls.DiffViewer();
            this._diffViewerBody = new gitter.Git.Gui.Controls.DiffViewer();
			this.SuspendLayout();
			// 
			// _diffViewer1
			// 
			this._diffViewerHead.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this._diffViewerHead.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this._diffViewerHead.Location = new System.Drawing.Point(0, 0);
            this._diffViewerHead.Name = "_diffViewerHead";
			this._diffViewerHead.Size = new System.Drawing.Size(555, 200);
			this._diffViewerHead.TabIndex = 0;
            this._diffViewerHead.Text = "diffViewerHead";
            this._diffViewerHead.ShowHeader=true;
            this._diffViewerHead.ShowBody = false;

            // 
            // _diffViewer
            // 
            this._diffViewerBody.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._diffViewerBody.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this._diffViewerBody.Location = new System.Drawing.Point(0, 200);
            this._diffViewerBody.Name = "_diffViewer";
            this._diffViewerBody.Size = new System.Drawing.Size(555, 162);
            this._diffViewerBody.TabIndex = 0;
            this._diffViewerBody.Text = "diffViewer";
            this._diffViewerBody.ShowHeader = false;
            this._diffViewerBody.ShowBody = true;
			// 
			// DiffTool
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
			this.Controls.Add(this._diffViewerHead);
            this.Controls.Add(this._diffViewerBody);
			this.Name = "DiffTool";
			this.ResumeLayout(false);

		}

		#endregion
        private Controls.DiffViewer _diffViewerHead;
        private Controls.DiffViewer _diffViewerBody;
	}
}
