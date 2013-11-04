//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.App.DataFillers;
using Epsitec.Cresus.Assets.App.Popups;
using Epsitec.Cresus.Assets.App.Widgets;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.App.Views
{
	public class EventsToolbarTreeTableController : AbstractToolbarTreeTableController
	{
		public EventsToolbarTreeTableController(DataAccessor accessor, BaseType baseType)
			: base (accessor)
		{
			this.baseType = baseType;

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
					this.obj = this.accessor.GetObject (this.baseType, this.objectGuid);

					this.dataFiller.DataObject = this.obj;
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


		protected override void CreateNodeFiller()
		{
			var nodeFiller = new NodeFiller (this);

			switch (this.baseType)
			{
				case BaseType.Objects:
					this.dataFiller = new EventsObjectsDataFiller (this.accessor, this.baseType, this.controller, nodeFiller);
					break;

				case BaseType.Categories:
					this.dataFiller = new EventsCategoriesDataFiller (this.accessor, this.baseType, this.controller, nodeFiller);
					break;
			}

			base.CreateNodeFiller ();
		}

		private class NodeFiller : AbstractNodeFiller
		{
			public NodeFiller(EventsToolbarTreeTableController controller)
			{
				this.controller = controller;
			}

			public override int NodesCount
			{
				get
				{
					return this.controller.NodesCount;
				}
			}

			public override Node GetNode(int index)
			{
				return this.controller.GetNode (index);
			}

			private readonly EventsToolbarTreeTableController controller;
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
