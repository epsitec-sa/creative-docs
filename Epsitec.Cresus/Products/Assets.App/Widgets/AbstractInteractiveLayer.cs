﻿//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;

using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.Data;

namespace Epsitec.Cresus.Assets.App.Widgets
{
	/// <summary>
	/// Surcouche interactive pour TreeTable s'occupant du déplacement d'un élément quelconque.
	/// </summary>
	public abstract class AbstractInteractiveLayer
	{
		public AbstractInteractiveLayer(TreeTable treeTable)
		{
			this.treeTable = treeTable;
		}


		public void CreateUI()
		{
			this.foreground = new Foreground
			{
				Parent  = this.treeTable,
				Anchor  = AnchorStyles.All,
				Margins = new Margins (0, 0, 0, AbstractScroller.DefaultBreadth),
			};
		}


		public bool								IsDragging
		{
			get
			{
				return this.isDragging;
			}
		}

		public virtual void MouseDown(Point pos)
		{
		}

		public virtual void MouseMove(Point pos)
		{
		}

		public virtual void MouseUp(Point pos)
		{
		}

		public void MouseLeave(Point pos)
		{
			this.ClearActiveHover ();
		}


		public MouseCursorType					MouseCursorType
		{
			get
			{
				return this.mouseCursorType;
			}
		}


		public virtual bool						HasActiveHover
		{
			get
			{
				return false;
			}
		}

		public virtual void ClearActiveHover()
		{
		}


		protected void ClearForeground()
		{
			this.foreground.ClearZones ();
			this.foreground.Invalidate ();
		}


		protected Scrollable					ColumnsContainer
		{
			get
			{
				return this.treeTable.ColumnsContainer;
			}
		}

		protected int							HeaderHeight
		{
			get
			{
				return this.treeTable.HeaderHeight;
			}
		}


		protected Rectangle GetSeparatorRect(double x, int thickness)
		{
			x = System.Math.Floor (x);
			return new Rectangle (x-thickness, 0, thickness*2+1, this.foreground.ActualHeight);
		}

		protected double? GetSeparatorX(int sepRank)
		{
			//	Retourne la position d'une frontière. S'il existe n colonnes, on peut
			//	obtenir les positions 0..n (0 = tout à gauche, n = tout à droite).
			//
			//	|     |     |     |     |
			//	|  0  |  1  |  2  |  3  |  <-- rangs des colonnes
			//	|     |     |     |     |
			//
			//	0     1     2     3     4  <-- rangs des frontières

			if (sepRank != -1)
			{
				if (sepRank == 0)  // tout à gauche ?
				{
					var column = this.GetColumn (0);
					return column.ActualBounds.Left;
				}
				else  // cherche une frontière droite ?
				{
					sepRank--;  // 0..n-1
					var column = this.GetColumn (sepRank);

					if (column.DockToLeft)
					{
						return column.ActualBounds.Right;
					}
					else
					{
						double offset = this.ColumnsContainer.ViewportOffsetX;
						double position = column.ActualBounds.Right;

						if (position >= offset)
						{
							var x = this.ColumnsContainer.ActualBounds.Left - offset + position;

							if (sepRank == this.ColumnCount-1)  // dernière colonne ?
							{
								x--;  // pour ne pas être sous l'ascenseur vertical
							}

							return x;
						}
					}
				}
			}

			return null;
		}


		protected void ChangeColumnOrder(int columnSrc, TreeTableColumnSeparator separatorDst)
		{
			this.treeTable.ChangeColumnOrder (columnSrc, separatorDst);
			this.ForceLayout ();
		}

		protected void ForceLayout()
		{
			//	Comme GetSeparatorX est basé sur la géométrie actuellle (ActualBounds) et 
			//	non préférée (PreferredWidth), il est nécessaire de forcer le mise à jour
			//	du layout après avoir modifié une largeur de colonne.
			this.foreground.Window.ForceLayout ();
		}

		protected void SetHoverMode(TreeTableHoverMode mode)
		{
			this.treeTable.SetHoverMode (mode);
		}

		protected int							DockToLeftCount
		{
			get
			{
				return this.treeTable.DockToLeftCount;
			}
		}

		protected void MoveColumnWidth(int rank, int width)
		{
			//	Déplacement interactif de la largeur d'une colonne. On se contente donc
			//	de modifier le widget, sans rien mémoriser.
			var column = this.GetColumn (rank);

			if (column != null)
			{
				column.PreferredWidth = width;
			}
		}

		protected void SetColumnWidth(int rank, int? width)
		{
			//	Modifie et mémorise la largeur d'une colonne. Si la largeur n'est pas
			//	précisée (null), on restitue la largeur originale.
			this.treeTable.SetColumnWidth (rank, width);
		}

		protected int							ColumnCount
		{
			get
			{
				return this.treeTable.TreeTableColumns.Count;
			}
		}

		protected AbstractTreeTableColumn GetColumn(int rank)
		{
			if (rank >= 0 && rank < this.treeTable.TreeTableColumns.Count)
			{
				return this.treeTable.TreeTableColumns[rank];
			}
			else
			{
				return null;
			}
		}

		protected void AddSortedColumn(ObjectField field)
		{
			this.treeTable.AddSortedColumn (field);
		}


		private readonly TreeTable				treeTable;

		protected Foreground					foreground;
		protected bool							isDragging;
		protected MouseCursorType				mouseCursorType;
	}
}