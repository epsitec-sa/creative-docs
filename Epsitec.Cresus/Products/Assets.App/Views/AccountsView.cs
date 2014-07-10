//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.App.Views.CommandToolbars;
using Epsitec.Cresus.Assets.App.Views.Editors;
using Epsitec.Cresus.Assets.App.Views.ToolbarControllers;
using Epsitec.Cresus.Assets.App.Views.ViewStates;
using Epsitec.Cresus.Assets.App.Widgets;
using Epsitec.Cresus.Assets.Data;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.App.Views
{
	public class AccountsView : AbstractView
	{
		public AccountsView(DataAccessor accessor, MainToolbar toolbar, BaseType baseType)
			: base (accessor, toolbar)
		{
			this.baseType = baseType;

			this.listController = new AccountsToolbarTreeTableController (this.accessor, this.baseType);
			this.objectEditor   = new ObjectEditor (this.accessor, this.baseType, this.baseType, isTimeless: true);
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

			this.listFrameBox = new FrameBox
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

			this.listController.CreateUI (this.listFrameBox);
			this.objectEditor.CreateUI (this.editFrameBox);

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

				this.listController.ChangeView += delegate (object sender, ViewType viewType)
				{
					this.OnChangeView (viewType);
				};
			}

			//	Connexion des événements de l'éditeur.
			{
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
			this.listController.DirtyData = true;
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

			this.listController.InUse = true;
			this.editFrameBox.Visibility = false;

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


		public static AbstractViewState GetViewState(string account)
		{
			//	Retourne un ViewState permettant de voir un compte donné.
			return new AccountsViewState
			{
				ViewType    = ViewType.Accounts,
				ShowGraphic = false,
			};
		}


		public override AbstractViewState ViewState
		{
			get
			{
				return new AccountsViewState
				{
					ViewType    = new ViewType (ViewTypeKind.Accounts, this.baseType.AccountsDateRange),
					ShowGraphic = this.listController.ShowGraphic,
				};
			}
			set
			{
				var viewState = value as AccountsViewState;
				System.Diagnostics.Debug.Assert (viewState != null);

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
			this.mainToolbar.SetCommandState (ToolbarCommand.Edit,   ToolbarCommandState.Hide);
			this.mainToolbar.SetCommandState (ToolbarCommand.Accept, ToolbarCommandState.Hide);
			this.mainToolbar.SetCommandState (ToolbarCommand.Cancel, ToolbarCommandState.Hide);
		}


		private readonly AccountsToolbarTreeTableController listController;
		private readonly ObjectEditor						objectEditor;

		private FrameBox									listFrameBox;
		private FrameBox									editFrameBox;

		private Guid										selectedGuid;
	}
}
