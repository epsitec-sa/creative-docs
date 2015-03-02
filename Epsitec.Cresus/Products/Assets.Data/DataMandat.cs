//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.Data.Reports;
using Epsitec.Cresus.Assets.Data.Helpers;
using Epsitec.Cresus.Assets.Data.Serialization;

namespace Epsitec.Cresus.Assets.Data
{
	public class DataMandat
	{
		public DataMandat(ComputerSettings computerSettings, string name, System.DateTime startDate)
		{
			this.guid             = Guid.NewGuid ();
			this.computerSettings = computerSettings;
			this.name             = name;
			this.startDate        = startDate;

			this.undoManager    = new UndoManager ();
			this.globalSettings = new GlobalSettings (this.undoManager);

			this.assetsUserFields  = new GuidDictionary<DataObject> (this.undoManager);
			this.personsUserFields = new GuidDictionary<DataObject> (this.undoManager);
			this.assets            = new GuidDictionary<DataObject> (this.undoManager);
			this.categories        = new GuidDictionary<DataObject> (this.undoManager);
			this.groups            = new GuidDictionary<DataObject> (this.undoManager);
			this.persons           = new GuidDictionary<DataObject> (this.undoManager);
			this.entries           = new GuidDictionary<DataObject> (this.undoManager);
			this.methods           = new GuidDictionary<DataObject> (this.undoManager);
			this.arguments         = new GuidDictionary<DataObject> (this.undoManager);
			this.rangeAccounts     = new UndoableDictionary<DateRange, GuidDictionary<DataObject>> (this.undoManager);
			this.rangeVatCodes     = new UndoableDictionary<DateRange, GuidDictionary<DataObject>> (this.undoManager);
			this.reports           = new GuidDictionary<AbstractReportParams> (this.undoManager);
		}

		public DataMandat(ComputerSettings computerSettings, System.Xml.XmlReader reader)
		{
			this.computerSettings = computerSettings;
			this.undoManager      = new UndoManager ();
			this.globalSettings   = new GlobalSettings (this.undoManager);

			this.assetsUserFields  = new GuidDictionary<DataObject> (this.undoManager);
			this.personsUserFields = new GuidDictionary<DataObject> (this.undoManager);
			this.assets            = new GuidDictionary<DataObject> (this.undoManager);
			this.categories        = new GuidDictionary<DataObject> (this.undoManager);
			this.groups            = new GuidDictionary<DataObject> (this.undoManager);
			this.persons           = new GuidDictionary<DataObject> (this.undoManager);
			this.entries           = new GuidDictionary<DataObject> (this.undoManager);
			this.methods           = new GuidDictionary<DataObject> (this.undoManager);
			this.arguments         = new GuidDictionary<DataObject> (this.undoManager);
			this.rangeAccounts     = new UndoableDictionary<DateRange, GuidDictionary<DataObject>> (this.undoManager);
			this.rangeVatCodes     = new UndoableDictionary<DateRange, GuidDictionary<DataObject>> (this.undoManager);
			this.reports           = new GuidDictionary<AbstractReportParams> (this.undoManager);

			this.Deserialize (reader);
		}


		public Guid								Guid
		{
			get
			{
				return this.guid;
			}
		}

		public string							Name
		{
			get
			{
				return this.name;
			}
		}

		public System.DateTime					StartDate
		{
			get
			{
				return this.startDate;
			}
		}

		public GlobalSettings					GlobalSettings
		{
			get
			{
				return this.globalSettings;
			}
		}

		public UndoManager						UndoManager
		{
			get
			{
				return this.undoManager;
			}
		}

		public GuidDictionary<AbstractReportParams>	Reports
		{
			get
			{
				return this.reports;
			}
		}


		#region Mandat informations
		public MandatInfo						MandatInfo
		{
			get
			{
				var statistics = new MandatStatistics (this.assets.Count, this.EventCount, this.categories.Count,
					this.groups.Count, this.persons.Count, this.reports.Count, this.rangeAccounts.Count);

				return new MandatInfo (this.SoftwareKey, this.SoftwareVersion, this.SoftwareLanguage,
					this.name, this.guid, DataMandat.SerializationVersion, this.DocummentLanguage, statistics);
			}
		}

		private string							SoftwareKey
		{
			//	Retourne le numéro d'identification du logiciel (parfois appelé clé).
			get
			{
				return "02600-300001-3876-123456";  // TODO: obtenir le vrai !
			}
		}

		private string							SoftwareLanguage
		{
			//	Retourne la langue du logiciel.
			get
			{
				return this.computerSettings.SoftwareLanguage;
			}
		}

		private string							SoftwareVersion
		{
			//	Retourne le numéro de version (celle du projet Assets.Data).
			get
			{
				return typeof (DataMandat).Assembly.FullName.Split (',')[1].Split ('=')[1];
			}
		}

		private string							DocummentLanguage
		{
			//	Retourne la langue du logiciel.
			get
			{
				return this.globalSettings.MandatLanguage;
			}
		}

		public const string						SerializationVersion = "1.0";

		private int								EventCount
		{
			get
			{
				return this.assets.Select (x => x.EventsCount).Sum ();
			}
		}
		#endregion


		public GuidDictionary<DataObject> GetData(BaseType type)
		{
			switch (type.Kind)
			{
				case BaseTypeKind.AssetsUserFields:
					return this.assetsUserFields;

				case BaseTypeKind.PersonsUserFields:
					return this.personsUserFields;

				case BaseTypeKind.Assets:
					return this.assets;

				case BaseTypeKind.Categories:
					return this.categories;

				case BaseTypeKind.Groups:
					return this.groups;

				case BaseTypeKind.Persons:
					return this.persons;

				case BaseTypeKind.Entries:
					return this.entries;

				case BaseTypeKind.Accounts:
					return this.GetAccounts (type.AccountsDateRange);

				case BaseTypeKind.VatCodes:
					return this.GetVatCodes (type.AccountsDateRange);

				case BaseTypeKind.Methods:
					return this.methods;

				case BaseTypeKind.Arguments:
					return this.arguments;

				default:
					// Il vaut mieux retourner un dictionnaire vide, plutôt que null.
					return new GuidDictionary<DataObject> (this.undoManager);
			}
		}


		#region Accounts
		public IEnumerable<DateRange>			AccountsDateRanges
		{
			//	Retourne la liste des périodes de tous les plans comptables connus.
			get
			{
				return this.rangeAccounts.Select (x => x.Key);
			}
		}

		public BaseType GetAccountsBase(System.DateTime date)
		{
			//	Retourne la base correspondant à une date.
			//	Si plusieurs périodes se recouvrent, on prend la dernière définie.
			var range = this.GetBestAccountsDateRange (date);
			return new BaseType (BaseTypeKind.Accounts, range);
		}

		public GuidDictionary<DataObject> GetAccounts(DateRange range)
		{
			//	Retourne le plan comptable correspondant à une période.
			GuidDictionary<DataObject> accounts;
			if (!range.IsEmpty && this.rangeAccounts.TryGetValue (range, out accounts))
			{
				return accounts;
			}
			else
			{
				// Il vaut mieux retourner un dictionnaire vide, plutôt que null.
				return new GuidDictionary<DataObject> (this.undoManager);
			}
		}

		public void AddAccounts(DateRange dateRange, GuidDictionary<DataObject> accounts)
		{
			//	Prend connaissance d'un nouveau plan comptable, qui est ajouté ou
			//	qui remplace un existant, selon sa période.
			this.rangeAccounts[dateRange] = accounts;
		}

		public DateRange GetBestAccountsDateRange(System.DateTime date)
		{
			//	Retourne la période comptable correspondant à une date donnée.
			//	Si plusieurs périodes se recouvrent, on prend la dernière définie.
			return this.AccountsDateRanges
				.Reverse ()
				.Where (x => x.IsInside (date))
				.FirstOrDefault ();
		}
		#endregion


		#region VatCodes
		public IEnumerable<DateRange> VatCodesDateRanges
		{
			//	Retourne la liste des périodes de tous les codes TVA connus.
			get
			{
				return this.rangeVatCodes.Select (x => x.Key);
			}
		}

		public BaseType GetVatCodesBase(System.DateTime date)
		{
			//	Retourne la base correspondant à une date.
			//	Si plusieurs périodes se recouvrent, on prend la dernière définie.
			var range = this.GetBestVatCodesDateRange (date);
			return new BaseType (BaseTypeKind.VatCodes, range);
		}

		public GuidDictionary<DataObject> GetVatCodes(DateRange range)
		{
			//	Retourne le code TVA correspondant à une période.
			GuidDictionary<DataObject> vatCodes;
			if (!range.IsEmpty && this.rangeVatCodes.TryGetValue (range, out vatCodes))
			{
				return vatCodes;
			}
			else
			{
				// Il vaut mieux retourner un dictionnaire vide, plutôt que null.
				return new GuidDictionary<DataObject> (this.undoManager);
			}
		}

		public void AddVatCodes(DateRange dateRange, GuidDictionary<DataObject> vatCodes)
		{
			//	Prend connaissance d'un nouveau code TVA, qui est ajouté ou
			//	qui remplace un existant, selon sa période.
			this.rangeVatCodes[dateRange] = vatCodes;
		}

		private DateRange GetBestVatCodesDateRange(System.DateTime date)
		{
			//	Retourne la période comptable correspondant à une date donnée.
			//	Si plusieurs périodes se recouvrent, on prend la dernière définie.
			return this.VatCodesDateRanges
				.Reverse ()
				.Where (x => x.IsInside (date))
				.FirstOrDefault ();
		}
		#endregion


		#region Serialize
		public void Serialize(System.Xml.XmlWriter writer)
		{
			writer.WriteStartDocument ();
			writer.WriteStartElement (X.Document);

			writer.WriteElementString (X.DocumentVersion, DataMandat.SerializationVersion);
			this.SerializeDefinitions (writer);
			this.SerializeObjects (writer);

			writer.WriteEndElement ();
			writer.WriteEndDocument ();
		}

		public void SerializeAccountsAndCo(System.Xml.XmlWriter writer)
		{
			writer.WriteStartDocument ();
			writer.WriteStartElement (X.Document);

			writer.WriteElementString (X.DocumentVersion, DataMandat.SerializationVersion);
			this.SerializeAccounts (writer);
			this.SerializeVatCodes (writer);

			writer.WriteEndDocument ();
		}

		private void SerializeAccounts(System.Xml.XmlWriter writer)
		{
			writer.WriteStartElement (X.Accounts);

			foreach (var pair in this.rangeAccounts)
			{
				writer.WriteStartElement (X.Period);

				pair.Key.Serialize (writer, X.DateRange);
				this.SerializeObjects (writer, X.List, pair.Value);

				writer.WriteEndElement ();
			}

			writer.WriteEndElement ();
		}

		private void SerializeVatCodes(System.Xml.XmlWriter writer)
		{
			writer.WriteStartElement (X.VatCodes);

			foreach (var pair in this.rangeVatCodes)
			{
				writer.WriteStartElement (X.Period);

				pair.Key.Serialize (writer, X.DateRange);
				this.SerializeObjects (writer, X.List, pair.Value);

				writer.WriteEndElement ();
			}

			writer.WriteEndElement ();
		}

		private void SerializeDefinitions(System.Xml.XmlWriter writer)
		{
			writer.WriteStartElement (X.Definitions);

			writer.WriteGuidAttribute   (X.Attr.Guid,      this.Guid);
			writer.WriteStringAttribute (X.Attr.Name,      this.Name);
			writer.WriteDateAttribute   (X.Attr.StartDate, this.StartDate);

			writer.WriteEndElement ();
		}

		public void SerializeObjects(System.Xml.XmlWriter writer)
		{
			writer.WriteStartElement (X.Objects);

			this.SerializeObjects (writer, X.Arguments,         this.arguments);
			this.SerializeObjects (writer, X.Methods,           this.methods);
			this.SerializeObjects (writer, X.AssetsUserFields,  this.assetsUserFields);
			this.SerializeObjects (writer, X.PersonsUserFields, this.personsUserFields);
			this.SerializeObjects (writer, X.Categories,        this.categories);
			this.SerializeObjects (writer, X.Groups,            this.groups);
			this.SerializeObjects (writer, X.Persons,           this.persons);
			this.SerializeObjects (writer, X.Assets,            this.assets);
			this.SerializeObjects (writer, X.Entries,           this.entries);
			this.SerializeReports (writer);

			writer.WriteEndElement ();
		}

		private void SerializeObjects(System.Xml.XmlWriter writer, string name, GuidDictionary<DataObject> objects)
		{
			writer.WriteStartElement (name);

			foreach (var obj in objects)
			{
				obj.Serialize (writer);
			}

			writer.WriteEndElement ();
		}

		private void SerializeReports(System.Xml.XmlWriter writer)
		{
			writer.WriteStartElement (X.Reports);

			foreach (var report in this.reports)
			{
				report.Serialize (writer);
			}

			writer.WriteEndElement ();
		}
		#endregion


		#region Deserialize
		private void Deserialize(System.Xml.XmlReader reader)
		{
			while (reader.Read ())
			{
				if (reader.NodeType == System.Xml.XmlNodeType.Element)
				{
					switch (reader.Name)
					{
						case X.Document:
							this.DeserializeMandat (reader);
							break;
					}
				}
				else if (reader.NodeType == System.Xml.XmlNodeType.EndElement)
				{
					break;
				}
			}
		}

		private void DeserializeMandat(System.Xml.XmlReader reader)
		{
			while (reader.Read ())
			{
				if (reader.NodeType == System.Xml.XmlNodeType.Element)
				{
					switch (reader.Name)
					{
						case X.DocumentVersion:
							var version = reader.ReadElementContentAsString ();
							break;

						case X.Definitions:
							this.DeserializeDefinitions (reader);
							break;
						
						case X.Objects:
							this.DeserializeObjects (reader);
							break;
					}
				}
				else if (reader.NodeType == System.Xml.XmlNodeType.EndElement)
				{
					break;
				}
			}
		}

		private void DeserializeDefinitions(System.Xml.XmlReader reader)
		{
			this.guid      = reader.ReadGuidAttribute   (X.Attr.Guid);
			this.name      = reader.ReadStringAttribute (X.Attr.Name);
			this.startDate = reader.ReadDateAttribute   (X.Attr.StartDate).GetValueOrDefault ();

			reader.Read ();  // on avance sur le noeud suivant
		}

		public void DeserializeAccountsAndCo(System.Xml.XmlReader reader)
		{
			while (reader.Read ())
			{
				if (reader.NodeType == System.Xml.XmlNodeType.Element)
				{
					switch (reader.Name)
					{
						case X.Document:
							this.DeserializeAccountsMandat (reader);
							break;
					}
				}
				else if (reader.NodeType == System.Xml.XmlNodeType.EndElement)
				{
					break;
				}
			}
		}

		private void DeserializeAccountsMandat(System.Xml.XmlReader reader)
		{
			while (reader.Read ())
			{
				if (reader.NodeType == System.Xml.XmlNodeType.Element)
				{
					switch (reader.Name)
					{
						case X.DocumentVersion:
							var version = reader.ReadElementContentAsString ();
							break;

						case X.Accounts:
							this.DeserializeAccounts (reader);
							break;

						case X.VatCodes:
							this.DeserializeVatCodes (reader);
							break;
					}
				}
				else if (reader.NodeType == System.Xml.XmlNodeType.EndElement)
				{
					break;
				}
			}
		}

		private void DeserializeAccounts(System.Xml.XmlReader reader)
		{
			while (reader.Read ())
			{
				if (reader.NodeType == System.Xml.XmlNodeType.Element)
				{
					switch (reader.Name)
					{
						case X.Period:
							this.DeserializeAccountsPeriod (reader);
							break;
					}
				}
				else if (reader.NodeType == System.Xml.XmlNodeType.EndElement)
				{
					break;
				}
			}
		}

		private void DeserializeAccountsPeriod(System.Xml.XmlReader reader)
		{
			var dateRange = DateRange.Empty;
			var objects = new GuidDictionary<DataObject> (null);

			while (reader.Read ())
			{
				if (reader.NodeType == System.Xml.XmlNodeType.Element)
				{
					switch (reader.Name)
					{
						case X.DateRange:
							dateRange = new DateRange (reader);
							break;

						case X.List:
							this.DeserializeObjects (reader, objects);
							break;
					}
				}
				else if (reader.NodeType == System.Xml.XmlNodeType.EndElement)
				{
					if (!dateRange.IsEmpty && objects.Any ())
					{
						this.rangeAccounts.Add (dateRange, objects);
					}

					break;
				}
			}
		}

		private void DeserializeVatCodes(System.Xml.XmlReader reader)
		{
			while (reader.Read ())
			{
				if (reader.NodeType == System.Xml.XmlNodeType.Element)
				{
					switch (reader.Name)
					{
						case X.Period:
							this.DeserializeVatCodesPeriod (reader);
							break;
					}
				}
				else if (reader.NodeType == System.Xml.XmlNodeType.EndElement)
				{
					break;
				}
			}
		}

		private void DeserializeVatCodesPeriod(System.Xml.XmlReader reader)
		{
			var dateRange = DateRange.Empty;
			var objects = new GuidDictionary<DataObject> (null);

			while (reader.Read ())
			{
				if (reader.NodeType == System.Xml.XmlNodeType.Element)
				{
					switch (reader.Name)
					{
						case X.DateRange:
							dateRange = new DateRange (reader);
							break;

						case X.List:
							this.DeserializeObjects (reader, objects);
							break;
					}
				}
				else if (reader.NodeType == System.Xml.XmlNodeType.EndElement)
				{
					if (!dateRange.IsEmpty && objects.Any ())
					{
						this.rangeVatCodes.Add (dateRange, objects);
					}

					break;
				}
			}
		}

		private void DeserializeObjects(System.Xml.XmlReader reader)
		{
			while (reader.Read ())
			{
				if (reader.NodeType == System.Xml.XmlNodeType.Element)
				{
					switch (reader.Name)
					{
						case X.Arguments:
							this.DeserializeObjects (reader, this.arguments);
							break;

						case X.Methods:
							this.DeserializeObjects (reader, this.methods);
							break;

						case X.AssetsUserFields:
							this.DeserializeObjects (reader, this.assetsUserFields);
							break;

						case X.PersonsUserFields:
							this.DeserializeObjects (reader, this.personsUserFields);
							break;

						case X.Assets:
							this.DeserializeObjects (reader, this.assets);
							break;

						case X.Categories:
							this.DeserializeObjects (reader, this.categories);
							break;

						case X.Groups:
							this.DeserializeObjects (reader, this.groups);
							break;

						case X.Persons:
							this.DeserializeObjects (reader, this.persons);
							break;

						case X.Entries:
							this.DeserializeObjects (reader, this.entries);
							break;

						case X.Reports:
							this.DeserializeReports (reader);
							break;
					}
				}
				else if (reader.NodeType == System.Xml.XmlNodeType.EndElement)
				{
					break;
				}
			}
		}

		private void DeserializeObjects(System.Xml.XmlReader reader, GuidDictionary<DataObject> objects)
		{
			if (!reader.IsEmptyElement)
			{
				while (reader.Read ())
				{
					if (reader.NodeType == System.Xml.XmlNodeType.Element)
					{
						if (reader.Name == X.Object)
						{
							var obj = new DataObject (this.undoManager, reader);
							objects.Add (obj);
						}
					}
					else if (reader.NodeType == System.Xml.XmlNodeType.EndElement)
					{
						break;
					}
				}
			}
		}

		private void DeserializeReports(System.Xml.XmlReader reader)
		{
			while (reader.Read ())
			{
				if (reader.NodeType == System.Xml.XmlNodeType.Element)
				{
					switch (reader.Name)
					{
						case X.Report_MCH2Summary:
							this.reports.Add (new MCH2SummaryParams (reader));
							break;

						case X.Report_Assets:
							this.reports.Add (new AssetsParams (reader));
							break;

						case X.Report_Persons:
							this.reports.Add (new PersonsParams (reader));
							break;
					}
				}
				else if (reader.NodeType == System.Xml.XmlNodeType.EndElement)
				{
					break;
				}
			}
		}
		#endregion


		private readonly ComputerSettings								computerSettings;
		private readonly GlobalSettings									globalSettings;
		private readonly UndoManager									undoManager;
		private readonly GuidDictionary<DataObject>						assetsUserFields;
		private readonly GuidDictionary<DataObject>						personsUserFields;
		private readonly GuidDictionary<DataObject>						assets;
		private readonly GuidDictionary<DataObject>						categories;
		private readonly GuidDictionary<DataObject>						groups;
		private readonly GuidDictionary<DataObject>						persons;
		private readonly GuidDictionary<DataObject>						entries;
		private readonly GuidDictionary<DataObject>						methods;
		private readonly GuidDictionary<DataObject>						arguments;
		private readonly UndoableDictionary<DateRange, GuidDictionary<DataObject>> rangeAccounts;
		private readonly UndoableDictionary<DateRange, GuidDictionary<DataObject>> rangeVatCodes;
		private readonly GuidDictionary<AbstractReportParams>			reports;

		private Guid													guid;
		private string													name;
		private System.DateTime											startDate;
	}
}
