//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Samuel LOUP, Maintainer: Samuel LOUP

using Epsitec.Aider.Entities;
using System.Linq;
using Epsitec.Cresus.Core;
using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.DataLayer.Expressions;
using Epsitec.Cresus.DataLayer.Loader;



namespace Epsitec.Aider.Data.Job
{
	internal static class PersonDeathFixer
	{
		public static void FixAll(CoreData coreData)
		{
			
			using (var businessContext = new BusinessContext (coreData, false))
			{
				var personExample = new AiderPersonEntity ()
				{
					eCH_Person = new eCH_PersonEntity ()
				};

				var request = new Request ()
				{
					RootEntity = personExample
				};

				request.AddCondition
				(
					businessContext.DataContext,
					personExample.eCH_Person,
					p => p.PersonDateOfDeath != null
				);

				var personsToFix = businessContext.DataContext.GetByRequest<AiderPersonEntity> (request);

				foreach (var personToFix in personsToFix)
				{
					AiderPersonEntity.KillPerson (businessContext, personToFix, personToFix.eCH_Person.PersonDateOfDeath.Value);
				}

				businessContext.SaveChanges
				(
					LockingPolicy.ReleaseLock, EntitySaveMode.IgnoreValidationErrors
				);
			}
		}
	}
}
