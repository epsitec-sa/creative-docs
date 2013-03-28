

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Xml.Linq;

namespace Data.Platform.EchDataComparer
{


	internal sealed class EChPerson
	{


		// NOTE: Here we discard the fields person.personId.personIdCategory,
		// nationality.country.countryId, nationality.country.countryNameShort,
		// maritalStatus.dateOfMaritalStatus, maritalStatus.separation,
		// maritalStatus.dateOfSeparation and maritalStatus.cancelationReason.


		public EChPerson(string id, string officialname, string firstNames, string dateOfBirth, string sex, string nationalityStatus, string nationalCountryCode, IEnumerable<EChPlace> originPlaces, string maritalStatus,XElement xml)
		{
			this.Id = id;
			this.OfficialName = officialname;
			this.FirstNames = firstNames;
			this.DateOfBirth = dateOfBirth;
			this.Sex = sex;
			this.NationalityStatus = nationalityStatus;
			this.NationalCountryCode = nationalCountryCode;
			this.OriginPlaces = originPlaces.ToList().AsReadOnly ();
			this.MaritalStatus = maritalStatus;
			this.xml = new XElement(xml);
		}


		public bool CheckData(string on,string fn,string dob, string s, string ns, string ncc, string ms,string op)
		{
			bool result = this.OfficialName == on && this.FirstNames == fn && this.DateOfBirth == dob && this.Sex == s && this.NationalityStatus == ns && this.NationalCountryCode == ncc && this.MaritalStatus == ms && string.Join ("", this.OriginPlaces.Select (k => k.Name + k.Canton)) == op;
			return result;
		}

		public XElement GetXml()
		{
			return new XElement (this.xml);
		}

		public readonly string Id;
		public readonly string OfficialName;
		public readonly string FirstNames;
		public readonly string DateOfBirth;
		public readonly string Sex;
		public readonly string NationalityStatus;
		public readonly string NationalCountryCode;
		public readonly ReadOnlyCollection<EChPlace> OriginPlaces;
		public readonly string MaritalStatus;
		private XElement xml;

	}


}
