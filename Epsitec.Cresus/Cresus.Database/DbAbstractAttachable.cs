using Epsitec.Common.Support.Extensions;

using Epsitec.Cresus.Database.Collections;

using System.Collections.Generic;

using System.Data;


namespace Epsitec.Cresus.Database
{


	// TODO Comment what is not commented in this class.
	// Marc


	/// <summary>
	/// The <c>DbAbstractAttachable</c> class provides the basic functions for classes that must deal
	/// with a single <see cref="DbTable"/> in the database.
	/// </summary>
	public class DbAbstractAttachable : IAttachable
	{

		/// <summary>
		/// Builds a new instance of <c>DbAbstractAttachable</c>
		/// </summary>
		public DbAbstractAttachable()
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
		/// The <see cref="DbTable"/> used to store the counters data.
		/// </summary>
		protected DbTable DbTable
		{
			get;
			private set;
		}
		

		#region IAttachable Members

		
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


		#endregion
		

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


		protected long AddRow(SqlFieldList fieldsToInsert)
		{
			using (DbTransaction transaction = this.DbInfrastructure.InheritOrBeginTransaction (DbTransactionMode.ReadWrite))
			{
				DbColumn columnId = this.DbTable.Columns[Tags.ColumnId];

				SqlFieldList fieldsToReturn = new Collections.SqlFieldList ()
				{
					new SqlField() { Alias = columnId.GetSqlName (), },
				};

				transaction.SqlBuilder.InsertData (this.DbTable.GetSqlName (), fieldsToInsert, fieldsToReturn);

				object id = this.DbInfrastructure.ExecuteScalar (transaction);

				transaction.Commit ();

				return (long) id;
			}
		}


		protected void RemoveRow(SqlFieldList conditions)
		{
			using (DbTransaction transaction = this.DbInfrastructure.InheritOrBeginTransaction (DbTransactionMode.ReadWrite))
			{
				transaction.SqlBuilder.RemoveData (this.DbTable.GetSqlName (), conditions);

				this.DbInfrastructure.ExecuteSilent (transaction);

				transaction.Commit ();
			}
		}


		protected bool RowExists(SqlFieldList conditions)
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
				query.Conditions.AddRange (conditions);

				transaction.SqlBuilder.SelectData (query);

				object value = this.DbInfrastructure.ExecuteScalar (transaction);

				transaction.Commit ();

				return (value != null) && (((int) value) > 0);
			}
		}


		protected object GetRowValue(DbColumn dbColumn, SqlFieldList conditions)
		{
			
			using (DbTransaction transaction = this.DbInfrastructure.InheritOrBeginTransaction (DbTransactionMode.ReadOnly))
			{
				SqlSelect query = new SqlSelect ();
				query.Tables.Add ("t", SqlField.CreateName (this.DbTable.GetSqlName ()));
				query.Fields.Add ("c", SqlField.CreateName ("t", dbColumn.GetSqlName ()));
				query.Conditions.AddRange (conditions);

				transaction.SqlBuilder.SelectData (query);

				DataTable table = this.DbInfrastructure.ExecuteSqlSelect (transaction, query, 0);

				transaction.Commit ();

				return table.Rows[0]["c"];
			}
		}


		protected void SetRowValue(SqlFieldList fields, SqlFieldList conditions)
		{
			using (DbTransaction transaction = this.DbInfrastructure.InheritOrBeginTransaction (DbTransactionMode.ReadWrite))
			{
				transaction.SqlBuilder.UpdateData (this.DbTable.GetSqlName (), fields, conditions);

				this.DbInfrastructure.ExecuteNonQuery (transaction);

				transaction.Commit ();
			}
		}


	}


}
