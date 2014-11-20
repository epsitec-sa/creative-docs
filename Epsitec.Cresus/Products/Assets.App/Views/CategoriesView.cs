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
	public class CategoriesView : AbstractView, System.IDisposable
	{
		public CategoriesView(DataAccessor accessor, CommandContext commandContext, MainToolbar toolbar, ViewType viewType)
			: base (accessor, commandContext, toolbar, viewType)
		{
			this.baseType = BaseType.Categories;

			this.listController = new CategoriesToolbarTreeTableController (this.accessor, this.commandContext, BaseType.Categories);
			this.objectEditor   = new ObjectEditor (this.accessor, this.baseType, isTimeless: true);
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

				this.objectEditor.Goto += delegate (object sender, AbstractViewState viewState)
				{
					this.OnGoto (viewState);
				};

				this.listController.RowDoubleClicked += delegate
				{
					this.OnListDoubleClicked ();
				};

				this.listController.UpdateAfterCreate += delegate (object sender, Guid guid, EventType eventType, Timestamp timestamp)
				{
					this.OnUpdateAfterCreate (guid);
				};

				this.listController.UpdateAfterDelete += delegate
				{
					this.OnUpdateAfterDelete ();
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
			if (this.accessor.EditionAccessor.SaveObjectEdition ())
			{
				this.DataChanged ();
			}

			this.editFrameBox.Visibility = this.isEditing;

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

			this.listController.HelplineVisibility = this.listController.HelplineDesired;

			this.UpdateToolbars ();
			this.UpdateEditor ();
			this.mainToolbar.UpdateWarningsRedDot ();

			this.OnViewStateChanged (this.ViewState);
		}


		public static AbstractViewState GetViewState(Guid categoryGuid, ObjectField field)
		{
			//	Retourne un ViewState permettant de voir une catégorie donnée.
			if (categoryGuid.IsEmpty)
			{
				return new CategoriesViewState
				{
					ViewType     = ViewType.Categories,
					PageType     = PageType.Unknown,  // pas d'édition
					Field        = ObjectField.Unknown,
					SelectedGuid = Guid.Empty,
				};
			}
			else
			{
				return new CategoriesViewState
				{
					ViewType     = ViewType.Categories,
					PageType     = PageType.Category,  // pour éditer directement
					Field        = field,
					SelectedGuid = categoryGuid,
				};
			}
		}


		public override AbstractViewState ViewState
		{
			get
			{
				return new CategoriesViewState
				{
					ViewType     = ViewType.Categories,
					PageType     = this.isEditing ? this.objectEditor.PageType : PageType.Unknown,
					Field        = this.isEditing ? this.objectEditor.FocusField : ObjectField.Unknown,
					SelectedGuid = this.selectedGuid,
				};
			}
			set
			{
				var viewState = value as CategoriesViewState;
				System.Diagnostics.Debug.Assert (viewState != null);

				this.selectedGuid = viewState.SelectedGuid;

				if (viewState.PageType == PageType.Unknown)
				{
					this.isEditing = false;
				}
				else
				{
					this.isEditing = true;
					this.objectEditor.SetPage (viewState.PageType, viewState.Field);
				}

				this.UpdateUI ();
			}
		}


		protected override void OnMainEdit(Widget target)
		{
			if (!this.isEditing && this.selectedGuid.IsEmpty)
			{
				return;
			}

			this.isEditing = !this.isEditing;
			this.UpdateUI ();
		}

		protected override void OnEditAccept(Widget target)
		{
			this.isEditing = false;
			this.UpdateUI ();
		}

		protected override void OnEditCancel(Widget target)
		{
			this.accessor.EditionAccessor.CancelObjectEdition ();
			this.isEditing = false;
			this.UpdateUI ();
		}

		private void OnListDoubleClicked()
		{
			this.OnStartEdit ();
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

		private void OnUpdateAfterCreate(Guid guid)
		{
			//	Démarre une édition après avoir créé une catégorie.
			this.isEditing = true;
			this.selectedGuid = guid;
			this.objectEditor.PageType = this.objectEditor.GetMainPageType ();

			this.DeepUpdateUI ();
		}

		private void OnUpdateAfterDelete()
		{
			this.isEditing = false;
			this.selectedGuid = Guid.Empty;

			this.DeepUpdateUI ();
		}


		private void UpdateAfterListChanged()
		{
			this.selectedGuid = this.listController.SelectedGuid;

			if (this.selectedGuid.IsEmpty)
			{
				this.isEditing = false;
			}

			this.UpdateUI ();
		}


		private void UpdateEditor()
		{
			var timestamp = this.GetLastTimestamp (this.selectedGuid);

			if (!timestamp.HasValue)
			{
				timestamp = new Timestamp (this.accessor.Mandat.StartDate, 0);
			}

			this.objectEditor.SetObject (this.selectedGuid, timestamp);
		}


		protected override bool IsEditingPossible
		{
			get
			{
				return this.listController.SelectedRow != -1;
			}
		}


		private readonly CategoriesToolbarTreeTableController listController;
		private readonly ObjectEditor						objectEditor;

		private FrameBox									listFrameBox;
		private FrameBox									editFrameBox;

		private Guid										selectedGuid;
	}
}
