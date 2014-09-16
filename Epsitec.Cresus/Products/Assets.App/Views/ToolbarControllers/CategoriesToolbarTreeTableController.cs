//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.App.Helpers;
using Epsitec.Cresus.Assets.App.Popups;
using Epsitec.Cresus.Assets.App.Views.CommandToolbars;
using Epsitec.Cresus.Assets.Data;
using Epsitec.Cresus.Assets.Server.DataFillers;
using Epsitec.Cresus.Assets.Server.NodeGetters;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.App.Views.ToolbarControllers
{
	public class CategoriesToolbarTreeTableController : AbstractToolbarBothTreesController<SortableNode>, IDirty, System.IDisposable
	{
		public CategoriesToolbarTreeTableController(DataAccessor accessor, CommandContext commandContext, BaseType baseType)
			: base (accessor, commandContext, baseType)
		{
			this.hasFilter         = false;
			this.hasTreeOperations = false;
			this.hasMoveOperations = false;

			this.title = AbstractView.GetViewTitle (this.accessor, ViewType.Categories);

			var primary = this.accessor.GetNodeGetter (BaseType.Categories);
			this.secondaryGetter = new SortableNodeGetter (primary, this.accessor, BaseType.Categories);
			this.nodeGetter = new SorterNodeGetter (this.secondaryGetter);
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


		public override void UpdateData()
		{
			this.secondaryGetter.SetParams (null, this.sortingInstructions);
			(this.nodeGetter as SorterNodeGetter).SetParams (this.sortingInstructions);

			this.UpdateController ();
			this.UpdateToolbar ();
		}


		public override Guid					SelectedGuid
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


		protected override void AdaptToolbarCommand()
		{
			this.toolbar.SetCommandDescription (ToolbarCommand.New,      "TreeTable.New.Category", Res.Strings.ToolbarControllers.CategoriesTreeTable.New.ToString (), new Shortcut (KeyCode.AlphaI | KeyCode.ModifierControl));
			this.toolbar.SetCommandDescription (ToolbarCommand.Delete,   "TreeTable.Delete",       Res.Strings.ToolbarControllers.CategoriesTreeTable.Delete.ToString ());
			this.toolbar.SetCommandDescription (ToolbarCommand.Deselect, null,                     Res.Strings.ToolbarControllers.CategoriesTreeTable.Deselect.ToString ());
			this.toolbar.SetCommandDescription (ToolbarCommand.Copy,     "TreeTable.Copy",         Res.Strings.ToolbarControllers.CategoriesTreeTable.Copy.ToString ());
			this.toolbar.SetCommandDescription (ToolbarCommand.Paste,    "TreeTable.Paste",        Res.Strings.ToolbarControllers.CategoriesTreeTable.Paste.ToString ());
			this.toolbar.SetCommandDescription (ToolbarCommand.Export,   null,                     Res.Strings.ToolbarControllers.CategoriesTreeTable.Export.ToString ());
			this.toolbar.SetCommandDescription (ToolbarCommand.Import,   CommandDescription.Empty);
			this.toolbar.SetCommandDescription (ToolbarCommand.Goto,     CommandDescription.Empty);
		}

		protected override void CreateNodeFiller()
		{
			this.dataFiller = new CategoriesTreeTableFiller (this.accessor, this.nodeGetter)
			{
				Title = this.title,
			};

			TreeTableFiller<SortableNode>.FillColumns (this.treeTableController, this.dataFiller, "View.Categories");

			this.sortingInstructions = TreeTableFiller<SortableNode>.GetSortingInstructions (this.treeTableController);
		}


		protected override void OnDeselect()
		{
			this.VisibleSelectedRow = -1;
		}

		protected override void OnNew()
		{
			var target = this.toolbar.GetTarget (ToolbarCommand.New);
			this.ShowCreatePopup (target);
		}

		protected override void OnDelete()
		{
			var target = this.toolbar.GetTarget (ToolbarCommand.Delete);

			YesNoPopup.Show (target, Res.Strings.ToolbarControllers.CategoriesTreeTable.DeleteQuestion.ToString (), delegate
			{
				this.accessor.RemoveObject (BaseType.Categories, this.SelectedGuid);
				this.UpdateData ();
				this.OnUpdateAfterDelete ();
			});
		}


		private void ShowCreatePopup(Widget target)
		{
			var popup = new CreateCategoryPopup (this.accessor)
			{
				ObjectModel = this.SelectedGuid,
			};

			popup.Create (target, leftOrRight: true);

			popup.ButtonClicked += delegate (object sender, string name)
			{
				if (name == "ok")
				{
					this.CreateObject (popup.ObjectName, popup.ObjectModel);
				}
			};
		}

		private void CreateObject(string name, Guid model)
		{
			var date = this.accessor.Mandat.StartDate;
			var guid = this.accessor.CreateObject (BaseType.Categories, date, name, Guid.Empty, addDefaultGroups: false);
			var obj = this.accessor.GetObject (BaseType.Categories, guid);
			System.Diagnostics.Debug.Assert (obj != null);

			if (!model.IsEmpty)
			{
				var objModel = this.accessor.GetObject (BaseType.Categories, model);
				this.accessor.CopyObject (obj, objModel);
			}
			
			this.UpdateData ();

			this.SelectedGuid = guid;
			this.OnUpdateAfterCreate (guid, EventType.Input, Timestamp.Now);  // Timestamp quelconque !
		}


		private SortableNodeGetter secondaryGetter;
	}
}
