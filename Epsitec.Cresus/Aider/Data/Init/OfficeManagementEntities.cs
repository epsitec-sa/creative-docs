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
	/// This job create office management groups
	/// </summary>
	public static class OfficeManagementEntities
	{
		public static void CreateOfficeManagementEntities(CoreData coreData)
		{
			using (var businessContext = new BusinessContext (coreData, false))
			{
				var parishGroupDef = AiderGroupEntity.FindGroups (businessContext, "R001.P001.").First ().GroupDef;

				var parishGroups = AiderGroupEntity.FindGroupsFromPathAndLevel (businessContext, parishGroupDef.Level, parishGroupDef.PathTemplate);
				foreach (var group in parishGroups)
				{
					AiderOfficeManagementEntity.Create (businessContext, group.Name, group);
				}
					

				businessContext.SaveChanges (LockingPolicy.ReleaseLock, EntitySaveMode.None);		
			}
		}
	}
}
