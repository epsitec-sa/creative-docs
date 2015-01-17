//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.App.Views.CommandToolbars;
using Epsitec.Cresus.Assets.App.Views.ToolbarControllers;
using Epsitec.Cresus.Assets.App.Views.ViewStates;
using Epsitec.Cresus.Assets.Data;
using Epsitec.Cresus.Assets.Server.BusinessLogic;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.App.Views
{
	public class EntriesView : AbstractView, System.IDisposable
	{
		public EntriesView(DataAccessor accessor, CommandContext commandContext, MainToolbar toolbar, ViewType viewType)
			: base (accessor, commandContext, toolbar, viewType)
		{
			this.listController = new EntriesToolbarTreeTableController (this.accessor, this.commandContext, this.baseType);
		}


		public override void Dispose()
		{
			this.listController.Dispose ();
			base.Dispose ();
		}


		public override void CreateUI(Widget parent)
		{
			base.CreateUI (parent);

			if (this.accessor.EditionAccessor.SaveObjectEdition ())
			{
				this.DataChanged ();
			}

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

				this.listController.RowDoubleClicked += delegate
				{
					if (this.ignoreChanges.IsZero)
					{
						this.UpdateAfterListChanged ();
						this.GotoAsset ();
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

			//	Met à jour les données des différents contrôleurs.
			using (this.ignoreChanges.Enter ())
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

			this.UpdateToolbars ();
			this.mainToolbar.UpdateWarningsRedDot ();

			this.OnViewStateChanged (this.ViewState);
		}


		public static AbstractViewState GetViewState(Guid entryGuid)
		{
			//	Retourne un ViewState permettant de voir une écriture donnée.
			return new EntriesViewState
			{
				ViewType     = ViewType.Entries,
				SelectedGuid = entryGuid,
				ShowGraphic  = false,
				SortingField = ObjectField.EntryTitle,
			};
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
					SortingField = this.listController.SortingInstructions.PrimaryField,
				};
			}
			set
			{
				var viewState = value as EntriesViewState;
				System.Diagnostics.Debug.Assert (viewState != null);

				this.selectedGuid = viewState.SelectedGuid;
				this.listController.ShowGraphic = viewState.ShowGraphic;
				this.listController.SortingInstructions = new SortingInstructions (viewState.SortingField, SortedType.Ascending, ObjectField.Unknown, SortedType.None);

				this.UpdateUI ();
			}
		}


		private void UpdateAfterListChanged()
		{
			this.selectedGuid = this.listController.SelectedGuid;

			this.UpdateUI ();
		}

		private void GotoAsset()
		{
			var entry = this.accessor.GetObject (BaseType.Entries, this.selectedGuid);
			if (entry != null)
			{
				var assetGuid = ObjectProperties.GetObjectPropertyGuid (entry, null, ObjectField.EntryAssetGuid);
				var eventGuid = ObjectProperties.GetObjectPropertyGuid (entry, null, ObjectField.EntryEventGuid);

				var asset = this.accessor.GetObject (BaseType.Assets, assetGuid);
				if (asset != null)
				{
					var e = asset.GetEvent (eventGuid);
					if (e != null)
					{
						var viewState = AssetsView.GetViewState (assetGuid, e.Timestamp, PageType.AmortizationValue, ObjectField.Unknown);
						this.OnGoto (viewState);
					}
				}
			}
		}


		private void UpdateToolbars()
		{
			this.mainToolbar.SetVisibility (Res.Commands.Main.Edit,   false);
			this.mainToolbar.SetVisibility (Res.Commands.Edit.Accept, false);
			this.mainToolbar.SetVisibility (Res.Commands.Edit.Cancel, false);
		}


		private readonly EntriesToolbarTreeTableController	listController;

		private FrameBox									listFrameBox;
		private Guid										selectedGuid;
	}
}
