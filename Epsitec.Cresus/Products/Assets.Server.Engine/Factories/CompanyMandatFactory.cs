//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.Data;
using Epsitec.Cresus.Assets.Server.BusinessLogic;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.Server.Engine
{
	/// <summary>
	/// Cette classe sait fabriquer un nouveau mandat pour une entreprise, tout beau tout propre.
	/// </summary>
	public class CompanyMandatFactory : AbstractMandatFactory
	{
		public CompanyMandatFactory(DataAccessor accessor)
			: base (accessor)
		{
		}


		public override DataMandat Create(string name, System.DateTime startDate, bool withSamples)
		{
			this.withSamples = withSamples;

			this.accessor.Mandat = new DataMandat (this.accessor.ComputerSettings, name, startDate);

			this.AddAssetsSettings ();
			this.AddPersonsSettings ();

			if (this.withSamples)
			{
				DummyAccounts.AddAccounts (this.accessor.Mandat, "pme 2011");
			}

			this.CreateArgumentsSamples ();
			this.CreateMethodsSamples ();
			this.CreateGroupsSamples ();
			this.CreateCatsSamples ();

			if (this.withSamples)
			{
				this.AddPersonsSamples ();
				this.AddAssetsSamples ();
			}

			//	Recalcule tout.
			foreach (var obj in this.accessor.Mandat.GetData (BaseType.Assets))
			{
				Amortizations.UpdateAmounts (this.accessor, obj);
			}

			this.AddReports ();

			return this.accessor.Mandat;
		}


		protected override void AddAssetsSettings()
		{
			base.AddAssetsSettings ();

			if (this.withSamples)
			{
				this.fieldAssetValue1 = this.AddSettings (BaseType.AssetsUserFields, "Valeur remplacement", FieldType.ComputedAmount, false, 120, null, null, null, 10);
				this.fieldAssetValue2 = this.AddSettings (BaseType.AssetsUserFields, "Valeur fiscale",      FieldType.ComputedAmount, false, 120, null, null, null,  0);
				this.fieldAssetOwner1 = this.AddSettings (BaseType.AssetsUserFields, "Responsable",         FieldType.GuidPerson,     false, 150, null, null, null, 10);
				this.fieldAssetOwner2 = this.AddSettings (BaseType.AssetsUserFields, "Remplaçant",          FieldType.GuidPerson,     false, 150, null, null, null,  0);
				this.fieldAssetDesc   = this.AddSettings (BaseType.AssetsUserFields, "Description",         FieldType.String,         false, 120,  380,    5, null, 10);
			}
		}


		protected override void AddAssetsSamples()
		{
			var i1 = this.AddAssetsSamples (this.accessor.Mandat.StartDate.AddDays  (  0), "Siège social",             "100", 2400000.0m, 3500000.0m, 2100000.0m, "Dupond",   "Nicolet",  "Immobilier",             "Bâtiments", "Bureaux",                         "Nord");
			var i2 = this.AddAssetsSamples (this.accessor.Mandat.StartDate.AddDays  (  0), "Centre logistique",        "110", 1200000.0m, 2000000.0m, 1000000.0m, "Dupond",   "Nicolet",  "Immobilier",             "Bâtiments", "Bureaux",                         "Est/0.5", "Nord/0.5");
			         this.AddAssetsSamples (this.accessor.Mandat.StartDate.AddDays  (  0), "Centre d'expédition",      "111", 3500000.0m, 4000000.0m, 3000000.0m, "Dupond",   null,       "Immobilier",             "Bâtiments", "Bureaux/0.4", "Distribution/0.6", "Nord");
			         this.AddAssetsSamples (this.accessor.Mandat.StartDate.AddDays  (  0), "Centre d'usinage",         "112", 5100000.0m, 6000000.0m, 4000000.0m, "Dupond",   null,       "Immobilier",             "Usines",    "Production",                      "Est");
			         this.AddAssetsSamples (this.accessor.Mandat.StartDate.AddYears (  1), "Centre d'assemblage",      "113", 3200000.0m, 4000000.0m, 2000000.0m, "Dubosson", "Nicolet",  "Immobilier",             "Usines",    "Production/0.8", "Stockage/0.2",  "Sud");
			         this.AddAssetsSamples (this.accessor.Mandat.StartDate.AddYears (  2), "Centre de recyclage",      "114",  200000.0m,  400000.0m,  200000.0m, "Dubosson", "Nicolet",  "Immobilier",             "Usines",    "Production",                      "Sud");
			var i3 = this.AddAssetsSamples (this.accessor.Mandat.StartDate.AddYears (  1), "Dépôt principal",          "120", 3200000.0m, 5000000.0m, 2800000.0m, "Dubosson", "Nicolet",  "Immobilier",             "Entrepôts", "Stockage",                        "Sud");
			         this.AddAssetsSamples (this.accessor.Mandat.StartDate.AddYears (  3), "Dépôt secondaire",         "121", 1300000.0m, 2000000.0m, 1100000.0m, "Dubosson", "Nicolet",  "Immobilier",             "Entrepôts", "Stockage",                        "Nord");
			         this.AddAssetsSamples (this.accessor.Mandat.StartDate.AddDays  ( 50), "Scania X-20",              "200",  142000.0m,  150000.0m,  150000.0m, "Dupond",   "Nicolet",  "Véhicules",              "Camions",   "Transport");
			         this.AddAssetsSamples (this.accessor.Mandat.StartDate.AddDays  ( 31), "Scania X-45",              "201",   84000.0m,  100000.0m,   90000.0m, "Dupond",   "Nicolet",  "Véhicules",              "Camions",   "Transport");
			         this.AddAssetsSamples (this.accessor.Mandat.StartDate.AddDays  ( 31), "Volvo Truck P2",           "205",   90000.0m,  100000.0m,  110000.0m, "Nicolet",  "Zumstein", "Véhicules",              "Camions",   "Transport");
			var v1 = this.AddAssetsSamples (this.accessor.Mandat.StartDate.AddYears (  1), "Fiat Uno",                 "300",    8000.0m,  180000.0m,   10000.0m, "Nicolet",  null,       "Véhicules",              "Voitures",  "Bureaux");
			var v2 = this.AddAssetsSamples (this.accessor.Mandat.StartDate.AddDays  (100), "Citroën C4 Picasso",       "304",   22000.0m,   35000.0m,   35000.0m, "Nicolet",  null,       "Véhicules",              "Voitures",  "Production");
			         this.AddAssetsSamples (this.accessor.Mandat.StartDate.AddDays  (  0), "Participations Nestlé",    "500",  300000.0m,       null,  290000.0m, "Zumstein", null,       "Amortissements manuels", "Investissements");
			         this.AddAssetsSamples (this.accessor.Mandat.StartDate.AddDays  (  0), "Participations Logitech",  "501",   10000.0m,       null,   15000.0m, "Zumstein", null,       "Amortissements manuels", "Investissements");
			         this.AddAssetsSamples (this.accessor.Mandat.StartDate.AddDays  (200), "Participations Raifeisen", "502",  250000.0m,       null,  250000.0m, "Dubosson", null,       "Amortissements manuels", "Investissements");

			{
				var e = this.AddAssetEvent (i1, this.accessor.Mandat.StartDate.AddYears (1), EventType.Modification);
				this.AddAssetComputedAmount (e, this.fieldAssetValue1, 3600000.0m);
				this.AddAssetComputedAmount (e, this.fieldAssetValue2, 1900000.0m);
			}

			{
				var e = this.AddAssetEvent (i1, this.accessor.Mandat.StartDate.AddYears (2), EventType.Modification);
				this.AddAssetComputedAmount (e, this.fieldAssetValue1, 3900000.0m);
			}

			{
				var e = this.AddAssetEvent (i3, this.accessor.Mandat.StartDate.AddYears (2).AddDays (-1), EventType.AmortizationExtra);
				this.AddAssetAmortizedAmount (e, 1300000.0m, 1100000.0m);
			}

			{
				var e = this.AddAssetEvent (i2, this.accessor.Mandat.StartDate.AddYears (1).AddDays (-1), EventType.Modification);
				this.AddAssetGroup (e, 2, "Est/0.4");
				this.AddAssetGroup (e, 3, "Nord/0.6");
			}

			{
				var e = this.AddAssetEvent (v1, this.accessor.Mandat.StartDate.AddYears (1).AddDays (40), EventType.Modification);
				this.AddAssetPerson (e, this.fieldAssetOwner1, "Zumstein");
			}

			{
				var e1 = this.AddAssetEvent (v2, this.accessor.Mandat.StartDate.AddYears (1).AddDays (-1), EventType.AmortizationExtra);
				this.AddAssetAmortizedAmount (e1, 22000.0m, 18000.0m);

				var e2 = this.AddAssetEvent (v2, this.accessor.Mandat.StartDate.AddYears (2).AddDays (-1), EventType.AmortizationExtra);
				this.AddAssetAmortizedAmount (e2, 18000.0m, 10000.0m);
			}
		}


		protected override void AddPersonsSamples()
		{
			this.AddPersonSample ("Monsieur", "Jean",      "Dupond",   null,                 "av. des Planches 12 bis",                     "1023", "Crissier",             "Suisse", null,            null,            null,            "jeandupond@bluewin.ch");
			this.AddPersonSample ("Madame",   "Renata",    "Zumstein", null,                 "Crésentine 21",                               "1023", "Crissier",             "Suisse", "021 512 44 55", null,            null,            "zumstein@crissier.ch");
			this.AddPersonSample ("Monsieur", "Alfred",    "Dubosson", null,                 "ch. des Tilleuls 4",                          "1020", "Renens",               "Suisse", "021 512 44 55", "021 600 22 33", null,            "dubosson@crissier.ch");
			this.AddPersonSample ("Madame",   "Sandra",    "Nicolet",  null,                 "Place du Marché",                             "2000", "Neuchâtel",            "Suisse", null,            null,            "079 810 20 30", "sandranicolet5@gmail.com");
			this.AddPersonSample ("Madame",   "Sylvianne", "Galbato",  "Les Bons Tuyaux SA", "Z.I. Budron 12A",                             "1052", "Le Mont-sur-Lausanne", "Suisse", "021 312 28 29", null,            null,            "sylvianne@lesbonstuyaux.ch");
			this.AddPersonSample ("Monsieur", "André",     "Mercier",  "Mecatronic SA",      "Y-Parc - Swiss Technopole<br/>Rue Galilée 7", "1400", "Yverdon-les-Bains",    "Suisse", "024 444 11 22", "022 871 98 76", null,            "mercier@mecatronic.ch");
		}


		protected override void CreateGroupsSamples()
		{
			var root = this.AddGroup (null, "Groupes", null);

			this.CreateGroupsCatsSamples    (root);
			this.CreateGroupsCentresSamples (root);

			if (this.withSamples)
			{
				this.CreateGroupsSectorsSamples (root);
			}
		}

		private void CreateGroupsCatsSamples(DataObject parent)
		{
			var root = this.AddGroup (parent, "Catégories", "100", true);

			var imm = this.AddGroup (root, "Immeubles",              "10");
			          this.AddGroup (root, "Mobilier",               "20");
			var veh = this.AddGroup (root, "Véhicules",              "30");
			          this.AddGroup (root, "Machines",               "40");
			          this.AddGroup (root, "Investissements",        "50");
			          this.AddGroup (root, "Autres immobilisations", "90");

			this.AddGroup (imm, "Bâtiments",   "10");
			this.AddGroup (imm, "Usines",      "20");
			this.AddGroup (imm, "Entrepôts",   "30");

			this.AddGroup (veh, "Camions",      "10");
			this.AddGroup (veh, "Camionnettes", "20");
			this.AddGroup (veh, "Voitures",     "30");
		}

		private void CreateGroupsCentresSamples(DataObject parent)
		{
			var root = this.AddGroup (parent, "Centres de frais", "200", true);

			this.AddGroup (root, "Bureaux",      "10");
			this.AddGroup (root, "Production",   "20");
			this.AddGroup (root, "Distribution", "30");
			this.AddGroup (root, "Stockage",     "40");
			this.AddGroup (root, "Transport",    "50");
		}

		private void CreateGroupsSectorsSamples(DataObject parent)
		{
			var root = this.AddGroup (parent, "Secteurs", "400");

			this.AddGroup (root, "Nord",  "10");
			this.AddGroup (root, "Sud",   "20");
			this.AddGroup (root, "Est",   "30");
			this.AddGroup (root, "Ouest", "40");
		}


		protected override void CreateCatsSamples()
		{
			this.AddCat ("Amortissements manuels", 
				"Un objet de cette catégorie ne sera jamais amorti automatiquement.",
				"0", "Aucun", Periodicity.Annual);

			this.AddCat ("Immobilier", null, "10", "Taux linéaire", Periodicity.Annual,
				"1600", "1000",
				"1010", "1600",
				"6930", "1600",
				"6930", "1600",
				"6900", "1600",
				"6900", "1600",
				"6900", "1600",
				0.1m, null, 1.0m, 1.0m);

			this.AddCat ("Véhicules", null, "20", "Taux linéaire", Periodicity.Annual,
				"1600", "1000",
				"1010", "1530",
				"6920", "1530",
				"6920", "1530",
				"6900", "1530",
				"6900", "1530",
				"6900", "1530",
				0.2m, null, 1.0m, 1.0m);
		}
	}
}
