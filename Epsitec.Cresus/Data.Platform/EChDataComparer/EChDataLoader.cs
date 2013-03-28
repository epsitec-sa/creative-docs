using System;

using System.Collections.Generic;

using System.IO;

using System.Linq;

using System.Reflection;

using System.Xml.Linq;


namespace Data.Platform.EchDataComparer
{


	internal static class EChDataLoader
	{


		public static IList<EChReportedPerson> Load(FileInfo inputFile, int maxCount = int.MaxValue)
		{
			var xDocument = EChDataLoader.GetDocument (inputFile);

			

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

			return new EChReportedPerson (adult1, adult2, children, address,xReportedPerson);
		}


		public static IEnumerable<EChPerson> GetAdults(XElement xReportedPerson)
		{
			foreach (var xAdult in xReportedPerson.Elements (EChXmlTags.EVd0002.Adult))
			{
				yield return EChDataLoader.GetEChPerson (xAdult);
			}
		}


		public static IEnumerable<EChPerson> GetChildren(XElement xReportedPerson)
		{
			var xChildren = xReportedPerson.Element (EChXmlTags.EVd0002.Children);

			if (xChildren != null)
			{
				foreach (var xChild in xChildren.Elements (EChXmlTags.EVd0002.Child))
				{
					yield return EChDataLoader.GetEChPerson (xChild);
				}
			}
		}


		public static EChPerson GetEChPerson(XElement xEChPerson)
		{
			string id = null;
			string officialName = null;
			string firstNames = null;
			string dateOfBirth = null;
			string sex = null;

			var xPerson = xEChPerson.Element (EChXmlTags.EVd0002.Person);

			if (xPerson != null)
			{
				id = EChDataLoader.GetEChPersonId (xPerson);
				officialName = EChDataLoader.GetEChPersonOfficialName (xPerson);
				firstNames = EChDataLoader.GetEChPersonFirstNames (xPerson);
				dateOfBirth = EChDataLoader.GetEChPersonDateOfBirth (xPerson);
				sex = EChDataLoader.GetEChPersonSex (xPerson);
			}

			string nationalityStatus = null;
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

			string maritalStatus = "";

			var xMaritalStatus = xEChPerson.Element (EChXmlTags.EVd0002.MaritalStatus);

			if (xMaritalStatus != null)
			{
				maritalStatus = EChDataLoader.GetEChPersonMaritalStatus (xMaritalStatus);
			}

			return new EChPerson (id, officialName, firstNames, dateOfBirth, sex, nationalityStatus, nationalCountryCode, originPlaces, maritalStatus,xPerson);
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
			var name = EChDataLoader.GetChildStringValue (xPerson, EChXmlTags.EVd0004.OfficialName);

			return name;
		}


		private static string GetEChPersonFirstNames(XElement xPerson)
		{
			var name = EChDataLoader.GetChildStringValue (xPerson, EChXmlTags.EVd0004.FirstNames);

			return name;
		}


		private static String GetEChPersonDateOfBirth(XElement xPerson)
		{
			var xDateOfBirth = xPerson.Element (EChXmlTags.EVd0004.DateOfBirth);

			if (xDateOfBirth==null)
			{
				return "";
			}

			var xDateOfBirthChild = xDateOfBirth.Elements ().Single ();
			var xDateOfBirthChildName = xDateOfBirthChild.Name;

			if (xDateOfBirthChildName == EChXmlTags.ECh0044.Year)
			{
				return xDateOfBirthChild.Value;
				
			}


			return xDateOfBirthChild.Value;
		}


		public static String GetEChPersonSex(XElement xPerson)
		{
			var sex = EChDataLoader.GetChildStringValue (xPerson, EChXmlTags.EVd0004.Sex);

			return sex;
		}


		public static String GetEChPersonNationalityStatus(XElement xNationality)
		{
			var status = EChDataLoader.GetChildStringValue (xNationality, EChXmlTags.ECh0011.NationalityStatus);

			return status;
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


		public static string GetEChPersonMaritalStatus(XElement xMaritalStatus)
		{
			var status = EChDataLoader.GetChildStringValue (xMaritalStatus, EChXmlTags.ECh0011.MaritalStatus);

			return status;
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
			var addressLine1 = EChDataLoader.GetChildStringValue (xAddress, EChXmlTags.ECh0010.AddressLine1);
			var street = EChDataLoader.GetChildStringValue (xAddress, EChXmlTags.ECh0010.Street);

			var rawHouseNumber = EChDataLoader.GetChildStringValue (xAddress, EChXmlTags.ECh0010.HouseNumber);
			

			var town = EChDataLoader.GetChildStringValue (xAddress, EChXmlTags.ECh0010.Town);

			var rawSwissZipCode = EChDataLoader.GetChildStringValue (xAddress, EChXmlTags.ECh0010.SwissZipCode);
			var swissZipCode = rawSwissZipCode;

			var rawSwissZipCodeAddOn = EChDataLoader.GetChildStringValue (xAddress, EChXmlTags.ECh0010.SwissZipCodeAddOn);
			var swissZipCodeAddOn = rawSwissZipCodeAddOn;

			var rawSwissZipCodeId = EChDataLoader.GetChildStringValue (xAddress, EChXmlTags.ECh0010.SwissZipCodeId);
			var swissZipCodeId = rawSwissZipCodeId;

			var countryCode = EChDataLoader.GetChildStringValue (xAddress, EChXmlTags.ECh0010.Country);

			

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


		private static readonly string[] yearFormats = new string[] { "yyyy", "'+'yyyy", "'-'yyyy", "yyyyzzz", "'+'yyyyzzz", "'-'yyyyzzz" };


	}


}
