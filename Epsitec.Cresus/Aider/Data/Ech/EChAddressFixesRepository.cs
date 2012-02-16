//	Copyright © 2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Data.Platform;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Aider.Data.Ech
{
	public sealed class EChAddressFixesRepository
	{
		private EChAddressFixesRepository()
		{
			this.fixes = new Dictionary<string, System.Tuple<string, string>> ();
			this.failures = new HashSet<string> ();

			foreach (var item in EChAddressFixesRepository.GetFixes ())
			{
				fixes.Add (item.Key, item.Value);
			}
		}


		public static readonly EChAddressFixesRepository	Current = new EChAddressFixesRepository ();

	
		public IEnumerable<string> GetFailures()
		{
			return this.failures;
		}


		public bool ApplyFix(ref string zipCode, ref string streetName, string addressLine1 = null)
		{
			bool fixApplied = false;
			
			fixApplied |= this.ApplyQuickFix (ref zipCode, ref streetName);
			fixApplied |= this.ApplySwissPostFix (zipCode, ref streetName, addressLine1) == Fix.Applied;

			return fixApplied;
		}

		private bool ApplyQuickFix(ref string zipCode, ref string streetName)
		{
			string key = string.Concat (zipCode, " ", streetName);

			System.Tuple<string, string> fix;

			if (this.fixes.TryGetValue (key, out fix))
			{
				zipCode    = fix.Item1;
				streetName = fix.Item2;

				return true;
			}

			return false;
		}

		private Fix ApplySwissPostFix(string zipCode, ref string streetName, string addressLine1, bool logFailures = true)
		{
			if (string.IsNullOrEmpty (streetName))
			{
				return Fix.Invalid;
			}

			int zip     = int.Parse (zipCode, System.Globalization.CultureInfo.InvariantCulture);
			var streets = SwissPostStreetRepository.Current.FindStreets (zip);
			var tokens  = SwissPostStreet.TokenizeStreetName (streetName).ToArray ();
			
			int n = tokens.Length;

			if (n == 0)
			{
				return Fix.Invalid;
			}

			//	The tokens are words in upper case, without any accented letters. For instance
			//	"CHEMIN"/"FONTENAY" or "AVENUE"/"QUATRE"/"MARRONNIERS".

			//	Sometimes, we need to try several permutations in order to find the proper street,
			//	as the root of the name used by MAT[CH] depends on subtle language-based heuristics.

			var shuffles = new string[][]
			{
				tokens,
				new string[] { tokens[0] },
				(n > 1) ? new string[] { tokens[n-1] } : null,
				(n > 1) ? new string[] { tokens[n-2], tokens[n-1] } : null,
				(n > 2) ? new string[] { tokens[n-3], tokens[n-2], tokens[n-1] } : null
			};

			SwissPostStreetInformation match = null;

			for (int i = 0; match == null; i++)
			{
				if (i == 5)
				{
					//	Failed to match any of the 5 attempts: if there is an additional address
					//	line, try that one too (in case the address was stored in the wrong field
					//	by the eCH software).

					if (string.IsNullOrEmpty (addressLine1) == false)
					{
						var status = this.ApplySwissPostFix (zipCode, ref addressLine1, null, false);

						if (status == Fix.Invalid)
						{
							if (logFailures)
							{
								this.failures.Add (string.Concat (zipCode, " ", streetName));
							}

							return Fix.Invalid;
						}
						else
						{
							//	Yep, the additional address line was in fact the street name. Use
							//	it instead:

							streetName = addressLine1;

							return Fix.Applied;
						}
					}

					//	Failed to resolve the name, log this if desired.

					if (logFailures)
					{
						this.failures.Add (string.Concat (zipCode, " ", streetName));
					}

					return Fix.Invalid;

				}

				var shuffle = shuffles[i];
				
				if (shuffle != null)
				{
					match = streets.Where (x => x.MatchNameWithHeuristics (shuffle)).FirstOrDefault ();
				}
			}

			string preferred = match.StreetName;

			if (preferred == streetName)
			{
				return Fix.Unchanged;
			}
			else
			{
				streetName = preferred;
				return Fix.Applied;
			}
		}

		public enum Fix
		{
			Unchanged,
			Invalid,
			Applied
		}

		private static IEnumerable<KeyValuePair<string, System.Tuple<string, string>>> GetFixes()
		{
			foreach (var line in EChAddressFixesRepository.GetSourceFile ())
			{
				var tokens = line.Split ('\t');

				if (tokens.Length == 4)
				{
					var key   = tokens[0] + " " + tokens[1];
					var tuple = new System.Tuple<string, string> (tokens[2], tokens[3]);

					yield return new KeyValuePair<string, System.Tuple<string, string>> (key, tuple);
				}
			}
		}

		private static IEnumerable<string> GetSourceFile()
		{
			var assembly = System.Reflection.Assembly.GetExecutingAssembly ();
			var resource = "Epsitec.Aider.DataFiles.eCH-StreetFixes.zip";
			var source   = Epsitec.Common.IO.ZipFile.DecompressTextFile (assembly, resource);

			return Epsitec.Common.IO.StringLineExtractor.GetLines (source);
		}


		private readonly Dictionary<string, System.Tuple<string, string>>	fixes;
		private readonly HashSet<string>	failures;
	}
}
