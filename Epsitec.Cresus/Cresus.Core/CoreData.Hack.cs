//	Copyright © 2008-2010, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;

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

		public IEnumerable<UnitOfMeasureEntity> GetUnitOfMeasure()
		{
			return new UnitOfMeasureRepository (this.DataContext).GetAllUnitOfMeasure ();
		}

		public IEnumerable<ArticleDefinitionEntity> GetArticleDefinitions()
		{
			return new ArticleDefinitionRepository (this.DataContext).GetAllArticleDefinitions ();
		}

		public IEnumerable<InvoiceDocumentEntity> GetInvoiceDocuments()
		{
			return new InvoiceDocumentRepository (this.DataContext).GetAllInvoiceDocuments ();
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
			UnitOfMeasureEntity[] units = this.InsertUnitOfMeasureInDatabase ().ToArray ();
			ArticleDefinitionEntity[] articleDefs = this.InsertArticleDefinitionsInDatabase (units).ToArray ();
			InvoiceDocumentEntity[] invoices = this.InsertInvoiceDocumentInDatabase (abstractPersons.Where (x => x.Contacts.Count > 0 && x.Contacts[0] is MailContactEntity).First ().Contacts[0] as MailContactEntity, articleDefs).ToArray ();

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

			for (int i = 0; i < CoreData.swissLocations.Length; i += 2)
			{
				LocationEntity location = this.DataContext.CreateEmptyEntity<LocationEntity> ();

				location.Country = swiss;
				location.PostalCode = CoreData.swissLocations[i + 0];
				location.Name = CoreData.swissLocations[i + 1];

				yield return location;
			}

			CountryEntity french = countries.First (c => c.Code == "FR");

			for (int i = 0; i < CoreData.frenchLocations.Length; i += 2)
			{
				LocationEntity location = this.DataContext.CreateEmptyEntity<LocationEntity> ();

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
				CaseEventTypeEntity eventType = this.DataContext.CreateEmptyEntity<CaseEventTypeEntity> ();

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
			LegalPersonEntity companyEpsitec = this.DataContext.CreateEmptyEntity<LegalPersonEntity> ();
			LegalPersonEntity companyMigros  = this.DataContext.CreateEmptyEntity<LegalPersonEntity> ();

			NaturalPersonEntity personPA = this.DataContext.CreateEmptyEntity<NaturalPersonEntity> ();
			NaturalPersonEntity personDR = this.DataContext.CreateEmptyEntity<NaturalPersonEntity> ();

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

			StreetEntity streetEpsitec = this.DataContext.CreateEmptyEntity<StreetEntity> ();
			streetEpsitec.StreetName = "Ch. du Fontenay 3";
			streetEpsitec.Complement = "2ème étage";

			PostBoxEntity postboxEpsitec = this.DataContext.CreateEmptyEntity<PostBoxEntity> ();
			postboxEpsitec.Number = "Case postale 1234";

			AddressEntity addressEpsitec = this.DataContext.CreateEmptyEntity<AddressEntity> ();
			addressEpsitec.Location = locationYverdon;
			addressEpsitec.Street = streetEpsitec;
			addressEpsitec.PostBox = postboxEpsitec;

			// addressPA

			StreetEntity streetPA = this.DataContext.CreateEmptyEntity<StreetEntity> ();
			streetPA.StreetName = "Ch. du Fontenay 6";

			AddressEntity addressPA = this.DataContext.CreateEmptyEntity<AddressEntity> ();
			addressPA.Location = locationYverdon;
			addressPA.Street = streetPA;

			// companyEpsitec

			CommentEntity commentEpsitec = this.DataContext.CreateEmptyEntity<CommentEntity> ();
			commentEpsitec.Text = "Bureaux ouverts de 9h-12h et 14h-16h30";

			MailContactEntity mailEpsitec1 = this.DataContext.CreateEmptyEntity<MailContactEntity> ();
			mailEpsitec1.LegalPerson = companyEpsitec;
			mailEpsitec1.Address = addressEpsitec;
			mailEpsitec1.Comments.Add (commentEpsitec);
			mailEpsitec1.Roles.Add (roleFact);

			CommentEntity commentEpsitecT1 = this.DataContext.CreateEmptyEntity<CommentEntity> ();
			commentEpsitecT1.Text = "Administration et vente";

			TelecomContactEntity telecomEpsitec1 = this.DataContext.CreateEmptyEntity<TelecomContactEntity> ();
			telecomEpsitec1.LegalPerson = companyEpsitec;
			telecomEpsitec1.TelecomType = telecomTypeFix;
			telecomEpsitec1.Number = "+41 848 27 37 87";
			telecomEpsitec1.Comments.Add (commentEpsitecT1);
			telecomEpsitec1.Roles.Add (roleProf);
			telecomEpsitec1.Roles.Add (roleFact);

			CommentEntity commentEpsitecT2 = this.DataContext.CreateEmptyEntity<CommentEntity> ();
			commentEpsitecT2.Text = "Assistance technique (hotline)";

			TelecomContactEntity telecomEpsitec2 = this.DataContext.CreateEmptyEntity<TelecomContactEntity> ();
			telecomEpsitec2.LegalPerson = companyEpsitec;
			telecomEpsitec2.TelecomType = telecomTypeFix;
			telecomEpsitec2.Number = "+41 848 27 37 89";
			telecomEpsitec2.Comments.Add (commentEpsitecT2);
			telecomEpsitec2.Roles.Add (roleProf);

			UriContactEntity uriEpsitec1 = this.DataContext.CreateEmptyEntity<UriContactEntity> ();
			uriEpsitec1.LegalPerson = companyEpsitec;
			uriEpsitec1.Uri = "epsitec@epsitec.ch";
			uriEpsitec1.UriScheme = uriSchemeMailto;
			uriEpsitec1.Roles.Add (roleProf);

			UriContactEntity uriEpsitec2 = this.DataContext.CreateEmptyEntity<UriContactEntity> ();
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

			MailContactEntity mailPA1 = this.DataContext.CreateEmptyEntity<MailContactEntity> ();
			mailPA1.NaturalPerson = personPA;
			mailPA1.LegalPerson = companyEpsitec;
			mailPA1.Address = addressEpsitec;
			mailPA1.Complement = "Direction";
			mailPA1.Roles.Add (roleProf);

			MailContactEntity mailPA2 = this.DataContext.CreateEmptyEntity<MailContactEntity> ();
			mailPA2.NaturalPerson = personPA;
			mailPA2.Address = addressPA;
			mailPA2.Roles.Add (rolePrive);

			TelecomContactEntity telecomPA1 = this.DataContext.CreateEmptyEntity<TelecomContactEntity> ();
			telecomPA1.NaturalPerson = personPA;
			telecomPA1.TelecomType = telecomTypeMobile;
			telecomPA1.Number = "+41 79 367 45 97";
			telecomPA1.Roles.Add (rolePrive);
			telecomPA1.Roles.Add (roleProf);

			TelecomContactEntity telecomPA2 = this.DataContext.CreateEmptyEntity<TelecomContactEntity> ();
			telecomPA2.NaturalPerson = personPA;
			telecomPA2.TelecomType = telecomTypeFix;
			telecomPA2.Number = "+41 24 425 08 09";
			telecomPA2.Roles.Add (roleProf);

			TelecomContactEntity telecomPA3 = this.DataContext.CreateEmptyEntity<TelecomContactEntity> ();
			telecomPA3.NaturalPerson = personPA;
			telecomPA3.TelecomType = telecomTypeFax;
			telecomPA3.Number = "+41 24 555 83 59";
			telecomPA3.Roles.Add (rolePrive);
			telecomPA3.Roles.Add (roleProf);

			UriContactEntity uriPA1 = this.DataContext.CreateEmptyEntity<UriContactEntity> ();
			uriPA1.NaturalPerson = personPA;
			uriPA1.Uri = "arnaud@epsitec.ch";
			uriPA1.UriScheme = uriSchemeMailto;
			uriPA1.Roles.Add (rolePrive);

			UriContactEntity uriPA2 = this.DataContext.CreateEmptyEntity<UriContactEntity> ();
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
				RelationEntity relation = this.DataContext.CreateEmptyEntity<RelationEntity> ();

				relation.IdA = (id++).ToString ();
				relation.Person = person;
				relation.FirstContactDate = Common.Types.Date.Today;

				yield return relation;
			}
		}

		private IEnumerable<UnitOfMeasureEntity> InsertUnitOfMeasureInDatabase()
		{
			var uomUnit1 = this.DataContext.CreateEmptyEntity<UnitOfMeasureEntity> ();
			var uomUnit2 = this.DataContext.CreateEmptyEntity<UnitOfMeasureEntity> ();
			var uomUnit3 = this.DataContext.CreateEmptyEntity<UnitOfMeasureEntity> ();

			uomUnit1.Code = "pce";
			uomUnit1.Name = "Pièce";
			uomUnit1.DivideRatio = 1;
			uomUnit1.MultiplyRatio = 1;
			uomUnit1.SmallestIncrement = 1;
			uomUnit1.Category = BusinessLogic.UnitOfMeasureCategory.Unit;

			uomUnit2.Code = "box";
			uomUnit2.Name = "Carton de 6";
			uomUnit2.DivideRatio = 1;
			uomUnit2.MultiplyRatio = 6;
			uomUnit2.SmallestIncrement = 1;
			uomUnit2.Category = BusinessLogic.UnitOfMeasureCategory.Unit;

			uomUnit3.Code = "x";
			uomUnit3.Name = "Fois";
			uomUnit3.DivideRatio = 1;
			uomUnit3.MultiplyRatio = 1;
			uomUnit3.SmallestIncrement = 1;
			uomUnit3.Category = BusinessLogic.UnitOfMeasureCategory.Unit;

			yield return uomUnit1;
			yield return uomUnit2;
			yield return uomUnit3;
		}

		private IEnumerable<ArticleDefinitionEntity> InsertArticleDefinitionsInDatabase(IEnumerable<UnitOfMeasureEntity> units)
		{
			var uomUnit1 = units.Where (x => x.Code == "pce").First ();
			var uomUnit2 = units.Where (x => x.Code == "box").First ();
			var uomUnit3 = units.Where (x => x.Code == "x").First ();

			var uomGroup1 = this.DataContext.CreateEmptyEntity<UnitOfMeasureGroupEntity> ();
			uomGroup1.Name = "Unités d'emballage soft/standard";
			uomGroup1.Description = "Unités d'emballage pour les logiciels Crésus standard";
			uomGroup1.Category = BusinessLogic.UnitOfMeasureCategory.Unit;
			uomGroup1.Units.Add (uomUnit1);
			uomGroup1.Units.Add (uomUnit2);

			var uomGroup2 = this.DataContext.CreateEmptyEntity<UnitOfMeasureGroupEntity> ();
			uomGroup2.Name = "Frais";
			uomGroup2.Description = "Frais";
			uomGroup2.Category = BusinessLogic.UnitOfMeasureCategory.Unit;
			uomGroup2.Units.Add (uomUnit3);

			var articleGroup1 = this.DataContext.CreateEmptyEntity<ArticleGroupEntity> ();

			articleGroup1.Rank = 0;
			articleGroup1.Code = "SOFT";
			articleGroup1.Name = "Logiciels";

			var accountingDef = this.DataContext.CreateEmptyEntity<ArticleAccountingDefinitionEntity> ();

			accountingDef.BeginDate = new System.DateTime (2010, 1, 1);
			accountingDef.EndDate   = new System.DateTime (2099, 1, 1);
			accountingDef.SellingBookAccount = "3200";
			accountingDef.SellingDiscountBookAccount = "3900";
			accountingDef.PurchaseBookAccount = "4200";
			accountingDef.PurchaseDiscountBookAccount = "4900";
			accountingDef.CurrencyCode = BusinessLogic.Finance.CurrencyCode.Chf;

			var priceRoundingMode = this.DataContext.CreateEmptyEntity<PriceRoundingModeEntity> ();

			priceRoundingMode.Name = "Arrondi à 5ct";
			priceRoundingMode.Modulo = 0.05M;
			priceRoundingMode.AddBeforeModulo = 0.025M;
			priceRoundingMode.PriceRoundingPolicy = BusinessLogic.Finance.RoundingPolicy.OnFinalPriceAfterTax;

			var articleCategory1 = this.DataContext.CreateEmptyEntity<ArticleCategoryEntity> ();

			articleCategory1.Name = "Logiciels Crésus";
			articleCategory1.DefaultVatCode = BusinessLogic.Finance.VatCode.StandardTax;
			articleCategory1.DefaultAccounting.Add (accountingDef);
			articleCategory1.DefaultRoundingMode = priceRoundingMode;

			var articleCategory2 = this.DataContext.CreateEmptyEntity<ArticleCategoryEntity> ();

			articleCategory2.Name = "Ports/emballages";
			articleCategory2.DefaultVatCode = BusinessLogic.Finance.VatCode.StandardTax;
			articleCategory2.NeverApplyDiscount = true;

			var articlePriceGroup1 = this.DataContext.CreateEmptyEntity<ArticlePriceGroupEntity> ();
			var articlePriceGroup2 = this.DataContext.CreateEmptyEntity<ArticlePriceGroupEntity> ();
			var articlePriceGroup3 = this.DataContext.CreateEmptyEntity<ArticlePriceGroupEntity> ();

			articlePriceGroup1.Code = "USER";
			articlePriceGroup1.Name = "Prix catalogue";

			articlePriceGroup2.Code = "RSLR";
			articlePriceGroup2.Name = "Revendeur agréé";
			articlePriceGroup2.MultiplyRatio = 0.70M;
			articlePriceGroup2.DivideRatio = 1.00M;

			articlePriceGroup3.Code = "CRTR";
			articlePriceGroup3.Name = "Revendeur certifié";
			articlePriceGroup3.MultiplyRatio = 0.70M;
			articlePriceGroup3.DivideRatio = 1.00M;

			var articleDef1 = this.DataContext.CreateEmptyEntity<ArticleDefinitionEntity> ();
			var articleDef2 = this.DataContext.CreateEmptyEntity<ArticleDefinitionEntity> ();
			var articleDef3 = this.DataContext.CreateEmptyEntity<ArticleDefinitionEntity> ();
			var articleDef4 = this.DataContext.CreateEmptyEntity<ArticleDefinitionEntity> ();

			articleDef1.IdA = "CR-CP";
			articleDef1.ShortDescription = "Crésus Comptabilité PRO";
			articleDef1.LongDescription  = "Crésus Comptabilité PRO<br/>Logiciel de comptabilité pour PME, artisans et indépendants.<br/>Jusqu'à 64'000 écritures par année.";
			articleDef1.ArticleGroups.Add (articleGroup1);
			articleDef1.ArticleCategory = articleCategory1;
			articleDef1.BillingUnit = uomUnit1;
			articleDef1.Units = uomGroup1;
			articleDef1.ArticlePrices.Add (this.CreateArticlePrice (446.10M, articlePriceGroup1, articlePriceGroup2, articlePriceGroup3));

			articleDef2.IdA = "CR-SP";
			articleDef2.ShortDescription = "Crésus Salaires PRO";
			articleDef2.LongDescription  = "Crésus Salaires PRO<br/>Logiciel de comptabilité salariale.<br/>Jusqu'à 20 salaires par mois.";
			articleDef2.ArticleGroups.Add (articleGroup1);
			articleDef2.ArticleCategory = articleCategory1;
			articleDef2.BillingUnit = uomUnit1;
			articleDef2.Units = uomGroup1;
			articleDef2.ArticlePrices.Add (this.CreateArticlePrice (446.10M, articlePriceGroup1, articlePriceGroup2, articlePriceGroup3));

			articleDef3.IdA = "CR-FL";
			articleDef3.ShortDescription = "Crésus Facturation LARGO";
			articleDef3.LongDescription  = "Crésus Facturation LARGO<br/>Logiciel de facturation avec gestion des débiteurs et des créanciers.<br/><br/>Quelques textes pour faire long :<br/>Lundi<br/>Mardi<br/>Mercredi<br/>Jeudi<br/>Vendredi<br/>Samedi<br/>Dimanche";
			articleDef3.ArticleGroups.Add (articleGroup1);
			articleDef3.ArticleCategory = articleCategory1;
			articleDef3.BillingUnit = uomUnit1;
			articleDef3.Units = uomGroup1;
			articleDef3.ArticlePrices.Add (this.CreateArticlePrice (892.20M, articlePriceGroup1, articlePriceGroup2, articlePriceGroup3));

			articleDef4.IdA = "EMB";
			articleDef4.ShortDescription = "Port et emballage";
			articleDef4.ArticleCategory = articleCategory2;
			articleDef4.BillingUnit = uomUnit3;
			articleDef4.Units = uomGroup2;
			articleDef4.ArticlePrices.Add (this.CreateArticlePrice (11.15M));

			yield return articleDef1;
			yield return articleDef2;
			yield return articleDef3;
			yield return articleDef4;
		}

		private IEnumerable<InvoiceDocumentEntity> InsertInvoiceDocumentInDatabase(MailContactEntity billingAddress, ArticleDefinitionEntity[] articleDefs)
		{
			var decimalType = DecimalType.Default;
			decimal vatRate = 0.076M;

			var billingA = this.DataContext.CreateEmptyEntity<BillingDetailsEntity> ();
			var invoiceA = this.DataContext.CreateEmptyEntity<InvoiceDocumentEntity> ();

			invoiceA.IdA = "1000";
			invoiceA.DocumentSource = BusinessLogic.DocumentSource.Generated;
			invoiceA.Description = "Facture de test #1000";
			invoiceA.CreationDate = new System.DateTime (2010, 7, 8);
			invoiceA.LastModificationDate = System.DateTime.Now;
			invoiceA.BillingMailContact = billingAddress;
			invoiceA.ShippingMailContact = billingAddress;
			invoiceA.OtherPartyBillingMode = BusinessLogic.Finance.BillingMode.IncludingTax;
			invoiceA.OtherPartyTaxMode = BusinessLogic.Finance.TaxMode.LiableForVat;
			invoiceA.CurrencyCode = BusinessLogic.Finance.CurrencyCode.Chf;
			invoiceA.BillingStatus = BusinessLogic.Finance.BillingStatus.DebtorBillOpen;
			invoiceA.BillingDetails.Add (billingA);
			invoiceA.DebtorBookAccount = "1100";

			var textA1 = this.DataContext.CreateEmptyEntity<TextDocumentItemEntity> ();

			textA1.Visibility = true;
			textA1.Text = "Logiciels";

			var lineA1 = this.DataContext.CreateEmptyEntity<ArticleDocumentItemEntity> ();
			var quantityA1 = this.DataContext.CreateEmptyEntity<ArticleQuantityEntity> ();

			lineA1.Visibility = true;
			lineA1.IndentationLevel = 0;
			lineA1.BeginDate = invoiceA.CreationDate;
			lineA1.EndDate = invoiceA.CreationDate;
			lineA1.ArticleDefinition = articleDefs.Where (x => x.IdA == "CR-CP").FirstOrDefault ();
			lineA1.VatCode = BusinessLogic.Finance.VatCode.StandardTaxOnTurnover;
			lineA1.PrimaryUnitPriceBeforeTax = lineA1.ArticleDefinition.ArticlePrices[0].ValueBeforeTax;
			lineA1.PrimaryLinePriceBeforeTax = lineA1.PrimaryUnitPriceBeforeTax * 3;
			lineA1.NeverApplyDiscount = false;
			lineA1.ResultingLinePriceBeforeTax = (int) lineA1.PrimaryLinePriceBeforeTax;
			lineA1.ResultingLineTax = lineA1.ResultingLinePriceBeforeTax * vatRate;
			lineA1.ArticleShortDescriptionCache = lineA1.ArticleDefinition.ShortDescription;
			lineA1.ArticleLongDescriptionCache = lineA1.ArticleDefinition.LongDescription;
			
			quantityA1.Code = "livré";
			quantityA1.Quantity = 3;
			quantityA1.Unit = lineA1.ArticleDefinition.BillingUnit;
			quantityA1.ExpectedDate = new Date (2010, 7, 8);

			lineA1.ArticleQuantities.Add (quantityA1);

			var lineA2 = this.DataContext.CreateEmptyEntity<ArticleDocumentItemEntity> ();
			var quantityA2_1 = this.DataContext.CreateEmptyEntity<ArticleQuantityEntity> ();
			var quantityA2_2 = this.DataContext.CreateEmptyEntity<ArticleQuantityEntity> ();

			lineA2.Visibility = true;
			lineA2.IndentationLevel = 0;
			lineA2.BeginDate = invoiceA.CreationDate;
			lineA2.EndDate = invoiceA.CreationDate;
			lineA2.ArticleDefinition = articleDefs.Where (x => x.IdA == "CR-FL").FirstOrDefault ();
			lineA2.VatCode = BusinessLogic.Finance.VatCode.StandardTaxOnTurnover;
			lineA2.PrimaryUnitPriceBeforeTax = lineA2.ArticleDefinition.ArticlePrices[0].ValueBeforeTax;
			lineA2.PrimaryLinePriceBeforeTax = lineA2.PrimaryUnitPriceBeforeTax;
			lineA2.NeverApplyDiscount = false;
			lineA2.ResultingLinePriceBeforeTax = (int) lineA2.PrimaryLinePriceBeforeTax;
			lineA2.ResultingLineTax = lineA2.ResultingLinePriceBeforeTax * vatRate;
			lineA2.ArticleShortDescriptionCache = lineA2.ArticleDefinition.ShortDescription;
			lineA2.ArticleLongDescriptionCache = lineA2.ArticleDefinition.LongDescription;

			quantityA2_1.Code = "livré";
			quantityA2_1.Quantity = 1;
			quantityA2_1.Unit = lineA2.ArticleDefinition.BillingUnit;
			quantityA2_1.ExpectedDate = new Date (2010, 7, 8);

			quantityA2_2.Code = "suivra";
			quantityA2_2.Quantity = 1;
			quantityA2_2.Unit = lineA2.ArticleDefinition.BillingUnit;
			quantityA2_2.ExpectedDate = new Date (2010, 7, 19);

			lineA2.ArticleQuantities.Add (quantityA2_1);
			lineA2.ArticleQuantities.Add (quantityA2_2);

			var lineA3 = this.DataContext.CreateEmptyEntity<ArticleDocumentItemEntity> ();
			var quantityA3_1 = this.DataContext.CreateEmptyEntity<ArticleQuantityEntity> ();
			var quantityA3_2 = this.DataContext.CreateEmptyEntity<ArticleQuantityEntity> ();

			lineA3.Visibility = true;
			lineA3.IndentationLevel = 0;
			lineA3.BeginDate = invoiceA.CreationDate;
			lineA3.EndDate = invoiceA.CreationDate;
			lineA3.ArticleDefinition = articleDefs.Where (x => x.IdA == "CR-SP").FirstOrDefault ();
			lineA3.VatCode = BusinessLogic.Finance.VatCode.StandardTaxOnTurnover;
			lineA3.PrimaryUnitPriceBeforeTax = lineA3.ArticleDefinition.ArticlePrices[0].ValueBeforeTax;
			lineA3.PrimaryLinePriceBeforeTax = lineA3.PrimaryUnitPriceBeforeTax * 0;
			lineA3.NeverApplyDiscount = false;
			lineA3.ResultingLinePriceBeforeTax = (int) lineA3.PrimaryLinePriceBeforeTax;
			lineA3.ResultingLineTax = lineA3.ResultingLinePriceBeforeTax * vatRate;
			lineA3.ArticleShortDescriptionCache = lineA3.ArticleDefinition.ShortDescription;
			lineA3.ArticleLongDescriptionCache = lineA3.ArticleDefinition.LongDescription;

			quantityA3_1.Code = "suivra";
			quantityA3_1.Quantity = 2;
			quantityA3_1.Unit = lineA3.ArticleDefinition.BillingUnit;
			quantityA3_1.ExpectedDate = new Date (2010, 9, 1);

			quantityA3_2.Code = "suivra";
			quantityA3_2.Quantity = 1;
			quantityA3_2.Unit = lineA3.ArticleDefinition.BillingUnit;
			quantityA3_2.ExpectedDate = new Date (2011, 1, 31);

			lineA3.ArticleQuantities.Add (quantityA3_1);
			lineA3.ArticleQuantities.Add (quantityA3_2);

			var lineA4 = this.DataContext.CreateEmptyEntity<ArticleDocumentItemEntity> ();
			var quantityA4 = this.DataContext.CreateEmptyEntity<ArticleQuantityEntity> ();

			lineA4.Visibility = true;
			lineA4.IndentationLevel = 0;
			lineA4.BeginDate = invoiceA.CreationDate;
			lineA4.EndDate = invoiceA.CreationDate;
			lineA4.ArticleDefinition = articleDefs.Where (x => x.IdA == "EMB").FirstOrDefault ();
			lineA4.VatCode = BusinessLogic.Finance.VatCode.StandardTaxOnTurnover;
			lineA4.PrimaryUnitPriceBeforeTax = lineA4.ArticleDefinition.ArticlePrices[0].ValueBeforeTax;
			lineA4.PrimaryLinePriceBeforeTax = lineA4.PrimaryUnitPriceBeforeTax;
			lineA4.NeverApplyDiscount = true;
			lineA4.ResultingLinePriceBeforeTax = (int) lineA4.PrimaryUnitPriceBeforeTax;
			lineA4.ResultingLineTax = lineA4.ResultingLinePriceBeforeTax * vatRate;
			lineA4.ArticleShortDescriptionCache = lineA4.ArticleDefinition.ShortDescription;
			lineA4.ArticleLongDescriptionCache = lineA4.ArticleDefinition.LongDescription;

			quantityA4.Code = "livré";
			quantityA4.Quantity = 1;
			quantityA4.Unit = lineA4.ArticleDefinition.BillingUnit;
			quantityA4.ExpectedDate = new Date (2010, 7, 8);

			lineA4.ArticleQuantities.Add (quantityA4);

			var discountA1 = this.DataContext.CreateEmptyEntity<DiscountEntity> ();

			discountA1.Description = "Rabais de quantité";
			discountA1.DiscountRate = 0.20M;

			var totalA1 = this.DataContext.CreateEmptyEntity<PriceDocumentItemEntity> ();

			//	Total avec rabais; devrait s'imprimer ainsi :
			//
			//	Total avant rabais ............................. xxxx
			//	Rabais de quantité ............................... xx
			//	Total après rabais ............................. xxxx

			//	Voici comment ça fonctionne :

			//	1. On calcule le total HT et le total TVA en entrée (résultat par ex. des lignes
			//	   précédentes) --> PrimaryPriceBeforeTax / PrimaryTax
			//
			//	2. On applique un rabais, un arrondi ou un prix arrêté (FixedPrice) qui peut être
			//	   soit HT, soit TTC (en règle générale, c'est un prix TTC que l'on arrête)
			//
			//	3. Cela donne le prix HT et la TVA résultants --> ResultingPriceBeforeTax /
			//	   ResultingTax.
			//
			//	Plus tard, quand on a fini la facture, on doit appliquer les rabais à l'envers
			//	en remontant du pied de la facture pour arriver aux articles, afin de savoir à
			//	quel prix réel chaque article a été vendu; ces infos de 'remontée' sont alors
			//	stockées dans FinalPriceBeforeTax et FinalTax et servent à la comptabilisation
			//	uniquement, mais pas à l'impression d'une facture.

			totalA1.Visibility = true;
			totalA1.Discount = discountA1;
			totalA1.PrimaryPriceBeforeTax = (lineA1.ResultingLinePriceBeforeTax + lineA2.ResultingLinePriceBeforeTax + lineA3.ResultingLinePriceBeforeTax);
			totalA1.PrimaryTax = totalA1.PrimaryPriceBeforeTax * vatRate;
			totalA1.ResultingPriceBeforeTax = totalA1.PrimaryPriceBeforeTax * (1.0M - discountA1.DiscountRate);
			totalA1.ResultingTax = totalA1.FinalPriceBeforeTax * vatRate;
			totalA1.TextForPrimaryPrice = "Total avant rabais";
			totalA1.TextForResultingPrice = "Total après rabais";
			totalA1.DisplayModes = BusinessLogic.Finance.PriceDisplayModes.PrimaryTotal | BusinessLogic.Finance.PriceDisplayModes.Discount | BusinessLogic.Finance.PriceDisplayModes.ResultingTotal;

			var totalA2 = this.DataContext.CreateEmptyEntity<PriceDocumentItemEntity> ();

			//	TVA ............................................ xx
			//	Total arrêté ................................. xxxx

			totalA2.Visibility = true;
			totalA2.PrimaryPriceBeforeTax = totalA1.ResultingPriceBeforeTax + lineA4.ResultingLinePriceBeforeTax;
			totalA2.PrimaryTax = totalA1.ResultingTax + lineA4.PrimaryLinePriceBeforeTax;
			totalA2.FixedPriceAfterTax = (int) (totalA2.PrimaryPriceBeforeTax * (1+vatRate) / 10) * 10M;
			totalA2.ResultingPriceBeforeTax = decimalType.Range.ConstrainToZero (totalA2.FixedPriceAfterTax / (1 + vatRate));
			totalA2.ResultingTax = decimalType.Range.ConstrainToZero (totalA2.ResultingPriceBeforeTax * vatRate);
			totalA2.TextForFixedPrice = "Total arrêté";
			totalA2.TextForTax = "TVA";

			//	Le total arrêté force un rabais supplémentaire qu'il faut remonter dans les
			//	lignes d'articles qui peuvent être soumis à un rabais :

			totalA2.FinalPriceBeforeTax = totalA2.ResultingPriceBeforeTax;
			totalA2.FinalTax = decimalType.Range.ConstrainToZero (totalA2.FinalPriceBeforeTax * vatRate);

			decimal? fixedPriceDiscount = (totalA2.FinalPriceBeforeTax - lineA4.PrimaryLinePriceBeforeTax) / totalA1.ResultingPriceBeforeTax.Value;

			totalA1.FinalPriceBeforeTax = decimalType.Range.ConstrainToZero (totalA1.ResultingPriceBeforeTax * fixedPriceDiscount);
			totalA1.FinalTax = decimalType.Range.ConstrainToZero (totalA1.FinalPriceBeforeTax * vatRate);

			lineA1.FinalLinePriceBeforeTax = decimalType.Range.ConstrainToZero (lineA1.ResultingLinePriceBeforeTax * fixedPriceDiscount);
			lineA1.FinalLineTax = decimalType.Range.ConstrainToZero (lineA1.ResultingLineTax * fixedPriceDiscount);
			lineA2.FinalLinePriceBeforeTax = decimalType.Range.ConstrainToZero (lineA2.ResultingLinePriceBeforeTax * fixedPriceDiscount);
			lineA2.FinalLineTax = decimalType.Range.ConstrainToZero (lineA2.ResultingLineTax * fixedPriceDiscount);
			lineA3.FinalLinePriceBeforeTax = decimalType.Range.ConstrainToZero (lineA3.ResultingLinePriceBeforeTax * fixedPriceDiscount);
			lineA3.FinalLineTax = decimalType.Range.ConstrainToZero (lineA3.ResultingLineTax * fixedPriceDiscount);
			lineA4.FinalLinePriceBeforeTax = lineA4.ResultingLinePriceBeforeTax;
			lineA4.FinalLineTax = lineA4.ResultingLineTax;

			invoiceA.Lines.Add (textA1);		//	Logiciels
			invoiceA.Lines.Add (lineA1);		//	  Crésus Compta PRO x 3
			invoiceA.Lines.Add (lineA2);		//	  Crésus Facturation LARGO x 1 (et 1 qui sera livré le 19/07/2010)
			invoiceA.Lines.Add (lineA3);		//	  Crésus Salaires PRO x 0 (et 2+1 qui seront livrés les 01/09/2010 et 31.01.2011)
			invoiceA.Lines.Add (totalA1);		//	Rabais de quantité et sous-total après rabais
			invoiceA.Lines.Add (lineA4);		//	  Frais de port
			invoiceA.Lines.Add (totalA2);		//	Total arrêté à 1790.00

			var paymentMode = this.DataContext.CreateEmptyEntity<PaymentModeEntity> ();

			paymentMode.Code = "BILL";
			paymentMode.Name = "BVR à 30 jours net";
			paymentMode.Description = "Facture payable au moyen du bulletin de versement ci-joint.<br/>Conditions: 30 jours net.";
			paymentMode.BookAccount = "1010";
			paymentMode.StandardPaymentTerm = 30;

			var paymentA = this.DataContext.CreateEmptyEntity<PaymentDetailEntity> ();

			paymentA.PaymentType = BusinessLogic.Finance.PaymentDetailType.AmountDue;
			paymentA.PaymentMode = paymentMode;
			paymentA.Amount = (totalA2.FinalPriceBeforeTax + totalA2.FinalTax).Value;
			paymentA.Date = new Date (2010, 08, 06);

			billingA.Title = "Votre commande du 5 juillet 2010<br/>S/notre directeur M. P. Arnaud";
			billingA.AmountDue = paymentA;
			billingA.EsrCustomerNumber = "01-69444-3";										//	compte BVR
			billingA.EsrReferenceNumber = "96 13070 01000 02173 50356 73892";				//	n° de réf BVR lié

			yield return invoiceA;
		}

		private ArticlePriceEntity CreateArticlePrice(decimal price, ArticlePriceGroupEntity articlePriceGroup1 = null, ArticlePriceGroupEntity articlePriceGroup2 = null, ArticlePriceGroupEntity articlePriceGroup3 = null)
		{
			var articlePrice1 = this.DataContext.CreateEmptyEntity<ArticlePriceEntity> ();

			articlePrice1.BeginDate = new System.DateTime (2010,  1,  1,  0,  0,  0);  // 1 janvier 00:00:00
			articlePrice1.EndDate   = new System.DateTime (2020, 12, 31, 23, 59, 59);  // 31 décembre 23:59:59 (c'est important de donner l'heure)
			articlePrice1.MinQuantity = 1;
			articlePrice1.MaxQuantity = null;
			articlePrice1.CurrencyCode = BusinessLogic.Finance.CurrencyCode.Chf;
			articlePrice1.ValueBeforeTax = price;
			if (articlePriceGroup1 != null) articlePrice1.PriceGroups.Add (articlePriceGroup1);
			if (articlePriceGroup2 != null) articlePrice1.PriceGroups.Add (articlePriceGroup2);
			if (articlePriceGroup3 != null) articlePrice1.PriceGroups.Add (articlePriceGroup3);
			return articlePrice1;
		}
	}
}
