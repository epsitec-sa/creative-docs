//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.Data;
using Epsitec.Cresus.Assets.Data.Reports;
using Epsitec.Cresus.Assets.Server.BusinessLogic;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.Server.Engine
{
	/// <summary>
	/// Cette classe sait fabriquer un nouveau mandat MCH2, tout beau tout propre.
	/// </summary>
	public class MCH2MandatFactory : AbstractMandatFactory
	{
		public MCH2MandatFactory(DataAccessor accessor)
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
				DummyAccounts.AddAccounts (this.accessor.Mandat, "monvillage 2014");
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
			var i1 = this.AddAssetsSamples (this.accessor.Mandat.StartDate.AddDays  (  0), "Bâtiment administratif",   "100", 2400000.0m, 3500000.0m, 2100000.0m, "Dupond",   "Nicolet",  "Immobilier",             "Bâtiments",              "Immobilisations corporelles",   "Patrimoine administratif");
			         this.AddAssetsSamples (this.accessor.Mandat.StartDate.AddDays  (  0), "Voirie",                   "105", 1200000.0m, 1500000.0m,  500000.0m, "Dupond",   "Nicolet",  "Immobilier",             "Déchèteries",            "Immobilisations corporelles",   "Patrimoine administratif");
			         this.AddAssetsSamples (this.accessor.Mandat.StartDate.AddDays  (  0), "Déchèterie communale",     "106", 3500000.0m, 4100000.0m, 3000000.0m, "Dupond",   null,       "Immobilier",             "Déchèteries",            "Immobilisations corporelles",   "Patrimoine administratif");
			var i2 = this.AddAssetsSamples (this.accessor.Mandat.StartDate.AddDays  (  0), "Collège de Marcolet",      "200", 5100000.0m, 7500000.0m, 4000000.0m, "Dupond",   null,       "Immobilier",             "Ecoles",                 "Immobilisations corporelles",   "Patrimoine administratif");
			         this.AddAssetsSamples (this.accessor.Mandat.StartDate.AddDays  (  0), "Ecole des Trois Sapins",   "201", 3200000.0m, 3300000.0m, 3000000.0m, "Dupond",   null,       "Immobilier",             "Ecoles",                 "Immobilisations corporelles",   "Patrimoine administratif");
			         this.AddAssetsSamples (this.accessor.Mandat.StartDate.AddYears (  2), "STEP intercommunale",      "115", 3200000.0m, 4000000.0m, 2500000.0m, "Dubosson", "Nicolet",  "Immobilier",             "Traitement des eaux",    "Immobilisations corporelles",   "Patrimoine administratif");
			         this.AddAssetsSamples (this.accessor.Mandat.StartDate.AddDays  ( 50), "Scania X-20",              "200",  142000.0m,  160000.0m,  150000.0m, "Dupond",   "Nicolet",  "Véhicules",              "Camions",                "Immobilisations corporelles",   "Patrimoine administratif");
			         this.AddAssetsSamples (this.accessor.Mandat.StartDate.AddDays  ( 31), "Scania X-45",              "201",   84000.0m,  100000.0m,  110000.0m, "Dupond",   "Nicolet",  "Véhicules",              "Camions",                "Immobilisations corporelles",   "Patrimoine administratif");
			         this.AddAssetsSamples (this.accessor.Mandat.StartDate.AddDays  ( 31), "Volvo Truck P2",           "205",   90000.0m,  100000.0m,  100000.0m, "Nicolet",  "Zumstein", "Véhicules",              "Camions",                "Immobilisations corporelles",   "Patrimoine administratif");
			var v1 = this.AddAssetsSamples (this.accessor.Mandat.StartDate.AddYears (  1), "Fiat Uno",                 "300",    8000.0m,   20000.0m,   10000.0m, "Nicolet",  null,       "Véhicules",              "Voitures",               "Immobilisations corporelles",   "Patrimoine administratif");
			var v2 = this.AddAssetsSamples (this.accessor.Mandat.StartDate.AddDays  (100), "Citroën C4 Picasso",       "304",   22000.0m,   35000.0m,   35000.0m, "Nicolet",  null,       "Véhicules",              "Voitures",               "Immobilisations corporelles",   "Patrimoine administratif");
			         this.AddAssetsSamples (this.accessor.Mandat.StartDate.AddDays  (  0), "Parcelle du Crêt-au-Clos", "400", 1000000.0m,       null,  900000.0m, "Dupond",   "Nicolet",  "Immobilier",             "Terrains",               "Immobilisations corporelles",   "Patrimoine administratif");
			         this.AddAssetsSamples (this.accessor.Mandat.StartDate.AddDays  (  0), "Participations Nestlé",    "500",  300000.0m,       null,  290000.0m, "Zumstein", null,       "Amortissements manuels", "Autres immobilisations", "Immobilisations incorporelles", "Patrimoine financier");
			var p1 = this.AddAssetsSamples (this.accessor.Mandat.StartDate.AddDays  (  0), "Participations Logitech",  "501",   10000.0m,       null,   15000.0m, "Zumstein", null,       "Amortissements manuels", "Autres immobilisations", "Immobilisations incorporelles", "Patrimoine financier");
			         this.AddAssetsSamples (this.accessor.Mandat.StartDate.AddDays  (200), "Participations Raifeisen", "502",  250000.0m,       null,  250000.0m, "Dubosson", null,       "Amortissements manuels", "Autres immobilisations", "Immobilisations incorporelles", "Patrimoine financier");

			{
				var e = this.AddAssetEvent (i1, this.accessor.Mandat.StartDate.AddYears (1), EventType.Modification);
				this.AddAssetComputedAmount (e, this.fieldAssetValue1, 3600000.0m);
				this.AddAssetComputedAmount (e, this.fieldAssetValue2, 1900000.0m);
			}

			{
				var e = this.AddAssetEvent (i1, this.accessor.Mandat.StartDate.AddYears (2), EventType.Modification);
				this.AddAssetComputedAmount (e, this.fieldAssetValue1, 3800000.0m);
			}

			{
				var e = this.AddAssetEvent (i2, this.accessor.Mandat.StartDate.AddYears (1).AddDays (-1), EventType.AmortizationExtra);
				this.AddAssetAmortizedAmount (e, 4200000.0m, 3600000.0m);
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

			{
				var e = this.AddAssetEvent (p1, this.accessor.Mandat.StartDate.AddYears (2), EventType.Output);
				this.AddAssetAmortizedAmount (e, 0.0m);
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

			//?for (int i=0; i<5000; i++)
			//?{
			//?	this.AddPersonSample ("Monsieur", "Daniel", "Roux-"+i.ToString(), "Epsitec SA", "Rte de Neuchâtel 32", "1400", "Yverdon-les-Bains", "Suisse", "024 444 11 22", "022 871 98 76", null, "roux@epsitec.ch");
			//?}
		}


		protected override void CreateGroupsSamples()
		{
			var root = this.AddGroup (null, "Groupes", null);

			this.CreateGroupsCatsMCH2Samples  (root);
			this.CreateGroupsTypesMCH2Samples (root);
			this.CreateGroupsPatsMCH2Samples  (root);
		}

		public void CreateGroupsCatsMCH2Samples(DataObject parent)
		{
			var root = this.AddGroup (parent, "Catégories MCH2", "100", true);

			          this.AddGroup (root, "Terrains",                "10");
			          this.AddGroup (root, "Routes",                  "15");
			          this.AddGroup (root, "Traitement des eaux",     "20");
			          this.AddGroup (root, "Traveaux de génie civil", "30");
			var imm = this.AddGroup (root, "Immeubles",               "40");
			          this.AddGroup (root, "Mobilier",                "45");
			var veh = this.AddGroup (root, "Véhicules",               "50");
			          this.AddGroup (root, "Machines",                "55");
			          this.AddGroup (root, "En construction",         "60");
			          this.AddGroup (root, "Autres immobilisations",  "90");

			this.AddGroup (imm, "Bâtiments",   "10");
			this.AddGroup (imm, "Ecoles",      "20");
			this.AddGroup (imm, "Dépôts",      "30");
			this.AddGroup (imm, "Déchèteries", "40");

			this.AddGroup (veh, "Camions",      "10");
			this.AddGroup (veh, "Camionnettes", "20");
			this.AddGroup (veh, "Voitures",     "30");
		}

		private void CreateGroupsTypesMCH2Samples(DataObject parent)
		{
			var root = this.AddGroup (parent, "Types MCH2", "200", true);

			this.AddGroup (root, "Immobilisations corporelles",   "10");
			this.AddGroup (root, "Immobilisations incorporelles", "20");
			this.AddGroup (root, "Immobilisations financières",   "30");
		}

		private void CreateGroupsPatsMCH2Samples(DataObject parent)
		{
			var root = this.AddGroup (parent, "Patrimoine MCH2", "300", true);

			this.AddGroup (root, "En construction",          "10");
			this.AddGroup (root, "Patrimoine administratif", "20");
			this.AddGroup (root, "Patrimoine financier",     "20");
		}


		protected override void CreateCatsSamples()
		{
			this.AddCat ("Amortissements manuels",
				"Un objet de cette catégorie ne sera jamais amorti automatiquement.",
				"0", "Aucun", Periodicity.Annual);

			this.AddCat ("Immobilier", null, "10", "Durée linéaire", Periodicity.Annual,
				".14040.01", ".10020.01",
				".14040.01", ".10020.01",
				".10010.01", ".14040.01",
				"0290.3300.40", ".14040.01",
				"0290.3300.40", ".14040.01",
				"9610.3499.00", ".14040.01",
				"9610.3499.00", ".14040.01",
				"9610.3499.00", ".14040.01",
				null, 20.0m, 1.0m, 1.0m);

			this.AddCat ("Véhicules", null, "20", "Durée linéaire", Periodicity.Annual,
				".14040.01", ".10020.01",
				".14040.01", ".10020.01",
				".10010.01", ".14060.01",
				"0290.3300.60", ".14060.01",
				"0290.3300.60", ".14060.01",
				"9610.3499.00", ".14060.01",
				"9610.3499.00", ".14060.01",
				"9610.3499.00", ".14060.01",
				null, 10.0m, 1.0m, 1.0m);
		}


		protected override void AddReports()
		{
			var dateRange = new DateRange (this.accessor.Mandat.StartDate, this.accessor.Mandat.StartDate.AddYears (1));
			var initialTimestamp = new Timestamp (this.accessor.Mandat.StartDate, 0);
			var finalTimestamp   = new Timestamp (new System.DateTime (2099, 12, 31), 0);

			this.accessor.Mandat.Reports.Add (new MCH2SummaryParams (null, dateRange, Guid.Empty, 1, Guid.Empty, directMode: true));

			{
				var group  = this.GetGroup ("Catégories MCH2");
				var filter = this.GetGroup ("Patrimoine administratif");
				this.accessor.Mandat.Reports.Add (new MCH2SummaryParams ("Patrimoine administratif MCH2 &lt;DATE&gt; &lt;DIRECTMODE&gt;", dateRange, group.Guid, 1, filter.Guid, directMode: true));
			}

			{
				var group  = this.GetGroup ("Catégories MCH2");
				var filter = this.GetGroup ("Patrimoine financier");
				this.accessor.Mandat.Reports.Add (new MCH2SummaryParams ("Patrimoine financier MCH2 &lt;DATE&gt; &lt;DIRECTMODE&gt;", dateRange, group.Guid, 1, filter.Guid, directMode: true));
			}

			this.accessor.Mandat.Reports.Add (new AssetsParams ("Etat initial au &lt;DATE&gt;", initialTimestamp, Guid.Empty, null));
			this.accessor.Mandat.Reports.Add (new AssetsParams ("Etat final", finalTimestamp, Guid.Empty, null));

			this.accessor.Mandat.Reports.Add (new PersonsParams ());
		}
	}
}
