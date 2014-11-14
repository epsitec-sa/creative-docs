//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.App.Helpers;
using Epsitec.Cresus.Assets.App.Popups;
using Epsitec.Cresus.Assets.App.Settings;
using Epsitec.Cresus.Assets.App.Views.CommandToolbars;
using Epsitec.Cresus.Assets.App.Views.Editors;
using Epsitec.Cresus.Assets.App.Views.ToolbarControllers;
using Epsitec.Cresus.Assets.App.Views.ViewStates;
using Epsitec.Cresus.Assets.App.Widgets;
using Epsitec.Cresus.Assets.Data;
using Epsitec.Cresus.Assets.Server.BusinessLogic;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.App.Views
{
	public class AssetsView : AbstractView, System.IDisposable
	{
		public AssetsView(DataAccessor accessor, CommandContext commandContext, MainToolbar toolbar, ViewType viewType)
			: base (accessor, commandContext, toolbar, viewType)
		{
			this.baseType = BaseType.Assets;

			this.listController     = new AssetsToolbarTreeTableController (this.accessor, this.commandContext, this.baseType);
			this.timelineController = new AssetsToolbarTimelineController  (this.accessor, this.commandContext, this.baseType);
			this.eventsController   = new EventsToolbarTreeTableController (this.accessor, this.commandContext, this.baseType);

			this.timelinesArrayController = new TimelinesArrayController (this.accessor, this.commandContext, this.mainToolbar)
			{
				Title = this.GetViewTitle (ViewType.Assets),
			};

			this.objectEditor = new ObjectEditor (this.accessor, this.baseType, isTimeless: false);
		}


		public override void Dispose()
		{
			this.listController.Dispose ();
			this.timelineController.Dispose ();
			this.eventsController.Dispose ();
			this.timelinesArrayController.Dispose ();

			base.Dispose ();
		}


		public override void CreateUI(Widget parent)
		{
			base.CreateUI (parent);

			switch (this.mainToolbar.ViewMode)
			{
				case ViewMode.Single:
					this.CreateUISingle (parent);
					break;

				case ViewMode.Event:
					this.CreateUIEvent (parent);
					break;

				case ViewMode.Multiple:
					this.CreateUIMultiple (parent);
					break;
			}
		}

		private void CreateUISingle(Widget parent)
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

			this.timelineFrameBox = new FrameBox
			{
				Parent  = parent,
				Dock    = DockStyle.Bottom,
				Margins = new Margins (0, 0, 10, 0),
			};

			this.listController.CreateUI (this.listFrameBox);
			this.timelineController.CreateUI (this.timelineFrameBox);

			this.CreateUIEdit (topBox);
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

				this.listController.UpdateView += delegate (object sender)
				{
					this.UpdateUI ();
				};

				this.listController.ChangeView += delegate (object sender, ViewType viewType)
				{
					this.OnChangeView (viewType);
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
					if (this.IsEditingPossible)
					{
						this.OnStartEdit ();
					}
				};
			}
		}

		private void CreateUIEvent(Widget parent)
		{
			var topBox = new FrameBox
			{
				Parent  = parent,
				Dock    = DockStyle.Fill,
			};

			this.listFrameBox = new FrameBox
			{
				Parent         = topBox,
				Dock           = DockStyle.Fill,
				PreferredWidth = LocalSettings.SplitterAssetsEventPos,
			};

			this.splitter = new VSplitter
			{
				Parent         = topBox,
				Dock           = DockStyle.Left,
				PreferredWidth = 10,
			};

			this.eventsFrameBox = new FrameBox
			{
				Parent  = topBox,
				Dock    = DockStyle.Fill,
				Margins = new Margins (0, 0, 0, AbstractScroller.DefaultBreadth),  // place pour StateAtController
			};

			this.listController.CreateUI (this.listFrameBox);
			this.eventsController.CreateUI (this.eventsFrameBox);

			this.closeButton = new IconButton
			{
				Parent        = parent,
				IconUri       = Misc.GetResourceIconUri ("TreeTable.Close"),
				AutoFocus     = false,
				Anchor        = AnchorStyles.TopRight,
				PreferredSize = new Size (AbstractCommandToolbar.secondaryToolbarHeight, AbstractCommandToolbar.secondaryToolbarHeight),
				Margins       = new Margins (0, 0, TopTitle.height, 0),
			};

			ToolTip.Default.SetToolTip (this.closeButton, Res.Strings.AssetsView.EventsClose.Tooltip.ToString ());

			this.CreateUIEdit (topBox);
			this.DeepUpdateUI ();

			this.splitter.SplitterDragged += delegate
			{
				LocalSettings.SplitterAssetsEventPos = (int) this.listFrameBox.PreferredWidth;
			};

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

				this.listController.UpdateView += delegate (object sender)
				{
					this.UpdateUI ();
				};

				this.listController.ChangeView += delegate (object sender, ViewType viewType)
				{
					this.OnChangeView (viewType);
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

			this.closeButton.Clicked += delegate
			{
				this.OnCloseColumn ();
			};
		}

		private void CreateUIMultiple(Widget parent)
		{
			this.timelinesArrayFrameBox = new FrameBox
			{
				Parent = parent,
				Dock   = DockStyle.Fill,
			};

			this.timelinesArrayController.CreateUI (this.timelinesArrayFrameBox);

			this.CreateUIEdit (parent);
			this.DeepUpdateUI ();

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
					if (this.IsEditingPossible)
					{
						this.OnStartEdit ();
					}
				};
			}
		}

		private void CreateUIEdit(Widget parent)
		{
			this.editFrameBox = new FrameBox
			{
				Parent         = parent,
				Dock           = DockStyle.Right,
				PreferredWidth = AbstractView.editionWidth,
				BackColor      = ColorManager.GetBackgroundColor (),
			};

			this.objectEditor.CreateUI (this.editFrameBox);

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
		}


		public override void DataChanged()
		{
			this.listController          .DirtyData = true;
			this.timelineController      .DirtyData = true;
			this.eventsController        .DirtyData = true;
			this.timelinesArrayController.DirtyData = true;
		}

		public override void DeepUpdateUI()
		{
			this.DataChanged ();
			this.UpdateUI ();
		}

		public override void UpdateUI()
		{
			Timestamp? curTimestamp = this.accessor.EditionAccessor.EditedTimestamp;
			Timestamp? newTimestamp;

			if (this.accessor.EditionAccessor.SaveObjectEdition (out newTimestamp))
			{
				this.DataChanged ();

				if (newTimestamp.HasValue && curTimestamp != newTimestamp)  // événement déplacé dans le temps ?
				{
					this.selectedTimestamp = newTimestamp;  // sélectionne l'événement à sa nouvelle position
				}
			}

			//	Met à jour la géométrie des différents contrôleurs.
			if (this.lastViewMode     != this.mainToolbar.ViewMode ||
				this.lastIsShowEvents != this.isShowEvents         ||
				this.lastIsEditing    != this.isEditing            )
			{
				this.UpdateViewModeGeometry ();

				if (this.editFrameBox.Window != null)
				{
					this.editFrameBox.Window.ForceLayout ();
				}

				this.lastViewMode     = this.mainToolbar.ViewMode;
				this.lastIsShowEvents = this.isShowEvents;
				this.lastIsEditing    = this.isEditing;
			}

			//	Met à jour les données des différents contrôleurs.
			using (this.ignoreChanges.Enter ())
			{
				if (this.mainToolbar.ViewMode == ViewMode.Single ||
					this.mainToolbar.ViewMode == ViewMode.Event)
				{
					this.listController.UpdateGraphicMode ();

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
						//	Si la liste des objets d'immobilisations est groupée selon les
						//	catégories d'immobilisations (par exemple), le même objet peut
						//	apparaître plusieurs fois dans la liste (à cause des ratios, 40%
						//	dans un groupe et 60% dans un autre).
						//	Lors d'une mise à jour après une recherche, il ne faut pas mettre
						//	à jour le SelectedGuid s'il n'a pas changé. Si on le fait, on
						//	sélectionne toujours la première ligne correspondant à l'objet,
						//	et on ne peut donc jamais atteindre la deuxième !

						if (this.listController.SelectedGuid != this.selectedGuid)
						{
							this.listController.SelectedGuid = this.selectedGuid;
						}

						if (this.listController.SelectedTimestamp != this.selectedTimestamp)
						{
							this.listController.SelectedTimestamp = this.selectedTimestamp;
						}

						//	Si l'objet sélectionné dans la liste de gauche a changé, il
						//	faudra aussi mettre à jour les contrôleurs qui en dépendent.
						this.timelineController.DirtyData = true;
						this.eventsController.DirtyData   = true;
					}
				}

				if (this.mainToolbar.ViewMode == ViewMode.Single)
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

				if (this.mainToolbar.ViewMode == ViewMode.Event)
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

				if (this.mainToolbar.ViewMode == ViewMode.Multiple)
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

			this.listController.HelplineVisibility = this.listController.HelplineDesired;

			this.UpdateToolbars ();
			this.UpdateEditor ();
			this.mainToolbar.UpdateWarningsRedDot ();

			this.OnViewStateChanged (this.ViewState);
		}


		public static AbstractViewState GetViewState(Guid assetGuid, Timestamp? timestamp, PageType pageType, ObjectField field)
		{
			//	Retourne un ViewState permettant de voir un objet à un instant donné.
			return new AssetsViewState
			{
				ViewType          = ViewType.Assets,
				PageType          = pageType,
				Field             = field,
				ViewMode          = ViewMode.Single,
				SelectedGuid      = assetGuid,
				SelectedTimestamp = timestamp,
				TimelinesMode     = TimelinesMode.Wide,
			};
		}

		public override AbstractViewState ViewState
		{
			get
			{
				return new AssetsViewState
				{
					ViewType            = ViewType.Assets,
					ViewMode            = this.mainToolbar.ViewMode,
					PageType            = this.isEditing ? this.objectEditor.PageType : PageType.Unknown,
					Field               = this.isEditing ? this.objectEditor.FocusField : ObjectField.Unknown,
					IsShowEvents        = this.isShowEvents,
					SelectedTimestamp   = this.selectedTimestamp,
					SelectedGuid        = this.selectedGuid,
					FilterTreeTableGuid = this.listController.FilterGuid,
					FilterTimelinesGuid = this.timelinesArrayController.FilterGuid,
					TimelinesMode       = this.timelinesArrayController.TimelinesMode,
					ShowGraphic         = this.listController.ShowGraphic,
				};
			}
			set
			{
				var viewState = value as AssetsViewState;
				System.Diagnostics.Debug.Assert (viewState != null);

				this.mainToolbar.ViewMode = viewState.ViewMode;
				this.isShowEvents         = viewState.IsShowEvents;
				this.selectedTimestamp    = viewState.SelectedTimestamp;
				this.selectedGuid         = viewState.SelectedGuid;

				this.listController.FilterGuid              = viewState.FilterTreeTableGuid;
				this.timelinesArrayController.FilterGuid    = viewState.FilterTimelinesGuid;
				this.timelinesArrayController.TimelinesMode = viewState.TimelinesMode;
				this.listController.ShowGraphic             = viewState.ShowGraphic;

				if (viewState.PageType == PageType.Unknown)
				{
					this.isEditing = false;
				}
				else
				{
					this.isEditing = true;
					this.objectEditor.SetPage (viewState.PageType, viewState.Field);
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


		protected override void OnMainEdit(Widget target)
		{
			if (!this.isEditing && this.selectedGuid.IsEmpty)
			{
				return;
			}

			this.isEditing = !this.isEditing;
			this.UpdateUI ();
		}

		protected override void OnEditAccept(Widget target)
		{
			this.isEditing = false;
			this.UpdateUI ();
		}

		protected override void OnEditCancel(Widget target)
		{
			this.accessor.EditionAccessor.CancelObjectEdition ();
			this.isEditing = false;
			this.UpdateUI ();
		}

		private void OnListDoubleClicked()
		{
			switch (this.mainToolbar.ViewMode)
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
				//	Si le popup pour la création d'un événement est présent (CreateEventPopup),
				//	il ne faut pas fermer la liste des événements, car c'est forcément le popup
				//	qui est à l'origine de la demande de mise à jour. Comme il s'affiche avec
				//	un lien sur le bouton de la commande source (Events.New), il ne faut pas
				//	que le bouton disparaisse !
				if (!PopupStack.HasPopup)
				{
					this.isShowEvents = false;  // ferme la liste des événements
					this.isEditing    = false;
				}
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
			if (this.mainToolbar.ViewMode == ViewMode.Event)
			{
				this.UpdateEventGeometry ();
			}

			this.editFrameBox.Visibility = this.isEditing;
		}

		private void UpdateEventGeometry()
		{
			if (!this.isShowEvents && !this.isEditing)
			{
				this.listFrameBox  .Visibility = true;
				this.splitter      .Visibility = false;
				this.eventsFrameBox.Visibility = false;

				this.listFrameBox.Dock = DockStyle.Fill;
			}
			else if (this.isShowEvents && !this.isEditing)
			{
				this.listFrameBox  .Visibility = true;
				this.splitter      .Visibility = true;
				this.eventsFrameBox.Visibility = true;

				this.listFrameBox.Dock = DockStyle.Left;
			}
			else if (this.isEditing)
			{
				this.listFrameBox  .Visibility = false;
				this.splitter      .Visibility = false;
				this.eventsFrameBox.Visibility = true;
			}
			else
			{
				System.Diagnostics.Debug.Fail ("Impossible statment");
			}

			this.closeButton.Visibility = this.isShowEvents || this.isEditing;
		}


		protected override bool IsEditingPossible
		{
			get
			{
				switch (this.mainToolbar.ViewMode)
				{
					case ViewMode.Single:
						return this.listController.SelectedRow != -1;

					case ViewMode.Event:
						return this.listController.SelectedRow != -1
							&& this.isShowEvents;

					case ViewMode.Multiple:
						return this.timelinesArrayController.HasSelectedEvent;

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
		private VSplitter									splitter;
		private FrameBox									timelineFrameBox;
		private FrameBox									eventsFrameBox;
		private FrameBox									timelinesArrayFrameBox;
		private FrameBox									editFrameBox;

		private IconButton									closeButton;

		private bool										isShowEvents;
		private Guid										selectedGuid;
		private Timestamp?									selectedTimestamp;

		private ViewMode									lastViewMode;
		private bool										lastIsShowEvents;
		private bool										lastIsEditing;
	}
}
