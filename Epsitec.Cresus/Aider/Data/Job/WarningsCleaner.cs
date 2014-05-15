//	Copyright © 2013-2014, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
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
	internal static class WarningsCleaner
	{
		public static void Before(CoreData coreData, Epsitec.Common.Types.Date date)
		{
			var cleanable = new List<WarningType> ();
			cleanable.Add (WarningType.EChPersonDataChanged);
			cleanable.Add (WarningType.EChHouseholdChanged);
			cleanable.Add (WarningType.EChHouseholdMissing);
			cleanable.Add (WarningType.HouseholdMissing);
			cleanable.Add (WarningType.EChAddressChanged);
			cleanable.Add (WarningType.ParishArrival);
			cleanable.Add (WarningType.ParishDeparture);
			cleanable.Add (WarningType.DerogationChange);
			cleanable.Add (WarningType.PersonBirth);
			cleanable.Add (WarningType.ParishMismatch);

			using (var businessContext = new BusinessContext (coreData, false))
			{
				foreach (var type in cleanable)
				{
					WarningsCleaner.LogToConsole ("Cleaning {0}",type.ToString ());
					DeleteWarningsBefore (businessContext, date, type);
					businessContext.SaveChanges (LockingPolicy.ReleaseLock, EntitySaveMode.None);
				}
			}
		}

		private static void DeleteWarningsBefore(BusinessContext businessContext, Epsitec.Common.Types.Date date, WarningType deleteValue)
		{
			

			var dataContext = businessContext.DataContext;

			var example = new AiderPersonWarningEntity ()
			{
				WarningType = deleteValue
			};

			var request = Request.Create (example);

			request.AddCondition (dataContext, example, x => x.StartDate < date);

			var warningsToDelete = dataContext.GetByRequest (request);

			var total = warningsToDelete.Count ();
			WarningsCleaner.LogToConsole ("{0} warnings to delete", total);

			var current = 1;
			foreach (var warn in warningsToDelete)
			{
				WarningsCleaner.ClearWarningAndRefreshCaches (businessContext, warn);
				System.Console.SetCursorPosition (0, 2);
				WarningsCleaner.LogToConsole ("{0}/{1}", current, total);
				current++;
			}
			System.Console.Clear ();
		}

		private static void ClearWarningAndRefreshCaches(BusinessContext businessContext, AiderPersonWarningEntity warning)
		{
			var person  = warning.Person;
			var context = businessContext;

			person.Contacts.ForEach (x => x.RefreshCache ());
			person.Households.ForEach (x => x.RefreshCache ());

			AiderPersonWarningEntity.Delete (context, warning);
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
