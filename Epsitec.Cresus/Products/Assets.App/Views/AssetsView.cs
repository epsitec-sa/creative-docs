﻿//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
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
			this.baseType = BaseType.Objects;

			this.timelinesArrayController = new TimelinesArrayController (this.accessor);
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

			this.timelinesArrayFrameBox = new FrameBox
			{
				Parent = topBox,
				Dock   = DockStyle.Fill,
			};

			this.timelinesArrayController.CreateUI (this.timelinesArrayFrameBox);
			this.timelinesArrayController.Filter = AssetsView.EventFilter;

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
			return e.Type == EventType.AmortissementAuto
				|| e.Type == EventType.AmortissementExtra;
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
				return new AssetsViewState
				{
					ViewType          = ViewType.Assets,
					SelectedTimestamp = this.selectedTimestamp,
					SelectedGuid      = this.selectedGuid,
				};
			}
			set
			{
				var viewState = value as AssetsViewState;
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

			this.mainToolbar.SetCommandState (ToolbarCommand.Amortissement, ToolbarCommandState.Enable);
		}


		private readonly TimelinesArrayController			timelinesArrayController;

		private FrameBox									timelinesArrayFrameBox;

		private Guid										selectedGuid;
		private Timestamp?									selectedTimestamp;
	}
}
