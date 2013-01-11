//	Copyright © 2012-2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support.Extensions;
using Epsitec.Common.Types;
using Epsitec.Common.Types.Converters;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Data.Platform
{
	/// <summary>
	/// The <c>SwissPostStreet</c> class helps normalizing street names and house numbers.
	/// It provides also low level access to the official streets database (MAT[CH]street).
	/// </summary>
	public static class SwissPostStreet
	{
		/// <summary>
		/// Converts the Swiss Post street name (as represent in the MAT[CH]street database,
		/// that is "Neuchâtel, rue de") into a user friendly street name (such as
		/// "rue de Neuchâtel 32").
		/// </summary>
		/// <param name="street">The Swiss Post street name.</param>
		/// <returns>The user friendly street name.</returns>
		public static string ConvertToUserFriendlyStreetName(string street)
		{
			if (string.IsNullOrEmpty (street))
			{
				return street;
			}

			int pos = street.LastIndexOf (',');

			if (pos < 0)
			{
				return street;
			}

			var root = street.Substring (0, pos);
			var prefix = street.Substring (pos+1).Trim ();

			if (prefix.Length > 0)
			{
				char end = prefix.LastCharacter ();

				switch (end)
				{
					case '-':
					case '\'':
						return string.Concat (prefix, root);
					
					default:
						return string.Concat (prefix, " ", root);
				}
			}
			else
			{
				return root;
			}
		}

		/// <summary>
		/// Converts a user friendly street name to a Swiss Post street name, as defined
		/// by the MAT[CH]street database. This requires a ZIP code in order to match the
		/// correct street name, as the encoding is not unique.
		/// </summary>
		/// <param name="zipCode">The zip code.</param>
		/// <param name="street">The user friendly street name.</param>
		/// <returns>The Swiss Post street name or <c>null</c> if the name could not be
		/// resolved back using the MAT[CH]street database.</returns>
		public static string ConvertFromUserFriendlyStreetName(int zipCode, string street)
		{
			if (string.IsNullOrEmpty (street))
			{
				return street;
			}

			var repository = SwissPostStreetRepository.Current;
			var matchStreet = repository.FindStreetFromUserFriendlyStreetNameDictionary (zipCode, street)
				/**/       ?? repository.FindStreetFromStreetName (zipCode, street);

			if (matchStreet != null)
			{
				return matchStreet.StreetName;
			}

			return null;
		}

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
				return Enumerable.Empty<string> ();
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

		/// <summary>
		/// Strips an house number from its terminal non digit part. This will remove any "bis",
		/// "ter", "A", "B", "C", etc. at the end of the house number.
		/// </summary>
		public static string StripHouseNumber(string number)
		{
			string result = null;

			if (number != null)
			{
				var index = SwissPostStreet.GetHouseNumberComplementIndex (number);

				if (index > 0)
				{
					result = number.Substring (0, index);
				}
			}

			return result;
		}

		/// <summary>
		/// Returns terminal non digit part of an house number. This will return "bis", "ter", "A",
		/// "B", "C", ect. at the end of the house number if present.
		/// </summary>
		public static string GetHouseNumberComplement(string number)
		{
			string result = null;

			if (number != null)
			{
				var index = SwissPostStreet.GetHouseNumberComplementIndex (number);

				result = number.Substring (index).Trim ();
			}

			return result;
		}

		private static int GetHouseNumberComplementIndex(string number)
		{
			int index = 0;

			while (index < number.Length && System.Char.IsDigit (number[index]))
			{
				index++;
			}

			return index;
		}

		public static int StripAndNormalizeHouseNumber(string number)
		{
			var strippedNumber = SwissPostStreet.StripHouseNumber (number);

			return SwissPostStreet.NormalizeHouseNumber (strippedNumber);
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
