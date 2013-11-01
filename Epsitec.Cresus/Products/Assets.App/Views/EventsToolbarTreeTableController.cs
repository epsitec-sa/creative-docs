//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.App.Popups;
using Epsitec.Cresus.Assets.App.Widgets;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.App.Views
{
	public class EventsToolbarTreeTableController : AbstractToolbarTreeTableController
	{
		public EventsToolbarTreeTableController(DataAccessor accessor)
			: base (accessor)
		{
			this.title = "Evénements";
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
					this.obj = this.accessor.GetObject (this.objectGuid);

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
				return this.RowToTimestamp (this.SelectedRow);
			}
			set
			{
				this.VisibleSelectedRow = this.TimestampToRow (value);
			}
		}


		public void Update()
		{
			this.UpdateData ();
			this.UpdateController ();
			this.UpdateToolbar ();
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
			if (this.obj != null)
			{
				var type = EventsToolbarTreeTableController.ParseEventType (buttonName);
				var e = this.accessor.CreateObjectEvent (this.obj, date, type);

				if (e != null)
				{
					this.UpdateData ();
					this.UpdateController ();
					this.UpdateToolbar ();

					this.SelectedRow = this.TimestampToRow (e.Timestamp);
				}

				this.OnStartEditing (type);
				this.OnUpdateAll ();
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
				list.Add (new TreeTableColumnDescription (TreeTableColumnType.Glyph,           20, ""));
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
			var c2 = new List<TreeTableCellGlyph> ();
			var c3 = new List<TreeTableCellString> ();
			var c4 = new List<TreeTableCellComputedAmount> ();
			var c5 = new List<TreeTableCellComputedAmount> ();
			var c6 = new List<TreeTableCellString> ();
			var c7 = new List<TreeTableCellString> ();
			var c8 = new List<TreeTableCellString> ();
			var c9 = new List<TreeTableCellString> ();
			var c10 = new List<TreeTableCellString> ();

			for (int i=0; i<count; i++)
			{
				if (firstRow+i >= this.NodesCount)
				{
					break;
				}

				var node = this.GetNode (firstRow+i);
				var guid = node.Guid;
				var e    = this.obj.GetEvent (guid);

				var timestamp  = e.Timestamp;
				var eventType  = e.Type;

				var date        = Helpers.Converters.DateToString (timestamp.Date);
				var glyph       = TimelineData.TypeToGlyph (eventType);
				var type        = StaticDescriptions.GetEventDescription (eventType);
				var nom         = ObjectCalculator.GetObjectPropertyString (this.obj, timestamp, ObjectField.Nom);
				var numéro      = ObjectCalculator.GetObjectPropertyString (this.obj, timestamp, ObjectField.Numéro);
				var responsable = ObjectCalculator.GetObjectPropertyString (this.obj, timestamp, ObjectField.Responsable);
				var couleur     = ObjectCalculator.GetObjectPropertyString (this.obj, timestamp, ObjectField.Couleur);
				var série       = ObjectCalculator.GetObjectPropertyString (this.obj, timestamp, ObjectField.NuméroSérie);
				var valeur1     = ObjectCalculator.GetObjectPropertyComputedAmount (this.obj, timestamp, ObjectField.Valeur1);
				var valeur2     = ObjectCalculator.GetObjectPropertyComputedAmount (this.obj, timestamp, ObjectField.Valeur2);
				var valeur3     = ObjectCalculator.GetObjectPropertyComputedAmount (this.obj, timestamp, ObjectField.Valeur3);

				var s1 = new TreeTableCellString (true, date, isSelected: (i == selection));
				var s2 = new TreeTableCellGlyph (true, glyph, isSelected: (i == selection));
				var s3 = new TreeTableCellString (true, type, isSelected: (i == selection));
				var s4 = new TreeTableCellComputedAmount (true, valeur1, isSelected: (i == selection));
				var s5 = new TreeTableCellComputedAmount (true, valeur2, isSelected: (i == selection));
				var s6 = new TreeTableCellString (true, responsable, isSelected: (i == selection));
				var s7 = new TreeTableCellString (true, couleur, isSelected: (i == selection));
				var s8 = new TreeTableCellString (true, série, isSelected: (i == selection));
				var s9 = new TreeTableCellString (true, nom, isSelected: (i == selection));
				var s10 = new TreeTableCellString (true, numéro, isSelected: (i == selection));

				c1.Add (s1);
				c2.Add (s2);
				c3.Add (s3);
				c4.Add (s4);
				c5.Add (s5);
				c6.Add (s6);
				c7.Add (s7);
				c8.Add (s8);
				c9.Add (s9);
				c10.Add (s10);
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
			this.controller.SetColumnCells (c++, c10.ToArray ());
		}


		protected override int DataCount
		{
			get
			{
				if (this.obj == null)
				{
					return 0;
				}
				else
				{
					return this.obj.EventsCount;
				}
			}
		}

		protected override void GetData(int row, out Guid guid, out int level)
		{
			var e = this.obj.GetEvent (row);

			if (e == null)
			{
				guid = Guid.Empty;
				level = 0;
			}
			else
			{
				guid = e.Guid;
				level = 0;
			}
		}


		private int TimestampToRow(Timestamp? timestamp)
		{
			if (timestamp.HasValue && this.obj != null)
			{
				int count = this.obj.EventsCount;
				for (int row = 0; row < count; row++)
				{
					var e = this.obj.GetEvent (row);

					if (e != null && e.Timestamp == timestamp.Value)
					{
						return row;
					}
				}
			}

			return -1;
		}

		private Timestamp? RowToTimestamp(int row)
		{
			var e = this.obj.GetEvent (row);

			if (e == null)
			{
				return null;
			}
			else
			{
				return e.Timestamp;
			}
		}


		#region Events handler
		private void OnStartEditing(EventType eventType)
		{
			if (this.StartEditing != null)
			{
				this.StartEditing (this, eventType);
			}
		}

		public delegate void StartEditingEventHandler(object sender, EventType eventType);
		public event StartEditingEventHandler StartEditing;


		private void OnUpdateAll()
		{
			if (this.UpdateAll != null)
			{
				this.UpdateAll (this);
			}
		}

		public delegate void UpdateAllEventHandler(object sender);
		public event UpdateAllEventHandler UpdateAll;
		#endregion

	
		private Guid							objectGuid;
		private DataObject						obj;
	}
}
