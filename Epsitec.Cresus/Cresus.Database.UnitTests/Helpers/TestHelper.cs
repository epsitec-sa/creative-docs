using Epsitec.Common.Support;
using Epsitec.Cresus.Database;


namespace Epsitec.Cresus.Database.UnitTests.Helpers
{


	/// <summary>
	/// The <c>TestSetup</c> class manages the global state needed to
	/// successfully run the tests.
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


		public static DbInfrastructure DbInfrastructure
		{
			get
			{
				return TestHelper.dbInfrastructure;
			}
		}

		
		public static DbInfrastructure CreateAndConnectToDatabase()
		{
			TestHelper.DeleteDatabase ();
			TestHelper.CreateDatabase ();

			return TestHelper.ConnectToDatabase ();
		}


		public static void CreateDatabase()
		{
			TestHelper.DisposeInfrastructure ();

			using (DbInfrastructure infrastructure = new DbInfrastructure ())
			{
				DbAccess access = TestHelper.CreateDbAccess ();
				infrastructure.CreateDatabase (access);
			}
		}


		public static DbInfrastructure ConnectToDatabase()
		{
			TestHelper.DisposeInfrastructure ();
			
			DbAccess dbAccess = TestHelper.CreateDbAccess ();

			TestHelper.dbInfrastructure = new DbInfrastructure ();
			TestHelper.dbInfrastructure.AttachToDatabase (dbAccess);

			return TestHelper.dbInfrastructure;
		}


		public static void DisposeInfrastructure()
		{
			if (TestHelper.dbInfrastructure != null)
			{
				TestHelper.dbInfrastructure.Dispose ();
				TestHelper.dbInfrastructure = null;

				System.Threading.Thread.Sleep (100);
			}
		}


		public static DbAccess CreateDbAccess()
		{
			string dbHost = "localhost";
			string dbName = "CORETEST";
			
			return new DbAccess ("Firebird", dbName, dbHost, "sysdba", "masterkey", false);
		}

		
		public static void DeleteDatabase()
		{
			TestHelper.DisposeInfrastructure ();

			using (DbInfrastructure infrastructure = new DbInfrastructure ())
			{
				DbAccess access = TestHelper.CreateDbAccess ();

				infrastructure.AttachToDatabase (access);
				infrastructure.DropDatabase ();
			}
		}


		public static IDbAbstraction CreateDbAbstraction(bool forceDbCreation)
		{
			IDbAbstraction dbAbstraction;
		
			DbAccess dbAccess = new DbAccess ("Firebird", "test", "localhost", "sysdba", "masterkey", forceDbCreation);

			try
			{
				dbAbstraction = DbFactory.CreateDatabaseAbstraction (dbAccess);
			}
			catch (Exceptions.ExistsException)
			{
				dbAccess.CreateDatabase = false;
				dbAbstraction = DbFactory.CreateDatabaseAbstraction (dbAccess);
			}

			return dbAbstraction;
		}


		public static DbInfrastructure GetInfrastructureFromBase(string name)
		{
			DbInfrastructure infrastructure = new DbInfrastructure ();
			DbAccess dbAccess = DbInfrastructure.CreateDatabaseAccess (name);

			infrastructure.AttachToDatabase (dbAccess);

			return infrastructure;
		}


		private static DbInfrastructure dbInfrastructure;


	}


}
