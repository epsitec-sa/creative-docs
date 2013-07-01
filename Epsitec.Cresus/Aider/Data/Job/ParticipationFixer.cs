using Epsitec.Aider.Data.Common;
using Epsitec.Aider.Entities;

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


	internal static class ParticipationFixer
	{


		public static void FixParticipations(CoreData coreData)
		{
			Logger.LogToConsole ("START ALL BATCHES");
			
			AiderEnumerator.Execute (coreData, ParticipationFixer.FixParticipations);

			Logger.LogToConsole ("DONE ALL BATCHES");
		}


		private static void FixParticipations
		(
			BusinessContext businessContext,
			IEnumerable<AiderGroupParticipantEntity> particpations)
		{
			Logger.LogToConsole ("START BATCH");

			foreach (var participation in particpations)
			{
				if (participation.Contact.IsNull ())
				{
					ParticipationFixer.FixParticipations (businessContext, participation);
				}
			}

			businessContext.SaveChanges (LockingPolicy.KeepLock, EntitySaveMode.IgnoreValidationErrors);

			Logger.LogToConsole ("DONE BATCH");
		}


		private static void FixParticipations
		(
			BusinessContext businessContext,
			AiderGroupParticipantEntity participation
		)
		{
			AiderContactEntity contact = null;

			var dataContext = businessContext.DataContext;
			var entityKey = dataContext.GetNormalizedEntityKey (participation).Value;

			if (participation.Person.IsNotNull ())
			{
				contact = participation.Person.GetMainContact ();
			}
			else if (participation.LegalPerson.IsNotNull ())
			{
				contact = participation.LegalPerson.GetMainContact ();
			}
			else
			{
				Logger.LogToConsole ("[WARNING] empty participation: " + entityKey);
			}

			if (contact != null)
			{
				participation.Contact = contact;
			}
			else
			{
				Logger.LogToConsole ("[WARNING] no contact: " + entityKey);
			}
		}


	}


}
