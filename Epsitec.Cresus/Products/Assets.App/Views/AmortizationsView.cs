//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.App.Popups;
using Epsitec.Cresus.Assets.Server.BusinessLogic;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.App.Views
{
	public class AmortizationsView : AbstractView
	{
		public AmortizationsView(DataAccessor accessor, MainToolbar toolbar)
			: base (accessor, toolbar)
		{
			this.baseType = BaseType.Objects;

			this.timelinesArrayController = new TimelinesArrayController (this.accessor)
			{
				HasAmortizationsToolbar = true,
			};
		}


		public override void Dispose()
		{
			this.mainToolbar.SetCommandState (ToolbarCommand.Edit,         ToolbarCommandState.Hide);
			this.mainToolbar.SetCommandState (ToolbarCommand.Amortization, ToolbarCommandState.Hide);
			this.mainToolbar.SetCommandState (ToolbarCommand.Accept,       ToolbarCommandState.Hide);
			this.mainToolbar.SetCommandState (ToolbarCommand.Cancel,       ToolbarCommandState.Hide);
		}


		public override void CreateUI(Widget parent)
		{
			var topBox = new FrameBox
			{
				Parent  = parent,
				Dock    = DockStyle.Fill,
			};

			var timelinesArrayFrameBox = new FrameBox
			{
				Parent = topBox,
				Dock   = DockStyle.Fill,
			};

			this.timelinesArrayController.CreateUI (timelinesArrayFrameBox);
			this.timelinesArrayController.Filter = AmortizationsView.EventFilter;

			this.timelinesArrayController.AmortizationsToolbar.SetCommandEnable (ToolbarCommand.AmortizationsPreview,   true);
			this.timelinesArrayController.AmortizationsToolbar.SetCommandEnable (ToolbarCommand.AmortizationsFix,       true);
			this.timelinesArrayController.AmortizationsToolbar.SetCommandEnable (ToolbarCommand.AmortizationsUnpreview, true);
			this.timelinesArrayController.AmortizationsToolbar.SetCommandEnable (ToolbarCommand.AmortizationsDelete,    true);
			this.timelinesArrayController.AmortizationsToolbar.SetCommandEnable (ToolbarCommand.AmortizationsInfo,      true);

			this.DeepUpdateUI ();

			//	Connexion des événements du tableau des objets et timelines.
			{
				this.timelinesArrayController.StartEditing += delegate (object sender, EventType eventType, Timestamp timestamp)
				{
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
				};
			}

			{
				var toolbar = this.timelinesArrayController.AmortizationsToolbar;
				toolbar.CommandClicked += delegate (object sender, ToolbarCommand command)
				{
					var target = toolbar.GetTarget (command);

					switch (command)
					{
						case ToolbarCommand.AmortizationsPreview:
							this.ShowAmortizationsPopup (target, true, true, "Générer l'aperçu des amortissements", "Générer pour un", "Générer pour tous", this.PreviewAmortisations);
							break;

						case ToolbarCommand.AmortizationsFix:
							this.ShowAmortizationsPopup (target, false, false, "Fixer l'aperçu des amortissements", "Fixer pour un", "Fixer pour tous", this.FixAmortisations);
							break;

						case ToolbarCommand.AmortizationsUnpreview:
							this.ShowAmortizationsPopup (target, false, false, "Supprimer l'aperçu des amortissements", "Supprimer pour un", "Supprimer pour tous", this.UnpreviewAmortisations);
							break;

						case ToolbarCommand.AmortizationsDelete:
							this.ShowAmortizationsPopup (target, true, false, "Supprimer des amortissements automatiques", "Supprimer pour un", "Supprimer pour tous", this.DeleteAmortisations);
							break;

						case ToolbarCommand.AmortizationsInfo:
							this.ShowErrorPopup (target, this.errors);
							break;
					}
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

			this.timelinesArrayController.InUse = true;

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

			this.OnViewStateChanged (this.ViewState);
		}


		public override AbstractViewState ViewState
		{
			get
			{
				return new AmortizationsViewState
				{
					ViewType          = ViewType.Amortizations,
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


		private void ShowAmortizationsPopup(Widget target, bool fromAllowed, bool toAllowed, string title, string one, string all, System.Action<DateRange, bool> action)
		{
			var now = System.DateTime.Now;

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
					var range = new DateRange (popup.DateFrom.Value, popup.DateTo.Value);

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


		private void UpdateAfterMultipleChanged()
		{
			this.selectedGuid      = this.timelinesArrayController.SelectedGuid;
			this.selectedTimestamp = this.timelinesArrayController.SelectedTimestamp;

			this.UpdateUI ();
		}


		private void UpdateToolbars()
		{
			this.mainToolbar.SetCommandEnable (ToolbarCommand.Edit, false);

			this.mainToolbar.SetCommandState (ToolbarCommand.Accept, ToolbarCommandState.Hide);
			this.mainToolbar.SetCommandState (ToolbarCommand.Cancel, ToolbarCommandState.Hide);

			this.mainToolbar.SetCommandState (ToolbarCommand.Amortization, ToolbarCommandState.Enable);
		}


		private static System.DateTime lastDateFrom = new System.DateTime (System.DateTime.Now.Year, 1, 1);
		private static System.DateTime lastDateTo   = new System.DateTime (System.DateTime.Now.Year, 12, 31);

		private readonly TimelinesArrayController			timelinesArrayController;

		private Guid										selectedGuid;
		private Timestamp?									selectedTimestamp;
		private List<Error>									errors;
	}
}
