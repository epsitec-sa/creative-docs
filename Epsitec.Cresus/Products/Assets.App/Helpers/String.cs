//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Drawing;

namespace Epsitec.Cresus.Assets.App.Helpers
{
	public static class String
	{
		public static string Bold(this string text)
		{
			return string.Concat ("<b>", text, "</b>");
		}

		public static string Italic(this string text)
		{
			return string.Concat ("<i>", text, "</i>");
		}

		public static string Underline(this string text)
		{
			return string.Concat ("<u>", text, "</u>");
		}

		public static string Color(this string text, Color color)
		{
			var h = Epsitec.Common.Drawing.Color.ToHexa (color);
			var tag = string.Format ("<font color=\"#{0}\">", h);
			return string.Concat (tag, text, "</font>");
		}
	}
}
