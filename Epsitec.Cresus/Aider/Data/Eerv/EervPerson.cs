using Epsitec.Aider.Enumerations;

using Epsitec.Common.Support.Extensions;

using Epsitec.Common.Types;

using System.Collections.Generic;

using System.Linq;


namespace Epsitec.Aider.Data.Eerv
{


	internal sealed class EervPerson : Freezable
	{


		public EervPerson(string id, string firstname, string lastname, string originalName, Date? dateOfBirth, Date? dateOfDeath, string honorific, PersonSex sex, PersonMaritalStatus maritalStatus, string origins, string profession, PersonConfession confession, string remarks, string father, string mother, string placeOfBirth, string placeOfBaptism, Date? dateOfBaptism, string placeOfChildBenediction, Date? dateOfChildBenediction, string placeOfCatechismBenediction, Date? dateOfCatechismBenediction, int? schoolYearOffset, EervCoordinates coordinates)
		{
			this.Id = id;
			this.Firstname = firstname;
			this.Lastname = lastname;
			this.OriginalName =	originalName;
			this.DateOfBirth = dateOfBirth;
			this.DateOfDeath = dateOfDeath;
			this.Honorific = honorific;
			this.Sex = sex;
			this.MaritalStatus = maritalStatus;
			this.Origins = origins;
			this.Profession = profession;
			this.Confession = confession;
			this.Remarks = remarks;

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

			this.Coordinates = coordinates;

			this.activities = new List<EervActivity> ();
		}

		public EervHousehold HouseHold
		{
			get
			{
				return this.houseHold;
			}
			set
			{
				this.ThrowIfReadOnly ();

				this.houseHold = value;
			}
		}


		public IList<EervActivity> Activities
		{
			get
			{
				return this.activities;
			}
		}


		protected override void HandleFreeze()
		{
			base.HandleFreeze ();

			this.activities = this.activities.AsReadOnlyCollection ();
		}


		public readonly string Id;
		public readonly string Firstname;
		public readonly string Lastname;
		public readonly string OriginalName;
		public readonly Date? DateOfBirth;
		public readonly Date? DateOfDeath;
		public readonly string Honorific;
		public readonly PersonSex Sex;
		public readonly PersonMaritalStatus MaritalStatus;
		public readonly string Origins;
		public readonly string Profession;
		public readonly PersonConfession Confession;
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


		public readonly EervCoordinates Coordinates;


		private EervHousehold houseHold;
		private IList<EervActivity> activities;


	}


}
