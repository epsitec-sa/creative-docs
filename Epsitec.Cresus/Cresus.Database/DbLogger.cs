//	Copyright © 2004-2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;

using Epsitec.Common.Types;

namespace Epsitec.Cresus.Database
{
	/// <summary>
	/// The <c>DbLogger</c> class manages the modification log (stored in a "CR_LOG"
	/// table, in the database).
	/// </summary>
	
	public sealed class DbLogger : IAttachable
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="DbLogger"/> class.
		/// </summary>
		public DbLogger()
		{
		}


		/// <summary>
		/// Gets the current id used to tag actions in the log.
		/// </summary>
		/// <value>The current id.</value>
		public DbId								CurrentId
		{
			get
			{
				if ((this.clientId != -1) &&
					(this.nextId != -1))
				{
					return this.currentId;
				}
				
				throw new System.InvalidOperationException ("DbLogger not initialised");
			}
		}


		/// <summary>
		/// Defines the client id.
		/// </summary>
		/// <param name="clientId">The client id.</param>
		internal void DefineClientId(int clientId)
		{
			System.Diagnostics.Debug.Assert (this.clientId == -1);
			System.Diagnostics.Debug.Assert (this.nextId == -1);
			System.Diagnostics.Debug.Assert (this.infrastructure == null);
			
			this.clientId = clientId;
		}

		/// <summary>
		/// Defines the initial log id.
		/// </summary>
		/// <param name="logId">The log id.</param>
		internal void DefineInitialLogId(long logId)
		{
			System.Diagnostics.Debug.Assert (this.clientId != -1);
			System.Diagnostics.Debug.Assert (this.nextId == -1);
			System.Diagnostics.Debug.Assert (this.infrastructure == null);
			System.Diagnostics.Debug.Assert (logId > 0);
			System.Diagnostics.Debug.Assert (logId < DbId.LocalRange);
			
			this.currentId = DbId.CreateId (logId, this.clientId);
			this.nextId    = logId + 1;
		}


		/// <summary>
		/// Resets the current log id based on the log found in the database.
		/// </summary>
		/// <param name="transaction">The transaction.</param>
		internal void ResetCurrentLogId(DbTransaction transaction)
		{
			System.Diagnostics.Debug.Assert (this.clientId != -1);
			System.Diagnostics.Debug.Assert (this.nextId == -1);
			System.Diagnostics.Debug.Assert (this.infrastructure != null);
			System.Diagnostics.Debug.Assert (this.table != null);
			
			//	L'identificateur local stocké dans la base correspond toujours à celui de la
			//	prochaine ligne à créer, mais l'identificateur de client correspond au dernier
			//	enregistré dans le LOG. Ainsi, id.ClientId n'est pas nécessairement égal à
			//	l'identificateur de client actif.
			
			DbId id = this.infrastructure.NextRowIdInTable (transaction, this.tableKey);
			
			this.nextId    = id.LocalId;
			this.currentId = DbId.CreateId (this.nextId - 1, id.ClientId);
			
			System.Diagnostics.Debug.Assert (this.nextId > 0);
			System.Diagnostics.Debug.Assert (this.nextId < DbId.LocalRange);
		}

		/// <summary>
		/// Creates the initial log entry.
		/// </summary>
		/// <param name="transaction">The transaction.</param>
		internal void CreateInitialEntry(DbTransaction transaction)
		{
			this.Insert (transaction, new DbLogEntry (DbId.CreateId (1, this.clientId)));
		}


		/// <summary>
		/// Creates a permanent entry into the log.
		/// </summary>
		/// <param name="transaction">The transaction.</param>
		/// <returns>The id for the log entry.</returns>
		public DbId CreatePermanentEntry(DbTransaction transaction)
		{
			lock (this.exclusion)
			{
				return this.Insert (transaction, new DbLogEntry (DbId.CreateId (this.nextId, this.clientId)));
			}
		}

		/// <summary>
		/// Creates a temporary entry into the log.
		/// </summary>
		/// <param name="transaction">The transaction.</param>
		/// <returns>The id for the log entry.</returns>
		public DbId CreateTemporaryEntry(DbTransaction transaction)
		{
			lock (this.exclusion)
			{
				return this.Insert (transaction, new DbLogEntry (DbId.CreateTempId (this.nextId)));
			}
		}


		/// <summary>
		/// Removes the specified entry from the log.
		/// </summary>
		/// <param name="transaction">The transaction.</param>
		/// <param name="id">The id of the entry.</param>
		/// <returns><c>true</c> if the entry was removed; otherwise, <c>false</c>.</returns>
		public bool Remove(DbTransaction transaction, DbId id)
		{
			if (transaction == null)
			{
				try
				{
					transaction = this.infrastructure.BeginTransaction (DbTransactionMode.ReadWrite);
					return this.Remove (transaction, id);
				}
				finally
				{
					transaction.Commit ();
					transaction.Dispose ();
				}
			}
			else
			{
				Collections.SqlFields conditions = new Collections.SqlFields ();
				
				SqlField logIdName  = SqlField.CreateName (this.tableSqlName, Tags.ColumnId);
				SqlField logIdValue = SqlField.CreateConstant (id.Value, DbKey.RawTypeForId);
				
				conditions.Add (new SqlFunction (SqlFunctionCode.CompareEqual, logIdName, logIdValue));
				
				transaction.SqlBuilder.RemoveData (this.table.CreateSqlName (), conditions);
				int result = (int) this.infrastructure.ExecuteNonQuery (transaction);
				
				return 1 == result;
			}
		}

		/// <summary>
		/// Removes a range of consecutive entries, starting at the specified entry
		/// up to the end of the log.
		/// </summary>
		/// <param name="transaction">The transaction.</param>
		/// <param name="startId">The start id.</param>
		public void RemoveRange(DbTransaction transaction, DbId startId)
		{
			this.RemoveRange (transaction, startId, DbId.CreateId (DbId.LocalRange - 1, startId.ClientId));
		}

		/// <summary>
		/// Removes a range of consecutive entries, from start to stop, including
		/// the start and stop entries.
		/// </summary>
		/// <param name="transaction">The transaction.</param>
		/// <param name="startId">The start id.</param>
		/// <param name="endId">The end id.</param>
		public void RemoveRange(DbTransaction transaction, DbId startId, DbId endId)
		{
			if (transaction == null)
			{
				try
				{
					transaction = this.infrastructure.BeginTransaction (DbTransactionMode.ReadWrite);
					this.RemoveRange (transaction, startId, endId);
				}
				finally
				{
					transaction.Commit ();
					transaction.Dispose ();
				}
			}
			else
			{
				Collections.SqlFields conditions = new Collections.SqlFields ();
				
				SqlField logIdName = SqlField.CreateName (this.tableSqlName, Tags.ColumnId);
				SqlField logIdVal1 = SqlField.CreateConstant (startId.Value, DbKey.RawTypeForId);
				SqlField logIdVal2 = SqlField.CreateConstant (endId.Value, DbKey.RawTypeForId);
				
				conditions.Add (new SqlFunction (SqlFunctionCode.CompareGreaterThanOrEqual, logIdName, logIdVal1));
				conditions.Add (new SqlFunction (SqlFunctionCode.CompareLessThanOrEqual, logIdName, logIdVal2));
				
				transaction.SqlBuilder.RemoveData (this.table.CreateSqlName (), conditions);
				this.infrastructure.ExecuteSilent (transaction);
			}
		}


		/// <summary>
		/// Finds the specified log entries with ids between start and end, including
		/// start and end.
		/// </summary>
		/// <param name="transaction">The transaction.</param>
		/// <param name="startId">The start id.</param>
		/// <param name="endId">The end id.</param>
		/// <returns>The log entries.</returns>
		public DbLogEntry[] Find(DbTransaction transaction, DbId startId, DbId endId)
		{
			if (transaction == null)
			{
				try
				{
					transaction = this.infrastructure.BeginTransaction (DbTransactionMode.ReadWrite);
					return this.Find (transaction, startId, endId);
				}
				finally
				{
					transaction.Commit ();
					transaction.Dispose ();
				}
			}
			else
			{
				SqlField logIdName = SqlField.CreateName ("T", Tags.ColumnId);
				SqlField logIdVal1 = SqlField.CreateConstant (startId.Value, DbKey.RawTypeForId);
				SqlField logIdVal2 = SqlField.CreateConstant (endId.Value, DbKey.RawTypeForId);
				
				SqlSelect query = new SqlSelect ();
				
				query.Fields.Add ("T_ID", SqlField.CreateName ("T", Tags.ColumnId));
				query.Fields.Add ("T_DT", SqlField.CreateName ("T", Tags.ColumnDateTime));
				
				query.Tables.Add ("T", SqlField.CreateName (this.tableSqlName));
				
				query.Conditions.Add (new SqlFunction (SqlFunctionCode.CompareGreaterThanOrEqual, logIdName, logIdVal1));
				query.Conditions.Add (new SqlFunction (SqlFunctionCode.CompareLessThanOrEqual, logIdName, logIdVal2));
				
				System.Data.DataTable table = this.infrastructure.ExecuteSqlSelect (transaction, query, 0);
				
				int n = table.Rows.Count;
				DbLogEntry[] entries = new DbLogEntry[n];
				
				for (int i = 0; i < n; i++)
				{
					System.Data.DataRow row = table.Rows[i];
					
					long            logId    = InvariantConverter.ToLong (row["T_ID"]);
					System.DateTime dateTime = InvariantConverter.ToDateTime (row["T_DT"]);
					
					entries[i] = new DbLogEntry (new DbId (logId), dateTime);
				}
				
				return entries;
			}
		}
		
		
		#region IAttachable Members

		/// <summary>
		/// Attaches this instance to the specified database table.
		/// </summary>
		/// <param name="infrastructure">The infrastructure.</param>
		/// <param name="table">The database table.</param>
		public void Attach(DbInfrastructure infrastructure, DbTable table)
		{
			this.infrastructure = infrastructure;
			this.table          = table;
			this.tableKey       = table.Key;
			this.tableSqlName   = table.CreateSqlName ();
		}

		/// <summary>
		/// Detaches this instance from the database.
		/// </summary>
		public void Detach()
		{
			this.infrastructure = null;
			this.table          = null;
		}
		
		#endregion
		
		private DbId Insert(DbTransaction transaction, DbLogEntry entry)
		{
			if (transaction == null)
			{
				try
				{
					transaction = this.infrastructure.BeginTransaction (DbTransactionMode.ReadWrite);
					return this.Insert (transaction, entry);
				}
				finally
				{
					transaction.Commit ();
					transaction.Dispose ();
				}
			}
			else
			{
				Collections.SqlFields fields = new Collections.SqlFields ();

				fields.Add (this.infrastructure.CreateSqlField (this.table.Columns[Tags.ColumnId], entry.Id));
				fields.Add (this.infrastructure.CreateSqlField (this.table.Columns[Tags.ColumnDateTime], entry.DateTime));

				long nextId = entry.Id.LocalId + 1;

				transaction.SqlBuilder.InsertData (this.tableSqlName, fields);
				this.infrastructure.ExecuteSilent (transaction);

				//	Enregistre dans la base le prochain ID à utiliser, en prenant note du
				//	ClientId appliqué à l'élément que l'on vient d'enregistrer dans le LOG :

				this.infrastructure.UpdateTableNextId (transaction, this.tableKey, DbId.CreateId (nextId, entry.Id.ClientId));

				this.nextId    = nextId;
				this.currentId = entry.Id;
				
				return entry.Id;
			}
		}
		
		
		private DbInfrastructure				infrastructure;
		private DbTable							table;
		private DbKey							tableKey;
		private string							tableSqlName;

		private object							exclusion = new object ();
		
		private int								clientId  = -1;
		private long							nextId    = -1;
		private DbId							currentId;
	}
}
