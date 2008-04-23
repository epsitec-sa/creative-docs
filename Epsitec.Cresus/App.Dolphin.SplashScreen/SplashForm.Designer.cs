//	Copyright © 2006-2008, OPaC bright ideas, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

namespace Epsitec.App.Dolphin
{
	partial class SplashForm
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
				components.Dispose ();
			}
			base.Dispose (disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.labelMessage = new System.Windows.Forms.Label ();
			this.labelCopyright = new System.Windows.Forms.Label ();
			this.SuspendLayout ();
			// 
			// labelMessage
			// 
			this.labelMessage.AutoSize = true;
			this.labelMessage.BackColor = System.Drawing.Color.Transparent;
			this.labelMessage.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.labelMessage.Location = new System.Drawing.Point (4, 170);
			this.labelMessage.Name = "labelMessage";
			this.labelMessage.Size = new System.Drawing.Size (0, 13);
			this.labelMessage.TabIndex = 0;
			// 
			// labelCopyright
			// 
			this.labelCopyright.AutoSize = true;
			this.labelCopyright.BackColor = System.Drawing.Color.Transparent;
			this.labelCopyright.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.labelCopyright.ForeColor = System.Drawing.Color.White;
			this.labelCopyright.Location = new System.Drawing.Point (4, 183);
			this.labelCopyright.Name = "labelCopyright";
			this.labelCopyright.Size = new System.Drawing.Size (0, 13);
			this.labelCopyright.TabIndex = 1;
			// 
			// SplashForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF (6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.SystemColors.Window;
			this.BackgroundImage = global::Epsitec.App.Dolphin.Properties.Resources.PhotoBook;
			this.ClientSize = new System.Drawing.Size (320, 200);
			this.ControlBox = false;
			this.Controls.Add (this.labelMessage);
			this.Controls.Add (this.labelCopyright);
			this.Cursor = System.Windows.Forms.Cursors.AppStarting;
			this.DoubleBuffered = true;
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "SplashForm";
			this.Padding = new System.Windows.Forms.Padding (4);
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "SplashForm";
			this.ResumeLayout (false);
			this.PerformLayout ();

		}

		#endregion

		internal System.Windows.Forms.Label labelMessage;
		private System.Windows.Forms.Label labelCopyright;
	}
}