//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.App.Views
{
	public class AmortizationsView : AbstractView
	{
		public AmortizationsView(DataAccessor accessor, MainToolbar toolbar)
			: base (accessor, toolbar)
		{
			this.baseType = BaseType.Objects;

			this.timelinesArrayController = new TimelinesArrayController (this.accessor);

			this.amortizationController = new AmortizationController (this.accessor)
			{
				Date = new System.DateTime (System.DateTime.Now.Year, 12, 31),
			};
		}


		public override void Dispose()
		{
			this.mainToolbar.SetCommandState (ToolbarCommand.Edit,          ToolbarCommandState.Hide);
			this.mainToolbar.SetCommandState (ToolbarCommand.Amortization, ToolbarCommandState.Hide);
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

			var timelinesArrayFrameBox = new FrameBox
			{
				Parent = topBox,
				Dock   = DockStyle.Fill,
			};

			var controllerFrameBox = new FrameBox
			{
				Parent = topBox,
				Dock   = DockStyle.Bottom,
			};

			this.timelinesArrayController.CreateUI (timelinesArrayFrameBox);
			this.timelinesArrayController.Filter = AmortizationsView.EventFilter;

			this.amortizationController.CreateUI (controllerFrameBox);

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
		}

		private static bool EventFilter(DataEvent e)
		{
			return e.Type == EventType.AmortizationAuto
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


		private readonly AmortizationController				amortizationController;
		private readonly TimelinesArrayController			timelinesArrayController;

		private Guid										selectedGuid;
		private Timestamp?									selectedTimestamp;
	}
}
