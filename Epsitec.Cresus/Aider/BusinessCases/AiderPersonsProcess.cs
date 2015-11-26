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
using Epsitec.Cresus.DataLayer.Loader;
using Epsitec.Cresus.DataLayer.Expressions;

namespace Epsitec.Aider.BusinessCases
{
	/// <summary>
	/// Exit Process Business Case
	/// </summary>
	public static class AiderPersonsProcess
	{
		public static AiderOfficeProcessEntity StartExitProcess(BusinessContext businessContext, AiderPersonEntity person, OfficeProcessType type)
		{
			if (person.Employee.IsNotNull ())
			{
				throw new BusinessRuleException ("Impossible de démarrer le processus pour " + person.GetFullName () + ", la personne est employée");
			}

			var notif = NotificationManager.GetCurrentNotificationManager ();
			var user  = AiderUserManager.Current.AuthenticatedUser;
			if (user.IsNotNull ())
			{
				notif.Notify (user.LoginName,
				new NotificationMessage ()
				{
					Title = "Processus démarré",
					Body = person.GetSummary ()
				},
				When.Now);
			}
			var process = AiderOfficeProcessEntity
							.Create (businessContext, type, person);

			AiderPersonsProcess.AddCheckParticipationTasksForPerson (businessContext, process, person);

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
				AiderHouseholdEntity.DeleteEmptyHouseholds (businessContext, person.Households);
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
			// This action is redo'able!
			var process       = task.Process;
			var dataContext   = businessContext.DataContext;
			var participation = task.GetSourceEntity<AiderGroupParticipantEntity> (dataContext);
			task.IsDone       = true;

			if (task.Actor.IsNull ()) // don't change actor name in case of redo
			{
				task.Actor = businessContext.GetLocalEntity (AiderUserManager.Current.AuthenticatedUser);
			}
			
			if (participation.IsNotNull ())
			{
				AiderGroupParticipantEntity.StopParticipation (participation, Date.Today);
				businessContext.DeleteEntity (participation);
			}		
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
			if (person.GetParticipations (reload: true).Count == 0)
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

			// prevent bindind side-effects during save
			businessContext.ClearRegisteredEntities ();
			businessContext.SaveChanges (LockingPolicy.ReleaseLock, EntitySaveMode.None);
		}

		public static void AddCheckParticipationTasksForPerson(BusinessContext businessContext, AiderOfficeProcessEntity process, AiderPersonEntity person)
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
					participationsByGroup[group].ForEach (p =>
					{
						AiderGroupParticipantEntity.StopParticipation (p, Date.Today);
						businessContext.DeleteEntity (p);
					});
					continue;
				}

				if (group.IsParish ())
				{
					participationsByGroup[group].ForEach (p =>
					{
						AiderGroupParticipantEntity.StopParticipation (p, Date.Today);
						businessContext.DeleteEntity (p);
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
						businessContext.DeleteEntity (p);
					});

					continue;
				}

				var searchPath     = Enumerable.Repeat (group, 1).Union (group.Parents);
				if (searchPath.Any (g => g.Name == "Archives"))
				{
					participationsByGroup[group].ForEach (p =>
					{
						AiderGroupParticipantEntity.StopParticipation (p, Date.Today);
						businessContext.DeleteEntity (p);
					});
					continue;
				}


				var matchingGroups = searchPath.Intersect (officesGroups);
				if (matchingGroups.IsEmpty ())
				{
					throw new BusinessRuleException ("Processus de sortie des personnes:\nIl manque une gestion dans:\n " + 
						group.GetRootName () + 
						"\ngroupe:\n" + 
						group.Name + 
						"\nchemin du groupe:\n" + 
					group.Path);
				}
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

			// prevent bindind side-effects during save
			businessContext.ClearRegisteredEntities ();
			businessContext.SaveChanges (LockingPolicy.ReleaseLock, EntitySaveMode.None);
		}
	}
}
