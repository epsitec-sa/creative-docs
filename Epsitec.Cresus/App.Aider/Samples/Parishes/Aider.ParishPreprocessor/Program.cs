//	Copyright © 2012-2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using System.Linq;
using System.Text;
using Epsitec.Data.Platform;

namespace Aider.ParishProcessor
{
	class Program
	{
		static void Main(string[] args)
		{
			var zips    = SwissPostZipRepository.Current;
			var streets = SwissPostStreetRepository.Current;

			string inputPath  = System.IO.Path.Combine (Program.PathPrefix, @"Source\street list.txt");
			string input2Path = System.IO.Path.Combine (Program.PathPrefix, @"Source\parish names.txt");
			string outputPath = System.IO.Path.Combine (Program.PathPrefix, @"parishes.txt");

//-			Program.VerifyPrilly ();
			
			string[] input = System.IO.File.ReadAllLines (inputPath, System.Text.Encoding.Default).Where (x => x.Contains ('#') == false).ToArray ();

			Program.CheckTownFoundInMatchZip (zips, streets, input);
			Program.CheckStreetFoundInMatchStreetLight (zips, streets, input);

			foreach (var line in System.IO.File.ReadAllLines (input2Path, System.Text.Encoding.Default).Where (x => x.Contains ("\t")))
			{
				string[] items = line.Split ('\t');
				
				string region = items[0].Trim ();
				string parish = items[1].Trim ();

				string prefix = "";
				string realName = parish;

				if (items.Length > 2)
				{
					prefix = items[2].Trim ();
				}
				if (items.Length > 3)
				{
					realName = items[3].Trim ();
				}

				Program.parishRegions[parish] = new ParishInfo
					{
						RealName = realName,
						Region = region,
						Prefix = prefix,
					};
			}


			/*
			 *	Lit le fichier de base des paroisses et produit un fichier plus cohérent utilisable par AIDER
			 *	directement, sous une forme comprimée, stockée dans Aider\DataFiles\ParishAddresses.zip
			 */

			System.IO.File.WriteAllLines (outputPath, input.Where (x => !string.IsNullOrWhiteSpace (x)).Select (x => Program.Transform2CleanLine (zips, x)), System.Text.Encoding.Default);

			/*
			 *	Vérifie que toutes les localités connues dans le canton de Vaud sont bien représentées dans
			 *	le fichier  des paroisses...
			 */
			
			foreach (var zip in zips.FindAll ().Where (x => x.Canton == "VD"))
			{
				switch (zip.ZipType)
				{
					case SwissPostZipType.POBoxOnly:
					case SwissPostZipType.Company:
					case SwissPostZipType.Internal:
						continue;
				}
				
				if (Program.parishInfo.Contains (zip.ZipCode + " " + zip.LongName))
				{
					//	Found
				}
				else
				{
					System.Console.WriteLine ("Sans paroisse: {0} {1}", zip.ZipCode, zip.LongName);
				}
			}
		}

		private static void VerifyPrilly()
		{
			var streets = SwissPostStreetRepository.Current;

			var prillyPath = @"C:\Users\Arnaud\Documents\Epsitec\Projets\Software\EERV - Projet AIDER\prilly.txt";
			var input = System.IO.File.ReadAllLines (prillyPath, System.Text.Encoding.Default);
			var hash = new HashSet<string> ();

			foreach (var streetWithNum in input.Select (x => x.Substring (0, x.Length-4)))
			{
				var street = streetWithNum;

				if (street.StartsWith ("*"))
				{
					continue;
				}

				var pos = streetWithNum.IndexOf ('(');

				if (pos > 0)
				{
					street = street.Substring (0, pos-1);
				}


				var normalized = SwissPostStreet.NormalizeStreetName (street);
				var streetName = street.Split (',')[0];
				
				var matchStreets = streets.Streets.Where (x => x.ZipCode == 1008).ToArray ();

				var found = matchStreets.Where (x => x.MatchShortNameOrRootName (streetName)).ToArray ();

				if (found.Length > 0)
				{
					var officialNormalized = found.Select (x => SwissPostStreet.NormalizeStreetName (x.StreetName)).ToArray ();

					if (officialNormalized.Any (x => string.Equals (x, normalized, System.StringComparison.OrdinalIgnoreCase)))
					{
					}
					else
					{
						System.Console.ForegroundColor = System.ConsoleColor.Yellow;
						System.Console.WriteLine ("{0}: {1} --> {2}", street, normalized, string.Join ("/", officialNormalized));
						System.Console.ResetColor ();
					}
				}

				hash.Add (normalized);
			}

			foreach (var info in streets.Streets.Where (x => x.ZipCode == 1008).ToArray ())
			{
				if (hash.Contains (info.NormalizedStreetName) == false)
				{
					System.Console.WriteLine ("{0} not found", info.StreetName);
				}
			}
		}

		private static int lineCounter = 0;
		private static HashSet<string> duplicateCheck = new HashSet<string> ();
		private static HashSet<string> parishInfo = new HashSet<string> ();
		private static Dictionary<string, ParishInfo> parishRegions = new Dictionary<string, ParishInfo> ();

		struct ParishInfo
		{
			public string RealName;
			public string Region;
			public string Prefix;
		}

		private static string Transform2CleanLine(SwissPostZipRepository zips, string line)
		{
			string[] cols = line.Split ('\t');

			if (cols.Length < 6)
			{
				System.Console.WriteLine ("Error on line: {0}", line);
				return "";
			}

			if (cols[1].Length == 0)
			{
				return "";
			}

			string id           = cols[0];
			int    zipCode      = int.Parse (cols[1], System.Globalization.CultureInfo.InvariantCulture);
			string streetName   = cols[2];
			string streetPrefix = cols[3];
			string range        = cols[4];
			string parish       = cols[5].Replace (" - ", " – ");
			string town;
			string zipTown;

			if ((string.IsNullOrEmpty (streetPrefix) && string.IsNullOrEmpty (range)))
			{
				town = streetName;
				streetName = "";
				streetPrefix = "";
			}
			else
			{
				town = "?";
				foreach (var zip in zips.FindZips (zipCode))
				{
					town = zip.LongName;
					break;
				}
				if (town == "?")
				{
					System.Console.Out.WriteLine ("Error: ZIP {0} not matched to town", zipCode);
				}
			}

			if (town.Contains (">"))
			{
				var split = town.Split ('>');
				
				zipTown = split[1];
				town    = split[0];
			}
			else
			{
				zipTown = town;

				if (town == "Yverdon-les-Bains")
				{
					town = "Yverdon";
				}
			}

			id = string.Format ("{0:0000}", ++lineCounter);

			string key = string.Concat (zipCode, " ", town, " ", streetName, ", ", streetPrefix, " ", range);

			if (Program.duplicateCheck.Add (key) == false)
			{
				System.Console.WriteLine ("Duplicate: {0} {1}", id, key);
			}

			Program.parishInfo.Add (zipCode + " " + zipTown);

			if ((streetPrefix.Length > 0) &&
				(char.IsUpper (streetPrefix[0])))
			{
				System.Console.WriteLine ("Uppercase street prefix: {0} in {1}", streetPrefix, line);
			}

			ParishInfo info;

			if (Program.parishRegions.TryGetValue (parish, out info) == false)
			{
				System.Console.WriteLine ("Paroisse sans région: {0}", parish);
			}
			
			return string.Concat (id, "\t", zipCode, "\t", town, "\t", zipTown, "\t", streetName, "\t", streetPrefix, "\t", range, "\t", parish, "\t", info.Region, "\t", info.Prefix, "\t", info.RealName);
		}


		private static void CheckTownFoundInMatchZip(SwissPostZipRepository zips, SwissPostStreetRepository streets, string[] input)
		{
			foreach (var line in input)
			{
				string[] cols = line.Split ('\t');

				if (cols[1].Length == 0)
				{
					//	Line with empty ZIP code, just skip it.
					continue;
				}

				int    zipCode = int.Parse (cols[1], System.Globalization.CultureInfo.InvariantCulture);
				string streetName = cols[2];
				string streetPrefix = cols[3];


				if ((string.IsNullOrEmpty (streetPrefix) && string.IsNullOrEmpty (cols[4])))
				{
					//	Town
					var townName  = streetName.Split ('>').Last ();
					var matchZips = zips.FindZips (zipCode).Where (x => x.MatchName (townName)).ToArray ();

					if (matchZips.Length == 0)
					{
						System.Console.Out.WriteLine ("Could not find town: {0}", townName);

						foreach (var zip in zips.FindZips (zipCode))
						{
							System.Console.Out.WriteLine ("  {0} = {1}", zip.ZipCode, zip.LongName);
						}

						var substitute = zips.FindAll ().Where (x => x.MatchName (townName)).FirstOrDefault ();

						if (substitute != null)
						{
							System.Console.Out.WriteLine (" ===> corriger selon {0}", substitute.ZipCode);
						}
					}
				}
			}
		}

		private static void CheckStreetFoundInMatchStreetLight(SwissPostZipRepository zips, SwissPostStreetRepository streets, string[] input)
		{
			foreach (var line in input)
			{
				string[] cols = line.Split ('\t');

				if (cols[1].Length == 0)
				{
					continue;
				}

				int    zipCode = int.Parse (cols[1], System.Globalization.CultureInfo.InvariantCulture);
				string streetName = cols[2];
				string streetPrefix = cols[3];


				if ((string.IsNullOrEmpty (streetPrefix) && string.IsNullOrEmpty (cols[4])))
				{
					continue;
				}

				string normalized = SwissPostStreet.NormalizeStreetName (streetName + ", " + streetPrefix);

				var matchZips = zips.FindZips (zipCode).ToArray ();
				var matchStreets = streets.Streets.Where (x => x.ZipCode == zipCode).ToArray ();

				var found = matchStreets.Where (x => x.MatchShortNameOrRootName (streetName)).ToArray ();

				if (found.Length > 0)
				{
					var officialNormalized = found.Select (x => SwissPostStreet.NormalizeStreetName (x.StreetName)).ToArray ();

					if (officialNormalized.Any (x => string.Equals (x, normalized, System.StringComparison.OrdinalIgnoreCase)))
					{
					}
					else
					{
						System.Console.ForegroundColor = System.ConsoleColor.Yellow;
						System.Console.WriteLine ("{0}: {1} --> {2}", cols[0], normalized, string.Join ("/", officialNormalized));
						System.Console.ResetColor ();
					}
				}
				else
				{
					if (streetName.StartsWith ("*"))
					{
					}
					else
					{
						System.Console.ForegroundColor = System.ConsoleColor.Red;
						System.Console.Out.WriteLine ("{0}: {1} {2}", cols[0], zipCode, streetName);
						System.Console.ResetColor ();
					}
				}
			}
		}
		
		
		private static void Transform1()
		{
			string inputPath = @"C:\Users\Arnaud\Documents\Epsitec\Projets\EERV - Projet AIDER\2012-02-xx Clean wp npa.txt";
			string outputPath = @"C:\Users\Arnaud\Documents\Epsitec\Projets\EERV - Projet AIDER\2012-02-xx Parish Info.txt";

			string[] input = System.IO.File.ReadAllLines (inputPath, System.Text.Encoding.Default);
			string[] output = input.Select (x => Program.Transform1CleanLine (x)).Where (x => x != null).ToArray ();

			System.IO.File.WriteAllLines (outputPath, output, System.Text.Encoding.Default);
		}

		private static string Transform1CleanLine(string line)
		{
			string[] cols = line.Split ('\t').Select (x => x.Trim ('"', ' ')).ToArray ();

			if ((cols.Length != 4) ||
				(string.IsNullOrWhiteSpace (cols[0])) ||
				(string.IsNullOrWhiteSpace (cols[1])))
			{
				return null;
			}

			string id       = cols[0];
			string zipCode  = cols[1];
			string location = cols[2];

			int pos1 = location.IndexOf ('(');
			int pos2 = location.IndexOf (')');

			if (pos1 > 0)
			{
				if (pos2 < 0)
				{
					throw new System.Exception ("Invalid () in line "+id);
				}

				location = location.Substring (0, pos1).Trim () + location.Substring (pos2+1).Trim ();
			}

			string[] args = location.Split (new string[] { ",", " et " }, System.StringSplitOptions.RemoveEmptyEntries).Select (x => x.Trim ()).ToArray ();

			string name   = args[0];
			string prefix = "";
			string ranges = "";

			if (args.Length > 1)
			{
				string streetPrefix = args[1];
				
				if ((char.IsDigit (streetPrefix[0])) ||
					(streetPrefix[0] == '-' && char.IsDigit (streetPrefix[1])))
				{
					ranges = string.Join ("/", args.Skip (1).ToArray ());
				}
				else if (streetPrefix.Any (x => char.IsDigit (x)))
				{
					System.Console.Out.WriteLine ("Error, missing separator: {0}: in '{1}'", id, streetPrefix);
				}
				else
				{
					prefix = streetPrefix;
					ranges = string.Join ("/", args.Skip (2).ToArray ());
				}
			}

			return string.Concat (id, "\t", zipCode, "\t", name, "\t", prefix, "\t", ranges, "\t", cols[3]);
		}

		private static readonly string PathPrefix = @"S:\Epsitec.Cresus\App.Aider\Samples\Parishes";
	}
}