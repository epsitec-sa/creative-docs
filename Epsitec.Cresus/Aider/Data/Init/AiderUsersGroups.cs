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
using Epsitec.Common.Types;

namespace Epsitec.Aider.Data.Groups
{
	/// <summary>
	/// This job create missing Aider Users groups if needed
	/// </summary>
	public static class AiderUsersGroups
	{	
		public static void PopulateUserGroupsWithUsers(CoreData coreData)
		{
			using (var businessContext = new BusinessContext (coreData, false))
			{
				var exempleUser = new AiderUserEntity ()
				{
					Disabled = false
				};

				var usersToPlace = businessContext.GetByExample<AiderUserEntity> (exempleUser);

				foreach(var user in usersToPlace)
				{
					if(user.Parish.IsNotNull() && user.Contact.IsNotNull())
					{
						var contact = user.Contact;
						var newUserGroup = user.Parish.Subgroups.Single (s => s.GroupDef.Classification == Enumerations.GroupClassification.Users);
						if(!newUserGroup.FindParticipationsByGroup (businessContext, contact, newUserGroup).Any ())
						{
							var participationData = new List<ParticipationData> ();
							participationData.Add (new ParticipationData (contact));
							newUserGroup.AddParticipations (businessContext, participationData, Date.Today, FormattedText.Null);
						}
						
					}

					if (user.Office.IsNotNull () && user.Contact.IsNotNull ())
					{
						var contact = user.Contact;
						var newUserGroup = user.Office.ParishGroup.Subgroups.Single (s => s.GroupDef.Classification == Enumerations.GroupClassification.ResponsibleUser);
						if (!newUserGroup.FindParticipationsByGroup (businessContext, contact, newUserGroup).Any ())
						{
							var participationData = new List<ParticipationData> ();
							participationData.Add (new ParticipationData (contact));
							newUserGroup.AddParticipations (businessContext, participationData, Date.Today, FormattedText.Null);
						}
						
					}
				}

				businessContext.SaveChanges (LockingPolicy.ReleaseLock, EntitySaveMode.None);
			}
		}

		public static void InitParishUserGroups(CoreData coreData)
		{
			using (var businessContext = new BusinessContext (coreData, false))
			{
				var exampleParishGroupDef = AiderGroupEntity.FindGroups (businessContext, "R001.P001.").First ().GroupDef;

				AiderUsersGroups.CreateUserGroupsIfNeeded (businessContext, exampleParishGroupDef, "Utilisateurs AIDER", GroupClassification.Users,true);
				AiderUsersGroups.CreateUserGroupsIfNeeded (businessContext, exampleParishGroupDef, "Suppléant AIDER", GroupClassification.ActingUser,false);
				AiderUsersGroups.CreateUserGroupsIfNeeded (businessContext, exampleParishGroupDef, "Responsable AIDER", GroupClassification.ResponsibleUser,true);

				businessContext.SaveChanges (LockingPolicy.ReleaseLock, EntitySaveMode.None);
			}
		}

		public static void InitRegionalUserGroups(CoreData coreData)
		{
			using (var businessContext = new BusinessContext (coreData, false))
			{
				var exampleRegionalGroupDef = AiderGroupEntity.FindGroups (businessContext, "R001.").First ().GroupDef;

				AiderUsersGroups.CreateUserGroupsIfNeeded (businessContext, exampleRegionalGroupDef, "Utilisateurs AIDER", GroupClassification.Users, true);
				AiderUsersGroups.CreateUserGroupsIfNeeded (businessContext, exampleRegionalGroupDef, "Suppléant AIDER", GroupClassification.ActingUser, false);
				AiderUsersGroups.CreateUserGroupsIfNeeded (businessContext, exampleRegionalGroupDef, "Responsable AIDER", GroupClassification.ResponsibleUser, true);

				businessContext.SaveChanges (LockingPolicy.ReleaseLock, EntitySaveMode.None);
			}
		}

		public static void CreateForGroup(BusinessContext businessContext, AiderGroupEntity group)
		{
			var usersDef		= AiderUsersGroups.CreateUserGroupDef (businessContext, group.GroupDef, "Utilisateurs AIDER", GroupClassification.Users, true);
			var actingDef		= AiderUsersGroups.CreateUserGroupDef (businessContext, group.GroupDef, "Suppléant AIDER", GroupClassification.ActingUser, false);
			var responsibleDef  = AiderUsersGroups.CreateUserGroupDef (businessContext, group.GroupDef, "Responsable AIDER", GroupClassification.ResponsibleUser, true);

			group.CreateSubgroup (businessContext, usersDef);
			group.CreateSubgroup (businessContext, actingDef);
			group.CreateSubgroup (businessContext, responsibleDef);
		}

		private static void CreateUserGroupsIfNeeded(BusinessContext businessContext, AiderGroupDefEntity parishGroupDef, string name, GroupClassification classif, bool readOnlyMembers)
		{
			var groupExist = AiderGroupEntity.FindGroups (businessContext, "R001.P001.").First ()
												.Subgroups.Where (s => s.GroupDef.Classification == classif)
												.Any ();
			if (!groupExist)
			{
				var aiderUsersDef = AiderUsersGroups.CreateUserGroupDef (businessContext, parishGroupDef, name, classif, readOnlyMembers);
				AiderUsersGroups.DeployUserGroups (businessContext, parishGroupDef, aiderUsersDef);
			}		
		}

		private static AiderGroupDefEntity CreateUserGroupDef(BusinessContext businessContext, AiderGroupDefEntity parishGroupDef, string name, GroupClassification classification, bool readOnlyMembers)
		{
			var def = AiderGroupDefEntity.CreateDefinitionSubGroup (businessContext,
					parishGroupDef, name, classification,
					subgroupsAllowed: false, membersAllowed: true, mutability: Mutability.SystemDefined
				);

			def.MembersReadOnly = readOnlyMembers;

			return def;
		}

		private static void DeployUserGroups (BusinessContext businessContext,AiderGroupDefEntity parishGroupDef,AiderGroupDefEntity aiderUsersDef)
		{
			var groupToComplete = AiderGroupEntity.FindGroupsFromPathAndLevel (businessContext, parishGroupDef.Level, parishGroupDef.PathTemplate);
			foreach (var group in groupToComplete)
			{
				group.CreateSubgroup (businessContext, aiderUsersDef);
			}
		}
	}
}
