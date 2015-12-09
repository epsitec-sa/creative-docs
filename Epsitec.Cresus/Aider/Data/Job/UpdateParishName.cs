using Epsitec.Aider.Data.Common;
using Epsitec.Aider.Entities;

using Epsitec.Common.IO;

using Epsitec.Cresus.Core;
using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Entities;
using Epsitec.Common.Support.Extensions;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Epsitec.Aider.Helpers;
using System.Data;
using Epsitec.Cresus.Database;


namespace Epsitec.Aider.Data.Job
{
	internal static class UpdateParishName
	{
		public static void Update(CoreData coreData, string currentName, string newName)
		{
			using (var businessContext = new BusinessContext (coreData, false))
			{
				Logger.LogToConsole ("Update Parish Name...");
				var repo = ParishAddressRepository.Current;
				if (ParishAssigner.GetParishGroupName (repo, newName) == "Paroisse inconnue (" + newName + ")")
				{
					throw new BusinessRuleException ("le nouveau nom n'a pas été changé dans le fichier de correspondance externe à AIDER");
				}
				if (ParishAssigner.GetParishGroupName (repo, currentName) != "Paroisse inconnue (" + currentName + ")")
				{
					throw new BusinessRuleException ("le nom actuel fournit est encore dans le fichier de correspondance !");
				}

				var currentParishGroupName = "Paroisse de " + currentName;
				var newParishGroupName     = ParishAssigner.GetParishGroupName (repo, newName);

				var groupToRename = ParishAssigner.FindGroup (businessContext, currentParishGroupName, Enumerations.GroupClassification.Parish);
				groupToRename.Name = newParishGroupName;
				var officeToRename = AiderOfficeManagementEntity.Find (businessContext, groupToRename);
				officeToRename.OfficeName = newParishGroupName;
				officeToRename.RefreshOfficeShortName ();

				Logger.LogToConsole ("Updated group name: " + groupToRename.Name);
				Logger.LogToConsole ("Updated office name: " + officeToRename.OfficeName);
				Logger.LogToConsole ("Updated office shortname: " + officeToRename.OfficeShortName);
				businessContext.SaveChanges (LockingPolicy.ReleaseLock, EntitySaveMode.None);
				Logger.LogToConsole ("Update participations...");
				AiderGroupEntity.FindGroupsAndSubGroupsFromPathPrefix (businessContext, groupToRename.Path)
				.ForEach (g =>
				{
					if (g.GroupDef.RoleCacheDisabled)
					{
						return;
					}
					g.FindParticipations (businessContext).ForEach (p =>
					{
						if (p.RoleCacheDisabled)
						{
							return;
						}
						if (p.Contact.IsNotNull ())
						{
							var role		= AiderParticipationsHelpers.BuildRoleFromParticipation (p)
																		.GetRole (p);

							var rolePath	= AiderParticipationsHelpers.GetRolePath (p);

							p.RoleCache		= role;
							p.RolePathCache = rolePath;

							Logger.LogToConsole (string.Format ("{0} is {1} ({2})", p.Contact.DisplayName, role, rolePath));
						}
					});
				});
				businessContext.SaveChanges (LockingPolicy.ReleaseLock, EntitySaveMode.None);
				Logger.LogToConsole ("Job done!");
			}
		}

	}


}
