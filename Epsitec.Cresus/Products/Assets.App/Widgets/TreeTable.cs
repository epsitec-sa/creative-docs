//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.App.Helpers;
using Epsitec.Cresus.Assets.Server.DataFillers;
using Epsitec.Common.Support;
using Epsitec.Cresus.Assets.Data;

namespace Epsitec.Cresus.Assets.App.Widgets
{
	/// <summary>
	/// TreeTable de base, constituée de lignes AbstractTreeTableColumn créées créées avec SetColumns.
	/// La première colonne de gauche est spéciale; elle contient les informations sur l'arborescence
	/// et elle ne fait pas partie des colonnes scrollables horizontalement.
	/// On ne gère ici aucun déplacement vertical.
	/// On se contente d'afficher les AbstractTreeTableColumn passées avec SetColumns.
	/// Un seul événement RowClicked permet de connaître la colonne et la ligne cliquée.
	/// </summary>
	public class TreeTable : Widget
	{
		public TreeTable(int rowHeight, int headerHeight, int footerHeight)
		{
			this.rowHeight    = rowHeight;
			this.headerHeight = headerHeight;
			this.footerHeight = footerHeight;

			this.hoverMode = TreeTableHoverMode.VerticalGradient;

			this.treeTableColumns = new List<AbstractTreeTableColumn> ();

			//	Crée le conteneur de gauche, qui contiendra toutes les colonnes
			//	en mode DockToLeft (habituellement la seule TreeTableColumnTree).
			//	Ce conteneur n'est pas scrollable horizontalement; sa largeur
			//	s'adapte en fonctions du total des colonnes contenues.
			this.leftContainer = new FrameBox
			{
				Parent         = this,
				Dock           = DockStyle.Left,
				PreferredWidth = 0,
				Margins        = new Margins (0, 0, 0, AbstractScroller.DefaultBreadth),
			};

			//	Crée le conteneur de droite, qui contiendra toutes les colonnes
			//	qui n'ont pas le mode DockToLeft. Ce conteneur est scrollable
			//	horizontalement.
			this.columnsContainer = new Scrollable
			{
				Parent                 = this,
				HorizontalScrollerMode = ScrollableScrollerMode.ShowAlways,
				VerticalScrollerMode   = ScrollableScrollerMode.HideAlways,
				Dock                   = DockStyle.Fill,
			};

			this.columnsContainer.Viewport.IsAutoFitting = true;

			//	Crée les surcouches interactives. La permier surcouche sera dessinée
			//	sous les autres. La dernière surcouche est celle qui réagit prioritairement
			//	aux autres.
			this.interactiveLayers = new List<AbstractInteractiveLayer> ();

			this.interactiveLayers.Add (new InteractiveLayerColumnOrder (this));
			this.interactiveLayers.Add (new InteractiveLayerColumnSeparator (this));
			this.interactiveLayers.Add (new InteractiveLayerColumnResurrect (this));

			foreach (var layer in this.interactiveLayers)
			{
				layer.CreateUI ();
			}

			this.AllowsMovement = true;
			this.AllowsSorting  = true;
		}


		public bool								AllowsMovement;

		public bool								AllowsSorting
		{
			get
			{
				return this.allowsSorting;
			}
			set
			{
				if (this.allowsSorting != value)
				{
					this.allowsSorting = value;
					this.UpdateSortedColumns ();
				}
			}
		}

		public int								VScrollerTopMargin
		{
			get
			{
				return this.headerHeight;
			}
		}

		public int								VScrollerBottomMargin
		{
			get
			{
				return this.footerHeight + (int) AbstractScroller.DefaultBreadth;
			}
		}

		public int								VisibleRowsCount
		{
			get
			{
				return (int) ((this.ActualHeight - this.headerHeight - this.footerHeight - AbstractScroller.DefaultBreadth) / this.rowHeight);
			}
		}

		public int								HeaderHeight
		{
			get
			{
				return this.headerHeight;
			}
		}

		public int								FooterHeight
		{
			get
			{
				return this.footerHeight;
			}
		}

		public FrameBox							LeftContainer
		{
			get
			{
				return this.leftContainer;
			}
		}

		public Scrollable						ColumnsContainer
		{
			get
			{
				return this.columnsContainer;
			}
		}

		public int								DockToLeftCount
		{
			get
			{
				return this.columnsState.DockToLeftCount;
			}
		}

		public List<AbstractTreeTableColumn>	TreeTableColumns
		{
			get
			{
				return this.treeTableColumns;
			}
		}


		public IEnumerable<SortedColumn>		SortedColumns
		{
			get
			{
				return this.columnsState.Sorted;
			}
		}

		public ColumnsState						ColumnsState
		{
			get
			{
				return this.columnsState;
			}
			set
			{
				this.columnsState = value;
				this.CreateColumns ();
			}
		}

		public void AddSortedColumn(ObjectField field)
		{
			var list = this.columnsState.Sorted.ToList ();

			if (list.Count == 0)
			{
				list.Add (new SortedColumn (field, SortedType.Ascending));
			}
			else
			{
				if (field == list[0].Field)
				{
					if (list[0].Type == SortedType.Ascending)
					{
						list[0] = new SortedColumn (field, SortedType.Descending);
					}
					else
					{
						list[0] = new SortedColumn (field, SortedType.Ascending);
					}
				}
				else
				{
					list.Insert (0, new SortedColumn (field, SortedType.Ascending));

					//	Jamais plus de 2 (un tri primaire et un tri secondaire).
					while (list.Count > 2)
					{
						list.RemoveAt (2);
					}
				}
			}

			this.columnsState = new ColumnsState (this.columnsState.Mapper, this.columnsState.Columns, list.ToArray (), this.columnsState.DockToLeftCount);

			this.UpdateSortedColumns ();
			this.OnSortingChanged ();
		}

		private void UpdateSortedColumns()
		{
			if (this.columnsState.Sorted == null)  // garde-fou
			{
				return;
			}

			var sorted = this.columnsState.Sorted.ToList();

			for (int i=0; i<this.treeTableColumns.Count; i++)
			{
				var column = this.treeTableColumns[i];
				int j = sorted.FindIndex (x => x.Field == column.Field);

				if (j == -1 || !this.AllowsSorting)
				{
					column.SetSortedColumn (SortedType.None, false);
				}
				else
				{
					column.SetSortedColumn (sorted[j].Type, j == 0);
				}
			}
		}


		public void SetColumns(TreeTableColumnDescription[] descriptions, ObjectField defaultSortedField, int defaultDockToLeftCount)
		{
			//	Spécifie les colonnes à afficher, et réinitialise le mapping ainsi
			//	que les largeurs courantes.
			this.columnDescriptions = descriptions;

			var mapper = new int[this.columnDescriptions.Length];
			var columnState = new ColumnState[this.columnDescriptions.Length];

			for (int i=0; i<this.columnDescriptions.Length; i++)
			{
				var columnDescription = this.columnDescriptions[i];
				mapper[i] = i;
				columnState[i] = new ColumnState (columnDescription.Field, columnDescription.Width, false);
			}

			var sorted = new SortedColumn[1];
			sorted[0] = new SortedColumn (defaultSortedField, SortedType.Ascending);

			this.columnsState = new ColumnsState (mapper, columnState, sorted, defaultDockToLeftCount);

			this.CreateColumns ();
		}

		public void SetColumnWidth(int rank, int? width)
		{
			//	Modifie la largeur d'une colonne.
			//	Si width == 0		->	cache la colonne
			//	Si width == null	->	restitue la largeur originale
			rank = this.columnsState.MappedToAbsolute (rank);

			var field = this.columnsState.Columns[rank].Field;
			int newWidth;
			bool hide;

			if (width.HasValue)
			{
				if (width == 0)  // cache la colonne ?
				{
					newWidth = this.columnsState.Columns[rank].OriginalWidth;
					hide = true;
				}
				else  // modifie la largeur ?
				{
					newWidth = width.Value;
					hide = false;
				}
			}
			else  // restitue la largeur originale ?
			{
				newWidth = this.columnsState.Columns[rank].OriginalWidth;
				hide = false;
			}

			this.columnsState.Columns[rank] = new ColumnState (field, newWidth, hide);

			this.GetColumn (rank).PreferredWidth = this.columnsState.Columns[rank].FinalWidth;
		}

		public void ChangeColumnOrder(int columnSrc, TreeTableColumnSeparator separatorDst)
		{
			//	Change l'ordre des colonnes en déplaçant une colonne vers la gauche
			//	ou vers la droite. On modifie simplement le 'mapping', puis on
			//	reconstruit le tableau.
			var dockToLeftCount = this.columnsState.DockToLeftCount;

			bool srcDockToLeft = (columnSrc < dockToLeftCount);
			bool dstDockToLeft = (separatorDst.Rank < dockToLeftCount);

			//	Si on est sur la frontière des conteneurs left|scrollable, on force
			//	la destination dans la bonne direction.
			if (separatorDst.Left)
			{
				dstDockToLeft = true;
			}

			if (separatorDst.Right)
			{
				dstDockToLeft = false;
			}

			var mapper = this.columnsState.Mapper.ToList ();

			if (separatorDst.Rank <= columnSrc)  // déplacement vers la gauche ?
			{
				int x = mapper[columnSrc];
				mapper.RemoveAt (columnSrc);
				mapper.Insert (separatorDst.Rank, x);

				if (!srcDockToLeft && dstDockToLeft)
				{
					dockToLeftCount++;
				}
			}
			else  // déplacement vers la droite ?
			{
				int x = mapper[columnSrc];
				mapper.RemoveAt (columnSrc);
				mapper.Insert (separatorDst.Rank-1, x);

				if (srcDockToLeft && !dstDockToLeft)
				{
					System.Diagnostics.Debug.Assert (dockToLeftCount > 0);
					dockToLeftCount--;
				}
			}

			this.columnsState = new ColumnsState (mapper.ToArray (), this.columnsState.Columns, this.columnsState.Sorted, dockToLeftCount);

			this.CreateColumns ();
			this.OnContentChanged (true);  // on demande de mettre à jour le contenu
		}

		private void CreateColumns()
		{
			//	Crée les widgets des différentes colonnes, selon le mapping.
			this.treeTableColumns.Clear ();
			this.leftContainer.Children.Clear ();
			this.columnsContainer.Viewport.Children.Clear ();

			int index = 0;

			foreach (var columnState in this.columnsState.MappedColumns)
			{
				var description = this.columnDescriptions.Where (x => x.Field == columnState.Field).FirstOrDefault ();

				var column = TreeTableColumnHelper.Create (description);
				column.PreferredWidth = columnState.FinalWidth;
				column.Index = index;

				this.treeTableColumns.Add (column);

				if (index < this.columnsState.DockToLeftCount)  // dans le conteneur fixe de gauche ?
				{
					column.DockToLeft = true;
					column.Dock = DockStyle.Left;
					this.leftContainer.Children.Add (column);
				}
				else  // dans le conteneur scrollable de droite ?
				{
					column.DockToLeft = false;
					column.Dock = DockStyle.Left;
					this.columnsContainer.Viewport.Children.Add (column);
				}

				column.ChildrenMouseMove += delegate (object sender, int row)
				{
					this.SetHilitedHoverRow (row);
				};

				if (column is TreeTableColumnTree)
				{
					var tree = column as TreeTableColumnTree;

					tree.TreeButtonClicked += delegate (object sender, int row, NodeType type)
					{
						this.OnTreeButtonClicked (row, type);
					};
				}

				index++;
			}

			this.UpdateHoverMode ();
			this.UpdateChildrensGeometry ();
		}

		public void SetColumnCells(int rank, TreeTableColumnItem columnItem)
		{
			var columnWidget = this.GetColumn (rank);
			columnWidget.SetCells (columnItem);
		}


		protected override void OnExited(MessageEventArgs e)
		{
			this.ProcessMouseLeave (e.Point);

			base.OnExited (e);
		}

		protected override void ProcessMessage(Message message, Point pos)
		{
			if (message.IsMouseType)
			{
				if (message.MessageType == MessageType.MouseDown)
				{
					this.ProcessMouseDown (pos);
					this.ProcessMouseClick (pos);
				}
				else if (message.MessageType == MessageType.MouseMove)
				{
					this.ProcessMouseMove (pos);
				}
				else if (message.MessageType == MessageType.MouseUp)
				{
					this.ProcessMouseUp (pos);
					this.ProcessMouseMove (pos);
				}
				else if (message.MessageType == MessageType.MouseLeave)
				{
					this.ProcessMouseLeave (pos);
				}
			}
		
			base.ProcessMessage (message, pos);
		}

		protected override void OnDoubleClicked(MessageEventArgs e)
		{
			int row = this.DetectRow (e.Point);
			if (row != -1)
			{
				this.OnRowDoubleClicked (row);
			}

			base.OnDoubleClicked (e);
		}

		private void ProcessMouseDown(Point pos)
		{
			AbstractInteractiveLayer draggingLayer = null;

			//	Essaie de démarrer le drag d'une surcouche.
			if (this.AllowsMovement)
			{
				for (int i=this.interactiveLayers.Count-1; i>=0; i--)
				{
					var layer = this.interactiveLayers[i];

					layer.MouseDown (pos);

					if (layer.IsDragging)
					{
						draggingLayer = layer;
						break;
					}
				}

				//	Si une surcouche a démarré un drag, on clear toutes les autres.
				if (draggingLayer != null)
				{
					foreach (var layer in this.interactiveLayers.Where (x => x != draggingLayer))
					{
						layer.ClearActiveHover ();
					}
				}
			}
		}

		private void ProcessMouseMove(Point pos)
		{
			if (this.AllowsMovement)
			{
				var draggingLayer = this.DraggingLayer;

				if (draggingLayer == null)  // pas de drag en cours ?
				{
					//	Fait bouger le hover d'une surcouche.
					for (int i=this.interactiveLayers.Count-1; i>=0; i--)
					{
						var layer = this.interactiveLayers[i];

						layer.MouseMove (pos);

						if (layer.HasActiveHover)
						{
							//	Dès qu'une surcouche a réagi, on clear celles qui sont en dessous.
							for (int j=i-1; j>=0; j--)
							{
								this.interactiveLayers[j].ClearActiveHover ();
							}

							break;
						}
					}
				}
				else  // drag en cours ?
				{
					//	Fait réagir uniquement la surcouche en cours de drag.
					draggingLayer.MouseMove (pos);
				}

				this.UpdateMouseCursor ();
			}

			this.SetHilitedHoverRow (this.DetectRow (pos));
		}

		private void ProcessMouseUp(Point pos)
		{
			if (this.AllowsMovement)
			{
				for (int i=this.interactiveLayers.Count-1; i>=0; i--)
				{
					var layer = this.interactiveLayers[i];
					layer.MouseUp (pos);
				}
			}
		}

		private void ProcessMouseClick(Point pos)
		{
			int row = this.DetectRow (pos);
			if (row != -1)
			{
				this.OnRowClicked (row, this.DetectColumn (pos));
			}
		}

		private void ProcessMouseLeave(Point pos)
		{
			if (this.AllowsMovement)
			{
				for (int i=this.interactiveLayers.Count-1; i>=0; i--)
				{
					var layer = this.interactiveLayers[i];
					layer.MouseLeave (pos);
				}
			}

			this.SetHilitedHoverRow (-1);

			MouseCursorManager.Clear ();
		}


		private int DetectRow(Point pos)
		{
			int max = this.VisibleRowsCount;
			double h = this.ActualHeight - this.headerHeight - this.footerHeight - AbstractScroller.DefaultBreadth;
			double dy = h / max;

			double y = this.ActualHeight - this.headerHeight - pos.Y;
			if (y >= 0)
			{
				int row = (int) (y / dy);

				if (row >= 0 && row < max)
				{
					return row;
				}
			}

			return -1;
		}

		private int DetectColumn(Point pos)
		{
			var layer = this.interactiveLayers.Where (x => x is InteractiveLayerColumnOrder).FirstOrDefault () as InteractiveLayerColumnOrder;
			int rank = layer.DetectColumn (pos);

			if (rank == -1)
			{
				return -1;
			}
			else
			{
				return this.columnsState.MappedToAbsolute (rank);
			}
		}


		private void UpdateMouseCursor()
		{
			var cursor = MouseCursorType.Default;

			for (int i=this.interactiveLayers.Count-1; i>=0; i--)
			{
				var layer = this.interactiveLayers[i];
				var c = layer.MouseCursorType;

				if (c != MouseCursorType.Default)
				{
					cursor = c;
					break;
				}
			}

			MouseCursorManager.SetMouseCursor (cursor);
		}


		protected override void UpdateClientGeometry()
		{
			base.UpdateClientGeometry ();
			this.UpdateChildrensGeometry ();
		}

		protected override void PaintBackgroundImplementation(Graphics graphics, Rectangle clipRect)
		{
			graphics.AddFilledRectangle (new Rectangle (0, AbstractScroller.DefaultBreadth, this.ActualWidth, this.ActualHeight-AbstractScroller.DefaultBreadth));
			graphics.RenderSolid (ColorManager.TreeTableBackgroundColor);
		}


		public void SetHoverMode(TreeTableHoverMode mode)
		{
			this.hoverMode = mode;
			this.UpdateHoverMode ();
		}

		private void UpdateHoverMode()
		{
			foreach (var column in this.treeTableColumns)
			{
				column.HoverMode = this.hoverMode;
			}
		}

		private void SetHilitedHoverRow(int row)
		{
			foreach (var column in this.treeTableColumns)
			{
				column.HilitedHoverRow = row;
			}
		}

		private void UpdateChildrensGeometry()
		{
			foreach (var column in this.treeTableColumns)
			{
				column.HeaderHeight = this.headerHeight;
				column.FooterHeight = this.footerHeight;
				column.RowHeight    = this.rowHeight;
			}
		}

		private AbstractTreeTableColumn GetColumn(int absRank)
		{
			System.Diagnostics.Debug.Assert (absRank >= 0 && absRank < this.treeTableColumns.Count);
			int rank = this.columnsState.AbsoluteToMapped (absRank);
			return this.treeTableColumns[rank];
		}


		private AbstractInteractiveLayer DraggingLayer
		{
			get
			{
				return this.interactiveLayers.Where (x => x.IsDragging).FirstOrDefault ();
			}
		}


		#region Events handler
		private void OnRowClicked(int row, int column)
		{
			this.RowClicked.Raise (this, row, column);
		}

		public event EventHandler<int, int> RowClicked;


		private void OnRowDoubleClicked(int row)
		{
			this.RowDoubleClicked.Raise (this, row);
		}

		public event EventHandler<int> RowDoubleClicked;


		private void OnContentChanged(bool crop)
		{
			this.ContentChanged.Raise (this, crop);
		}

		public event EventHandler<bool> ContentChanged;


		private void OnSortingChanged()
		{
			this.SortingChanged.Raise (this);
		}

		public event EventHandler SortingChanged;


		private void OnTreeButtonClicked(int row, NodeType type)
		{
			this.TreeButtonClicked.Raise (this, row, type);
		}
		
		public event EventHandler<int, NodeType> TreeButtonClicked;
		#endregion


		private readonly List<AbstractTreeTableColumn>	treeTableColumns;
		private readonly FrameBox						leftContainer;
		private readonly Scrollable						columnsContainer;
		private readonly List<AbstractInteractiveLayer>	interactiveLayers;

		private TreeTableColumnDescription[]			columnDescriptions;
		private ColumnsState							columnsState;
		private int										headerHeight;
		private int										footerHeight;
		private int										rowHeight;
		private TreeTableHoverMode						hoverMode;
		private bool									allowsSorting;
	}
}