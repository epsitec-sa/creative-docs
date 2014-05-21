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
	public class CategoriesToolbarTreeTableController : AbstractToolbarBothTreesController<SortableNode>, IDirty
	{
		public CategoriesToolbarTreeTableController(DataAccessor accessor, BaseType baseType)
			: base (accessor, baseType)
		{
			this.hasFilter         = false;
			this.hasTreeOperations = false;
			this.hasMoveOperations = false;

			this.NewCustomization      = new CommandCustomization ("TreeTable.New.Category", "Nouvelle catégorie d'immobilisation");
			this.DeleteCustomization   = new CommandCustomization (null, "Supprimer la catégorie d'immobilisation");
			this.DeselectCustomization = new CommandCustomization (null, "Désélectionner la catégorie d'immobilisation");
			this.CopyCustomization     = new CommandCustomization (null, "Copier la catégorie d'immobilisation");
			this.PasteCustomization    = new CommandCustomization (null, "Coller la catégorie d'immobilisation");
			this.ExportCustomization   = new CommandCustomization (null, "Exporter les catégories d'immobilisations");

			this.title = AbstractView.GetViewTitle (this.accessor, ViewType.Categories);

			var primary = this.accessor.GetNodeGetter (BaseType.Categories);
			this.secondaryGetter = new SortableNodeGetter (primary, this.accessor, BaseType.Categories);
			this.nodeGetter = new SorterNodeGetter (this.secondaryGetter);

			this.sortingInstructions = SortingInstructions.Default;
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


		protected override void CreateNodeFiller()
		{
			this.dataFiller = new CategoriesTreeTableFiller (this.accessor, this.nodeGetter);
			TreeTableFiller<SortableNode>.FillColumns (this.treeTableController, this.dataFiller);

			this.treeTableController.AddSortedColumn (0);
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

			YesNoPopup.Show (target, "Voulez-vous supprimer la catégorie sélectionnée ?", delegate
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
			var guid = this.accessor.CreateObject (BaseType.Categories, date, name, Guid.Empty);
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
