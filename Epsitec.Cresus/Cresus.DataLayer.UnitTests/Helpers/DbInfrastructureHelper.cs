using Epsitec.Cresus.Database;


namespace Epsitec.Cresus.DataLayer.UnitTests.Helpers
{


	internal static class DbInfrastructureHelper
	{
		

		public static bool CheckDatabaseExistence()
		{
			// TODO This method is not very reliable, as it could tell that the database does not
			// exists when the database exists but the login information is not valid. This might
			// be improved, but it doesn't seem to be an eays way to ask Firebird if a database
			// does exist.
			// Marc

			DbAccess access = DbInfrastructureHelper.GetDbAccessForTestDatabase ();

			bool databaseExists = true;

			try
			{
				using (IDbAbstraction idbAbstraction = DbFactory.CreateDatabaseAbstraction (access))
				{
					idbAbstraction.Connection.Open ();
					idbAbstraction.Connection.Close ();
				}
			}
			catch
			{
				databaseExists = false;
			}

			return databaseExists;
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
