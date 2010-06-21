//	Copyright © 2008-2010, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support.EntityEngine;

using Epsitec.Cresus.Core;
using Epsitec.Cresus.Core.Data;
using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Database;
using Epsitec.Cresus.DataLayer;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core
{
	public sealed partial class CoreData
	{
		public IEnumerable<LegalPersonEntity> GetLegalPersons()
		{
			return new LegalPersonRepository (this.DataContext).GetAllLegalPersons ();
		}

		public IEnumerable<CountryEntity> GetCountries()
		{
			return new CountryRepository (this.DataContext).GetAllCountries ();
		}

		public IEnumerable<LocationEntity> GetLocations()
		{
			return new LocationRepository (this.DataContext).GetAllLocations ();
		}

		public IEnumerable<ContactRoleEntity> GetRoles()
		{
			return new ContactRoleRepository (this.DataContext).GetAllContactRoles ();
		}

		public IEnumerable<UriSchemeEntity> GetUriSchemes()
		{
			return new UriSchemeRepository (this.DataContext).GetAllUriSchemes ();
		}

		public UriSchemeEntity GetUriScheme(string code)
		{
			return new UriSchemeRepository (this.DataContext).GetUriSchemesByCode (code, 0, 1).FirstOrDefault ();
		}

		public IEnumerable<TelecomTypeEntity> GetTelecomTypes()
		{
			return new TelecomTypeRepository (this.DataContext).GetAllTelecomTypes ();
		}

		public IEnumerable<AbstractPersonEntity> GetAbstractPersons()
		{
			return new AbstractPersonRepository (this.DataContext).GetAllAbstractPersons ();
		}

		public IEnumerable<PersonTitleEntity> GetTitles()
		{
			return new PersonTitleRepository (this.DataContext).GetAllPersonTitles ();
		}

		public IEnumerable<PersonGenderEntity> GetGenders()
		{
			return new PersonGenderRepository (this.DataContext).GetAllPersonGenders ();
		}

		public IEnumerable<CustomerEntity> GetCustomers()
		{
			return new CustomerRepository (this.DataContext).GetAllCustomers ();
		}


		private void PopulateDatabaseHack()
		{
			CountryEntity[] countries = this.InsertCountriesInDatabase ().ToArray ();
			LocationEntity[] locations = this.InsertLocationsInDatabase (countries).ToArray ();
			ContactRoleEntity[] roles = this.InsertContactRolesInDatabase ().ToArray ();
			UriSchemeEntity[] uriSchemes = this.InsertUriSchemesInDatabase ().ToArray ();
			TelecomTypeEntity[] telecomTypes = this.InsertTelecomTypesInDatabase ().ToArray ();
			PersonTitleEntity[] personTitles = this.InsertPersonTitlesInDatabase ().ToArray ();
			PersonGenderEntity[] personGenders = this.InsertPersonGendersInDatabase ().ToArray ();
			AbstractPersonEntity[] abstractPersons = this.InsertAbstractPersonsInDatabase (locations, roles, uriSchemes, telecomTypes, personTitles, personGenders).ToArray ();
			CustomerEntity[] customers = this.InsertCustomersInDatabase (abstractPersons).ToArray ();

			this.DataContext.SaveChanges ();
		}


		private IEnumerable<CountryEntity> InsertCountriesInDatabase()
		{
			for (int i = 0; i < CoreData.countries.Length; i += 2)
			{
				CountryEntity country = this.DataContext.CreateEmptyEntity<CountryEntity> ();

				country.Code = CoreData.countries[i + 0];
				country.Name = CoreData.countries[i + 1];

				yield return country;
			}
		}

		private IEnumerable<LocationEntity> InsertLocationsInDatabase(IEnumerable<CountryEntity> countries)
		{
			CountryEntity swiss = countries.First (c => c.Code == "CH");

			for (int i = 0; i < CoreData.locations.Length; i += 2)
			{
				LocationEntity location = this.DataContext.CreateEmptyEntity<LocationEntity>();
				
				location.Country = swiss;
				location.PostalCode = CoreData.locations[i + 0];
				location.Name = CoreData.locations[i + 1];
				
				yield return location;
			}
		}

		private IEnumerable<ContactRoleEntity> InsertContactRolesInDatabase()
		{
			string[] names = new string[] { "professionnel", "commande", "livraison", "facturation", "privé" };
			int rank = 0;

			foreach (string name in names)
			{
				ContactRoleEntity contactRole = this.DataContext.CreateEmptyEntity<ContactRoleEntity> ();

				contactRole.Name = name;
				contactRole.Rank = rank++;

				yield return contactRole;
			}
		}

		private IEnumerable<UriSchemeEntity> InsertUriSchemesInDatabase()
		{
			string[] codes = new string[] { "mailto" };
			string[] names = new string[] { "Mail" };

			for (int i = 0; i < codes.Length && i < names.Length; i++)
			{
				UriSchemeEntity uriScheme = this.DataContext.CreateEmptyEntity<UriSchemeEntity> ();

				uriScheme.Code = codes[i];
				uriScheme.Name = names[i];

				yield return uriScheme;
			}
		}

		private IEnumerable<TelecomTypeEntity> InsertTelecomTypesInDatabase()
		{
			string[] codes = new string[] { "fixnet", "mobile", "fax" };
			string[] names = new string[] { "Téléphone fixe", "Téléphone mobile", "Télécopieur (fax)" };
			int rank = 0;

			for (int i = 0; i < codes.Length && i < names.Length; i++)
			{
				TelecomTypeEntity telecomType = this.DataContext.CreateEmptyEntity<TelecomTypeEntity> ();

				telecomType.Code = codes[i];
				telecomType.Name = names[i];
				telecomType.Rank = rank++;

				yield return telecomType;
			}
		}

		private IEnumerable<PersonTitleEntity> InsertPersonTitlesInDatabase()
		{
			string[] shortNames = new string[] { "M.", "Mme", "Mlle" };
			string[] names = new string[] { "Monsieur", "Madame", "Mademoiselle" };

			for (int i = 0; i < shortNames.Length && i < names.Length; i++)
			{
				PersonTitleEntity personTitle = this.DataContext.CreateEmptyEntity<PersonTitleEntity> ();

				personTitle.ShortName = shortNames[i];
				personTitle.Name = names[i];
				
				yield return personTitle;
			}
		}

		private IEnumerable<PersonGenderEntity> InsertPersonGendersInDatabase()
		{
			string[] codes = new string[] { "♂", "♀" };
			string[] names = new string[] { "Homme", "Femme" };

			for (int i = 0; i < codes.Length && i < names.Length; i++)
			{
				PersonGenderEntity personGender = this.DataContext.CreateEmptyEntity<PersonGenderEntity> ();

				personGender.Code = codes[i];
				personGender.Name = names[i];

				yield return personGender;
			}
		}

		private IEnumerable<AbstractPersonEntity> InsertAbstractPersonsInDatabase(IEnumerable<LocationEntity> locations, IEnumerable<ContactRoleEntity> roles, IEnumerable<UriSchemeEntity> uriSchemes, IEnumerable<TelecomTypeEntity> telecomTypes, IEnumerable<PersonTitleEntity> personTitles, IEnumerable<PersonGenderEntity> personGenders)
		{
			LegalPersonEntity company = this.DataContext.CreateEmptyEntity<LegalPersonEntity> ();
			NaturalPersonEntity person1 = this.DataContext.CreateEmptyEntity<NaturalPersonEntity> ();
			NaturalPersonEntity person2 = this.DataContext.CreateEmptyEntity<NaturalPersonEntity> ();

			ContactRoleEntity role1 = roles.Where (x => x.Name == "facturation").First ();
			ContactRoleEntity role2 = roles.Where (x => x.Name == "professionnel").First ();
			ContactRoleEntity role3 = roles.Where (x => x.Name == "privé").First ();

			TelecomTypeEntity telecomType1 = telecomTypes.Where (x => x.Code == "fixnet").First ();
			TelecomTypeEntity telecomType2 = telecomTypes.Where (x => x.Code == "mobile").First ();
			TelecomTypeEntity telecomType3 = telecomTypes.Where (x => x.Code == "fixnet").First ();

			UriSchemeEntity uriScheme1 = uriSchemes.Where (x => x.Code == "mailto").First ();

			LocationEntity location1 = locations.Where (x => x.PostalCode == "1400").First ();

			PersonTitleEntity title1 = personTitles.Where (x => x.ShortName == "M.").First ();
			PersonGenderEntity gender1 = personGenders.Where (x => x.Code == "♂").First ();

			StreetEntity street1 = this.DataContext.CreateEmptyEntity<StreetEntity> ();
			street1.StreetName = "Ch. du Fontenay 3";
			street1.Complement = "2ème étage";

			StreetEntity street2 = this.DataContext.CreateEmptyEntity<StreetEntity> ();
			street2.StreetName = "Ch. du Fontenay 6";

			PostBoxEntity postbox1 = this.DataContext.CreateEmptyEntity<PostBoxEntity> ();
			postbox1.Number = "Case postale 1234";


			AddressEntity address1 = this.DataContext.CreateEmptyEntity<AddressEntity> ();
			address1.Location = location1;
			address1.Street = street1;
			address1.PostBox = postbox1;

			AddressEntity address2 = this.DataContext.CreateEmptyEntity<AddressEntity> ();
			address2.Location = location1;
			address2.Street = street2;

			CommentEntity comment1 = this.DataContext.CreateEmptyEntity<CommentEntity> ();
			comment1.Text = "Bureaux ouverts de 9h-12h et 14h-16h30";

			MailContactEntity mail1 = this.DataContext.CreateEmptyEntity<MailContactEntity> ();
			mail1.Address = address1;
			mail1.Complement = "Direction";
			mail1.Comments.Add (comment1);
			mail1.Roles.Add (role1);
			mail1.LegalPerson = company;
			mail1.NaturalPerson = person1;

			MailContactEntity mail2 = this.DataContext.CreateEmptyEntity<MailContactEntity> ();
			mail2.Address = address2;
			mail2.Roles.Add (role3);
			mail2.NaturalPerson = person1;

			TelecomContactEntity telecom1 = this.DataContext.CreateEmptyEntity<TelecomContactEntity> ();
			telecom1.TelecomType = telecomType1;
			telecom1.LegalPerson = company;
			telecom1.Number = "+41 848 27 37 87";
			telecom1.Roles.Add (role1);
			telecom1.Roles.Add (role2);

			TelecomContactEntity telecom2 = this.DataContext.CreateEmptyEntity<TelecomContactEntity> ();
			telecom2.TelecomType = telecomType2;
			telecom2.NaturalPerson = person1;
			telecom2.Number = "+41 79 555 55 55";
			telecom2.Roles.Add (role3);

			TelecomContactEntity telecom3 = this.DataContext.CreateEmptyEntity<TelecomContactEntity> ();
			telecom3.TelecomType = telecomType3;
			telecom3.LegalPerson = company;
			telecom3.NaturalPerson = person1;
			telecom3.Number = "+41 24 425 08 30";
			telecom3.Roles.Add (role2);

			UriContactEntity uri1 = this.DataContext.CreateEmptyEntity<UriContactEntity> ();
			uri1.LegalPerson = company;
			uri1.Uri = "epsitec@epsitec.ch";
			uri1.UriScheme = uriScheme1;
			uri1.Roles.Add (role2);
			uri1.Roles.Add (role3);

			UriContactEntity uri2 = this.DataContext.CreateEmptyEntity<UriContactEntity> ();
			uri2.LegalPerson = company;
			uri2.Uri = "arnaud@epsitec.ch";
			uri2.UriScheme = uriScheme1;
			uri2.Roles.Add (role3);

			UriContactEntity uri3 = this.DataContext.CreateEmptyEntity<UriContactEntity> ();
			uri3.LegalPerson = company;
			uri3.Uri = "perre.arnaud@opac.ch";
			uri3.UriScheme = uriScheme1;
			uri3.Roles.Add (role3);

			company.Complement = "Logiciels de gestion Crésus";
			company.Name = "Epsitec SA";
			company.Contacts.Add (mail1);
			company.Contacts.Add (telecom3);
			company.Contacts.Add (uri1);

			person1.BirthDate = new Common.Types.Date (day: 11, month: 2, year: 1972);
			person1.Firstname = "Pierre";
			person1.Lastname = "Arnaud";
			person1.Title = title1;
			person1.Contacts.Add (mail1);
			person1.Contacts.Add (mail2);
			person1.Contacts.Add (telecom1);
			person1.Contacts.Add (telecom2);
			person1.Contacts.Add (telecom3);
			person1.Contacts.Add (uri1);
			person1.Contacts.Add (uri2);
			person1.Contacts.Add (uri3);
			
			person2.Firstname = "Daniel";
			person2.Lastname  = "Roux";
			person2.BirthDate = new Common.Types.Date (day: 31, month: 3, year: 1958);

			yield return person1;
			yield return person2;
			yield return company;
		}
		
		private IEnumerable<CustomerEntity> InsertCustomersInDatabase(IEnumerable<AbstractPersonEntity> persons)
		{
			int id = 1000;

			foreach (var person in persons)
			{
				CustomerEntity customer = this.DataContext.CreateEmptyEntity<CustomerEntity> ();

				customer.Id = (id++).ToString ();
				customer.Person = person;
				customer.CustomerSince = Common.Types.Date.Today;

				yield return customer;
			}
		}
	}
}
