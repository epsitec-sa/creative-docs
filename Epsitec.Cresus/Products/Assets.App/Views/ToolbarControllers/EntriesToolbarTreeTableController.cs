//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Cresus.Assets.App.Helpers;
using Epsitec.Cresus.Assets.App.Views.CommandToolbars;
using Epsitec.Cresus.Assets.App.Views.TreeGraphicControllers;
using Epsitec.Cresus.Assets.Data;
using Epsitec.Cresus.Assets.Server.DataFillers;
using Epsitec.Cresus.Assets.Server.NodeGetters;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.App.Views.ToolbarControllers
{
	public class EntriesToolbarTreeTableController : AbstractToolbarBothTreesController<EntryNode>, IDirty, System.IDisposable
	{
		public EntriesToolbarTreeTableController(DataAccessor accessor, CommandContext commandContext, BaseType baseType)
			: base (accessor, commandContext, baseType)
		{
			this.title = AbstractView.GetViewTitle (this.accessor, ViewType.Entries);

			this.nodeGetter = new EntriesNodeGetter (this.accessor);
		}

		public override void Dispose()
		{
			base.Dispose ();
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


		protected override void CreateToolbar()
		{
			this.toolbar = new EntriesToolbar (this.accessor, this.commandContext);
		}

		protected override void CreateNodeFiller()
		{
			this.dataFiller = new EntriesTreeTableFiller (this.accessor, this.nodeGetter)
			{
				Title = this.title,
			};

			TreeTableFiller<EntryNode>.FillColumns (this.treeTableController, this.dataFiller, "View.Entries");

			this.sortingInstructions = TreeTableFiller<EntryNode>.GetSortingInstructions (this.treeTableController);
		}


		[Command (Res.CommandIds.Entries.First)]
		protected override void OnFirst()
		{
			base.OnFirst ();
		}

		[Command (Res.CommandIds.Entries.Prev)]
		protected override void OnPrev()
		{
			base.OnPrev ();
		}

		[Command (Res.CommandIds.Entries.Next)]
		protected override void OnNext()
		{
			base.OnNext ();
		}

		[Command (Res.CommandIds.Entries.Last)]
		protected override void OnLast()
		{
			base.OnLast ();
		}

		[Command (Res.CommandIds.Entries.CompactAll)]
		protected override void OnCompactAll()
		{
			base.OnCompactAll ();
		}

		[Command (Res.CommandIds.Entries.CompactOne)]
		protected override void OnCompactOne()
		{
			base.OnCompactOne ();
		}

		[Command (Res.CommandIds.Entries.ExpandOne)]
		protected override void OnExpandOne()
		{
			base.OnExpandOne ();
		}

		[Command (Res.CommandIds.Entries.ExpandAll)]
		protected override void OnExpandAll()
		{
			base.OnExpandAll ();
		}

		[Command (Res.CommandIds.Entries.Deselect)]
		protected void OnDeselect()
		{
			this.VisibleSelectedRow = -1;
		}


		protected override void ShowContextMenu(Point pos)
		{
			//	Pas de menu contextuel.
		}


		protected override void UpdateToolbar()
		{
			base.UpdateToolbar ();

			int row = this.VisibleSelectedRow;

			this.UpdateSelCommand (Res.Commands.Entries.First, row, this.FirstRowIndex);
			this.UpdateSelCommand (Res.Commands.Entries.Prev,  row, this.PrevRowIndex);
			this.UpdateSelCommand (Res.Commands.Entries.Next,  row, this.NextRowIndex);
			this.UpdateSelCommand (Res.Commands.Entries.Last,  row, this.LastRowIndex);

			bool compactEnable = !this.NodeGetter.IsAllCompacted;
			bool expandEnable  = !this.NodeGetter.IsAllExpanded;

			this.toolbar.SetEnable (Res.Commands.Entries.CompactAll, compactEnable);
			this.toolbar.SetEnable (Res.Commands.Entries.ExpandAll,  expandEnable);

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
