//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;

using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.App.Widgets;
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
				this.isEditing = true;
				this.Update ();
			};

			this.timelineController.CellClicked += delegate
			{
				this.UpdateAfterTimelineChanged ();
			};

			this.timelineController.StartEdition += delegate
			{
				this.isEditing = true;
				this.Update ();
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

					case ToolbarCommand.Edit:
						this.OnTreeTableEdit ();
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

					case ToolbarCommand.Now:
						this.OnTimelineNow ();
						break;

					case ToolbarCommand.Last:
						this.OnTimelineLast ();
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


		protected override string Title
		{
			get
			{
				return "Objets d'immobilisation";
			}
		}


		protected void OnTreeTableNew()
		{
		}

		protected void OnTreeTableDelete()
		{
		}

		protected void OnTreeTableEdit()
		{
			this.isEditing = true;
			this.Update ();
		}

		protected void OnTreeTableDeselect()
		{
			this.SelectedRow = -1;
			this.Update ();
		}

		protected void OnTimelineFirst()
		{
			var index = this.timelineController.FirstEventIndex;

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

		protected void OnTimelineLast()
		{
			var index = this.timelineController.LastEventIndex;

			if (index.HasValue)
			{
				this.timelineController.SelectedCell = index.Value;
			}
		}

		protected void OnTimelineNew()
		{
		}

		protected void OnTimelineDelete()
		{
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



		protected override void Update()
		{
			base.Update ();
			this.UpdateToolbars ();
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
		}

		private void UpdateAfterTimelineChanged()
		{
			var timestamp = this.timelineController.SelectedTimestamp;

			if (!timestamp.HasValue)
			{
				timestamp = new Timestamp (this.accessor.StartDate, 0);
			}

			this.treeTableController.Timestamp = timestamp.Value;

			this.UpdateToolbars ();
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
				this.treeTableToolbar.SetCommandState (ToolbarCommand.New,      ToolbarCommandState.Disable);
				this.treeTableToolbar.SetCommandState (ToolbarCommand.Delete,   ToolbarCommandState.Disable);
				this.treeTableToolbar.SetCommandState (ToolbarCommand.Edit,     ToolbarCommandState.Disable);
				this.treeTableToolbar.SetCommandState (ToolbarCommand.Deselect, ToolbarCommandState.Disable);
			}
			else
			{
				if (this.treeTableController.SelectedRow == -1)
				{
					this.treeTableToolbar.SetCommandState (ToolbarCommand.New,      ToolbarCommandState.Enable);
					this.treeTableToolbar.SetCommandState (ToolbarCommand.Delete,   ToolbarCommandState.Disable);
					this.treeTableToolbar.SetCommandState (ToolbarCommand.Edit,     ToolbarCommandState.Disable);
					this.treeTableToolbar.SetCommandState (ToolbarCommand.Deselect, ToolbarCommandState.Disable);
				}
				else
				{
					this.treeTableToolbar.SetCommandState (ToolbarCommand.New,      ToolbarCommandState.Enable);
					this.treeTableToolbar.SetCommandState (ToolbarCommand.Delete,   ToolbarCommandState.Enable);
					this.treeTableToolbar.SetCommandState (ToolbarCommand.Edit,     ToolbarCommandState.Enable);
					this.treeTableToolbar.SetCommandState (ToolbarCommand.Deselect, ToolbarCommandState.Enable);
				}
			}
		}

		private void UpdateTimelineToolbar()
		{
			int sel = this.timelineController.SelectedCell;

			if (sel == this.timelineController.FirstEventIndex.GetValueOrDefault (-999))
			{
				this.timelineToolbar.SetCommandState (ToolbarCommand.First, ToolbarCommandState.Disable);
			}
			else
			{
				this.timelineToolbar.SetCommandState (ToolbarCommand.First, ToolbarCommandState.Enable);
			}

			if (sel == this.timelineController.NowEventIndex.GetValueOrDefault (-999))
			{
				this.timelineToolbar.SetCommandState (ToolbarCommand.Now, ToolbarCommandState.Disable);
			}
			else
			{
				this.timelineToolbar.SetCommandState (ToolbarCommand.Now, ToolbarCommandState.Enable);
			}

			if (sel == this.timelineController.LastEventIndex.GetValueOrDefault (-999))
			{
				this.timelineToolbar.SetCommandState (ToolbarCommand.Last, ToolbarCommandState.Disable);
			}
			else
			{
				this.timelineToolbar.SetCommandState (ToolbarCommand.Last, ToolbarCommandState.Enable);
			}

			if (this.isEditing)
			{
				this.timelineToolbar.SetCommandState (ToolbarCommand.New,      ToolbarCommandState.Disable);
				this.timelineToolbar.SetCommandState (ToolbarCommand.Delete,   ToolbarCommandState.Disable);
				this.timelineToolbar.SetCommandState (ToolbarCommand.Deselect, ToolbarCommandState.Disable);
			}
			else
			{
				this.timelineToolbar.SetCommandState (ToolbarCommand.New, ToolbarCommandState.Enable);

				if (this.timelineController.HasSelectedEvent)
				{
					this.timelineToolbar.SetCommandState (ToolbarCommand.Delete, ToolbarCommandState.Enable);
				}
				else
				{
					this.timelineToolbar.SetCommandState (ToolbarCommand.Delete, ToolbarCommandState.Disable);
				}

				if (sel == -1)
				{
					this.timelineToolbar.SetCommandState (ToolbarCommand.Deselect, ToolbarCommandState.Disable);
				}
				else
				{
					this.timelineToolbar.SetCommandState (ToolbarCommand.Deselect, ToolbarCommandState.Enable);
				}
			}
		}


		private readonly ObjectsTreeTableController		treeTableController;
		private readonly ObjectsTimelineController		timelineController;

		private TreeTableToolbar				treeTableToolbar;
		private TimelineToolbar					timelineToolbar;
		private EditToolbar						editToolbar;

		private FrameBox						timelineFrameBox1;
		private FrameBox						timelineFrameBox2;
	}
}
