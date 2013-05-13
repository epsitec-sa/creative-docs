using Epsitec.Aider.Enumerations;

using Epsitec.Cresus.Core.Business;

using Epsitec.Common.Types;

using Epsitec.Cresus.DataLayer.Loader;

using System;

using System.Collections.Generic;

using System.Linq;


namespace Epsitec.Aider.Entities
{


	public partial class AiderSubscriptionEntity
	{


		public override FormattedText GetSummary()
		{
			return TextFormatter.FormatText
			(
				"Abonnement N°", this.Id, "\n",
				this.Count, " exemplaire(s)", "\n",
				"Cahier de la ", this.RegionalEdition.Name
			);
		}


		public override FormattedText GetCompactSummary()
		{
			return this.GetSummary ();
		}


		public string GetEditionId()
		{
			// Editions ids should be N1 to NB, which by chance is the hexadecimal representation of
			// the region id.

			var regionId = this.RegionalEdition.GetRegionId ();

			return "N" + regionId.ToString ("X1");
		}


		public AiderAddressEntity GetAddress()
		{
			switch (this.SubscriptionType)
			{
				case SubscriptionType.Household:
					return this.Household.Address;

				case SubscriptionType.LegalPerson:
					return this.LegalPersonContact.Address;

				default:
					throw new NotImplementedException ();
			}
		}


		public static AiderSubscriptionEntity Create
		(
			BusinessContext businessContext,
			AiderHouseholdEntity household,
			AiderGroupEntity regionalEdition,
			int count
		)
		{
			var subscription = AiderSubscriptionEntity.Create
			(
				businessContext, regionalEdition, count
			);

			subscription.SubscriptionType = SubscriptionType.Household;
			subscription.Household = household;

			return subscription;
		}


		public static AiderSubscriptionEntity Create
		(
			BusinessContext businessContext,
			AiderContactEntity legalPersonContact,
			AiderGroupEntity regionalEdition,
			int count
		)
		{
			var subscription = AiderSubscriptionEntity.Create
			(
				businessContext, regionalEdition, count
			);

			subscription.SubscriptionType = SubscriptionType.LegalPerson;
			subscription.LegalPersonContact = legalPersonContact;

			return subscription;
		}


		private static AiderSubscriptionEntity Create
		(
			BusinessContext businessContext,
			AiderGroupEntity regionalEdition,
			int count
		)
		{
			var subscription = businessContext.CreateAndRegisterEntity<AiderSubscriptionEntity> ();

			subscription.Count = count;
			subscription.RegionalEdition = regionalEdition;

			return subscription;
		}


		public static void Delete
		(
			BusinessContext businessContext,
			AiderSubscriptionEntity subscription
		)
		{
			businessContext.DeleteEntity (subscription);
		}


		public static AiderSubscriptionEntity FindSubscription
		(
			BusinessContext businessContext,
			AiderHouseholdEntity household
		)
		{
			var example = new AiderSubscriptionEntity ()
			{
				SubscriptionType = SubscriptionType.Household,
				Household = household,
			};

			return AiderSubscriptionEntity.FindSubscription (businessContext, example);
		}


		public static AiderSubscriptionEntity FindSubscription
		(
			BusinessContext businessContext,
			AiderContactEntity legalPersonContact
		)
		{
			var example = new AiderSubscriptionEntity ()
			{
				SubscriptionType = SubscriptionType.LegalPerson,
				LegalPersonContact = legalPersonContact,
			};

			return AiderSubscriptionEntity.FindSubscription (businessContext, example);
		}


		public static IList<AiderSubscriptionEntity> FindSubscriptions
		(
			BusinessContext businessContext,
			AiderLegalPersonEntity legalPerson
		)
		{
			var example = new AiderSubscriptionEntity ()
			{
				SubscriptionType = SubscriptionType.LegalPerson,
				LegalPersonContact = new AiderContactEntity ()
				{
					ContactType = ContactType.Legal,
					LegalPerson = legalPerson
				},
			};

			return businessContext.DataContext.GetByExample (example);
		}


		private static AiderSubscriptionEntity FindSubscription
		(
			BusinessContext businessContext,
			AiderSubscriptionEntity example
		)
		{
			var request = new Request ()
			{
				RootEntity = example,
			};

			var dataContext = businessContext.DataContext;
			var result = dataContext.GetByRequest<AiderSubscriptionEntity> (request);

			return result.FirstOrDefault ();
		}


	}


}
