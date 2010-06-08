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


		public IEnumerable<CountryEntity> GetCountries()
		{
			//	HACK: this method will soon be replaced by repositories

			if (this.UseHack)
			{
				if (this.sampleCountries == null)
				{
					this.sampleCountries = new List<CountryEntity> ();
					this.CreateSampleCountries (this.sampleCountries);
				}

				return this.sampleCountries;
			}
			else
			{
				return new CountryRepository (this.DataContext).GetAllCountries ();
			}
		}


		public IEnumerable<LocationEntity> GetLocations()
		{
			//	HACK: this method will soon be replaced by repositories

			if (this.UseHack)
			{
				if (this.sampleLocations == null)
				{
					IEnumerable<CountryEntity> countries = this.GetCountries ();

					this.sampleLocations = new List<LocationEntity> ();
					this.CreateSampleLocations (this.sampleLocations, countries);
				}

				return this.sampleLocations;
			}
			else
			{
				return new LocationRepository (this.DataContext).GetAllLocations ();
			}
		}


		public IEnumerable<ContactRoleEntity> GetRoles()
		{
			//	HACK: this method will soon be replaced by repositories

			if (this.UseHack)
			{
				if (this.samplePersons == null)
				{
					this.sampleRoles = new List<ContactRoleEntity> ();
					this.CreateSampleRoles (this.sampleRoles);
				}

				return this.sampleRoles;
			}
			else
			{
				return new ContactRoleRepository (this.DataContext).GetAllContactRoles ();
			}
		}


		public IEnumerable<UriSchemeEntity> GetUriSchemes()
		{
			//	HACK: this method will soon be replaced by repositories

			if (this.UseHack)
			{
				if (this.uriSchemes == null)
				{
					this.uriSchemes = new List<UriSchemeEntity> ();
					this.CreateSampleUriSchemes (this.uriSchemes);
				}

				return this.uriSchemes;
			}
			else
			{
				return new UriSchemeRepository (this.DataContext).GetAllUriSchemes ();
			}
		}

		public UriSchemeEntity GetUriScheme(string code)
		{
			//	HACK: this method will soon be replaced by repositories

			if (this.UseHack)
			{
				return this.GetUriSchemes ().Where (x => x.Code == code).FirstOrDefault ();
			}
			else
			{
				return new UriSchemeRepository (this.DataContext).GetUriSchemesByCode (code, 0, 1).FirstOrDefault ();
			}
		}


		public IEnumerable<TelecomTypeEntity> GetTelecomTypes()
		{
			//	HACK: this method will soon be replaced by repositories

			if (this.UseHack)
			{
				if (this.telecomTypes == null)
				{
					this.telecomTypes = new List<TelecomTypeEntity> ();
					this.CreateSampleTelecomTypes (this.telecomTypes);
				}

				return this.telecomTypes;
			}
			else
			{
				return new TelecomTypeRepository (this.DataContext).GetAllTelecomTypes ();
			}
		}


		public IEnumerable<AbstractPersonEntity> GetAbstractPersons()
		{
			//	HACK: this method will soon be replaced by repositories

			if (this.UseHack)
			{
				if (this.samplePersons == null)
				{
					IEnumerable<ContactRoleEntity> roles = this.GetRoles ();
					IEnumerable<LocationEntity> locations = this.GetLocations ();

					this.samplePersons = new List<AbstractPersonEntity> ();
					this.CreateSamplePersons (this.samplePersons);
				}

				return this.samplePersons;
			}
			else
			{
				return new AbstractPersonRepository (this.DataContext).GetAllAbstractPersons ();
			}
		}


		public IEnumerable<Entities.PersonTitleEntity> GetTitles()
		{
			//	HACK: this method will soon be replaced by repositories

			if (this.UseHack)
			{
				if (this.sampleTitles == null)
				{
					this.sampleTitles = new List<PersonTitleEntity> ();
					this.CreateSampleTitles (this.sampleTitles);
				}

				return this.sampleTitles;
			}
			else
			{
				return new PersonTitleRepository (this.DataContext).GetAllPersonTitles ();
			}
		}


		public IEnumerable<Entities.PersonGenderEntity> GetGenders()
		{
			//	HACK: this method will soon be replaced by repositories

			if (this.UseHack)
			{
				if (this.sampleGenders == null)
				{
					this.sampleGenders = new List<PersonGenderEntity> ();
					this.CreateSampleGenders (this.sampleGenders);
				}

				return this.sampleGenders;
			}
			else
			{
				return new PersonGenderRepository (this.DataContext).GetAllPersonGenders ();
			}
		}


		private void CreateSampleCountries(List<CountryEntity> countries)
		{
			
			for (int i = 0; i < CoreData.countries.Length; i+=2)
			{
				var entity = new CountryEntity ();

				entity.Code = CoreData.countries[i+0];
				entity.Name = CoreData.countries[i+1];

				countries.Add (entity);
			}
		}


		private void CreateSampleLocations(List<LocationEntity> locations, IEnumerable<CountryEntity> countries)
		{
			CountryEntity swiss = null;

			foreach (var country in countries)
			{
				if (country.Code == "CH")
				{
					swiss = country;
					break;
				}
			}

			for (int i = 0; i < CoreData.locations.Length; i+=2)
			{
				var entity = new LocationEntity ();

				entity.Country    = swiss;
				entity.PostalCode = CoreData.locations[i+0];
				entity.Name       = CoreData.locations[i+1];

				locations.Add (entity);
			}
		}


		private void CreateSampleRoles(List<ContactRoleEntity> roles)
		{
			var context = EntityContext.Current;

			var role1 = context.CreateEmptyEntity<ContactRoleEntity> ();
			role1.Name = "professionnel";

			var role2 = context.CreateEmptyEntity<ContactRoleEntity> ();
			role2.Name = "commande";

			var role3 = context.CreateEmptyEntity<ContactRoleEntity> ();
			role3.Name = "livraison";

			var role4 = context.CreateEmptyEntity<ContactRoleEntity> ();
			role4.Name = "facturation";

			var role5 = context.CreateEmptyEntity<ContactRoleEntity> ();
			role5.Name = "privé";

			roles.Add (role1);
			roles.Add (role2);
			roles.Add (role3);
			roles.Add (role4);
			roles.Add (role5);
		}


		private void CreateSampleUriSchemes(List<UriSchemeEntity> uriSchemes)
		{
			var uriScheme1 = EntityContext.Current.CreateEmptyEntity<UriSchemeEntity> ();

			using (uriScheme1.DefineOriginalValues ())
			{
				uriScheme1.Code = "mailto";
				uriScheme1.Name = "Mail";
			}

			uriSchemes.Add (uriScheme1);
		}


		private void CreateSampleTelecomTypes(List<TelecomTypeEntity> telecomTypes)
		{
			var telecomType1 = EntityContext.Current.CreateEmptyEntity<TelecomTypeEntity> ();
			var telecomType2 = EntityContext.Current.CreateEmptyEntity<TelecomTypeEntity> ();
			var telecomType3 = EntityContext.Current.CreateEmptyEntity<TelecomTypeEntity> ();

			using (telecomType1.DefineOriginalValues ())
			{
				telecomType1.Code = "fixnet";
				telecomType1.Name = "Téléphone fixe";
			}

			using (telecomType2.DefineOriginalValues ())
			{
				telecomType2.Code = "mobile";
				telecomType2.Name = "Téléphone mobile";
			}

			using (telecomType3.DefineOriginalValues ())
			{
				telecomType3.Code = "fax";
				telecomType3.Name = "Télécopieur (fax)";
			}

			telecomTypes.Add (telecomType1);
			telecomTypes.Add (telecomType2);
			telecomTypes.Add (telecomType3);
		}


		private void CreateSampleTitles(List<PersonTitleEntity> titles)
		{
			var context = EntityContext.Current;

			var title1 = context.CreateEmptyEntity<PersonTitleEntity> ();
			var title2 = context.CreateEmptyEntity<PersonTitleEntity> ();
			var title3 = context.CreateEmptyEntity<PersonTitleEntity> ();

			using (title1.DefineOriginalValues ())
			{
				title1.Name = "Monsieur";
				title1.ShortName = "M.";
			}

			using (title2.DefineOriginalValues ())
			{
				title2.Name = "Madame";
				title2.ShortName = "Mme";
			}

			using (title3.DefineOriginalValues ())
			{
				title3.Name = "Mademoiselle";
				title3.ShortName = "Mlle";
			}

			titles.Add (title1);
			titles.Add (title2);
			titles.Add (title3);
		}


		private void CreateSampleGenders(List<PersonGenderEntity> genders)
		{
			var context = EntityContext.Current;

			var gender1 = context.CreateEmptyEntity<PersonGenderEntity> ();
			var gender2 = context.CreateEmptyEntity<PersonGenderEntity> ();

			using (gender1.DefineOriginalValues ())
			{
				gender1.Name = "Homme";
				gender1.Code = "♂";
			}

			using (gender2.DefineOriginalValues ())
			{
				gender2.Name = "Femme";
				gender2.Code = "♀";
			}

			genders.Add (gender1);
			genders.Add (gender2);
		}


		private void CreateSamplePersons(List<AbstractPersonEntity> persons)
		{
			var context = EntityContext.Current;

			var role1 = this.GetRoles ().Where (x => x.Name == "facturation").First ();
			var role2 = this.GetRoles ().Where (x => x.Name == "professionnel").First ();
			var role3 = this.GetRoles ().Where (x => x.Name == "privé").First ();

			var telecomType1 = this.GetTelecomTypes ().Where (x => x.Code == "fixnet").First ();
			var telecomType2 = this.GetTelecomTypes ().Where (x => x.Code == "mobile").First ();
			var telecomType3 = this.GetTelecomTypes ().Where (x => x.Code == "fixnet").First ();
			
			var uriScheme1 = this.GetUriSchemes ().Where (x => x.Code == "mailto").First ();
			
			var yverdon = this.GetLocations ().Where (x => x.PostalCode == "1400").First ();

			var street1 = context.CreateEmptyEntity<StreetEntity> ();
			var street2 = context.CreateEmptyEntity<StreetEntity> ();
			var postbox1 = context.CreateEmptyEntity<PostBoxEntity> ();
			var address1 = context.CreateEmptyEntity<AddressEntity> ();
			var address2 = context.CreateEmptyEntity<AddressEntity> ();
			var comment1 = context.CreateEmptyEntity<CommentEntity> ();
			var contact1 = context.CreateEmptyEntity<MailContactEntity> ();
			var contact2 = context.CreateEmptyEntity<MailContactEntity> ();
			var title1 = this.GetTitles ().Where (x => x.ShortName == "M.").First ();
			var person1 = context.CreateEmptyEntity<NaturalPersonEntity> ();
			var person2 = context.CreateEmptyEntity<NaturalPersonEntity> ();
			var enterprise = context.CreateEmptyEntity<LegalPersonEntity> ();
			
			using (street1.DefineOriginalValues ())
			{
				street1.StreetName = "Ch. du Fontenay 3";
				street1.Complement = "2ème étage";
			}

			using (street2.DefineOriginalValues ())
			{
				street2.StreetName = "Ch. du Fontenay 6";
			}

			using (postbox1.DefineOriginalValues ())
			{
				postbox1.Number = "Case postale 1234";
			}

			using (address1.DefineOriginalValues ())
			{
				address1.Location = yverdon;
				address1.Street = street1;
				address1.PostBox = postbox1;
			}

			using (address2.DefineOriginalValues ())
			{
				address2.Location = yverdon;
				address2.Street = street2;
			}

			using (comment1.DefineOriginalValues ())
			{
				comment1.Text = "Bureaux ouverts de 9h-12h et 14h-16h30";
			}

			using (contact1.DefineOriginalValues ())
			{
				contact1.Address = address1;
				contact1.Complement = "Direction";
				contact1.Comments.Add (comment1);
				contact1.Roles.Add (role1);
				contact1.LegalPerson = enterprise;
				contact1.NaturalPerson = person1;
			}

			using (contact2.DefineOriginalValues ())
			{
				contact2.Address = address2;
				contact2.Roles.Add (role3);
				contact2.NaturalPerson = person1;
			}

			var telecom1 = context.CreateEmptyEntity<TelecomContactEntity> ();
			var telecom2 = context.CreateEmptyEntity<TelecomContactEntity> ();
			var telecom3 = context.CreateEmptyEntity<TelecomContactEntity> ();

			using (telecom1.DefineOriginalValues ())
			{
				telecom1.TelecomType = telecomType1;
				telecom1.LegalPerson = enterprise;
				telecom1.Number = "+41 848 27 37 87";
				telecom1.Roles.Add (role1);
				telecom1.Roles.Add (role2);
			}

			using (telecom2.DefineOriginalValues ())
			{
				telecom2.TelecomType = telecomType2;
				telecom2.NaturalPerson = person1;
				telecom2.Number = "+41 79 555 55 55";
				telecom2.Roles.Add (role3);
			}

			using (telecom3.DefineOriginalValues ())
			{
				telecom3.TelecomType = telecomType3;
				telecom3.LegalPerson = enterprise;
				telecom3.NaturalPerson = person1;
				telecom3.Number = "+41 24 425 08 30";
				telecom3.Roles.Add (role2);
			}


			var uri1 = context.CreateEmptyEntity<UriContactEntity> ();
			var uri2 = context.CreateEmptyEntity<UriContactEntity> ();
			var uri3 = context.CreateEmptyEntity<UriContactEntity> ();

			using (uri1.DefineOriginalValues ())
			{
				uri1.LegalPerson = enterprise;
				uri1.Uri = "epsitec@epsitec.ch";
				uri1.UriScheme = uriScheme1;
				uri1.Roles.Add (role2);
				uri1.Roles.Add (role3);
			}

			using (enterprise.DefineOriginalValues ())
			{
				enterprise.Complement = "Logiciels de gestion Crésus";
				enterprise.Name = "Epsitec SA";
				enterprise.Contacts.Add (contact1);
				enterprise.Contacts.Add (telecom3);
				enterprise.Contacts.Add (uri1);
			}


			using (uri2.DefineOriginalValues ())
			{
				uri2.LegalPerson = enterprise;
				uri2.Uri = "arnaud@epsitec.ch";
				uri2.UriScheme = uriScheme1;
				uri2.Roles.Add (role3);
			}

			using (uri3.DefineOriginalValues ())
			{
				uri3.LegalPerson = enterprise;
				uri3.Uri = "perre.arnaud@opac.ch";
				uri3.UriScheme = uriScheme1;
				uri3.Roles.Add (role3);
			}

			using (person1.DefineOriginalValues ())
			{
				person1.BirthDate = new Common.Types.Date (day: 11, month: 2, year: 1972);
				person1.Firstname = "Pierre";
				person1.Lastname = "Arnaud";
				person1.Title = title1;
				person1.Contacts.Add (contact1);
				person1.Contacts.Add (contact2);
				person1.Contacts.Add (telecom1);
				person1.Contacts.Add (telecom2);
				person1.Contacts.Add (telecom3);
				person1.Contacts.Add (uri1);
				person1.Contacts.Add (uri2);
				person1.Contacts.Add (uri3);
			}

			using (person2.DefineOriginalValues ())
			{
				person2.Firstname = "Daniel";
				person2.Lastname  = "Roux";
			}

			persons.Add (person1);
			persons.Add (person2);
			persons.Add (enterprise);
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
			string[] names = new string[] { "professionnel", "commande", "livraison", "privé", "facturation" };

			foreach (string name in names)
			{
				ContactRoleEntity contactRole = this.DataContext.CreateEmptyEntity<ContactRoleEntity> ();

				contactRole.Name = name;

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

			for (int i = 0; i < codes.Length && i < names.Length; i++)
			{
				TelecomTypeEntity telecomType = this.DataContext.CreateEmptyEntity<TelecomTypeEntity> ();

				telecomType.Code = codes[i];
				telecomType.Name = names[i];

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

			StreetEntity street1 = this.DataContext.CreateEmptyEntity<StreetEntity> ();
			street1.StreetName = "Ch. du Fontenay 3";
			street1.Complement = "2ème étage";

			StreetEntity street2 = this.DataContext.CreateEmptyEntity<StreetEntity> ();
			using (street2.DefineOriginalValues ())
			{
				street2.StreetName = "Ch. du Fontenay 6";
			}

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

			MailContactEntity contact1 = this.DataContext.CreateEmptyEntity<MailContactEntity> ();
			contact1.Address = address1;
			contact1.Complement = "Direction";
			contact1.Comments.Add (comment1);
			contact1.Roles.Add (role1);
			contact1.LegalPerson = company;
			contact1.NaturalPerson = person1;

			MailContactEntity contact2 = this.DataContext.CreateEmptyEntity<MailContactEntity> ();
			contact2.Address = address2;
			contact2.Roles.Add (role3);
			contact2.NaturalPerson = person1;

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
			company.Contacts.Add (contact1);
			company.Contacts.Add (telecom3);
			company.Contacts.Add (uri1);

			person1.BirthDate = new Common.Types.Date (day: 11, month: 2, year: 1972);
			person1.Firstname = "Pierre";
			person1.Lastname = "Arnaud";
			person1.Title = title1;
			person1.Contacts.Add (contact1);
			person1.Contacts.Add (contact2);
			person1.Contacts.Add (telecom1);
			person1.Contacts.Add (telecom2);
			person1.Contacts.Add (telecom3);
			person1.Contacts.Add (uri1);
			person1.Contacts.Add (uri2);
			person1.Contacts.Add (uri3);
			person1.BirthDate = null;

			person2.Firstname = "Daniel";
			person2.Lastname  = "Roux";
			person2.BirthDate = null;

			yield return person1;
			yield return person2;
			yield return company;
		}


		List<CountryEntity> sampleCountries;
		List<LocationEntity> sampleLocations;
		List<ContactRoleEntity> sampleRoles;
		List<AbstractPersonEntity> samplePersons;
		List<TelecomTypeEntity> telecomTypes;
		List<UriSchemeEntity> uriSchemes;
		List<PersonTitleEntity> sampleTitles;
		List<PersonGenderEntity> sampleGenders;


	}


}
