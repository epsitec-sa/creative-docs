//	Copyright © 2004-2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

namespace Epsitec.Cresus.Database
{
	/// <summary>
	/// The <c>DbClientManager</c> class manages the table of known clients (stored
	/// in the database as CR_CLIENT_DEF).
	/// </summary>
	public sealed class DbClientManager : IAttachable, IPersistable
	{
		public DbClientManager()
		{
		}
		
		
		public int FindFreeClientId()
		{
			Entry[] entries  = this.FindAllClients ();
			int     n        = entries.Length;
			
			if (n == 0)
			{
				return DbClientManager.MinClientId;
			}
			
			int[] usedIds = new int[n];
			
			for (int i = 0; i < n; i++)
			{
				usedIds[i] = entries[i].ClientId;
			}
			
			System.Array.Sort (usedIds);
			
			int newId = DbClientManager.MinClientId;
			
			for (int i = 0; i < n; i++)
			{
				if (newId < usedIds[i])
				{
					return newId;
				}
				newId++;
			}
			
			if (newId > DbClientManager.MaxClientId)
			{
				throw new Exceptions.InvalidIdException ("No more client IDs available.");
			}
			
			return newId;
		}


		public DbClientManager.Entry CreateNewClient(string clientName)
		{
			int clientId   = this.FindFreeClientId ();
			long syncLogId = 0;

			Entry entry = new Entry (clientName, clientId, syncLogId);
			
			return entry;
		}

		public DbClientManager.Entry CreateAndInsertNewClient(string clientName)
		{
			int clientId   = this.FindFreeClientId ();
			long syncLogId = 0;

			Entry entry = new Entry (clientName, clientId, syncLogId);
			
			this.Insert (entry);
			
			return entry;
		}

		
		public void Insert(DbClientManager.Entry entry)
		{
			if (this.richCommand == null)
			{
				throw new Exceptions.InvalidIdException ("Cannot insert client: call RestoreFromBase first.");
			}
			
			System.Data.DataTable table = this.richCommand.DataSet.Tables[0];
			System.Data.DataRow   row;
			
			for (int i = 0; i < table.Rows.Count; i++)
			{
				row = table.Rows[i];
				
				if (DbRichCommand.IsRowDeleted (row))
				{
					continue;
				}

				int clientId;

				Common.Types.InvariantConverter.Convert (row[Tags.ColumnClientId], out clientId);

				if (clientId == entry.ClientId)
				{
					throw new Exceptions.ExistsException ("Cannot insert same client twice.");
				}
			}
			
			this.richCommand.CreateNewRow (table.TableName, out row);
			
			row.BeginEdit ();
			row[Tags.ColumnClientName]    = entry.ClientName;
			row[Tags.ColumnClientId]      = entry.ClientId;
			row[Tags.ColumnClientSync]    = entry.SyncLogId;
			row[Tags.ColumnClientCreDate] = entry.CreationDateTime;
			row[Tags.ColumnClientConDate] = entry.ConnectionDateTime;
			row.EndEdit ();
		}
		
		public void Update(DbClientManager.Entry entry)
		{
			if (this.richCommand == null)
			{
				throw new Exceptions.InvalidIdException ("Cannot update client: call RestoreFromBase first.");
			}
			
			System.Data.DataTable table = this.richCommand.DataSet.Tables[0];
			
			for (int i = 0; i < table.Rows.Count; i++)
			{
				System.Data.DataRow row = table.Rows[i];
				
				if (DbRichCommand.IsRowDeleted (row))
				{
					continue;
				}

				int clientId;

				Common.Types.InvariantConverter.Convert (row[Tags.ColumnClientId], out clientId);

				if (clientId == entry.ClientId)
				{
					row.BeginEdit ();
					row[Tags.ColumnClientSync]    = entry.SyncLogId;
					row[Tags.ColumnClientConDate] = entry.ConnectionDateTime;
					row.EndEdit ();
					
					return;
				}
			}
			
			throw new Exceptions.InvalidIdException ("Cannot find client entry.");
		}

		public void Remove(int entryClientId)
		{
			if (this.richCommand == null)
			{
				throw new Exceptions.InvalidIdException ("Cannot remove client: call RestoreFromBase first.");
			}
			
			System.Data.DataTable table = this.richCommand.DataSet.Tables[0];
			
			for (int i = 0; i < table.Rows.Count; i++)
			{
				System.Data.DataRow row = table.Rows[i];
				
				if (DbRichCommand.IsRowDeleted (row))
				{
					continue;
				}

				int clientId;

				Common.Types.InvariantConverter.Convert (row[Tags.ColumnClientId], out clientId);

				if (clientId == entryClientId)
				{
					this.richCommand.DeleteExistingRow (row);
					return;
				}
			}
			
			throw new Exceptions.InvalidIdException ("Cannot find client entry.");
		}
		
		
		public DbClientManager.Entry[] FindAllClients()
		{
			if (this.richCommand == null)
			{
				throw new Exceptions.InvalidIdException ("Cannot find clients: call RestoreFromBase first.");
			}
			
			System.Data.DataTable table = this.richCommand.DataSet.Tables[0];
			
			int n = table.Rows.Count;
			System.Collections.ArrayList list = new System.Collections.ArrayList ();
			
			for (int i = 0; i < n; i++)
			{
				System.Data.DataRow row = table.Rows[i];
				
				if (DbRichCommand.IsRowDeleted (row))
				{
					continue;
				}

				string clientName;
				int clientId;
				long clientSyncLogId;

				System.DateTime creationDateTime;
				System.DateTime connectionDateTime;

				Epsitec.Common.Types.InvariantConverter.Convert (row[Tags.ColumnClientName], out clientName);
				Epsitec.Common.Types.InvariantConverter.Convert (row[Tags.ColumnClientId], out clientId);
				Epsitec.Common.Types.InvariantConverter.Convert (row[Tags.ColumnClientSync], out clientSyncLogId);
				Epsitec.Common.Types.InvariantConverter.Convert (row[Tags.ColumnClientCreDate], out creationDateTime);
				Epsitec.Common.Types.InvariantConverter.Convert (row[Tags.ColumnClientConDate], out connectionDateTime);

				list.Add (new Entry (clientName, clientId, clientSyncLogId, creationDateTime, connectionDateTime));
			}
			
			Entry[] entries = new Entry[list.Count];
			list.CopyTo (entries);
			
			return entries;
		}
		
		
		#region IAttachable Members
		public void Attach(DbInfrastructure infrastructure, DbTable table)
		{
			this.infrastructure = infrastructure;
			this.table          = table;
			this.tableKey      = table.Key;
			this.tableSqlName = table.CreateSqlName ();
		}
		
		public void Detach()
		{
			this.infrastructure = null;
			this.table          = null;
		}
		#endregion
		
		#region IPersistable Members
		public void SerializeToBase(DbTransaction transaction)
		{
			if (this.richCommand != null)
			{
				this.richCommand.UpdateLogIds ();
				this.richCommand.UpdateRealIds (transaction);
				this.richCommand.UpdateTables (transaction);
			}
		}
		
		public void RestoreFromBase(DbTransaction transaction)
		{
			if (this.richCommand != null)
			{
				this.richCommand.Dispose ();
				this.richCommand = null;
			}
			
			this.richCommand = DbRichCommand.CreateFromTable (this.infrastructure, transaction, this.table);
		}
		#endregion
		
		#region Entry Class
		
		public sealed class Entry
		{
			public Entry() : this ("", 0, 0)
			{
			}

			public Entry(string clientName, int clientId, long syncLogId)
			{
				this.clientName   = clientName;
				this.clientId     = clientId;
				this.syncLogId   = syncLogId;
				this.creationDateTime = System.DateTime.UtcNow;
				this.connectionDateTime = this.creationDateTime;
			}

			public Entry(string clientName, int clientId, long syncLogId, System.DateTime creationDateTime, System.DateTime connectionDateTime)
			{
				this.clientName   = clientName;
				this.clientId     = clientId;
				this.syncLogId   = syncLogId;
				this.creationDateTime = creationDateTime;
				this.connectionDateTime = connectionDateTime;
			}
			
			
			public string						ClientName
			{
				get
				{
					return this.clientName;
				}
			}
			
			public int							ClientId
			{
				get
				{
					return this.clientId;
				}
			}
			
			public long							SyncLogId
			{
				get
				{
					return this.syncLogId;
				}
			}
			
			public System.DateTime				CreationDateTime
			{
				get
				{
					return this.creationDateTime;
				}
			}
			
			public System.DateTime				ConnectionDateTime
			{
				get
				{
					return this.connectionDateTime;
				}
			}


			private string clientName;
			private int clientId;
			private long syncLogId;
			private System.DateTime creationDateTime;
			private System.DateTime connectionDateTime;
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
