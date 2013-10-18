//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Drawing;

namespace Epsitec.Cresus.Assets.App.Helpers
{
	public static class Text
	{
		public static int GetTextWidth(string text)
		{
			return Text.GetTextWidth (text, Font.DefaultFont, Font.DefaultFontSize);
		}

		public static int GetTextWidth(string text, Font font, double fontSize)
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
