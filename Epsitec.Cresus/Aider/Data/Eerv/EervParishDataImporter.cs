using Epsitec.Aider.Entities;
using Epsitec.Aider.Enumerations;

using Epsitec.Common.Support;
using Epsitec.Common.Support.Extensions;
using Epsitec.Common.Types;

using Epsitec.Cresus.Core;
using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.DataLayer.Context;

using Epsitec.Data.Platform;

using Epsitec.TwixClip;

using System;

using System.Collections.Generic;

using System.Linq;


namespace Epsitec.Aider.Data.Eerv
{


	internal static class EervParishDataImporter
	{


		public static void Import(Func<BusinessContext> businessContextCreator, Action<BusinessContext> businessContextCleaner, string parishName, IEnumerable<EervPerson> eervPersons, IEnumerable<EervHousehold> eervHouseholds)
		{
			// TODO Import group & activity data.
			// TODO Import household data (match and merge or create if no match)
		
			var eervPhysicalPersons = eervPersons.Where (p => string.IsNullOrWhiteSpace (p.CorporateName));
			EervParishDataImporter.ImportEervPhysicalPersons (businessContextCreator, businessContextCleaner, parishName, eervPhysicalPersons, eervHouseholds);
		
			var eervLegalPersons = eervPersons.Where (p => !string.IsNullOrWhiteSpace (p.CorporateName));
			EervParishDataImporter.ImportEervLegalPersons (businessContextCreator, businessContextCleaner, eervLegalPersons);
		}


		private static void ImportEervPhysicalPersons(Func<BusinessContext> businessContextCreator, Action<BusinessContext> businessContextCleaner, string parishName, IEnumerable<EervPerson> eervPersons, IEnumerable<EervHousehold> eervHouseholds)
		{
			var matches = EervParishDataImporter.FindMatches (businessContextCreator, businessContextCleaner, eervPersons, eervHouseholds);

			EervParishDataImporter.ProcessMatches (businessContextCreator, businessContextCleaner, parishName, matches);
		}


		private static void ImportEervLegalPersons(Func<BusinessContext> businessContextCreator, Action<BusinessContext> businessContextCleaner, IEnumerable<EervPerson> eervPersons)
		{
			// TODO Import legal persons.
		}


		private static Dictionary<EervPerson, List<Tuple<EntityKey, MatchData>>> FindMatches(Func<BusinessContext> businessContextCreator, Action<BusinessContext> businessContextCleaner, IEnumerable<EervPerson> eervPersons, IEnumerable<EervHousehold> eervHouseholds)
		{
			using (BusinessContext businessContext = businessContextCreator ())
			{
				try
				{
					return EervParishDataImporter.FindMatches (businessContext, eervPersons, eervHouseholds);
				}
				finally
				{
					if (businessContext != null)
					{
						businessContextCleaner (businessContext);
					}
				}
			}
		}

		
		private static Dictionary<EervPerson, List<Tuple<EntityKey, MatchData>>> FindMatches(BusinessContext businessContext, IEnumerable<EervPerson> eervPersons, IEnumerable<EervHousehold> eervHouseholds)
		{
			var databasePersons = businessContext.GetAllEntities<AiderPersonEntity> ();

			// NOTE Here we fetch all this stuff in memory at once, so we don't make gazillions of
			// requests to the database later, by fetching them one by one.
			businessContext.GetAllEntities<eCH_PersonEntity> ();
			businessContext.GetAllEntities<eCH_AddressEntity> ();
			businessContext.GetAllEntities<eCH_ReportedPersonEntity> ();

			var dataContext = businessContext.DataContext;

			var matches = EervParishDataImporter.FindMatches (eervPersons, eervHouseholds, databasePersons);

			return matches.ToDictionary
			(
				kvp => kvp.Key,
				kvp => kvp.Value.Select (m => Tuple.Create (dataContext.GetNormalizedEntityKey (m.Item1).Value, m.Item2)).ToList ()
			);
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
					match.DateOfBirth = null;

					yield return databasePerson;
				}
				else if (date1 == date2)
				{
					match.DateOfBirth = true;

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
					match.Sex = null;

					yield return databasePerson;
				}
				else if (sex1 == sex2)
				{
					match.Sex = true;

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
			var eChPerson = person.eCH_Person;

			return new NormalizedPerson ()
			{
				Firstnames = EervParishDataImporter.NormalizeComposedName (eChPerson.PersonFirstNames),
				Lastnames = EervParishDataImporter.NormalizeComposedName (eChPerson.PersonOfficialName),
				DateOfBirth = eChPerson.PersonDateOfBirth,
				Sex = eChPerson.PersonSex,
				Origins = eChPerson.Origins,
				Address = EervParishDataImporter.Normalize (eChPerson.Address1),
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


		private static void ProcessMatches(Func<BusinessContext> businessContextCreator, Action<BusinessContext> businessContextCleaner, string parishName, Dictionary<EervPerson, List<Tuple<EntityKey, MatchData>>> matches)
		{
			using (BusinessContext businessContext = businessContextCreator ())
			{
				try
				{
					EervParishDataImporter.ProcessMatches (businessContext, parishName, matches);
				}
				finally
				{
					if (businessContext != null)
					{
						businessContext.SaveChanges ();
						businessContextCleaner (businessContext);
					}
				}
			}
		}


		private static void ProcessMatches(BusinessContext businessContext, string parishName, Dictionary<EervPerson, List<Tuple<EntityKey, MatchData>>> matches)
		{
			var dataContext = businessContext.DataContext;

			foreach (var match in matches)
			{
				var eervPerson = match.Key;

				if (match.Value.Any ())
				{
					foreach (var m in match.Value)
					{
						var aiderPerson = (AiderPersonEntity) dataContext.ResolveEntity (m.Item1);
						var matchData = m.Item2;

						EervParishDataImporter.CombineAiderPersonWithEervPerson (businessContext, eervPerson, aiderPerson);
						EervParishDataImporter.AddMatchComment (eervPerson, aiderPerson, matchData, parishName);
					}
				}
				else
				{
					var aiderPerson = EervParishDataImporter.CreateAiderPersonWithEervPerson (businessContext, eervPerson);

					EervParishDataImporter.CombineAiderPersonWithEervPerson (businessContext, eervPerson, aiderPerson);
					EervParishDataImporter.AddMatchComment (eervPerson, aiderPerson, null, parishName);
				}
			}
		}


		private static void CombineAiderPersonWithEervPerson(BusinessContext businessContext, EervPerson eervPerson, AiderPersonEntity aiderPerson)
		{
			// TODO ADD PlaceOfBirth ?
			// TODO ADD PlaceOfBaptism ?
			// TODO ADD DateOfBaptism ?
			// TODO ADD PlaceOfChildBenediction ?
			// TODO ADD DateOfChildBenediction ?
			// TODO ADD PlaceOfCatechismBenediction ?
			// TODO ADD DateOfCatechismBenediction ?
			// TODO ADD SchoolYearOffset ?

			EervParishDataImporter.CombineOriginalName (eervPerson, aiderPerson);
			EervParishDataImporter.CombineHonorific (eervPerson, aiderPerson);
			EervParishDataImporter.CombineProfession (eervPerson, aiderPerson);
			EervParishDataImporter.CombineDateOfDeath (eervPerson, aiderPerson);
			EervParishDataImporter.CombineConfession (eervPerson, aiderPerson);
			EervParishDataImporter.CombineRemarks (eervPerson, aiderPerson);
			EervParishDataImporter.CombineCoordinates (businessContext, eervPerson, aiderPerson);
		}


		private static void CombineOriginalName(EervPerson eervPerson, AiderPersonEntity aiderPerson)
		{
			var originalName = eervPerson.OriginalName;

			if (!string.IsNullOrEmpty (originalName))
			{
				aiderPerson.OriginalName = originalName;
			}
		}


		private static void CombineHonorific(EervPerson eervPerson, AiderPersonEntity aiderPerson)
		{
			var honorific = eervPerson.Honorific;

			if (!string.IsNullOrEmpty (honorific))
			{
				switch (honorific)
				{
					case "Monsieur":
						aiderPerson.MrMrs = PersonMrMrs.Monsieur;
						break;

					case "Madame":
						aiderPerson.MrMrs = PersonMrMrs.Madame;
						break;

					case "Mademoiselle":
						aiderPerson.MrMrs = PersonMrMrs.Mademoiselle;
						break;

					default:
						aiderPerson.Title = honorific;
						break;
				}
			}
		}


		private static void CombineProfession(EervPerson eervPerson, AiderPersonEntity aiderPerson)
		{
			var profession = eervPerson.Profession;

			if (!string.IsNullOrEmpty (profession))
			{
				aiderPerson.Profession = profession;
			}
		}


		private static void CombineDateOfDeath(EervPerson eervPerson, AiderPersonEntity aiderPerson)
		{
			var dateOfDeath = eervPerson.DateOfDeath;

			if (dateOfDeath.HasValue)
			{
				aiderPerson.eCH_Person.PersonDateOfDeath = dateOfDeath;
			}
		}


		private static void CombineConfession(EervPerson eervPerson, AiderPersonEntity aiderPerson)
		{
			var confession = eervPerson.Confession;

			if (confession != PersonConfession.Unknown)
			{
				aiderPerson.Confession = confession;
			}
		}


		private static void CombineRemarks(EervPerson eervPerson, AiderPersonEntity aiderPerson)
		{
			var remarks = eervPerson.Remarks;

			if (!string.IsNullOrEmpty (remarks))
			{
				EervParishDataImporter.CombineComments (aiderPerson, remarks);
			}
		}


		private static void CombineCoordinates(BusinessContext businessContext, EervPerson eervPerson, AiderPersonEntity aiderPerson)
		{
			var email = eervPerson.EmailAddress;
			var mobile = eervPerson.MobilePhoneNumber;

			var hasEmail = !string.IsNullOrEmpty (email);
			var hasMobile = !string.IsNullOrEmpty (mobile);

			if (hasEmail || hasMobile)
			{
				// NOTE Here we need to check this because the list is implemented by 4 fields and
				// we don't want to skip the address in this case. It will probably never happen,
				// but in case it does, it won't go unnoticed.
				if (aiderPerson.AdditionalAddresses.Count >= 4)
				{
					throw new InvalidOperationException ();
				}

				var address = businessContext.CreateEntity<AiderAddressEntity> ();

				EervParishDataImporter.AddEmail (address, email);
				EervParishDataImporter.AddMobilePhoneNumber (address, mobile);

				aiderPerson.AdditionalAddresses.Add (address);
			}
		}


		private static void AddEmail(AiderAddressEntity address, string email)
		{
			if (!string.IsNullOrEmpty (email))
			{
				address.Email = email;
			}
		}


		private static void AddMobilePhoneNumber(AiderAddressEntity address, string mobilePhoneNumber)
		{
			if (!string.IsNullOrEmpty (mobilePhoneNumber))
			{
				var parsedNumber = TwixTel.ParsePhoneNumber (mobilePhoneNumber);

				if (TwixTel.IsValidPhoneNumber (parsedNumber, false))
				{
					address.Mobile = parsedNumber;
				}
				else
				{
					var text = "Téléphone invalide ou non reconnu par le système : " + mobilePhoneNumber;
					
					EervParishDataImporter.CombineComments (address, text);
				}
			}
		}


		private static void CombineComments(IComment entity, string text)
		{
			// With the null reference virtualizer, we don't need to handle explicitely the case
			// when there is no comment defined yet.

			var comment = entity.Comment;

			var escapedText = FormattedText.Escape (text);
			var combinedText = TextFormatter.FormatText (comment.Text, "~\n\n", escapedText);

			comment.Text = combinedText;
		}


		private static AiderPersonEntity CreateAiderPersonWithEervPerson(BusinessContext businessContext, EervPerson eervPerson)
		{
			var aiderPerson = businessContext.CreateEntity<AiderPersonEntity> ();
			var eChPerson = aiderPerson.eCH_Person;

			eChPerson.PersonFirstNames = eervPerson.Firstnames;
			eChPerson.PersonOfficialName = eervPerson.Lastname;
			eChPerson.PersonDateOfBirth = eervPerson.DateOfBirth;
			eChPerson.PersonSex = eervPerson.Sex;
			eChPerson.AdultMaritalStatus = eervPerson.MaritalStatus;

			var origins = eervPerson.Origins;

			if (!string.IsNullOrEmpty (origins))
			{
				eChPerson.Origins = origins;
			}

			eChPerson.DataSource = Enumerations.DataSource.Undefined;
			eChPerson.DeclarationStatus = PersonDeclarationStatus.NotDeclared;
			eChPerson.RemovalReason = RemovalReason.None;

			return aiderPerson;
		}


		private static void AddMatchComment(EervPerson eervPerson, AiderPersonEntity aiderPerson, MatchData match, string parishName)
		{
			string text;

			if (match != null)
			{
				text = "Cette Personne correspond à la personne N° " + eervPerson.Id + " du fichier de la paroisse de " + parishName + ".";

				text += "\nLa correspondance a été faite sur les critères suivants : ";
				text += "\n - Nom de famille : " + EervParishDataImporter.GetTextForNameMatch (match.Lastname);
				text += "\n - Prénom : " + EervParishDataImporter.GetTextForNameMatch (match.Firstname);
				text += "\n - Sexe : " + EervParishDataImporter.GetTextForSexMatch (match.Sex);
				text += "\n - Date de naissance : " + EervParishDataImporter.GetTextForDateOfBirthMatch (match.DateOfBirth);
				text += "\n - Adresse : " + EervParishDataImporter.GetTextForAddressMatch (match.Address);
			}
			else
			{
				text = "Cette personne a été crée à partir de la personne N°" + eervPerson.Id + " du fichier de la paroisse de " + parishName + " et n'existe pas dans le registre cantonal des personnes protestantes.";
			}

			EervParishDataImporter.CombineComments (aiderPerson, text);
		}


		private static string GetTextForAddressMatch(AddressMatch match)
		{
			switch (match)
			{
				case AddressMatch.Full:
					return "la rue, le numéro dans la rue, le numéro postal et la localité correspondent";

				case AddressMatch.None:
					return "l'adresse ne correspond pas";

				case AddressMatch.StreetZipCity:
					return "la rue, le numéro postal et la localité correspondent";

				case AddressMatch.ZipCity:
					return "le numéro postal et la localité correspondent";

				default:
					throw new NotImplementedException ();
			}
		}


		private static string GetTextForNameMatch(NameMatch match)
		{
			switch (match)
			{
				case NameMatch.Full:
					return "le nom correspond";

				case NameMatch.OrderedPartial:
					return "une partie du nom manque (nom composé)";

				case NameMatch.Partial:
					return "une partie du nom manque ou est dans le désordre (nom composé)";

				default:
					throw new NotImplementedException ();
			}
		}


		private static string GetTextForSexMatch(bool? match)
		{
			switch (match)
			{
				case true:
					return "le sexe correspond";

				case false:
					return "le sexe ne correspond pas";

				case null:
					return "la correspondance n'a pas pu être établie (sexe manquant)";

				default:
					throw new NotImplementedException ();
			}
		}


		private static string GetTextForDateOfBirthMatch(bool? match)
		{
			switch (match)
			{
				case true:
					return "la date de naissance correspond";

				case false:
					return "la date de naissance ne correspond pas";

				case null:
					return "la correspondance n'a pas pu être établie (date manquante)";

				default:
					throw new NotImplementedException ();
			}
		}


	}


}
