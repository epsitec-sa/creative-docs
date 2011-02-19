//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Cresus.Database;
using Epsitec.Cresus.Database.Implementation;
using Epsitec.Common.IO;

namespace Epsitec.Cresus.PerformanceTests
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
			//	See http://geekswithblogs.net/sdorman/archive/2009/01/31/migrating-from-nunit-to-mstest.aspx
			//	for migration tips, from nUnit to MSTest.

			ResourceManagerPool.Default = new ResourceManagerPool ("default");
			ResourceManagerPool.Default.AddResourceProbingPath (@"S:\Epsitec.Cresus\X.Core.PerformanceTests");
		}

		
		/// <summary>
		/// Returns a clean database infrastructure instance.
		/// </summary>
		/// <returns>The infrastructure.</returns>
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

			TestHelper.infrastructure = new DbInfrastructure ();
			TestHelper.infrastructure.AttachToDatabase (dbAccess);

			return TestHelper.infrastructure;
		}

		public static void DisposeInfrastructure()
		{
			if (TestHelper.infrastructure != null)
			{
				TestHelper.infrastructure.Dispose ();
				TestHelper.infrastructure = null;

				System.Threading.Thread.Sleep (100);
			}
		}


		public static DbAccess CreateDbAccess()
		{
			// local machine
			string dbHost = "localhost";

			// Marc's virtual machine
			//string dbHost = "192.168.1.50";

			// Mathieu's machine
			//string dbHost = "DevBox2-PC";

			string dbName = "CORETEST";
			
			return new DbAccess ("Firebird", dbName, dbHost, "sysdba", "masterkey", false);
		}

		
		/// <summary>
		/// Deletes the database files on disk. This works for Firebird.
		/// </summary>
		/// <param name="name">The database name.</param>
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



		public static void PrintStartTest(string name)
		{
			string message = TestHelper.GetStartTestString (name);

			System.Diagnostics.Debug.WriteLine (message);
		}


		public static void WriteStartTest(string name, string file)
		{
			string message = TestHelper.GetStartTestString (name);

			Logger.Log (message, file);

			TestHelper.PrintStartTest (name);
		}


		private static string GetStartTestString(string name)
		{
			return "===========================================================================================================================================================\n"
				 + "[" + System.DateTime.Now + "]\t Starting test: " + name + "\n"
				 + "===========================================================================================================================================================";
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


		public static void MeasureAndWriteTime(string message, string file, System.Action action, int count)
		{
			System.Diagnostics.Stopwatch watch = new System.Diagnostics.Stopwatch ();

			watch.Start ();

			for (int i = 0; i < count; i++)
			{
				action ();
			}

			watch.Stop ();

			string text = message + "\t\t\t\tnumber of runs: " + count + "\t average time (ms): " + watch.ElapsedMilliseconds / count;

			Logger.Log (text, file);
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
