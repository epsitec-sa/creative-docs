using Epsitec.Cresus.Database.Collections;

using System.Collections.Generic;


namespace Epsitec.Cresus.Database.Services
{
	
	
	public sealed class DbLogger : DbAbstractAttachable
	{
		
		
		internal DbLogger() : base()
		{
		}


		public DbLogEntry CreateLogEntry(DbId connectionId)
		{
			this.CheckIsAttached ();

			DbColumn columnConnectionId = this.DbTable.Columns[Tags.ColumnConnectionId];
						
			SqlFieldList fieldsToInsert = new SqlFieldList ()
			{
				this.DbInfrastructure.CreateSqlFieldFromAdoValue (columnConnectionId, connectionId.Value)
			};

			long entryId = this.AddRow (fieldsToInsert);

			return this.GetLogEntry (new DbId (entryId));
		}


		public DbLogEntry GetLogEntry(DbId entryId)
		{
			this.CheckIsAttached ();

			using (DbTransaction transaction = this.DbInfrastructure.InheritOrBeginTransaction (DbTransactionMode.ReadOnly))
			{
				if (!this.LogEntryExists (entryId))
				{
					throw new System.InvalidOperationException ("The log entry does not exist.");
				}

				List<DbColumn> dbColumnsToRetrieve = new List<DbColumn> ()
				{
					this.DbTable.Columns[Tags.ColumnConnectionId],
					this.DbTable.Columns[Tags.ColumnDateTime],
				};

				SqlFieldList conditions = new SqlFieldList ()
				{
					this.CreateConditionForEntryId (entryId),
				};

				List<object> data = this.GetRowValues (dbColumnsToRetrieve, conditions);

				transaction.Commit ();

				DbId connectionId = new DbId ((long) data[0]);
				System.DateTime dateTime = (System.DateTime) data[1];

				return new DbLogEntry (entryId, connectionId, dateTime);
			}
		}


		public void RemoveLogEntry(DbId entryId)
		{
			this.CheckIsAttached ();

			SqlFieldList conditions = new SqlFieldList ()
			{
				this.CreateConditionForEntryId (entryId),
			};

			this.RemoveRows (conditions);
		}


		public bool LogEntryExists(DbId entryId)
		{
			this.CheckIsAttached ();

			SqlFieldList conditions = new SqlFieldList ()
			{
				this.CreateConditionForEntryId (entryId),
			};

			return this.RowExists (conditions);
		}


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
