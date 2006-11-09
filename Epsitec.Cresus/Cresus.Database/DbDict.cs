//	Copyright © 2004-2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;

using System.Collections.Generic;

namespace Epsitec.Cresus.Database
{
	/// <summary>
	/// The <c>DbDict</c> class manages a database backed dictionary of key/value
	/// pairs, where both keys and values are represented using <c>string</c>.
	/// </summary>
	public sealed class DbDict : IStringDict, IPersistable, IAttachable, System.IDisposable, IDictionary<string, string>
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="DbDict"/> class.
		/// </summary>
		public DbDict()
		{
		}

		/// <summary>
		/// Gets the change count for this dictionary.
		/// </summary>
		/// <value>The change count.</value>
		public int								ChangeCount
		{
			get
			{
				return this.changeCount;
			}
		}

		#region IStringDict Members

		/// <summary>
		/// Gets or sets the value with the specified key.
		/// </summary>
		/// <value>The <c>string</c> value.</value>
		public string							this[string key]
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

		/// <summary>
		/// Gets the keys of the known values.
		/// </summary>
		/// <value>The keys.</value>
		public string[]							Keys
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

		/// <summary>
		/// Gets the number of known values.
		/// </summary>
		/// <value>The number of known values.</value>
		public int								Count
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

		/// <summary>
		/// Adds the specified key/value pair to the dictionary.
		/// </summary>
		/// <param name="key">The key.</param>
		/// <param name="value">The value.</param>
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

		/// <summary>
		/// Removes the specified key/value pair from the dictionary.
		/// </summary>
		/// <param name="key">The key.</param>
		/// <returns>
		/// 	<c>true</c> if the key was successfully removed; otherwise, <c>false</c>.
		/// </returns>
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

		/// <summary>
		/// Clears the dictionary.
		/// </summary>
		public void Clear()
		{
			if (this.dataTable.Rows.Count > 0)
			{
				this.dataTable.Rows.Clear ();
				this.NotifyChanged ();
			}
		}

		/// <summary>
		/// Determines whether the dictionary contains the specified key.
		/// </summary>
		/// <param name="key">The key.</param>
		/// <returns>
		/// 	<c>true</c> if the dictionary contains the specified key; otherwise, <c>false</c>.
		/// </returns>
		public bool ContainsKey(string key)
		{
			return this.FindRow (key) == null ? false : true;
		}
		
		#endregion

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
		}

		/// <summary>
		/// Detaches this instance from the database.
		/// </summary>
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

		/// <summary>
		/// Saves the instance data to the database.
		/// </summary>
		/// <param name="transaction">The transaction.</param>
		public void PersistToBase(DbTransaction transaction)
		{
			this.command.UpdateLogIds ();
			this.command.AssignRealRowIds (transaction);
			this.command.UpdateTables (transaction);
		}

		/// <summary>
		/// Loads the instance data from the database.
		/// </summary>
		/// <param name="transaction">The transaction.</param>
		public void LoadFromBase(DbTransaction transaction)
		{
			this.command   = DbRichCommand.CreateFromTable (this.infrastructure, transaction, this.table, DbSelectRevision.LiveActive);
			this.dataSet   = this.command.DataSet;
			this.dataTable = this.dataSet.Tables[0];
		}
		
		#endregion

		#region IDisposable Members

		/// <summary>
		/// Disposes the dictionary; this will call <c>Detach</c> if needed.
		/// </summary>
		public void Dispose()
		{
			if (this.infrastructure != null)
			{
				this.Detach ();
			}
		}
		
		#endregion

		#region IDictionary<string,string> Members

		/// <summary>
		/// Gets the keys of the known values.
		/// </summary>
		/// <value>The keys.</value>
		ICollection<string> IDictionary<string, string>.Keys
		{
			get
			{
				return this.Keys;
			}
		}

		/// <summary>
		/// Tries to get the value for the specified key.
		/// </summary>
		/// <param name="key">The key.</param>
		/// <param name="value">The value.</param>
		/// <returns><c>true</c> if the value was found; otherwise, <c>false</c>.</returns>
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

		/// <summary>
		/// Gets the values contained in the dictionary.
		/// </summary>
		/// <returns>The values contained in the dictionary.</returns>
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

		/// <summary>
		/// Adds an item to the collection.</see>.
		/// </summary>
		/// <param name="item">The item to add.</param>
		public void Add(KeyValuePair<string, string> item)
		{
			this.Add (item.Key, item.Value);
		}

		/// <summary>
		/// Determines whether the collection contains a specific item.
		/// </summary>
		/// <param name="item">The item to locate.</param>
		/// <returns>
		/// 	<c>true</c> if the collection contains the item; otherwise, <c>false</c>.
		/// </returns>
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

		/// <summary>
		/// Copies the elements the collection to the array.
		/// </summary>
		/// <param name="array">The output array.</param>
		/// <param name="arrayIndex">The zero-based index in array at which copying begins.</param>
		public void CopyTo(KeyValuePair<string, string>[] array, int arrayIndex)
		{
			foreach (KeyValuePair<string, string> item in this)
			{
				array[arrayIndex++] = item;
			}
		}

		/// <summary>
		/// Gets a value indicating whether the collection is read-only.
		/// </summary>
		/// <value></value>
		/// <returns><c>true</c> if the collection is read-only; otherwise, <c>false</c>.</returns>
		public bool IsReadOnly
		{
			get
			{
				return false;
			}
		}

		/// <summary>
		/// Removes the specified item from the collection.
		/// </summary>
		/// <param name="item">The item to remove.</param>
		/// <returns>
		/// 	<c>true</c> if item was successfully removed; otherwise <c>false</c>.
		/// </returns>
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

		/// <summary>
		/// Returns an enumerator that iterates through the collection.
		/// </summary>
		/// <returns>
		/// An enumerator that can be used to iterate through the collection.
		/// </returns>
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

		/// <summary>
		/// Returns an enumerator that iterates through the collection.
		/// </summary>
		/// <returns>
		/// An enumerator that can be used to iterate through the collection.
		/// </returns>
		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator ();
		}

		#endregion

		/// <summary>
		/// Creates a table in the database which can then be used to store
		/// the contents of the dictionary.
		/// </summary>
		/// <param name="infrastructure">The infrastructure.</param>
		/// <param name="transaction">The transaction.</param>
		/// <param name="tableName">Name of the table.</param>
		public static void CreateTable(DbInfrastructure infrastructure, DbTransaction transaction, string tableName)
		{
			DbDict.CreateTable (infrastructure, transaction, tableName, DbElementCat.ManagedUserData, DbRevisionMode.Disabled, DbReplicationMode.Automatic);
		}

		/// <summary>
		/// Creates a table in the database which can then be used to store
		/// the contents of the dictionary.
		/// </summary>
		/// <param name="infrastructure">The infrastructure.</param>
		/// <param name="transaction">The transaction.</param>
		/// <param name="tableName">Name of the table.</param>
		/// <param name="category">The table category.</param>
		/// <param name="revisionMode">The table revision mode.</param>
		/// <param name="replicationMode">The table replication mode.</param>
		public static void CreateTable(DbInfrastructure infrastructure, DbTransaction transaction, string tableName, DbElementCat category, DbRevisionMode revisionMode, DbReplicationMode replicationMode)
		{
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

		private DbInfrastructure				infrastructure;
		private DbTable							table;
		private DbRichCommand					command;
		private System.Data.DataSet				dataSet;
		private System.Data.DataTable			dataTable;
		private int								changeCount;
	}
}
