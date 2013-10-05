//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;

using Epsitec.Common.Drawing;

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


		public override void MouseDown(Point pos)
		{
			if (this.detectedColumnRank != -1)
			{
				this.isDragging = true;
			}
		}

		public override void MouseMove(Point pos)
		{
			pos = this.foreground.MapParentToClient (pos);

			var show = this.DetectHeader (pos);
			var rank = this.DetectButton (pos);
			this.SetActiveHover (show, rank);
		}

		public override void MouseUp(Point pos)
		{
			pos = this.foreground.MapParentToClient (pos);

			if (this.detectedColumnRank != -1)
			{
				this.ResurrectColumn(this.detectedColumnRank, 75);
				this.ClearActiveHover ();
			}

			this.isDragging = false;
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
			this.SetActiveHover (false, -1);
		}

		private void SetActiveHover(bool show, int rank)
		{
			if (this.showButtons != show || this.detectedColumnRank != rank)
			{
				this.showButtons = show;
				this.detectedColumnRank = rank;

				if (this.showButtons)
				{
					this.UpdateForeground (this.detectedColumnRank);
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
			//	On cherche à l'envers (de droite à gauche), pour ressuciter d'abord
			//	la dernière colonne compactée.
			var rects = this.ButtonRectangles.ToArray ();

			for (int i=rects.Length-1; i>=0; i--)
			{
				var rect = rects[i];

				if (rect.IsValid && rect.Contains (pos))
				{
					return i;
				}
			}

			return -1;
		}

	
		private void UpdateForeground(int hoverRank)
		{
			this.foreground.ClearZones ();

			foreach (var rect in this.ButtonRectangles)
			{
				bool hover = (hoverRank-- == 0);

				if (rect.IsValid)
				{
					if (hover)
					{
						this.foreground.AddSurface (rect, ColorManager.MoveColumnColor);
						this.foreground.AddOutline (InteractiveLayerColumnResurrect.GetPlusPath (rect), ColorManager.TextColor, 2, CapStyle.Butt);

						var line = new DashedPath ();
						line.AddDash (4, 4);
						line.MoveTo (rect.Center.X+0.5, this.foreground.ActualHeight-this.HeaderHeight);
						line.LineTo (rect.Center.X+0.5, 0);
						var dash = line.GenerateDashedPath ();

						this.foreground.AddOutline (dash, ColorManager.TextColor);
					}
					else
					{
						var r = rect;
						r.Deflate (0.5);

						var line = new DashedPath ();
						line.AddDash (1, 3);
						line.AppendRectangle (r);
						var dash = line.GenerateDashedPath ();

						this.foreground.AddOutline (dash, ColorManager.TextColor, 1, CapStyle.Butt);
					}
				}
			}

			this.foreground.Invalidate ();
		}

		private static Path GetPlusPath(Rectangle rect)
		{
			var path = new Path ();

			rect.Deflate (System.Math.Floor (rect.Height*0.3));

			path.MoveTo (rect.Left, rect.Center.Y);
			path.LineTo (rect.Right, rect.Center.Y);

			path.MoveTo (rect.Center.X, rect.Bottom);
			path.LineTo (rect.Center.X, rect.Top);

			return path;
		}


		private void ResurrectColumn(int rank, int width)
		{
			this.GetColumn (rank).PreferredWidth = width;

			//	Comme GetSeparatorX est basé sur la géométrie actuellle (ActualBounds) et 
			//	non préférée (PreferredWidth), il est nécessaire de forcer le mise à jour
			//	du layout.
			this.foreground.Window.ForceLayout ();
		}

		private IEnumerable<Rectangle> ButtonRectangles
		{
			//	Retourne un rectangle par colonne. Les colonnes non nulles retournent
			//	un rectangle vide.
			get
			{
				for (int i=0; i<this.ColumnCount; i++)
				{
					var column = this.GetColumn (i);
					var x = this.GetSeparatorX (i);

					if (column.PreferredWidth == 0 && x.HasValue)
					{
						var h = this.HeaderHeight;
						var y = this.foreground.ActualHeight-h;
						yield return new Rectangle (x.Value-h/2, y, h, h);
					}
					else
					{
						yield return Rectangle.Empty;
					}
				}
			}
		}


		private bool							showButtons;
		private int								detectedColumnRank;
	}
}