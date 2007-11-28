//	Copyright � 2003-2007, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using FirebirdSql.Data.FirebirdClient;
using System.Collections.Generic;

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
			//	�tre utilis� par un appelant. C'est le cas lorsque l'on est en mode AutoClear.
			
			this.expectMore    = false;
			this.commandCache  = null;
			this.commandType   = DbCommandType.None;
			this.commandCount  = 0;
			this.buffer.Length = 0;
			this.commandParams.Clear ();
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
				this.Append (DbSqlStandard.ConcatNames ("PK_", table.Name));
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

			int indexNum = 0;

			foreach (SqlIndex index in table.Indexes)
			{
				indexNum++;
				this.commandCount++;
				
				string indexName = string.Concat (table.Name, "_IDX", indexNum.ToString (System.Globalization.CultureInfo.InvariantCulture));

				if (indexName.Length > 30)
				{
					indexName = indexName.Substring (indexName.Length-30);
					while (indexName[0] == '_')
					{
						indexName = indexName.Substring (1);
					}
				}
				
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

				this.Append (" INDEX ");
				this.Append (indexName);
				this.Append (" ON ");
				this.Append (table.Name);
				this.Append (" (");

				SqlColumn[] columns = index.Columns;

				for (int i = 0; i < columns.Length; i++)
				{
					if (i > 0)
					{
						this.Append (", ");
					}
					
					this.Append (columns[i].Name);
				}

				this.Append (");\n");
			}
		}

		public void RemoveTable(string tableName)
		{
			if (!this.ValidateName (tableName))
			{
				throw new Exceptions.SyntaxException (this.fb.DbAccess, string.Format ("Invalid table {0}", tableName));
			}
			
			this.PrepareCommand ();
			
			this.commandType = DbCommandType.Silent;
			this.commandCount++;
			
			this.Append ("DROP TABLE ");
			this.Append (tableName);
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
			}
		}

		public void UpdateTableColumns(string tableName, SqlColumn[] columns)
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

			throw new Exceptions.SyntaxException (this.fb.DbAccess, string.Format ("Cannot update table {0}. Not supported", tableName));
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
			
			this.Append ("INSERT INTO ");
			this.Append (tableName);
			this.Append ("(");
			
			bool isFirstField = true;
			
			foreach (SqlField field in fields)
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
			
			foreach (SqlField field in fields)
			{
				string data = null;
				
				switch (field.FieldType)
				{
					case SqlFieldType.Default:		data = "DEFAULT";						break;
					case SqlFieldType.Null:			data = "NULL";							break;
					case SqlFieldType.Constant:		data = this.MakeCommandParam (field);	break;
					
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

			System.Diagnostics.Debug.Assert (isFirstField == false);
			
			this.Append (");\n");
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
				
				//	Construit l'objet commande en se basant sur les param�tres d�finis pour
				//	celle-ci.
				
				foreach (SqlField field in this.commandParams)
				{
					FbDbType    fbType  = this.GetFbType (field.RawType);
					string      fbName  = string.Format (TypeConverter.InvariantFormatProvider, "@PARAM_{0}", fieldIndex++);
					FbParameter fbParam = new FbParameter (fbName, fbType);
					
					//	Pour l'instant, on ne supporte que des valeurs constantes dans la d�finition des
					//	param�tres de la commande (on aurait pu imaginer accepter ici les r�sultats d'une
					//	sous-requ�te ou encore d'une proc�dure SQL) :
					
					System.Diagnostics.Debug.Assert (field.FieldType == SqlFieldType.Constant);
					
					fbParam.Value = field.AsConstant;
					fbParam.SourceColumn = field.Alias;
					
					this.commandCache.Parameters.Add (fbParam);
				}
				
				//	Pour l'instant, la commande est toujours de type texte et construite par les
				//	diverses m�thodes publiques.
				
				this.commandCache.CommandType = System.Data.CommandType.Text;
				this.commandCache.CommandText = this.buffer.ToString ();
			}
		}
		
		private string MakeCommandParam(SqlField field)
		{
			//	Ajoute un champ SQL comme param�tre de la commande. C'est la m�thode UpdateCommand qui
			//	va faire le lien entre le num du param�tre (@PARAM_n) et sa valeur, telle que d�finie
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
					this.Append (field.AsQualifiedName);
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
				
				default:
					throw new Exceptions.SyntaxException (this.fb.DbAccess, string.Format ("Field {0} not supported", field.FieldType));
			}
		}
		
		private void Append(SqlAggregate sqlAggregate)
		{
			switch (sqlAggregate.Function)
			{
				case SqlAggregateFunction.Count:
					this.Append ("COUNT(");
					break;
				
				case SqlAggregateFunction.Max:
					this.Append ("MAX(");
					break;
				
				case SqlAggregateFunction.Min:
					this.Append ("MIN(");
					break;
				
				case SqlAggregateFunction.Sum:
					this.Append ("SUM(");
					break;
				
				case SqlAggregateFunction.Average:
					this.Append ("AVG(");
					break;
				
				default:
					throw new Exceptions.SyntaxException (this.fb.DbAccess, string.Format ("Aggregate {0} not supported", sqlAggregate.Function));
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
						case SqlFunctionCode.CompareNotLike:			this.Append (" NOT LIKE ");	break;
						case SqlFunctionCode.SetIn:						this.Append (" IN ");		break;
						case SqlFunctionCode.SetNotIn:					this.Append (" NOT IN ");	break;
						case SqlFunctionCode.LogicAnd:					this.Append (" AND ");		break;
						case SqlFunctionCode.LogicOr:					this.Append (" OR ");		break;
						
						default:
							throw new Exceptions.SyntaxException (this.fb.DbAccess, string.Format ("SQL Function {0} not supported", sqlFunction.Code));
					}
					
					this.Append (sqlFunction.B, onlyAcceptQualifiedNames);
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

		private void Append(SqlJoin sqlJoin, Collections.SqlFieldList sqlTables, int row)
		{
			System.Diagnostics.Debug.Assert (sqlJoin != null);
			System.Diagnostics.Debug.Assert (row > 0);

			//	Convertit la jointure en SQL. La liste des tables est n�cessaire pour
			//	retrouver le nom de la table et son alias.

			if (row == 1)
			{
				this.Append (sqlTables[0]);

				if (!this.AppendAlias (sqlTables[0]))
				{
					throw new Exceptions.SyntaxException (this.fb.DbAccess, string.Format ("Unqualified table {0} in JOIN", sqlTables[0].AsName));
				}
			}

			switch (sqlJoin.Code)
			{
				case SqlJoinCode.Inner:			this.Append (" INNER JOIN ");		break;
				case SqlJoinCode.OuterLeft:		this.Append (" LEFT OUTER JOIN ");	break;
				case SqlJoinCode.OuterRight:	this.Append (" RIGHT OUTER JOIN ");	break;

				default:
					throw new Exceptions.SyntaxException (this.fb.DbAccess, string.Format ("SQL Join {0} not supported", sqlJoin.Code));
			}
			
			this.Append (sqlTables[row]);

			if (!this.AppendAlias (sqlTables[row]))
			{
				throw new Exceptions.SyntaxException (this.fb.DbAccess, string.Format ("Unqualified table {0} in JOIN", sqlTables[row].AsName));
			}

			this.Append (" ON ");
			this.Append (sqlJoin.A.AsQualifiedName);
			this.Append (" = ");
			this.Append (sqlJoin.B.AsQualifiedName);
		}

		private void Append(SqlSelect sqlQuery)
		{
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
						this.Append (field.AsQualifiedName);
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
				//	Aucun champ n'a �t� sp�cifi�. On ne peut pas faire un SELECT vide.

				throw new Exceptions.SyntaxException (this.fb.DbAccess, string.Format ("No field specified in SELECT"));
			}

			this.Append (" FROM ");
			isFirstField = true;

			if (sqlQuery.Joins.Count > 0)
			{
				//	Cas particulier pour les jointures :

				int row = 1;

				foreach (SqlField field in sqlQuery.Joins)
				{
					this.Append (field.AsJoin, sqlQuery.Tables, row++);
					isFirstField = false;
				}
			}
			else
			{
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
			}

			if (isFirstField)
			{
				//	Aucune table n'a �t� sp�cifi�e. On ne peut pas faire un SELECT sans tables.

				throw new Exceptions.SyntaxException (this.fb.DbAccess, string.Format ("No table specified in SELECT"));
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
							this.Append (field.AsQualifiedName);
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
					if ((aggregateCount > 0) && 
						(notAggregateCount > 0))
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
				//	qualifi�s.
				
				this.Append (field.AsFunction, onlyAcceptQualifiedNames);
			}

			isFirstField = true;

			foreach (SqlField field in sqlQuery.Fields)
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

				//	TODO:	si un alias existe on devrait l'utiliser � la place du nom
				//	TODO:	sinon faut-il utiliser le nom qualifi� de pr�f�rence ?
				this.Append (field.AsQualifiedName);

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
					case SqlSelectSetOp.Union:		this.Append (" UNION ");		break;
					case SqlSelectSetOp.Except:		this.Append (" EXCEPT ");		break;
					case SqlSelectSetOp.Intersect:	this.Append (" INTERSECT ");	break;
					
					default:
						throw new Exceptions.SyntaxException (this.fb.DbAccess, string.Format ("Invalid union {0} of 2 SELECT", sqlQuery.SetOp));
				}

				if (sqlQuery.SetQuery.Predicate == SqlSelectPredicate.All)
				{
					this.Append ( "ALL ");
				}

				this.Append (sqlQuery.SetQuery);
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

		private void AppendAliasOrName(SqlField field)
		{
			//	Utilise le nom d'alias de pr�f�rence, sinon le nom qualifi�, sinon le nom
			//	non qualifi�.

			string alias = field.Alias;
			
			if (string.IsNullOrEmpty (alias) == false)
			{
				this.buffer.Append (alias);
			}
			else if (field.FieldType == SqlFieldType.QualifiedName)
			{
				this.buffer.Append (field.AsQualifiedName);
			}
			else
			{
				this.buffer.Append (field.AsName);
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
					basicType = "BLOB SUB_TYPE 0 SEGMENT SIZE 1024";
					break;

				//	Tous les types ne sont pas g�r�s ici, seuls ceux support�s en natif par
				//	Firebird sont list�s ici. Pour une base plus compl�te que Firebird, il
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
			//	Construit les attributs de la colonne, tels qu'ils sont utilis�s dans la
			//	d�finition d'une table (ce sont g�n�ralement des contraintes).
			
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


		private FirebirdAbstraction				fb;
		private bool							autoClear;
		private bool							expectMore;
		private FbCommand						commandCache;
		private System.Text.StringBuilder		buffer;
		private List<SqlField>					commandParams;
		private DbCommandType					commandType;
		private int								commandCount;
	}
}
