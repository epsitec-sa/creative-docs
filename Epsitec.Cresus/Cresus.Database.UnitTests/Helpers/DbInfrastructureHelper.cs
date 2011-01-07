namespace Epsitec.Cresus.Database.UnitTests.Helpers
{


	internal static class DbInfrastructureHelper
	{


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
			// TODO Have a better method to drop the database, that does not require to connect to
			// it before, and that does not fail if the database does not exists or is invalid.

			using (DbInfrastructure infrastructure = new DbInfrastructure ())
			{
				DbAccess access = TestHelper.GetDbAccessForTestDatabase ();

				infrastructure.AttachToDatabase (access);
				infrastructure.DropDatabase ();
			}
		}


		public static void ResetTestDatabase()
		{
			DbInfrastructureHelper.DeleteTestDatabase ();
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
