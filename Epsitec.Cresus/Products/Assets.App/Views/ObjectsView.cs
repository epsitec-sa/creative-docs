//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Drawing;
using Epsitec.Common.Support;
using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.App.Popups;
using Epsitec.Cresus.Assets.App.Widgets;
using Epsitec.Cresus.Assets.Server.NaiveEngine;

namespace Epsitec.Cresus.Assets.App.Views
{
	public class ObjectsView : AbstractView
	{
		public ObjectsView(DataAccessor accessor, MainToolbar toolbar)
			: base (accessor, toolbar)
		{
			this.listController     = new ObjectsToolbarTreeTableController (this.accessor);
			this.timelineController = new ObjectsToolbarTimelineController (this.accessor);
			this.eventsController   = new EventsToolbarTreeTableController (this.accessor);
			this.objectEditor       = new ObjectEditor (this.accessor);

			this.ignoreChanges = new SafeCounter ();

			this.objectEditor.Navigate += delegate (object sender, Timestamp timestamp)
			{
				var index = this.timelineController.GetEventIndex (timestamp);

				if (index.HasValue)
				{
					this.timelineController.SelectedCell = index.Value;
				}

				this.eventsController.SelectedTimestamp = timestamp;
			};

			this.objectEditor.ValueChanged += delegate (object sender, ObjectField field)
			{
				this.UpdateToolbars ();
			};

			this.isWithTimelineView = true;
		}


		public override void Dispose()
		{
			this.mainToolbar.SetCommandState (ToolbarCommand.Edit,          ToolbarCommandState.Hide);
			this.mainToolbar.SetCommandState (ToolbarCommand.Amortissement, ToolbarCommandState.Hide);
			this.mainToolbar.SetCommandState (ToolbarCommand.Accept,        ToolbarCommandState.Hide);
			this.mainToolbar.SetCommandState (ToolbarCommand.Cancel,        ToolbarCommandState.Hide);
		}


		public override void CreateUI(Widget parent)
		{
			var topBox = new FrameBox
			{
				Parent  = parent,
				Dock    = DockStyle.Fill,
			};

			this.listFrameBox = new FrameBox
			{
				Parent = topBox,
				Dock   = DockStyle.Fill,
			};

			this.eventsFrameBox = new FrameBox
			{
				Parent = topBox,
				Dock   = DockStyle.Fill,
			};

			this.editFrameBox = new FrameBox
			{
				Parent         = topBox,
				Dock           = DockStyle.Right,
				PreferredWidth = 600,
				BackColor      = ColorManager.GetBackgroundColor (),
			};

			this.timelineFrameBox = new FrameBox
			{
				Parent = parent,
				Dock   = DockStyle.Bottom,
				Margins = new Margins (0, 0, 10, 0),
			};

			this.listController.CreateUI (this.listFrameBox);
			this.timelineController.CreateUI (this.timelineFrameBox);
			this.eventsController.CreateUI (this.eventsFrameBox);
			this.objectEditor.CreateUI (this.editFrameBox);

			this.withoutButton = new IconButton
			{
				Parent        = this.timelineFrameBox,
				AutoFocus     = false,
				IconUri       = AbstractCommandToolbar.GetResourceIconUri ("View.WithoutTimeline"),
				Anchor        = AnchorStyles.TopRight,
				PreferredSize = new Size (AbstractCommandToolbar.SecondaryToolbarHeight, AbstractCommandToolbar.SecondaryToolbarHeight),
			};
			ToolTip.Default.SetToolTip (this.withoutButton, "Cache l'axe du temps");

			this.withButton = new IconButton
			{
				Parent        = parent,
				AutoFocus     = false,
				IconUri       = AbstractCommandToolbar.GetResourceIconUri ("View.WithTimeline"),
				Anchor        = AnchorStyles.BottomRight,
				PreferredSize = new Size (AbstractCommandToolbar.SecondaryToolbarHeight, AbstractCommandToolbar.SecondaryToolbarHeight),
			};
			ToolTip.Default.SetToolTip (this.withButton, "Montre l'axe du temps");

			this.closeButton = new GlyphButton
			{
				Parent        = parent,
				GlyphShape    = GlyphShape.Close,
				ButtonStyle   = ButtonStyle.ToolItem,
				Anchor        = AnchorStyles.TopRight,
				PreferredSize = new Size (AbstractCommandToolbar.SecondaryToolbarHeight, AbstractCommandToolbar.SecondaryToolbarHeight),
				Margins       = new Margins (0, 0, TopTitle.Height, 0),
			};

			this.Update ();

			//	Connexion des événements.
			this.withoutButton.Clicked += delegate
			{
				this.OnChangeWithWithout (false);
			};

			this.withButton.Clicked += delegate
			{
				this.OnChangeWithWithout (true);
			};

			this.closeButton.Clicked += delegate
			{
				this.OnCloseColumn ();
			};

			this.listController.SelectedRowChanged += delegate
			{
				if (this.ignoreChanges.IsZero)
				{
					this.UpdateAfterListChanged ();
				}
			};

			this.listController.RowDoubleClicked += delegate
			{
				this.OnListDoubleClicked ();
			};

			this.timelineController.SelectedCellChanged += delegate
			{
				if (this.ignoreChanges.IsZero)
				{
					this.UpdateAfterTimelineChanged ();
				}
			};

			this.timelineController.CellDoubleClicked += delegate
			{
				this.OnStartStopEdit ();
			};

			this.eventsController.SelectedRowChanged += delegate
			{
				if (this.ignoreChanges.IsZero)
				{
					this.UpdateAfterEventsChanged ();
				}
			};

			this.eventsController.RowDoubleClicked += delegate
			{
				this.OnEventDoubleClicked ();
			};
		}


		public override void OnCommand(ToolbarCommand command)
		{
			base.OnCommand (command);

			switch (command)
			{
				case ToolbarCommand.Edit:
					this.OnStartStopEdit ();
					break;

				case ToolbarCommand.Amortissement:
					this.OnMainAmortissement ();
					break;

				case ToolbarCommand.Accept:
					this.OnEditAccept ();
					break;

				case ToolbarCommand.Cancel:
					this.OnEditCancel ();
					break;
			}
		}


		private void OnChangeWithWithout(bool isWith)
		{
			this.isWithTimelineView = isWith;

			if (this.isWithTimelineView)
			{
				this.listController.Timestamp = this.selectedTimestamp;
			}
			else
			{
				this.listController.Timestamp = new Timestamp (System.DateTime.MaxValue, 0);
			}

			this.UpdateGeometryWithWithoutTimeline ();
		}

		private void OnListDoubleClicked()
		{
			if (this.isWithTimelineView)
			{
				this.OnStartStopEdit ();
			}
			else
			{
				this.isShowEvents = !this.isShowEvents;
				this.isEditing = false;
				this.Update ();
			}
		}

		private void OnEventDoubleClicked()
		{
			this.OnStartStopEdit ();
		}

		private void OnStartStopEdit()
		{
			if (!this.isEditing && this.selectedGuid.IsEmpty)
			{
				return;
			}

			this.isEditing = !this.isEditing;
			this.Update ();
		}

		private void OnMainAmortissement()
		{
			var target = this.mainToolbar.GetCommandWidget (ToolbarCommand.Amortissement);

			if (target != null)
			{
				var popup = new YesNoPopup
				{
					Question = "Voulez-vous générer les amortissements ?",
				};

				if (this.listController.SelectedRow == -1)
				{
					popup.Radios.Add (new YesNoPopup.Radio ("all", "Pour tous les objets", activate: true));
				}
				else
				{
					popup.Radios.Add (new YesNoPopup.Radio ("one", "Pour l'objet sélectionné", activate: true));
					popup.Radios.Add (new YesNoPopup.Radio ("all", "Pour tous les objets"));
				}

				popup.Create (target);

				popup.ButtonClicked += delegate (object sender, string name)
				{
					if (name == "yes")
					{
						if (popup.RadioSelected == "one")
						{
							var guid = this.accessor.GetObjectGuid (this.listController.SelectedRow);
							this.businessLogic.GeneratesAmortissementsAuto (guid);
						}
						else
						{
							this.businessLogic.GeneratesAmortissementsAuto ();
						}

						this.Update ();
						this.timelineController.Update ();
						this.eventsController.Update ();
					}
				};
			}
		}

		private void OnCloseColumn()
		{
			if (this.isEditing)
			{
				this.isEditing = false;
			}
			else if (this.isShowEvents)
			{
				this.isShowEvents = false;
			}

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


		protected override void Update()
		{
			this.UpdateGeometryWithWithoutTimeline ();
			this.UpdateToolbars ();
			this.UpdateEditor ();
		}


		private void UpdateAfterListChanged()
		{
			int row = this.listController.SelectedRow;
			if (row == -1)
			{
				this.selectedGuid = Guid.Empty;

				this.isShowEvents = false;
				this.isEditing    = false;
			}
			else
			{
				this.selectedGuid = this.accessor.GetObjectGuid (row);
			}

			this.timelineController.ObjectGuid = this.selectedGuid;
			this.eventsController.ObjectGuid = this.selectedGuid;

			using (this.ignoreChanges.Enter ())
			{
				this.eventsController.SelectedTimestamp = this.selectedTimestamp;
			}

			this.Update ();
		}

		private void UpdateAfterTimelineChanged()
		{
			this.selectedTimestamp = this.timelineController.SelectedTimestamp;

			this.listController.Timestamp = this.selectedTimestamp;

			using (this.ignoreChanges.Enter ())
			{
				this.eventsController.SelectedTimestamp = this.selectedTimestamp;
			}

			this.UpdateToolbars ();
			this.UpdateEditor ();
		}

		private void UpdateAfterEventsChanged()
		{
			this.selectedTimestamp = this.eventsController.SelectedTimestamp;

			using (this.ignoreChanges.Enter ())
			{
				this.timelineController.SelectedTimestamp = this.selectedTimestamp;
			}

			this.UpdateToolbars ();
			this.UpdateEditor ();
		}


		private void UpdateEditor()
		{
			this.objectEditor.SetObject (this.selectedGuid, this.selectedTimestamp);
		}


		private void UpdateGeometryWithWithoutTimeline()
		{
			this.withoutButton.Visibility =  this.isWithTimelineView;
			this.withButton   .Visibility = !this.isWithTimelineView;

			if (this.isWithTimelineView)
			{
				this.UpdateGeometryWithTimeline ();
			}
			else
			{
				this.UpdateGeometryWithoutTimeline ();
			}
		}

		private void UpdateGeometryWithTimeline()
		{
			this.eventsFrameBox.Visibility = false;
			this.closeButton   .Visibility = false;

			this.listFrameBox    .Visibility = true;
			this.timelineFrameBox.Visibility = true;
			this.editFrameBox    .Visibility = this.isEditing;

			this.listFrameBox.Dock    = DockStyle.Fill;
			this.listFrameBox.Margins = new Margins (0);

			this.editFrameBox.Dock    = DockStyle.Right;
			this.editFrameBox.Margins = new Margins (10, 0, 0, 0);
		}

		private void UpdateGeometryWithoutTimeline()
		{
			this.timelineFrameBox.Visibility = false;

			if (this.isEditing)
			{
				this.isShowEvents = true;
			}

			if (!this.isShowEvents)
			{
				this.listFrameBox  .Visibility = true;
				this.eventsFrameBox.Visibility = false;
				this.editFrameBox  .Visibility = false;

				this.listFrameBox.Dock    = DockStyle.Fill;
				this.listFrameBox.Margins = new Margins (0, 0, 0, AbstractCommandToolbar.SecondaryToolbarHeight);
			}
			else if (this.isShowEvents && !this.isEditing)
			{
				this.listFrameBox  .Visibility = true;
				this.eventsFrameBox.Visibility = true;
				this.editFrameBox  .Visibility = false;

				this.listFrameBox.Dock           = DockStyle.Left;
				this.listFrameBox.PreferredWidth = 190;
				this.listFrameBox.Margins        = new Margins (0, 0, 0, AbstractCommandToolbar.SecondaryToolbarHeight);

				this.eventsFrameBox.Dock = DockStyle.Fill;
				this.eventsFrameBox.Margins = new Margins (10, 0, 0, AbstractCommandToolbar.SecondaryToolbarHeight);
			}
			else if (this.isEditing)
			{
				this.listFrameBox.Visibility   = false;
				this.eventsFrameBox.Visibility = true;
				this.editFrameBox.Visibility   = true;

				this.eventsFrameBox.Dock    = DockStyle.Fill;
				this.eventsFrameBox.Margins = new Margins (0, 0, 0, AbstractCommandToolbar.SecondaryToolbarHeight);

				this.editFrameBox.Dock    = DockStyle.Right;
				this.editFrameBox.Margins = new Margins (10, 0, 0, AbstractCommandToolbar.SecondaryToolbarHeight);
			}
			else
			{
				System.Diagnostics.Debug.Fail ("Impossible statment");
			}

			this.closeButton.Visibility = this.isShowEvents || this.isEditing;
		}


		private void UpdateToolbars()
		{
			if (this.isEditing)
			{
				this.mainToolbar.SetCommandState (ToolbarCommand.Edit, ToolbarCommandState.Activate);

				this.mainToolbar.UpdateCommand (ToolbarCommand.Accept, this.objectEditor.EditionDirty);
				this.mainToolbar.SetCommandState (ToolbarCommand.Cancel, ToolbarCommandState.Enable);
			}
			else
			{
				this.mainToolbar.UpdateCommand (ToolbarCommand.Edit, this.IsEditingPossible);

				this.mainToolbar.SetCommandState (ToolbarCommand.Accept, ToolbarCommandState.Hide);
				this.mainToolbar.SetCommandState (ToolbarCommand.Cancel, ToolbarCommandState.Hide);
			}
		}

		private bool IsEditingPossible
		{
			get
			{
				if (this.isWithTimelineView)
				{
					return this.listController.SelectedRow != -1;
				}
				else
				{
					return this.listController.SelectedRow != -1 && this.isShowEvents;
				}
			}
		}


		private readonly ObjectsToolbarTreeTableController	listController;
		private readonly ObjectsToolbarTimelineController	timelineController;
		private readonly EventsToolbarTreeTableController	eventsController;
		private readonly ObjectEditor						objectEditor;
		private readonly SafeCounter						ignoreChanges;

		private FrameBox									listFrameBox;
		private FrameBox									timelineFrameBox;
		private FrameBox									eventsFrameBox;
		private FrameBox									editFrameBox;

		private IconButton									withoutButton;
		private IconButton									withButton;
		private GlyphButton									closeButton;

		private bool										isWithTimelineView;
		private bool										isShowEvents;
		private bool										isEditing;
		private Guid										selectedGuid;
		private Timestamp?									selectedTimestamp;
	}
}
