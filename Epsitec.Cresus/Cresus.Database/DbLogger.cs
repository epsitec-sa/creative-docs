//	Copyright © 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Cresus.Database
{
	/// <summary>
	/// La classe DbLogger gère le journal des modifications (CR_LOG).
	/// </summary>
	
	public sealed class DbLogger : IAttachable
	{
		public DbLogger()
		{
		}
		
		
		public DbId								CurrentId
		{
			get
			{
				if ((this.client_id >= 0) &&
					(this.log_id >= 0))
				{
					return DbId.CreateID (this.log_id, this.client_id);
				}
				
				throw new System.InvalidOperationException ("DbLogger not initialised.");
			}
		}
		
		
		internal void DefineClientId(int client_id)
		{
			System.Diagnostics.Debug.Assert (this.client_id == -1);
			System.Diagnostics.Debug.Assert (this.infrastructure == null);
			
			this.client_id = client_id;
		}
		
		internal void DefineLogId(long log_id)
		{
			System.Diagnostics.Debug.Assert (this.log_id == -1);
			System.Diagnostics.Debug.Assert (this.infrastructure == null);
			
			this.log_id = log_id;
		}
		
		
		internal void FindCurrentLogId(DbTransaction transaction)
		{
			System.Diagnostics.Debug.Assert (this.client_id >= 0);
			System.Diagnostics.Debug.Assert (this.infrastructure != null);
			System.Diagnostics.Debug.Assert (this.table != null);
			
			this.log_id = this.infrastructure.NextRowIdInTable (transaction, this.table_key);
			
			System.Diagnostics.Debug.Assert (this.log_id > 0);
			System.Diagnostics.Debug.Assert (this.log_id < DbId.LocalRange);
		}
		
		
		internal void InsertEntry(DbTransaction transaction)
		{
			Entry entry = new Entry (this.log_id + 1, this.client_id);
			
			this.Insert (transaction, entry);
		}
		
		internal void Insert(DbTransaction transaction, DbLogger.Entry entry)
		{
			Collections.SqlFields fields = new Collections.SqlFields ();
			
			fields.Add (this.table.Columns[Tags.ColumnId].CreateSqlField (this.infrastructure.TypeConverter, entry.Id));
			fields.Add (this.table.Columns[Tags.ColumnDateTime].CreateSqlField (this.infrastructure.TypeConverter, entry.DateTime));
			
			long log_id = entry.Id.LocalID;
			
			this.infrastructure.SqlBuilder.InsertData (this.table.CreateSqlName (), fields);
			this.infrastructure.ExecuteSilent (transaction);
			this.infrastructure.UpdateTableNextId (transaction, this.table_key, log_id + 1);
			
			this.log_id = log_id;
		}
		
		
		#region IAttachable Members
		public void Attach(DbInfrastructure infrastructure, DbTable table)
		{
			this.infrastructure = infrastructure;
			this.table          = table;
			this.table_key      = table.InternalKey;
		}
		
		public void Detach()
		{
			this.infrastructure = null;
			this.table          = null;
		}
		#endregion
		
		
		internal class Entry
		{
			public Entry() : this (0, 0)
			{
			}
			
			public Entry(long log_id, int client_id)
			{
				this.id = DbId.CreateID (log_id, client_id);
				this.date_time = 0;
			}
			
			
			public DbId							Id
			{
				get
				{
					return this.id;
				}
			}
			
			public long							DateTime
			{
				get
				{
					return this.date_time;
				}
			}
			
			
			private DbId						id;
			private long						date_time;
		}
		
		private DbInfrastructure				infrastructure;
		private DbTable							table;
		private DbKey							table_key;
		
		private int								client_id = -1;
		private long							log_id    = -1;
	}
}
