//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Drawing;
using Epsitec.Common.Support;
using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.App.Widgets;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.App.Views
{
	public class ObjectsView : AbstractView
	{
		public ObjectsView(DataAccessor accessor, MainToolbar toolbar)
			: base (accessor, toolbar)
		{
			this.baseType = BaseType.Objects;

			this.listController           = new ObjectsToolbarTreeTableController (this.accessor);
			this.timelineController       = new ObjectsToolbarTimelineController (this.accessor, this.baseType);
			this.eventsController         = new EventsToolbarTreeTableController (this.accessor);
			this.timelinesArrayController = new TimelinesArrayController (this.accessor);
			this.objectEditor             = new ObjectEditor (this.accessor, this.baseType, isTimeless: false);

			this.ignoreChanges = new SafeCounter ();

			this.viewMode = ViewMode.Single;
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
				PreferredWidth = 750,
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

			this.Update ();
			this.OnChangeViewMode (this.viewMode);

			//	Connexion des événements de la liste des objets à gauche.
			{
				this.listController.StartEditing += delegate (object sender, EventType eventType, Timestamp timestamp)
				{
					this.OnStartEdit (eventType, timestamp);
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
			}

			//	Connexion des événements de la timeline en bas.
			{
				this.timelineController.StartEditing += delegate (object sender, EventType eventType)
				{
					this.OnStartEdit (eventType);
				};

				this.timelineController.UpdateAll += delegate
				{
					this.Update ();
					this.eventsController.Update ();
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
				this.eventsController.StartEditing += delegate (object sender, EventType eventType, Timestamp timestamp)
				{
					this.OnStartEdit (eventType, timestamp);
				};

				this.eventsController.UpdateAll += delegate
				{
					this.Update ();
					this.timelineController.Update ();
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
					this.timelineController.SelectedTimestamp = timestamp;
					this.eventsController.SelectedTimestamp = timestamp;
				};

				this.objectEditor.Goto += delegate (object sender, AbstractViewState viewState)
				{
					this.OnGoto (viewState);
				};

				this.objectEditor.ValueChanged += delegate (object sender, ObjectField field)
				{
					this.UpdateToolbars ();
				};

				this.objectEditor.UpdateData += delegate
				{
					this.UpdateData ();
				};
			}

			this.closeButton.Clicked += delegate
			{
				this.OnCloseColumn ();
			};
		}


		public override AbstractViewState ViewState
		{
			get
			{
				return new ObjectsViewState
				{
					ViewType          = ViewType.Objects,
					ViewMode          = this.viewMode,
					PageType          = this.isEditing ? this.objectEditor.PageType : PageType.Unknown,
					SelectedTimestamp = this.selectedTimestamp,
					SelectedGuid      = this.selectedGuid,
				};
			}
			set
			{
				var viewState = value as ObjectsViewState;
				System.Diagnostics.Debug.Assert (viewState != null);

				this.viewMode          = viewState.ViewMode;
				this.selectedTimestamp = viewState.SelectedTimestamp;
				this.selectedGuid      = viewState.SelectedGuid;

				this.mainToolbar.ViewMode = this.viewMode;

				if (viewState.PageType == PageType.Unknown)
				{
					this.isEditing = false;
				}
				else
				{
					this.isEditing = true;
					this.objectEditor.PageType = viewState.PageType;
				}

				//?this.selectedGuid = value.Guid;
				//?this.OnChangeViewMode (value.ViewMode);
				//?
				//?if (value.PageType == PageType.Person)
				//?{
				//?	this.isEditing = true;
				//?}
				//?else
				//?{
				//?	this.isEditing = false;
				//?}

				this.Update ();
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
					this.OnChangeViewMode (ViewMode.Single);
					break;

				case ToolbarCommand.ViewModeEvent:
					this.OnChangeViewMode (ViewMode.Event);
					break;

				case ToolbarCommand.ViewModeMultiple:
					this.OnChangeViewMode (ViewMode.Multiple);
					break;
			}
		}


		private void OnChangeViewMode(ViewMode viewMode)
		{
			if (this.viewMode != viewMode)
			{
				this.viewMode = viewMode;
				this.OnViewStateChanged (this.ViewState);
				this.UpdateViewModeGeometry ();

				this.editFrameBox.Window.ForceLayout ();
			}

			using (this.ignoreChanges.Enter ())
			{
				switch (this.viewMode)
				{
					case ViewMode.Single:
						this.listController.UpdateData ();
						this.listController.SelectedGuid = this.selectedGuid;
						this.listController.SelectedTimestamp = this.selectedTimestamp;

						this.timelineController.ObjectGuid = this.selectedGuid;
						this.timelineController.SelectedTimestamp = this.selectedTimestamp;
						break;

					case ViewMode.Event:
						this.listController.UpdateData ();
						this.listController.SelectedGuid = this.selectedGuid;
						this.listController.SelectedTimestamp = Timestamp.MaxValue;

						this.eventsController.ObjectGuid = this.selectedGuid;
						this.eventsController.SelectedTimestamp = this.selectedTimestamp;
						break;

					case ViewMode.Multiple:
						this.timelinesArrayController.UpdateData ();
						this.timelinesArrayController.SelectedGuid = this.selectedGuid;
						this.timelinesArrayController.SelectedTimestamp = this.selectedTimestamp;
						break;
				}
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
					this.Update ();
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
			this.Update ();
		}

		private void OnStartEdit()
		{
			if (!this.isEditing && this.selectedGuid.IsEmpty)
			{
				return;
			}

			this.isEditing = true;
			this.Update ();
		}

		private void OnStartEdit(EventType eventType, Timestamp? timestamp = null)
		{
			//	Démarre une édition après avoir créé un événement.
			this.isEditing = true;
			this.Update ();

			if (timestamp.HasValue)
			{
				switch (this.viewMode)
				{
					case ViewMode.Single:
						this.timelineController.SelectedTimestamp = timestamp.Value;
						break;

					case ViewMode.Event:
						this.eventsController.SelectedTimestamp = timestamp.Value;
						break;

					case ViewMode.Multiple:
						this.timelinesArrayController.SelectedTimestamp = timestamp.Value;
						break;
				}
			}

			this.objectEditor.OpenMainPage (eventType);
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
			this.accessor.EditionAccessor.CancelObjectEdition ();
			this.isEditing = false;
			this.Update ();
		}


		protected override void Update(bool dataChanged = false)
		{
			bool updateData = dataChanged;

			if (!this.isEditing)
			{
				if (this.accessor.EditionAccessor.SaveObjectEdition ())
				{
					updateData = true;
				}
			}

			if (updateData)
			{
				this.UpdateData ();
			}

			if (dataChanged)
			{
				this.timelineController.Update ();
				this.eventsController.Update ();
			}

			this.UpdateViewModeGeometry ();
			this.UpdateToolbars ();
			this.UpdateEditor ();
		}


		private void UpdateAfterListChanged()
		{
			this.OnViewStateChanged (this.ViewState);
			this.selectedGuid = this.listController.SelectedGuid;

			if (this.selectedGuid.IsEmpty)
			{
				this.isShowEvents = false;
				this.isEditing    = false;
			}

			this.timelineController.ObjectGuid = this.selectedGuid;

			if (!this.eventsController.DataFreezed)
			{
				this.eventsController.ObjectGuid = this.selectedGuid;
			}

			//	On sélectionne le dernier événement de l'objet dans la timeline.
			var timestamp = this.GetLastTimestamp (this.selectedGuid);
			if (timestamp.HasValue)
			{
				this.selectedTimestamp = timestamp;
			}

			using (this.ignoreChanges.Enter ())
			{
				if (!this.timelineController.DataFreezed)
				{
					this.timelineController.SelectedTimestamp = this.selectedTimestamp;
				}

				if (!this.eventsController.DataFreezed)
				{
					this.eventsController.SelectedTimestamp = this.selectedTimestamp;
				}
			}

			this.Update ();
		}

		private void UpdateAfterTimelineChanged()
		{
			this.selectedTimestamp = this.timelineController.SelectedTimestamp;

			this.listController.SelectedTimestamp = this.selectedTimestamp;

			this.UpdateToolbars ();
			this.UpdateEditor ();
		}

		private void UpdateAfterEventsChanged()
		{
			this.selectedTimestamp = this.eventsController.SelectedTimestamp;

			this.UpdateToolbars ();
			this.UpdateEditor ();
		}

		private void UpdateAfterMultipleChanged()
		{
			this.selectedGuid      = this.timelinesArrayController.SelectedGuid;
			this.selectedTimestamp = this.timelinesArrayController.SelectedTimestamp;

			if (this.selectedTimestamp == null)
			{
				this.isEditing = false;
			}

			this.Update ();
		}


		private void UpdateData()
		{
			switch (this.viewMode)
			{
				case ViewMode.Single:
				case ViewMode.Event:
					this.listController.UpdateData ();
					this.listController.SelectedGuid = this.selectedGuid;
					break;

				case ViewMode.Multiple:
					this.timelinesArrayController.UpdateData ();
					this.timelinesArrayController.SelectedGuid = this.selectedGuid;
					break;
			}
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
			this.listController          .DataFreezed = false;
			this.eventsController        .DataFreezed = true;
			this.timelineController      .DataFreezed = false;
			this.timelinesArrayController.DataFreezed = true;

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
			this.listController          .DataFreezed = false;
			this.eventsController        .DataFreezed = false;
			this.timelineController      .DataFreezed = true;
			this.timelinesArrayController.DataFreezed = true;

			this.timelineFrameBox      .Visibility = false;
			this.timelinesArrayFrameBox.Visibility = false;

			if (this.isEditing)
			{
				this.isShowEvents = true;
			}

			if (!this.isShowEvents)
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
			this.listController          .DataFreezed = true;
			this.eventsController        .DataFreezed = true;
			this.timelineController      .DataFreezed = true;
			this.timelinesArrayController.DataFreezed = false;

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

			this.mainToolbar.SetCommandState (ToolbarCommand.Amortissement, ToolbarCommandState.Enable);
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


		private readonly ObjectsToolbarTreeTableController	listController;
		private readonly ObjectsToolbarTimelineController	timelineController;
		private readonly EventsToolbarTreeTableController	eventsController;
		private readonly TimelinesArrayController			timelinesArrayController;
		private readonly ObjectEditor						objectEditor;
		private readonly SafeCounter						ignoreChanges;

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
	}
}
