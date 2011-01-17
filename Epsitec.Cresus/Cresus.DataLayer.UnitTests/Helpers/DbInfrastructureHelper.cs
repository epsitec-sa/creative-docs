using Epsitec.Cresus.Database;


namespace Epsitec.Cresus.DataLayer.UnitTests.Helpers
{


	internal static class DbInfrastructureHelper
	{


		public static bool CheckDatabaseExistence()
		{
			DbAccess access = DbInfrastructureHelper.GetDbAccessForTestDatabase ();

			return DbInfrastructure.CheckDatabaseExistence (access);
		}


		public static void CreateTestDatabase()
		{
			using (DbInfrastructure infrastructure = new DbInfrastructure ())
			{
				DbAccess access = DbInfrastructureHelper.GetDbAccessForTestDatabase ();

				infrastructure.CreateDatabase (access);
			}
		}


		public static void DeleteTestDatabase()
		{
			DbAccess access = DbInfrastructureHelper.GetDbAccessForTestDatabase ();

			DbInfrastructure.DropDatabase (access);
		}


		public static void ResetTestDatabase()
		{
			if (DbInfrastructureHelper.CheckDatabaseExistence ())
			{
				DbInfrastructureHelper.DeleteTestDatabase ();
			}

			DbInfrastructureHelper.CreateTestDatabase ();
		}


		public static DbInfrastructure ConnectToTestDatabase()
		{
			DbAccess dbAccess = DbInfrastructureHelper.GetDbAccessForTestDatabase ();

			DbInfrastructure dbInfrastructure = new DbInfrastructure ();

			dbInfrastructure.AttachToDatabase (dbAccess);

			return dbInfrastructure;
		}


		public static DbAccess GetDbAccessForTestDatabase()
		{
			return new DbAccess ("Firebird", "UTD_DATALAYER", "localhost", "sysdba", "masterkey", false);
		}


	}


}
