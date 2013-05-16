using Epsitec.Aider.Enumerations;

using Epsitec.Cresus.Core.Business;

using Epsitec.Cresus.DataLayer.Loader;

using System.Linq;


namespace Epsitec.Aider.Entities
{


	public partial class AiderSubscriptionRefusalEntity
	{


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


	}


}
