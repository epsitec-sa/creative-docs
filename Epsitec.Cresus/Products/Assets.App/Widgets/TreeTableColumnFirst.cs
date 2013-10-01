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

			this.CreateTinyButtons ();
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
				graphics.RenderSolid (AbstractTreeTableColumn.GetFirstCellColor (y == this.hilitedHoverRow, cellFirst.IsSelected));

				int leftMargin = this.DescriptionMargin*cellFirst.Level + this.DescriptionMargin*5/2;
				rect.Deflate (leftMargin, 0, 0, 0);

				var font = Font.DefaultFont;

				graphics.Color = ColorManager.TextColor;
				graphics.PaintText (rect, cellFirst.Description, font, this.FontSize, ContentAlignment.MiddleLeft);

				y++;
			}
		}

		protected override void OnSizeChanged(Size oldValue, Size newValue)
		{
			base.OnSizeChanged (oldValue, newValue);

			this.CreateTinyButtons ();
		}

		private void CreateTinyButtons()
		{
			this.Children.Clear ();

			int y = 0;

			foreach (var cellFirst in this.cellFirsts)
			{
				if (cellFirst.Type == TreeTableFirstType.Compacted ||
					cellFirst.Type == TreeTableFirstType.Extended)
				{
					var rect = this.GetCellsRect (y);
					rect.Deflate (this.DescriptionMargin * cellFirst.Level, 0, 0, 0);
					var r = new Rectangle (rect.Left, rect.Bottom, rect.Height, rect.Height);

					var button = new GlyphButton
					{
						GlyphShape    = cellFirst.Type == TreeTableFirstType.Compacted ? GlyphShape.ArrowRight : GlyphShape.ArrowDown,
						ButtonStyle   = ButtonStyle.ToolItem,
						PreferredSize = r.Size,
						Anchor        = AnchorStyles.BottomLeft,
						Margins       = new Margins (r.Left, 0, 0, r.Bottom),
					};

					this.Children.Add (button);
				}

				y++;
			}
		}


		private TreeTableCellFirst[] cellFirsts;
	}
}
