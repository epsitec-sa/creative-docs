//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;

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
	}
}
