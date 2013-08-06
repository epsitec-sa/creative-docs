//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Aider.Data.Common;
using Epsitec.Aider.Entities;
using Epsitec.Aider.Enumerations;

using Epsitec.Cresus.Bricks;

using Epsitec.Cresus.Core.Business;

using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Core.Controllers.CreationControllers;

using Epsitec.Cresus.Core.Entities;

using Epsitec.Cresus.Core.Business.UserManagement;

namespace Epsitec.Aider.Controllers.CreationControllers
{
	[ControllerSubType (0)]
	public sealed class BrickCreationAiderLegalPersonViewController0 : BrickCreationViewController<AiderLegalPersonEntity>
	{
		protected override void GetForm(ActionBrick<AiderLegalPersonEntity, SimpleBrick<AiderLegalPersonEntity>> action)
		{
			var currentUser = UserManager.Current.AuthenticatedUser;
			var favorites = AiderTownEntity.GetTownFavoritesByUserScope (this.BusinessContext, currentUser as AiderUserEntity);

			action
				.Title ("Créer une nouvelle personne morale")
				.Field<string> ()
					.Title ("Nom de la société/entreprise/association/...")
				.End ()
				.Field<ContactRole> ()
					.Title ("Role de la personne de contact (optionnel)")
					.InitialValue (ContactRole.None)
				.End ()
				.Field<PersonMrMrs> ()
					.Title ("Appellation de la personne de contact (optionnel)")
					.InitialValue (PersonMrMrs.None)
				.End ()
				.Field<string> ()
					.Title ("Nom de la personne de contact (optionnel)")
				.End ()
				.Field<AiderTownEntity> ()
					.Title ("Localité")
					.WithFavorites (favorites)
				.End ()
				.Field<string> ()
					.Title ("Rue avec numéro de maison")
				.End ()
				.Field<string> ()
					.Title ("Case postale")
				.End ()
				.Field<bool> ()
					.Title ("Inscription au Bonne Nouvelle")
					.InitialValue (false)
				.End ();
		}

		public override FunctionExecutor GetExecutor()
		{
			return FunctionExecutor.Create<string, ContactRole, PersonMrMrs, string, AiderTownEntity, string, string, bool, AiderLegalPersonEntity> (this.Execute);
		}

		private AiderLegalPersonEntity Execute(string name, ContactRole personRole, PersonMrMrs personMrMrs, string personName, AiderTownEntity town, string street, string postBox, bool generateSubscription)
		{
			if (string.IsNullOrEmpty (name))
			{
				throw new BusinessRuleException ("Le nom de la société est obligatoire");
			}

			if (town.IsNull ())
			{
				throw new BusinessRuleException ("La localité est obligatoire");
			}

			if (string.IsNullOrEmpty (street))
			{
				throw new BusinessRuleException ("La rue est obligatoire");
			}

			var legalPerson = this.BusinessContext.CreateAndRegisterEntity<AiderLegalPersonEntity> ();
			legalPerson.Name = name;

			var address = legalPerson.Address;
			address.Town = town;
			address.StreetHouseNumberAndComplement = street;
			address.PostBox = postBox;

			var contact = AiderContactEntity.Create (this.BusinessContext, legalPerson, personMrMrs, personName, personRole);

			// Here we assign directly the parish, so we can use this information later on if there
			// is a subscription to generate. This is not strictly necessery here, as it would be
			// done by the business rules, and the business rules of the legal person are called
			// before the business rules of the contact, so the contact business rules can safely
			// use the parish information from the legal person to set up its parish group path
			// cache.
			var parishRepository = ParishAddressRepository.Current;
			ParishAssigner.AssignToParish(parishRepository, this.BusinessContext, legalPerson);

			if (generateSubscription)
			{
				// Here we know that the parish has been set up just before, so we reuse that
				// information to get the subscription group. If the address is not within a parish
				// we use the region of Lausanne by default.
				var region = legalPerson.ParishGroup.IsNotNull ()
					? legalPerson.ParishGroup.Parent
					: ParishAssigner.FindRegionGroup (this.BusinessContext, 4);

				AiderSubscriptionEntity.Create (this.BusinessContext, contact, region, 1);
			}

			return legalPerson;
		}
	}
}
