using Epsitec.Aider.Enumerations;

using Epsitec.Cresus.Core.Business;

using Epsitec.Common.Types;


namespace Epsitec.Aider.Entities
{


	public partial class AiderSubscriptionEntity
	{


		public override FormattedText GetSummary()
		{
			return "Subscription summary";
		}


		public override FormattedText GetCompactSummary()
		{
			return this.GetSummary ();
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
			AiderLegalPersonEntity legalPerson,
			AiderGroupEntity regionalEdition,
			int count
		)
		{
			var subscription = AiderSubscriptionEntity.Create
			(
				businessContext, regionalEdition, count
			);

			subscription.SubscriptionType = SubscriptionType.LegalPerson;
			subscription.LegalPerson = legalPerson;

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


	}


}
