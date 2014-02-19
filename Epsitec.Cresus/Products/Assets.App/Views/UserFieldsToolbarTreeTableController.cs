//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
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

			this.nodeGetter = new UserFieldNodeGetter (this.accessor, this.baseType);

			switch (this.baseType)
			{
				case BaseType.Assets:
					this.title = "Champs supplémentaires des objets d'immobilisation";
					break;

				case BaseType.Persons:
					this.title = "Champs supplémentaires des personnes";
					break;

				default:
					this.title = "?";
					break;
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


		public override void UpdateData()
		{
			(this.nodeGetter as UserFieldNodeGetter).SetParams ();

			this.UpdateController ();
			this.UpdateToolbar ();
		}


		public Guid								SelectedGuid
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


		protected override void CreateNodeFiller()
		{
			this.dataFiller = new UserFieldsTreeTableFiller (this.accessor, this.nodeGetter);
			TreeTableFiller<GuidNode>.FillColumns (this.controller, this.dataFiller);

			this.controller.AddSortedColumn (0);
		}


		protected override void OnDeselect()
		{
			this.VisibleSelectedRow = -1;
		}

		protected override void OnNew()
		{
			var newField = this.accessor.Settings.GetNewUserObjectField();
			var userField = new UserField ("Nouveau", newField, FieldType.String);
			this.accessor.Settings.AddUserField (this.baseType, userField);

			this.UpdateData ();
			this.OnUpdateAfterCreate (Guid.Empty, EventType.Unknown, Timestamp.Now);
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


		private BaseType baseType;
	}
}
