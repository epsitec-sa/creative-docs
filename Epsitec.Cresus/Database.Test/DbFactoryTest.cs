using NUnit.Framework;

namespace Epsitec.Cresus.Database
{
	[TestFixture]
	public class DbFactoryTest
	{
		[SetUp]
		public void LoadAssemblies()
		{
			try
			{
				System.Diagnostics.Debug.WriteLine ("");
			}
			catch
			{
			}
			
			DbFactory.Initialise ();
		}
		
		[Test]
		public void CheckFindDbAbstraction()
		{
			DbAccess db_access = new DbAccess ();
			
			db_access.provider = "Firebird";
			
			IDbAbstraction db_abstraction = DbFactory.FindDbAbstraction (db_access);
			
			Assertion.AssertNotNull ("Could not instanciate Firebird abstraction", db_abstraction);
		}
		
		[Test]
		public void CheckDebugDumpRegisteredDbAbstractions ()
		{
			DbFactory.DebugDumpRegisteredDbAbstractions ();
		}
	}
}
