//	Copyright © 2003, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Statut : en chantier
//	DD 27/11/2003	Ajouté la construction de la chaîne pour SqlFunction / SqlField en utilisant les paramètres

using FirebirdSql.Data.Firebird;

namespace Epsitec.Cresus.Database.Implementation
{
	using Epsitec.Cresus.Database;
	
	/// <summary>
	/// Implémentation de ISqlBuilder pour Firebird.
	/// </summary>
	public class FirebirdSqlBuilder : ISqlBuilder, System.IDisposable
	{
		public FirebirdSqlBuilder(FirebirdAbstraction fb)
		{
			this.fb = fb;
			this.buffer = new System.Text.StringBuilder ();
			this.command_params = new System.Collections.ArrayList ();
		}
		
		
		protected void UpdateCommand()
		{
			if (this.command_cache == null)
			{
				this.command_cache = this.fb.NewDbCommand () as FbCommand;
				
				int field_i = 0;
				
				//	Construit l'objet commande en se basant sur les paramètres définis pour
				//	celle-ci.
				
				foreach (SqlField field in this.command_params)
				{
					FbDbType    fb_type  = this.GetFbType (field.RawType);
					string      fb_name  = string.Format (TypeConverter.InvariantFormatProvider, "@PARAM_{0}", field_i++);
					FbParameter fb_param = new FbParameter (fb_name, fb_type);
					
					//	Pour l'instant, on ne supporte que des valeurs constantes dans la définition des
					//	paramètres de la commande (on aurait pu imaginer accepter ici les résultats d'une
					//	sous-requête ou encore d'une procédure SQL) :
					
					System.Diagnostics.Debug.Assert (field.Type == SqlFieldType.Constant);
					
					fb_param.Value = field.AsConstant;
					fb_param.SourceColumn = field.Alias;
					
					this.command_cache.Parameters.Add (fb_param);
				}
				
				//	Pour l'instant, la commande est toujours de type texte et construite par les
				//	diverses méthodes publiques.
				
				this.command_cache.CommandType = System.Data.CommandType.Text;
				this.command_cache.CommandText = this.buffer.ToString ();
			}
		}
		
		
		protected string GetSqlType(SqlColumn column)
		{
			string basic_type = null;
			string length;
			
			//	Construit le nom du type SQL en fonction de la description de la
			//	colonne.
			
			switch (column.Type)
			{
				case DbRawType.Int16:			basic_type = "SMALLINT";		break;
				case DbRawType.Int32:			basic_type = "INTEGER";			break;
				case DbRawType.Int64:			basic_type = "BIGINT";			break;
				case DbRawType.Date:			basic_type = "DATE";			break;
				case DbRawType.Time:			basic_type = "TIME";			break;
				case DbRawType.DateTime:		basic_type = "TIMESTAMP";		break;
				case DbRawType.SmallDecimal:	basic_type = "DECIMAL(18,9)";	break;
				case DbRawType.LargeDecimal:	basic_type = "DECIMAL(18,3)";	break;
				
				case DbRawType.String:
					length     = column.Length.ToString (TypeConverter.InvariantFormatProvider);
					basic_type = (column.IsFixedLength ? "CHAR(" : "VARCHAR(") + length + ") CHARACTER SET UNICODE_FSS";
					break;
				
				case DbRawType.ByteArray:
					length     = column.Length.ToString (TypeConverter.InvariantFormatProvider);
					basic_type = "BLOB SUB_TYPE 0 SEGMENT SIZE " + length;
					break;
				
				//	Tous les types ne sont pas gérés ici, seuls ceux supportés en natif par
				//	Firebird sont listés ici. Pour une base plus complète que Firebird, il
				//	faudra par exemple ajouter un support pour Guid.
				
				default:
					break;
			}
			
			if (basic_type == null)
			{
				throw new DbFormatException (string.Format ("Unsupported type {0} in column {1}.", column.Type.ToString (), column.Name));
			}
			
			return basic_type;
		}
		
		protected string AddFieldAsParam(SqlField field)
		{
			//	Ajoute un champ SQL comme paramètre de la commande. C'est la méthode UpdateCommand qui
			//	va faire le lien entre le num du paramètre (@PARAM_n) et sa valeur, telle que définie
			//	dans le champ SqlField.
			
			string name = string.Format (TypeConverter.InvariantFormatProvider, "@PARAM_{0}", this.command_params.Count);
			this.command_params.Add (field);
			return name;
		}

		protected void AppendField(SqlField field)
		{
			//	Ajoute au buffer un champ, quel que soit son type

			switch(field.Type)
			{
				case SqlFieldType.Unsupported:
					this.buffer.Append ("<unsupported>");
					return;
				case SqlFieldType.Null:
					this.buffer.Append ("NULL");
					return;
				case SqlFieldType.All:
					this.buffer.Append ("*");
					return;
				case SqlFieldType.Default:
					this.buffer.Append ("<default>");
					return;
				case SqlFieldType.Constant:
				case SqlFieldType.ParameterOut:
				case SqlFieldType.ParameterInOut:
				case SqlFieldType.ParameterResult:
					this.buffer.Append (this.AddFieldAsParam (field));
					return;
				case SqlFieldType.Name:
					this.buffer.Append (field.AsName);
					return;
				case SqlFieldType.QualifiedName:
					this.buffer.Append (field.AsQualifiedName);
					return;
				case SqlFieldType.Aggregate:
					this.AppendAggregate (field.AsAggregate);
					return;
				case SqlFieldType.Variable:
					this.buffer.Append (field.AsVariable.ToString());
					return;
				case SqlFieldType.Function:
					this.AppendFunction (field.AsFunction);
					return;
				case SqlFieldType.Procedure:
					this.buffer.Append (field.AsProcedure);
					return;
				case SqlFieldType.SubQuery:
					this.buffer.Append (field.AsSubQuery.ToString());
					return;
				default:
					this.buffer.Append ("<unsupported>");
					return;
			}
		}
		
		protected void AppendAggregate(SqlAggregate sql_aggregate)
		{
			switch (sql_aggregate.Type)
			{
				case SqlAggregateType.Count:
					this.buffer.Append ("COUNT(");
					this.AppendField (sql_aggregate.Field);
					this.buffer.Append (")");
					return;
			}
			
			throw new System.NotImplementedException (string.Format ("Aggregate {0} not implemented.", sql_aggregate.Type));
		}

		protected void AppendFunction(SqlFunction sql_function)
		{
			System.Diagnostics.Debug.Assert (sql_function != null);

			//	Converti la fonction en chaîne de caractère SQL
			switch (sql_function.ArgumentCount)
			{
				case 2:
					if ( sql_function.Type == SqlFunctionType.JoinInner )
					{
						this.buffer.Append (sql_function.A.AsName);
						this.buffer.Append (" INNER JOIN ");
						this.buffer.Append (sql_function.B.AsName);
						this.buffer.Append (" ON ");
						this.buffer.Append (sql_function.A.AsQualifiedName);
						this.buffer.Append (" = ");
						this.buffer.Append (sql_function.B.AsQualifiedName);
						return;
					}

					AppendField (sql_function.A);
					switch (sql_function.Type)
					{
						case SqlFunctionType.MathAdd:
							this.buffer.Append (" + ");	break;
						case SqlFunctionType.MathSubstract:
							this.buffer.Append (" - "); break;
						case SqlFunctionType.MathMultiply:
							this.buffer.Append (" * "); break;
						case SqlFunctionType.MathDivide:
							this.buffer.Append (" / "); break;
						case SqlFunctionType.CompareEqual:
							this.buffer.Append (" = "); break;
						case SqlFunctionType.CompareNotEqual:
							this.buffer.Append (" <> "); break;
						case SqlFunctionType.CompareLessThan:
							this.buffer.Append (" < "); break;
						case SqlFunctionType.CompareLessThanOrEqual:
							this.buffer.Append (" <= "); break;
						case SqlFunctionType.CompareGreaterThan:
							this.buffer.Append (" > "); break;
						case SqlFunctionType.CompareGreaterThanOrEqual:
							this.buffer.Append (" >= "); break;
						case SqlFunctionType.CompareLike:
							this.buffer.Append (" LIKE "); break;
						case SqlFunctionType.CompareNotLike:
							this.buffer.Append (" NOT LIKE "); break;				
						case SqlFunctionType.SetIn:
							this.buffer.Append (" IN "); break;
						case SqlFunctionType.SetNotIn:
							this.buffer.Append (" NOT IN "); break;
						case SqlFunctionType.LogicAnd:
							this.buffer.Append (" AND "); break;
						case SqlFunctionType.LogicOr:
							this.buffer.Append (" OR "); break;
						default:
							System.Diagnostics.Debug.Assert (false); break;
					}
					AppendField (sql_function.B);
					return;
				
				case 1:
					switch (sql_function.Type)
					{
						case SqlFunctionType.LogicNot:
							this.buffer.Append ("NOT ");
							AppendField (sql_function.A);
							return;
						case SqlFunctionType.CompareIsNull:
							AppendField (sql_function.A);
							this.buffer.Append (" IS NULL");
							return;
						case SqlFunctionType.CompareIsNotNull:
							AppendField (sql_function.A);
							this.buffer.Append (" IS NOT NULL");
							return;
						case SqlFunctionType.SetExists:
							AppendField (sql_function.A);
							this.buffer.Append (" EXISTS");
							return;

						case SqlFunctionType.SetNotExists:
							AppendField (sql_function.A);
							this.buffer.Append (" NOT EXISTS");
							return;
						case SqlFunctionType.Upper:
							this.buffer.Append ("UPPER(");
							AppendField (sql_function.A);
							this.buffer.Append (")");
							return;
						default:
							System.Diagnostics.Debug.Assert (false);
							return;
					}

				case 3:
					if (sql_function.Type == SqlFunctionType.Substring)
					{
						this.buffer.Append ("SUBSTRING(");
						this.AppendField (sql_function.A);
						this.buffer.Append (" FROM ");
						this.AppendField (sql_function.B);
						this.buffer.Append (" FOR ");
						this.AppendField (sql_function.C);
						this.buffer.Append (")");
						return;
					}

					AppendField (sql_function.A);
					switch (sql_function.Type)
					{
						case SqlFunctionType.SetBetween:
							this.buffer.Append (" BETWEEN ");
							this.AppendField (sql_function.B);
							this.buffer.Append (" AND ");
							break;

						case SqlFunctionType.SetNotBetween:
							this.buffer.Append (" NOT BETWEEN ");
							this.AppendField (sql_function.B);
							this.buffer.Append (" AND ");
							break;
						default:
							System.Diagnostics.Debug.Assert (false);
							break;
					}
					AppendField (sql_function.C);
					return;

				default:
					System.Diagnostics.Debug.Assert (false);
					return;
			}			
		}
		
		protected string GetSqlColumnAttributes(SqlColumn column)
		{
			//	Construit les attributs de la colonne, tels qu'ils sont utilisés dans la
			//	définition d'une table (se sont généralement des contraintes).
			
			System.Text.StringBuilder buffer = new System.Text.StringBuilder ();
			
			if (!column.IsNullAllowed)
			{
				buffer.Append (" NOT NULL");
			}
			
			if (column.IsUnique)
			{
				System.Diagnostics.Debug.WriteLine (string.Format ("Column {0} should be unique", column.Name));
			}
			
			return buffer.ToString ();
		}
		
		
		protected FbDbType GetFbType(DbRawType raw_type)
		{
			//	Convertit un type brut en un type Firebird correspondant.
			
			switch (raw_type)
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
				case DbRawType.ByteArray:		return FbDbType.LongVarBinary;
			}
			
			this.ThrowError (string.Format (TypeConverter.InvariantFormatProvider, "Type {0} cannot be mapped to Firebird Type.", raw_type.ToString ()));
			
			//	code jamais atteint:
			
			throw null;
		}
		
		protected void PrepareCommand()
		{
			if (this.auto_clear)
			{
				this.Clear ();
			}
			
			if (this.command_type != DbCommandType.None)
			{
				throw new DbException (this.fb.DbAccess, "Previous command not cleared");
			}
		}
		
		
		#region ISqlBuilder Members
		public bool								AutoClear
		{
			get { return this.auto_clear; }
			set { this.auto_clear = value; }
		}
		
		public DbCommandType					CommandType
		{
			get { return this.command_type; }
		}
		
		public int								CommandCount
		{
			get { return this.command_count; }
		}

		public System.Data.IDbCommand			Command
		{
			get
			{
				this.UpdateCommand ();
				return this.command_cache;
			}
		}		
		
		public System.Data.IDbCommand CreateCommand(System.Data.IDbTransaction transaction)
		{
			this.UpdateCommand ();
			this.command_cache.Transaction = transaction as FirebirdSql.Data.Firebird.FbTransaction;
			return this.command_cache;
		}
		
		public System.Data.IDbCommand CreateCommand(System.Data.IDbTransaction transaction, string text)
		{
			FbCommand command = this.fb.NewDbCommand () as FbCommand;
			command.CommandText = text;
			command.CommandType = System.Data.CommandType.Text;
			command.Transaction = transaction as FirebirdSql.Data.Firebird.FbTransaction;
			return command;
		}
				
		public void Clear()
		{
			//	On n'a pas le droit de faire un Dispose de l'objet 'commande', car il peut encore
			//	être utilisé par un appelant. C'est le cas lorsque l'on est en mode AutoClear.
			
			this.command_cache = null;
			this.command_type  = DbCommandType.None;
			this.command_count = 0;
			this.buffer = new System.Text.StringBuilder ();
			this.command_params.Clear ();
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

		
		public void ThrowError(string message)
		{
			throw new DbSyntaxException (this.fb.DbAccess, message);
		}
		
		
		public void InsertTable(SqlTable table)
		{
			if (!table.Validate (this))
			{
				this.ThrowError (string.Format ("Invalid table {0}.", table.Name));
			}
			
			this.PrepareCommand ();
			this.command_type = DbCommandType.Silent;
			this.command_count++;
			
			this.buffer.Append ("CREATE TABLE ");
			this.buffer.Append (table.Name);
			this.buffer.Append ("(");
			
			bool first_field = true;
			
			foreach (SqlColumn column in table.Columns)
			{
				if (!column.Validate (this))
				{
					this.ThrowError (string.Format ("Invalid column {0} in table {1}.", column.Name, table.Name));
				}
				
				if (first_field)
				{
					first_field = false;
				}
				else
				{
					this.buffer.Append (", ");
				}
				
				this.buffer.Append (column.Name);
				this.buffer.Append (" ");
				this.buffer.Append (this.GetSqlType (column));
				this.buffer.Append (this.GetSqlColumnAttributes (column));
			}
			
			this.buffer.Append (");\n");
			
			if (table.HasPrimaryKeys)
			{
				this.command_count++;
				
				this.buffer.Append ("ALTER TABLE ");
				this.buffer.Append (table.Name);
				this.buffer.Append (" ADD CONSTRAINT ");
				this.buffer.Append (DbSqlStandard.ConcatNames ("PK_", table.Name));
				this.buffer.Append (" PRIMARY KEY ");
				this.buffer.Append ("(");
				
				first_field = true;
				
				foreach (SqlColumn column in table.PrimaryKey)
				{
					if (!table.Columns.Contains (column))
					{
						this.ThrowError (string.Format ("Column {0} specified as primary key, not found in table {1}.", column.Name, table.Name));
					}
					
					if (first_field)
					{
						first_field = false;
					}
					else
					{
						this.buffer.Append (", ");
					}
				
					this.buffer.Append (column.Name);
				}
				
				this.buffer.Append (");\n");
			}
		}

		public void RemoveTable(string table_name)
		{
			if (!this.ValidateName (table_name))
			{
				this.ThrowError (string.Format ("Invalid table {0}.", table_name));
			}
			
			this.PrepareCommand ();
			this.command_type = DbCommandType.Silent;
			this.command_count++;
			
			this.buffer.Append ("DROP TABLE ");
			this.buffer.Append (table_name);
			this.buffer.Append (";\n");
		}
		
		public void InsertTableColumns(string table_name, SqlColumn[] columns)
		{
			if (!this.ValidateName (table_name))
			{
				this.ThrowError (string.Format ("Invalid table {0}.", table_name));
			}
			
			this.PrepareCommand ();
			this.command_type = DbCommandType.Silent;
			
			foreach (SqlColumn column in columns)
			{
				if (!column.Validate (this))
				{
					this.ThrowError (string.Format ("Invalid column {0} in table {1}.", column.Name, table_name));
				}
				
				this.command_count++;
				
				this.buffer.Append ("ALTER TABLE ");
				this.buffer.Append (table_name);
				this.buffer.Append (" ADD ");	//	not 	this.buffer.Append (" ADD COLUMN ");
				this.buffer.Append (column.Name);
				this.buffer.Append (" ");
				this.buffer.Append (this.GetSqlType (column));
				this.buffer.Append (this.GetSqlColumnAttributes (column));
				this.buffer.Append (";\n");
			}
		}

		public void UpdateTableColumns(string table_name, SqlColumn[] columns)
		{
			this.PrepareCommand ();
			this.command_type = DbCommandType.Silent;
			
			this.ThrowError (string.Format ("Cannot update table {0}. Not supported.", table_name));
		}

		public void RemoveTableColumns(string table_name, SqlColumn[] columns)
		{
			if (!this.ValidateName (table_name))
			{
				this.ThrowError (string.Format ("Invalid table {0}.", table_name));
			}
			
			this.PrepareCommand ();
			this.command_type = DbCommandType.Silent;
			
			foreach (SqlColumn column in columns)
			{
				if (!column.Validate (this))
				{
					this.ThrowError (string.Format ("Invalid column {0} in table {1}.", column.Name, table_name));
				}
				
				this.command_count++;
				
				this.buffer.Append ("ALTER TABLE ");
				this.buffer.Append (table_name);
				this.buffer.Append (" DROP ");		// not this.buffer.Append (" DROP COLUMN ");
				this.buffer.Append (column.Name);
				this.buffer.Append (";\n");
			}
		}
		
		protected void AppendAlias(SqlField field)
		{
			//	Si un alias existe, ajoute celui-ci dans le buffer.
			
			string alias = field.Alias;
			
			if ((alias != null) && (alias.Length > 0))
			{
				this.buffer.Append (' ');
				this.buffer.Append (alias);
			}
		}
		
		
		public void SelectData(SqlSelect query)
		{
			this.PrepareCommand ();
			
			this.command_type = DbCommandType.ReturningData;
			this.command_count++;
			
			//	Cela consiste à créer la commande "SELECT * FROM ..."
			//	dans toutes ses variantes...

			//	TODO-DD:  Compléter encore, notemment pour les JOINTURES
			
			this.buffer.Append ("SELECT ");
			bool first_field = true;

			if (query.Predicate == SqlSelectPredicate.Distinct)
			{
				this.buffer.Append ("DISTINCT ");
			}
			
			foreach (SqlField field in query.Fields)
			{
				if (first_field)
				{
					first_field = false;
				}
				else
				{
					this.buffer.Append (", ");
				}

				switch (field.Type)
				{
					case SqlFieldType.All:
						this.buffer.Append ("*");
						break;
					case SqlFieldType.Name:
						this.buffer.Append (field.AsName);
						this.AppendAlias (field);
						break;
					case SqlFieldType.QualifiedName:
						this.buffer.Append (field.AsQualifiedName);
						this.AppendAlias (field);
						break;
					case SqlFieldType.Aggregate:
						this.AppendAggregate (field.AsAggregate);
						this.AppendAlias (field);
						break;
					default:
						this.ThrowError (string.Format ("Unsupported field {0} in SELECT.", field.AsName));
						break;
				}
			}

			if (first_field)
			{
				// aucun champ spécifiée !
				this.ThrowError (string.Format ("No field specified in SELECT."));
			}

			this.buffer.Append (" FROM ");
			first_field = true;

			foreach (SqlField field in query.Tables)
			{
				if (first_field)
				{
					first_field = false;
				}
				else
				{
					this.buffer.Append (", ");
				}

				switch ( field.Type )
				{
					case SqlFieldType.Name:
						this.buffer.Append (field.AsName);
						this.AppendAlias (field);
						this.buffer.Append (' ');
						break;
					default:
						this.ThrowError (string.Format ("Unsupported field {0} in SELECT FROM.", field.AsName));
						break;
				}
			}

			if (first_field)
			{
				// aucune table spécifiée !
				this.ThrowError (string.Format ("No table specified in SELECT."));
			}

			first_field = true;

			foreach (SqlField field in query.Conditions)
			{
				if (first_field)
				{
					this.buffer.Append ("WHERE ");
					first_field = false;
				}
				else
				{
					this.buffer.Append (" AND ");
				}

				if (field.Type != SqlFieldType.Function)
				{
					this.ThrowError (string.Format ("Invalid field {0} in SELECT ... WHERE.", field.AsName));
					break;
				}

				this.AppendFunction (field.AsFunction);
			}

			first_field = true;

			foreach (SqlField field in query.Fields)
			{
				if (field.Order == SqlFieldOrder.None) continue;
		
				if (first_field)
				{
					this.buffer.Append ("ORDER BY ");
				}

				this.buffer.Append (field.AsName);
				this.buffer.Append (' ');

				if (field.Order == SqlFieldOrder.Inverse)
				{
					this.buffer.Append ("DESC");
				}

				if (first_field)
				{
					first_field = false;
				}				
				else
				{
					this.buffer.Append (", ");
				}
			}
			
			this.buffer.Append (";\n");
		}

		public void InsertData(string table_name, SqlFieldCollection fields)
		{
			if (!this.ValidateName (table_name))
			{
				this.ThrowError (string.Format ("Invalid table {0}.", table_name));
			}
			
			if (fields.Count == 0)
			{
				return;
			}
			
			this.PrepareCommand ();
			this.command_type = DbCommandType.NonQuery;
			this.command_count++;
			
			this.buffer.Append ("INSERT INTO ");
			this.buffer.Append (table_name);
			this.buffer.Append ("(");
			
			bool first_field = true;
			
			foreach (SqlField field in fields)
			{
				if (!this.ValidateName (field.Alias))
				{
					this.ThrowError (string.Format ("Invalid field '{0}' in table {1}.", field.Alias, table_name));
				}
				
				if (first_field)
				{
					first_field = false;
				}
				else
				{
					this.buffer.Append (",");
				}
				
				this.buffer.Append (field.Alias);
			}
			
			this.buffer.Append (") VALUES (");
			
			first_field = true;
			
			foreach (SqlField field in fields)
			{
				string data = null;
				
				switch (field.Type)
				{
					case SqlFieldType.Default:		data = "DEFAULT";						break;
					case SqlFieldType.Null:			data = "NULL";							break;
					case SqlFieldType.Constant:		data = this.AddFieldAsParam (field);	break;
					
					default:
						this.ThrowError (string.Format ("Invalid field type {0} for field {1} in table {2}.", field.Type.ToString (), field.Alias, table_name));
						break;
				}
				
				if (first_field)
				{
					first_field = false;
				}
				else
				{
					this.buffer.Append (",");
				}
				
				this.buffer.Append (data);
			}
			
			this.buffer.Append (");\n");
		}

		public void UpdateData(string table_name, SqlFieldCollection fields, SqlFieldCollection conditions)
		{
			this.PrepareCommand ();
			this.command_type = DbCommandType.NonQuery;
			this.command_count++;
			
			this.buffer.Append ("UPDATE ");
			this.buffer.Append (table_name);
			bool first_field = true;

			foreach (SqlField field in fields)
			{
				if (first_field)
				{
					first_field = false;
				}
				else
				{
					this.buffer.Append (",");
				}

				this.buffer.Append (" SET ");
				this.buffer.Append (field.Alias);
				this.buffer.Append (" = ");
				switch (field.Type)
				{
					case SqlFieldType.Constant:
						this.AppendField (field);
						break;
					case SqlFieldType.Function:
						this.AppendFunction (field.AsFunction);
						break;
						//	TODO-DD	y a-t-il d'autres cas possible ?
						//	on pourrait généraliser ces switch / case entre les diverses méthodes...
					default:
						this.ThrowError (string.Format ("Unsupported field {0} in UPDATE.", field.AsName));
						break;
				}
			}

			if (first_field)
			{
				this.ThrowError (string.Format ("No field specified in UPDATE."));
			}

			first_field = true;

			foreach (SqlField field in conditions)
			{
				if (first_field)
				{
					this.buffer.Append (" WHERE ");
					first_field = false;
				}
				else
				{
					this.buffer.Append (" AND ");
				}

				if (field.Type != SqlFieldType.Function)
				{
					this.ThrowError (string.Format ("Invalid field {0} in UPDATE ... WHERE.", field.AsName));
					break;
				}

				this.AppendFunction (field.AsFunction);
			}
			this.buffer.Append (";\n");
		}

		public void RemoveData(string table_name, SqlFieldCollection conditions)
		{
			this.PrepareCommand ();
			this.command_type = DbCommandType.NonQuery;
//			this.command_count++;
			
			// TODO:  Add FirebirdSqlBuilder.RemoveData implementation
		}

		
		public void ExecuteProcedure(string procedure_name, SqlFieldCollection fields)
		{
			this.PrepareCommand ();
//?			this.command_type = DbCommandType.NonQuery;
//			this.command_count++;
			
			// TODO:  Add FirebirdSqlBuilder.ExecuteProcedure implementation
		}

		
		public void GetSqlParameters(System.Data.IDbCommand command, SqlFieldCollection fields)
		{
			// TODO:  Add FirebirdSqlBuilder.GetSqlParameters implementation
#if false
			FbCommand fb_command = command as FbCommand;
			
			for (int i = 0; i < fb_command.Parameters.Count; i++)
			{
				FbParameter param = fb_command.Parameters[i] as FbParameter;
				
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
#endif
		}

		public void SetSqlParameters(System.Data.IDbCommand command, SqlFieldCollection fields)
		{
			// TODO:  Add FirebirdSqlBuilder.SetSqlParameters implementation
		}

		#endregion
		
		#region IDisposable Members
		public void Dispose()
		{
			this.Dispose (true);
			System.GC.SuppressFinalize (this);
		}
		#endregion
		
		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (this.command_cache != null)
				{
					this.command_cache.Dispose ();
					this.command_cache = null;
				}
			}
		}
		
		
		private FirebirdAbstraction				fb;
		private bool							auto_clear;
		private FbCommand						command_cache;
		private System.Text.StringBuilder		buffer;
		private System.Collections.ArrayList	command_params;
		private DbCommandType					command_type;
		private int								command_count;
	}
}
