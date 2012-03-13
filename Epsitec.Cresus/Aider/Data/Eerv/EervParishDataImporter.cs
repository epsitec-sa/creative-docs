using Epsitec.Aider.Entities;
using Epsitec.Aider.Enumerations;

using Epsitec.Common.Support;
using Epsitec.Common.Support.Extensions;

using Epsitec.Common.Types;

using Epsitec.Cresus.Core.Business;

using Epsitec.Data.Platform;

using System;

using System.Collections.Generic;


using System.Linq;


namespace Epsitec.Aider.Data.Eerv
{


	internal static class EervParishDataImporter
	{


		public static void Import(Func<BusinessContext> businessContextCreator, Action<BusinessContext> businessContextCleaner, IEnumerable<EervPerson> eervPersons, IEnumerable<EervHousehold> eervHouseholds)
		{
			BusinessContext businessContext = null;
			
			try
			{
				businessContext = businessContextCreator ();

				// Filters the legal persons. We don't match them here.
				eervPersons = eervPersons.Where (p => string.IsNullOrWhiteSpace (p.CorporateName));

				var databasePersons = businessContext.GetAllEntities<AiderPersonEntity> ();
				
				// Fetches all this stuff in memory at once, so we don't make gazillions of requests
				// to the database.
				businessContext.GetAllEntities<eCH_PersonEntity> ();
				businessContext.GetAllEntities<eCH_AddressEntity> ();
				businessContext.GetAllEntities<eCH_ReportedPersonEntity> ();
				
				var matches = EervParishDataImporter.FindMatches (eervPersons, eervHouseholds, databasePersons);

				foreach (var match in matches)
				{
					EervParishDataImporter.ProcessMatch (businessContext, match);
				}
			}
			finally
			{
				if (businessContext != null)
				{
					businessContext.SaveChanges ();
					businessContextCleaner (businessContext);
					businessContext.Dispose ();
				}
			}
		}


		private static Dictionary<EervPerson, List<Tuple<AiderPersonEntity, MatchData>>> FindMatches(IEnumerable<EervPerson> eervPersons, IEnumerable<EervHousehold> eervHouseholds, IEnumerable<AiderPersonEntity> databasePersons)
		{
			var normalizedDatabasePersons = databasePersons.ToDictionary (p => EervParishDataImporter.Normalize (p));
			
			var idToEervHouseholds = eervHouseholds.ToDictionary (h => h.Id);

			var normalizedEervPersons = eervPersons
				.Select (p => Tuple.Create (p, idToEervHouseholds[p.HouseholdId]))
				.ToDictionary (p => EervParishDataImporter.Normalize (p.Item1, p.Item2));

			var matches = EervParishDataImporter.FindMatches (normalizedEervPersons.Keys, normalizedDatabasePersons.Keys);

			return matches.ToDictionary
			(
				kvp => normalizedEervPersons[kvp.Key].Item1,
				kvp => kvp.Value.Select (m => Tuple.Create (normalizedDatabasePersons[m.Item1], m.Item2)).ToList ()
			);
		}


		private static Dictionary<NormalizedPerson, List<Tuple<NormalizedPerson, MatchData>>> FindMatches(IEnumerable<NormalizedPerson> eervPersons, IEnumerable<NormalizedPerson> databasePersons)
		{
			var lastnamesToDatabasePersons = EervParishDataImporter.GroupPersonsByLastnames (databasePersons);

			return eervPersons.ToDictionary
			(
				p => p,
				p => EervParishDataImporter.FindMatches (p, databasePersons, lastnamesToDatabasePersons)
			);
		}


		private static Dictionary<string[], List<NormalizedPerson>> GroupPersonsByLastnames(IEnumerable<NormalizedPerson> persons)
		{
			var comparer = ArrayEqualityComparer<string>.Instance;

			return persons
				.GroupBy (p => p.Lastnames, comparer)
				.ToDictionary (g => g.Key, g => g.ToList (), comparer);
		}

		private static List<Tuple<NormalizedPerson, MatchData>> FindMatches(NormalizedPerson eervPerson, IEnumerable<NormalizedPerson> databasePersons, Dictionary<string[], List<NormalizedPerson>> lastNamesToDatabasePersons)
		{
			Func<string[], List<Tuple<NormalizedPerson, MatchData>>> fullLastnameMatcher = null;
			Func<string[], List<Tuple<NormalizedPerson, MatchData>>> orderedPartialLastnameMatcher = null;
			Func<string[], List<Tuple<NormalizedPerson, MatchData>>> partialLastnameMatcher = null;

			fullLastnameMatcher = ln => EervParishDataImporter.FindFullLastnameMatches (ln, lastNamesToDatabasePersons);
			orderedPartialLastnameMatcher = ln => EervParishDataImporter.FindOrderedPartialLastnameMatches (ln, databasePersons);
			partialLastnameMatcher = ln => EervParishDataImporter.FindPartialLastnameMatches (ln, databasePersons);

			var matches = EervParishDataImporter.FindMatches (eervPerson, fullLastnameMatcher)
				?? EervParishDataImporter.FindMatches (eervPerson, orderedPartialLastnameMatcher)
				?? EervParishDataImporter.FindMatches (eervPerson, partialLastnameMatcher)
				?? new List<Tuple<NormalizedPerson, MatchData>> (0);

			matches = EervParishDataImporter.TakeAddressIntoAccount (eervPerson, matches);
			matches = EervParishDataImporter.TakeFirstFirstnameIntoAccount (eervPerson, matches);

			return matches;
		}


		private static List<Tuple<NormalizedPerson, MatchData>> TakeAddressIntoAccount(NormalizedPerson eervPerson, List<Tuple<NormalizedPerson, MatchData>> matches)
		{
			foreach (var match in matches)
			{
				var addressMatch = EervParishDataImporter.AreAddresseMatches (eervPerson.Address, match.Item1.Address);

				match.Item2.Address = addressMatch;
			}

			if (matches.Count > 1)
			{
				var matchesWithSameAddresses = matches
					.Where (m => m.Item2.Address != AddressMatch.None)
					.ToList ();

				if (matchesWithSameAddresses.Count > 0)
				{
					return matchesWithSameAddresses;
				}
			}

			return matches;
		}


		private static List<Tuple<NormalizedPerson, MatchData>> TakeFirstFirstnameIntoAccount(NormalizedPerson eervPerson, List<Tuple<NormalizedPerson, MatchData>> matches)
		{
			if (matches.Count > 1)
			{
				var matchesWithSameFirstFirstnames = matches
					.Where (m => m.Item1.Firstnames[0] == eervPerson.Firstnames[0])
					.ToList ();

				if (matchesWithSameFirstFirstnames.Count > 0)
				{
					return matchesWithSameFirstFirstnames;
				}
			}

			return matches;
		}


		private static List<Tuple<NormalizedPerson, MatchData>> FindMatches(NormalizedPerson eervPerson, Func<string[], List<Tuple<NormalizedPerson, MatchData>>> lastnameMatcher)
		{
			var lastnames = eervPerson.Lastnames;
			var lastnameMatches = lastnameMatcher (lastnames);

			lastnameMatches = EervParishDataImporter.FilterByDateOfBirthAndSex (eervPerson, lastnameMatches);

			var firstnames = eervPerson.Firstnames;

			return EervParishDataImporter.FindFullFirstnameMatches (firstnames, lastnameMatches)
				?? EervParishDataImporter.FindOrderedPartialFirstnameMatches (firstnames, lastnameMatches)
				?? EervParishDataImporter.FindPartialFirstnameMatches (firstnames, lastnameMatches);
		}


		private static List<Tuple<NormalizedPerson, MatchData>> FindFullLastnameMatches(string[] lastnames, Dictionary<string[], List<NormalizedPerson>> lastNamesToDatabasePersons)
		{
			List<NormalizedPerson> matches;

			var isMatch = lastNamesToDatabasePersons.TryGetValue (lastnames, out matches);

			if (!isMatch)
			{
				matches = new List<NormalizedPerson> ();
			}

			return matches
				.Select (m => Tuple.Create (m, new MatchData () { Lastname = NameMatch.Full } ))
				.ToList();
		}


		private static List<Tuple<NormalizedPerson, MatchData>> FindOrderedPartialLastnameMatches(string[] lastnames, IEnumerable<NormalizedPerson> databasePersons)
		{
			Func<string[], string[], bool> predicate = EervParishDataImporter.IsOrderedSubset;
			var match = NameMatch.OrderedPartial;

			return EervParishDataImporter.FindLastnameMatches (lastnames, databasePersons, predicate, match);
		}


		private static List<Tuple<NormalizedPerson, MatchData>> FindPartialLastnameMatches(string[] lastnames, IEnumerable<NormalizedPerson> databasePersons)
		{
			Func<string[], string[], bool> predicate = EervParishDataImporter.IsSubset;
			var match = NameMatch.Partial;

			return EervParishDataImporter.FindLastnameMatches (lastnames, databasePersons, predicate, match);
		}


		private static List<Tuple<NormalizedPerson, MatchData>> FindLastnameMatches(string[] lastnames, IEnumerable<NormalizedPerson> databasePersons, Func<string[], string[], bool> predicate, NameMatch match)
		{
			var result = databasePersons
				.Where (p => predicate (lastnames, p.Lastnames))
				.Select (m => Tuple.Create (m, new MatchData ()))
				.ToList ();

			foreach (var r in result)
			{
				r.Item2.Lastname = match;
			}

			return result;
		}


		private static List<Tuple<NormalizedPerson, MatchData>> FindFullFirstnameMatches(string[] firstnames, IEnumerable<Tuple<NormalizedPerson, MatchData>> databasePersons)
		{
			Func<string[], string[], bool> predicate = ArrayEqualityComparer<string>.Instance.Equals;
			var match = NameMatch.Full;

			return EervParishDataImporter.FindFirstnameMatches (firstnames, databasePersons, predicate, match);
		}


		private static List<Tuple<NormalizedPerson, MatchData>> FindOrderedPartialFirstnameMatches(string[] firstnames, IEnumerable<Tuple<NormalizedPerson, MatchData>> databasePersons)
		{
			Func<string[], string[], bool> predicate = EervParishDataImporter.IsOrderedSubset;
			var match = NameMatch.OrderedPartial;

			return EervParishDataImporter.FindFirstnameMatches (firstnames, databasePersons, predicate, match);
		}


		private static List<Tuple<NormalizedPerson, MatchData>> FindPartialFirstnameMatches(string[] firstnames, IEnumerable<Tuple<NormalizedPerson, MatchData>> databasePersons)
		{
			Func<string[], string[], bool> predicate = EervParishDataImporter.IsSubset;
			var match = NameMatch.Partial;

			return EervParishDataImporter.FindFirstnameMatches (firstnames, databasePersons, predicate, match);
		}


		private static List<Tuple<NormalizedPerson, MatchData>> FindFirstnameMatches(string[] firstnames, IEnumerable<Tuple<NormalizedPerson, MatchData>> databasePersons, Func<string[], string[], bool> predicate, NameMatch match)
		{
			var result = databasePersons
				.Where (p => predicate(firstnames, p.Item1.Firstnames))
				.ToList ();

			foreach (var r in result)
			{
				r.Item2.Firstname = match;
			}

			if (result.Count == 0)
			{
				result = null;
			}

			return result;
		}


		private static List<Tuple<NormalizedPerson, MatchData>> FilterByDateOfBirthAndSex(NormalizedPerson eervPerson, IEnumerable<Tuple<NormalizedPerson, MatchData>> databasePersons)
		{
			var filtered = databasePersons;
			
			filtered = EervParishDataImporter.FilterByDateOfBirth (eervPerson, databasePersons);
			filtered = EervParishDataImporter.FilterBySex (eervPerson, filtered);

			return filtered.ToList ();
		}


		private static IEnumerable<Tuple<NormalizedPerson, MatchData>> FilterByDateOfBirth(NormalizedPerson eervPerson, IEnumerable<Tuple<NormalizedPerson, MatchData>> databasePersons)
		{
			var date1 = eervPerson.DateOfBirth;
			
			foreach (var databasePerson in databasePersons)
			{
				var date2 = databasePerson.Item1.DateOfBirth;
				var match = databasePerson.Item2;

				if (!date1.HasValue || !date2.HasValue)
				{
					match.SameDateOfBirth = null;

					yield return databasePerson;
				}
				else if (date1 == date2)
				{
					match.SameDateOfBirth = true;

					yield return databasePerson;
				}
			}
		}


		private static IEnumerable<Tuple<NormalizedPerson, MatchData>> FilterBySex(NormalizedPerson eervPerson, IEnumerable<Tuple<NormalizedPerson, MatchData>> databasePersons)
		{
			var sex1 = eervPerson.Sex;

			foreach (var databasePerson in databasePersons)
			{
				var sex2 = databasePerson.Item1.Sex;
				var match = databasePerson.Item2;

				if (sex1 == PersonSex.Unknown || sex2 == PersonSex.Unknown)
				{
					match.SameSex = null;

					yield return databasePerson;
				}
				else if (sex1 == sex2)
				{
					match.SameSex = true;

					yield return databasePerson;
				}
			}
		}


		private static bool IsOrderedSubset(string[] a, string[] b)
		{
			if (a.Length > b.Length)
			{
				return false;
			}

			int nbMatches = 0;

			for (int i = 0, j = 0; i < a.Length && j < b.Length; i++)
			{
				var matched = false;

				for (; !matched && j < b.Length; j++)
				{
					matched = a[i] == b[j];
				}

				if (matched)
				{
					nbMatches++;
				}
			}

			return nbMatches == a.Length;
		}


		private static bool IsSubset(string[] a, string[] b)
		{
			return !a.Except (b).Any ();
		}


		private static AddressMatch AreAddresseMatches(NormalizedAddress a, NormalizedAddress b)
		{
			var sameTown = a.Town == b.Town;
			var sameZipCode = a.ZipCode == b.ZipCode;

			if (!sameTown && !sameZipCode)
			{
				return AddressMatch.None;
			}

			var sameStreet = a.Street == b.Street;

			if (!sameStreet)
			{
				return AddressMatch.ZipCity;
			}

			var sameHouseNumber = a.HouseNumber == b.HouseNumber;

			if (!sameHouseNumber)
			{
				return AddressMatch.StreetZipCity;
			}

			return AddressMatch.Full;
		}


		private static NormalizedPerson Normalize(EervPerson person, EervHousehold household)
		{
			return new NormalizedPerson ()
			{
				Firstnames = EervParishDataImporter.NormalizeComposedName (person.Firstnames),
				Lastnames = EervParishDataImporter.NormalizeComposedName (person.Lastname),
				DateOfBirth = person.DateOfBirth,
				Sex = person.Sex,
				Origins = person.Origins,
				Address = EervParishDataImporter.Normalize (household),
			};
		}


		private static NormalizedAddress Normalize(EervHousehold household)
		{
			return new NormalizedAddress ()
			{
				Street = SwissPostStreet.NormalizeStreetName (household.StreetName),
				HouseNumber = SwissPostStreet.NormalizeHouseNumber ((household.HouseNumber ?? 0).ToString ()),
				ZipCode = InvariantConverter.ParseInt (household.ZipCode),
				Town = EervParishDataImporter.NormalizeTown (household.City),
			};
		}


		private static NormalizedPerson Normalize(AiderPersonEntity person)
		{
			var echPerson = person.eCH_Person;

			return new NormalizedPerson ()
			{
				Firstnames = EervParishDataImporter.NormalizeComposedName (echPerson.PersonFirstNames),
				Lastnames = EervParishDataImporter.NormalizeComposedName (echPerson.PersonOfficialName),
				DateOfBirth = echPerson.PersonDateOfBirth,
				Sex = echPerson.PersonSex,
				Origins = echPerson.Origins,
				Address = EervParishDataImporter.Normalize (echPerson.Address1),
			};
		}


		private static NormalizedAddress Normalize(eCH_AddressEntity address)
		{
			return new NormalizedAddress ()
			{
				Street = SwissPostStreet.NormalizeStreetName (address.Street),
				HouseNumber = SwissPostStreet.StripAndNormalizeHouseNumber (address.HouseNumber),
				ZipCode = address.SwissZipCode,
				Town = EervParishDataImporter.NormalizeTown (address.Town),
			};
		}
		

		private static string[] NormalizeComposedName(string names)
		{
			return EervParishDataImporter.Normalize (names).Replace ('-', ' ').Split (new char[] { ' ' });
		}


		private static string Normalize(string data)
		{
			return StringUtils.RemoveDiacritics (data).ToLowerInvariant ();
		}


		private static string NormalizeTown(string data)
		{
			var tmp = data.Split (new char[] { ' ', '-', }).Join (" ");

			return EervParishDataImporter.Normalize (tmp);
		}


		private static void ProcessMatch(BusinessContext businessContext, KeyValuePair<EervPerson, List<Tuple<AiderPersonEntity, MatchData>>> match)
		{
			// TODO
		}


	}


}
