//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Drawing;
using Epsitec.Common.Support;
using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.App.Popups;
using Epsitec.Cresus.Assets.App.Widgets;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.App.Views
{
	public class ObjectsView : AbstractView
	{
		public ObjectsView(DataAccessor accessor, BaseType baseType, MainToolbar toolbar)
			: base (accessor, toolbar)
		{
			this.baseType = baseType;

			this.listController           = new ObjectsToolbarTreeTableController (this.accessor, this.baseType);
			this.timelineController       = new ObjectsToolbarTimelineController (this.accessor, this.baseType);
			this.eventsController         = new EventsToolbarTreeTableController (this.accessor, this.baseType);
			this.timelinesArrayController = new TimelinesArrayController (this.accessor, this.baseType);
			this.objectEditor             = new ObjectEditor (this.accessor, this.baseType);

			this.ignoreChanges = new SafeCounter ();
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
				PreferredSize = new Size (AbstractCommandToolbar.SecondaryToolbarHeight, AbstractCommandToolbar.SecondaryToolbarHeight),
				Margins       = new Margins (0, 0, TopTitle.Height, 0),
			};

			this.Update ();

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
			}

			this.closeButton.Clicked += delegate
			{
				this.OnCloseColumn ();
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
			this.viewMode = viewMode;
			this.UpdateViewModeGeometry ();

			this.editFrameBox.Window.ForceLayout ();

			using (this.ignoreChanges.Enter ())
			{
				if (this.viewMode == ViewMode.Single)
				{
					this.listController.UpdateData ();
					this.listController.SelectedGuid = this.selectedGuid;
					this.listController.Timestamp = this.selectedTimestamp;

					this.timelineController.ObjectGuid = this.selectedGuid;
					this.timelineController.SelectedTimestamp = this.selectedTimestamp;
				}
				else if (this.viewMode == ViewMode.Event)
				{
					this.listController.UpdateData ();
					this.listController.SelectedGuid = this.selectedGuid;
					this.listController.Timestamp = new Timestamp (System.DateTime.MaxValue, 0);

					this.eventsController.ObjectGuid = this.selectedGuid;
					this.eventsController.SelectedTimestamp = this.selectedTimestamp;
				}
				else if (this.viewMode == ViewMode.Multiple)
				{
					this.timelinesArrayController.UpdateData ();
					this.timelinesArrayController.SelectedGuid = this.selectedGuid;
					this.timelinesArrayController.SelectedTimestamp = this.selectedTimestamp;
				}
			}
		}

		private void OnListDoubleClicked()
		{
			if (this.viewMode == ViewMode.Single)
			{
				this.OnStartEdit ();
			}
			else if (this.viewMode == ViewMode.Event)
			{
				this.isShowEvents = !this.isShowEvents;
				this.isEditing = false;
				this.Update ();
			}
			else if (this.viewMode == ViewMode.Multiple)
			{
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
				if (this.viewMode == ViewMode.Single)
				{
					this.timelineController.SelectedTimestamp = timestamp.Value;
				}
				else if (this.viewMode == ViewMode.Event)
				{
					this.eventsController.SelectedTimestamp = timestamp.Value;
				}
				else if (this.viewMode == ViewMode.Multiple)
				{
					this.timelinesArrayController.SelectedTimestamp = timestamp.Value;
				}
			}

			this.objectEditor.OpenMainPage (eventType);
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
							var guid = this.accessor.GetObjectGuids (this.baseType, this.listController.SelectedRow, 1).First ();
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
			this.accessor.CancelObjectEdition ();
			this.isEditing = false;
			this.Update ();
		}


		protected override void Update()
		{
			if (!this.isEditing)
			{
				this.accessor.SaveObjectEdition ();
			}

			this.UpdateViewModeGeometry ();
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
				this.selectedGuid = this.accessor.GetObjectGuids (this.baseType, row, 1).First ();
			}

			this.timelineController.ObjectGuid = this.selectedGuid;

			if (!this.eventsController.DataFreezed)
			{
				this.eventsController.ObjectGuid = this.selectedGuid;
			}

			//	On sélectionne le dernier événement de l'objet dans la timeline.
			var obj = this.accessor.GetObject (this.baseType, this.selectedGuid);
			if (obj != null)
			{
				var timestamp = ObjectCalculator.GetLastTimestamp (obj);
				if (timestamp.HasValue)
				{
					this.selectedTimestamp = timestamp;
				}
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

			this.listController.Timestamp = this.selectedTimestamp;

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


		private void UpdateEditor()
		{
			this.objectEditor.SetObject (this.selectedGuid, this.selectedTimestamp);
		}


		private void UpdateViewModeGeometry()
		{
			if (this.viewMode == ViewMode.Single)
			{
				this.UpdateSingleGeometry ();
			}
			else if (this.viewMode == ViewMode.Event)
			{
				this.UpdateEventGeometry ();
			}
			else if (this.viewMode == ViewMode.Multiple)
			{
				this.UpdateMultipleGeometry ();
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

				this.mainToolbar.UpdateCommand (ToolbarCommand.Accept, this.objectEditor.EditionDirty);
				this.mainToolbar.SetCommandState (ToolbarCommand.Cancel, ToolbarCommandState.Enable);
			}
			else
			{
				this.mainToolbar.UpdateCommand (ToolbarCommand.Edit, this.IsEditingPossible);

				this.mainToolbar.SetCommandState (ToolbarCommand.Accept, ToolbarCommandState.Hide);
				this.mainToolbar.SetCommandState (ToolbarCommand.Cancel, ToolbarCommandState.Hide);
			}

			this.mainToolbar.SetCommandState (ToolbarCommand.Amortissement, ToolbarCommandState.Enable);
		}

		private bool IsEditingPossible
		{
			get
			{
				if (this.viewMode == ViewMode.Single)
				{
					return this.listController.SelectedRow != -1;
				}
				else if (this.viewMode == ViewMode.Event)
				{
					return this.listController.SelectedRow != -1
						&& this.isShowEvents;
				}
				else if (this.viewMode == ViewMode.Multiple)
				{
					return !this.timelinesArrayController.SelectedGuid.IsEmpty
						&& this.timelinesArrayController.SelectedTimestamp != null;
				}

				return false;
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
