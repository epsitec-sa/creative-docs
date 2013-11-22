//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.App.Helpers;
using Epsitec.Cresus.Assets.App.Popups;
using Epsitec.Cresus.Assets.Server.DataFillers;
using Epsitec.Cresus.Assets.Server.NodesGetter;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.App.Views
{
	public class CategoriesToolbarTreeTableController : AbstractToolbarTreeTableController<OrderNode>
	{
		public CategoriesToolbarTreeTableController(DataAccessor accessor)
			: base (accessor)
		{
			this.hasFilter         = false;
			this.hasTreeOperations = false;

			var primary = this.accessor.GetNodesGetter (BaseType.Categories);
			this.secondaryGetter = new OrderNodesGetter (primary, this.accessor, BaseType.Categories);
			this.nodesGetter = new SortNodesGetter (this.secondaryGetter);

			this.title = "Catégories d'immobilisation";
		}


		public void UpdateData()
		{
			this.nodesGetter.UpdateData ();

			this.UpdateController ();
			this.UpdateToolbar ();
		}


		public override Guid					SelectedGuid
		{
			//	Retourne le Guid de l'objet actuellement sélectionné.
			get
			{
				int sel = this.VisibleSelectedRow;
				if (sel != -1 && sel < this.nodesGetter.Count)
				{
					return this.nodesGetter[sel].Guid;
				}
				else
				{
					return Guid.Empty;
				}
			}
			//	Sélectionne l'objet ayant un Guid donné.
			set
			{
				this.VisibleSelectedRow = this.nodesGetter.Nodes.ToList ().FindIndex (x => x.Guid == value);
			}
		}


		protected override void CreateNodeFiller()
		{
			this.dataFiller = new CategoriesTreeTableFiller (this.accessor, this.nodesGetter);
			TreeTableFiller<OrderNode>.FillColumns (this.dataFiller, this.controller);

			this.controller.AddSortedColumn (0);
		}


		protected override void OnDeselect()
		{
			this.VisibleSelectedRow = -1;
		}

		protected override void OnNew()
		{
			var target = this.toolbar.GetCommandWidget (ToolbarCommand.New);
			this.ShowCreatePopup (target);
		}

		protected override void OnDelete()
		{
			var target = this.toolbar.GetCommandWidget (ToolbarCommand.Delete);

			if (target != null)
			{
				var popup = new YesNoPopup
				{
					Question = "Voulez-vous supprimer la catégorie sélectionnée ?",
				};

				popup.Create (target);

				popup.ButtonClicked += delegate (object sender, string name)
				{
					if (name == "yes")
					{
						this.accessor.RemoveObject (BaseType.Categories, this.SelectedGuid);
						this.UpdateData ();
					}
				};
			}
		}


		protected override void SetSortingInstructions(SortingInstructions instructions)
		{
			this.secondaryGetter.SortingInstructions = instructions;
			(this.nodesGetter as SortNodesGetter).SortingInstructions = instructions;

			this.UpdateData ();
		}


		private void ShowCreatePopup(Widget target)
		{
			var popup = new CreateCategoryPopup (this.accessor, BaseType.Categories, this.SelectedGuid);

			popup.Create (target, leftOrRight: true);

			popup.ButtonClicked += delegate (object sender, string name)
			{
				if (name == "create")
				{
					this.CreateObject (popup.ObjectName);
				}
			};
		}

		private void CreateObject(string name)
		{
			var date = Timestamp.Now.Date;
			var guid = this.accessor.CreateObject (BaseType.Categories, date, name, Guid.Empty);
			var obj = this.accessor.GetObject (BaseType.Categories, guid);
			System.Diagnostics.Debug.Assert (obj != null);
			
			this.UpdateData ();

			this.SelectedGuid = guid;
			this.OnStartEditing (EventType.Entrée, Timestamp.Now);  // Timestamp quelconque !
		}


		private OrderNodesGetter secondaryGetter;
	}
}
