//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;

using Epsitec.Common.Drawing;

namespace Epsitec.Cresus.Assets.App.Widgets
{
	/// <summary>
	/// Surcouche interactive pour TreeTable s'occupant du déplacement d'un séparateur de colonne.
	/// Ceci permet de modifier les largeurs des colonnes.
	/// </summary>
	public class InteractiveLayerColumnSeparator : AbstractInteractiveLayer
	{
		public InteractiveLayerColumnSeparator(TreeTable treeTable)
			: base (treeTable)
		{
			this.detectedSeparatorRank = -1;
		}


		public override void MouseDown(Point pos)
		{
			pos = this.foreground.MapParentToClient (pos);

			if (this.detectedSeparatorRank != -1)
			{
				this.isDragging = true;

				int rank = this.detectedSeparatorRank;
				this.dragInitialMouse = pos.X;
				this.dragInitialLeft = this.GetSeparatorX (rank).Value - this.GetColumn (rank-1).ActualWidth;
				this.dragInitialWidth = this.GetColumn (rank-1).ActualWidth;

				this.SetHoverMode (TreeTableHoverMode.OnlyVerticalSeparators);
			}
		}

		public override void MouseMove(Point pos)
		{
			pos = this.foreground.MapParentToClient (pos);

			if (this.isDragging)
			{
				var width = this.GetWidth (pos);
				this.MoveColumnWidth (this.detectedSeparatorRank-1, width);

				var x = this.dragInitialLeft + width;
				this.UpdateForeground (x);
			}
			else
			{
				int rank = this.DetectSeparator (pos);
				this.SetActiveHover (rank);
			}
		}

		public override void MouseUp(Point pos)
		{
			if (this.isDragging)
			{
				this.isDragging = false;

				var width = this.GetWidth (pos);
				this.SetColumnWidth (this.detectedSeparatorRank-1, width);
				
				this.ClearForeground ();
				this.SetHoverMode (TreeTableHoverMode.VerticalGradient);
			}
		}

		private int GetWidth(Point pos)
		{
			var delta = pos.X - this.dragInitialMouse;
			return (int) System.Math.Max (this.dragInitialWidth + delta, 0.0);
		}


		public override bool HasActiveHover
		{
			get
			{
				return this.detectedSeparatorRank != -1;
			}
		}

		public override void ClearActiveHover()
		{
			this.SetActiveHover (-1);
		}

		private void SetActiveHover(int rank)
		{
			if (rank != this.detectedSeparatorRank)
			{
				this.detectedSeparatorRank = rank;

				if (this.detectedSeparatorRank != -1)
				{
					var x = this.GetSeparatorX (this.detectedSeparatorRank);
					this.UpdateForeground (x);
				}
				else
				{
					this.ClearForeground ();
				}

				this.mouseCursorType = (this.detectedSeparatorRank == -1)
					? MouseCursorType.Default
					: MouseCursorType.HorizontalMoveSeparator;
			}
		}


		private int DetectSeparator(Point pos)
		{
			if (pos.Y >= 0 && pos.Y < this.foreground.ActualHeight)
			{
				//	On saute la colonne 0 qui est tout à gauche.
				for (int i=1; i<=this.ColumnCount; i++)
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


		private void UpdateForeground(double? x)
		{
			this.foreground.ClearZones ();

			if (x.HasValue)
			{
				var rect = this.GetSeparatorRect (x.Value-1, 0);
				this.foreground.AddSurface (rect, ColorManager.MoveColumnColor);
			}

			this.foreground.Invalidate ();
		}


		private int								detectedSeparatorRank;
		private double							dragInitialMouse;
		private double							dragInitialLeft;
		private double							dragInitialWidth;
	}
}