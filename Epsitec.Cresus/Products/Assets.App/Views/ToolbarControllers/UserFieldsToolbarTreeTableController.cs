//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.App.Helpers;
using Epsitec.Cresus.Assets.App.Popups;
using Epsitec.Cresus.Assets.App.Views.CommandToolbars;
using Epsitec.Cresus.Assets.App.Views.FieldControllers;
using Epsitec.Cresus.Assets.Data;
using Epsitec.Cresus.Assets.Server.DataFillers;
using Epsitec.Cresus.Assets.Server.NodeGetters;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.App.Views.ToolbarControllers
{
	public class UserFieldsToolbarTreeTableController : AbstractToolbarBothTreesController<GuidNode>, IDirty
	{
		public UserFieldsToolbarTreeTableController(DataAccessor accessor, BaseType baseType)
			: base (accessor, baseType)
		{
			this.hasFilter         = false;
			this.hasTreeOperations = false;
			this.hasMoveOperations = true;

			switch (this.baseType.Kind)
			{
				case BaseTypeKind.Assets:
					this.title = AbstractView.GetViewTitle (this.accessor, ViewType.AssetsSettings);
					break;

				case BaseTypeKind.Persons:
					this.title = AbstractView.GetViewTitle (this.accessor, ViewType.PersonsSettings);
					break;

				default:
					throw new System.InvalidOperationException (string.Format ("Unknown BaseType {0}", this.baseType.ToString ()));
			}

			this.nodeGetter = new UserFieldNodeGetter (this.accessor, this.baseType);
		}


		#region IDirty Members
		public bool InUse
		{
			get;
			set;
		}

		public bool DirtyData
		{
			get;
			set;
		}
		#endregion


		public override Guid					SelectedGuid
		{
			//	Retourne le champ actuellement sélectionné.
			get
			{
				int sel = this.VisibleSelectedRow;
				if (sel != -1 && sel < this.nodeGetter.Count)
				{
					return this.nodeGetter[sel].Guid;
				}
				else
				{
					return Guid.Empty;
				}
			}
			set
			{
				this.VisibleSelectedRow = this.nodeGetter.GetNodes ().ToList ().FindIndex (x => x.Guid == value);
			}
		}


		protected override void CreateControllerUI(Widget parent)
		{
			base.CreateControllerUI (parent);

			this.treeTableController.AllowsSorting = false;
		}


		public override void UpdateData()
		{
			(this.nodeGetter as UserFieldNodeGetter).SetParams ();

			this.UpdateController ();
			this.UpdateToolbar ();
		}


		protected override void AdaptToolbarCommand()
		{
			this.toolbar.SetCommandDescription (ToolbarCommand.New,      null, "Nouveau champ");
			this.toolbar.SetCommandDescription (ToolbarCommand.Delete,   null, "Supprimer le champ");
			this.toolbar.SetCommandDescription (ToolbarCommand.Deselect, null, "Désélectionner le champ");
			this.toolbar.SetCommandDescription (ToolbarCommand.Copy,     null, "Copier le champ");
			this.toolbar.SetCommandDescription (ToolbarCommand.Paste,    null, "Coller le champ");
			this.toolbar.SetCommandDescription (ToolbarCommand.Export,   null, "Exporter les champs");
			this.toolbar.SetCommandDescription (ToolbarCommand.Import,   CommandDescription.Empty);
			this.toolbar.SetCommandDescription (ToolbarCommand.Goto,     CommandDescription.Empty);
		}

		protected override void CreateNodeFiller()
		{
			this.dataFiller = new UserFieldsTreeTableFiller (this.accessor, this.nodeGetter)
			{
				Title = this.title,
			};

			TreeTableFiller<GuidNode>.FillColumns (this.treeTableController, this.dataFiller, "View.UserFields");

			this.sortingInstructions = TreeTableFiller<GuidNode>.GetSortingInstructions (this.treeTableController);
		}


		protected override void OnMoveTop()
		{
			this.MoveUserField (this.VisibleSelectedRow, this.FirstRowIndex);
		}

		protected override void OnMoveUp()
		{
			this.MoveUserField (this.VisibleSelectedRow, this.PrevRowIndex);
		}

		protected override void OnMoveDown()
		{
			this.MoveUserField (this.VisibleSelectedRow, this.NextRowIndex);
		}

		protected override void OnMoveBottom()
		{
			this.MoveUserField (this.VisibleSelectedRow, this.LastRowIndex);
		}

		protected override void OnDeselect()
		{
			this.VisibleSelectedRow = -1;
		}

		protected override void OnNew()
		{
			var newField = this.accessor.GlobalSettings.GetNewUserField();
			if (newField == ObjectField.Unknown)
			{
				return;
			}

			var userField = new UserField ("Nouveau", newField, FieldType.String, false, 120, AbstractFieldController.maxWidth, 1, null, 0);

			int index = this.VisibleSelectedRow;
			if (index == -1)  // pas de sélection ?
			{
				index = this.nodeGetter.Count;  // insère à la fin
			}

			this.accessor.GlobalSettings.InsertUserField (this.baseType, index, userField);

			this.UpdateData ();
			this.OnUpdateAfterCreate (userField.Guid, EventType.Unknown, Timestamp.Now);
		}

		protected override void OnDelete()
		{
			var target = this.toolbar.GetTarget (ToolbarCommand.Delete);

			YesNoPopup.Show (target, "Voulez-vous supprimer le champ sélectionné ?", delegate
			{
				this.accessor.GlobalSettings.RemoveUserField (this.SelectedGuid);
				this.UpdateData ();
				this.OnUpdateAfterDelete ();
			});
		}

		protected override void OnCopy()
		{
			var userField = this.accessor.GlobalSettings.GetUserField (this.SelectedGuid);
			this.accessor.Clipboard.CopyUserField (this.accessor, this.baseType, userField);

			this.UpdateToolbar ();
		}

		protected override void OnPaste()
		{
			int index = this.VisibleSelectedRow;
			if (index == -1)  // pas de sélection ?
			{
				index = this.nodeGetter.Count;  // insère à la fin
			}

			var userField = this.accessor.Clipboard.PasteUserField (this.accessor, this.baseType, index);

			if (userField.IsEmpty)
			{
				var target = this.toolbar.GetTarget (ToolbarCommand.Paste);
				MessagePopup.ShowPasteError (target);
			}
			else
			{
				this.UpdateData ();
				this.OnUpdateAfterCreate (userField.Guid, EventType.Unknown, Timestamp.Now);
			}
		}


		private void MoveUserField(int currentRow, int? newRow)
		{
			//	Déplace une rubrique à un nouvel emplacement.
			if (currentRow == -1 || !newRow.HasValue)
			{
				return;
			}

			var node = (this.nodeGetter as UserFieldNodeGetter)[currentRow];
			var userField = this.accessor.GlobalSettings.GetUserField (node.Guid);
			System.Diagnostics.Debug.Assert (!userField.IsEmpty);

			//	Supprime la rubrique à l'endroit actuel.
			this.accessor.GlobalSettings.RemoveUserField (node.Guid);

			//	Insère la rubrique au nouvel endroit.
			int index = newRow.Value;
			this.accessor.GlobalSettings.InsertUserField (this.baseType, index, userField);

			//	Met à jour et sélectionne la rubrique déplacée.
			this.UpdateData ();
			this.VisibleSelectedRow = index;
		}


		protected override void UpdateToolbar()
		{
			base.UpdateToolbar ();

			this.toolbar.SetCommandEnable (ToolbarCommand.Copy,  true);
			this.toolbar.SetCommandEnable (ToolbarCommand.Paste, this.accessor.Clipboard.HasUserField (this.baseType));
		}
	}
}
