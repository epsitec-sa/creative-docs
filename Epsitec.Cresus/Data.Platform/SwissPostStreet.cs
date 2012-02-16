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
		public static SwissPostStreetRepository CreateRepository()
		{
			return new SwissPostStreetRepository (SwissPostStreet.GetStreets ());
		}

		public static string NormalizeStreetName(string name)
		{
			name = TextConverter.ConvertToUpperAndStripAccents (name);

			int pos = name.IndexOf (',');

			if (pos < 0)
			{
				var meaningfulTokens = name.Split (SwissPostStreet.tokenSeparators, System.StringSplitOptions.RemoveEmptyEntries).Where (x => SwissPostStreet.IsMeaningful (x));
				var normalizedTokens = meaningfulTokens.Select (x => SwissPostStreet.NormalizeToken (x));

				name = string.Join (" ", normalizedTokens);
			}
			else
			{
				string root   = name.Substring (0, pos);
				string prefix = name.Substring (pos+1);

				var meaningfulTokens = prefix.Split (SwissPostStreet.tokenSeparators, System.StringSplitOptions.RemoveEmptyEntries).Where (x => SwissPostStreet.IsMeaningful (x));
				var normalizedTokens = meaningfulTokens.Select (x => SwissPostStreet.NormalizeToken (x));

				name = string.Join (" ", normalizedTokens) + " " + root;
			}

			int len = name.Length;
			bool skipSpace = true;
			System.Text.StringBuilder buffer = new System.Text.StringBuilder (len);

			for (int i = 0; i < len; i++)
			{
				char c = name[i];

				if ((c == ' ') || (c == '-'))
				{
					if (skipSpace == false)
					{
						skipSpace = true;
						buffer.Append (' ');
					}
				}
				else
				{
					buffer.Append (c);
					skipSpace = false;
				}
			}

			return buffer.ToString ();
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

		private static IEnumerable<SwissPostStreetInformation> GetStreets()
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
		internal static readonly char[] tokenSeparators = new char[] { ' ', '-' };

		private static readonly Dictionary<string,string> normalizationTuples = new Dictionary<string, string> ()
		{
			{"CH.", "CHEMIN"},
			{"AV.", "AVENUE"},
			{"RTE", "ROUTE"},
			{"PL.", "PLACE"},
			{"PROM.", "PROMENADE"},
			{"QUART.", "QUARTIER"},
		};

		private static readonly HashSet<string> normalizationNoise = new HashSet<string> ()
		{
			"DE", "DU", "D'", "DES", "LE", "LA", "L'", "LES"
		};
	}
}
