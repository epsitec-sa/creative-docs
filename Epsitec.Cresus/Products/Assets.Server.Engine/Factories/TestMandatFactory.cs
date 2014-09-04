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
	/// Cas test MCH2 fourni par Olivier le 02.07.2014.
	/// </summary>
	public class TestMandatFactory : AbstractMandatFactory
	{
		public TestMandatFactory(DataAccessor accessor)
			: base (accessor)
		{
			this.amortizations = new Amortizations (this.accessor);
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

			this.AddReports ();

			return this.accessor.Mandat;
		}


		protected override void AddAssetsSettings()
		{
			base.AddAssetsSettings ();

			if (this.withSamples)
			{
				this.fieldAssetValue1 = this.AddSettings (BaseType.Assets, "Valeur remplacement", FieldType.ComputedAmount, false, 120, null, null, null, 10);
				this.fieldAssetValue2 = this.AddSettings (BaseType.Assets, "Valeur fiscale",      FieldType.ComputedAmount, false, 120, null, null, null,  0);
				this.fieldAssetOwner1 = this.AddSettings (BaseType.Assets, "Responsable",         FieldType.GuidPerson,     false, 150, null, null, null, 10);
				this.fieldAssetOwner2 = this.AddSettings (BaseType.Assets, "Remplaçant",          FieldType.GuidPerson,     false, 150, null, null, null,  0);
				this.fieldAssetDesc   = this.AddSettings (BaseType.Assets, "Description",         FieldType.String,         false, 120,  380,    5, null, 10);
			}
		}


		protected override void AddAssetsSamples()
		{
			var year0 = this.accessor.Mandat.StartDate.AddYears (0);
			var year1 = this.accessor.Mandat.StartDate.AddYears (1);
			var year2 = this.accessor.Mandat.StartDate.AddYears (2);
			var year3 = this.accessor.Mandat.StartDate.AddYears (3);

			var range0 = new DateRange (year0, year1);
			var range1 = new DateRange (year1, year2);
			var range2 = new DateRange (year2, year3);

			//	Bâtiment communal
			{
				var obj = this.AddAssetsSamples (year0.AddDays (0), "Bâtiment communal", "100", 1000000.0m, 1200000.0m, 1800000.0m, "Dupond", "Nicolet", "Immobilier", "Bâtiments", "Immobilisations corporelles", "Patrimoine administratif");

				{
					var e = this.AddAssetEvent (obj, year0.AddDays (400), EventType.Revaluation);
					this.AddAssetAmortizedAmount (e, 1200000.0m);
				}

				this.Amortize (range0, obj.Guid);
				this.Amortize (range1, obj.Guid);
				this.Amortize (range2, obj.Guid);
			}

			//	Ecole
			{
				var obj = this.AddAssetsSamples (year0.AddDays (0), "Ecole", "101", 500000.0m, 600000.0m, 450000.0m, "Dupond", "Nicolet", "Immobilier", "Bâtiments", "Immobilisations corporelles", "Patrimoine administratif");
				this.Amortize (range0, obj.Guid);
				this.Amortize (range1, obj.Guid);
				this.Amortize (range2, obj.Guid);
			}

			//	Camion
			{
				var obj = this.AddAssetsSamples (year0.AddDays (0), "Camion", "200", 100000.0m, 80000.0m, 10000.0m, "Nicolet", null, "Véhicules", "Camions", "Immobilisations corporelles", "Patrimoine administratif");
				this.Amortize (range0, obj.Guid);
				this.Amortize (range1, obj.Guid);
				this.Amortize (range2, obj.Guid);
			}

			//	Voiture grise
			{
				var obj = this.AddAssetsSamples (year0.AddDays (0), "Voiture grise", "201", 40000.0m, 35000.0m, 10000.0m, "Nicolet", null, "Véhicules", "Voitures", "Immobilisations corporelles", "Patrimoine administratif");

				{
					var e = this.AddAssetEvent (obj, year0.AddDays (188), EventType.Output);
					this.AddAssetAmortizedAmount (e, 0.0m);
				}
			}

			//	Voiture blanche
			{
				var obj = this.AddAssetsSamples (year0.AddDays (35), "Voiture blanche", "202", 35000.0m, 25000.0m, 10000.0m, "Nicolet", null, "Véhicules", "Voitures", "Immobilisations corporelles", "Patrimoine administratif");
				this.Amortize (range0, obj.Guid);

				{
					var e = this.AddAssetEvent (obj, year1.AddDays (190), EventType.Output);
					this.AddAssetAmortizedAmount (e, 0.0m);
				}
			}

			//	Voiture noire
			{
				var obj = this.AddAssetsSamples (year0.AddDays (200), "Voiture noire", "203", 25000.0m, 24000.0m, 10000.0m, "Nicolet", null, "Véhicules", "Voitures", "Immobilisations corporelles", "Patrimoine administratif");
				this.Amortize (range0, obj.Guid);
				this.Amortize (range1, obj.Guid);
				this.Amortize (range2, obj.Guid);
			}

			//	Immeuble locatif
			{
				var obj = this.AddAssetsSamples (year2.AddDays (100), "Immeuble locatif", "150", 2000000.0m, 1200000.0m, 1800000.0m, "Dupond", "Nicolet", "Immobilier", "Bâtiments", "Immobilisations corporelles", "Patrimoine financier");
				this.Amortize (range2, obj.Guid);
			}
		}

		private void Amortize(DateRange range, Guid objectGuid)
		{
			this.amortizations.Create (range, objectGuid);
			this.amortizations.Fix (objectGuid);
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

			this.AddGroup (imm, "Bâtiments",   "10", groupUsedDuringCreation: true);
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

			this.AddGroup (root, "Patrimoine administratif", "10", groupUsedDuringCreation: true);
			this.AddGroup (root, "Patrimoine financier",     "20");
		}


		protected override void CreateCatsSamples()
		{
			this.AddCat ("Immobilier", null, "10", 0.04m, AmortizationType.Linear,
				Periodicity.Annual, ProrataType.None, 1.0m, 1.0m,
				"1000", "1010", "1600", "1600", "6930", "6900");

			this.AddCat ("Véhicules", null, "20", 0.10m, AmortizationType.Linear,
				Periodicity.Annual, ProrataType.None, 1.0m, 1.0m,
				"1000", "1010", "1530", "1530", "6920", "6900");
		}


		protected override void AddReports()
		{
			var dateRange = new DateRange (this.accessor.Mandat.StartDate, this.accessor.Mandat.StartDate.AddYears (1));
			var timestamp = new Timestamp (this.accessor.Mandat.StartDate, 0);

			this.accessor.Mandat.Reports.Add (new MCH2SummaryParams (dateRange, Guid.Empty, 1, Guid.Empty));

			{
				var group = this.GetGroup ("Catégories MCH2");
				var filter = this.GetGroup ("Patrimoine administratif");
				this.accessor.Mandat.Reports.Add (new MCH2SummaryParams (dateRange, group.Guid, 1, filter.Guid));
			}

			{
				var group = this.GetGroup ("Catégories MCH2");
				var filter = this.GetGroup ("Patrimoine financier");
				this.accessor.Mandat.Reports.Add (new MCH2SummaryParams (dateRange, group.Guid, 1, filter.Guid));
			}

			this.accessor.Mandat.Reports.Add (new AssetsParams (timestamp, Guid.Empty, null));
			this.accessor.Mandat.Reports.Add (new PersonsParams ());
		}


		private readonly Amortizations amortizations;
	}
}
