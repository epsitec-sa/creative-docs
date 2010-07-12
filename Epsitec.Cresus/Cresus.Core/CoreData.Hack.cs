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
		public IEnumerable<NaturalPersonEntity> GetNaturalPersons()
		{
			return new NaturalPersonRepository (this.DataContext).GetAllNaturalPersons ();
		}

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

		public IEnumerable<LocationEntity> GetLocations(CountryEntity country)
		{
			return new LocationRepository (this.DataContext).GetLocationsByCountry (country);
		}

		public IEnumerable<CaseEventTypeEntity> GetCaseEventTypes()
		{
			return new CaseEventTypeRepository (this.DataContext).GetAllCaseEventTypes ();
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

		public IEnumerable<RelationEntity> GetCustomers()
		{
			return new RelationRepository (this.DataContext).GetAllRelations ();
		}

		public IEnumerable<RelationEntity> GetCustomers(AbstractPersonEntity person)
		{
			var repository = new RelationRepository (this.DataContext);
			var example = repository.CreateRelationExample ();
			example.Person = person;
			return repository.GetRelationsByExample (example);
		}


		private void PopulateDatabaseHack()
		{
			CountryEntity[] countries = this.InsertCountriesInDatabase ().ToArray ();
			LocationEntity[] locations = this.InsertLocationsInDatabase (countries).ToArray ();
			CaseEventTypeEntity[] eventTypes = this.InsertCaseEventTypesInDatabase ().ToArray ();
			ContactRoleEntity[] roles = this.InsertContactRolesInDatabase ().ToArray ();
			UriSchemeEntity[] uriSchemes = this.InsertUriSchemesInDatabase ().ToArray ();
			TelecomTypeEntity[] telecomTypes = this.InsertTelecomTypesInDatabase ().ToArray ();
			PersonTitleEntity[] personTitles = this.InsertPersonTitlesInDatabase ().ToArray ();
			PersonGenderEntity[] personGenders = this.InsertPersonGendersInDatabase ().ToArray ();
			AbstractPersonEntity[] abstractPersons = this.InsertAbstractPersonsInDatabase (locations, roles, uriSchemes, telecomTypes, personTitles, personGenders).ToArray ();
			RelationEntity[] relations = this.InsertRelationsInDatabase (abstractPersons).ToArray ();

			this.DataContext.SaveChanges ();
		}


		private IEnumerable<CountryEntity> InsertCountriesInDatabase()
		{
			for (int i = 0; i < CoreData.countries.Length; i += 2)
			{
				CountryEntity country = this.DataContext.CreateEntity<CountryEntity> ();

				country.Code = CoreData.countries[i + 0];
				country.Name = CoreData.countries[i + 1];

				yield return country;
			}
		}

		private IEnumerable<LocationEntity> InsertLocationsInDatabase(IEnumerable<CountryEntity> countries)
		{
			CountryEntity swiss = countries.First (c => c.Code == "CH");

			for (int i = 0; i < CoreData.swissLocations.Length; i += 2)
			{
				LocationEntity location = this.DataContext.CreateEntity<LocationEntity> ();

				location.Country = swiss;
				location.PostalCode = CoreData.swissLocations[i + 0];
				location.Name = CoreData.swissLocations[i + 1];

				yield return location;
			}

			CountryEntity french = countries.First (c => c.Code == "FR");

			for (int i = 0; i < CoreData.frenchLocations.Length; i += 2)
			{
				LocationEntity location = this.DataContext.CreateEntity<LocationEntity> ();

				location.Country = french;
				location.PostalCode = CoreData.frenchLocations[i + 0];
				location.Name = CoreData.frenchLocations[i + 1];

				yield return location;
			}
		}

		private IEnumerable<CaseEventTypeEntity> InsertCaseEventTypesInDatabase()
		{
			string[] names = new string[]
			{
				"Réception d'une demande",
				"Envoi d'une offre",
				"Envoi d'un bulletin de livraison",
				"Envoi d'une facture",
				"Réception d'un courrier",
				"Téléphone entrant",
				"Téléphone sortant"
			};

			int rank = 0;

			foreach (string name in names)
			{
				CaseEventTypeEntity eventType = this.DataContext.CreateEntity<CaseEventTypeEntity> ();

				eventType.Code = name;
				//?eventType.Rank = rank++;

				yield return eventType;
			}
		}

		private IEnumerable<ContactRoleEntity> InsertContactRolesInDatabase()
		{
			string[] names = new string[] { "professionnel", "commande", "livraison", "facturation", "privé" };
			int rank = 0;

			foreach (string name in names)
			{
				ContactRoleEntity contactRole = this.DataContext.CreateEntity<ContactRoleEntity> ();

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
				UriSchemeEntity uriScheme = this.DataContext.CreateEntity<UriSchemeEntity> ();

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
				TelecomTypeEntity telecomType = this.DataContext.CreateEntity<TelecomTypeEntity> ();

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
				PersonTitleEntity personTitle = this.DataContext.CreateEntity<PersonTitleEntity> ();

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
				PersonGenderEntity personGender = this.DataContext.CreateEntity<PersonGenderEntity> ();

				personGender.Code = codes[i];
				personGender.Name = names[i];

				yield return personGender;
			}
		}

		private IEnumerable<AbstractPersonEntity> InsertAbstractPersonsInDatabase(IEnumerable<LocationEntity> locations, IEnumerable<ContactRoleEntity> roles, IEnumerable<UriSchemeEntity> uriSchemes, IEnumerable<TelecomTypeEntity> telecomTypes, IEnumerable<PersonTitleEntity> personTitles, IEnumerable<PersonGenderEntity> personGenders)
		{
			LegalPersonEntity companyEpsitec = this.DataContext.CreateEntity<LegalPersonEntity> ();
			LegalPersonEntity companyMigros  = this.DataContext.CreateEntity<LegalPersonEntity> ();

			NaturalPersonEntity personPA = this.DataContext.CreateEntity<NaturalPersonEntity> ();
			NaturalPersonEntity personDR = this.DataContext.CreateEntity<NaturalPersonEntity> ();

			ContactRoleEntity roleFact  = roles.Where (x => x.Name == "facturation").First ();
			ContactRoleEntity roleProf  = roles.Where (x => x.Name == "professionnel").First ();
			ContactRoleEntity rolePrive = roles.Where (x => x.Name == "privé").First ();

			TelecomTypeEntity telecomTypeFix    = telecomTypes.Where (x => x.Code == "fixnet").First ();
			TelecomTypeEntity telecomTypeMobile = telecomTypes.Where (x => x.Code == "mobile").First ();
			TelecomTypeEntity telecomTypeFax    = telecomTypes.Where (x => x.Code == "fax").First ();

			UriSchemeEntity uriSchemeMailto = uriSchemes.Where (x => x.Code == "mailto").First ();

			LocationEntity locationYverdon = locations.Where (x => x.PostalCode == "1400").First ();

			PersonTitleEntity titleMonsieur = personTitles.Where (x => x.ShortName == "M.").First ();
			PersonGenderEntity genderHomme = personGenders.Where (x => x.Code == "♂").First ();

			// addressEpsitec

			StreetEntity streetEpsitec = this.DataContext.CreateEntity<StreetEntity> ();
			streetEpsitec.StreetName = "Ch. du Fontenay 3";
			streetEpsitec.Complement = "2ème étage";

			PostBoxEntity postboxEpsitec = this.DataContext.CreateEntity<PostBoxEntity> ();
			postboxEpsitec.Number = "Case postale 1234";

			AddressEntity addressEpsitec = this.DataContext.CreateEntity<AddressEntity> ();
			addressEpsitec.Location = locationYverdon;
			addressEpsitec.Street = streetEpsitec;
			addressEpsitec.PostBox = postboxEpsitec;

			// addressPA

			StreetEntity streetPA = this.DataContext.CreateEntity<StreetEntity> ();
			streetPA.StreetName = "Ch. du Fontenay 6";

			AddressEntity addressPA = this.DataContext.CreateEntity<AddressEntity> ();
			addressPA.Location = locationYverdon;
			addressPA.Street = streetPA;

			// companyEpsitec

			CommentEntity commentEpsitec = this.DataContext.CreateEntity<CommentEntity> ();
			commentEpsitec.Text = "Bureaux ouverts de 9h-12h et 14h-16h30";

			MailContactEntity mailEpsitec1 = this.DataContext.CreateEntity<MailContactEntity> ();
			mailEpsitec1.LegalPerson = companyEpsitec;
			mailEpsitec1.Address = addressEpsitec;
			mailEpsitec1.Comments.Add (commentEpsitec);
			mailEpsitec1.Roles.Add (roleFact);

			CommentEntity commentEpsitecT1 = this.DataContext.CreateEntity<CommentEntity> ();
			commentEpsitecT1.Text = "Administration et vente";

			TelecomContactEntity telecomEpsitec1 = this.DataContext.CreateEntity<TelecomContactEntity> ();
			telecomEpsitec1.LegalPerson = companyEpsitec;
			telecomEpsitec1.TelecomType = telecomTypeFix;
			telecomEpsitec1.Number = "+41 848 27 37 87";
			telecomEpsitec1.Comments.Add (commentEpsitecT1);
			telecomEpsitec1.Roles.Add (roleProf);
			telecomEpsitec1.Roles.Add (roleFact);

			CommentEntity commentEpsitecT2 = this.DataContext.CreateEntity<CommentEntity> ();
			commentEpsitecT2.Text = "Assistance technique (hotline)";

			TelecomContactEntity telecomEpsitec2 = this.DataContext.CreateEntity<TelecomContactEntity> ();
			telecomEpsitec2.LegalPerson = companyEpsitec;
			telecomEpsitec2.TelecomType = telecomTypeFix;
			telecomEpsitec2.Number = "+41 848 27 37 89";
			telecomEpsitec2.Comments.Add (commentEpsitecT2);
			telecomEpsitec2.Roles.Add (roleProf);

			UriContactEntity uriEpsitec1 = this.DataContext.CreateEntity<UriContactEntity> ();
			uriEpsitec1.LegalPerson = companyEpsitec;
			uriEpsitec1.Uri = "epsitec@epsitec.ch";
			uriEpsitec1.UriScheme = uriSchemeMailto;
			uriEpsitec1.Roles.Add (roleProf);

			UriContactEntity uriEpsitec2 = this.DataContext.CreateEntity<UriContactEntity> ();
			uriEpsitec2.LegalPerson = companyEpsitec;
			uriEpsitec2.Uri = "support@epsitec.ch";
			uriEpsitec2.UriScheme = uriSchemeMailto;
			uriEpsitec2.Roles.Add (roleProf);

			companyEpsitec.Complement = "Logiciels de gestion Crésus";
			companyEpsitec.Name = "Epsitec SA";
			companyEpsitec.Contacts.Add (mailEpsitec1);
			companyEpsitec.Contacts.Add (telecomEpsitec1);
			companyEpsitec.Contacts.Add (telecomEpsitec2);
			companyEpsitec.Contacts.Add (uriEpsitec1);
			companyEpsitec.Contacts.Add (uriEpsitec2);

			// companyMigros

			companyMigros.Complement = "Le géant de l'alimentation";
			companyMigros.Name = "Migros SA";

			// personPA

			MailContactEntity mailPA1 = this.DataContext.CreateEntity<MailContactEntity> ();
			mailPA1.NaturalPerson = personPA;
			mailPA1.LegalPerson = companyEpsitec;
			mailPA1.Address = addressEpsitec;
			mailPA1.Complement = "Direction";
			mailPA1.Roles.Add (roleProf);

			MailContactEntity mailPA2 = this.DataContext.CreateEntity<MailContactEntity> ();
			mailPA2.NaturalPerson = personPA;
			mailPA2.Address = addressPA;
			mailPA2.Roles.Add (rolePrive);

			TelecomContactEntity telecomPA1 = this.DataContext.CreateEntity<TelecomContactEntity> ();
			telecomPA1.NaturalPerson = personPA;
			telecomPA1.TelecomType = telecomTypeMobile;
			telecomPA1.Number = "+41 79 367 45 97";
			telecomPA1.Roles.Add (rolePrive);
			telecomPA1.Roles.Add (roleProf);

			TelecomContactEntity telecomPA2 = this.DataContext.CreateEntity<TelecomContactEntity> ();
			telecomPA2.NaturalPerson = personPA;
			telecomPA2.TelecomType = telecomTypeFix;
			telecomPA2.Number = "+41 24 425 08 09";
			telecomPA2.Roles.Add (roleProf);

			TelecomContactEntity telecomPA3 = this.DataContext.CreateEntity<TelecomContactEntity> ();
			telecomPA3.NaturalPerson = personPA;
			telecomPA3.TelecomType = telecomTypeFax;
			telecomPA3.Number = "+41 24 555 83 59";
			telecomPA3.Roles.Add (rolePrive);
			telecomPA3.Roles.Add (roleProf);

			UriContactEntity uriPA1 = this.DataContext.CreateEntity<UriContactEntity> ();
			uriPA1.NaturalPerson = personPA;
			uriPA1.Uri = "arnaud@epsitec.ch";
			uriPA1.UriScheme = uriSchemeMailto;
			uriPA1.Roles.Add (rolePrive);

			UriContactEntity uriPA2 = this.DataContext.CreateEntity<UriContactEntity> ();
			uriPA2.NaturalPerson = personPA;
			uriPA2.Uri = "perre.arnaud@opac.ch";
			uriPA2.UriScheme = uriSchemeMailto;
			uriPA2.Roles.Add (rolePrive);

			personPA.BirthDate = new Common.Types.Date (day: 11, month: 2, year: 1972);
			personPA.Firstname = "Pierre";
			personPA.Lastname = "Arnaud";
			personPA.Title = titleMonsieur;
			personPA.Contacts.Add (mailPA1);
			personPA.Contacts.Add (mailPA2);
			personPA.Contacts.Add (telecomPA1);
			personPA.Contacts.Add (telecomPA2);
			personPA.Contacts.Add (telecomPA3);
			personPA.Contacts.Add (uriPA1);
			personPA.Contacts.Add (uriPA2);
			
			// personDR

			personDR.Firstname = "Daniel";
			personDR.Lastname  = "Roux";
			personDR.BirthDate = new Common.Types.Date (day: 31, month: 3, year: 1958);

			yield return personPA;
			yield return personDR;
			yield return companyEpsitec;
			yield return companyMigros;
		}

		private IEnumerable<RelationEntity> InsertRelationsInDatabase(IEnumerable<AbstractPersonEntity> persons)
		{
			int id = 1000;

			foreach (var person in persons)
			{
				RelationEntity relation = this.DataContext.CreateEntity<RelationEntity> ();

				relation.Id = (id++).ToString ();
				relation.Person = person;
				relation.FirstContactDate = Common.Types.Date.Today;

				yield return relation;
			}
		}
	}
}
