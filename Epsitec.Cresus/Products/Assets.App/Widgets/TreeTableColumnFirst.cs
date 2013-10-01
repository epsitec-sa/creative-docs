//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using System.Text;

using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;

namespace Epsitec.Cresus.Assets.App.Widgets
{
	/// <summary>
	/// Première colonne de TreeTable affichant des chaînes avec une indication
	/// du niveau dans l'arborescence.
	/// </summary>
	public class TreeTableColumnFirst : AbstractTreeTableColumn
	{
		public void SetCellFirsts(TreeTableCellFirst[] cellFirsts)
		{
			this.cellFirsts = cellFirsts;
			this.Invalidate ();
		}


		protected override void PaintBackgroundImplementation(Graphics graphics, Rectangle clipRect)
		{
			base.PaintBackgroundImplementation(graphics, clipRect);

			int y = 0;

			foreach (var cellFirst in this.cellFirsts)
			{
				var rect = this.GetCellsRect (y);

				graphics.AddFilledRectangle (rect);
				graphics.RenderSolid (AbstractTreeTableColumn .GetFirstCellColor (y == this.hilitedHoverRow, cellFirst.IsSelected));

				rect.Deflate (this.DescriptionMargin * (cellFirst.Level+1), 0, 0, 0);

				if (cellFirst.Type == TreeTableFirstType.Compacted ||
					cellFirst.Type == TreeTableFirstType.Extended)
				{
					this.PaintGlyph (graphics, cellFirst.Type, rect);
				}

				rect.Deflate (this.DescriptionMargin * 2, 0, 0, 0);

				var font = Font.DefaultFont;

				graphics.Color = ColorManager.TextColor;
				graphics.PaintText (rect, cellFirst.Description, font, this.FontSize, ContentAlignment.MiddleLeft);

				y++;
			}
		}

		private void PaintGlyph(Graphics graphics, TreeTableFirstType type, Rectangle rect)
		{
			var r = new Rectangle (rect.Left, rect.Bottom, rect.Height, rect.Height);
			var path = new Path ();

			switch (type)
			{
				case TreeTableFirstType.Compacted:
					{
						r.Deflate (r.Height * 0.4, r.Height * 0.4, r.Height * 0.3, r.Height * 0.3);

						var p1 = new Point (r.Left, r.Bottom);
						var p2 = new Point (r.Right, r.Center.Y);
						var p3 = new Point (r.Left, r.Top);

						path.MoveTo (p1);
						path.LineTo (p2);
						path.LineTo (p3);
					}
					break;

				case TreeTableFirstType.Extended:
					{
						r.Deflate (r.Height * 0.3, r.Height * 0.3, r.Height * 0.4, r.Height * 0.4);

						var p1 = new Point (r.Left, r.Top);
						var p2 = new Point (r.Center.X, r.Bottom);
						var p3 = new Point (r.Right, r.Top);

						path.MoveTo (p1);
						path.LineTo (p2);
						path.LineTo (p3);
					}
					break;
			}

			if (!path.IsEmpty)
			{
				graphics.LineWidth = rect.Height * 0.1;
				graphics.AddPath (path);
				graphics.RenderSolid (ColorManager.TreeTableArrowColor);
				graphics.LineWidth = 1;
			}
		}


		private TreeTableCellFirst[] cellFirsts;
	}
}
