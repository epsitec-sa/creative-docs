using Epsitec.Aider.Enumerations;

using Epsitec.Common.Support.Extensions;

using System;

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


		public EChPerson(string id, string officialname, string firstNames, DateTime dateOfBirth, PersonSex sex, PersonNationalityStatus nationalityStatus, string nationalCountryCode, IEnumerable<EChPlace> originPlaces, PersonMaritalStatus maritalStatus)
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


		public readonly string Id;
		public readonly string OfficialName;
		public readonly string FirstNames;
		public readonly DateTime DateOfBirth;
		public readonly PersonSex Sex;
		public readonly PersonNationalityStatus NationalityStatus;
		public readonly string NationalCountryCode;
		public readonly ReadOnlyCollection<EChPlace> OriginPlaces;
		public readonly PersonMaritalStatus MaritalStatus;


	}


}
