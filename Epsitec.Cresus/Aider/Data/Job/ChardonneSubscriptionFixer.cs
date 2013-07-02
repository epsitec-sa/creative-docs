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
	/// 20925.
	/// </summary>
	internal static class ChardonneSubscriptionFixer
	{


		public static void FixChardonneSubscriptions(CoreData coreData)
		{
			using (var businessContext = new BusinessContext (coreData, false))
			{
				var region09 = ParishAssigner.FindRegionGroup (businessContext, 09);
				var region10 = ParishAssigner.FindRegionGroup (businessContext, 10);
					
				var example = new AiderSubscriptionEntity ()
				{
					DisplayZipCode = "1803",
					RegionalEdition = region09
				};

				var subscriptions = businessContext.DataContext.GetByExample (example);

				foreach (var subscription in subscriptions)
				{
					subscription.RegionalEdition = region10;
				}

				businessContext.SaveChanges
				(
					LockingPolicy.ReleaseLock, EntitySaveMode.IgnoreValidationErrors
				);
			}
		}


	}


}
