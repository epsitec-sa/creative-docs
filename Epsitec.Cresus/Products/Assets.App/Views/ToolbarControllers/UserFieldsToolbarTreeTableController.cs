//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
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
	public class UserFieldsToolbarTreeTableController : AbstractToolbarBothTreesController<GuidNode>, IDirty, System.IDisposable
	{
		public UserFieldsToolbarTreeTableController(DataAccessor accessor, CommandContext commandContext, BaseType baseType)
			: base (accessor, commandContext, baseType)
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

		public void Dispose()
		{
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


		protected override void CreateToolbar()
		{
			this.toolbar = new UserFieldsToolbar (this.accessor, this.commandContext);
		}
		//?protected override void AdaptToolbarCommand()
		//?{
		//?	this.toolbar.SetCommandDescription (ToolbarCommand.New,      null, Res.Strings.ToolbarControllers.UserFieldsTreeTable.New.ToString (), new Shortcut (KeyCode.AlphaI | KeyCode.ModifierControl));
		//?	this.toolbar.SetCommandDescription (ToolbarCommand.Delete,   null, Res.Strings.ToolbarControllers.UserFieldsTreeTable.Delete.ToString ());
		//?	this.toolbar.SetCommandDescription (ToolbarCommand.Deselect, null, Res.Strings.ToolbarControllers.UserFieldsTreeTable.Deselect.ToString ());
		//?	this.toolbar.SetCommandDescription (ToolbarCommand.Copy,     null, Res.Strings.ToolbarControllers.UserFieldsTreeTable.Copy.ToString ());
		//?	this.toolbar.SetCommandDescription (ToolbarCommand.Paste,    null, Res.Strings.ToolbarControllers.UserFieldsTreeTable.Paste.ToString ());
		//?	this.toolbar.SetCommandDescription (ToolbarCommand.Export,   null, Res.Strings.ToolbarControllers.UserFieldsTreeTable.Export.ToString ());
		//?	this.toolbar.SetCommandDescription (ToolbarCommand.Import,   CommandDescription.Empty);
		//?	this.toolbar.SetCommandDescription (ToolbarCommand.Goto,     CommandDescription.Empty);
		//?}

		protected override void CreateNodeFiller()
		{
			this.dataFiller = new UserFieldsTreeTableFiller (this.accessor, this.nodeGetter)
			{
				Title = this.title,
			};

			TreeTableFiller<GuidNode>.FillColumns (this.treeTableController, this.dataFiller, "View.UserFields");

			this.sortingInstructions = TreeTableFiller<GuidNode>.GetSortingInstructions (this.treeTableController);
		}


		[Command (Res.CommandIds.TreeTable.MoveTop)]
		protected void OnMoveTop()
		{
			this.MoveUserField (this.VisibleSelectedRow, this.FirstRowIndex);
		}

		[Command (Res.CommandIds.TreeTable.MoveUp)]
		protected void OnMoveUp()
		{
			this.MoveUserField (this.VisibleSelectedRow, this.PrevRowIndex);
		}

		[Command (Res.CommandIds.TreeTable.MoveDown)]
		protected void OnMoveDown()
		{
			this.MoveUserField (this.VisibleSelectedRow, this.NextRowIndex);
		}

		[Command (Res.CommandIds.TreeTable.MoveBottom)]
		protected void OnMoveBottom()
		{
			this.MoveUserField (this.VisibleSelectedRow, this.LastRowIndex);
		}

		[Command (Res.CommandIds.UserFields.Deselect)]
		protected void OnDeselect()
		{
			this.VisibleSelectedRow = -1;
		}

		[Command (Res.CommandIds.UserFields.New)]
		protected void OnNew()
		{
			var newField = this.accessor.GlobalSettings.GetNewUserField();
			if (newField == ObjectField.Unknown)
			{
				return;
			}

			var userField = new UserField (Res.Strings.ToolbarControllers.UserFieldsTreeTable.NewName.ToString (), newField, FieldType.String, false, 120, AbstractFieldController.maxWidth, 1, null, 0);

			int index = this.VisibleSelectedRow;
			if (index == -1)  // pas de sélection ?
			{
				index = this.nodeGetter.Count;  // insère à la fin
			}

			this.accessor.GlobalSettings.InsertUserField (this.baseType, index, userField);

			this.UpdateData ();
			this.OnUpdateAfterCreate (userField.Guid, EventType.Unknown, Timestamp.Now);
		}

		[Command (Res.CommandIds.UserFields.Delete)]
		protected void OnDelete()
		{
			var target = this.toolbar.GetTarget (ToolbarCommand.Delete);

			YesNoPopup.Show (target, Res.Strings.ToolbarControllers.UserFieldsTreeTable.DeleteQuestion.ToString (), delegate
			{
				this.accessor.GlobalSettings.RemoveUserField (this.SelectedGuid);
				this.UpdateData ();
				this.OnUpdateAfterDelete ();
			});
		}

		[Command (Res.CommandIds.UserFields.Copy)]
		protected override void OnCopy(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			var userField = this.accessor.GlobalSettings.GetUserField (this.SelectedGuid);
			this.accessor.Clipboard.CopyUserField (this.accessor, this.baseType, userField);

			this.UpdateToolbar ();
		}

		[Command (Res.CommandIds.UserFields.Paste)]
		protected override void OnPaste(CommandDispatcher dispatcher, CommandEventArgs e)
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

		[Command (Res.CommandIds.UserFields.Export)]
		protected override void OnExport(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			base.OnExport (dispatcher, e);
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
