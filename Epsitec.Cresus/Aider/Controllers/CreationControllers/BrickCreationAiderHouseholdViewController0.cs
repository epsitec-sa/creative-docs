using Epsitec.Aider.Entities;
using Epsitec.Aider.Enumerations;

using Epsitec.Cresus.Bricks;

using Epsitec.Cresus.Core.Business;

using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Core.Controllers.CreationControllers;

using Epsitec.Cresus.Core.Entities;


namespace Epsitec.Aider.Controllers.CreationControllers
{
	[ControllerSubType (0)]
	public sealed class BrickCreationAiderHouseholdViewController0 : BrickCreationViewController<AiderHouseholdEntity>
	{
		protected override void GetForm(ActionBrick<AiderHouseholdEntity, SimpleBrick<AiderHouseholdEntity>> action)
		{
			action
				.Title ("Créer un nouveau ménage")
				.Field<string> ()
					.Title ("Prénom du chef de famille")
				.End ()
				.Field<string> ()
					.Title ("Nom du chef de famille")
				.End ()
				.Field<PersonSex> ()
					.Title ("Sexe du chef de famille")
					.InitialValue (PersonSex.Unknown)
				.End ()
				.Field<AiderTownEntity> ()
					.Title ("Localité")
				.End ()
				.Field<string> ()
					.Title ("Rue")
				.End ()
				.Field<string> ()
					.Title ("Numéro de maison (avec complément)")
				.End ();
		}

		public override FunctionExecutor GetExecutor()
		{
			return FunctionExecutor.Create<string, string, PersonSex, AiderTownEntity, string, string, AiderHouseholdEntity> (this.Execute);
		}

		private AiderHouseholdEntity Execute(string firstname, string lastname, PersonSex sex, AiderTownEntity town, string street, string number)
		{
			if (string.IsNullOrEmpty (firstname))
			{
				throw new BusinessRuleException ("Le prénom est obligatoire");
			}

			if (string.IsNullOrEmpty (lastname))
			{
				throw new BusinessRuleException ("Le nom est obligatoire");
			}

			if (town.IsNull())
			{
				throw new BusinessRuleException ("La localité est obligatoire");
			}

			if (string.IsNullOrEmpty (street))
			{
				throw new BusinessRuleException ("La rue est obligatoire");
			}

			var household = this.BusinessContext.CreateAndRegisterEntity<AiderHouseholdEntity> ();
			
			var address = household.Address;
			address.Town = town;
			address.StreetUserFriendly = street;
			address.HouseNumberAndComplement = number;

			var person = this.BusinessContext.CreateAndRegisterEntity<AiderPersonEntity> ();
			person.eCH_Person.PersonFirstNames = firstname;
			person.eCH_Person.PersonOfficialName = lastname;
			person.eCH_Person.PersonSex = sex;
			AiderContactEntity.Create (this.BusinessContext, person, household, true);

			return household;
		}
	}
}
