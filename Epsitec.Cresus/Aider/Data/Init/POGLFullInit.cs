//	Copyright © 2013-2014, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
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
		public static void StartJob(CoreData coreData)
		{
			using (var businessContext = new BusinessContext (coreData, false))
			{
				if (AiderGroupEntity.FindGroups (businessContext, "POGL.").Any ())
				{
					//Previous "demo" model existing
					POGLFullInit.ClearOldPOGLDefinitionsAndGroups (coreData);
					POGLFullInit.CreatePOGLParishAndRegion (coreData);
				}
				else
				{
					//Nothing exist
					POGLFullInit.CreatePOGLParishAndRegion (coreData);
					POGLFullInit.CreateVillamont (coreData);
				}

			}
		}

		private static void ClearOldPOGLDefinitionsAndGroups(CoreData coreData)
		{
			using (var businessContext = new BusinessContext (coreData, false))
			{
				var poglRoot = AiderGroupEntity.FindGroups (businessContext, "POGL.").First ();
				var pogls = AiderGroupEntity.FindGroups (businessContext, "POGL.").First ().Subgroups;

				businessContext.DeleteEntity (pogls.First().GroupDef);
				foreach (var pogl in pogls)
				{
					var office = AiderOfficeManagementEntity.Find (businessContext, pogl);
					businessContext.DeleteEntity (office);
					businessContext.DeleteEntity (pogl);
				}

				businessContext.DeleteEntity (poglRoot.GroupDef);
				businessContext.DeleteEntity (poglRoot);

				businessContext.SaveChanges (LockingPolicy.ReleaseLock, EntitySaveMode.None);
			}
		}

		private static void CreatePOGLParishAndRegion(CoreData coreData)
		{
			using (var businessContext = new BusinessContext (coreData, false))
			{
				var exempleRegion = AiderGroupEntity.FindGroups (businessContext, "R001.").First ();
				var exempleParishGroupDef = exempleRegion.Subgroups.Where (p => p.IsParish ()).First ().GroupDef;

				//INSTANTIATE PLA ROOT
				var rootGroupDef = exempleRegion.GroupDef;

				rootGroupDef.InstantiateRegion (businessContext, "PLA", 12);

				businessContext.SaveChanges (LockingPolicy.ReleaseLock, EntitySaveMode.None);

				var poglRegion = AiderGroupEntity.FindGroups (businessContext, "R012.").First ();

				//INSTANTIATE GROUPS
				exempleParishGroupDef.InstantiateParish (businessContext, poglRegion, "PLA Broyetal", 1);
				exempleParishGroupDef.InstantiateParish (businessContext, poglRegion, "PLA La Côte", 2);
				exempleParishGroupDef.InstantiateParish (businessContext, poglRegion, "PLA Nord Vaudois", 3);
				exempleParishGroupDef.InstantiateParish (businessContext, poglRegion, "PLA Riviera Chablais", 4);

				businessContext.SaveChanges (LockingPolicy.ReleaseLock, EntitySaveMode.None);

				//Create office management entity for POGL
				var parishGroups = AiderGroupEntity.FindGroupsFromPathAndLevel (businessContext, exempleParishGroupDef.Level, "R012.P___.");
				foreach (var group in parishGroups)
				{
					AiderOfficeManagementEntity.Create (businessContext, group.Name, group);
				}

				businessContext.SaveChanges (LockingPolicy.ReleaseLock, EntitySaveMode.None);

				
			}
		}

		public static void CreateVillamont(CoreData coreData)
		{
			using (var businessContext = new BusinessContext (coreData, false))
			{
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
				
			}
		}
	}
}
