using Epsitec.Common.Types;

using Epsitec.Cresus.Database;
using Epsitec.Cresus.Database.Collections;
using Epsitec.Cresus.Database.Services;

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


		public TableDefinition(string name, TableCategory category, bool containsLogColumn, IEnumerable<ColumnDefinition> columns)
		{
			this.Name = name;
			this.Category = category;
			this.ContainsLogColumn = containsLogColumn;
			this.Columns = columns.ToList ();
		}


		public string Name
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
			this.WriteXmlName (xmlWriter);
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


		private void WriteXmlName(XmlWriter xmlWriter)
		{
			xmlWriter.WriteStartElement ("name");
			xmlWriter.WriteValue (this.Name);
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



		public void WriteXmlData(DbInfrastructure dbInfrastructure, XmlWriter xmlWriter, int index)
		{
			this.WriteXmlStart (xmlWriter, index);

			foreach (IList<string> row in this.ProcessRowsWrite(dbInfrastructure.Converter, this.GetRows (dbInfrastructure)))
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


		private IEnumerable<IList<object>> GetRows(DbInfrastructure dbInfrastructure)
		{
			SqlField tableSqlField = this.CreateTableSqlField ();
			IList<SqlField> columnSqlFields = this.CreateColumnSqlFields ();

			int startIndex = DbInfrastructure.AutoIncrementStartIndex;
			int length = 1000;

			using (DbTransaction transaction = dbInfrastructure.BeginTransaction (DbTransactionMode.ReadOnly))
			{
				bool done = false;

				for (long i = startIndex; !done; i += length)
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

				transaction.Commit ();
			}
		}


		private SqlField CreateTableSqlField()
		{
			return SqlField.CreateName (this.Name);
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
			return SqlField.CreateFunction
			(
				new SqlFunction (
					SqlFunctionCode.LogicAnd,
					SqlField.CreateFunction
					(
						new SqlFunction
						(
							SqlFunctionCode.CompareGreaterThanOrEqual,
							SqlField.CreateName (Tags.ColumnId),
							SqlField.CreateConstant (firstIndex, DbRawType.Int64)
						)
					),
					SqlField.CreateFunction
					(
						new SqlFunction
						(
							SqlFunctionCode.CompareLessThanOrEqual,
							SqlField.CreateName (Tags.ColumnId),
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

			string name = TableDefinition.ReadXmlName (xmlReader);
			TableCategory category = TableDefinition.ReadXmlCategory (xmlReader);
			bool containsLogColumn = TableDefinition.ReadXmlContainsLogColumn (xmlReader);
			IEnumerable<ColumnDefinition> columns = TableDefinition.ReadXmlColumns (xmlReader);

			TableDefinition.ReadXmlEnd (xmlReader);


			return new TableDefinition (name, category, containsLogColumn, columns);
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


		private static string ReadXmlName(XmlReader xmlReader)
		{
			xmlReader.ReadStartElement ("name");

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


		public void ReadXmlData(DbInfrastructure dbInfrastructure, XmlReader xmlReader, DbLogEntry dbLogEntry, int index, bool decrementIds)
		{
			bool isEmpty = xmlReader.IsEmptyElement;

			TableDefinition.ReadXmlStart (xmlReader, index);

			if (!isEmpty)
			{
				this.InsertRows (dbInfrastructure, dbLogEntry, this.ProcessRowsRead (dbInfrastructure.Converter, decrementIds, this.ReadXmlRows (xmlReader)));

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

				if (isIdColumn[i])
				{
					valueAsObject = ((long) valueAsObject) - DbInfrastructure.AutoIncrementStartIndex;
				}

				processedRow.Add (valueAsObject);
			}

			return processedRow;
		}


		private void InsertRows(DbInfrastructure dbInfrastructure, DbLogEntry dbLogEntry, IEnumerable<IList<object>> rows)
		{
			using (DbTransaction dbTransaction = dbInfrastructure.BeginTransaction (DbTransactionMode.ReadWrite))
			{
				foreach (IList<object> row in rows)
				{
					this.InsertRow (dbInfrastructure, dbTransaction, dbLogEntry, row);
				}

				dbTransaction.Commit ();
			}
		}


		private void InsertRow(DbInfrastructure dbInfrastructure, DbTransaction dbTransaction, DbLogEntry dbLogEntry, IList<object> row)
		{
			string tableName = this.Name;
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
				sqlFields.Add (this.CreateSqlFieldForLog (dbLogEntry));
			}

			dbTransaction.SqlBuilder.InsertData (tableName, sqlFields);
			dbInfrastructure.ExecuteNonQuery (dbTransaction);
		}


		private SqlField CreateSqlFieldForLog(DbLogEntry logEntryId)
		{
			SqlField sqlField = SqlField.CreateConstant (logEntryId.EntryId, DbRawType.Int64);
			sqlField.Alias = Tags.ColumnRefLog;

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

		public void Clean(DbInfrastructure dbInfrastructure, bool cleanWholeTable)
		{
			using (DbTransaction dbTransaction = dbInfrastructure.BeginTransaction (DbTransactionMode.ReadWrite))
			{
				string tableName = this.Name;
				SqlFieldList conditions = new SqlFieldList ();

				if (!cleanWholeTable)
				{
					conditions.Add (this.CreateConditionForInterval (0, DbInfrastructure.AutoIncrementStartIndex));
				}

				dbTransaction.SqlBuilder.RemoveData (tableName, conditions);
				dbInfrastructure.ExecuteNonQuery (dbTransaction);

				dbTransaction.Commit ();
			}
		}


        public override string ToString()
		{
			string value = "Table : Name = " + this.Name;

			foreach (ColumnDefinition column in this.Columns)
			{
				value += "\n" + column.ToString ();
			}

			return value;
		}


	}


}
