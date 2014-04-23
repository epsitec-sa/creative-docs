//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.App.Helpers;
using Epsitec.Cresus.Assets.Server.DataFillers;
using Epsitec.Cresus.Assets.Server.NodeGetters;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.App.Views
{
	public class EntriesToolbarTreeTableController : AbstractToolbarBothTreesController<EntryNode>, IDirty
	{
		public EntriesToolbarTreeTableController(DataAccessor accessor, BaseType baseType)
			: base (accessor, baseType)
		{
			this.hasGraphic        = true;
			this.hasFilter         = false;
			this.hasTreeOperations = true;
			this.hasMoveOperations = false;

			this.title = AbstractView.GetViewTitle (this.accessor, ViewType.Entries);

			this.nodeGetter = new EntriesNodeGetter (this.accessor);

			this.sortingInstructions = new SortingInstructions (ObjectField.EntryDate, SortedType.Ascending, ObjectField.Unknown, SortedType.None);
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


		protected override void CreateGraphicControllerUI()
		{
			this.graphicController = new EntriesTreeGraphicController (this.accessor, this.baseType);
		}


		public override void UpdateData()
		{
			this.NodeGetter.SetParams (this.sortingInstructions);

			this.UpdateController ();
			this.UpdateToolbar ();
		}


		protected override int					VisibleSelectedRow
		{
			get
			{
				return this.NodeGetter.AllToVisible (this.selectedRow);
			}
			set
			{
				this.SelectedRow = this.NodeGetter.VisibleToAll (value);
			}
		}

		public override Guid					SelectedGuid
		{
			//	Retourne le Guid de l'objet actuellement sélectionné.
			get
			{
				int sel = this.VisibleSelectedRow;
				if (sel != -1 && sel < this.nodeGetter.Count)
				{
					return this.nodeGetter[sel].EntryGuid;
				}
				else
				{
					return Guid.Empty;
				}
			}
			//	Sélectionne l'objet ayant un Guid donné. Si la ligne correspondante
			//	est cachée, on est assez malin pour sélectionner la prochaine ligne
			//	visible, vers le haut.
			set
			{
				this.VisibleSelectedRow = this.NodeGetter.SearchBestIndex (value);
			}
		}


		protected override void CreateNodeFiller()
		{
			this.dataFiller = new EntriesTreeTableFiller (this.accessor, this.nodeGetter);
			TreeTableFiller<EntryNode>.FillColumns (this.treeTableController, this.dataFiller);

			this.treeTableController.AddSortedColumn (0);
		}


		protected override void OnDeselect()
		{
			this.VisibleSelectedRow = -1;
		}

	
		protected override void UpdateToolbar()
		{
			base.UpdateToolbar ();

			this.toolbar.SetCommandEnable (ToolbarCommand.CompactAll, !this.NodeGetter.IsAllCompacted);
			this.toolbar.SetCommandEnable (ToolbarCommand.CompactOne, !this.NodeGetter.IsAllCompacted);
			this.toolbar.SetCommandEnable (ToolbarCommand.ExpandOne,  !this.NodeGetter.IsAllExpanded);
			this.toolbar.SetCommandEnable (ToolbarCommand.ExpandAll,  !this.NodeGetter.IsAllExpanded);

			this.toolbar.SetCommandEnable (ToolbarCommand.New,    false);
			this.toolbar.SetCommandEnable (ToolbarCommand.Delete, false);
		}


		private EntriesNodeGetter NodeGetter
		{
			get
			{
				return this.nodeGetter as EntriesNodeGetter;
			}
		}
	}
}
