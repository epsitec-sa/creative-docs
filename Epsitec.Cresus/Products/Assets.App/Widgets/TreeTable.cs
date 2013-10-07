//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;

using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;

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

			this.columnsMapper = new List<int> ();
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
				return this.dockToLeftCount;
			}
		}

		public List<AbstractTreeTableColumn>	TreeTableColumns
		{
			get
			{
				return this.treeTableColumns;
			}
		}

		public void SetColumns(TreeTableColumnDescription[] descriptions, int dockToLeftCount)
		{
			//	Spécifie les colonnes à afficher, et réinitialise le mapping ainsi
			//	que les largeurs courantes.
			this.columnDescriptions = descriptions;
			this.dockToLeftCount = dockToLeftCount;

			this.columnWidths = new ColumnWidth[this.columnDescriptions.Length];
			for (int i=0; i<this.columnDescriptions.Length; i++)
			{
				this.columnWidths[i].SetWidth (this.columnDescriptions[i].Width);
			}

			this.columnsMapper.Clear ();

			for (int i=0; i<descriptions.Length; i++)
			{
				this.columnsMapper.Add (i);
			}

			this.CreateColumns ();
		}

		public void SetColumnWidth(int rank, int? width)
		{
			//	Modifie la largeur d'une colonne. Si la largeur n'est pas précisée (null),
			//	on restitue la largeur originale.
			rank = this.MapRelativeToAbsolute (rank);

			if (!width.HasValue)
			{
				width = this.columnWidths[rank].OriginalWidth;
			}

			this.columnWidths[rank].SetWidth (width.Value);

			this.GetColumn (rank).PreferredWidth = this.columnWidths[rank].FinalWidth;
		}

		public void ChangeColumnOrder(int columnSrc, TreeTableColumnSeparator separatorDst)
		{
			//	Change l'ordre des colonnes en déplaçant une colonne vers la gauche
			//	ou vers la droite. On modifie simplement le 'mapping', puis on
			//	reconstruit le tableau.
			bool srcDockToLeft = (columnSrc < this.dockToLeftCount);
			bool dstDockToLeft = (separatorDst.Rank < this.dockToLeftCount);

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

			if (separatorDst.Rank <= columnSrc)  // déplacement vers la gauche ?
			{
				int x = this.columnsMapper[columnSrc];
				this.columnsMapper.RemoveAt (columnSrc);
				this.columnsMapper.Insert (separatorDst.Rank, x);

				if (!srcDockToLeft && dstDockToLeft)
				{
					this.dockToLeftCount++;
				}
			}
			else  // déplacement vers la droite ?
			{
				int x = this.columnsMapper[columnSrc];
				this.columnsMapper.RemoveAt (columnSrc);
				this.columnsMapper.Insert (separatorDst.Rank-1, x);

				if (srcDockToLeft && !dstDockToLeft)
				{
					System.Diagnostics.Debug.Assert (this.dockToLeftCount > 0);
					this.dockToLeftCount--;
				}
			}

			this.CreateColumns ();
			this.OnContentChanged ();  // on demande de mettre à jour le contenu
		}

		private void CreateColumns()
		{
			//	Crée les widgets des différentes colonnes, selon le mapping.
			this.treeTableColumns.Clear ();
			this.leftContainer.Children.Clear ();
			this.columnsContainer.Viewport.Children.Clear ();

			int index = 0;

			for (int i=0; i<this.columnsMapper.Count; i++)
			{
				var ii = this.MapRelativeToAbsolute (i);
				var description = this.columnDescriptions[ii];

				var column = TreeTableColumnDescription.Create (description);
				column.PreferredWidth = this.columnWidths[ii].FinalWidth;
				column.Index = index++;

				this.treeTableColumns.Add (column);

				if (i < this.dockToLeftCount)  // dans le conteneur fixe de gauche ?
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

				//	Connecte les événements de la colonne.
				column.CellHovered += delegate (object sender, int row)
				{
					this.SetHilitedHoverRow (row);
				};

				column.CellClicked += delegate (object sender, int row)
				{
					if (!this.HasDraggingLayer)
					{
						this.OnRowClicked (column.Index, row);
					}
				};

				if (column is TreeTableColumnTree)
				{
					var tree = column as TreeTableColumnTree;

					tree.TreeButtonClicked += delegate (object sender, int row, TreeTableTreeType type)
					{
						this.OnTreeButtonClicked (row, type);
					};
				}
			}

			this.UpdateHoverMode ();
			this.UpdateChildrensGeometry ();
		}

		public void SetColumnCells(int rank, TreeTableCellTree[] cells)
		{
			var column = this.GetColumn (rank) as TreeTableColumnTree;
			System.Diagnostics.Debug.Assert (column != null);

			column.SetCells (cells);
		}

		public void SetColumnCells(int rank, TreeTableCellString[] cells)
		{
			var column = this.GetColumn (rank) as TreeTableColumnString;
			System.Diagnostics.Debug.Assert (column != null);

			column.SetCells (cells);
		}

		public void SetColumnCells(int rank, TreeTableCellDecimal[] cells)
		{
			var column = this.GetColumn (rank) as TreeTableColumnDecimal;
			System.Diagnostics.Debug.Assert (column != null);

			column.SetCells (cells);
		}


		protected override void OnExited(MessageEventArgs e)
		{
			foreach (var column in this.treeTableColumns)
			{
				column.ClearDetectedHoverRow ();
			}

			base.OnExited (e);
		}

		protected override void ProcessMessage(Message message, Point pos)
		{
			if (message.IsMouseType)
			{
				if (message.MessageType == MessageType.MouseDown)
				{
					this.ProcessMouseDown (pos);
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
			}

			base.ProcessMessage (message, pos);
		}

		private void ProcessMouseDown(Point pos)
		{
			AbstractInteractiveLayer draggingLayer = null;

			//	Essaie de démarrer le drag d'une surcouche.
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

		private void ProcessMouseMove(Point pos)
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
		}

		private void ProcessMouseUp(Point pos)
		{
			for (int i=this.interactiveLayers.Count-1; i>=0; i--)
			{
				var layer = this.interactiveLayers[i];
				layer.MouseUp (pos);
			}
		}


		protected override void UpdateClientGeometry()
		{
			base.UpdateClientGeometry ();
			this.UpdateChildrensGeometry ();
		}

		protected override void PaintBackgroundImplementation(Graphics graphics, Rectangle clipRect)
		{
			graphics.AddFilledRectangle (new Rectangle (Point.Zero, this.ActualSize));
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
			int rank = this.MapAbsoluteToRelative (absRank);
			return this.treeTableColumns[rank];
		}


		private bool HasDraggingLayer
		{
			get
			{
				return this.DraggingLayer != null;
			}
		}

		private AbstractInteractiveLayer DraggingLayer
		{
			get
			{
				return this.interactiveLayers.Where (x => x.IsDragging).FirstOrDefault ();
			}
		}


		private int MapAbsoluteToRelative(int absRank)
		{
			return this.columnsMapper.IndexOf (absRank);
		}

		private int MapRelativeToAbsolute(int relRank)
		{
			return this.columnsMapper[relRank];
		}


		private struct ColumnWidth
		{
			public void SetWidth(int width)
			{
				if (width == 0)
				{
					this.hide = true;
				}
				else
				{
					this.width = width;
					this.hide  = false;
				}
			}

			public int OriginalWidth
			{
				get
				{
					return this.width;
				}
			}

			public int FinalWidth
			{
				get
				{
					return this.hide ? 0 : this.width;
				}
			}

			private int  width;
			private bool hide;
		}


		#region Events handler
		private void OnRowClicked(int column, int row)
		{
			if (this.RowClicked != null)
			{
				this.RowClicked (this, column, row);
			}
		}

		public delegate void RowClickedEventHandler(object sender, int column, int row);
		public event RowClickedEventHandler RowClicked;


		private void OnContentChanged()
		{
			if (this.ContentChanged != null)
			{
				this.ContentChanged (this);
			}
		}

		public delegate void ContentChangedEventHandler(object sender);
		public event ContentChangedEventHandler ContentChanged;


		private void OnTreeButtonClicked(int row, TreeTableTreeType type)
		{
			if (this.TreeButtonClicked != null)
			{
				this.TreeButtonClicked (this, row, type);
			}
		}

		public delegate void TreeButtonClickedEventHandler(object sender, int row, TreeTableTreeType type);
		public event TreeButtonClickedEventHandler TreeButtonClicked;
		#endregion


		private readonly List<int>				columnsMapper;
		private readonly List<AbstractTreeTableColumn> treeTableColumns;
		private readonly FrameBox				leftContainer;
		private readonly Scrollable				columnsContainer;
		private readonly List<AbstractInteractiveLayer> interactiveLayers;

		private TreeTableColumnDescription[]	columnDescriptions;
		private ColumnWidth[]					columnWidths;
		private int								dockToLeftCount;
		private int								headerHeight;
		private int								footerHeight;
		private int								rowHeight;
		private TreeTableHoverMode				hoverMode;
	}
}