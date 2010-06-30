//	Copyright © 2004-2009, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Cresus.Database;

using Epsitec.Cresus.Remoting;

using System.Collections.Generic;
using System.Linq;


namespace Epsitec.Cresus.Replication
{


	/// <summary>
	/// The <c>ClientReplicationEngine</c> class runs on the client side and knows how
	/// to apply changes received from the server to replicate a data set.
	/// </summary>
	public sealed class ClientReplicationEngine
	{


		/// <summary>
		/// Initializes a new instance of the <see cref="ClientReplicationEngine"/> class.
		/// </summary>
		/// <param name="infrastructure">The infrastructure.</param>
		/// <param name="service">The service.</param>
		public ClientReplicationEngine(DbInfrastructure infrastructure, IReplicationService service)
		{
			this.infrastructure = infrastructure;
			this.replicationService = service;
			this.largestLogId = DbId.Invalid;
		}


		/// <summary>
		/// Gets the largest server log id.
		/// </summary>
		/// <value>The largest server log id.</value>
		public DbId								LargestServerLogId
		{
			get
			{
				return this.largestLogId;
			}
		}


		/// <summary>
		/// Applies the changes associated with a given operation.
		/// </summary>
		/// <param name="database">The database.</param>
		/// <param name="operationId">The operation id.</param>
		public void ApplyChanges(IDbAbstraction database, long operationId)
		{
			this.ApplyChanges (database, operationId, null);
		}


		/// <summary>
		/// Applies the changes associated with a given operation.
		/// </summary>
		/// <param name="database">The database.</param>
		/// <param name="operationId">The operation id.</param>
		/// <param name="beforeCommitCallback">The callback to call before commit.</param>
		public void ApplyChanges(IDbAbstraction database, long operationId, System.Action<ClientReplicationEngine, DbTransaction> beforeCommitCallback)
		{
			byte[] compressedData = this.replicationService.GetReplicationData (operationId);

			this.ApplyChanges (database, Common.IO.Serialization.DeserializeAndDecompressFromMemory<ReplicationData> (compressedData),
				transaction => 
				{
					if (beforeCommitCallback != null)
					{
						beforeCommitCallback (this, transaction);
					}
				}
			);
		}


		/// <summary>
		/// Applies the changes.
		/// </summary>
		/// <param name="database">The database.</param>
		/// <param name="data">The replication data.</param>
		/// <param name="notifyBeforeCommitCallback">The callback to notify before commit.</param>
		private void ApplyChanges(IDbAbstraction database, ReplicationData data, System.Action<DbTransaction> notifyBeforeCommitCallback)
		{
			if (data == null)
			{
				return;
			}

			List<PackedTableData> list = new List<PackedTableData> ();
			
			list.AddRange (data.PackedTableData);

			PackedTableData defTable   = list.FirstOrDefault (t => t.Name == Tags.TableTableDef);
			PackedTableData defColumn  = list.FirstOrDefault (t => t.Name == Tags.TableColumnDef);
			PackedTableData defType    = list.FirstOrDefault (t => t.Name == Tags.TableTypeDef);
			PackedTableData logTable   = list.FirstOrDefault (t => t.Name == Tags.TableLog);
			
			if (logTable != null)
			{
				list.Remove (logTable);
			}
			
			//	First apply the changes to the internal schema tables (if any), before we try to apply
			//	any other changes :
			
			if (defTable != null || defColumn != null || defType != null)
			{
				//	We need to globally lock the database to be able to do these changes...
				
				using (this.infrastructure.GlobalLock ())
				{
					using (DbTransaction transaction = infrastructure.BeginTransaction (DbTransactionMode.ReadWrite, database))
					{
						this.ApplyChanges (transaction, defTable);
						this.ApplyChanges (transaction, defColumn);
						this.ApplyChanges (transaction, defType);
						
						//	Make sure we validate the transaction since the changes we have just applied
						//	might really change the schema of the database !
						
						this.infrastructure.ClearCaches ();
						
						transaction.Commit ();
					}
					
					list.Remove (defTable);
					list.Remove (defColumn);
					list.Remove (defType);
					
					//	Now, do the low level structural changes in the database. This will modify the
					//	underlying tables.
					
					this.ApplyStructuralChanges (database, defTable, defColumn, defType);
					
					using (DbTransaction transaction = infrastructure.BeginTransaction (DbTransactionMode.ReadWrite, database))
					{
						this.ApplyLogChanges (transaction, logTable);
						this.ApplyChanges (transaction, list);
						this.UpdateSyncLogId (transaction);
						notifyBeforeCommitCallback (transaction);
						transaction.Commit ();
					}
				}
			}
			else
			{
				using (DbTransaction transaction = infrastructure.BeginTransaction (DbTransactionMode.ReadWrite, database))
				{
					this.ApplyLogChanges (transaction, logTable);
					this.ApplyChanges (transaction, list);
					this.UpdateSyncLogId (transaction);
					notifyBeforeCommitCallback (transaction);
					transaction.Commit ();
				}
			}
		}

		/// <summary>
		/// Applies the changes.
		/// </summary>
		/// <param name="transaction">The transaction.</param>
		/// <param name="list">The list of changed table data.</param>
		private void ApplyChanges(DbTransaction transaction, IEnumerable<PackedTableData> list)
		{
			foreach (PackedTableData data in list)
			{
				this.ApplyChanges (transaction, data);
			}
		}

		/// <summary>
		/// Applies the changes.
		/// </summary>
		/// <param name="transaction">The transaction.</param>
		/// <param name="data">The changed table data.</param>
		private void ApplyChanges(DbTransaction transaction, PackedTableData data)
		{
			if (data == null)
			{
				return;
			}

			//	Applique les modifications décrites pour la table spécifiée. Pour ce faire,
			//	on remplit une table avec les lignes à répliquer et on utilise un 'REPLACE'
			//	de toutes celles-ci :
			
			DbTable table = this.infrastructure.ResolveDbTable (transaction, new DbKey (data.Key));
			
			using (DbRichCommand command = DbRichCommand.CreateFromTable (this.infrastructure, transaction, table))
			{
				System.Diagnostics.Debug.Assert (command.DataSet != null);
				System.Diagnostics.Debug.Assert (command.DataSet.Tables.Count == 1);

				System.Data.DataTable dataTable = command.DataTable;
				
				//	The data from the replication engine will be written to the local table;
				//	duplicate rows will be overwritten and new rows will be added.
				
				data.FillTable (dataTable);
				
				System.Diagnostics.Debug.Assert (dataTable.Rows.Count > 0);
				
				if (dataTable.TableName == Tags.TableTableDef)
				{
					//	Beware : we may not simply replicate the CR_TABLE_DEF table; its CR_NEXT_ID
					//	column must be reset to a proper local value :
					
					Database.Options.ReplaceIgnoreColumns options = new Database.Options.ReplaceIgnoreColumns ();
					DbId nextId = DbId.CreateId (1, this.infrastructure.LocalSettings.ClientId);
					
					options.AddIgnoreColumn (Tags.ColumnNextId, nextId.Value);
					
					command.ReplaceTables (transaction, options);
				}
				else
				{
					command.ReplaceTables (transaction);
				}
			}
		}

		/// <summary>
		/// Applies the log changes.
		/// </summary>
		/// <param name="transaction">The transaction.</param>
		/// <param name="data">The changed table data.</param>
		private void ApplyLogChanges(DbTransaction transaction, PackedTableData data)
		{
			if (data == null)
			{
				return;
			}
			
			DbTable table = this.infrastructure.ResolveDbTable (transaction, new DbKey (data.Key));
			
			using (DbRichCommand command = DbRichCommand.CreateFromTable (this.infrastructure, transaction, table))
			{
				System.Diagnostics.Debug.Assert (command.DataSet != null);
				System.Diagnostics.Debug.Assert (command.DataSet.Tables.Count == 1);

				System.Data.DataTable dataTable = command.DataTable;
				
				data.FillTable (dataTable);

				//	The data from the replication engine will be written to the local table;
				//	duplicate rows will be overwritten and new rows will be added.

				System.Diagnostics.Debug.Assert (dataTable.Rows.Count > 0);
				
				command.ReplaceTablesWithoutValidityChecking (transaction, null);
				
				System.Diagnostics.Debug.WriteLine (string.Format ("Replicated {0} lines from CR_LOG.", dataTable.Rows.Count));

				//	Find the largest server log id and remember it; we can start from there the
				//	next time we need to replicate.
				
				foreach (System.Data.DataRow row in dataTable.Rows)
				{
					DbId id = new DbId ((long) row[0]);
					
					if ((id.IsServer) &&
						(id.LocalId > this.largestLogId.LocalId))
					{
						this.largestLogId = id;
					}
				}
			}
		}

		/// <summary>
		/// Applies structural changes to the underlying database : adding tables, columns,
		/// changing their type, etc.
		/// </summary>
		/// <param name="database">The database.</param>
		/// <param name="defTable">The table definitions.</param>
		/// <param name="defColumn">The column definitions.</param>
		/// <param name="defType">The type definitions.</param>
		private void ApplyStructuralChanges(IDbAbstraction database, PackedTableData defTable, PackedTableData defColumn, PackedTableData defType)
		{
			//	Currently, only the creation of new tables is handled here.

			//	TODO: handle changes to existing tables
			
			if (defTable != null)
			{
				object[][] defTableRows = defTable.GetAllValues ();
				
				//	Let's see what changed in CR_TABLE_DEF :
				
				foreach (var defTableRow in defTableRows)
				{
					DbKey   defTableRowKey  = new DbKey (defTableRow);
					DbTable defTableRuntime = this.infrastructure.ResolveDbTable (defTableRowKey);
					
					System.Diagnostics.Debug.Assert (defTableRuntime != null);
					
					if (defTableRowKey.Status == DbRowStatus.Deleted)
					{
						//	The table was deleted; since we never really delete anything in the database,
						//	there is nothing more to do here.
						
						System.Diagnostics.Debug.WriteLine (string.Format ("Replication: table {0} was deleted.", defTableRuntime.Name));
					}
					else
					{
						//	The table was modified, maybe created.
						
						string   findSqlName    = defTableRuntime.GetSqlName ();
						string[] knownSqlTables = database.QueryUserTableNames ();
						
						bool found = false;
						
						foreach (string name in knownSqlTables)
						{
							if (name == findSqlName)
							{
								found = true;
								break;
							}
						}
						
						if (found)
						{
							System.Diagnostics.Debug.WriteLine (string.Format ("Replication: table {0} was modified. SQL Name is {1}.", defTableRuntime.Name, findSqlName));
							
							//	TODO: handle updating a table

							throw new System.NotImplementedException ();
						}
						else
						{
							System.Diagnostics.Debug.WriteLine (string.Format ("Replication: table {0} was created. SQL Name is {1}.", defTableRuntime.Name, findSqlName));
							
							//	The table has to be created; we need not create/update the schema definitions
							//	at our level, since the corresponding tables will be automatically updated in
							//	the replication process :
							
							using (DbTransaction transaction = this.infrastructure.BeginTransaction (DbTransactionMode.ReadWrite, database))
							{
								this.infrastructure.RegisterKnownDbTable (transaction, defTableRuntime);
								
								transaction.Commit ();
							}
						}
					}
				}
			}
		}

		/// <summary>
		/// Updates the sync log id in the local settings of the database.
		/// </summary>
		/// <param name="transaction">The active transaction.</param>
		private void UpdateSyncLogId(DbTransaction transaction)
		{
			if (this.LargestServerLogId.IsValid)
			{
				this.infrastructure.LocalSettings.SyncLogId = this.LargestServerLogId;
				this.infrastructure.LocalSettings.PersistToBase (transaction);
				
				System.Diagnostics.Debug.WriteLine ("Persisted SyncLogId.");
			}
		}
		
		
		readonly DbInfrastructure				infrastructure;
		readonly IReplicationService	replicationService;
		
		private DbId							largestLogId;
	}
}
