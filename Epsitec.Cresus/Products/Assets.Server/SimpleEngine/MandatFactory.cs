//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using System.Text;
using Epsitec.Cresus.Assets.Server.BusinessLogic;

namespace Epsitec.Cresus.Assets.Server.SimpleEngine
{
	/// <summary>
	/// Cette classe sait fabriquer un nouveau mandat de base, tout beau tout propre.
	/// </summary>
	public class MandatFactory : System.IDisposable
	{
		public MandatFactory(DataAccessor accessor)
		{
			this.accessor = accessor;
		}

		public void Dispose()
		{
		}


		public DataMandat Create(string name, System.DateTime startDate, bool withSamples)
		{
			this.withSamples = withSamples;

			this.accessor.Mandat = new DataMandat (name, startDate);

			this.AddAssetsSettings ();
			this.AddPersonsSettings ();

			this.CreateGroupsMCH2Samples ();
			this.CreateCatsMCH2Samples ();

			if (this.withSamples)
			{
				this.AddPersonsSamples ();
				this.AddAssetsSamples ();
			}

			return this.accessor.Mandat;
		}


		private void AddAssetsSettings()
		{
			this.fieldAssetName   = this.AddSettings (BaseType.Assets, "Nom",         FieldType.String, 180, 380, 1,    1,  0);
			this.fieldAssetNumber = this.AddSettings (BaseType.Assets, "Numéro",      FieldType.String,  90,  90, 1, null,  0);
			this.fieldAssetDesc   = this.AddSettings (BaseType.Assets, "Description", FieldType.String, 120, 380, 5, null, 10);

			if (this.withSamples)
			{
				this.fieldAssetOwner1 = this.AddSettings (BaseType.Assets, "Responsable", FieldType.GuidPerson, 150, null, null, null, 10);
				this.fieldAssetOwner2 = this.AddSettings (BaseType.Assets, "Remplaçant",  FieldType.GuidPerson, 150, null, null, null,  0);
			}
		}

		private void AddPersonsSettings()
		{
			this.fieldPersonLastName  = this.AddSettings (BaseType.Persons, "Nom",           FieldType.String, 120, 380, 1, 2,     0);
			this.fieldPersonFirstName = this.AddSettings (BaseType.Persons, "Prénom",        FieldType.String, 120, 380, 1, 1,     0);
			this.fieldPersonTitle     = this.AddSettings (BaseType.Persons, "Titre",         FieldType.String,  80, 120, 1, null,  0);
			this.fieldPersonCompany   = this.AddSettings (BaseType.Persons, "Entreprise",    FieldType.String, 120, 380, 1, 3,     0);
			this.fieldPersonAddress   = this.AddSettings (BaseType.Persons, "Adresse",       FieldType.String, 150, 380, 2, null,  0);
			this.fieldPersonZip       = this.AddSettings (BaseType.Persons, "NPA",           FieldType.String,  50,  60, 1, null,  0);
			this.fieldPersonCity      = this.AddSettings (BaseType.Persons, "Ville",         FieldType.String, 120, 380, 1, null,  0);
			this.fieldPersonCountry   = this.AddSettings (BaseType.Persons, "Pays",          FieldType.String, 120, 380, 1, null,  0);
			this.fieldPersonPhone1    = this.AddSettings (BaseType.Persons, "Tél. prof.",    FieldType.String, 100, 120, 1, null, 10);
			this.fieldPersonPhone2    = this.AddSettings (BaseType.Persons, "Tél. privé",    FieldType.String, 100, 120, 1, null,  0);
			this.fieldPersonPhone3    = this.AddSettings (BaseType.Persons, "Tél. portable", FieldType.String, 100, 120, 1, null,  0);
			this.fieldPersonMail      = this.AddSettings (BaseType.Persons, "E-mail",        FieldType.String, 200, 380, 1, null,  0);
			this.fieldPersonDesc      = this.AddSettings (BaseType.Persons, "Description",   FieldType.String, 200, 380, 5, null, 10);
		}

		private ObjectField AddSettings(BaseType baseType, string name, FieldType type, int columnWidth, int? lineWidth, int? lineCount, int? summaryOrder, int topMargin)
		{
			var field = this.accessor.Mandat.Settings.GetNewUserField ();
			this.accessor.Mandat.Settings.AddUserField (baseType, new UserField (name, field, type, columnWidth, lineWidth, lineCount, summaryOrder, topMargin));
			return field;
		}



		private void AddAssetsSamples()
		{
			this.AddAssetsSamples (this.accessor.Mandat.StartDate.AddDays  (  0), "Bâtiment administratif",   "100.10", 2400000.0m, "Dupond",   "Nicolet",  "Bâtiments",              "Immobilisations corporelles",   "Administratif");
			this.AddAssetsSamples (this.accessor.Mandat.StartDate.AddDays  (  0), "Voirie",                   "100.20", 1200000.0m, "Dupond",   "Nicolet",  "Décheteries",            "Immobilisations corporelles",   "Administratif");
			this.AddAssetsSamples (this.accessor.Mandat.StartDate.AddDays  (  0), "Déchéterie communale",     "100.25", 3500000.0m, "Dupond",   null,       "Décheteries",            "Immobilisations corporelles",   "Administratif");
			this.AddAssetsSamples (this.accessor.Mandat.StartDate.AddDays  (  0), "Collège de Marcolet",      "200.10", 5100000.0m, "Dupond",   null,       "Ecoles",                 "Immobilisations corporelles",   "Administratif");
			this.AddAssetsSamples (this.accessor.Mandat.StartDate.AddDays  (  0), "Ecole des trois sapins",   "200.11", 3200000.0m, "Dupond",   null,       "Ecoles",                 "Immobilisations corporelles",   "Administratif");
			this.AddAssetsSamples (this.accessor.Mandat.StartDate.AddYears (  2), "STEP intercommunale",      "300.20", 3200000.0m, "Dubosson", "Nicolet",  "Traitement des eaux",    "Immobilisations corporelles",   "Administratif");
			this.AddAssetsSamples (this.accessor.Mandat.StartDate.AddDays  ( 50), "Scania X-20",              "500.10",  142000.0m, "Dupond",   "Nicolet",  "Camions",                "Immobilisations corporelles",   "Administratif");
			this.AddAssetsSamples (this.accessor.Mandat.StartDate.AddDays  ( 31), "Scania X-45",              "500.11",   84000.0m, "Dupond",   "Nicolet",  "Camions",                "Immobilisations corporelles",   "Administratif");
			this.AddAssetsSamples (this.accessor.Mandat.StartDate.AddDays  ( 31), "Volvo Truck P2",           "500.13",   90000.0m, "Nicolet",  "Zumstein", "Camions",                "Immobilisations corporelles",   "Administratif");
			this.AddAssetsSamples (this.accessor.Mandat.StartDate.AddYears (  1), "Fiat Uno",                 "520.10",    8000.0m, "Nicolet",  null,       "Voitures",               "Immobilisations corporelles",   "Administratif");
			this.AddAssetsSamples (this.accessor.Mandat.StartDate.AddDays  (100), "Citroën C4 Picasso",       "520.14",   22000.0m, "Nicolet",  null,       "Voitures",               "Immobilisations corporelles",   "Administratif");
			this.AddAssetsSamples (this.accessor.Mandat.StartDate.AddDays  (  0), "Parcelle du Crêt-au-Clos", "800.10", 1000000.0m, "Dupond",   "Nicolet",  "Terrains",               "Immobilisations corporelles",   "Administratif");
			this.AddAssetsSamples (this.accessor.Mandat.StartDate.AddDays  (  0), "Participations Nestlé",    "900.10",  250000.0m, "Zumstein", null,       "Autres immobilisations", "Immobilisations incorporelles", "Financier");
			this.AddAssetsSamples (this.accessor.Mandat.StartDate.AddDays  (  0), "Participations Logitech",  "900.11",   10000.0m, "Zumstein", null,       "Autres immobilisations", "Immobilisations incorporelles", "Financier");
			this.AddAssetsSamples (this.accessor.Mandat.StartDate.AddDays  (200), "Participations Raifaisen", "900.12",  300000.0m, "Dubosson", null,       "Autres immobilisations", "Immobilisations incorporelles", "Financier");
		}

		private void AddAssetsSamples(System.DateTime date, string name, string number, decimal value, string owner1, string owner2, params string[] groups)
		{
			var guid = this.accessor.CreateObject (BaseType.Assets, date, name, Guid.Empty);
			var o = this.accessor.GetObject (BaseType.Assets, guid);

			var e = o.GetEvent (0);

			this.AddField (e, this.fieldAssetNumber, number);
			this.AddAssetAmortizedAmount (e, value);

			this.AddAssetPerson (e, this.fieldAssetOwner1, owner1);
			this.AddAssetPerson (e, this.fieldAssetOwner2, owner2);

			int i = 0;
			foreach (var group in groups)
			{
				this.AddAssetGroup (e, i++, group);
			}
		}

		private void AddAssetAmortizedAmount(DataEvent e, decimal value)
		{
			var p = e.GetProperty (ObjectField.MainValue) as DataAmortizedAmountProperty;
			var aa = p.Value;

			aa.InitialAmount = value;
		}

		private void AddAssetPerson(DataEvent e, ObjectField field, string lastName)
		{
			if (!string.IsNullOrEmpty (lastName))
			{
				var person = this.GetPerson (lastName);
				this.AddField (e, field, person.Guid);
			}
		}

		private void AddAssetGroup(DataEvent e, int index, string groupName)
		{
			if (!string.IsNullOrEmpty (groupName))
			{
				e.AddProperty (new DataGuidRatioProperty (ObjectField.GroupGuidRatioFirst+index, this.GetGroup (groupName)));
			}
		}


		private void AddPersonsSamples()
		{
			this.AddPersonSample ("Monsieur", "Jean",      "Dupond",   null,                 "av. des Planches 12 bis",                     "1023", "Crissier",             "Suisse", null,            null,            null,            "jeandupond@bluewin.ch");
			this.AddPersonSample ("Madame",   "Renata",    "Zumstein", null,                 "Crésentine 21",                               "1023", "Crissier",             "Suisse", "021 512 44 55", null,            null,            "zumstein@crissier.ch");
			this.AddPersonSample ("Monsieur", "Alfred",    "Dubosson", null,                 "ch. des Tilleuls 4",                          "1020", "Renens",               "Suisse", "021 512 44 55", "021 600 22 33", null,            "dubosson@crissier.ch");
			this.AddPersonSample ("Madame",   "Sandra",    "Nicolet",  null,                 "Place du Marché",                             "2000", "Neuchâtel",            "Suisse", null,            null,            "079 810 20 30", "sandranicolet5@gmail.com");
			this.AddPersonSample ("Madame",   "Sylvianne", "Galbato",  "Les Bons Tuyaux SA", "En Budron 12B",                               "1052", "Le Mont-sur-Lausanne", "Suisse", "021 312 28 29", null,            null,            "sylvianne@lesbonstuyaux.ch");
			this.AddPersonSample ("Monsieur", "André",     "Mercier",  "Mecatronic SA",      "Y-Parc - Swiss Technopole<br/>Rue Galilée 7", "1400", "Yverdon-les-Bains",    "Suisse", "024 444 11 22", "022 871 98 76", null,            "mercier@mecatronic.ch");
		}

		private void AddPersonSample(string title, string firstName, string lastName, string company, string address, string zip, string city, string country, string phone1, string phone2, string phone3, string mail)
		{
			var persons = this.accessor.Mandat.GetData (BaseType.Persons);
			var start  = new Timestamp (new System.DateTime (2000, 1, 1), 0);

			var o = new DataObject ();
			persons.Add (o);

			var e = new DataEvent (start, EventType.Input);
			o.AddEvent (e);

			this.AddField (e, this.fieldPersonTitle,     title);
			this.AddField (e, this.fieldPersonFirstName, firstName);
			this.AddField (e, this.fieldPersonLastName,  lastName);
			this.AddField (e, this.fieldPersonCompany,   company);
			this.AddField (e, this.fieldPersonAddress,   address);
			this.AddField (e, this.fieldPersonZip,       zip);
			this.AddField (e, this.fieldPersonCity,      city);
			this.AddField (e, this.fieldPersonCountry,   country);
			this.AddField (e, this.fieldPersonPhone1,    phone1);
			this.AddField (e, this.fieldPersonPhone2,    phone2);
			this.AddField (e, this.fieldPersonPhone3,    phone3);
			this.AddField (e, this.fieldPersonMail,      mail);
		}


		private void CreateGroupsMCH2Samples()
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

		private DataObject AddGroup(DataObject parent, string name, string number)
		{
			var groups = this.accessor.Mandat.GetData (BaseType.Groups);
			var start  = new Timestamp (new System.DateTime (2000, 1, 1), 0);

			var o = new DataObject ();
			groups.Add (o);

			var e = new DataEvent (start, EventType.Input);
			o.AddEvent (e);

			if (parent != null)
			{
				this.AddField (e, ObjectField.GroupParent, parent.Guid);
			}

			this.AddField(e, ObjectField.Name,   name);
			this.AddField(e, ObjectField.Number, number);

			return o;
		}


		private void CreateCatsMCH2Samples()
		{
			this.AddCat ("Immobilier", "10", 0.10m, AmortizationType.Linear,     Periodicity.Annual, ProrataType.Prorata12, 1000.0m, 1000.0m);
			this.AddCat ("Véhicules",  "20", 0.20m, AmortizationType.Degressive, Periodicity.Annual, ProrataType.Prorata12,  100.0m,  100.0m);
		}

		private void AddCat(string name, string number, decimal rate, AmortizationType type, Periodicity periodicity, ProrataType prorata, decimal round, decimal residual)
		{
			var cats = this.accessor.Mandat.GetData (BaseType.Categories);
			var start  = new Timestamp (new System.DateTime (2000, 1, 1), 0);

			var o = new DataObject ();
			cats.Add (o);

			var e = new DataEvent (start, EventType.Input);
			o.AddEvent (e);

			this.AddField (e, ObjectField.Name,             name);
			this.AddField (e, ObjectField.Number,           number);
			this.AddField (e, ObjectField.AmortizationRate, rate);
			this.AddField (e, ObjectField.AmortizationType, (int) type);
			this.AddField (e, ObjectField.Periodicity,      (int) periodicity);
			this.AddField (e, ObjectField.Prorata,          (int) prorata);
			this.AddField (e, ObjectField.Round,            round);
			this.AddField (e, ObjectField.ResidualValue,    residual);
		}


		private void AddField(DataEvent e, ObjectField field, string value)
		{
			if (!string.IsNullOrEmpty (value))
			{
				e.AddProperty (new DataStringProperty (field, value));
			}
		}

		private void AddField(DataEvent e, ObjectField field, int? value)
		{
			if (value.HasValue)
			{
				e.AddProperty (new DataIntProperty (field, value.Value));
			}
		}

		private void AddField(DataEvent e, ObjectField field, decimal? value)
		{
			if (value.HasValue)
			{
				e.AddProperty (new DataDecimalProperty (field, value.Value));
			}
		}

		private void AddField(DataEvent e, ObjectField field, Guid value)
		{
			if (!value.IsEmpty)
			{
				e.AddProperty (new DataGuidProperty (field, value));
			}
		}


		private DataObject GetPerson(string lastName)
		{
			var list = this.accessor.Mandat.GetData (BaseType.Persons);

			foreach (var person in list)
			{
				var s = ObjectProperties.GetObjectPropertyString (person, null, this.fieldPersonLastName);
				if (s == lastName)
				{
					return person;
				}
			}

			System.Diagnostics.Debug.Fail (string.Format ("La personne {0} n'existe pas !", lastName));
			return null;
		}

		private GuidRatio GetGroup(string text, decimal? ratio = null)
		{
			var list = this.accessor.Mandat.GetData (BaseType.Groups);

			foreach (var group in list)
			{
				var nom = ObjectProperties.GetObjectPropertyString (group, null, ObjectField.Name);
				if (nom == text)
				{
					return new GuidRatio (group.Guid, ratio);
				}
			}

			System.Diagnostics.Debug.Fail (string.Format ("Le groupe {0} n'existe pas !", text));
			return GuidRatio.Empty;
		}


		private readonly DataAccessor accessor;

		private bool withSamples;

		private ObjectField fieldAssetName;
		private ObjectField fieldAssetNumber;
		private ObjectField fieldAssetDesc;
		private ObjectField fieldAssetOwner1;
		private ObjectField fieldAssetOwner2;

		private ObjectField fieldPersonLastName;
		private ObjectField fieldPersonFirstName;
		private ObjectField fieldPersonTitle;
		private ObjectField fieldPersonCompany;
		private ObjectField fieldPersonAddress;
		private ObjectField fieldPersonZip;
		private ObjectField fieldPersonCity;
		private ObjectField fieldPersonCountry;
		private ObjectField fieldPersonPhone1;
		private ObjectField fieldPersonPhone2;
		private ObjectField fieldPersonPhone3;
		private ObjectField fieldPersonMail;
		private ObjectField fieldPersonDesc;
	}
}
