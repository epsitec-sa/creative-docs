//	Copyright © 2003-2011, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support.Extensions;

using FirebirdSql.Data.FirebirdClient;

using System.Collections.Generic;

using System.Globalization;

using System.Linq;

namespace Epsitec.Cresus.Database.Implementation
{
	/// <summary>
	/// The <c>FirebirdSqlBuilder</c> class implements the <c>ISqlBuilder</c> interface
	/// for the Firebird engine.
	/// </summary>
	internal sealed class FirebirdSqlBuilder : ISqlBuilder
	{
		public FirebirdSqlBuilder(FirebirdAbstraction fb)
		{
			this.fb = fb;
			this.buffer = new System.Text.StringBuilder ();
			this.commandParams = new List<SqlField> ();
			this.tableAliases = new Dictionary<string, string> ();
		}

		#region ISqlBuilder Members
		
		public ISqlBuilder NewSqlBuilder()
		{
			return new FirebirdSqlBuilder (this.fb);
		}


		public bool								AutoClear
		{
			get
			{
				return this.autoClear;
			}
			set
			{
				this.autoClear = value;
			}
		}

		public DbCommandType					CommandType
		{
			get
			{
				return this.commandType;
			}
		}

		public int								CommandCount
		{
			get
			{
				return this.commandCount;
			}
		}

		public System.Data.IDbCommand			Command
		{
			get
			{
				this.UpdateCommand ();
				return this.commandCache;
			}
		}		
		
		public System.Data.IDbCommand CreateCommand(System.Data.IDbTransaction transaction)
		{
			this.UpdateCommand ();
			this.commandCache.Transaction = transaction as FirebirdSql.Data.FirebirdClient.FbTransaction;
			return this.commandCache;
		}
		
		public System.Data.IDbCommand CreateCommand(System.Data.IDbTransaction transaction, string text)
		{
			FbCommand command = this.fb.NewDbCommand () as FbCommand;
			command.CommandText = text;
			command.CommandType = System.Data.CommandType.Text;
			command.Transaction = transaction as FirebirdSql.Data.FirebirdClient.FbTransaction;
			return command;
		}
				
		public void Clear()
		{
			//	On n'a pas le droit de faire un Dispose de l'objet 'commande', car il peut encore
			//	être utilisé par un appelant. C'est le cas lorsque l'on est en mode AutoClear.
			
			this.expectMore    = false;
			this.commandCache  = null;
			this.commandType   = DbCommandType.None;
			this.commandCount  = 0;
			this.buffer.Length = 0;
			this.commandParams.Clear ();
			this.tableAliases.Clear ();
		}
		
		public void AppendMore()
		{
			if (this.expectMore)
			{
				throw new Exceptions.GenericException (this.fb.DbAccess, "AppendMore called twice");
			}
			
			this.expectMore = true;
		}

		
		public bool ValidateName(string value)
		{
			return DbSqlStandard.ValidateName (value);
		}
		
		public bool ValidateQualifiedName(string value)
		{
			return DbSqlStandard.ValidateQualifiedName (value);
		}

		public bool ValidateString(string value)
		{
			return DbSqlStandard.ValidateString (value);
		}

		public bool ValidateNumber(string value)
		{
			return DbSqlStandard.ValidateNumber (value);
		}

		public void InsertTable(SqlTable table)
		{
			if (!this.ValidateName (table.Name))
			{
				throw new Exceptions.SyntaxException (this.fb.DbAccess, string.Format ("Invalid table {0}", table.Name));
			}
			
			this.PrepareCommand ();
			
			this.commandType = DbCommandType.Silent;
			this.commandCount++;
			
			this.Append ("CREATE TABLE ");
			this.Append (table.Name);
			this.Append ("(");
			
			bool isFirstField = true;
			
			foreach (SqlColumn column in table.Columns)
			{
				if (!this.ValidateName (column.Name))
				{
					throw new Exceptions.SyntaxException (this.fb.DbAccess, string.Format ("Invalid column {0} in table {1}", column.Name, table.Name));
				}
				
				if (isFirstField)
				{
					isFirstField = false;
				}
				else
				{
					this.Append (", ");
				}
				
				this.Append (column.Name);
				this.Append (' ');
				this.Append (this.GetSqlType (column));
				this.Append (this.GetSqlColumnAttributes (column));
			}
			
			this.Append (");\n");
			
			if (table.HasPrimaryKey)
			{
				this.commandCount++;
				
				this.Append ("ALTER TABLE ");
				this.Append (table.Name);
				this.Append (" ADD CONSTRAINT ");
				this.Append (DbSqlStandard.FitNameToMaximumLength (DbSqlStandard.ConcatNames ("PK_", table.Name)));
				this.Append (" PRIMARY KEY ");
				this.Append ("(");
				
				isFirstField = true;
				
				foreach (SqlColumn column in table.PrimaryKey)
				{
					if (!table.Columns.Contains (column))
					{
						throw new Exceptions.SyntaxException (this.fb.DbAccess, string.Format ("Column {0} specified as primary key, not found in table {1}", column.Name, table.Name));
					}
					
					if (isFirstField)
					{
						isFirstField = false;
					}
					else
					{
						this.Append (", ");
					}
				
					this.Append (column.Name);
				}
				
				this.Append (");\n");

				System.Diagnostics.Debug.Assert (isFirstField == false);
			}

			if (table.HasForeignKeys)
			{
				//	TODO: add the missing foreign key (FK_) constraints
			}

			if (table.HasForeignKeys)
			{
				//	TODO: add the missing foreign key (FK_) constraints
			}

			if (!string.IsNullOrEmpty (table.Comment))
			{
				this.BuildSetTableCommentSqlCommand (table.Name, table.Comment);
			}

			foreach (SqlColumn column in table.Columns)
			{
				if (!string.IsNullOrEmpty (column.Comment))
				{
					this.BuildSetColumnCommentSqlCommand (table.Name, column.Name, column.Comment);
				}
			}
		}

		public void SetTableComment(string tableName, string comment)
		{
			this.PrepareCommand ();

			this.commandType = DbCommandType.Silent;

			this.BuildSetTableCommentSqlCommand (tableName, comment);
		}

		private void BuildSetTableCommentSqlCommand(string tableName, string comment)
		{
			this.BuildSetCommentSqlCommand ("TABLE", tableName, comment);
		}

		private void BuildSetColumnCommentSqlCommand(string tableName, string columnName, string comment)
		{
			this.BuildSetCommentSqlCommand ("COLUMN", tableName + "." + columnName, comment);
		}

		private void BuildSetCommentSqlCommand(string objectType, string objectName, string comment)
		{
			this.commandCount++;

			string escapedComment = comment.Replace ("'", "''");

			string query = "COMMENT ON " + objectType + " " + objectName + " IS '" + escapedComment + "';\n";

			this.Append (query);
		}

		public void RemoveTable(SqlTable table)
		{
			if (!this.ValidateName (table.Name))
			{
				throw new Exceptions.SyntaxException (this.fb.DbAccess, string.Format ("Invalid table {0}", table.Name));
			}

			this.PrepareCommand ();

			this.commandType = DbCommandType.Silent;

			this.commandCount++;

			this.Append ("DROP TABLE ");
			this.Append (table.Name);
			this.Append (";\n");
		}

		public void InsertTableColumns(string tableName, SqlColumn[] columns)
		{
			if (!this.ValidateName (tableName))
			{
				throw new Exceptions.SyntaxException (this.fb.DbAccess, string.Format ("Invalid table {0}", tableName));
			}

			this.PrepareCommand ();

			this.commandType = DbCommandType.Silent;

			foreach (SqlColumn column in columns)
			{
				if (!this.ValidateName (column.Name))
				{
					throw new Exceptions.SyntaxException (this.fb.DbAccess, string.Format ("Invalid column {0} in table {1}", column.Name, tableName));
				}

				this.commandCount++;

				this.Append ("ALTER TABLE ");
				this.Append (tableName);
				this.Append (" ADD ");			//	not " ADD COLUMN "
				this.Append (column.Name);
				this.Append (" ");
				this.Append (this.GetSqlType (column));
				this.Append (this.GetSqlColumnAttributes (column));
				this.Append (";\n");

				if (!string.IsNullOrEmpty (column.Comment))
				{
					this.BuildSetColumnCommentSqlCommand (tableName, column.Name, column.Comment);
				}
			}
		}

		public void RenameTableColumn(string tableName, string oldColumnName, string newColumnName)
		{
			tableName.ThrowIf (n => !this.ValidateName (n), "invalid table name");
			oldColumnName.ThrowIf (n => !this.ValidateName (n), "invalid old column name");
			newColumnName.ThrowIf (n => !this.ValidateName (n), "invalid new column name");

			this.PrepareCommand ();
			this.commandType = DbCommandType.Silent;

			this.commandCount++;
			this.Append
			(
				"ALTER TABLE " + tableName + " ALTER " + oldColumnName + " TO " + newColumnName + ";\n"
			);
		}

		public void RemoveTableColumns(string tableName, SqlColumn[] columns)
		{
			if (!this.ValidateName (tableName))
			{
				throw new Exceptions.SyntaxException (this.fb.DbAccess, string.Format ("Invalid table {0}", tableName));
			}

			if (columns.Length == 0)
			{
				return;
			}

			this.PrepareCommand ();

			this.commandType = DbCommandType.Silent;

			foreach (SqlColumn column in columns)
			{
				if (!this.ValidateName (column.Name))
				{
					throw new Exceptions.SyntaxException (this.fb.DbAccess, string.Format ("Invalid column {0} in table {1}", column.Name, tableName));
				}

				this.commandCount++;

				this.Append ("ALTER TABLE ");
				this.Append (tableName);
				this.Append (" DROP ");		// not " DROP COLUMN "
				this.Append (column.Name);
				this.Append (";\n");
			}
		}

		public void SetAutoIncrementOnTableColumn(string tableName, string columnName, long initialValue)
		{
			// Firebird does not support directly the auto incremented columns, so we need to use
			// a workaround described here : http://www.firebirdfaq.org/faq29/ . Basically, we need
			// to create an sequence generator and call that generator in a trigger before a row is
			// inserted in the table.
			// Marc

			this.PrepareCommand ();
			this.commandType = DbCommandType.Silent;

			string generatorName = this.GetAutoIncrementGeneratorName (tableName, columnName);
			string triggerName = this.GetAutoIncrementTriggerName (tableName, columnName);

			this.commandCount++;
			this.Append ("CREATE GENERATOR " + generatorName + ";\n");

			this.commandCount++;
			this.Append ("SET GENERATOR " + generatorName + " TO " + initialValue + ";\n");

			this.commandCount++;
			this.Append
			(
				  "CREATE TRIGGER " + triggerName + " FOR " + tableName + " "
				+ "ACTIVE BEFORE INSERT POSITION 0 "
				+ "AS "
				+ "BEGIN "
				+ "IF (NEW." + columnName + " IS NULL) THEN NEW." + columnName + " = GEN_ID(" + generatorName + ", 1); "
				+ "END;\n"
			);
		}

		public void DropAutoIncrementOnTableColumn(string tableName, string columnName)
		{
			// As Firebird does not support directly auto incremented columns, we need a way to
			// cleanup the database, for instance if we are removing such a column. So first we
			// remove the sequence generator and then we remove the trigger.
			// Marc.

			this.PrepareCommand ();
			this.commandType = DbCommandType.Silent;

			string generatorName = this.GetAutoIncrementGeneratorName (tableName, columnName);
			string triggerName = this.GetAutoIncrementTriggerName (tableName, columnName);

			this.commandCount++;
			this.Append ("DROP GENERATOR " + generatorName + ";\n");

			this.commandCount++;
			this.Append ("DROP TRIGGER " + triggerName + ";\n");
		}

		private string GetAutoIncrementGeneratorName(string tableName, string columnName)
		{
			string generatorName = tableName + "_" + columnName + "_gid";

			if (generatorName.Length > 32)
			{
				throw new System.Exception ("Generator name for column " + columnName + " of table " + tableName + " is too long.");
			}

			return generatorName;
		}

		private string GetAutoIncrementTriggerName(string tableName, string columnName)
		{
			string triggerName = tableName + "_" + columnName + "_tid";

			if (triggerName.Length > 32)
			{
				throw new System.Exception ("Trigger name for column " + columnName + " of table " + tableName + " is too long.");
			}

			return triggerName;
		}

		public void SetAutoTimeStampOnTableColumn(string tableName, string columnName, bool onInsert, bool onUpdate)
		{
			// Firebird does not support directly the auto timestamp columns, so we need to use
			// a workaround described here : http://www.firebirdfaq.org/faq77/ . Basically, we need
			// to create a trigger that assigns the timestamp on insert and updates.
			// Marc

			if (!onInsert && !onUpdate)
			{
				throw new System.ArgumentException ();
			}

			this.PrepareCommand ();
			this.commandType = DbCommandType.Silent;

			string triggerName = this.GetAutoTimeStampTriggerName (tableName, columnName);

			this.commandCount++;

			var insert = (onInsert ? "INSERT" : "");
			var or = (onInsert && onUpdate ? " OR " : "");
			var update = (onUpdate ? "UPDATE" : "");

			this.Append
			(
				  "CREATE TRIGGER " + triggerName + " FOR " + tableName + " "
				+ "ACTIVE BEFORE " + insert + or + update + " POSITION 0 "
				+ "AS "
				+ "BEGIN "
				+ "NEW." + columnName + " = CAST('NOW' AS TIMESTAMP);"
				+ "END;\n"
			);
		}

		public void DropAutoTimeStampOnTableColumn(string tableName, string columnName)
		{
			// As Firebird does not support directly auto timestamp columns, we need to provide a
			// way to cleanup the database, for instance if we are removing such a column.
			// Marc

			this.PrepareCommand ();
			this.commandType = DbCommandType.Silent;

			string triggerName = this.GetAutoTimeStampTriggerName (tableName, columnName);

			this.commandCount++;
			this.Append ("DROP TRIGGER " + triggerName + ";\n");
		}

		private string GetAutoTimeStampTriggerName(string tableName, string columnName)
		{
			string triggerName = tableName + "_" + columnName + "_tts";

			if (triggerName.Length > 32)
			{
				throw new System.Exception ("Trigger name for column " + columnName + " of table " + tableName + " is too long.");
			}

			return triggerName;
		}        

		public void CreateIndex(string tableName, SqlIndex index)
		{
			this.PrepareCommand ();
			this.commandType = DbCommandType.Silent;

			this.commandCount++;

			this.Append ("CREATE ");

			switch (index.SortOrder)
			{
				case SqlSortOrder.Ascending:
					this.Append ("ASC");
					break;

				case SqlSortOrder.Descending:
					this.Append ("DESC");
					break;
			}

			this.Append (" INDEX " + index.Name + " ON " + tableName + " (");
			this.Append (string.Join (", ", index.Columns.Select (c => c.Name)));
			this.Append (");\n");
		}
		
		public void ResetIndex(SqlIndex index)
		{
			this.PrepareCommand ();
			this.commandType = DbCommandType.Silent;

			this.commandCount++;
			this.Append ("ALTER INDEX " + index.Name + " INACTIVE;\n");

			this.commandCount++;
			this.Append ("ALTER INDEX " + index.Name + " ACTIVE;\n");

			this.commandCount++;
			this.Append ("SET STATISTICS INDEX " + index.Name + ";\n");
		}

		public void DropIndex(SqlIndex index)
		{
			this.PrepareCommand ();
			this.commandType = DbCommandType.Silent;

			this.commandCount++;

			this.Append ("DROP INDEX " + index.Name + ";\n");
		}

		public void SelectData(SqlSelect sqlQuery)
		{
			this.PrepareCommand ();
			
			this.commandType = DbCommandType.ReturningData;
			this.commandCount++;
			
			this.Append (sqlQuery);			
			this.Append (";\n");
		}

		public void InsertData(string tableName, Collections.SqlFieldList fields)
		{
			if (!this.ValidateName (tableName))
			{
				throw new Exceptions.SyntaxException (this.fb.DbAccess, string.Format ("Invalid table {0}", tableName));
			}
			
			if (fields.Count == 0)
			{
				return;
			}
			
			this.PrepareCommand ();
			this.commandType = DbCommandType.NonQuery;
			this.commandCount++;

			this.BuildInsertSqlClause (tableName, fields);
			
			this.Append (";\n");
		}
		
		public void InsertData(string tableName, Collections.SqlFieldList fieldsToInsert, Collections.SqlFieldList fieldsToReturn)
		{
			if (!this.ValidateName (tableName))
			{
				throw new Exceptions.SyntaxException (this.fb.DbAccess, string.Format ("Invalid table {0}", tableName));
			}

			if (fieldsToInsert.Count == 0)
			{
				return;
			}

			this.PrepareCommand ();
			this.commandType = DbCommandType.NonQuery;
			this.commandCount++;

			this.BuildInsertSqlClause (tableName, fieldsToInsert);
			this.BuildReturningSqlClause (tableName, fieldsToInsert, fieldsToReturn);

			this.Append (";\n");
		}

		public void BuildInsertSqlClause(string tableName, Collections.SqlFieldList fieldsToInsert)
		{
			this.Append ("INSERT INTO ");
			this.Append (tableName);
			this.Append ("(");

			bool isFirstField = true;

			foreach (SqlField field in fieldsToInsert)
			{
				if (!this.ValidateName (field.Alias))
				{
					throw new Exceptions.SyntaxException (this.fb.DbAccess, string.Format ("Invalid field '{0}' in table {1}", field.Alias, tableName));
				}

				if (isFirstField)
				{
					isFirstField = false;
				}
				else
				{
					this.Append (",");
				}

				this.Append (field.Alias);
			}

			System.Diagnostics.Debug.Assert (isFirstField == false);

			this.Append (") VALUES (");

			isFirstField = true;

			foreach (SqlField field in fieldsToInsert)
			{
				string data = null;

				switch (field.FieldType)
				{
					case SqlFieldType.Default:
						data = "DEFAULT";
						break;
					case SqlFieldType.Null:
						data = "NULL";
						break;
					case SqlFieldType.Constant:
						data = this.MakeCommandParam (field);
						break;

					default:
						throw new Exceptions.SyntaxException (this.fb.DbAccess, string.Format ("Invalid field type {0} for field {1} in table {2}", field.FieldType.ToString (), field.Alias, tableName));
				}

				if (isFirstField)
				{
					isFirstField = false;
				}
				else
				{
					this.Append (",");
				}

				this.Append (data);
			}

			this.Append (") ");

			System.Diagnostics.Debug.Assert (isFirstField == false);
		}



		private void BuildReturningSqlClause(string tableName, Collections.SqlFieldList fieldsToInsert, Collections.SqlFieldList fieldsToReturn)
		{
			if (fieldsToReturn.Count > 0)
			{
				if (fieldsToInsert.Count == 0)
				{
					throw new Exceptions.SyntaxException (this.fb.DbAccess, "Invalid query: returning clause without inserted fields in insert clause.");
				}

				this.Append ("RETURNING ");

				bool isFirstField = true;

				foreach (SqlField field in fieldsToReturn)
				{
					if (!this.ValidateName (field.Alias))
					{
						throw new Exceptions.SyntaxException (this.fb.DbAccess, string.Format ("Invalid field '{0}' in table {1}", field.Alias, tableName));
					}

					if (isFirstField)
					{
						isFirstField = false;
					}
					else
					{
						this.Append (", ");
					}

					this.Append (field.Alias);
					this.MakeCommandParam (SqlField.CreateParameterOut ());
				}

				System.Diagnostics.Debug.Assert (isFirstField == false);
			}
		}

		public void UpdateData(string tableName, Collections.SqlFieldList fields, Collections.SqlFieldList conditions)
		{
			if (!this.ValidateName (tableName))
			{
				throw new Exceptions.SyntaxException (this.fb.DbAccess, string.Format ("Invalid table {0}", tableName));
			}

			if (fields.Count == 0)
			{
				throw new Exceptions.SyntaxException (this.fb.DbAccess, string.Format ("No field specified in UPDATE"));
			}
			
			this.PrepareCommand ();
			
			this.commandType = DbCommandType.NonQuery;
			this.commandCount++;
			
			this.Append ("UPDATE ");
			this.Append (tableName);
			
			bool isFirstField = true;
			
			foreach (SqlField field in fields)
			{
				if (isFirstField)
				{
					this.Append (" SET ");
					isFirstField = false;
				}
				else
				{
					this.Append (",");
				}
				
				this.Append (field.Alias);
				this.Append (" = ");
				this.Append (field);
			}

			System.Diagnostics.Debug.Assert (isFirstField == false);
			
			isFirstField = true;
			
			if ((conditions != null) &&
				(conditions.Count > 0))
			{
				foreach (SqlField field in conditions)
				{
					if (isFirstField)
					{
						this.Append (" WHERE ");
						isFirstField = false;
					}
					else
					{
						this.Append (" AND ");
					}
					
					if (field.FieldType != SqlFieldType.Function)
					{
						throw new Exceptions.SyntaxException (this.fb.DbAccess, string.Format ("Invalid field {0} in UPDATE ... WHERE", field.AsName));
					}
					
					this.Append (field.AsFunction);
				}
			}
			
			this.Append (";\n");
		}

		public void RemoveData(string tableName, Collections.SqlFieldList conditions)
		{
			if (!this.ValidateName (tableName))
			{
				throw new Exceptions.SyntaxException (this.fb.DbAccess, string.Format ("Invalid table {0}", tableName));
			}
			
			this.PrepareCommand ();
			
			this.commandType = DbCommandType.NonQuery;
			this.commandCount++;
			
			this.Append ("DELETE FROM ");
			this.Append (tableName);
			
			if ((conditions != null) &&
				(conditions.Count > 0))
			{
				bool isFirstField = true;
				
				foreach (SqlField field in conditions)
				{
					if (isFirstField)
					{
						this.Append (" WHERE ");
						isFirstField = false;
					}
					else
					{
						this.Append (" AND ");
					}

					if (field.FieldType != SqlFieldType.Function)
					{
						throw new Exceptions.SyntaxException (this.fb.DbAccess, string.Format ("Invalid field {0} in UPDATE ... WHERE", field.AsName));
					}

					this.Append (field.AsFunction);
				}
			}
			
			this.Append (";\n");
		}

		
		public void ExecuteProcedure(string procedureName, Collections.SqlFieldList fields)
		{
			//	TODO:  Add FirebirdSqlBuilder.ExecuteProcedure implementation
			
			throw new System.NotImplementedException ();
		}

		public void GetCurrentTimeStamp()
		{
			this.PrepareCommand ();

			this.commandType = DbCommandType.ReturningData;
			this.commandCount++;

			this.Append ("SELECT (" + this.GetSqlFieldForCurrentTimeStamp().AsRawSql + ") FROM RDB$DATABASE");
		}

		public SqlField GetSqlFieldForCurrentTimeStamp()
		{
			string rawSql = "CAST('NOW' AS TIMESTAMP)";

			return SqlField.CreateRawSql (rawSql);
		}

				
		public void GetSqlParameters(System.Data.IDbCommand command, Collections.SqlFieldList fields)
		{
			//	TODO:  Add FirebirdSqlBuilder.GetSqlParameters implementation
#if false
			FbCommand fbCommand = command as FbCommand;
			
			for (int i = 0; i < fbCommand.Parameters.Count; i++)
			{
				FbParameter param = fbCommand.Parameters[i] as FbParameter;
				
				System.Diagnostics.Debug.Assert (param != null);
				System.Diagnostics.Debug.WriteLine (param.ParameterName + ": " + param.FbDbType.ToString ());
				
				switch (param.Direction)
				{
					case System.Data.ParameterDirection.Input:
						break;
					
					case System.Data.ParameterDirection.InputOutput:
					case System.Data.ParameterDirection.Output:
						break;
					
					case System.Data.ParameterDirection.ReturnValue:
						break;
				}
			}
#else
			throw new System.NotImplementedException ();
#endif
		}

		public void SetSqlParameters(System.Data.IDbCommand command, Collections.SqlFieldList fields)
		{
			//	TODO:  Add FirebirdSqlBuilder.SetSqlParameters implementation
			throw new System.NotImplementedException ();
		}

		
		public void SetCommandParameterValue(System.Data.IDbCommand command, int index, object value)
		{
			FbCommand   fbCommand = command as FbCommand;
			FbParameter fbParam   = fbCommand.Parameters[index] as FbParameter;
			
			fbParam.Value = value;
		}
		
		public object GetCommandParameterValue(System.Data.IDbCommand command, int index)
		{
			FbCommand   fbCommand = command as FbCommand;
			FbParameter fbParam   = fbCommand.Parameters[index] as FbParameter;
			
			return fbParam.Value;
		}

		public char[] GetSupportedCompareLikeWildcards()
		{
			return new char[] { '%', '_' };
		}
		
		#endregion
		
		#region IDisposable Members
		
		public void Dispose()
		{
			if (this.commandCache != null)
			{
				this.commandCache.Dispose ();
				this.commandCache = null;
			}
		}
		
		#endregion

		private void PrepareCommand()
		{
			if (this.expectMore)
			{
				this.expectMore = false;
			}
			else
			{
				if (this.autoClear)
				{
					this.Clear ();
				}

				if (this.commandType != DbCommandType.None)
				{
					throw new Exceptions.GenericException (this.fb.DbAccess, "Previous command not cleared");
				}
			}
		}

		private void UpdateCommand()
		{
			if (this.expectMore)
			{
				throw new Exceptions.SyntaxException (this.fb.DbAccess, string.Format ("Command is defined partially: AppendMore called without additional commands"));
			}
			
			if (this.commandCache == null)
			{
				this.commandCache = this.fb.NewDbCommand () as FbCommand;
				
				int fieldIndex = 0;
				
				//	Construit l'objet commande en se basant sur les paramètres définis pour
				//	celle-ci.
				
				foreach (SqlField field in this.commandParams)
				{
					FbParameter fbParam = new FbParameter ()
					{
						ParameterName = string.Format (TypeConverter.InvariantFormatProvider, "@PARAM_{0}", fieldIndex++)
					};

					switch (field.FieldType)
					{
						case SqlFieldType.ParameterIn:
					
							// SqlFieldType.ParameterIn is the same as SqlFieldType.Constant.
							// Marc

							fbParam.Value = field.AsConstant;
							fbParam.FbDbType = this.GetFbType (field.RawType);
							fbParam.SourceColumn = field.Alias;
							fbParam.Direction = System.Data.ParameterDirection.Input;

							break;

						case SqlFieldType.ParameterOut:

							fbParam.Direction = System.Data.ParameterDirection.Output;

							break;

						default:

							throw new System.NotImplementedException ();
					}
					
					this.commandCache.Parameters.Add (fbParam);
				}

				//	Pour l'instant, la commande est toujours de type texte et construite par les
				//	diverses méthodes publiques.
				
				this.commandCache.CommandType = System.Data.CommandType.Text;
				this.commandCache.CommandText = this.buffer.ToString ();
			}
		}
		
		private string MakeCommandParam(SqlField field)
		{
			//	Ajoute un champ SQL comme paramètre de la commande. C'est la méthode UpdateCommand qui
			//	va faire le lien entre le num du paramètre (@PARAM_n) et sa valeur, telle que définie
			//	dans le champ SqlField.
			
			string name = string.Format (TypeConverter.InvariantFormatProvider, "@PARAM_{0}", this.commandParams.Count);
			this.commandParams.Add (field);
			return name;
		}

		private void Append(string str)
		{
			this.buffer.Append (str);
		}

		private void Append(char c)
		{
			this.buffer.Append (c);
		}

		private void Append(SqlField field)
		{
			this.Append (field, false);
		}

		private void Append(SqlField field, bool onlyAcceptQualifiedNames)
		{
			//	Ajoute au buffer un champ, quel que soit son type

			switch (field.FieldType)
			{
				case SqlFieldType.Null:
					this.Append ("NULL");
					break;
				
				case SqlFieldType.All:
					this.Append ("*");
					break;
				
				case SqlFieldType.Constant:
				case SqlFieldType.ParameterOut:
				case SqlFieldType.ParameterInOut:
				case SqlFieldType.ParameterResult:
					this.Append (this.MakeCommandParam (field));
					break;
				
				case SqlFieldType.Name:
					if (onlyAcceptQualifiedNames)
					{
						throw new Exceptions.SyntaxException (this.fb.DbAccess, string.Format ("Not qualified name {0} in multiple tables SQL command", field.AsName));
					}
					this.Append (field.AsName);
					break;
				
				case SqlFieldType.QualifiedName:
					this.Append (this.GetQualifiedName (field));
					break;
				
				case SqlFieldType.Aggregate:
					this.Append (field.AsAggregate);
					break;
				
				case SqlFieldType.Function:
					this.Append (field.AsFunction, onlyAcceptQualifiedNames);
					break;
				
				case SqlFieldType.Procedure:
					this.Append (field.AsProcedure);
					break;
				
				case SqlFieldType.SubQuery:
					this.Append ('(');
					this.Append (field.AsSubQuery);
					this.Append (')');
					break;

				case SqlFieldType.RawSql:
					this.Append ('(');
					this.Append (field.AsRawSql);
					this.Append (')');
					break;

				case SqlFieldType.Set:
					SqlSet set = field.AsSet;

					var values = set.Values
						.Select (v => SqlField.CreateConstant (v, set.Type))
						.Select (c => this.MakeCommandParam (c));
										
					this.Append ('(');
					this.Append (string.Join(", ", values));
					this.Append (')');
					break;
				
				default:
					throw new Exceptions.SyntaxException (this.fb.DbAccess, string.Format ("Field {0} not supported", field.FieldType));
			}
		}

		private string GetQualifiedName(SqlField field)
		{
			if (this.tableAliases.Count > 0)
			{
				string tableAlias;
				string tableQualifier;
				string columnName;

				DbSqlStandard.SplitQualifiedName (field.AsQualifiedName, out tableQualifier, out columnName);

				if (this.tableAliases.TryGetValue (tableQualifier, out tableAlias))
				{
					System.Diagnostics.Debug.Fail ("Incorrect table qualifier used",
						/**/						string.Format ("The qualifier '{0}' was used for an aliased table; '{1}' should have been used instead for '{2}'", tableQualifier, tableAlias, field.AsQualifiedName));
					return DbSqlStandard.QualifyName (tableAlias, columnName);
				}
			}

			return field.AsQualifiedName;
		}
		
		private void Append(SqlAggregate sqlAggregate)
		{
			switch (sqlAggregate.Function)
			{
				case SqlAggregateFunction.Count:
					this.Append ("COUNT");
					break;
				
				case SqlAggregateFunction.Max:
					this.Append ("MAX");
					break;
				
				case SqlAggregateFunction.Min:
					this.Append ("MIN");
					break;
				
				case SqlAggregateFunction.Sum:
					this.Append ("SUM");
					break;
				
				case SqlAggregateFunction.Average:
					this.Append ("AVG");
					break;
				
				default:
					throw new Exceptions.SyntaxException (this.fb.DbAccess, string.Format ("Aggregate {0} not supported", sqlAggregate.Function));
			}

			this.Append ("(");

			switch (sqlAggregate.Predicate)
			{
				case SqlSelectPredicate.Distinct:
					this.Append ("DISTINCT ");
					break;

				case SqlSelectPredicate.All:
					// ALL is implicitely used when distinct is not used, so we don't need to add
					// anything to the query text in this case.
					break;

				default:
					throw new System.NotImplementedException();
			}
			
			this.Append (sqlAggregate.Field);
			this.Append (')');
		}

		private void Append(SqlFunction sqlFunction)
		{
			this.Append (sqlFunction, false);
		}

		private void Append(SqlFunction sqlFunction, bool onlyAcceptQualifiedNames)
		{
			System.Diagnostics.Debug.Assert (sqlFunction != null);

			//	Convertit la fonction en SQL

			switch (sqlFunction.ArgumentCount)
			{
				case 2:
					this.Append ('(');
					this.Append (sqlFunction.A, onlyAcceptQualifiedNames);
					
					switch (sqlFunction.Code)
					{
						case SqlFunctionCode.MathAdd:					this.Append (" + ");		break;
						case SqlFunctionCode.MathSubstract:				this.Append (" - ");		break;
						case SqlFunctionCode.MathMultiply:				this.Append (" * ");		break;
						case SqlFunctionCode.MathDivide:				this.Append (" / ");		break;
						case SqlFunctionCode.CompareEqual:				this.Append (" = ");		break;
						case SqlFunctionCode.CompareNotEqual:			this.Append (" <> ");		break;
						case SqlFunctionCode.CompareLessThan:			this.Append (" < ");		break;
						case SqlFunctionCode.CompareLessThanOrEqual:	this.Append (" <= ");		break;
						case SqlFunctionCode.CompareGreaterThan:		this.Append (" > ");		break;
						case SqlFunctionCode.CompareGreaterThanOrEqual:	this.Append (" >= ");		break;
						case SqlFunctionCode.CompareLike:				this.Append (" LIKE ");		break;
						case SqlFunctionCode.CompareLikeEscape:			this.Append (" LIKE ");		break;
						case SqlFunctionCode.CompareNotLike:			this.Append (" NOT LIKE ");	break;
						case SqlFunctionCode.CompareNotLikeEscape:		this.Append (" NOT LIKE ");	break;
						case SqlFunctionCode.SetIn:						this.Append (" IN ");		break;
						case SqlFunctionCode.SetNotIn:					this.Append (" NOT IN ");	break;
						case SqlFunctionCode.LogicAnd:					this.Append (" AND ");		break;
						case SqlFunctionCode.LogicOr:					this.Append (" OR ");		break;
						
						default:
							throw new Exceptions.SyntaxException (this.fb.DbAccess, string.Format ("SQL Function {0} not supported", sqlFunction.Code));
					}
					
					this.Append (sqlFunction.B, onlyAcceptQualifiedNames);

					switch (sqlFunction.Code)
					{
						case SqlFunctionCode.CompareLikeEscape:
						case SqlFunctionCode.CompareNotLikeEscape:
							if (sqlFunction.B.FieldType != SqlFieldType.Constant)
							{
								throw new Exceptions.SyntaxException (this.fb.DbAccess, string.Format ("SQL Function {0} requires a constant argument", sqlFunction.Code));
							}
							
							string constantValue = sqlFunction.B.Value as string;
							
							if ((constantValue != null) &&
								(constantValue.Contains (DbSqlStandard.CompareLikeEscape)))
							{
								//	TODO: make sure the user escaped only escapable characters here !
								
								this.Append (string.Concat (" ESCAPE '", DbSqlStandard.CompareLikeEscape, "'"));
							}
							break;
					}

					this.Append (')');
					break;

				case 0:
					switch (sqlFunction.Code)
					{
						case SqlFunctionCode.CompareFalse:				this.Append ("(0 = 1)");	break;
						case SqlFunctionCode.CompareTrue:				this.Append ("(1 = 1)");	break;

						default:
							throw new Exceptions.SyntaxException (this.fb.DbAccess, string.Format ("SQL Function {0} not supported", sqlFunction.Code));
					}
					break;

				case 1:
					switch (sqlFunction.Code)
					{
						case SqlFunctionCode.LogicNot:
							this.Append ("NOT ");
							this.Append (sqlFunction.A, onlyAcceptQualifiedNames);
							break;
						
						case SqlFunctionCode.CompareIsNull:
							this.Append (sqlFunction.A, onlyAcceptQualifiedNames);
							this.Append (" IS NULL");
							break;
						
						case SqlFunctionCode.CompareIsNotNull:
							this.Append (sqlFunction.A, onlyAcceptQualifiedNames);
							this.Append (" IS NOT NULL");
							break;
						
						case SqlFunctionCode.SetExists:
							this.Append (sqlFunction.A, onlyAcceptQualifiedNames);
							this.Append (" EXISTS");
							break;

						case SqlFunctionCode.SetNotExists:
							this.Append (sqlFunction.A, onlyAcceptQualifiedNames);
							this.Append (" NOT EXISTS");
							break;
						
						case SqlFunctionCode.Upper:
							this.Append ("UPPER(");
							this.Append (sqlFunction.A, onlyAcceptQualifiedNames);
							this.Append (")");
							break;
						
						default:
							throw new Exceptions.SyntaxException (this.fb.DbAccess, string.Format ("SQL Function {0} not supported", sqlFunction.Code));
					}
					break;

				case 3:
					if (sqlFunction.Code == SqlFunctionCode.Substring)
					{
						this.Append ("SUBSTRING(");
						this.Append (sqlFunction.A, onlyAcceptQualifiedNames);
						this.Append (" FROM ");
						this.Append (sqlFunction.B, onlyAcceptQualifiedNames);
						this.Append (" FOR ");
						this.Append (sqlFunction.C, onlyAcceptQualifiedNames);
						this.Append (")");
					}
					else
					{
						this.Append (sqlFunction.A, onlyAcceptQualifiedNames);

						switch (sqlFunction.Code)
						{
							case SqlFunctionCode.SetBetween:
								this.Append (" BETWEEN ");
								this.Append (sqlFunction.B, onlyAcceptQualifiedNames);
								this.Append (" AND ");
								break;

							case SqlFunctionCode.SetNotBetween:
								this.Append (" NOT BETWEEN ");
								this.Append (sqlFunction.B, onlyAcceptQualifiedNames);
								this.Append (" AND ");
								break;
							
							default:
								System.Diagnostics.Debug.Assert (false);
								break;
						}
						
						this.Append (sqlFunction.C, onlyAcceptQualifiedNames);
					}
					break;

				default:
					throw new Exceptions.SyntaxException (this.fb.DbAccess, string.Format ("SQL Function {0} not supported", sqlFunction.Code));
			}
		}

		private void Append(SqlJoin sqlJoin)
		{
			this.Append (" " + this.GetSql (sqlJoin.Code) + " ");
			this.Append (sqlJoin.Table);

			if (!this.AppendAlias (sqlJoin.Table))
			{
				var message = string.Format ("Unqualified table {0} in JOIN", sqlJoin.Table.AsName);

				throw new Exceptions.SyntaxException (this.fb.DbAccess, message);
			}

			if (sqlJoin.Code != SqlJoinCode.Cross)
			{
				this.Append (" ON ( ");
				this.Append (sqlJoin.Condition, true);
				this.Append (" ) ");
			}
		}


		private string GetSql(SqlJoinCode code)
		{
			switch (code)
			{
				case SqlJoinCode.Inner:
					return "INNER JOIN";
				
				case SqlJoinCode.OuterLeft:
					return "LEFT OUTER JOIN";
				
				case SqlJoinCode.OuterRight:
					return "RIGHT OUTER JOIN";
				
				case SqlJoinCode.OuterFull:
					return "FULL OUTER JOIN";
				
				case SqlJoinCode.Cross:
					return "CROSS JOIN";

				default:

					var message = string.Format ("SQL Join {0} not supported", code);

					throw new Exceptions.SyntaxException (this.fb.DbAccess, message);
			}
		}


		private void Append(SqlSelect sqlQuery)
		{
			this.tableAliases.Clear ();

			var simpleTables = sqlQuery.Tables;
			var joinTables = sqlQuery.Joins.Select (j => j.AsJoin.Table);
			var tables = simpleTables.Concat (joinTables);

			foreach (SqlField field in tables)
			{
				if (field.AsName == null || string.IsNullOrEmpty (field.Alias))
				{
					continue;
				}

				if (field.AsName != field.Alias)
				{
					this.tableAliases[field.AsName] = field.Alias;
				}
			}

			int	aggregateCount = 0;
			int	notAggregateCount = 0;

			bool onlyAcceptQualifiedNames = (sqlQuery.Tables.Count + sqlQuery.Joins.Count) > 1;

			this.Append ("SELECT ");
			bool isFirstField = true;

			if (sqlQuery.Predicate == SqlSelectPredicate.Distinct)
			{
				this.Append ("DISTINCT ");
			}
			
			foreach (SqlField field in sqlQuery.Fields)
			{
				if (isFirstField)
				{
					isFirstField = false;
				}
				else
				{
					this.Append (", ");
				}

				switch (field.FieldType)
				{
					case SqlFieldType.All:
						this.Append ("*");
						break;
					
					case SqlFieldType.Name:
						this.Append (field.AsName);
						this.AppendAlias (field);
						notAggregateCount++;
						break;
					
					case SqlFieldType.QualifiedName:
						this.Append (this.GetQualifiedName (field));
						this.AppendAlias (field);
						notAggregateCount++;
						break;
					
					case SqlFieldType.Aggregate:
						this.Append (field.AsAggregate);
						this.AppendAlias (field);
						aggregateCount++;
						break;
					
					default:
						throw new Exceptions.SyntaxException (this.fb.DbAccess, string.Format ("Unsupported field {0} in SELECT", field.AsName));
				}
			}

			if (isFirstField)
			{
				//	Aucun champ n'a été spécifié. On ne peut pas faire un SELECT vide.

				throw new Exceptions.SyntaxException (this.fb.DbAccess, string.Format ("No field specified in SELECT"));
			}

			this.Append (" FROM ");
			isFirstField = true;

			foreach (SqlField field in sqlQuery.Tables)
			{
				if (isFirstField)
				{
					isFirstField = false;
				}
				else
				{
					this.Append (", ");
				}

				switch (field.FieldType)
				{
					case SqlFieldType.Name:
						this.Append (field.AsName);
						this.AppendAlias (field);
						break;

					default:
						throw new Exceptions.SyntaxException (this.fb.DbAccess, string.Format ("Unsupported field {0} in SELECT FROM", field.AsName));
				}
			}

			if (isFirstField)
			{
				//	Aucune table n'a été spécifiée. On ne peut pas faire un SELECT sans tables.

				throw new Exceptions.SyntaxException (this.fb.DbAccess, string.Format ("No table specified in SELECT"));
			}

			if (sqlQuery.Tables.Count > 1 && sqlQuery.Joins.Any ())
			{
				var message = "Combination of implicit and explicit JOIN is forbidden";

				throw new Exceptions.SyntaxException (this.fb.DbAccess, message);
			}

			foreach (SqlField field in sqlQuery.Joins)
			{
				this.Append (field.AsJoin);
			}

			if ((aggregateCount > 0) && 
				(notAggregateCount > 0))
			{
				//	Ajoute une condition GROUP BY sur les champs non "aggregate" :
				
				this.Append (" GROUP BY ");
				isFirstField = true;

				foreach (SqlField field in sqlQuery.Fields)
				{
					if ((field.FieldType == SqlFieldType.All) ||
						(field.FieldType == SqlFieldType.Aggregate))
					{
						continue;
					}

					if (isFirstField)
					{
						isFirstField = false;
					}
					else
					{
						this.Append (", ");
					}

					switch (field.FieldType)
					{
						case SqlFieldType.Name:
							this.Append (field.AsName);
							this.AppendAlias (field);
							break;
						
						case SqlFieldType.QualifiedName:
							this.Append (this.GetQualifiedName (field));
							this.AppendAlias (field);
							break;

						default:
							throw new Exceptions.SyntaxException (this.fb.DbAccess, string.Format ("Unsupported field type {0} in GROUP BY clause", field.FieldType));
					}
				}
            }

			isFirstField = true;

			foreach (SqlField field in sqlQuery.Conditions)
			{
				if (isFirstField)
				{
					if (aggregateCount > 0 && notAggregateCount > 0)
					{
						this.Append (" HAVING ");
					}
					else
					{
						this.Append (" WHERE ");
					}

					isFirstField = false;
				}
				else
				{
					this.Append (" AND ");
				}

				if (field.FieldType != SqlFieldType.Function)
				{
					throw new Exceptions.SyntaxException (this.fb.DbAccess, string.Format ("Invalid field {0} in SELECT ... WHERE clause", field.AsName));
				}

				//	S'il y a plusieurs tables dans sqlQuery.Tables,  on va refuser les noms non
				//	qualifiés.
				
				this.Append (field.AsFunction, onlyAcceptQualifiedNames);
			}

			isFirstField = true;

			foreach (SqlField field in sqlQuery.OrderBy)
			{
				if (field.SortOrder == SqlSortOrder.None)
				{
					continue;
				}

				if (isFirstField)
				{
					isFirstField = false;
					this.Append (" ORDER BY ");
				}
				else
				{
					this.Append (", ");
				}

				this.Append (this.GetQualifiedName (field) ?? field.AsName);

				if (field.SortOrder == SqlSortOrder.Descending)
				{
					this.Append (" DESC");
				}
			}

			//	Traite encore les UNION s'il y a lieu :
			
			if (sqlQuery.SetQuery != null)
			{
				switch (sqlQuery.SetOp)
				{
					case SqlSelectSetOp.Union:
						this.Append (" UNION ");
						break;
					case SqlSelectSetOp.Except:
						this.Append (" EXCEPT ");
						break;
					case SqlSelectSetOp.Intersect:
						this.Append (" INTERSECT ");
						break;

					default:
						throw new Exceptions.SyntaxException (this.fb.DbAccess, string.Format ("Invalid union {0} of 2 SELECT", sqlQuery.SetOp));
				}

				if (sqlQuery.SetQuery.Predicate == SqlSelectPredicate.All)
				{
					this.Append ("ALL ");
				}

				this.Append (sqlQuery.SetQuery);
			}

			var skip = sqlQuery.Skip;
			var take = sqlQuery.Take;

			if (skip.HasValue)
			{
				// The one based inclusive index of the first row to return.
				int m = skip.Value + 1;

				// The one based inclusive index of the last row to return.
				int n = take.HasValue
					? n = m + take.Value - 1
					: int.MaxValue;

				this.Append (" ROWS ");
				this.Append (m.ToString (CultureInfo.InvariantCulture));
				this.Append (" TO ");
				this.Append (n.ToString (CultureInfo.InvariantCulture));
			}
			else if (take.HasValue)
			{
				this.Append (" ROWS ");
				this.Append (take.Value.ToString (CultureInfo.InvariantCulture));
			}
		}
		
		private bool AppendAlias(SqlField field)
		{
			//	Si un alias existe, ajoute celui-ci dans le buffer. Retourne false s'il n'y a pas
			//	de nom d'alias.
			
			string alias = field.Alias;

			if (string.IsNullOrEmpty (alias))
			{
				return false;
			}
			else
			{
				this.buffer.Append (' ');
				this.buffer.Append (DbSqlStandard.MakeDelimitedIdentifier (alias));
				return true;
			}
		}

		private string GetSqlType(SqlColumn column)
		{
			string basicType = null;
			string length;
			string encoding;

			//	Construit le nom du type SQL en fonction de la description de la
			//	colonne.

			switch (column.Type)
			{
				case DbRawType.Int16:
					basicType = "SMALLINT";
					break;
				case DbRawType.Int32:
					basicType = "INTEGER";
					break;
				case DbRawType.Int64:
					basicType = "BIGINT";
					break;
				case DbRawType.Date:
					basicType = "DATE";
					break;
				case DbRawType.Time:
					basicType = "TIME";
					break;
				case DbRawType.DateTime:
					basicType = "TIMESTAMP";
					break;
				case DbRawType.SmallDecimal:
					basicType = "DECIMAL(18,9)";
					break;
				case DbRawType.LargeDecimal:
					basicType = "DECIMAL(18,3)";
					break;

				case DbRawType.String:
					length    = column.Length.ToString (TypeConverter.InvariantFormatProvider);
					encoding  = column.Encoding == DbCharacterEncoding.Ascii ? "ASCII" : "UTF8";
					basicType = (column.IsFixedLength ? "CHAR(" : "VARCHAR(") + length + ") CHARACTER SET " + encoding;
					break;

				case DbRawType.ByteArray:
					basicType = "BLOB SUB_TYPE 0 SEGMENT SIZE 2048";
					break;

				//	Tous les types ne sont pas gérés ici, seuls ceux supportés en natif par
				//	Firebird sont listés ici. Pour une base plus complète que Firebird, il
				//	faudra par exemple ajouter un support pour Guid.

				default:
					break;
			}

			if (basicType == null)
			{
				throw new Exceptions.FormatException (string.Format ("Unsupported type {0} in column {1}", column.Type.ToString (), column.Name));
			}

			return basicType;
		}

		private string GetSqlColumnAttributes(SqlColumn column)
		{
			//	Construit les attributs de la colonne, tels qu'ils sont utilisés dans la
			//	définition d'une table (ce sont généralement des contraintes).
			
			System.Text.StringBuilder buffer = new System.Text.StringBuilder ();
			
			if (!column.IsNullable)
			{
				buffer.Append (" NOT NULL");
			}
			
			return buffer.ToString ();
		}

		private FbDbType GetFbType(DbRawType rawType)
		{
			//	Convertit un type brut en un type Firebird correspondant.
			
			switch (rawType)
			{
				case DbRawType.Boolean:			return FbDbType.SmallInt;
				case DbRawType.Int16:			return FbDbType.SmallInt;
				case DbRawType.Int32:			return FbDbType.Integer;
				case DbRawType.Int64:			return FbDbType.BigInt;
				case DbRawType.SmallDecimal:	return FbDbType.Decimal;
				case DbRawType.LargeDecimal:	return FbDbType.Decimal;
				case DbRawType.Time:			return FbDbType.Time;
				case DbRawType.Date:			return FbDbType.Date;
				case DbRawType.DateTime:		return FbDbType.TimeStamp;
				case DbRawType.String:			return FbDbType.VarChar;
				case DbRawType.ByteArray:		return FbDbType.Binary;
			}

			throw new Exceptions.SyntaxException (this.fb.DbAccess, string.Format ("Type {0} cannot be mapped to Firebird Type", rawType.ToString ()));
		}

		private readonly FirebirdAbstraction		fb;
		private readonly System.Text.StringBuilder	buffer;
		private readonly List<SqlField>				commandParams;
		private readonly Dictionary<string, string>	tableAliases;
		private bool								autoClear;
		private bool								expectMore;
		private FbCommand							commandCache;
		private DbCommandType						commandType;
		private int									commandCount;
	}
}
