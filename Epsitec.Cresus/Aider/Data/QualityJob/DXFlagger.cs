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
	internal static class FlagDuplicatedPersons
	{
		public static void Run(CoreData coreData)
		{
			FlagDuplicatedPersons.LogToConsole ("Flagging duplicated persons...", false);
			var addressesToCheckKeys = FlagDuplicatedPersons.GetAddressesToFix (coreData);

			var total = addressesToCheckKeys.Count ();

			System.Console.Clear ();
			var current = 1;
			foreach (var key in addressesToCheckKeys)
			{
				FlagDuplicatedPersons.LogToConsole ("{0}/{1}", true, current, total);
				current++;
				using (var businessContext = new BusinessContext (coreData, false))
				{
					var address = (AiderAddressEntity)businessContext.DataContext.ResolveEntity (key);

					var contactExample = new AiderContactEntity ();
					contactExample.Address = address;
					contactExample.AddressType = AddressType.Default;

					var livingContacts = businessContext.DataContext.GetByExample<AiderContactEntity> (contactExample).ToList ();


					var potentialDuplicateChecker = new Dictionary<string, AiderPersonEntity> ();
					foreach (var contact in livingContacts)
					{
						if (contact.Person.eCH_Person.PersonFirstNames.IsNullOrWhiteSpace ())
						{
							continue;
						}

						var checkKey = contact.Person.BirthdayDay.ToString ()
								+ contact.Person.BirthdayMonth.ToString ()
								+ contact.Person.BirthdayYear.ToString ()
								+ contact.Person.eCH_Person.PersonFirstNames.Split (",").First ();

						if (!potentialDuplicateChecker.ContainsKey (checkKey))
						{
							potentialDuplicateChecker.Add (checkKey, contact.Person);
						}
						else
						{
							var duplicatedPerson = potentialDuplicateChecker[checkKey];
							var contactsToFlag = duplicatedPerson.Contacts.Union (contact.Person.Contacts);
							var code = FlagDuplicatedPersons.GetCode (contactsToFlag);
							foreach (var c2f in contactsToFlag)
							{
								if (c2f.QualityCode.IsNullOrWhiteSpace ())
								{
									c2f.QualityCode += code;
								}
								else
								{
									if (c2f.QualityCode.IndexOf ('D') < 0)
									{
										c2f.QualityCode += code;
									}
								}
							}
						}
					}
					businessContext.SaveChanges (LockingPolicy.ReleaseLock, EntitySaveMode.None);
				}
			}
		}

		private static string GetCode (IEnumerable<AiderContactEntity> contacts)
		{
			// D1 si vrai doublon (l'un est RCH et l'autre de source manuelle)
			// D2 idem, mais le contact RCH est Hidden
			// D3 si le deux sont RCH
			var isPair = contacts.Count () == 2;

			if (isPair)
			{
				var c1 = contacts.ElementAt (0);
				var c2 = contacts.ElementAt (1);

				var c1IsGov = c1.Person.IsGovernmentDefined;
				var c2IsGov = c2.Person.IsGovernmentDefined;
				var oneOfTwoIsGov = (c1IsGov || c2IsGov) && !(c1IsGov && c2IsGov);
                var c1IsHidden = c1.Person.Visibility == PersonVisibilityStatus.Hidden;
				var c2IsHidden = c2.Person.Visibility == PersonVisibilityStatus.Hidden;

				if (oneOfTwoIsGov)
				{
					if (c1IsHidden || c2IsHidden)
					{
						return "D2;";
					}
					return "D1;";
				}
				else
				{
					if (c1IsGov && c2IsGov)
					{
						return "D3;";
					}
					return "D;";
				}
			}
			return "D;";
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
				(b, a) => FlagDuplicatedPersons.GetAddressesToFix (b, a, keys)
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
