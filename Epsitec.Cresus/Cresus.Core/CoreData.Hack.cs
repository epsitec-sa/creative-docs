﻿//	Copyright © 2008-2010, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;

using Epsitec.Cresus.Core;
using Epsitec.Cresus.Core.Repositories;
using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Database;
using Epsitec.Cresus.DataLayer;

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.DataLayer.Context;
using Epsitec.Cresus.Core.Business;

namespace Epsitec.Cresus.Core
{
	public sealed partial class CoreData
	{
		public IEnumerable<RelationEntity> GetCustomers(AbstractPersonEntity person)
		{
			var repository = new RelationRepository (this);
			var example = repository.CreateExample ();
			example.Person = person;
			return repository.GetByExample (example);
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
			RelationEntity[] relations = this.InsertRelationsInDatabase (abstractPersons).ToArray ();
			UnitOfMeasureEntity[] units = this.InsertUnitsOfMeasureInDatabase ().ToArray ();
			ArticleDefinitionEntity[] articleDefs = this.InsertArticleDefinitionsInDatabase (units).ToArray ();
			PaymentModeEntity[] paymentDefs = this.InsertPaymentModesInDatabase ().ToArray ();
			CurrencyEntity[] currencyDefs = this.InsertCurrenciesInDatabase ().ToArray ();
			VatDefinitionEntity[] vatDefs = this.InsertVatDefinitionsInDatabase ().ToArray ();
			BusinessSettingsEntity[] settings = this.InsertBusinessSettingsInDatabase ().ToArray ();
			BusinessDocumentEntity[] invoices = this.InsertInvoiceDocumentsInDatabase (abstractPersons.Where (x => x.Contacts.Count > 0 && x.Contacts[0] is MailContactEntity).First ().Contacts[0] as MailContactEntity, paymentDefs, currencyDefs, articleDefs, vatDefs, settings).ToArray ();
			var workflowDefinitions = this.InsertWorkflowDefinitionsInDatabase ().ToArray ();
			
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
				location.Name       = CoreData.swissLocations[i + 1];

				yield return location;
			}

			CountryEntity french = countries.First (c => c.Code == "FR");

			for (int i = 0; i < CoreData.frenchLocations.Length; i += 2)
			{
				LocationEntity location = this.DataContext.CreateEntity<LocationEntity> ();

				location.Country = french;
				location.PostalCode = CoreData.frenchLocations[i + 0];
				location.Name       = CoreData.frenchLocations[i + 1];

				yield return location;
			}
		}

		private IEnumerable<ContactRoleEntity> InsertContactRolesInDatabase()
		{
			string[] names = new string[] { "Professionnel", "Commande", "Livraison", "Facturation", "Privé" };
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

			ContactRoleEntity roleFact  = roles.Where (x => x.Name == "Facturation").First ();
			ContactRoleEntity roleProf  = roles.Where (x => x.Name == "Professionnel").First ();
			ContactRoleEntity rolePrive = roles.Where (x => x.Name == "Privé").First ();

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
			personDR.Lastname  = FormattedText.FromSimpleText ("Roux");
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

				relation.IdA = (id++).ToString ();
				relation.Person = person;
				relation.FirstContactDate = Common.Types.Date.Today;

				yield return relation;
			}
		}

		private IEnumerable<UnitOfMeasureEntity> InsertUnitsOfMeasureInDatabase()
		{
			var uomUnit1 = this.DataContext.CreateEntity<UnitOfMeasureEntity> ();
			var uomUnit2 = this.DataContext.CreateEntity<UnitOfMeasureEntity> ();
			var uomUnit3 = this.DataContext.CreateEntity<UnitOfMeasureEntity> ();
			var uomUnit4 = this.DataContext.CreateEntity<UnitOfMeasureEntity> ();
			var uomUnit5 = this.DataContext.CreateEntity<UnitOfMeasureEntity> ();

			uomUnit1.Code = "pce";
			uomUnit1.Name = "Pièce";
			uomUnit1.DivideRatio = 1;
			uomUnit1.MultiplyRatio = 1;
			uomUnit1.SmallestIncrement = 1;
			uomUnit1.Category = Business.UnitOfMeasureCategory.Unit;

			uomUnit2.Code = "box";
			uomUnit2.Name = "Carton de 6";
			uomUnit2.DivideRatio = 1;
			uomUnit2.MultiplyRatio = 6;
			uomUnit2.SmallestIncrement = 1;
			uomUnit2.Category = Business.UnitOfMeasureCategory.Unit;

			uomUnit3.Code = "×";  // caractère Unicode 00D7
			uomUnit3.Name = "Fois";
			uomUnit3.DivideRatio = 1;
			uomUnit3.MultiplyRatio = 1;
			uomUnit3.SmallestIncrement = 1;
			uomUnit3.Category = Business.UnitOfMeasureCategory.Unrelated;

			uomUnit4.Code = "mm";
			uomUnit4.Name = "Millimètre";
			uomUnit4.DivideRatio = 1;
			uomUnit4.MultiplyRatio = 1;
			uomUnit4.SmallestIncrement = 1;
			uomUnit4.Category = Business.UnitOfMeasureCategory.Length;

			uomUnit5.Code = "l";
			uomUnit5.Name = "Litre";
			uomUnit5.DivideRatio = 1;
			uomUnit5.MultiplyRatio = 1;
			uomUnit5.SmallestIncrement = 1;
			uomUnit5.Category = Business.UnitOfMeasureCategory.Volume;

			yield return uomUnit1;
			yield return uomUnit2;
			yield return uomUnit3;
			yield return uomUnit4;
			yield return uomUnit5;
		}

		private IEnumerable<ArticleDefinitionEntity> InsertArticleDefinitionsInDatabase(IEnumerable<UnitOfMeasureEntity> units)
		{
			var uomUnit1 = units.Where (x => x.Code == "pce").First ();
			var uomUnit2 = units.Where (x => x.Code == "box").First ();
			var uomUnit3 = units.Where (x => x.Code == "×").First ();
			var uomUnit4 = units.Where (x => x.Code == "l").First ();

			var uomUnitMm = units.Where (x => x.Code == "mm").First ();

			var uomGroup1 = this.DataContext.CreateEntity<UnitOfMeasureGroupEntity> ();
			uomGroup1.Name = "Unités d'emballage soft/standard";
			uomGroup1.Description = "Unités d'emballage pour les logiciels Crésus standard";
			uomGroup1.Category = Business.UnitOfMeasureCategory.Unit;
			uomGroup1.Units.Add (uomUnit1);
			uomGroup1.Units.Add (uomUnit2);

			var uomGroup2 = this.DataContext.CreateEntity<UnitOfMeasureGroupEntity> ();
			uomGroup2.Name = "Unités pour boissons";
			uomGroup2.Description = "Unités pour les boissons";
			uomGroup2.Category = Business.UnitOfMeasureCategory.Unit;
			uomGroup2.Units.Add (uomUnit2);
			uomGroup2.Units.Add (uomUnit4);

			var uomGroup3 = this.DataContext.CreateEntity<UnitOfMeasureGroupEntity> ();
			uomGroup3.Name = "Frais";
			uomGroup3.Description = "Frais";
			uomGroup3.Category = Business.UnitOfMeasureCategory.Unrelated;
			uomGroup3.Units.Add (uomUnit3);

			var articleGroup1 = this.DataContext.CreateEntity<ArticleGroupEntity> ();

			articleGroup1.Rank = 0;
			articleGroup1.Code = "SOFT";
			articleGroup1.Name = "Logiciels";

			var articleGroup2 = this.DataContext.CreateEntity<ArticleGroupEntity> ();

			articleGroup2.Rank = 1;
			articleGroup2.Code = "MAT";
			articleGroup2.Name = "Matériel";

			var accountingDef = this.DataContext.CreateEntity<ArticleAccountingDefinitionEntity> ();

			accountingDef.BeginDate = new System.DateTime (2010, 1, 1);
			accountingDef.EndDate   = new System.DateTime (2099, 1, 1);
			accountingDef.SellingBookAccount = "3200";
			accountingDef.SellingDiscountBookAccount = "3900";
			accountingDef.PurchaseBookAccount = "4200";
			accountingDef.PurchaseDiscountBookAccount = "4900";
			accountingDef.CurrencyCode = Business.Finance.CurrencyCode.Chf;

			var priceRoundingMode = this.DataContext.CreateEntity<PriceRoundingModeEntity> ();

			priceRoundingMode.Name = "Arrondi à 5ct";
			priceRoundingMode.Modulo = 0.05M;
			priceRoundingMode.AddBeforeModulo = 0.025M;
			priceRoundingMode.PriceRoundingPolicy = Business.Finance.RoundingPolicy.OnFinalPriceAfterTax;

			var articleCategory1 = this.DataContext.CreateEntity<ArticleCategoryEntity> ();

			articleCategory1.Name = "Logiciels Crésus";
			articleCategory1.DefaultOutputVatCode = Business.Finance.VatCode.StandardTax;
			articleCategory1.DefaultAccounting.Add (accountingDef);
			articleCategory1.DefaultRoundingMode = priceRoundingMode;
			articleCategory1.ArticleType = Business.ArticleType.Goods;

			var articleCategory2 = this.DataContext.CreateEntity<ArticleCategoryEntity> ();

			articleCategory2.Name = "Ports/emballages";
			articleCategory2.DefaultOutputVatCode = Business.Finance.VatCode.StandardTax;
			articleCategory2.NeverApplyDiscount = true;
			articleCategory2.ArticleType = Business.ArticleType.Freight;

			var articleCategory3 = this.DataContext.CreateEntity<ArticleCategoryEntity> ();

			articleCategory3.Name = "Fenêtres";
			articleCategory3.DefaultOutputVatCode = Business.Finance.VatCode.StandardTax;
			articleCategory3.ArticleType = Business.ArticleType.Goods;
			articleCategory3.DefaultAccounting.Add (accountingDef);

			var articleCategory4 = this.DataContext.CreateEntity<ArticleCategoryEntity> ();

			articleCategory4.Name = "Aliments";
			articleCategory4.DefaultOutputVatCode = Business.Finance.VatCode.ReducedTax;
			articleCategory4.ArticleType = Business.ArticleType.Goods;
			articleCategory4.DefaultAccounting.Add (accountingDef);

			var articlePriceGroup1 = this.DataContext.CreateEntity<ArticlePriceGroupEntity> ();
			var articlePriceGroup2 = this.DataContext.CreateEntity<ArticlePriceGroupEntity> ();
			var articlePriceGroup3 = this.DataContext.CreateEntity<ArticlePriceGroupEntity> ();

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

			var articleDef1 = this.DataContext.CreateEntity<ArticleDefinitionEntity> ();
			var articleDef2 = this.DataContext.CreateEntity<ArticleDefinitionEntity> ();
			var articleDef3 = this.DataContext.CreateEntity<ArticleDefinitionEntity> ();
			var articleDef4 = this.DataContext.CreateEntity<ArticleDefinitionEntity> ();
			var articleDef5 = this.DataContext.CreateEntity<ArticleDefinitionEntity> ();
			var articleDef6 = this.DataContext.CreateEntity<ArticleDefinitionEntity> ();

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
			articleDef4.Units = uomGroup3;
			articleDef4.ArticlePrices.Add (this.CreateArticlePrice (11.15M));

			var param5_1 = this.DataContext.CreateEntity<NumericValueArticleParameterDefinitionEntity> ();
			var param5_2 = this.DataContext.CreateEntity<NumericValueArticleParameterDefinitionEntity> ();
			var param5_3 = this.DataContext.CreateEntity<EnumValueArticleParameterDefinitionEntity> ();

			param5_1.Code = "H";
			param5_1.Name = "Hauteur";
			param5_1.Rank = 0;
			param5_1.Modulo = 5;
			param5_1.UnitOfMeasure = uomUnitMm;
			param5_1.DefaultValue = 1200;
			param5_1.MinValue = 400;
			param5_1.MaxValue = 3000;
			param5_1.PreferredValues = AbstractArticleParameterDefinitionEntity.Join ("800", "900", "1000", "1200", "1600", "1800", "2000");

			param5_2.Code = "L";
			param5_2.Name = "Largeur";
			param5_2.Rank = 1;
			param5_2.Modulo = 5;
			param5_2.UnitOfMeasure = uomUnitMm;
			param5_2.DefaultValue = 500;
			param5_2.MinValue = 200;
			param5_2.MaxValue = 1000;
			param5_2.PreferredValues = AbstractArticleParameterDefinitionEntity.Join ("400", "500", "800");

			param5_3.Code = "TYPVER";
			param5_3.Name = "Type de verre";
			param5_3.Rank = 2;
			param5_3.DefaultValue = "STD";
			param5_3.Cardinality = Business.EnumValueCardinality.ExactlyOne;
			param5_3.Values = AbstractArticleParameterDefinitionEntity.Join ("STD", "UV-1", "UV-2");
			param5_3.ShortDescriptions = AbstractArticleParameterDefinitionEntity.Join ("Standard", "Anti-UV 1", "Anti-UV 2");
			param5_3.LongDescriptions = AbstractArticleParameterDefinitionEntity.Join ("Verre standard", "Verre anti-UV avec protection à 100%", "Verre anti-UV avec protection à 100%, incluant un laminage anti-reflets");


			articleDef5.IdA = "WDO-DESIGN";
			articleDef5.ShortDescription = "Fenêtre Design";
			articleDef5.LongDescription  = "Fenêtre Design pour tout type de façades, produite à la main par des artisans de la région";
			articleDef5.ArticleGroups.Add (articleGroup2);
			articleDef5.ArticleCategory = articleCategory3;
			articleDef5.BillingUnit = uomUnit1;
			articleDef5.Units = uomGroup1;
			articleDef5.ArticlePrices.Add (this.CreateArticlePrice (2000, articlePriceGroup1, articlePriceGroup2, articlePriceGroup3));
			articleDef5.ArticleParameterDefinitions.Add (param5_1);
			articleDef5.ArticleParameterDefinitions.Add (param5_2);
			articleDef5.ArticleParameterDefinitions.Add (param5_3);

			articleDef6.IdA = "FOOD-MILCH";
			articleDef6.ShortDescription = "Lait";
			articleDef6.LongDescription  = "Lait pasteurisé";
			articleDef6.ArticleGroups.Add (articleGroup2);
			articleDef6.ArticleCategory = articleCategory4;
			articleDef6.BillingUnit = uomUnit4;
			articleDef6.Units = uomGroup2;
			articleDef6.ArticlePrices.Add (this.CreateArticlePrice (1.50M, articlePriceGroup1));

			yield return articleDef1;
			yield return articleDef2;
			yield return articleDef3;
			yield return articleDef4;
			yield return articleDef5;
			yield return articleDef6;
		}

		private IEnumerable<PaymentModeEntity> InsertPaymentModesInDatabase()
		{
			var paymentMode1 = this.DataContext.CreateEntity<PaymentModeEntity> ();
			var paymentMode2 = this.DataContext.CreateEntity<PaymentModeEntity> ();
			var paymentMode3 = this.DataContext.CreateEntity<PaymentModeEntity> ();
			var paymentMode4 = this.DataContext.CreateEntity<PaymentModeEntity> ();
			var paymentMode5 = this.DataContext.CreateEntity<PaymentModeEntity> ();

			paymentMode1.Rank = 0;
			paymentMode1.Code = "BILL10";
			paymentMode1.Name = "BVR à 10 jours net";
			paymentMode1.Description = "Facture payable au moyen du bulletin de versement ci-joint.<br/>Conditions: 10 jours net.";
			paymentMode1.BookAccount = "1010";
			paymentMode1.StandardPaymentTerm = 10;

			paymentMode2.Rank = 1;
			paymentMode2.Code = "BILL30";
			paymentMode2.Name = "BVR à 30 jours net";
			paymentMode2.Description = "Facture payable au moyen du bulletin de versement ci-joint.<br/>Conditions: 30 jours net.";
			paymentMode2.BookAccount = "1010";
			paymentMode2.StandardPaymentTerm = 30;

			paymentMode3.Rank = 4;
			paymentMode3.Code = "PAYED";
			paymentMode3.Name = "Au comptant";
			paymentMode3.Description = "Facture payée au comptant.";
			paymentMode3.BookAccount = "1000";

			yield return paymentMode1;
			yield return paymentMode2;
			yield return paymentMode3;
		}

		private IEnumerable<CurrencyEntity> InsertCurrenciesInDatabase()
		{
			var currency1 = this.DataContext.CreateEntity<CurrencyEntity> ();
			var currency2 = this.DataContext.CreateEntity<CurrencyEntity> ();
			var currency3 = this.DataContext.CreateEntity<CurrencyEntity> ();

			currency1.CurrencyCode = Business.Finance.CurrencyCode.Chf;
			currency1.ExchangeRate = 1.0M;

			currency2.CurrencyCode = Business.Finance.CurrencyCode.Eur;
			currency2.ExchangeRate = 1.5M;  // au hasard

			currency3.CurrencyCode = Business.Finance.CurrencyCode.Usd;
			currency3.ExchangeRate = 1.2M;  // au hasard

			yield return currency1;
			yield return currency2;
			yield return currency3;
		}

		private IEnumerable<VatDefinitionEntity> InsertVatDefinitionsInDatabase()
		{
			var vatDef2010_1 = this.DataContext.CreateEntity<VatDefinitionEntity> ();
			var vatDef2010_2 = this.DataContext.CreateEntity<VatDefinitionEntity> ();
			var vatDef2010_3 = this.DataContext.CreateEntity<VatDefinitionEntity> ();
			var vatDef2010_4 = this.DataContext.CreateEntity<VatDefinitionEntity> ();

			var vatDef2011_1 = this.DataContext.CreateEntity<VatDefinitionEntity> ();
			var vatDef2011_2 = this.DataContext.CreateEntity<VatDefinitionEntity> ();
			var vatDef2011_3 = this.DataContext.CreateEntity<VatDefinitionEntity> ();
			var vatDef2011_4 = this.DataContext.CreateEntity<VatDefinitionEntity> ();

			vatDef2010_1.Rank = 0;
			vatDef2010_1.BeginDate = new System.DateTime (2000, 1, 1, 0, 0, 0);
			vatDef2010_1.EndDate   = new System.DateTime (2010, 12, 31, 23, 59, 59);
			vatDef2010_1.Code = Business.Finance.VatCode.StandardTaxOnTurnover;
			vatDef2010_1.Name = "TVA sur le chiffre d'affaires, taux standard";
			vatDef2010_1.Rate = 7.6M * 0.01M;

			vatDef2010_2.Rank = 1;
			vatDef2010_2.BeginDate = new System.DateTime (2000, 1, 1, 0, 0, 0);
			vatDef2010_2.EndDate   = new System.DateTime (2010, 12, 31, 23, 59, 59);
			vatDef2010_2.Code = Business.Finance.VatCode.ReducedTaxOnTurnover;
			vatDef2010_2.Name = "TVA sur le chiffre d'affaires, taux réduit";
			vatDef2010_2.Rate = 2.4M * 0.01M;

			vatDef2010_3.Rank = 2;
			vatDef2010_3.BeginDate = new System.DateTime (2000, 1, 1, 0, 0, 0);
			vatDef2010_3.EndDate   = new System.DateTime (2010, 12, 31, 23, 59, 59);
			vatDef2010_3.Code = Business.Finance.VatCode.SpecialTaxOnTurnover;
			vatDef2010_3.Name = "TVA sur le chiffre d'affaires, taux spécial";
			vatDef2010_3.Rate = 3.6M * 0.01M;

			vatDef2010_4.Rank = 3;
			vatDef2010_4.BeginDate = new System.DateTime (2000, 1, 1, 0, 0, 0);
			vatDef2010_4.EndDate   = new System.DateTime (2010, 12, 31, 23, 59, 59);
			vatDef2010_4.Code = Business.Finance.VatCode.Excluded;
			vatDef2010_4.Name = "Exclu du champ d'application de la TVA";
			vatDef2010_4.Rate = 0.0M * 0.01M;

			vatDef2011_1.Rank = 0;
			vatDef2011_1.BeginDate = new System.DateTime (2011, 1, 1, 0, 0, 0);
			vatDef2011_1.EndDate   = null;
			vatDef2011_1.Code = Business.Finance.VatCode.StandardTaxOnTurnover;
			vatDef2011_1.Name = "TVA sur le chiffre d'affaires, taux standard";
			vatDef2011_1.Rate = 8.0M * 0.01M;

			vatDef2011_2.Rank = 1;
			vatDef2011_2.BeginDate = new System.DateTime (2011, 1, 1, 0, 0, 0);
			vatDef2011_2.EndDate   = null;
			vatDef2011_2.Code = Business.Finance.VatCode.ReducedTaxOnTurnover;
			vatDef2011_2.Name = "TVA sur le chiffre d'affaires, taux réduit";
			vatDef2011_2.Rate = 2.5M * 0.01M;

			vatDef2011_3.Rank = 2;
			vatDef2011_3.BeginDate = new System.DateTime (2011, 1, 1, 0, 0, 0);
			vatDef2011_3.EndDate   = null;
			vatDef2011_3.Code = Business.Finance.VatCode.SpecialTaxOnTurnover;
			vatDef2011_3.Name = "TVA sur le chiffre d'affaires, taux spécial";
			vatDef2011_3.Rate = 3.8M * 0.01M;

			vatDef2011_4.Rank = 3;
			vatDef2011_4.BeginDate = new System.DateTime (2011, 1, 1, 0, 0, 0);
			vatDef2011_4.EndDate   = null;
			vatDef2011_4.Code = Business.Finance.VatCode.Excluded;
			vatDef2011_4.Name = "Exclu du champ d'application de la TVA";
			vatDef2011_4.Rate = 0.0M * 0.01M;

			yield return vatDef2010_1;
			yield return vatDef2010_2;
			yield return vatDef2010_3;
			yield return vatDef2010_4;
			
			yield return vatDef2011_1;
			yield return vatDef2011_2;
			yield return vatDef2011_3;
			yield return vatDef2011_4;
		}

		private IEnumerable<BusinessSettingsEntity> InsertBusinessSettingsInDatabase()
		{
			var business = this.DataContext.CreateEntity<BusinessSettingsEntity> ();
			var tax = this.DataContext.CreateEntity<TaxSettingsEntity> ();
			var finance = this.DataContext.CreateEntity<FinanceSettingsEntity> ();
			var isrDef1 = this.DataContext.CreateEntity<IsrDefinitionEntity> ();

			business.Company = this.DataContext.GetEntitiesOfType<RelationEntity> (x => x.Person is LegalPersonEntity && (x.Person as LegalPersonEntity).Name == "Epsitec SA").FirstOrDefault ();
			business.Finance = finance;
			business.Tax = tax;

			tax.VatNumber = "199160";
			tax.TaxMode = Business.Finance.TaxMode.LiableForVat;

			isrDef1.Currency = Business.Finance.CurrencyCode.Chf;
			isrDef1.SubscriberNumber = "010694443";
			isrDef1.SubscriberAddress = "Epsitec SA<br/>Ch. du Fontenay 6<br/>1400 Yverdon-les-Bains";
			isrDef1.IncomingBookAccount = "1010";
			
			finance.IsrDefs.Add (isrDef1);

			yield return business;
		}

		private IEnumerable<BusinessDocumentEntity> InsertInvoiceDocumentsInDatabase(MailContactEntity billingAddress, PaymentModeEntity[] paymentDefs, CurrencyEntity[] currencyDefs, ArticleDefinitionEntity[] articleDefs, VatDefinitionEntity[] vatDefs, BusinessSettingsEntity[] settings)
		{
			var decimalType = DecimalType.Default;
			decimal vatRate = vatDefs.Where (x => x.Code == Business.Finance.VatCode.StandardTaxOnTurnover).First ().Rate;

			var billingA1 = this.DataContext.CreateEntity<BillingDetailEntity> ();
			var billingA2 = this.DataContext.CreateEntity<BillingDetailEntity> ();
			var invoiceA = this.DataContext.CreateEntity<BusinessDocumentEntity> ();

			invoiceA.IdA = "1000-00";
//-			invoiceA.DocumentSource = Business.DocumentSource.Generated;
			invoiceA.DocumentTitle = "Votre commande du 5 juillet 2010<br/>S/notre directeur M. P. Arnaud";
//-			invoiceA.Description = "Facture de test #1000";
			invoiceA.BillingDate = new Date (2010, 7, 8);
			invoiceA.BillingMailContact = billingAddress;
			invoiceA.ShippingMailContact = billingAddress;
			invoiceA.OtherPartyBillingMode = Business.Finance.BillingMode.IncludingTax;
			invoiceA.OtherPartyTaxMode = Business.Finance.TaxMode.LiableForVat;
			invoiceA.BillingCurrencyCode = Business.Finance.CurrencyCode.Chf;
			invoiceA.BillingStatus = Business.Finance.BillingStatus.DebtorBillOpen;
			invoiceA.BillingDetails.Add (billingA1);
			invoiceA.BillingDetails.Add (billingA2);
			invoiceA.DebtorBookAccount = "1100";

			var textA1 = this.DataContext.CreateEntity<TextDocumentItemEntity> ();

			textA1.Visibility = true;
			textA1.Text = "Logiciels";

			var lineA1 = this.DataContext.CreateEntity<ArticleDocumentItemEntity> ();
			var quantityA1 = this.DataContext.CreateEntity<ArticleQuantityEntity> ();

			lineA1.Visibility = true;
			lineA1.IndentationLevel = 0;
			lineA1.BeginDate = invoiceA.BillingDate;
			lineA1.EndDate = invoiceA.BillingDate;
			lineA1.ArticleDefinition = articleDefs.Where (x => x.IdA == "CR-CP").FirstOrDefault ();
			lineA1.VatCode = Business.Finance.VatCode.StandardTaxOnTurnover;
			lineA1.PrimaryUnitPriceBeforeTax = lineA1.ArticleDefinition.ArticlePrices[0].ValueBeforeTax;
			lineA1.PrimaryLinePriceBeforeTax = lineA1.PrimaryUnitPriceBeforeTax * 3;
			lineA1.NeverApplyDiscount = false;
			lineA1.ResultingLinePriceBeforeTax = (int) lineA1.PrimaryLinePriceBeforeTax;
			lineA1.ResultingLineTax = lineA1.ResultingLinePriceBeforeTax * vatRate;
			lineA1.ArticleShortDescriptionCache = lineA1.ArticleDefinition.ShortDescription;
			lineA1.ArticleLongDescriptionCache = lineA1.ArticleDefinition.LongDescription;
			
			quantityA1.ColumnName = "livré";
			quantityA1.QuantityType = Business.ArticleQuantityType.Billed;
			quantityA1.Quantity = 3;
			quantityA1.Unit = lineA1.ArticleDefinition.BillingUnit;
			quantityA1.ExpectedDate = new Date (2010, 7, 8);

			lineA1.ArticleQuantities.Add (quantityA1);

			var lineA2 = this.DataContext.CreateEntity<ArticleDocumentItemEntity> ();
			var quantityA2_1 = this.DataContext.CreateEntity<ArticleQuantityEntity> ();
			var quantityA2_2 = this.DataContext.CreateEntity<ArticleQuantityEntity> ();

			lineA2.Visibility = true;
			lineA2.IndentationLevel = 0;
			lineA2.BeginDate = invoiceA.BillingDate;
			lineA2.EndDate = invoiceA.BillingDate;
			lineA2.ArticleDefinition = articleDefs.Where (x => x.IdA == "CR-FL").FirstOrDefault ();
			lineA2.VatCode = Business.Finance.VatCode.StandardTaxOnTurnover;
			lineA2.PrimaryUnitPriceBeforeTax = lineA2.ArticleDefinition.ArticlePrices[0].ValueBeforeTax;
			lineA2.PrimaryLinePriceBeforeTax = lineA2.PrimaryUnitPriceBeforeTax;
			lineA2.NeverApplyDiscount = false;
			lineA2.ResultingLinePriceBeforeTax = (int) lineA2.PrimaryLinePriceBeforeTax;
			lineA2.ResultingLineTax = lineA2.ResultingLinePriceBeforeTax * vatRate;
			lineA2.ArticleShortDescriptionCache = lineA2.ArticleDefinition.ShortDescription;
			lineA2.ArticleLongDescriptionCache = lineA2.ArticleDefinition.LongDescription;

			quantityA2_1.ColumnName = "livré";
			quantityA2_1.QuantityType = Business.ArticleQuantityType.Billed;
			quantityA2_1.Quantity = 1;
			quantityA2_1.Unit = lineA2.ArticleDefinition.BillingUnit;
			quantityA2_1.ExpectedDate = new Date (2010, 7, 8);

			quantityA2_2.ColumnName = "suivra";
			quantityA2_2.QuantityType = Business.ArticleQuantityType.Delayed;
			quantityA2_2.Quantity = 1;
			quantityA2_2.Unit = lineA2.ArticleDefinition.BillingUnit;
			quantityA2_2.ExpectedDate = new Date (2010, 7, 19);

			lineA2.ArticleQuantities.Add (quantityA2_1);
			lineA2.ArticleQuantities.Add (quantityA2_2);

			var lineA3 = this.DataContext.CreateEntity<ArticleDocumentItemEntity> ();
			var quantityA3_1 = this.DataContext.CreateEntity<ArticleQuantityEntity> ();
			var quantityA3_2 = this.DataContext.CreateEntity<ArticleQuantityEntity> ();

			lineA3.Visibility = true;
			lineA3.IndentationLevel = 0;
			lineA3.BeginDate = invoiceA.BillingDate;
			lineA3.EndDate = invoiceA.BillingDate;
			lineA3.ArticleDefinition = articleDefs.Where (x => x.IdA == "CR-SP").FirstOrDefault ();
			lineA3.VatCode = Business.Finance.VatCode.StandardTaxOnTurnover;
			lineA3.PrimaryUnitPriceBeforeTax = lineA3.ArticleDefinition.ArticlePrices[0].ValueBeforeTax;
			lineA3.PrimaryLinePriceBeforeTax = lineA3.PrimaryUnitPriceBeforeTax * 0;
			lineA3.NeverApplyDiscount = false;
			lineA3.ResultingLinePriceBeforeTax = (int) lineA3.PrimaryLinePriceBeforeTax;
			lineA3.ResultingLineTax = lineA3.ResultingLinePriceBeforeTax * vatRate;
			lineA3.ArticleShortDescriptionCache = lineA3.ArticleDefinition.ShortDescription;
			lineA3.ArticleLongDescriptionCache = lineA3.ArticleDefinition.LongDescription;

			quantityA3_1.ColumnName = "suivra";
			quantityA3_1.QuantityType = Business.ArticleQuantityType.Delayed;
			quantityA3_1.Quantity = 2;
			quantityA3_1.Unit = lineA3.ArticleDefinition.BillingUnit;
			quantityA3_1.ExpectedDate = new Date (2010, 9, 1);

			quantityA3_2.ColumnName = "suivra";
			quantityA3_2.QuantityType = Business.ArticleQuantityType.Delayed;
			quantityA3_2.Quantity = 1;
			quantityA3_2.Unit = lineA3.ArticleDefinition.BillingUnit;
			quantityA3_2.ExpectedDate = new Date (2011, 1, 31);

			lineA3.ArticleQuantities.Add (quantityA3_1);
			lineA3.ArticleQuantities.Add (quantityA3_2);

			var lineA4 = this.DataContext.CreateEntity<ArticleDocumentItemEntity> ();
			var quantityA4 = this.DataContext.CreateEntity<ArticleQuantityEntity> ();

			lineA4.Visibility = true;
			lineA4.IndentationLevel = 0;
			lineA4.BeginDate = invoiceA.BillingDate;
			lineA4.EndDate = invoiceA.BillingDate;
			lineA4.ArticleDefinition = articleDefs.Where (x => x.IdA == "EMB").FirstOrDefault ();
			lineA4.VatCode = Business.Finance.VatCode.StandardTaxOnTurnover;
			lineA4.PrimaryUnitPriceBeforeTax = lineA4.ArticleDefinition.ArticlePrices[0].ValueBeforeTax;
			lineA4.PrimaryLinePriceBeforeTax = lineA4.PrimaryUnitPriceBeforeTax;
			lineA4.NeverApplyDiscount = true;
			lineA4.ResultingLinePriceBeforeTax = (int) lineA4.PrimaryUnitPriceBeforeTax;
			lineA4.ResultingLineTax = lineA4.ResultingLinePriceBeforeTax * vatRate;
			lineA4.ArticleShortDescriptionCache = lineA4.ArticleDefinition.ShortDescription;
			lineA4.ArticleLongDescriptionCache = lineA4.ArticleDefinition.LongDescription;

			quantityA4.ColumnName = "livré";
			quantityA4.QuantityType = Business.ArticleQuantityType.Billed;
			quantityA4.Quantity = 1;
			quantityA4.Unit = lineA4.ArticleDefinition.BillingUnit;
			quantityA4.ExpectedDate = new Date (2010, 7, 8);

			lineA4.ArticleQuantities.Add (quantityA4);

			var discountA1 = this.DataContext.CreateEntity<DiscountEntity> ();

			discountA1.Description = "Rabais de quantité";
			discountA1.DiscountRate = 0.20M;

			var totalA1 = this.DataContext.CreateEntity<PriceDocumentItemEntity> ();

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
			totalA1.ResultingTax = totalA1.ResultingPriceBeforeTax * vatRate;
			totalA1.TextForPrimaryPrice = "Total HT avant rabais";
			totalA1.TextForResultingPrice = "Total HT après rabais";
			totalA1.DisplayModes = Business.Finance.PriceDisplayModes.PrimaryTotal | Business.Finance.PriceDisplayModes.Discount | Business.Finance.PriceDisplayModes.ResultingTotal;

			var taxA1 = this.DataContext.CreateEntity<TaxDocumentItemEntity> ();

			taxA1.Visibility = true;
			taxA1.VatCode = Business.Finance.VatCode.StandardTaxOnTurnover;
			taxA1.Rate = vatRate;
			taxA1.BaseAmount = totalA1.ResultingPriceBeforeTax + lineA4.ResultingLinePriceBeforeTax;
			taxA1.ResultingTax = taxA1.Rate * taxA1.BaseAmount; // devrait être égal à 'totalA1.ResultingTax + lineA4.ResultingLineTax'
			taxA1.Text = "TVA au taux standard";
			
			var totalA2 = this.DataContext.CreateEntity<TotalDocumentItemEntity> ();

			totalA2.Visibility = true;
			totalA2.PrimaryPriceAfterTax = totalA1.ResultingPriceBeforeTax + totalA1.ResultingTax + lineA4.ResultingLinePriceBeforeTax + lineA4.ResultingLineTax;
			totalA2.FixedPriceAfterTax = (int) (totalA2.PrimaryPriceAfterTax / 10) * 10.00M;	//	arrondi à 10.-
			totalA2.TextForPrimaryPrice = "Total TTC";
			totalA2.TextForFixedPrice = "Total TTC arrêté";

			//	Le total arrêté force un rabais supplémentaire qu'il faut remonter dans les
			//	lignes d'articles qui peuvent être soumis à un rabais :

			decimal? totalBeforeTax = totalA1.ResultingPriceBeforeTax + lineA4.ResultingLinePriceBeforeTax;
			decimal? fixedPriceDiscount = (totalBeforeTax - lineA4.PrimaryLinePriceBeforeTax) / totalA1.ResultingPriceBeforeTax.Value;

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
			invoiceA.Lines.Add (taxA1);			//	  TVA de 136.xx
			invoiceA.Lines.Add (totalA2);		//	Total arrêté à 1930.00

			var paymentA1 = this.DataContext.CreateEntity<PaymentDetailEntity> ();
			var paymentA2 = this.DataContext.CreateEntity<PaymentDetailEntity> ();

			paymentA1.PaymentType = Business.Finance.PaymentDetailType.AmountDue;
			paymentA1.PaymentMode = paymentDefs.Where (x => x.Code == "BILL30").FirstOrDefault ();
			paymentA1.Amount = 1000.00M;
			paymentA1.Currency = currencyDefs.Where (x => x.CurrencyCode == Business.Finance.CurrencyCode.Chf).FirstOrDefault ();
			paymentA1.Date = new Date (2010, 08, 06);

			paymentA2.PaymentType = Business.Finance.PaymentDetailType.AmountDue;
			paymentA2.PaymentMode = paymentDefs.Where (x => x.Code == "BILL30").FirstOrDefault ();
			paymentA2.Amount = totalA2.FixedPriceAfterTax.Value - paymentA1.Amount;
			paymentA2.Currency = currencyDefs.Where (x => x.CurrencyCode == Business.Finance.CurrencyCode.Chf).FirstOrDefault ();
			paymentA2.Date = new Date (2010, 09, 05);

			var isrDefiniton = settings.First ().Finance.IsrDefs.First ();

			var isrSubscriber = isrDefiniton.SubscriberNumber;
			var isrRef1 = Isr.GetNewReferenceNumber (this, isrSubscriber);
			var isrRef2 = Isr.GetNewReferenceNumber (this, isrSubscriber);

			billingA1.Title = "Facture 1000-00, 1ère tranche";
			billingA1.AmountDue = paymentA1;
			billingA1.IsrDefinition = isrDefiniton;			//	compte BVR
			billingA1.IsrReferenceNumber = isrRef1;			//	n° de réf BVR lié
			billingA1.InstalmentRank = 0;
			billingA1.InstalmentName = "1/2";

			billingA2.Title = "Facture 1000-00, 2ème tranche";
			billingA2.AmountDue = paymentA2;
			billingA2.IsrDefinition = isrDefiniton;			//	compte BVR
			billingA2.IsrReferenceNumber = isrRef2;			//	n° de réf BVR lié
			billingA2.InstalmentRank = 1;
			billingA2.InstalmentName = "2/2";

			yield return invoiceA;
		}

		private ArticlePriceEntity CreateArticlePrice(decimal price, ArticlePriceGroupEntity articlePriceGroup1 = null, ArticlePriceGroupEntity articlePriceGroup2 = null, ArticlePriceGroupEntity articlePriceGroup3 = null)
		{
			var articlePrice1 = this.DataContext.CreateEntity<ArticlePriceEntity> ();

			articlePrice1.BeginDate = new System.DateTime (2010,  1,  1,  0,  0,  0);  // 1 janvier 00:00:00
			articlePrice1.EndDate   = new System.DateTime (2020, 12, 31, 23, 59, 59);  // 31 décembre 23:59:59 (c'est important de donner l'heure)
			articlePrice1.MinQuantity = 1;
			articlePrice1.MaxQuantity = null;
			articlePrice1.CurrencyCode = Business.Finance.CurrencyCode.Chf;
			articlePrice1.ValueBeforeTax = price;
			if (articlePriceGroup1 != null) articlePrice1.PriceGroups.Add (articlePriceGroup1);
			if (articlePriceGroup2 != null) articlePrice1.PriceGroups.Add (articlePriceGroup2);
			if (articlePriceGroup3 != null) articlePrice1.PriceGroups.Add (articlePriceGroup3);
			return articlePrice1;
		}

		private IEnumerable<WorkflowDefinitionEntity> InsertWorkflowDefinitionsInDatabase()
		{
			var def = this.DataContext.CreateEntity<WorkflowDefinitionEntity> ();

			var nodeA = this.DataContext.CreateEntity<WorkflowNodeEntity> ();
			var nodeB = this.DataContext.CreateEntity<WorkflowNodeEntity> ();
			var nodeC = this.DataContext.CreateEntity<WorkflowNodeEntity> ();

			var edgeAB = this.DataContext.CreateEntity<WorkflowEdgeEntity> ();
			var edgeAC = this.DataContext.CreateEntity<WorkflowEdgeEntity> ();
			var edgeCA = this.DataContext.CreateEntity<WorkflowEdgeEntity> ();

			def.Code = "CUST-ORDER";
			def.EnableCondition = "MasterEntity=[L0AB2]";
			def.Name = FormattedText.FromSimpleText ("Commande client");
			def.Description = FormattedText.FromSimpleText ("Workflow pour le traitement d'une commande client (offre, bon pour commande, confirmation de commande, production, livraison)");
			def.StartingEdges.Add (edgeAB);

			nodeA.Code = "SALES-QUOTE(1)";
			nodeA.Name = FormattedText.FromSimpleText ("Préparation de l'offre");
			nodeA.Edges.Add (edgeAB);
			nodeA.Edges.Add (edgeAC);

			edgeAB.Name = FormattedText.FromSimpleText ("Créer une nouvelle offre");
			edgeAB.Description = FormattedText.FromSimpleText ("Crée une nouvelle offre liée à une nouvelle affaire pour ce client.");
			edgeAB.TransitionAction = "WorkflowAction.NewAffair";
			edgeAB.NextNode = nodeB;

			edgeAC.NextNode = nodeC;
			edgeCA.NextNode = nodeA;

			nodeB.Code = "SALES-QUOTE(2)";
			nodeB.Name = "Offre envoyée";

			nodeC.Code = "SALES-QUOTE(3)";
			nodeC.Name = "Variante de l'offre";
			nodeC.Edges.Add (edgeCA);

			yield return def;
		}
	}
}
