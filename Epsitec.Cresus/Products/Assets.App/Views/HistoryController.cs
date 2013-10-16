//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.App.Widgets;
using Epsitec.Cresus.Assets.Server.NaiveEngine;

namespace Epsitec.Cresus.Assets.App.Views
{
	public class HistoryController
	{
		public HistoryController(DataAccessor accessor, Guid objectGuid, Timestamp? timestamp, int field)
		{
			this.accessor = accessor;

			this.fieldType = this.accessor.GetFieldType ((ObjectField) field);

			this.content    = new List<List<AbstractSimpleTreeTableCell>> ();
			this.timestamps = new List<Timestamp> ();

			this.InitializeContent (objectGuid, timestamp, field);
		}


		public int RowsCount
		{
			get
			{
				return this.content.Count ();
			}
		}


		public void CreateUI(Widget parent)
		{
			new StaticText
			{
				Parent           = parent,
				Text             = "Historique des modifications",
				ContentAlignment = ContentAlignment.MiddleCenter,
				Dock             = DockStyle.Top,
				PreferredHeight  = HistoryController.TitleHeight - 4,
				BackColor        = ColorManager.SelectionColor,
			};

			new StaticText
			{
				Parent           = parent,
				Dock             = DockStyle.Top,
				PreferredHeight  = 4,
				BackColor        = ColorManager.SelectionColor,
			};

			if (this.RowsCount == 0)
			{
				new StaticText
				{
					Parent           = parent,
					Text             = "Ce champ n'est jamais défini.",
					ContentAlignment = ContentAlignment.MiddleCenter,
					Dock             = DockStyle.Fill,
				};
			}
			else
			{
				this.treeTableController = new SimpleTreeTableController ();
				this.treeTableController.CreateUI (parent, rowHeight: HistoryController.RowHeight, headerHeight: HistoryController.HeaderHeight, footerHeight: 0);
				this.treeTableController.AllowsMovement = false;
				this.treeTableController.SetColumns (this.Columns);
				this.treeTableController.SetContent (this.content);
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

		private void InitializeContent(Guid objectGuid, Timestamp? timestamp, int field)
		{
			this.selectedRow = -1;
			this.content.Clear ();
			this.timestamps.Clear ();

			int count = this.accessor.GetObjectEventsCount (objectGuid);
			for (int i=0; i<count; i++)
			{
				var eventTimestamp = this.accessor.GetObjectEventTimestamp (objectGuid, i);
				if (eventTimestamp.HasValue)
				{
					var properties = this.accessor.GetObjectSingleProperties (objectGuid, eventTimestamp.Value);

					var state = DataAccessor.GetPropertyState (properties, field);
					if (state != PropertyState.Undefined)
					{
						var row = new List<AbstractSimpleTreeTableCell> ();

						//	Ajoute la date.
						string date = Helpers.Converters.DateToString (eventTimestamp.Value.Date);
						row.Add (new SimpleTreeTableCellString (date));

						//	Ajoute la valeur.
						if (this.fieldType == FieldType.Amount ||
							this.fieldType == FieldType.Int    ||
							this.fieldType == FieldType.Rate)
						{
							var value = DataAccessor.GetDecimalProperty (properties, field);
							row.Add (new SimpleTreeTableCellDecimal (value));
						}
						else
						{
							string s = DataAccessor.GetStringProperty (properties, field);
							row.Add (new SimpleTreeTableCellString (s));
						}

						this.content.Add (row);

						if (timestamp != null &&
							timestamp.Value.Date == eventTimestamp.Value.Date)
						{
							this.selectedRow = this.content.Count-1;
						}

						this.timestamps.Add (eventTimestamp.Value);
					}
				}
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


		public static readonly int TitleHeight      = 24;
		public static readonly int HeaderHeight     = 22;
		public static readonly int RowHeight        = 18;
		public static readonly int DateColumnWidth  = 80;
		public static readonly int ValueColumnWidth = 150;

		private readonly DataAccessor								accessor;
		private readonly FieldType									fieldType;
		private readonly List<List<AbstractSimpleTreeTableCell>>	content;
		private readonly List<Timestamp>							timestamps;

		private SimpleTreeTableController							treeTableController;
		private int													selectedRow;
	}
}
