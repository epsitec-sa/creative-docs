//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.App.DataFillers;
using Epsitec.Cresus.Assets.App.Helpers;
using Epsitec.Cresus.Assets.App.Views.CommandToolbars;
using Epsitec.Cresus.Assets.App.Views.ViewStates;
using Epsitec.Cresus.Assets.Data;
using Epsitec.Cresus.Assets.Server.BusinessLogic;
using Epsitec.Cresus.Assets.Server.NodeGetters;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.App.Views.ToolbarControllers
{
	public class WarningsToolbarTreeTableController : AbstractToolbarBothTreesController<Warning>, IDirty
	{
		public WarningsToolbarTreeTableController(DataAccessor accessor, BaseType baseType)
			: base (accessor, baseType)
		{
			this.hasFilter         = false;
			this.hasTreeOperations = false;
			this.hasMoveOperations = false;

			this.title = AbstractView.GetViewTitle (this.accessor, ViewType.Warnings);

			this.warnings = new List<Warning>();
			WarningsLogic.GetWarnings (this.warnings, this.accessor);
			this.nodeGetter = new WarningNodeGetter (this.warnings);
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
			this.UpdateController ();
			this.UpdateToolbar ();
		}


		public override Guid SelectedGuid
		{
			//	Retourne le Guid de l'objet actuellement sélectionné.
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
			//	Sélectionne l'objet ayant un Guid donné.
			set
			{
				this.VisibleSelectedRow = this.nodeGetter.GetNodes ().ToList ().FindIndex (x => x.Guid == value);
			}
		}


		#region Goto Logic
		public AbstractViewState Goto(Guid warningGuid)
		{
			var warning = this.warnings.Where (x => x.Guid == warningGuid).FirstOrDefault ();

			switch (warning.BaseType.Kind)
			{
				case BaseTypeKind.Assets:
					return this.GotoAsset (warning);

				case BaseTypeKind.Categories:
					return this.GotoCategory (warning);

				case BaseTypeKind.Groups:
					return this.GotoGroup (warning);

				case BaseTypeKind.Persons:
					return this.GotoPerson (warning);

				default:
					return null;
			}
		}

		private AbstractViewState GotoAsset(Warning warning)
		{
			var obj = this.accessor.GetObject (warning.BaseType, warning.ObjectGuid);
			var e = obj.GetEvent (warning.EventGuid);

			PageType page;

			if (warning.Field >= ObjectField.UserFieldFirst &&
				warning.Field <= ObjectField.UserFieldLast)
			{
				page = PageType.Asset;
			}
			else
			{
				page = PageType.AmortizationDefinition;
			}

			return AssetsView.GetViewState (warning.ObjectGuid, e.Timestamp, page);
		}

		private AbstractViewState GotoCategory(Warning warning)
		{
			var obj = this.accessor.GetObject (warning.BaseType, warning.ObjectGuid);
			var e = obj.GetEvent (warning.EventGuid);

			return CategoriesView.GetViewState (warning.ObjectGuid);
		}

		private AbstractViewState GotoGroup(Warning warning)
		{
			var obj = this.accessor.GetObject (warning.BaseType, warning.ObjectGuid);
			var e = obj.GetEvent (warning.EventGuid);

			return GroupsView.GetViewState (warning.ObjectGuid);
		}

		private AbstractViewState GotoPerson(Warning warning)
		{
			var obj = this.accessor.GetObject (warning.BaseType, warning.ObjectGuid);
			var e = obj.GetEvent (warning.EventGuid);

			return PersonsView.GetViewState (warning.ObjectGuid);
		}
		#endregion


		protected override void AdaptToolbarCommand()
		{
			this.toolbar.SetCommandDescription (ToolbarCommand.New,      CommandDescription.Empty);
			this.toolbar.SetCommandDescription (ToolbarCommand.Delete,   CommandDescription.Empty);
			this.toolbar.SetCommandDescription (ToolbarCommand.Deselect, null, "Désélectionner l'avertissement");
			this.toolbar.SetCommandDescription (ToolbarCommand.Copy,     CommandDescription.Empty);
			this.toolbar.SetCommandDescription (ToolbarCommand.Paste,    CommandDescription.Empty);
			this.toolbar.SetCommandDescription (ToolbarCommand.Export,   null, "Exporter les avertissements");
			this.toolbar.SetCommandDescription (ToolbarCommand.Import,   CommandDescription.Empty);
			this.toolbar.SetCommandDescription (ToolbarCommand.Goto,     null, "Aller sur l'avertissement");
		}

		protected override void CreateNodeFiller()
		{
			this.dataFiller = new WarningsTreeTableFiller (this.accessor, this.nodeGetter);
			TreeTableFiller<Warning>.FillColumns (this.treeTableController, this.dataFiller, "View.Warnings");

			this.sortingInstructions = TreeTableFiller<Warning>.GetSortingInstructions (this.treeTableController);
		}


		protected override void OnDeselect()
		{
			this.VisibleSelectedRow = -1;
		}

		protected override void OnGoto()
		{
			this.OnRowDoubleClicked (this.VisibleSelectedRow);
		}


		protected override void UpdateToolbar()
		{
			base.UpdateToolbar ();
			this.toolbar.SetCommandEnable (ToolbarCommand.Goto, this.VisibleSelectedRow != -1);
		}


		private readonly List<Warning>			warnings;
	}
}
