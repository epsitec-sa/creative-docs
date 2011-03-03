#define LOCALHOST
#define REMOTE_HOST_MARC


using Epsitec.Common.Support;


namespace Epsitec.Cresus.Database.Tests.Vs.Helpers
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


		public static DbAccess GetDbAccessForTestDatabase()
		{

#if LOCALHOST
			string host = "localhost";
#elif REMOTE_HOST_MARC
			string host = "WIN-CDMPHQRQD03";
#endif

			return new DbAccess ("Firebird", "UTD_DATABASE", host, "sysdba", "masterkey", false);
		}


		public static string GetEmployeeDatabaseFilePath()
		{

#if LOCALHOST
			string file = new System.IO.FileInfo (@"Resources\employee.gbak").FullName;
#elif REMOTE_HOST_MARC
			string file = @"C:\Users\bettex\Documents\Cresus.Core\employee.gbak";
#endif

			return file;
		}


		public static string GetLargeDatabaseFilePath()
		{

#if LOCALHOST
			string file = new System.IO.FileInfo (@"Resources\large.gbak").FullName;
#elif REMOTE_HOST_MARC
			string file = @"C:\Users\bettex\Documents\Cresus.Core\large.gbak";
#endif

			return file;
		}


		public static string GetTmpBackupFilePath()
		{

#if LOCALHOST
			string file = new System.IO.FileInfo (@"Resources\test.gbak").FullName;
#elif REMOTE_HOST_MARC
			string file = @"C:\Users\bettex\Documents\Cresus.Core\test.gbak";
#endif

			return file;
		}


	}


}
