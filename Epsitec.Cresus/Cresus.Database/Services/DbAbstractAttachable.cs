using Epsitec.Common.Support.Extensions;

using Epsitec.Cresus.Database.Collections;

using System.Collections.Generic;

using System.Data;

using System.Linq;


namespace Epsitec.Cresus.Database.Services
{


	/// <summary>
	/// The <c>DbAbstractAttachable</c> class provides the basic functions for classes that must deal
	/// with a single <see cref="DbTable"/> in the database.
	/// </summary>
	public class DbAbstractAttachable
	{


		/// <summary>
		/// Builds a new instance of <c>DbAbstractAttachable</c>
		/// </summary>
		internal DbAbstractAttachable()
		{
			this.IsAttached = false;
		}


		/// <summary>
		/// The state of this instance.
		/// </summary>
		protected bool IsAttached
		{
			get;
			private set;
		}


		/// <summary>
		/// The <see cref="DbInfrastructure"/> object to use to communicate with the database.
		/// </summary>
		protected DbInfrastructure DbInfrastructure
		{
			get;
			private set;
		}


		/// <summary>
		/// The <see cref="DbTable"/> used by this instance to store its data.
		/// </summary>
		protected DbTable DbTable
		{
			get;
			private set;
		}
		

		/// <summary>
		/// Attaches this instance to the specified database table.
		/// </summary>
		/// <param name="dbInfrastructure">The infrastructure.</param>
		/// <param name="dbTable">The database table.</param>
		/// <exception cref="System.ArgumentNullException">If <paramref name="dbInfrastructure" /> is <c>null</c>.</exception>
		/// <exception cref="System.ArgumentNullException">If <paramref name="dbTable" /> is <c>null</c>.</exception>
		public void Attach(DbInfrastructure dbInfrastructure, DbTable dbTable)
		{			
			dbInfrastructure.ThrowIfNull ("dbInfrastructure");
			dbTable.ThrowIfNull ("dbTable");

			this.DbInfrastructure = dbInfrastructure;
			this.DbTable = dbTable;

			this.IsAttached = true;
		}


		/// <summary>
		/// Detaches this instance from the database.
		/// </summary>
		public void Detach()
		{
			this.IsAttached = false;

			this.DbInfrastructure = null;
			this.DbTable = null;
		}
		

		/// <summary>
		/// Checks that this instance is attached to a <see cref="DbInfrastructure"/>.
		/// </summary>
		/// <exception cref="System.InvalidOperationException">If this instance is not attached.</exception>
		protected void CheckIsAttached()
		{
			if (!this.IsAttached)
			{
				throw new System.InvalidOperationException ("Cannot use this instance because it is detached.");
			}
		}


		/// <summary>
		/// Adds a new row to the table in the database with the given values and returns the value
		/// of each column of the new row.
		/// </summary>
		/// <param name="columnNamesToValues">The mapping between the column names and their values.</param>
		/// <returns>The list of values of the columns in the new row, sorted by column order in the table.</returns>
		protected IList<object> AddRow(IDictionary<string, object> columnNamesToValues)
		{
			using (DbTransaction transaction = this.DbInfrastructure.InheritOrBeginTransaction (DbTransactionMode.ReadWrite))
			{
				SqlFieldList fieldsToInsert = new SqlFieldList ();
				SqlFieldList fieldsToReturn = new SqlFieldList ();

				foreach (var item in columnNamesToValues)
				{
					DbColumn dbColumn = this.DbTable.Columns[item.Key];
					SqlField sqlField = this.DbInfrastructure.CreateSqlFieldFromAdoValue (dbColumn, item.Value);

					fieldsToInsert.Add (sqlField);
				}

				foreach (DbColumn dbColumn in this.DbTable.Columns)
				{
					SqlField sqlField = new SqlField ()
					{
						Alias = dbColumn.GetSqlName ()
					};

					fieldsToReturn.Add (sqlField);
				}

				transaction.SqlBuilder.InsertData (this.DbTable.GetSqlName (), fieldsToInsert, fieldsToReturn);

				IList<object> outputParameters = this.DbInfrastructure.ExecuteOutputParameters (transaction);

				transaction.Commit ();

				return outputParameters;
			}
		}


		/// <summary>
		/// Removes the rows from the table in the database that match the given conditions.
		/// </summary>
		/// <param name="conditions">The conditions defining which rows will be deleted.</param>
		protected void RemoveRows(params SqlFunction[] conditions)
		{
			using (DbTransaction transaction = this.DbInfrastructure.InheritOrBeginTransaction (DbTransactionMode.ReadWrite))
			{
				SqlFieldList c = new SqlFieldList ();
				c.AddRange (conditions.Select (f => SqlField.CreateFunction (f)));

				transaction.SqlBuilder.RemoveData (this.DbTable.GetSqlName (), c);

				this.DbInfrastructure.ExecuteSilent (transaction);

				transaction.Commit ();
			}
		}


		/// <summary>
		/// Tells whether a row which satisfies some conditions exists in the database.
		/// </summary>
		/// <param name="conditions">The condition that the rows must satisfy.</param>
		/// <returns><c>true</c> if there is at least one row that satisfies the conditions, <c>false</c> if there is none.</returns>
		protected bool RowExists(params SqlFunction[] conditions)
		{
			using (DbTransaction transaction = this.DbInfrastructure.InheritOrBeginTransaction (DbTransactionMode.ReadOnly))
			{
				SqlSelect query = new SqlSelect ();

				query.Tables.Add (SqlField.CreateName (this.DbTable.GetSqlName ()));
				query.Fields.Add
				(
					SqlField.CreateAggregate
					(
						SqlAggregateFunction.Count,
						SqlField.CreateName (Tags.ColumnId)
					)
				);
				query.Conditions.AddRange (conditions.Select (f => SqlField.CreateFunction (f)));

				transaction.SqlBuilder.SelectData (query);

				object value = this.DbInfrastructure.ExecuteScalar (transaction);

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
		protected IList<IList<object>> GetRowValues(params SqlFunction[] conditions)
		{
			using (DbTransaction transaction = this.DbInfrastructure.InheritOrBeginTransaction (DbTransactionMode.ReadOnly))
			{
				SqlSelect query = new SqlSelect ();

				query.Tables.Add (SqlField.CreateName (this.DbTable.GetSqlName ()));

				foreach (DbColumn dbColumn in this.DbTable.Columns)
				{
					query.Fields.Add (SqlField.CreateName (dbColumn.GetSqlName ()));
				}

				query.Conditions.AddRange (conditions.Select (f => SqlField.CreateFunction (f)));

				transaction.SqlBuilder.SelectData (query);

				DataTable table = this.DbInfrastructure.ExecuteSqlSelect (transaction, query, 0);

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
		protected int SetRowValues(IDictionary<string, object> columnNamesToValues, params SqlFunction[] conditions)
		{
			using (DbTransaction transaction = this.DbInfrastructure.InheritOrBeginTransaction (DbTransactionMode.ReadWrite))
			{
				SqlFieldList fields = new SqlFieldList ();

				foreach (var item in columnNamesToValues)
				{
					DbColumn dbColumn = this.DbTable.Columns[item.Key];
					SqlField sqlField = this.DbInfrastructure.CreateSqlFieldFromAdoValue (dbColumn, item.Value);

					fields.Add (sqlField);
				}

				SqlFieldList c = new SqlFieldList ();

				c.AddRange (conditions.Select (f => SqlField.CreateFunction (f)));

				transaction.SqlBuilder.UpdateData (this.DbTable.GetSqlName (), fields, c);

				object nbRowsAffected = this.DbInfrastructure.ExecuteNonQuery (transaction);

				transaction.Commit ();

				return (int) nbRowsAffected;
			}
		}


	}


}
