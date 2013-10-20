//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;

using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.App.Widgets;
using Epsitec.Cresus.Assets.Server.NaiveEngine;

namespace Epsitec.Cresus.Assets.App.Views
{
	public class EventsTreeTableController
	{
		public EventsTreeTableController(DataAccessor accessor)
		{
			this.accessor = accessor;
		}

		public void CreateUI(Widget parent)
		{
			this.topTitle = new TopTitle
			{
				Parent = parent,
			};

			this.topTitle.SetTitle ("Evénements");

			this.CreateTreeTable (parent);
		}


		public Guid								ObjectGuid
		{
			get
			{
				return this.objectGuid;
			}
			set
			{
				if (this.objectGuid != value)
				{
					this.objectGuid = value;
					this.selectedTimestamp = null;

					this.UpdateTreeTableController ();
				}
			}
		}

		public Timestamp?						SelectedTimestamp
		{
			get
			{
				return this.selectedTimestamp;
			}
			set
			{
				if (this.selectedTimestamp != value)
				{
					this.selectedTimestamp = value;
					this.selectedRow = this.TimestampToRow (this.selectedTimestamp);

					this.UpdateTreeTableController ();
					this.OnRowClicked (this.selectedTimestamp);
				}
			}
		}


		private void CreateTreeTable(Widget parent)
		{
			this.selectedRow = -1;
			this.selectedTimestamp = null;

			this.controller = new NavigationTreeTableController();

			var frame = new FrameBox
			{
				Parent = parent,
				Dock   = DockStyle.Fill,
			};

			this.controller.CreateUI (frame, footerHeight: 0);
			this.controller.SetColumns (this.TreeTableColumns, 2);

			//	Pour que le calcul du nombre de lignes visibles soit correct.
			parent.Window.ForceLayout ();

			this.UpdateTreeTableController ();

			//	Connexion des événements.
			this.controller.ContentChanged += delegate (object sender, bool crop)
			{
				this.UpdateTreeTableController (crop);
			};

			this.controller.RowClicked += delegate (object sender, int row)
			{
				this.selectedRow = this.controller.TopVisibleRow + row;
				this.selectedTimestamp= this.RowToTimestamp (this.selectedRow);
				this.UpdateTreeTableController ();
				this.OnRowClicked (this.selectedTimestamp);
			};

			this.controller.RowDoubleClicked += delegate (object sender, int row)
			{
				this.selectedRow = this.controller.TopVisibleRow + row;
				this.selectedTimestamp= this.RowToTimestamp (this.selectedRow);
				this.UpdateTreeTableController ();
				this.OnRowDoubleClicked (this.selectedTimestamp);
			};

			this.controller.TreeButtonClicked += delegate (object sender, int row, TreeTableTreeType type)
			{
			};
		}

		private TreeTableColumnDescription[] TreeTableColumns
		{
			get
			{
				var list = new List<TreeTableColumnDescription> ();

				list.Add (new TreeTableColumnDescription (TreeTableColumnType.String,          70, "Date"));
				list.Add (new TreeTableColumnDescription (TreeTableColumnType.String,         110, "Type"));
				list.Add (new TreeTableColumnDescription (TreeTableColumnType.ComputedAmount, 120, "Valeur comptable"));
				list.Add (new TreeTableColumnDescription (TreeTableColumnType.ComputedAmount, 120, "Valeur assurance"));
				list.Add (new TreeTableColumnDescription (TreeTableColumnType.String,         120, "Responsable"));
				list.Add (new TreeTableColumnDescription (TreeTableColumnType.String,          60, "Couleur"));
				list.Add (new TreeTableColumnDescription (TreeTableColumnType.String,         200, "Numéro de série"));
				list.Add (new TreeTableColumnDescription (TreeTableColumnType.String,         180, "Objet"));
				list.Add (new TreeTableColumnDescription (TreeTableColumnType.String,          50, "N°"));

				return list.ToArray ();
			}
		}

		private void UpdateTreeTableController(bool crop = true)
		{
			this.controller.RowsCount = this.accessor.GetObjectEventsCount (this.objectGuid);

			int visibleCount = this.controller.VisibleRowsCount;
			int rowsCount    = this.controller.RowsCount;
			int count        = System.Math.Min (visibleCount, rowsCount);
			int firstRow     = this.controller.TopVisibleRow;
			int selection    = this.selectedRow;

			if (selection != -1)
			{
				//	La sélection ne peut pas dépasser le nombre maximal de lignes.
				selection = System.Math.Min (selection, rowsCount-1);

				//	Si la sélection est hors de la zone visible, on choisit un autre cadrage.
				if (crop && (selection < firstRow || selection >= firstRow+count))
				{
					firstRow = this.controller.GetTopVisibleRow (selection);
				}

				if (this.controller.TopVisibleRow != firstRow)
				{
					this.controller.TopVisibleRow = firstRow;
				}

				selection -= this.controller.TopVisibleRow;
			}

			var c1 = new List<TreeTableCellString> ();
			var c2 = new List<TreeTableCellString> ();
			var c3 = new List<TreeTableCellComputedAmount> ();
			var c4 = new List<TreeTableCellComputedAmount> ();
			var c5 = new List<TreeTableCellString> ();
			var c6 = new List<TreeTableCellString> ();
			var c7 = new List<TreeTableCellString> ();
			var c8 = new List<TreeTableCellString> ();
			var c9 = new List<TreeTableCellString> ();

			for (int i=0; i<count; i++)
			{
				if (firstRow+i >= this.controller.RowsCount)
				{
					break;
				}

				var timestamp  = this.accessor.GetObjectEventTimestamp(this.objectGuid, firstRow+i);
				var eventType  = this.accessor.GetObjectEventType (this.objectGuid, timestamp.Value);
				var properties = this.accessor.GetObjectSingleProperties (this.objectGuid, timestamp.Value);

				var date        = Helpers.Converters.DateToString (timestamp.Value.Date);
				var type        = StaticDescriptions.GetEventDescription (eventType.Value);
				var nom         = DataAccessor.GetStringProperty (properties, (int) ObjectField.Nom);
				var numéro      = DataAccessor.GetStringProperty (properties, (int) ObjectField.Numéro);
				var responsable = DataAccessor.GetStringProperty (properties, (int) ObjectField.Responsable);
				var couleur     = DataAccessor.GetStringProperty (properties, (int) ObjectField.Couleur);
				var série       = DataAccessor.GetStringProperty (properties, (int) ObjectField.NuméroSérie);
				var valeur1     = DataAccessor.GetComputedAmountProperty (properties, (int) ObjectField.Valeur1);
				var valeur2     = DataAccessor.GetComputedAmountProperty (properties, (int) ObjectField.Valeur2);

				var s1 = new TreeTableCellString (true, date, isSelected: (i == selection));
				var s2 = new TreeTableCellString (true, type, isSelected: (i == selection));
				var s3 = new TreeTableCellComputedAmount (true, valeur1, isSelected: (i == selection));
				var s4 = new TreeTableCellComputedAmount (true, valeur2, isSelected: (i == selection));
				var s5 = new TreeTableCellString (true, responsable, isSelected: (i == selection));
				var s6 = new TreeTableCellString (true, couleur, isSelected: (i == selection));
				var s7 = new TreeTableCellString (true, série, isSelected: (i == selection));
				var s8 = new TreeTableCellString (true, nom, isSelected: (i == selection));
				var s9 = new TreeTableCellString (true, numéro, isSelected: (i == selection));

				c1.Add (s1);
				c2.Add (s2);
				c3.Add (s3);
				c4.Add (s4);
				c5.Add (s5);
				c6.Add (s6);
				c7.Add (s7);
				c8.Add (s8);
				c9.Add (s9);
			}

			int c = 0;
			this.controller.SetColumnCells (c++, c1.ToArray ());
			this.controller.SetColumnCells (c++, c2.ToArray ());
			this.controller.SetColumnCells (c++, c3.ToArray ());
			this.controller.SetColumnCells (c++, c4.ToArray ());
			this.controller.SetColumnCells (c++, c5.ToArray ());
			this.controller.SetColumnCells (c++, c6.ToArray ());
			this.controller.SetColumnCells (c++, c7.ToArray ());
			this.controller.SetColumnCells (c++, c8.ToArray ());
			this.controller.SetColumnCells (c++, c9.ToArray ());
		}


		private int TimestampToRow(Timestamp? timestamp)
		{
			if (timestamp.HasValue)
			{
				int count = this.accessor.GetObjectEventsCount (this.objectGuid);
				for (int row = 0; row < count; row++)
				{
					var ts = this.accessor.GetObjectEventTimestamp (this.objectGuid, row);

					if (ts.HasValue && ts.Value == timestamp.Value)
					{
						return row;
					}
				}
			}

			return -1;
		}

		private Timestamp? RowToTimestamp(int row)
		{
			var timestamp = this.accessor.GetObjectEventTimestamp (this.objectGuid, row);

			if (timestamp == null || !timestamp.HasValue)
			{
				return null;
			}
			else
			{
				return timestamp.Value;
			}
		}


		#region Events handler
		private void OnRowClicked(Timestamp? timestamp)
		{
			if (this.RowClicked != null)
			{
				this.RowClicked (this, timestamp);
			}
		}

		public delegate void RowClickedEventHandler(object sender, Timestamp? timestamp);
		public event RowClickedEventHandler RowClicked;


		private void OnRowDoubleClicked(Timestamp? timestamp)
		{
			if (this.RowDoubleClicked != null)
			{
				this.RowDoubleClicked (this, timestamp);
			}
		}

		public delegate void RowDoubleClickedEventHandler(object sender, Timestamp? timestamp);
		public event RowDoubleClickedEventHandler RowDoubleClicked;
		#endregion


		private readonly DataAccessor			accessor;

		private Guid							objectGuid;
		private TopTitle						topTitle;
		private NavigationTreeTableController	controller;
		private int								selectedRow;
		private Timestamp?						selectedTimestamp;
	}
}
