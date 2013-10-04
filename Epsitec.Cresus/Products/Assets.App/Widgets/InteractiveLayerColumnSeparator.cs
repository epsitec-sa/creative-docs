//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;

using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;

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
			this.lastColumnRank = -1;
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
		private double							dragInitialMouse;
		private double							dragInitialLeft;
		private double							dragInitialWidth;
	}
}