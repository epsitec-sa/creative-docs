//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Support;
using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.App.Widgets;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.App.Views
{
	public class CategoriesView : AbstractView
	{
		public CategoriesView(DataAccessor accessor, MainToolbar toolbar)
			: base (accessor, toolbar)
		{
			this.baseType = BaseType.Categories;

			this.listController = new CategoriesToolbarTreeTableController (this.accessor);
			this.objectEditor   = new ObjectEditor (this.accessor, BaseType.Categories);

			this.ignoreChanges = new SafeCounter ();

			this.listController.StartEditing += delegate (object sender, EventType eventType, Timestamp timestamp)
			{
				this.OnStartEdit (eventType, timestamp);
			};

			this.objectEditor.ValueChanged += delegate (object sender, ObjectField field)
			{
				this.UpdateToolbars ();
			};
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
			this.listFrameBox = new FrameBox
			{
				Parent = parent,
				Dock   = DockStyle.Fill,
			};

			this.editFrameBox = new FrameBox
			{
				Parent         = parent,
				Dock           = DockStyle.Right,
				PreferredWidth = 600,
				BackColor      = ColorManager.GetBackgroundColor (),
			};

			this.listController.CreateUI (this.listFrameBox);
			this.objectEditor.CreateUI (this.editFrameBox);

			this.Update ();

			//	Connexion des événements.
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
		}


		public override void OnCommand(ToolbarCommand command)
		{
			base.OnCommand (command);

			switch (command)
			{
				case ToolbarCommand.Edit:
					this.OnStartStopEdit ();
					break;

				case ToolbarCommand.Accept:
					this.OnEditAccept ();
					break;

				case ToolbarCommand.Cancel:
					this.OnEditCancel ();
					break;
			}
		}


		private void OnListDoubleClicked()
		{
			this.OnStartEdit ();
		}

		private void OnStartStopEdit()
		{
			if (!this.isEditing && this.selectedGuid.IsEmpty)
			{
				return;
			}

			this.isEditing = !this.isEditing;
			this.Update ();
		}

		private void OnStartEdit()
		{
			if (!this.isEditing && this.selectedGuid.IsEmpty)
			{
				return;
			}

			this.isEditing = true;
			this.Update ();
		}

		private void OnStartEdit(EventType eventType, Timestamp? timestamp = null)
		{
			//	Démarre une édition après avoir créé un événement.
			this.isEditing = true;
			this.Update ();

			this.objectEditor.OpenMainPage (eventType);
		}

		private void OnEditAccept()
		{
			this.isEditing = false;
			this.Update ();
		}

		private void OnEditCancel()
		{
			this.accessor.CancelObjectEdition ();
			this.isEditing = false;
			this.Update ();
		}


		protected override void Update()
		{
			if (!this.isEditing)
			{
				this.accessor.SaveObjectEdition ();
			}

			this.UpdateGeometry ();
			this.UpdateToolbars ();
			this.UpdateEditor ();
		}


		private void UpdateAfterListChanged()
		{
			int row = this.listController.SelectedRow;
			if (row == -1)
			{
				this.selectedGuid = Guid.Empty;

				this.isEditing = false;
			}
			else
			{
				this.selectedGuid = this.accessor.GetObjectGuids (this.baseType, row, 1).First ();
			}

			this.Update ();
		}

		private void UpdateEditor()
		{
			this.objectEditor.SetObject (this.selectedGuid, this.selectedTimestamp);
		}

		private void UpdateGeometry()
		{
			this.editFrameBox.Visibility = this.isEditing;
		}

		private void UpdateToolbars()
		{
			if (this.isEditing)
			{
				this.mainToolbar.SetCommandState (ToolbarCommand.Edit, ToolbarCommandState.Activate);

				this.mainToolbar.UpdateCommand (ToolbarCommand.Accept, this.objectEditor.EditionDirty);
				this.mainToolbar.SetCommandState (ToolbarCommand.Cancel, ToolbarCommandState.Enable);
			}
			else
			{
				this.mainToolbar.UpdateCommand (ToolbarCommand.Edit, this.IsEditingPossible);

				this.mainToolbar.SetCommandState (ToolbarCommand.Accept, ToolbarCommandState.Hide);
				this.mainToolbar.SetCommandState (ToolbarCommand.Cancel, ToolbarCommandState.Hide);
			}

			this.mainToolbar.SetCommandState (ToolbarCommand.Amortissement, ToolbarCommandState.Hide);
		}

		private bool IsEditingPossible
		{
			get
			{
				return this.listController.SelectedRow != -1;
			}
		}


		private readonly CategoriesToolbarTreeTableController listController;
		private readonly ObjectEditor			objectEditor;
		private readonly SafeCounter			ignoreChanges;

		private FrameBox						listFrameBox;
		private FrameBox						editFrameBox;

		private bool							isEditing;
		private Guid							selectedGuid;
		private Timestamp?						selectedTimestamp;
	}
}
