//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Assets.App.Export
{
	public static class Converters
	{
		static Converters()
		{
			Converters.dict = new Dictionary<string, string> ();

			Converters.dict.Add ("\t", "&lt;tab&gt;");
			Converters.dict.Add ("\r", "&lt;cr&gt;");
			Converters.dict.Add ("\n", "&lt;lf&gt;");
			Converters.dict.Add (" ", "&lt;space&gt;");
			Converters.dict.Add ("<", "&lt;");
			Converters.dict.Add (">", "&gt;");
		}


		public static string InternalToEditable(string text)
		{
			if (!string.IsNullOrEmpty (text))
			{
				foreach (var pair in Converters.dict)
				{
					text = text.Replace (pair.Key, pair.Value);
				}
			}

			return text;
		}

		public static string EditableToInternal(string text)
		{
			if (!string.IsNullOrEmpty (text))
			{
				foreach (var pair in Converters.dict)
				{
					text = text.Replace (pair.Value, pair.Key);
				}
			}

			return text;
		}


		private static Dictionary<string, string> dict;
	}
}