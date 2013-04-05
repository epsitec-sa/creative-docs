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
			
		}

		private void cmd_search_Click(object sender, EventArgs e)
		{

			var entries = ws.SearchAddressByPhone(this.txt_value.Text);

			foreach (DirectoriesEntry entry in entries)
			{
				lst_result.Items.Add (entry.EntryId);
			}
		}
	}
}
