using Epsitec.Common.Types;

using Epsitec.Cresus.Core.Data;
using Epsitec.Cresus.Core.Entities;

using Epsitec.Cresus.Database;
using Epsitec.Cresus.DataLayer;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.Collections.Generic;
using System.Linq;
using System.Collections;

namespace Epsitec.Cresus.Core
{


	[TestClass]
	public class UnitTestRepository
	{


		[ClassInitialize]
		public static void Initialize(TestContext testContext)
		{
			TestSetup.Initialize ();
		}


		[TestMethod]
		public void CreateDatabase()
		{
			this.StartTest ("Create database");

			UnitTestRepository.dbInfrastructure = TestSetup.CreateDbInfrastructure ();

			Assert.IsTrue (UnitTestRepository.dbInfrastructure.IsConnectionOpen);
		}


		[TestMethod]
		public void PupulateDatabase()
		{
			this.StartTest ("Populate database");

			using (DataContext dataContext = new DataContext (UnitTestRepository.dbInfrastructure))
			{
				dataContext.CreateSchema<AbstractPersonEntity> ();
				dataContext.CreateSchema<MailContactEntity> ();
				dataContext.CreateSchema<TelecomContactEntity> ();
				dataContext.CreateSchema<UriContactEntity> ();

				UriSchemeEntity mailScheme = EntityBuilder.CreateUriScheme (dataContext, "mailto:", "email");

				UriContactEntity contactAlfred1 = EntityBuilder.CreateUriContact (dataContext, "alfred@coucou.com", mailScheme);
				UriContactEntity contactAlfred2 = EntityBuilder.CreateUriContact (dataContext, "alfred@blabla.com", mailScheme);
				UriContactEntity contactGertrude = EntityBuilder.CreateUriContact (dataContext, "gertrude@coucou.com", mailScheme);
				UriContactEntity contactNobody = EntityBuilder.CreateUriContact (dataContext, "nobody@nowhere.com", mailScheme);

				LanguageEntity french = EntityBuilder.CreateLanguage (dataContext, "Fr", "French");
				LanguageEntity german = EntityBuilder.CreateLanguage (dataContext, "Ge", "German");

				PersonGenderEntity male = EntityBuilder.CreatePersonGender (dataContext, "M", "Male");
				PersonGenderEntity female = EntityBuilder.CreatePersonGender (dataContext, "F", "Female");

				PersonTitleEntity mister = EntityBuilder.CreatePersonTitle (dataContext, "Mister", "M");
				PersonTitleEntity lady = EntityBuilder.CreatePersonTitle (dataContext, "Lady", "L");
				
				NaturalPersonEntity alfred = EntityBuilder.CreateNaturalPerson (dataContext, "Alfred", "Dupond", new Date (1950, 12, 31), french, null, male);
				alfred.Contacts.Add (contactAlfred1);
				alfred.Contacts.Add (contactAlfred2);
				contactAlfred1.NaturalPerson = alfred;
				contactAlfred2.NaturalPerson = alfred;

				NaturalPersonEntity gertrude = EntityBuilder.CreateNaturalPerson (dataContext, "Gertrude", "De-La-Motte", new Date (1965, 5, 3), null, lady, female);
				gertrude.Contacts.Add (contactGertrude);
				contactGertrude.NaturalPerson = gertrude;

				NaturalPersonEntity hans = EntityBuilder.CreateNaturalPerson (dataContext, "Hans", "Strüdel", new Date (1984, 8, 9), german, mister, null);

				dataContext.SaveChanges (); 
			}
		}

		
		[TestMethod]
		public void SaveWithoutChanges1()
		{
			this.StartTest ("Save without changes 1");

			using (DataContext dataContext = new DataContext (UnitTestRepository.dbInfrastructure))
			{
				dataContext.SaveChanges ();
			}
		}


		[TestMethod]
		public void SaveWithoutChanges2()
		{
			this.StartTest ("Save without changes 2");
			
			using (DataContext dataContext = new DataContext (UnitTestRepository.dbInfrastructure))
			{
				Repository repository = new Repository (UnitTestRepository.dbInfrastructure, dataContext);

				UriContactEntity[] contacts = repository.GetEntitiesByExample<UriContactEntity> (new UriContactEntity ()).ToArray ();

				Assert.IsTrue (contacts.Length == 4);

				this.CheckUriContacts (contacts);

				dataContext.SaveChanges ();
			}
		}


		[TestMethod]
		public void GetObjects1()
		{
			this.StartTest ("Get objects 1");

			using (DataContext dataContext = new DataContext (UnitTestRepository.dbInfrastructure))
			{
				Repository repository = new Repository (UnitTestRepository.dbInfrastructure, dataContext);

				NaturalPersonEntity[] persons = repository.GetEntitiesByExample<NaturalPersonEntity> (new NaturalPersonEntity ()).ToArray ();

				Assert.IsTrue (persons.Length == 3);

				this.CheckAlfred (persons);
				this.CheckGertrude (persons);
				this.CheckHans (persons);

			}
		}


		[TestMethod]
		public void GetObjects2()
		{
			this.StartTest ("Get objects 2");

			using (DataContext dataContext = new DataContext (UnitTestRepository.dbInfrastructure))
			{
				Repository repository = new Repository (UnitTestRepository.dbInfrastructure, dataContext);

				NaturalPersonEntity[] persons = repository.GetEntitiesByExample<AbstractPersonEntity> (new AbstractPersonEntity ()).Cast<NaturalPersonEntity> ().ToArray ();

				Assert.IsTrue (persons.Length == 3);

				this.CheckAlfred (persons);
				this.CheckGertrude (persons);
				this.CheckHans (persons);
			}
		}


		[TestMethod]
		public void GetObjects3()
		{
			this.StartTest ("Get objects 3");

			using (DataContext dataContext = new DataContext (UnitTestRepository.dbInfrastructure))
			{
				Repository repository = new Repository (UnitTestRepository.dbInfrastructure, dataContext);

				NaturalPersonEntity example = this.GetCorrectExample1 ();

				NaturalPersonEntity[] persons = repository.GetEntitiesByExample<NaturalPersonEntity> (example).ToArray ();

				Assert.IsTrue (persons.Count () == 1);

				this.CheckAlfred (persons);
			}
		}


		[TestMethod]
		public void GetObjects4()
		{
			this.StartTest ("Get objects 4");

			using (DataContext dataContext = new DataContext (UnitTestRepository.dbInfrastructure))
			{
				Repository repository = new Repository (UnitTestRepository.dbInfrastructure, dataContext);

				NaturalPersonEntity example = this.GetCorrectExample2 ();

				NaturalPersonEntity[] persons = repository.GetEntitiesByExample<NaturalPersonEntity> (example).ToArray ();

				Assert.IsTrue (persons.Count () == 1);

				this.CheckAlfred (persons);
			}
		}


		[TestMethod]
		public void GetObjects5()
		{
			this.StartTest ("Get objects 5");

			using (DataContext dataContext = new DataContext (UnitTestRepository.dbInfrastructure))
			{
				Repository repository = new Repository (UnitTestRepository.dbInfrastructure, dataContext);

				NaturalPersonEntity example = this.GetCorrectExample3 ();

				NaturalPersonEntity[] persons = repository.GetEntitiesByExample<NaturalPersonEntity> (example).ToArray ();

				Assert.IsTrue (persons.Length == 2);

				this.CheckAlfred (persons);
				this.CheckGertrude (persons);
			}
		}


		[TestMethod]
		public void GetObjects6()
		{
			this.StartTest ("Get objects 6");

			using (DataContext dataContext = new DataContext (UnitTestRepository.dbInfrastructure))
			{
				Repository repository = new Repository (UnitTestRepository.dbInfrastructure, dataContext);

				NaturalPersonEntity example = this.GetCorrectExample4 ();

				NaturalPersonEntity[] persons = repository.GetEntitiesByExample<NaturalPersonEntity> (example).ToArray ();

				Assert.IsTrue (persons.Count () == 1);

				this.CheckAlfred (persons);
			}
		}


		[TestMethod]
		public void GetObjects7()
		{
			this.StartTest ("Get objects 7");

			using (DataContext dataContext = new DataContext (UnitTestRepository.dbInfrastructure))
			{
				Repository repository = new Repository (UnitTestRepository.dbInfrastructure, dataContext);

				NaturalPersonEntity example = this.GetIncorrectExample1 ();

				NaturalPersonEntity[] persons = repository.GetEntitiesByExample<NaturalPersonEntity> (example).ToArray ();

				Assert.IsTrue (persons.Count () == 0);
			}
		}


		[TestMethod]
		public void GetObjects8()
		{
			this.StartTest ("Get objects 8");

			using (DataContext dataContext = new DataContext (UnitTestRepository.dbInfrastructure))
			{
				Repository repository = new Repository (UnitTestRepository.dbInfrastructure, dataContext);

				NaturalPersonEntity example = this.GetIncorrectExample2 ();

				NaturalPersonEntity[] persons = repository.GetEntitiesByExample<NaturalPersonEntity> (example).ToArray ();

				Assert.IsTrue (persons.Count () == 0);
			}
		}


		[TestMethod]
		public void GetObjects9()
		{
			this.StartTest ("Get objects 9");

			using (DataContext dataContext = new DataContext (UnitTestRepository.dbInfrastructure))
			{
				Repository repository = new Repository (UnitTestRepository.dbInfrastructure, dataContext);

				NaturalPersonEntity example = this.GetIncorrectExample3 ();

				NaturalPersonEntity[] persons = repository.GetEntitiesByExample<NaturalPersonEntity> (example).ToArray ();

				Assert.IsTrue (persons.Count () == 0);
			}
		}


		[TestMethod]
		public void GetObjects10()
		{
			this.StartTest ("Get objects 10");

			using (DataContext dataContext = new DataContext (UnitTestRepository.dbInfrastructure))
			{
				Repository repository = new Repository (UnitTestRepository.dbInfrastructure, dataContext);

				NaturalPersonEntity example = this.GetIncorrectExample4 ();

				NaturalPersonEntity[] persons = repository.GetEntitiesByExample<NaturalPersonEntity> (example).ToArray ();

				Assert.IsTrue (persons.Count () == 0);
			}
		}


		private void CheckAlfred(IEnumerable<NaturalPersonEntity> persons)
		{
			bool alfred = persons.Count (person =>

				person.Firstname == "Alfred"
		        && person.Lastname == "Dupond"
		        && person.BirthDate == new Date (1950, 12, 31)

		        && person.Title == null

		        && person.Gender.Name == "Male"
		        && person.Gender.Code == "M"

		        && person.PreferredLanguage.Name == "French"
		        && person.PreferredLanguage.Code == "Fr"

				&& person.Contacts.Count == 2

		        && (person.Contacts[0] as UriContactEntity).Uri == "alfred@coucou.com"
		        && (person.Contacts[0] as UriContactEntity).UriScheme.Code == "mailto:"
		        && (person.Contacts[0] as UriContactEntity).UriScheme.Name == "email"

		        && (person.Contacts[1] as UriContactEntity).Uri == "alfred@blabla.com"
		        && (person.Contacts[1] as UriContactEntity).UriScheme.Code == "mailto:"
		        && (person.Contacts[1] as UriContactEntity).UriScheme.Name == "email"

			) == 1;

			Assert.IsTrue (alfred);
		}


		private void CheckGertrude(IEnumerable<NaturalPersonEntity> persons)
		{
			bool gertrude = persons.Count (person =>

				person.Firstname == "Gertrude"
		        && person.Lastname == "De-La-Motte"
		        && person.BirthDate == new Date (1965, 5, 3)

		        && person.Title.Name == "Lady"
				&& person.Title.ShortName == "L"

		        && person.Gender.Name == "Female"
		        && person.Gender.Code == "F"

		        && person.PreferredLanguage == null

				&& person.Contacts.Count == 1

		        && (person.Contacts[0] as UriContactEntity).Uri == "gertrude@coucou.com"
		        && (person.Contacts[0] as UriContactEntity).UriScheme.Code == "mailto:"
		        && (person.Contacts[0] as UriContactEntity).UriScheme.Name == "email"

			) == 1;

			Assert.IsTrue (gertrude);
		}


		private void CheckHans(IEnumerable<NaturalPersonEntity> persons)
		{
			bool alfred = persons.Count (person =>

				person.Firstname == "Hans"
		        && person.Lastname == "Strüdel"
		        && person.BirthDate == new Date (1984, 8, 9)

				&& person.Gender == null

		        && person.Title.Name == "Mister"
				&& person.Title.ShortName == "M"

		        && person.PreferredLanguage.Name == "German"
		        && person.PreferredLanguage.Code == "Ge"

				&& person.Contacts.Count == 0

			) == 1;

			Assert.IsTrue (alfred);
		}


		private void CheckUriContacts(IEnumerable<UriContactEntity> contacts)
		{
			string[] urls = {
				"alfred@coucou.com",
				"alfred@blabla.com",
				"gertrude@coucou.com",
				"nobody@nowhere.com"
			};

			Assert.IsTrue (contacts.Count () == 4);

			foreach (string url in urls)
			{
				bool contains = contacts.Count (c =>
					
					c.Uri == url
					&& c.UriScheme.Code == "mailto:"
					&& c.UriScheme.Name == "email"
					&& c.Comments.Count == 0
					&& c.Roles.Count == 0
					&& c.LegalPerson == null
	
				) == 1;

				Assert.IsTrue (contains);
			}

			Assert.IsTrue (contacts.First (c => c.Uri == "nobody@nowhere.com").NaturalPerson == null);
			Assert.IsTrue (contacts.First (c => c.Uri == "gertrude@coucou.com").NaturalPerson.Firstname == "Gertrude");
			Assert.IsTrue (contacts.First (c => c.Uri == "alfred@blabla.com").NaturalPerson.Firstname == "Alfred");
			Assert.IsTrue (contacts.First (c => c.Uri == "alfred@coucou.com").NaturalPerson.Firstname == "Alfred");
		}


		private NaturalPersonEntity GetCorrectExample1()
		{
			NaturalPersonEntity example = new NaturalPersonEntity ()
			{
				Firstname = "Alfred",
				Lastname = "Dupond",
			};

			return example;
		}


		private NaturalPersonEntity GetCorrectExample2()
		{
			NaturalPersonEntity example = new NaturalPersonEntity ()
			{
				PreferredLanguage = new LanguageEntity ()
				{
					Code = "Fr",
					Name = "French",
				},
			};

			return example;
		}


		private NaturalPersonEntity GetCorrectExample3()
		{
			NaturalPersonEntity example = new NaturalPersonEntity ();

			example.Contacts.Add (new UriContactEntity ()
			{
				UriScheme = new UriSchemeEntity ()
				{
					Name = "email",
					Code = "mailto:",
				},
			});

			return example;
		}


		private NaturalPersonEntity GetCorrectExample4()
		{
			NaturalPersonEntity example = new NaturalPersonEntity ();

			example.Contacts.Add (new UriContactEntity ()
			{
				Uri = "alfred@blabla.com",
			});

			return example;
		}


		private NaturalPersonEntity GetIncorrectExample1()
		{
			NaturalPersonEntity example = new NaturalPersonEntity ()
			{
				Firstname = "WRONG NAME",
			};

			return example;
		}


		private NaturalPersonEntity GetIncorrectExample2()
		{
			NaturalPersonEntity example = new NaturalPersonEntity ()
			{
				PreferredLanguage = new LanguageEntity ()
				{
					Name = "WRONG NAME",
				},
			};

			return example;
		}


		private NaturalPersonEntity GetIncorrectExample3()
		{
			NaturalPersonEntity example = new NaturalPersonEntity ();

			example.Contacts.Add (new UriContactEntity ()
			{
				UriScheme = new UriSchemeEntity ()
				{
					Name = "WRONG NAME",
				},
			});

			return example;
		}


		private NaturalPersonEntity GetIncorrectExample4()
		{
			NaturalPersonEntity example = new NaturalPersonEntity ()
			{
			};

			example.Contacts.Add (new UriContactEntity ()
			{
				Uri = "WRONG URI"
			});

			return example;
		}


		private void StartTest(string name)
		{
			System.Diagnostics.Debug.WriteLine ("===========================================================================================================================================================");
			System.Diagnostics.Debug.WriteLine ("Starting test: " + name);
			System.Diagnostics.Debug.WriteLine ("===========================================================================================================================================================");
		}


		private static DbInfrastructure dbInfrastructure;


	}


}
