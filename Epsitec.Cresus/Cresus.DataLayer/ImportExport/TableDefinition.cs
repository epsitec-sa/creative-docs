using Epsitec.Common.Types;

using Epsitec.Cresus.Database;
using Epsitec.Cresus.Database.Collections;

using Epsitec.Cresus.DataLayer.Infrastructure;
using Epsitec.Cresus.DataLayer.Schema;

using System.Collections.Generic;

using System.Data;

using System.Linq;

using System.Xml;


namespace Epsitec.Cresus.DataLayer.ImportExport
{


	// TODO Comment this class
	// Marc


	internal sealed class TableDefinition
	{


		public TableDefinition(string dbName, string sqlName, TableCategory category, bool containsLogColumn, IEnumerable<ColumnDefinition> columns)
		{
			this.DbName = dbName;
			this.SqlName = sqlName;
			this.Category = category;
			this.ContainsLogColumn = containsLogColumn;
			this.Columns = columns.ToList ();
		}


		public string DbName
		{
			get;
			private set;
		}


		public string SqlName
		{
			get;
			private set;
		}


		public TableCategory Category
		{
			get;
			private set;
		}


		public bool ContainsLogColumn
		{
			get;
			private set;
		}


		public IEnumerable<ColumnDefinition> Columns
		{
			get;
			private set;
		}


		public void WriteXmlDefinition(XmlWriter xmlWriter, int index)
		{
			this.WriteXmlStart (xmlWriter, index);
			this.WriteXmlDbName (xmlWriter);
			this.WriteXmlSqlName (xmlWriter);
			this.WriteXmlCategory (xmlWriter);
			this.WriteXmlContainsLogColumn (xmlWriter);
			this.WriteXmlColumns (xmlWriter);
			this.WriteXmlEnd (xmlWriter);
		}


		private void WriteXmlStart(XmlWriter xmlWriter, int index)
		{
			xmlWriter.WriteStartElement ("table");
			xmlWriter.WriteAttributeString ("id", InvariantConverter.ConvertToString (index));
		}


		private void WriteXmlDbName(XmlWriter xmlWriter)
		{
			xmlWriter.WriteStartElement ("dbname");
			xmlWriter.WriteValue (this.DbName);
			xmlWriter.WriteEndElement ();
		}


		private void WriteXmlSqlName(XmlWriter xmlWriter)
		{
			xmlWriter.WriteStartElement ("sqlname");
			xmlWriter.WriteValue (this.SqlName);
			xmlWriter.WriteEndElement ();
		}


		private void WriteXmlCategory(XmlWriter xmlWriter)
		{
			xmlWriter.WriteStartElement ("category");
			xmlWriter.WriteValue (System.Enum.GetName (typeof (TableCategory), this.Category));
			xmlWriter.WriteEndElement ();
		}


		private void WriteXmlContainsLogColumn(XmlWriter xmlWriter)
		{
			xmlWriter.WriteStartElement ("log");
			xmlWriter.WriteValue (InvariantConverter.ConvertToString (this.ContainsLogColumn));
			xmlWriter.WriteEndElement ();
		}


		private void WriteXmlColumns(XmlWriter xmlWriter)
		{
			xmlWriter.WriteStartElement ("columns");

			List<ColumnDefinition> columns = this.Columns.ToList ();

			for (int i = 0; i < columns.Count; i++)
			{
				columns[i].WriteXmlDefinition (xmlWriter, i);
			}

			xmlWriter.WriteEndElement ();
		}


		private void WriteXmlEnd(XmlWriter xmlWriter)
		{
			xmlWriter.WriteEndElement ();
		}



		public void WriteXmlData(DbInfrastructure dbInfrastructure, XmlWriter xmlWriter, int index, bool exportOnlyUserData)
		{
			this.WriteXmlStart (xmlWriter, index);

			foreach (IList<string> row in this.ProcessRowsWrite(dbInfrastructure.Converter, this.GetRows (dbInfrastructure, exportOnlyUserData)))
			{
				this.WriteXmlRow (xmlWriter, row);
			}

			this.WriteXmlEnd (xmlWriter);
		}


		private void WriteXmlRow(XmlWriter xmlWriter, IList<string> row)
		{
			xmlWriter.WriteStartElement ("row");

			for (int i = 0; i < row.Count; i++)
			{
				this.WriteXmlCell (xmlWriter, row[i], i);
			}

			xmlWriter.WriteEndElement ();
		}


		private void WriteXmlCell(XmlWriter xmlWriter, string data, int index)
		{
			xmlWriter.WriteStartElement ("column");
			xmlWriter.WriteAttributeString ("id", InvariantConverter.ConvertToString (index));
			xmlWriter.WriteValue (data);
			xmlWriter.WriteEndElement ();
		}


		private IEnumerable<IList<object>> GetRows(DbInfrastructure dbInfrastructure, bool exportOnlyUserData)
		{
			SqlField tableSqlField = this.CreateTableSqlField ();
			IList<SqlField> columnSqlFields = this.CreateColumnSqlFields ();

			List<System.Tuple<long, long>> ranges = new List<System.Tuple<long, long>> ();

			if (!exportOnlyUserData)
			{
				ranges.Add (System.Tuple.Create ((long) 0, (long) EntitySchemaBuilder.AutoIncrementStartValue - 1));
			}

			ranges.Add (System.Tuple.Create ((long) EntitySchemaBuilder.AutoIncrementStartValue, long.MaxValue));

			int length = 1000;

			using (DbTransaction transaction = dbInfrastructure.BeginTransaction (DbTransactionMode.ReadOnly))
			{
				foreach (var range in ranges)
				{
					long rangeStartIndex = range.Item1;
					long rangeLastIndex = range.Item2;

					bool done = false;

					for (long i = rangeStartIndex; (i + length - 1 <= rangeLastIndex) && !done; i += length)
					{
						SqlSelect query = this.CreateQueryForInterval (tableSqlField, columnSqlFields, i, i + length - 1);

						transaction.SqlBuilder.SelectData (query);

						DataSet data = dbInfrastructure.ExecuteRetData (transaction);
						DataRowCollection rows = data.Tables[0].Rows;

						if (rows.Count == 0)
						{
							done = true;
						}
						else
						{
							foreach (DataRow row in rows)
							{
								yield return row.ItemArray.ToList ();
							}
						}
					}
				}

				transaction.Commit ();
			}
		}


		private SqlField CreateTableSqlField()
		{
			return SqlField.CreateName (this.SqlName);
		}


		private IList<SqlField> CreateColumnSqlFields()
		{
			return this.Columns.Select (c => SqlField.CreateName (c.Name)).ToList ();
		}


		private SqlSelect CreateQueryForInterval(SqlField tableSqlField, IList<SqlField> columnSqlFields, long firstIndex, long lastIndex)
		{
			SqlSelect query = new SqlSelect ();

			query.Tables.Add (tableSqlField);
			query.Fields.AddRange (columnSqlFields);
			query.Conditions.Add (this.CreateConditionForInterval (firstIndex, lastIndex));

			return query;
		}


		private SqlField CreateConditionForInterval(long firstIndex, long lastIndex)
		{
			string rowIdColumnName = GetRowIdColumnName ();		
			
			return SqlField.CreateFunction
			(
				new SqlFunction (
					SqlFunctionCode.LogicAnd,
					SqlField.CreateFunction
					(
						new SqlFunction
						(
							SqlFunctionCode.CompareGreaterThanOrEqual,
							SqlField.CreateName (rowIdColumnName),
							SqlField.CreateConstant (firstIndex, DbRawType.Int64)
						)
					),
					SqlField.CreateFunction
					(
						new SqlFunction
						(
							SqlFunctionCode.CompareLessThanOrEqual,
							SqlField.CreateName (rowIdColumnName),
							SqlField.CreateConstant (lastIndex, DbRawType.Int64)
						)
					)
				)
			);
		}


		private IEnumerable<IList<string>> ProcessRowsWrite(ITypeConverter iTypeConverter, IEnumerable<object> rows)
		{
			var converters = this.Columns.Select (c => this.GetSerializationConverter (iTypeConverter, c)).ToList ();
			
			foreach (IList<object> row in rows)
			{
				yield return this.ProcessRowWrite (converters, row);
			}
		}


		private IList<string> ProcessRowWrite(IList<ISerializationConverter> converters, IList<object> row)
		{
			List<string> processedRow = new List<string> ();
			
			for (int i = 0; i < row.Count; i++)
			{
				object valueAsObject = row[i];

				ISerializationConverter converter = converters[i];

				string valueAsString = (valueAsObject == null || valueAsObject == System.DBNull.Value) ? null : converter.ConvertToString (valueAsObject, null);

				processedRow.Add (valueAsString);
			}

			return processedRow;
		}


		public static TableDefinition ReadXmlDefinition(XmlReader xmlReader, int index)
		{
			TableDefinition.ReadXmlStart (xmlReader, index);

			string dbName = TableDefinition.ReadXmlDbName (xmlReader);
			string sqlName = TableDefinition.ReadXmlSqlName (xmlReader);
			TableCategory category = TableDefinition.ReadXmlCategory (xmlReader);
			bool containsLogColumn = TableDefinition.ReadXmlContainsLogColumn (xmlReader);
			IEnumerable<ColumnDefinition> columns = TableDefinition.ReadXmlColumns (xmlReader);

			TableDefinition.ReadXmlEnd (xmlReader);

			return new TableDefinition (dbName, sqlName, category, containsLogColumn, columns);
		}


		private static void ReadXmlStart(XmlReader xmlReader, int index)
		{
			if (!string.Equals ("table", xmlReader.Name))
			{
				throw new System.FormatException ("Unexpected tag: " + xmlReader.Name + " found but table expected.");
			}

			string idAsString = xmlReader.GetAttribute ("id");
			int idAsInt = InvariantConverter.ConvertFromString<int> (idAsString);

			xmlReader.ReadStartElement ("table");

			if (index != idAsInt)
			{
				throw new System.FormatException ("Unexpected index for table: " + idAsInt + " found but " + index + " expected.");
			}
		}


		private static string ReadXmlDbName(XmlReader xmlReader)
		{
			xmlReader.ReadStartElement ("dbname");

			string name = xmlReader.ReadContentAsString ();

			xmlReader.ReadEndElement ();

			return name;
		}


		private static string ReadXmlSqlName(XmlReader xmlReader)
		{
			xmlReader.ReadStartElement ("sqlname");

			string name = xmlReader.ReadContentAsString ();

			xmlReader.ReadEndElement ();

			return name;
		}


		private static TableCategory ReadXmlCategory(XmlReader xmlReader)
		{
			xmlReader.ReadStartElement ("category");

			string categoryAsString = xmlReader.ReadContentAsString ();

			xmlReader.ReadEndElement ();

			return (TableCategory) System.Enum.Parse (typeof (TableCategory), categoryAsString);
		}


		private static bool ReadXmlContainsLogColumn(XmlReader xmlReader)
		{
			xmlReader.ReadStartElement ("log");

			string containsLogColumnAsString = xmlReader.ReadContentAsString ();

			xmlReader.ReadEndElement ();

			return InvariantConverter.ConvertFromString<bool> (containsLogColumnAsString);
		}


		private static IList<ColumnDefinition> ReadXmlColumns(XmlReader xmlReader)
		{
			xmlReader.ReadStartElement ("columns");

			List<ColumnDefinition> columnDefinitions = new List<ColumnDefinition> ();
			
			while (xmlReader.IsStartElement () && string.Equals (xmlReader.Name, "column"))
			{
				ColumnDefinition columnDefinition = ColumnDefinition.ReadXmlDefinition (xmlReader, columnDefinitions.Count);

				columnDefinitions.Add (columnDefinition);
			}

			xmlReader.ReadEndElement ();

			return columnDefinitions;
		}


		private static void ReadXmlEnd(XmlReader xmlReader)
		{
			xmlReader.ReadEndElement ();
		}


		public void ReadXmlData(DbInfrastructure dbInfrastructure, XmlReader xmlReader, EntityModificationEntry entityModificationEntry, int index, bool decrementIds)
		{
			bool isEmpty = xmlReader.IsEmptyElement;

			TableDefinition.ReadXmlStart (xmlReader, index);

			if (!isEmpty)
			{
				this.InsertRows (dbInfrastructure, entityModificationEntry, this.ProcessRowsRead (dbInfrastructure.Converter, decrementIds, this.ReadXmlRows (xmlReader)));

				if (!decrementIds)
				{
					this.UpdateAutoIncrementStartValue (dbInfrastructure);
				}

				TableDefinition.ReadXmlEnd (xmlReader);
			}
		}


		private IEnumerable<IList<string>> ReadXmlRows(XmlReader xmlReader)
		{
			List<ColumnDefinition> columns = this.Columns.ToList ();
			
			while (xmlReader.IsStartElement () && string.Equals (xmlReader.Name, "row"))
			{
				List<string> row = new List<string> ();

				xmlReader.ReadStartElement ("row");

				int index = 0;

				while (xmlReader.IsStartElement () && string.Equals (xmlReader.Name, "column"))
				{
					if (index < columns.Count)
					{
						bool isEmpty = xmlReader.IsEmptyElement;
						
						string idAsString = xmlReader.GetAttribute ("id");
						int idAsInt = InvariantConverter.ConvertFromString<int> (idAsString);

						xmlReader.ReadStartElement ("column");

						if (index != idAsInt)
						{
							throw new System.FormatException ("Unexpected index for column: " + idAsInt + " found but " + index + " expected.");
						}

						if (isEmpty)
						{
							row.Add (null);
						}
						else
						{
							row.Add (xmlReader.ReadString ());

							xmlReader.ReadEndElement ();
						}
					}

					index++;
				}

				if (index != columns.Count)
				{
					throw new System.FormatException ("Invalid number of columns in row.");
				}
				
				xmlReader.ReadEndElement ();

				yield return row;
			}
		}


		private IEnumerable<IList<object>> ProcessRowsRead(ITypeConverter iTypeConverter, bool decrementIds, IEnumerable<IList<string>> rows)
		{
			var converters = this.Columns.Select (c => this.GetSerializationConverter (iTypeConverter, c)).ToList ();
			var isIdColumn = this.Columns.Select (c => c.IsIdColumn && decrementIds).ToList ();

			foreach (IList<string> row in rows)
			{
				yield return this.ProcessRowRead (converters, isIdColumn, row);
			}
		}


		private IList<object> ProcessRowRead(IList<ISerializationConverter> converters, IList<bool> isIdColumn, IList<string> row)
		{
			List<object> processedRow = new List<object> ();

			for (int i = 0; i < row.Count; i++)
			{
				string valueAsString = row[i];

				ISerializationConverter converter = converters[i];

				object valueAsObject = (valueAsString == null) ? null : converter.ConvertFromString (valueAsString, null);

				if (isIdColumn[i] && valueAsObject is long)
				{
					valueAsObject = ((long) valueAsObject) - EntitySchemaBuilder.AutoIncrementStartValue;
				}

				processedRow.Add (valueAsObject);
			}

			return processedRow;
		}


		private void InsertRows(DbInfrastructure dbInfrastructure, EntityModificationEntry entityModificationEntry, IEnumerable<IList<object>> rows)
		{
			using (DbTransaction dbTransaction = dbInfrastructure.BeginTransaction (DbTransactionMode.ReadWrite))
			{
				foreach (IList<object> row in rows)
				{
					this.InsertRow (dbInfrastructure, dbTransaction, entityModificationEntry, row);
				}

				dbTransaction.Commit ();
			}
		}


		private void InsertRow(DbInfrastructure dbInfrastructure, DbTransaction dbTransaction, EntityModificationEntry entityModificationEntry, IList<object> row)
		{
			string tableName = this.SqlName;
			SqlFieldList sqlFields = new SqlFieldList ();

			int index = 0;

			foreach (ColumnDefinition column in this.Columns)
			{
				DbRawType internalDbRawType = this.GetInternalRawType (dbInfrastructure.Converter, column.DbRawType);

				SqlField sqlField = SqlField.CreateConstant (row[index], internalDbRawType);
				sqlField.Alias = column.Name;

				sqlFields.Add (sqlField);

				index++;
			}

			if (this.Category == TableCategory.Data && this.ContainsLogColumn)
			{
				sqlFields.Add (this.CreateSqlFieldForEntitModificationLog (entityModificationEntry));
			}

			dbTransaction.SqlBuilder.InsertData (tableName, sqlFields);
			dbInfrastructure.ExecuteNonQuery (dbTransaction);
		}


		private void UpdateAutoIncrementStartValue(DbInfrastructure dbInfrastructure)
		{
			using (DbTransaction dbTransaction = dbInfrastructure.BeginTransaction(DbTransactionMode.ReadWrite))
			{
				string rowIdColumnName = this.GetRowIdColumnName ();

				DbTable dbTable = dbInfrastructure.ResolveDbTable (dbTransaction, this.DbName);
				DbColumn dbColumn = dbTable.Columns[rowIdColumnName];

				if (dbColumn != null && dbColumn.IsAutoIncremented)
				{
					SqlSelect query = new SqlSelect ();

					query.Tables.Add (SqlField.CreateName (dbTable.GetSqlName ()));
					query.Fields.Add
					(
						SqlField.CreateAggregate
						(
							SqlAggregateFunction.Max,
							SqlField.CreateName (dbColumn.GetSqlName ())
						)
					);
					
					dbTransaction.SqlBuilder.SelectData (query);

					object value = dbInfrastructure.ExecuteScalar (dbTransaction);
					long startValue = System.Math.Max ((long) value, EntitySchemaBuilder.AutoIncrementStartValue);


					dbInfrastructure.SetColumnAutoIncrementValue (dbTransaction, dbTable, dbColumn, startValue);
				}

				dbTransaction.Commit ();
			}
		}


		private SqlField CreateSqlFieldForEntitModificationLog(EntityModificationEntry entityModificationEntry)
		{
			SqlField sqlField = SqlField.CreateConstant (entityModificationEntry.EntryId, DbRawType.Int64);
			sqlField.Alias = EntitySchemaBuilder.EntityTableColumnEntityModificationEntryIdName;

			return sqlField;
		}


		private ISerializationConverter GetSerializationConverter(ITypeConverter iTypeConverter, ColumnDefinition column)
		{
			DbRawType rawType = this.GetInternalRawType (iTypeConverter, column.DbRawType);

			System.Type adoType = TypeConverter.GetAdoType (rawType);

			return InvariantConverter.GetSerializationConverter (adoType);
		}



		private DbRawType GetInternalRawType(ITypeConverter iTypeConverter, DbRawType rawType)
		{
			DbRawType internalRawType = rawType;

			if (!iTypeConverter.CheckNativeSupport (internalRawType))
			{
				IRawTypeConverter iRawTypeConverter;

				if (!iTypeConverter.GetRawTypeConverter (internalRawType, out iRawTypeConverter))
				{
					throw new System.Exception ("Cannot convert type.");
				}

				internalRawType = iRawTypeConverter.InternalType;
			}

			return internalRawType;
		}


		private string GetRowIdColumnName()
		{
			switch (this.Category)
			{
				case TableCategory.Data:
					return EntitySchemaBuilder.EntityTableColumnIdName;

				case TableCategory.Relation:
					return EntitySchemaBuilder.EntityFieldTableColumnIdName;
					
				default:
					throw new System.NotImplementedException ();
			}
		}


		public void Clean(DbInfrastructure dbInfrastructure, bool cleanOnlyEpsitecData)
		{
			using (DbTransaction dbTransaction = dbInfrastructure.BeginTransaction (DbTransactionMode.ReadWrite))
			{
				string tableName = this.SqlName;
				SqlFieldList conditions = new SqlFieldList ();

				if (cleanOnlyEpsitecData)
				{
					conditions.Add (this.CreateConditionForInterval (0, EntitySchemaBuilder.AutoIncrementStartValue));
				}

				dbTransaction.SqlBuilder.RemoveData (tableName, conditions);
				dbInfrastructure.ExecuteNonQuery (dbTransaction);

				dbTransaction.Commit ();
			}
		}


        public override string ToString()
		{
			string value = "Table : Name = " + this.SqlName;

			foreach (ColumnDefinition column in this.Columns)
			{
				value += "\n" + column.ToString ();
			}

			return value;
		}


	}


}
