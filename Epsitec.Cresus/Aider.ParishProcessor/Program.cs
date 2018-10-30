//	Copyright © 2012-2017, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using System.Linq;
using System.Text;
using Epsitec.Data.Platform;

namespace AIDER.ParishPreprocessor
{
	class Program
	{
		static void Main(string[] args)
		{
			var zips = SwissPostZipRepository.Current;
			var streets = SwissPostStreetRepository.Current;

			var exePath = System.Reflection.Assembly.GetExecutingAssembly ().GetFiles ()[0].Name;
			var exeDir = System.IO.Path.GetDirectoryName (exePath);
			var dataDir = System.IO.Path.GetFullPath (System.IO.Path.Combine (exeDir, "..", "..", "Data"));

			string inputPath = System.IO.Path.Combine (dataDir, "2018-01-xx Clean Parish Info.txt");
			string input2Path = System.IO.Path.Combine (dataDir, "2018-01-xx parish names.txt");
			string outputPath = System.IO.Path.Combine (dataDir, "2018-01-xx parishes.txt");
			string missingPath = System.IO.Path.Combine (dataDir, "2018-01-xx missing.txt");

			Program.ReadParishNames (input2Path);

			//-			Program.VerifyPrilly ();

			var input = System.IO.File
				.ReadAllLines (inputPath, System.Text.Encoding.Default)
				.Where (x => x.Contains ('#') == false)
				.ToArray ();

			Program.CheckTownFoundInMatchZip (zips, streets, input);
			Program.CheckStreetFoundInMatchStreetLight (zips, streets, input);

			/*
			 *	Lit le fichier de base des paroisses et produit un fichier plus cohérent utilisable par AIDER
			 *	directement, sous une forme comprimée, stockée dans Aider\DataFiles\ParishAddresses.zip
			 */

			var parishAddresses = input
				.Where (x => !string.IsNullOrWhiteSpace (x))
				.Select (x => Program.Transform2CleanLine (zips, x))
				.ToList ();

			System.IO.File.WriteAllLines (outputPath, parishAddresses.Select (x => x.ToString ()), System.Text.Encoding.Default);

			var streetsWithMissingParishAddresses = Program.VerifyStreets (streets, parishAddresses).ToList ();

			System.IO.File.WriteAllLines (missingPath, streetsWithMissingParishAddresses.Select (
				x => $"{x.Zip.ZipCode}\t{x.StreetName.Split (',')[0]}\t{x.StreetName.Split (',').Skip (1).FirstOrDefault ()?.Trim () ?? ""}"),
				System.Text.Encoding.Default);


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

			System.Diagnostics.Process.Start (@"S:\git\core\cresus\Epsitec.Cresus\Aider\DataFiles");
			System.Diagnostics.Process.Start (dataDir);
		}

		private static IEnumerable<SwissPostStreetInformation> VerifyStreets(SwissPostStreetRepository streetRepository, List<ParishAddress> parishAddresses)
		{
			var allZips   = parishAddresses.Select (x => x.Zip).Distinct ().OrderBy (x => x).ToList ();
			var multiZips = parishAddresses.Where (x => x.StreetPrefix.Length > 0).Select (x => x.Zip).Distinct ().OrderBy (x => x).ToList ();

			foreach (var zip in multiZips)
			{
				var streets = streetRepository.Streets
					.Where (x => x.Zip.ZipCode == zip)
					.ToList ();

				var dividers = streets.Where (x => x.DividerCode != SwissPostDividerCode.None).ToList ();

				var addresses = parishAddresses
					.Where (x => x.Zip == zip)
					.ToList ();

				foreach (var street in streets)
				{
					var name = street.StreetName;
					var index = addresses.FindIndex (x => x.StreetFullName == name);

					if (index < 0)
					{
						var name1 = Program.SimplifyName (name);
						var index1 = addresses.FindIndex (x => Program.SimplifyName (x.StreetFullName) == name1);

						if (index1 < 0)
						{
							System.Console.ForegroundColor = System.ConsoleColor.Red;
							System.Console.WriteLine (street);
							yield return street;
						}
						else
						{
							System.Console.ForegroundColor = System.ConsoleColor.Yellow;
							System.Console.WriteLine (street);
						}
					}
					else
					{
						System.Console.ForegroundColor = System.ConsoleColor.Green;
						System.Console.WriteLine (street);
						addresses.RemoveAt (index);
					}
					System.Console.ResetColor ();
				}
			}
		}

		private static string SimplifyName(string name)
		{
			return name
				.Replace (" du", "")
				.Replace (" de l'", "")
				.Replace (" de la", "")
				.Replace (" des", "")
				.Replace (" de", "")
				.Replace (", -", "")
				.Replace ("-", " ");
		}

		private static void ReadParishNames(string input2Path)
		{
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
				
				var matchStreets = streets.Streets.Where (x => x.ZipCodeAndAddOn.ZipCode == 1008).ToArray ();

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

			foreach (var info in streets.Streets.Where (x => x.ZipCodeAndAddOn.ZipCode == 1008).ToArray ())
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

		private static ParishAddress Transform2CleanLine(SwissPostZipRepository zips, string line)
		{
			string[] cols = line.Split ('\t');

			if (cols.Length < 6)
			{
				System.Console.WriteLine ("Error on line: {0}", line);
				return null;
			}

			if (cols[1].Length == 0)
			{
				return null;
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

			return new ParishAddress (id, zipCode, town, zipTown, streetName, streetPrefix, range,
				parish, info.Region, info.Prefix, info.RealName);
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
				var matchStreets = streets.Streets.Where (x => x.ZipCodeAndAddOn.ZipCode == zipCode).ToArray ();

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
	}
}
