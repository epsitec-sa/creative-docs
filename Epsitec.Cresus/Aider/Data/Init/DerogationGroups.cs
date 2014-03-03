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
	/// This job create missing derogation groups if needed
	/// </summary>
	public static class DerogationGroups
	{
		public static void CreateIfNeeded(CoreData coreData)
		{
			using (var businessContext = new BusinessContext (coreData, false))
			{
				var initNeeded = ! AiderGroupEntity.FindGroups (businessContext, "R001.P001.").First ()
													.Subgroups.Where(s => s.GroupDef.Classification == Enumerations.GroupClassification.DerogationIn)
													.Any ();
				if (initNeeded)
				{

					var parishGroupDef = AiderGroupEntity.FindGroups (businessContext, "R001.P001.").First ().GroupDef;

					var derogationInDef = AiderGroupDefEntity.CreateDefinitionSubGroup (
						businessContext,
						parishGroupDef,
						"Dérogations entrantes",
						GroupClassification.DerogationIn,
						false,
						true,
						false
					);

					var groupToComplete = AiderGroupEntity.FindGroupsFromPathAndLevel (businessContext, parishGroupDef.Level, parishGroupDef.PathTemplate);
					foreach (var group in groupToComplete)
					{
						group.CreateSubgroup (businessContext, derogationInDef);
					}

					var derogationOutDef = AiderGroupDefEntity.CreateDefinitionSubGroup (
						businessContext,
						parishGroupDef,
						"Dérogations sortantes",
						GroupClassification.DerogationOut,
						false,
						true,
						false
					);

					groupToComplete = AiderGroupEntity.FindGroupsFromPathAndLevel (businessContext, parishGroupDef.Level, parishGroupDef.PathTemplate);
					foreach (var group in groupToComplete)
					{
						group.CreateSubgroup (businessContext, derogationOutDef);
					}

					businessContext.SaveChanges (LockingPolicy.ReleaseLock, EntitySaveMode.None);
				}
			}
		}
	}
}
