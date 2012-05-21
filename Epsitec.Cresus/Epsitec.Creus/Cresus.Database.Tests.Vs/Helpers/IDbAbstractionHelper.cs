namespace Epsitec.Cresus.Database.Tests.Vs.Helpers
{


	internal static class IDbAbstractionHelper
	{


		public static bool CheckDatabaseExistence()
		{
			DbAccess dbAccess = TestHelper.GetDbAccessForTestDatabase ();

			bool databaseExists;

			try
			{
				using (IDbAbstraction idbAbstraction = DbFactory.CreateDatabaseAbstraction (dbAccess))
				{
					idbAbstraction.Connection.Open ();
					idbAbstraction.Connection.Close ();
				}

				databaseExists = true;
			}
			catch
			{
				databaseExists = false;
			}

			return databaseExists;
		}


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

			dbAccess.CheckConnection = false;
			dbAccess.IgnoreInitialConnectionErrors = true;

			using (IDbAbstraction idbAbstraction = DbFactory.CreateDatabaseAbstraction (dbAccess))
			{
				idbAbstraction.DropDatabase ();
			}
		}


		public static void ResetTestDatabase()
		{
			if (IDbAbstractionHelper.CheckDatabaseExistence ())
			{
				IDbAbstractionHelper.DeleteTestDatabase ();
			}

			IDbAbstractionHelper.CreateTestDatabase ();
		}


		public static void RestoreDatabase(string file)
		{
			DbAccess dbAccess = TestHelper.GetDbAccessForTestDatabase ();

			if (IDbAbstractionHelper.CheckDatabaseExistence ())
			{
				IDbAbstractionHelper.DeleteTestDatabase ();
			}

			dbAccess.CheckConnection = false;
			dbAccess.IgnoreInitialConnectionErrors = false;

			using (IDbAbstraction idbAbstraction = DbFactory.CreateDatabaseAbstraction (dbAccess))
			{
				idbAbstraction.ServiceTools.Restore (file);

				System.Threading.Thread.Sleep (1000);
			}
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
