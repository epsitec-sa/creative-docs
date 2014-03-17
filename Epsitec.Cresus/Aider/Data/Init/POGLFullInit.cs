﻿//	Copyright © 2013-2014, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
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
	/// This job create missing POGLE base group, derogations subgroups and office management entity
	/// </summary>
	public static class POGLFullInit
	{
		public static void CreateIfNeeded(CoreData coreData)
		{
			using (var businessContext = new BusinessContext (coreData, false))
			{
				var initNeeded = !AiderGroupEntity.FindGroups (businessContext, "POGL.").Any ();

				if (initNeeded)
				{
					var poglRoot = businessContext.CreateAndRegisterEntity<AiderGroupDefEntity> ();

					poglRoot.Name = "PLA";
					poglRoot.Number = ""; //?
					poglRoot.Level = AiderGroupIds.TopLevel;
					poglRoot.SubgroupsAllowed = true;
					poglRoot.MembersAllowed = false;
					poglRoot.PathTemplate = AiderGroupIds.CreateTopLevelPathTemplate (GroupClassification.ParishOfGermanLanguage);

					poglRoot.Classification = GroupClassification.ParishOfGermanLanguage;
					poglRoot.Mutability = Mutability.None;

					var poglParishBase = businessContext.CreateAndRegisterEntity<AiderGroupDefEntity> ();

					poglParishBase.Name = "Paroisse";
					poglParishBase.Number = ""; //?
					poglParishBase.Level = AiderGroupIds.TopLevel + 1;
					poglParishBase.SubgroupsAllowed = true;
					poglParishBase.MembersAllowed = true;
					poglParishBase.PathTemplate = AiderGroupIds.CreateParishSubgroupPath (poglRoot.PathTemplate);

					poglParishBase.Classification = GroupClassification.Parish;
					poglParishBase.Mutability = Mutability.None;

					//Uplink
					poglRoot.Subgroups.Add (poglParishBase);


					//INSTANTIATE GROUPS
					var poglRegion = poglRoot.Instantiate (businessContext);
					poglParishBase.InstantiateParish (businessContext, poglRegion, "PLA Broyetal", 1);
					poglParishBase.InstantiateParish (businessContext, poglRegion, "PLA La Côte", 2);
					poglParishBase.InstantiateParish (businessContext, poglRegion, "PLA Nord Vaudois", 3);
					poglParishBase.InstantiateParish (businessContext, poglRegion, "PLA Riviera Chablais", 4);

					//Villamont exception
					var region4 = AiderGroupEntity.FindGroups (businessContext, "R004.").First ();
					var exempleParishGroupDef = region4.Subgroups.Where (p => p.IsParish ()).First ().GroupDef;
					var number = AiderGroupIds.FindNextSubGroupDefNumber (region4.Subgroups.Select (g => g.Path), 'P');
					exempleParishGroupDef.InstantiateParish (businessContext, region4, "PLA Villamont", number);

					businessContext.SaveChanges (LockingPolicy.ReleaseLock, EntitySaveMode.None);

					//Office management for Villamont
					var specificGroup = AiderGroupEntity.FindGroups (businessContext, "R004.P00"+number+".").First ();
					AiderOfficeManagementEntity.Create (businessContext, specificGroup.Name, specificGroup);

					businessContext.SaveChanges (LockingPolicy.ReleaseLock, EntitySaveMode.None);

					//Create derogation groups for POGL

					var derogationInDef = AiderGroupDefEntity.CreateDefinitionSubGroup (
						businessContext,
						poglRoot,
						"Dérogations entrantes",
						GroupClassification.DerogationIn,
						false,
						true,
						Mutability.SystemDefined
					);

					var groupToComplete = AiderGroupEntity.FindGroupsFromPathAndLevel (businessContext, poglParishBase.Level, poglParishBase.PathTemplate);
					foreach (var group in groupToComplete)
					{
						group.CreateSubgroup (businessContext, derogationInDef);
					}

					var derogationOutDef = AiderGroupDefEntity.CreateDefinitionSubGroup (
						businessContext,
						poglRoot,
						"Dérogations sortantes",
						GroupClassification.DerogationOut,
						false,
						true,
						Mutability.SystemDefined
					);

					groupToComplete = AiderGroupEntity.FindGroupsFromPathAndLevel (businessContext, poglParishBase.Level, poglParishBase.PathTemplate);
					foreach (var group in groupToComplete)
					{
						group.CreateSubgroup (businessContext, derogationOutDef);
					}

					businessContext.SaveChanges (LockingPolicy.ReleaseLock, EntitySaveMode.None);

					//Create office management entity for POGL
					var parishGroups = AiderGroupEntity.FindGroupsFromPathAndLevel (businessContext, poglParishBase.Level, poglParishBase.PathTemplate);
					foreach (var group in parishGroups)
					{
						AiderOfficeManagementEntity.Create (businessContext, group.Name, group);
					}

					businessContext.SaveChanges (LockingPolicy.ReleaseLock, EntitySaveMode.None);


					//Create user groups
					var aiderUsersDef = AiderGroupDefEntity.CreateDefinitionSubGroup (businessContext,
						poglParishBase, "Utilisateurs AIDER", GroupClassification.Users,
						subgroupsAllowed: false, membersAllowed: true, mutability: Mutability.SystemDefined
					);

					aiderUsersDef.MembersReadOnly = true;

					groupToComplete = AiderGroupEntity.FindGroupsFromPathAndLevel (businessContext, poglParishBase.Level, poglParishBase.PathTemplate);
					foreach (var group in groupToComplete)
					{
						group.CreateSubgroup (businessContext, aiderUsersDef);
					}

					businessContext.SaveChanges (LockingPolicy.ReleaseLock, EntitySaveMode.None);
				}
			}
		}
	}
}
