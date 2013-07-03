using Epsitec.Aider.Enumerations;

using Epsitec.Common.Support.Extensions;

using Epsitec.Common.Types;

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Xml.Linq;


namespace Epsitec.Aider.Data.ECh
{

    [System.Serializable]
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

		public bool CheckData(string on, string fn, Date dob, PersonSex s, PersonNationalityStatus ns, string ncc, PersonMaritalStatus ms, ReadOnlyCollection<EChPlace> op)
		{
			var originList= op.Select (p => System.Tuple.Create (p.Canton, p.Name)).ToList ();
			var originListToCompare = this.OriginPlaces.Select(p => System.Tuple.Create (p.Canton, p.Name)).ToList ();

			bool result = this.OfficialName == on && this.FirstNames == fn && this.DateOfBirth == dob && this.Sex == s && this.NationalityStatus == ns && this.NationalCountryCode == ncc && this.MaritalStatus == ms && originList.SetEquals(originListToCompare);
			return result;
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


	}


}
