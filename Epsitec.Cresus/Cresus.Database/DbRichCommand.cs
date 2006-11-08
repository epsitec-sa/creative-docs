//	Copyright © 2003-2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;

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
		
		
		public int								ReplaceStatisticsInsertCount
		{
			get
			{
				return this.stat_replace_insert_count;
			}
		}
		
		public int								ReplaceStatisticsUpdateCount
		{
			get
			{
				return this.stat_replace_update_count;
			}
		}
		
		public int								ReplaceStatisticsDeleteCount
		{
			get
			{
				return this.stat_replace_delete_count;
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
			return DbRichCommand.CreateFromTables (infrastructure, transaction, tables.ToArray ());
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
			
			foreach (System.Data.DataTable data_table in command.DataSet.Tables)
			{
				DbRichCommand.RelaxConstraints (data_table);
			}
			
			return command;
		}
		
		
		public static void RelaxConstraints(System.Data.DataTable data_table)
		{
			if (data_table.Columns[Tags.ColumnId].Unique == false)
			{
				System.Diagnostics.Debug.WriteLine (string.Format ("Warning: Table {0} ID not unique, fixing.", data_table.TableName));
				data_table.Columns[Tags.ColumnId].Unique = true;
			}
			
			//	Si certaines colonnes empêchent l'utilisateur de valeurs 'null' dans la
			//	base de données, il faut effacer ces fanions pour éviter des problèmes
			//	pendant le peuplement de la table (où toutes les colonnes ne sont pas
			//	encore affectées) :
			
			foreach (System.Data.DataColumn data_column in data_table.Columns)
			{
				if (data_column.AllowDBNull == false)
				{
					data_column.AllowDBNull = true;
				}
			}
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
			
			//	Sauve les données du DataSet dans la base de données (mise à jour soit
			//	par UPDATE, soit par INSERT, en fonction de l'état de chaque ligne);
			//	pour que cela fonctionne, il faut que DbRichCommand ait été rempli
			//	correctement au préalable, au moyen de DbInfrastructure.Execute.
			
			if (transaction == null)
			{
				throw new Exceptions.MissingTransactionException (this.db_access);
			}
			
			this.CheckValidState ();
			this.CheckRowIds ();
			
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
				this.PopCommandTransaction ();
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
				this.PopCommandTransaction ();
			}
		}
		
		public void UpdateLogIds()
		{
			this.UpdateLogIds (this.infrastructure.Logger.CurrentId);
		}
		
		public void UpdateLogIds(DbId log_id)
		{
			if (this.is_read_only)
			{
				throw new Exceptions.ReadOnlyException (this.db_access);
			}
			
			foreach (System.Data.DataTable table in this.data_set.Tables)
			{
				DbRichCommand.UpdateLogIds (table, log_id);
			}
		}
		
		
		public void ReplaceTables(DbTransaction transaction)
		{
			this.ReplaceTables (transaction, null);
		}
		
		public void ReplaceTables(DbTransaction transaction, IReplaceOptions options)
		{
			//	Similaire à UpdateTables, mais en écrasant les données contenues dans
			//	la base, sans égard pour d'éventuelles anciennes données déjà présentes
			//	(là aussi, UPDATE sera utilisé d'abord et INSERT en cas de besoin).
			
			if (transaction == null)
			{
				throw new Exceptions.MissingTransactionException (this.db_access);
			}
			
			this.CheckValidState ();
			this.CheckRowIds ();
			
			this.ReplaceTablesWithoutValidityChecking(transaction, options);
		}
		
		public void ReplaceTablesWithoutValidityChecking(DbTransaction transaction, IReplaceOptions options)
		{
			//	ATTENTION: Cette méthode ne gère pas les conflits; elle écrase les données
			//	dans la base en fonction du contenu des tables du DataSet.
			
			if (this.is_read_only)
			{
				throw new Exceptions.ReadOnlyException (this.db_access);
			}
			
			//	On ne touche à rien dans les tables ! Le log ID par exemple est conservé
			//	tel quel. On va simplement exécuter "à la main" une série de UPDATE et
			//	si besoin de INSERT.
			
			for (int i = 0; i < this.adapters.Length; i++)
			{
				System.Data.DataTable data_table = this.data_set.Tables[i];
				
				int change_count = 0;
				int delete_count = 0;
				
				for (int r = 0; r < data_table.Rows.Count; r++)
				{
					System.Data.DataRow data_row = data_table.Rows[r];
					
					switch (data_row.RowState)
					{
						case System.Data.DataRowState.Added:
						case System.Data.DataRowState.Modified:
							change_count++;
							break;
						
						case System.Data.DataRowState.Deleted:
							delete_count++;
							break;
					}
				}
				
				if ((change_count > 0) ||
					(delete_count > 0))
				{
					this.ReplaceTable (transaction, data_table, this.Tables[i], options);
				}
				
				data_table.AcceptChanges ();
			}
		}

		public System.Data.DataRow CreateNewRow(string tableName)
		{
			System.Data.DataRow row;
			this.CreateNewRow (tableName, out row);
			return row;
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

		public System.Data.IDbTransaction GetActiveTransaction()
		{
			if (this.activeTransactions.Count == 0)
			{
				return null;
			}
			else
			{
				return this.activeTransactions.Peek ();
			}
		}
		
		public static System.Data.DataRow[] CopyLiveRows(System.Collections.IEnumerable rows)
		{
			List<System.Data.DataRow> list = new List<System.Data.DataRow> ();
			
			foreach (System.Data.DataRow row in rows)
			{
				if (DbRichCommand.IsRowLive (row))
				{
					list.Add (row);
				}
			}
			
			return list.ToArray ();
		}

		public static bool IsRowLive(System.Data.DataRow row)
		{
			return !DbRichCommand.IsRowDeleted (row);
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
			
			System.Diagnostics.Debug.WriteLine (string.Format ("Allocating {0} new IDs for table {1} starting at {2}.", list.Count, table.TableName, id));
			
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
		
		public static void UpdateLogIds(System.Data.DataTable table, DbId log_id)
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
		
		public static void DefineLogId(System.Data.DataRow row, DbId log_id)
		{
			if (row.RowState != System.Data.DataRowState.Deleted)
			{
				row.BeginEdit ();
				row[Tags.ColumnRefLog] = log_id.Value;
				row.EndEdit ();
			}
		}
		
		public static void CreateRow(System.Data.DataTable table, out System.Data.DataRow data_row)
		{
			//	Crée une ligne, mais ne l'ajoute pas à la table. L'ID affecté à la
			//	ligne est temporaire (mais unique); cf. DbKey.CheckTemporaryId.
			
			data_row = table.NewRow ();
			
			DbKey key = new DbKey (DbKey.CreateTemporaryId (), DbRowStatus.Live);
			
			data_row.BeginEdit ();
			data_row[Tags.ColumnId]     = key.Id.Value;
			data_row[Tags.ColumnStatus] = key.IntStatus;
			data_row.EndEdit ();
		}
		public static void CreateRow(System.Data.DataTable table, DbId log_id, out System.Data.DataRow data_row)
		{
			DbRichCommand.CreateRow (table, out data_row);
			DbRichCommand.DefineLogId (data_row, log_id);
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
		
		
		public static System.Data.DataRow CopyRowIfValid(System.Data.DataRow row)
		{
			if ((row == null) ||
				(row.Table == null))
			{
				return null;
			}
			
			switch (row.RowState)
			{
				case System.Data.DataRowState.Deleted:
				case System.Data.DataRowState.Detached:
					return null;
			}
			
			System.Data.DataRow copy = row.Table.NewRow ();
			
			copy.ItemArray = row.ItemArray;
			
			return copy;
		}
		
		public static System.Data.DataRow FindRow(System.Data.DataTable table, DbId id)
		{
			int n = table.Rows.Count;
			
			for (int i = 0; i < n; i++)
			{
				System.Data.DataRow row = table.Rows[i];
				long row_id;
				
				if (row.RowState == System.Data.DataRowState.Deleted)
				{
					row_id = (long) row[Tags.ColumnId, System.Data.DataRowVersion.Original];
				}
				else if (row.RowState == System.Data.DataRowState.Detached)
				{
					continue;
				}
				else
				{
					row_id = (long) row[Tags.ColumnId];
				}
						
				if (id.Value == row_id)
				{
					return row;
				}
			}
			
			return null;
		}
		
		public static System.Data.DataRow FindRow(System.Data.DataRow[] rows, DbId id)
		{
			int n = rows.Length;
			
			for (int i = 0; i < n; i++)
			{
				System.Data.DataRow row = rows[i];
				long row_id;
				
				if (row.RowState == System.Data.DataRowState.Deleted)
				{
					row_id = (long) row[Tags.ColumnId, System.Data.DataRowVersion.Original];
				}
				else if (row.RowState == System.Data.DataRowState.Detached)
				{
					continue;
				}
				else
				{
					row_id = (long) row[Tags.ColumnId];
				}
				
				if (id.Value == row_id)
				{
					return row;
				}
			}
			
			return null;
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
		
		public static void DebugDumpRow(System.Data.DataRow row)
		{
			System.Text.StringBuilder buffer = new System.Text.StringBuilder ();
			
			foreach (object o in row.ItemArray)
			{
				if (buffer.Length > 0)
				{
					buffer.Append (", ");
				}
				
				buffer.Append ("'");
				buffer.Append (o.ToString ());
				buffer.Append ("'");
			}
			
			System.Diagnostics.Debug.WriteLine (buffer.ToString ());
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

					System.Data.DataTable ado_target_table = this.data_set.Tables[fk.TargetTableName];

					if (ado_target_table == null)
					{
						//	La table cible n'est pas chargée dans le DataSet, ce qui veut dire
						//	que l'on doit ignorer la relation.
						
						continue;
					}
					
					System.Data.DataColumn[] ado_target_cols = new System.Data.DataColumn[n];
					System.Data.DataColumn[] ado_child_cols  = new System.Data.DataColumn[n];
					
					for (int j = 0; j < n; j++)
					{
						ado_child_cols[j]  = ado_child_table.Columns[fk.Columns[j].CreateDisplayName ()];
						ado_target_cols[j] = ado_target_table.Columns[fk.Columns[j].TargetColumnName];
					}

					System.Data.DataRelation relation = new System.Data.DataRelation (null, ado_target_cols, ado_child_cols);
					this.data_set.Relations.Add (relation);
					
					System.Data.ForeignKeyConstraint constraint = relation.ChildKeyConstraint;
					
					System.Diagnostics.Debug.Assert (constraint != null);
					System.Diagnostics.Debug.Assert (constraint.UpdateRule == System.Data.Rule.Cascade);
				}
			}
		}

		private void SetCommandTransaction(System.Data.IDbTransaction transaction)
		{
			if (transaction is DbTransaction)
			{
				transaction = ((DbTransaction) transaction).Transaction;
			}

			System.Diagnostics.Debug.Assert (transaction != null);

			for (int i = 0; i < this.commands.Count; i++)
			{
				this.commands[i].Transaction = transaction;
			}

			this.activeTransactions.Push (transaction);
		}

		private void PopCommandTransaction()
		{
			this.activeTransactions.Pop ();
			
			for (int i = 0; i < this.commands.Count; i++)
			{
				this.commands[i].Transaction = null;
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
		
		protected void ReplaceTable(DbTransaction transaction, System.Data.DataTable data_table, DbTable db_table, IReplaceOptions options)
		{
			string table_name = db_table.CreateSqlName ();
			
			IDbAbstraction database  = transaction.Database;
			ISqlBuilder    builder   = database.SqlBuilder;
			ITypeConverter converter = this.infrastructure.TypeConverter;
			
			Collections.SqlFields sql_update = new Collections.SqlFields ();
			Collections.SqlFields sql_insert = new Collections.SqlFields ();
			Collections.SqlFields sql_conds  = new Collections.SqlFields ();
			
			int col_count = db_table.Columns.Count;
			int row_count = data_table.Rows.Count;
			
			//	Crée pour chaque colonne de la table une représentation SQL et un
			//	champ qui permettra de stocker la valeur :
			
			SqlColumn[] sql_columns = new SqlColumn[col_count];
			
			int[]    update_map     = new int[col_count];
			object[] insert_default = new object[col_count];
			
			for (int c = 0; c < col_count; c++)
			{
				//	TODO: handle multiple cultures...
				DbColumn column = db_table.Columns[c];
				sql_columns[c] = column.CreateSqlColumn (converter, null);
				
				if ((options == null) ||
					(options.IgnoreColumn (c, column) == false))
				{
					//	Aucune option particulière pour cette colonne. Ajoute simplement
					//	la colonne à la fois pour le UPDATE et pour le INSERT :
					
					sql_update.Add (this.infrastructure.CreateEmptySqlField (column));
					sql_insert.Add (this.infrastructure.CreateEmptySqlField (column));
					
					update_map[c]     = c;
					insert_default[c] = null;
				}
				else
				{
					//	Les options indiquent que l'on doit ignorer cette colonne lors du
					//	UPDATE; on va aussi fournir une valeur par défaut pour le INSERT :
					
					sql_insert.Add (this.infrastructure.CreateEmptySqlField (column));
					
					update_map[c]     = -1;
					insert_default[c] = options.GetDefaultValue (c, column);
				}
			}
			
			//	Crée la condition pour le UPDATE ... WHERE CR_ID = n
			
			SqlField field_id_name  = SqlField.CreateName (table_name, sql_columns[0].Name);
			SqlField field_id_value = sql_update[0];
			
			sql_conds.Add (new SqlFunction (SqlFunctionType.CompareEqual, field_id_name, field_id_value));
			
			
			//	Crée les commandes pour le UPDATE et pour le INSERT :
			
			System.Data.IDbCommand update_command;
			System.Data.IDbCommand insert_command;
			System.Data.IDbCommand delete_command;
			
			builder.Clear ();
			builder.UpdateData (table_name, sql_update, sql_conds);
			
			update_command = builder.Command;
			
			builder.Clear ();
			builder.InsertData (table_name, sql_insert);
			
			insert_command = builder.Command;
			
			builder.Clear ();
			builder.RemoveData (table_name, sql_conds);
			
			delete_command = builder.Command;
			
			update_command.Transaction = transaction.Transaction;
			insert_command.Transaction = transaction.Transaction;
			delete_command.Transaction = transaction.Transaction;
			
			int param_id_index = update_command.Parameters.Count - 1;
			
			try
			{
				//	Passe en revue toutes les lignes de la table :
				
				for (int r = 0; r < row_count; r++)
				{
					System.Data.DataRow row = data_table.Rows[r];
					
					if ((row.RowState != System.Data.DataRowState.Added) &&
						(row.RowState != System.Data.DataRowState.Modified) &&
						(row.RowState != System.Data.DataRowState.Deleted))
					{
						continue;
					}
					
					if (row.RowState == System.Data.DataRowState.Deleted)
					{
						//	Supprime la ligne en question de la table; met juste à jour l'ID de
						//	la ligne dans la commande avant d'exécuter celle-ci :
						
						int    count;
						object value_id = sql_columns[0].ConvertToInternalType (row[0, System.Data.DataRowVersion.Original]);
						
						builder.SetCommandParameterValue (delete_command, 0, value_id);
						count = delete_command.ExecuteNonQuery ();
						
						this.stat_replace_delete_count += count;
					}
					else
					{
						//	Met à jour la ligne en question dans la table. Tente d'abord un UPDATE
						//	et en cas d'échec, recourt à INSERT.
						//	Commence par mettre à jour tous les paramètres des deux commandes :
						
						int    count;
						object value_id = sql_columns[0].ConvertToInternalType (row[0, System.Data.DataRowVersion.Current]);
						
						builder.SetCommandParameterValue (update_command, param_id_index, value_id);
						
						for (int c = 0; c < col_count; c++)
						{
							int map_c = update_map[c];
							
							//	La colonne peut-elle être utilisée telle quelle dans un UPDATE ?
							
							if (map_c < 0)
							{
								//	La colonne ne sera utilisée que pour le INSERT; dans ce cas
								//	il faudra utiliser une valeur par défaut en lieu et place de
								//	la valeur proposée dans la source :
								
								builder.SetCommandParameterValue (insert_command, c, insert_default[c]);
							}
							else
							{
								object value = sql_columns[c].ConvertToInternalType (row[c]);
								
								builder.SetCommandParameterValue (update_command, map_c, value);
								builder.SetCommandParameterValue (insert_command, c, value);
							}
						}
						
						count = update_command.ExecuteNonQuery ();
						
						if (count == 0)
						{
							//	Le UPDATE n'a modifié aucune ligne dans la base de données; cela signifie que la
							//	ligne n'était pas connue. On va donc procéder à son insertion :
							
							count = insert_command.ExecuteNonQuery ();
							
							if (count != 1)
							{
								throw new Exceptions.FormatException (string.Format ("Insert into table {0} produced {1} changes (ID = {2}). Expected exactly 1.", table_name, count, insert_command.Parameters[0]));
							}
							
							this.stat_replace_insert_count++;
						}
						else if (count == 1)
						{
							this.stat_replace_update_count++;
						}
						else
						{
							throw new Exceptions.FormatException (string.Format ("Update of table {0} produced {1} changes (ID = {2}). Expected 0 or 1.", table_name, count, update_command.Parameters[0]));
						}
					}
				}
			}
			finally
			{
				delete_command.Dispose ();
				update_command.Dispose ();
				insert_command.Dispose ();
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
					
					this.adapters[i].MissingSchemaAction = System.Data.MissingSchemaAction.AddWithKey;
					this.adapters[i].Fill (this.data_set);
				}
			}
			finally
			{
				this.PopCommandTransaction ();
			}
			
			this.CreateDataRelations ();
		}
		
		
		#region IDisposable Members
		public void Dispose()
		{
			this.Dispose (true);
		}
		#endregion
		
		#region IReplaceOptions Interface
		public interface IReplaceOptions
		{
			bool IgnoreColumn(int index, DbColumn column);
			object GetDefaultValue(int index, DbColumn column);
		}
		#endregion
		
		#region ReplaceIgnoreColumns Class
		public class ReplaceIgnoreColumns : IReplaceOptions
		{
			public ReplaceIgnoreColumns()
			{
				this.columns = new System.Collections.Hashtable ();
			}
			
			
			public void AddIgnoreColumn(string name, object default_value)
			{
				this.columns[name] = default_value;
			}
			
			
			#region IReplaceOptions Members
			public bool IgnoreColumn(int index, DbColumn column)
			{
				return this.columns.ContainsKey (column.Name);
			}
			
			public object GetDefaultValue(int index, DbColumn column)
			{
				return this.columns[column.Name];
			}
			#endregion
			
			System.Collections.Hashtable		columns;
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

		Stack<System.Data.IDbTransaction>		activeTransactions = new Stack<System.Data.IDbTransaction> ();
		
		private bool							is_read_only;
		private int								stat_replace_update_count;
		private int								stat_replace_insert_count;
		private int								stat_replace_delete_count;
	}
}
