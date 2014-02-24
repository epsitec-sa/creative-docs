//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.App.Widgets;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.App.Views
{
	public class AssetsView : AbstractView
	{
		public AssetsView(DataAccessor accessor, MainToolbar toolbar)
			: base (accessor, toolbar)
		{
			this.baseType = BaseType.Assets;

			this.listController     = new AssetsToolbarTreeTableController (this.accessor);
			this.timelineController = new AssetsToolbarTimelineController (this.accessor, this.baseType);
			this.eventsController   = new EventsToolbarTreeTableController (this.accessor);

			this.timelinesArrayController = new TimelinesArrayController (this.accessor)
			{
				Title = StaticDescriptions.GetViewTypeDescription (ViewType.Assets),
			};

			this.objectEditor = new ObjectEditor (this.accessor, this.baseType, this.baseType, isTimeless: false);

			this.viewMode = ViewMode.Single;
		}


		public override void Dispose()
		{
			this.mainToolbar.SetCommandState (ToolbarCommand.Edit,   ToolbarCommandState.Hide);
			this.mainToolbar.SetCommandState (ToolbarCommand.Accept, ToolbarCommandState.Hide);
			this.mainToolbar.SetCommandState (ToolbarCommand.Cancel, ToolbarCommandState.Hide);
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
				PreferredWidth = AbstractView.editionWidth,
				BackColor      = ColorManager.GetBackgroundColor (),
			};

			this.timelinesArrayFrameBox = new FrameBox
			{
				Parent = topBox,
				Dock   = DockStyle.Fill,
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
			this.timelinesArrayController.CreateUI (this.timelinesArrayFrameBox);
			this.objectEditor.CreateUI (this.editFrameBox);

			this.closeButton = new GlyphButton
			{
				Parent        = parent,
				GlyphShape    = GlyphShape.Close,
				ButtonStyle   = ButtonStyle.ToolItem,
				Anchor        = AnchorStyles.TopRight,
				PreferredSize = new Size (AbstractCommandToolbar.secondaryToolbarHeight, AbstractCommandToolbar.secondaryToolbarHeight),
				Margins       = new Margins (0, 0, TopTitle.height, 0),
			};

			this.DeepUpdateUI ();

			//	Connexion des événements de la liste des objets à gauche.
			{
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

				this.listController.UpdateAfterCreate += delegate (object sender, Guid guid, EventType eventType, Timestamp timestamp)
				{
					this.OnUpdateAfterObjectCreate (guid);
				};

				this.listController.UpdateAfterDelete += delegate
				{
					this.OnUpdateAfterObjectDelete ();
				};
			}

			//	Connexion des événements de la timeline en bas.
			{
				this.timelineController.StartEditing += delegate (object sender, EventType eventType, Timestamp timestamp)
				{
					this.OnStartEdit (eventType, timestamp);
				};

				this.timelineController.DeepUpdate += delegate
				{
					this.DeepUpdateUI ();
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
					this.OnStartEdit ();
				};
			}

			//	Connexion des événements de la liste des événements.
			{
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

				this.eventsController.UpdateAfterCreate += delegate (object sender, Guid guid, EventType eventType, Timestamp timestamp)
				{
					this.OnUpdateAfterEventCreate (guid, timestamp);
				};

				this.eventsController.UpdateAfterDelete += delegate
				{
					this.OnUpdateAfterEventDelete ();
				};
			}

			//	Connexion des événements du tableau des objets et timelines.
			{
				this.timelinesArrayController.StartEditing += delegate (object sender, EventType eventType, Timestamp timestamp)
				{
					this.OnStartEdit (eventType, timestamp);
				};

				this.timelinesArrayController.SelectedCellChanged += delegate
				{
					if (this.ignoreChanges.IsZero)
					{
						this.UpdateAfterMultipleChanged ();
					}
				};

				this.timelinesArrayController.CellDoubleClicked += delegate
				{
					this.OnStartEdit ();
				};
			}

			//	Connexion des événements de l'éditeur.
			{
				this.objectEditor.Navigate += delegate (object sender, Timestamp timestamp)
				{
					this.selectedTimestamp = timestamp;
					this.UpdateUI ();
				};

				this.objectEditor.Goto += delegate (object sender, AbstractViewState viewState)
				{
					this.OnGoto (viewState);
				};

				this.objectEditor.PageTypeChanged += delegate (object sender, PageType pageType)
				{
					this.UpdateUI ();
				};

				this.objectEditor.ValueChanged += delegate (object sender, ObjectField field)
				{
					this.UpdateToolbars ();
				};

				this.objectEditor.DataChanged += delegate
				{
					this.DataChanged ();
				};
			}

			this.closeButton.Clicked += delegate
			{
				this.OnCloseColumn ();
			};
		}


		public override void DataChanged()
		{
			this.listController.DirtyData = true;
			this.timelineController.DirtyData = true;
			this.eventsController.DirtyData = true;
			this.timelinesArrayController.DirtyData = true;
		}

		public override void DeepUpdateUI()
		{
			this.DataChanged ();
			this.UpdateUI ();
		}

		public override void UpdateUI()
		{
			if (this.accessor.EditionAccessor.SaveObjectEdition ())
			{
				this.DataChanged ();
			}

			//	Met à jour la géométrie des différents contrôleurs.
			if (this.lastViewMode     != this.viewMode    ||
				this.lastIsShowEvents != this.isShowEvents||
				this.lastIsEditing    != this.isEditing   )
			{
				this.UpdateViewModeGeometry ();
				this.editFrameBox.Window.ForceLayout ();

				this.lastViewMode     = this.viewMode;
				this.lastIsShowEvents = this.isShowEvents;
				this.lastIsEditing    = this.isEditing;
			}

			//	Met à jour les données des différents contrôleurs.
			using (this.ignoreChanges.Enter ())
			{
				if (this.listController.InUse)
				{
					if (this.listController.DirtyData)
					{
						this.listController.UpdateData ();
						this.listController.SelectedGuid      = this.selectedGuid;
						this.listController.SelectedTimestamp = this.selectedTimestamp;

						this.listController.DirtyData = false;

						//	Si les données de la liste de gauche ont changé, il faudra aussi
						//	mettre à jour les contrôleurs qui en dépendent.
						this.timelineController.DirtyData = true;
						this.eventsController.DirtyData   = true;
					}
					else if (this.listController.SelectedGuid      != this.selectedGuid     ||
							 this.listController.SelectedTimestamp != this.selectedTimestamp)
					{
						this.listController.SelectedGuid      = this.selectedGuid;
						this.listController.SelectedTimestamp = this.selectedTimestamp;

						//	Si l'objet sélectionné dans la liste de gauche a changé, il
						//	faudra aussi mettre à jour les contrôleurs qui en dépendent.
						this.timelineController.DirtyData = true;
						this.eventsController.DirtyData   = true;
					}
				}

				if (this.timelineController.InUse)
				{
					if (this.timelineController.DirtyData)
					{
						this.timelineController.ObjectGuid = this.selectedGuid;
						this.timelineController.UpdateData ();
						this.timelineController.SelectedTimestamp = this.selectedTimestamp;

						this.timelineController.DirtyData = false;
					}
					else if (this.timelineController.SelectedTimestamp != this.selectedTimestamp)
					{
						this.timelineController.SelectedTimestamp = this.selectedTimestamp;
					}
				}

				if (this.eventsController.InUse)
				{
					if (this.eventsController.DirtyData)
					{
						this.eventsController.ObjectGuid = this.selectedGuid;
						this.eventsController.UpdateData ();
						this.eventsController.SelectedTimestamp = this.selectedTimestamp;

						this.eventsController.DirtyData = false;
					}
					else if (this.eventsController.SelectedTimestamp != this.selectedTimestamp)
					{
						this.eventsController.SelectedTimestamp = this.selectedTimestamp;
					}
				}

				if (this.timelinesArrayController.InUse)
				{
					if (this.timelinesArrayController.DirtyData)
					{
						this.timelinesArrayController.UpdateData ();
						this.timelinesArrayController.SelectedGuid      = this.selectedGuid;
						this.timelinesArrayController.SelectedTimestamp = this.selectedTimestamp;

						this.timelinesArrayController.DirtyData = false;
					}
					else if (this.timelinesArrayController.SelectedGuid      != this.selectedGuid     ||
							 this.timelinesArrayController.SelectedTimestamp != this.selectedTimestamp)
					{
						this.timelinesArrayController.SelectedGuid      = this.selectedGuid;
						this.timelinesArrayController.SelectedTimestamp = this.selectedTimestamp;
					}
				}
			}

			this.UpdateToolbars ();
			this.UpdateEditor ();

			this.OnViewStateChanged (this.ViewState);
		}


		public override AbstractViewState ViewState
		{
			get
			{
				return new AssetsViewState
				{
					ViewType          = ViewType.Assets,
					ViewMode          = this.viewMode,
					PageType          = this.isEditing ? this.objectEditor.PageType : PageType.Unknown,
					IsShowEvents      = this.isShowEvents,
					SelectedTimestamp = this.selectedTimestamp,
					SelectedGuid      = this.selectedGuid,
				};
			}
			set
			{
				var viewState = value as AssetsViewState;
				System.Diagnostics.Debug.Assert (viewState != null);

				this.viewMode          = viewState.ViewMode;
				this.isShowEvents      = viewState.IsShowEvents;
				this.selectedTimestamp = viewState.SelectedTimestamp;
				this.selectedGuid      = viewState.SelectedGuid;

				if (viewState.PageType == PageType.Unknown)
				{
					this.isEditing = false;
				}
				else
				{
					this.isEditing = true;
					this.objectEditor.PageType = viewState.PageType;
				}

				this.UpdateUI ();
			}
		}

		protected override Guid SelectedGuid
		{
			get
			{
				return this.selectedGuid;
			}
		}


		public override void OnCommand(ToolbarCommand command)
		{
			base.OnCommand (command);

			switch (command)
			{
				case ToolbarCommand.Edit:
					this.OnStartStopEdit ();
					break;

				case ToolbarCommand.Accept:
					this.OnEditAccept ();
					break;

				case ToolbarCommand.Cancel:
					this.OnEditCancel ();
					break;

				case ToolbarCommand.ViewModeSingle:
					this.viewMode = ViewMode.Single;
					this.UpdateUI ();
					break;

				case ToolbarCommand.ViewModeEvent:
					this.viewMode = ViewMode.Event;
					this.UpdateUI ();
					break;

				case ToolbarCommand.ViewModeMultiple:
					this.viewMode = ViewMode.Multiple;
					this.UpdateUI ();
					break;
			}
		}


		private void OnListDoubleClicked()
		{
			switch (this.viewMode)
			{
				case ViewMode.Single:
					this.OnStartEdit ();
					break;

				case ViewMode.Event:
					this.isShowEvents = !this.isShowEvents;
					this.isEditing = false;
					this.UpdateUI ();
					break;
			}
		}

		private void OnEventDoubleClicked()
		{
			this.OnStartEdit ();
		}

		private void OnStartStopEdit()
		{
			if (!this.isEditing && this.selectedGuid.IsEmpty)
			{
				return;
			}

			this.isEditing = !this.isEditing;
			this.UpdateUI ();
		}

		private void OnStartEdit()
		{
			if (!this.isEditing && this.selectedGuid.IsEmpty)
			{
				return;
			}

			this.isEditing = true;
			this.UpdateUI ();
		}

		private void OnStartEdit(EventType eventType, Timestamp? timestamp)
		{
			this.isEditing = true;
			this.selectedTimestamp = timestamp;
			this.objectEditor.PageType = this.objectEditor.MainPageType;

			this.UpdateUI ();
		}

		private void OnUpdateAfterObjectCreate(Guid guid)
		{
			this.isEditing = true;
			this.selectedGuid = guid;
			this.objectEditor.PageType = this.objectEditor.MainPageType;

			this.DeepUpdateUI ();
		}

		private void OnUpdateAfterObjectDelete()
		{
			this.isEditing = false;
			this.selectedGuid = Guid.Empty;

			this.DeepUpdateUI ();
		}

		private void OnUpdateAfterEventCreate(Guid guid, Timestamp timestamp)
		{
			this.isShowEvents = true;
			this.isEditing = true;
			this.selectedTimestamp = timestamp;
			this.objectEditor.PageType = this.objectEditor.MainPageType;

			this.DeepUpdateUI ();
		}

		private void OnUpdateAfterEventDelete()
		{
			this.isShowEvents = true;
			this.isEditing = false;
			this.selectedTimestamp = null;

			this.DeepUpdateUI ();
		}

		private void OnCloseColumn()
		{
			if (this.isEditing)
			{
				this.isEditing = false;
				this.isShowEvents = true;
			}
			else if (this.isShowEvents)
			{
				this.isShowEvents = false;
			}

			this.UpdateUI ();
		}

		private void OnEditAccept()
		{
			this.isEditing = false;
			this.UpdateUI ();
		}

		private void OnEditCancel()
		{
			this.accessor.EditionAccessor.CancelObjectEdition ();
			this.isEditing = false;
			this.UpdateUI ();
		}


		private void UpdateAfterListChanged()
		{
			this.selectedGuid = this.listController.SelectedGuid;

			//	On sélectionne le dernier événement de l'objet dans la timeline.
			this.selectedTimestamp = this.GetLastTimestamp (this.selectedGuid);

			if (this.selectedGuid.IsEmpty || !this.selectedTimestamp.HasValue)
			{
				this.isShowEvents = false;
				this.isEditing    = false;
			}

			this.timelineController.DirtyData = true;
			this.eventsController.DirtyData = true;
			this.timelinesArrayController.DirtyData = true;

			this.UpdateUI ();
		}

		private void UpdateAfterTimelineChanged()
		{
			this.selectedTimestamp = this.timelineController.SelectedTimestamp;

			if (this.selectedGuid.IsEmpty || !this.selectedTimestamp.HasValue)
			{
				this.isShowEvents = false;
				this.isEditing    = false;
			}

			this.UpdateUI ();
		}

		private void UpdateAfterEventsChanged()
		{
			this.selectedTimestamp = this.eventsController.SelectedTimestamp;

			if (this.selectedGuid.IsEmpty || !this.selectedTimestamp.HasValue)
			{
				this.isShowEvents = false;
				this.isEditing    = false;
			}

			this.UpdateUI ();
		}

		private void UpdateAfterMultipleChanged()
		{
			this.selectedGuid      = this.timelinesArrayController.SelectedGuid;
			this.selectedTimestamp = this.timelinesArrayController.SelectedTimestamp;

			if (this.selectedGuid.IsEmpty || !this.selectedTimestamp.HasValue)
			{
				this.isShowEvents = false;
				this.isEditing    = false;
			}

			this.UpdateUI ();
		}


		private void UpdateEditor()
		{
			this.objectEditor.SetObject (this.selectedGuid, this.selectedTimestamp);
		}


		private void UpdateViewModeGeometry()
		{
			switch (this.viewMode)
			{
				case ViewMode.Single:
					this.UpdateSingleGeometry ();
					break;

				case ViewMode.Event:
					this.UpdateEventGeometry ();
					break;

				case ViewMode.Multiple:
					this.UpdateMultipleGeometry ();
					break;
			}
		}

		private void UpdateSingleGeometry()
		{
			this.listController          .InUse = true;
			this.eventsController        .InUse = false;
			this.timelineController      .InUse = true;
			this.timelinesArrayController.InUse = false;

			this.eventsFrameBox.Visibility = false;
			this.closeButton   .Visibility = false;

			this.listFrameBox          .Visibility = true;
			this.timelineFrameBox      .Visibility = true;
			this.editFrameBox          .Visibility = this.isEditing;
			this.timelinesArrayFrameBox.Visibility = false;

			this.listFrameBox.Dock = DockStyle.Fill;

			this.editFrameBox.Dock = DockStyle.Right;
		}

		private void UpdateEventGeometry()
		{
			this.listController          .InUse = true;
			this.eventsController        .InUse = true;
			this.timelineController      .InUse = false;
			this.timelinesArrayController.InUse = false;

			this.timelineFrameBox      .Visibility = false;
			this.timelinesArrayFrameBox.Visibility = false;

			//-if (this.isEditing)
			//-{
			//-	this.isShowEvents = true;
			//-}

			if (!this.isShowEvents && !this.isEditing)
			{
				this.listFrameBox  .Visibility = true;
				this.eventsFrameBox.Visibility = false;
				this.editFrameBox  .Visibility = false;

				this.listFrameBox.Dock = DockStyle.Fill;
			}
			else if (this.isShowEvents && !this.isEditing)
			{
				this.listFrameBox  .Visibility = true;
				this.eventsFrameBox.Visibility = true;
				this.editFrameBox  .Visibility = false;

				this.listFrameBox.Dock           = DockStyle.Left;
				this.listFrameBox.PreferredWidth = 190;

				this.eventsFrameBox.Dock = DockStyle.Fill;
			}
			else if (this.isEditing)
			{
				this.listFrameBox  .Visibility = false;
				this.eventsFrameBox.Visibility = true;
				this.editFrameBox  .Visibility = true;

				this.eventsFrameBox.Dock = DockStyle.Fill;

				this.editFrameBox.Dock = DockStyle.Right;
			}
			else
			{
				System.Diagnostics.Debug.Fail ("Impossible statment");
			}

			this.closeButton.Visibility = this.isShowEvents || this.isEditing;
		}

		private void UpdateMultipleGeometry()
		{
			this.listController          .InUse = false;
			this.eventsController        .InUse = false;
			this.timelineController      .InUse = false;
			this.timelinesArrayController.InUse = true;

			this.eventsFrameBox.Visibility = false;
			this.closeButton   .Visibility = false;

			this.listFrameBox          .Visibility = false;
			this.timelineFrameBox      .Visibility = false;
			this.editFrameBox          .Visibility = this.isEditing;
			this.timelinesArrayFrameBox.Visibility = true;

			this.editFrameBox.Dock = DockStyle.Right;
		}


		private void UpdateToolbars()
		{
			if (this.isEditing)
			{
				this.mainToolbar.SetCommandState (ToolbarCommand.Edit, ToolbarCommandState.Activate);

				this.mainToolbar.SetCommandEnable (ToolbarCommand.Accept, this.objectEditor.EditionDirty);
				this.mainToolbar.SetCommandState (ToolbarCommand.Cancel, ToolbarCommandState.Enable);
			}
			else
			{
				this.mainToolbar.SetCommandEnable (ToolbarCommand.Edit, this.IsEditingPossible);

				this.mainToolbar.SetCommandState (ToolbarCommand.Accept, ToolbarCommandState.Hide);
				this.mainToolbar.SetCommandState (ToolbarCommand.Cancel, ToolbarCommandState.Hide);
			}

			this.mainToolbar.ViewMode = this.viewMode;
		}

		private bool IsEditingPossible
		{
			get
			{
				switch (this.viewMode)
				{
					case ViewMode.Single:
						return this.listController.SelectedRow != -1;

					case ViewMode.Event:
						return this.listController.SelectedRow != -1
							&& this.isShowEvents;

					case ViewMode.Multiple:
						return !this.timelinesArrayController.SelectedGuid.IsEmpty
							&& this.timelinesArrayController.SelectedTimestamp != null;

					default:
						return false;
				}
			}
		}


		private readonly AssetsToolbarTreeTableController	listController;
		private readonly AssetsToolbarTimelineController	timelineController;
		private readonly EventsToolbarTreeTableController	eventsController;
		private readonly TimelinesArrayController			timelinesArrayController;
		private readonly ObjectEditor						objectEditor;

		private FrameBox									listFrameBox;
		private FrameBox									timelineFrameBox;
		private FrameBox									eventsFrameBox;
		private FrameBox									timelinesArrayFrameBox;
		private FrameBox									editFrameBox;

		private GlyphButton									closeButton;

		private ViewMode									viewMode;
		private bool										isShowEvents;
		private bool										isEditing;
		private Guid										selectedGuid;
		private Timestamp?									selectedTimestamp;

		private ViewMode									lastViewMode;
		private bool										lastIsShowEvents;
		private bool										lastIsEditing;
	}
}
