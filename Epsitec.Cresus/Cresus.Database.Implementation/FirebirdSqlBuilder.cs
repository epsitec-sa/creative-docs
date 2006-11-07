//	Copyright © 2003-2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
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
			//	être utilisé par un appelant. C'est le cas lorsque l'on est en mode AutoClear.
			
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
			if (!table.Validate (this))
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
				if (!column.Validate (this))
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
				if (!column.Validate (this))
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
				if (!column.Validate (this))
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

		public void InsertData(string tableName, Collections.SqlFields fields)
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
				
				switch (field.Type)
				{
					case SqlFieldType.Default:		data = "DEFAULT";						break;
					case SqlFieldType.Null:			data = "NULL";							break;
					case SqlFieldType.Constant:		data = this.MakeCommandParam (field);	break;
					
					default:
						throw new Exceptions.SyntaxException (this.fb.DbAccess, string.Format ("Invalid field type {0} for field {1} in table {2}", field.Type.ToString (), field.Alias, tableName));
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

		public void UpdateData(string tableName, Collections.SqlFields fields, Collections.SqlFields conditions)
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
					
					if (field.Type != SqlFieldType.Function)
					{
						throw new Exceptions.SyntaxException (this.fb.DbAccess, string.Format ("Invalid field {0} in UPDATE ... WHERE", field.AsName));
					}
					
					this.Append (field.AsFunction);
				}
			}
			
			this.Append (";\n");
		}

		public void RemoveData(string tableName, Collections.SqlFields conditions)
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

					if (field.Type != SqlFieldType.Function)
					{
						throw new Exceptions.SyntaxException (this.fb.DbAccess, string.Format ("Invalid field {0} in UPDATE ... WHERE", field.AsName));
					}

					this.Append (field.AsFunction);
				}
			}
			
			this.Append (";\n");
		}

		
		public void ExecuteProcedure(string procedureName, Collections.SqlFields fields)
		{
			//	TODO:  Add FirebirdSqlBuilder.ExecuteProcedure implementation
			
			throw new System.NotImplementedException ();
		}

		
		public void GetSqlParameters(System.Data.IDbCommand command, Collections.SqlFields fields)
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

		public void SetSqlParameters(System.Data.IDbCommand command, Collections.SqlFields fields)
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
				
				//	Construit l'objet commande en se basant sur les paramètres définis pour
				//	celle-ci.
				
				foreach (SqlField field in this.commandParams)
				{
					FbDbType    fbType  = this.GetFbType (field.RawType);
					string      fbName  = string.Format (TypeConverter.InvariantFormatProvider, "@PARAM_{0}", fieldIndex++);
					FbParameter fbParam = new FbParameter (fbName, fbType);
					
					//	Pour l'instant, on ne supporte que des valeurs constantes dans la définition des
					//	paramètres de la commande (on aurait pu imaginer accepter ici les résultats d'une
					//	sous-requête ou encore d'une procédure SQL) :
					
					System.Diagnostics.Debug.Assert (field.Type == SqlFieldType.Constant);
					
					fbParam.Value = field.AsConstant;
					fbParam.SourceColumn = field.Alias;
					
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

			switch (field.Type)
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
					throw new Exceptions.SyntaxException (this.fb.DbAccess, string.Format ("Field {0} not supported", field.Type));
			}
		}
		
		private void Append(SqlAggregate sqlAggregate)
		{
			switch (sqlAggregate.Type)
			{
				case SqlAggregateType.Count:
					this.Append ("COUNT(");
					break;
				
				case SqlAggregateType.Max:
					this.Append ("MAX(");
					break;
				
				case SqlAggregateType.Min:
					this.Append ("MIN(");
					break;
				
				case SqlAggregateType.Sum:
					this.Append ("SUM(");
					break;
				
				case SqlAggregateType.Average:
					this.Append ("AVG(");
					break;
				
				default:
					throw new Exceptions.SyntaxException (this.fb.DbAccess, string.Format ("Aggregate {0} not supported", sqlAggregate.Type));
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
					
					switch (sqlFunction.Type)
					{
						case SqlFunctionType.MathAdd:					this.Append (" + ");		break;
						case SqlFunctionType.MathSubstract:				this.Append (" - ");		break;
						case SqlFunctionType.MathMultiply:				this.Append (" * ");		break;
						case SqlFunctionType.MathDivide:				this.Append (" / ");		break;
						case SqlFunctionType.CompareEqual:				this.Append (" = ");		break;
						case SqlFunctionType.CompareNotEqual:			this.Append (" <> ");		break;
						case SqlFunctionType.CompareLessThan:			this.Append (" < ");		break;
						case SqlFunctionType.CompareLessThanOrEqual:	this.Append (" <= ");		break;
						case SqlFunctionType.CompareGreaterThan:		this.Append (" > ");		break;
						case SqlFunctionType.CompareGreaterThanOrEqual:	this.Append (" >= ");		break;
						case SqlFunctionType.CompareLike:				this.Append (" LIKE ");		break;
						case SqlFunctionType.CompareNotLike:			this.Append (" NOT LIKE ");	break;
						case SqlFunctionType.SetIn:						this.Append (" IN ");		break;
						case SqlFunctionType.SetNotIn:					this.Append (" NOT IN ");	break;
						case SqlFunctionType.LogicAnd:					this.Append (" AND ");		break;
						case SqlFunctionType.LogicOr:					this.Append (" OR ");		break;
						
						default:
							throw new Exceptions.SyntaxException (this.fb.DbAccess, string.Format ("SQL Function {0} not supported", sqlFunction.Type));
					}
					
					this.Append (sqlFunction.B, onlyAcceptQualifiedNames);
					this.Append (')');
					break;

				case 0:
					switch (sqlFunction.Type)
					{
						case SqlFunctionType.CompareFalse:				this.Append ("(0 = 1)");	break;
						case SqlFunctionType.CompareTrue:				this.Append ("(1 = 1)");	break;

						default:
							throw new Exceptions.SyntaxException (this.fb.DbAccess, string.Format ("SQL Function {0} not supported", sqlFunction.Type));
					}
					break;

				case 1:
					switch (sqlFunction.Type)
					{
						case SqlFunctionType.LogicNot:
							this.Append ("NOT ");
							this.Append (sqlFunction.A, onlyAcceptQualifiedNames);
							break;
						
						case SqlFunctionType.CompareIsNull:
							this.Append (sqlFunction.A, onlyAcceptQualifiedNames);
							this.Append (" IS NULL");
							break;
						
						case SqlFunctionType.CompareIsNotNull:
							this.Append (sqlFunction.A, onlyAcceptQualifiedNames);
							this.Append (" IS NOT NULL");
							break;
						
						case SqlFunctionType.SetExists:
							this.Append (sqlFunction.A, onlyAcceptQualifiedNames);
							this.Append (" EXISTS");
							break;

						case SqlFunctionType.SetNotExists:
							this.Append (sqlFunction.A, onlyAcceptQualifiedNames);
							this.Append (" NOT EXISTS");
							break;
						
						case SqlFunctionType.Upper:
							this.Append ("UPPER(");
							this.Append (sqlFunction.A, onlyAcceptQualifiedNames);
							this.Append (")");
							break;
						
						default:
							throw new Exceptions.SyntaxException (this.fb.DbAccess, string.Format ("SQL Function {0} not supported", sqlFunction.Type));
					}
					break;

				case 3:
					if (sqlFunction.Type == SqlFunctionType.Substring)
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

						switch (sqlFunction.Type)
						{
							case SqlFunctionType.SetBetween:
								this.Append (" BETWEEN ");
								this.Append (sqlFunction.B, onlyAcceptQualifiedNames);
								this.Append (" AND ");
								break;

							case SqlFunctionType.SetNotBetween:
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
					throw new Exceptions.SyntaxException (this.fb.DbAccess, string.Format ("SQL Function {0} not supported", sqlFunction.Type));
			}
		}

		private void Append(SqlJoin sqlJoin, Collections.SqlFields sqlTables, int row)
		{
			System.Diagnostics.Debug.Assert (sqlJoin != null);
			System.Diagnostics.Debug.Assert (row > 0);

			//	Convertit la jointure en SQL. La liste des tables est nécessaire pour
			//	retrouver le nom de la table et son alias.

			if (row == 1)
			{
				this.Append (sqlTables[0]);

				if (!this.AppendAlias (sqlTables[0]))
				{
					throw new Exceptions.SyntaxException (this.fb.DbAccess, string.Format ("Unqualified table {0} in JOIN", sqlTables[0].AsName));
				}
			}

			switch (sqlJoin.Type)
			{
				case SqlJoinType.Inner:			this.Append (" INNER JOIN ");		break;
				case SqlJoinType.OuterLeft:		this.Append (" LEFT OUTER JOIN ");	break;
				case SqlJoinType.OuterRight:	this.Append (" RIGHT OUTER JOIN ");	break;

				default:
					throw new Exceptions.SyntaxException (this.fb.DbAccess, string.Format ("SQL Join {0} not supported", sqlJoin.Type));
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

				switch (field.Type)
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
				//	Aucun champ n'a été spécifié. On ne peut pas faire un SELECT vide.

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

					switch (field.Type)
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
				//	Aucune table n'a été spécifiée. On ne peut pas faire un SELECT sans tables.

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
					if ((field.Type == SqlFieldType.All) ||
						(field.Type == SqlFieldType.Aggregate))
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

					switch (field.Type)
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
							throw new Exceptions.SyntaxException (this.fb.DbAccess, string.Format ("Unsupported field type {0} in GROUP BY clause", field.Type));
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

				if (field.Type != SqlFieldType.Function)
				{
					throw new Exceptions.SyntaxException (this.fb.DbAccess, string.Format ("Invalid field {0} in SELECT ... WHERE clause", field.AsName));
				}

				//	S'il y a plusieurs tables dans sqlQuery.Tables,  on va refuser les noms non
				//	qualifiés.
				
				this.Append (field.AsFunction, onlyAcceptQualifiedNames);
			}

			isFirstField = true;

			foreach (SqlField field in sqlQuery.Fields)
			{
				if (field.Order == SqlFieldOrder.None)
				{
					continue;
				}

				if (isFirstField)
				{
					this.Append (" ORDER BY ");
				}

				//	TODO:	si un alias existe on devrait l'utiliser à la place du nom
				//	TODO:	sinon faut-il utiliser le nom qualifié de préférence ?
				this.Append (field.AsName);

				if (field.Order == SqlFieldOrder.Inverse)
				{
					this.Append (" DESC");
				}

				if (isFirstField)
				{
					isFirstField = false;
				}
				else
				{
					this.Append (", ");
				}
			}

			//	Traite encore les UNION s'il y a lieu :
			
			if (sqlQuery.SelectSetQuery != null)
			{
				switch (sqlQuery.SelectSetOp)
				{
					case SqlSelectSetOp.Union:		this.Append (" UNION ");		break;
					case SqlSelectSetOp.Except:		this.Append (" EXCEPT ");		break;
					case SqlSelectSetOp.Intersect:	this.Append (" INTERSECT ");	break;
					
					default:
						throw new Exceptions.SyntaxException (this.fb.DbAccess, string.Format ("Invalid union {0} of 2 SELECT", sqlQuery.SelectSetOp));
				}

				if (sqlQuery.SelectSetQuery.Predicate == SqlSelectPredicate.All)
				{
					this.Append ( "ALL ");
				}

				this.Append (sqlQuery.SelectSetQuery);
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
			//	Utilise le nom d'alias de préférence, sinon le nom qualifié, sinon le nom
			//	non qualifié.

			string alias = field.Alias;
			
			if (string.IsNullOrEmpty (alias) == false)
			{
				this.buffer.Append (alias);
			}
			else if (field.Type == SqlFieldType.QualifiedName)
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
					basicType = (column.IsFixedLength ? "CHAR(" : "VARCHAR(") + length + ") CHARACTER SET UNICODE_FSS";
					break;

				case DbRawType.ByteArray:
					basicType = "BLOB SUB_TYPE 0 SEGMENT SIZE 1024";
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
			
			if (column.IsUnique)
			{
				//	TODO: add a UNIQUE constraint; does this exist ?
				
				System.Diagnostics.Debug.WriteLine (string.Format ("Column {0} should be unique", column.Name));
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
