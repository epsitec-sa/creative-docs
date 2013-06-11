using Epsitec.Aider.Data.Common;
using Epsitec.Aider.Entities;
using Epsitec.Aider.Enumerations;

using Epsitec.Cresus.Core;
using Epsitec.Cresus.Core.Business;

using Epsitec.Cresus.DataLayer.Context;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;


namespace Epsitec.Aider.Data.Job
{


	public static class ParishAssignationFixer
	{


		public static void FixParishAssignations
		(
			ParishAddressRepository parishRepository,
			CoreData coreData
		)
		{
			var personKeys = ParishAssignationFixer.GetPersonsToFix (parishRepository, coreData);
			Debug.WriteLine ("[" + DateTime.Now + "] " + personKeys.Count + " PERSONS TO FIX");
			ParishAssignationFixer.FixPersons (coreData, parishRepository, personKeys);
			Debug.WriteLine ("[" + DateTime.Now + "] PERSONS FIXED");

			var legalPersonKeys = ParishAssignationFixer.GetLegalPersonsToFix (parishRepository, coreData);
			Debug.WriteLine ("[" + DateTime.Now + "] " + legalPersonKeys.Count + " LEGAL PERSONS TO FIX");
			ParishAssignationFixer.FixLegalPersons (coreData, parishRepository, legalPersonKeys);
			Debug.WriteLine ("[" + DateTime.Now + "] LEGAL PERSONS FIXED");

			Debug.WriteLine ("[" + DateTime.Now + "] CLEANING WARNINGS");
			ParishAssignationFixer.RemoveParishMismatchWarnings (coreData);
			Debug.WriteLine ("[" + DateTime.Now + "] CLEANED WARNINGS");
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

			Debug.WriteLine ("[" + DateTime.Now + "] DONE PERSON BATCH");
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

			Debug.WriteLine ("[" + DateTime.Now + "] DONE LEGAL PERSON BATCH");
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
