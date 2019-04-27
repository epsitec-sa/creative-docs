using Epsitec.Aider.Data.Common;

using Epsitec.Aider.Entities;

using Epsitec.Cresus.Core;
using Epsitec.Cresus.Core.Business;
using System.Linq;

namespace Epsitec.Aider.Data.Job
{

	internal static class MailingCategoriesCleaner
	{

		public static void Clean(CoreData coreData)
		{
			using (var businessContext = new BusinessContext (coreData, false))
			{
				var categories = businessContext.GetAllEntities<AiderMailingCategoryEntity> ();
				var mailings = businessContext.GetAllEntities<AiderMailingEntity> ();
				var duplicates = categories.GroupBy (x => x.DisplayName)
							.Where (g => g.Count () > 1)
							.Select (y => new { Element = y.Key, Counter = y.Count (), Values = y.ToList () })
							.ToList ();

				foreach (var dup in duplicates)
				{
					var toKeep = dup.Values.First ();
					var toUpdate = mailings.Where (
						m => 
							m.Category.Name == toKeep.Name && 
							m.Category.GroupPathCache == toKeep.GroupPathCache
					);

					// Re-assign good one
					foreach (var toUp in toUpdate)
					{
						toUp.Category = toKeep;
					}

					// Delete unecessary
					var toDelete = dup.Values.Skip (1).ToList ();
					foreach (var toDel in toDelete)
					{
						businessContext.DeleteEntity (toDel);
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
