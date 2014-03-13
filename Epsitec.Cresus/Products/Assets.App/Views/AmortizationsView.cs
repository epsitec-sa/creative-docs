//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.App.Popups;
using Epsitec.Cresus.Assets.App.Widgets;
using Epsitec.Cresus.Assets.Server.BusinessLogic;
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
				Title                   = StaticDescriptions.GetViewTypeDescription (ViewType.Amortizations),
				HasAmortizationsToolbar = true,
				Filter                  = AmortizationsView.EventFilter,
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

			this.timelinesArrayController.AmortizationsToolbar.SetCommandEnable (ToolbarCommand.AmortizationsPreview,   true);
			this.timelinesArrayController.AmortizationsToolbar.SetCommandEnable (ToolbarCommand.AmortizationsFix,       true);
			this.timelinesArrayController.AmortizationsToolbar.SetCommandEnable (ToolbarCommand.AmortizationsToExtra,   true);
			this.timelinesArrayController.AmortizationsToolbar.SetCommandEnable (ToolbarCommand.AmortizationsUnpreview, true);
			this.timelinesArrayController.AmortizationsToolbar.SetCommandEnable (ToolbarCommand.AmortizationsDelete,    true);
			this.timelinesArrayController.AmortizationsToolbar.SetCommandEnable (ToolbarCommand.AmortizationsInfo,      true);

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

			//	Connexion des événements de la toolbar des amortissements.
			{
				var toolbar = this.timelinesArrayController.AmortizationsToolbar;
				toolbar.CommandClicked += delegate (object sender, ToolbarCommand command)
				{
					var target = toolbar.GetTarget (command);

					switch (command)
					{
						case ToolbarCommand.AmortizationsPreview:
							this.ShowAmortizationsPopup (target, true, true,
								"Générer l'aperçu des amortissements ordinaires",
								"Générer pour un",
								"Générer pour tous",
								this.PreviewAmortisations);
							break;

						case ToolbarCommand.AmortizationsFix:
							this.ShowAmortizationsPopup (target, false, false,
								"Fixer l'aperçu des amortissements ordinaires",
								"Fixer pour un",
								"Fixer pour tous",
								this.FixAmortisations);
							break;

						case ToolbarCommand.AmortizationsToExtra:
							this.TransformToExtra ();
							break;

						case ToolbarCommand.AmortizationsUnpreview:
							this.ShowAmortizationsPopup (target, false, false,
								"Supprimer l'aperçu des amortissements ordinaires",
								"Supprimer pour un",
								"Supprimer pour tous",
								this.UnpreviewAmortisations);
							break;

						case ToolbarCommand.AmortizationsDelete:
							this.ShowAmortizationsPopup (target, true, false,
								"Supprimer des amortissements ordinaires",
								"Supprimer pour un",
								"Supprimer pour tous",
								this.DeleteAmortisations);
							break;

						case ToolbarCommand.AmortizationsInfo:
							this.ShowErrorPopup (target, this.errors);
							break;
					}
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
			if (this.accessor.EditionAccessor.SaveObjectEdition ())
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


		private void PreviewAmortisations(DateRange processRange, bool allObjects)
		{
			if (allObjects)
			{
				this.errors = this.amortizations.Preview (processRange);
			}
			else
			{
				this.errors = this.amortizations.Create (processRange, this.SelectedGuid);
			}

			this.DeepUpdateUI ();
		}

		private void FixAmortisations(DateRange processRange, bool allObjects)
		{
			if (allObjects)
			{
				this.errors = this.amortizations.Fix ();
			}
			else
			{
				this.errors = this.amortizations.Fix (this.SelectedGuid);
			}

			this.DeepUpdateUI ();
		}

		private void UnpreviewAmortisations(DateRange processRange, bool allObjects)
		{
			if (allObjects)
			{
				this.errors = this.amortizations.Unpreview ();
			}
			else
			{
				this.errors = this.amortizations.Unpreview (this.SelectedGuid);
			}

			this.DeepUpdateUI ();
		}

		private void DeleteAmortisations(DateRange processRange, bool allObjects)
		{
			if (allObjects)
			{
				this.errors = this.amortizations.Delete (processRange.IncludeFrom);
			}
			else
			{
				this.errors = this.amortizations.Delete (processRange.IncludeFrom, this.SelectedGuid);
			}

			this.DeepUpdateUI ();
		}

		private void TransformToExtra()
		{
			//	Transforme un amortissement ordinaire en extraordinaire.
			if (!this.selectedGuid.IsEmpty && this.selectedTimestamp.HasValue)
			{
				var asset = this.accessor.GetObject (BaseType.Assets, this.selectedGuid);
				if (asset != null)
				{
					var e = asset.GetEvent (this.selectedTimestamp.Value);
					if (e != null)
					{
						//	Supprime l'amortissement ordinaire.
						asset.RemoveEvent (e);

						//	Crée un amortissement extraordinaire.
						var newEvent = new DataEvent (e.Guid, e.Timestamp, EventType.AmortizationExtra);
						newEvent.SetProperties (e);
						asset.AddEvent (newEvent);

						this.DeepUpdateUI ();
					}
				}
			}
		}


		private void ShowAmortizationsPopup(Widget target, bool fromAllowed, bool toAllowed, string title, string one, string all, System.Action<DateRange, bool> action)
		{
			var popup = new AmortizationsPopup (this.accessor)
			{
				Title               = title,
				ActionOne           = one,
				ActionAll           = all,
				DateFromAllowed     = fromAllowed,
				DateToAllowed       = toAllowed,
				OneSelectionAllowed = !this.SelectedGuid.IsEmpty,
				DateFrom            = AmortizationsView.lastDateFrom,
				DateTo              = AmortizationsView.lastDateTo,
			};

			popup.Create (target);

			popup.ButtonClicked += delegate (object sender, string name)
			{
				if (name == "ok")
				{
					System.Diagnostics.Debug.Assert (popup.DateFrom.HasValue);
					System.Diagnostics.Debug.Assert (popup.DateTo.HasValue);
					var range = new DateRange (popup.DateFrom.Value, popup.DateTo.Value.AddDays (1));

					AmortizationsView.lastDateFrom = popup.DateFrom.Value;
					AmortizationsView.lastDateTo   = popup.DateTo.Value;

					action (range, popup.IsAll);
				}
			};
		}

		private void ShowErrorPopup(Widget target, List<Error> errors)
		{
			if (errors != null)
			{
				var popup = new ErrorsPopup (this.accessor, errors);
				popup.Create (target);
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
				this.timelinesArrayController.AmortizationsToolbar.SetCommandEnable (ToolbarCommand.AmortizationsToExtra, false);

				this.mainToolbar.SetCommandState (ToolbarCommand.Edit, ToolbarCommandState.Activate);

				this.mainToolbar.SetCommandEnable (ToolbarCommand.Accept, this.objectEditor.EditionDirty);
				this.mainToolbar.SetCommandState (ToolbarCommand.Cancel, ToolbarCommandState.Enable);
			}
			else
			{
				this.timelinesArrayController.AmortizationsToolbar.SetCommandEnable (ToolbarCommand.AmortizationsToExtra, this.IsToExtraPossible);

				this.mainToolbar.SetCommandEnable (ToolbarCommand.Edit, this.IsEditingPossible);

				this.mainToolbar.SetCommandState (ToolbarCommand.Accept, ToolbarCommandState.Hide);
				this.mainToolbar.SetCommandState (ToolbarCommand.Cancel, ToolbarCommandState.Hide);
			}
		}

		private bool IsToExtraPossible
		{
			get
			{
				if (!this.selectedGuid.IsEmpty && this.selectedTimestamp.HasValue)
				{
					var asset = this.accessor.GetObject (BaseType.Assets, this.selectedGuid);
					if (asset != null)
					{
						var e = asset.GetEvent (this.selectedTimestamp.Value);
						if (e != null)
						{
							return e.Type == EventType.AmortizationPreview
								|| e.Type == EventType.AmortizationAuto;
						}
					}
				}

				return false;
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


		private static System.DateTime lastDateFrom = new System.DateTime (System.DateTime.Now.Year, 1, 1);
		private static System.DateTime lastDateTo   = new System.DateTime (System.DateTime.Now.Year, 12, 31);

		private readonly TimelinesArrayController			timelinesArrayController;
		private readonly ObjectEditor						objectEditor;

		private FrameBox									timelinesArrayFrameBox;
		private FrameBox									editFrameBox;

		private bool										isEditing;
		private Guid										selectedGuid;
		private Timestamp?									selectedTimestamp;
		private List<Error>									errors;

		private bool										lastIsEditing;
	}
}
