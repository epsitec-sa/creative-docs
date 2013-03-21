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
			this.manualCorrections = TownChecker.BuildManualCorrection ();
			this.towns = TownChecker.BuildTowns ();
			this.zipCodeToTown = TownChecker.BuildDictionary (this.towns, t => t.zipCode);
			this.nameToTown = TownChecker.BuildDictionary (this.towns, t => t.name);
			this.normalizedNameToTown = TownChecker.BuildDictionary (this.towns, t => t.normalizedName);
		}


		private static List<Town> BuildTowns()
		{
			var zipWithShortNames = TownChecker.GetTowns ()
				.Select (e => Tuple.Create (e.ZipCode, e.LongName, e.ShortName));

			var zipWithLongNames = TownChecker.GetTowns ()
				.Select (e => Tuple.Create (e.ZipCode, e.LongName, e.LongName));

			return zipWithShortNames.Concat (zipWithLongNames)
				.Distinct ()
				.Select (e => new Town (e.Item1.ToString (), e.Item2, e.Item3))
				.ToList ();
		}


		private static IEnumerable<SwissPostZipInformation> GetTowns()
		{
			return SwissPostZipRepository
				.Current
				.FindAll ()
				.Where (t => t.ZipType != SwissPostZipType.Internal);
		}


		private static Dictionary<Tuple<string, string>, Tuple<string, string>> BuildManualCorrection()
		{
			var corrections = new Dictionary<Tuple<string, string>, Tuple<string, string>> ();

			// These are manual corrections that the generic algorithm can't find. There are several
			// different kind of corrections.
			// 1) When the correct name is more different than another, like for Yverdon for which
			//    the correction is Yverdon-les-Bains but for which the closest match would be
			//    Yverdon 2.
			// 2) Old towns that are not town anymore, like for Champmartin or Verschiez
			// 3) Language problems, like for Morat whose real name is Murten.
			// 4) Cases where the address is is completely wrong.

			TownChecker.Add (corrections, "1400", "yverdon", "1400", "Yverdon-les-Bains");
			TownChecker.Add (corrections, "1588", "champmartin", "1588", "Cudrefin");
			TownChecker.Add (corrections, "1867", "verschiez", "1867", "Ollon VD");
			TownChecker.Add (corrections, "1867", "les combes", "1867", "Ollon VD");
			TownChecker.Add (corrections, "1422", "les tuileries", "1422", "Grandson");
			TownChecker.Add (corrections, "1588", "montet", "1588", "Cudrefin");
			TownChecker.Add (corrections, "1258", "certoux", "1258", "Perly");
			TownChecker.Add (corrections, "1261", "st george", "1188", "St-George");
			TownChecker.Add (corrections, "3280", "morat", "3280", "Murten");
			TownChecker.Add (corrections, "1569", "atavaux", "1475", "Autavaux");
			TownChecker.Add (corrections, "2501", "bienne", "2501", "Biel/Bienne");
			TownChecker.Add (corrections, "2503", "bienne", "2503", "Biel/Bienne");
			TownChecker.Add (corrections, "2504", "bienne", "2504", "Biel/Bienne");
			TownChecker.Add (corrections, "1053", "montheron", "1053", "Cugy VD");
			TownChecker.Add (corrections, "1890", "epinassey", "1890", "St-Maurice");
			TownChecker.Add (corrections, "1867", "villy ollon vd", "1867", "Ollon VD");
			TownChecker.Add (corrections, "1200", "geneve 3", "1211", "Genève 3");
			TownChecker.Add (corrections, "1228", "geneve", "1228", "Plan-les-Ouates");
			TownChecker.Add (corrections, "1200", "geneve 7", "1211", "Genève 7");
			TownChecker.Add (corrections, "3000", "berne 7", "3000", "Bern 7 Bärenplatz");
			TownChecker.Add (corrections, "1000", "lausanne 2", "1002", "Lausanne");
			TownChecker.Add (corrections, "1001", "lausanne 22", "1000", "Lausanne 22");
			TownChecker.Add (corrections, "1010", "lausanne 10", "1000", "Lausanne 10");
			TownChecker.Add (corrections, "1000", "lausanne 17", "1017", "Lausanne Charles Veillon SA");
			TownChecker.Add (corrections, "1000", "lausanne 4", "1000", "Lausanne");

			return corrections;
		}


		private static void Add(Dictionary<Tuple<string, string>, Tuple<string, string>> corrections, string zip1, string name1, string zip2, string name2)
		{
			var key = Tuple.Create (zip1, name1);
			var value = Tuple.Create (zip2, name2);

			corrections[key] = value;
		}


		private static Dictionary<TKey, HashSet<TValue>> BuildDictionary<TKey, TValue>(IEnumerable<TValue> sequence, Func<TValue, TKey> keySelector)
		{
			return sequence.GroupBy (keySelector).ToDictionary (g => g.Key, g => g.ToSet ());
		}


		/// <summary>
		/// This method corrects invalid zip codes and names by using a fuzzy algorithm. It works
		/// only for swiss towns, so don't use it for towns that are in another country as it will
		/// not work.
		/// </summary>
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

			// The "Saint" or "Sainte" prefix are always abbreviated to "St" or "Ste" in the swiss
			// town names.
			if (town.normalizedName.StartsWith ("saint"))
			{
				var newNormalizedName = "st" + town.normalizedName.Substring (5);

				town = new Town (town.zipCode, town.name, newNormalizedName);
			}

			var manualCorrection = this.FindManualCorrection (town);

			if (manualCorrection != null)
			{
				town = new Town (manualCorrection.Item1, manualCorrection.Item2);
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

			if (emptyName)
			{
				return this.FindMatchWithZipCode (town);
			}

			return this.FindMatchWithZipCodeAndName (town);
		}


		private Tuple<string, string> FindManualCorrection(Town town)
		{
			var key = Tuple.Create (town.zipCode, town.normalizedName);

			Tuple<string, string> correction;

			if (this.manualCorrections.TryGetValue (key, out correction))
			{
				return correction;
			}

			return null;
		}


		private Tuple<string, string> FindMatchWithName(Town town)
		{
			HashSet<Town> towns;
			Town match = null;
			
			if (this.nameToTown.TryGetValue (town.name, out towns))
			{
				match = towns.First ();
			}
			else if (this.normalizedNameToTown.TryGetValue (town.normalizedName, out towns))
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
					match = towns.FirstOrDefault (t => t.normalizedName == town.normalizedName);
				}

				if (match != null)
				{
					return match.ToTuple ();
				}
			}

			return this.FindFuzzyMatch (town);
		}


		private Tuple<string, string> FindFuzzyMatch(Town town)
		{
			var candidates =
				from candidate in this.towns
				let zipCodeDistance = JaroWinkler.ComputeJaroDistance (town.zipCode, candidate.zipCode)
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


		private bool IsAcceptableMatch(double zipCodeDistance, double nameDistance)
		{
			return (zipCodeDistance == JaroWinkler.MaxValue && nameDistance > 0.80)
				|| (nameDistance == JaroWinkler.MaxValue && zipCodeDistance > 0.65)
				|| (zipCodeDistance > 0.8 && nameDistance > 0.9)
				|| (zipCodeDistance == JaroWinkler.MinValue && nameDistance > 0.95);
		}


		private double GetMergedDistance(double zipCodeDistance, double nameDistance)
		{
			return zipCodeDistance + nameDistance;
		}


		private readonly Dictionary<Tuple<string, string>, Tuple<string, string>> manualCorrections;


		private readonly List<Town> towns;


		private readonly Dictionary<string, HashSet<Town>> zipCodeToTown;


		private readonly Dictionary<string, HashSet<Town>> nameToTown;


		private readonly Dictionary<string, HashSet<Town>> normalizedNameToTown;


		private sealed class Town
		{


			public Town(string zipCode, string name)
				: this(zipCode, name, name)
			{
			}


			public Town(string zipCode, string name, string nameToNormalize)
			{
				this.zipCode = Town.GetString (zipCode);
				this.name = Town.GetString (name);
				this.normalizedName = Town.GetString (Normalizer.NormalizeText (nameToNormalize));
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
