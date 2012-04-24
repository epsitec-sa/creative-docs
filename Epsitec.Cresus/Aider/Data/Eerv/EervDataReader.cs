using ClosedXML.Excel;

using Epsitec.Common.Support.Extensions;

using System.Collections.Generic;

using System.IO;

using System.Linq;


namespace Epsitec.Aider.Data.Eerv
{


	internal static class EervDataReader
	{


		public static IEnumerable<Dictionary<GroupHeader, string>> ReadGroups(FileInfo groupFile, FileInfo superGroupFile)
		{
			var groups = EervDataReader.GetRecords (groupFile, EervDataReader.groupHeaders);
			var superGroups = EervDataReader.GetRecords (superGroupFile, EervDataReader.superGroupHeaders);

			return groups.Concat (superGroups);
		}


		public static IEnumerable<Dictionary<ActivityHeader, string>> ReadActivities(FileInfo input)
		{
			return EervDataReader.GetRecords (input, EervDataReader.activityHeaders);
		}


		public static IEnumerable<Dictionary<PersonHeader, string>> ReadPersons(FileInfo input)
		{
			return EervDataReader.GetRecords (input, EervDataReader.personHeaders);
		}


		public static IEnumerable<Dictionary<GroupDefinitionHeader, string>> ReadGroupDefinitions(FileInfo input)
		{
			return EervDataReader.GetRecords (input, EervDataReader.groupDefinitionHeaders);
		}


		public static IEnumerable<Dictionary<IdHeader, string>> ReadIds(FileInfo input)
		{
			return EervDataReader.GetRecords (input, EervDataReader.idHeaders);
		}


		private static IEnumerable<Dictionary<T, string>> GetRecords<T>(FileInfo input, Dictionary<T, string> stringMapping)
		{
			Dictionary<T, int?> indexMapping = null;

			foreach (var line in EervDataReader.GetLines(input))
			{
				if (indexMapping == null)
				{
					indexMapping = EervDataReader.GetIndexMapping (line, stringMapping);
				}
				else
				{
					yield return EervDataReader.GetRecord(line, indexMapping);
				}
			}
		}


		private static IEnumerable<IList<string>> GetLines(FileInfo input)
		{
			var lines = new List<List<string>> ();

			using (var workbook = new XLWorkbook (input.FullName))
			{
				var worksheet = workbook.Worksheets.First();

				foreach (var row in worksheet.RowsUsed())
				{
					var line = new List<string> ();

					foreach (var cell in row.CellsUsed ())
					{
						var columnIndex = cell.Address.ColumnNumber - 1;
						var value = cell.GetValue<string> ();

						line.InsertAtIndex (columnIndex, value);
					}

					lines.Add (line);
				}
			}

			return lines;
		}


		private static Dictionary<T, int?> GetIndexMapping<T>(IList<string> headers, Dictionary<T, string> stringMapping)
		{
			var indexMapping = new Dictionary<T, int?> ();

			foreach (var item in stringMapping)
			{
				var key = item.Key;
				var index = headers.IndexOf (item.Value);

				indexMapping[key] = index >= 0
					? index
					: (int?) null;
			}

			return indexMapping;
		}


		private static Dictionary<T, string> GetRecord<T>(IList<string> line, Dictionary<T, int?> indexMapping)
		{
			var r = new Dictionary<T, string> ();

			foreach (var mapping in indexMapping)
			{
				var key = mapping.Key;
				var index = mapping.Value;

				var value = index.HasValue && line.Count > index.Value
					? line[index.Value]
					: null;

				if (value != null)
				{
					value = value.Trim ();

					if (string.IsNullOrWhiteSpace (value))
					{
						value = null;
					}
				}

				r[key] = value;
			}

			return r;
		}


		private static readonly Dictionary<GroupHeader, string> groupHeaders = new Dictionary<GroupHeader, string> ()
		{
			{ GroupHeader.Id, "IdxG" },
			{ GroupHeader.Name, "DesG" },
			{ GroupHeader.SuperGroupId, "IdxSG" },
			{ GroupHeader.ParishId, "IdxPar" },
		};


		private static readonly Dictionary<GroupHeader, string> superGroupHeaders = new Dictionary<GroupHeader, string> ()
		{
			{ GroupHeader.Id, "IdxSG" },
			{ GroupHeader.Name, "DesSG" },
			{ GroupHeader.SuperGroupId, "DUMMY" },
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
			{ PersonHeader.RemarksHousehold, "Remarque2" },

			{ PersonHeader.ParishId, "IdxPar" },
		};


		private static readonly Dictionary<GroupDefinitionHeader, string> groupDefinitionHeaders = new Dictionary<GroupDefinitionHeader, string> ()
		{
			{ GroupDefinitionHeader.Id, "Id" },
			{ GroupDefinitionHeader.NameLevel1, "Level1" },
			{ GroupDefinitionHeader.NameLevel2, "Level2" },
			{ GroupDefinitionHeader.NameLevel3, "Level3" },
			{ GroupDefinitionHeader.NameLevel4, "Level4" },
			{ GroupDefinitionHeader.NameLevel5, "Level5" },
		};


		private static readonly Dictionary<IdHeader, string> idHeaders = new Dictionary<IdHeader, string> ()
		{
			{ IdHeader.Id, "IdxPar" },
			{ IdHeader.Name, "NomParoisse" },
		};


	}


	internal enum GroupHeader
	{
		Id,
		Name,
		SuperGroupId,
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
	}


	internal enum IdHeader
	{
		Id,
		Name,
	}


}
