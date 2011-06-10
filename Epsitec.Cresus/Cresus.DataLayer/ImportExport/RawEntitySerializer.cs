using Epsitec.Common.Support;

using Epsitec.Common.Types;

using Epsitec.Cresus.Database;

using Epsitec.Cresus.DataLayer.Infrastructure;
using Epsitec.Cresus.DataLayer.Schema;

using System.Collections.Generic;

using System.IO;

using System.Linq;

using System.Xml;


namespace Epsitec.Cresus.DataLayer.ImportExport
{


	// TODO Comment this class.
	// Marc


	internal static class RawEntitySerializer
	{


		public static void Export(FileInfo file, DbInfrastructure dbInfrastructure, EntityTypeEngine typeEngine, EntitySchemaEngine schemaEngine, RawExportMode exportMode)
		{
			List<TableDefinition> tableDefinitions = RawEntitySerializer.GetTableDefinitions (typeEngine, schemaEngine).ToList ();

			XmlWriterSettings settings = new XmlWriterSettings ()
			{
				CheckCharacters = true,
				ConformanceLevel = ConformanceLevel.Document,
				Indent = true,
			};

			string version = "1.0.0";
			long idShift = EntitySchemaBuilder.AutoIncrementStartValue;

			using (XmlWriter xmlWriter = XmlWriter.Create (file.FullName, settings))
			{
				RawEntitySerializer.WriteDocumentStart (xmlWriter);
				RawEntitySerializer.WriteHeader (xmlWriter, version, idShift);
				RawEntitySerializer.WriteDefinition (xmlWriter, tableDefinitions);
				RawEntitySerializer.WriteData (dbInfrastructure, xmlWriter, tableDefinitions, exportMode);
				RawEntitySerializer.WriteDocumentEnd (xmlWriter);
			}
		}


		private static IEnumerable<TableDefinition> GetTableDefinitions(EntityTypeEngine typeEngine, EntitySchemaEngine schemaEngine)
		{
			var dataTableDefinitions = RawEntitySerializer.GetValueTableDefinitions (typeEngine, schemaEngine);
			var relationTableDefitions = RawEntitySerializer.GetCollectionTableDefinitions (typeEngine, schemaEngine);

			return dataTableDefinitions.Concat (relationTableDefitions);
		}


		private static IEnumerable<TableDefinition> GetValueTableDefinitions(EntityTypeEngine typeEngine, EntitySchemaEngine schemaEngine)
		{
			return from entityType in typeEngine.GetEntityTypes()
				   select RawEntitySerializer.GetValueTableDefinition (typeEngine, schemaEngine, entityType);
		}


		private static IEnumerable<TableDefinition> GetCollectionTableDefinitions(EntityTypeEngine typeEngine, EntitySchemaEngine schemaEngine)
		{
			return from entityType in typeEngine.GetEntityTypes ()
				   from field in typeEngine.GetLocalCollectionFields(entityType.CaptionId)
				   select RawEntitySerializer.GetCollectionTableDefinition (schemaEngine, entityType, field);
		}


		private static TableDefinition GetValueTableDefinition(EntityTypeEngine typeEngine, EntitySchemaEngine schemaEngine, StructuredType entityType)
		{
			Druid entityTypeId = entityType.CaptionId;

			DbTable dbTable = schemaEngine.GetEntityTable (entityTypeId);

			IList<DbColumn> idDbColumns = new List<DbColumn> ()
		    {
		        dbTable.Columns[EntitySchemaBuilder.EntityTableColumnIdName],
		    };

			foreach (var field in typeEngine.GetLocalReferenceFields (entityTypeId))
			{
				Druid fieldId = field.CaptionId;
				DbColumn fieldColumn = schemaEngine.GetEntityFieldColumn (entityTypeId, fieldId);

				idDbColumns.Add (fieldColumn);
			}

			IList<DbColumn> regularDbColumns = dbTable.Columns
				.Where (c => !idDbColumns.Contains (c))
				.Where (c => c.Name != EntitySchemaBuilder.EntityTableColumnEntityModificationEntryIdName)
				.Where (c => c.Cardinality == DbCardinality.None)
				.ToList ();

			return RawEntitySerializer.GetTableDefitition (dbTable, TableCategory.Data, idDbColumns, regularDbColumns);
		}


		private static TableDefinition GetCollectionTableDefinition(EntitySchemaEngine schemaEngine, StructuredType type, StructuredTypeField field)
		{
			DbTable dbTable = schemaEngine.GetEntityFieldTable (type.CaptionId, field.CaptionId);

			IList<DbColumn> idDbColumns = new List<DbColumn> ()
		    {
		        dbTable.Columns[EntitySchemaBuilder.EntityFieldTableColumnIdName],
		        dbTable.Columns[EntitySchemaBuilder.EntityFieldTableColumnSourceIdName],
		        dbTable.Columns[EntitySchemaBuilder.EntityFieldTableColumnTargetIdName],
		    };

			IList<DbColumn> regularDbColumns = new List<DbColumn> ()
		    {
		        dbTable.Columns[EntitySchemaBuilder.EntityFieldTableColumnRankName],
		    };

			return RawEntitySerializer.GetTableDefitition (dbTable, TableCategory.Relation, idDbColumns, regularDbColumns);
		}


		private static TableDefinition GetTableDefitition(DbTable dbTable, TableCategory tableCategory, IEnumerable<DbColumn> idDbColumns, IEnumerable<DbColumn> regularDbColumns)
		{
			string dbName = dbTable.Name;
			string sqlName = dbTable.GetSqlName ();

			IEnumerable<ColumnDefinition> idColumns = RawEntitySerializer.GetColumnDefinitions (idDbColumns, true);
			IEnumerable<ColumnDefinition> regularColumns = RawEntitySerializer.GetColumnDefinitions (regularDbColumns, false);

			bool containsLogColumn = dbTable.Columns.Any (c => c.Name == EntitySchemaBuilder.EntityTableColumnEntityModificationEntryIdName);

			return new TableDefinition (dbName, sqlName, tableCategory, containsLogColumn, idColumns.Concat (regularColumns));
		}


		private static IEnumerable<ColumnDefinition> GetColumnDefinitions(IEnumerable<DbColumn> dbColumns, bool isIdColumn)
		{
			return from dbColumn in dbColumns
				   let dbName = dbColumn.Name
				   let sqlName = dbColumn.GetSqlName ()
				   let dbRawType = dbColumn.Type.RawType
				   let adoType = TypeConverter.GetAdoType (dbColumn.Type.RawType)
				   select new ColumnDefinition (dbName, sqlName, dbRawType, adoType, isIdColumn);
		}


		private static void WriteDocumentStart(XmlWriter xmlWriter)
		{
			xmlWriter.WriteStartDocument ();
			xmlWriter.WriteStartElement ("export");
		}


		private static void WriteHeader(XmlWriter xmlWriter, string version, long idShift)
		{
			xmlWriter.WriteStartElement ("header");
			xmlWriter.WriteStartElement ("version");
			xmlWriter.WriteValue (version);
			xmlWriter.WriteEndElement ();
			xmlWriter.WriteStartElement ("idShift");
			xmlWriter.WriteValue (idShift);
			xmlWriter.WriteEndElement ();
			xmlWriter.WriteEndElement ();
		}


		private static void WriteDefinition(XmlWriter xmlWriter, IList<TableDefinition> tableDefinitions)
		{
			xmlWriter.WriteStartElement ("definition");
			
			for (int i = 0; i < tableDefinitions.Count; i++)
			{
				tableDefinitions[i].WriteXmlDefinition (xmlWriter, i);
			}

			xmlWriter.WriteEndElement ();
		}


		private static void WriteData(DbInfrastructure dbInfrastructure, XmlWriter xmlWriter, IList<TableDefinition> tableDefinitions, RawExportMode exportMode)
		{
			bool exportOnlyUserData = exportMode == RawExportMode.UserData;

			xmlWriter.WriteStartElement ("data");

			for (int i = 0; i < tableDefinitions.Count; i++)
			{
				tableDefinitions[i].WriteXmlData (dbInfrastructure, xmlWriter, i, exportOnlyUserData);
			}

			xmlWriter.WriteEndElement ();
		}


		private static void WriteDocumentEnd(XmlWriter xmlWriter)
		{
			xmlWriter.WriteEndElement ();
			xmlWriter.WriteEndDocument ();
		}


		public static void Import(FileInfo file, DbInfrastructure dbInfrastructure, EntityModificationEntry entityModificationEntry, RawImportMode importMode)
		{
			using (XmlReader xmlReader = XmlReader.Create (file.FullName))
			{
				RawEntitySerializer.ReadDocumentStart (xmlReader);
				RawEntitySerializer.ReadHeader (xmlReader);
				var tableDefinitions = RawEntitySerializer.ReadDefinition(xmlReader);
				RawEntitySerializer.ReadData (dbInfrastructure, xmlReader, entityModificationEntry, tableDefinitions, importMode);
				RawEntitySerializer.ReadDocumentEnd (xmlReader);
			}
		}


		private static void ReadDocumentStart(XmlReader xmlReader)
		{
			xmlReader.ReadStartElement ("export");
		}


		private static void ReadHeader(XmlReader xmlReader)
		{
			xmlReader.ReadStartElement ("header");
			xmlReader.ReadStartElement ("version");
			
			string version = xmlReader.ReadString ();
			
			xmlReader.ReadEndElement ();
			xmlReader.ReadStartElement ("idShift");

			string idShift = xmlReader.ReadString ();

			xmlReader.ReadEndElement ();
			xmlReader.ReadEndElement ();

			if (!string.Equals (version, "1.0.0"))
			{
				throw new System.FormatException ("Invalid version number: 1.0.0 expected but " + version + " found");
			}

			long idShiftAsLong;

			if (!long.TryParse (idShift, out idShiftAsLong))
			{
				throw new System.FormatException ("Invalid id shift.");
			}

			if (idShiftAsLong != EntitySchemaBuilder.AutoIncrementStartValue)
			{
				throw new System.FormatException ("Invalid id shift.");
			}
		}


		private static IList<TableDefinition> ReadDefinition(XmlReader xmlReader)
		{
			List<TableDefinition> tableDefinitions = new List<TableDefinition> ();

			bool isEmpty = xmlReader.IsEmptyElement;

			if (!isEmpty)
			{
				xmlReader.ReadStartElement ("definition");

				while (xmlReader.IsStartElement () && string.Equals (xmlReader.Name, "table"))
				{
					TableDefinition tableDefinition = TableDefinition.ReadXmlDefinition(xmlReader, tableDefinitions.Count);

					tableDefinitions.Add (tableDefinition);
				}

				xmlReader.ReadEndElement ();
			}

			return tableDefinitions;
		}


		private static void ReadData(DbInfrastructure dbInfrastructure, XmlReader xmlReader, EntityModificationEntry entityModificationEntry, IList<TableDefinition> tableDefinitions, RawImportMode importMode)
		{
			bool isEmpty = xmlReader.IsEmptyElement;
			bool decrementIds = importMode == RawImportMode.DecrementIds;
			
			xmlReader.ReadStartElement ("data");

			if (!isEmpty)
			{
				int index = 0;

				while (xmlReader.IsStartElement () && string.Equals (xmlReader.Name, "table"))
				{
					if (index < tableDefinitions.Count)
					{
						tableDefinitions[index].ReadXmlData (dbInfrastructure, xmlReader, entityModificationEntry, index, decrementIds);
					}

					index++;
				}

				if (index != tableDefinitions.Count)
				{
					throw new System.FormatException ("Invalid number of table data.");
				}

				xmlReader.ReadEndElement ();
			}
		}


		private static void ReadDocumentEnd(XmlReader xmlReader)
		{
			xmlReader.ReadEndElement ();
		}


		public static void CleanDatabase(FileInfo file, DbInfrastructure dbInfrastructure, RawImportMode importMode)
		{
			using (XmlReader xmlReader = XmlReader.Create (file.FullName))
			{
				RawEntitySerializer.ReadDocumentStart (xmlReader);
				RawEntitySerializer.ReadHeader (xmlReader);

				var tableDefinitions = RawEntitySerializer.ReadDefinition(xmlReader);

				RawEntitySerializer.CleanTables (dbInfrastructure, importMode, tableDefinitions);
			}
		}


		private static void CleanTables(DbInfrastructure dbInfrastructure, RawImportMode importMode, IList<TableDefinition> tableDefinitions)
		{
			bool cleanOnlyEpsitecData = importMode == RawImportMode.DecrementIds;

			foreach (TableDefinition tableDefinition in tableDefinitions)
			{
				tableDefinition.Clean (dbInfrastructure, cleanOnlyEpsitecData);
			}
		}


	}


}
