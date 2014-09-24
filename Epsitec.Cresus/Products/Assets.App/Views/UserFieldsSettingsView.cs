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
	public class UserFieldsSettingsView : AbstractView, System.IDisposable
	{
		public UserFieldsSettingsView(DataAccessor accessor, CommandContext commandContext, MainToolbar toolbar, ViewType viewType, BaseType baseType)
			: base (accessor, commandContext, toolbar, viewType)
		{
			this.baseType = baseType;

			this.listController = new UserFieldsToolbarTreeTableController (this.accessor, this.commandContext, this.baseType);
			this.objectEditor   = new ObjectEditor (this.accessor, BaseType.UserFields, this.baseType, isTimeless: true);
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

			this.UpdateToolbars ();
			this.UpdateEditor ();
			this.UpdateWarningsRedDot ();

			this.OnViewStateChanged (this.ViewState);
		}


		public override AbstractViewState ViewState
		{
			get
			{
				ViewType viewType;

				switch (this.baseType.Kind)
				{
					case BaseTypeKind.Assets:
						viewType = ViewType.AssetsSettings;
						break;

					case BaseTypeKind.Persons:
						viewType = ViewType.PersonsSettings;
						break;

					default:
						throw new System.InvalidOperationException (string.Format ("Unknown BaseType {0}", this.baseType.ToString ()));
				}

				return new SettingsViewState
				{
					ViewType     = viewType,
					BaseType     = this.baseType,
					SelectedGuid = this.selectedGuid,
				};
			}
			set
			{
				var viewState = value as SettingsViewState;
				System.Diagnostics.Debug.Assert (viewState != null);

				this.selectedGuid = viewState.SelectedGuid;

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
			//	Démarre une édition après avoir créé un contact.
			this.isEditing = true;
			this.selectedGuid = guid;
			this.objectEditor.PageType = this.objectEditor.MainPageType;

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
			this.objectEditor.SetObject (this.selectedGuid, Timestamp.MaxValue);
		}


		protected override bool IsEditingPossible
		{
			get
			{
				return this.listController.SelectedRow != -1;
			}
		}


		private readonly UserFieldsToolbarTreeTableController listController;
		private readonly ObjectEditor						objectEditor;

		private FrameBox									listFrameBox;
		private FrameBox									editFrameBox;

		private Guid										selectedGuid;
	}
}
