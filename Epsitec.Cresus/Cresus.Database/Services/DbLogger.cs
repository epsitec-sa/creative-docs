using Epsitec.Cresus.Database.Collections;

using System.Collections.Generic;

using System.Linq;


namespace Epsitec.Cresus.Database.Services
{
	
	
	/// <summary>
	/// The <c>DbLogger</c> class is used to manage the <c>CR_LOG</c> table in the database, that
	/// contains the log entries used to archive who has done what and when in the database.
	/// </summary>
	public sealed class DbLogger : DbAbstractTableService
	{


		// TODO Comment this class.
		// Marc


		/// <summary>
		/// Creates a new <c>DbLogger</c>.
		/// </summary>
		internal DbLogger(DbInfrastructure dbInfrastructure)
			: base (dbInfrastructure)
		{
		}


		internal override string GetDbTableName()
		{
			return Tags.TableLog;
		}


		internal override DbTable CreateDbTable()
		{
			DbInfrastructure.TypeHelper types = this.DbInfrastructure.TypeManager;

			DbTable table = new DbTable (Tags.TableLog);

			DbColumn columnId = new DbColumn (Tags.ColumnId, types.KeyId, DbColumnClass.KeyId, DbElementCat.Internal)
			{
				IsAutoIncremented = true
			};
			
			DbColumn columnConnectionId = new DbColumn (Tags.ColumnConnectionId, types.KeyId, DbColumnClass.Data, DbElementCat.Internal);
			
			DbColumn columnDateTime = new DbColumn (Tags.ColumnDateTime, types.DateTime, DbColumnClass.Data, DbElementCat.Internal)
			{
				IsAutoTimeStampOnInsert = true
			};

			table.Columns.Add (columnId);
			table.Columns.Add (columnConnectionId);
			table.Columns.Add (columnDateTime);

			table.DefineCategory (DbElementCat.Internal);
			
			table.DefinePrimaryKey (columnId);
			table.UpdatePrimaryKeyInfo ();

			table.AddIndex ("IDX_LOG_ID", SqlSortOrder.Descending, columnId);

			return table;
		}


		/// <summary>
		/// Inserts a fresh new log entry in the database.
		/// </summary>
		/// <param name="connectionId">The <see cref="DbId"/> of the connection referenced by the new entry.</param>
		/// <returns>The data of the new entry.</returns>
		public DbLogEntry CreateLogEntry(DbId connectionId)
		{
			this.CheckIsTurnedOn ();

			IDictionary<string, object> columnNamesToValues = new Dictionary<string, object> ()
			{
				{ Tags.ColumnConnectionId, connectionId.Value },
			};

			IList<object> data = this.AddRow (columnNamesToValues);

			return DbLogEntry.CreateDbLogEntry (data);
		}


		/// <summary>
		/// Gets a log entry in the database.
		/// </summary>
		/// <param name="entryId">The <see cref="DbId"/> of the entry to get.</param>
		/// <returns>The data of the entry.</returns>
		public DbLogEntry GetLogEntry(DbId entryId)
		{
			this.CheckIsTurnedOn ();

			SqlFunction condition = this.CreateConditionForEntryId (entryId);

			return this.GetLogEntry (condition);
		}


		public DbLogEntry GetLatestLogEntry()
		{
			this.CheckIsTurnedOn ();

			SqlFunction condition = this.CreateConditionForlatestEntry ();

			return this.GetLogEntry (condition);
		}


		private DbLogEntry GetLogEntry(SqlFunction condition)
		{
			var data = this.GetRowValues (condition);

			return data.Any () ? DbLogEntry.CreateDbLogEntry (data.First ()) : null;
		}



		/// <summary>
		/// Removes a log entry from the database.
		/// </summary>
		/// <param name="entryId">The <see cref="DbId"/> of the entry to remove.</param>
		public void RemoveLogEntry(DbId entryId)
		{
			this.CheckIsTurnedOn ();

			SqlFunction condition = this.CreateConditionForEntryId (entryId);

			this.RemoveRows (condition);
		}


		/// <summary>
		/// Tells whether a log entry exists in the database.
		/// </summary>
		/// <param name="entryId">The <see cref="DbId"/> of the entry whose existence to check.</param>
		/// <returns><c>true</c> if a log entry with the given <see cref="DbId"/> exists, <c>false</c> if there is n't.</returns>
		public bool LogEntryExists(DbId entryId)
		{
			this.CheckIsTurnedOn ();

			SqlFunction condition = this.CreateConditionForEntryId (entryId);

			return this.RowExists (condition);
		}


		/// <summary>
		/// Creates the <see cref="SqlFunction"/> that represents the condition which holds for a log
		/// entry when its <see cref="DbId"/> is equal to the given <see cref="DbId"/>.
		/// </summary>
		/// <param name="entryId">The <see cref="DbId"/> that the entry must have in order to satisfy the condition.</param>
		/// <returns>The <see cref="SqlFunction"/> that represents the condition.</returns>
		private SqlFunction CreateConditionForEntryId(DbId entryId)
		{
			return new SqlFunction
			(
				SqlFunctionCode.CompareEqual,
				SqlField.CreateName (this.DbTable.Columns[Tags.ColumnId].GetSqlName ()),
				SqlField.CreateConstant (entryId.Value, DbRawType.Int64)
			);
		}


		private SqlFunction CreateConditionForlatestEntry()
		{
			string tableName = this.DbTable.GetSqlName ();
			string columnName = this.DbTable.Columns[Tags.ColumnId].GetSqlName ();

			SqlSelect subQuery = new SqlSelect ();
			subQuery.Fields.Add (SqlField.CreateAggregate (SqlAggregateFunction.Max, SqlField.CreateName (columnName)));
			subQuery.Tables.Add (SqlField.CreateName (tableName));

			SqlFunction condition = new SqlFunction
			(
				SqlFunctionCode.CompareEqual,
				SqlField.CreateName (columnName),
				SqlField.CreateSubQuery (subQuery)
			);

			return condition;
		}
                            
		
	}


}
