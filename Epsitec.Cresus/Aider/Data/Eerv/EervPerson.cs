using Epsitec.Aider.Enumerations;

using Epsitec.Common.Types;

using System;



namespace Epsitec.Aider.Data.Eerv
{


	internal sealed class EervPerson
	{

		public EervPerson(string id, string firstname1, string firstname2, string lastname, string originalName, string corporateName, Date? dateOfBirth, Date? dateOfDeath, string honorific, PersonSex sex, PersonMaritalStatus maritalStatus, string origins, string profession, PersonConfession confession, string emailAddress, string mobilPhoneNumber, string remarks, string father, string mother, string placeOfBirth, string placeOfBaptism, Date? dateOfBaptism, string placeOfChildBenediction, Date? dateOfChildBenediction, string placeOfCatechismBenediction, Date? dateOfCatechismBenediction, int? schoolYearOffset, string householdId, int householdRank)
		{
			this.Id = id;

			this.Firstname1 = firstname1;
			this.Firstname2 = firstname2;
			this.Lastname = lastname;
			this.OriginalName =	originalName;
			this.CorporateName = corporateName;

			this.DateOfBirth = dateOfBirth;
			this.DateOfDeath = dateOfDeath;

			this.Honorific = honorific;
			this.Sex = sex;
			this.MaritalStatus = maritalStatus;
			this.Origins = origins;
			this.Profession = profession;
			this.Confession = confession;
			this.EmailAddress =	 emailAddress;
			this.MobilPhoneNumber = mobilPhoneNumber;
			this.Remarks =	 remarks;

			this.Father = father;
			this.Mother = mother;
			this.PlaceOfBirth = placeOfBirth;
			this.PlaceOfBaptism = placeOfBaptism;
			this.DateOfBaptism = dateOfBaptism;
			this.PlaceOfChildBenediction = placeOfChildBenediction;
			this.DateOfChildBenediction = dateOfChildBenediction;
			this.PlaceOfCatechismBenediction = placeOfCatechismBenediction;
			this.DateOfCatechismBenediction = dateOfCatechismBenediction;
			this.SchoolYearOffset = schoolYearOffset;

			this.HouseholdId = householdId;
			this.HouseholdRank = householdRank;
		}



		public readonly string Id;

		public readonly string Firstname1;
		public readonly string Firstname2;
		public readonly string Lastname;
		public readonly string OriginalName;
		public readonly string CorporateName;

		public readonly Date? DateOfBirth;
		public readonly Date? DateOfDeath;

		public readonly string Honorific;
		public readonly PersonSex Sex;
		public readonly PersonMaritalStatus MaritalStatus;
		public readonly string Origins;
		public readonly string Profession;
		public readonly PersonConfession Confession;
		public readonly string EmailAddress;
		public readonly string MobilPhoneNumber;
		public readonly string Remarks;

		public readonly string Father;
		public readonly string Mother;
		public readonly string PlaceOfBirth;
		public readonly string PlaceOfBaptism;
		public readonly Date? DateOfBaptism;
		public readonly string PlaceOfChildBenediction;
		public readonly Date? DateOfChildBenediction;
		public readonly string PlaceOfCatechismBenediction;
		public readonly Date? DateOfCatechismBenediction;
		public readonly int? SchoolYearOffset;

		public readonly string HouseholdId;
		public readonly int HouseholdRank;


	}


}
