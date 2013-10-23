//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.App.Popups;
using Epsitec.Cresus.Assets.App.Widgets;
using Epsitec.Cresus.Assets.Server.NaiveEngine;

namespace Epsitec.Cresus.Assets.App.Views
{
	public class EventsToolbarTreeTableController : AbstractToolbarTreeTableController
	{
		public EventsToolbarTreeTableController(DataAccessor accessor)
			: base (accessor)
		{
			this.title = "Evénements";
		}


		public void Update()
		{
			this.UpdateController ();
			this.UpdateToolbar ();
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

					this.UpdateData ();
					this.UpdateController ();
					this.UpdateToolbar ();
				}
			}
		}

		public Timestamp?						SelectedTimestamp
		{
			get
			{
				return this.RowToTimestamp (this.selectedRow);
			}
			set
			{
				this.SelectedRow = this.TimestampToRow (value);
			}
		}


		protected override void OnNew()
		{
			var target = this.toolbar.GetCommandWidget (ToolbarCommand.New);
			var timestamp = this.SelectedTimestamp;

			if (!timestamp.HasValue)
			{
				timestamp = Timestamp.Now;
			}

			if (target != null)
			{
				System.DateTime? createDate = timestamp.Value.Date;

				var popup = new NewEventPopup
				{
					Date = timestamp.Value.Date,
				};

				popup.Create (target);

				popup.DateChanged += delegate (object sender, System.DateTime? dateTime)
				{
					if (dateTime.HasValue)
					{
						createDate = dateTime.Value;

						int sel = this.TimestampToRow (new Timestamp (dateTime.Value, 0));
						if (sel != -1)
						{
							this.SelectedRow = sel;
						}
					}
				};

				popup.ButtonClicked += delegate (object sender, string name)
				{
					if (createDate.HasValue)
					{
						this.CreateEvent (createDate.Value, name);
					}
				};
			}
		}

		private void CreateEvent(System.DateTime date, string buttonName)
		{
			var guid = this.objectGuid;

			if (!guid.IsEmpty)
			{
				var type = EventsToolbarTreeTableController.ParseEventType (buttonName);
				var timestamp = this.accessor.CreateObjectEvent (guid, date, type);

				if (timestamp.HasValue)
				{
					this.UpdateController ();
					this.UpdateToolbar ();

					this.SelectedRow = this.TimestampToRow (timestamp.Value);
				}
			}
		}

		private static EventType ParseEventType(string text)
		{
			var type = EventType.Unknown;
			System.Enum.TryParse<EventType> (text, out type);
			return type;
		}


		protected override void OnDelete()
		{
			var target = this.toolbar.GetCommandWidget (ToolbarCommand.Delete);

			if (target != null)
			{
				var popup = new YesNoPopup
				{
					Question = "Voulez-vous supprimer l'événement sélectionné ?",
				};

				popup.Create (target);

				popup.ButtonClicked += delegate (object sender, string name)
				{
					if (name == "yes")
					{
					}
				};
			}
		}


		protected override TreeTableColumnDescription[] TreeTableColumns
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

		protected override void UpdateContent(int firstRow, int count, int selection, bool crop = true)
		{
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
				if (firstRow+i >= this.NodesCount)
				{
					break;
				}

				var node = this.GetNode (firstRow+i);
				var guid = node.Guid;

				var timestamp  = this.accessor.GetObjectEventTimestamp (this.objectGuid, guid);
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


		protected override int DataCount
		{
			get
			{
				return this.accessor.GetObjectEventsCount (this.objectGuid);
			}
		}

		protected override void GetData(int row, out Guid guid, out int level)
		{
			guid = this.accessor.GetObjectEventGuid (this.objectGuid, row);
			level = 0;
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


		private Guid							objectGuid;
	}
}
