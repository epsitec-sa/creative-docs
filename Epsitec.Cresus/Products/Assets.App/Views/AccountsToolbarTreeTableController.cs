//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.App.Export;
using Epsitec.Cresus.Assets.App.Helpers;
using Epsitec.Cresus.Assets.App.Popups;
using Epsitec.Cresus.Assets.Data;
using Epsitec.Cresus.Assets.Server.DataFillers;
using Epsitec.Cresus.Assets.Server.NodeGetters;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.App.Views
{
	public class AccountsToolbarTreeTableController : AbstractToolbarBothTreesController<TreeNode>, IDirty
	{
		public AccountsToolbarTreeTableController(DataAccessor accessor, BaseType baseType)
			: base (accessor, baseType)
		{
			this.hasGraphic        = true;
			this.hasFilter         = false;
			this.hasDateRange      = true;
			this.hasTreeOperations = true;
			this.hasMoveOperations = false;

			//	GuidNode -> ParentPositionNode -> LevelNode -> TreeNode
			var primaryNodeGetter = this.accessor.GetNodeGetter (BaseType.Accounts);
			this.nodeGetter = new GroupTreeNodeGetter (this.accessor, BaseType.Accounts, primaryNodeGetter);
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
			this.graphicController = new AccountsTreeGraphicController (this.accessor, this.baseType);
		}


		public override void UpdateData()
		{
			this.NodeGetter.SetParams (null, this.sortingInstructions);

			this.title = AbstractView.GetViewTitle (this.accessor, ViewType.AccountsSettings)
				+ " — "
				+ this.accessor.Mandat.CurrentAccountsDateRange.ToNiceString ();

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


		protected override void AdaptToolbarCommand()
		{
			this.toolbar.SetCommandDescription (ToolbarCommand.DateRange, null, "Choix du plan comptable");
			this.toolbar.SetCommandDescription (ToolbarCommand.New,       null, "Nouveau compte");
			this.toolbar.SetCommandDescription (ToolbarCommand.Delete,    null, "Supprimer le compte");
			this.toolbar.SetCommandDescription (ToolbarCommand.Deselect,  null, "Désélectionner le compte");
			this.toolbar.SetCommandDescription (ToolbarCommand.Copy,      null, "Copier le compte");
			this.toolbar.SetCommandDescription (ToolbarCommand.Paste,     null, "Coller le compte");
			this.toolbar.SetCommandDescription (ToolbarCommand.Export,    null, "Exporter le plan comptable");
			this.toolbar.SetCommandDescription (ToolbarCommand.Import,    null, "Importer un plan comptable Crésus (fichier .cre)");
		}

		protected override void CreateNodeFiller()
		{
			this.dataFiller = new AccountsTreeTableFiller (this.accessor, this.nodeGetter);
			TreeTableFiller<TreeNode>.FillColumns (this.treeTableController, this.dataFiller, "View.Accounts");

			this.sortingInstructions = TreeTableFiller<TreeNode>.GetSortingInstructions (this.treeTableController);
		}


		protected override void OnDateRange()
		{
			var target = this.toolbar.GetTarget (ToolbarCommand.DateRange);
			this.ShowDateRangePopup (target);
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

			YesNoPopup.Show (target, "Voulez-vous supprimer le compte sélectionné ?", delegate
			{
				this.accessor.RemoveObject (BaseType.Accounts, this.SelectedGuid);
				this.UpdateData ();
				this.OnUpdateAfterDelete ();
			});
		}

		protected override void OnImport()
		{
			var target = this.toolbar.GetTarget (ToolbarCommand.Import);

			using (var h = new AccountsImportHelpers (this.accessor, target, this.UpdateAfterImport))
			{
				h.ShowImportPopup ();  // choix du fichier puis importation
			}
		}

		private void UpdateAfterImport()
		{
			this.SelectedRow = -1;  // car le plan comptable peut avoir changé
			this.UpdateData ();
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

				if (range == this.accessor.Mandat.CurrentAccountsDateRange)
				{
					popup.SelectedItem = i;  // sélectionne la période courante actuelle dans le popup
				}

				i++;
			}

			popup.Create (target, leftOrRight: true);

			popup.ItemClicked += delegate (object sender, int rank)
			{
				var range = this.accessor.Mandat.AccountsDateRanges.ToArray()[rank];
				this.accessor.Mandat.CurrentAccountsDateRange = range;  // change la période courante
				this.UpdateAfterImport ();
			};
		}

		private void ShowCreatePopup(Widget target)
		{
			var popup = new CreateGroupPopup (this.accessor)
			{
				ObjectParent = this.SelectedGuid,
			};

			popup.Create (target, leftOrRight: true);

			popup.ButtonClicked += delegate (object sender, string name)
			{
				if (name == "ok")
				{
					this.CreateObject (popup.ObjectName, popup.ObjectParent);
				}
			};
		}

		private void CreateObject(string name, Guid parent)
		{
			var date = this.accessor.Mandat.StartDate;
			var guid = this.accessor.CreateObject (BaseType.Accounts, date, name, parent);
			var obj = this.accessor.GetObject (BaseType.Accounts, guid);
			System.Diagnostics.Debug.Assert (obj != null);
			
			this.UpdateData ();

			this.SelectedGuid = guid;
			this.OnUpdateAfterCreate (guid, EventType.Input, Timestamp.Now);  // Timestamp quelconque !
		}




		protected override void ShowContextMenu(Point pos)
		{
			//	Pas de menu contextuel.
		}


		protected override void UpdateToolbar()
		{
			base.UpdateToolbar ();

			bool compactEnable = !this.NodeGetter.IsAllCompacted;
			bool expandEnable  = !this.NodeGetter.IsAllExpanded;

			this.toolbar.SetCommandEnable (ToolbarCommand.DateRange,  this.accessor.Mandat.AccountsDateRanges.Count () > 1);
			this.toolbar.SetCommandEnable (ToolbarCommand.CompactAll, compactEnable);
			this.toolbar.SetCommandEnable (ToolbarCommand.CompactOne, compactEnable);
			this.toolbar.SetCommandEnable (ToolbarCommand.ExpandOne,  expandEnable);
			this.toolbar.SetCommandEnable (ToolbarCommand.ExpandAll,  expandEnable);

			this.toolbar.SetCommandState (ToolbarCommand.New,    ToolbarCommandState.Hide);
			this.toolbar.SetCommandState (ToolbarCommand.Delete, ToolbarCommandState.Hide);
			this.toolbar.SetCommandState (ToolbarCommand.Copy,   ToolbarCommandState.Hide);
			this.toolbar.SetCommandState (ToolbarCommand.Paste,  ToolbarCommandState.Hide);
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
