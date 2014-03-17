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
	/// This job create missing POGLE base group
	/// </summary>
	public static class POGLBaseGroups
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
					poglParishBase.MembersAllowed = false;
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


					//TODO Derog In Out

					//Villamont exception
					var region4 = AiderGroupEntity.FindGroups (businessContext, "R004.").First ();
					var exempleParishGroupDef = region4.Subgroups.Where (p => p.IsParish ()).First ().GroupDef;
					var number = AiderGroupIds.FindNextSubGroupDefNumber (region4.Subgroups.Select (g => g.Path), 'P');
					exempleParishGroupDef.InstantiateParish (businessContext, region4, "PLA Villamont", number);

					businessContext.SaveChanges (LockingPolicy.ReleaseLock, EntitySaveMode.None);
				}
			}
		}
	}
}
