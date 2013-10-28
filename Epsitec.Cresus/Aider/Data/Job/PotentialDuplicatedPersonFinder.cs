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
	internal static class PotentialDuplicatedPersonFinder
	{
		public static void FindAll(CoreData coreData)
		{
			
			using (var businessContext = new BusinessContext (coreData, false))
			{
				PotentialDuplicatedPersonFinder.LogToConsole ("Perform DataQuality on Persons");
				var jobDateTime    = System.DateTime.Now;
				var jobName        = "PotentialDuplicatedPersonFinder";
				var jobDescription = string.Format ("Recherches de doublons potentiels");

				var warningSource = AiderPersonWarningSourceEntity.Create (businessContext, jobDateTime, jobName, TextFormatter.FormatText (jobDescription));
				var warningTitleMessage = TextFormatter.FormatText ("Doublon potentiel", jobDateTime.ToShortDateString ());

				var allAddresses = businessContext.GetAllEntities<AiderAddressEntity> ();
				var total = allAddresses.Count ();
				PotentialDuplicatedPersonFinder.LogToConsole ("{0} Addresses to check...",total);
				var addressesToCheck = allAddresses.Where (a => !a.StreetUserFriendly.IsNullOrWhiteSpace ());
				
				var current = 1;
				foreach (var address in addressesToCheck)
				{
					PotentialDuplicatedPersonFinder.LogToConsole ("{0}/{1}",current, total);
					current++;

					var contactExample = new AiderContactEntity ();
					contactExample.Address = address;
					contactExample.AddressType = AddressType.Default;

					var livingContacts = businessContext.DataContext.GetByExample<AiderContactEntity> (contactExample);				
					PotentialDuplicatedPersonFinder.LogToConsole ("{0} Persons at this address", livingContacts.Count ());

					var potentialDuplicateChecker = new Dictionary<string, AiderPersonEntity> ();
					foreach (var contact in livingContacts)
					{
						if (contact.Person.eCH_Person.PersonFirstNames.IsNullOrWhiteSpace ())
						{
							continue;
						}

						var key =	contact.Person.BirthdayDay.ToString ()
							 +		contact.Person.BirthdayMonth.ToString ()
							 +		contact.Person.BirthdayYear.ToString ()
							 +		contact.Person.eCH_Person.PersonFirstNames.Split (",").First ();

						if (!potentialDuplicateChecker.ContainsKey (key))
						{
							potentialDuplicateChecker.Add (key, contact.Person);
						}
						else
						{
							//AiderPersonWarningEntity.Create (businessContext, contact.Person, contact.Person.ParishGroupPathCache, WarningType.PotentialDuplicatedPerson, warningTitleMessage, warningSource);
						}
					}
				}

				businessContext.SaveChanges (LockingPolicy.ReleaseLock, EntitySaveMode.None);

				
			}
		}

		
		private static System.Diagnostics.Stopwatch LogToConsole(string format, params object[] args)
		{
			var message = string.Format (format, args);

			if (message.StartsWith ("Error"))
			{
				System.Console.ForegroundColor = System.ConsoleColor.Red;
			}

			System.Console.WriteLine ("PotentialDuplicatedPersonFinder: {0}", message);
			System.Console.ResetColor ();

			var time = new System.Diagnostics.Stopwatch ();

			time.Start ();

			return time;
		}

		private static void CreateWarning(BusinessContext context, AiderPersonEntity person, string parishGroupPath,
								   WarningType warningType, FormattedText title, FormattedText description, AiderWarningSourceEntity source)
		{
			var personId = person.eCH_Person.PersonId;

			if (string.IsNullOrEmpty (personId))
			{
				personId = context.DataContext.GetNormalizedEntityKey (person).ToString ();
			}

			AiderPersonWarningEntity.Create (context, person, parishGroupPath, warningType, title, description, source);
		}
	}
}
