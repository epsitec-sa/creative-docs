//	Copyright © 2003, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Statut : en chantier

using FirebirdSql.Data.Firebird;

namespace Epsitec.Cresus.Database.Implementation
{
	using Epsitec.Cresus.Database;
	
	/// <summary>
	/// Implémentation de ISqlBuilder pour Firebird.
	/// </summary>
	public class FirebirdSqlBuilder : ISqlBuilder
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

		public System.Data.IDbCommand			Command
		{
			get
			{
				this.UpdateCommand ();
				return this.command_cache;
			}
		}

		
		public void Clear()
		{
			//	On n'a pas le droit de faire un Dispose de l'objet 'commande', car il peut encore
			//	être utilisé par un appelant. C'est le cas lorsque l'on est en mode AutoClear.
			
			this.command_cache = null;
			this.command_type  = DbCommandType.None;
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
				this.command_type |= DbCommandType.FlagMultiple;
				
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
				
				this.buffer.Append ("ALTER TABLE ");
				this.buffer.Append (table_name);
				this.buffer.Append (" ADD ");	//	not 	this.buffer.Append (" ADD COLUMN ");
				this.buffer.Append (column.Name);
				this.buffer.Append (" ");
				this.buffer.Append (this.GetSqlType (column));
				this.buffer.Append (this.GetSqlColumnAttributes (column));
				this.buffer.Append (";\n");
			}
			
			if (columns.Length > 1)
			{
				this.command_type |= DbCommandType.FlagMultiple;
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
				
				this.buffer.Append ("ALTER TABLE ");
				this.buffer.Append (table_name);
				this.buffer.Append (" DROP ");		// not this.buffer.Append (" DROP COLUMN ");
				this.buffer.Append (column.Name);
				this.buffer.Append (";\n");
			}
			
			if (columns.Length > 1)
			{
				this.command_type |= DbCommandType.FlagMultiple;
			}
		}

		
		public void SelectData(SqlSelect query)
		{
			this.PrepareCommand ();
			this.command_type = DbCommandType.ReturningData;
			
			//	TODO-DD:  Add FirebirdSqlBuilder.SelectData implementation

			//	Cela consiste à créer la commande "SELECT * FROM ..."
			//	dans toutes ses variantes...
			
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

				switch ( field.Type )
				{
					case SqlFieldType.All:
						this.buffer.Append ("* ");
						break;
					case SqlFieldType.Name:
						this.buffer.Append (field.AsName);
						this.buffer.Append (' ');
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

			this.buffer.Append ("FROM ");
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
					this.buffer.Append ("AND ");
				}

				if (field.Type != SqlFieldType.Function)
				{
					this.ThrowError (string.Format ("Invalid field {0} in SELECT ... WHERE.", field.AsName));
					break;
				}

				this.buffer.Append( field.AsFunction.ToString() );
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
			
			this.buffer.Append ("INSERT INTO ");
			this.buffer.Append (table_name);
			this.buffer.Append ("(");
			
			bool first_field = true;
			
			foreach (SqlField field in fields)
			{
				if (!this.ValidateName (field.Alias))
				{
					this.ThrowError (string.Format ("Invalid field {0} in table {1}.", field.Alias, table_name));
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
			
			// TODO:  Add FirebirdSqlBuilder.UpdateData implementation
		}

		public void RemoveData(string table_name, SqlFieldCollection conditions)
		{
			this.PrepareCommand ();
			this.command_type = DbCommandType.NonQuery;
			
			// TODO:  Add FirebirdSqlBuilder.RemoveData implementation
		}

		
		public void ExecuteProcedure(string procedure_name, SqlFieldCollection fields)
		{
			this.PrepareCommand ();
//?			this.command_type = DbCommandType.NonQuery;
			
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
		
		private FirebirdAbstraction				fb;
		private bool							auto_clear;
		private FbCommand						command_cache;
		private System.Text.StringBuilder		buffer;
		private System.Collections.ArrayList	command_params;
		private DbCommandType					command_type;
	}
}
