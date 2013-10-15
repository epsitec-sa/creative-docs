//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.App.Widgets;
using Epsitec.Cresus.Assets.Server.NaiveEngine;

namespace Epsitec.Cresus.Assets.App.Views
{
	public class HistoryController
	{
		public HistoryController(DataAccessor accessor, Guid objectGuid, Timestamp? timestamp, int field)
		{
			this.accessor   = accessor;
			this.objectGuid = objectGuid;
			this.timestamp  = timestamp;
			this.field      = field;

			this.timestamps = new List<Timestamp> ();
			this.fieldType = this.accessor.GetFieldType ((ObjectField) this.field);
		}


		public void CreateUI(Widget parent)
		{
			this.treeTableController = new SimpleTreeTableController ();
			this.treeTableController.CreateUI (parent, footerHeight: 0);
			this.treeTableController.AllowsMovement = false;
			this.treeTableController.SetColumns (this.Columns);
			this.treeTableController.SetContent (this.Content);
			this.treeTableController.SelectedRow = this.selectedRow;
			this.treeTableController.ShowSelection ();

			this.treeTableController.RowClicked += delegate (object sender, int column, int row)
			{
				if (row >= 0 && row < this.timestamps.Count)
				{
					this.OnNavigate (this.timestamps[row]);
				}
			};
		}

		private TreeTableColumnDescription[] Columns
		{
			get
			{
				var list = new List<TreeTableColumnDescription> ();

				list.Add (new TreeTableColumnDescription (TreeTableColumnType.String, HistoryController.DateColumnWidth, "Date"));

				if (this.fieldType == FieldType.Amount ||
					this.fieldType == FieldType.Int)
				{
					list.Add (new TreeTableColumnDescription (TreeTableColumnType.Decimal, HistoryController.ValueColumnWidth, "Valeur"));
				}
				else if (this.fieldType == FieldType.Rate)
				{
					list.Add (new TreeTableColumnDescription (TreeTableColumnType.Rate, HistoryController.ValueColumnWidth, "Valeur"));
				}
				else
				{
					list.Add (new TreeTableColumnDescription (TreeTableColumnType.String, HistoryController.ValueColumnWidth, "Valeur"));
				}

				return list.ToArray ();
			}
		}

		private List<List<AbstractSimpleTreeTableCell>> Content
		{
			get
			{
				this.selectedRow = -1;
				this.timestamps.Clear ();
				var list = new List<List<AbstractSimpleTreeTableCell>> ();

				int count = this.accessor.GetObjectEventsCount (this.objectGuid);
				for (int i=0; i<count; i++)
				{
					var timestamp = this.accessor.GetObjectEventTimestamp (this.objectGuid, i);
					if (timestamp.HasValue)
					{
						var properties = this.accessor.GetObjectSingleProperties (this.objectGuid, timestamp.Value);

						var state = DataAccessor.GetPropertyState (properties, this.field);
						if (state != PropertyState.Undefined)
						{
							var row = new List<AbstractSimpleTreeTableCell> ();

							//	Ajoute la date.
							string date = Helpers.Converters.DateToString (timestamp.Value.Date);
							row.Add (new SimpleTreeTableCellString (date));

							//	Ajoute la valeur.
							if (this.fieldType == FieldType.Amount ||
								this.fieldType == FieldType.Int    ||
								this.fieldType == FieldType.Rate)
							{
								var value = DataAccessor.GetDecimalProperty (properties, this.field);
								row.Add (new SimpleTreeTableCellDecimal (value));
							}
							else
							{
								string s = DataAccessor.GetStringProperty (properties, this.field);
								row.Add (new SimpleTreeTableCellString (s));
							}

							list.Add (row);

							if (this.timestamp != null &&
								this.timestamp.Value.Date == timestamp.Value.Date)
							{
								this.selectedRow = list.Count-1;
							}

							this.timestamps.Add (timestamp.Value);
						}
					}
				}

				return list;
			}
		}


		#region Events handler
		private void OnNavigate(Timestamp timestamp)
		{
			if (this.Navigate != null)
			{
				this.Navigate (this, timestamp);
			}
		}

		public delegate void NavigateEventHandler(object sender, Timestamp timestamp);
		public event NavigateEventHandler Navigate;
		#endregion


		public static readonly int DateColumnWidth  = 80;
		public static readonly int ValueColumnWidth = 150;

		private readonly DataAccessor			accessor;
		private readonly Guid					objectGuid;
		private readonly Timestamp?				timestamp;
		private readonly int					field;
		private readonly FieldType				fieldType;
		private readonly List<Timestamp>		timestamps;

		private SimpleTreeTableController		treeTableController;
		private int								selectedRow;
	}
}
