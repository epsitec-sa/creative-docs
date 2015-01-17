//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Assets.Export.Helpers
{
	public static class TagConverters
	{
		static TagConverters()
		{
			//	Constructeur statique, qui peuple une fois pour toutes le dictionnaire.
			TagConverters.dict = new Dictionary<string, string> ();

			TagConverters.dict.Add (TagConverters.Compile ("<BULLET>"), "•" );	// unicode 2022
			TagConverters.dict.Add (TagConverters.Compile ("<TIRET>"),  "—" );	// unicode 2014
			TagConverters.dict.Add (TagConverters.Compile ("<TAB>"),    "\t");
			TagConverters.dict.Add (TagConverters.Compile ("<CR>"),     "\r");
			TagConverters.dict.Add (TagConverters.Compile ("<LF>"),     "\n");
			TagConverters.dict.Add (TagConverters.Compile ("<SPACE>"),  " " );
		}


		public static void AddDefaultTags(Dictionary<string, string> tags)
		{
			foreach (var pair in TagConverters.dict)
			{
				tags.Add (pair.Key, pair.Value);
			}
		}


		public static string Compile(string text)
		{
			//	"Compile" un texte écrit dans le source C#. Cette méthode permet, pour
			//	plus de clarté, d'écrire "<TAB>" au lieu de "&lt;TAB&gt;". Cette méthode
			//	ne doit jamais être utilisée à partir de textes entrés par l'utilisateur !
			if (!string.IsNullOrEmpty (text))
			{
				text = text.Replace ("<", "&lt;");
				text = text.Replace (">", "&gt;");
			}

			return text;
		}


		public static string GetFinalText(string text)
		{
			//	Retourne un texte interne utilisable par les moteurs d'exportation. Les caractères de
			//	contrôles peuvent être en majuscules ou en minuscules (par exemple <TAB> ou <tab>).
			return TagConverters.GetFinalText (TagConverters.dict, text);
		}

		public static string GetFinalText(Dictionary<string, string> tags, string text)
		{
			//	Retourne un texte final dans lequel les tags ont été remplacés par leurs
			//	valeurs équivalentes. Les tags peuvent être en majuscules ou en minuscules
			//	(par exemple <TAB> ou <tab>).
			if (!string.IsNullOrEmpty (text))
			{
				foreach (var pair in tags)
				{
					text = text.Replace (pair.Key, pair.Value);
					text = text.Replace (pair.Key.ToLower (), pair.Value);
				}
			}

			return text;
		}


		public static string Eol = TagConverters.Compile ("<CR><LF>");

		private static Dictionary<string, string> dict;
	}
}