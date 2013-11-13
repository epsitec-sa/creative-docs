//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.App.DataFillers;
using Epsitec.Cresus.Assets.App.Popups;
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
			var primaryNodesGetter = this.accessor.GetNodesGetter (this.baseType);
			var ppNodeGetter = new ParentPositionNodesGetter (primaryNodesGetter, this.accessor, this.baseType);
			this.pp2lNodesGetter = new ParentPositionToLevelNodesGetter (ppNodeGetter, this.accessor, this.baseType);
			this.nodesGetter = new TreeObjectsNodesGetter (this.pp2lNodesGetter);

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


		public bool								DataFreezed;

		public void UpdateData()
		{
			this.pp2lNodesGetter.UpdateData ();
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
					this.dataFiller = new ObjectsTreeTableFiller (this.accessor, this.baseType, this.controller, this.nodesGetter);
					break;

				case BaseType.Categories:
					this.dataFiller = new CategoriesTreeTableFiller (this.accessor, this.baseType, this.controller, this.nodesGetter);
					break;

				case BaseType.Groups:
					this.dataFiller = new GroupsTreeTableFiller (this.accessor, this.baseType, this.controller, this.nodesGetter);
					break;
			}

			this.dataFiller.UpdateColumns ();

			this.UpdateData ();
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
			var modelGuid = this.SelectedGuid;
			if (modelGuid.IsEmpty)
			{
				return;
			}

			int sel = this.SelectedRow;
			if (sel == -1)
			{
				return;
			}

			var e = this.accessor.CreateObject (this.baseType, sel+1, modelGuid);

			this.UpdateData ();

			this.SelectedRow = sel+1;
			this.OnStartEditing (e.Type, e.Timestamp);
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


		private TreeObjectsNodesGetter NodesGetter
		{
			get
			{
				return this.nodesGetter as TreeObjectsNodesGetter;
			}
		}


		private ParentPositionToLevelNodesGetter	pp2lNodesGetter;
		private Timestamp?							timestamp;
	}
}
