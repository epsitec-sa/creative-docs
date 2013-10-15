//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.App.Popups;
using Epsitec.Cresus.Assets.Server.NaiveEngine;

namespace Epsitec.Cresus.Assets.App.Views
{
	public class ObjectsView : AbstractView
	{
		public ObjectsView(DataAccessor accessor)
			: base (accessor)
		{
			this.treeTableController = new ObjectsTreeTableController (this.accessor);
			this.timelineController = new ObjectsTimelineController (this.accessor);
			this.objectEditor = new ObjectEditor (this.accessor);
		}

		public override void CreateUI(Widget parent, MainToolbar toolbar)
		{
			base.CreateUI (parent, toolbar);

			this.timelineFrameBox2 = new FrameBox
			{
				Parent = parent,
				Dock   = DockStyle.Bottom,
			};

			this.timelineFrameBox1 = new FrameBox
			{
				Parent = parent,
				Dock   = DockStyle.Bottom,
			};

			this.treeTableController.CreateUI (this.listFrameBox);
			this.timelineController.CreateUI (this.timelineFrameBox2);
			this.objectEditor.CreateUI (this.editFrameBox);

			this.treeTableToolbar = new TreeTableToolbar ();
			this.treeTableToolbar.CreateUI (this.listFrameBox);

			this.timelineToolbar = new TimelineToolbar ();
			this.timelineToolbar.CreateUI (this.timelineFrameBox1);
			this.timelineToolbar.TimelineMode = this.timelineController.TimelineMode;

			this.editToolbar = new EditToolbar ();
			this.editToolbar.CreateUI (this.editFrameBox);

			this.Update ();

			// provisoire:
			this.editToolbar.SetCommandState (ToolbarCommand.Accept, ToolbarCommandState.Disable);
			this.editToolbar.SetCommandState (ToolbarCommand.Cancel, ToolbarCommandState.Enable);

			//	Connexion des événements.
			this.treeTableController.RowClicked += delegate
			{
				this.UpdateAfterTreeTableChanged ();
			};

			this.treeTableController.StartEdition += delegate
			{
				this.OnMainEdit ();
			};

			this.timelineController.CellClicked += delegate
			{
				this.UpdateAfterTimelineChanged ();
			};

			this.timelineController.StartEdition += delegate
			{
				this.OnMainEdit ();
			};

			this.mainToolbar.CommandClicked += delegate (object sender, ToolbarCommand command)
			{
				switch (command)
				{
					case ToolbarCommand.Edit:
						this.OnMainEdit ();
						break;

					case ToolbarCommand.Amortissement:
						this.OnMainAmortissement ();
						break;

					case ToolbarCommand.Simulation:
						this.OnMainSimulation ();
						break;
				}
			};

			this.treeTableToolbar.CommandClicked += delegate (object sender, ToolbarCommand command)
			{
				switch (command)
				{
					case ToolbarCommand.New:
						this.OnTreeTableNew ();
						break;

					case ToolbarCommand.Delete:
						this.OnTreeTableDelete ();
						break;

					case ToolbarCommand.Deselect:
						this.OnTreeTableDeselect ();
						break;
				}
			};

			this.timelineToolbar.CommandClicked += delegate (object sender, ToolbarCommand command)
			{
				switch (command)
				{
					case ToolbarCommand.First:
						this.OnTimelineFirst ();
						break;

					case ToolbarCommand.Last:
						this.OnTimelineLast ();
						break;

					case ToolbarCommand.Prev:
						this.OnTimelinePrev ();
						break;

					case ToolbarCommand.Next:
						this.OnTimelineNext ();
						break;

					case ToolbarCommand.Now:
						this.OnTimelineNow ();
						break;

					case ToolbarCommand.New:
						this.OnTimelineNew ();
						break;

					case ToolbarCommand.Delete:
						this.OnTimelineDelete ();
						break;

					case ToolbarCommand.Deselect:
						this.OnTimelineDeselect ();
						break;
				}
			};

			this.editToolbar.CommandClicked += delegate (object sender, ToolbarCommand command)
			{
				switch (command)
				{
					case ToolbarCommand.Accept:
						this.OnEditAccept ();
						break;

					case ToolbarCommand.Cancel:
						this.OnEditCancel ();
						break;
				}
			};

			this.timelineToolbar.ModeChanged += delegate
			{
				this.timelineController.TimelineMode = this.timelineToolbar.TimelineMode;
			};
		}


		protected void OnMainEdit()
		{
			this.isEditing = true;
			this.Update ();
		}

		protected void OnMainAmortissement()
		{
			var target = this.mainToolbar.GetCommandWidget (ToolbarCommand.Amortissement);

			if (target != null)
			{
				var popup = new DeletePopup
				{
					Question = "Voulez-vous générer les amortissements ?",
				};

				popup.Create (target);
			}
		}

		protected void OnMainSimulation()
		{
			var target = this.mainToolbar.GetCommandWidget (ToolbarCommand.Simulation);

			if (target != null)
			{
				var popup = new DeletePopup
				{
					Question = "Voulez-vous débuter une simulation ?",
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

		protected void OnTreeTableNew()
		{
		}

		protected void OnTreeTableDelete()
		{
			var target = this.treeTableToolbar.GetCommandWidget (ToolbarCommand.Delete);

			if (target != null)
			{
				var popup = new DeletePopup
				{
					Question = "Voulez-vous supprimer l'objet sélectionné ?",
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

		protected void OnTreeTableDeselect()
		{
			this.treeTableController.SelectedRow = -1;
		}

		protected void OnTimelineFirst()
		{
			var index = this.timelineController.FirstEventIndex;

			if (index.HasValue)
			{
				this.timelineController.SelectedCell = index.Value;
			}
		}

		protected void OnTimelinePrev()
		{
			var index = this.timelineController.PrevEventIndex;

			if (index.HasValue)
			{
				this.timelineController.SelectedCell = index.Value;
			}
		}

		protected void OnTimelineNext()
		{
			var index = this.timelineController.NextEventIndex;

			if (index.HasValue)
			{
				this.timelineController.SelectedCell = index.Value;
			}
		}

		protected void OnTimelineLast()
		{
			var index = this.timelineController.LastEventIndex;

			if (index.HasValue)
			{
				this.timelineController.SelectedCell = index.Value;
			}
		}

		protected void OnTimelineNow()
		{
			var index = this.timelineController.NowEventIndex;

			if (index.HasValue)
			{
				this.timelineController.SelectedCell = index.Value;
			}
		}

		protected void OnTimelineNew()
		{
			var target = this.timelineToolbar.GetCommandWidget (ToolbarCommand.New);
			var timestamp = this.timelineController.SelectedTimestamp;

			if (target != null && timestamp.HasValue)
			{
				var popup = new NewEventPopup
				{
					Date = timestamp.Value.Date,
				};

				popup.Create (target);

				popup.DateChanged += delegate (object sender, System.DateTime? dateTime)
				{
					var index = this.timelineController.GetEventIndex (dateTime);

					if (index.HasValue)
					{
						this.timelineController.SelectedCell = index.Value;
					}
				};

				popup.ButtonClicked += delegate (object sender, string name)
				{
					this.CreateEvent (timestamp.Value.Date, name);
				};
			}
		}

		protected void OnTimelineDelete()
		{
			var target = this.timelineToolbar.GetCommandWidget (ToolbarCommand.Delete);

			if (target != null)
			{
				var popup = new DeletePopup
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

		protected void OnTimelineDeselect()
		{
			this.timelineController.SelectedCell = -1;
		}

		protected void OnEditAccept()
		{
			this.isEditing = false;
			this.Update ();
		}

		protected void OnEditCancel()
		{
			this.isEditing = false;
			this.Update ();
		}


		private void CreateEvent(System.DateTime date, string buttonName)
		{
		}


		protected override void Update()
		{
			base.Update ();
			this.UpdateToolbars ();
			this.UpdateEditor ();
		}

		private void UpdateAfterTreeTableChanged()
		{
			int row = this.treeTableController.SelectedRow;

			if (row == -1)
			{
				this.timelineController.ObjectGuid = Guid.Empty;
			}
			else
			{
				this.timelineController.ObjectGuid = this.accessor.GetObjectGuid (row);
			}

			this.UpdateToolbars ();
			this.UpdateEditor ();
		}

		private void UpdateAfterTimelineChanged()
		{
			var timestamp = this.timelineController.SelectedTimestamp;

			if (!timestamp.HasValue)
			{
				timestamp = new Timestamp (System.DateTime.MaxValue, 0);
			}

			this.treeTableController.Timestamp = timestamp.Value;

			this.UpdateToolbars ();
			this.UpdateEditor ();
		}

		private void UpdateEditor()
		{
			var guid = this.accessor.GetObjectGuid (this.treeTableController.SelectedRow);
			var timestamp = this.timelineController.SelectedTimestamp;

			this.objectEditor.SetObject (guid, timestamp);
		}

		private void UpdateToolbars()
		{
			this.UpdateTreeTableToolbar ();
			this.UpdateTimelineToolbar ();
		}

		private void UpdateTreeTableToolbar()
		{
			if (this.isEditing)
			{
				this.mainToolbar.SetCommandState (ToolbarCommand.Edit,     ToolbarCommandState.Activate);

				this.treeTableToolbar.SetCommandState (ToolbarCommand.New,      ToolbarCommandState.Disable);
				this.treeTableToolbar.SetCommandState (ToolbarCommand.Delete,   ToolbarCommandState.Disable);
				this.treeTableToolbar.SetCommandState (ToolbarCommand.Deselect, ToolbarCommandState.Disable);
			}
			else
			{
				if (this.treeTableController.SelectedRow == -1)
				{
					this.mainToolbar.SetCommandState (ToolbarCommand.Edit, ToolbarCommandState.Disable);

					this.treeTableToolbar.SetCommandState (ToolbarCommand.New,      ToolbarCommandState.Enable);
					this.treeTableToolbar.SetCommandState (ToolbarCommand.Delete,   ToolbarCommandState.Disable);
					this.treeTableToolbar.SetCommandState (ToolbarCommand.Deselect, ToolbarCommandState.Disable);
				}
				else
				{
					this.mainToolbar.SetCommandState (ToolbarCommand.Edit, ToolbarCommandState.Enable);

					this.treeTableToolbar.SetCommandState (ToolbarCommand.New,      ToolbarCommandState.Enable);
					this.treeTableToolbar.SetCommandState (ToolbarCommand.Delete,   ToolbarCommandState.Enable);
					this.treeTableToolbar.SetCommandState (ToolbarCommand.Deselect, ToolbarCommandState.Enable);
				}
			}
		}

		private void UpdateTimelineToolbar()
		{
			int sel = this.timelineController.SelectedCell;

			this.UpdateTimelineCommand (ToolbarCommand.First, sel, this.timelineController.FirstEventIndex);
			this.UpdateTimelineCommand (ToolbarCommand.Prev,  sel, this.timelineController.PrevEventIndex);
			this.UpdateTimelineCommand (ToolbarCommand.Next,  sel, this.timelineController.NextEventIndex);
			this.UpdateTimelineCommand (ToolbarCommand.Last,  sel, this.timelineController.LastEventIndex);
			this.UpdateTimelineCommand (ToolbarCommand.Now,   sel, this.timelineController.NowEventIndex);
			
			if (this.isEditing)
			{
				this.timelineToolbar.SetCommandState (ToolbarCommand.New,    ToolbarCommandState.Disable);
				this.timelineToolbar.SetCommandState (ToolbarCommand.Delete, ToolbarCommandState.Disable);
			}
			else
			{
				var guid = this.accessor.GetObjectGuid (this.treeTableController.SelectedRow);

				this.UpdateTimelineCommand (ToolbarCommand.New,    !guid.IsEmpty && this.timelineController.SelectedTimestamp.HasValue);
				this.UpdateTimelineCommand (ToolbarCommand.Delete, this.timelineController.HasSelectedEvent);
			}

			this.UpdateTimelineCommand (ToolbarCommand.Deselect, sel != -1);
		}

		private void UpdateTimelineCommand(ToolbarCommand command, int selectedCell, int? newSelection)
		{
			bool enable = (newSelection.HasValue && selectedCell != newSelection.Value);
			this.UpdateTimelineCommand (command, enable);
		}

		private void UpdateTimelineCommand(ToolbarCommand command, bool enable)
		{
			if (enable)
			{
				this.timelineToolbar.SetCommandState (command, ToolbarCommandState.Enable);
			}
			else
			{
				this.timelineToolbar.SetCommandState (command, ToolbarCommandState.Disable);
			}
		}


		private readonly ObjectsTreeTableController		treeTableController;
		private readonly ObjectsTimelineController		timelineController;
		private readonly ObjectEditor					objectEditor;

		private TreeTableToolbar						treeTableToolbar;
		private TimelineToolbar							timelineToolbar;
		private EditToolbar								editToolbar;

		private FrameBox								timelineFrameBox1;
		private FrameBox								timelineFrameBox2;
	}
}
