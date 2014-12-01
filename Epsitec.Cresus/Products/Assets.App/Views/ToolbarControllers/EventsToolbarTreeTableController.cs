//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Cresus.Assets.App.Helpers;
using Epsitec.Cresus.Assets.App.Popups;
using Epsitec.Cresus.Assets.App.Views.CommandToolbars;
using Epsitec.Cresus.Assets.Data;
using Epsitec.Cresus.Assets.Server.BusinessLogic;
using Epsitec.Cresus.Assets.Server.DataFillers;
using Epsitec.Cresus.Assets.Server.NodeGetters;
using Epsitec.Cresus.Assets.Server.SimpleEngine;
using Epsitec.Common.Drawing;

namespace Epsitec.Cresus.Assets.App.Views.ToolbarControllers
{
	public class EventsToolbarTreeTableController : AbstractToolbarBothTreesController<SortableNode>, IDirty, System.IDisposable
	{
		public EventsToolbarTreeTableController(DataAccessor accessor, CommandContext commandContext, BaseType baseType)
			: base(accessor, commandContext, baseType)
		{
			this.eventsNodeGetter = new ObjectEventsNodeGetter (this.accessor);
			this.nodeGetter = new SorterNodeGetter (this.eventsNodeGetter);

			this.title = Res.Strings.ToolbarControllers.EventsTreeTable.Title.ToString ();
		}

		public override void Dispose()
		{
			base.Dispose ();
		}


		#region IDirty Members
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


		protected override void CreateToolbar()
		{
			//	Crée la toolbar, en réservant de la place à droite pour le bouton de
			//	fermeture 'x' (closeButton).
			this.toolbar = new EventsToolbar (this.accessor, this.commandContext)
			{
				RightMargin = AbstractCommandToolbar.secondaryToolbarHeight,
			};

			this.ConnectSearch ();
		}

		protected override void CreateNodeFiller()
		{
			this.dataFiller = new EventsAssetsTreeTableFiller (this.accessor, this.nodeGetter);
			this.UpdateFillerTitle();

			TreeTableFiller<SortableNode>.FillColumns (this.treeTableController, this.dataFiller, "View.Events");

			this.sortingInstructions = TreeTableFiller<SortableNode>.GetSortingInstructions (this.treeTableController);
		}

		protected override void UpdateFillerTitle()
		{
			this.dataFiller.Title = this.FullTitle;
		}

		private string FullTitle
		{
			get
			{
				var list = new List<string> ();
				list.Add (this.title);

				if (!this.ObjectGuid.IsEmpty)
				{
					var name = AssetsLogic.GetSummary (this.accessor, this.ObjectGuid);
					list.Add (name);
				}

				return UniversalLogic.NiceJoin (list.ToArray ());
			}
		}


		[Command (Res.CommandIds.Events.First)]
		protected override void OnFirst()
		{
			base.OnFirst ();
		}

		[Command (Res.CommandIds.Events.Prev)]
		protected override void OnPrev()
		{
			base.OnPrev ();
		}

		[Command (Res.CommandIds.Events.Next)]
		protected override void OnNext()
		{
			base.OnNext ();
		}

		[Command (Res.CommandIds.Events.Last)]
		protected override void OnLast()
		{
			base.OnLast ();
		}

		[Command (Res.CommandIds.Events.Deselect)]
		protected void OnDeselect()
		{
			this.SelectedRow = -1;
		}

		[Command (Res.CommandIds.Events.New)]
		protected void OnNew(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			var target = this.toolbar.GetTarget (e);
			var timestamp = this.SelectedTimestamp;

			if (!timestamp.HasValue)
			{
				timestamp = Timestamp.Now;
			}

			if (target != null)
			{
				CreateEventPopup.Show (target, this.accessor, this.obj, timestamp.Value,
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

		[Command (Res.CommandIds.Events.Delete)]
		protected void OnDelete(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			var target = this.toolbar.GetTarget (e);

			if (AssetCalculator.IsLocked (this.obj, this.SelectedTimestamp.GetValueOrDefault ()))
			{
				MessagePopup.ShowAssetsDeleteEventWarning (target);
			}
			else
			{
				YesNoPopup.ShowAssetsDeleteEventQuestion (target, delegate
				{
					this.accessor.UndoManager.Start ();
					var desc = UndoManager.GetDescription (Res.Commands.Events.Delete.Description, AssetsLogic.GetSummary (this.accessor, this.obj.Guid, this.SelectedTimestamp));
					this.accessor.UndoManager.SetDescription (desc);

					this.accessor.RemoveObjectEvent (this.obj, this.SelectedTimestamp);
					this.UpdateData ();
					this.OnUpdateAfterDelete ();

					this.accessor.UndoManager.SetAfterViewState ();
				});
			}
		}

		[Command (Res.CommandIds.Events.Copy)]
		protected override void OnCopy(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			var target = this.toolbar.GetTarget (e);

			if (this.obj != null && this.SelectedTimestamp.HasValue)
			{
				var ev = this.obj.GetEvent (this.SelectedTimestamp.Value);
				this.accessor.Clipboard.CopyEvent (this.accessor, ev);

				this.UpdateToolbar ();
			}
			else
			{
				MessagePopup.ShowError (target, Res.Strings.ToolbarControllers.EventsTreeTable.CopyError.ToString ());
			}
		}

		[Command (Res.CommandIds.Events.Paste)]
		protected override void OnPaste(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			var target = this.toolbar.GetTarget (e);

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
					this.accessor.UndoManager.Start ();

					var ev = this.accessor.Clipboard.PasteEvent (this.accessor, this.obj, date);

					if (ev == null)
					{
						MessagePopup.ShowPasteError (target);
					}
					else
					{
						this.UpdateData ();
						this.SelectedTimestamp = ev.Timestamp;
						this.OnUpdateAfterCreate (ev.Guid, ev.Type, ev.Timestamp);
					}

					var desc = UndoManager.GetDescription (Res.Commands.Events.Paste.Description, AssetsLogic.GetSummary (this.accessor, this.obj.Guid, ev.Timestamp));
					this.accessor.UndoManager.SetDescription (desc);
					this.accessor.UndoManager.SetAfterViewState ();
				});
			}
			else
			{
				MessagePopup.ShowError (target, Res.Strings.ToolbarControllers.EventsTreeTable.PasteError.ToString ());
			}
		}

		[Command (Res.CommandIds.Events.Export)]
		protected override void OnExport(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			base.OnExport (dispatcher, e);
		}


		protected override void ShowContextMenu(Point pos)
		{
			//	Affiche le menu contextuel.
			MenuPopup.Show (this.toolbar, this.treeTableFrame, pos,
				Res.Commands.Events.New,
				Res.Commands.Events.Delete,
				null,
				Res.Commands.Events.Copy,
				Res.Commands.Events.Paste);
		}


		protected override void UpdateToolbar()
		{
			base.UpdateToolbar ();

			int row = this.VisibleSelectedRow;

			this.UpdateSelCommand (Res.Commands.Events.First, row, this.FirstRowIndex);
			this.UpdateSelCommand (Res.Commands.Events.Prev,  row, this.PrevRowIndex);
			this.UpdateSelCommand (Res.Commands.Events.Next,  row, this.NextRowIndex);
			this.UpdateSelCommand (Res.Commands.Events.Last,  row, this.LastRowIndex);

			this.toolbar.SetEnable (Res.Commands.Events.New,      true);
			this.toolbar.SetEnable (Res.Commands.Events.Delete,   row != -1);
			this.toolbar.SetEnable (Res.Commands.Events.Deselect, row != -1);

			this.toolbar.SetEnable (Res.Commands.Events.Copy,   this.IsCopyEnable);
			this.toolbar.SetEnable (Res.Commands.Events.Paste,  this.accessor.Clipboard.HasEvent);
			this.toolbar.SetEnable (Res.Commands.Events.Export, !this.IsEmpty);
		}

		private void CreateEvent(System.DateTime date, string buttonName)
		{
			if (this.obj != null)
			{
				this.accessor.UndoManager.Start ();

				var type = EventsToolbarTreeTableController.ParseEventType (buttonName);
				var e = this.accessor.CreateAssetEvent (this.obj, date, type);

				if (e != null)
				{
					this.UpdateData ();
					this.SelectedRow = this.TimestampToRow (e.Timestamp);
					this.OnUpdateAfterCreate (e.Guid, type, e.Timestamp);
				}

				var desc = UndoManager.GetDescription (Res.Commands.Events.New.Description, AssetsLogic.GetSummary (this.accessor, this.obj.Guid, e.Timestamp));
				this.accessor.UndoManager.SetDescription (desc);
				this.accessor.UndoManager.SetAfterViewState ();
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
				return this.obj.FindEventIndex (timestamp.Value);
			}

			return -1;
		}

		private Timestamp? RowToTimestamp(int row)
		{
			if (this.obj != null)
			{
				if (row >= 0 && row < this.obj.EventsCount)
				{
					return this.obj.Events[row].Timestamp;
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
