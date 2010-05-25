using Epsitec.Common.Types;

using Epsitec.Cresus.DataLayer;
using Epsitec.Cresus.Core.Entities;

using System.Collections.Generic;
using System.Linq;

using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace Epsitec.Cresus.Core
{


	public class Database2 : Database
	{


		public static void PupulateDatabase()
		{
			using (DataContext dataContext = new DataContext (Database.DbInfrastructure))
			{
				dataContext.CreateSchema<AbstractPersonEntity> ();
				dataContext.CreateSchema<MailContactEntity> ();
				dataContext.CreateSchema<TelecomContactEntity> ();
				dataContext.CreateSchema<UriContactEntity> ();

				UriSchemeEntity mailScheme = Database.CreateUriScheme (dataContext, "mailto:", "email");

				UriContactEntity contactAlfred1 = Database.CreateUriContact (dataContext, "alfred@coucou.com", mailScheme);
				UriContactEntity contactAlfred2 = Database.CreateUriContact (dataContext, "alfred@blabla.com", mailScheme);
				UriContactEntity contactGertrude = Database.CreateUriContact (dataContext, "gertrude@coucou.com", mailScheme);
				UriContactEntity contactNobody = Database.CreateUriContact (dataContext, "nobody@nowhere.com", mailScheme);

				LanguageEntity french = Database.CreateLanguage (dataContext, "Fr", "French");
				LanguageEntity german = Database.CreateLanguage (dataContext, "Ge", "German");

				PersonGenderEntity male = Database.CreatePersonGender (dataContext, "M", "Male");
				PersonGenderEntity female = Database.CreatePersonGender (dataContext, "F", "Female");

				PersonTitleEntity mister = Database.CreatePersonTitle (dataContext, "Mister", "M");
				PersonTitleEntity lady = Database.CreatePersonTitle (dataContext, "Lady", "L");

				NaturalPersonEntity alfred = Database.CreateNaturalPerson (dataContext, "Alfred", "Dupond", new Date (1950, 12, 31), french, null, male);
				alfred.Contacts.Add (contactAlfred1);
				alfred.Contacts.Add (contactAlfred2);
				contactAlfred1.NaturalPerson = alfred;
				contactAlfred2.NaturalPerson = alfred;

				NaturalPersonEntity gertrude = Database.CreateNaturalPerson (dataContext, "Gertrude", "De-La-Motte", new Date (1965, 5, 3), null, lady, female);
				gertrude.Contacts.Add (contactGertrude);
				contactGertrude.NaturalPerson = gertrude;

				NaturalPersonEntity hans = Database.CreateNaturalPerson (dataContext, "Hans", "Strüdel", new Date (1984, 8, 9), german, mister, null);

				dataContext.SaveChanges ();
			}
		}


		public static void CheckAlfred(IEnumerable<NaturalPersonEntity> persons)
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


		public static void CheckGertrude(IEnumerable<NaturalPersonEntity> persons)
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


		public static void CheckHans(IEnumerable<NaturalPersonEntity> persons)
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


		public static void CheckUriContacts(IEnumerable<UriContactEntity> contacts)
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


		public static NaturalPersonEntity GetCorrectExample1()
		{
			NaturalPersonEntity example = new NaturalPersonEntity ()
			{
				Firstname = "Alfred",
				Lastname = "Dupond",
			};

			return example;
		}


		public static NaturalPersonEntity GetCorrectExample2()
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


		public static NaturalPersonEntity GetCorrectExample3()
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


		public static NaturalPersonEntity GetCorrectExample4()
		{
			NaturalPersonEntity example = new NaturalPersonEntity ();

			example.Contacts.Add (new UriContactEntity ()
			{
				Uri = "alfred@blabla.com",
			});

			return example;
		}


		public static NaturalPersonEntity GetIncorrectExample1()
		{
			NaturalPersonEntity example = new NaturalPersonEntity ()
			{
				Firstname = "WRONG NAME",
			};

			return example;
		}


		public static NaturalPersonEntity GetIncorrectExample2()
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


		public static NaturalPersonEntity GetIncorrectExample3()
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


		public static NaturalPersonEntity GetIncorrectExample4()
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


	}


}
