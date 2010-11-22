using Epsitec.Cresus.Database.Collections;

using System.Collections.Generic;

using System.Linq;


namespace Epsitec.Cresus.Database.Services
{
	
	
	/// <summary>
	/// The <c>DbLogger</c> class is used to manage the <c>CR_LOG</c> table in the database, that
	/// contains the log entries used to archive who has done what and when in the database.
	/// </summary>
	public sealed class DbLogger : DbAbstractAttachable
	{
		
		
		/// <summary>
		/// Creates a new <c>DbLogger</c>.
		/// </summary>
		internal DbLogger() : base()
		{
		}


		/// <summary>
		/// Inserts a fresh new log entry in the database.
		/// </summary>
		/// <param name="connectionId">The <see cref="DbId"/> of the connection referenced by the new entry.</param>
		/// <returns>The data of the new entry.</returns>
		public DbLogEntry CreateLogEntry(DbId connectionId)
		{
			this.CheckIsAttached ();

			IDictionary<string, object> columnNamesToValues = new Dictionary<string, object> ()
			{
				{ Tags.ColumnConnectionId, connectionId.Value },
			};

			IList<object> data = this.AddRow (columnNamesToValues);

			return this.CreateDbLogEntry (data);
		}


		/// <summary>
		/// Gets a log entry in the database.
		/// </summary>
		/// <param name="entryId">The <see cref="DbId"/> of the entry to get.</param>
		/// <returns>The data of the entry.</returns>
		public DbLogEntry GetLogEntry(DbId entryId)
		{
			this.CheckIsAttached ();

			using (DbTransaction transaction = this.DbInfrastructure.InheritOrBeginTransaction (DbTransactionMode.ReadOnly))
			{
				SqlFieldList conditions = new SqlFieldList ()
				{
					this.CreateConditionForEntryId (entryId),
				};

				var data = this.GetRows (conditions);

				transaction.Commit ();

				return data.Any () ? this.CreateDbLogEntry (data.First ()) : null;
			}
		}


		/// <summary>
		/// Removes a log entry from the database.
		/// </summary>
		/// <param name="entryId">The <see cref="DbId"/> of the entry to remove.</param>
		public void RemoveLogEntry(DbId entryId)
		{
			this.CheckIsAttached ();

			SqlFieldList conditions = new SqlFieldList ()
			{
				this.CreateConditionForEntryId (entryId),
			};

			this.RemoveRows (conditions);
		}


		/// <summary>
		/// Tells whether a log entry exists in the database.
		/// </summary>
		/// <param name="entryId">The <see cref="DbId"/> of the entry whose existence to check.</param>
		/// <returns><c>true</c> if a log entry with the given <see cref="DbId"/> exists, <c>false</c> if there is n't.</returns>
		public bool LogEntryExists(DbId entryId)
		{
			this.CheckIsAttached ();

			SqlFieldList conditions = new SqlFieldList ()
			{
				this.CreateConditionForEntryId (entryId),
			};

			return this.RowExists (conditions);
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


		/// <summary>
		/// Creates a new instance of <see cref="DbLogEntry"/> based on the given data.
		/// </summary>
		/// <param name="data">The data of the log entry.</param>
		/// <returns>The <see cref="DbLogEntry"/>.</returns>
		private DbLogEntry CreateDbLogEntry(IList<object> data)
		{
			DbId entryId = new DbId ((long) data[0]);
			DbId connectionId = new DbId ((long) data[1]);
			System.DateTime dateTime = (System.DateTime) data[2];
			long sequenceNumber = (long) data[3];

			return new DbLogEntry (entryId, connectionId, dateTime, sequenceNumber);
		}
                
		
	}


}
