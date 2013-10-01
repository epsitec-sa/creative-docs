using Epsitec.Aider.Entities;

using Epsitec.Cresus.Core;
using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Entities;

using Epsitec.Cresus.DataLayer.Loader;

using System;
using System.Linq;
using Epsitec.Aider.Enumerations;
using Epsitec.Common.Types;


namespace Epsitec.Aider.Data.Job
{

	
	/// <summary>
	/// This fixer removes the subscriptions and the subscription refusals that are empty, i.e.
	/// that don't target either an household or a legal person contact. These have appeared
	/// because for a while, the deletion of an household of of a legal person did not delete the
	/// subscriptions or subscription refusals.
	/// </summary>
	public static class SubscriptionAndRefusalFixer
	{


		public static void FixEmptySubscriptions(CoreData coreData)
		{
			using (var businessContext = new BusinessContext (coreData, false))
			{
				var example = new AiderSubscriptionEntity ();
				var request = Request.Create (example);
				request.AddCondition (businessContext.DataContext, example, e => e.Household == null && e.LegalPersonContact == null);

				var emptySubscriptions = businessContext.DataContext.GetByRequest (request);

				foreach (var subscription in emptySubscriptions)
				{
					businessContext.DeleteEntity (subscription);
				}

				businessContext.SaveChanges (LockingPolicy.ReleaseLock, EntitySaveMode.None);
			}
		}


		public static void FixEmptyRefusals(CoreData coreData)
		{
			using (var businessContext = new BusinessContext (coreData, false))
			{
				var example = new AiderSubscriptionRefusalEntity ();
				var request = Request.Create (example);
				request.AddCondition (businessContext.DataContext, example, e => e.Household == null && e.LegalPersonContact == null);

				var emptyRefusals = businessContext.DataContext.GetByRequest (request);

				foreach (var refusal in emptyRefusals)
				{
					businessContext.DeleteEntity (refusal);
				}

				businessContext.SaveChanges (LockingPolicy.ReleaseLock, EntitySaveMode.None);
			}
		}

		public static void WarnHouseholdWithNoSubscription(CoreData coreData)
		{
			using (var businessContext = new BusinessContext (coreData, false))
			{
				var jobDateTime    = System.DateTime.Now;
				var jobName        = "SubscriptionAndRefusalFixer.WarnHouseholdWithNoSubscription()";
				var jobDescription = string.Format ("Avertissement des ménages sans Abo BN");

				var warningSource = AiderPersonWarningSourceEntity.Create (businessContext, jobDateTime, jobName, TextFormatter.FormatText (jobDescription));

				var subscription =	businessContext.GetAllEntities<AiderSubscriptionEntity> ().Select(s => s.Household).ToList();
				var refusal =		businessContext.GetAllEntities<AiderSubscriptionRefusalEntity> ().Select(s => s.Household).ToList();
				var households =	businessContext.GetAllEntities<AiderHouseholdEntity> ();

				var householdWithoutSubscription = households.Where (h => !subscription.Contains (h) && refusal.Contains (h)).ToList();
				foreach (var household in householdWithoutSubscription)
				{
					var person = household.Members.Where (m => household.IsHead (m)).FirstOrDefault ();

					AiderPersonWarningEntity.Create (businessContext, person, person.ParishGroupPathCache, WarningType.HouseholdWithoutSubscription, "Ménage sans abo.", "Ce ménage n'est pas référencé dans les abonnements ou refus", warningSource);
				}
				businessContext.SaveChanges (LockingPolicy.ReleaseLock, EntitySaveMode.None);
			}
		}

	}


}
