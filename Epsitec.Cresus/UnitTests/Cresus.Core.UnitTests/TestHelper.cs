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
	public static class TestHelper
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
			if (TestHelper.infrastructure != null)
			{
				TestHelper.infrastructure.Dispose ();
				TestHelper.infrastructure = null;
				
				System.Threading.Thread.Sleep (100);
			}

			const string databaseName = "CORETEST";
            
			TestHelper.DeleteDatabase (databaseName);

			using (DbInfrastructure infrastructure = new DbInfrastructure ())
			{
				infrastructure.CreateDatabase (DbInfrastructure.CreateDatabaseAccess (databaseName));
			}

			TestHelper.infrastructure = new DbInfrastructure ();
			TestHelper.infrastructure.AttachToDatabase (DbInfrastructure.CreateDatabaseAccess (databaseName));

			return TestHelper.infrastructure;
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



		public static void PrintStartTest(string name)
		{
			System.Diagnostics.Debug.WriteLine ("===========================================================================================================================================================");
			System.Diagnostics.Debug.WriteLine ("Starting test: " + name);
			System.Diagnostics.Debug.WriteLine ("===========================================================================================================================================================");
		}


		public static void MeasureAndDisplayTime(string message, System.Action action)
		{
			TestHelper.MeasureAndDisplayTime (message, action, 1);
		}


		public static void MeasureAndDisplayTime(string message, System.Action action, int count)
		{
			System.Diagnostics.Stopwatch watch = new System.Diagnostics.Stopwatch ();

			watch.Start ();

			for (int i = 0; i < count; i++)
			{
				action ();
			}

			watch.Stop ();

			System.Diagnostics.Debug.WriteLine (message + "\t\t\t\tnumber of runs: " + count + "\t average time (ms): " + watch.ElapsedMilliseconds / count);
		}


		public static string extendString(string text, int length)
		{
			string result = text;

			while (result.Length < length)
			{
				result += " ";
			}

			return result;
		}


		static DbInfrastructure infrastructure;


	}


}
