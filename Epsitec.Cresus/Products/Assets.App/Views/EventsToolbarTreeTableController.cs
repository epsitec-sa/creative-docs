//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.App.Helpers;
using Epsitec.Cresus.Assets.App.Popups;
using Epsitec.Cresus.Assets.Data;
using Epsitec.Cresus.Assets.Server.BusinessLogic;
using Epsitec.Cresus.Assets.Server.DataFillers;
using Epsitec.Cresus.Assets.Server.NodeGetters;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.App.Views
{
	public class EventsToolbarTreeTableController : AbstractToolbarBothTreesController<SortableNode>, IDirty
	{
		public EventsToolbarTreeTableController(DataAccessor accessor, BaseType baseType)
			: base(accessor, baseType)
		{
			this.hasFilter         = false;
			this.hasTreeOperations = false;
			this.hasMoveOperations = false;

			this.NewCustomization      = new CommandCustomization ("TreeTable.New.Event",   "Nouvel événement");
			this.DeleteCustomization   = new CommandCustomization (null,                    "Supprimer l'événement");
			this.DeselectCustomization = new CommandCustomization (null,                    "Désélectionner l'événement");
			this.CopyCustomization     = new CommandCustomization ("TreeTable.Copy.Event",  "Copier l'événement");
			this.PasteCustomization    = new CommandCustomization ("TreeTable.Paste.Event", "Coller l'événement");
			this.ExportCustomization   = new CommandCustomization (null,                    "Exporter les événements");

			this.eventsNodeGetter = new ObjectEventsNodeGetter ();
			this.nodeGetter = new SorterNodeGetter (this.eventsNodeGetter);

			this.title = "Evénements";
		}


		#region IDirty Members
		public bool InUse
		{
			get;
			set;
		}

		public bool DirtyData
		{
			get;
			set;
		}
		#endregion

	
		public override void UpdateData()
		{
			this.eventsNodeGetter.SetParams (this.obj, this.sortingInstructions);
			this.NodeGetter.SetParams (this.sortingInstructions);

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
					this.obj = this.accessor.GetObject (BaseType.Assets, this.objectGuid);

					this.dataFiller.DataObject = this.obj;

					this.UpdateData ();
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
			this.dataFiller = new EventsAssetsTreeTableFiller (this.accessor, this.nodeGetter);
			//?this.dataFiller = new AmortizationsObjectsTreeTableFiller (this.accessor, this.nodeGetter);
			TreeTableFiller<SortableNode>.FillColumns (this.treeTableController, this.dataFiller, "View.Events");

			this.sortingInstructions = TreeTableFiller<SortableNode>.GetSortingInstructions (this.treeTableController);
		}


		protected override void OnDeselect()
		{
			this.SelectedRow = -1;
		}

		protected override void OnNew()
		{
			var target = this.toolbar.GetTarget (ToolbarCommand.New);
			var timestamp = this.SelectedTimestamp;

			if (!timestamp.HasValue)
			{
				timestamp = Timestamp.Now;
			}

			if (target != null)
			{
				NewEventPopup.Show (target, this.accessor, this.baseType, this.obj, timestamp.Value,
				timestampChanged: delegate (Timestamp? t)
				{
					this.SelectedTimestamp = t;
				},
				action: delegate (System.DateTime date, string name)
				{
					this.CreateEvent (date, name);
				});
			}
		}

		protected override void OnDelete()
		{
			var target = this.toolbar.GetTarget (ToolbarCommand.Delete);

			if (AssetCalculator.IsLocked (this.obj, this.SelectedTimestamp.GetValueOrDefault ()))
			{
				MessagePopup.ShowAssetsDeleteEventWarning (target);
			}
			else
			{
				YesNoPopup.ShowAssetsDeleteEventQuestion (target, delegate
				{
					this.accessor.RemoveObjectEvent (this.obj, this.SelectedTimestamp);
					this.UpdateData ();
					this.OnUpdateAfterDelete ();
				});
			}
		}

		protected override void OnCopy()
		{
			var target = this.toolbar.GetTarget (ToolbarCommand.Copy);

			if (this.obj != null && this.SelectedTimestamp.HasValue)
			{
				var e = this.obj.GetEvent (this.SelectedTimestamp.Value);
				this.accessor.Clipboard.CopyEvent (this.accessor, e);

				this.UpdateToolbar ();
			}
			else
			{
				MessagePopup.ShowError (target, "La copie est impossible, car aucun événement n'est sélectionné.");
			}
		}

		protected override void OnPaste()
		{
			var target = this.toolbar.GetTarget (ToolbarCommand.Paste);

			if (this.obj != null && this.accessor.Clipboard.HasEvent)
			{
				EventPastePopup.Show (target, this.accessor, this.obj,
				this.accessor.Clipboard.EventType,
				this.accessor.Clipboard.EventTimestamp.Value.Date,
				dateChanged: delegate (System.DateTime? date)
				{
					// Ne surtout pas mettre à jour this.SelectedTimestamp !
				},
				action: delegate (System.DateTime date)
				{
					var e = this.accessor.Clipboard.PasteEvent (this.accessor, this.obj, date);

					if (e == null)
					{
						MessagePopup.ShowPasteError (target);
					}
					else
					{
						this.UpdateData ();
						this.SelectedTimestamp = e.Timestamp;
						this.OnUpdateAfterCreate (e.Guid, e.Type, e.Timestamp);
					}
				});
			}
			else
			{
				MessagePopup.ShowError (target, "Aucun événement ne peut être collé, car le bloc-notes est vide.");
			}
		}

		protected override void UpdateToolbar()
		{
			base.UpdateToolbar ();
			this.toolbar.SetCommandEnable (ToolbarCommand.Paste, this.accessor.Clipboard.HasEvent);
		}

		private void CreateEvent(System.DateTime date, string buttonName)
		{
			if (this.obj != null)
			{
				var type = EventsToolbarTreeTableController.ParseEventType (buttonName);
				var e = this.accessor.CreateAssetEvent (this.obj, date, type);

				if (e != null)
				{
					this.UpdateData ();
					this.SelectedRow = this.TimestampToRow (e.Timestamp);
					this.OnUpdateAfterCreate (e.Guid, type, e.Timestamp);
				}
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
			if (this.obj != null)
			{
				var e = this.obj.GetEvent (row);

				if (e != null)
				{
					return e.Timestamp;
				}
			}

			return null;
		}


		private SorterNodeGetter NodeGetter
		{
			get
			{
				return this.nodeGetter as SorterNodeGetter;
			}
		}


		private readonly ObjectEventsNodeGetter		eventsNodeGetter;

		private Guid								objectGuid;
		private DataObject							obj;
	}
}
