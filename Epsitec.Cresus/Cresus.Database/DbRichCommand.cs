//	Copyright © 2003-2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Cresus.Database
{
	/// <summary>
	/// La classe DbRichCommand permet de regrouper plusieurs commandes pour
	/// n'en faire qu'une qui peut ensuite être exécutée au moyen de ISqlEngine,
	/// avec récupération des données dans les tables ad hoc.
	/// </summary>
	public class DbRichCommand
	{
		public DbRichCommand(DbInfrastructure infrastructure)
		{
			this.infrastructure = infrastructure;
			
			this.commands = new Collections.DbCommands ();
			this.tables   = new Collections.DbTables ();
		}
		
		
		public Collections.DbCommands			Commands
		{
			get
			{
				return this.commands;
			}
		}
		
		public Collections.DbTables				Tables
		{
			get
			{
				return this.tables;
			}
		}
		
		public System.Data.DataSet				DataSet
		{
			get
			{
				return this.data_set;
			}
		}
		
		public DbInfrastructure					Infrastructure
		{
			get
			{
				return this.infrastructure;
			}
		}
		
		
		public static DbRichCommand CreateFromTable(DbInfrastructure infrastructure, DbTransaction transaction, DbTable table)
		{
			return DbRichCommand.CreateFromTables (infrastructure, transaction, new DbTable[] { table }, new DbSelectCondition[] { null });
		}
		
		public static DbRichCommand CreateFromTable(DbInfrastructure infrastructure, DbTransaction transaction, DbTable table, DbSelectCondition condition)
		{
			return DbRichCommand.CreateFromTables (infrastructure, transaction, new DbTable[] { table }, new DbSelectCondition[] { condition });
		}
		
		public static DbRichCommand CreateFromTables(DbInfrastructure infrastructure, DbTransaction transaction, Collections.DbTables tables)
		{
			return DbRichCommand.CreateFromTables (infrastructure, transaction, tables.Array);
		}
		
		public static DbRichCommand CreateFromTables(DbInfrastructure infrastructure, DbTransaction transaction, params DbTable[] tables)
		{
			return DbRichCommand.CreateFromTables (infrastructure, transaction, tables, new DbSelectCondition[tables.Length]);
		}
		
		public static DbRichCommand CreateFromTables(DbInfrastructure infrastructure, DbTransaction transaction, DbTable[] tables, DbSelectCondition[] conditions)
		{
			System.Diagnostics.Debug.Assert (tables.Length == conditions.Length);
			
			DbRichCommand command = new DbRichCommand (infrastructure);
			
			int n = tables.Length;
			
			for (int i = 0; i < n; i++)
			{
				DbTable           table     = tables[i];
				DbSelectCondition condition = conditions[i];
				
				SqlSelect   select  = new SqlSelect ();
				ISqlBuilder builder = infrastructure.SqlBuilder;
				
				select.Fields.Add (SqlField.CreateAll ());
				select.Tables.Add (table.Name, SqlField.CreateName (table.CreateSqlName ()));
				
				if (condition == null)
				{
					select.Conditions.Add (new SqlFunction (SqlFunctionType.CompareFalse));
				}
				else
				{
					condition.CreateConditions (table, select.Conditions);
				}
				
				builder.SelectData (select);
				
				command.Commands.Add (builder.Command);
				command.Tables.Add (table);
			}
			
			infrastructure.Execute (transaction, command);
			
			return command;
		}
		
		
		public void InternalFillDataSet(DbAccess db_access, System.Data.IDbTransaction transaction, System.Data.IDbDataAdapter[] adapters)
		{
			//	Utiliser DbInfrastructure.Execute en lieu et place de cette méthode !
			
			//	Cette méthode ne devrait jamais être appelée par un utilisateur : elle est réservée
			//	aux classes implémentant ISqlEngine. Pour s'assurer que personne ne se trompe, on
			//	vérifie l'identité de l'appelant :
			//
			//	xxx --> DbInfrastructure --> ISqlEngine --> DbRichCommand
			
			System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace (true);
			System.Diagnostics.StackFrame caller_1 = trace.GetFrame (1);
			System.Diagnostics.StackFrame caller_2 = trace.GetFrame (2);
			
			System.Type caller_class_type = caller_1.GetMethod ().DeclaringType;
			System.Type req_interf_type   = typeof (Epsitec.Cresus.Database.ISqlEngine);
			
			if ((caller_class_type.GetInterface (req_interf_type.FullName) != req_interf_type) ||
				(caller_2.GetMethod ().DeclaringType != typeof (DbInfrastructure)))
			{
				throw new System.InvalidOperationException (string.Format ("Method may not be called by {0}.{1}", caller_class_type.FullName, caller_1.GetMethod ().Name));
			}
			
			System.Diagnostics.Debug.Assert (db_access.IsValid);
			
			if (transaction == null)
			{
				throw new DbMissingTransactionException (db_access);
			}
			
			if (this.data_set != null)
			{
				throw new DbException (db_access, "DataSet already exists.");
			}
			
			//	Définit et remplit le DataSet en se basant sur les données fournies
			//	par l'objet 'adapter' (ADO.NET).
			
			this.db_access = db_access;
			this.data_set  = new System.Data.DataSet ();
			this.adapters  = adapters;
			
			this.SetCommandTransaction (transaction);
			
			try
			{
				for (int i = 0; i < this.tables.Count; i++)
				{
					DbTable db_table = this.tables[i];
					
					string  ado_name_table = "Table";
					string  db_name_table  = db_table.Name;
					
					//	Il faut (re)nommer les tables afin d'avoir les noms qui correspondent
					//	à ce que définit DbTable, et faire pareil pour les colonnes.
					
					System.Data.ITableMapping mapping = this.adapters[i].TableMappings.Add (ado_name_table, db_name_table);
					
					for (int c = 0; c < db_table.Columns.Count; c++)
					{
						DbColumn db_column = db_table.Columns[c];
						
						string db_name_column  = db_column.CreateDisplayName ();
						string ado_name_column = db_column.CreateSqlName ();
						
						mapping.ColumnMappings.Add (ado_name_column, db_name_column);
					}
					
					this.adapters[i].Fill (this.data_set);
				}
			}
			finally
			{
				this.SetCommandTransaction (null);
			}
			
			this.CreateDataRelations ();
		}
		
		
		public void UpdateTables(DbTransaction transaction)
		{
			//	Sauve les données contenues du DataSet dans la base de données;
			//	pour cela, il faut que DbRichCommand ait été rempli correctement
			//	au moyen de DbInfrastructure.Execute.
			
			if (transaction == null)
			{
				throw new DbMissingTransactionException (this.db_access);
			}
			
			this.CheckValidState ();
			this.SetCommandTransaction (transaction);
			
			try
			{
				for (int i = 0; i < this.adapters.Length; i++)
				{
					this.adapters[i].Update (this.data_set);
				}
			}
			finally
			{
				this.SetCommandTransaction (null);
			}
		}
		
		public void UpdateRealIds(DbTransaction transaction)
		{
			//	Met à jour les IDs des nouvelles lignes des diverses tables.
			
			if (transaction == null)
			{
				throw new DbMissingTransactionException (this.db_access);
			}
			
			this.CheckValidState ();
			this.SetCommandTransaction (transaction);
			
			try
			{
				foreach (System.Data.DataTable table in this.data_set.Tables)
				{
					DbKey table_key = this.infrastructure.FindDbTableKey (transaction, table.TableName);
					
					int n = table.Rows.Count;
					int m = 0;
					
					for (int i = 0; i < n; i++)
					{
						DbKey key = new DbKey (table.Rows[i]);
						
						if (key.IsTemporary)
						{
							m++;
						}
					}
					
					if (m > 0)
					{
						long id = this.infrastructure.NewRowIdInTable (transaction, table_key, m);
						
						for (int i = 0; i < n; i++)
						{
							System.Data.DataRow data_row = table.Rows[i];
							
							DbKey key = new DbKey (data_row);
							
							if (key.IsTemporary)
							{
								key = new DbKey (id++);
								
								data_row.BeginEdit ();
								data_row[Tags.ColumnId]       = key.Id;
								data_row[Tags.ColumnRevision] = key.Revision;
								data_row[Tags.ColumnStatus]   = key.IntStatus;
								data_row.EndEdit ();
							}
						}
					}
				}
			}
			finally
			{
				this.SetCommandTransaction (null);
			}
		}
		
		
		public void DeleteRow(System.Data.DataRow data_row)
		{
			data_row[Tags.ColumnStatus] = DbKey.ConvertToIntStatus (DbRowStatus.Deleted);
		}
		
		public void CreateNewRow(string table_name, out System.Data.DataRow data_row)
		{
			this.CheckValidState ();
			
			System.Data.DataTable table = this.data_set.Tables[table_name];
			
			if (table == null)
			{
				throw new System.ArgumentException (string.Format ("Table {0} not found.", table_name), "table_name");
			}
			
			data_row = table.NewRow ();
			
			DbKey key = new DbKey (DbKey.CreateTemporaryId (), 0, DbRowStatus.Live);
			
			data_row[Tags.ColumnId]       = key.Id;
			data_row[Tags.ColumnRevision] = key.Revision;
			data_row[Tags.ColumnStatus]   = key.IntStatus;
			
			table.Rows.Add (data_row);
		}
		
		
		protected void CreateDataRelations ()
		{
			//	Crée pour le DataSet actuel les relations entre les diverses colonnes,
			//	en s'appuyant sur les propriétés de DbTable/DbColumn.
			
			for (int i = 0; i < this.tables.Count; i++)
			{
				DbTable               db_child_table  = this.tables[i];
				System.Data.DataTable ado_child_table = this.data_set.Tables[db_child_table.Name];
				DbForeignKey[]        db_foreign_keys = db_child_table.ForeignKeys;
				
				foreach (DbForeignKey fk in db_foreign_keys)
				{
					int n = fk.Columns.Length;
					
					System.Data.DataTable ado_parent_table = this.data_set.Tables[fk.ParentTableName];
					
					if (ado_parent_table == null)
					{
						//	La table parent n'est pas chargée dans le DataSet, ce qui veut dire
						//	que l'on doit ignorer la relation.
						
						continue;
					}
					
					System.Data.DataColumn[] ado_parent_cols = new System.Data.DataColumn[n];
					System.Data.DataColumn[] ado_child_cols  = new System.Data.DataColumn[n];
					
					for (int j = 0; j < n; j++)
					{
						ado_child_cols[j]  = ado_child_table.Columns[fk.Columns[j].CreateDisplayName ()];
						ado_parent_cols[j] = ado_parent_table.Columns[fk.Columns[j].ParentColumnName];
					}
					
					System.Data.DataRelation relation = new System.Data.DataRelation (null, ado_parent_cols, ado_child_cols);
					this.data_set.Relations.Add (relation);
					
					System.Data.ForeignKeyConstraint constraint = relation.ChildKeyConstraint;
					
					System.Diagnostics.Debug.Assert (constraint != null);
					System.Diagnostics.Debug.Assert (constraint.UpdateRule == System.Data.Rule.Cascade);
				}
			}
		}
		
		protected void SetCommandTransaction(System.Data.IDbTransaction transaction)
		{
			if (transaction is DbTransaction)
			{
				transaction = ((DbTransaction) transaction).Transaction;
			}
			
			for (int i = 0; i < this.commands.Count; i++)
			{
				this.commands[i].Transaction = transaction;
			}
		}
		
		protected void CheckValidState()
		{
			if (this.db_access.IsValid == false)
			{
				throw new System.InvalidOperationException ("No database access defined.");
			}
			
			if (this.data_set == null)
			{
				throw new DbException (this.db_access, "No data set defined.");
			}
			
			if ((this.adapters == null) ||
				(this.adapters.Length == 0))
			{
				throw new DbException (this.db_access, "No adapters defined.");
			}
			
			if (this.commands.Count == 0)
			{
				throw new DbException (this.db_access, "No commands defined.");
			}
			
			if (this.tables.Count == 0)
			{
				throw new DbException (this.db_access, "No tables defined.");
			}
			
			if (this.infrastructure == null)
			{
				throw new DbException (this.db_access, "No infrastructure defined.");
			}
		}
		
		
		
		protected DbInfrastructure				infrastructure;
		protected Collections.DbCommands		commands;
		protected Collections.DbTables			tables;
		protected System.Data.DataSet			data_set;
		protected DbAccess						db_access;
		protected System.Data.IDataAdapter[]	adapters;
	}
}
