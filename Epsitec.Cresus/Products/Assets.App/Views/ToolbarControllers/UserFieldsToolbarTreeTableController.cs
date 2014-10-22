﻿//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
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
using Epsitec.Common.Drawing;
using Epsitec.Cresus.Assets.Server.BusinessLogic;

namespace Epsitec.Cresus.Assets.App.Views.ToolbarControllers
{
	public class UserFieldsToolbarTreeTableController : AbstractToolbarBothTreesController<GuidNode>, IDirty, System.IDisposable
	{
		public UserFieldsToolbarTreeTableController(DataAccessor accessor, CommandContext commandContext, BaseType baseType)
			: base (accessor, commandContext, baseType)
		{
			switch (this.baseType.Kind)
			{
				case BaseTypeKind.AssetsUserFields:
					this.title = AbstractView.GetViewTitle (this.accessor, ViewType.AssetsSettings);
					break;

				case BaseTypeKind.PersonsUserFields:
					this.title = AbstractView.GetViewTitle (this.accessor, ViewType.PersonsSettings);
					break;

				default:
					throw new System.InvalidOperationException (string.Format ("Unknown BaseType {0}", this.baseType.ToString ()));
			}

			this.nodeGetter = new UserFieldNodeGetter (this.accessor, this.baseType);
		}

		public override void Dispose()
		{
			base.Dispose ();
		}


		#region IDirty Members
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
			this.ConnectSearch ();
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


		[Command (Res.CommandIds.UserFields.First)]
		protected override void OnFirst()
		{
			base.OnFirst ();
		}

		[Command (Res.CommandIds.UserFields.Prev)]
		protected override void OnPrev()
		{
			base.OnPrev ();
		}

		[Command (Res.CommandIds.UserFields.Next)]
		protected override void OnNext()
		{
			base.OnNext ();
		}

		[Command (Res.CommandIds.UserFields.Last)]
		protected override void OnLast()
		{
			base.OnLast ();
		}

		[Command (Res.CommandIds.UserFields.MoveTop)]
		protected void OnMoveTop()
		{
			this.MoveUserField (this.VisibleSelectedRow, this.FirstRowIndex, Res.Commands.UserFields.MoveTop.Description);
		}

		[Command (Res.CommandIds.UserFields.MoveUp)]
		protected void OnMoveUp()
		{
			this.MoveUserField (this.VisibleSelectedRow, this.PrevRowIndex, Res.Commands.UserFields.MoveUp.Description);
		}

		[Command (Res.CommandIds.UserFields.MoveDown)]
		protected void OnMoveDown()
		{
			this.MoveUserField (this.VisibleSelectedRow, this.NextRowIndex, Res.Commands.UserFields.MoveDown.Description);
		}

		[Command (Res.CommandIds.UserFields.MoveBottom)]
		protected void OnMoveBottom()
		{
			this.MoveUserField (this.VisibleSelectedRow, this.LastRowIndex, Res.Commands.UserFields.MoveBottom.Description);
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

			this.accessor.UndoManager.Start ();
			this.accessor.UndoManager.SetDescription (Res.Commands.UserFields.New.Description);

			var userField = new UserField (Res.Strings.ToolbarControllers.UserFieldsTreeTable.NewName.ToString (), newField, FieldType.String, false, 120, AbstractFieldController.maxWidth, 1, null, 0);

			int index = this.VisibleSelectedRow + 1;  // insère après la sélection actuelle
			if (index == 0)  // pas de sélection ?
			{
				index = this.nodeGetter.Count;  // insère à la fin
			}

			this.accessor.GlobalSettings.InsertUserField (this.baseType, index, userField);
			accessor.WarningsDirty = true;
			this.UpdateData ();
			this.OnUpdateAfterCreate (userField.Guid, EventType.Unknown, Timestamp.Now);

			this.accessor.UndoManager.SetAfterViewState ();
		}

		[Command (Res.CommandIds.UserFields.Delete)]
		protected void OnDelete(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			var target = this.toolbar.GetTarget (e);

			var name = UserFieldsLogic.GetSummary (this.accessor, this.baseType, this.SelectedGuid);
			var question = string.Format (Res.Strings.ToolbarControllers.UserFieldsTreeTable.DeleteQuestion.ToString (), name);

			YesNoPopup.Show (target, question, delegate
			{
				this.accessor.UndoManager.Start ();
				this.accessor.UndoManager.SetDescription (Res.Commands.UserFields.Delete.Description);

				this.accessor.GlobalSettings.RemoveUserField (this.SelectedGuid);
				accessor.WarningsDirty = true;
				this.UpdateData ();
				this.OnUpdateAfterDelete ();

				this.accessor.UndoManager.SetAfterViewState ();
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
				var target = this.toolbar.GetTarget (e);
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


		protected override void ShowContextMenu(Point pos)
		{
			//	Affiche le menu contextuel.
			MenuPopup.Show (this.toolbar, this.treeTableFrame, pos,
				Res.Commands.UserFields.New,
				Res.Commands.UserFields.Delete,
				null,
				Res.Commands.UserFields.Copy,
				Res.Commands.UserFields.Paste);
		}


		private void MoveUserField(int currentRow, int? newRow, string description)
		{
			//	Déplace une rubrique à un nouvel emplacement.
			if (currentRow == -1 || !newRow.HasValue)
			{
				return;
			}

			this.accessor.UndoManager.Start ();
			this.accessor.UndoManager.SetDescription (description);

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

			this.accessor.UndoManager.SetAfterViewState ();
		}


		protected override void UpdateToolbar()
		{
			base.UpdateToolbar ();

			int row = this.VisibleSelectedRow;

			this.UpdateSelCommand (Res.Commands.UserFields.First, row, this.FirstRowIndex);
			this.UpdateSelCommand (Res.Commands.UserFields.Prev,  row, this.PrevRowIndex);
			this.UpdateSelCommand (Res.Commands.UserFields.Next,  row, this.NextRowIndex);
			this.UpdateSelCommand (Res.Commands.UserFields.Last,  row, this.LastRowIndex);

			this.UpdateSelCommand (Res.Commands.UserFields.MoveTop,    row, this.PrevRowIndex);
			this.UpdateSelCommand (Res.Commands.UserFields.MoveUp,     row, this.PrevRowIndex);
			this.UpdateSelCommand (Res.Commands.UserFields.MoveDown,   row, this.NextRowIndex);
			this.UpdateSelCommand (Res.Commands.UserFields.MoveBottom, row, this.NextRowIndex);

			this.toolbar.SetEnable (Res.Commands.UserFields.New,      true);
			this.toolbar.SetEnable (Res.Commands.UserFields.Delete,   row != -1);
			this.toolbar.SetEnable (Res.Commands.UserFields.Deselect, row != -1);

			this.toolbar.SetEnable (Res.Commands.UserFields.Copy,   this.IsCopyEnable);
			this.toolbar.SetEnable (Res.Commands.UserFields.Paste,  this.accessor.Clipboard.HasUserField (this.baseType));
			this.toolbar.SetEnable (Res.Commands.UserFields.Export, !this.IsEmpty);
		}
	}
}
