#define LOCALHOST
#define REMOTE_HOST_MARC
#define FBEMBEDDED


using Epsitec.Common.Support;

using Epsitec.Cresus.Database;

using Epsitec.Cresus.DataLayer.Infrastructure;
using Epsitec.Cresus.DataLayer.Schema;

using System;

using System.Collections.Generic;

using System.Threading;


namespace Epsitec.Cresus.Core.Library.Data.Tests.Vs.Helpers
{


	internal static class DatabaseHelper
	{
		
		
		public static bool CheckDatabaseExistence()
		{
			DbAccess access = DatabaseHelper.GetDbAccessForTestDatabase ();

			return DbInfrastructure.CheckDatabaseExistence (access);
		}


		public static void CreateTestDatabase()
		{
			DbAccess access = DatabaseHelper.GetDbAccessForTestDatabase ();

			using (DbInfrastructure infrastructure = new DbInfrastructure ())
			{
				infrastructure.CreateDatabase (access);
				EntityEngine.Create (infrastructure, new List<Druid> ());
			}
		}


		public static void DeleteTestDatabase()
		{
			DbAccess access = DatabaseHelper.GetDbAccessForTestDatabase ();

			try
			{
				DbInfrastructure.DropDatabase (access);
			}
			catch (Exception)
			{
				Thread.Sleep (500);

				DbInfrastructure.DropDatabase (access);
			}
		}


		public static void ResetTestDatabase()
		{
			if (DatabaseHelper.CheckDatabaseExistence ())
			{
				DatabaseHelper.DeleteTestDatabase ();
			}

			DatabaseHelper.CreateTestDatabase ();
		}


		public static DbInfrastructure CreateDbInfrastructure()
		{
			DbAccess dbAccess = DatabaseHelper.GetDbAccessForTestDatabase ();

			DbInfrastructure dbInfrastructure = new DbInfrastructure ();

			dbInfrastructure.AttachToDatabase (dbAccess);

			return dbInfrastructure;
		}


		public static DataInfrastructure CreateDataInfrastructure(DbInfrastructure dbInfrastructure)
		{
			EntityEngine engine = EntityEngine.Connect (dbInfrastructure, new Druid[0]);

			return new DataInfrastructure (dbInfrastructure, engine);
		}


		public static DbAccess GetDbAccessForTestDatabase()
		{

#if LOCALHOST
			string host = "localhost";
#elif REMOTE_HOST_MARC
			string host = "WIN-CDMPHQRQD03";
#endif

#if FBSERVER
			string provider = "Firebird";
#elif FBEMBEDDED
			string provider = "FirebirdEmbedded";
#endif

			return new DbAccess ("Firebird", "UTD_CORE", host, "sysdba", "masterkey", false);
		}


	}


}
