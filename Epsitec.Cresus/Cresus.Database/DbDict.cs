//	Copyright © 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Cresus.Database
{
	/// <summary>
	/// La classe DbDict représente un dictionnaire (stockage de données par
	/// paires clef/valeur) stocké dans une table de la base.
	/// </summary>
	
	public class DbDict : IPersistable, IAttachable, System.IDisposable
	{
		public DbDict()
		{
		}
		
		
		public string							this[string key]
		{
			get
			{
				System.Data.DataRow row = this.FindRow (key, false);
				
				if (row != null)
				{
					return row[Tags.ColumnDictValue] as string;
				}
				
				return null;
			}
			set
			{
				System.Data.DataRow row = this.FindRow (key, true);
				
				if (row == null)
				{
					throw new System.ArgumentOutOfRangeException ("key", key, "Unknown key in DbDict.");
				}
				
				row.BeginEdit ();
				row[Tags.ColumnDictValue] = value;
				row.EndEdit ();
			}
		}
		
		
		public void Add(string key, string value)
		{
			if (this.FindRow (key, false) != null)
			{
				throw new System.ArgumentOutOfRangeException ("key", key, "Key already exists in DbDict.");
			}
			
			System.Data.DataRow row;
			
			this.command.CreateNewRow (this.table.Name, out row);
			
			System.Diagnostics.Debug.Assert (row != null);
			
			this[key] = value;
		}
		
		public void Remove(string key)
		{
			System.Data.DataRow row;
			
			row = this.FindRow (key, false);
			
			if (row == null)
			{
				throw new System.ArgumentOutOfRangeException ("key", key, "Key does not exist in DbDict.");
			}
			
			this.command.DeleteRow (row);
		}
		
		
		public static void CreateTable(DbInfrastructure infrastructure, DbTransaction transaction, string table_name)
		{
			//	Crée une table pour stocker un dictionnaire.
			
			//	TODO: ...
		}
		
		
		#region IAttachable Members
		public void Attach(DbInfrastructure infrastructure, DbTable table)
		{
			this.infrastructure = infrastructure;
			this.table          = table;
		}
		
		public void Detach()
		{
			this.data_set.Dispose ();
			
			this.infrastructure = null;
			this.table          = null;
			this.command        = null;
			this.data_set       = null;
			this.data_table     = null;
		}
		#endregion
		
		#region IPersistable Members
		public void SerializeToBase(DbTransaction transaction)
		{
			this.command.UpdateTables (transaction);
		}
		
		public void RestoreFromBase(DbTransaction transaction)
		{
			this.command    = DbRichCommand.CreateFromTable (this.infrastructure, transaction, this.table);
			this.data_set   = this.command.DataSet;
			this.data_table = this.data_set.Tables[0];
		}
		#endregion
		
		#region IDisposable Members
		public void Dispose()
		{
			this.Dispose (true);
			System.GC.SuppressFinalize (this);
		}
		#endregion
		
		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (this.infrastructure != null)
				{
					this.Detach ();
				}
			}
		}
		
		protected virtual System.Data.DataRow FindRow(string key, bool write)
		{
			System.Data.DataRowCollection rows = this.data_table.Rows;
			
			for (int i = 0; i < rows.Count; i++)
			{
				string row_key_name = rows[i][Tags.ColumnDictKey] as string;
				DbKey  row_key_id   = new DbKey (rows[i]);
					
				if ((row_key_id.Status != DbRowStatus.Deleted) &&
					(row_key_name == key))
				{
					return rows[i];
				}
			}
			
			return null;
		}
		
		
		private DbInfrastructure				infrastructure;
		private DbTable							table;
		private DbRichCommand					command;
		private System.Data.DataSet				data_set;
		private System.Data.DataTable			data_table;
	}
}
