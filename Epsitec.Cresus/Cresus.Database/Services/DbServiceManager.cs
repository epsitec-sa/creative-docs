using System.Collections.Generic;


namespace Epsitec.Cresus.Database.Services
{


	public sealed class DbServiceManager : DbAbstractService
	{


		// TODO Comment this class.
		// Marc


		internal DbServiceManager(DbInfrastructure dbInfrastructure)
			: base (dbInfrastructure)
		{
			this.ConnectionManager = new DbConnectionManager (dbInfrastructure);
			this.InfoManager = new DbInfoManager (dbInfrastructure);
			this.LockManager = new DbLockManager (dbInfrastructure);
			this.Logger = new DbLogger (dbInfrastructure);
			this.UidManager = new DbUidManager (dbInfrastructure);
			this.EntityDeletionLogger = new DbEntityDeletionLogger (dbInfrastructure);
		}


		public DbConnectionManager ConnectionManager
		{
			get;
			private set;
		}


		public DbInfoManager InfoManager
		{
			get;
			private set;
		}


		public DbLockManager LockManager
		{
			get;
			private set;
		}


		public DbLogger Logger
		{
			get;
			private set;
		}


		public DbUidManager UidManager
		{
			get;
			private set;
		}


		public DbEntityDeletionLogger EntityDeletionLogger
		{
			get;
			private set;
		}


		internal void RegisterServiceTables()
		{
			foreach (DbTable dbTable in this.GetServiceTables ())
			{
				this.DbInfrastructure.AddTable (dbTable);
			}
		}


		internal override void TurnOn()
		{
			this.ConnectionManager.TurnOn ();
			this.InfoManager.TurnOn ();
			this.LockManager.TurnOn ();
			this.Logger.TurnOn ();
			this.UidManager.TurnOn ();
			this.EntityDeletionLogger.TurnOn ();

			base.TurnOn ();
		}
		

		private IEnumerable<DbTable> GetServiceTables()
		{
			return new List<DbTable> ()
			{
				this.ConnectionManager.CreateDbTable (),
				this.InfoManager.CreateDbTable (),
				this.LockManager.CreateDbTable (),
				this.Logger.CreateDbTable (),
				this.UidManager.CreateDbTable (),
				this.EntityDeletionLogger.CreateDbTable (),
			};
		}

	
	}


}
