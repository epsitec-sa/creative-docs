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
using Epsitec.Cresus.DataLayer.Context;
using Epsitec.Aider.Data.Common;



namespace Epsitec.Aider.Data.Job
{
	internal static class DuplicatedPersonAutoMerger
	{
		public static void FindAndMerge(CoreData coreData)
		{
			
			using (var businessContext = new BusinessContext (coreData, false))
			{
				DuplicatedPersonAutoMerger.LogToConsole ("Perform automerge on duplicated persons", false);
				var addressesToCheckKeys = DuplicatedPersonAutoMerger.GetAddressesToFix (coreData);

				var total = addressesToCheckKeys.Count ();

				System.Console.Clear ();
				var current = 1;
				foreach (var key in addressesToCheckKeys)
				{

					DuplicatedPersonAutoMerger.LogToConsole ("{0}/{1}", true, current, total);
					current++;
					var address = (AiderAddressEntity) businessContext.DataContext.ResolveEntity (key);

					var contactExample = new AiderContactEntity ();
					contactExample.Address = address;
					contactExample.AddressType = AddressType.Default;

					var livingContacts = businessContext.DataContext.GetByExample<AiderContactEntity> (contactExample).ToList();				
					

					var potentialDuplicateChecker = new Dictionary<string, AiderPersonEntity> ();
					foreach (var contact in livingContacts)
					{
						if (contact.Person.eCH_Person.PersonFirstNames.IsNullOrWhiteSpace ())
						{
							continue;
						}

						var checkKey =	contact.Person.BirthdayDay.ToString ()
							 +		contact.Person.BirthdayMonth.ToString ()
							 +		contact.Person.BirthdayYear.ToString ()
							 +		contact.Person.eCH_Person.PersonFirstNames.Split (",").First ();

						if (!potentialDuplicateChecker.ContainsKey (checkKey))
						{
							potentialDuplicateChecker.Add (checkKey, contact.Person);
						}
						else
						{					
							//Try to merge
							try
							{
								var duplicatedPerson = potentialDuplicateChecker[checkKey];
								DuplicatedPersonAutoMerger.LogToConsole ("Found! {0}, try to merge with {1}", false, contact.GetDisplayName (), duplicatedPerson.GetDisplayName ());
								AiderPersonEntity.MergePersons (businessContext, contact.Person, duplicatedPerson);
								
							}
							catch
							{

							}
						}
					}
				}

				businessContext.SaveChanges (LockingPolicy.ReleaseLock, EntitySaveMode.None);		
			}
		}

		private static List<EntityKey> GetAddressesToFix
		(
			CoreData coreData
		)
		{
			var keys = new List<EntityKey> ();

			AiderEnumerator.Execute
			(
				coreData,
				(b, a) => DuplicatedPersonAutoMerger.GetAddressesToFix (b, a, keys)
			);

			return keys;
		}

		private static void GetAddressesToFix
		(
			BusinessContext businessContext,
			IEnumerable<AiderAddressEntity> addresses,
			List<EntityKey> addressKeys
		)
		{
			var addressesToFix = addresses
				.Where (a => !a.StreetUserFriendly.IsNullOrWhiteSpace ())
				.Select (a => businessContext.DataContext.GetNormalizedEntityKey (a).Value)
				.ToList ();

			addressKeys.AddRange (addressesToFix);
		}
		
		private static System.Diagnostics.Stopwatch LogToConsole(string format, bool fixedTop,params object[] args)
		{
			var message = string.Format (format, args);

			if (message.StartsWith ("Error"))
			{
				System.Console.ForegroundColor = System.ConsoleColor.Red;
			}

			if (fixedTop)
			{
				System.Console.SetCursorPosition (0, 0);
			}
			else
			{
				System.Console.Clear ();
				System.Console.SetCursorPosition (0, 2);
			}
		
			System.Console.WriteLine ("DuplicatedPersonAutoMerger: {0}", message);
			System.Console.ResetColor ();

			var time = new System.Diagnostics.Stopwatch ();

			time.Start ();

			return time;
		}
	}
}
