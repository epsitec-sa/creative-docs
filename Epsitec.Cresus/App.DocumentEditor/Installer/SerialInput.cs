//	Copyright © 2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.App.DocumentEditor.Installer
{
	/// <summary>
	/// Summary description for SerialInput.
	/// </summary>
	public class SerialInput : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Label		label1;
		private System.Windows.Forms.Label		label2;
		private System.Windows.Forms.Label		label3;
		private System.Windows.Forms.Label		label4;
		private System.Windows.Forms.TextBox	text_a;
		private System.Windows.Forms.TextBox	text_b;
		private System.Windows.Forms.TextBox	text_c;
		private System.Windows.Forms.TextBox	text_d;
		private System.Windows.Forms.Button		button_ok;
		private System.Windows.Forms.Button		button_cancel;

		private string							serial_key;
		private bool							serial_ok;
		
		private System.ComponentModel.Container components = null;
		
		public SerialInput()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
		}

		
		public bool								SerialOk
		{
			get
			{
				return this.serial_ok;
			}
		}
		
		public string							SerialKey
		{
			get
			{
				return this.serial_key;
			}
		}
		
		
		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (components != null)
				{
					components.Dispose ();
				}
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
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(SerialInput));
			this.button_ok = new System.Windows.Forms.Button();
			this.label1 = new System.Windows.Forms.Label();
			this.text_a = new System.Windows.Forms.TextBox();
			this.text_b = new System.Windows.Forms.TextBox();
			this.text_c = new System.Windows.Forms.TextBox();
			this.button_cancel = new System.Windows.Forms.Button();
			this.text_d = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// button_ok
			// 
			this.button_ok.Enabled = false;
			this.button_ok.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.button_ok.Location = new System.Drawing.Point(160, 120);
			this.button_ok.Name = "button_ok";
			this.button_ok.TabIndex = 4;
			this.button_ok.Text = "OK";
			this.button_ok.Click += new System.EventHandler(this.button_ok_Click);
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(8, 16);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(320, 24);
			this.label1.TabIndex = 6;
			this.label1.Text = "Veuillez introduire le numéro de série associé à votre logiciel :";
			// 
			// text_a
			// 
			this.text_a.Location = new System.Drawing.Point(16, 56);
			this.text_a.MaxLength = 5;
			this.text_a.Name = "text_a";
			this.text_a.Size = new System.Drawing.Size(48, 20);
			this.text_a.TabIndex = 0;
			this.text_a.Text = "";
			this.text_a.TextChanged += new System.EventHandler(this.text_a_TextChanged);
			// 
			// text_b
			// 
			this.text_b.Location = new System.Drawing.Point(88, 56);
			this.text_b.MaxLength = 6;
			this.text_b.Name = "text_b";
			this.text_b.Size = new System.Drawing.Size(64, 20);
			this.text_b.TabIndex = 1;
			this.text_b.Text = "";
			this.text_b.TextChanged += new System.EventHandler(this.text_b_TextChanged);
			// 
			// text_c
			// 
			this.text_c.Location = new System.Drawing.Point(176, 56);
			this.text_c.MaxLength = 4;
			this.text_c.Name = "text_c";
			this.text_c.Size = new System.Drawing.Size(48, 20);
			this.text_c.TabIndex = 2;
			this.text_c.Text = "";
			this.text_c.TextChanged += new System.EventHandler(this.text_c_TextChanged);
			// 
			// button_cancel
			// 
			this.button_cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.button_cancel.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.button_cancel.Location = new System.Drawing.Point(248, 120);
			this.button_cancel.Name = "button_cancel";
			this.button_cancel.TabIndex = 5;
			this.button_cancel.Text = "Annule";
			this.button_cancel.Click += new System.EventHandler(this.button_cancel_Click);
			// 
			// text_d
			// 
			this.text_d.Location = new System.Drawing.Point(248, 56);
			this.text_d.MaxLength = 6;
			this.text_d.Name = "text_d";
			this.text_d.Size = new System.Drawing.Size(64, 20);
			this.text_d.TabIndex = 3;
			this.text_d.Text = "";
			this.text_d.TextChanged += new System.EventHandler(this.text_d_TextChanged);
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(64, 56);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(24, 20);
			this.label2.TabIndex = 7;
			this.label2.Text = "-";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(152, 56);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(24, 20);
			this.label3.TabIndex = 7;
			this.label3.Text = "-";
			this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(224, 56);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(24, 20);
			this.label4.TabIndex = 7;
			this.label4.Text = "-";
			this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// SerialInput
			// 
			this.AcceptButton = this.button_ok;
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.CancelButton = this.button_cancel;
			this.ClientSize = new System.Drawing.Size(338, 160);
			this.ControlBox = false;
			this.Controls.Add(this.text_a);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.button_ok);
			this.Controls.Add(this.text_b);
			this.Controls.Add(this.text_c);
			this.Controls.Add(this.button_cancel);
			this.Controls.Add(this.text_d);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.label4);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "SerialInput";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Installation de Crésus Documents";
			this.Load += new System.EventHandler(this.SerialInput_Load);
			this.ResumeLayout(false);

		}
		#endregion
		
		private void button_ok_Click(object sender, System.EventArgs e)
		{
			this.serial_ok = true;
			this.Close ();
		}

		private void button_cancel_Click(object sender, System.EventArgs e)
		{
			this.serial_ok = false;
			this.Close ();
		}

		private void text_a_TextChanged(object sender, System.EventArgs e)
		{
			this.ValidateFields ();
			
			if (this.text_a.Text.Length == this.text_a.MaxLength)
			{
				this.text_b.SelectAll ();
				this.text_b.Focus ();
			}
		}

		private void text_b_TextChanged(object sender, System.EventArgs e)
		{
			this.ValidateFields ();
			
			if (this.text_b.Text.Length == this.text_b.MaxLength)
			{
				this.text_c.SelectAll ();
				this.text_c.Focus ();
			}
		}

		private void text_c_TextChanged(object sender, System.EventArgs e)
		{
			this.ValidateFields ();
			
			if (this.text_c.Text.Length == this.text_c.MaxLength)
			{
				this.text_d.SelectAll ();
				this.text_d.Focus ();
			}
		}

		private void text_d_TextChanged(object sender, System.EventArgs e)
		{
			this.ValidateFields ();
		}
		
		private void ValidateFields()
		{
			try
			{
				if ((this.text_a.Text.Length != 5) ||
					(this.text_b.Text.Length != 6) ||
					(this.text_c.Text.Length != 4) ||
					((this.text_d.Text.Length != 0) && (this.text_d.Text.Length != 6)))
				{
					throw new System.ApplicationException ("Invalid serial.");
				}
				
				int num_a = System.Int32.Parse (this.text_a.Text);
				int num_b = System.Int32.Parse (this.text_b.Text);
				int num_c = System.Int32.Parse (this.text_c.Text);
				int num_d = this.text_d.Text.Length == 0 ? 0 : System.Int32.Parse (this.text_d.Text);
				
				this.serial_key = string.Format ("{0:00000}-{1:000000}-{2:0000}-{3:000000}", num_a, num_b, num_c, num_d);
				
				this.button_ok.Enabled = SerialAlgorithm.TestSerial (this.serial_key);
			}
			catch
			{
				this.button_ok.Enabled = false;
			}
		}

		private void SerialInput_Load(object sender, System.EventArgs e)
		{
			this.TopLevel = true;
			this.TopMost  = true;
			
			this.text_a.Focus ();
		}
	}
}
