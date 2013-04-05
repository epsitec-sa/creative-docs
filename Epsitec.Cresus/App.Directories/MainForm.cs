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

            DirectoriesSearchAddressResult result = null;
            if (this.chk_use_phone.Checked)
            {
                result = ws.SearchAddressByPhone(this.txt_value.Text);
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
                this.result_tree.Nodes.Add("Query has " + result.Info.MatchedEntries.ToString() + " Result."); 
            }
            else
            {
                this.result_tree.Nodes.Add("Query has " + result.Info.MatchedEntries.ToString() + " Results."); 

            }
            
           
            foreach (DirectoriesEntry entries in result.Entries)
            {
                foreach (DirectoriesEntryAdd add in entries.EntryAdds)
                {
                    var node = this.result_tree.Nodes.Add(add.FirstName + " " + add.LastName + ", " + add.Profession + ", " + add.LocaPostName);
                    foreach (DirectoriesEntryAddService ser in add.Services)
                    {
                        node.Nodes.Add(ser.Value);
                    }
                }    
            }
                    
            
            this.result_tree.ExpandAll();
                
            
            
            
		}

        private void reportViewer1_Load(object sender, EventArgs e)
        {

        }
	}
}
