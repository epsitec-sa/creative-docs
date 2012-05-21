using Epsitec.Cresus.Database;
using Epsitec.Cresus.Database.Collections;

using System.Collections.Generic;

using System.Data;

using System.Linq;


namespace Epsitec.Cresus.DataLayer.Infrastructure
{


	internal sealed class TableQueryHelper
	{


		// TODO Comment this class
		// Marc


		public TableQueryHelper(DbInfrastructure dbInfrastructure, DbTable dbTable)
		{
			this.dbInfrastructure = dbInfrastructure;
			this.dbTable = dbTable;
		}
		

		/// <summary>
		/// Adds a new row to the table in the database with the given values and returns the value
		/// of each column of the new row.
		/// </summary>
		/// <param name="columnNamesToValues">The mapping between the column names and their values.</param>
		/// <returns>The list of values of the columns in the new row, sorted by column order in the table.</returns>
		public IList<object> AddRow(IDictionary<string, object> columnNamesToValues)
		{
			using (DbTransaction transaction = this.dbInfrastructure.InheritOrBeginTransaction (DbTransactionMode.ReadWrite))
			{
				SqlFieldList fieldsToInsert = new SqlFieldList ();
				SqlFieldList fieldsToReturn = new SqlFieldList ();

				foreach (var item in columnNamesToValues)
				{
					DbColumn dbColumn = this.dbTable.Columns[item.Key];
					SqlField sqlField = this.dbInfrastructure.CreateSqlFieldFromAdoValue (dbColumn, item.Value);

					fieldsToInsert.Add (sqlField);
				}

				foreach (DbColumn dbColumn in this.dbTable.Columns)
				{
					SqlField sqlField = new SqlField ()
					{
						Alias = dbColumn.GetSqlName ()
					};

					fieldsToReturn.Add (sqlField);
				}

				transaction.SqlBuilder.InsertData (this.dbTable.GetSqlName (), fieldsToInsert, fieldsToReturn);

				IList<object> outputParameters = this.dbInfrastructure.ExecuteOutputParameters (transaction);

				transaction.Commit ();

				return outputParameters;
			}
		}


		/// <summary>
		/// Removes the rows from the table in the database that match the given conditions.
		/// </summary>
		/// <param name="conditions">The conditions defining which rows will be deleted.</param>
		public int RemoveRows(params SqlFunction[] conditions)
		{
			using (DbTransaction transaction = this.dbInfrastructure.InheritOrBeginTransaction (DbTransactionMode.ReadWrite))
			{
				SqlFieldList c = new SqlFieldList ();
				c.AddRange (conditions.Select (f => SqlField.CreateFunction (f)));

				transaction.SqlBuilder.RemoveData (this.dbTable.GetSqlName (), c);

				object nbRowsAffected = this.dbInfrastructure.ExecuteSilent (transaction);

				transaction.Commit ();

				return (int) nbRowsAffected;
			}
		}


		/// <summary>
		/// Tells whether a row which satisfies some conditions exists in the database.
		/// </summary>
		/// <param name="conditions">The condition that the rows must satisfy.</param>
		/// <returns><c>true</c> if there is at least one row that satisfies the conditions, <c>false</c> if there is none.</returns>
		public bool DoesRowExist(params SqlFunction[] conditions)
		{
			using (DbTransaction transaction = this.dbInfrastructure.InheritOrBeginTransaction (DbTransactionMode.ReadOnly))
			{
				SqlSelect query = new SqlSelect ();

				query.Tables.Add (SqlField.CreateName (this.dbTable.GetSqlName ()));
				query.Fields.Add
				(
					SqlField.CreateAggregate
					(
						SqlAggregateFunction.Count,
						SqlField.CreateName (this.dbTable.Columns.First ().GetSqlName ())
					)
				);
				query.Conditions.AddRange (conditions.Select (f => SqlField.CreateFunction (f)));

				transaction.SqlBuilder.SelectData (query);

				object value = this.dbInfrastructure.ExecuteScalar (transaction);

				transaction.Commit ();

				return (value != null) && (((int) value) > 0);
			}
		}


		/// <summary>
		/// Gets the column values of each row in the table in the database that satisfies the given
		/// conditions.
		/// </summary>
		/// <param name="conditions">The conditions that the rows must satisfy.</param>
		/// <returns>The value of each column of each row that satisfies the conditions.</returns>
		public IList<IList<object>> GetRows(params SqlFunction[] conditions)
		{
			using (DbTransaction transaction = this.dbInfrastructure.InheritOrBeginTransaction (DbTransactionMode.ReadOnly))
			{
				SqlSelect query = new SqlSelect ();

				query.Tables.Add (SqlField.CreateName (this.dbTable.GetSqlName ()));

				foreach (DbColumn dbColumn in this.dbTable.Columns)
				{
					query.Fields.Add (SqlField.CreateName (dbColumn.GetSqlName ()));
				}

				query.Conditions.AddRange (conditions.Select (f => SqlField.CreateFunction (f)));

				transaction.SqlBuilder.SelectData (query);

				DataTable table = this.dbInfrastructure.ExecuteSqlSelect (transaction, query, 0);

				List<IList<object>> data = new List<IList<object>> ();

				transaction.Commit ();

				foreach (DataRow row in table.Rows)
				{
					data.Add (row.ItemArray.ToList ());
				}

				return data;
			}
		}


		/// <summary>
		/// Sets some values in the rows of the table that satisfy some conditions.
		/// </summary>
		/// <param name="columnNamesToValues">The mapping between colum names and the values to assign to them.</param>
		/// <param name="conditions">The conditions defining the rows in which the values must be set.</param>
		/// <returns>The number of rows affected.</returns>
		public int SetRow(IDictionary<string, object> columnNamesToValues, params SqlFunction[] conditions)
		{
			using (DbTransaction transaction = this.dbInfrastructure.InheritOrBeginTransaction (DbTransactionMode.ReadWrite))
			{
				SqlFieldList fields = new SqlFieldList ();

				foreach (var item in columnNamesToValues)
				{
					DbColumn dbColumn = this.dbTable.Columns[item.Key];
					SqlField sqlField = this.dbInfrastructure.CreateSqlFieldFromAdoValue (dbColumn, item.Value);

					fields.Add (sqlField);
				}

				SqlFieldList c = new SqlFieldList ();

				c.AddRange (conditions.Select (f => SqlField.CreateFunction (f)));

				transaction.SqlBuilder.UpdateData (this.dbTable.GetSqlName (), fields, c);

				object nbRowsAffected = this.dbInfrastructure.ExecuteNonQuery (transaction);

				transaction.Commit ();

				return (int) nbRowsAffected;
			}
		}


		public DbTransaction CreateLockTransaction()
		{
			List<DbTable> tablesToLock = new List<DbTable> ()
		    {
		       this.dbTable
		    };

			return this.dbInfrastructure.BeginTransaction (DbTransactionMode.ReadWrite, tablesToLock);
		}


		private readonly DbInfrastructure dbInfrastructure;


		private readonly DbTable dbTable;


	}


}
