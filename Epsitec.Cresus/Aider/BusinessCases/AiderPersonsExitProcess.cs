//	Copyright © 2014, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Samuel LOUP, Maintainer: Samuel LOUP

using System.Collections.Generic;
using System.Linq;

using Epsitec.Aider.Entities;
using Epsitec.Aider.Enumerations;
using Epsitec.Aider.Reporting;
using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Library;
using Epsitec.Cresus.Core.Entities;
using Epsitec.Common.Types;

namespace Epsitec.Aider.BusinessCases
{
	/// <summary>
	/// Exit Process Business Case
	/// </summary>
	public static class AiderPersonsExitProcess
	{
		public static void StartProcess(BusinessContext businessContext, AiderPersonEntity person)
		{
			var process = AiderOfficeProcessEntity
							.Create (businessContext, OfficeProcessType.PersonsOutputProcess, person);
			
			var offices        = businessContext.GetAllEntities<AiderOfficeManagementEntity> ();
			var officesGroups  = offices.Select (o => o.ParishGroup);
			var participations = person.GetParticipations ();
			var groups         = participations.Select (p => p.Group);
			var participationsByGroup = participations.ToDictionary (p => p.Group, p => p);
			
			foreach (var group in groups)
			{
				AiderOfficeManagementEntity office = null;
				if (group.IsParish ())
				{
					office = offices.Single (o => o.ParishGroup == group);
				}
				else
				{
					var searchPath = Enumerable.Repeat (group, 1).Union (group.Parents);
					var matchingGroups = searchPath.Intersect (officesGroups);
					office = offices.Single (o => o.ParishGroup == matchingGroups.Last ());
				}

				var participation = participationsByGroup[group];
				process.StartTaskInOffice (businessContext, OfficeTaskKind.CheckParticipation, office, participation);
	
			}
		}

		public static void EndProcess (BusinessContext businessContext, AiderOfficeProcessEntity process)
		{
			// persist last changes
			businessContext.SaveChanges (LockingPolicy.ReleaseLock, EntitySaveMode.None);

			var dataContext   = businessContext.DataContext;
			var person = process.GetSourceEntity<AiderPersonEntity> (dataContext);
			// check remaining participations
			if (person.GetParticipations ().Count == 0)
			{
				person.Visibility = PersonVisibilityStatus.Hidden;
				businessContext.DeleteEntity (person.MainContact);
				businessContext.DeleteEntity (person.HouseholdContact);
				// persist last changes
				businessContext.SaveChanges (LockingPolicy.ReleaseLock, EntitySaveMode.None);
				AiderHouseholdEntity.DeleteEmptyHouseholds (businessContext, person.Households);
			}
		}

		public static void DoRemoveParticipationTask (BusinessContext businessContext, AiderOfficeTaskEntity task)
		{
			var process       = task.Process;
			var dataContext   = businessContext.DataContext;
			var participation = task.GetSourceEntity<AiderGroupParticipantEntity> (dataContext);
			task.IsDone       = true;
			businessContext.DeleteEntity (participation);
			process.SetNextStatus ();
			if (process.Status == OfficeProcessStatus.Ended)
			{
				AiderPersonsExitProcess.EndProcess (businessContext, process);
			}
		}

		public static void DoKeepParticipationTask(BusinessContext businessContext, AiderOfficeTaskEntity task)
		{
			var process = task.Process;
			task.IsDone = true;
			process.SetNextStatus ();
			if (process.Status == OfficeProcessStatus.Ended)
			{
				AiderPersonsExitProcess.EndProcess (businessContext, process);
			}
		}
	}
}
