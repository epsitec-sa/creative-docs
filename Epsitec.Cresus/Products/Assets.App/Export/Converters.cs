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
			//	Constructeur statique, qui peuple une fois pour toutes le dictionnaire.
			Converters.dict = new Dictionary<string, string> ();

			Converters.dict.Add ("•",  "&lt;BULLET&gt;");	// édite <BULLET> (unicode 2022)
			Converters.dict.Add ("\t", "&lt;TAB&gt;");		// édite <TAB>
			Converters.dict.Add ("\r", "&lt;CR&gt;");		// édite <CR>
			Converters.dict.Add ("\n", "&lt;LF&gt;");		// édite <LF>
			Converters.dict.Add (" ",  "&lt;SPACE&gt;");	// édite <SPACE>
			Converters.dict.Add ("<",  "&lt;");				// édite <
			Converters.dict.Add (">",  "&gt;");				// édite >
		}


		public static string InternalToEditable(string text)
		{
			//	Retourne un texte éditable. S'il contient des caractères de contrôles (du genre tab "\t"),
			//	ils sont convertis (du genre <TAB>). Les majuscules évitent de les confondre avec les tags
			//	xml du genre <br/>.
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
			//	Retourne un texte interne utilisable par les moteurs d'exportation. Les caractères de
			//	contrôles peuvent être en majuscules ou en minuscules (par exemple <TAB> ou <tab>).
			if (!string.IsNullOrEmpty (text))
			{
				foreach (var pair in Converters.dict)
				{
					text = text.Replace (pair.Value, pair.Key);
					text = text.Replace (pair.Value.ToLower (), pair.Key);
				}
			}

			return text;
		}


		private static Dictionary<string, string> dict;
	}
}