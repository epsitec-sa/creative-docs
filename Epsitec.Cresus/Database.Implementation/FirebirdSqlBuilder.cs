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
				
				foreach (SqlField field in this.command_params)
				{
					FbDbType    fb_type  = this.GetFbType (field.RawType);
					string      fb_name    = string.Format (TypeConverter.InvariantFormatProvider, "@PARAM_{0}", field_i++);
					FbParameter fb_param = new FbParameter (fb_name, fb_type);
					
					fb_param.Value = field.AsConstant;
					
					this.command_cache.Parameters.Add (fb_param);
				}
				
				this.command_cache.CommandType = System.Data.CommandType.Text;
				this.command_cache.CommandText = this.buffer.ToString ();
			}
		}
		
		protected string GetSqlType(SqlColumn column)
		{
			string basic_type = null;
			string length;
			
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
			string name = string.Format (TypeConverter.InvariantFormatProvider, "@PARAM_{0}", this.command_params.Count);
			this.command_params.Add (field);
			return name;
		}
		
		protected string GetSqlColumnAttributes(SqlColumn column)
		{
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
		
		
		
		
		#region ISqlBuilder Members

		public bool								AutoClear
		{
			get { return this.auto_clear; }
			set { this.auto_clear = value; }
		}

		public System.Data.IDbCommand			Command
		{
			get
			{
				if (this.auto_clear)
				{
					this.Clear ();
				}
				
				this.UpdateCommand ();
				
				return this.command_cache;
			}
		}

		
		public void Clear()
		{
			this.command_cache = null;
			this.buffer = new System.Text.StringBuilder ();
			this.command_params.Clear ();
		}

		
		public bool ValidateName(string value)
		{
			return DbSqlStandard.ValidateName (value);
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
			
			foreach (SqlColumn column in columns)
			{
				if (!column.Validate (this))
				{
					this.ThrowError (string.Format ("Invalid column {0} in table {1}.", column.Name, table_name));
				}
				
				this.buffer.Append ("ALTER TABLE ");
				this.buffer.Append (table_name);
				this.buffer.Append (" ADD COLUMN ");
				this.buffer.Append (column.Name);
				this.buffer.Append (" ");
				this.buffer.Append (this.GetSqlType (column));
				this.buffer.Append (this.GetSqlColumnAttributes (column));
				this.buffer.Append (";\n");
			}
		}

		public void UpdateTableColumns(string table_name, SqlColumn[] columns)
		{
			this.ThrowError (string.Format ("Cannot update table {0}. Not supported.", table_name));
		}

		public void RemoveTableColumns(string table_name, SqlColumn[] columns)
		{
			if (!this.ValidateName (table_name))
			{
				this.ThrowError (string.Format ("Invalid table {0}.", table_name));
			}
			
			foreach (SqlColumn column in columns)
			{
				if (!column.Validate (this))
				{
					this.ThrowError (string.Format ("Invalid column {0} in table {1}.", column.Name, table_name));
				}
				
				this.buffer.Append ("ALTER TABLE ");
				this.buffer.Append (table_name);
				this.buffer.Append (" DROP COLUMN ");
				this.buffer.Append (column.Name);
				this.buffer.Append (";\n");
			}
		}

		
		public void SelectData(SqlSelect query)
		{
			// TODO:  Add FirebirdSqlBuilder.SelectData implementation
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
			// TODO:  Add FirebirdSqlBuilder.UpdateData implementation
		}

		public void RemoveData(string table_name, SqlFieldCollection conditions)
		{
			// TODO:  Add FirebirdSqlBuilder.RemoveData implementation
		}

		
		public void ExecuteProcedure(string procedure_name, SqlFieldCollection fields)
		{
			// TODO:  Add FirebirdSqlBuilder.ExecuteProcedure implementation
		}

		
		public void GetSqlParameters(System.Data.IDbCommand command, SqlFieldCollection fields)
		{
			// TODO:  Add FirebirdSqlBuilder.GetSqlParameters implementation
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
	}
}
