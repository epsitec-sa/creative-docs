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
	public class ObjectsWithoutTimelineView : AbstractObjectsView
	{
		public ObjectsWithoutTimelineView(DataAccessor accessor, MainToolbar toolbar)
			: base (accessor, toolbar)
		{
			this.objectsController = new ObjectsToolbarTreeTableController (this.accessor);
			this.eventsController  = new EventsToolbarTreeTableController (this.accessor);
			this.objectEditor      = new ObjectEditor (this.accessor);

			this.objectEditor.Navigate += delegate (object sender, Timestamp timestamp)
			{
				this.eventsController.SelectedTimestamp = timestamp;
			};
		}


		public override void Dispose()
		{
		}


		public override Guid ObjectGuid
		{
			get
			{
				return this.objectsController.SelectedGuid;
			}
			set
			{
				this.objectsController.SelectedGuid = value;
			}
		}

		public override Timestamp? Timestamp
		{
			get
			{
				return this.eventsController.SelectedTimestamp;
			}
			set
			{
				this.eventsController.SelectedTimestamp = value;

				this.openedColumnsCount = value.HasValue ? 2 : 1;
				this.Update ();
			}
		}


		public override void CreateUI(Widget parent)
		{
			this.objectsFrameBox = new FrameBox
			{
				Parent         = parent,
				Dock           = DockStyle.Left,
			};

			this.eventsFrameBox = new FrameBox
			{
				Parent         = parent,
				Dock           = DockStyle.Left,
				BackColor      = ColorManager.GetBackgroundColor (),
			};

			this.editFrameBox = new FrameBox
			{
				Parent         = parent,
				Dock           = DockStyle.Left,
				BackColor      = ColorManager.GetBackgroundColor (),
			};

			this.objectsController.CreateUI (this.objectsFrameBox);
			this.eventsController.CreateUI (this.eventsFrameBox);
			this.objectEditor.CreateUI (this.editFrameBox);

			this.closeButton = new GlyphButton
			{
				Parent        = parent,
				GlyphShape    = GlyphShape.Close,
				ButtonStyle   = ButtonStyle.ToolItem,
				Anchor        = AnchorStyles.TopRight,
				PreferredSize = new Size (TopTitle.Height, TopTitle.Height),
			};

			this.swapViewButton = new IconButton
			{
				Parent        = parent,
				AutoFocus     = false,
				IconUri       = AbstractCommandToolbar.GetResourceIconUri ("View.WithTimeline"),
				Anchor        = AnchorStyles.BottomRight,
				PreferredSize = new Size (AbstractCommandToolbar.SecondaryToolbarHeight, AbstractCommandToolbar.SecondaryToolbarHeight),
			};
			ToolTip.Default.SetToolTip (this.swapViewButton, "Montre l'axe du temps");

			this.openedColumnsCount = 1;
			this.Update ();

			//	Connexion des événements.
			this.swapViewButton.Clicked += delegate
			{
				this.OnViewChanged ();
			};

			this.objectsController.SelectedRowChanged += delegate
			{
				this.UpdateAfterObjectsChanged ();
			};

			this.objectsController.RowDoubleClicked += delegate
			{
				this.OnObjectDoubleClicked ();
			};

			this.eventsController.SelectedRowChanged += delegate
			{
				this.UpdateAfterEventsChanged ();
			};

			this.eventsController.RowDoubleClicked += delegate
			{
				this.OnEventDoubleClicked ();
			};

			this.closeButton.Clicked += delegate
			{
				this.CloseColumn ();
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
			if (this.openedColumnsCount == 2)
			{
				this.openedColumnsCount = 3;  // active l'édition
			}
			else
			{
				this.openedColumnsCount = 2;  // plus d'édition
			}

			this.Update ();
		}

		private void OnEditAccept()
		{
		}

		private void OnEditCancel()
		{
		}

		private void OnObjectDoubleClicked()
		{
			this.openedColumnsCount = 2;
			this.Update ();
		}

		private void OnEventDoubleClicked()
		{
			this.objectEditor.SetObject (this.eventsController.ObjectGuid, this.eventsController.SelectedTimestamp);
			this.openedColumnsCount = 3;
			this.Update ();
		}

		private void CloseColumn()
		{
			this.openedColumnsCount--;
			this.Update ();
		}


		public override void Update()
		{
			this.UpdateOpenedColumns ();
			this.UpdateToolbars ();
		}

		private void UpdateOpenedColumns()
		{
			if (this.openedColumnsCount == 1)
			{
				this.objectsFrameBox.Visibility = true;
				this.eventsFrameBox.Visibility = false;
				this.editFrameBox.Visibility = false;

				this.objectsFrameBox.Dock = DockStyle.Fill;
				this.objectsFrameBox.Margins = new Margins (0, 0, 0, AbstractCommandToolbar.SecondaryToolbarHeight);
			}
			else if (this.openedColumnsCount == 2)
			{
				this.objectsFrameBox.Visibility = true;
				this.eventsFrameBox.Visibility = true;
				this.editFrameBox.Visibility = false;

				this.objectsFrameBox.Dock = DockStyle.Left;
				this.objectsFrameBox.PreferredWidth = 190;
				this.objectsFrameBox.Margins = new Margins (0, 0, 0, AbstractCommandToolbar.SecondaryToolbarHeight);

				this.eventsFrameBox.Dock = DockStyle.Fill;
				this.eventsFrameBox.Margins = new Margins (10, 0, 0, AbstractCommandToolbar.SecondaryToolbarHeight);
			}
			else if (this.openedColumnsCount == 3)
			{
				this.objectsFrameBox.Visibility = false;
				this.eventsFrameBox.Visibility = true;
				this.editFrameBox.Visibility = true;

				this.eventsFrameBox.Dock = DockStyle.Fill;
				this.eventsFrameBox.Margins = new Margins (0, 0, 0, AbstractCommandToolbar.SecondaryToolbarHeight);

				this.editFrameBox.Dock = DockStyle.Right;
				this.editFrameBox.PreferredWidth = 600;
				this.editFrameBox.Margins = new Margins (10, 0, 0, AbstractCommandToolbar.SecondaryToolbarHeight);
			}
			else
			{
				this.objectsFrameBox.Visibility = true;
				this.eventsFrameBox.Visibility = true;
				this.editFrameBox.Visibility = true;

				this.objectsFrameBox.Dock = DockStyle.Left;
				this.objectsFrameBox.PreferredWidth = 190;
				this.objectsFrameBox.Margins = new Margins (0, 0, 0, AbstractCommandToolbar.SecondaryToolbarHeight);

				this.eventsFrameBox.Dock = DockStyle.Fill;
				this.eventsFrameBox.Margins = new Margins (10, 0, 0, AbstractCommandToolbar.SecondaryToolbarHeight);

				this.editFrameBox.Dock = DockStyle.Right;
				this.editFrameBox.PreferredWidth = 600;
				this.editFrameBox.Margins = new Margins (10, 0, 0, AbstractCommandToolbar.SecondaryToolbarHeight);
			}

			this.closeButton.Visibility = this.openedColumnsCount > 1;
		}

		private void UpdateAfterObjectsChanged()
		{
			int row = this.objectsController.SelectedRow;

			if (row == -1)
			{
				this.eventsController.ObjectGuid = Guid.Empty;

				if (this.openedColumnsCount > 1)
				{
					this.openedColumnsCount = 1;
					this.Update ();
				}
			}
			else
			{
				this.eventsController.ObjectGuid = this.accessor.GetObjectGuid (row);
				this.eventsController.SelectedTimestamp = this.selectedTimestamp;
			}

			this.UpdateToolbars ();
		}

		private void UpdateAfterEventsChanged()
		{
			this.selectedTimestamp = this.eventsController.SelectedTimestamp;
			this.objectEditor.SetObject (this.eventsController.ObjectGuid, this.eventsController.SelectedTimestamp);

			if (!this.selectedTimestamp.HasValue)
			{
				if (this.openedColumnsCount == 3)  // édition en cours ?
				{
					this.openedColumnsCount = 2;
					this.Update ();
				}
			}

			this.UpdateToolbars ();
		}

		private void UpdateToolbars()
		{
			this.UpdateMainToolbar ();
		}

		private void UpdateMainToolbar()
		{
			if (this.openedColumnsCount == 2)
			{
				this.mainToolbar.SetCommandState (ToolbarCommand.Edit, ToolbarCommandState.Enable);
			}
			else if (this.openedColumnsCount == 3)
			{
				this.mainToolbar.SetCommandState (ToolbarCommand.Edit, ToolbarCommandState.Activate);
			}
			else
			{
				this.mainToolbar.SetCommandState (ToolbarCommand.Edit, ToolbarCommandState.Disable);
			}

			if (this.openedColumnsCount == 3)
			{
				this.mainToolbar.SetCommandState (ToolbarCommand.Accept, ToolbarCommandState.Enable);
				this.mainToolbar.SetCommandState (ToolbarCommand.Cancel, ToolbarCommandState.Enable);
			}
			else
			{
				this.mainToolbar.SetCommandState (ToolbarCommand.Accept, ToolbarCommandState.Hide);
				this.mainToolbar.SetCommandState (ToolbarCommand.Cancel, ToolbarCommandState.Hide);
			}

			this.mainToolbar.SetCommandState (ToolbarCommand.Accept, ToolbarCommandState.Hide);
			this.mainToolbar.SetCommandState (ToolbarCommand.Cancel, ToolbarCommandState.Hide);
		}


		private readonly ObjectsToolbarTreeTableController	objectsController;
		private readonly EventsToolbarTreeTableController	eventsController;
		private readonly ObjectEditor						objectEditor;

		private FrameBox									objectsFrameBox;
		private FrameBox									eventsFrameBox;
		private FrameBox									editFrameBox;
		private GlyphButton									closeButton;
		private IconButton									swapViewButton;

		private int											openedColumnsCount;
		private Timestamp?									selectedTimestamp;
	}
}
