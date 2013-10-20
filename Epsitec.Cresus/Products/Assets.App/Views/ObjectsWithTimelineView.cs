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
	public class ObjectsWithTimelineView : AbstractObjectsView
	{
		public ObjectsWithTimelineView(DataAccessor accessor, MainToolbar toolbar)
			: base (accessor, toolbar)
		{
			this.treeTableController = new ObjectsTreeTableController (this.accessor);
			this.timelineController  = new ObjectsTimelineController (this.accessor);
			this.objectEditor        = new ObjectEditor (this.accessor);

			this.objectEditor.Navigate += delegate (object sender, Timestamp timestamp)
			{
				var index = this.timelineController.GetEventIndex (timestamp);

				if (index.HasValue)
				{
					this.timelineController.SelectedCell = index.Value;
				}
			};
		}


		public override void Dispose()
		{
		}


		public override Guid ObjectGuid
		{
			get
			{
				return this.treeTableController.SelectedGuid;
			}
			set
			{
				this.treeTableController.SelectedGuid = value;
			}
		}

		public override Timestamp? Timestamp
		{
			get
			{
				return this.timelineController.SelectedTimestamp;
			}
			set
			{
				this.timelineController.SelectedTimestamp = value;
			}
		}

	
		public override void CreateUI(Widget parent)
		{
			var topBox = new FrameBox
			{
				Parent  = parent,
				Dock    = DockStyle.Fill,
				Margins = new Margins (0, 0, 0, 10),
			};

			this.listFrameBox = new FrameBox
			{
				Parent = topBox,
				Dock   = DockStyle.Fill,
			};

			this.editFrameBox = new FrameBox
			{
				Parent         = topBox,
				Dock           = DockStyle.Right,
				PreferredWidth = 600,
				Margins        = new Margins (10, 0, 0, 0),
				BackColor      = ColorManager.GetBackgroundColor (),
			};

			this.timelineFrameBox = new FrameBox
			{
				Parent = parent,
				Dock   = DockStyle.Bottom,
			};

			this.treeTableController.CreateUI (this.listFrameBox);
			this.timelineController.CreateUI (this.timelineFrameBox);
			this.objectEditor.CreateUI (this.editFrameBox);

			this.swapViewButton = new IconButton
			{
				Parent        = this.timelineFrameBox,
				AutoFocus     = false,
				IconUri       = AbstractCommandToolbar.GetResourceIconUri ("View.WithoutTimeline"),
				Anchor        = AnchorStyles.TopRight,
				PreferredSize = new Size (AbstractCommandToolbar.SecondaryToolbarHeight, AbstractCommandToolbar.SecondaryToolbarHeight),
			};
			ToolTip.Default.SetToolTip (this.swapViewButton, "Cache l'axe du temps");

			this.Update ();

			//	Connexion des événements.
			this.swapViewButton.Clicked += delegate
			{
				this.OnViewChanged ();
			};

			this.treeTableController.SelectedRowChanged += delegate
			{
				this.UpdateAfterTreeTableChanged ();
			};

			this.treeTableController.RowDoubleClicked += delegate
			{
				this.OnMainEdit ();
			};

			this.timelineController.SelectedCellChanged += delegate
			{
				this.UpdateAfterTimelineChanged ();
			};

			this.timelineController.StartEdition += delegate
			{
				this.OnMainEdit ();
			};
		}

		public override void OnCommand(ToolbarCommand command)
		{
			base.OnCommand (command);

			switch (command)
			{
				case ToolbarCommand.Edit:
					this.OnMainEdit ();
					break;

				case ToolbarCommand.Accept:
					this.OnEditAccept ();
					break;

				case ToolbarCommand.Cancel:
					this.OnEditCancel ();
					break;
			}
		}


		private void OnMainEdit()
		{
			this.isEditing = !this.isEditing;
			this.Update ();
		}

		private void OnEditAccept()
		{
			this.isEditing = false;
			this.Update ();
		}

		private void OnEditCancel()
		{
			this.isEditing = false;
			this.Update ();
		}



		public override void Update()
		{
			this.editFrameBox.Visibility = this.isEditing;

			this.UpdateToolbars ();
			this.UpdateEditor ();
		}

		private void UpdateAfterTreeTableChanged()
		{
			int row = this.treeTableController.SelectedRow;

			if (row == -1)
			{
				this.timelineController.ObjectGuid = Guid.Empty;

				if (this.isEditing)
				{
					this.isEditing = false;
					this.Update ();
				}
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
			this.UpdateMainToolbar ();
		}

		private void UpdateMainToolbar()
		{
			if (this.isEditing)
			{
				this.mainToolbar.SetCommandState (ToolbarCommand.Edit, ToolbarCommandState.Activate);

				this.mainToolbar.SetCommandState (ToolbarCommand.Accept, ToolbarCommandState.Enable);
				this.mainToolbar.SetCommandState (ToolbarCommand.Cancel, ToolbarCommandState.Enable);
			}
			else
			{
				if (this.treeTableController.SelectedRow == -1)
				{
					this.mainToolbar.SetCommandState (ToolbarCommand.Edit, ToolbarCommandState.Disable);
				}
				else
				{
					this.mainToolbar.SetCommandState (ToolbarCommand.Edit, ToolbarCommandState.Enable);
				}

				this.mainToolbar.SetCommandState (ToolbarCommand.Accept, ToolbarCommandState.Hide);
				this.mainToolbar.SetCommandState (ToolbarCommand.Cancel, ToolbarCommandState.Hide);
			}
		}


		private readonly ObjectsTreeTableController		treeTableController;
		private readonly ObjectsTimelineController		timelineController;
		private readonly ObjectEditor					objectEditor;

		private FrameBox								listFrameBox;
		private FrameBox								editFrameBox;
		private FrameBox								timelineFrameBox;

		private IconButton								swapViewButton;

		private bool									isEditing;
	}
}
