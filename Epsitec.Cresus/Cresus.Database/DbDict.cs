//	Copyright © 2004-2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using Epsitec.Common.Types;

using System.Collections.Generic;

namespace Epsitec.Cresus.Database
{
	/// <summary>
	/// The <c>DbDict</c> class manages a database backed dictionary of string
	/// key/value pairs.
	/// </summary>
	public sealed class DbDict : IStringDict, IPersistable, IAttachable, System.IDisposable, IDictionary<string, string>
	{
		public DbDict()
		{
		}

		public int ChangeCount
		{
			get
			{
				return this.changeCount;
			}
		}

		#region IStringDict Members
		
		public string this[string key]
		{
			get
			{
				if (string.IsNullOrEmpty (key))
				{
					throw new System.ArgumentNullException ("Null or empty key prohibited in dictionary");
				}
				
				string value;
				
				if (this.TryGetValue (key, out value))
				{
					return value;
				}
				else
				{
					throw new KeyNotFoundException (string.Format ("Cannot find key {0} in dictionary", key));
				}
			}
			set
			{
				if (string.IsNullOrEmpty (key))
				{
					throw new System.ArgumentNullException ("Null or empty key prohibited in dictionary");
				}
				
				System.Data.DataRow row = this.FindRow (key);

				if (row == null)
				{
					throw new KeyNotFoundException (string.Format ("Cannot find key {0} in dictionary", key));
				}
				
				string currentValue = row[Tags.ColumnDictValue] as string;

				if (currentValue != value)
				{
					row.BeginEdit ();
					row[Tags.ColumnDictValue] = value;
					row.EndEdit ();

					this.NotifyChanged ();
				}
			}
		}

		public string[] Keys
		{
			get
			{
				System.Data.DataRowCollection rows = this.dataTable.Rows;
				string[] keys = new string[rows.Count];

				for (int i = 0; i < rows.Count; i++)
				{
					keys[i] = rows[i][Tags.ColumnDictKey] as string;
				}

				return keys;
			}
		}

		public int Count
		{
			get
			{
				//	Compte les lignes réellement présentes (ne compte pas ce qui a été effacé).

				int count = 0;

				foreach (System.Data.DataRow row in this.dataTable.Rows)
				{
					if (!DbRichCommand.IsRowDeleted (row))
					{
						count++;
					}
				}

				return count;
			}
		}

		public void Add(string key, string value)
		{
			if (string.IsNullOrEmpty (key))
			{
				throw new System.ArgumentNullException ("Null or empty key prohibited in dictionary");
			}
			
			if (this.FindRow (key) != null)
			{
				throw new System.ArgumentException (string.Format ("Key {0} already exists in dictionary", key));
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

			row = this.FindRow (key);

			if (row == null)
			{
				return false;
			}
			else
			{
				this.command.DeleteExistingRow (row);

				this.NotifyChanged ();
				
				return true;
			}
		}

		public void Clear()
		{
			if (this.dataTable.Rows.Count > 0)
			{
				this.dataTable.Rows.Clear ();
				this.NotifyChanged ();
			}
		}

		public bool ContainsKey(string key)
		{
			return this.FindRow (key) == null ? false : true;
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
			if (this.dataSet != null)
			{
				this.dataSet.Dispose ();
			}

			this.infrastructure = null;
			this.table          = null;
			this.command        = null;
			this.dataSet        = null;
			this.dataTable      = null;
		}
		
		#endregion

		#region IPersistable Members
		
		public void PersistToBase(DbTransaction transaction)
		{
			this.command.UpdateLogIds ();
			this.command.UpdateRealIds (transaction);
			this.command.UpdateTables (transaction);
		}

		public void LoadFromBase(DbTransaction transaction)
		{
			this.command   = DbRichCommand.CreateFromTable (this.infrastructure, transaction, this.table, DbSelectRevision.LiveActive);
			this.dataSet   = this.command.DataSet;
			this.dataTable = this.dataSet.Tables[0];
		}
		
		#endregion

		#region IDisposable Members
		
		public void Dispose()
		{
			if (this.infrastructure != null)
			{
				this.Detach ();
			}
		}
		
		#endregion

		#region IDictionary<string,string> Members

		ICollection<string> IDictionary<string, string>.Keys
		{
			get
			{
				return this.Keys;
			}
		}

		public bool TryGetValue(string key, out string value)
		{
			System.Data.DataRow row = this.FindRow (key);
			
			if (row == null)
			{
				value = null;
				return false;
			}
			else
			{
				value = InvariantConverter.ToString (row[Tags.ColumnDictValue]);
				return true;
			}
		}

		public ICollection<string> Values
		{
			get
			{
				List<string> values = new List<string> ();

				foreach (string key in this.Keys)
				{
					values.Add (this[key]);
				}
				
				return values;
			}
		}

		#endregion

		#region ICollection<KeyValuePair<string,string>> Members

		public void Add(KeyValuePair<string, string> item)
		{
			this.Add (item.Key, item.Value);
		}

		public bool Contains(KeyValuePair<string, string> item)
		{
			string value;

			if (this.TryGetValue (item.Key, out value))
			{
				return value == item.Value;
			}
			else
			{
				return false;
			}
		}

		public void CopyTo(KeyValuePair<string, string>[] array, int arrayIndex)
		{
			foreach (KeyValuePair<string, string> item in this)
			{
				array[arrayIndex++] = item;
			}
		}

		public bool IsReadOnly
		{
			get
			{
				return false;
			}
		}

		public bool Remove(KeyValuePair<string, string> item)
		{
			if (this.Contains (item))
			{
				return this.Remove (item.Key);
			}
			else
			{
				return false;
			}
		}

		#endregion

		#region IEnumerable<KeyValuePair<string,string>> Members

		public IEnumerator<KeyValuePair<string, string>> GetEnumerator()
		{
			List<KeyValuePair<string, string>> pairs = new List<KeyValuePair<string, string>> ();

			foreach (string key in this.Keys)
			{
				pairs.Add (new KeyValuePair<string, string> (key, this[key]));
			}

			return pairs.GetEnumerator ();
		}

		#endregion

		#region IEnumerable Members

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator ();
		}

		#endregion

		public static void CreateTable(DbInfrastructure infrastructure, DbTransaction transaction, string tableName)
		{
			DbDict.CreateTable (infrastructure, transaction, tableName, DbElementCat.UserDataManaged, DbRevisionMode.Disabled, DbReplicationMode.Shared);
		}

		public static void CreateTable(DbInfrastructure infrastructure, DbTransaction transaction, string tableName, DbElementCat category, DbRevisionMode revisionMode, DbReplicationMode replicationMode)
		{
			//	Crée une table pour stocker un dictionnaire.

			DbTable table = infrastructure.CreateTable (tableName, category, revisionMode, replicationMode);

			DbTypeDef typeDictKey   = infrastructure.ResolveDbType (transaction, Tags.TypeDictKey);
			DbTypeDef typeDictValue = infrastructure.ResolveDbType (transaction, Tags.TypeDictValue);

			System.Diagnostics.Debug.Assert (typeDictKey.IsNullable == false);
			System.Diagnostics.Debug.Assert (typeDictValue.IsNullable == true);

			DbColumn col1 = new DbColumn (Tags.ColumnDictKey, typeDictKey, DbColumnClass.Data, DbElementCat.Internal);
			DbColumn col2 = new DbColumn (Tags.ColumnDictValue, typeDictValue, DbColumnClass.Data, DbElementCat.Internal);

			table.Columns.Add (col1);
			table.Columns.Add (col2);

			infrastructure.RegisterNewDbTable (transaction, table);
		}

		private System.Data.DataRow FindRow(string key)
		{
			foreach (System.Data.DataRow row in this.dataTable.Rows)
			{
				if ((DbRichCommand.IsRowLive (row)) &&
					(InvariantConverter.ToString (row[Tags.ColumnDictKey]) == key))
				{
					return row;
				}
			}

			return null;
		}

		private void NotifyChanged()
		{
			this.changeCount++;
		}

		private DbInfrastructure infrastructure;
		private DbTable table;
		private DbRichCommand command;
		private System.Data.DataSet dataSet;
		private System.Data.DataTable dataTable;
		private int changeCount;
	}
}
