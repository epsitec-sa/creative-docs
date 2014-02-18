//	Copyright © 2013-2014, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Marc BETTEX, Maintainer: Pierre ARNAUD

using Epsitec.Aider.Data.Common;
using Epsitec.Aider.Entities;
using Epsitec.Aider.Enumerations;

using Epsitec.Common.IO;

using Epsitec.Cresus.Core;
using Epsitec.Cresus.Core.Business;

using Epsitec.Cresus.DataLayer.Context;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Epsitec.Cresus.Core.Entities;

namespace Epsitec.Aider.Data.Job
{
	/// <summary>
	/// This job corrects all parish assignations in the database.
	/// </summary>
	public static class ParishAssignationFixer
	{
		public static void FixNoParish(CoreData coreData)
		{
			using (var businessContext = new BusinessContext (coreData, false))
			{
				var noParishGroup = ParishAssigner.FindNoParishGroup (businessContext);

				int count = noParishGroup.FindParticipantCount (businessContext.DataContext);
				var all   = noParishGroup.FindParticipants (businessContext.DataContext, count).ToArray ();

				foreach (var person in all.Select (x => x.Person).Where (x => x.IsNotNull ()))
				{
					if (Rules.AiderPersonBusinessRules.ReassignParish (businessContext, person))
					{
						System.Diagnostics.Debug.WriteLine ("Reassigned "+person.DisplayName);
					}
				}

				businessContext.SaveChanges (LockingPolicy.ReleaseLock, EntitySaveMode.None);
			}
		}

		public static void FixParishAssignations
		(
			ParishAddressRepository parishRepository,
			CoreData coreData
		)
		{
			var personKeys = ParishAssignationFixer.GetPersonsToFix (parishRepository, coreData);
			Logger.LogToConsole (personKeys.Count + " PERSONS TO FIX");
			ParishAssignationFixer.FixPersons (coreData, parishRepository, personKeys);
			Logger.LogToConsole ("PERSONS FIXED");

			var legalPersonKeys = ParishAssignationFixer.GetLegalPersonsToFix (parishRepository, coreData);
			Logger.LogToConsole (legalPersonKeys.Count + " LEGAL PERSONS TO FIX");
			ParishAssignationFixer.FixLegalPersons (coreData, parishRepository, legalPersonKeys);
			Logger.LogToConsole ("LEGAL PERSONS FIXED");

			Logger.LogToConsole ("CLEANING WARNINGS");
			ParishAssignationFixer.RemoveParishMismatchWarnings (coreData);
			Logger.LogToConsole ("CLEANED WARNINGS");
		}


		private static List<EntityKey> GetPersonsToFix
		(
			ParishAddressRepository parishRepository,
			CoreData coreData
		)
		{
			var keys = new List<EntityKey> ();

			AiderEnumerator.Execute
			(
				coreData,
				(b, p) => ParishAssignationFixer.GetPersonsToFix (parishRepository, b, p, keys)
			);

			return keys;
		}


		private static void GetPersonsToFix
		(
			ParishAddressRepository parishRepository,
			BusinessContext businessContext,
			IEnumerable<AiderPersonEntity> persons,
			List<EntityKey> personKeys
		)
		{
			var personsToFix = persons
				.Where (p => !ParishAssigner.IsInValidParish (parishRepository, p))
				.Select (p => businessContext.DataContext.GetNormalizedEntityKey (p).Value)
				.ToList ();

			personKeys.AddRange (personsToFix);

			Logger.LogToConsole ("DONE PERSON BATCH");
		}


		private static void FixPersons
		(
			CoreData coreData,
			ParishAddressRepository parishRepository,
			IEnumerable<EntityKey> keys
		)
		{
			using (var businessContext = new BusinessContext (coreData, false))
			{
				foreach (var key in keys)
				{
					ParishAssignationFixer.FixPerson (businessContext, parishRepository, key);
				}

				businessContext.SaveChanges
				(
					LockingPolicy.ReleaseLock, EntitySaveMode.IgnoreValidationErrors
				);
			}
		}


		private static void FixPerson
		(
			BusinessContext businessContext,
			ParishAddressRepository parishRepository,
			EntityKey key
		)
		{
			var dataContext = businessContext.DataContext;

			var person = (AiderPersonEntity) dataContext.ResolveEntity (key);

			businessContext.Register (person);

			ParishAssigner.ReassignToParish (parishRepository, businessContext, person);
		}


		private static List<EntityKey> GetLegalPersonsToFix
		(
			ParishAddressRepository parishRepository,
			CoreData coreData
		)
		{
			var keys = new List<EntityKey> ();

			AiderEnumerator.Execute
			(
				coreData,
				(b, l) => ParishAssignationFixer.GetLegalPersonsToFix (parishRepository, b, l, keys)
			);

			return keys;
		}


		private static void GetLegalPersonsToFix
		(
			ParishAddressRepository parishRepository,
			BusinessContext businessContext,
			IEnumerable<AiderLegalPersonEntity> legalPersons,
			List<EntityKey> legalPersonKeys
		)
		{
			var legalPersonsToFix = legalPersons
				.Where (lp => !ParishAssigner.IsInValidParish (parishRepository, lp))
				.Select (lp => businessContext.DataContext.GetNormalizedEntityKey (lp).Value)
				.ToList ();

			legalPersonKeys.AddRange (legalPersonsToFix);

			Logger.LogToConsole ("DONE LEGAL PERSON BATCH");
		}


		private static void FixLegalPersons
		(
			CoreData coreData,
			ParishAddressRepository parishRepository,
			IEnumerable<EntityKey> keys
		)
		{
			using (var businessContext = new BusinessContext (coreData, false))
			{
				foreach (var key in keys)
				{
					ParishAssignationFixer.FixLegalPerson (businessContext, parishRepository, key);
				}

				businessContext.SaveChanges
				(
					LockingPolicy.ReleaseLock, EntitySaveMode.IgnoreValidationErrors
				);
			}
		}


		private static void FixLegalPerson
		(
			BusinessContext businessContext,
			ParishAddressRepository parishRepository,
			EntityKey key
		)
		{
			var dataContext = businessContext.DataContext;

			var legalPerson = (AiderLegalPersonEntity) dataContext.ResolveEntity (key);

			businessContext.Register (legalPerson);

			ParishAssigner.ReassignToParish (parishRepository, businessContext, legalPerson);
		}


		private static void RemoveParishMismatchWarnings(CoreData coreData)
		{
			// This cleanup method might be too simplistic in the future. For now it works because
			// the warnings are almost not used and not tied to business rules. Should that change
			// in the future, we might want to redesign this method.

			using (var businessContext = new BusinessContext (coreData, false))
			{
				var warningExample = new AiderPersonWarningEntity ()
				{
					WarningType = WarningType.ParishMismatch
				};

				var warnings = businessContext.DataContext.GetByExample (warningExample);

				foreach (var warning in warnings)
				{
					businessContext.DeleteEntity (warning);
				}

				businessContext.SaveChanges
				(
					LockingPolicy.ReleaseLock, EntitySaveMode.IgnoreValidationErrors
				);
			}
		}


	}
}
