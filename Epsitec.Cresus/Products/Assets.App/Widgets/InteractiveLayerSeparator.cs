//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;

using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;

namespace Epsitec.Cresus.Assets.App.Widgets
{
	public class InteractiveLayerSeparator : AbstractInteractiveLayer
	{
		public InteractiveLayerSeparator(TreeTable treeTable)
			: base (treeTable)
		{
			this.lastColumnRank = -1;
		}


		public override bool IsDragging
		{
			get
			{
				return this.isDragging;
			}
		}

		public override void BeginDrag(Point pos)
		{
			pos = this.foreground.MapParentToClient (pos);

			if (this.lastColumnRank != -1)
			{
				this.isDragging = true;

				int rank = this.lastColumnRank;
				this.dragInitialMouse = pos.X;
				this.dragInitialLeft = this.GetSeparatorX (rank).Value - this.GetColumn (rank-1).ActualWidth;
				this.dragInitialWidth = this.GetColumn (rank-1).ActualWidth;
			}
		}

		public override void ProcessDrag(Point pos)
		{
			pos = this.foreground.MapParentToClient (pos);

			if (this.isDragging)
			{
				var delta = pos.X - this.dragInitialMouse;
				var width = System.Math.Max (this.dragInitialWidth + delta, 0.0);
				this.GetColumn (this.lastColumnRank-1).PreferredWidth = width;

				var x = this.dragInitialLeft + width;
				this.UpdateForeground (x);
			}
			else
			{
				int rank = this.DetectSeparator (pos);

				if (rank != this.lastColumnRank)
				{
					this.lastColumnRank = rank;

					if (this.lastColumnRank != -1)
					{
						var x = this.GetSeparatorX (this.lastColumnRank);
						this.UpdateForeground (x);
					}
					else
					{
						this.ClearForeground ();
					}
				}
			}
		}

		public override void EndDrag(Point pos)
		{
			pos = this.foreground.MapParentToClient (pos);

			if (this.isDragging)
			{
				this.isDragging = false;
				this.ClearForeground ();
			}
		}


		private Rectangle GetSeparatorRect(double x, int thickness)
		{
			x = System.Math.Floor (x);
			return new Rectangle (x-thickness, 0, thickness*2+1, this.foreground.ActualHeight);
		}

		private int DetectSeparator(Point pos)
		{
			if (pos.Y >= 0 && pos.Y < this.foreground.ActualHeight)
			{
				//	A l'envers, pour pouvoir déployer une colonne de largeur nulle.
				//	On saute la colonne 0 qui est tout à gauche.
				for (int i=this.ColumnCount; i>0; i--)
				{
					double? x = this.GetSeparatorX (i);

					if (x.HasValue &&
						pos.X >= x.Value - 4 &&
						pos.X <= x.Value + 4)
					{
						return i;
					}
				}
			}

			return -1;
		}

		private double? GetSeparatorX(int rank)
		{
			//	Retourne la position d'un frontière. S'il existe n colonnes, on peut
			//	obtenir les positions 0..n (0 = tout à gauche, n = tout à droite).
			if (rank != -1)
			{
				if (rank == 0)  // tout à gauche ?
				{
					var column = this.GetColumn (0);
					return column.ActualBounds.Left;
				}
				else  // cherche une frontière droite ?
				{
					rank--;  // 0..n-1
					var column = this.GetColumn (rank);

					if (column.DockToLeft)
					{
						return column.ActualBounds.Right;
					}
					else
					{
						double offset = this.ColumnsContainer.ViewportOffsetX;
						double position = column.ActualBounds.Right;

						if (position > offset)
						{
							var x = this.ColumnsContainer.ActualBounds.Left - offset + position;

							if (rank == this.ColumnCount-1)  // dernière colonne ?
							{
								x -= 2;  // pour ne pas être sous l'ascenseur vertical
							}

							return x;
						}
					}
				}
			}

			return null;
		}


		private void UpdateForeground(double? x)
		{
			this.foreground.ClearZones ();

			if (x.HasValue)
			{
				var rect = this.GetSeparatorRect (x.Value, 1);
				this.foreground.AddZone (rect, ColorManager.MoveColumnColor);
			}

			this.foreground.Invalidate ();
		}


		private int								lastColumnRank;
		private bool							isDragging;
		private double							dragInitialMouse;
		private double							dragInitialLeft;
		private double							dragInitialWidth;
	}
}