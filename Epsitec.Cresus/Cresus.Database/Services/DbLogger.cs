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

			DbColumn[] columns = new DbColumn[]
		    {
		        new DbColumn (Tags.ColumnId, types.KeyId, DbColumnClass.KeyId, DbElementCat.Internal, DbRevisionMode.Immutable)
		        {
		            IsAutoIncremented = true,
		        },
		        new DbColumn (Tags.ColumnConnectionId, types.KeyId, DbColumnClass.Data, DbElementCat.Internal, DbRevisionMode.IgnoreChanges),
		        new DbColumn (Tags.ColumnDateTime, types.DateTime, DbColumnClass.Data, DbElementCat.Internal, DbRevisionMode.IgnoreChanges)
		        {
		            IsAutoTimeStampOnInsert = true,
		        },
		        new DbColumn (Tags.ColumnSequenceNumber, types.KeyId, DbColumnClass.KeyId, DbElementCat.Internal, DbRevisionMode.IgnoreChanges)
		        {
		            IsAutoIncremented = true,
		        },
		    };

			table.DefineCategory (DbElementCat.Internal);
			table.Columns.AddRange (columns);
			table.DefinePrimaryKey (columns[0]);

			table.UpdatePrimaryKeyInfo ();
			table.UpdateRevisionMode ();

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
                
		
	}


}
