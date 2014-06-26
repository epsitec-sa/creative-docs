//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;

namespace Epsitec.Cresus.Assets.App.Helpers
{
	public static class Text
	{
		public static int GetTextHeight(this string text, int width)
		{
			//	Retourne la hauteur requise pour un texte multilignes, connaissant la largeur
			//	à disposition.
			return Text.GetTextHeight (text, width, Font.DefaultFont, Font.DefaultFontSize);
		}

		public static int GetTextHeight(this string text, int width, Font font, double fontSize)
		{
			if (string.IsNullOrEmpty (text))
			{
				return 0;
			}
			else
			{
				var textLayout = new TextLayout
				{
					Text            = text,
					Alignment       = ContentAlignment.TopLeft,
					DefaultFont     = font,
					DefaultFontSize = fontSize,
					LayoutSize      = new Size (width, 10000),
				};

				double height = 0;

				int lineCount = textLayout.TotalLineCount;
				for (int i = 0; i < lineCount; i++)
				{
					height += textLayout.GetLineHeight (i);
				}

				return (int) height + 1;
			}
		}


		public static int GetTextWidth(this string text)
		{
			//	Retourne la largeur requise pour une ligne de texte.
			return Text.GetTextWidth (text, Font.DefaultFont, Font.DefaultFontSize);
		}

		public static int GetTextWidth(this string text, Font font, double fontSize)
		{
			if (string.IsNullOrEmpty (text))
			{
				return 0;
			}
			else
			{
				var width = new TextGeometry (0, 0, 1000, 100, text, font, fontSize, ContentAlignment.MiddleLeft).Width;
				return (int) width + 1;
			}
		}
	}
}
