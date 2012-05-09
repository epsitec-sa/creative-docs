//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.IO;
using Epsitec.Common.Support;


namespace Epsitec.Cresus.DataLayer.Tests.Vs.Helpers
{


	/// <summary>
	/// The <c>TestSetup</c> class manages the global state needed to successfully run the tests.
	/// </summary>
	internal static class TestHelper
	{
		
		
		/// <summary>
		/// Initializes the global state of the assembly so that the tests can find the resources.
		/// </summary>
		public static void Initialize()
		{
			ResourceManagerPool.Default = new ResourceManagerPool ("default");
			ResourceManagerPool.Default.AddResourceProbingPath (@"S:\Epsitec.Cresus\Cresus.DataLayer.Tests.Vs");
		}

		
		public static void WriteStartTest(string name, string file)
		{
			string message = TestHelper.GetStartTestString (name);

			Logger.Log (message, file);
		}


		private static string GetStartTestString(string name)
		{
			return "===========================================================================================================================================================\n"
				 + "[" + System.DateTime.Now + "]\t Starting test: " + name + "\n"
				 + "===========================================================================================================================================================";
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

			string text = message + "\tnumber of runs: " + count + "\t average time (ms): " + watch.ElapsedMilliseconds / count;

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


	}


}
