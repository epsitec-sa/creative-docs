using Epsitec.Aider.Enumerations;

using Epsitec.Cresus.Core.Business;

using Epsitec.Cresus.DataLayer.Loader;

using System;

using System.Collections.Generic;

using System.Linq;


namespace Epsitec.Aider.Entities
{


	public partial class AiderSubscriptionRefusalEntity
	{


		public void RefreshCache()
		{
			this.DisplayName = this.GetDisplayName ();
			this.DisplayZipCode = this.GetDisplayZipCode ();
			this.DisplayAddress = this.GetDisplayAddress ();
		}


		private string GetDisplayName()
		{
			switch (this.RefusalType)
			{
				case SubscriptionType.Household:
					return this.Household.GetDisplayName ();

				case SubscriptionType.LegalPerson:
					return this.LegalPersonContact.GetDisplayName ();

				default:
					throw new NotImplementedException ();
			}
		}


		private string GetDisplayZipCode()
		{
			return this.GetAddress ().GetDisplayZipCode ().ToSimpleText ();
		}


		private string GetDisplayAddress()
		{
			return this.GetAddress ().GetDisplayAddress ().ToSimpleText ();
		}


		public AiderAddressEntity GetAddress()
		{
			switch (this.RefusalType)
			{
				case SubscriptionType.Household:
					return this.Household.Address;

				case SubscriptionType.LegalPerson:
					return this.LegalPersonContact.Address;

				default:
					throw new NotImplementedException ();
			}
		}


		public static AiderSubscriptionRefusalEntity Create
		(
			BusinessContext businessContext,
			AiderHouseholdEntity household
		)
		{
			var refusal = businessContext.CreateAndRegisterEntity<AiderSubscriptionRefusalEntity> ();

			refusal.RefusalType = SubscriptionType.Household;
			refusal.Household = household;

			return refusal;
		}


		public static AiderSubscriptionRefusalEntity Create
		(
			BusinessContext businessContext,
			AiderContactEntity legalPersonContact
		)
		{
			var refusal = businessContext.CreateAndRegisterEntity<AiderSubscriptionRefusalEntity> ();

			refusal.RefusalType = SubscriptionType.LegalPerson;
			refusal.LegalPersonContact = legalPersonContact;

			return refusal;
		}


		public static void Delete
		(
			BusinessContext businessContext,
			AiderSubscriptionRefusalEntity refusal
		)
		{
			businessContext.DeleteEntity (refusal);
		}


		public static AiderSubscriptionRefusalEntity FindRefusal
		(
			BusinessContext businessContext,
			AiderHouseholdEntity household
		)
		{
			var example = new AiderSubscriptionRefusalEntity ()
			{
				RefusalType = SubscriptionType.Household,
				Household = household,
			};

			return AiderSubscriptionRefusalEntity.FindRefusal (businessContext, example);
		}


		public static AiderSubscriptionRefusalEntity FindRefusal
		(
			BusinessContext businessContext,
			AiderContactEntity legalPersonContact
		)
		{
			var example = new AiderSubscriptionRefusalEntity ()
			{
				RefusalType = SubscriptionType.LegalPerson,
				LegalPersonContact = legalPersonContact,
			};

			return AiderSubscriptionRefusalEntity.FindRefusal (businessContext, example);
		}


		private static AiderSubscriptionRefusalEntity FindRefusal
		(
			BusinessContext businessContext,
			AiderSubscriptionRefusalEntity example
		)
		{
			var request = new Request ()
			{
				RootEntity = example,
			};

			var dataContext = businessContext.DataContext;
			var result = dataContext.GetByRequest<AiderSubscriptionRefusalEntity> (request);

			return result.FirstOrDefault ();
		}


		public static IList<AiderSubscriptionRefusalEntity> FindRefusals
		(
			BusinessContext businessContext,
			AiderLegalPersonEntity legalPerson
		)
		{
			var example = new AiderSubscriptionRefusalEntity ()
			{
				RefusalType = SubscriptionType.LegalPerson,
				LegalPersonContact = new AiderContactEntity ()
				{
					ContactType = ContactType.Legal,
					LegalPerson = legalPerson
				},
			};

			return businessContext.DataContext.GetByExample (example);
		}


		public static void CheckRefusalDoesNotExist
		(
			BusinessContext businessContext,
			AiderHouseholdEntity receiver
		)
		{
			var result = AiderSubscriptionRefusalEntity.FindRefusal (businessContext, receiver);

			AiderSubscriptionRefusalEntity.CheckRefusalDoesNotExist (result);
		}


		public static void CheckRefusalDoesNotExist
		(
			BusinessContext businessContext,
			AiderContactEntity receiver
		)
		{
			var result = AiderSubscriptionRefusalEntity.FindRefusal (businessContext, receiver);

			AiderSubscriptionRefusalEntity.CheckRefusalDoesNotExist (result);
		}


		private static void CheckRefusalDoesNotExist(AiderSubscriptionRefusalEntity result)
		{
			if (result != null)
			{
				var message = "Un refus existe déjà pour ce destinataire.";

				throw new BusinessRuleException (message);
			}
		}


	}


}
