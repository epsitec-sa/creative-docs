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
		

		public static DbAccess GetDbAccessForTestDatabase()
		{
			return new DbAccess ("Firebird", "UTD_DATABASE", "localhost", "sysdba", "masterkey", false);
		}


	}


}
