//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Marc BETTEX, Maintainer: Marc BETTEX

using Epsitec.Common.Types;

using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Database;
using Epsitec.Cresus.DataLayer;

namespace Epsitec.Cresus.Core
{


	public class Database
	{


		public static DbInfrastructure DbInfrastructure
		{
			get
			{
				return Database.dbInfrastructure;
			}
		}


		public static void CreateAndConnectToDatabase()
		{
			Database.dbInfrastructure = TestHelper.CreateDbInfrastructure ();
		}


		public static void ConnectToDatabase()
		{
			Database.dbInfrastructure = new DbInfrastructure ();
			Database.dbInfrastructure.AttachToDatabase (DbInfrastructure.CreateDatabaseAccess ("CORETEST"));
		}


		public static ContactRoleEntity[] CreateContactRoles(DataContext dataContext, int number)
		{
			System.Diagnostics.Debug.WriteLine ("Building contact roles");

			ContactRoleEntity[] roles = new ContactRoleEntity[number];

			for (int i = 0; i < number; i++)
			{
				roles[i] = Database.CreateContactRole (dataContext, "name" + i);
			}

			return roles;
		}


		public static CommentEntity[] CreateComments(DataContext dataContext, int number)
		{
			System.Diagnostics.Debug.WriteLine ("Building comments");

			CommentEntity[] comments = new CommentEntity[number];

			for (int i = 0; i < number; i++)
			{
				comments[i] = Database.CreateComment (dataContext, "text" + i);
			}

			return comments;
		}


		public static UriSchemeEntity[] CreateUriSchemes(DataContext dataContext, int number)
		{
			System.Diagnostics.Debug.WriteLine ("Building uri schemes");

			UriSchemeEntity[] uriSchemes = new UriSchemeEntity[number];

			for (int i = 0; i < number; i++)
			{
				uriSchemes[i] = Database.CreateUriScheme (dataContext, "code" + i, "name" + i);
			}

			return uriSchemes;
		}


		public static UriContactEntity[] CreateUriContacts(DataContext dataContext, UriSchemeEntity[] uriSchemes, int number)
		{
			System.Diagnostics.Debug.WriteLine ("Building uri contacts");

			UriContactEntity[] uriContacts = new UriContactEntity[number];

			for (int i = 0; i < number; i++)
			{
				uriContacts[i] = Database.CreateUriContact (dataContext, "uri" + i, uriSchemes[i % uriSchemes.Length]);
			}

			return uriContacts;
		}


		public static TelecomTypeEntity[] CreateTelecomTypes(DataContext dataContext, int number)
		{
			System.Diagnostics.Debug.WriteLine ("Building telecom types");

			TelecomTypeEntity[] telecomTypes = new TelecomTypeEntity[number];

			for (int i = 0; i < number; i++)
			{
				telecomTypes[i] = Database.CreateTelecomType (dataContext, "code" + i, "name" + i);
			}

			return telecomTypes;
		}


		public static TelecomContactEntity[] CreateTelecomContacts(DataContext dataContext, TelecomTypeEntity[] telecomTypes, int number)
		{
			System.Diagnostics.Debug.WriteLine ("Building telecom contacts");

			TelecomContactEntity[] telecomContacts = new TelecomContactEntity[number];

			for (int i = 0; i < number; i++)
			{
				telecomContacts[i] = Database.CreateTelecomContact (dataContext, "number" + i, "extension" + i, telecomTypes[i % telecomTypes.Length]);
			}

			return telecomContacts;
		}


		public static CountryEntity[] CreateCountries(DataContext dataContext, int number)
		{
			System.Diagnostics.Debug.WriteLine ("Building countries");

			CountryEntity[] countries = new CountryEntity[number];

			for (int i = 0; i < number; i++)
			{
				countries[i] = Database.CreateCountry (dataContext, "code" + i, "name" + i);
			}

			return countries;
		}


		public static RegionEntity[] CreateRegions(DataContext dataContext, CountryEntity[] countries, int number)
		{
			System.Diagnostics.Debug.WriteLine ("Building regions");


			RegionEntity[] regions = new RegionEntity[number];

			for (int i = 0; i < number; i++)
			{
				regions[i] = Database.CreateRegion (dataContext, "code" + i, "name" + i, countries[i % countries.Length]);
			}

			return regions;
		}


		public static LocationEntity[] CreateLocations(DataContext dataContext, RegionEntity[] regions, int number)
		{
			System.Diagnostics.Debug.WriteLine ("Building locations");

			LocationEntity[] locations = new LocationEntity[number];

			for (int i = 0; i < number; i++)
			{
				RegionEntity region = regions[i % regions.Length];
				locations[i] = Database.CreateLocation (dataContext, "postalCode" + i, "name" + i, region.Country, region);
			}

			return locations;
		}


		public static PostBoxEntity[] CreatePostBoxes(DataContext dataContext, int number)
		{
			System.Diagnostics.Debug.WriteLine ("Building post boxes");

			PostBoxEntity[] postboxes = new PostBoxEntity[number];

			for (int i = 0; i < number; i++)
			{
				postboxes[i] = Database.CreatePostBox (dataContext, "number" + i);
			}

			return postboxes;
		}


		public static StreetEntity[] CreateStreets(DataContext dataContext, int number)
		{
			System.Diagnostics.Debug.WriteLine ("Building streets");

			StreetEntity[] streets = new StreetEntity[number];

			for (int i = 0; i < number; i++)
			{
				streets[i] = Database.CreateStreet (dataContext, "complement" + i, "name" + i);
			}

			return streets;
		}


		public static AddressEntity[] CreateAddresses(DataContext dataContext, StreetEntity[] streets, PostBoxEntity[] postBoxes, LocationEntity[] locations, int number)
		{
			System.Diagnostics.Debug.WriteLine ("Building addresses");

			AddressEntity[] addresses = new AddressEntity[number];

			for (int i = 0; i < number; i++)
			{
				addresses[i] = Database.CreateAddresss (dataContext, "complement" + i, streets[i % streets.Length], postBoxes[i % postBoxes.Length], locations[i % locations.Length]);
			}

			return addresses;
		}


		public static MailContactEntity[] CreateMailContact(DataContext dataContext, AddressEntity[] addresses, int number)
		{
			System.Diagnostics.Debug.WriteLine ("Building mail contacts");

			MailContactEntity[] mailContact = new MailContactEntity[number];

			for (int i = 0; i < number; i++)
			{
				mailContact[i] = Database.CreateMailContact (dataContext, "complement" + i, addresses[i % addresses.Length]);
			}

			return mailContact;
		}


		public static LanguageEntity[] CreateLanguages(DataContext dataContext, int number)
		{
			System.Diagnostics.Debug.WriteLine ("Building languages");

			LanguageEntity[] languages = new LanguageEntity[number];

			for (int i = 0; i < number; i++)
			{
				languages[i] = Database.CreateLanguage (dataContext, "code" + i, "name" + i);
			}

			return languages;
		}


		public static PersonTitleEntity[] CreatePersonTitles(DataContext dataContext, int number)
		{
			System.Diagnostics.Debug.WriteLine ("Building person titles");

			PersonTitleEntity[] titles = new PersonTitleEntity[number];

			for (int i = 0; i < number; i++)
			{
				titles[i] = Database.CreatePersonTitle (dataContext, "name" + i, "shortName" + i);
			}

			return titles;
		}


		public static PersonGenderEntity[] CreatePersonGenders(DataContext dataContext, int number)
		{
			System.Diagnostics.Debug.WriteLine ("Building person genders");

			PersonGenderEntity[] genders = new PersonGenderEntity[number];

			for (int i = 0; i < number; i++)
			{
				genders[i] = Database.CreatePersonGender (dataContext, "code" + i, "name" + i);
			}

			return genders;
		}


		public static LegalPersonTypeEntity[] CreateLegalPersonTypes(DataContext dataContext, int number)
		{
			System.Diagnostics.Debug.WriteLine ("Building legal person types");

			LegalPersonTypeEntity[] legalPersonTypes = new LegalPersonTypeEntity[number];

			for (int i = 0; i < number; i++)
			{
				legalPersonTypes[i] = Database.CreateLegalPersonType (dataContext, "name" + i, "shortName" + i);
			}

			return legalPersonTypes;
		}


		public static NaturalPersonEntity[] CreateNaturalPersons(DataContext dataContext, LanguageEntity[] languages, PersonTitleEntity[] titles, PersonGenderEntity[] genders, int number)
		{
			System.Diagnostics.Debug.WriteLine ("Building natural persons");

			NaturalPersonEntity[] naturalPerson = new NaturalPersonEntity[number];

			for (int i = 0; i < number; i++)
			{
				naturalPerson[i] = Database.CreateNaturalPerson (dataContext, "firstname" + i, "lastname" + i, new Date (1950, 12, 31), languages[i % languages.Length], titles[i % titles.Length], genders[i % genders.Length]);
			}

			return naturalPerson;
		}


		public static LegalPersonEntity[] CreateLegalPersons(DataContext dataContext, LanguageEntity[] languages, LegalPersonTypeEntity[] types, int number)
		{
			System.Diagnostics.Debug.WriteLine ("Building legal persons");

			LegalPersonEntity[] legalPersons = new LegalPersonEntity[number];

			for (int i = 0; i < number; i++)
			{
				legalPersons[i] = Database.CreateLegalPerson (dataContext, "name" + i, "shortName" + i, "complement" + i, types[i % types.Length], languages[i % languages.Length]);
			}

			return legalPersons;
		}


		public static void AssignRoles(AbstractContactEntity[] contacts, ContactRoleEntity[] roles)
		{
			System.Diagnostics.Debug.WriteLine ("Assigning roles to contacts");

			for (int i = 0; i < contacts.Length; i++)
			{
				contacts[i].Roles.Add (roles[i % roles.Length]);
				contacts[i].Roles.Add (roles[(i + 1) % roles.Length]);
			}
		}


		public static void AssignComments(AbstractContactEntity[] contacts, CommentEntity[] comments)
		{
			System.Diagnostics.Debug.WriteLine ("Assigning comments to contacts");

			for (int i = 0; i < comments.Length; i++)
			{
				contacts[i % contacts.Length].Comments.Add (comments[i]);
			}
		}


		public static void AssignContacts(AbstractContactEntity[] contacts, NaturalPersonEntity[] naturalPersons, LegalPersonEntity[] legalPersons)
		{
			System.Diagnostics.Debug.WriteLine ("Assigning contacts to persons");

			for (int i = 0; i < contacts.Length; i++)
			{
				contacts[i].NaturalPerson = naturalPersons[i % naturalPersons.Length];
				contacts[i].LegalPerson = legalPersons[i % legalPersons.Length];

				naturalPersons[i % naturalPersons.Length].Contacts.Add (contacts[i]);
				legalPersons[i % legalPersons.Length].Contacts.Add (contacts[i]);
			}
		}


		public static void AssignParents(LegalPersonEntity[] legalPersons)
		{
			System.Diagnostics.Debug.WriteLine ("Assigning parents to legal persons");

			for (int i = 0; i < legalPersons.Length; i++)
			{
				//legalPersons[i].p Parent = legalPersons[(i + 1) % legalPersons.Length];
			}
		}


		public static CommentEntity CreateComment(DataContext dataContext, string text)
		{
			CommentEntity comment = dataContext.CreateEmptyEntity<CommentEntity> ();

			comment.Text = text;

			return comment;
		}


		public static ContactRoleEntity CreateContactRole(DataContext dataContext, string name)
		{
			ContactRoleEntity contactRole = dataContext.CreateEmptyEntity<ContactRoleEntity> ();

			contactRole.Name = name;

			return contactRole;
		}


		public static MailContactEntity CreateMailContact(DataContext dataContext, string complement, AddressEntity address)
		{
			MailContactEntity mailContact = dataContext.CreateEmptyEntity<MailContactEntity> ();

			mailContact.Complement = complement;
			mailContact.Address = address;

			return mailContact;
		}


		public static AddressEntity CreateAddresss(DataContext dataContext, string complement, StreetEntity street, PostBoxEntity postBox, LocationEntity location)
		{
			AddressEntity address = dataContext.CreateEmptyEntity<AddressEntity> ();

			address.Street = street;
			address.PostBox = postBox;
			address.Location = location;

			return address;
		}


		public static StreetEntity CreateStreet(DataContext dataContext, string complement, string streetName)
		{
			StreetEntity location = dataContext.CreateEmptyEntity<StreetEntity> ();

			location.Complement = complement;
			location.StreetName = streetName;

			return location;
		}


		public static PostBoxEntity CreatePostBox(DataContext dataContext, string number)
		{
			PostBoxEntity location = dataContext.CreateEmptyEntity<PostBoxEntity> ();

			location.Number = number;

			return location;
		}


		public static LocationEntity CreateLocation(DataContext dataContext, string postalCode, string name, CountryEntity country, RegionEntity region)
		{
			LocationEntity location = dataContext.CreateEmptyEntity<LocationEntity> ();

			location.PostalCode = postalCode;
			location.Name = name;
			location.Country = country;
			location.Region = region;

			return location;
		}


		public static RegionEntity CreateRegion(DataContext dataContext, string code, string name, CountryEntity country)
		{
			RegionEntity region = dataContext.CreateEmptyEntity<RegionEntity> ();

			region.Code = code;
			region.Name = name;
			region.Country = country;

			return region;
		}


		public static CountryEntity CreateCountry(DataContext dataContext, string code, string name)
		{
			CountryEntity country = dataContext.CreateEmptyEntity<CountryEntity> ();

			country.Code = code;
			country.Name = name;

			return country;
		}


		public static TelecomTypeEntity CreateTelecomType(DataContext dataContext, string code, string name)
		{
			TelecomTypeEntity telecomType = dataContext.CreateEmptyEntity<TelecomTypeEntity> ();

			telecomType.Code = code;
			telecomType.Name = name;

			return telecomType;
		}


		public static TelecomContactEntity CreateTelecomContact(DataContext dataContext, string number, string extension, TelecomTypeEntity telecomType)
		{
			TelecomContactEntity contact = dataContext.CreateEmptyEntity<TelecomContactEntity> ();

			contact.Number = number;
			contact.Extension = extension;
			contact.TelecomType = telecomType;

			return contact;
		}


		public static UriSchemeEntity CreateUriScheme(DataContext dataContext, string code, string name)
		{
			UriSchemeEntity uriScheme = dataContext.CreateEmptyEntity<UriSchemeEntity> ();

			uriScheme.Code = code;
			uriScheme.Name = name;

			return uriScheme;
		}


		public static UriContactEntity CreateUriContact(DataContext dataContext, string uri, UriSchemeEntity uriScheme)
		{
			UriContactEntity contact = dataContext.CreateEmptyEntity<UriContactEntity> ();

			contact.Uri = uri;
			contact.UriScheme = uriScheme;

			return contact;
		}


		public static LanguageEntity CreateLanguage(DataContext dataContext, string code, string name)
		{
			LanguageEntity language = dataContext.CreateEmptyEntity<LanguageEntity> ();

			language.Code = code;
			language.Name = name;

			return language;
		}


		public static PersonGenderEntity CreatePersonGender(DataContext dataContext, string code, string name)
		{
			PersonGenderEntity gender = dataContext.CreateEmptyEntity<PersonGenderEntity> ();

			gender.Code = code;
			gender.Name = name;

			return gender;
		}


		public static PersonTitleEntity CreatePersonTitle(DataContext dataContext, string name, string shortName)
		{
			PersonTitleEntity title = dataContext.CreateEmptyEntity<PersonTitleEntity> ();

			title.ShortName = shortName;
			title.Name = name;

			return title;
		}


		public static LegalPersonTypeEntity CreateLegalPersonType(DataContext dataContext, string name, string shortName)
		{
			LegalPersonTypeEntity type = dataContext.CreateEmptyEntity<LegalPersonTypeEntity> ();

			type.ShortName = shortName;
			type.Name = name;

			return type;
		}


		public static NaturalPersonEntity CreateNaturalPerson(DataContext dataContext, string firstName, string lastName, Date birthday, LanguageEntity language, PersonTitleEntity title, PersonGenderEntity gender)
		{
			NaturalPersonEntity person = dataContext.CreateEmptyEntity<NaturalPersonEntity> ();

			person.Firstname = firstName;
			person.Lastname = lastName;
			person.PreferredLanguage = language;
			person.Gender = gender;
			person.Title = title;
			person.BirthDate = birthday;

			return person;
		}


		public static LegalPersonEntity CreateLegalPerson(DataContext dataContext, string name, string shortName, string complement, LegalPersonTypeEntity type, LanguageEntity language)
		{
			LegalPersonEntity person = dataContext.CreateEmptyEntity<LegalPersonEntity> ();

			person.Name = name;
			person.ShortName = shortName;
			person.Complement = complement;
			person.PreferredLanguage = language;
			person.LegalPersonType = type;

			return person;
		}


		private static DbInfrastructure dbInfrastructure;


	}


}
