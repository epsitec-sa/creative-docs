using Epsitec.Common.Support;

using Epsitec.Cresus.Database;

using Epsitec.Cresus.DataLayer.Infrastructure;
using Epsitec.Cresus.DataLayer.Schema;

using System;

using System.Collections.Generic;


namespace Epsitec.Cresus.DataLayer.Tests.Vs.Helpers
{


	internal sealed class DB : IDisposable
	{


		private DB(DbInfrastructure dbInfrastructure, DataInfrastructure dataInfrastructure)
		{
			this.dbInfrastructure = dbInfrastructure;
			this.dataInfrastructure = dataInfrastructure;
		}


		public DbInfrastructure DbInfrastructure
		{
			get
			{
				return this.dbInfrastructure;
			}
		}


		public DataInfrastructure DataInfrastructure
		{
			get
			{
				return this.dataInfrastructure;
			}
		}


		#region IDisposable Members


		public void Dispose()
		{
			this.dataInfrastructure.Dispose ();
			this.DbInfrastructure.Dispose ();
		}


		#endregion


		public static DB ConnectToTestDatabase()
		{
			var dbInfrastructure = DbInfrastructureHelper.ConnectToTestDatabase();

			var entityEngine = EntityEngineHelper.ConnectToTestDatabase (dbInfrastructure);

			var dataInfrastructure = new DataInfrastructure (dbInfrastructure, entityEngine);
			dataInfrastructure.OpenConnection ("id");

			return new DB (dbInfrastructure, dataInfrastructure);
		}


		public static DB ConnectToTestDatabase(EntityEngine entityEngine)
		{
			var dbInfrastructure = DbInfrastructureHelper.ConnectToTestDatabase ();
			
			var dataInfrastructure = new DataInfrastructure (dbInfrastructure, entityEngine);
			dataInfrastructure.OpenConnection ("id");

			return new DB (dbInfrastructure, dataInfrastructure);
		}


		private readonly DbInfrastructure dbInfrastructure;


		private readonly DataInfrastructure dataInfrastructure;


	}


}
