using Epsitec.Aider.Data.Common;

using Epsitec.Aider.Entities;

using Epsitec.Cresus.Core;
using Epsitec.Cresus.Core.Business;
using System.Linq;

namespace Epsitec.Aider.Data.Job
{

	internal static class PersonMrMrsSetter
	{

		public static void Set(CoreData coreData)
		{
			using (var businessContext = new BusinessContext (coreData, false))
			{
				var persons = businessContext.GetAllEntities<AiderPersonEntity> ();
				foreach (var person in persons)
				{
					if (person.eCH_Person != null)
					{
						PersonMrMrsSetter.SetMrMrsBasedOnSex (person, person.eCH_Person);
					}
				}

				businessContext.SaveChanges
				(
					LockingPolicy.ReleaseLock, EntitySaveMode.None
				);
			}
		}

		public static void SetMrMrsBasedOnSex(AiderPersonEntity person, eCH_PersonEntity eCH)
		{
			var sex = eCH.PersonSex;

			if (eCH.PersonDateOfBirth.HasValue)
			{
				var age = eCH.PersonDateOfBirth.Value.ComputeAge ();
				if ((age.HasValue) &&
					(age.Value < 20))
				{
					person.MrMrs = Enumerations.PersonMrMrs.None;
					return;
				}
			}
			
			if (sex == Enumerations.PersonSex.Male)
			{
				person.MrMrs = Enumerations.PersonMrMrs.Monsieur;
				return;
			}

			if (sex == Enumerations.PersonSex.Female)
			{
				person.MrMrs = Enumerations.PersonMrMrs.Madame;
				return;
			}

			if (sex == Enumerations.PersonSex.Unknown)
			{
				person.MrMrs = Enumerations.PersonMrMrs.None;
				return;
			}
		}
	}


}
