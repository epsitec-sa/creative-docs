namespace Epsitec.Cresus.Database.UnitTests.Helpers
{


	internal static class IDbAbstractionHelper
	{


		public static void CreateTestDatabase()
		{
			DbAccess dbAccess = TestHelper.GetDbAccessForTestDatabase ();

			dbAccess.CreateDatabase = true;

			using (IDbAbstraction idbAbstraction = DbFactory.CreateDatabaseAbstraction (dbAccess))
			{
				idbAbstraction.Connection.Open ();
				idbAbstraction.Connection.Close ();
			}
		}


		public static void DeleteTestDatabase()
		{
			DbAccess dbAccess = TestHelper.GetDbAccessForTestDatabase ();

			using (IDbAbstraction idbAbstraction = DbFactory.CreateDatabaseAbstraction (dbAccess))
			{
				idbAbstraction.DropDatabase ();
			}
		}


		public static void ResetTestDatabase()
		{
			IDbAbstractionHelper.DeleteTestDatabase ();
			IDbAbstractionHelper.CreateTestDatabase ();
		}


		public static void CloneDatabase(string backupPath)
		{
			// TODO Improve this method, because now it assumes that the database is a local one.
			// Marc

			string name = TestHelper.GetDbAccessForTestDatabase ().Database;

			DbTools.RestoreDatabase (name, backupPath);

			System.Threading.Thread.Sleep (1000);
		}


		public static IDbAbstraction ConnectToTestDatabase()
		{
			DbAccess dbAccess = TestHelper.GetDbAccessForTestDatabase ();

			IDbAbstraction idbAbstraction = DbFactory.CreateDatabaseAbstraction (dbAccess);

			idbAbstraction.Connection.Open ();

			return idbAbstraction;
		}


	}


}
