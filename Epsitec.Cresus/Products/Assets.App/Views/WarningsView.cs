//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.App.Views.CommandToolbars;
using Epsitec.Cresus.Assets.App.Views.ToolbarControllers;
using Epsitec.Cresus.Assets.App.Views.ViewStates;
using Epsitec.Cresus.Assets.Data;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.App.Views
{
	public class WarningsView : AbstractView, System.IDisposable
	{
		public WarningsView(DataAccessor accessor, CommandContext commandContext, MainToolbar toolbar, ViewType viewType)
			: base (accessor, commandContext, toolbar, viewType)
		{
			this.baseType = BaseType.Persons;

			this.listController = new WarningsToolbarTreeTableController (this.accessor, this.commandDispatcher, this.commandContext, BaseType.Persons);
		}

		public override void Dispose()
		{
			if (this.listController != null)
			{
				this.listController.Dispose ();
			}

			base.Dispose ();
		}

		public override void Close()
		{
			this.listController.Close ();
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

				this.listController.RowDoubleClicked += delegate
				{
					this.OnListDoubleClicked ();
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
			this.listController.InUse = true;

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


		public override AbstractViewState ViewState
		{
			get
			{
				return new WarningsViewState
				{
					ViewType               = ViewType.Warnings,
					PersistantUniqueId     = this.listController.SelectedPersistantUniqueId,
					NextPersistantUniqueId = this.listController.NextPersistantUniqueId,
					PrevPersistantUniqueId = this.listController.PrevPersistantUniqueId,
				};
			}
			set
			{
				var viewState = value as WarningsViewState;
				System.Diagnostics.Debug.Assert (viewState != null);

				this.listController.SelectedPersistantUniqueId = viewState.PersistantUniqueId;

				//	Si on n'a pas pu sélectionner l'avertissement précédemment sélectionné,
				//	c'est probablement qu'il a été corrigé. On essaie alors de sélectionner
				//	l'avertissement suivant.
				if (this.listController.SelectedGuid.IsEmpty)
				{
					this.listController.SelectedPersistantUniqueId = viewState.NextPersistantUniqueId;
				}

				//	Si on n'a toujours pas réussi, on essaie alors de sélectionner
				//	l'avertissement précédent.
				if (this.listController.SelectedGuid.IsEmpty)
				{
					this.listController.SelectedPersistantUniqueId = viewState.PrevPersistantUniqueId;
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


		private void OnListDoubleClicked()
		{
			this.selectedGuid = this.listController.SelectedGuid;
			var viewState = this.listController.Goto (this.selectedGuid);

			if (viewState != null)
			{
				this.OnGoto (viewState);
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


		private readonly WarningsToolbarTreeTableController	listController;

		private FrameBox									listFrameBox;
		private Guid										selectedGuid;
	}
}
