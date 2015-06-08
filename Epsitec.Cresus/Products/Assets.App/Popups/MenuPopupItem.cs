//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.App.Helpers;
using Epsitec.Cresus.Assets.App.Widgets;

namespace Epsitec.Cresus.Assets.App.Popups
{
	/// <summary>
	/// Case d'un menu, composée d'une icône à gauche et d'un texte à droite.
	/// </summary>
	public class MenuPopupItem : ColoredButton
	{
		public MenuPopupItem()
		{
			this.iconLayout = new TextLayout ();
			this.textLayout = new TextLayout ();

			this.NormalColor = Color.Empty;
			this.HoverColor  = ColorManager.HoverColor;
		}


		protected override void PaintBackgroundImplementation(Graphics graphics, Rectangle clipRect)
		{
			var rect = new Rectangle (0, 0, this.ActualWidth, this.ActualHeight);

			//	Dessine le fond coloré.
			var color = this.BackColor;

			if (!this.Enable)
			{
				color = color.Delta (0.1);  // plus clair si disable
			}

			if (this.CommandState.ActiveState == ActiveState.Yes)
			{
				color = ColorManager.SelectionColor;
			}

			graphics.AddFilledRectangle (rect);
			graphics.RenderSolid (color);

			//	Dessine l'icône.
			{
				var r = new Rectangle (rect.Left, rect.Bottom, rect.Height, rect.Height);

				this.iconLayout.Text            = Misc.GetRichTextImg (this.IconUri, verticalOffset: 0);
				this.iconLayout.DefaultFont     = Font.DefaultFont;
				this.iconLayout.DefaultFontSize = Font.DefaultFontSize;
				this.iconLayout.LayoutSize      = r.Size;
				this.iconLayout.Alignment       = ContentAlignment.MiddleCenter;

				double brightness = this.Enable ? 0.0 : 0.7;  // gris clair si disable
				this.iconLayout.Paint (r.BottomLeft, graphics, r, Color.FromBrightness (brightness), GlyphPaintStyle.Normal);
			}

			//	Dessine le texte.
			{
				var r = new Rectangle (rect.Left+rect.Height+MenuPopupItem.gap, rect.Bottom, rect.Width-rect.Height-MenuPopupItem.gap, rect.Height);

				this.textLayout.Text            = this.Text;
				this.textLayout.DefaultFont     = Font.DefaultFont;
				this.textLayout.DefaultFontSize = Font.DefaultFontSize;
				this.textLayout.LayoutSize      = r.Size;
				this.textLayout.BreakMode       = TextBreakMode.Ellipsis | TextBreakMode.Split | TextBreakMode.SingleLine;
				this.textLayout.Alignment       = ContentAlignment.MiddleLeft;

				double brightness = this.Enable ? 0.0 : 0.7;  // gris clair si disable
				this.textLayout.Paint (r.BottomLeft, graphics, r, Color.FromBrightness (brightness), GlyphPaintStyle.Normal);
			}
		}


		public static int GetRequiredWidth(int height, string text)
		{
			//	Retourne la largeur requise pour une case d'un menu.
			return height					// icône carrée
				 + MenuPopupItem.gap		// espace entre l'icône et le texte
				 + text.GetTextWidth ()		// largeur du texte
				 + 10;						// pour ne pas coller le texte sur le bord droite
		}


		private const int gap = 10;

		private readonly TextLayout				iconLayout;
		private readonly TextLayout				textLayout;
	}
}