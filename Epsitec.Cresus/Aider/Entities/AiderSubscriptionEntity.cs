using Epsitec.Aider.Enumerations;

using Epsitec.Cresus.Core.Business;

using Epsitec.Common.Types;

using System;

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


		public string GetHonorific()
		{
			switch (this.SubscriptionType)
			{
				case SubscriptionType.Household:
					return this.Household.GetHonorific ();

				case SubscriptionType.LegalPerson:
					// TODO Implement this.
					throw new NotImplementedException ();

				default:
					throw new NotImplementedException ();
			}
		}


		public string GetLastname()
		{
			switch (this.SubscriptionType)
			{
				case SubscriptionType.Household:
					return this.Household.GetLastname ();

				case SubscriptionType.LegalPerson:
					// TODO Implement this.
					throw new NotImplementedException ();

				default:
					throw new NotImplementedException ();
			}
		}


		public string GetFirstname()
		{
			switch (this.SubscriptionType)
			{
				case SubscriptionType.Household:
					// TODO Implement this
					return "";

				case SubscriptionType.LegalPerson:
					// TODO Implement this.
					throw new NotImplementedException ();

				default:
					throw new NotImplementedException ();
			}
		}


		public string GetAddressComplement()
		{
			var address = this.GetAddress ();

			switch (this.SubscriptionType)
			{
				case SubscriptionType.Household:
					// If the household has a post box, we drop the complement if there is one and
					// we replace it by the post box.
					return string.IsNullOrEmpty (address.PostBox)
						? address.AddressLine1
						: address.PostBox;

				case SubscriptionType.LegalPerson:
					return address.AddressLine1;

				default:
					throw new NotImplementedException ();
			}
		}


		public AiderAddressEntity GetAddress()
		{
			switch (this.SubscriptionType)
			{
				case SubscriptionType.Household:
					return this.Household.Address;

				case SubscriptionType.LegalPerson:
					return this.LegalPerson.Address;

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
