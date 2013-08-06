using Epsitec.Aider.Data.Common;
using Epsitec.Aider.Entities;

using Epsitec.Cresus.Core;
using Epsitec.Cresus.Core.Business;

using System.Collections.Generic;

namespace Epsitec.Aider.Data.Job
{


	internal static class DisplayValuesFixer
	{



		public static void FixAiderContactDisplayValues(CoreData coreData)
		{
			AiderEnumerator.Execute (coreData, DisplayValuesFixer.FixAiderContacts);
		}


		private static void FixAiderContacts
		(
			BusinessContext businessContext,
			IEnumerable<AiderContactEntity> contacts
		)
		{
			foreach (var contact in contacts)
			{
				contact.RefreshCache ();
			}

			businessContext.SaveChanges (LockingPolicy.ReleaseLock, EntitySaveMode.IgnoreValidationErrors);
		}


		public static void FixAiderHouseholdDisplayValues(CoreData coreData)
		{
			AiderEnumerator.Execute (coreData, DisplayValuesFixer.FixAiderHouseholds);
		}


		private static void FixAiderHouseholds
		(
			BusinessContext businessContext,
			IEnumerable<AiderHouseholdEntity> households
		)
		{
			foreach (var household in households)
			{
				household.RefreshCache ();
			}

			businessContext.SaveChanges (LockingPolicy.ReleaseLock, EntitySaveMode.IgnoreValidationErrors);
		}


		public static void FixAiderLegalPersonDisplayValues(CoreData coreData)
		{
			AiderEnumerator.Execute (coreData, DisplayValuesFixer.FixAiderLegalPersons);
		}


		private static void FixAiderLegalPersons
		(
			BusinessContext businessContext,
			IEnumerable<AiderLegalPersonEntity> legalPersons
		)
		{
			foreach (var legalPerson in legalPersons)
			{
				legalPerson.RefreshCache ();
			}

			businessContext.SaveChanges (LockingPolicy.ReleaseLock, EntitySaveMode.IgnoreValidationErrors);
		}



		public static void FixAiderSubscriptionDisplayValues(CoreData coreData)
		{
			AiderEnumerator.Execute (coreData, DisplayValuesFixer.FixAiderSubscriptions);
		}


		private static void FixAiderSubscriptions
		(
			BusinessContext businessContext,
			IEnumerable<AiderSubscriptionEntity> subscriptions
		)
		{
			foreach (var subscription in subscriptions)
			{
				subscription.RefreshCache ();
			}

			businessContext.SaveChanges (LockingPolicy.ReleaseLock, EntitySaveMode.IgnoreValidationErrors);
		}



		public static void FixAiderSubscriptionRefusalDisplayValues(CoreData coreData)
		{
			AiderEnumerator.Execute (coreData, DisplayValuesFixer.FixAiderSubscriptionRefusals);
		}


		private static void FixAiderSubscriptionRefusals
		(
			BusinessContext businessContext,
			IEnumerable<AiderSubscriptionRefusalEntity> refusals
		)
		{
			foreach (var refusal in refusals)
			{
				refusal.RefreshCache ();
			}

			businessContext.SaveChanges (LockingPolicy.ReleaseLock, EntitySaveMode.IgnoreValidationErrors);
		}


	}


}
