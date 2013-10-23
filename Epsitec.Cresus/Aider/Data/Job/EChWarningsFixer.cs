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
				EChWarningsFixer.LogToConsole ("Migrating old Ech Warnings");
				
				EChWarningsFixer.MigrateWarning (businessContext, WarningType.EChPersonMissing, WarningType.EChProcessDeparture);

				EChWarningsFixer.MigrateWarning (businessContext, WarningType.EChPersonNew, WarningType.EChProcessArrival);

				EChWarningsFixer.DeleteWarning (businessContext, WarningType.EChPersonDuplicated);

				EChWarningsFixer.DeleteWarning (businessContext, WarningType.EChHouseholdChanged);

				EChWarningsFixer.RemoveWarningInTimeMismatch (businessContext);

				businessContext.SaveChanges (LockingPolicy.ReleaseLock, EntitySaveMode.None);
			}
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
				EChWarningsFixer.LogToConsole ("{0}/{1}", current, total);
				current++;
			}
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
				EChWarningsFixer.LogToConsole ("{0}/{1}",current, total);
				current++;
			}
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
