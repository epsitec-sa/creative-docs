//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.App.Helpers;
using Epsitec.Cresus.Assets.App.Popups;
using Epsitec.Cresus.Assets.Server.BusinessLogic;
using Epsitec.Cresus.Assets.Server.DataFillers;
using Epsitec.Cresus.Assets.Server.NodesGetter;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.App.Views
{
	public class ObjectsToolbarTreeTableController : AbstractToolbarTreeTableController<TreeNode>
	{
		public ObjectsToolbarTreeTableController(DataAccessor accessor, BaseType baseType)
			: base (accessor, baseType)
		{
			this.hasTreeOperations = true;

			//	GuidNode -> ParentPositionNode -> LevelNode -> TreeNode
#if false
			var primaryNodesGetter = this.accessor.GetNodesGetter (this.baseType);
			this.nodesGetter = new TreeNodesGetter (this.accessor, this.baseType, primaryNodesGetter);
#else
			var groupNodesGetter = this.accessor.GetNodesGetter (BaseType.Groups);
			var objectNodesGetter = this.accessor.GetNodesGetter (BaseType.Objects);
			this.nodesGetter = new TreeNodesGetter2 (this.accessor, groupNodesGetter, objectNodesGetter);
#endif

			switch (this.baseType)
			{
				case BaseType.Objects:
					this.title = "Objets d'immobilisation";
					break;

				case BaseType.Categories:
					this.title = "Catégories d'immobilisation";
					break;

				case BaseType.Groups:
					this.title = "Groupes d'immobilisation";
					break;
			}
		}


		public override void CreateUI(Widget parent)
		{
			base.CreateUI (parent);

			this.stateAtController = new StateAtController ();
			this.stateAtController.CreateUI (parent);

			this.stateAtController.DateChanged += delegate
			{
				if (this.stateAtController.Date.HasValue)
				{
					this.NodesGetter.Timestamp = new Timestamp (this.stateAtController.Date.Value, 0);
				}
				else
				{
					this.NodesGetter.Timestamp = null;
				}

				this.UpdateController ();
				this.UpdateToolbar ();
			};
		}


		public bool								DataFreezed;

		public void UpdateData()
		{
			this.NodesGetter.UpdateData ();

			this.UpdateController ();
			this.UpdateToolbar ();
		}


		protected override int					VisibleSelectedRow
		{
			get
			{
				return this.NodesGetter.AllToVisible (this.selectedRow);
			}
			set
			{
				this.SelectedRow = this.NodesGetter.VisibleToAll (value);
			}
		}

		public Timestamp?						Timestamp
		{
			get
			{
				return this.timestamp;
			}
			set
			{
				if (this.timestamp != value)
				{
					this.timestamp = value;

					this.dataFiller.Timestamp = this.timestamp;

					this.UpdateController ();
					this.UpdateToolbar ();
				}
			}
		}

		public Guid								SelectedGuid
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
			//	Sélectionne l'objet ayant un Guid donné. Si la ligne correspondante
			//	est cachée, on est assez malin pour sélectionner la prochaine ligne
			//	visible, vers le haut.
			set
			{
				this.VisibleSelectedRow = this.NodesGetter.SearchBestIndex (value);
			}
		}


		protected override void CreateNodeFiller()
		{
			switch (this.baseType)
			{
				case BaseType.Objects:
					this.dataFiller = new ObjectsTreeTableFiller (this.accessor, this.nodesGetter);
					break;

				case BaseType.Categories:
					this.dataFiller = new CategoriesTreeTableFiller (this.accessor, this.nodesGetter);
					break;

				case BaseType.Groups:
					this.dataFiller = new GroupsTreeTableFiller (this.accessor, this.nodesGetter);
					break;
			}

			TreeTableFiller<TreeNode>.FillColumns (this.dataFiller, this.controller);

			this.UpdateData ();
		}


		protected override void OnFilter()
		{
			var target = this.toolbar.GetCommandWidget (ToolbarCommand.Filter);
			var popup = new ObjectsPopup (this.accessor, BaseType.Groups, this.NodesGetter.RootGuid, TreeNodeOutputMode.All);

			popup.Create (target, leftOrRight: true);

			popup.Navigate += delegate (object sender, Guid guid)
			{
				this.NodesGetter.RootGuid = guid;
				this.UpdateData ();
			};
		}

		private void OnCompactOrExpand(int row)
		{
			//	Etend ou compacte une ligne (inverse son mode actuel).
			var guid = this.SelectedGuid;

			this.NodesGetter.CompactOrExpand (row);
			this.UpdateController ();
			this.UpdateToolbar ();

			this.SelectedGuid = guid;
		}

		protected override void OnCompactAll()
		{
			//	Compacte toutes les lignes.
			var guid = this.SelectedGuid;

			this.NodesGetter.CompactAll ();
			this.UpdateController ();
			this.UpdateToolbar ();

			this.SelectedGuid = guid;
		}

		protected override void OnExpandAll()
		{
			//	Etend toutes les lignes.
			var guid = this.SelectedGuid;

			this.NodesGetter.ExpandAll ();
			this.UpdateController ();
			this.UpdateToolbar ();

			this.SelectedGuid = guid;
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
					Question = "Voulez-vous supprimer l'objet sélectionné ?",
				};

				popup.Create (target);

				popup.ButtonClicked += delegate (object sender, string name)
				{
					if (name == "yes")
					{
						this.accessor.RemoveObject (this.baseType, this.SelectedGuid);
						this.UpdateData ();
					}
				};
			}
		}


		private void ShowCreatePopup(Widget target)
		{
			var popup = new CreateObjectPopup (this.accessor, this.baseType, this.SelectedGuid);

			popup.Create (target, leftOrRight: true);

			popup.ButtonClicked += delegate (object sender, string name)
			{
				if (name == "create")
				{
					this.CreateObject (popup.ObjectDate.Value, popup.ObjectName, popup.ObjectParent, popup.ObjectGrouping);
				}
			};
		}

		private void CreateObject(System.DateTime date, string name, Guid parent, bool grouping)
		{
			var guid = this.accessor.CreateObject (this.baseType, date, name, parent, grouping);
			var obj = this.accessor.GetObject (this.baseType, guid);
			System.Diagnostics.Debug.Assert (obj != null);
			
			this.UpdateData ();

			this.SelectedGuid = guid;
			this.Timestamp = ObjectCalculator.GetLastTimestamp (obj);
			
			this.OnStartEditing (EventType.Entrée, this.timestamp.GetValueOrDefault ());
		}

	
		protected override void CreateTreeTable(Widget parent)
		{
			base.CreateTreeTable (parent);

			this.controller.TreeButtonClicked += delegate (object sender, int row, NodeType type)
			{
				this.OnCompactOrExpand (this.controller.TopVisibleRow + row);
			};
		}


		protected override void UpdateToolbar()
		{
			base.UpdateToolbar ();

			this.toolbar.UpdateCommand (ToolbarCommand.CompactAll, !this.NodesGetter.IsAllCompacted);
			this.toolbar.UpdateCommand (ToolbarCommand.ExpandAll,  !this.NodesGetter.IsAllExpanded);
		}


		private TreeNodesGetter2 NodesGetter
		{
			get
			{
				return this.nodesGetter as TreeNodesGetter2;
			}
		}


		private StateAtController					stateAtController;
		private Timestamp?							timestamp;
	}
}
