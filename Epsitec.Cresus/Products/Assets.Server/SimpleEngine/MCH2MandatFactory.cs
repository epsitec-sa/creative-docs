//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using System.Text;
using Epsitec.Cresus.Assets.Server.BusinessLogic;

namespace Epsitec.Cresus.Assets.Server.SimpleEngine
{
	/// <summary>
	/// Cette classe sait fabriquer un nouveau mandat MCH2, tout beau tout propre.
	/// </summary>
	public class MCH2MandatFactory : AbstractMandatFactory, System.IDisposable
	{
		public MCH2MandatFactory(DataAccessor accessor)
			: base (accessor)
		{
		}

		public void Dispose()
		{
		}


		public override DataMandat Create(string name, System.DateTime startDate, bool withSamples)
		{
			this.withSamples = withSamples;

			this.accessor.Mandat = new DataMandat (name, startDate);

			this.AddAssetsSettings ();
			this.AddPersonsSettings ();

			this.CreateGroupsSamples ();
			this.CreateCatsSamples ();

			if (this.withSamples)
			{
				this.AddPersonsSamples ();
				this.AddAssetsSamples ();
			}

			return this.accessor.Mandat;
		}


		protected override void AddAssetsSettings()
		{
			base.AddAssetsSettings ();

			if (this.withSamples)
			{
				this.fieldAssetOwner1 = this.AddSettings (BaseType.Assets, "Responsable", FieldType.GuidPerson, 150, null, null, null, 10);
				this.fieldAssetOwner2 = this.AddSettings (BaseType.Assets, "Remplaçant",  FieldType.GuidPerson, 150, null, null, null,  0);
			}
		}


		protected override void AddAssetsSamples()
		{
			this.AddAssetsSamples (this.accessor.Mandat.StartDate.AddDays  (  0), "Bâtiment administratif",   "100.10", 2400000.0m, "Dupond",   "Nicolet",  "Immobilier", "Bâtiments",              "Immobilisations corporelles",   "Administratif");
			this.AddAssetsSamples (this.accessor.Mandat.StartDate.AddDays  (  0), "Voirie",                   "100.20", 1200000.0m, "Dupond",   "Nicolet",  "Immobilier", "Décheteries",            "Immobilisations corporelles",   "Administratif");
			this.AddAssetsSamples (this.accessor.Mandat.StartDate.AddDays  (  0), "Déchéterie communale",     "100.25", 3500000.0m, "Dupond",   null,       "Immobilier", "Décheteries",            "Immobilisations corporelles",   "Administratif");
			this.AddAssetsSamples (this.accessor.Mandat.StartDate.AddDays  (  0), "Collège de Marcolet",      "200.10", 5100000.0m, "Dupond",   null,       "Immobilier", "Ecoles",                 "Immobilisations corporelles",   "Administratif");
			this.AddAssetsSamples (this.accessor.Mandat.StartDate.AddDays  (  0), "Ecole des trois sapins",   "200.11", 3200000.0m, "Dupond",   null,       "Immobilier", "Ecoles",                 "Immobilisations corporelles",   "Administratif");
			this.AddAssetsSamples (this.accessor.Mandat.StartDate.AddYears (  2), "STEP intercommunale",      "300.20", 3200000.0m, "Dubosson", "Nicolet",  "Immobilier", "Traitement des eaux",    "Immobilisations corporelles",   "Administratif");
			this.AddAssetsSamples (this.accessor.Mandat.StartDate.AddDays  ( 50), "Scania X-20",              "500.10",  142000.0m, "Dupond",   "Nicolet",  "Véhicules",  "Camions",                "Immobilisations corporelles",   "Administratif");
			this.AddAssetsSamples (this.accessor.Mandat.StartDate.AddDays  ( 31), "Scania X-45",              "500.11",   84000.0m, "Dupond",   "Nicolet",  "Véhicules",  "Camions",                "Immobilisations corporelles",   "Administratif");
			this.AddAssetsSamples (this.accessor.Mandat.StartDate.AddDays  ( 31), "Volvo Truck P2",           "500.13",   90000.0m, "Nicolet",  "Zumstein", "Véhicules",  "Camions",                "Immobilisations corporelles",   "Administratif");
			this.AddAssetsSamples (this.accessor.Mandat.StartDate.AddYears (  1), "Fiat Uno",                 "520.10",    8000.0m, "Nicolet",  null,       "Véhicules",  "Voitures",               "Immobilisations corporelles",   "Administratif");
			this.AddAssetsSamples (this.accessor.Mandat.StartDate.AddDays  (100), "Citroën C4 Picasso",       "520.14",   22000.0m, "Nicolet",  null,       "Véhicules",  "Voitures",               "Immobilisations corporelles",   "Administratif");
			this.AddAssetsSamples (this.accessor.Mandat.StartDate.AddDays  (  0), "Parcelle du Crêt-au-Clos", "800.10", 1000000.0m, "Dupond",   "Nicolet",  "Immobilier", "Terrains",               "Immobilisations corporelles",   "Administratif");
			this.AddAssetsSamples (this.accessor.Mandat.StartDate.AddDays  (  0), "Participations Nestlé",    "900.10",  250000.0m, "Zumstein", null,       null,         "Autres immobilisations", "Immobilisations incorporelles", "Financier");
			this.AddAssetsSamples (this.accessor.Mandat.StartDate.AddDays  (  0), "Participations Logitech",  "900.11",   10000.0m, "Zumstein", null,       null,         "Autres immobilisations", "Immobilisations incorporelles", "Financier");
			this.AddAssetsSamples (this.accessor.Mandat.StartDate.AddDays  (200), "Participations Raifeisen", "900.12",  300000.0m, "Dubosson", null,       null,         "Autres immobilisations", "Immobilisations incorporelles", "Financier");
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
			this.AddCat ("Immobilier", "10", 0.10m, AmortizationType.Linear,     Periodicity.Annual, ProrataType.Prorata12, 1000.0m, 1000.0m);
			this.AddCat ("Véhicules",  "20", 0.20m, AmortizationType.Degressive, Periodicity.Annual, ProrataType.Prorata12,  100.0m,  100.0m);
		}
	}
}
