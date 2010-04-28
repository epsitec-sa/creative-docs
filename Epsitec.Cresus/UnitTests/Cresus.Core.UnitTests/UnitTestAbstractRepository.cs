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

				UriSchemeEntity mailScheme = this.CreateUriScheme(dataContext, "mailto:", "email");
				
				UriContactEntity contactAlfred = this.CreateUriContact(dataContext, "alfred@coucou.com", mailScheme);
				UriContactEntity contactGertrude = this.CreateUriContact (dataContext, "gertrude@coucou.com", mailScheme);
				UriContactEntity contactHans = this.CreateUriContact (dataContext, "hans@coucou.com", mailScheme);

				LanguageEntity french = this.CreateLanguage (dataContext, "Fr", "French");
				LanguageEntity german = this.CreateLanguage (dataContext, "Ge", "German");

				PersonGenderEntity male = this.CreateGender (dataContext, "M", "Male");
				PersonGenderEntity female = this.CreateGender (dataContext, "F", "Female");

				PersonTitleEntity mister = this.CreateTitle (dataContext, "Mister", "M");
				PersonTitleEntity lady = this.CreateTitle (dataContext, "Lady", "L");

				LegalPersonTypeEntity sa = this.CreateLegalPersonType (dataContext, "Société anonyme", "SA");
				LegalPersonTypeEntity sarl = this.CreateLegalPersonType (dataContext, "Société à responsabilité limitée", "SARL");

				NaturalPersonEntity alfred = this.CreateNaturalPerson (dataContext, "Alfred", "Dupond", new Date (1950, 12, 31), french, mister, male, contactAlfred);
				NaturalPersonEntity gertrude = this.CreateNaturalPerson (dataContext, "Gertrude", "De-La-Motte", new Date (1965, 5, 3), french, lady, female, contactGertrude);
				NaturalPersonEntity hans = this.CreateNaturalPerson (dataContext, "Hans", "Strüdel", new Date (1984, 8, 9), german, mister, male, contactHans);

				LegalPersonEntity papetVaudois = this.CreateLegalPerson (dataContext, "Papet Vaudois SA", sa, french);
				LegalPersonEntity bratwurst = this.CreateLegalPerson (dataContext, "Bratwurst SARL", sarl, german);

				dataContext.SaveChanges ();
			}
		}

		[TestMethod]
		public void Check02GetObjects1()
		{
			using (DataContext dataContext = new DataContext (UnitTestAbstractRepository.dbInfrastructure))
			{
				GenericRepository<NaturalPersonEntity> naturalPersonRepository = new GenericRepository<NaturalPersonEntity> (UnitTestAbstractRepository.dbInfrastructure, dataContext);
				GenericRepository<AbstractPersonEntity> abstractPersonRepository = new GenericRepository<AbstractPersonEntity> (UnitTestAbstractRepository.dbInfrastructure, dataContext);

				NaturalPersonEntity alfredExample = new NaturalPersonEntity ()
				{
				    Firstname = "Alfred",
				};

				AbstractPersonEntity personExample = new AbstractPersonEntity();

				//AbstractPersonEntity person = abstractPersonRepository.GetEntityByExample (personExample);
				NaturalPersonEntity alfred = naturalPersonRepository.GetEntityByExample (alfredExample);
			}
		}

		private UriSchemeEntity CreateUriScheme(DataContext dataContext, string code, string name)
		{
			UriSchemeEntity uriScheme = dataContext.CreateEmptyEntity<UriSchemeEntity> ();

			uriScheme.Code = code;
			uriScheme.Name = name;

			return uriScheme;
		}

		private UriContactEntity CreateUriContact(DataContext dataContext, string uri, UriSchemeEntity uriScheme)
		{
			UriContactEntity contact = dataContext.CreateEmptyEntity<UriContactEntity> ();

			contact.Uri = uri;
			contact.UriScheme = uriScheme;

			return contact;
		}


		private LanguageEntity CreateLanguage(DataContext dataContext, string code, string name)
		{
			LanguageEntity language = dataContext.CreateEmptyEntity<LanguageEntity> ();

			language.Code = code;
			language.Name = name;

			return language;
		}

		private PersonGenderEntity CreateGender(DataContext dataContext, string code, string name)
		{
			PersonGenderEntity gender = dataContext.CreateEmptyEntity<PersonGenderEntity> ();

			gender.Code = code;
			gender.Name = name;

			return gender;
		}

		private PersonTitleEntity CreateTitle(DataContext dataContext, string name, string shortName)
		{
			PersonTitleEntity title = dataContext.CreateEmptyEntity<PersonTitleEntity> ();

			title.ShortName = shortName;
			title.Name = name;

			return title;
		}

		private LegalPersonTypeEntity CreateLegalPersonType(DataContext dataContext, string name, string shortName)
		{
			LegalPersonTypeEntity type = dataContext.CreateEmptyEntity<LegalPersonTypeEntity> ();

			type.ShortName = shortName;
			type.Name = name;

			return type;
		}

		private NaturalPersonEntity CreateNaturalPerson(DataContext dataContext, string firstName, string lastName, Date birthday, LanguageEntity language, PersonTitleEntity title, PersonGenderEntity gender, AbstractContactEntity contact)
		{
			NaturalPersonEntity person = dataContext.CreateEmptyEntity<NaturalPersonEntity> ();

			person.Firstname = firstName;
			person.Lastname = lastName;
			person.PreferredLanguage = language;
			person.Gender = gender;
			person.Title = title;
			person.BirthDate = birthday;
			person.Contacts.Add (contact);

			return person;
		}

		private LegalPersonEntity CreateLegalPerson(DataContext dataContext, string name, LegalPersonTypeEntity type, LanguageEntity language)
		{
			LegalPersonEntity person = dataContext.CreateEmptyEntity<LegalPersonEntity> ();

			person.Name = name;
			person.PreferredLanguage = language;
			person.LegalPersonType = type;

			return person;
		}


		private static DbInfrastructure dbInfrastructure;

	}

}
