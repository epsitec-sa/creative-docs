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
using Epsitec.Aider.Override;
using Epsitec.Common.Support.Extensions;

namespace Epsitec.Aider.BusinessCases
{
	/// <summary>
	/// Parish Change Business Case
	/// </summary>
	public static class AiderParishChangeProcess
	{
		public static void StartProcess(BusinessContext businessContext, AiderPersonEntity person)
		{
			var process = AiderOfficeProcessEntity
							.Create (businessContext, OfficeProcessType.PersonsParishChangeProcess, person);
			
			var offices        = businessContext.GetAllEntities<AiderOfficeManagementEntity> ();
			var officesGroups  = offices.Select (o => o.ParishGroup);
			var participations = person.GetParticipations ();
			var groups         = participations.Select (p => p.Group).ToList ();

			var participationsByGroup = participations.ToLookup (p => p.Group, p => p);
			
			foreach (var group in groups)
			{
				AiderOfficeManagementEntity office = null;
				if (group.IsNoParish ())
				{
					continue;
				}

				if (group.IsParish ())
				{
					participationsByGroup[group].ForEach (p =>
					{
						AiderGroupParticipantEntity.StopParticipation (p, Date.Today);
					});

					continue;
				}

				
				var searchPath     = Enumerable.Repeat (group, 1).Union (group.Parents);
				if (searchPath.Any (g => g.Name == "Archives"))
				{
					participationsByGroup[group].ForEach (p =>
					{
						AiderGroupParticipantEntity.StopParticipation (p, Date.Today);
					});
					continue;
				}


				var matchingGroups = searchPath.Intersect (officesGroups);
				var officeGroup    = matchingGroups.Last ();
				office = offices.Single (o => o.ParishGroup == officeGroup);
				

				participationsByGroup[group].ForEach (p =>
				{
					process.StartTaskInOffice (businessContext, OfficeTaskKind.CheckParticipation, office, p);
				});
				
	
			}

			AiderParishChangeProcess.Next (businessContext, process);
		}

		public static void EndProcess (BusinessContext businessContext, AiderOfficeProcessEntity process)
		{
			// noting to do
		}

		public static void Next (BusinessContext businessContext, AiderOfficeProcessEntity process)
		{
			process.SetNextStatus ();
			// persist last changes
			businessContext.SaveChanges (LockingPolicy.ReleaseLock, EntitySaveMode.None);

			if (process.Status == OfficeProcessStatus.Ended)
			{
				AiderParishChangeProcess.EndProcess (businessContext, process);
			}
		}

		public static void DoRemoveParticipationTask (BusinessContext businessContext, AiderOfficeTaskEntity task)
		{
			var process       = task.Process;
			var dataContext   = businessContext.DataContext;
			var participation = task.GetSourceEntity<AiderGroupParticipantEntity> (dataContext);
			task.IsDone       = true;
			task.Actor        = businessContext.GetLocalEntity (AiderUserManager.Current.AuthenticatedUser);
			businessContext.DeleteEntity (participation);

			AiderParishChangeProcess.Next (businessContext, process);
		}

		public static void DoKeepParticipationTask(BusinessContext businessContext, AiderOfficeTaskEntity task)
		{
			var process = task.Process;
			task.IsDone = true;
			task.Actor  = businessContext.GetLocalEntity (AiderUserManager.Current.AuthenticatedUser);

			AiderParishChangeProcess.Next (businessContext, process);
		}
	}
}
