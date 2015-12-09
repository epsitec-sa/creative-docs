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


	/// <summary>
	/// Update CalculatedAge on persons
	/// </summary>
	internal static class AgeCalculator
	{
		public static void Start (CoreData coreData)
		{
			Logger.LogToConsole ("START ALL BATCHES");

			AiderEnumerator.Execute (coreData, AgeCalculator.CalculateAge);

			Logger.LogToConsole ("DONE ALL BATCHES");
		}


		private static void CalculateAge
		(
			BusinessContext businessContext,
			IEnumerable<AiderPersonEntity> persons)
		{
			Logger.LogToConsole ("START BATCH");

			foreach (var person in persons)
			{
				person.CalculatedAge = person.Age;
			}

			businessContext.SaveChanges (LockingPolicy.KeepLock, EntitySaveMode.IgnoreValidationErrors);

			Logger.LogToConsole ("DONE BATCH");
		}


	}


}
