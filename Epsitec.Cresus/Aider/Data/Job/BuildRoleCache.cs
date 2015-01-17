using Epsitec.Aider.Data.Common;
using Epsitec.Aider.Entities;

using Epsitec.Common.IO;

using Epsitec.Cresus.Core;
using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Entities;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Epsitec.Aider.Helpers;


namespace Epsitec.Aider.Data.Job
{
	internal static class BuildRoleCache
	{


		public static void Build(CoreData coreData)
		{
			Logger.LogToConsole ("START BUILD ROLE CACHE");

			AiderEnumerator.ParticipationRoleCache (coreData, BuildRoleCache.BuildBatch);

			Logger.LogToConsole ("DONE BUILD ROLE CACHE ");
		}


		private static void BuildBatch
		(
			BusinessContext businessContext,
			IEnumerable<AiderGroupParticipantEntity> participations)
		{
			Logger.LogToConsole ("START BATCH");

			foreach (var participation in participations)
			{
				if (participation.Contact.IsNotNull ())
				{
					var role = AiderParticipationsHelpers.BuildRoleFromParticipation (participation).GetRole (participation);
					Logger.LogToConsole (string.Format("{0} is {1}",participation.Contact.DisplayName ,role));
					participation.RoleCache = role;
				}
			}

			businessContext.SaveChanges (LockingPolicy.KeepLock, EntitySaveMode.IgnoreValidationErrors);

			Logger.LogToConsole ("DONE BATCH");
		}


	}


}
