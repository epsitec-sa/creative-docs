﻿//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Support;
using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.App.Popups;
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
			this.objectEditor   = new ObjectEditor (this.accessor, this.baseType, isTimeless: true);

			this.ignoreChanges = new SafeCounter ();
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

			this.listFrameBox = new FrameBox
			{
				Parent = topBox,
				Dock   = DockStyle.Fill,
			};

			this.editFrameBox = new FrameBox
			{
				Parent         = topBox,
				Dock           = DockStyle.Right,
				PreferredWidth = 750,
				BackColor      = ColorManager.GetBackgroundColor (),
			};

			this.listController.CreateUI (this.listFrameBox);
			this.objectEditor.CreateUI (this.editFrameBox);

			this.Update ();

			//	Connexion des événements de la liste des objets à gauche.
			{
				this.listController.StartEditing += delegate (object sender, EventType eventType, Timestamp timestamp)
				{
					this.OnStartEdit (eventType);
				};

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

			//	Connexion des événements de l'éditeur.
			{
				this.objectEditor.ValueChanged += delegate (object sender, ObjectField field)
				{
					this.UpdateToolbars ();
				};

				this.objectEditor.UpdateData += delegate
				{
					this.UpdateData ();
				};
			}
		}


		public override void OnCommand(ToolbarCommand command)
		{
			base.OnCommand (command);

			switch (command)
			{
				case ToolbarCommand.Edit:
					this.OnStartStopEdit ();
					break;

				case ToolbarCommand.Amortissement:
					this.OnMainAmortissement ();
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

		private void OnStartEdit(EventType eventType)
		{
			//	Démarre une édition après avoir créé un événement.
			this.isEditing = true;
			this.Update ();

			this.objectEditor.OpenMainPage (eventType);
		}

		private void OnMainAmortissement()
		{
			var target = this.mainToolbar.GetCommandWidget (ToolbarCommand.Amortissement);

			if (target != null)
			{
				var popup = new YesNoPopup
				{
					Question = "Voulez-vous générer les amortissements ?",
				};

				if (this.listController.SelectedRow == -1)
				{
					popup.Radios.Add (new YesNoPopup.Radio ("all", "Pour tous les objets", activate: true));
				}
				else
				{
					popup.Radios.Add (new YesNoPopup.Radio ("one", "Pour l'objet sélectionné", activate: true));
					popup.Radios.Add (new YesNoPopup.Radio ("all", "Pour tous les objets"));
				}

				popup.Create (target);

				popup.ButtonClicked += delegate (object sender, string name)
				{
					if (name == "yes")
					{
						if (popup.RadioSelected == "one")
						{
							var guid = this.listController.SelectedGuid;
							this.amortissements.GeneratesAmortissementsAuto (guid);
						}
						else
						{
							this.amortissements.GeneratesAmortissementsAuto ();
						}

						this.Update ();
					}
				};
			}
		}

		private void OnEditAccept()
		{
			this.isEditing = false;
			this.Update ();
		}

		private void OnEditCancel()
		{
			this.accessor.EditionAccessor.CancelObjectEdition ();
			this.isEditing = false;
			this.Update ();
		}


		protected override void Update()
		{
			if (!this.isEditing)
			{
				if (this.accessor.EditionAccessor.SaveObjectEdition ())
				{
					this.UpdateData ();
				}
			}

			this.editFrameBox.Visibility = this.isEditing;

			this.UpdateToolbars ();
			this.UpdateEditor ();
		}


		private void UpdateAfterListChanged()
		{
			this.selectedGuid = this.listController.SelectedGuid;

			if (this.selectedGuid.IsEmpty)
			{
				this.isShowEvents = false;
				this.isEditing    = false;
			}

			this.Update ();
		}


		private void UpdateData()
		{
			this.listController.UpdateData ();
			this.listController.SelectedGuid = this.selectedGuid;
		}

		private void UpdateEditor()
		{
			var timestamp = this.GetLastTimestamp (this.selectedGuid);

			if (!timestamp.HasValue)
			{
				timestamp = Timestamp.Now;
			}

			this.objectEditor.SetObject (this.selectedGuid, timestamp);
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

			this.mainToolbar.SetCommandState (ToolbarCommand.Amortissement, ToolbarCommandState.Enable);
		}

		private bool IsEditingPossible
		{
			get
			{
				return this.listController.SelectedRow != -1;
			}
		}


		private readonly CategoriesToolbarTreeTableController listController;
		private readonly ObjectEditor						objectEditor;
		private readonly SafeCounter						ignoreChanges;

		private FrameBox									listFrameBox;
		private FrameBox									editFrameBox;

		private bool										isShowEvents;
		private bool										isEditing;
		private Guid										selectedGuid;
	}
}