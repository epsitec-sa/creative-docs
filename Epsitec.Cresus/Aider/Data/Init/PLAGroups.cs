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
	/// This job create missing PLA root group
	/// </summary>
	public static class PLAGroups
	{
		public static void Create(CoreData coreData)
		{
			using (var businessContext = new BusinessContext (coreData, false))
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

				poglRoot.Instantiate (businessContext);

				businessContext.SaveChanges (LockingPolicy.ReleaseLock, EntitySaveMode.None);
			}
		}
	}
}
