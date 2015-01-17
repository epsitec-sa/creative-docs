using Epsitec.Aider.Data.Common;

using Epsitec.Aider.Entities;

using Epsitec.Cresus.Core;
using Epsitec.Cresus.Core.Business;

namespace Epsitec.Aider.Data.Job
{


	/// <summary>
	/// This fixer is used to fix invalid subscriptions, where the regional edition has been set to
	/// the wrong region. These wrong subscriptions are due to a wrong entry in the parish
	/// definition file. The parish assignation for the persons has been corrected at the time, but
	/// not the region assignation for the subscriptions. The file has been corrected by commit
	/// 21207.
	/// </summary>
	internal static class PrillySubscriptionFixer
	{


		public static void FixPrillySubscriptions(CoreData coreData)
		{
			using (var businessContext = new BusinessContext (coreData, false))
			{
				var region3 = ParishAssigner.FindRegionGroup (businessContext, 3);
				var region4 = ParishAssigner.FindRegionGroup (businessContext, 4);

				var example = new AiderSubscriptionEntity ()
				{
					DisplayZipCode = "1008",
					RegionalEdition = region4
				};

				var subscriptions = businessContext.DataContext.GetByExample (example);

				foreach (var subscription in subscriptions)
				{
					var streetName = subscription.GetAddress ().StreetUserFriendly;

					if (streetName == "avenue de la Confrérie")
					{
						subscription.RegionalEdition = region3;
					}
				}

				businessContext.SaveChanges
				(
					LockingPolicy.ReleaseLock, EntitySaveMode.IgnoreValidationErrors
				);
			}
		}


	}


}
