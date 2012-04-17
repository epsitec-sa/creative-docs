using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;

using Epsitec.Common.Support.Extensions;

using System;

using System.Collections.Generic;

using System.IO;

using System.Linq;


namespace Epsitec.Aider.Data.Eerv
{


	internal static class EervDataReader
	{


		public static IEnumerable<Dictionary<GroupHeader, string>> ReadGroups(FileInfo input)
		{
			return EervDataReader.GetRecords (input, EervDataReader.groupHeaders);
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


		private static IEnumerable<Dictionary<T, string>> GetRecords<T>(FileInfo input, Dictionary<T, string> stringMapping)
		{
			var lines = EervDataReader.GetLines (input);

			Dictionary<T, int?> indexMapping = null;

			foreach (var line in lines)
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
			using (var document = SpreadsheetDocument.Open (input.FullName, false))
			{
				var worksheet = EervDataReader.GetWorksheet (document);
				var sharedStringTable = EervDataReader.GetSharedStringTable (document);

				return EervDataReader.GetLines (worksheet, sharedStringTable).ToList ();
			}
		}


		private static Worksheet GetWorksheet(SpreadsheetDocument document)
		{
			return document.WorkbookPart.WorksheetParts.First().Worksheet;
		}


		private static SharedStringTable GetSharedStringTable(SpreadsheetDocument document)
		{
			var sharedStringTablePart = document.WorkbookPart.SharedStringTablePart;

			return sharedStringTablePart != null
				? sharedStringTablePart.SharedStringTable
				: null;
		}


		private static IEnumerable<IList<string>> GetLines(Worksheet worksheet, SharedStringTable sharedStringTable)
		{
			foreach (var row in worksheet.Descendants<Row> ())
			{
				var line = new List<string> ();

				foreach (var cell in row.Descendants<Cell> ())
				{
					var index = EervDataReader.GetColumnIndex (cell);
					var value = EervDataReader.GetValue (cell, sharedStringTable);

					line.InsertAtIndex (index, value);
				}

				yield return line;
			}
		}


		private static int GetColumnIndex(Cell cell)
		{
			// We must begin by extracting the colum part of the index which is in upper case.		
			var cellReference = cell.CellReference.Value;

			var length = 0;

			while (length < cellReference.Length && char.IsLetter (cellReference[length]) && char.IsUpper (cellReference[length]))
			{
				length++;
			}

			var columnReference = cellReference.Substring (0, length);
			
			// We start at -1 and not at zero because we want the zero based index and not the
			// one based index.
			var columnIndex = -1;

			// We convert the ABC style index to a numeric index.
			for (int i = 0; i < columnReference.Length; i++)
			{
				var power = columnReference.Length - 1 - i;
				var factor = (int) columnReference[i] - 64;

				columnIndex += factor * (int) Math.Pow (26, power);
			}

			return columnIndex;
		}


		private static string GetValue(Cell cell, SharedStringTable sharedStringTable)
		{
			var value = "";

			if (cell.CellValue != null)
			{
				value = cell.CellValue.InnerText;

				if (cell.DataType != null && cell.DataType == CellValues.SharedString)
				{
					var index = int.Parse (value);

					value = sharedStringTable.ChildElements[index].InnerText;
				}
			}

			return value;
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
			{ GroupHeader.DefinitionId, "IdxGD" },
			{ GroupHeader.Name, "DesG" },
		};


		private static readonly Dictionary<ActivityHeader, string> activityHeaders = new Dictionary<ActivityHeader, string> ()
		{
			{ ActivityHeader.PersonId, "IdxP" },
			{ ActivityHeader.GroupId, "IdxG" },
			{ ActivityHeader.StartDate, "Début" },
			{ ActivityHeader.EndDate, "Fin" },
			{ ActivityHeader.Remarks, "TextLibre" },
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


	}


	internal enum GroupHeader
	{
		Id,
		DefinitionId,
		Name,
	}


	internal enum ActivityHeader
	{
		PersonId,
		GroupId,
		StartDate,
		EndDate,
		Remarks,
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


}
