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
using Epsitec.Common.Support.Extensions;

namespace Epsitec.Aider.Data.Groups
{
	/// <summary>
	/// Init ParishGroupPathCache for already created events and participants
	/// </summary>
	public static class AiderEvents
	{		
		public static void InitGroupPathCacheIfNeeded(CoreData coreData)
		{
			using (var businessContext = new BusinessContext (coreData, false))
			{
				businessContext.GetAllEntities<AiderEventEntity> ().Where (e => e.ParishGroupPathCache == null).ForEach (e =>
				{
					var pathCache = e.Office.ParishGroupPathCache;
					e.ParishGroupPathCache = pathCache;
					e.Participants.ForEach (p => p.ParishGroupPathCache = pathCache);
				});

				businessContext.SaveChanges (LockingPolicy.ReleaseLock, EntitySaveMode.None);
				Console.WriteLine ("Done! press any key...");
				Console.ReadLine ();
			}
		}

		public static void RebuildMainActorsOnValidatedEvents(CoreData coreData)
		{
			using (var businessContext = new BusinessContext (coreData, false))
			{
				var example = new AiderEventEntity ()
				{
					State = EventState.Validated
				};
				businessContext.GetByExample<AiderEventEntity> (example).ForEach (e =>
				{
					e.BuildMainActorsSummary ();
				});

				businessContext.SaveChanges (LockingPolicy.ReleaseLock, EntitySaveMode.None);
				Console.WriteLine ("Done! press any key...");
				Console.ReadLine ();
			}
		}
	}
}
