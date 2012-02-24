//	Copyright © 2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types.Converters;
using Epsitec.Common.Types.Collections;

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Types;

namespace Epsitec.Data.Platform
{
	/// <summary>
	/// The <c>SwissPostStreet</c> class helps normalizing street names and house numbers.
	/// It provides also low level access to the official streets database (MAT[CH]street).
	/// </summary>
	public static class SwissPostStreet
	{
		/// <summary>
		/// Tokenizes the name of the street by exploding it into uppercase tokens, without
		/// any accents, ordering the name in the normal reading order (i.e. "Lac, au Grand-"
		/// becomes "au Grand-Lac" before being tokenized to "GRAND"/"LAC") and removing
		/// noise (such as "le", "la") and using full street designations ("AVENUE" instead
		/// of "AV.").
		/// </summary>
		/// <param name="name">The full street name.</param>
		/// <returns>The tokenized street name.</returns>
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

			var meaningfulTokens = name.Split (SwissPostStreet.TokenSeparators, System.StringSplitOptions.RemoveEmptyEntries).Where (x => SwissPostStreet.IsMeaningful (x));
			var normalizedTokens = meaningfulTokens.Select (x => SwissPostStreet.NormalizeToken (x));

			return normalizedTokens;
		}

		/// <summary>
		/// Normalizes the name of the street by tokenizing it (<see cref="TokenizeStreetName"/>)
		/// and then joining the tokens together.
		/// </summary>
		/// <param name="name">The full street name.</param>
		/// <returns>The normalized street name.</returns>
		public static string NormalizeStreetName(string name)
		{
			return string.Join (" ", SwissPostStreet.TokenizeStreetName (name));
		}

		/// <summary>
		/// Normalizes the house number by parsing digits as long as there are any; an empty
		/// house number will be treated as zero. This method always succeeds.
		/// </summary>
		/// <param name="number">The house number.</param>
		/// <returns>The normalized house number.</returns>
		public static int NormalizeHouseNumber(string number)
		{
			return InvariantConverter.ParseInt (number);
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
				return SwissPostStreet.NormalizationNoise.Contains (token) == false;
			}
		}

		private static string NormalizeToken(string token)
		{
			string output;

			if (SwissPostStreet.NormalizationTuples.TryGetValue (token, out output))
			{
				return output;
			}
			else
			{
				return token;
			}
		}

		#region Internal Constants

		internal static readonly char[] NameSeparators  = new char[] { ' ', '-', '.', '\'' };
		internal static readonly char[] TokenSeparators = new char[] { ' ', '-', '\'' };

		internal static readonly Dictionary<string,string> NormalizationTuples = new Dictionary<string, string> ()
		{
			{"CH.", "CHEMIN"},
			{"AV.", "AVENUE"},
			{"RTE", "ROUTE"},
			{"R.", "RUE"},
			{"RLLE", "RUELLE"},
			{"PL.", "PLACE"},
			{"PROM.", "PROMENADE"},
			{"QUART.", "QUARTIER"},
			{"ST", "SAINT"},
			{"STE", "SAINTE"},
		};

		internal static readonly HashSet<string> HeuristicTokens = new HashSet<string> ()
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

		internal static readonly HashSet<string> NormalizationNoise = new HashSet<string> ()
		{
			"DE", "DU", "D", "DES", "LE", "LA", "L", "LES", "EN", "AU"
		};

		#endregion
	}
}
