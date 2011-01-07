using Epsitec.Common.Support;


namespace Epsitec.Cresus.Database.UnitTests.Helpers
{


	/// <summary>
	/// The <c>TestHelper</c> class manages the global state needed to successfully run the tests.
	/// </summary>
	internal static class TestHelper
	{
		
		
		/// <summary>
		/// Initializes the global state of the assembly so that the tests can
		/// find the resources.
		/// </summary>
		public static void Initialize()
		{
			ResourceManagerPool.Default = new ResourceManagerPool ("default");
			ResourceManagerPool.Default.AddResourceProbingPath (@"S:\Epsitec.Cresus\Cresus.Database.UnitTests");
		}


		public static bool CheckDatabaseExistence()
		{
			// TODO This method is not very reliable, as it could tell that the database does not
			// exists when the database exists but the login information is not valid. This might
			// be improved, but it doesn't seem to be an eays way to ask Firebird if a database
			// does exist.
			// Marc

			DbAccess access = TestHelper.GetDbAccessForTestDatabase ();

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


		public static DbAccess GetDbAccessForTestDatabase()
		{
			return new DbAccess ("Firebird", "UTD_DATABASE", "localhost", "sysdba", "masterkey", false);
		}


	}


}
