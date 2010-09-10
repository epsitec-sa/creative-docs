using Epsitec.Common.Support.Extensions;

using Epsitec.Cresus.Database.Collections;


namespace Epsitec.Cresus.Database
{


    public sealed class DbConnexionManager : DbAbstractAttachable
	{


		// TODO Comment this class.
		// Marc

		
		public DbConnexionManager(System.TimeSpan timeOutValue) : base ()
		{
			this.TimeOutValue = timeOutValue;
		}


		public System.TimeSpan TimeOutValue
		{
			get;
			private set;
		}


		public long OpenConnexion(string connexionIdentity)
		{
			this.CheckIsAttached ();

			connexionIdentity.ThrowIfNullOrEmpty ("connexionIdentity");

			System.DateTime databaseTime = this.DbInfrastructure.GetDatabaseTime ();
			int status = (int) DbConnexionStatus.Opened;

			SqlFieldList fieldsToInsert = new SqlFieldList ();

			DbColumn columnConnexionIdentity = this.DbTable.Columns[Tags.ColumnConnexionIdentity];
			DbColumn columnConnexionSince = this.DbTable.Columns[Tags.ColumnConnexionSince];
			DbColumn columnCounexionLastSeen = this.DbTable.Columns[Tags.ColumnConnexionLastSeen];
			DbColumn columnCounexionStatus = this.DbTable.Columns[Tags.ColumnConnexionStatus];

			fieldsToInsert.Add (this.DbInfrastructure.CreateSqlFieldFromAdoValue (columnConnexionIdentity, connexionIdentity));
			fieldsToInsert.Add (this.DbInfrastructure.CreateSqlFieldFromAdoValue (columnConnexionSince, databaseTime));
			fieldsToInsert.Add (this.DbInfrastructure.CreateSqlFieldFromAdoValue (columnCounexionLastSeen, databaseTime));
			fieldsToInsert.Add (this.DbInfrastructure.CreateSqlFieldFromAdoValue (columnCounexionStatus, status));

			return this.AddRow (fieldsToInsert);
		}


		public void CloseConnexion(long connexionId)
		{
			this.CheckIsAttached ();

			connexionId.ThrowIf (cId => cId < 0, "connexionId cannot be lower than zero.");

			DbColumn dbColumn = this.DbTable.Columns[Tags.ColumnConnexionStatus];
			int status = (int) DbConnexionStatus.Closed;

			SqlFieldList fields = new SqlFieldList ()
			{
				this.DbInfrastructure.CreateSqlFieldFromAdoValue (dbColumn, status)
			};

			SqlFieldList conditions = new SqlFieldList ()
			{
				this.CreateConditionForConnexionId (connexionId),
				this.CreateConditionForConnexionStatus (DbConnexionStatus.Opened),
			};

			int nbRowsAffected = this.SetRowValue (fields, conditions);

			if (nbRowsAffected == 0)
			{
				throw new System.Exception ("Could not close connexion. It does not exist or it is not open anymore.");
			}
		}


		public bool ConnexionExists(long connexionId)
		{
			this.CheckIsAttached ();

			connexionId.ThrowIf (cId => cId < 0, "connexionId cannot be lower than zero.");

			SqlFieldList conditions = new SqlFieldList ()
			{
				this.CreateConditionForConnexionId (connexionId),
			};

			return this.RowExists (conditions);
		}


		public void KeepConnexionAlive(long connexionId)
		{
			this.CheckIsAttached ();

			connexionId.ThrowIf (cId => cId < 0, "connexionId cannot be lower than zero.");

			DbColumn dbColumn = this.DbTable.Columns[Tags.ColumnConnexionLastSeen];
			System.DateTime databaseTime = this.DbInfrastructure.GetDatabaseTime ();
			
			SqlFieldList fields = new SqlFieldList ()
			{
				this.DbInfrastructure.CreateSqlFieldFromAdoValue (dbColumn, databaseTime)
			};

			SqlFieldList conditions = new SqlFieldList ()
			{
				this.CreateConditionForConnexionId (connexionId),
				this.CreateConditionForConnexionStatus (DbConnexionStatus.Opened),
			};

			int nbRowsAffected = this.SetRowValue (fields, conditions);

			if (nbRowsAffected == 0)
			{
				throw new System.Exception ("Could not keep connexion alive. It does not exist or it is not open anymore.");
			}
		}


		public bool InterruptDeadConnexions()
		{
			this.CheckIsAttached ();

			DbColumn dbColumn = this.DbTable.Columns[Tags.ColumnConnexionStatus];
			int status = (int) DbConnexionStatus.Interrupted;
			
			SqlFieldList fields = new SqlFieldList ()
			{
				this.DbInfrastructure.CreateSqlFieldFromAdoValue (dbColumn, status)
			};

			SqlFieldList conditions = new SqlFieldList ()
			{
				this.CreateConditionForConnexionStatus (DbConnexionStatus.Opened),
				this.CreateConditionForTimeOut (),
			};

			int nbRowsAffected = this.SetRowValue (fields, conditions);

			return nbRowsAffected > 0;
		}


		public string GetConnexionIdentity(long connexionId)
		{
			this.CheckIsAttached ();

			connexionId.ThrowIf (cId => cId < 0, "connexionId cannot be lower than zero.");

			using (DbTransaction transaction = DbInfrastructure.InheritOrBeginTransaction (DbTransactionMode.ReadOnly))
			{
				if (!this.ConnexionExists (connexionId))
				{
					throw new System.InvalidOperationException ("The connexion does not exist.");
				}

				DbColumn dbColumn = this.DbTable.Columns[Tags.ColumnConnexionIdentity];

				SqlFieldList conditions = new SqlFieldList ()
				{
					this.CreateConditionForConnexionId (connexionId),
				};

				object connexionIdentity = this.GetRowValue (dbColumn, conditions);

				transaction.Commit ();

				return (string) connexionIdentity;
			}
		}


		public DbConnexionStatus GetConnexionStatus(long connexionId)
		{
			this.CheckIsAttached ();

			connexionId.ThrowIf (cId => cId < 0, "connexionId cannot be lower than zero.");

			using (DbTransaction transaction = DbInfrastructure.InheritOrBeginTransaction (DbTransactionMode.ReadOnly))
			{
				if (!this.ConnexionExists (connexionId))
				{
					throw new System.InvalidOperationException ("The connexion does not exist.");
				}

				DbColumn dbColumn = this.DbTable.Columns[Tags.ColumnConnexionStatus];

				SqlFieldList conditions = new SqlFieldList ()
				{
					this.CreateConditionForConnexionId (connexionId),
				};

				object connexionStatus = this.GetRowValue (dbColumn, conditions);

				transaction.Commit ();

				return (DbConnexionStatus) connexionStatus;
			}
		}


		public System.DateTime GetConnexionSince(long connexionId)
		{
			this.CheckIsAttached ();

			connexionId.ThrowIf (cId => cId < 0, "connexionId cannot be lower than zero.");

			using (DbTransaction transaction = DbInfrastructure.InheritOrBeginTransaction (DbTransactionMode.ReadOnly))
			{
				if (!this.ConnexionExists (connexionId))
				{
					throw new System.InvalidOperationException ("The connexion does not exist.");
				}

				DbColumn dbColumn = this.DbTable.Columns[Tags.ColumnConnexionSince];

				SqlFieldList conditions = new SqlFieldList ()
				{
					this.CreateConditionForConnexionId (connexionId),
				};

				object connexionSince = this.GetRowValue (dbColumn, conditions);

				transaction.Commit ();

				return (System.DateTime) connexionSince;
			}
		}


		public System.DateTime GetConnexionLastSeen(long connexionId)
		{
			this.CheckIsAttached ();

			connexionId.ThrowIf (cId => cId < 0, "connexionId cannot be lower than zero.");

			using (DbTransaction transaction = DbInfrastructure.InheritOrBeginTransaction (DbTransactionMode.ReadOnly))
			{
				if (!this.ConnexionExists (connexionId))
				{
					throw new System.InvalidOperationException ("The connexion does not exist.");
				}

				DbColumn dbColumn = this.DbTable.Columns[Tags.ColumnConnexionLastSeen];

				SqlFieldList conditions = new SqlFieldList ()
				{
					this.CreateConditionForConnexionId (connexionId),
				};

				object connexionLastSeen = this.GetRowValue (dbColumn, conditions);

				transaction.Commit ();

				return (System.DateTime) connexionLastSeen;
			}
		}


		private SqlFunction CreateConditionForConnexionId(long connexionId)
		{
			return new SqlFunction
			(
				SqlFunctionCode.CompareEqual,
				SqlField.CreateName (this.DbTable.Columns[Tags.ColumnId].GetSqlName ()),
				SqlField.CreateConstant (connexionId, DbRawType.Int64)
			);
		}


		private SqlFunction CreateConditionForConnexionStatus(DbConnexionStatus connexionStatus)
		{
			return new SqlFunction
			(
				SqlFunctionCode.CompareEqual,
				SqlField.CreateName (this.DbTable.Columns[Tags.ColumnConnexionStatus].GetSqlName ()),
				SqlField.CreateConstant ((int) connexionStatus, DbRawType.Int32)
			);
		}


		private SqlFunction CreateConditionForTimeOut()
		{
			System.DateTime databaseTime = this.DbInfrastructure.GetDatabaseTime ();
			System.DateTime timeOutTime = databaseTime - this.TimeOutValue;
			
			return new SqlFunction
			(
				SqlFunctionCode.CompareLessThan,
				SqlField.CreateName (this.DbTable.Columns[Tags.ColumnConnexionLastSeen].GetSqlName ()),
				SqlField.CreateConstant (timeOutTime, DbRawType.DateTime)
			);
		}
		

	}


}
