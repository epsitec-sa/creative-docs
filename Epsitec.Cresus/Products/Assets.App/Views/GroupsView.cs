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
	public class GroupsView : AbstractView, System.IDisposable
	{
		public GroupsView(DataAccessor accessor, CommandContext commandContext, MainToolbar toolbar, ViewType viewType)
			: base (accessor, commandContext, toolbar, viewType)
		{
			this.baseType = BaseType.Groups;

			this.listController = new GroupsToolbarTreeTableController (this.accessor, this.commandContext, BaseType.Groups);
			this.objectEditor   = new ObjectEditor (this.accessor, this.baseType, this.baseType, isTimeless: true);
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
			if (!this.objectEditor.HasError && this.accessor.EditionAccessor.SaveObjectEdition ())
			{
				this.DataChanged ();
			}

			this.listController.InUse = true;
			this.editFrameBox.Visibility = this.isEditing;

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
			this.UpdateEditor ();
			this.UpdateWarningsRedDot ();

			this.OnViewStateChanged (this.ViewState);
		}


		public static AbstractViewState GetViewState(Guid groupGuid, ObjectField field)
		{
			//	Retourne un ViewState permettant de voir un groupe donné.
			return new GroupsViewState
			{
				ViewType     = ViewType.Groups,
				PageType     = PageType.Group,  // pour éditer directement
				Field        = field,
				SelectedGuid = groupGuid,
			};
		}


		public override AbstractViewState ViewState
		{
			get
			{
				return new GroupsViewState
				{
					ViewType     = ViewType.Groups,
					PageType     = this.isEditing ? this.objectEditor.PageType : PageType.Unknown,
					Field        = this.isEditing ? this.objectEditor.FocusField : ObjectField.Unknown,
					SelectedGuid = this.selectedGuid,
					ShowGraphic  = this.listController.ShowGraphic,
				};
			}
			set
			{
				var viewState = value as GroupsViewState;
				System.Diagnostics.Debug.Assert (viewState != null);

				this.selectedGuid = viewState.SelectedGuid;
				this.listController.ShowGraphic = viewState.ShowGraphic;

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


		protected override void CommandMainEdit(Widget target)
		{
			if (!this.isEditing && this.selectedGuid.IsEmpty)
			{
				return;
			}

			this.isEditing = !this.isEditing;
			this.UpdateUI ();
		}

		protected override void CommandEditAccept(Widget target)
		{
			this.isEditing = false;
			this.UpdateUI ();
		}

		protected override void CommandEditCancel(Widget target)
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
			//	Démarre une édition après avoir créé un groupe.
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
			var timestamp = this.GetLastTimestamp (this.selectedGuid);

			if (!timestamp.HasValue)
			{
				timestamp = new Timestamp (this.accessor.Mandat.StartDate, 0);
			}

			this.objectEditor.SetObject (this.selectedGuid, timestamp);
		}


		private void UpdateToolbars()
		{
			var edit   = this.commandContext.GetCommandState (Res.Commands.Main.Edit);
			var accept = this.commandContext.GetCommandState (Res.Commands.Edit.Accept);
			var cancel = this.commandContext.GetCommandState (Res.Commands.Edit.Cancel);

			if (this.isEditing)
			{
				edit.Visibility  = true;
				edit.ActiveState = ActiveState.Yes;
				edit.Enable      = true;

				accept.Visibility = true;
				cancel.Visibility = true;
				accept.Enable     = !this.objectEditor.HasError;
				cancel.Enable     = true;
			}
			else
			{
				edit.Visibility  = true;
				edit.ActiveState = ActiveState.No;
				edit.Enable      = this.IsEditingPossible;

				accept.Visibility = false;
				cancel.Visibility = false;
				accept.Enable     = false;
				cancel.Enable     = false;
			}
		}

		private bool IsEditingPossible
		{
			get
			{
				return this.listController.SelectedRow != -1;
			}
		}


		private readonly GroupsToolbarTreeTableController	listController;
		private readonly ObjectEditor						objectEditor;

		private FrameBox									listFrameBox;
		private FrameBox									editFrameBox;

		private bool										isEditing;
		private Guid										selectedGuid;
	}
}
