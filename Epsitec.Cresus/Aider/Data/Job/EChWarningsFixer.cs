//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Samuel LOUP, Maintainer: Samuel LOUP

using Epsitec.Aider.Entities;
using System.Linq;
using Epsitec.Common.Support.Extensions;
using Epsitec.Cresus.Core;
using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.DataLayer.Expressions;
using Epsitec.Cresus.DataLayer.Loader;
using Epsitec.Cresus.Database;
using System.Data;
using System.Collections.Generic;
using Epsitec.Common.Types;
using Epsitec.Aider.Enumerations;



namespace Epsitec.Aider.Data.Job
{
	internal static class EChWarningsFixer
	{
		public static void TryFixAll(CoreData coreData)
		{
			
			using (var businessContext = new BusinessContext (coreData, false))
			{

				EChWarningsFixer.LogToConsole ("Fix Reported Person Linkage For arrivals");

				EChWarningsFixer.FixReportedPersonLinkageForArrivals (businessContext);

				EChWarningsFixer.LogToConsole ("Migrating old Ech Warnings: EChPersonMissing -> EChProcessDeparture");
				
				EChWarningsFixer.MigrateWarning (businessContext, WarningType.EChPersonMissing, WarningType.EChProcessDeparture);

				EChWarningsFixer.LogToConsole ("Fixing old Ech Status from EChProcessDeparture Warnings");

				EChWarningsFixer.ResetEChStatusForProcessDepartureWarnings (businessContext);

				EChWarningsFixer.LogToConsole ("Migrating old Ech Warnings: EChPersonNew -> EChProcessArrival");

				EChWarningsFixer.MigrateWarning (businessContext, WarningType.EChPersonNew, WarningType.EChProcessArrival);

				EChWarningsFixer.LogToConsole ("Delete old Ech Warnings: EChPersonDuplicated ");

				EChWarningsFixer.DeleteWarning (businessContext, WarningType.EChPersonDuplicated);

				EChWarningsFixer.LogToConsole ("Delete old Ech Warnings: EChHouseholdChanged ");

				EChWarningsFixer.DeleteWarning (businessContext, WarningType.EChHouseholdChanged);


				EChWarningsFixer.LogToConsole ("Delete warnings mismatch in time");

				EChWarningsFixer.RemoveWarningInTimeMismatch (businessContext);

				

				businessContext.SaveChanges (LockingPolicy.ReleaseLock, EntitySaveMode.None);
			}
		}

		private static void FixReportedPersonLinkageForArrivals(BusinessContext businessContext)
		{
			var example = new AiderPersonWarningEntity ()
			{
				WarningType = WarningType.EChProcessArrival
			};

			var allArrivalsByPersonId = businessContext.DataContext.GetByExample<AiderPersonWarningEntity> (example).Select(k => k.Person.eCH_Person);

			var membersInReportedPerson = new Dictionary<string, List<eCH_ReportedPersonEntity>> ();

			var allReportedPersons = businessContext.GetAllEntities<eCH_ReportedPersonEntity> ();

			var total = allReportedPersons.Count ();
			var current = 1;
			foreach (var reportedPerson in allReportedPersons)
			{
				System.Console.SetCursorPosition (0, 2);
				EChWarningsFixer.LogToConsole ("{0}/{1}", current, total);
				current++;
	
				var reportedPersonList = new List<eCH_ReportedPersonEntity> ();
				foreach (var member in reportedPerson.Members)
				{
					if (member.IsNotNull ())
					{
					
						if (!membersInReportedPerson.TryGetValue (member.PersonId, out reportedPersonList))
						{
							reportedPersonList = new List<eCH_ReportedPersonEntity> ();
							reportedPersonList.Add (reportedPerson);
							membersInReportedPerson.Add (member.PersonId, reportedPersonList);
						}
						else
						{
							reportedPersonList.Add (reportedPerson);
						}
					}
				}
			}

			foreach (var eChPerson in allArrivalsByPersonId)
			{
				if (eChPerson.ReportedPersons.Count () < 1)
				{
					var reportedPersons = membersInReportedPerson[eChPerson.PersonId];
					eChPerson.ReportedPerson1 = reportedPersons[0];
					if (reportedPersons.Count > 1)
					{
						eChPerson.ReportedPerson2 = reportedPersons[1];
					}
				}
			}

			businessContext.SaveChanges (LockingPolicy.ReleaseLock, EntitySaveMode.None);
		}

		private static void ResetEChStatusForProcessDepartureWarnings(BusinessContext businessContext)
		{
			var example = new AiderPersonWarningEntity ()
			{
				WarningType = WarningType.EChProcessDeparture
			};

			var warningsToFix = businessContext.DataContext.GetByExample<AiderPersonWarningEntity> (example);

			var total = warningsToFix.Count ();
			EChWarningsFixer.LogToConsole ("{0} EChStatus to Reset", total);

			var current = 1;
			foreach (var warning in warningsToFix)
			{
				warning.Person.eCH_Person.DeclarationStatus = PersonDeclarationStatus.Removed;
				warning.Person.eCH_Person.RemovalReason = RemovalReason.None;
				System.Console.SetCursorPosition (0, 2);
				EChWarningsFixer.LogToConsole ("{0}/{1}", current, total);
				current++;
			}
			System.Console.Clear ();
		}

		private static void RemoveWarningInTimeMismatch(BusinessContext businessContext)
		{
			var example = new AiderPersonWarningEntity ()
			{
				WarningType = WarningType.EChProcessArrival
			};

			var allArrivalsByPersonId = businessContext.DataContext.GetByExample<AiderPersonWarningEntity> (example).ToLookup (k => k.Person.eCH_Person);

			example = new AiderPersonWarningEntity ()
			{
				WarningType = WarningType.EChProcessDeparture
			};

			var allDepartureByPersonId = businessContext.DataContext.GetByExample<AiderPersonWarningEntity> (example).ToLookup (k => k.Person.eCH_Person);

			var personsWithMismatch = allDepartureByPersonId.Where (a => allArrivalsByPersonId.Contains (a.Key));

			foreach (var person in personsWithMismatch)
			{
				var personExample = new AiderPersonEntity ()
				{
					eCH_Person = person.Key
				};

				var personEntity = businessContext.DataContext.GetByExample<AiderPersonEntity> (personExample).FirstOrDefault ();
				EChWarningsFixer.LogToConsole ("/// {0}:", personEntity.GetFullName ());

				var warnings = personEntity.Warnings.OrderBy (w => w.StartDate);

				

				foreach (var warn in warnings)
				{
					EChWarningsFixer.LogToConsole ("{0} {1} detected",warn.Title, warn.WarningType);
				}

				var warningToDelete = warnings.Where(w => w.WarningType == WarningType.EChProcessDeparture || w.WarningType == WarningType.EChProcessArrival).First ();
				EChWarningsFixer.LogToConsole ("REMOVED {0} {1}", warningToDelete.Title, warningToDelete.WarningType);
				businessContext.DeleteEntity (warningToDelete);

				EChWarningsFixer.LogToConsole ("/////////////////");

			}			
		}

		private static void DeleteWarning(BusinessContext businessContext, WarningType deleteValue)
		{
			var example = new AiderPersonWarningEntity ()
			{
				WarningType = deleteValue
			};

			var warningsToDelete = businessContext.DataContext.GetByExample<AiderPersonWarningEntity> (example);

			var total = warningsToDelete.Count ();
			EChWarningsFixer.LogToConsole ("{0} warnings to delete", total);

			var current = 1;
			foreach (var warn in warningsToDelete)
			{
				businessContext.DeleteEntity (warn);
				System.Console.SetCursorPosition (0, 2);
				EChWarningsFixer.LogToConsole ("{0}/{1}", current, total);
				current++;
			}
			System.Console.Clear ();
		}

		private static void MigrateWarning(BusinessContext businessContext, WarningType initialValue, WarningType migratedValue)
		{
			var example = new AiderPersonWarningEntity()
			{
				WarningType = initialValue
			};

			var warningsToMigrate = businessContext.DataContext.GetByExample<AiderPersonWarningEntity>(example);

			var total = warningsToMigrate.Count ();
			EChWarningsFixer.LogToConsole ("{0} warnings to migrate", total);

			var current = 1;
			foreach (var warn in warningsToMigrate)
			{
				warn.WarningType = migratedValue;
				System.Console.SetCursorPosition (0, 2);
				EChWarningsFixer.LogToConsole ("{0}/{1}",current, total);
				current++;
			}
			System.Console.Clear ();
		}

		private static System.Diagnostics.Stopwatch LogToConsole(string format, params object[] args)
		{
			var message = string.Format (format, args);

			if (message.StartsWith ("Error"))
			{
				System.Console.ForegroundColor = System.ConsoleColor.Red;
			}

			System.Console.WriteLine ("WarningFixer: {0}", message);
			System.Console.ResetColor ();

			var time = new System.Diagnostics.Stopwatch ();

			time.Start ();

			return time;
		}
	}
}
