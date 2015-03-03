using Epsitec.Aider.Data.Common;

using Epsitec.Common.Support.Extensions;

using Epsitec.Data.Platform;

using System;

using System.Collections.Generic;

using System.IO;

using System.Linq;


namespace Epsitec.Aider.Tests
{


	public static class ParishFileAnalyzer
	{


		public static void Analyze(FileInfo input, FileInfo output)
		{
			var repository = ParishFileAnalyzer.GetRepository (input);
			var parishes = ParishFileAnalyzer.GetParishes (repository);

			ParishFileAnalyzer.Write (parishes, output);
		}


		private static ParishAddressRepository GetRepository(FileInfo input)
		{
			var lines = File.ReadAllLines (input.FullName, System.Text.Encoding.Default);

			return new ParishAddressRepository (lines);
		}


		private static IEnumerable<Parish> GetParishes(ParishAddressRepository repository)
		{
			var parishes = ParishFileAnalyzer
				.GetParishList (repository)
				.ToDictionary (p => p.Name);

			var townToAddresses = ParishFileAnalyzer
				.GetTownToAddresses (repository);

			var townsWithSingleParish = ParishFileAnalyzer.
				GetTownsWithSingleParish (townToAddresses);

			var townsWithSeveralParishes = ParishFileAnalyzer.
				GetTownsWithSeveralParishes (townToAddresses);

			ParishFileAnalyzer.AssignToParishes (parishes, townsWithSingleParish);
			ParishFileAnalyzer.AssignToParishes (repository, parishes, townsWithSeveralParishes);
			ParishFileAnalyzer.AssignUnassignedTowns (parishes, townToAddresses.Keys);

			return parishes.Values;
		}


		private static IEnumerable<Parish> GetParishList(ParishAddressRepository repository)
		{
			return repository
				.FindAllAddressInformations ()
				.Select (a => a.ParishName)
				.Distinct ()
				.Append (ParishFileAnalyzer.NoParish)
				.Select (n => new Parish (n));
		}


		private static Dictionary<Town, List<ParishAddressInformation>> GetTownToAddresses(ParishAddressRepository repository)
		{
			return repository
				.FindAllAddressInformations ()
				.GroupBy (a => new Town (a.ZipCode, a.TownNameOfficial))
				.ToDictionary
				(
					g => g.Key,
					g => g.ToList ()
				);
		}


		private static Dictionary<Town, string> GetTownsWithSingleParish(Dictionary<Town, List<ParishAddressInformation>> townToAddresses)
		{
			return townToAddresses
				.Where (i => ParishFileAnalyzer.HasSingleParish (i.Value))
				.ToDictionary
				(
					i => i.Key,
					i => i.Value.First ().ParishName
				);
		}


		private static Dictionary<Town, List<ParishAddressInformation>> GetTownsWithSeveralParishes(Dictionary<Town, List<ParishAddressInformation>> townToAddresses)
		{
			return townToAddresses
				.Where (i => !ParishFileAnalyzer.HasSingleParish (i.Value))
				.ToDictionary
				(
					i => i.Key,
					i => i.Value
				);
		}


		private static bool HasSingleParish(List<ParishAddressInformation> addresses)
		{
			var nbParishes = addresses
				.Select (a => a.ParishName)
				.Distinct ()
				.Count ();

			return nbParishes == 1;
		}


		private static void AssignToParishes(Dictionary<string, Parish> parishes, Dictionary<Town, string> townsWithSingleParish)
		{
			foreach (var item in townsWithSingleParish)
			{
				var town = item.Key;

				var parishName = item.Value;
				var parish = parishes[parishName];

				parish.Towns.Add (town);
			}
		}


		private static void AssignToParishes(ParishAddressRepository repository, Dictionary<string, Parish> parishes, Dictionary<Town, List<ParishAddressInformation>> townsWithSeveralParishes)
		{
			foreach (var item in townsWithSeveralParishes)
			{
				var town = item.Key;
				var addresses = item.Value;

				ParishFileAnalyzer.AssignToParishes (repository, parishes, town, addresses);
			}
		}


		private static void AssignToParishes(ParishAddressRepository repository, Dictionary<string, Parish> parishes, Town town, List<ParishAddressInformation> addresses)
		{
			var streets = ParishFileAnalyzer.FindStreets (town);

			foreach (var street in streets)
			{
				var streetName = street.Key;
				var streetData = street.Value;

				// If the street is not explicitely referenced by a parish or if the whole street is
				// explicitely referenced by a single parish, we know that the whole street is
				// assigned to a single parish and we don't need to check every number.
				if (ParishFileAnalyzer.IsSimpleCase (addresses, streetName))
				{
					ParishFileAnalyzer.AssignToParishes (repository, parishes, town, streetName);
				}
				// Otherwise, we need to check every number to know at which parish it is assigned
				// to.
				else
				{
					ParishFileAnalyzer.AssignToParishes (repository, parishes, town, streetName, streetData);
				}
			}
		}

		private static bool IsSimpleCase(List<ParishAddressInformation> addresses, string streetName)
		{
			var matches = addresses
				.Where (v => v.NormalizedStreetName == streetName)
				.ToList ();

			return (matches.Count == 0)
				|| (matches.Count == 1 && matches.Single ().StreetNumberSubset == null);
		}


		private static void AssignToParishes(ParishAddressRepository repository, Dictionary<string, Parish> parishes, Town town, string streetName)
		{
			var parishName = ParishFileAnalyzer.FindParishName(repository, town, streetName, 1);
			var parish = parishes[parishName];

			var street = new Street (town, streetName, new int[0]);

			parish.Streets.Add (street);
		}


		private static void AssignToParishes(ParishAddressRepository repository, Dictionary<string, Parish> parishes, Town town, string streetName, List<SwissPostStreetInformation> streets)
		{
			var numbers = new Dictionary<string, HashSet<int>> ();

			foreach (var street in streets)
			{
				var min = street.HouseNumberFrom;
				var max = street.HouseNumberTo;
				var step = 2;

				switch (street.DividerCode)
				{
					case SwissPostDividerCode.All:
						step = 1;
						break;

					// 401 is the maximum value that we might have for street number, according to
					// the implementation of ParishAddressInformation. So when we don't have this
					// info from the swiss post, we guess it to this number.
					case SwissPostDividerCode.None:
						step = 1;
						min = 1;
						max = 401;
						break;
				}

				for (int i = min; i <= max; i += step)
				{
					ParishFileAnalyzer.AssignNumberToParishes (repository, numbers, town, streetName, i);
				}
			}

			foreach (var subset in numbers)
			{
				var street = new Street (town, streetName, subset.Value);
				var parish = parishes[subset.Key];

				parish.Streets.Add (street);
			}
		}


		private static void AssignNumberToParishes(ParishAddressRepository repository, Dictionary<string, HashSet<int>> numbers, Town town, string streetName, int i)
		{
			var parishName = ParishFileAnalyzer.FindParishName (repository, town, streetName, i);

			HashSet<int> parishNumbers;

			if (!numbers.TryGetValue (parishName, out parishNumbers))
			{
				parishNumbers = new HashSet<int> ();
				numbers[parishName] = parishNumbers;
			}

			parishNumbers.Add (i);
		}


		private static string FindParishName(ParishAddressRepository repository, Town town, string streetName, int nb)
		{
			var parishName = repository.FindParishName (town.Zip, town.Name, streetName, nb);

			return parishName ?? ParishFileAnalyzer.NoParish;
		}


		private static Dictionary<string, List<SwissPostStreetInformation>> FindStreets(Town town)
		{
			return ParishFileAnalyzer
				.FindTowns (town)
				.SelectMany (t => ParishFileAnalyzer.FindStreets (t))
				.GroupBy (s => s.NormalizedStreetName)
				.ToDictionary
				(
					g => g.Key,
					g => g.ToList ()
				);
		}


		private static IEnumerable<SwissPostZipInformation> FindTowns(Town town)
		{
			return SwissPostZipRepository
				.Current
				.FindZips (town.Zip, town.Name);
		}


		private static IEnumerable<SwissPostStreetInformation> FindStreets(SwissPostZipInformation town)
		{
			return SwissPostStreetRepository
				.Current
				.FindStreets (town.ZipCode, town.ZipCodeAddOn)
				.Where (s => s.Zip.ZipCodeAddOn == town.ZipCodeAddOn);
		}


		private static void AssignUnassignedTowns(Dictionary<string, Parish> parishes, IEnumerable<Town> assignedTowns)
		{
			var okTowns = assignedTowns.ToSet ();

			var unassignedTowns = SwissPostZipRepository
				.Current
				.FindAll ()
				.Where (t => t.ZipType != SwissPostZipType.Internal)
				.Where (t => t.Canton == "VD")
				.Select (t => new Town (t.ZipCode, t.LongName))
				.Where (t => !okTowns.Contains (t));

			var noParish = parishes[ParishFileAnalyzer.NoParish];

			noParish.Towns.AddRange (unassignedTowns);
		}


		private static void Write(IEnumerable<Parish> parishes, FileInfo output)
		{
			var lines = parishes
				// This way we have the no parish parish first.
				.OrderBy (p => p.Name == ParishFileAnalyzer.NoParish ? " " : p.Name)
				.SelectMany (p => p.GetLines ());

			File.WriteAllLines (output.FullName, lines);
		}


		private sealed class Parish
		{
			public Parish(string name)
			{
				this.Name = name;
				this.Towns = new List<Town> ();
				this.Streets = new List<Street> ();
			}

			public IEnumerable<string> GetLines()
			{
				yield return this.Name;

				var townLines = this.Towns.Select (t => t.GetLine ());
				var streetLines = this.Streets.Select (s => s.GetLine ());
				var allLines = townLines.Concat (streetLines).OrderBy (l => l);

				foreach (var line in allLines)
				{
					yield return line;
				}

				yield return "";
			}

			public readonly string Name;
			public readonly IList<Town> Towns;
			public readonly IList<Street> Streets;
		}


		private sealed class Town
		{
			public Town(int zip, string name)
			{
				this.Zip = zip;
				this.Name = name;
			}

			public string GetLine()
			{
				return this.Zip + "\t" + this.Name;
			}

			public override bool Equals(object obj)
			{
				var other = obj as Town;

				return other != null
					&& other.Zip == this.Zip
					&& other.Name == this.Name;
			}

			public override int GetHashCode()
			{
				return this.Zip ^ this.Name.GetHashCode ();
			}

			public readonly int Zip;
			public readonly string Name;
		}


		private sealed class Street
		{
			public Street(Town town, string name, IEnumerable<int> numbers)
			{
				this.Town = town;
				this.Name = name;
				this.Numbers = numbers.OrderBy (n => n).ToList ().AsReadOnly ();
			}

			public string GetLine()
			{
				return this.Town.GetLine () + "\t" + this.Name + "\t" + this.GetNumbers ();
			}

			public string GetNumbers()
			{
				var odd = this.Numbers.Where (n => n % 2 == 1).ToList ();
				var even = this.Numbers.Where (n => n % 2 == 0).ToList ();

				var oddRanges = Street.GetRanges (odd);
				var evenRanges = Street.GetRanges (even);

				var data = string.Join (",", oddRanges.Concat (evenRanges));

				return data;
			}

			private static IEnumerable<string> GetRanges(IEnumerable<int> numbers)
			{
				int? lower = null;
				int? upper = null;

				foreach (var number in numbers)
				{
					if (lower == null)
					{
						lower = number;
						upper = number;
					}
					else if (upper + 2 < number)
					{
						yield return Street.GetRange (lower.Value, upper.Value);

						lower = number;
						upper = number;
					}
					else
					{
						upper = number;
					}
				}

				if (lower != null)
				{
					yield return Street.GetRange (lower.Value, upper.Value);
				}
			}

			private static string GetRange(int lower, int upper)
			{
				if (lower == upper)
				{
					return lower.ToString ();
				}

				// 401 is the maximum value that we might have here, so we assume that it means "the
				// rest of the street". That's because of the code in ParishAddressInformation. 
				if (upper == 400 || upper == 401)
				{
					return lower + "-";
				}

				return lower + "-" + upper;
			}

			public readonly Town Town;
			public readonly string Name;
			public readonly IList<int> Numbers;
		}


		private static string NoParish = "Aucune paroisse";


	}


}
