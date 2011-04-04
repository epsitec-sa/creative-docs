using Epsitec.Common.Support.Extensions;

using Epsitec.Common.Types;

using Epsitec.Cresus.Database;

using Epsitec.Cresus.DataLayer.Schema;

using System.Collections.Generic;

using System.Linq;


namespace Epsitec.Cresus.DataLayer.Infrastructure
{


	/// <summary>
	/// The <c>InfoManager</c> class provides access to the table that stores informations about
	/// the database. Informations are provided as key/value pairs of string.
	/// </summary>
	internal sealed class InfoManager
	{


		// TODO Comment this class.
		// Marc


		/// <summary>
		/// Builds a new <c>InfoManager</c>.
		/// </summary>
		public InfoManager(DbInfrastructure dbInfrastructure, ServiceSchemaEngine schemaEngine)
		{
			dbInfrastructure.ThrowIfNull ("dbInfrastructure");
			schemaEngine.ThrowIfNull ("schemaEngine");

			var table = schemaEngine.GetServiceTable (InfoManager.TableFactory.TableName);
			var tableQueryGenerator = new TableQueryHelper (dbInfrastructure, table);

			this.table = table;
			this.dbInfrastructure = dbInfrastructure;
			this.tableQueryHelper = tableQueryGenerator;
		}


		/// <summary>
		/// Checks whether the information corresponding to the given key exists in the database.
		/// </summary>
		/// <param name="key">The key of the information.</param>
		/// <returns><c>true</c> if the information exists, <c>false</c> if it does not.</returns>
		/// <exception cref="System.ArgumentException">If <paramref name="key"/> is <c>null</c> or empty.</exception>
		public bool DoesInfoExists(string key)
		{	
			key.ThrowIfNullOrEmpty ("key");

			return this.DoesValueExists (key);
		}


		/// <summary>
		/// Sets the value of an information in the database. To remove an information, call this
		/// method with <paramref name="value"/> with the value <c>null</c>.
		/// </summary>
		/// <param name="key">The key of the information.</param>
		/// <param name="value">The new value of the information.</param>
		/// <exception cref="System.ArgumentException">If <paramref name="key"/> is <c>null</c> or empty.</exception>
		public void SetInfo(string key, string value)
		{
			key.ThrowIfNullOrEmpty ("key");

			if (value == null)
			{
				this.RemoveValue (key);
			}
			else
			{
				using (DbTransaction transaction = this.tableQueryHelper.CreateLockTransaction ())
				{
					int nbRowsAffected = this.SetValue (key, value);

					if (nbRowsAffected == 0)
					{
						this.InsertValue (key, value);
					}

					transaction.Commit ();
				}
			}
		}


		/// <summary>
		/// Gets an information out of the database.
		/// </summary>
		/// <param name="key">The key of the information to get.</param>
		/// <returns>The value of the information.</returns>
		/// <exception cref="System.ArgumentException">If <paramref name="key"/> is <c>null</c> or empty.</exception>
		public string GetInfo(string key)
		{
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
				 {InfoManager.TableFactory.ColumnKeyName, key},
				 {InfoManager.TableFactory.ColumnValueName, value},
			};

			this.tableQueryHelper.AddRow (columnNamesToValues);
		}


		/// <summary>
		/// Removes a key/value pair of information from the database.
		/// </summary>
		/// <param name="key">The key of the information.</param>
		private void RemoveValue(string key)
		{
			SqlFunction condition = this.CreateConditionForValueKey (key);

			this.tableQueryHelper.RemoveRows (condition);
		}


		/// <summary>
		/// Checks whether the information corresponding to the given key exists in the database.
		/// </summary>
		/// <param name="key">The key of the information.</param>
		/// <returns><c>true</c> if the information exists, <c>false</c> if it does not.</returns>
		private bool DoesValueExists(string key)
		{
			SqlFunction condition = this.CreateConditionForValueKey (key);

			return this.tableQueryHelper.DoesRowExist (condition);
		}


		/// <summary>
		/// Gets the value of a key/value pair of information from the database.
		/// </summary>
		/// <param name="key">The key of the information.</param>
		/// <returns>The value of the information.</returns>
		private string GetValue(string key)
		{
			SqlFunction condition = this.CreateConditionForValueKey (key);

			var data = this.tableQueryHelper.GetRows (condition);

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
				 {InfoManager.TableFactory.ColumnValueName, value},
			};
			
			SqlFunction condition = this.CreateConditionForValueKey (key);

			return this.tableQueryHelper.SetRow (columNamesToValues, condition);
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
				SqlField.CreateName (this.table.Columns[InfoManager.TableFactory.ColumnKeyName].GetSqlName ()),
				SqlField.CreateConstant (key, DbRawType.String)
			);
		}


		private readonly DbTable table;


		private readonly DbInfrastructure dbInfrastructure;


		private readonly TableQueryHelper tableQueryHelper;


		public static TableBuilder TableFactory
		{
			get
			{
				return InfoManager.tableFactory;
			}
		}


		private static readonly TableBuilder tableFactory = new TableBuilder ();


		public class TableBuilder : ITableFactory
		{


			#region ITableBuilder Members


			public string TableName
			{
				get
				{
					return "CR_INFO";
				}
			}


			public DbTable BuildTable()
			{
				DbTypeDef typeKeyId = new DbTypeDef (Epsitec.Cresus.Database.Res.Types.Num.KeyId);
				DbTypeDef typeString = new DbTypeDef (StringType.Default);
				DbTypeDef typeDateTime = new DbTypeDef (Epsitec.Cresus.Database.Res.Types.Other.DateTime);
				DbTypeDef typeInteger = new DbTypeDef (IntegerType.Default);

				DbTable table = new DbTable (this.TableName);

				DbColumn columnId = new DbColumn (this.ColumnIdName, typeKeyId, DbColumnClass.KeyId, DbElementCat.Internal)
				{
					IsAutoIncremented = true
				};

				DbColumn columnKey = new DbColumn (this.ColumnKeyName, typeString, DbColumnClass.Data, DbElementCat.Internal);
				DbColumn columnValue = new DbColumn (this.ColumnValueName, typeString, DbColumnClass.Data, DbElementCat.Internal);

				table.Columns.Add (columnId);
				table.Columns.Add (columnKey);
				table.Columns.Add (columnValue);

				table.DefineCategory (DbElementCat.Internal);

				table.DefinePrimaryKey (columnId);
				table.UpdatePrimaryKeyInfo ();

				table.AddIndex ("IDX_INFO_KEY", SqlSortOrder.Ascending, columnKey);

				return table;
			}


			#endregion


			public string ColumnIdName
			{
				get
				{
					return "CR_ID";
				}
			}


			public string ColumnKeyName
			{
				get
				{
					return "CR_KEY";
				}
			}



			public string ColumnValueName
			{
				get
				{
					return "CR_VALUE";
				}
			}

		}


	}


}
