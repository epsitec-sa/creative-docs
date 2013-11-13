//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.App.DataFillers;
using Epsitec.Cresus.Assets.App.Popups;
using Epsitec.Cresus.Assets.App.Widgets;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.App.Views
{
	public class EventsToolbarTreeTableController : AbstractToolbarTreeTableController<GuidNode>
	{
		public EventsToolbarTreeTableController(DataAccessor accessor, BaseType baseType)
			: base(accessor, baseType)
		{
			this.hasTreeOperations = false;

			this.nodesGetter = new ObjectEventsNodesGetter ();

			this.title = "Evénements";
		}


		public bool								DataFreezed;


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
					this.obj = this.accessor.GetObject (this.baseType, this.objectGuid);

					this.NodesGetter.DataObject = this.obj;
					this.dataFiller.DataObject = this.obj;

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
				this.SelectedRow = this.TimestampToRow (value);
			}
		}


		protected override void CreateNodeFiller()
		{
			switch (this.baseType)
			{
				case BaseType.Objects:
					this.dataFiller = new EventsObjectsTreeTableFiller (this.accessor, this.baseType, this.controller, this.nodesGetter);
					break;

				case BaseType.Categories:
					this.dataFiller = new EventsCategoriesTreeTableFiller (this.accessor, this.baseType, this.controller, this.nodesGetter);
					break;

				case BaseType.Groups:
					this.dataFiller = new EventsGroupsTreeTableFiller (this.accessor, this.baseType, this.controller, this.nodesGetter);
					break;
			}

			this.dataFiller.UpdateColumns ();

			this.UpdateController ();
			this.UpdateToolbar ();
		}


		protected override void OnDeselect()
		{
			this.SelectedRow = -1;
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
					BaseType   = this.baseType,
					DataObject = this.obj,
					Timestamp  = timestamp.Value,
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
						this.accessor.RemoveObjectEvent (this.obj, this.SelectedTimestamp);
						this.UpdateController ();
						this.UpdateToolbar ();
						this.OnUpdateAll ();
					}
				};
			}
		}


		public void Update()
		{
			this.UpdateController ();
			this.UpdateToolbar ();
		}


		private void CreateEvent(System.DateTime date, string buttonName)
		{
			if (this.obj != null)
			{
				var type = EventsToolbarTreeTableController.ParseEventType (buttonName);
				var e = this.accessor.CreateObjectEvent (this.obj, date, type);

				if (e != null)
				{
					this.UpdateController ();
					this.UpdateToolbar ();

					this.SelectedRow = this.TimestampToRow (e.Timestamp);
				}

				this.OnStartEditing (type, e.Timestamp);
				this.OnUpdateAll ();
			}
		}

		private static EventType ParseEventType(string text)
		{
			var type = EventType.Unknown;
			System.Enum.TryParse<EventType> (text, out type);
			return type;
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


		private ObjectEventsNodesGetter NodesGetter
		{
			get
			{
				return this.nodesGetter as ObjectEventsNodesGetter;
			}
		}


		private Guid							objectGuid;
		private DataObject						obj;
	}
}
