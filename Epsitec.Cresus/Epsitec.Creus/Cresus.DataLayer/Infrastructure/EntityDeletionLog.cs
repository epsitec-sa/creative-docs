using Epsitec.Common.Support;
using Epsitec.Common.Support.Extensions;

using Epsitec.Cresus.Database;

using Epsitec.Cresus.DataLayer.Schema;

using System.Collections.Generic;

using System.Linq;


namespace Epsitec.Cresus.DataLayer.Infrastructure
{


	internal sealed class EntityDeletionLog
	{


		// TODO Comment this class
		// Marc


		public EntityDeletionLog(DbInfrastructure dbInfrastructure, ServiceSchemaEngine schemaEngine)
		{
			dbInfrastructure.ThrowIfNull ("dbInfrastructure");
			schemaEngine.ThrowIfNull ("schemaEngine");

			var table = schemaEngine.GetServiceTable (EntityDeletionLog.TableFactory.TableName);
			var tableQueryHelper = new TableQueryHelper (dbInfrastructure, table);

			this.table = table;
			this.tableQueryHelper = tableQueryHelper;
		}
		
		
		public EntityDeletionEntry CreateEntry(DbId entityModificationEntryId, Druid entityTypeId, DbId entityId)
		{
			entityModificationEntryId.ThrowIf (id => id.IsEmpty, "entityModificationEntryId cannot be empty.");
			entityTypeId.ThrowIf (id => id.IsEmpty, "entityTypeId cannot be empty.");
			entityId.ThrowIf (id => id.IsEmpty, "entityId cannot be empty.");

			IDictionary<string, object> columnNamesToValues = new Dictionary<string, object> ()
			{
			    { EntityDeletionLog.TableFactory.ColumnEntityModificationEntryIdName, entityModificationEntryId.Value },
				{ EntityDeletionLog.TableFactory.ColumnEntityTypeIdName, entityTypeId.ToLong () },
				{ EntityDeletionLog.TableFactory.ColumnEntityIdName, entityId },
			};

			IList<object> data = this.tableQueryHelper.AddRow (columnNamesToValues);

			return this.CreateEntityDeletionEntry (data);
		}


		public IEnumerable<EntityDeletionEntry> GetEntriesNewerThan(DbId minimumId)
		{
			minimumId.ThrowIf (id => id.IsEmpty, "minimumId cannot be empty.");
			
			SqlFunction condition = this.CreateConditionForMinimumEntryId (minimumId);

			IList<IList<object>> data = this.tableQueryHelper.GetRows (condition);

			return data.Select (d => this.CreateEntityDeletionEntry (d));
		}


		private SqlFunction CreateConditionForMinimumEntryId(DbId minimumLogId)
		{
			string columnName = this.table.Columns[EntityDeletionLog.TableFactory.ColumnEntityModificationEntryIdName].GetSqlName ();

			SqlFunction condition = new SqlFunction
			(
				SqlFunctionCode.CompareGreaterThanOrEqual,
				SqlField.CreateName (columnName),
				SqlField.CreateConstant (minimumLogId.Value, DbRawType.Int64)
			);

			return condition;
		}
		

		/// <summary>
		/// Creates a new instance of <see cref="EntityDeletionEntry"/> based on the given data.
		/// </summary>
		/// <param name="data">The data of the deletion log entry.</param>
		/// <returns>The <see cref="EntityDeletionEntry"/>.</returns>
		private EntityDeletionEntry CreateEntityDeletionEntry(IList<object> data)
		{
			DbId entryId = new DbId ((long) data[0]);
			DbId entityModificationEntryId = new DbId ((long) data[1]);
			Druid entityTypeId = Druid.FromLong ((long) data[2]);
			DbId entityId = new DbId ((long) data[3]);

			return new EntityDeletionEntry (entryId, entityModificationEntryId, entityTypeId, entityId);
		}


		private readonly DbTable table;


		private readonly TableQueryHelper tableQueryHelper;


		public static TableBuilder TableFactory
		{
			get
			{
				return EntityDeletionLog.tableFactory;
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
					return "CR_ED_LOG";
				}
			}


			public DbTable BuildTable()
			{
				DbTypeDef typeKeyId = new DbTypeDef (Epsitec.Cresus.Database.Res.Types.Num.KeyId);

				DbTable table = new DbTable (this.TableName);

				DbColumn columnId = new DbColumn (this.ColumnIdName, typeKeyId, DbColumnClass.KeyId, DbElementCat.Internal)
				{
					IsAutoIncremented = true
				};

				DbColumn columnEnityModificationEntryId = new DbColumn (this.ColumnEntityModificationEntryIdName, typeKeyId, DbColumnClass.RefInternal, DbElementCat.Internal);
				DbColumn columnEntityTypeId = new DbColumn (this.ColumnEntityTypeIdName, typeKeyId, DbColumnClass.Data, DbElementCat.Internal);
				DbColumn columnEntityId = new DbColumn (this.ColumnEntityIdName, typeKeyId, DbColumnClass.Data, DbElementCat.Internal);

				table.Columns.Add (columnId);
				table.Columns.Add (columnEnityModificationEntryId);
				table.Columns.Add (columnEntityTypeId);
				table.Columns.Add (columnEntityId);

				table.DefineCategory (DbElementCat.Internal);

				table.DefinePrimaryKey (columnId);
				table.UpdatePrimaryKeyInfo ();

				table.AddIndex ("IDX_EDL_REF_LOG", SqlSortOrder.Descending, columnEnityModificationEntryId);

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


			public string ColumnEntityModificationEntryIdName
			{
				get
				{
					return "CR_EM_ID";
				}
			}


			public string ColumnEntityTypeIdName
			{
				get
				{
					return "CR_ENTITY_TYPE_ID";
				}
			}


			public string ColumnEntityIdName
			{
				get
				{
					return "CR_ENTITY_ID";
				}
			}


		}


	}


}
