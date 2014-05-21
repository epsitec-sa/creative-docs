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
using Epsitec.Common.Support.EntityEngine;



namespace Epsitec.Aider.Data.Job
{
	internal static class WarningCleaner
	{
		public static void Before(CoreData coreData, Epsitec.Common.Types.Date date, bool canKillPersons, bool canCreateSubscriptions)
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
					WarningCleaner.LogToConsole ("Cleaning {0}",type.ToString ());
					WarningCleaner.DeleteWarningsBefore (businessContext, date, type);
					businessContext.SaveChanges (LockingPolicy.ReleaseLock, EntitySaveMode.None);
				}

				//Process old EChProcessDeparture
				ProcessDepartureWarningsBefore (businessContext, date, canKillPersons);

				//Process old EChProcessArrival
				ProcessArrivalWarningsBefore (businessContext, date, canCreateSubscriptions);

				//Process old missing subscription
				ProcessMissingSubscriptionsWarningsBefore (businessContext, date);
			}
		}

		private static void ProcessMissingSubscriptionsWarningsBefore(BusinessContext businessContext, Epsitec.Common.Types.Date date)
		{
			var dataContext = businessContext.DataContext;

			var example = new AiderPersonWarningEntity ()
			{
				WarningType = WarningType.SubscriptionMissing
			};

			var request = Request.Create (example);

			request.AddCondition (dataContext, example, x => x.StartDate < date);

			var warningsToDelete = dataContext.GetByRequest (request);

			var total = warningsToDelete.Count ();
			WarningCleaner.LogToConsole ("{0} subscriptions missing warnings to process", total);

			var current = 1;
			foreach (var warn in warningsToDelete)
			{
				var contact = warn.Person.MainContact;
				if(contact.IsNotNull ())
				{
					var household = contact.Household;
					if(household.Address.Town.IsNotNull ())
					{
						EChDataHelpers.CreateOrUpdateAiderSubscription (businessContext, household);
					}					
				}

				WarningCleaner.ClearWarningAndRefreshCaches (businessContext, warn);
				System.Console.SetCursorPosition (0, 2);
				WarningCleaner.LogToConsole ("{0}/{1}", current, total);
				current++;
			}
			System.Console.Clear ();
		}

		private static void ProcessDepartureWarningsBefore(BusinessContext businessContext, Epsitec.Common.Types.Date date, bool canKillPersons)
		{
			var dataContext = businessContext.DataContext;

			var example = new AiderPersonWarningEntity ()
			{
				WarningType = WarningType.EChProcessDeparture
			};

			var request = Request.Create (example);

			request.AddCondition (dataContext, example, x => x.StartDate < date);

			var warningsToDelete = dataContext.GetByRequest (request);

			var total = warningsToDelete.Count ();
			WarningCleaner.LogToConsole ("{0} departure warnings to process", total);

			var current = 1;
			foreach (var warn in warningsToDelete)
			{
				var person  = warn.Person;

				if(person.IsAlive)
				{
					//hide person
					person.HidePerson (businessContext);
					WarningCleaner.ClearWarningAndRefreshCaches (businessContext, warn);
				}
				else
				{
					if(canKillPersons)
					{
						//kill person at with an uncertain warning start date
						var deceaseDate = warn.StartDate.HasValue ? warn.StartDate.Value : date;
						person.KillPerson (businessContext, deceaseDate, true);
						WarningCleaner.ClearWarningAndRefreshCaches (businessContext, warn);
					}				
				}
					
				System.Console.SetCursorPosition (0, 2);
				WarningCleaner.LogToConsole ("{0}/{1}", current, total);
				current++;
			}
			System.Console.Clear ();
		}

		private static void ProcessArrivalWarningsBefore(BusinessContext businessContext, Epsitec.Common.Types.Date date, bool canCreateSubscriptions)
		{
			var dataContext = businessContext.DataContext;

			var example = new AiderPersonWarningEntity ()
			{
				WarningType = WarningType.EChProcessArrival
			};

			var request = Request.Create (example);

			request.AddCondition (dataContext, example, x => x.StartDate < date);

			var warningsToDelete = dataContext.GetByRequest (request);

			var total = warningsToDelete.Count ();
			WarningCleaner.LogToConsole ("{0} arrivals warnings to process", total);

			var current = 1;
			foreach (var warn in warningsToDelete)
			{
				var person  = warn.Person;
				if(person.MainContact.IsNull ())
				{
					//Skip, PersonWithoutContact job, may resolve this before
					continue;
				}

				if (person.HouseholdContact.IsNotNull () && canCreateSubscriptions)
				{
					var subscription = AiderSubscriptionEntity.FindSubscription (businessContext, person.MainContact.Household);
					var refusal = AiderSubscriptionRefusalEntity.FindRefusal (businessContext, person.MainContact.Household);
					if(subscription.IsNull () && refusal.IsNull ())
					{
						var household = person.Households.FirstOrDefault ();
						AiderSubscriptionEntity.Create (businessContext, household);
					}
				}

				WarningCleaner.ClearWarningAndRefreshCaches (businessContext, warn);
				System.Console.SetCursorPosition (0, 2);
				WarningCleaner.LogToConsole ("{0}/{1}", current, total);
				current++;
			}
			System.Console.Clear ();
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
			WarningCleaner.LogToConsole ("{0} warnings to delete", total);

			var current = 1;
			foreach (var warn in warningsToDelete)
			{
				WarningCleaner.ClearWarningAndRefreshCaches (businessContext, warn);
				System.Console.SetCursorPosition (0, 2);
				WarningCleaner.LogToConsole ("{0}/{1}", current, total);
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
