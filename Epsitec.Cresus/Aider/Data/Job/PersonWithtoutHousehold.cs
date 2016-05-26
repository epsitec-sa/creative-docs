using Epsitec.Aider.Data.Common;
using Epsitec.Aider.Entities;
using Epsitec.Aider.Enumerations;
using Epsitec.Common.IO;
using Epsitec.Cresus.Core;
using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Entities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;


namespace Epsitec.Aider.Data.Job
{

	internal static class PersonWithoutHousehold
	{
		public static void FlagContacts(CoreData coreData)
		{
			Logger.LogToConsole ("START ALL BATCHES");

			AiderEnumerator.Execute (coreData, PersonWithoutHousehold.FlagContacts);

			Logger.LogToConsole ("DONE ALL BATCHES");
		}

		private static void FlagContacts
		(
			BusinessContext businessContext,
			IEnumerable<AiderPersonEntity> batch)
		{
			Logger.LogToConsole ("START BATCH");
			var persons = batch.Where (
				p =>
				p.MainContact.IsNotNull () &&
				p.Households.Count == 0 &&
				p.Visibility == PersonVisibilityStatus.Default);
			foreach (var person in persons)
			{
				PersonWithoutHousehold.Flag (businessContext, person);
			}

			businessContext.SaveChanges (LockingPolicy.KeepLock, EntitySaveMode.IgnoreValidationErrors);

			Logger.LogToConsole ("DONE BATCH");
		}


		private static void Flag
		(
			BusinessContext businessContext,
			AiderPersonEntity person
		)
		{
			person.MainContact.QualityCode += "M";
		}


	}


}
