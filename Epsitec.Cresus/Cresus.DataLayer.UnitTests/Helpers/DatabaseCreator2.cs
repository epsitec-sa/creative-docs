﻿using Epsitec.Common.Types;

using Epsitec.Cresus.Database;

using Epsitec.Cresus.DataLayer.Context;
using Epsitec.Cresus.DataLayer.Infrastructure;
using Epsitec.Cresus.DataLayer.UnitTests.Entities;


namespace Epsitec.Cresus.DataLayer.UnitTests.Helpers
{


	internal static class DatabaseCreator2
	{
		

		public static void ResetEmptyTestDatabase()
		{
			DbInfrastructureHelper.ResetTestDatabase ();

			using (DbInfrastructure dbInfrastructure = DbInfrastructureHelper.ConnectToTestDatabase ())
			using (DataInfrastructure dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase (dbInfrastructure))
			{
				DatabaseCreator2.RegisterSchema (dataInfrastructure);
			}
		}


		public static void ResetPopulatedTestDatabase()
		{
			DbInfrastructureHelper.ResetTestDatabase ();

			using (DbInfrastructure dbInfrastructure = DbInfrastructureHelper.ConnectToTestDatabase ())
			using (DataInfrastructure dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase (dbInfrastructure))
			using (DataContext dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
			{
				DatabaseCreator2.RegisterSchema (dataInfrastructure);
				DatabaseCreator2.PopulateDatabase (dataContext);
			}
		}


		public static void RegisterSchema(DataInfrastructure dataInfrastructure)
		{
			dataInfrastructure.CreateSchema<NaturalPersonEntity> ();
			dataInfrastructure.CreateSchema<MailContactEntity> ();
			dataInfrastructure.CreateSchema<TelecomContactEntity> ();
			dataInfrastructure.CreateSchema<UriContactEntity> ();
		}
		

		public static void PopulateDatabase(DataContext dataContext)
		{
			UriSchemeEntity mailScheme = DataContextHelper.CreateUriScheme (dataContext, "mailto:", "email");

			UriContactEntity contactAlfred1 = DataContextHelper.CreateUriContact (dataContext, "alfred@coucou.com", mailScheme);
			UriContactEntity contactAlfred2 = DataContextHelper.CreateUriContact (dataContext, "alfred@blabla.com", mailScheme);
			UriContactEntity contactGertrude = DataContextHelper.CreateUriContact (dataContext, "gertrude@coucou.com", mailScheme);
			UriContactEntity contactNobody = DataContextHelper.CreateUriContact (dataContext, "nobody@nowhere.com", mailScheme);

			LanguageEntity french = DataContextHelper.CreateLanguage (dataContext, "Fr", "French");
			LanguageEntity german = DataContextHelper.CreateLanguage (dataContext, "Ge", "German");

			PersonGenderEntity male = DataContextHelper.CreatePersonGender (dataContext, "M", "Male");
			PersonGenderEntity female = DataContextHelper.CreatePersonGender (dataContext, "F", "Female");

			PersonTitleEntity mister = DataContextHelper.CreatePersonTitle (dataContext, "Mister", "M");
			PersonTitleEntity lady = DataContextHelper.CreatePersonTitle (dataContext, "Lady", "L");

			NaturalPersonEntity alfred = DataContextHelper.CreateNaturalPerson (dataContext, "Alfred", "Dupond", new Date (1950, 12, 31), french, null, male);
			alfred.Contacts.Add (contactAlfred1);
			alfred.Contacts.Add (contactAlfred2);
			contactAlfred1.NaturalPerson = alfred;
			contactAlfred2.NaturalPerson = alfred;

			NaturalPersonEntity gertrude = DataContextHelper.CreateNaturalPerson (dataContext, "Gertrude", "De-La-Motte", new Date (1965, 5, 3), null, lady, female);
			gertrude.Contacts.Add (contactGertrude);
			contactGertrude.NaturalPerson = gertrude;

			NaturalPersonEntity hans = DataContextHelper.CreateNaturalPerson (dataContext, "Hans", "Strüdel", new Date (1984, 8, 9), german, mister, null);

			dataContext.SaveChanges ();
		}

		public static bool CheckAlfred(NaturalPersonEntity person)
		{
			return person.Firstname == "Alfred"
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
		        && (person.Contacts[1] as UriContactEntity).UriScheme.Name == "email";
		}


		public static bool CheckGertrude(NaturalPersonEntity person)
		{
			return person.Firstname == "Gertrude"
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
		        && (person.Contacts[0] as UriContactEntity).UriScheme.Name == "email";
		}


		public static bool CheckHans(NaturalPersonEntity person)
		{
			return person.Firstname == "Hans"
		        && person.Lastname == "Strüdel"
		        && person.BirthDate == new Date (1984, 8, 9)

				&& person.Gender == null

		        && person.Title.Name == "Mister"
				&& person.Title.ShortName == "M"

		        && person.PreferredLanguage.Name == "German"
		        && person.PreferredLanguage.Code == "Ge"

				&& person.Contacts.Count == 0;
		}


		public static bool CheckUriContact(UriContactEntity contact, string uri, string firstname)
		{
			return contact.Uri == uri
				&& contact.UriScheme.Code == "mailto:"
				&& contact.UriScheme.Name == "email"
				&& contact.Comments.Count == 0
				&& contact.Roles.Count == 0
				&& contact.LegalPerson == null
				&& ((contact.NaturalPerson == null && firstname == null) || (contact.NaturalPerson != null && contact.NaturalPerson.Firstname == firstname));
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


		public static NaturalPersonEntity GetCorrectExample5()
		{
			NaturalPersonEntity example = new NaturalPersonEntity ();

			example.Contacts.Add (new UriContactEntity ()
			{
				Uri = "alfred@blabla.com",
			});

			example.Contacts.Add (new UriContactEntity ()
			{
				Uri = "alfred@coucou.com",
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
			NaturalPersonEntity example = new NaturalPersonEntity ();

			example.Contacts.Add (new UriContactEntity ()
			{
				Uri = "WRONG URI"
			});

			return example;
		}


	}


}
