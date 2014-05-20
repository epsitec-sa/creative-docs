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

			this.accessor.Mandat = new DataMandat (name, startDate);

			this.AddAssetsSettings ();
			this.AddPersonsSettings ();

			if (this.withSamples)
			{
				DummyAccounts.AddAccounts (this.accessor.Mandat);
			}

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

			return this.accessor.Mandat;
		}


		protected override void AddAssetsSettings()
		{
			base.AddAssetsSettings ();

			if (this.withSamples)
			{
				this.fieldAssetValue1 = this.AddSettings (BaseType.Assets, "Valeur remplacement", FieldType.ComputedAmount, 150, null, null, null, 10);
				this.fieldAssetValue2 = this.AddSettings (BaseType.Assets, "Valeur fiscale",      FieldType.ComputedAmount, 150, null, null, null,  0);
				this.fieldAssetOwner1 = this.AddSettings (BaseType.Assets, "Responsable",         FieldType.GuidPerson,     150, null, null, null, 10);
				this.fieldAssetOwner2 = this.AddSettings (BaseType.Assets, "Remplaçant",          FieldType.GuidPerson,     150, null, null, null,  0);
			}
		}


		protected override void AddAssetsSamples()
		{
			var i1 = this.AddAssetsSamples (this.accessor.Mandat.StartDate.AddDays  (  0), "Bâtiment administratif",   "100.10", 2400000.0m, 3500000.0m, 2100000.0m, "Dupond",   "Nicolet",  "Immobilier", "Bâtiments",              "Immobilisations corporelles",   "Administratif");
			         this.AddAssetsSamples (this.accessor.Mandat.StartDate.AddDays  (  0), "Voirie",                   "100.20", 1200000.0m, 1500000.0m,  500000.0m, "Dupond",   "Nicolet",  "Immobilier", "Décheteries",            "Immobilisations corporelles",   "Administratif");
			         this.AddAssetsSamples (this.accessor.Mandat.StartDate.AddDays  (  0), "Déchéterie communale",     "100.25", 3500000.0m, 4100000.0m, 3000000.0m, "Dupond",   null,       "Immobilier", "Décheteries",            "Immobilisations corporelles",   "Administratif");
			var i2 = this.AddAssetsSamples (this.accessor.Mandat.StartDate.AddDays  (  0), "Collège de Marcolet",      "200.10", 5100000.0m, 7500000.0m, 4000000.0m, "Dupond",   null,       "Immobilier", "Ecoles",                 "Immobilisations corporelles",   "Administratif");
			         this.AddAssetsSamples (this.accessor.Mandat.StartDate.AddDays  (  0), "Ecole des Trois Sapins",   "200.11", 3200000.0m, 3300000.0m, 3000000.0m, "Dupond",   null,       "Immobilier", "Ecoles",                 "Immobilisations corporelles",   "Administratif");
			         this.AddAssetsSamples (this.accessor.Mandat.StartDate.AddYears (  2), "STEP intercommunale",      "300.20", 3200000.0m, 4000000.0m, 2500000.0m, "Dubosson", "Nicolet",  "Immobilier", "Traitement des eaux",    "Immobilisations corporelles",   "Administratif");
			         this.AddAssetsSamples (this.accessor.Mandat.StartDate.AddDays  ( 50), "Scania X-20",              "500.10",  142000.0m,  160000.0m,  150000.0m, "Dupond",   "Nicolet",  "Véhicules",  "Camions",                "Immobilisations corporelles",   "Administratif");
			         this.AddAssetsSamples (this.accessor.Mandat.StartDate.AddDays  ( 31), "Scania X-45",              "500.11",   84000.0m,  100000.0m,  110000.0m, "Dupond",   "Nicolet",  "Véhicules",  "Camions",                "Immobilisations corporelles",   "Administratif");
			         this.AddAssetsSamples (this.accessor.Mandat.StartDate.AddDays  ( 31), "Volvo Truck P2",           "500.13",   90000.0m,  100000.0m,  100000.0m, "Nicolet",  "Zumstein", "Véhicules",  "Camions",                "Immobilisations corporelles",   "Administratif");
			var v1 = this.AddAssetsSamples (this.accessor.Mandat.StartDate.AddYears (  1), "Fiat Uno",                 "520.10",    8000.0m,   20000.0m,   10000.0m, "Nicolet",  null,       "Véhicules",  "Voitures",               "Immobilisations corporelles",   "Administratif");
			var v2 = this.AddAssetsSamples (this.accessor.Mandat.StartDate.AddDays  (100), "Citroën C4 Picasso",       "520.14",   22000.0m,   35000.0m,   35000.0m, "Nicolet",  null,       "Véhicules",  "Voitures",               "Immobilisations corporelles",   "Administratif");
			         this.AddAssetsSamples (this.accessor.Mandat.StartDate.AddDays  (  0), "Parcelle du Crêt-au-Clos", "800.10", 1000000.0m,       null,  900000.0m, "Dupond",   "Nicolet",  "Immobilier", "Terrains",               "Immobilisations corporelles",   "Administratif");
			         this.AddAssetsSamples (this.accessor.Mandat.StartDate.AddDays  (  0), "Participations Nestlé",    "900.10",  250000.0m,       null,  290000.0m, "Zumstein", null,       null,         "Autres immobilisations", "Immobilisations incorporelles", "Financier");
			         this.AddAssetsSamples (this.accessor.Mandat.StartDate.AddDays  (  0), "Participations Logitech",  "900.11",   10000.0m,       null,   15000.0m, "Zumstein", null,       null,         "Autres immobilisations", "Immobilisations incorporelles", "Financier");
			         this.AddAssetsSamples (this.accessor.Mandat.StartDate.AddDays  (200), "Participations Raifeisen", "900.12",  300000.0m,       null,  250000.0m, "Dubosson", null,       null,         "Autres immobilisations", "Immobilisations incorporelles", "Financier");

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

			this.CreateGroupsCatsMCH2Samples  (root);
			this.CreateGroupsTypesMCH2Samples (root);
			this.CreateGroupsPatsMCH2Samples  (root);
		}

		private void CreateGroupsCatsMCH2Samples(DataObject parent)
		{
			var root = this.AddGroup (parent, "Catégories MCH2", "100");

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
			this.AddGroup (imm, "Décheteries", "40");

			this.AddGroup (veh, "Camions",      "10");
			this.AddGroup (veh, "Camionnettes", "20");
			this.AddGroup (veh, "Voitures",     "30");
		}

		private void CreateGroupsTypesMCH2Samples(DataObject parent)
		{
			var root = this.AddGroup (parent, "Types MCH2", "200");

			this.AddGroup (root, "Immobilisations corporelles",   "10");
			this.AddGroup (root, "Immobilisations incorporelles", "20");
			this.AddGroup (root, "Immobilisations financières",   "30");
		}

		private void CreateGroupsPatsMCH2Samples(DataObject parent)
		{
			var root = this.AddGroup (parent, "Patrimoine MCH2", "300");

			this.AddGroup (root, "Administratif", "10");
			this.AddGroup (root, "Financier",     "20");
		}


		protected override void CreateCatsSamples()
		{
			this.AddCat ("Immobilier", "10", 0.10m, AmortizationType.Linear,     Periodicity.Annual, ProrataType.Prorata12, 1000.0m, 1.0m);
			this.AddCat ("Véhicules",  "20", 0.20m, AmortizationType.Degressive, Periodicity.Annual, ProrataType.Prorata12,  100.0m, 1.0m);
		}
	}
}
