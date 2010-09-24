using Epsitec.Common.Support.Extensions;

using Epsitec.Cresus.Database.Collections;


namespace Epsitec.Cresus.Database
{


	/// <summary>
	/// The <c>DbInfoManager</c> class provides access to the table that stores informations about
	/// the database. Informations are provided as key/value pairs of string.
	/// </summary>
	public sealed class DbInfoManager : DbAbstractAttachable
	{


		/// <summary>
		/// Builds a new <c>DbInfoManager</c>.
		/// </summary>
		internal DbInfoManager() : base ()
		{
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
			this.CheckIsAttached ();

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
			this.CheckIsAttached ();

			key.ThrowIfNullOrEmpty ("key");

			using (DbTransaction transaction = this.DbInfrastructure.InheritOrBeginTransaction(DbTransactionMode.ReadWrite))
			{
				if (value == null)
				{
					if (this.ExistsInfo (key))
					{
						this.RemoveValue (key);
					}
				}
				else
				{
					if (this.ExistsInfo (key))
					{
						this.SetValue (key, value);
					}
					else
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
			this.CheckIsAttached ();

			key.ThrowIfNullOrEmpty ("key");

			using (DbTransaction transaction = this.DbInfrastructure.InheritOrBeginTransaction (DbTransactionMode.ReadWrite))
			{
				string value = null;
				
				if (this.ExistsInfo (key))
				{
					value = this.GetValue (key);
				}
				
				transaction.Commit ();

				return value;
			}
		}


		/// <summary>
		/// Inserts a key/value pair of information in the database.
		/// </summary>
		/// <param name="key">The key of the information.</param>
		/// <param name="value">The value of the information.</param>
		private void InsertValue(string key, string value)
		{
			SqlFieldList fields = new SqlFieldList ();

			DbColumn columnKey = this.DbTable.Columns[Tags.ColumnKey];
			DbColumn columnValue = this.DbTable.Columns[Tags.ColumnValue];

			fields.Add (this.DbInfrastructure.CreateSqlFieldFromAdoValue (columnKey, key));
			fields.Add (this.DbInfrastructure.CreateSqlFieldFromAdoValue (columnValue, value));

			this.AddRow (fields);
		}


		/// <summary>
		/// Removes a key/value pair of information from the database.
		/// </summary>
		/// <param name="key">The key of the information.</param>
		private void RemoveValue(string key)
		{
			SqlFieldList conditions = new SqlFieldList ()
			{
				this.CreateConditionForValueKey (key),
			};

			this.RemoveRows (conditions);
		}


		/// <summary>
		/// Checks whether the information corresponding to the given key exists in the database.
		/// </summary>
		/// <param name="key">The key of the information.</param>
		/// <returns><c>true</c> if the information exists, <c>false</c> if it does not.</returns>
		private bool ExistsValue(string key)
		{
			SqlFieldList conditions = new SqlFieldList ()
            {
            	this.CreateConditionForValueKey (key),
            };

			return this.RowExists (conditions);
		}


		/// <summary>
		/// Gets the value of a key/value pair of information from the database.
		/// </summary>
		/// <param name="key">The key of the information.</param>
		/// <returns>The value of the information.</returns>
		private string GetValue(string key)
		{
			DbColumn column = this.DbTable.Columns[Tags.ColumnValue];

			SqlFieldList conditions = new SqlFieldList ()
			{
				this.CreateConditionForValueKey (key),
			};

			return (string) this.GetRowValue (column, conditions);
		}


		/// <summary>
		/// Sets the value of a key/value pair of information in the database.
		/// </summary>
		/// <param name="key">The key of the information.</param>
		/// <param name="value">The new value of the information.</param>
		private void SetValue(string key, string value)
		{
			DbColumn column = this.DbTable.Columns[Tags.ColumnValue];

			SqlFieldList fields = new SqlFieldList ()
		    {
		        this.DbInfrastructure.CreateSqlFieldFromAdoValue (column, value)
		    };

			SqlFieldList conditions = new SqlFieldList ()
			{
				this.CreateConditionForValueKey (key),
			};

			this.SetRowValue (fields, conditions);
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
