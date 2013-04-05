namespace App.Directories
{
	partial class MainForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.cmd_search = new System.Windows.Forms.Button();
            this.txt_value = new System.Windows.Forms.TextBox();
            this.chk_use_phone = new System.Windows.Forms.CheckBox();
            this.result_tree = new System.Windows.Forms.TreeView();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.Location = new System.Drawing.Point(427, 385);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(169, 26);
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            // 
            // cmd_search
            // 
            this.cmd_search.Location = new System.Drawing.Point(453, 26);
            this.cmd_search.Name = "cmd_search";
            this.cmd_search.Size = new System.Drawing.Size(142, 39);
            this.cmd_search.TabIndex = 2;
            this.cmd_search.Text = "Search";
            this.cmd_search.UseVisualStyleBackColor = true;
            this.cmd_search.Click += new System.EventHandler(this.cmd_search_Click);
            // 
            // txt_value
            // 
            this.txt_value.Font = new System.Drawing.Font("Segoe UI", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txt_value.Location = new System.Drawing.Point(12, 26);
            this.txt_value.Name = "txt_value";
            this.txt_value.Size = new System.Drawing.Size(435, 39);
            this.txt_value.TabIndex = 3;
            // 
            // chk_use_phone
            // 
            this.chk_use_phone.AutoSize = true;
            this.chk_use_phone.Location = new System.Drawing.Point(12, 68);
            this.chk_use_phone.Name = "chk_use_phone";
            this.chk_use_phone.Size = new System.Drawing.Size(121, 17);
            this.chk_use_phone.TabIndex = 4;
            this.chk_use_phone.Text = "Search Dial-Number";
            this.chk_use_phone.UseVisualStyleBackColor = true;
            // 
            // result_tree
            // 
            this.result_tree.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.result_tree.Location = new System.Drawing.Point(14, 94);
            this.result_tree.Name = "result_tree";
            this.result_tree.Size = new System.Drawing.Size(581, 278);
            this.result_tree.TabIndex = 5;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(608, 423);
            this.Controls.Add(this.result_tree);
            this.Controls.Add(this.chk_use_phone);
            this.Controls.Add(this.txt_value);
            this.Controls.Add(this.cmd_search);
            this.Controls.Add(this.pictureBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.Text = "Directories";
            this.Load += new System.EventHandler(this.MainForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

        private System.Windows.Forms.PictureBox pictureBox1;
		private System.Windows.Forms.Button cmd_search;
		private System.Windows.Forms.TextBox txt_value;
        private System.Windows.Forms.CheckBox chk_use_phone;
        private System.Windows.Forms.TreeView result_tree;
	}
}

