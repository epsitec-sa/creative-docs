//	Copyright © 2003-2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Cresus.Database
{
	/// <summary>
	/// La classe DbRichCommand permet de regrouper plusieurs commandes pour
	/// n'en faire qu'une qui peut ensuite être exécutée au moyen de ISqlEngine,
	/// avec récupération des données dans les tables ad hoc.
	/// </summary>
	public class DbRichCommand : System.IDisposable
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
		
		
		public bool								IsReadOnly
		{
			get
			{
				return this.is_read_only;
			}
		}
		
		public bool								IsReadWrite
		{
			get
			{
				return ! this.is_read_only;
			}
		}
		
		
		public static DbRichCommand CreateFromTable(DbInfrastructure infrastructure, DbTransaction transaction, DbTable table)
		{
			return DbRichCommand.CreateFromTables (infrastructure, transaction, new DbTable[] { table }, new DbSelectCondition[] { null });
		}
		
		public static DbRichCommand CreateFromTable(DbInfrastructure infrastructure, DbTransaction transaction, DbTable table, DbSelectRevision select_revision)
		{
			DbSelectCondition condition = new DbSelectCondition (infrastructure.TypeConverter);
			condition.Revision = select_revision;
			return DbRichCommand.CreateFromTable (infrastructure, transaction, table, condition);
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
			
			if (transaction == null)
			{
				try
				{
					transaction = infrastructure.BeginTransaction (DbTransactionMode.ReadOnly);
					return DbRichCommand.CreateFromTables (infrastructure, transaction, tables, conditions);
				}
				finally
				{
					transaction.Commit ();
					transaction.Dispose ();
				}
			}
			
			DbRichCommand command = new DbRichCommand (infrastructure);
			
			int n = tables.Length;
			
			for (int i = 0; i < n; i++)
			{
				DbTable           table     = tables[i];
				DbSelectCondition condition = conditions[i];
				
				SqlSelect   select  = new SqlSelect ();
				ISqlBuilder builder = transaction.SqlBuilder;
				
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
		
		
		public void LockReadOnly()
		{
			//	En verrouillant le DbRichCommand contre les modifications, on évite que
			//	l'utilisateur n'appelle par inadvertance UpdateTables ou UpdateRealIds.
			
			this.is_read_only = true;
		}
		
		
		public void UpdateTables(DbTransaction transaction)
		{
			if (this.is_read_only)
			{
				throw new Exceptions.ReadOnlyException (this.db_access);
			}
			
			//	Sauve les données contenues du DataSet dans la base de données;
			//	pour cela, il faut que DbRichCommand ait été rempli correctement
			//	au moyen de DbInfrastructure.Execute.
			
			if (transaction == null)
			{
				throw new Exceptions.MissingTransactionException (this.db_access);
			}
			
			this.CheckValidState ();
			this.CheckRowIds ();
			
			this.UpdateLogId ();
			
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
			if (this.is_read_only)
			{
				throw new Exceptions.ReadOnlyException (this.db_access);
			}
			
			//	Met à jour les IDs des nouvelles lignes des diverses tables.
			
			if (transaction == null)
			{
				throw new Exceptions.MissingTransactionException (this.db_access);
			}
			
			this.CheckValidState ();
			this.SetCommandTransaction (transaction);
			
			try
			{
				foreach (System.Data.DataTable table in this.data_set.Tables)
				{
					//	S'il y a des clefs temporaires dans la table, on va les remplacer par
					//	des clefs définitives; en effet, on n'a pas le droit de "persister" des
					//	lignes utilisant des clefs temporaires dans la base.
					
					DbRichCommand.UpdateRealIds (this.infrastructure, transaction, table);
				}
			}
			finally
			{
				this.SetCommandTransaction (null);
			}
		}
		
		public void UpdateLogId()
		{
			if (this.is_read_only)
			{
				throw new Exceptions.ReadOnlyException (this.db_access);
			}
			
			foreach (System.Data.DataTable table in this.data_set.Tables)
			{
				DbRichCommand.UpdateLogId (table, this.infrastructure.Logger.CurrentId);
			}
		}
		
		
		public void CreateNewRow(string table_name, out System.Data.DataRow data_row)
		{
			//	Crée une ligne et ajoute celle-ci dans la table.
			
			if (this.is_read_only)
			{
				throw new Exceptions.ReadOnlyException (this.db_access);
			}
			
			this.CheckValidState ();
			
			System.Data.DataTable table = this.data_set.Tables[table_name];
			
			if (table == null)
			{
				throw new System.ArgumentException (string.Format ("Table {0} not found.", table_name), "table_name");
			}
			
			DbRichCommand.CreateRow (table, out data_row);
			
			table.Rows.Add (data_row);
		}
		
		public void DeleteExistingRow(System.Data.DataRow data_row)
		{
			if (this.is_read_only)
			{
				throw new Exceptions.ReadOnlyException (this.db_access);
			}
			
			DbRichCommand.DeleteRow (data_row);
		}
		
		public void AcceptChanges()
		{
			if (this.is_read_only)
			{
				throw new Exceptions.ReadOnlyException (this.db_access);
			}
			
			this.data_set.AcceptChanges ();
		}
		
		public void CheckRowIds()
		{
			foreach (System.Data.DataTable table in this.data_set.Tables)
			{
				DbRichCommand.CheckRowIds (table);
			}
		}
		
		
		public static bool IsRowDeleted(System.Data.DataRow row)
		{
			if (row.RowState == System.Data.DataRowState.Deleted)
			{
				return true;
			}
			
			DbKey key = new DbKey (row);
			
			if (key.Status == DbRowStatus.Deleted)
			{
				return true;
			}
			
			return false;
		}
		
		public static void CheckRowIds(System.Data.DataTable table)
		{
			for (int i = 0; i < table.Rows.Count; i++)
			{
				System.Data.DataRow row = table.Rows[i];
				
				if (row.RowState != System.Data.DataRowState.Deleted)
				{
					DbKey key = new DbKey (row);
					
					System.Diagnostics.Debug.Assert (key.IsTemporary == false);
					System.Diagnostics.Debug.Assert (key.Id.ClientId != 0);
				}
			}
		}
		
		
		public static void UpdateRealIds(DbInfrastructure infrastructure, DbTransaction transaction, System.Data.DataTable table)
		{
			System.Collections.ArrayList list = DbRichCommand.FindRowsUsingTemporaryIds (table);
			
			if (list.Count == 0)
			{
				return;
			}
			
			//	Trouve la clef identifiant la table courante (la recherche est basée sur
			//	le nom de la table) :
			
			DbKey table_key = infrastructure.FindDbTableKey (transaction, table.TableName);
			
			//	Alloue une série de clefs (contiguës) pour la table et attribue les
			//	séquentiellement aux diverses clefs temporaires; grâce aux relations
			//	mises en place dans le DataSet, les foreign keys seront automatiquement
			//	synchronisées aussi.
			
			long id = infrastructure.NewRowIdInTable (transaction, table_key, list.Count);
			
			foreach (System.Data.DataRow data_row in list)
			{
				DbKey key = new DbKey (data_row);
				
				System.Diagnostics.Debug.Assert (key.IsTemporary);
				
				key = new DbKey (id++);
				
				data_row.BeginEdit ();
				data_row[Tags.ColumnId]     = key.Id.Value;
				data_row[Tags.ColumnStatus] = key.IntStatus;
				data_row.EndEdit ();
			}
		}
		
		public static void UpdateLogId(System.Data.DataTable table, DbId log_id)
		{
			for (int i = 0; i < table.Rows.Count; i++)
			{
				System.Data.DataRow row = table.Rows[i];
				
				switch (row.RowState)
				{
					case System.Data.DataRowState.Added:
					case System.Data.DataRowState.Modified:
						row[Tags.ColumnRefLog] = log_id.Value;
						break;
				}
			}
		}
		
		
		public static void CreateRow(System.Data.DataTable table, out System.Data.DataRow data_row)
		{
			//	Crée une ligne, mais ne l'ajoute pas à la table.
			
			data_row = table.NewRow ();
			
			DbKey key = new DbKey (DbKey.CreateTemporaryId (), DbRowStatus.Live);
			
			data_row.BeginEdit ();
			data_row[Tags.ColumnId]     = key.Id.Value;
			data_row[Tags.ColumnStatus] = key.IntStatus;
			data_row.EndEdit ();
		}
		
		public static void DeleteRow(System.Data.DataRow data_row)
		{
			DbKey row_key = new DbKey (data_row);
			
			//	Si la ligne a encore une clef temporaire, cela signifie qu'elle n'a pas encore
			//	été écrite dans la base; on peut donc simplement supprimer la ligne de la table.
			//	Dans le cas contraire, on ne supprime jamais réellement les lignes effacées et
			//	on change simplement le statut de la ligne à "deleted".
			
			if (row_key.IsTemporary)
			{
				System.Data.DataTable data_table = data_row.Table;
				data_table.Rows.Remove (data_row);
			}
			else
			{
				data_row[Tags.ColumnStatus] = DbKey.ConvertToIntStatus (DbRowStatus.Deleted);
			}
		}
		
		public static void KillRow(System.Data.DataRow data_row)
		{
			//	Supprime réellement la ligne de la table. Cette méthode est réservée à un
			//	usage très limité; en principe, on utilisera DeleteRow, sauf pour la queue
			//	des requêtes, par exemple.
			
			if (data_row.RowState != System.Data.DataRowState.Deleted)
			{
				data_row.Delete ();
			}
		}
		
		
		public static System.Collections.ArrayList FindRowsUsingTemporaryIds(System.Data.DataTable table)
		{
			//	Passe en revue toutes les lignes de la table pour déterminer s'il y a des
			//	clefs temporaires en utilisation et retourne la liste des lignes concernées.
			
			System.Collections.ArrayList list = new System.Collections.ArrayList ();
			
			for (int i = 0; i < table.Rows.Count; i++)
			{
				System.Data.DataRow row = table.Rows[i];
				
				if (row.RowState != System.Data.DataRowState.Deleted)
				{
					DbKey key = new DbKey (row);
					
					if (key.IsTemporary)
					{
						list.Add (row);
					}
				}
			}
			
			return list;
		}
		
		
		public static void DebugDumpCommand(System.Data.IDbCommand command)
		{
			System.Diagnostics.Debug.WriteLine (command.CommandText);
			
			foreach (System.Data.IDataParameter command_parameter in command.Parameters)
			{
				System.Diagnostics.Debug.WriteLine (string.Format ("  {0} = {1}, type {2}", command_parameter.ParameterName, command_parameter.Value, command_parameter.Value.GetType ().FullName));
			}
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
				throw new Exceptions.GenericException (this.db_access, "No data set defined.");
			}
			
			if ((this.adapters == null) ||
				(this.adapters.Length == 0))
			{
				throw new Exceptions.GenericException (this.db_access, "No adapters defined.");
			}
			
			if (this.commands.Count == 0)
			{
				throw new Exceptions.GenericException (this.db_access, "No commands defined.");
			}
			
			if (this.tables.Count == 0)
			{
				throw new Exceptions.GenericException (this.db_access, "No tables defined.");
			}
			
			if (this.infrastructure == null)
			{
				throw new Exceptions.GenericException (this.db_access, "No infrastructure defined.");
			}
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
				throw new Exceptions.MissingTransactionException (db_access);
			}
			
			if (this.data_set != null)
			{
				throw new Exceptions.GenericException (db_access, "DataSet already exists.");
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
		
		
		#region IDisposable Members
		public void Dispose()
		{
			this.Dispose (true);
		}
		#endregion
		
		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (this.data_set != null)
				{
					this.data_set.Dispose ();
					this.data_set = null;
				}
				
				System.Data.IDbCommand[] commands = this.commands.ToArray ();
				
				for (int i = 0; i < commands.Length; i++)
				{
					commands[i].Dispose ();
				}
			}
		}
		
		
		protected DbInfrastructure				infrastructure;
		protected Collections.DbCommands		commands;
		protected Collections.DbTables			tables;
		protected System.Data.DataSet			data_set;
		protected DbAccess						db_access;
		protected System.Data.IDataAdapter[]	adapters;
		
		private bool							is_read_only;
	}
}
