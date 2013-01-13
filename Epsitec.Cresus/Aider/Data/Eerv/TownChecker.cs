using Epsitec.Common.Support.Extensions;

using Epsitec.Common.Text;

using Epsitec.Data.Platform;

using System;

using System.Collections.Generic;

using System.Linq;


namespace Epsitec.Aider.Data.Eerv
{


	internal sealed class TownChecker
	{


		public TownChecker()
		{
			this.towns = TownChecker.BuildTowns ();
			this.zipCodeToTown = TownChecker.BuildDictionary (this.towns, t => t.zipCode);
			this.nameToTown = TownChecker.BuildDictionary (this.towns, t => t.name);
			this.normalizedNameToTown = TownChecker.BuildDictionary (this.towns, t => t.normalizedName);
		}


		private static List<Town> BuildTowns()
		{
			var zipWithShortNames = SwissPostZipRepository.Current.FindAll ()
				.Select (e => Tuple.Create (e.ZipCode, e.LongName, e.ShortName));

			var zipWithLongNames = SwissPostZipRepository.Current.FindAll ()
				.Select (e => Tuple.Create (e.ZipCode, e.LongName, e.LongName));

			return zipWithShortNames.Concat (zipWithLongNames)
				.Distinct ()
				.Select (e => new Town (e.Item1.ToString (), e.Item2))
				.ToList ();
		}


		private static Dictionary<TKey, HashSet<TValue>> BuildDictionary<TKey, TValue>(IEnumerable<TValue> sequence, Func<TValue, TKey> keySelector)
		{
			return sequence.GroupBy (keySelector).ToDictionary (g => g.Key, g => g.ToSet ());
		}


		public Tuple<string, string> Validate(string zipCode, string name)
		{
			var town = new Town (zipCode, name);

			var isZipInName = string.IsNullOrEmpty (town.zipCode) 
				&& town.name.Length >= 4
				&& town.name.Substring (0, 4).IsInteger ();

			if (isZipInName)
			{
				town = new Town (town.name.Substring (0, 4), town.name.Substring (4));
			}

			var emptyZipCode = string.IsNullOrEmpty (town.zipCode);
			var emptyName = string.IsNullOrEmpty(town.name);

			if (emptyZipCode && emptyName)
			{
				return Tuple.Create ("", "");
			}

			if (emptyZipCode)
			{
			    return this.FindMatchWithName (town);
			}

			if (town.zipCode.Length != 4 || !town.zipCode.IsInteger ())
			{
				return Tuple.Create (zipCode, name);
			}

			if (emptyName)
			{
				return this.FindMatchWithZipCode (town);
			}

			return this.FindMatchWithZipCodeAndName (town);
		}


		private Tuple<string, string> FindMatchWithName(Town town)
		{
			HashSet<Town> towns;
			Town match = null;
			
			if (this.nameToTown.TryGetValue (town.name, out towns))
			{
				match = towns.First ();
			}
			else if (this.normalizedNameToTown.TryGetValue (town.name, out towns))
			{
				match = towns.First ();
			}

			if (match != null)
			{
				return match.ToTuple ();
			}

			return this.FindFuzzyMatch (town);
		}


		private Tuple<string, string> FindMatchWithZipCode(Town town)
		{
			HashSet<Town> towns;

			if (this.zipCodeToTown.TryGetValue (town.zipCode, out towns))
			{
				return towns.First ().ToTuple ();
			}
			
			return Tuple.Create (town.zipCode, "");
		}


		private Tuple<string, string> FindMatchWithZipCodeAndName(Town town)
		{
			HashSet<Town> towns;

			if (this.zipCodeToTown.TryGetValue (town.zipCode, out towns))
			{
				var match = towns.FirstOrDefault(t=> t.name == town.name);

				if (match == null)
				{
					towns.FirstOrDefault (t => t.normalizedName == town.normalizedName);
				}

				if (match != null)
				{
					return match.ToTuple ();
				}
			}

			return this.FindMatchWithName (town);
		}


		private Tuple<string, string> FindFuzzyMatch(Town town)
		{
			var candidates =
				from candidate in this.towns
				let zipCodeDistance = this.ComputeJaroDistance (town.zipCode, candidate.zipCode)
				let nameDistance = JaroWinkler.ComputeJaroWinklerDistance (town.normalizedName, candidate.normalizedName)
				where this.IsAcceptableMatch (zipCodeDistance, nameDistance)
				orderby this.GetMergedDistance (zipCodeDistance, nameDistance) descending
				select candidate;

			var result = candidates.FirstOrDefault ();

			if (result != null)
			{
				return result.ToTuple ();
			}
			else
			{
				return new Town ("", "").ToTuple ();
			}
		}


		private double ComputeJaroDistance(string s1, string s2)
		{
			// There is a bug in the jaro winkler distance. It should be symmetric but my
			// implementation is not. I tried to correct it but I could not implement it properly.
			// And maybe it is better not to touch it too much becauses if I change it, it will
			// change the behavior of the person matcher.
			// So here I try to mitigate this problem by computing the distance both ways and take
			// their average.

			var d1 = JaroWinkler.ComputeJaroDistance (s1, s2);
			var d2 = JaroWinkler.ComputeJaroDistance (s1, s2);

			return (d1 + d2) / 2;
		}


		private bool IsAcceptableMatch(double zipCodeDistance, double nameDistance)
		{
			return (zipCodeDistance == JaroWinkler.MaxValue && nameDistance > 0.80)
				|| (nameDistance == JaroWinkler.MaxValue && zipCodeDistance > 0.65)
				|| (zipCodeDistance > 0.9 && nameDistance > 0.9)
				|| (zipCodeDistance == JaroWinkler.MinValue && nameDistance > 0.95);
		}


		private double GetMergedDistance(double zipCodeDistance, double nameDistance)
		{
			return zipCodeDistance + nameDistance;
		}


		private readonly List<Town> towns;


		private readonly Dictionary<string, HashSet<Town>> zipCodeToTown;


		private readonly Dictionary<string, HashSet<Town>> nameToTown;


		private readonly Dictionary<string, HashSet<Town>> normalizedNameToTown;


		private sealed class Town
		{


			public Town(string zipCode, string name)
				: this (zipCode, name, name)
			{
			}


			public Town(string zipCode, string name, string nameToNormalize)
			{
				this.zipCode = Town.GetString (zipCode);
				this.name = Town.GetString (name);
				this.normalizedName = Normalizer.NormalizeText (Town.GetString(nameToNormalize));
			}


			private static string GetString(string value)
			{
				return (value ?? "").Trim ();
			}


			public Tuple<string, string> ToTuple()
			{
				return Tuple.Create (this.zipCode, this.name);
			}


			public readonly string zipCode;


			public readonly string name;


			public readonly string normalizedName;


		}


	}


}
