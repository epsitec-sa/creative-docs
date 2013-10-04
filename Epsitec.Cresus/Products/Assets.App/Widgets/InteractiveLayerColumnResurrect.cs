//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;

using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;

namespace Epsitec.Cresus.Assets.App.Widgets
{
	/// <summary>
	/// Surcouche interactive pour TreeTable s'occupant de la résurrection d'une
	/// colonne de largeur nulle.
	/// </summary>
	public class InteractiveLayerColumnResurrect : AbstractInteractiveLayer
	{
		public InteractiveLayerColumnResurrect(TreeTable treeTable)
			: base (treeTable)
		{
		}


		public override void BeginDrag(Point pos)
		{
			pos = this.foreground.MapParentToClient (pos);

		}

		public override void ProcessDrag(Point pos)
		{
			pos = this.foreground.MapParentToClient (pos);

			this.SetActiveHover (this.DetectButton (pos));
			this.SetShowButtons (this.DetectHeader (pos));
		}

		public override void EndDrag(Point pos)
		{
			pos = this.foreground.MapParentToClient (pos);

		}


		public override bool HasActiveHover
		{
			get
			{
				return this.detectedColumnRank != -1;
			}
		}

		public override void ClearActiveHover()
		{
			this.SetActiveHover (-1);
			this.SetShowButtons (false);
		}

		private void SetActiveHover(int rank)
		{
			if (rank != this.detectedColumnRank)
			{
				this.detectedColumnRank = rank;

				if (this.detectedColumnRank != -1)
				{
					this.UpdateForeground ();
				}
				else
				{
					this.ClearForeground ();
				}
			}
		}

		private void SetShowButtons(bool show)
		{
			if (this.showButtons != show)
			{
				this.showButtons = show;

				if (this.showButtons)
				{
					this.UpdateForeground ();
				}
				else
				{
					this.ClearForeground ();
				}
			}
		}


		private bool DetectHeader(Point pos)
		{
			return pos.Y >= this.foreground.ActualHeight - this.HeaderHeight;
		}

		private int DetectButton(Point pos)
		{
			var rects = this.ButtonRectangles.ToArray();

			for (int i=0; i<rects.Length; i++)
			{
				var rect = rects[i];

				if (rect.Contains (pos))
				{
					return i;
				}
			}

			return -1;
		}

	
		private void UpdateForeground()
		{
			this.foreground.ClearZones ();

			foreach (var rect in this.ButtonRectangles)
			{
				this.foreground.AddSurface (rect, ColorManager.MoveColumnColor);
				this.foreground.AddOutline (InteractiveLayerColumnResurrect.GetPlusPath (rect), ColorManager.TextColor, 2, CapStyle.Butt);
			}

			this.foreground.Invalidate ();
		}

		private static Path GetPlusPath(Rectangle rect)
		{
			var path = new Path ();

			rect.Deflate (5.0);

			path.MoveTo (rect.Left, rect.Center.Y);
			path.LineTo (rect.Right, rect.Center.Y);

			path.MoveTo (rect.Center.X, rect.Bottom);
			path.LineTo (rect.Center.X, rect.Top);

			return path;
		}

		private IEnumerable<Rectangle> ButtonRectangles
		{
			get
			{
				foreach (var rank in this.NullColumns)
				{
					var x = (double) this.GetSeparatorX (rank);

					yield return new Rectangle (
						x,
						this.foreground.ActualHeight-InteractiveLayerColumnResurrect.size,
						InteractiveLayerColumnResurrect.size,
						InteractiveLayerColumnResurrect.size);
				}
			}
		}

		private IEnumerable<int> NullColumns
		{
			get
			{
				for (int i=0; i<this.ColumnCount; i++)
				{
					var column = this.GetColumn (i);

					if (column.ActualWidth == 0)
					{
						yield return i;
					}
				}
			}
		}


		private static readonly int size = 20;

		private int								detectedColumnRank;
		private bool							showButtons;
	}
}