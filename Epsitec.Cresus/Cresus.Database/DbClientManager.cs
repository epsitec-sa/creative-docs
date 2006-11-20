//	Copyright © 2004-2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;

using Epsitec.Common.Types;

namespace Epsitec.Cresus.Database
{
	/// <summary>
	/// The <c>DbClientManager</c> class manages the table of known clients (stored
	/// in the database as CR_CLIENT_DEF).
	/// </summary>
	public sealed class DbClientManager : IAttachable, IPersistable
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="DbClientManager"/> class.
		/// </summary>
		public DbClientManager()
		{
		}

		/// <summary>
		/// Finds a free client id.
		/// </summary>
		/// <returns>A free client id.</returns>
		public int FindFreeClientId()
		{
			DbClientRecord[] records = this.FindAllClients ();
			int n = records.Length;
			
			if (n == 0)
			{
				return DbClientManager.MinClientId;
			}
			
			//	Create a sorted table of all known client ids :
			
			int[] usedIds = new int[n];
			
			for (int i = 0; i < n; i++)
			{
				usedIds[i] = records[i].ClientId;
			}
			
			System.Array.Sort (usedIds);
			
			//	Find the first hole in the current client id numbering sequence
			//	and re-use it :
			
			int newId = DbClientManager.MinClientId;
			
			for (int i = 0; i < n; i++)
			{
				if (newId < usedIds[i])
				{
					return newId;
				}
				
				System.Diagnostics.Debug.Assert (newId == usedIds[i]);

				newId++;
			}

			//	There was no hole in the id numbering sequence; allocate a fresh
			//	client id...
			
			if (newId > DbClientManager.MaxClientId)
			{
				throw new Exceptions.InvalidIdException ("No more client IDs available.");
			}
			
			return newId;
		}

		/// <summary>
		/// Creates a new client record.
		/// </summary>
		/// <param name="clientName">Name of the client.</param>
		/// <returns>The client record.</returns>
		public DbClientRecord CreateNewClient(string clientName)
		{
			int  clientId  = this.FindFreeClientId ();
			long syncLogId = 0;

			return new DbClientRecord (clientName, clientId, syncLogId);
		}

		/// <summary>
		/// Creates a new client record and inserts it into the database.
		/// </summary>
		/// <param name="clientName">Name of the client.</param>
		/// <returns>The client record.</returns>
		public DbClientRecord CreateAndInsertNewClient(string clientName)
		{
			int clientId   = this.FindFreeClientId ();
			long syncLogId = 0;

			DbClientRecord record = new DbClientRecord (clientName, clientId, syncLogId);

			this.Insert (record);

			return record;
		}

		/// <summary>
		/// Inserts the specified client record into the database.
		/// </summary>
		/// <param name="record">The client record.</param>
		public void Insert(DbClientRecord record)
		{
			if (this.richCommand == null)
			{
				throw new Exceptions.InvalidIdException ("Cannot insert client: call LoadFromBase first");
			}
			
			//	First, make sure that we don't create a duplicate client
			//	record :
			
			System.Data.DataTable table = this.richCommand.DataSet.Tables[0];
			
			foreach (System.Data.DataRow row in table.Rows)
			{
				if (DbRichCommand.IsRowDeleted (row))
				{
					continue;
				}

				int clientId = InvariantConverter.ToInt (row[Tags.ColumnClientId]);

				if (clientId == record.ClientId)
				{
					throw new Exceptions.ExistsException ("Cannot insert same client twice");
				}
			}
			
			//	Fill in a new row with the client record contents :
			
			record.SaveToEmptyRow (this.richCommand.CreateRow (table.TableName));
		}

		/// <summary>
		/// Updates the specified record in the database table.
		/// </summary>
		/// <param name="record">The record.</param>
		public void Update(DbClientRecord record)
		{
			if (this.richCommand == null)
			{
				throw new Exceptions.InvalidIdException ("Cannot update client: call LoadFromBase first");
			}
			
			System.Data.DataTable table = this.richCommand.DataSet.Tables[0];
			
			foreach (System.Data.DataRow row in table.Rows)
			{
				if (DbRichCommand.IsRowDeleted (row))
				{
					continue;
				}
				
				int clientId = InvariantConverter.ToInt (row[Tags.ColumnClientId]);

				if (clientId == record.ClientId)
				{
					record.UpdateRow (row);
					return;
				}
			}

			throw new Exceptions.InvalidIdException ("Cannot find client record");
		}

		/// <summary>
		/// Removes the specified record from the database table.
		/// </summary>
		/// <param name="recordClientId">The record client id.</param>
		public void Remove(int recordClientId)
		{
			if (this.richCommand == null)
			{
				throw new Exceptions.InvalidIdException ("Cannot remove client: call LoadFromBase first");
			}
			
			System.Data.DataTable table = this.richCommand.DataSet.Tables[0];
			
			foreach (System.Data.DataRow row in table.Rows)
			{
				if (DbRichCommand.IsRowDeleted (row))
				{
					continue;
				}

				int clientId = InvariantConverter.ToInt (row[Tags.ColumnClientId]);

				if (clientId == recordClientId)
				{
					this.richCommand.DeleteExistingRow (row);
					return;
				}
			}

			throw new Exceptions.InvalidIdException ("Cannot find client record");
		}

		/// <summary>
		/// Finds all client records.
		/// </summary>
		/// <returns>The client records.</returns>
		public DbClientRecord[] FindAllClients()
		{
			if (this.richCommand == null)
			{
				throw new Exceptions.InvalidIdException ("Cannot find clients: call LoadFromBase first");
			}
			
			System.Data.DataTable table = this.richCommand.DataSet.Tables[0];
			List<DbClientRecord>  list  = new List<DbClientRecord> ();
			
			foreach (System.Data.DataRow row in table.Rows)
			{
				if (DbRichCommand.IsRowDeleted (row))
				{
					continue;
				}

				DbClientRecord record = new DbClientRecord ();
				record.LoadFromRow (row);
				list.Add (record);
			}
			
			return list.ToArray ();
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
			this.tableSqlName   = table.GetSqlName ();
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
		
		#region IPersistable Members

		/// <summary>
		/// Persists the instance data to the database.
		/// </summary>
		/// <param name="transaction">The transaction.</param>
		public void PersistToBase(DbTransaction transaction)
		{
			if (this.richCommand != null)
			{
				this.richCommand.UpdateLogIds ();
				this.richCommand.AssignRealRowIds (transaction);
				this.richCommand.UpdateTables (transaction);
			}
		}

		/// <summary>
		/// Loads the instance data from the database.
		/// </summary>
		/// <param name="transaction">The transaction.</param>
		public void LoadFromBase(DbTransaction transaction)
		{
			if (this.richCommand != null)
			{
				this.richCommand.Dispose ();
				this.richCommand = null;
			}
			
			this.richCommand = DbRichCommand.CreateFromTable (this.infrastructure, transaction, this.table);
		}
		
		#endregion
		
		private DbInfrastructure				infrastructure;
		private DbTable							table;
		private DbKey							tableKey;
		private string							tableSqlName;
		private DbRichCommand					richCommand;
		
		public const int						MinClientId		= 1000;
		public const int						MaxClientId		= 1999;
	}
}
