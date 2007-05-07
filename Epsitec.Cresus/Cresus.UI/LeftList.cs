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
	public class LeftList : DependencyObject
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
				this.scrollArray.SetColumnWidth (x, 65);
				this.scrollArray.SetColumnAlignment (x, Epsitec.Common.Drawing.ContentAlignment.MiddleLeft);
			}
			this.scrollArray.SelectedIndex = 2;
		}

		protected string FillText(int row, int column)
		{
			//	Appelé par ScrollArray pour remplir une cellule.
			if (row >= this.tableBroker.Count || column >= this.arrayNbColumns)
				return "";

			DataBrokerRecord record = this.tableBroker.GetRowFromIndex (row); // - 2
			return (string) record.GetValue (this.arrayColumnIds[column]);
		}

		protected void UpdateTable()
		{
			//	Met à jour le contenu de la table.
			this.scrollArray.Clear ();
			this.scrollArray.RowCount = this.tableBroker.Count; // + 2
			int temp = this.tableBroker.Count;

			this.scrollArray.SetSortingHeader (0, SortMode.Up);

			this.scrollArray.SelectedIndex = this.recordRank;
			this.scrollArray.ShowSelected (ScrollShowMode.Extremity);
		}

		public int RecordRank
		{
			get
			{
				return recordRank;
			}
		}

		private void HandleSelectedIndexChanged(object sender)
		{
			this.recordRank = this.scrollArray.SelectedIndex;
			OnSelectedChanged ();
		}

		private void HandleSortChanged(object sender)
		{
		}

		public event EventHandler SelectedChanged
		{
			add
			{
				this.AddUserEventHandler ("SelectedChanged", value);
			}
			remove
			{
				this.RemoveUserEventHandler ("SelectedChanged", value);
			}
		}

		protected virtual void OnSelectedChanged()
		{
			EventHandler handler = (EventHandler) this.GetUserEventHandler ("SelectedChanged");
			if (handler != null)
			{
				handler (this);
			}
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
