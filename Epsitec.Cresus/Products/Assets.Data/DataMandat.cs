//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.Data.Reports;

namespace Epsitec.Cresus.Assets.Data
{
	public class DataMandat
	{
		public DataMandat(string name, System.DateTime startDate)
		{
			this.guid      = Guid.NewGuid ();
			this.name      = name;
			this.startDate = startDate;

			this.undoManager    = new UndoManager ();
			this.globalSettings = new GlobalSettings (this.undoManager);

			this.assetsUserFields  = new GuidList<DataObject> (this.undoManager);
			this.personsUserFields = new GuidList<DataObject> (this.undoManager);
			this.assets            = new GuidList<DataObject> (this.undoManager);
			this.categories        = new GuidList<DataObject> (this.undoManager);
			this.groups            = new GuidList<DataObject> (this.undoManager);
			this.persons           = new GuidList<DataObject> (this.undoManager);
			this.entries           = new GuidList<DataObject> (this.undoManager);
			this.rangeAccounts     = new UndoableDictionary<DateRange, GuidList<DataObject>> (this.undoManager);
			this.reports           = new GuidList<AbstractReportParams> (this.undoManager);
		}

		public DataMandat(System.Xml.XmlReader reader)
		{
			this.undoManager    = new UndoManager ();
			this.globalSettings = new GlobalSettings (this.undoManager);

			this.assetsUserFields  = new GuidList<DataObject> (this.undoManager);
			this.personsUserFields = new GuidList<DataObject> (this.undoManager);
			this.assets            = new GuidList<DataObject> (this.undoManager);
			this.categories        = new GuidList<DataObject> (this.undoManager);
			this.groups            = new GuidList<DataObject> (this.undoManager);
			this.persons           = new GuidList<DataObject> (this.undoManager);
			this.entries           = new GuidList<DataObject> (this.undoManager);
			this.rangeAccounts     = new UndoableDictionary<DateRange, GuidList<DataObject>> (this.undoManager);
			this.reports           = new GuidList<AbstractReportParams> (this.undoManager);

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

		public GuidList<AbstractReportParams>	Reports
		{
			get
			{
				return this.reports;
			}
		}


		public GuidList<DataObject> GetData(BaseType type)
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

				default:
					// Il vaut mieux retourner une liste vide, plutôt que null.
					return new GuidList<DataObject> (this.undoManager);
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
			var range = this.GetBestDateRange (date);
			return new BaseType (BaseTypeKind.Accounts, range);
		}

		public GuidList<DataObject> GetAccounts(DateRange range)
		{
			//	Retourne le plan comptable correspondant à une période.
			GuidList<DataObject> accounts;
			if (!range.IsEmpty && this.rangeAccounts.TryGetValue (range, out accounts))
			{
				return accounts;
			}
			else
			{
				// Il vaut mieux retourner une liste vide, plutôt que null.
				return new GuidList<DataObject> (this.undoManager);
			}
		}

		public void AddAccounts(DateRange dateRange, GuidList<DataObject> accounts)
		{
			//	Prend connaissance d'un nouveau plan comptable, qui est ajouté ou
			//	qui remplace un existant, selon sa période.
			this.rangeAccounts[dateRange] = accounts;
		}

		public DateRange GetBestDateRange(System.DateTime date)
		{
			//	Retourne la période comptable correspondant à une date donnée.
			//	Si plusieurs périodes se recouvrent, on prend la dernière définie.
			return this.AccountsDateRanges
				.Reverse ()
				.Where (x => x.IsInside (date))
				.FirstOrDefault ();
		}
		#endregion


		#region Serialize
		public void Serialize(System.Xml.XmlWriter writer)
		{
			writer.WriteStartDocument ();
			writer.WriteStartElement ("Mandat");

			this.SerializeDefinitions (writer);
			this.SerializeObjects (writer);

			writer.WriteEndElement ();
			writer.WriteEndDocument ();
		}

		private void SerializeDefinitions(System.Xml.XmlWriter writer)
		{
			writer.WriteStartElement ("Definitions");

			this.Guid.Serialize (writer, "Guid");
			writer.WriteElementString ("Name", this.Name);
			writer.WriteElementString ("StartDate", this.StartDate.ToString (System.Globalization.CultureInfo.InvariantCulture));

			writer.WriteEndElement ();
		}

		public void SerializeObjects(System.Xml.XmlWriter writer)
		{
			writer.WriteStartElement ("Objects");

			this.SerializeObjects (writer, "AssetsUserFields",  this.assetsUserFields);
			this.SerializeObjects (writer, "PersonsUserFields", this.personsUserFields);
			this.SerializeObjects (writer, "Categories",        this.categories);
			this.SerializeObjects (writer, "Groups",            this.groups);
			this.SerializeObjects (writer, "Persons",           this.persons);
			this.SerializeObjects (writer, "Assets",            this.assets);
			this.SerializeObjects (writer, "Entries",           this.entries);
			this.SerializeReports (writer);

			writer.WriteEndElement ();
		}

		private void SerializeObjects(System.Xml.XmlWriter writer, string name, GuidList<DataObject> objects)
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
			writer.WriteStartElement ("Reports");

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
						case "Mandat":
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
						case "Definitions":
							this.DeserializeDefinitions (reader);
							break;

						case "Objects":
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
			while (reader.Read ())
			{
				if (reader.NodeType == System.Xml.XmlNodeType.Element)
				{
					switch (reader.Name)
					{
						case "Guid":
							this.guid = new Guid (reader);
							break;

						case "Name":
							this.name = reader.ReadElementContentAsString ();
							break;

						case "StartDate":
							var s = reader.ReadElementContentAsString ();
							this.startDate = System.DateTime.Parse (s, System.Globalization.CultureInfo.InvariantCulture);
							break;
					}
				}
				else if (reader.NodeType == System.Xml.XmlNodeType.EndElement)
				{
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
						case "AssetsUserFields":
							this.DeserializeObjects (reader, this.assetsUserFields);
							break;

						case "PersonsUserFields":
							this.DeserializeObjects (reader, this.personsUserFields);
							break;

						case "Assets":
							this.DeserializeObjects (reader, this.assets);
							break;

						case "Categories":
							this.DeserializeObjects (reader, this.categories);
							break;

						case "Groups":
							this.DeserializeObjects (reader, this.groups);
							break;

						case "Persons":
							this.DeserializeObjects (reader, this.persons);
							break;

						case "Entries":
							this.DeserializeObjects (reader, this.entries);
							break;

						case "Reports":
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

		private void DeserializeObjects(System.Xml.XmlReader reader, GuidList<DataObject> objects)
		{
			while (reader.Read ())
			{
				if (reader.NodeType == System.Xml.XmlNodeType.Element)
				{
					if (reader.Name == "Object")
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

		private void DeserializeReports(System.Xml.XmlReader reader)
		{
			while (reader.Read ())
			{
				if (reader.NodeType == System.Xml.XmlNodeType.Element)
				{
					if (reader.Name.StartsWith ("Report."))
					{
						var name = reader.Name.Substring (7);  // nom après "Report."
						switch (name)
						{
							case "MCH2Summary":
								this.reports.Add (new MCH2SummaryParams (reader));
								break;

							case "Assets":
								this.reports.Add (new AssetsParams (reader));
								break;

							case "Persons":
								this.reports.Add (new PersonsParams (reader));
								break;
						}
					}
				}
				else if (reader.NodeType == System.Xml.XmlNodeType.EndElement)
				{
					break;
				}
			}
		}
		#endregion


		private readonly GlobalSettings									globalSettings;
		private readonly UndoManager									undoManager;
		private readonly GuidList<DataObject>							assetsUserFields;
		private readonly GuidList<DataObject>							personsUserFields;
		private readonly GuidList<DataObject>							assets;
		private readonly GuidList<DataObject>							categories;
		private readonly GuidList<DataObject>							groups;
		private readonly GuidList<DataObject>							persons;
		private readonly GuidList<DataObject>							entries;
		private readonly UndoableDictionary<DateRange, GuidList<DataObject>> rangeAccounts;
		private readonly GuidList<AbstractReportParams>					reports;

		private Guid													guid;
		private string													name;
		private System.DateTime											startDate;
	}
}
