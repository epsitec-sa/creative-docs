using Epsitec.Aider.Data.Common;
using Epsitec.Aider.Entities;
using Epsitec.Aider.Enumerations;

using Epsitec.Cresus.Bricks;
using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Business.UserManagement;
using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Core.Controllers.CreationControllers;
using Epsitec.Cresus.Core.Entities;

using System.Linq;

namespace Epsitec.Aider.Controllers.CreationControllers
{
	[ControllerSubType (0)]
	public sealed class BrickCreationAiderHouseholdViewController0 : BrickCreationViewController<AiderHouseholdEntity>
	{
		protected override void GetForm(ActionBrick<AiderHouseholdEntity, SimpleBrick<AiderHouseholdEntity>> action)
		{
			var currentUser = UserManager.Current.AuthenticatedUser;
			
			var towns     = AiderTownEntity.GetTownFavoritesByUserScope (this.BusinessContext, currentUser as AiderUserEntity);
			var countries = AiderCountryEntity.GetCountryFavorites (this.BusinessContext);
			var switzerland = this.BusinessContext.GetLocalEntity (countries.FirstOrDefault (x => x.IsoCode == "CH"));

			action
				.Title ("Créer un nouveau ménage")
				.Field<PersonMrMrs> ()
					.Title ("Appellation du chef de famille")
					.InitialValue (PersonMrMrs.Monsieur)
				.End ()
				.Field<string> ()
					.Title ("Prénom du chef de famille")
				.End ()
				.Field<string> ()
					.Title ("Nom du chef de famille")
				.End ()
				.Field<AiderTownEntity> ()
					.Title ("Localité")
					.WithFavorites (towns)
				.End ()
				.Field<string> ()
					.Title ("Rue et numéro (avec complément)")
				.End ()
				.Field<PersonConfession> ()
					.Title ("Confession du chef de famille")
				.End ()
				.Field<AiderCountryEntity> ()
					.Title ("Nationalité du chef de famille")
					.InitialValue (switzerland)
					.WithFavorites (countries)
				.End ();
		}

		public override FunctionExecutor GetExecutor()
		{
			return FunctionExecutor.Create<PersonMrMrs, string, string, AiderTownEntity, string, PersonConfession, AiderCountryEntity, AiderHouseholdEntity> (this.Execute);
		}

		private AiderHouseholdEntity Execute(PersonMrMrs mrMrs, string firstname, string lastname, AiderTownEntity town, string streetHouseNumberAndComplement, PersonConfession confession, AiderCountryEntity nationality)
		{
			if (mrMrs == PersonMrMrs.None)
			{
				throw new BusinessRuleException ("L'appellation est obligatoire");
			}

			if (string.IsNullOrEmpty (firstname))
			{
				throw new BusinessRuleException ("Le prénom est obligatoire");
			}

			if (string.IsNullOrEmpty (lastname))
			{
				throw new BusinessRuleException ("Le nom est obligatoire");
			}

			if (town.IsNull ())
			{
				throw new BusinessRuleException ("La localité est obligatoire");
			}

			if (string.IsNullOrEmpty (streetHouseNumberAndComplement))
			{
				throw new BusinessRuleException ("La rue est obligatoire");
			}

			var household = this.BusinessContext.CreateAndRegisterEntity<AiderHouseholdEntity> ();

			var address = household.Address;
			address.Town = town;
			address.StreetHouseNumberAndComplement = streetHouseNumberAndComplement;

			var person = this.BusinessContext.CreateAndRegisterEntity<AiderPersonEntity> ();
			person.MrMrs = mrMrs;
			person.Confession = confession;
			person.eCH_Person.PersonFirstNames = firstname;
			person.eCH_Person.PersonOfficialName = lastname;
			person.eCH_Person.PersonSex = mrMrs == PersonMrMrs.Monsieur ? PersonSex.Male : PersonSex.Female;
			person.eCH_Person.Nationality = nationality;

			AiderContactEntity.Create (this.BusinessContext, person, household, true);

			// This is a hack around a limitation in the business rules. The business rules of the
			// person will assign the parish to the person, but it will do it only after that the
			// business rule of the household has used the (at the time null) parish of the person
			// to set its path. We come back to this old problem: there is no dependency management
			// between the execution of the business rules and we must pray that they execute in
			// the order in which we think they will.
			// So here we force the parish assignation now, so we know for sure that the household
			// will have its parish group path cache set correctly.
			var parishRepository = ParishAddressRepository.Current;
			ParishAssigner.AssignToParish (parishRepository, this.BusinessContext, person);

			var generateSubscription = address.Town.SwissCantonCode.Equals ("VD");
			if (generateSubscription)
			{
				// Here we know that the parish has been set up just before, so we reuse that
				// information to get the subscription group. If the address is not within a parish
				// we use the region of Lausanne by default.
				var region = person.ParishGroup.IsNotNull()
					? person.ParishGroup.Parent
					: ParishAssigner.FindRegionGroup (this.BusinessContext, 4);

				AiderSubscriptionEntity.Create (this.BusinessContext, household, region, 1);
			}

			return household;
		}
	}
}
