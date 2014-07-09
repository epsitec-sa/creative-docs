//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.App.Views.CommandToolbars;
using Epsitec.Cresus.Assets.App.Views.ViewStates;
using Epsitec.Cresus.Assets.App.Widgets;
using Epsitec.Cresus.Assets.Data;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.App.Views
{
	public class AmortizationsView : AbstractView
	{
		public AmortizationsView(DataAccessor accessor, MainToolbar toolbar)
			: base (accessor, toolbar)
		{
			this.baseType = BaseType.Assets;

			this.timelinesArrayController = new TimelinesArrayController (this.accessor)
			{
				Title                = this.GetViewTitle (ViewType.Amortizations),
				HasAmortizationsOper = true,
				Filter               = AmortizationsView.EventFilter,
			};

			this.objectEditor = new ObjectEditor (this.accessor, this.baseType, this.baseType, isTimeless: false);
		}


		public override void Dispose()
		{
			this.mainToolbar.SetCommandState (ToolbarCommand.Edit,   ToolbarCommandState.Hide);
			this.mainToolbar.SetCommandState (ToolbarCommand.Accept, ToolbarCommandState.Hide);
			this.mainToolbar.SetCommandState (ToolbarCommand.Cancel, ToolbarCommandState.Hide);
		}


		public override void CreateUI(Widget parent)
		{
			if (!this.objectEditor.HasError && this.accessor.EditionAccessor.SaveObjectEdition ())
			{
				this.DataChanged ();
			}

			var topBox = new FrameBox
			{
				Parent  = parent,
				Dock    = DockStyle.Fill,
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

			this.timelinesArrayController.CreateUI (this.timelinesArrayFrameBox);

			this.objectEditor.CreateUI (this.editFrameBox);

			this.lastIsEditing = true;  // pour forcer UpdateViewModeGeometry !
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
		}

		private static bool EventFilter(DataEvent e)
		{
			return e.Type == EventType.AmortizationAuto
				|| e.Type == EventType.AmortizationPreview
				|| e.Type == EventType.AmortizationExtra;
		}


		public override void DataChanged()
		{
			this.timelinesArrayController.DirtyData = true;
		}

		public override void DeepUpdateUI()
		{
			this.DataChanged ();
			this.UpdateUI ();
		}

		public override void UpdateUI()
		{
			if (!this.objectEditor.HasError && this.accessor.EditionAccessor.SaveObjectEdition ())
			{
				this.DataChanged ();
			}

			//	Met à jour la géométrie des différents contrôleurs.
			if (this.lastIsEditing != this.isEditing)
			{
				this.UpdateViewModeGeometry ();
				this.editFrameBox.Window.ForceLayout ();

				this.lastIsEditing = this.isEditing;
			}

			//	Met à jour les données des différents contrôleurs.
			using (this.ignoreChanges.Enter ())
			{
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
				return new AmortizationsViewState
				{
					ViewType          = ViewType.Amortizations,
					PageType          = this.isEditing ? this.objectEditor.PageType : PageType.Unknown,
					SelectedTimestamp = this.selectedTimestamp,
					SelectedGuid      = this.selectedGuid,
				};
			}
			set
			{
				var viewState = value as AmortizationsViewState;
				System.Diagnostics.Debug.Assert (viewState != null);

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
			}
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

		private void OnStartEdit(EventType eventType, Timestamp? timestamp = null)
		{
			this.isEditing = true;
			this.selectedTimestamp = timestamp;
			this.objectEditor.PageType = this.objectEditor.MainPageType;

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

		private void UpdateAfterMultipleChanged()
		{
			this.selectedGuid      = this.timelinesArrayController.SelectedGuid;
			this.selectedTimestamp = this.timelinesArrayController.SelectedTimestamp;

			this.UpdateUI ();
		}

		private void UpdateEditor()
		{
			this.objectEditor.SetObject (this.selectedGuid, this.selectedTimestamp);
		}

		private void UpdateViewModeGeometry()
		{
			this.timelinesArrayController.InUse = true;

			this.editFrameBox.Visibility = this.isEditing;
			this.timelinesArrayFrameBox.Visibility = true;

			this.editFrameBox.Dock = DockStyle.Right;
		}


		private void UpdateToolbars()
		{
			if (this.isEditing)
			{
				this.mainToolbar.SetCommandState  (ToolbarCommand.Edit,   ToolbarCommandState.Activate);
				this.mainToolbar.SetCommandEnable (ToolbarCommand.Accept, !this.objectEditor.HasError);
				this.mainToolbar.SetCommandState  (ToolbarCommand.Cancel, ToolbarCommandState.Enable);
			}
			else
			{
				this.mainToolbar.SetCommandEnable (ToolbarCommand.Edit,   this.IsEditingPossible);
				this.mainToolbar.SetCommandState  (ToolbarCommand.Accept, ToolbarCommandState.Hide);
				this.mainToolbar.SetCommandState  (ToolbarCommand.Cancel, ToolbarCommandState.Hide);
			}
		}

		private bool IsEditingPossible
		{
			get
			{
				return !this.timelinesArrayController.SelectedGuid.IsEmpty
					&& this.timelinesArrayController.SelectedTimestamp != null;
			}
		}


		private readonly TimelinesArrayController			timelinesArrayController;
		private readonly ObjectEditor						objectEditor;

		private FrameBox									timelinesArrayFrameBox;
		private FrameBox									editFrameBox;

		private bool										isEditing;
		private Guid										selectedGuid;
		private Timestamp?									selectedTimestamp;

		private bool										lastIsEditing;
	}
}
