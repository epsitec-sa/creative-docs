using Epsitec.Aider.Entities;
using Epsitec.Common.IO;
using Epsitec.Cresus.Core;
using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Database;


namespace Epsitec.Aider.Data.Job
{

	internal static class DataQualityFlagsFixer
	{
		public static void Run(CoreData coreData)
		{
			using (var businessContext = new BusinessContext (coreData, false))
			{
				Logger.LogToConsole ("Fixing flags...");

				SqlHelpers.SelectDbIds (businessContext, "mud_lvard", "mud_lvard.u_lvoh93 = 'D;'", (ids) =>
				{
					foreach (var id in ids)
					{
						var entity = businessContext.DataContext.ResolveEntity<AiderContactEntity> (new DbKey (id));
						if (entity.IsNotNull ())
						{
							if (entity.Household.IsNotNull ())
							{
								var household = entity.Household;
								AiderContactEntity.DeleteDuplicateContacts (businessContext, household.Contacts);
								household.RefreshCache ();
								entity.QualityCode = "";
								businessContext.SaveChanges (LockingPolicy.ReleaseLock, EntitySaveMode.IgnoreValidationErrors);
							}
							else
							{
								entity.QualityCode = MXFlagger.GetCode (entity.Person);
							}
						}
						
					}
					businessContext.SaveChanges (LockingPolicy.ReleaseLock, EntitySaveMode.IgnoreValidationErrors);
				});

				SqlHelpers.SelectDbIds (businessContext, "mud_lvard", "mud_lvard.u_lvoh93 = 'D3;'", (ids) =>
				{
					foreach (var id in ids)
					{
						var entity = businessContext.DataContext.ResolveEntity<AiderContactEntity> (new DbKey (id));
						if (entity.IsNotNull ())
						{
							if (entity.Household.IsNotNull ())
							{
								var household = entity.Household;
								AiderContactEntity.DeleteDuplicateContacts (businessContext, household.Contacts);
								household.RefreshCache ();
								entity.QualityCode = "";

							}
							else
							{
								entity.QualityCode = MXFlagger.GetCode (entity.Person);
							}
						}
					}
					businessContext.SaveChanges (LockingPolicy.ReleaseLock, EntitySaveMode.IgnoreValidationErrors);
				});
			}
		}
	}

}
