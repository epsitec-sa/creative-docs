//	Copyright © 2014, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Samuel LOUP, Maintainer: Samuel LOUP

using Epsitec.Aider.Data.Common;
using Epsitec.Aider.Entities;
using Epsitec.Aider.Enumerations;

using Epsitec.Common.IO;

using Epsitec.Cresus.Core;
using Epsitec.Cresus.Core.Business;

using Epsitec.Cresus.DataLayer.Context;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Epsitec.Cresus.Core.Entities;

namespace Epsitec.Aider.Data.Groups
{
	/// <summary>
	/// This job create missing Aider Users groups if needed
	/// </summary>
	public static class AiderUsersGroups
	{		
		public static void CreateIfNeeded(CoreData coreData)
		{
			using (var businessContext = new BusinessContext (coreData, false))
			{
				var exampleParishGroupDef = AiderGroupEntity.FindGroups (businessContext, "R001.P001.").First ().GroupDef;

				AiderUsersGroups.CreateUserGroupIfNeeded (businessContext, exampleParishGroupDef, "Utilisateurs AIDER", GroupClassification.Users);
				AiderUsersGroups.CreateUserGroupIfNeeded (businessContext, exampleParishGroupDef, "Suppléant AIDER", GroupClassification.ActingUser);
				AiderUsersGroups.CreateUserGroupIfNeeded (businessContext, exampleParishGroupDef, "Responsable AIDER", GroupClassification.ResponsibleUser);

				businessContext.SaveChanges (LockingPolicy.ReleaseLock, EntitySaveMode.None);
			}
		}

		private static void CreateUserGroupIfNeeded(BusinessContext businessContext, AiderGroupDefEntity parishGroupDef, string name, GroupClassification classif)
		{
			var groupExist = AiderGroupEntity.FindGroups (businessContext, "R001.P001.").First ()
												.Subgroups.Where (s => s.GroupDef.Classification == classif)
												.Any ();
			if (!groupExist)
			{
				var aiderUsersDef = AiderUsersGroups.CreateUserGroupDef (businessContext, parishGroupDef, name, classif);

				aiderUsersDef.MembersReadOnly = true;

				AiderUsersGroups.DeployUserGroup (businessContext, parishGroupDef, aiderUsersDef);
			}		
		}

		private static AiderGroupDefEntity CreateUserGroupDef(BusinessContext businessContext, AiderGroupDefEntity parishGroupDef, string name, GroupClassification classification)
		{
			return AiderGroupDefEntity.CreateDefinitionSubGroup (businessContext,
					parishGroupDef, name, classification,
					subgroupsAllowed: false, membersAllowed: true, mutability: Mutability.SystemDefined
				);
		}

		private static void DeployUserGroup (BusinessContext businessContext,AiderGroupDefEntity parishGroupDef,AiderGroupDefEntity aiderUsersDef)
		{
			var groupToComplete = AiderGroupEntity.FindGroupsFromPathAndLevel (businessContext, parishGroupDef.Level, parishGroupDef.PathTemplate);
			foreach (var group in groupToComplete)
			{
				group.CreateSubgroup (businessContext, aiderUsersDef);
			}
		}
	}
}
