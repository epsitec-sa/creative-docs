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

			this.CreateGlyphButtons ();
			this.Invalidate ();
		}


		protected override void PaintBackgroundImplementation(Graphics graphics, Rectangle clipRect)
		{
			base.PaintBackgroundImplementation(graphics, clipRect);

			int y = 0;

			foreach (var cellFirst in this.cellFirsts)
			{
				//	Dessine le fond.
				var rect = this.GetCellsRect (y);

				graphics.AddFilledRectangle (rect);
				graphics.RenderSolid (AbstractTreeTableColumn.GetFirstCellColor (y == this.hilitedHoverRow, cellFirst.IsSelected));

				//	Dessine le texte.
				rect = this.GetTextRectangle (y, cellFirst.Level);
				var font = Font.DefaultFont;

				graphics.Color = ColorManager.TextColor;
				graphics.PaintText (rect, cellFirst.Description, font, this.FontSize, ContentAlignment.MiddleLeft);

				y++;
			}
		}

		protected override void OnSizeChanged(Size oldValue, Size newValue)
		{
			base.OnSizeChanged (oldValue, newValue);

			this.CreateGlyphButtons ();
		}

		private void CreateGlyphButtons()
		{
			//	Crée les petits boutons pour la gestion de l'arborescence.
			this.Children.Clear ();

			int y = 0;

			foreach (var cellFirst in this.cellFirsts)
			{
				if (cellFirst.Type == TreeTableFirstType.Compacted ||
					cellFirst.Type == TreeTableFirstType.Extended)
				{
					var rect = this.GetGlyphRectangle (y, cellFirst.Level);

					var button = new GlyphButton
					{
						GlyphShape    = cellFirst.Type == TreeTableFirstType.Compacted ? GlyphShape.ArrowRight : GlyphShape.ArrowDown,
						ButtonStyle   = ButtonStyle.ToolItem,
						PreferredSize = rect.Size,
						Anchor        = AnchorStyles.BottomLeft,
						Margins       = new Margins (rect.Left, 0, 0, rect.Bottom),
					};

					this.Children.Add (button);
				}

				y++;
			}
		}

		private Rectangle GetGlyphRectangle(int y, int level)
		{
			//	Retourne le rectangle pour le petit GlyphButton.
			var rect = this.GetCellsRect (y);
			rect.Deflate (this.DescriptionMargin*level, 0, 0, 0);

			return new Rectangle (rect.Left, rect.Bottom, rect.Height, rect.Height);
		}

		private Rectangle GetTextRectangle(int y, int level)
		{
			//	Retourne le rectangle pour le texte, en laissant la place pour le GlyphButton à gauche.
			var rect = this.GetCellsRect (y);
			rect.Deflate (this.DescriptionMargin*level + this.DescriptionMargin*5/2, 0, 0, 0);

			return rect;
		}


		private TreeTableCellFirst[] cellFirsts;
	}
}
