using Epsitec.Common.Support.Extensions;

using System.Collections.Generic;

using System.Linq;


namespace Epsitec.Cresus.Database.Services
{


	/// <summary>
	/// The <c>DbInfoManager</c> class provides access to the table that stores informations about
	/// the database. Informations are provided as key/value pairs of string.
	/// </summary>
	public sealed class DbInfoManager : DbAbstractTableService
	{


		// TODO Comment this class.
		// Marc


		/// <summary>
		/// Builds a new <c>DbInfoManager</c>.
		/// </summary>
		internal DbInfoManager(DbInfrastructure dbInfrastructure)
			: base (dbInfrastructure)
		{
		}


		internal override string GetDbTableName()
		{
			return Tags.TableInfo;
		}


		internal override DbTable CreateDbTable()
		{
			DbInfrastructure.TypeHelper types = this.DbInfrastructure.TypeManager;

			DbTable table = new DbTable (Tags.TableInfo);

			DbColumn columnId = new DbColumn (Tags.ColumnId, types.KeyId, DbColumnClass.KeyId, DbElementCat.Internal)
			{
				IsAutoIncremented = true
			};

			DbColumn columnKey = new DbColumn (Tags.ColumnKey, types.DefaultString, DbColumnClass.Data, DbElementCat.Internal);
			DbColumn columnValue = new DbColumn (Tags.ColumnValue, types.DefaultString, DbColumnClass.Data, DbElementCat.Internal);

			table.Columns.Add (columnId);
			table.Columns.Add (columnKey);
			table.Columns.Add (columnValue);

			table.DefineCategory (DbElementCat.Internal);

			table.DefinePrimaryKey (columnId);
			table.UpdatePrimaryKeyInfo ();

			table.AddIndex ("IDX_INFO_KEY", SqlSortOrder.Ascending, columnKey);

			return table;
		}


		/// <summary>
		/// Checks whether the information corresponding to the given key exists in the database.
		/// </summary>
		/// <param name="key">The key of the information.</param>
		/// <returns><c>true</c> if the information exists, <c>false</c> if it does not.</returns>
		/// <exception cref="System.InvalidOperationException">If this instance is not attached.</exception>
		/// <exception cref="System.ArgumentException">If <paramref name="key"/> is <c>null</c> or empty.</exception>
		public bool ExistsInfo(string key)
		{
			this.CheckIsTurnedOn ();

			key.ThrowIfNullOrEmpty ("key");

			return this.ExistsValue (key);
		}


		/// <summary>
		/// Sets the value of an information in the database. To remove an information, call this
		/// method with <paramref name="value"/> with the value <c>null</c>.
		/// </summary>
		/// <param name="key">The key of the information.</param>
		/// <param name="value">The new value of the information.</param>
		/// <exception cref="System.InvalidOperationException">If this instance is not attached.</exception>
		/// <exception cref="System.ArgumentException">If <paramref name="key"/> is <c>null</c> or empty.</exception>
		public void SetInfo(string key, string value)
		{
			this.CheckIsTurnedOn ();

			key.ThrowIfNullOrEmpty ("key");

			using (DbTransaction transaction = this.DbInfrastructure.InheritOrBeginTransaction(DbTransactionMode.ReadWrite))
			{
				if (value == null)
				{
					this.RemoveValue (key);
				}
				else
				{
					int nbRowsAffected = this.SetValue (key, value);

					if (nbRowsAffected == 0)
					{
						this.InsertValue (key, value);
					}
				}

				transaction.Commit ();
			}
		}


		/// <summary>
		/// Gets an information out of the database.
		/// </summary>
		/// <param name="key">The key of the information to get.</param>
		/// <returns>The value of the information.</returns>
		/// <exception cref="System.InvalidOperationException">If this instance is not attached.</exception>
		/// <exception cref="System.ArgumentException">If <paramref name="key"/> is <c>null</c> or empty.</exception>
		public string GetInfo(string key)
		{
			this.CheckIsTurnedOn ();

			key.ThrowIfNullOrEmpty ("key");

			return this.GetValue (key);
		}


		/// <summary>
		/// Inserts a key/value pair of information in the database.
		/// </summary>
		/// <param name="key">The key of the information.</param>
		/// <param name="value">The value of the information.</param>
		private void InsertValue(string key, string value)
		{
			IDictionary<string, object> columnNamesToValues = new Dictionary<string, object> ()
			{
				 {Tags.ColumnKey, key},
				 {Tags.ColumnValue, value},
			};

			this.AddRow (columnNamesToValues);
		}


		/// <summary>
		/// Removes a key/value pair of information from the database.
		/// </summary>
		/// <param name="key">The key of the information.</param>
		private void RemoveValue(string key)
		{
			SqlFunction condition = this.CreateConditionForValueKey (key);

			this.RemoveRows (condition);
		}


		/// <summary>
		/// Checks whether the information corresponding to the given key exists in the database.
		/// </summary>
		/// <param name="key">The key of the information.</param>
		/// <returns><c>true</c> if the information exists, <c>false</c> if it does not.</returns>
		private bool ExistsValue(string key)
		{
			SqlFunction condition = this.CreateConditionForValueKey (key);

			return this.RowExists (condition);
		}


		/// <summary>
		/// Gets the value of a key/value pair of information from the database.
		/// </summary>
		/// <param name="key">The key of the information.</param>
		/// <returns>The value of the information.</returns>
		private string GetValue(string key)
		{
			SqlFunction condition = this.CreateConditionForValueKey (key);

			var data = this.GetRowValues (condition);

			return data.Any () ? (string) data[0][2] : null;
		}


		/// <summary>
		/// Sets the value of a key/value pair of information in the database.
		/// </summary>
		/// <param name="key">The key of the information.</param>
		/// <param name="value">The new value of the information.</param>
		private int SetValue(string key, string value)
		{
			IDictionary<string, object> columNamesToValues = new Dictionary<string, object> ()
			{
				 {Tags.ColumnValue, value},
			};
			
			SqlFunction condition = this.CreateConditionForValueKey (key);

			return this.SetRowValues (columNamesToValues, condition);
		}


		/// <summary>
		/// Creates the <see cref="SqlFunction"/> object that describes the condition that returns
		/// true for a given key of information.
		/// </summary>
		/// <param name="key">The key to match.</param>
		/// <returns>The <see cref="SqlFunction"/> object.</returns>
		private SqlFunction CreateConditionForValueKey(string key)
		{
			return new SqlFunction
			(
				SqlFunctionCode.CompareEqual,
				SqlField.CreateName (this.DbTable.Columns[Tags.ColumnKey].GetSqlName ()),
				SqlField.CreateConstant (key, DbRawType.String)
			);
		}


	}


}
