//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.App.Helpers;
using Epsitec.Cresus.Assets.App.Popups;
using Epsitec.Cresus.Assets.Server.DataFillers;
using Epsitec.Cresus.Assets.Server.NodeGetters;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.App.Views
{
	public class UserFieldsToolbarTreeTableController : AbstractToolbarTreeTableController<GuidNode>, IDirty
	{
		public UserFieldsToolbarTreeTableController(DataAccessor accessor, BaseType baseType)
			: base (accessor)
		{
			this.baseType = baseType;

			this.hasFilter         = false;
			this.hasTreeOperations = false;
			this.hasMoveOperations = true;

			this.nodeGetter = new UserFieldNodeGetter (this.accessor, this.baseType);

			switch (this.baseType)
			{
				case BaseType.Assets:
					this.title = StaticDescriptions.GetViewTypeDescription (ViewType.AssetsSettings);
					break;

				case BaseType.Persons:
					this.title = StaticDescriptions.GetViewTypeDescription (ViewType.PersonsSettings);
					break;

				default:
					throw new System.InvalidOperationException (string.Format ("Unknown BaseType {0}", this.baseType.ToString ()));
			}
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
				this.VisibleSelectedRow = this.nodeGetter.Nodes.ToList ().FindIndex (x => x.Guid == value);
			}
		}


		public override void CreateUI(Widget parent)
		{
			base.CreateUI (parent);

			this.controller.AllowsSorting = false;
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
			TreeTableFiller<GuidNode>.FillColumns (this.controller, this.dataFiller);

			this.controller.AddSortedColumn (0);
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
			var newField = this.accessor.Settings.GetNewUserField();
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

			this.accessor.Settings.InsertUserField (this.baseType, index, userField);

			this.UpdateData ();
			this.OnUpdateAfterCreate (userField.Guid, EventType.Unknown, Timestamp.Now);
		}

		protected override void OnDelete()
		{
			var target = this.toolbar.GetTarget (ToolbarCommand.Delete);

			if (target != null)
			{
				var popup = new YesNoPopup
				{
					Question = "Voulez-vous supprimer le champ sélectionné ?",
				};

				popup.Create (target, leftOrRight: true);

				popup.ButtonClicked += delegate (object sender, string name)
				{
					if (name == "yes")
					{
						this.accessor.Settings.RemoveUserField (this.SelectedGuid);
						this.UpdateData ();
						this.OnUpdateAfterDelete ();
					}
				};
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
			var userField = this.accessor.Settings.GetUserField (node.Guid);
			System.Diagnostics.Debug.Assert (!userField.IsEmpty);

			//	Supprime la rubrique à l'endroit actuel.
			this.accessor.Settings.RemoveUserField (node.Guid);

			//	Insère la rubrique au nouvel endroit.
			int index = newRow.Value;
			this.accessor.Settings.InsertUserField (this.baseType, index, userField);

			//	Met à jour et sélectionne la rubrique déplacée.
			this.UpdateData ();
			this.VisibleSelectedRow = index;
		}


		private BaseType						baseType;
	}
}
