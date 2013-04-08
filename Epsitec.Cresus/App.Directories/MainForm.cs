using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Epsitec.Data.Platform.Directories;
using Epsitec.Data.Platform.Directories.Entity;

namespace App.Directories
{
	public partial class MainForm : Form
	{
		public MainForm()
		{
			InitializeComponent ();
		}

		public DirectoriesWebService ws = new DirectoriesWebService();
		
		private void MainForm_Load(object sender, EventArgs e)
		{
			//TODO
			
		}

		private void cmd_search_Click(object sender, EventArgs e)
		{

			DirectoriesSearchAddressResult result = null;
			if (this.opt_phone.Checked || this.opt_email.Checked || this.opt_web.Checked)
			{
				var ValueList = new List<string> ();
				var Values = this.txt_value.Text.Split (',');
				foreach (string Value in Values)
				{
					ValueList.Add (Value);
				}
				if (this.opt_phone.Checked)
				{
					result = ws.SearchService (ValueList, DirectoriesServiceCode.TelCode);
				}
				if (this.opt_email.Checked)
				{
					result = ws.SearchService (ValueList, DirectoriesServiceCode.EmailCode);
				}
				if (this.opt_web.Checked)
				{
					result = ws.SearchService (ValueList, DirectoriesServiceCode.WebCode);
				}
				
			}
			else
			{
				string[] Args = this.txt_value.Text.Split(' ');
					
				switch(Args.Length)
				{
					case 0: result = ws.SearchAddressByPhone(this.txt_value.Text);
					break;

					case 1: result = ws.SearchAddressByFirstName(this.txt_value.Text, 1, 100);
					break;

					case 2: result = ws.SearchAddressByLastNameAndLocation(Args[0], Args[1], 1, 100);
					break;
				}
			}
				

			//Clear Tree
			this.result_tree.Nodes.Clear();
			if (result.Info.MatchedEntries < 2)
			{
				this.result_tree.Nodes.Add (String.Format ("Query has {0} Result.", result.Info.MatchedEntries)); 
			}
			else
			{
				this.result_tree.Nodes.Add (String.Format ("Query has {0} Results.", result.Info.MatchedEntries)); 
			}
			
		   
			foreach (DirectoriesEntry entries in result.Entries)
			{
				foreach (DirectoriesEntryAdd add in entries.EntryAdds)
				{
					var node = this.result_tree.Nodes.Add (String.Format ("{0} {1}, {2}, {3}", add.FirstName, add.LastName, add.Profession, add.LocaPostName));
					foreach (DirectoriesEntryAddService ser in add.Services)
					{
						node.Nodes.Add(ser.Value);
					}
				}    
			}
					
			
			this.result_tree.ExpandAll();
			this.chk_deploy_tree.CheckState = CheckState.Unchecked;
			
			
			
		}

		private void chk_deploy_tree_CheckedChanged(object sender, EventArgs e)
		{
			if (this.chk_deploy_tree.Checked)
			{
				this.result_tree.CollapseAll ();
			}
			else
			{
				this.result_tree.ExpandAll ();
			}
		}
	}
}
