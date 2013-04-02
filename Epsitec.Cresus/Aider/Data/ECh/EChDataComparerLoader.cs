using Epsitec.Aider.Enumerations;
using Epsitec.Aider.Tools;

using Epsitec.Common.Support;

using Epsitec.Common.Types;

using Epsitec.Data.Platform;

using System;

using System.Collections.Generic;

using System.IO;

using System.Linq;

using System.Reflection;

using System.Xml.Linq;


namespace Epsitec.Aider.Data.ECh
{

	/// <summary>
	/// Custom DataLoader for EChDataComparer, add the XElement to EchReportedPerson and EchPerson entity constructor
	/// </summary>
	internal static class EChDataComparerLoader
	{


		public static IList<EChReportedPerson> Load(FileInfo inputFile, int maxCount = int.MaxValue)
		{
			var xDocument = EChDataComparerLoader.GetDocument (inputFile);

			EChDataComparerLoader.CheckDocument (xDocument);

			// NOTE Here we don't use delayed execution but we return the result as a list. That's
			// because we have that huge XML document in memory which is like 400MB. So by calling
			// ToList(), we retrieve the data which takes around 100MB and we can forget about the
			// XML document, which will allow the garbage collector to reclaim that memory.

			// NOTE A possible optimization here would be to use the XmlReader class directly to
			// parse the XML data and transform it into ECh* objects. That would probably use less
			// memory as this would enable us to stream the XML file instead of reading it as a
			// whole. But right now I don't have time to do this.

			return EChDataComparerLoader.GetData (xDocument)
				.Take (maxCount).ToList ();
		}


		private static XDocument GetDocument(FileInfo inputFile)
		{
			return XDocument.Load (inputFile.FullName);
		}


		private static void CheckDocument(XDocument xDocument)
		{
			var assembly = Assembly.GetExecutingAssembly ();

			var xsdResourceNames = new List<string> ()
			{
				"Epsitec.Aider.Resources.Xsd.eCH-0006-2-0.xsd",
				"Epsitec.Aider.Resources.Xsd.eCH-0007-3-0.xsd",
				"Epsitec.Aider.Resources.Xsd.eCH-0008-2-0.xsd",
				"Epsitec.Aider.Resources.Xsd.eCH-0010-3-0.xsd",
				"Epsitec.Aider.Resources.Xsd.eCH-0011-3-0.xsd",
				"Epsitec.Aider.Resources.Xsd.eCH-0021-2-1.xsd",
				"Epsitec.Aider.Resources.Xsd.eCH-0044-1-0.xsd",
				"Epsitec.Aider.Resources.Xsd.eVD-0002-1-0.xsd",
				"Epsitec.Aider.Resources.Xsd.eVD-0004-1-0.xsd",
			};

			var xmlValidator = XmlValidator.Create (assembly, xsdResourceNames);

			xmlValidator.Validate (xDocument);
		}


		private static IEnumerable<EChReportedPerson> GetData(XDocument xDocument)
		{
			var xRoot = xDocument.Root;

			foreach (var xReportedPerson in xRoot.Elements (EChXmlTags.EVd0002.ReportedPerson))
			{
				yield return EChDataComparerLoader.GetEChReportedPerson (xReportedPerson);
			}
		}


		private static EChReportedPerson GetEChReportedPerson(XElement xReportedPerson)
		{
			var address = EChDataComparerLoader.GetAddress (xReportedPerson);
			var adults = EChDataComparerLoader.GetAdults (xReportedPerson).ToList ();
			var children = EChDataComparerLoader.GetChildren (xReportedPerson);

			var adult1 = adults.Count > 0 ? adults[0] : null;
			var adult2 = adults.Count > 1 ? adults[1] : null;

			//Add the XElement
			return new EChReportedPerson (adult1, adult2, children, address, xReportedPerson);
		}


		public static IEnumerable<EChPerson> GetAdults(XElement xReportedPerson)
		{
			foreach (var xAdult in xReportedPerson.Elements (EChXmlTags.EVd0002.Adult))
			{
				yield return EChDataComparerLoader.GetEChPerson (xAdult);
			}
		}


		public static IEnumerable<EChPerson> GetChildren(XElement xReportedPerson)
		{
			var xChildren = xReportedPerson.Element (EChXmlTags.EVd0002.Children);

			if (xChildren != null)
			{
				foreach (var xChild in xChildren.Elements (EChXmlTags.EVd0002.Child))
				{
					yield return EChDataComparerLoader.GetEChPerson (xChild);
				}
			}
		}


		public static EChPerson GetEChPerson(XElement xEChPerson)
		{
			string id = null;
			string officialName = null;
			string firstNames = null;
			Date dateOfBirth = new Date ();
			PersonSex sex = PersonSex.Unknown;

			var xPerson = xEChPerson.Element (EChXmlTags.EVd0002.Person);

			if (xPerson != null)
			{
				id = EChDataComparerLoader.GetEChPersonId (xPerson);
				officialName = EChDataComparerLoader.GetEChPersonOfficialName (xPerson);
				firstNames = EChDataComparerLoader.GetEChPersonFirstNames (xPerson);
				dateOfBirth = EChDataComparerLoader.GetEChPersonDateOfBirth (xPerson);
				sex = EChDataComparerLoader.GetEChPersonSex (xPerson);
			}

			PersonNationalityStatus nationalityStatus = PersonNationalityStatus.None;
			string nationalCountryCode = null;

			var xNationality = xEChPerson.Element (EChXmlTags.EVd0002.Nationality);

			if (xNationality != null)
			{
				nationalityStatus = EChDataComparerLoader.GetEChPersonNationalityStatus (xNationality);
				nationalCountryCode = EChDataComparerLoader.GetEChPersonNationalCountryCode (xNationality);
			}

			var originPlaces =
				from xOrigin in xEChPerson.Elements (EChXmlTags.EVd0002.Origin)
				select EChDataComparerLoader.GetEChPersonOriginPlace (xOrigin);

			PersonMaritalStatus maritalStatus = PersonMaritalStatus.None;

			var xMaritalStatus = xEChPerson.Element (EChXmlTags.EVd0002.MaritalStatus);

			if (xMaritalStatus != null)
			{
				maritalStatus = EChDataComparerLoader.GetEChPersonMaritalStatus (xMaritalStatus);
			}

			//!Add the XElement
			return new EChPerson (id, officialName, firstNames, dateOfBirth, sex, nationalityStatus, nationalCountryCode, originPlaces, maritalStatus, xPerson);
		}


		private static string GetEChPersonId(XElement xPerson)
		{
			var xPersonId = xPerson.Element (EChXmlTags.EVd0004.PersonId);

			if (xPersonId != null)
			{
				return EChDataComparerLoader.GetChildStringValue (xPersonId, EChXmlTags.ECh0044.PersonId);
			}
			else
			{
				return null;
			}
		}


		private static string GetEChPersonOfficialName(XElement xPerson)
		{
			var name = EChDataComparerLoader.GetChildStringValue (xPerson, EChXmlTags.EVd0004.OfficialName);

			return NamePatchEngine.SanitizeCapitalization (name);
		}


		private static string GetEChPersonFirstNames(XElement xPerson)
		{
			var name = EChDataComparerLoader.GetChildStringValue (xPerson, EChXmlTags.EVd0004.FirstNames);

			return NamePatchEngine.SanitizeCapitalization (name);
		}


		private static Date GetEChPersonDateOfBirth(XElement xPerson)
		{
			var xDateOfBirth = xPerson.Element (EChXmlTags.EVd0004.DateOfBirth);

			if (xDateOfBirth == null)
			{
				throw new FormatException ("No date of birth");
			}

			var xDateOfBirthChild = xDateOfBirth.Elements ().Single ();
			var xDateOfBirthChildName = xDateOfBirthChild.Name;

			if (xDateOfBirthChildName == EChXmlTags.ECh0044.Year)
			{
				int year = (int) xDateOfBirthChild;
				return new Date (year, 0, 0);
			}

			if (xDateOfBirthChildName != EChXmlTags.ECh0044.YearMonthDay)
			{
				throw new FormatException ("Partial dates are not supported.");
			}

			return new Date ((DateTime) xDateOfBirthChild);
		}


		public static PersonSex GetEChPersonSex(XElement xPerson)
		{
			var sex = EChDataComparerLoader.GetChildStringValue (xPerson, EChXmlTags.EVd0004.Sex);

			switch (sex)
			{
				case "1":

					return PersonSex.Male;

				case "2":
					return PersonSex.Female;

				case null:
					return PersonSex.Unknown;

				default:
					throw new FormatException ();
			}
		}


		public static PersonNationalityStatus GetEChPersonNationalityStatus(XElement xNationality)
		{
			var status = EChDataComparerLoader.GetChildStringValue (xNationality, EChXmlTags.ECh0011.NationalityStatus);

			switch (status)
			{
				case "0":
					return PersonNationalityStatus.Unknown;

				case "1":
					return PersonNationalityStatus.None;

				case "2":
					return PersonNationalityStatus.Defined;

				default:
					throw new FormatException ();
			}
		}


		public static string GetEChPersonNationalCountryCode(XElement xNationality)
		{
			string code = null;

			var xCountry = xNationality.Element (EChXmlTags.ECh0011.Country);

			if (xCountry != null)
			{
				code = EChDataComparerLoader.GetChildStringValue (xCountry, EChXmlTags.ECh0008.CountryIdIso2);
			}

			return code;
		}


		public static EChPlace GetEChPersonOriginPlace(XElement xOrigin)
		{
			var name = EChDataComparerLoader.GetChildStringValue (xOrigin, EChXmlTags.ECh0011.OriginName);
			var canton = EChDataComparerLoader.GetChildStringValue (xOrigin, EChXmlTags.ECh0011.Canton);

			return new EChPlace (name, canton);
		}


		public static PersonMaritalStatus GetEChPersonMaritalStatus(XElement xMaritalStatus)
		{
			var status = EChDataComparerLoader.GetChildStringValue (xMaritalStatus, EChXmlTags.ECh0011.MaritalStatus);

			switch (status)
			{
				case "1":
					return PersonMaritalStatus.Single;

				case "2":
					return PersonMaritalStatus.Married;

				case "3":
					return PersonMaritalStatus.Widowed;

				case "4":
					return PersonMaritalStatus.Divorced;

				case "5":
					return PersonMaritalStatus.Unmarried;

				case "6":
					return PersonMaritalStatus.Pacs;

				case "7":
					return PersonMaritalStatus.Dissolved;

				default:
					throw new FormatException ();
			}
		}


		public static EChAddress GetAddress(XElement xReportedPerson)
		{
			var xAddress = xReportedPerson.Element (EChXmlTags.EVd0002.Address);

			if (xAddress != null)
			{
				return EChDataComparerLoader.GetEChAddress (xAddress);
			}
			else
			{
				return null;
			}
		}


		public static EChAddress GetEChAddress(XElement xAddress)
		{
			var addressLine1 = EChDataComparerLoader.GetChildStringValue (xAddress, EChXmlTags.ECh0010.AddressLine1);
			var street = EChDataComparerLoader.GetChildStringValue (xAddress, EChXmlTags.ECh0010.Street);

			var rawHouseNumber = EChDataComparerLoader.GetChildStringValue (xAddress, EChXmlTags.ECh0010.HouseNumber);
			var stripedHouseNumber = SwissPostStreet.StripHouseNumber (rawHouseNumber);
			var houseNumber = string.IsNullOrWhiteSpace (stripedHouseNumber)
				? (int?) null
				: int.Parse (stripedHouseNumber);

			var town = EChDataComparerLoader.GetChildStringValue (xAddress, EChXmlTags.ECh0010.Town);

			var rawSwissZipCode = EChDataComparerLoader.GetChildStringValue (xAddress, EChXmlTags.ECh0010.SwissZipCode);
			var swissZipCode = InvariantConverter.ParseInt (rawSwissZipCode);

			var rawSwissZipCodeAddOn = EChDataComparerLoader.GetChildStringValue (xAddress, EChXmlTags.ECh0010.SwissZipCodeAddOn);
			var swissZipCodeAddOn = InvariantConverter.ParseInt (rawSwissZipCodeAddOn);

			var rawSwissZipCodeId = EChDataComparerLoader.GetChildStringValue (xAddress, EChXmlTags.ECh0010.SwissZipCodeId);
			var swissZipCodeId = InvariantConverter.ParseInt (rawSwissZipCodeId);

			var countryCode = EChDataComparerLoader.GetChildStringValue (xAddress, EChXmlTags.ECh0010.Country);

			EChAddressFixesRepository.FixAddress (ref street, houseNumber, ref swissZipCode, ref swissZipCodeAddOn, ref swissZipCodeId, ref town);
			AddressPatchEngine.Current.FixAddress (ref addressLine1, ref street, houseNumber, ref swissZipCode, ref swissZipCodeAddOn, ref swissZipCodeId, ref town);

			return new EChAddress (addressLine1, street, rawHouseNumber, town, swissZipCode, swissZipCodeAddOn, swissZipCodeId, countryCode);
		}


		private static string GetChildStringValue(XElement xElement, XName xName)
		{
			var xChild = xElement.Element (xName);

			if (xChild != null)
			{
				return xChild.Value;
			}
			else
			{
				return null;
			}
		}


	}


}
