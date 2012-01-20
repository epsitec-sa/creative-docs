using System.Xml.Linq;


namespace Epsitec.Aider.Data
{

	
	internal static class EChXmlTags
	{


		public static class EVd0002
		{


			private static readonly XNamespace Namespace = "http://evd.vd.ch/xmlns/eVD-0002/1";


			public static readonly XName Address = EVd0002.Namespace + "address";
			public static readonly XName Adult = EVd0002.Namespace + "adult";
			public static readonly XName Children = EVd0002.Namespace + "children";
			public static readonly XName Child = EVd0002.Namespace + "child";
			public static readonly XName MaritalStatus = EVd0002.Namespace + "maritalStatus";
			public static readonly XName Nationality = EVd0002.Namespace + "nationality";
			public static readonly XName Origin = EVd0002.Namespace + "origins";
			public static readonly XName Person = EVd0002.Namespace + "person";
			public static readonly XName ReportedPerson = EVd0002.Namespace + "reportedPerson";


		}


		public static class EVd0004
		{


			private static readonly XNamespace Namespace = "http://evd.vd.ch/xmlns/eVD-0004/1";


			public static readonly XName DateOfBirth = EVd0004.Namespace + "dateOfBirth";
			public static readonly XName FirstNames = EVd0004.Namespace + "firstNames";
			public static readonly XName OfficialName = EVd0004.Namespace + "officialName";
			public static readonly XName PersonId = EVd0004.Namespace + "personId";
			public static readonly XName Sex = EVd0004.Namespace + "sex";

		}


		public static class ECh0008
		{


			private static readonly XNamespace Namespace = "http://www.ech.ch/xmlns/eCH-0008/2";


			public static readonly XName CountryIdIso2 = ECh0008.Namespace + "countryIdISO2";


		}


		public static class ECh0010
		{


			private static readonly XNamespace Namespace = "http://www.ech.ch/xmlns/eCH-0010/3";


			public static readonly XName AddressLine1 = ECh0010.Namespace + "addressLine1";
			public static readonly XName Street = ECh0010.Namespace + "street";
			public static readonly XName HouseNumber = ECh0010.Namespace + "houseNumber";
			public static readonly XName Town = ECh0010.Namespace + "town";
			public static readonly XName SwissZipCode = ECh0010.Namespace + "swissZipCode";
			public static readonly XName SwissZipCodeAddOn = ECh0010.Namespace + "swissZipCodeAddOn";
			public static readonly XName SwissZipCodeId = ECh0010.Namespace + "swissZipCodeId";
			public static readonly XName Country = ECh0010.Namespace + "country";


		}


		public static class ECh0011
		{


			private static readonly XNamespace Namespace = "http://www.ech.ch/xmlns/eCH-0011/3";


			public static readonly XName NationalityStatus = ECh0011.Namespace + "nationalityStatus";
			public static readonly XName Country = ECh0011.Namespace + "country";
			public static readonly XName OriginName = ECh0011.Namespace + "originName";
			public static readonly XName Canton = ECh0011.Namespace + "canton";
			public static readonly XName MaritalStatus = ECh0011.Namespace + "maritalStatus";


		}


		public static class ECh0044
		{


			private static readonly XNamespace Namespace = "http://www.ech.ch/xmlns/eCH-0044/1";


			public static readonly XName PersonId = ECh0044.Namespace + "personId";
			public static readonly XName Year = ECh0044.Namespace + "year";
			public static readonly XName YearMonth = ECh0044.Namespace + "yearMonth";
			public static readonly XName YearMonthDay = ECh0044.Namespace + "yearMonthDay";


		}


	}


}

