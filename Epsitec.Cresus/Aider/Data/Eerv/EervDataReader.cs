using Epsitec.Aider.Data.Common;

using System.Collections.Generic;

using System.IO;

using System.Linq;


namespace Epsitec.Aider.Data.Eerv
{


	internal static class EervDataReader
	{


		public static IEnumerable<Dictionary<GroupHeader, string>> ReadGroups(FileInfo groupFile, FileInfo superGroupFile)
		{
			var groups = DataReader.GetRecords (groupFile, EervDataReader.groupHeaders);
			var superGroups = DataReader.GetRecords (superGroupFile, EervDataReader.superGroupHeaders);

			return groups.Concat (superGroups);
		}


		public static IEnumerable<Dictionary<ActivityHeader, string>> ReadActivities(FileInfo input)
		{
			return DataReader.GetRecords (input, EervDataReader.activityHeaders);
		}


		public static IEnumerable<Dictionary<PersonHeader, string>> ReadPersons(FileInfo input)
		{
			return DataReader.GetRecords (input, EervDataReader.personHeaders);
		}


		public static IEnumerable<Dictionary<GroupDefinitionHeader, string>> ReadGroupDefinitions(FileInfo input)
		{
			return DataReader.GetRecords (input, EervDataReader.groupDefinitionHeaders, 3);
		}


		public static IEnumerable<Dictionary<IdHeader, string>> ReadIds(FileInfo input)
		{
			return DataReader.GetRecords (input, EervDataReader.idHeaders);
		}


		private static readonly Dictionary<GroupHeader, string> groupHeaders = new Dictionary<GroupHeader, string> ()
		{
			{ GroupHeader.Id, "IdxG" },
			{ GroupHeader.Name, "DesG" },
			{ GroupHeader.ParishId, "IdxPar" },
		};


		private static readonly Dictionary<GroupHeader, string> superGroupHeaders = new Dictionary<GroupHeader, string> ()
		{
			{ GroupHeader.Id, "IdxSG" },
			{ GroupHeader.Name, "DesSG" },
			{ GroupHeader.ParishId, "IdxPar" },
		};


		private static readonly Dictionary<ActivityHeader, string> activityHeaders = new Dictionary<ActivityHeader, string> ()
		{
			{ ActivityHeader.PersonId, "IdxP" },
			{ ActivityHeader.GroupId, "IdxG" },
			{ ActivityHeader.StartDate, "Début" },
			{ ActivityHeader.EndDate, "Fin" },
			{ ActivityHeader.Remarks, "TextLibre" },
			{ ActivityHeader.ParishId, "IdxPar" },
		};


		private static readonly Dictionary<PersonHeader, string> personHeaders = new Dictionary<PersonHeader, string> ()
		{
			{ PersonHeader.PersonId, "IdxP" },
			{ PersonHeader.Firstname1, "Pren1" },
			{ PersonHeader.Firstname2, "Pren2" },
			{ PersonHeader.Lastname, "Nom" },
			{ PersonHeader.OriginalName, "NomNaiss" },
			{ PersonHeader.CorporateName, "RaisonSoc" },
			{ PersonHeader.DateOfBirth, "DateNaiss" },
			{ PersonHeader.DateOfDeath, "DateEciv" },
			{ PersonHeader.Honorific, "Intit" },
			{ PersonHeader.Sex, "Sex" },
			{ PersonHeader.MaritalStatus, "Eciv" },
			{ PersonHeader.Origins, "Orig" },
			{ PersonHeader.Profession, "Prof" },
			{ PersonHeader.Confession, "Conf" },
			{ PersonHeader.EmailAddress, "Email" },
			{ PersonHeader.MobilPhoneNumber, "TelM" },
			{ PersonHeader.RemarksPerson, "Rem" },
			{ PersonHeader.Father, "Pere" },
			{ PersonHeader.Mother, "Mere" },
			{ PersonHeader.PlaceOfBirth, "LieuNaiss" },
			{ PersonHeader.PlaceOfBaptism, "LieuBap" },
			{ PersonHeader.DateOfBaptism, "DateBap" },
			{ PersonHeader.PlaceOfChildBenediction, "LieuBenEnf" },
			{ PersonHeader.DateOfChildBenediction, "DateBenEnf" },
			{ PersonHeader.PlaceOfCatechismBenediction, "LieuBenKT" },
			{ PersonHeader.DateOfCatechismBenediction, "DateBenKT" },
			{ PersonHeader.SchoolYearOffset, "AnScol" },
			{ PersonHeader.HouseholdRank, "Rang" },

			{ PersonHeader.HouseholdId, "IdxF" },
			{ PersonHeader.FirstAddressLine, "RueBatim" },
			{ PersonHeader.StreetNamePart1, "RueIntit" },
			{ PersonHeader.StreetNamePart2, "RueNom" },
			{ PersonHeader.HouseNumber, "RueNo" },
			{ PersonHeader.HouseNumberComplement, "RueNoABC" },
			{ PersonHeader.PrivatePhoneNumber, "TelPriv" },
			{ PersonHeader.ProfessionalPhoneNumber, "TelProf" },
			{ PersonHeader.FaxNumber, "Fax" },
			{ PersonHeader.ZipCode, "NPA" },
			{ PersonHeader.Town, "Localité" },
			{ PersonHeader.CountryCode, "Pays" },
			{ PersonHeader.RemarksHousehold, "Remarque2" },

			{ PersonHeader.ParishId, "IdxPar" },
		};


		private static readonly Dictionary<GroupDefinitionHeader, string> groupDefinitionHeaders = new Dictionary<GroupDefinitionHeader, string> ()
		{
			{ GroupDefinitionHeader.Id, "Id" },
			{ GroupDefinitionHeader.NameLevel1, "Superstructures groupes A" },
			{ GroupDefinitionHeader.NameLevel2, "Superstructures groupes B" },
			{ GroupDefinitionHeader.NameLevel3, "Superstructures groupes C" },
			{ GroupDefinitionHeader.NameLevel4, "Groupes" },
			{ GroupDefinitionHeader.NameLevel5, "Sous-Groupes A" },
			{ GroupDefinitionHeader.Function, "Fonctions" },
			{ GroupDefinitionHeader.IsLeaf, "Feuille" },
		};


		private static readonly Dictionary<IdHeader, string> idHeaders = new Dictionary<IdHeader, string> ()
		{
			{ IdHeader.Id, "IdxPar" },
			{ IdHeader.Name, "NomParoisse" },
			{ IdHeader.Kind, "Type" },
		};


	}


	internal enum GroupHeader
	{
		Id,
		Name,
		ParishId,
	}


	internal enum ActivityHeader
	{
		PersonId,
		GroupId,
		StartDate,
		EndDate,
		Remarks,
		ParishId,
	}


	internal enum PersonHeader
	{
		PersonId,
		Firstname1,
		Firstname2,
		Lastname,
		OriginalName,
		CorporateName,
		DateOfBirth,
		DateOfDeath,
		Honorific,
		Sex,
		MaritalStatus,
		Origins,
		Profession,
		Confession,
		EmailAddress,
		MobilPhoneNumber,
		RemarksPerson,
		Father,
		Mother,
		PlaceOfBirth,
		PlaceOfBaptism,
		DateOfBaptism,
		PlaceOfChildBenediction,
		DateOfChildBenediction,
		PlaceOfCatechismBenediction,
		DateOfCatechismBenediction,
		SchoolYearOffset,
		HouseholdRank,
		
		HouseholdId,	
		FirstAddressLine,
		StreetNamePart1,
		StreetNamePart2,
		HouseNumber,
		HouseNumberComplement,
		PrivatePhoneNumber,
		ProfessionalPhoneNumber,
		FaxNumber,
		ZipCode,
		Town,
		CountryCode,
		RemarksHousehold,

		ParishId,
	}


	internal enum GroupDefinitionHeader
	{
		Id,
		NameLevel1,
		NameLevel2,
		NameLevel3,
		NameLevel4,
		NameLevel5,
		Function,
		IsLeaf,
	}


	internal enum IdHeader
	{
		Id,
		Name,
		Kind,
	}


}
