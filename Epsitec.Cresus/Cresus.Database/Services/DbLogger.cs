using Epsitec.Cresus.Database.Collections;

using System.Collections.Generic;

using System.Data;

using System.Linq;


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

			IDictionary<string, object> columnNamesToValues = new Dictionary<string, object> ()
			{
				{ Tags.ColumnConnectionId, connectionId.Value },
			};

			IList<object> data = this.AddRow (columnNamesToValues);

			return this.CreateLogEntry (data);
		}


		public DbLogEntry GetLogEntry(DbId entryId)
		{
			this.CheckIsAttached ();

			using (DbTransaction transaction = this.DbInfrastructure.InheritOrBeginTransaction (DbTransactionMode.ReadOnly))
			{
				SqlFieldList conditions = new SqlFieldList ()
				{
					this.CreateConditionForEntryId (entryId),
				};

				var data = this.GetRowValues (conditions);

				transaction.Commit ();

				return data.Any () ? this.CreateLogEntry (data.First ()) : null;
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


		private DbLogEntry CreateLogEntry(IList<object> data)
		{
			DbId entryId = new DbId ((long) data[0]);
			DbId connectionId = new DbId ((long) data[1]);
			System.DateTime dateTime = (System.DateTime) data[2];
			long sequenceNumber = (long) data[3];

			return new DbLogEntry (entryId, connectionId, dateTime, sequenceNumber);
		}


		// TODO Move these two functions in DbAbstractAttachable (and merge them with other)?
		// Marc
		
		private IList<object> AddRow(IDictionary<string, object> columnNamesToValues)
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


		private IList<IList<object>> GetRowValues(SqlFieldList conditions)
		{
			using (DbTransaction transaction = this.DbInfrastructure.InheritOrBeginTransaction (DbTransactionMode.ReadOnly))
			{
				SqlSelect query = new SqlSelect ();

				query.Tables.Add (SqlField.CreateName (this.DbTable.GetSqlName ()));

				foreach (DbColumn dbColumn in this.DbTable.Columns)
				{
					query.Fields.Add (SqlField.CreateName (dbColumn.GetSqlName ()));
				}
				query.Conditions.AddRange (conditions);

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
                
		
	}


}
