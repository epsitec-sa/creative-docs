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
	/// Exit Process Business Case
	/// </summary>
	public static class AiderPersonsProcess
	{
		public static void StartProcess(BusinessContext businessContext, AiderPersonEntity person, OfficeProcessType type)
		{
			var process = AiderOfficeProcessEntity
							.Create (businessContext, type, person);
			
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

			AiderPersonsProcess.Next (businessContext, process);
		}

		public static void EndProcess (BusinessContext businessContext, AiderOfficeProcessEntity process)
		{
			if (process.Type == OfficeProcessType.PersonsOutputProcess)
			{
				var dataContext   = businessContext.DataContext;
				var person = process.GetSourceEntity<AiderPersonEntity> (dataContext);
				// check remaining participations
				if (person.GetParticipations ().Count == 0)
				{
					person.Visibility = PersonVisibilityStatus.Hidden;
					if (person.MainContact.IsNotNull ())
					{
						businessContext.DeleteEntity (person.MainContact);
					}
					if (person.HouseholdContact.IsNotNull ())
					{
						businessContext.DeleteEntity (person.HouseholdContact);
					}

					// persist last changes
					businessContext.SaveChanges (LockingPolicy.ReleaseLock, EntitySaveMode.None);
					AiderHouseholdEntity.DeleteEmptyHouseholds (businessContext, person.Households);
				}
			}
		}

		public static void Next (BusinessContext businessContext, AiderOfficeProcessEntity process)
		{
			process.SetNextStatus ();
			// persist last changes
			businessContext.SaveChanges (LockingPolicy.ReleaseLock, EntitySaveMode.None);

			if (process.Status == OfficeProcessStatus.Ended)
			{
				AiderPersonsProcess.EndProcess (businessContext, process);
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

			AiderPersonsProcess.Next (businessContext, process);
		}

		public static void DoKeepParticipationTask(BusinessContext businessContext, AiderOfficeTaskEntity task)
		{
			var process = task.Process;
			
			// Add new address task if not alreay present
			if (!process.Tasks.Any (t => t.Kind == OfficeTaskKind.EnterNewAddress))
			{
				var dataContext = businessContext.DataContext;
				var person  = task.Process.GetSourceEntity<AiderPersonEntity> (dataContext);
				var address = person.MainContact.Address;
				process.StartTaskInOffice (businessContext, OfficeTaskKind.EnterNewAddress, task.Office, address);
			}
			

			task.IsDone = true;
			task.Actor  = businessContext.GetLocalEntity (AiderUserManager.Current.AuthenticatedUser);

			AiderPersonsProcess.Next (businessContext, process);
		}
	}
}
