//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.App.Helpers;
using Epsitec.Cresus.Assets.App.Popups;
using Epsitec.Cresus.Assets.Data;
using Epsitec.Cresus.Assets.Server.DataFillers;
using Epsitec.Cresus.Assets.Server.NodeGetters;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.App.Views
{
	public class UserFieldsToolbarTreeTableController : AbstractToolbarBothTreesController<GuidNode>, IDirty
	{
		public UserFieldsToolbarTreeTableController(DataAccessor accessor, BaseType baseType)
			: base (accessor, baseType)
		{
			this.hasFilter         = false;
			this.hasTreeOperations = false;
			this.hasMoveOperations = true;

			switch (this.baseType)
			{
				case BaseType.Assets:
					this.title = AbstractView.GetViewTitle (this.accessor, ViewType.AssetsSettings);
					break;

				case BaseType.Persons:
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


		protected override void CreateNodeFiller()
		{
			this.dataFiller = new UserFieldsTreeTableFiller (this.accessor, this.nodeGetter);
			TreeTableFiller<GuidNode>.FillColumns (this.treeTableController, this.dataFiller);

			this.treeTableController.AddSortedColumn (0);
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

			var userField = new UserField ("Nouveau", newField, FieldType.String, 120, AbstractFieldController.maxWidth, 1, null, 0);

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
	}
}
