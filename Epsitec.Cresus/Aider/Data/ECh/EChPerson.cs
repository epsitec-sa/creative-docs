using Epsitec.Aider.Enumerations;

using Epsitec.Common.Support.Extensions;

using Epsitec.Common.Types;

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Xml.Linq;


namespace Epsitec.Aider.Data.ECh
{


	internal sealed class EChPerson
	{


		// NOTE: Here we discard the fields person.personId.personIdCategory,
		// nationality.country.countryId, nationality.country.countryNameShort,
		// maritalStatus.dateOfMaritalStatus, maritalStatus.separation,
		// maritalStatus.dateOfSeparation and maritalStatus.cancelationReason.


		public EChPerson(string id, string officialname, string firstNames, Date dateOfBirth, PersonSex sex, PersonNationalityStatus nationalityStatus, string nationalCountryCode, IEnumerable<EChPlace> originPlaces, PersonMaritalStatus maritalStatus)
		{
			this.Id = id;
			this.OfficialName = officialname;
			this.FirstNames = firstNames;
			this.DateOfBirth = dateOfBirth;
			this.Sex = sex;
			this.NationalityStatus = nationalityStatus;
			this.NationalCountryCode = nationalCountryCode;
			this.OriginPlaces = originPlaces.AsReadOnlyCollection ();
			this.MaritalStatus = maritalStatus;
		}

		public EChPerson(string id, string officialname, string firstNames, Date dateOfBirth, PersonSex sex, PersonNationalityStatus nationalityStatus, string nationalCountryCode, IEnumerable<EChPlace> originPlaces, PersonMaritalStatus maritalStatus,XElement Xml)
		{
			this.Id = id;
			this.OfficialName = officialname;
			this.FirstNames = firstNames;
			this.DateOfBirth = dateOfBirth;
			this.Sex = sex;
			this.NationalityStatus = nationalityStatus;
			this.NationalCountryCode = nationalCountryCode;
			this.OriginPlaces = originPlaces.AsReadOnlyCollection ();
			this.MaritalStatus = maritalStatus;
			this.Xml = Xml;
		}

		public bool CheckData(string on, string fn, Date dob, PersonSex s, PersonNationalityStatus ns, string ncc, PersonMaritalStatus ms, ReadOnlyCollection<EChPlace> op)
		{
			bool result = this.OfficialName == on && this.FirstNames == fn && this.DateOfBirth == dob && this.Sex == s && this.NationalityStatus == ns && this.NationalCountryCode == ncc && this.MaritalStatus == ms && this.OriginPlaces == op;
			return result;
		}

		public XElement GetXml()
		{
			return new XElement (this.Xml);
		}

		public readonly string Id;
		public readonly string OfficialName;
		public readonly string FirstNames;
		public readonly Date DateOfBirth;
		public readonly PersonSex Sex;
		public readonly PersonNationalityStatus NationalityStatus;
		public readonly string NationalCountryCode;
		public readonly ReadOnlyCollection<EChPlace> OriginPlaces;
		public readonly PersonMaritalStatus MaritalStatus;

		//Used by EChDataComparer
		private XElement Xml;


	}


}
