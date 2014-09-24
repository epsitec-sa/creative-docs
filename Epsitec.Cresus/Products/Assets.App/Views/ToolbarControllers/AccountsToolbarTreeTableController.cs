//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Cresus.Assets.App.Export;
using Epsitec.Cresus.Assets.App.Helpers;
using Epsitec.Cresus.Assets.App.Popups;
using Epsitec.Cresus.Assets.App.Views.CommandToolbars;
using Epsitec.Cresus.Assets.App.Views.TreeGraphicControllers;
using Epsitec.Cresus.Assets.Data;
using Epsitec.Cresus.Assets.Server.DataFillers;
using Epsitec.Cresus.Assets.Server.NodeGetters;
using Epsitec.Cresus.Assets.Server.SimpleEngine;
using Epsitec.Cresus.Assets.Server.BusinessLogic;

namespace Epsitec.Cresus.Assets.App.Views.ToolbarControllers
{
	public class AccountsToolbarTreeTableController : AbstractToolbarBothTreesController<TreeNode>, IDirty, System.IDisposable
	{
		public AccountsToolbarTreeTableController(DataAccessor accessor, CommandContext commandContext, BaseType baseType)
			: base (accessor, commandContext, baseType)
		{
			//	GuidNode -> ParentPositionNode -> LevelNode -> TreeNode
			var primaryNodeGetter = this.accessor.GetNodeGetter (this.baseType);
			this.nodeGetter = new GroupTreeNodeGetter (this.accessor, this.baseType, primaryNodeGetter);
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


		protected override void CreateGraphicControllerUI()
		{
			this.graphicController = new AccountsTreeGraphicController (this.accessor, this.baseType);
		}


		public override void UpdateData()
		{
			this.NodeGetter.SetParams (null, this.sortingInstructions);

			this.title = UniversalLogic.NiceJoin (
				AbstractView.GetViewTitle (this.accessor, ViewType.Accounts),
				this.baseType.AccountsDateRange.ToNiceString ());

			this.topTitle.SetTitle (this.title);

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
					return this.nodeGetter[sel].Guid;
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
			this.toolbar = new AccountsToolbar (this.accessor, this.commandContext);
		}

		protected override void CreateNodeFiller()
		{
			this.dataFiller = new AccountsTreeTableFiller (this.accessor, this.nodeGetter)
			{
				Title = this.title,
			};

			TreeTableFiller<TreeNode>.FillColumns (this.treeTableController, this.dataFiller, "View.Accounts");

			this.sortingInstructions = TreeTableFiller<TreeNode>.GetSortingInstructions (this.treeTableController);
		}


		[Command (Res.CommandIds.Accounts.DateRange)]
		protected void OnDateRange(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			var target = this.toolbar.GetTarget (e);
			this.ShowDateRangePopup (target);
		}

		[Command (Res.CommandIds.Accounts.Graphic)]
		protected override void OnGraphic()
		{
			base.OnGraphic ();
		}

		[Command (Res.CommandIds.Accounts.First)]
		protected override void OnFirst()
		{
			base.OnFirst ();
		}

		[Command (Res.CommandIds.Accounts.Prev)]
		protected override void OnPrev()
		{
			base.OnPrev ();
		}

		[Command (Res.CommandIds.Accounts.Next)]
		protected override void OnNext()
		{
			base.OnNext ();
		}

		[Command (Res.CommandIds.Accounts.Last)]
		protected override void OnLast()
		{
			base.OnLast ();
		}

		[Command (Res.CommandIds.Accounts.CompactAll)]
		protected override void OnCompactAll()
		{
			base.OnCompactAll ();
		}

		[Command (Res.CommandIds.Accounts.CompactOne)]
		protected override void OnCompactOne()
		{
			base.OnCompactOne ();
		}

		[Command (Res.CommandIds.Accounts.ExpandOne)]
		protected override void OnExpandOne()
		{
			base.OnExpandOne ();
		}

		[Command (Res.CommandIds.Accounts.ExpandAll)]
		protected override void OnExpandAll()
		{
			base.OnExpandAll ();
		}

		[Command (Res.CommandIds.Accounts.Deselect)]
		protected void OnDeselect()
		{
			this.VisibleSelectedRow = -1;
		}

		[Command (Res.CommandIds.Accounts.Import)]
		protected void OnImport(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			var target = this.toolbar.GetTarget (e);

			using (var h = new AccountsImportHelpers (this.accessor, target, this.UpdateAfterImport))
			{
				h.ShowImportPopup ();  // choix du fichier puis importation
			}
		}

		[Command (Res.CommandIds.Accounts.Export)]
		protected override void OnExport(CommandDispatcher dispatcher, CommandEventArgs e)
		{
			base.OnExport (dispatcher, e);
		}


		private void UpdateAfterImport()
		{
			this.OnChangeView (ViewType.FromDefaultKind (this.accessor, ViewTypeKind.Accounts));
		}


		private void ShowDateRangePopup(Widget target)
		{
			//	Affiche la liste des périodes des plans comptables connus, afin d'en
			//	choisir une.
			var popup = new SimplePopup ();

			int i = 0;
			foreach (var range in this.accessor.Mandat.AccountsDateRanges)
			{
				popup.Items.Add ("Période " + range.ToNiceString ());

				if (range == this.baseType.AccountsDateRange)
				{
					popup.SelectedItem = i;  // sélectionne la période courante actuelle dans le popup
				}

				i++;
			}

			popup.Create (target, leftOrRight: true);

			popup.ItemClicked += delegate (object sender, int rank)
			{
				var range = this.accessor.Mandat.AccountsDateRanges.ToArray ()[rank];
				this.OnChangeView (new ViewType(ViewTypeKind.Accounts, range));
			};
		}


		protected override void ShowContextMenu(Point pos)
		{
			//	Pas de menu contextuel.
		}


		protected override void UpdateToolbar()
		{
			base.UpdateToolbar ();

			this.toolbar.SetEnable (Res.Commands.Accounts.DateRange, this.accessor.Mandat.AccountsDateRanges.Count () > 1);

			int row = this.VisibleSelectedRow;

			this.UpdateSelCommand (Res.Commands.Accounts.First, row, this.FirstRowIndex);
			this.UpdateSelCommand (Res.Commands.Accounts.Prev,  row, this.PrevRowIndex);
			this.UpdateSelCommand (Res.Commands.Accounts.Next,  row, this.NextRowIndex);
			this.UpdateSelCommand (Res.Commands.Accounts.Last,  row, this.LastRowIndex);

			bool compactEnable = !this.NodeGetter.IsAllCompacted;
			bool expandEnable  = !this.NodeGetter.IsAllExpanded;

			this.toolbar.SetEnable (Res.Commands.Accounts.CompactAll, compactEnable);
			this.toolbar.SetEnable (Res.Commands.Accounts.CompactOne, compactEnable);
			this.toolbar.SetEnable (Res.Commands.Accounts.ExpandOne,  expandEnable);
			this.toolbar.SetEnable (Res.Commands.Accounts.ExpandAll,  expandEnable);
		}


		private GroupTreeNodeGetter NodeGetter
		{
			get
			{
				return this.nodeGetter as GroupTreeNodeGetter;
			}
		}
	}
}
