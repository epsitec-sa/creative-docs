using Microsoft.Maps.MapControl.WPF;
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
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
			this.pictureBox1 = new System.Windows.Forms.PictureBox();
			this.cmd_search = new System.Windows.Forms.Button();
			this.txt_value = new System.Windows.Forms.TextBox();
			this.result_tree = new System.Windows.Forms.TreeView();
			this.opt_phone = new System.Windows.Forms.RadioButton();
			this.opt_email = new System.Windows.Forms.RadioButton();
			this.opt_web = new System.Windows.Forms.RadioButton();
			this.chk_deploy_tree = new System.Windows.Forms.CheckBox();
			this.Host = new System.Windows.Forms.Integration.ElementHost();
			this.Map = new Microsoft.Maps.MapControl.WPF.Map();
			this.img_google_signin = new System.Windows.Forms.PictureBox();
			this.lst_head = new System.Windows.Forms.ListView();
			this.google_plus_image_list = new System.Windows.Forms.ImageList(this.components);
			this.opt_name = new System.Windows.Forms.RadioButton();
			this.cmd_back = new System.Windows.Forms.Button();
			this.cmd_next = new System.Windows.Forms.Button();
			this.cmd_enable_match_sort = new System.Windows.Forms.Button();
			this.chk_update_matchsort = new System.Windows.Forms.CheckBox();
			this.cmd_test_match = new System.Windows.Forms.Button();
			this.txt_match_sql = new System.Windows.Forms.TextBox();
			this.matchSortEtlBindingSource = new System.Windows.Forms.BindingSource(this.components);
			this.button1 = new System.Windows.Forms.Button();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.img_google_signin)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.matchSortEtlBindingSource)).BeginInit();
			this.SuspendLayout();
			// 
			// pictureBox1
			// 
			this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
			this.pictureBox1.Location = new System.Drawing.Point(12, 3);
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
			this.txt_value.MouseClick += new System.Windows.Forms.MouseEventHandler(this.txt_value_MouseClick);
			this.txt_value.KeyUp += new System.Windows.Forms.KeyEventHandler(this.txt_value_KeyUp);
			// 
			// result_tree
			// 
			this.result_tree.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.result_tree.Location = new System.Drawing.Point(12, 94);
			this.result_tree.Name = "result_tree";
			this.result_tree.Size = new System.Drawing.Size(584, 403);
			this.result_tree.TabIndex = 5;
			this.result_tree.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.result_tree_NodeMouseClick);
			// 
			// opt_phone
			// 
			this.opt_phone.AutoSize = true;
			this.opt_phone.Location = new System.Drawing.Point(92, 71);
			this.opt_phone.Name = "opt_phone";
			this.opt_phone.Size = new System.Drawing.Size(56, 17);
			this.opt_phone.TabIndex = 6;
			this.opt_phone.Text = "Phone";
			this.opt_phone.UseVisualStyleBackColor = true;
			// 
			// opt_email
			// 
			this.opt_email.AutoSize = true;
			this.opt_email.Location = new System.Drawing.Point(181, 70);
			this.opt_email.Name = "opt_email";
			this.opt_email.Size = new System.Drawing.Size(50, 17);
			this.opt_email.TabIndex = 7;
			this.opt_email.Text = "Email";
			this.opt_email.UseVisualStyleBackColor = true;
			// 
			// opt_web
			// 
			this.opt_web.AutoSize = true;
			this.opt_web.Location = new System.Drawing.Point(256, 71);
			this.opt_web.Name = "opt_web";
			this.opt_web.Size = new System.Drawing.Size(64, 17);
			this.opt_web.TabIndex = 8;
			this.opt_web.Text = "Website";
			this.opt_web.UseVisualStyleBackColor = true;
			// 
			// chk_deploy_tree
			// 
			this.chk_deploy_tree.AutoSize = true;
			this.chk_deploy_tree.Location = new System.Drawing.Point(453, 70);
			this.chk_deploy_tree.Name = "chk_deploy_tree";
			this.chk_deploy_tree.Size = new System.Drawing.Size(99, 17);
			this.chk_deploy_tree.TabIndex = 9;
			this.chk_deploy_tree.Text = "Collapse results";
			this.chk_deploy_tree.UseVisualStyleBackColor = true;
			this.chk_deploy_tree.CheckedChanged += new System.EventHandler(this.chk_deploy_tree_CheckedChanged);
			// 
			// Host
			// 
			this.Host.Location = new System.Drawing.Point(602, 94);
			this.Host.Name = "Host";
			this.Host.Size = new System.Drawing.Size(660, 403);
			this.Host.TabIndex = 10;
			this.Host.Text = "WPF Host";
			this.Host.Child = this.Map;
			// 
			// img_google_signin
			// 
			this.img_google_signin.Image = ((System.Drawing.Image)(resources.GetObject("img_google_signin.Image")));
			this.img_google_signin.Location = new System.Drawing.Point(1030, 12);
			this.img_google_signin.Name = "img_google_signin";
			this.img_google_signin.Size = new System.Drawing.Size(232, 76);
			this.img_google_signin.TabIndex = 11;
			this.img_google_signin.TabStop = false;
			this.img_google_signin.Click += new System.EventHandler(this.img_google_signin_Click);
			// 
			// lst_head
			// 
			this.lst_head.LargeImageList = this.google_plus_image_list;
			this.lst_head.Location = new System.Drawing.Point(603, 503);
			this.lst_head.Name = "lst_head";
			this.lst_head.Size = new System.Drawing.Size(658, 117);
			this.lst_head.TabIndex = 12;
			this.lst_head.UseCompatibleStateImageBehavior = false;
			// 
			// google_plus_image_list
			// 
			this.google_plus_image_list.ColorDepth = System.Windows.Forms.ColorDepth.Depth32Bit;
			this.google_plus_image_list.ImageSize = new System.Drawing.Size(50, 50);
			this.google_plus_image_list.TransparentColor = System.Drawing.Color.Transparent;
			// 
			// opt_name
			// 
			this.opt_name.AutoSize = true;
			this.opt_name.Checked = true;
			this.opt_name.Location = new System.Drawing.Point(12, 70);
			this.opt_name.Name = "opt_name";
			this.opt_name.Size = new System.Drawing.Size(53, 17);
			this.opt_name.TabIndex = 13;
			this.opt_name.TabStop = true;
			this.opt_name.Text = "Name";
			this.opt_name.UseVisualStyleBackColor = true;
			// 
			// cmd_back
			// 
			this.cmd_back.Location = new System.Drawing.Point(496, 153);
			this.cmd_back.Name = "cmd_back";
			this.cmd_back.Size = new System.Drawing.Size(82, 26);
			this.cmd_back.TabIndex = 14;
			this.cmd_back.Text = "Back";
			this.cmd_back.UseVisualStyleBackColor = true;
			this.cmd_back.Visible = false;
			this.cmd_back.Click += new System.EventHandler(this.cmd_back_Click);
			// 
			// cmd_next
			// 
			this.cmd_next.Location = new System.Drawing.Point(496, 185);
			this.cmd_next.Name = "cmd_next";
			this.cmd_next.Size = new System.Drawing.Size(82, 26);
			this.cmd_next.TabIndex = 15;
			this.cmd_next.Text = "Next";
			this.cmd_next.UseVisualStyleBackColor = true;
			this.cmd_next.Visible = false;
			this.cmd_next.Click += new System.EventHandler(this.cmd_next_Click);
			// 
			// cmd_enable_match_sort
			// 
			this.cmd_enable_match_sort.Location = new System.Drawing.Point(3, 626);
			this.cmd_enable_match_sort.Name = "cmd_enable_match_sort";
			this.cmd_enable_match_sort.Size = new System.Drawing.Size(124, 33);
			this.cmd_enable_match_sort.TabIndex = 16;
			this.cmd_enable_match_sort.Text = "Enable Mat[CH] Sort";
			this.cmd_enable_match_sort.UseVisualStyleBackColor = true;
			this.cmd_enable_match_sort.Click += new System.EventHandler(this.cmd_enable_match_sort_Click);
			// 
			// chk_update_matchsort
			// 
			this.chk_update_matchsort.AutoSize = true;
			this.chk_update_matchsort.Location = new System.Drawing.Point(133, 626);
			this.chk_update_matchsort.Name = "chk_update_matchsort";
			this.chk_update_matchsort.Size = new System.Drawing.Size(135, 17);
			this.chk_update_matchsort.TabIndex = 17;
			this.chk_update_matchsort.Text = "chk_update_matchsort";
			this.chk_update_matchsort.UseVisualStyleBackColor = true;
			// 
			// cmd_test_match
			// 
			this.cmd_test_match.Location = new System.Drawing.Point(3, 704);
			this.cmd_test_match.Name = "cmd_test_match";
			this.cmd_test_match.Size = new System.Drawing.Size(124, 33);
			this.cmd_test_match.TabIndex = 18;
			this.cmd_test_match.Text = "Query";
			this.cmd_test_match.UseVisualStyleBackColor = true;
			this.cmd_test_match.Click += new System.EventHandler(this.cmd_test_match_Click);
			// 
			// txt_match_sql
			// 
			this.txt_match_sql.Font = new System.Drawing.Font("Verdana", 15.75F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.txt_match_sql.Location = new System.Drawing.Point(3, 665);
			this.txt_match_sql.Name = "txt_match_sql";
			this.txt_match_sql.Size = new System.Drawing.Size(1258, 33);
			this.txt_match_sql.TabIndex = 19;
			// 
			// matchSortEtlBindingSource
			// 
			this.matchSortEtlBindingSource.DataSource = typeof(Epsitec.Data.Platform.MatchSortEtl);
			// 
			// button1
			// 
			this.button1.Location = new System.Drawing.Point(3, 752);
			this.button1.Name = "button1";
			this.button1.Size = new System.Drawing.Size(124, 32);
			this.button1.TabIndex = 20;
			this.button1.Text = "button1";
			this.button1.UseVisualStyleBackColor = true;
			this.button1.Click += new System.EventHandler(this.button1_Click);
			// 
			// MainForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(1274, 891);
			this.Controls.Add(this.button1);
			this.Controls.Add(this.txt_match_sql);
			this.Controls.Add(this.cmd_test_match);
			this.Controls.Add(this.chk_update_matchsort);
			this.Controls.Add(this.cmd_enable_match_sort);
			this.Controls.Add(this.cmd_next);
			this.Controls.Add(this.cmd_back);
			this.Controls.Add(this.opt_name);
			this.Controls.Add(this.lst_head);
			this.Controls.Add(this.img_google_signin);
			this.Controls.Add(this.Host);
			this.Controls.Add(this.chk_deploy_tree);
			this.Controls.Add(this.opt_web);
			this.Controls.Add(this.opt_email);
			this.Controls.Add(this.opt_phone);
			this.Controls.Add(this.result_tree);
			this.Controls.Add(this.txt_value);
			this.Controls.Add(this.cmd_search);
			this.Controls.Add(this.pictureBox1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.MaximizeBox = false;
			this.Name = "MainForm";
			this.Text = "Epsitec Directories";
			this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.MainForm_FormClosed);
			this.Load += new System.EventHandler(this.MainForm_Load);
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.img_google_signin)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.matchSortEtlBindingSource)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.PictureBox pictureBox1;
		private System.Windows.Forms.Button cmd_search;
		private System.Windows.Forms.TextBox txt_value;
		private System.Windows.Forms.TreeView result_tree;
		private System.Windows.Forms.RadioButton opt_phone;
		private System.Windows.Forms.RadioButton opt_email;
		private System.Windows.Forms.RadioButton opt_web;
		private System.Windows.Forms.CheckBox chk_deploy_tree;
		private System.Windows.Forms.Integration.ElementHost Host;
		private Map Map;
		private System.Windows.Forms.PictureBox img_google_signin;
		private System.Windows.Forms.ListView lst_head;
		private System.Windows.Forms.ImageList google_plus_image_list;
		private System.Windows.Forms.RadioButton opt_name;
		private System.Windows.Forms.Button cmd_back;
		private System.Windows.Forms.Button cmd_next;
		private System.Windows.Forms.Button cmd_enable_match_sort;
		private System.Windows.Forms.CheckBox chk_update_matchsort;
		private System.Windows.Forms.Button cmd_test_match;
		private System.Windows.Forms.TextBox txt_match_sql;
		private System.Windows.Forms.BindingSource matchSortEtlBindingSource;
		private System.Windows.Forms.Button button1;
	}
}

