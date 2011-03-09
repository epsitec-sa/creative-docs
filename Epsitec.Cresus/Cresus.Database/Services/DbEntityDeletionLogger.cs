using System.Collections.Generic;

using System.Linq;


namespace Epsitec.Cresus.Database.Services
{


	/// <summary>
	/// The <c>DbEntityDeletionLogger</c> class is used to manage the <c>CR_EDL</c> table in the database,
	/// that contains the log entries used to archive data about the deleted entities.
	/// </summary>
	public sealed class DbEntityDeletionLogger : DbAbstractTableService
	{
		
		
		// TODO Comment this class.
		// Marc


		internal DbEntityDeletionLogger(DbInfrastructure dbInfrastructure)
		    : base (dbInfrastructure)
		{
		}


		internal override string GetDbTableName()
		{
			return Tags.TableEntityDeletionLog;
		}


		internal override DbTable CreateDbTable()
		{
			DbInfrastructure.TypeHelper types = this.DbInfrastructure.TypeManager;

			DbTable table = new DbTable (Tags.TableEntityDeletionLog);


			DbColumn columnId = new DbColumn (Tags.ColumnId, types.KeyId, DbColumnClass.KeyId, DbElementCat.Internal)
			{
				IsAutoIncremented = true
			};

			DbColumn columnRefLog = new DbColumn (Tags.ColumnRefLog, types.KeyId, DbColumnClass.RefInternal, DbElementCat.Internal);
			DbColumn columnInstanceType = new DbColumn (Tags.ColumnInstanceType, types.KeyId, DbColumnClass.Data, DbElementCat.Internal);
			DbColumn columnRefId = new DbColumn (Tags.ColumnRefId, types.KeyId, DbColumnClass.Data, DbElementCat.Internal);

			table.Columns.Add (columnId);
			table.Columns.Add (columnRefLog);
			table.Columns.Add (columnInstanceType);
			table.Columns.Add (columnRefId);
			
			table.DefineCategory (DbElementCat.Internal);
			
			table.DefinePrimaryKey (columnId);
			table.UpdatePrimaryKeyInfo ();

			table.AddIndex ("IDX_EDL_REF_LOG", SqlSortOrder.Descending, columnRefLog);

			return table;
		}
		
		
		public DbEntityDeletionLogEntry CreateEntityDeletionLogEntry(DbId logEntryId, long instanceType, DbId entityId)
		{
			this.CheckIsTurnedOn ();

			IDictionary<string, object> columnNamesToValues = new Dictionary<string, object> ()
			{
			    { Tags.ColumnRefLog, logEntryId.Value },
				{ Tags.ColumnInstanceType, instanceType },
				{ Tags.ColumnRefId, entityId },
			};

			IList<object> data = this.AddRow (columnNamesToValues);

			return DbEntityDeletionLogEntry.CreateDbEntityDeletionEntry (data);
		}


		public IEnumerable<DbEntityDeletionLogEntry> GetEntityDeletionLogEntries(DbId minimumLogId)
		{
			this.CheckIsTurnedOn ();
			
			SqlFunction condition = this.CreateConditionMinimumLogEntryId (minimumLogId);

			IList<IList<object>> data = this.GetRowValues (condition);

			return data.Select (d => DbEntityDeletionLogEntry.CreateDbEntityDeletionEntry (d));
		}


		private SqlFunction CreateConditionMinimumLogEntryId(DbId minimumLogId)
		{
			string columnName = this.DbTable.Columns[Tags.ColumnRefLog].GetSqlName ();

			SqlFunction condition = new SqlFunction
			(
				SqlFunctionCode.CompareGreaterThanOrEqual,
				SqlField.CreateName (columnName),
				SqlField.CreateConstant (minimumLogId.Value, DbRawType.Int64)
			);

			return condition;
		}


	}


}
