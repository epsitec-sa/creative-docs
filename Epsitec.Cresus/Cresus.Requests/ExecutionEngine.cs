//	Copyright © 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using Epsitec.Cresus.Database;

namespace Epsitec.Cresus.Requests
{
	using SqlFields = Database.Collections.SqlFields;
	
	/// <summary>
	/// La classe ExecutionEngine exécute les requêtes qui modifient la base de
	/// données.
	/// </summary>
	public class ExecutionEngine
	{
		public ExecutionEngine(DbInfrastructure infrastructure)
		{
			this.infrastructure = infrastructure;
		}
		
		
		public DbInfrastructure					Infrastructure
		{
			get
			{
				return this.infrastructure;
			}
		}
		
		public DbId								CurrentLogId
		{
			get
			{
				return this.current_log_id;
			}
		}
		
		public DbTransaction					CurrentTransaction
		{
			get
			{
				return this.current_transaction;
			}
		}
		
		
		public void Execute(DbTransaction transaction, AbstractRequest request)
		{
			try
			{
				this.DefineCurrentLogId ();
				this.DefineCurrentTransaction (transaction);
				
				request.Execute (this);
				
				DbRichCommand.DebugDumpCommand (this.CurrentTransaction.SqlBuilder.Command);
				
				this.infrastructure.ExecuteSilent (transaction);
			}
			finally
			{
				this.CleanUp ();
			}
		}
		
		
		public void GenerateInsertDataCommand(string table_name, string[] columns, object[] values)
		{
			//	Génère une commande SqlBuilder.InsertData avec les paramètres spécifiés. Les données
			//	sont converties au préalable dans le format propre à la base sous-jacente.
			
			if (columns.Length != values.Length)
			{
				throw new System.ArgumentException ("Columns/values mismatch.");
			}
			
			DbTable     table       = this.FindTable (table_name);
			SqlColumn[] sql_columns = this.CreateSqlColumns (table, columns);
			SqlFields   sql_fields  = this.CreateSqlValues (table, sql_columns, values);
			
			if (this.IsLogIdNeededForTable (table))
			{
				this.PatchLogId (table, columns, sql_fields);
			}
			
			this.PrepareNewCommand ();
			this.CurrentTransaction.SqlBuilder.InsertData (table.CreateSqlName (), sql_fields);
		}
		
		public void GenerateUpdateDataCommand(string table_name, string[] cond_columns, object[] cond_values, string[] data_columns, object[] data_values)
		{
			//	Génère une commande SqlBuilder.UpdateData avec les paramètres spécifiés. Les données
			//	sont converties au préalable dans le format propre à la base sous-jacente.
			
			if (cond_columns.Length != cond_values.Length)
			{
				throw new System.ArgumentException ("Condition columns/values mismatch.");
			}
			if (data_columns.Length != data_values.Length)
			{
				throw new System.ArgumentException ("Data columns/values mismatch.");
			}
			
			DbTable table = this.FindTable (table_name);
			
			SqlColumn[] sql_cond_columns = this.CreateSqlColumns (table, cond_columns);
			SqlColumn[] sql_data_columns = this.CreateSqlColumns (table, data_columns);
			SqlFields   sql_cond_values  = this.CreateSqlValues (table, sql_cond_columns, cond_values);
			SqlFields   sql_data_fields  = this.CreateSqlValues (table, sql_data_columns, data_values);
			
			if (this.IsLogIdNeededForTable (table))
			{
				this.PatchLogId (table, data_columns, sql_data_fields);
			}
			
			SqlFields sql_cond_fields = new Database.Collections.SqlFields ();
			
			for (int i = 0; i < sql_cond_values.Count; i++)
			{
				SqlField name  = SqlField.CreateName (sql_cond_columns[i].Name);
				SqlField value = sql_cond_values[i];
				
				sql_cond_fields.Add (new SqlFunction (SqlFunctionType.CompareEqual, name, value));
			}
			
			this.PrepareNewCommand ();
			this.CurrentTransaction.SqlBuilder.UpdateData (table.CreateSqlName (), sql_data_fields, sql_cond_fields);
		}
		
		
		protected bool IsLogIdNeededForTable(DbTable table)
		{
			return table.Columns.Contains (Tags.ColumnRefLog);
		}
		
		protected void PatchLogId(DbTable table, string[] db_column_names, SqlFields fields)
		{
			for (int i = 0; i < db_column_names.Length; i++)
			{
				if (db_column_names[i] == Tags.ColumnRefLog)
				{
					//	L'appelant spécifiait déjà une colonne pour le LOG ID, alors on met à jour
					//	le champ correspondant :
					
					System.Diagnostics.Debug.Assert (fields[i].RawType == DbKey.RawTypeForId);
					System.Diagnostics.Debug.Assert (fields[i].Alias == table.Columns[Tags.ColumnRefLog].CreateSqlName ());
					
					fields[i].Overwrite (SqlField.CreateConstant (this.CurrentLogId.Value, DbKey.RawTypeForId));
					fields[i].Alias = table.Columns[Tags.ColumnRefLog].CreateSqlName ();
					
					return;
				}
			}
			
			//	Aucun champ ne définissait de LOG ID; on doit donc ajouter un champ supplémentaire
			//	avec cette valeur :
			
			SqlField field = SqlField.CreateConstant (this.CurrentLogId.Value, DbKey.RawTypeForId);
			string   alias = table.Columns[Tags.ColumnRefLog].CreateSqlName ();
			
			fields.Add (alias, field);
		}
		
		
		
		
		private void DefineCurrentLogId()
		{
			System.Diagnostics.Debug.Assert (this.current_log_id.Value == 0);
			
			this.current_log_id = this.infrastructure.Logger.CurrentId;
		}
		
		private void DefineCurrentTransaction(DbTransaction transaction)
		{
			System.Diagnostics.Debug.Assert (this.current_transaction == null);
			
			this.current_transaction = transaction;
		}
		
		
		private DbTable FindTable(string table_name)
		{
			DbTable table = this.infrastructure.ResolveDbTable (this.CurrentTransaction, table_name);
			
			if (table == null)
			{
				throw new System.ArgumentException (string.Format ("Cannot find table {0}.", table_name));
			}
			
			return table;
		}
		
		private SqlColumn[] CreateSqlColumns(DbTable table, string[] column_names)
		{
			SqlColumn[]    columns   = new SqlColumn[column_names.Length];
			ITypeConverter converter = this.infrastructure.TypeConverter;
			
			for (int i = 0; i < columns.Length; i++)
			{
				columns[i] = table.Columns[column_names[i]].CreateSqlColumn (converter);
			}
			
			return columns;
		}
		
		private SqlFields CreateSqlValues(DbTable table, SqlColumn[] columns, object[] values)
		{
			SqlFields sql_fields = new Database.Collections.SqlFields ();
			
			for (int i = 0; i < columns.Length; i++)
			{
				SqlField field = SqlField.CreateConstant (columns[i].ConvertToInternalType (values[i]), columns[i].Type);
				string   alias = columns[i].Name;
				sql_fields.Add (alias, field);
			}
			
			System.Diagnostics.Debug.Assert (sql_fields.Count == columns.Length);
			System.Diagnostics.Debug.Assert (sql_fields.Count == values.Length);
			
			return sql_fields;
		}
		
		
		private void PrepareNewCommand()
		{
			if (this.pending_commands > 0)
			{
				this.CurrentTransaction.SqlBuilder.AppendMore ();
			}
			
			this.pending_commands++;
		}
		
		private void CleanUp()
		{
			System.Diagnostics.Debug.Assert (this.current_transaction != null);
			
			if (this.pending_commands > 0)
			{
				this.CurrentTransaction.SqlBuilder.Clear ();
				this.pending_commands = 0;
			}
			
			this.current_log_id      = DbId.Zero;
			this.current_transaction = null;
		}
		
		
		private DbInfrastructure				infrastructure;
		private DbId							current_log_id;
		private DbTransaction					current_transaction;
		private int								pending_commands;
	}
}
