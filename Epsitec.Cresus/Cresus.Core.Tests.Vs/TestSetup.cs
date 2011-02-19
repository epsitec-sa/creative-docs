//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Cresus.Database;

namespace Epsitec.Cresus.Core
{
	/// <summary>
	/// The <c>TestSetup</c> class manages the global state needed to
	/// successfully run the tests.
	/// </summary>
	public static class TestSetup
	{
		/// <summary>
		/// Initializes the global state of the assembly so that the tests can
		/// find the resources.
		/// </summary>
		public static void Initialize()
		{
			//	See http://geekswithblogs.net/sdorman/archive/2009/01/31/migrating-from-nunit-to-mstest.aspx
			//	for migration tips, from nUnit to MSTest.

			ResourceManagerPool.Default = new ResourceManagerPool ("default");
			ResourceManagerPool.Default.AddResourceProbingPath (@"S:\Epsitec.Cresus\Cresus.Core");
		}

		/// <summary>
		/// Returns a clean database infrastructure instance.
		/// </summary>
		/// <returns>The infrastructure.</returns>
		public static DbInfrastructure CreateDbInfrastructure()
		{
			if (TestSetup.infrastructure != null)
			{
				TestSetup.infrastructure.Dispose ();
				TestSetup.infrastructure = null;
				
				System.Threading.Thread.Sleep (100);
			}

			const string databaseName = "CORETEST";
            
			TestSetup.DeleteDatabase (databaseName);

			using (DbInfrastructure infrastructure = new DbInfrastructure ())
			{
				infrastructure.CreateDatabase (DbInfrastructure.CreateDatabaseAccess (databaseName));
			}

			TestSetup.infrastructure = new DbInfrastructure ();
			TestSetup.infrastructure.AttachToDatabase (DbInfrastructure.CreateDatabaseAccess (databaseName));

			return TestSetup.infrastructure;
		}

		/// <summary>
		/// Deletes the database files on disk. This works for Firebird.
		/// </summary>
		/// <param name="name">The database name.</param>
		public static void DeleteDatabase(string name)
		{
			DbAccess access = DbInfrastructure.CreateDatabaseAccess (name);
			string path = Epsitec.Common.Types.Collection.GetFirst (DbFactory.GetDatabaseFilePaths (access));

			try
			{
				if (System.IO.File.Exists (path))
				{
					System.IO.File.Delete (path);
				}
			}
			catch (System.IO.IOException ex)
			{
				System.Console.Out.WriteLine ("Cannot delete database file. Error message :\n{0}\nWaiting for 5 seconds...", ex.ToString ());
				System.Threading.Thread.Sleep (5000);

				try
				{
					System.IO.File.Delete (path);
					System.Console.Out.WriteLine ("Finally succeeded");
				}
				catch
				{
					System.Console.Out.WriteLine ("Failed again, giving up");
				}
			}
		}

		
		static DbInfrastructure infrastructure;
	}
}
