using Epsitec.Common.Types;

using Epsitec.Cresus.Core.Data;
using Epsitec.Cresus.Core.Entities;

using Epsitec.Cresus.Database;
using Epsitec.Cresus.DataLayer;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core
{


	[TestClass]
	public class UnitTestAbstractRepository
	{


		[ClassInitialize]
		public static void Initialize(TestContext testContext)
		{
			TestSetup.Initialize ();
		}


		[TestMethod]
		public void Check01CreateDatabase()
		{
			UnitTestAbstractRepository.dbInfrastructure = TestSetup.CreateDbInfrastructure ();
			Assert.IsTrue (UnitTestAbstractRepository.dbInfrastructure.IsConnectionOpen);
		}


		[TestMethod]
		public void Check02PupulateDatabase()
		{
			using (DataContext dataContext = new DataContext (UnitTestAbstractRepository.dbInfrastructure))
			{
				dataContext.CreateSchema<AbstractPersonEntity> ();
				dataContext.CreateSchema<MailContactEntity> ();
				dataContext.CreateSchema<TelecomContactEntity> ();
				dataContext.CreateSchema<UriContactEntity> ();

				UriSchemeEntity mailScheme = EntityBuilder.CreateUriScheme (dataContext, "mailto:", "email");

				UriContactEntity contactAlfred1 = EntityBuilder.CreateUriContact (dataContext, "alfred@coucou.com", mailScheme);
				UriContactEntity contactAlfred2 = EntityBuilder.CreateUriContact (dataContext, "alfred@blabla.com", mailScheme);
				UriContactEntity contactGertrude = EntityBuilder.CreateUriContact (dataContext, "gertrude@coucou.com", mailScheme);
				UriContactEntity contactHans = EntityBuilder.CreateUriContact (dataContext, "hans@coucou.com", mailScheme);

				LanguageEntity french = EntityBuilder.CreateLanguage (dataContext, "Fr", "French");
				LanguageEntity german = EntityBuilder.CreateLanguage (dataContext, "Ge", "German");

				PersonGenderEntity male = EntityBuilder.CreatePersonGender (dataContext, "M", "Male");
				PersonGenderEntity female = EntityBuilder.CreatePersonGender (dataContext, "F", "Female");

				PersonTitleEntity mister = EntityBuilder.CreatePersonTitle (dataContext, "Mister", "M");
				PersonTitleEntity lady = EntityBuilder.CreatePersonTitle (dataContext, "Lady", "L");
				
				//LegalPersonTypeEntity sa = EntityBuilder.CreateLegalPersonType (dataContext, "Société anonyme", "SA");
				//LegalPersonTypeEntity sarl = EntityBuilder.CreateLegalPersonType (dataContext, "Société à responsabilité limitée", "SARL");

				NaturalPersonEntity alfred = EntityBuilder.CreateNaturalPerson (dataContext, "Alfred", "Dupond", new Date (1950, 12, 31), french, mister, male);
				alfred.Contacts.Add (contactAlfred1);
				alfred.Contacts.Add (contactAlfred2);
				contactAlfred1.NaturalPerson = alfred;
				contactAlfred2.NaturalPerson = alfred;

				NaturalPersonEntity gertrude = EntityBuilder.CreateNaturalPerson (dataContext, "Gertrude", "De-La-Motte", new Date (1965, 5, 3), null, lady, female
					);
				gertrude.Contacts.Add (contactGertrude);
				contactGertrude.NaturalPerson = gertrude;

				NaturalPersonEntity hans = EntityBuilder.CreateNaturalPerson (dataContext, "Hans", "Strüdel", new Date (1984, 8, 9), german, null, null);
				hans.Contacts.Add (contactHans);
				contactHans.NaturalPerson = hans;

				//LegalPersonEntity papetVaudois = EntityBuilder.CreateLegalPerson (dataContext, "Papet Vaudois SA", "PV", "", sa, french);
				//LegalPersonEntity bratwurst = EntityBuilder.CreateLegalPerson (dataContext, "Bratwurst SARL", "B", "", sarl, german);

				dataContext.SaveChanges (); 
			}
		}


		[TestMethod]
		public void Check03GetObjects1()
		{
			using (DataContext dataContext = new DataContext (UnitTestAbstractRepository.dbInfrastructure))
			{
				Repository repository = new Repository (UnitTestAbstractRepository.dbInfrastructure, dataContext);

				NaturalPersonEntity example = new NaturalPersonEntity ()
				{
					PreferredLanguage = new LanguageEntity ()
					{
						Code = "Fr",
						Name = "French",
					},
					Firstname = "Alfred",
					Gender = new PersonGenderEntity ()
					{
						Code = "M",
						Name = "Male",
					},
				};

				example.Contacts.Add (new UriContactEntity ()
				{
					Uri = "alfred@coucou.com",
					UriScheme = new UriSchemeEntity ()
					{
						Name = "email",
						Code = "mailto:",
					},
				});

				example.Contacts.Add (new UriContactEntity ()
				{
					Uri = "alfred@blabla.com",
				});


				//NaturalPersonEntity[] persons = repository.GetEntitiesByExample<NaturalPersonEntity> (example).ToArray ();
				//NaturalPersonEntity[] persons = repository.GetEntitiesByExample<AbstractPersonEntity> (new AbstractPersonEntity ()).Cast<NaturalPersonEntity> ().ToArray ();
				NaturalPersonEntity[] persons = repository.GetEntitiesByExample<NaturalPersonEntity> (new NaturalPersonEntity ()).ToArray ();

				foreach (NaturalPersonEntity person in persons)
				{
					System.Diagnostics.Debug.WriteLine ("=====================================================");
					System.Diagnostics.Debug.WriteLine ("Firstname: " + person.Firstname);
					System.Diagnostics.Debug.WriteLine ("Lastname: " + person.Lastname);
					System.Diagnostics.Debug.WriteLine ("Birthday: " + person.BirthDate);
					System.Diagnostics.Debug.WriteLine ("Gender: " + ((person.Gender == null) ? "null" : person.Gender.Name));
					System.Diagnostics.Debug.WriteLine ("Title: " + ((person.Title == null) ? "null" : person.Title.Name));
					System.Diagnostics.Debug.WriteLine ("Language: " + ((person.PreferredLanguage == null) ? "null" : person.PreferredLanguage.Name));

					foreach (AbstractContactEntity contact in person.Contacts)
					{
						System.Diagnostics.Debug.WriteLine ("Contact: " + (contact as UriContactEntity).Uri);
					}

				}
				System.Diagnostics.Debug.WriteLine ("=====================================================");

				//dataContext.DeleteEntity (persons[0]);

				dataContext.SaveChanges ();
			}
		}


		private static DbInfrastructure dbInfrastructure;


	}


}
