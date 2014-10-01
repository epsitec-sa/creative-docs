//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Assets.Server.BusinessLogic
{
	public static class ApproximativeSearching
	{
		public static int GetRanking(string existing, string searching)
		{
			//	Effectue une recherche approximative "est-ce que existing ressemble à searching".
			//	Retourne un ranking (0 = aucune correspondance, max = identique).
			//	C'est de la bricole, qu'il faudra peut-être améliorer un jour !
			if (existing == searching)
			{
				return (int) Ranking.StrictlyEqual;
			}

			existing  = ApproximativeSearching.RemoveDiatritic (existing .ToLower ());
			searching = ApproximativeSearching.RemoveDiatritic (searching.ToLower ());

			if (existing == searching)
			{
				return (int) Ranking.DifferentCase;
			}

			if (existing.Contains (searching))
			{
				int delta = existing.Length - searching.Length;
				return (int) Ranking.DifferentCase-delta;
			}

			if (searching.Contains (existing))
			{
				int delta = searching.Length - existing.Length;
				return (int) Ranking.DifferentCase-delta;
			}

			existing  = ApproximativeSearching.Phonetic (existing);
			searching = ApproximativeSearching.Phonetic (searching);

			if (existing == searching)
			{
				return (int) Ranking.Phonetic;
			}

			if (existing.Contains (searching))
			{
				int delta = existing.Length - searching.Length;
				return (int) Ranking.Phonetic-delta;
			}

			if (searching.Contains (existing))
			{
				int delta = searching.Length - existing.Length;
				return (int) Ranking.Phonetic-delta;
			}

			return (int) Ranking.Discordant;
		}


		public static string Phonetic(string s)
		{
			//	C'est de la bricole, qu'il faudra peut-être améliorer un jour !
			s = s.Replace ("cc", "c");
			s = s.Replace ("dd", "d");
			s = s.Replace ("ff", "f");
			s = s.Replace ("gg", "g");
			s = s.Replace ("ll", "l");
			s = s.Replace ("mm", "m");
			s = s.Replace ("nn", "n");
			s = s.Replace ("pp", "p");
			s = s.Replace ("rr", "r");
			s = s.Replace ("ss", "s");
			s = s.Replace ("tt", "t");

			s = s.Replace ("ß", "s");
			s = s.Replace ("gu", "g");
			s = s.Replace ("ph", "f");
			s = s.Replace ("eau", "o");
			s = s.Replace ("au", "o");
			s = s.Replace ("qu", "c");
			s = s.Replace ("k", "c");
			s = s.Replace ("m", "n");
			s = s.Replace ("w", "v");
			s = s.Replace ("y", "i");
			s = s.Replace ("an", "en");
			s = s.Replace ("ain", "in");
			s = s.Replace ("ai", "e");

			s = s.Replace ("h", "");
			s = s.Replace (" ", "");
			s = s.Replace ("-", "");
			s = s.Replace (".", "");
			s = s.Replace (",", "");
			s = s.Replace (":", "");
			s = s.Replace (";", "");
			s = s.Replace (",", "");
			s = s.Replace ("(", "");
			s = s.Replace (")", "");
			s = s.Replace ("/", "");

			return s;
		}

		public static string RemoveDiatritic(string s)
		{
			var builder = new System.Text.StringBuilder ();

			foreach (var c in s)
			{
				builder.Append (ApproximativeSearching.RemoveDiacritic(c));
			}

			return builder.ToString ();
		}

		private static char RemoveDiacritic(char c)
		{
			//	Adapté au français et à l'allemand.
			switch (c)
			{
				case 'à':
				case 'â':
				case 'ä':
					return 'a';

				case 'ç':
					return 'c';

				case 'é':
				case 'è':
				case 'ê':
				case 'ë':
					return 'e';

				case 'î':
				case 'ï':
					return 'i';

				case 'ô':
				case 'ö':
					return 'o';

				case 'û':
				case 'ü':
				case 'ù':
					return 'u';

				case 'À':
				case 'Â':
				case 'Ä':
					return 'A';

				case 'Ç':
					return 'C';

				case 'É':
				case 'È':
				case 'Ê':
				case 'Ë':
					return 'E';

				case 'Î':
				case 'Ï':
					return 'I';

				case 'Ô':
				case 'Ö':
					return 'O';

				case 'Û':
				case 'Ü':
				case 'Ù':
					return 'U';

				default:
					return c;
			}
		}


		private enum Ranking
		{
			Discordant    =      0,
			Phonetic      =  50000,
			DifferentCase =  99999,
			StrictlyEqual = 100000,
		}
	}
}
