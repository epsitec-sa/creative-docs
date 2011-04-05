using Epsitec.Common.Support.Extensions;

using Epsitec.Cresus.Database;

using Epsitec.Cresus.DataLayer.Schema;

using System.Collections.Generic;

using System.Linq;
using System.Collections;


namespace Epsitec.Cresus.DataLayer.Infrastructure
{
	
	
	/// <summary>
	/// The <c>EntityModificationLog</c> class is used to manage the entity modification log table
	/// in the database, that contains the log entries used to archive who has done what and when in
	/// the database regarding entities.
	/// </summary>
	internal sealed class EntityModificationLog
	{


		// TODO Comment this class.
		// Marc


		/// <summary>
		/// Creates a new <c>EntityModificationLog</c>.
		/// </summary>
		public EntityModificationLog(DbInfrastructure dbInfrastructure, ServiceSchemaEngine schemaEngine)
		{
			dbInfrastructure.ThrowIfNull ("dbInfrastructure");
			schemaEngine.ThrowIfNull ("schemaEngine");

			var table = schemaEngine.GetServiceTable (EntityModificationLog.TableFactory.TableName);
			var tableQueryHelper = new TableQueryHelper (dbInfrastructure, table);

			this.table = table;
			this.tableQueryHelper = tableQueryHelper;
		}


		/// <summary>
		/// Inserts a fresh new log entry in the database.
		/// </summary>
		/// <param name="connectionId">The <see cref="DbId"/> of the connection referenced by the new entry.</param>
		/// <returns>The data of the new entry.</returns>
		public EntityModificationEntry CreateEntry(DbId connectionId)
		{
			connectionId.ThrowIf (id => id.IsEmpty, "connectionId cannot be empty");

			IDictionary<string, object> columnNamesToValues = new Dictionary<string, object> ()
			{
				{ EntityModificationLog.TableFactory.ColumnConnectionIdName, connectionId.Value },
			};

			IList<object> data = this.tableQueryHelper.AddRow (columnNamesToValues);

			return this.CreateEntityModificationEntryEntry (data);
		}


		/// <summary>
		/// Gets a log entry in the database.
		/// </summary>
		/// <param name="entryId">The <see cref="DbId"/> of the entry to get.</param>
		/// <returns>The data of the entry.</returns>
		public EntityModificationEntry GetEntry(DbId entryId)
		{
			entryId.ThrowIf (id => id.IsEmpty, "entryId cannot be empty");
			
			SqlFunction condition = this.CreateConditionForEntryId (entryId);

			return this.GetEntry (condition);
		}


		public EntityModificationEntry GetLatestEntry()
		{
			SqlFunction condition = this.CreateConditionForlatestEntry ();

			return this.GetEntry (condition);
		}


		/// <summary>
		/// Removes a log entry from the database.
		/// </summary>
		/// <param name="entryId">The <see cref="DbId"/> of the entry to remove.</param>
		public void DeleteEntry(DbId entryId)
		{
			entryId.ThrowIf (id => id.IsEmpty, "entryId cannot be empty");
			
			SqlFunction condition = this.CreateConditionForEntryId (entryId);

			using (DbTransaction transaction = this.tableQueryHelper.CreateLockTransaction ())
			{
				this.tableQueryHelper.RemoveRows (condition);

				transaction.Commit ();
			}
		}


		/// <summary>
		/// Tells whether a log entry exists in the database.
		/// </summary>
		/// <param name="entryId">The <see cref="DbId"/> of the entry whose existence to check.</param>
		/// <returns><c>true</c> if a log entry with the given <see cref="DbId"/> exists, <c>false</c> if there is n't.</returns>
		public bool DoesEntryExists(DbId entryId)
		{
			entryId.ThrowIf (id => id.IsEmpty, "entryId cannot be empty");
			
			SqlFunction condition = this.CreateConditionForEntryId (entryId);

			return this.tableQueryHelper.DoesRowExist (condition);
		}


		private EntityModificationEntry GetEntry(SqlFunction condition)
		{
			var data = this.tableQueryHelper.GetRows (condition);

			return data.Any () ? this.CreateEntityModificationEntryEntry (data.First ()) : null;
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
				SqlField.CreateName (this.table.Columns[EntityModificationLog.TableFactory.ColumnIdName].GetSqlName ()),
				SqlField.CreateConstant (entryId.Value, DbRawType.Int64)
			);
		}


		private SqlFunction CreateConditionForlatestEntry()
		{
			string tableName = this.table.GetSqlName ();
			string columnName = this.table.Columns[EntityModificationLog.TableFactory.ColumnIdName].GetSqlName ();

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


		/// <summary>
		/// Creates a new instance of <see cref="EntityModificationEntry"/> based on the given data.
		/// </summary>
		/// <param name="data">The data of the log entry.</param>
		/// <returns>The <see cref="EntityModificationEntry"/>.</returns>
		private EntityModificationEntry CreateEntityModificationEntryEntry(IList<object> data)
		{
			DbId entryId = new DbId ((long) data[0]);
			DbId connectionId = new DbId ((long) data[1]);
			System.DateTime dateTime = (System.DateTime) data[2];

			return new EntityModificationEntry (entryId, connectionId, dateTime);
		}


		private readonly DbTable table;


		private readonly TableQueryHelper tableQueryHelper;



		public static TableBuilder TableFactory
		{
			get
			{
				return EntityModificationLog.tableFactory;
			}
		}


		private static readonly TableBuilder tableFactory = new TableBuilder ();


		public class TableBuilder : ITableFactory
		{


			#region ITableHelper Members


			public string TableName
			{
				get
				{
					return "CR_EM_LOG";
				}
			}


			public DbTable BuildTable()
			{
				DbTypeDef typeKeyId = new DbTypeDef (Epsitec.Cresus.Database.Res.Types.Num.KeyId);
				DbTypeDef typeDateTime = new DbTypeDef (Epsitec.Cresus.Database.Res.Types.Other.DateTime);

				DbTable table = new DbTable (this.TableName);

				DbColumn columnId = new DbColumn (this.ColumnIdName, typeKeyId, DbColumnClass.KeyId, DbElementCat.Internal)
				{
					IsAutoIncremented = true
				};

				DbColumn columnConnectionId = new DbColumn (this.ColumnConnectionIdName, typeKeyId, DbColumnClass.Data, DbElementCat.Internal);

				DbColumn columnDateTime = new DbColumn (this.ColumnTimeName, typeDateTime, DbColumnClass.Data, DbElementCat.Internal)
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


			#endregion
			

			public string ColumnIdName
			{
				get
				{
					return "CR_ID";
				}
			}


			public string ColumnConnectionIdName
			{
				get
				{
					return "CR_CONNECTION_ID";
				}
			}


			public string ColumnTimeName
			{
				get
				{
					return "CR_TIME";
				}
			}


		}
                            
		
	}


}
