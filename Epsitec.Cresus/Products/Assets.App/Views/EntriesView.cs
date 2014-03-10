//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.App.Views
{
	public class EntriesView : AbstractView
	{
		public EntriesView(DataAccessor accessor, MainToolbar toolbar)
			: base (accessor, toolbar)
		{
			this.listController = new EntriesToolbarTreeTableController (this.accessor, this.baseType);
		}


		public override void Dispose()
		{
			this.mainToolbar.SetCommandState (ToolbarCommand.Edit,   ToolbarCommandState.Hide);
			this.mainToolbar.SetCommandState (ToolbarCommand.Accept, ToolbarCommandState.Hide);
			this.mainToolbar.SetCommandState (ToolbarCommand.Cancel, ToolbarCommandState.Hide);
		}


		public override void CreateUI(Widget parent)
		{
			this.listFrameBox = new FrameBox
			{
				Parent = parent,
				Dock   = DockStyle.Fill,
			};

			this.listController.CreateUI (this.listFrameBox);

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
			}
		}


		public override void DataChanged()
		{
			this.listController.DirtyData = true;
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

			this.listController.InUse = true;

			//	Met à jour les données des différents contrôleurs.
			using (this.ignoreChanges.Enter ())
			{
				if (this.listController.InUse)
				{
					if (this.listController.DirtyData)
					{
						this.listController.UpdateData ();
						this.listController.SelectedGuid = this.selectedGuid;

						this.listController.DirtyData = false;
					}
					else if (this.listController.SelectedGuid != this.selectedGuid)
					{
						this.listController.SelectedGuid = this.selectedGuid;
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
				return new EntriesViewState
				{
					ViewType     = ViewType.Entries,
					SelectedGuid = this.selectedGuid,
					ShowGraphic  = this.listController.ShowGraphic,
				};
			}
			set
			{
				var viewState = value as EntriesViewState;
				System.Diagnostics.Debug.Assert (viewState != null);

				this.selectedGuid = viewState.SelectedGuid;
				this.listController.ShowGraphic = viewState.ShowGraphic;

				this.UpdateUI ();
			}
		}


		private void UpdateAfterListChanged()
		{
			this.selectedGuid = this.listController.SelectedGuid;

			this.UpdateUI ();
		}


		private void UpdateToolbars()
		{
		}


		private readonly EntriesToolbarTreeTableController	listController;

		private FrameBox									listFrameBox;
		private Guid										selectedGuid;
	}
}
