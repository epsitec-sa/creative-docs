//	Copyright © 2004-2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Cresus.Database
{
	/// <summary>
	/// La classe DbClientManager gère la table des clients connus (CR_CLIENT_DEF).
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
			
			int[] used_ids = new int[n];
			
			for (int i = 0; i < n; i++)
			{
				used_ids[i] = entries[i].ClientId;
			}
			
			System.Array.Sort (used_ids);
			
			int new_id = DbClientManager.MinClientId;
			
			for (int i = 0; i < n; i++)
			{
				if (new_id < used_ids[i])
				{
					return new_id;
				}
				new_id++;
			}
			
			if (new_id > DbClientManager.MaxClientId)
			{
				throw new Exceptions.InvalidIdException ("No more client IDs available.");
			}
			
			return new_id;
		}
		
		
		public DbClientManager.Entry CreateNewClient(string client_name)
		{
			int  client_id   = this.FindFreeClientId ();
			long sync_log_id = 0;
			
			Entry entry = new Entry (client_name, client_id, sync_log_id);
			
			return entry;
		}

		public DbClientManager.Entry CreateAndInsertNewClient(string client_name)
		{
			int  client_id   = this.FindFreeClientId ();
			long sync_log_id = 0;
			
			Entry entry = new Entry (client_name, client_id, sync_log_id);
			
			this.Insert (entry);
			
			return entry;
		}

		
		public void Insert(DbClientManager.Entry entry)
		{
			if (this.rich_command == null)
			{
				throw new Exceptions.InvalidIdException ("Cannot insert client: call RestoreFromBase first.");
			}
			
			System.Data.DataTable data_table = this.rich_command.DataSet.Tables[0];
			System.Data.DataRow   data_row;
			
			for (int i = 0; i < data_table.Rows.Count; i++)
			{
				data_row = data_table.Rows[i];
				
				if (DbRichCommand.IsRowDeleted (data_row))
				{
					continue;
				}
				
				int client_id;
				
				Common.Types.InvariantConverter.Convert (data_row[Tags.ColumnClientId], out client_id);
				
				if (client_id == entry.ClientId)
				{
					throw new Exceptions.ExistsException ("Cannot insert same client twice.");
				}
			}
			
			this.rich_command.CreateNewRow (data_table.TableName, out data_row);
			
			data_row.BeginEdit ();
			data_row[Tags.ColumnClientName]    = entry.ClientName;
			data_row[Tags.ColumnClientId]      = entry.ClientId;
			data_row[Tags.ColumnClientSync]    = entry.SyncLogId;
			data_row[Tags.ColumnClientCreDate] = entry.CreationDateTime;
			data_row[Tags.ColumnClientConDate] = entry.ConnectionDateTime;
			data_row.EndEdit ();
		}
		
		public void Update(DbClientManager.Entry entry)
		{
			if (this.rich_command == null)
			{
				throw new Exceptions.InvalidIdException ("Cannot update client: call RestoreFromBase first.");
			}
			
			System.Data.DataTable data_table = this.rich_command.DataSet.Tables[0];
			
			for (int i = 0; i < data_table.Rows.Count; i++)
			{
				System.Data.DataRow data_row = data_table.Rows[i];
				
				if (DbRichCommand.IsRowDeleted (data_row))
				{
					continue;
				}
				
				int client_id;
				
				Common.Types.InvariantConverter.Convert (data_row[Tags.ColumnClientId], out client_id);
				
				if (client_id == entry.ClientId)
				{
					data_row.BeginEdit ();
					data_row[Tags.ColumnClientSync]    = entry.SyncLogId;
					data_row[Tags.ColumnClientConDate] = entry.ConnectionDateTime;
					data_row.EndEdit ();
					
					return;
				}
			}
			
			throw new Exceptions.InvalidIdException ("Cannot find client entry.");
		}
		
		public void Remove(int entry_client_id)
		{
			if (this.rich_command == null)
			{
				throw new Exceptions.InvalidIdException ("Cannot remove client: call RestoreFromBase first.");
			}
			
			System.Data.DataTable data_table = this.rich_command.DataSet.Tables[0];
			
			for (int i = 0; i < data_table.Rows.Count; i++)
			{
				System.Data.DataRow data_row = data_table.Rows[i];
				
				if (DbRichCommand.IsRowDeleted (data_row))
				{
					continue;
				}
				
				int client_id;
				
				Common.Types.InvariantConverter.Convert (data_row[Tags.ColumnClientId], out client_id);
				
				if (client_id == entry_client_id)
				{
					this.rich_command.DeleteExistingRow (data_row);
					return;
				}
			}
			
			throw new Exceptions.InvalidIdException ("Cannot find client entry.");
		}
		
		
		public DbClientManager.Entry[] FindAllClients()
		{
			if (this.rich_command == null)
			{
				throw new Exceptions.InvalidIdException ("Cannot find clients: call RestoreFromBase first.");
			}
			
			System.Data.DataTable data_table = this.rich_command.DataSet.Tables[0];
			
			int n = data_table.Rows.Count;
			System.Collections.ArrayList list = new System.Collections.ArrayList ();
			
			for (int i = 0; i < n; i++)
			{
				System.Data.DataRow row = data_table.Rows[i];
				
				if (DbRichCommand.IsRowDeleted (row))
				{
					continue;
				}
				
				string client_name;
				int    client_id;
				long   client_sync_log_id;
				
				System.DateTime creation_date_time;
				System.DateTime connection_date_time;
				
				Epsitec.Common.Types.InvariantConverter.Convert (row[Tags.ColumnClientName], out client_name);
				Epsitec.Common.Types.InvariantConverter.Convert (row[Tags.ColumnClientId], out client_id);
				Epsitec.Common.Types.InvariantConverter.Convert (row[Tags.ColumnClientSync], out client_sync_log_id);
				Epsitec.Common.Types.InvariantConverter.Convert (row[Tags.ColumnClientCreDate], out creation_date_time);
				Epsitec.Common.Types.InvariantConverter.Convert (row[Tags.ColumnClientConDate], out connection_date_time);
				
				list.Add (new Entry (client_name, client_id, client_sync_log_id, creation_date_time, connection_date_time));
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
			this.table_key      = table.Key;
			this.table_sql_name = table.CreateSqlName ();
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
			if (this.rich_command != null)
			{
				this.rich_command.UpdateLogIds ();
				this.rich_command.UpdateRealIds (transaction);
				this.rich_command.UpdateTables (transaction);
			}
		}
		
		public void RestoreFromBase(DbTransaction transaction)
		{
			if (this.rich_command != null)
			{
				this.rich_command.Dispose ();
				this.rich_command = null;
			}
			
			this.rich_command = DbRichCommand.CreateFromTable (this.infrastructure, transaction, this.table);
		}
		#endregion
		
		#region Entry Class
		public class Entry
		{
			public Entry() : this ("", 0, 0)
			{
			}
			
			public Entry(string client_name, int client_id, long sync_log_id)
			{
				this.client_name   = client_name;
				this.client_id     = client_id;
				this.sync_log_id   = sync_log_id;
				this.cre_date_time = System.DateTime.UtcNow;
				this.con_date_time = this.cre_date_time;
			}
			
			public Entry(string client_name, int client_id, long sync_log_id, System.DateTime creation_date_time, System.DateTime connection_date_time)
			{
				this.client_name   = client_name;
				this.client_id     = client_id;
				this.sync_log_id   = sync_log_id;
				this.cre_date_time = creation_date_time;
				this.con_date_time = connection_date_time;
			}
			
			
			public string						ClientName
			{
				get
				{
					return this.client_name;
				}
			}
			
			public int							ClientId
			{
				get
				{
					return this.client_id;
				}
			}
			
			public long							SyncLogId
			{
				get
				{
					return this.sync_log_id;
				}
			}
			
			public System.DateTime				CreationDateTime
			{
				get
				{
					return this.cre_date_time;
				}
			}
			
			public System.DateTime				ConnectionDateTime
			{
				get
				{
					return this.con_date_time;
				}
			}
			
			
			private string						client_name;
			private int							client_id;
			private long						sync_log_id;
			private System.DateTime				cre_date_time;
			private System.DateTime				con_date_time;
		}
		#endregion
		
		private DbInfrastructure				infrastructure;
		private DbTable							table;
		private DbKey							table_key;
		private string							table_sql_name;
		private DbRichCommand					rich_command;
		
		public const int						MinClientId		= 1000;
		public const int						MaxClientId		= 1999;
	}
}
