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
				if ((this.client_id != -1) &&
					(this.next_id != -1))
				{
					return this.current_id;
				}
				
				throw new System.InvalidOperationException ("DbLogger not initialised.");
			}
		}
		
		
		internal void DefineClientId(int client_id)
		{
			System.Diagnostics.Debug.Assert (this.client_id == -1);
			System.Diagnostics.Debug.Assert (this.next_id == -1);
			System.Diagnostics.Debug.Assert (this.infrastructure == null);
			
			this.client_id = client_id;
		}
		
		internal void DefineLogId(long log_id)
		{
			System.Diagnostics.Debug.Assert (this.client_id != -1);
			System.Diagnostics.Debug.Assert (this.next_id == -1);
			System.Diagnostics.Debug.Assert (this.infrastructure == null);
			System.Diagnostics.Debug.Assert (log_id > 0);
			System.Diagnostics.Debug.Assert (log_id < DbId.LocalRange);
			
			this.current_id = DbId.CreateId (log_id, this.client_id);
			this.next_id    = log_id + 1;
		}
		
		
		internal void ResetCurrentLogId(DbTransaction transaction)
		{
			System.Diagnostics.Debug.Assert (this.client_id != -1);
			System.Diagnostics.Debug.Assert (this.next_id == -1);
			System.Diagnostics.Debug.Assert (this.infrastructure != null);
			System.Diagnostics.Debug.Assert (this.table != null);
			
			//	L'identificateur local stocké dans la base correspond toujours à celui de la
			//	prochaine ligne à créer, mais l'identificateur de client correspond au dernier
			//	enregistré dans le LOG. Ainsi, id.ClientId n'est pas nécessairement égal à
			//	l'identificateur de client actif.
			
			DbId id = this.infrastructure.NextRowIdInTable (transaction, this.table_key);
			
			this.next_id    = id.LocalId;
			this.current_id = DbId.CreateId (this.next_id - 1, id.ClientId);
			
			System.Diagnostics.Debug.Assert (this.next_id > 0);
			System.Diagnostics.Debug.Assert (this.next_id < DbId.LocalRange);
		}
		
		
		public void CreatePermanentEntry(DbTransaction transaction)
		{
			Entry entry = new Entry (this.next_id, this.client_id);
			
			this.Insert (transaction, entry);
		}
		
		public void CreateTemporaryEntry(DbTransaction transaction)
		{
			Entry entry = new Entry (this.next_id, DbId.TempClientId);
			
			this.Insert (transaction, entry);
		}
		
		
		public void Insert(DbTransaction transaction, DbLogger.Entry entry)
		{
			Collections.SqlFields fields = new Collections.SqlFields ();
			
			fields.Add (this.table.Columns[Tags.ColumnId].CreateSqlField (this.infrastructure.TypeConverter, entry.Id));
			fields.Add (this.table.Columns[Tags.ColumnDateTime].CreateSqlField (this.infrastructure.TypeConverter, entry.DateTime));
			
			long next_id = entry.Id.LocalId + 1;
			
			this.infrastructure.SqlBuilder.InsertData (this.table_sql_name, fields);
			this.infrastructure.ExecuteSilent (transaction);
			
			//	Enregistre dans la base le prochain ID à utiliser, en prenant note du
			//	ClientId appliqué à l'élément que l'on vient d'enregistrer dans le LOG :
			
			this.infrastructure.UpdateTableNextId (transaction, this.table_key, DbId.CreateId (next_id, entry.Id.ClientId));
			
			this.next_id    = next_id;
			this.current_id = entry.Id;
		}
		
		public bool Remove(DbTransaction transaction, DbId id)
		{
			//	Supprime l'élément spécifié du log.
			
			Collections.SqlFields conditions = new Collections.SqlFields ();
			
			SqlField log_id_name  = SqlField.CreateName (this.table_sql_name, Tags.ColumnId);
			SqlField log_id_value = SqlField.CreateConstant (id.Value, DbKey.RawTypeForId);
			
			conditions.Add (new SqlFunction (SqlFunctionType.CompareEqual, log_id_name, log_id_value));
			
			this.infrastructure.SqlBuilder.RemoveData (this.table.CreateSqlName (), conditions);
			object result = this.infrastructure.ExecuteNonQuery (transaction);
			
			return 1 == (int) result;
		}
		
		public void RemoveRange(DbTransaction transaction, DbId id_start)
		{
			this.RemoveRange (transaction, id_start, DbId.CreateId (DbId.LocalRange - 1, id_start.ClientId));
		}
		
		public void RemoveRange(DbTransaction transaction, DbId id_start, DbId id_end)
		{
			//	Supprime les éléments allant de start à end, y compris start et end.
			
			Collections.SqlFields conditions = new Collections.SqlFields ();
			
			SqlField log_id_name  = SqlField.CreateName (this.table_sql_name, Tags.ColumnId);
			SqlField log_id_val_1 = SqlField.CreateConstant (id_start.Value, DbKey.RawTypeForId);
			SqlField log_id_val_2 = SqlField.CreateConstant (id_end.Value, DbKey.RawTypeForId);
			
			conditions.Add (new SqlFunction (SqlFunctionType.CompareGreaterThanOrEqual, log_id_name, log_id_val_1));
			conditions.Add (new SqlFunction (SqlFunctionType.CompareLessThanOrEqual, log_id_name, log_id_val_2));
			
			this.infrastructure.SqlBuilder.RemoveData (this.table.CreateSqlName (), conditions);
			this.infrastructure.ExecuteSilent (transaction);
		}
		
		
		public DbLogger.Entry[] Find(DbTransaction transaction, DbId id_start, DbId id_end)
		{
			//	Trouve les éléments allant de start à end, y compris start et end.
			
			SqlField log_id_name  = SqlField.CreateName ("T", Tags.ColumnId);
			SqlField log_id_val_1 = SqlField.CreateConstant (id_start.Value, DbKey.RawTypeForId);
			SqlField log_id_val_2 = SqlField.CreateConstant (id_end.Value, DbKey.RawTypeForId);
			
			SqlSelect query = new SqlSelect ();
			
			query.Fields.Add ("T_ID", SqlField.CreateName ("T", Tags.ColumnId));
			query.Fields.Add ("T_DT", SqlField.CreateName ("T", Tags.ColumnDateTime));
			
			query.Tables.Add ("T", SqlField.CreateName (this.table_sql_name));
			
			query.Conditions.Add (new SqlFunction (SqlFunctionType.CompareGreaterThanOrEqual, log_id_name, log_id_val_1));
			query.Conditions.Add (new SqlFunction (SqlFunctionType.CompareLessThanOrEqual, log_id_name, log_id_val_2));
			
			System.Data.DataTable data_table = this.infrastructure.ExecuteSqlSelect (transaction, query, 0);
			
			int n = data_table.Rows.Count;
			Entry[] entries = new Entry[n];
			
			for (int i = 0; i < n; i++)
			{
				System.Data.DataRow row = data_table.Rows[i];
				
				long            log_id;
				System.DateTime date_time;
				
				Epsitec.Common.Types.Converter.Convert (row["T_ID"], out log_id);
				Epsitec.Common.Types.Converter.Convert (row["T_DT"], out date_time);
				
				entries[i] = new Entry (log_id, date_time);
			}
			
			return entries;
		}
		
		
		#region IAttachable Members
		public void Attach(DbInfrastructure infrastructure, DbTable table)
		{
			this.infrastructure = infrastructure;
			this.table          = table;
			this.table_key      = table.InternalKey;
			this.table_sql_name = table.CreateSqlName ();
		}
		
		public void Detach()
		{
			this.infrastructure = null;
			this.table          = null;
		}
		#endregion
		
		#region Entry Class
		public class Entry
		{
			public Entry() : this (0, 0)
			{
			}
			
			public Entry(long log_id, int client_id)
			{
				this.id = DbId.CreateId (log_id, client_id);
				this.date_time = System.DateTime.UtcNow;
			}
			
			public Entry(long id, System.DateTime date_time)
			{
				this.id = new DbId (id);
				this.date_time = date_time;
			}
			
			
			public DbId							Id
			{
				get
				{
					return this.id;
				}
			}
			
			public System.DateTime				DateTime
			{
				get
				{
					return this.date_time;
				}
			}
			
			
			private DbId						id;
			private System.DateTime				date_time;
		}
		#endregion
		
		private DbInfrastructure				infrastructure;
		private DbTable							table;
		private DbKey							table_key;
		private string							table_sql_name;
		
		private int								client_id  = -1;
		private long							next_id    = -1;
		private DbId							current_id;
	}
}
