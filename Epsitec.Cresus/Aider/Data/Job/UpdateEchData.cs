using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Epsitec.Aider.Data.ECh;
using Epsitec.Aider.Entities;
using Epsitec.Cresus.Core;
using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.DataLayer.Loader;

namespace Epsitec.Aider.Data.Job
{
	internal static class UpdateEchData
	{
		public static void Update(string OldEchFile, string NewEchFile, CoreData coreData)
		{
			Console.WriteLine ("ECH DATA UPDATER : START UPDATE JOB");
			if (System.IO.File.Exists (OldEchFile) && System.IO.File.Exists (NewEchFile))
			{
				EChDataComparer comparer = new EChDataComparer (OldEchFile, NewEchFile);
				var personsToChange = comparer.GetPersonToChange ();
				Console.WriteLine (personsToChange.Count + " ECH PERSON TO CHANGE");
				using (var businessContext = new BusinessContext (coreData, false))
				{
					foreach(var toChange in personsToChange)
					{
						var personEntityToUpdate = GetEchPersonEntity (businessContext, toChange.Item1);
						
						var changedEChPersonEntity = EChDataImporter.ConvertEChPersonToEntity (toChange.Item1);

						if (!toChange.Item1.DateOfBirth.Equals (toChange.Item2.DateOfBirth))
						{
							personEntityToUpdate.PersonDateOfBirth = changedEChPersonEntity.PersonDateOfBirth;
						}
							
						if (!toChange.Item1.FirstNames.Equals (toChange.Item2.FirstNames))
						{
							personEntityToUpdate.PersonFirstNames = changedEChPersonEntity.PersonFirstNames;
						}

						if (!toChange.Item1.MaritalStatus.Equals (toChange.Item2.MaritalStatus))
						{
							personEntityToUpdate.AdultMaritalStatus = changedEChPersonEntity.AdultMaritalStatus;
						}

						if (!toChange.Item1.NationalCountryCode.Equals (toChange.Item2.NationalCountryCode))
						{
							personEntityToUpdate.NationalityCountryCode = changedEChPersonEntity.NationalityCountryCode;
						}

						if (!toChange.Item1.NationalityStatus.Equals (toChange.Item2.NationalityStatus))
						{
							personEntityToUpdate.NationalityStatus = changedEChPersonEntity.NationalityStatus;
						}

						if (!toChange.Item1.OfficialName.Equals (toChange.Item2.OfficialName))
						{
							personEntityToUpdate.PersonOfficialName = changedEChPersonEntity.PersonOfficialName;
						}

						if (!toChange.Item1.OriginPlaces.Equals (toChange.Item2.OriginPlaces))
						{
							personEntityToUpdate.Origins = changedEChPersonEntity.Origins;
						}

						if (!toChange.Item1.Sex.Equals (toChange.Item2.Sex))
						{
							personEntityToUpdate.PersonSex = changedEChPersonEntity.PersonSex;
						}
					}

					businessContext.SaveChanges(LockingPolicy.ReleaseLock, EntitySaveMode.IgnoreValidationErrors);

				}

				
				Console.WriteLine ("ECH DATA UPDATER : JOB DONE!");
			}
			else
			{
				Console.WriteLine ("ECH DATA UPDATER : FAIL... VERIFY YOUR ECH FILES PARAMETERS");
			}
		}



		private static eCH_PersonEntity GetEchPersonEntity(BusinessContext businessContext, EChPerson person)
		{

			var personExample = new eCH_PersonEntity ()
			{
				PersonId = person.Id
			};

			var request = new Request ()
			{
				RootEntity = personExample
			};


			return businessContext.DataContext.GetByRequest<eCH_PersonEntity> (request).First ();
		}
	}
}
