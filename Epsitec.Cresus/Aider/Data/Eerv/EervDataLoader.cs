using System.Collections.Generic;
using System.IO;
using LumenWorks.Framework.IO.Csv;
using System;
using Epsitec.Common.Support;
using Epsitec.Aider.Enumerations;
using Epsitec.Common.Types;
using System.Linq;
using System.Collections.ObjectModel;


namespace Epsitec.Aider.Data.Eerv
{


	internal static class EervDataLoader
	{


		public static IEnumerable<EervGroupDefinition> LoadEervGroupDefinitions(FileInfo inputFile)
		{
			// Skip the 4 first lines of the file as they are titles.
			var records = EervDataLoader.GetRecords (inputFile).Skip (4);

			Stack<string> parentIds = new Stack<string> ();

			parentIds.Push (null);

			foreach (var record in records)
			{
				var level = EervDataLoader.GetEervGroupDefinitionLevel (record);

				while (level <= parentIds.Count - 2)
				{
					parentIds.Pop ();
				}

				var parentId = parentIds.Peek ();

				var groupDefinition = EervDataLoader.GetEervGroupDefinition (record, level, parentId);

				if (!EervDataLoader.IsGroupDefinitionToDiscard (groupDefinition))
				{
					parentIds.Push (groupDefinition.Id);

					yield return groupDefinition;
				}
			}
		}


		private static int GetEervGroupDefinitionLevel(string[] record)
		{
			for (int i = 0; i < GroupDefinitionIndex.Names.Count; i++)
			{
				var name = record[GroupDefinitionIndex.Names[i]];

				if (!string.IsNullOrEmpty (name))
				{
					return i;
				}
			}

			throw new FormatException ("Invalid group definition level");
		}


		private static EervGroupDefinition GetEervGroupDefinition(string[] record, int groupLevel, string parentId)
		{
			var id = record[GroupDefinitionIndex.Id];
			var name = record[GroupDefinitionIndex.Names[groupLevel]];

			return new EervGroupDefinition (id, name, parentId);
		}


		private static bool IsGroupDefinitionToDiscard(EervGroupDefinition groupDefinition)
		{
			return groupDefinition.Name.Contains ("n1, n2, n3");
		}


		public static IEnumerable<EervGroup> LoadEervGroups(FileInfo inputFile)
		{
			foreach (var record in EervDataLoader.GetRecords (inputFile))
			{
				var group = EervDataLoader.GetEervGroup (record);

				if (!string.IsNullOrEmpty (group.DefinitionId))
				{
					yield return group;
				}
			}
		}


		private static EervGroup GetEervGroup(string[] record)
		{
			var id = record[GroupIndex.Id];
			var definitionId = record[GroupIndex.DefinitionId];
			var name = record[GroupIndex.Name];

			return new EervGroup (id, definitionId, name);
		}


		public static IEnumerable<EervActivity> LoadEervActivities(FileInfo inputFile)
		{
			foreach (var record in EervDataLoader.GetRecords (inputFile))
			{
				yield return EervDataLoader.GetEervActivity (record);
			}
		}


		private static EervActivity GetEervActivity(string[] record)
		{
			var personId = record[ActivityIndex.PersonId];
			var groupId = record[ActivityIndex.GroupId];
			var startDate = StringUtils.ParseNullableDate (record[ActivityIndex.StartDate]);
			var endDate = StringUtils.ParseNullableDate (record[ActivityIndex.EndDate]);
			var remarks = record[ActivityIndex.Remarks];

			return new EervActivity (personId, groupId, startDate, endDate, remarks);
		}


		public static IEnumerable<EervHousehold> LoadEervHouseholds(FileInfo inputFile)
		{
			HashSet<string> processedIds = new HashSet<string> ();

			foreach (var record in EervDataLoader.GetRecords (inputFile))
			{
				var householdId = record[HouseholdIndex.Id];

				if (!processedIds.Contains (householdId))
				{
					yield return EervDataLoader.GetEervHousehold (record);

					processedIds.Add (householdId);
				}
			}
		}


		private static EervHousehold GetEervHousehold(string[] record)
		{
			var id = record[HouseholdIndex.Id];
			var firstAddressLine = record[HouseholdIndex.FirstAddressLine];
			var streetName = EervDataLoader.GetStreetName (record[HouseholdIndex.StreetNamePart1], record[HouseholdIndex.StreetNamePart2]);
			var houseNumber = StringUtils.ParseNullableInt (record[HouseholdIndex.HouseNumber]);
			var houseNumberComplement = record[HouseholdIndex.HouseNumberComplement];
			var zipCode = record[HouseholdIndex.ZipCode];
			var city = record[HouseholdIndex.City];
			var faxNumber = record[HouseholdIndex.FaxNumber];
			var privatePhoneNumber = record[HouseholdIndex.PrivatePhoneNumber];
			var professionalPhoneNumber = record[HouseholdIndex.ProfessionalPhoneNumber];
			var remarks = record[HouseholdIndex.Remarks];

			return new EervHousehold (id, firstAddressLine, streetName, houseNumber, houseNumberComplement, zipCode, city, faxNumber, privatePhoneNumber, professionalPhoneNumber, remarks);
		}


		private static string GetStreetName(string part1, string part2)
		{
			bool addSpace = !string.IsNullOrEmpty(part1)
						 && !string.IsNullOrEmpty(part2)
						 && part1[part1.Length - 1] != '\'';

			string result = part1;
			
			if (addSpace)
			{
				result += " ";
			}

			result += part2;

			return result;
		}


		public static IEnumerable<EervPerson> LoadEervPersons(FileInfo inputFile)
		{
			foreach (var record in EervDataLoader.GetRecords (inputFile))
			{
				yield return EervDataLoader.GetEervPerson (record);
			}
		}


		private static EervPerson GetEervPerson(string[] record)
		{
			var id = record[PersonIndex.Id];
			var firstname1 = record[PersonIndex.Firstname1];
			var firstname2 = record[PersonIndex.Firstname2];
			var lastname = record[PersonIndex.Lastname];
			var originalName = record[PersonIndex.OriginalName];
			var corporateName = record[PersonIndex.CorporateName];
			var dateOfBirth = StringUtils.ParseNullableDate (record[PersonIndex.DateOfBirth]);
			Date? dateOfDeath = null;

			if (record[PersonIndex.MaritalStatus].ToLowerInvariant () == "dcd")
			{
				dateOfDeath = StringUtils.ParseNullableDate (record[PersonIndex.DateOfDeath]);
			}

			var honorific = EervDataLoader.ParseHonorific (record[PersonIndex.Honorific]);
			var sex = EervDataLoader.ParseSex (record[PersonIndex.Sex]);
			var maritalStatus = EervDataLoader.ParseMaritalStatus (record[PersonIndex.MaritalStatus]);
			var origins = record[PersonIndex.Origins];
			var profession = record[PersonIndex.Profession];
			var confession = EervDataLoader.ParseConfession (record[PersonIndex.Confession]);
			var emailAddress = record[PersonIndex.EmailAddress];
			var mobilPhoneNumber = record[PersonIndex.MobilPhoneNumber];
			var remarks = record[PersonIndex.Remarks];
			var father = record[PersonIndex.Father];
			var mother = record[PersonIndex.Mother];
			var placeOfBirth = record[PersonIndex.PlaceOfBirth];
			var placeOfBaptism = record[PersonIndex.PlaceOfBaptism];
			var dateOfBaptism = StringUtils.ParseNullableDate (record[PersonIndex.DateOfBaptism]);
			var placeOfChildBenediction = record[PersonIndex.PlaceOfChildBenediction];
			var dateOfChildBenediction = StringUtils.ParseNullableDate (record[PersonIndex.DateOfChildBenediction]);
			var placeOfCatechismBenediction = record[PersonIndex.PlaceOfCatechismBenediction];
			var dateOfCatechismBenediction = StringUtils.ParseNullableDate (record[PersonIndex.DateOfCatechismBenediction]);
			var schoolYearOffset = StringUtils.ParseNullableInt (record[PersonIndex.SchoolYearOffset]);
			var householdId = record[PersonIndex.HouseholdId];
			var householdRank = int.Parse (record[PersonIndex.HouseholdRank]);

			return new EervPerson (id, firstname1, firstname2, lastname, originalName, corporateName, dateOfBirth, dateOfDeath, honorific, sex, maritalStatus, origins, profession, confession, emailAddress, mobilPhoneNumber, remarks, father, mother, placeOfBirth, placeOfBaptism, dateOfBaptism, placeOfChildBenediction, dateOfChildBenediction, placeOfCatechismBenediction, dateOfCatechismBenediction, schoolYearOffset, householdId, householdRank);
		}


		private static PersonSex ParseSex(string text)
		{
			switch (text.ToLowerInvariant())
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
			switch (text.ToLowerInvariant ())
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
			switch (text.ToLowerInvariant ())
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
			switch (text.ToLowerInvariant())
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


		private static IEnumerable<string[]> GetRecords(FileInfo inputFile)
		{
			using (var csvReader = new CsvReader (new StreamReader (inputFile.FullName, System.Text.UTF8Encoding.Default), false, ';'))
			{
				foreach (var csvRecord in csvReader)
				{
					yield return csvRecord;
				}				
			}
		}


		private static class ActivityIndex
		{


			public static readonly int PersonId = 1;
			public static readonly int GroupId = 2;
			public static readonly int StartDate = 4;
			public static readonly int EndDate = 5;
			public static readonly int Remarks = 7;


		}


		private static class GroupIndex
		{


			public static readonly int Id = 1;
			public static readonly int DefinitionId = 6;
			public static readonly int Name = 2;


		}


		private static class GroupDefinitionIndex
		{


			public static readonly int Id = 2;
			public static readonly int NameLevel1 = 4;
			public static readonly int NameLevel2 = 6;
			public static readonly int NameLevel3 = 8;
			public static readonly int NameLevel4 = 10;
			public static readonly int NameLevel5 = 12;


			public static readonly ReadOnlyCollection<int> Names = new ReadOnlyCollection<int>
			(
				new List<int> ()
				{
					GroupDefinitionIndex.NameLevel1,
					GroupDefinitionIndex.NameLevel2,
					GroupDefinitionIndex.NameLevel3,
					GroupDefinitionIndex.NameLevel4,
					GroupDefinitionIndex.NameLevel5,
				}
			);


		}


		private static class PersonIndex
		{


			public static readonly int Id = 23;
			public static readonly int Firstname1 = 2;
			public static readonly int Firstname2 = 3;
			public static readonly int Lastname = 0;
			public static readonly int OriginalName = 1;
			public static readonly int CorporateName = 4;
			public static readonly int DateOfBirth = 30;
			public static readonly int DateOfDeath = 35;
			public static readonly int Honorific = 5;
			public static readonly int Sex = 6;
			public static readonly int MaritalStatus = 8;
			public static readonly int Origins = 7;
			public static readonly int Profession = 15;
			public static readonly int Confession = 18;
			public static readonly int EmailAddress = 16;
			public static readonly int MobilPhoneNumber = 17;
			public static readonly int Remarks = 22;
			public static readonly int Father = 9;
			public static readonly int Mother = 10;
			public static readonly int PlaceOfBirth = 11;
			public static readonly int PlaceOfBaptism = 19;
			public static readonly int DateOfBaptism = 32;
			public static readonly int PlaceOfChildBenediction = 20;
			public static readonly int DateOfChildBenediction = 33;
			public static readonly int PlaceOfCatechismBenediction = 21;
			public static readonly int DateOfCatechismBenediction = 34;
			public static readonly int SchoolYearOffset = 29;
			public static readonly int HouseholdId = 24;
			public static readonly int HouseholdRank = 28;


		}


		private static class HouseholdIndex
		{


			public static readonly int Id = 24;
			public static readonly int FirstAddressLine = 40;
			public static readonly int StreetNamePart1 = 41;
			public static readonly int StreetNamePart2 = 42;
			public static readonly int HouseNumber = 43;
			public static readonly int HouseNumberComplement = 44;
			public static readonly int PrivatePhoneNumber = 47;
			public static readonly int ProfessionalPhoneNumber = 48;
			public static readonly int FaxNumber = 49;
			public static readonly int ZipCode = 50;
			public static readonly int City = 51;
			public static readonly int Remarks = 52;


		}



	}


}
