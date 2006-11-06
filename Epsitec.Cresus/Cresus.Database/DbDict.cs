//	Copyright © 2004-2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Cresus.Database
{
	/// <summary>
	/// La classe DbDict représente un dictionnaire (stockage de données par
	/// paires clef/valeur) stocké dans une table de la base.
	/// </summary>
	
	public class DbDict : Epsitec.Common.Types.IStringDict, IPersistable, IAttachable, System.IDisposable
	{
		public DbDict()
		{
		}
		
		
		public int								ChangeCount
		{
			get
			{
				return this.change_count;
			}
		}
		
		
		#region IStringDict Members
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
				
				string current_value = row[Tags.ColumnDictValue] as string;
				
				if (current_value != value)
				{
					row.BeginEdit ();
					row[Tags.ColumnDictValue] = value;
					row.EndEdit ();
					
					this.NotifyChanged ();
				}
			}
		}
		
		public string[]							Keys
		{
			get
			{
				System.Data.DataRowCollection rows = this.data_table.Rows;
				string[] keys = new string[rows.Count];
				
				for (int i = 0; i < rows.Count; i++)
				{
					keys[i] = rows[i][Tags.ColumnDictKey] as string;
				}
				
				return keys;
			}
		}
		
		public int								Count
		{
			get
			{
				System.Data.DataRowCollection rows = this.data_table.Rows;
				
				//	Compte les lignes réellement présentes (ne compte pas ce qui a été effacé).
				
				int n = 0;
				
				for (int i = 0; i < rows.Count; i++)
				{
					if (! DbRichCommand.IsRowDeleted (rows[i]))
					{
						n++;
					}
				}
				
				return n;
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
			
			row.BeginEdit ();
			row[Tags.ColumnDictKey]   = key;
			row[Tags.ColumnDictValue] = value;
			row.EndEdit ();
			
			System.Diagnostics.Debug.Assert (this[key] == value);
			
			this.NotifyChanged ();
		}
		
		public bool Remove(string key)
		{
			System.Data.DataRow row;
			
			row = this.FindRow (key, false);
			
			if (row == null)
			{
				return false;
			}
			
			this.command.DeleteExistingRow (row);
			
			this.NotifyChanged ();
			
			return true;
		}
		
		public void Clear()
		{
			if (this.data_table.Rows.Count > 0)
			{
				this.data_table.Rows.Clear ();
				this.NotifyChanged ();
			}
		}
		
		public bool ContainsKey(string key)
		{
			System.Data.DataRow row = this.FindRow (key, false);
			return (row != null);
		}
		#endregion
		
		#region IAttachable Members
		public void Attach(DbInfrastructure infrastructure, DbTable table)
		{
			this.infrastructure = infrastructure;
			this.table          = table;
		}
		
		public void Detach()
		{
			if (this.data_set != null)
			{
				this.data_set.Dispose ();
			}
			
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
			this.command.UpdateLogIds ();
			this.command.UpdateRealIds (transaction);
			this.command.UpdateTables (transaction);
		}
		
		public void RestoreFromBase(DbTransaction transaction)
		{
			this.command    = DbRichCommand.CreateFromTable (this.infrastructure, transaction, this.table, DbSelectRevision.LiveActive);
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
		
		public static void CreateTable(DbInfrastructure infrastructure, DbTransaction transaction, string table_name)
		{
			DbDict.CreateTable (infrastructure, transaction, table_name, DbElementCat.UserDataManaged, DbRevisionMode.Disabled, DbReplicationMode.Shared);
		}
		
		public static void CreateTable(DbInfrastructure infrastructure, DbTransaction transaction, string table_name, DbElementCat category, DbRevisionMode revision_mode, DbReplicationMode replication_mode)
		{
			//	Crée une table pour stocker un dictionnaire.
			
			DbTable table = infrastructure.CreateTable (table_name, category, revision_mode, replication_mode);
			
			DbType type_dict_key   = infrastructure.ResolveDbType (transaction, Tags.TypeDictKey);
			DbType type_dict_value = infrastructure.ResolveDbType (transaction, Tags.TypeDictValue);
			
			DbColumn col1 = new DbColumn (Tags.ColumnDictKey,   type_dict_key,   Nullable.No,  DbColumnClass.Data, DbElementCat.Internal);
			DbColumn col2 = new DbColumn (Tags.ColumnDictValue, type_dict_value, Nullable.Yes, DbColumnClass.Data, DbElementCat.Internal);
			
			table.Columns.Add (col1);
			table.Columns.Add (col2);
			
			infrastructure.RegisterNewDbTable (transaction, table);
		}
		
		
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
					
				if ((! DbRichCommand.IsRowDeleted (rows[i])) &&
					(row_key_name == key))
				{
					return rows[i];
				}
			}
			
			return null;
		}
		
		
		protected virtual void NotifyChanged()
		{
			this.change_count++;
		}
		
		
		private DbInfrastructure				infrastructure;
		private DbTable							table;
		private DbRichCommand					command;
		private System.Data.DataSet				data_set;
		private System.Data.DataTable			data_table;
		private int								change_count;
	}
}
