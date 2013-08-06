using Epsitec.Aider.Entities;

using Epsitec.Cresus.Core;
using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Entities;

using Epsitec.Cresus.DataLayer.Loader;

using System;


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


	}


}
