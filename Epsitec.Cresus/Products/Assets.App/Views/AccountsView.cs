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
using Epsitec.Cresus.Assets.Server.BusinessLogic;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.App.Views
{
	public class AccountsView : AbstractView, System.IDisposable
	{
		public AccountsView(DataAccessor accessor, CommandContext commandContext, MainToolbar toolbar, ViewType viewType, BaseType baseType)
			: base (accessor, commandContext, toolbar, viewType)
		{
			this.baseType = baseType;

			this.listController = new AccountsToolbarTreeTableController (this.accessor, this.commandContext, this.baseType);
			this.objectEditor   = new ObjectEditor (this.accessor, this.baseType, this.baseType, isTimeless: true);
		}


		public override void Dispose()
		{
			base.Dispose ();
		}

		public override void Close()
		{
			this.listController.Close ();
		}


		public override void CreateUI(Widget parent)
		{
			base.CreateUI (parent);

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

				this.listController.UpdateView += delegate (object sender)
				{
					this.UpdateUI ();
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
					this.listController.UpdateGraphicMode ();

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
			this.UpdateWarningsRedDot ();

			this.OnViewStateChanged (this.ViewState);
		}


		public static AbstractViewState GetViewState(DataAccessor accessor, System.DateTime date, string account)
		{
			//	Retourne un ViewState permettant de voir un compte donné à une date donnée.
			var range = accessor.Mandat.GetBestDateRange (date);

			return new AccountsViewState
			{
				ViewType        = new ViewType(ViewTypeKind.Accounts, range),
				SelectedAccount = account,
				ShowGraphic     = false,
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

				var baseType = new BaseType(BaseTypeKind.Accounts, viewState.ViewType.AccountsDateRange);
				var obj = AccountsLogic.GetAccount(this.accessor, baseType, viewState.SelectedAccount);
				if (obj == null)
				{
					this.selectedGuid = Guid.Empty;
				}
				else
				{
					this.selectedGuid = obj.Guid;
				}

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
			this.commandContext.GetCommandState (Res.Commands.Main.Edit  ).Visibility = false;
			this.commandContext.GetCommandState (Res.Commands.Edit.Accept).Visibility = false;
			this.commandContext.GetCommandState (Res.Commands.Edit.Cancel).Visibility = false;
		}


		private readonly AccountsToolbarTreeTableController listController;
		private readonly ObjectEditor						objectEditor;

		private FrameBox									listFrameBox;
		private FrameBox									editFrameBox;

		private Guid										selectedGuid;
	}
}
