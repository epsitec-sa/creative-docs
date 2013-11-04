//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.App.DataFillers;
using Epsitec.Cresus.Assets.App.Widgets;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.App.Views
{
	public abstract class AbstractToolbarTreeTableController
	{
		public AbstractToolbarTreeTableController(DataAccessor accessor)
		{
			this.accessor = accessor;

			this.nodes = new List<Node> ();
			this.nodeIndexes = new List<int> ();
		}

		public void CreateUI(Widget parent)
		{
			this.topTitle = new TopTitle
			{
				Parent = parent,
			};

			this.topTitle.SetTitle (this.title);

			this.toolbar = new TreeTableToolbar ();
			this.toolbar.CreateUI (parent);
			this.toolbar.HasTreeOperations = this.hasTreeOperations;

			this.CreateTreeTable (parent);
			this.CreateNodeFiller ();

			this.toolbar.CommandClicked += delegate (object sender, ToolbarCommand command)
			{
				switch (command)
				{
					case ToolbarCommand.First:
						this.OnFirst ();
						break;

					case ToolbarCommand.Last:
						this.OnLast ();
						break;

					case ToolbarCommand.Prev:
						this.OnPrev ();
						break;

					case ToolbarCommand.Next:
						this.OnNext ();
						break;

					case ToolbarCommand.CompactAll:
						this.OnCompactAll ();
						break;

					case ToolbarCommand.ExpandAll:
						this.OnExpandAll ();
						break;

					case ToolbarCommand.New:
						this.OnNew ();
						break;

					case ToolbarCommand.Delete:
						this.OnDelete ();
						break;

					case ToolbarCommand.Deselect:
						this.OnDeselect ();
						break;
				}
			};
		}


		public int								SelectedRow
		{
			get
			{
				return this.selectedRow;
			}
			set
			{
				if (this.selectedRow != value)
				{
					this.selectedRow = value;

					this.UpdateController ();
					this.UpdateToolbar ();

					this.OnSelectedRowChanged (this.selectedRow);
				}
			}
		}

		protected int							VisibleSelectedRow
		{
			get
			{
				if (this.selectedRow != -1)
				{
					return this.nodeIndexes.IndexOf (this.selectedRow);
				}
				else
				{
					return -1;
				}
			}
			set
			{
				if (value >= 0 && value < this.nodeIndexes.Count)
				{
					this.SelectedRow = this.nodeIndexes[value];
				}
				else
				{
					this.SelectedRow = -1;
				}
			}
		}


		private void OnFirst()
		{
			var index = this.FirstRowIndex;

			if (index.HasValue)
			{
				this.VisibleSelectedRow = index.Value;
			}
		}

		private void OnPrev()
		{
			var index = this.PrevRowIndex;

			if (index.HasValue)
			{
				this.VisibleSelectedRow = index.Value;
			}
		}

		private void OnNext()
		{
			var index = this.NextRowIndex;

			if (index.HasValue)
			{
				this.VisibleSelectedRow = index.Value;
			}
		}

		private void OnLast()
		{
			var index = this.LastRowIndex;

			if (index.HasValue)
			{
				this.VisibleSelectedRow = index.Value;
			}
		}

		protected virtual void OnNew()
		{
		}

		protected virtual void OnDelete()
		{
		}

		private void OnDeselect()
		{
			this.VisibleSelectedRow = -1;
		}


		private void CreateTreeTable(Widget parent)
		{
			this.selectedRow = -1;

			this.controller = new NavigationTreeTableController();

			var frame = new FrameBox
			{
				Parent = parent,
				Dock   = DockStyle.Fill,
			};

			this.controller.CreateUI (frame, footerHeight: 0);

			//	Pour que le calcul du nombre de lignes visibles soit correct.
			parent.Window.ForceLayout ();

			//	Connexion des événements.
			this.controller.ContentChanged += delegate (object sender, bool crop)
			{
				this.UpdateController (crop);
			};

			this.controller.RowClicked += delegate (object sender, int row)
			{
				this.VisibleSelectedRow = this.controller.TopVisibleRow + row;
			};

			this.controller.RowDoubleClicked += delegate (object sender, int row)
			{
				this.VisibleSelectedRow = this.controller.TopVisibleRow + row;
				this.OnRowDoubleClicked (this.VisibleSelectedRow);
			};

			this.controller.TreeButtonClicked += delegate (object sender, int row, TreeTableTreeType type)
			{
				this.OnCompactOrExpand (this.controller.TopVisibleRow + row);
			};
		}

		protected virtual void CreateNodeFiller()
		{
			this.dataFiller.UpdateColumns ();

			this.UpdateData ();
			this.UpdateController ();
			this.UpdateToolbar ();
		}

		protected virtual TreeTableColumnDescription[] TreeTableColumns
		{
			get
			{
				return null;
			}
		}

		protected void UpdateController(bool crop = true)
		{
			this.controller.RowsCount = this.nodeIndexes.Count;

			int visibleCount = this.controller.VisibleRowsCount;
			int rowsCount    = this.controller.RowsCount;
			int count        = System.Math.Min (visibleCount, rowsCount);
			int firstRow     = this.controller.TopVisibleRow;
			int selection    = this.VisibleSelectedRow;

			if (selection != -1)
			{
				//	La sélection ne peut pas dépasser le nombre maximal de lignes.
				selection = System.Math.Min (selection, rowsCount-1);

				//	Si la sélection est hors de la zone visible, on choisit un autre cadrage.
				if (crop && (selection < firstRow || selection >= firstRow+count))
				{
					firstRow = this.controller.GetTopVisibleRow (selection);
				}

				if (this.controller.TopVisibleRow != firstRow)
				{
					this.controller.TopVisibleRow = firstRow;
				}

				selection -= this.controller.TopVisibleRow;
			}

			this.dataFiller.UpdateContent (firstRow, count, selection, crop);
		}


		private void OnCompactOrExpand(int row)
		{
			//	Etend ou compacte une ligne (inverse son mode actuel).
			var guid = this.SelectedGuid;

			int i = this.nodeIndexes[row];
			var node = this.nodes[i];

			if (node.Type == TreeTableTreeType.Compacted)
			{
				this.nodes[i] = new Node (node.Guid, node.Level, TreeTableTreeType.Expanded);
			}
			else if (node.Type == TreeTableTreeType.Expanded)
			{
				this.nodes[i] = new Node (node.Guid, node.Level, TreeTableTreeType.Compacted);
			}

			this.UpdateNodeIndexes ();
			this.UpdateController ();
			this.UpdateToolbar ();

			this.SelectedGuid = guid;
		}

		private void OnCompactAll()
		{
			//	Compacte toutes les lignes.
			var guid = this.SelectedGuid;

			for (int i=0; i<this.nodes.Count; i++)
			{
				var node = this.nodes[i];

				if (node.Type == TreeTableTreeType.Expanded)
				{
					this.nodes[i] = new Node (node.Guid, node.Level, TreeTableTreeType.Compacted);
				}
			}

			this.UpdateNodeIndexes ();
			this.UpdateController ();
			this.UpdateToolbar ();

			this.SelectedGuid = guid;
		}

		private void OnExpandAll()
		{
			//	Etend toutes les lignes.
			var guid = this.SelectedGuid;

			for (int i=0; i<this.nodes.Count; i++)
			{
				var node = this.nodes[i];

				if (node.Type == TreeTableTreeType.Compacted)
				{
					this.nodes[i] = new Node (node.Guid, node.Level, TreeTableTreeType.Expanded);
				}
			}

			this.UpdateNodeIndexes ();
			this.UpdateController ();
			this.UpdateToolbar ();

			this.SelectedGuid = guid;
		}

		private Guid SelectedGuid
		{
			//	Retourne le Guid de l'objet actuellement sélectionné.
			get
			{
				int sel = this.VisibleSelectedRow;

				if (sel != -1 && sel < this.nodeIndexes.Count)
				{
					return this.nodes[this.nodeIndexes[sel]].Guid;
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
				int sel = -1;

				if (!value.IsEmpty)
				{
					var i = this.nodes.FindIndex (x => x.Guid == value);
					if (i != -1)
					{
						sel = this.nodeIndexes.Where (x => x <= i).Count () - 1;
					}
				}

				this.VisibleSelectedRow = sel;
			}
		}

		private bool IsAllCompacted
		{
			get
			{
				return !this.nodes.Where (x => x.Type == TreeTableTreeType.Expanded).Any ();
			}
		}

		private bool IsAllExpanded
		{
			get
			{
				return !this.nodes.Where (x => x.Type == TreeTableTreeType.Compacted).Any ();
			}
		}

		protected void UpdateData()
		{
			//	Met à jour toutes les données en mode étendu.
			this.nodes.Clear ();

			int count = this.DataCount;
			for (int i=0; i<count; i++)
			{
				Guid currentGuid;
				int currentLevel;
				this.GetData(i, out currentGuid, out currentLevel);

				//	Par défaut, on considére que la ligne ne peut être ni étendue
				//	ni compactée.
				var type = TreeTableTreeType.Final;

				if (i < count-2)
				{
					Guid nextGuid;
					int nextLevel;
					this.GetData (i+1, out nextGuid, out nextLevel);

					//	Si le noeud suivant a un niveau plus élevé, il s'agit d'une
					//	ligne pouvant être étendue ou compactée.
					if (nextLevel > currentLevel)
					{
						type = TreeTableTreeType.Expanded;
					}
				}

				var node = new Node (currentGuid, currentLevel, type);
				this.nodes.Add (node);
			}

			this.UpdateNodeIndexes ();
		}

		protected virtual int DataCount
		{
			//	Retourne le nombre total de données disponibles.
			get
			{
				return 0;
			}
		}

		protected virtual void GetData(int row, out Guid guid, out int level)
		{
			//	Retourne une donnée.
			guid = Guid.Empty;
			level = 0;
		}

		private void UpdateNodeIndexes()
		{
			//	Met à jour l'accès aux noeuds (nodeIndexes) en sautant les
			//	noeuds cachés.
			this.nodeIndexes.Clear ();

			bool skip = false;
			int skipLevel = 0;

			for (int i=0; i<this.nodes.Count; i++)
			{
				var node = this.nodes[i];

				if (skip)
				{
					if (node.Level <= skipLevel)
					{
						skip = false;
					}
					else
					{
						continue;
					}
				}

				if (node.Type == TreeTableTreeType.Compacted)
				{
					skip = true;
					skipLevel = node.Level;
				}

				this.nodeIndexes.Add (i);
			}
		}

		public int NodesCount
		{
			//	Retourne le nombre de noeuds visibles.
			get
			{
				return this.nodeIndexes.Count;
			}
		}

		protected Node GetNode(int i)
		{
			//	Retourne un noeud visible.
			return this.nodes[this.nodeIndexes[i]];
		}


		protected void UpdateToolbar()
		{
			int row = this.VisibleSelectedRow;

			this.UpdateCommand (ToolbarCommand.First, row, this.FirstRowIndex);
			this.UpdateCommand (ToolbarCommand.Prev,  row, this.PrevRowIndex);
			this.UpdateCommand (ToolbarCommand.Next,  row, this.NextRowIndex);
			this.UpdateCommand (ToolbarCommand.Last,  row, this.LastRowIndex);

			this.toolbar.UpdateCommand (ToolbarCommand.CompactAll, !this.IsAllCompacted);
			this.toolbar.UpdateCommand (ToolbarCommand.ExpandAll,  !this.IsAllExpanded);

			this.toolbar.UpdateCommand (ToolbarCommand.New,      true);
			this.toolbar.UpdateCommand (ToolbarCommand.Delete,   row != -1);
			this.toolbar.UpdateCommand (ToolbarCommand.Deselect, row != -1);
		}

		private void UpdateCommand(ToolbarCommand command, int selectedCell, int? newSelection)
		{
			bool enable = (newSelection.HasValue && selectedCell != newSelection.Value);
			this.toolbar.UpdateCommand (command, enable);
		}


		private int? FirstRowIndex
		{
			get
			{
				return 0;
			}
		}

		private int? PrevRowIndex
		{
			get
			{
				if (this.VisibleSelectedRow == -1)
				{
					return null;
				}
				else
				{
					int i = this.VisibleSelectedRow - 1;
					i = System.Math.Max (i, 0);
					i = System.Math.Min (i, this.NodesCount - 1);
					return i;
				}
			}
		}

		private int? NextRowIndex
		{
			get
			{
				if (this.VisibleSelectedRow == -1)
				{
					return null;
				}
				else
				{
					int i = this.VisibleSelectedRow + 1;
					i = System.Math.Max (i, 0);
					i = System.Math.Min (i, this.NodesCount - 1);
					return i;
				}
			}
		}

		private int? LastRowIndex
		{
			get
			{
				return this.NodesCount - 1;
			}
		}


		#region Events handler
		private void OnSelectedRowChanged(int row)
		{
			if (this.SelectedRowChanged != null)
			{
				this.SelectedRowChanged (this, row);
			}
		}

		public delegate void SelectedRowChangedEventHandler(object sender, int row);
		public event SelectedRowChangedEventHandler SelectedRowChanged;


		private void OnRowDoubleClicked(int row)
		{
			if (this.RowDoubleClicked != null)
			{
				this.RowDoubleClicked (this, row);
			}
		}

		public delegate void RowDoubleClickedEventHandler(object sender, int row);
		public event RowDoubleClickedEventHandler RowDoubleClicked;
		#endregion


		protected readonly DataAccessor			accessor;
		private readonly List<Node>				nodes;
		private readonly List<int>				nodeIndexes;

		protected AbstractDataFiller			dataFiller;
		protected BaseType						baseType;
		protected bool							hasTreeOperations;
		protected string						title;
		protected TopTitle						topTitle;
		protected TreeTableToolbar				toolbar;
		protected NavigationTreeTableController	controller;
		private int								selectedRow;
	}
}
