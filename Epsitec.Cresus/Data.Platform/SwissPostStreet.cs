//	Copyright © 2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types.Converters;
using Epsitec.Common.Types.Collections;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Data.Platform
{
	public static class SwissPostStreet
	{
		public static IEnumerable<string> TokenizeStreetName(string name)
		{
			if (string.IsNullOrEmpty (name))
			{
				return EmptyEnumerable<string>.Instance;
			}

			name = TextConverter.ConvertToUpperAndStripAccents (name);

			int pos = name.IndexOf (',');

			if (pos >= 0)
			{
				string root   = name.Substring (0, pos);
				string prefix = name.Substring (pos+1);

				name = prefix + " " + root;
			}

			var meaningfulTokens = name.Split (SwissPostStreet.tokenSeparators, System.StringSplitOptions.RemoveEmptyEntries).Where (x => SwissPostStreet.IsMeaningful (x));
			var normalizedTokens = meaningfulTokens.Select (x => SwissPostStreet.NormalizeToken (x));

			return normalizedTokens;
		}

		public static string NormalizeStreetName(string name)
		{
			return string.Join (" ", SwissPostStreet.TokenizeStreetName (name));
		}

		public static int NormalizeHouseNumber(string number)
		{
			if (string.IsNullOrEmpty (number))
			{
				return 0;
			}
			int numeric = 0;
			int len = number.Length;

			for (int i = 0; i < len; i++)
			{
				char c = number[i];

				if (char.IsDigit (c))
				{
					numeric = numeric*10 + c - '0';
				}
				else
				{
					break;
				}
			}

			return numeric;
		}

		internal static IEnumerable<SwissPostStreetInformation> GetStreets()
		{
			foreach (var line in SwissPostStreet.GetStreetFile ())
			{
				yield return new SwissPostStreetInformation (line);
			}
		}

		private static IEnumerable<string> GetStreetFile()
		{
			var assembly = System.Reflection.Assembly.GetExecutingAssembly ();
			var resource = "Epsitec.Data.Platform.DataFiles.MatchStreetLight.zip";
			var source   = Epsitec.Common.IO.ZipFile.DecompressTextFile (assembly, resource);
			
			return Epsitec.Common.IO.StringLineExtractor.GetLines (source);
		}
		
		private static bool IsMeaningful(string token)
		{
			if (string.IsNullOrWhiteSpace (token))
			{
				return false;
			}
			else
			{
				return SwissPostStreet.normalizationNoise.Contains (token) == false;
			}
		}

		private static string NormalizeToken(string token)
		{
			string output;

			if (SwissPostStreet.normalizationTuples.TryGetValue (token, out output))
			{
				return output;
			}
			else
			{
				return token;
			}
		}

		internal static readonly char[] nameSeparators  = new char[] { ' ', '-', '.', '\'' };
		internal static readonly char[] tokenSeparators = new char[] { ' ', '-', '\'' };

		private static readonly Dictionary<string,string> normalizationTuples = new Dictionary<string, string> ()
		{
			{"CH.", "CHEMIN"},
			{"AV.", "AVENUE"},
			{"RTE", "ROUTE"},
			{"PL.", "PLACE"},
			{"PROM.", "PROMENADE"},
			{"QUART.", "QUARTIER"},
			{"ST", "SAINT"},
			{"STE", "SAINTE"},
		};

		public static readonly HashSet<string> heuristicTokens = new HashSet<string> ()
		{
			"AVENUE",
			"BATTERIE",
			"CHEMIN",
			"PARC",
			"PLACE",
			"PROMENADE",
			"QUAI",
			"QUARTIER",
			"ROUTE",
			"RUE",
			"SQUARE",
			"ZONE",
		};

		private static readonly HashSet<string> normalizationNoise = new HashSet<string> ()
		{
			"DE", "DU", "D", "DES", "LE", "LA", "L", "LES", "EN", "AU"
		};
	}
}
