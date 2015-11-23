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
		public static AiderOfficeProcessEntity StartHouseholdDeletionProcess(BusinessContext businessContext, AiderHouseholdEntity household)
		{
			var process = AiderOfficeProcessEntity
							.Create (businessContext, OfficeProcessType.HouseholdDeletionProcess, household);
			var members = household.Members.Distinct ();
			foreach (var person in members)
			{
				AiderPersonsProcess.AddTasksForPerson (businessContext, process, person);
			}

			AiderPersonsProcess.Next (businessContext, process);
			return process;
		}

		public static AiderOfficeProcessEntity StartExitProcess(BusinessContext businessContext, AiderPersonEntity person, OfficeProcessType type)
		{
			var process = AiderOfficeProcessEntity
							.Create (businessContext, type, person);

			AiderPersonsProcess.AddTasksForPerson (businessContext, process, person);

			AiderPersonsProcess.Next (businessContext, process);
			return process;
		}

		public static void EndProcess (BusinessContext businessContext, AiderOfficeProcessEntity process)
		{
			if (process.Type == OfficeProcessType.PersonsOutputProcess)
			{
				var dataContext   = businessContext.DataContext;
				var person = process.GetSourceEntity<AiderPersonEntity> (dataContext);
				AiderPersonsProcess.PersonExitProcess (businessContext, person);

				businessContext.SaveChanges (LockingPolicy.ReleaseLock, EntitySaveMode.None);
				AiderHouseholdEntity.DeleteEmptyHouseholds (businessContext, person.Households);
			}

			if (process.Type == OfficeProcessType.HouseholdDeletionProcess)
			{
				var household = process.GetSourceEntity<AiderHouseholdEntity> (businessContext.DataContext);
				var members = household.Members.Distinct ();
				foreach (var person in members)
				{
					AiderPersonsProcess.PersonExitProcess (businessContext, person);
				}

				businessContext.SaveChanges (LockingPolicy.ReleaseLock, EntitySaveMode.None);
				AiderHouseholdEntity.DeleteEmptyHouseholds (businessContext, household);
			}
		}

		public static void Next (BusinessContext businessContext, AiderOfficeProcessEntity process)
		{
			businessContext.SaveChanges (LockingPolicy.ReleaseLock, EntitySaveMode.None);
			process.SetNextStatus ();
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
			AiderGroupParticipantEntity.StopParticipation (participation, Date.Today);

			AiderPersonsProcess.Next (businessContext, process);
		}

		public static void DoKeepParticipationTask(BusinessContext businessContext, AiderOfficeTaskEntity task)
		{
			var process = task.Process;
			if (process.Type == OfficeProcessType.PersonsOutputProcess)
			{
				// Add new address task if not alreay present
				if (!process.Tasks.Any (t => t.Kind == OfficeTaskKind.EnterNewAddress))
				{
					var dataContext = businessContext.DataContext;
					var person  = task.Process.GetSourceEntity<AiderPersonEntity> (dataContext);
					if (person.MainContact.IsNull ())
					{
						throw new BusinessRuleException ("Impossible de conserver la participation, la personne n'a plus de contact");
					}
					process.StartTaskInOffice (businessContext, OfficeTaskKind.EnterNewAddress, task.Office, person.MainContact);
					businessContext.SaveChanges (LockingPolicy.ReleaseLock, EntitySaveMode.None);
				}
			}

			task.IsDone = true;
			task.Actor  = businessContext.GetLocalEntity (AiderUserManager.Current.AuthenticatedUser);

			AiderPersonsProcess.Next (businessContext, process);
		}

		private static void PersonExitProcess (BusinessContext businessContext, AiderPersonEntity person)
		{
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
			}
		}

		private static void AddTasksForPerson(BusinessContext businessContext, AiderOfficeProcessEntity process, AiderPersonEntity person)
		{
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

				var autoDeletedGroups = new List<string> ()
				{
					"Arrivées",
					"Personnes importées"
				};

				if (autoDeletedGroups.Contains (group.Name))
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


				var started = false;
				participationsByGroup[group].ForEach (p =>
				{
					if (!started)
					{
						process.StartTaskInOffice (businessContext, OfficeTaskKind.CheckParticipation, office, p);
						started = true;
					}
				});
			}
		}
	}
}
