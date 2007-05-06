using System.Text;
using System.Collections.Generic;
using System.Data;
using Epsitec.Cresus.Database;
using Epsitec.Cresus.DataLayer;
using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Types;
using Epsitec.Common.Drawing;

namespace Epsitec.Cresus.UI
{
	public class LeftList
	{
		public LeftList(DataBroker broker, string tableName, Widget parent)
		{
			this.broker = broker;
			this.tableBroker = broker.GetTableBroker (tableName);

			this.scrollArray = new ScrollArray ();
			this.scrollArray.SelectedIndexChanged += new EventHandler (this.HandleSelectedIndexChanged);
			this.scrollArray.SortChanged += new EventHandler (this.HandleSortChanged);
			this.InitTable ();
			this.UpdateTable ();
			this.scrollArray.Dock = DockStyle.Fill;
			parent.Children.Add (this.scrollArray);

		}

		protected void InitTable()
		{
			//	Initialise la table.
			this.scrollArray.TextProviderCallback = new TextProviderCallback (this.FillText);
			this.scrollArray.ColumnCount = arrayNbColumns;

			for (int x=0; x<arrayNbColumns; x++)
			{
				this.scrollArray.SetHeaderText (x, arrayColumnNames[x]);
				this.scrollArray.SetColumnWidth (x, 40);
				this.scrollArray.SetColumnAlignment (x, Epsitec.Common.Drawing.ContentAlignment.MiddleLeft);
			}
		}

		protected string FillText(int row, int column)
		{
			//	Appelé par ScrollArray pour remplir une cellule.
			if (row >= this.tableBroker.Count || column >= this.arrayNbColumns)
				return "";

			DataBrokerRecord record = this.tableBroker.GetRowFromIndex (row);
			return (string) record.GetValue (this.arrayColumnIds[column]);
		}

		protected void UpdateTable()
		{
			//	Met à jour le contenu de la table.
			this.scrollArray.Clear ();
			this.scrollArray.RowCount = this.tableBroker.Count;

			this.scrollArray.SetSortingHeader (0, SortMode.Up);

			this.scrollArray.SelectedIndex = this.recordRank;
			this.scrollArray.ShowSelected (ScrollShowMode.Extremity);
		}

		private void HandleSelectedIndexChanged(object sender)
		{
			this.recordRank = this.scrollArray.SelectedIndex;
		}

		private void HandleSortChanged(object sender)
		{
		}

		protected DataBroker broker;
		protected DataTableBroker tableBroker;
		protected ScrollArray scrollArray;
		protected int arrayNbColumns = 2;
		protected int recordRank = 0;
		protected string[] arrayColumnNames = {"Nom", "Prénom"};
		protected string[] arrayColumnIds = { "[9002]", "[9001]" };
	}
}
