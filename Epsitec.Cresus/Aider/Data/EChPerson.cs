using Epsitec.Aider.Enumerations;

using Epsitec.Common.Support.Extensions;

using System.Collections.Generic;
using System.Collections.ObjectModel;



namespace Epsitec.Aider.Data
{


	internal sealed class EChPerson
	{


		// NOTE: Here we discard the fields person.personId.personIdCategory,
		// nationality.country.countryId, nationality.country.countryNameShort,
		// maritalStatus.dateOfMaritalStatus, maritalStatus.separation,
		// maritalStatus.dateOfSeparation and maritalStatus.cancelationReason.


		public EChPerson(string id, string officialname, string firstNames, int? dateOfBirthDay, int? dateOfBirthMonth, int? dateOfBirthYear, DatePrecision dateOfBirthPrecision, PersonSex sex, PersonNationalityStatus nationalityStatus, string nationalCountryCode, IEnumerable<EChPlace> originPlaces, PersonMaritalStatus maritalStatus)
		{
			this.Id = id;
			this.OfficialName = officialname;
			this.FirstNames = firstNames;
			this.DateOfBirthDay = dateOfBirthDay;
			this.DateOfBirthMonth = dateOfBirthMonth;
			this.DateOfBirthYear = dateOfBirthYear;
			this.DateOfBirthPrecision = dateOfBirthPrecision;
			this.Sex = sex;
			this.NationalityStatus = nationalityStatus;
			this.NationalCountryCode = nationalCountryCode;
			this.OriginPlaces = originPlaces.AsReadOnlyCollection ();
			this.MaritalStatus = maritalStatus;
		}


		public readonly string Id;
		public readonly string OfficialName;
		public readonly string FirstNames;
		public readonly int? DateOfBirthDay;
		public readonly int? DateOfBirthMonth;
		public readonly int? DateOfBirthYear;
		public readonly DatePrecision DateOfBirthPrecision;
		public readonly PersonSex Sex;
		public readonly PersonNationalityStatus NationalityStatus;
		public readonly string NationalCountryCode;
		public readonly ReadOnlyCollection<EChPlace> OriginPlaces;
		public readonly PersonMaritalStatus MaritalStatus;


	}


}
