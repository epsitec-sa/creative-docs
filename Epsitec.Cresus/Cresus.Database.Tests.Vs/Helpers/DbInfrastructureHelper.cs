namespace Epsitec.Cresus.Database.Tests.Vs.Helpers
{


	internal static class DbInfrastructureHelper
	{


		public static bool CheckDatabaseExistence()
		{
			DbAccess dbAccess = TestHelper.GetDbAccessForTestDatabase ();

			return DbInfrastructure.CheckDatabaseExistence (dbAccess);
		}


		public static void CreateTestDatabase()
		{
			using (DbInfrastructure infrastructure = new DbInfrastructure ())
			{
				DbAccess access = TestHelper.GetDbAccessForTestDatabase ();

				infrastructure.CreateDatabase (access);
			}
		}


		public static void DeleteTestDatabase()
		{
			DbAccess access = TestHelper.GetDbAccessForTestDatabase ();

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
			DbAccess dbAccess = TestHelper.GetDbAccessForTestDatabase ();

			DbInfrastructure dbInfrastructure = new DbInfrastructure ();

			dbInfrastructure.AttachToDatabase (dbAccess);

			return dbInfrastructure;
		}


	}


}
