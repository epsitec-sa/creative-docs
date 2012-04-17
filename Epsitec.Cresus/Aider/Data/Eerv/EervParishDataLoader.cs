using Epsitec.Aider.Enumerations;

using Epsitec.Common.Support;
using Epsitec.Common.Support.Extensions;

using Epsitec.Common.Types;

using System;

using System.Collections.Generic;

using System.Diagnostics;

using System.IO;

using System.Linq;


namespace Epsitec.Aider.Data.Eerv
{


	internal static class EervParishDataLoader
	{


		public static EervParishData LoadEervParishData(FileInfo personFile, FileInfo activityFile, FileInfo groupFile)
		{
			var households = EervParishDataLoader.LoadEervHouseholds (personFile).ToList ();
			var persons = EervParishDataLoader.LoadEervPersons (personFile).ToList ();
			var legalPersons = EervParishDataLoader.LoadEervLegalPersons (personFile).ToList ();
			var activities = EervParishDataLoader.LoadEervActivities (activityFile).ToList ();
			var groups = EervParishDataLoader.LoadEervGroups (groupFile).ToList ();

			var rawPersons = persons.Select (t => t.Item1).ToList ();
			var rawActivities = activities.Select(t => t.Item1);
			
			var idToHouseholds = households.ToDictionary (h => h.Id);
			var idToPersons = rawPersons.ToDictionary (p => p.Id);
			var idToLegalPersons = legalPersons.ToDictionary (p => p.Id);
			var idToGroups = groups.ToDictionary (g => g.Id);

			var filteredActivities = EervParishDataLoader.FilterActivities (activities, idToPersons, idToLegalPersons, idToGroups);

			EervParishDataLoader.AssignPersonsToHouseholds (persons, idToHouseholds);
			EervParishDataLoader.AssignActivitiesToPersonsAndGroups (filteredActivities, idToPersons, idToLegalPersons, idToGroups);

			EervParishDataLoader.FilterHouseholdsAndPersons (households, rawPersons);

			EervParishDataLoader.FreezeData (rawActivities, groups, legalPersons, rawPersons, households);

			return new EervParishData(households, rawPersons, legalPersons, groups);
		}


		private static void FilterHouseholdsAndPersons(List<EervHousehold> households, List<EervPerson> persons)
		{
			foreach (var household in households.ToList ())
			{
				if (household.Address.Town == null)
				{
					EervParishDataLoader.Warn ("household " + household.Id + " is discarded because it has no town in its address");

					households.Remove (household);

					foreach (var member in household.Members)
					{
						EervParishDataLoader.Warn ("person " + member.Id + " is discarded because its household is");

						persons.Remove (member);
					}
				}
			}
		}


		private static void FreezeData(params IEnumerable<Freezable>[] freezableSequences)
		{
			foreach (var freezableSequence in freezableSequences)
			{
				foreach (var freezableItem in freezableSequence)
				{
					freezableItem.Freeze ();
				}
			}
		}


		private static void AssignPersonsToHouseholds(IEnumerable<Tuple<EervPerson, Tuple<string, int?>>> persons, Dictionary<string, EervHousehold> idToHouseholds)
		{
			foreach (var personData in persons)
			{
				var person = personData.Item1;
				var householdId = personData.Item2.Item1;
				var householdRank = personData.Item2.Item2;

				var household = idToHouseholds[householdId];

				switch (householdRank)
				{
					case 1:

						if (household.Head1 == null)
						{
							household.Head1 = person;
						}
						else
						{
							EervParishDataLoader.Warn ("person " + person.Id + " cannot be head1 in household " + household.Id + " and will be assigned to head2");

							goto case 2;
						}

						break;

					case 2:

						if (household.Head2 == null)
						{
							household.Head2 = person;
						}
						else
						{
							EervParishDataLoader.Warn ("person " + person.Id + " cannot be head2 in household " + household.Id + " and will be assigned to children");

							goto case 4;
						}

						break;

					case null:
					case 4:

						household.Children.Add (person);

						break;

					default:
						throw new NotImplementedException ();
				}

				person.HouseHold = household;
			}
		}


		private static IEnumerable<Tuple<EervActivity, string, string>> FilterActivities(IEnumerable<Tuple<EervActivity, string, string>> activities, Dictionary<string, EervPerson> idToPersons, Dictionary<string, EervLegalPerson> idToLegalPersons, Dictionary<string, EervGroup> idToGroups)
		{
			// NOTE Here we discard all the activities that are related to an undefined group or to
			// an undefined person or legal person.

			foreach (var activityData in activities)
			{
				var memberId = activityData.Item2;
				var groupId = activityData.Item3;

				var memberExists = idToPersons.ContainsKey (memberId) || idToLegalPersons.ContainsKey (memberId);
				var groupExists = idToGroups.ContainsKey (groupId);

				if (memberExists && groupExists)
				{
					yield return activityData;
				}
				else
				{
					EervParishDataLoader.Warn ("activity between member " + memberId + " and group " + groupId + " is discarded because one of these references is not defined");
				}
			}
		}


		private static void AssignActivitiesToPersonsAndGroups(IEnumerable<Tuple<EervActivity, string, string>> activities, Dictionary<string, EervPerson> idToPersons, Dictionary<string, EervLegalPerson> idToLegalPersons, Dictionary<string, EervGroup> idToGroups)
		{
			foreach (var activityData in activities)
			{
				var activity = activityData.Item1;

				var memberId = activityData.Item2;
				var groupId = activityData.Item3;

				var group = idToGroups[groupId];
				group.Activities.Add (activity);
				activity.Group = group;

				EervPerson person;
				EervLegalPerson legalPerson;
				if (idToPersons.TryGetValue (memberId, out person))
				{
					person.Activities.Add (activity);
					activity.Person = person;
				}
				else if (idToLegalPersons.TryGetValue (memberId, out legalPerson))
				{
					legalPerson.Activities.Add (activity);
					activity.LegalPerson = legalPerson;
				}
			}
		}


		internal static IEnumerable<EervGroup> LoadEervGroups(FileInfo inputFile)
		{
			var groupedRecords = EervDataReader.ReadGroups (inputFile)
				.GroupBy (g => g[GroupHeader.Id]);

			foreach (var records in groupedRecords)
			{
				var group = EervParishDataLoader.GetEervGroup (records.First ());

				foreach (var record in records)
				{
					var definitionId = record[GroupHeader.DefinitionId];

					if (!string.IsNullOrWhiteSpace (definitionId) && !group.GroupDefinitionIds.Contains (definitionId))
					{
						group.GroupDefinitionIds.Add (definitionId);
					}
				}

				if (group.GroupDefinitionIds.Any ())
				{
					yield return group;
				}
			}
		}


		private static EervGroup GetEervGroup(Dictionary<GroupHeader, string> group)
		{
			var id = group[GroupHeader.Id];
			var name = group[GroupHeader.Name];

			return new EervGroup (id, name);
		}


		internal static IEnumerable<Tuple<EervActivity, string, string>> LoadEervActivities(FileInfo inputFile)
		{
			foreach (var record in EervDataReader.ReadActivities (inputFile))
			{
				yield return EervParishDataLoader.GetEervActivity (record);
			}
		}


		private static Tuple<EervActivity, string, string> GetEervActivity(Dictionary<ActivityHeader, string> record)
		{
			var rawStartDate = record[ActivityHeader.StartDate];
			var rawEndDate = record[ActivityHeader.EndDate];
			
			var startDate = StringUtils.ParseNullableDate (rawStartDate);
			var endDate = StringUtils.ParseNullableDate(rawEndDate);
			var remarks = record[ActivityHeader.Remarks];

			var personId = record[ActivityHeader.PersonId];
			var groupId = record[ActivityHeader.GroupId];
			
			var activity = new EervActivity (startDate, endDate, remarks);

			return Tuple.Create (activity, personId, groupId);
		}


		internal static IEnumerable<EervHousehold> LoadEervHouseholds(FileInfo inputFile)
		{
			HashSet<string> processedIds = new HashSet<string> ();

			foreach (var record in EervDataReader.ReadPersons(inputFile))
			{
				var isHousehold = EervParishDataLoader.IsEervPerson (record);

				if (isHousehold)
				{
					var householdId = record[PersonHeader.HouseholdId];

					if (!processedIds.Contains (householdId))
					{
						yield return EervParishDataLoader.GetEervHousehold (record);

						processedIds.Add (householdId);
					}
				}
			}
		}


		private static EervHousehold GetEervHousehold(Dictionary<PersonHeader, string> record)
		{
			var id = record[PersonHeader.HouseholdId];

			var address = EervParishDataLoader.GetAddress (record);
			var coordinates = EervParishDataLoader.GetCoordinates1 (record);

			var remarks = record[PersonHeader.RemarksHousehold];

			return new EervHousehold (id, address, coordinates, remarks);
		}


		private static string GetStreetName(string part1, string part2)
		{
			var hasPart1 = !string.IsNullOrEmpty(part1);
			var hasPart2 = !string.IsNullOrEmpty(part2);
		
			var result = "";

			if (hasPart1)
			{
				result += part1.Trim ();

				var part1EndIsNotApostrophe = part1[part1.Length - 1] != '\'';

				if (part1EndIsNotApostrophe && hasPart2)
				{
					result += " ";
				}
			}

			if (hasPart2)
			{
				result += part2.Trim ();
			}

			if (string.IsNullOrWhiteSpace (result))
			{
				result = null;
			}

			return result;
		}


		internal static IEnumerable<Tuple<EervPerson, Tuple<string, int?>>> LoadEervPersons(FileInfo inputFile)
		{
			foreach (var record in EervDataReader.ReadPersons(inputFile))
			{
				var isEervPerson = EervParishDataLoader.IsEervPerson (record);

				if (isEervPerson)
				{
					yield return EervParishDataLoader.GetEervPerson (record);
				}
			}
		}


		internal static IEnumerable<EervLegalPerson> LoadEervLegalPersons(FileInfo inputFile)
		{
			foreach (var record in EervDataReader.ReadPersons(inputFile))
			{
				var isEervLegalPerson = !EervParishDataLoader.IsEervPerson (record);

				if (isEervLegalPerson)
				{
					yield return EervParishDataLoader.GetEervLegalPerson (record);
				}
			}
		}


		private static bool IsEervPerson(Dictionary<PersonHeader, string> record)
		{
			var corporateName = record[PersonHeader.CorporateName];
			var lastname = record[PersonHeader.Lastname];
			var firstname1 = record[PersonHeader.Firstname1];
			var firstname2 = record[PersonHeader.Firstname2];
			var firstname = EervParishDataLoader.GetFirstname (firstname1, firstname2);

			return string.IsNullOrWhiteSpace (corporateName) &&
				   !string.IsNullOrWhiteSpace (lastname) &&
				   !string.IsNullOrWhiteSpace (firstname);
		}


		private static Tuple<EervPerson, Tuple<string, int?>> GetEervPerson(Dictionary<PersonHeader, string> record)
		{
			var id = record[PersonHeader.PersonId];
			var firstname1 = record[PersonHeader.Firstname1];
			var firstname2 = record[PersonHeader.Firstname2];
			var firstname = EervParishDataLoader.GetFirstname (firstname1, firstname2);

			var lastname = record[PersonHeader.Lastname];
			var originalName = record[PersonHeader.OriginalName];

			var rawDateOfBirth = record[PersonHeader.DateOfBirth];
			var rawDateOfDeath = record[PersonHeader.DateOfDeath];
			var rawMaritalStatus = record[PersonHeader.MaritalStatus];

			var dateOfBirth = StringUtils.ParseNullableDate (rawDateOfBirth);
			Date? dateOfDeath = null;

			if (rawMaritalStatus != null && rawMaritalStatus.ToLowerInvariant () == "dcd")
			{
				dateOfDeath = StringUtils.ParseNullableDate (rawDateOfDeath);
			}

			var rawHonorific = record[PersonHeader.Honorific];
			var rawSex = record[PersonHeader.Sex];
			var rawConfession = record[PersonHeader.Confession];
			var rawDateOfBaptism = record[PersonHeader.DateOfBaptism];
			var rawDateOfChildBenediction = record[PersonHeader.DateOfChildBenediction];
			var rawDateOfCatechismBenediction = record[PersonHeader.DateOfCatechismBenediction];
			var rawSchoolYearOffset = record[PersonHeader.SchoolYearOffset];

			var honorific = EervParishDataLoader.ParseHonorific (rawHonorific);
			var sex = EervParishDataLoader.ParseSex (rawSex);
			var maritalStatus = EervParishDataLoader.ParseMaritalStatus (rawMaritalStatus);
			var origins = record[PersonHeader.Origins];
			var profession = record[PersonHeader.Profession];
			var confession = EervParishDataLoader.ParseConfession (rawConfession);
			var remarks = record[PersonHeader.RemarksPerson];

			var father = record[PersonHeader.Father];
			var mother = record[PersonHeader.Mother];
			var placeOfBirth = record[PersonHeader.PlaceOfBirth];
			var placeOfBaptism = record[PersonHeader.PlaceOfBaptism];
			var dateOfBaptism = StringUtils.ParseNullableDate (rawDateOfBaptism);
			var placeOfChildBenediction = record[PersonHeader.PlaceOfChildBenediction];
			var dateOfChildBenediction = StringUtils.ParseNullableDate (rawDateOfChildBenediction);
			var placeOfCatechismBenediction = record[PersonHeader.PlaceOfCatechismBenediction];
			var dateOfCatechismBenediction = StringUtils.ParseNullableDate (rawDateOfCatechismBenediction);
			var schoolYearOffset = StringUtils.ParseNullableInt (rawSchoolYearOffset);

			var coordinates = EervParishDataLoader.GetCoordinates2 (record);

			var person = new EervPerson (id, firstname, lastname, originalName, dateOfBirth, dateOfDeath, honorific, sex, maritalStatus, origins, profession, confession, remarks, father, mother, placeOfBirth, placeOfBaptism, dateOfBaptism, placeOfChildBenediction, dateOfChildBenediction, placeOfCatechismBenediction, dateOfCatechismBenediction, schoolYearOffset, coordinates);

			var householdId = record[PersonHeader.HouseholdId];
			var householdRank = StringUtils.ParseNullableInt (record[PersonHeader.HouseholdRank]);

			return Tuple.Create (person, Tuple.Create (householdId, householdRank));
		}


		private static EervLegalPerson GetEervLegalPerson(Dictionary<PersonHeader, string> record)
		{
			var id = record[PersonHeader.PersonId];
			var name = record[PersonHeader.CorporateName];

			var address = EervParishDataLoader.GetAddress (record);
			var coordinates = EervParishDataLoader.GetCoordinates1 (record);
			var contactPerson = EervParishDataLoader.GetEervPerson (record).Item1;

			return new EervLegalPerson (id, name, address, coordinates)
			{
				ContactPerson = contactPerson,
			};
		}


		private static EervAddress GetAddress(Dictionary<PersonHeader, string> record)
		{
			var rawStreetNamePart1 = record[PersonHeader.StreetNamePart1];
			var rawStreetNamePart2 = record[PersonHeader.StreetNamePart2];
			var rawHouseNumber = record[PersonHeader.HouseNumber];
			
			var firstAddressLine = record[PersonHeader.FirstAddressLine];
			var streetName = EervParishDataLoader.GetStreetName (rawStreetNamePart1, rawStreetNamePart2);
			var houseNumber = StringUtils.ParseNullableInt(rawHouseNumber);
			var houseNumberComplement = record[PersonHeader.HouseNumberComplement];
			var zipCode = record[PersonHeader.ZipCode];
			var town = record[PersonHeader.Town];

			return new EervAddress (firstAddressLine, streetName, houseNumber, houseNumberComplement, zipCode, town);
		}


		private static EervCoordinates GetCoordinates1(Dictionary<PersonHeader, string> record)
		{
			var faxNumber = record[PersonHeader.FaxNumber];
			var privatePhoneNumber = record[PersonHeader.PrivatePhoneNumber];
			var professionalPhoneNumber = record[PersonHeader.ProfessionalPhoneNumber];

			return new EervCoordinates (privatePhoneNumber, professionalPhoneNumber, null, faxNumber, null);
		}


		private static EervCoordinates GetCoordinates2(Dictionary<PersonHeader, string> record)
		{
			var emailAddress = record[PersonHeader.EmailAddress];
			var mobilePhoneNumber = record[PersonHeader.MobilPhoneNumber];

			return new EervCoordinates (null, null, mobilePhoneNumber, null, emailAddress);
		}


		private static string GetFirstname(string firstname1, string firstname2)
		{
			string firstnames;

			var hasFirstname1 = !string.IsNullOrWhiteSpace (firstname1);
			var hasFirstname2 = !string.IsNullOrWhiteSpace (firstname2);

			if (hasFirstname1 && hasFirstname2)
			{
				firstnames = firstname1 + " " + firstname2;
			}
			else if (hasFirstname1)
			{
				firstnames = firstname1;
			}
			else if (hasFirstname2)
			{
				firstnames = firstname2;
			}
			else
			{
				firstnames = "";
			}

			firstnames = firstnames.Split (new char[] { ' ' }).Distinct ().Join (" ");

			if (string.IsNullOrWhiteSpace (firstnames))
			{
				firstnames = null;
			}

			return firstnames;
		}


		private static PersonSex ParseSex(string text)
		{
			switch (EervParishDataLoader.Normalize (text))
			{
				case "f":
					return PersonSex.Female;

				case "h":
					return PersonSex.Male;

				default:
					return PersonSex.Unknown;
			}
		}


		private static PersonMaritalStatus ParseMaritalStatus(string text)
		{
			switch (EervParishDataLoader.Normalize (text))
			{
				case "cel":
				case "ul":
				case "se":
					return PersonMaritalStatus.Single;

				case "dcd":
					return PersonMaritalStatus.None;

				case "di":
					return PersonMaritalStatus.Divorced;

				case "ma":
					return PersonMaritalStatus.Married;

				case "ve":
					return PersonMaritalStatus.Widowed;

				default:
					return PersonMaritalStatus.None;
			}
		}


		private static PersonConfession ParseConfession(string text)
		{
			switch (EervParishDataLoader.Normalize (text))
			{
				case "e":
				case "ae":
					return PersonConfession.Evangelic;
				
				case "as":
					return PersonConfession.SalvationArmy;
				
				case "ang":
					return PersonConfession.Anglican;
				
				case "b":
					return PersonConfession.Buddhist;
				
				case "c":
					return PersonConfession.Catholic;
				
				case "d":
					return PersonConfession.Darbyst;
				
				case "in":
					return PersonConfession.Unknown;
				
				case "is":
					return PersonConfession.Israelite;
				
				case "mu":
					return PersonConfession.Muslim;
				
				case "na":
					return PersonConfession.NewApostolic;
				
				case "o":
					return PersonConfession.Orthodox;

				case "p":
				case "pc":
					return PersonConfession.Protestant;
				
				case "sa":
					return PersonConfession.Unknown;
				
				case "tj":
					return PersonConfession.JehovahsWitness;

				default:
					return PersonConfession.Unknown;
			}
		}


		private static string ParseHonorific(string text)
		{
			switch (EervParishDataLoader.Normalize (text))
			{
				case "cp":
					return "Capitaine";

				case "fa":
					return null;

				case "m":
					return "Monsieur";

				case "md":
					return "Madame";

				case "mj":
					return "Major";

				case "ml":
					return "Mademoiselle";

				default:
					return null;
			}
		}


		private static string Normalize(string data)
		{
			return data == null
				? null
				: data.ToLowerInvariant ();
		}


		private static void Warn(string warning)
		{
			if (EervParishDataLoader.displayWarnings)
			{
				Debug.WriteLine ("Warning: " + warning);
			}
		}


		private static readonly bool displayWarnings = false;


	}


}
