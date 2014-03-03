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
	public class AccountsToolbarTreeTableController : AbstractToolbarTreeTableController<TreeNode>, IDirty
	{
		public AccountsToolbarTreeTableController(DataAccessor accessor, BaseType baseType)
			: base (accessor, baseType)
		{
			this.hasGraphic        = true;
			this.hasFilter         = false;
			this.hasTreeOperations = true;
			this.hasMoveOperations = false;

			//	GuidNode -> ParentPositionNode -> LevelNode -> TreeNode
			var primaryNodeGetter = this.accessor.GetNodeGetter (BaseType.Accounts);
			this.nodeGetter = new GroupTreeNodeGetter (this.accessor, BaseType.Accounts, primaryNodeGetter);

			this.title = StaticDescriptions.GetViewTypeDescription (ViewType.AccountsSettings);
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
			this.NodeGetter.SetParams (null, this.sortingInstructions);

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

				if (this.graphicController != null && this.showGraphic)
				{
					this.graphicController.SelectedGuid = this.SelectedGuid;
				}
			}
		}


		protected override void CreateNodeFiller()
		{
			this.dataFiller = new AccountsTreeTableFiller (this.accessor, this.nodeGetter);
			TreeTableFiller<TreeNode>.FillColumns (this.treeTableController, this.dataFiller);

			this.treeTableController.AddSortedColumn (0);
		}

		protected override void CreateGraphic(Widget parent)
		{
			this.graphicController = new AccountsGraphicViewController (this.accessor, this.baseType)
			{
				GraphicViewMode = GraphicViewMode.VerticalFinalNode,
			};

			this.graphicController.CreateUI (parent);

			this.graphicController.SelectedTileChanged += delegate
			{
				this.SelectedGuid = this.graphicController.SelectedGuid;
			};

			this.graphicController.TileDoubleClicked += delegate
			{
				this.SelectedGuid = this.graphicController.SelectedGuid;
				this.OnRowDoubleClicked (this.selectedRow);
			};
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

			if (target != null)
			{
				var popup = new YesNoPopup
				{
					Question = "Voulez-vous supprimer le compte sélectionné ?",
				};

				popup.Create (target, leftOrRight: true);

				popup.ButtonClicked += delegate (object sender, string name)
				{
					if (name == "yes")
					{
						this.accessor.RemoveObject (BaseType.Accounts, this.SelectedGuid);
						this.UpdateData ();
						this.OnUpdateAfterDelete ();
					}
				};
			}
		}


		private void ShowCreatePopup(Widget target)
		{
			var popup = new CreateGroupPopup (this.accessor, this.SelectedGuid);

			popup.Create (target, leftOrRight: true);

			popup.ButtonClicked += delegate (object sender, string name)
			{
				if (name == "create")
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

	
		protected override void UpdateToolbar()
		{
			base.UpdateToolbar ();

			this.toolbar.SetCommandEnable (ToolbarCommand.CompactAll, !this.NodeGetter.IsAllCompacted);
			this.toolbar.SetCommandEnable (ToolbarCommand.ExpandAll,  !this.NodeGetter.IsAllExpanded);
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
