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
	/// This fixer cleans the contact entities. There was a bug in the contact logic that made
	/// person contact use some properties that where supposed to be used only for legal person
	/// contact. This bug has been corrected by the commit 21084. This fixer makes those properties
	/// blank when necessary.
	/// </summary>
	internal static class ContactNameFixer
	{


		public static void FixContactNames(CoreData coreData)
		{
			Logger.LogToConsole ("START ALL BATCHES");

			AiderEnumerator.Execute (coreData, ContactNameFixer.FixContactNames);

			Logger.LogToConsole ("DONE ALL BATCHES");
		}


		private static void FixContactNames
		(
			BusinessContext businessContext,
			IEnumerable<AiderContactEntity> contacts)
		{
			Logger.LogToConsole ("START BATCH");

			foreach (var contact in contacts)
			{
				ContactNameFixer.FixContactName (businessContext, contact);

				businessContext.Register (contact);
			}

			businessContext.SaveChanges (LockingPolicy.KeepLock, EntitySaveMode.IgnoreValidationErrors);

			Logger.LogToConsole ("DONE BATCH");
		}


		private static void FixContactName
		(
			BusinessContext businessContext,
			AiderContactEntity contact
		)
		{
			if (contact.ContactType == ContactType.Legal)
			{
				return;
			}

			contact.LegalPersonContactFullName = "";
			contact.LegalPersonContactMrMrs = PersonMrMrs.None;
			contact.LegalPersonContactRole = ContactRole.None;
			contact.LegalPersonContactPrincipal = false;
		}


	}


}
