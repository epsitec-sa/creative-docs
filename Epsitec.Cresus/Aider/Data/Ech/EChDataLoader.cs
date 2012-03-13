using Epsitec.Aider.Enumerations;

using Epsitec.Common.Support;

using Epsitec.Common.Types;

using System;

using System.Collections.Generic;

using System.Globalization;

using System.IO;

using System.Linq;

using System.Reflection;

using System.Xml.Linq;


namespace Epsitec.Aider.Data.Ech
{


	internal static class EChDataLoader
	{


		public static IList<EChReportedPerson> Load(FileInfo inputFile, int maxCount = int.MaxValue)
		{
			var xDocument = EChDataLoader.GetDocument (inputFile);

			EChDataLoader.CheckDocument (xDocument);

			// NOTE Here we don't use delayed execution but we return the result as a list. That's
			// because we have that huge XML document in memory which is like 400MB. So by calling
			// ToList(), we retrieve the data which takes around 100MB and we can forget about the
			// XML document, which will allow the garbage collector to reclaim that memory.

			// NOTE A possible optimization here would be to use the XmlReader class directly to
			// parse the XML data and transform it into ECh* objects. That would probably use less
			// memory as this would enable us to stream the XML file instead of reading it as a
			// whole. But right now I don't have time to do this.

			return EChDataLoader.GetData (xDocument)
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
				yield return EChDataLoader.GetEChReportedPerson (xReportedPerson);
			}
		}


		private static EChReportedPerson GetEChReportedPerson(XElement xReportedPerson)
		{
			var address = EChDataLoader.GetAddress (xReportedPerson);
			var adults = EChDataLoader.GetAdults (xReportedPerson).ToList ();
			var children = EChDataLoader.GetChildren (xReportedPerson);

			var adult1 = adults.Count > 0 ? adults[0] : null;
			var adult2 = adults.Count > 1 ? adults[1] : null;

			return new EChReportedPerson (adult1, adult2, children, address);
		}


		public static IEnumerable<EChPerson> GetAdults(XElement xReportedPerson)
		{
			foreach (var xAdult in xReportedPerson.Elements (EChXmlTags.EVd0002.Adult))
			{
				yield return EChDataLoader.GetEchPerson (xAdult);
			}
		}


		public static IEnumerable<EChPerson> GetChildren(XElement xReportedPerson)
		{
			var xChildren = xReportedPerson.Element (EChXmlTags.EVd0002.Children);

			if (xChildren != null)
			{
				foreach (var xChild in xChildren.Elements (EChXmlTags.EVd0002.Child))
				{
					yield return EChDataLoader.GetEchPerson (xChild);
				}
			}
		}


		public static EChPerson GetEchPerson(XElement xEChPerson)
		{
			string id = null;
			string officialName = null;
			string firstNames = null;
			Date dateOfBirth = new Date ();
			PersonSex sex = PersonSex.Unknown;

			var xPerson = xEChPerson.Element (EChXmlTags.EVd0002.Person);

			if (xPerson != null)
			{
				id = EChDataLoader.GetEChPersonId (xPerson);
				officialName = EChDataLoader.GetEChPersonOfficialName (xPerson);
				firstNames = EChDataLoader.GetEChPersonFirstNames (xPerson);
				dateOfBirth = EChDataLoader.GetEchPersonDateOfBirth (xPerson);
				sex = EChDataLoader.GetEchPersonSex (xPerson);
			}

			PersonNationalityStatus nationalityStatus = PersonNationalityStatus.None;
			string nationalCountryCode = null;

			var xNationality = xEChPerson.Element (EChXmlTags.EVd0002.Nationality);

			if (xNationality != null)
			{
				nationalityStatus = EChDataLoader.GetEChPersonNationalityStatus (xNationality);
				nationalCountryCode = EChDataLoader.GetEChPersonNationalCountryCode (xNationality);
			}

			var originPlaces =
				from xOrigin in xEChPerson.Elements (EChXmlTags.EVd0002.Origin)
				select EChDataLoader.GetEChPersonOriginPlace (xOrigin);

			PersonMaritalStatus maritalStatus = PersonMaritalStatus.None;

			var xMaritalStatus = xEChPerson.Element (EChXmlTags.EVd0002.MaritalStatus);

			if (xMaritalStatus != null)
			{
				maritalStatus = EChDataLoader.GetEChPersonMaritalStatus (xMaritalStatus);
			}

			return new EChPerson (id, officialName, firstNames, dateOfBirth, sex, nationalityStatus, nationalCountryCode, originPlaces, maritalStatus);
		}


		private static string GetEChPersonId(XElement xPerson)
		{
			var xPersonId = xPerson.Element (EChXmlTags.EVd0004.PersonId);

			if (xPersonId != null)
			{
				return EChDataLoader.GetChildStringValue (xPersonId, EChXmlTags.ECh0044.PersonId);
			}
			else
			{
				return null;
			}
		}


		private static string GetEChPersonOfficialName(XElement xPerson)
		{
			return EChDataLoader.GetChildStringValue (xPerson, EChXmlTags.EVd0004.OfficialName);
		}


		private static string GetEChPersonFirstNames(XElement xPerson)
		{
			return EChDataLoader.GetChildStringValue (xPerson, EChXmlTags.EVd0004.FirstNames);
		}


		private static Date GetEchPersonDateOfBirth(XElement xPerson)
		{
			var xDateOfBirth = xPerson.Element (EChXmlTags.EVd0004.DateOfBirth);

			if (xDateOfBirth == null)
			{
				throw new FormatException ("No date of birth");
			}

			var xDateOfBirthChild = xDateOfBirth.Elements ().Single ();
			var xDateOfBirthChildName = xDateOfBirthChild.Name;

			if (xDateOfBirthChildName != EChXmlTags.ECh0044.YearMonthDay)
			{
				throw new FormatException ("Partial dates are not supported.");
			}

			return new Date ((DateTime) xDateOfBirthChild);
		}


		public static PersonSex GetEchPersonSex(XElement xPerson)
		{
			var sex = EChDataLoader.GetChildStringValue (xPerson, EChXmlTags.EVd0004.Sex);

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
			var status = EChDataLoader.GetChildStringValue (xNationality, EChXmlTags.ECh0011.NationalityStatus);

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
				code = EChDataLoader.GetChildStringValue (xCountry, EChXmlTags.ECh0008.CountryIdIso2);
			}

			return code;
		}


		public static EChPlace GetEChPersonOriginPlace(XElement xOrigin)
		{
			var name = EChDataLoader.GetChildStringValue (xOrigin, EChXmlTags.ECh0011.OriginName);
			var canton = EChDataLoader.GetChildStringValue (xOrigin, EChXmlTags.ECh0011.Canton);

			return new EChPlace (name, canton);
		}


		public static PersonMaritalStatus GetEChPersonMaritalStatus(XElement xMaritalStatus)
		{
			var status = EChDataLoader.GetChildStringValue (xMaritalStatus, EChXmlTags.ECh0011.MaritalStatus);

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
				return EChDataLoader.GetEChAddress (xAddress);
			}
			else
			{
				return null;
			}
		}


		public static EChAddress GetEChAddress(XElement xAddress)
		{
			var addressLine1      = EChDataLoader.GetChildStringValue (xAddress, EChXmlTags.ECh0010.AddressLine1);
			var street            = EChDataLoader.GetChildStringValue (xAddress, EChXmlTags.ECh0010.Street);
			var houseNumber       = EChDataLoader.GetChildStringValue (xAddress, EChXmlTags.ECh0010.HouseNumber);
			var town              = EChDataLoader.GetChildStringValue (xAddress, EChXmlTags.ECh0010.Town);
			var swissZipCode      = EChDataLoader.GetChildStringValue (xAddress, EChXmlTags.ECh0010.SwissZipCode);
			var swissZipCodeAddOn = EChDataLoader.GetChildStringValue (xAddress, EChXmlTags.ECh0010.SwissZipCodeAddOn);
			var swissZipCodeId    = EChDataLoader.GetChildStringValue (xAddress, EChXmlTags.ECh0010.SwissZipCodeId);
			var countryCode       = EChDataLoader.GetChildStringValue (xAddress, EChXmlTags.ECh0010.Country);

			return new EChAddress (addressLine1, street, houseNumber, town, swissZipCode, swissZipCodeAddOn, swissZipCodeId, countryCode);
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


		private static readonly string[] yearFormats = new string[] { "yyyy", "'+'yyyy", "'-'yyyy", "yyyyzzz", "'+'yyyyzzz", "'-'yyyyzzz" };


	}


}
