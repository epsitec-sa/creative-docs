//	Copyright © 2003, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Statut : OK/PA, 24/11/2003

namespace Epsitec.Cresus.UserInterface.Debugging
{
	using Widgets = Epsitec.Common.Widgets;
	using Drawing = Epsitec.Common.Drawing;
	
	/// <summary>
	/// La classe DataSetDisplay permet de visualiser le contenu d'un DataSet ou de
	/// une ou plusieurs instances de DataTable.
	/// </summary>
	public class DataSetDisplay
	{
		public DataSetDisplay()
		{
			this.CreateWindow ();
		}
		
		public void Dispose()
		{
			this.window.Dispose ();
			
			this.window = null;
			this.book   = null;
		}
		
		public void ShowWindow()
		{
			this.window.Show();
		}
		
		public void AddTable(string table_name, System.Data.DataTable data_table)
		{
			Widgets.TabPage page = new Widgets.TabPage ();
			
			page.TabTitle = string.Format ("Table <i>{0}</i>...", table_name);
			page.DockMargins = new Drawing.Margins (4, 4, 4, 4);
			
			Widgets.CellTable table = new Widgets.CellTable ();
			
			int col_n = data_table.Columns.Count;
			int row_n = data_table.Rows.Count;
			
			table.StyleH  = Widgets.CellArrayStyle.ScrollNorm;
			table.StyleH |= Widgets.CellArrayStyle.Header;
			table.StyleH |= Widgets.CellArrayStyle.Separator;
			table.StyleH |= Widgets.CellArrayStyle.Mobile;
			table.StyleH |= Widgets.CellArrayStyle.Sort;
			table.StyleV  = Widgets.CellArrayStyle.ScrollNorm;
			table.StyleV |= Widgets.CellArrayStyle.Header;
			table.StyleV |= Widgets.CellArrayStyle.Separator;
			table.StyleV |= Widgets.CellArrayStyle.SelectLine;
			table.Name = "Table";
			table.SetArraySize(col_n, row_n);
			
			for (int i = 0; i < col_n; i++)
			{
				table.SetHeaderTextH (i, data_table.Columns[i].Caption);
			}
			for (int i = 0; i < row_n; i++)
			{
				table.SetHeaderTextV (i, i.ToString ());
			}
			
			for (int row = 0; row < row_n; row++)
			{
				System.Data.DataRow data_row = data_table.Rows[row];
				for (int col = 0; col < col_n; col++)
				{
					Widgets.StaticText text = new Widgets.StaticText ();
					text.Text          = Widgets.TextLayout.ConvertToTaggedText (data_row[col].ToString ());
					text.Alignment     = Drawing.ContentAlignment.MiddleLeft;
					text.TextBreakMode = Drawing.TextBreakMode.Ellipsis | Drawing.TextBreakMode.SingleLine;
					text.Dock          = Widgets.DockStyle.Fill;
					table[col,row].Insert (text);
				}
			}
			
			table.Dock = Widgets.DockStyle.Fill;
			table.Parent = page;
			
			this.book.Items.Add (page);
			this.book.ActivePage = page;
		}
		
		
		private void CreateWindow()
		{
			this.window = new Widgets.Window ();
			
			this.window.ClientSize = new Drawing.Size (400, 300);
			this.window.Text = "DataSet Explorer";
			this.window.Root.PreferHorizontalDockLayout = false;
			
			this.book = new Widgets.TabBook ();
			this.book.Dock = Widgets.DockStyle.Fill;
			this.book.Parent = this.window.Root;
		}
		
		
		private Widgets.Window			window;
		private Widgets.TabBook			book;
	}
}
