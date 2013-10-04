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

			//	Crée les surcouches interactives.
			this.interactiveLayerColumnSeparator = new InteractiveLayerColumnSeparator (this);
			this.interactiveLayerColumnSeparator.CreateUI ();

			this.interactiveLayerColumnOrder = new InteractiveLayerColumnOrder (this);
			this.interactiveLayerColumnOrder.CreateUI ();
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

		public List<AbstractTreeTableColumn> TreeTableColumns
		{
			get
			{
				return this.treeTableColumns;
			}
		}

		public void SetColumns(TreeTableColumnDescription[] descriptions)
		{
			this.treeTableColumns.Clear ();
			this.leftContainer.Children.Clear ();
			this.columnsContainer.Viewport.Children.Clear ();

			int index = 0;

			foreach (var description in descriptions)
			{
				var column = TreeTableColumnDescription.Create (description);
				column.Index = index++;

				this.treeTableColumns.Add (column);

				if (description.DockToLeft)
				{
					column.Dock = DockStyle.Left;
					this.leftContainer.Children.Add (column);
				}
				else
				{
					column.Dock = DockStyle.Left;
					this.columnsContainer.Viewport.Children.Add (column);
				}

				column.CellHovered += delegate (object sender, int row)
				{
					if (!this.interactiveLayerColumnOrder.IsDragging)
					{
						this.SetHilitedHoverRow (row);
					}
				};

				column.CellClicked += delegate (object sender, int row)
				{
					if (!this.interactiveLayerColumnOrder.IsDragging)
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
					this.interactiveLayerColumnSeparator.BeginDrag (pos);
					this.interactiveLayerColumnOrder.BeginDrag (pos);
				}
				else if (message.MessageType == MessageType.MouseMove)
				{
					if (this.interactiveLayerColumnSeparator.IsDragging)
					{
						this.interactiveLayerColumnSeparator.ProcessDrag (pos);
					}
					else if (this.interactiveLayerColumnOrder.IsDragging)
					{
						this.interactiveLayerColumnOrder.ProcessDrag (pos);
					}
					else
					{
						this.interactiveLayerColumnSeparator.ProcessDrag (pos);
						this.interactiveLayerColumnOrder.ProcessDrag (pos);
					}
				}
				else if (message.MessageType == MessageType.MouseUp)
				{
					this.interactiveLayerColumnSeparator.EndDrag (pos);
					this.interactiveLayerColumnOrder.EndDrag (pos);
				}
			}

			base.ProcessMessage (message, pos);
		}

		protected override void OnMouseMove(MessageEventArgs e)
		{

			base.OnMouseMove (e);
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

		private AbstractTreeTableColumn GetColumn(int rank)
		{
			System.Diagnostics.Debug.Assert (rank >= 0 && rank < this.treeTableColumns.Count);
			return this.treeTableColumns[rank];
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


		private readonly List<AbstractTreeTableColumn> treeTableColumns;
		private readonly FrameBox				leftContainer;
		private readonly Scrollable				columnsContainer;
		private readonly InteractiveLayerColumnSeparator interactiveLayerColumnSeparator;
		private readonly InteractiveLayerColumnOrder interactiveLayerColumnOrder;

		private int								headerHeight;
		private int								footerHeight;
		private int								rowHeight;
	}
}