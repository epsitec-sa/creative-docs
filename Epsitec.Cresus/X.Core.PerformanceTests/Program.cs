//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Cresus.Core;
using Epsitec.Cresus.Core.Data;
using Epsitec.Cresus.Core.Entities;

using Epsitec.Cresus.Database;
using Epsitec.Cresus.DataLayer;

using System.Linq;

namespace Epsitec.Cresus.Core
{
	class Program
	{
		static void Main(string[] args)
		{
			TestSetup.Initialize ();
			
#if false
			using (var test = new TestPerformance (true))
			{
			}
#endif

			using (var test = new TestPerformance (false))
			{
				var schemaEngine = new SchemaEngine (test.DbInfrastructure);
				SchemaEngine.SetSchemaEngine (test.DbInfrastructure, schemaEngine);

				test.RetrieveNaturalPerson ();

				System.Console.WriteLine ("Ready to run the performance test. Hit a key to start.");
				System.Console.ReadKey ();
				
				System.Diagnostics.Debug.WriteLine ("Test harness loaded");
				System.Diagnostics.Debug.WriteLine ("--------------------------------------------------------------------------------");

				var watch = new System.Diagnostics.Stopwatch ();
				const int loopCount = 100;
				watch.Start ();

				for (int i = 0; i < loopCount; i++)
				{
					test.RetrieveNaturalPerson ();
				}

				watch.Stop ();
				System.Console.WriteLine ("Executed {0} loops, mean time is {1} ms", loopCount, watch.ElapsedMilliseconds/loopCount);
			}
		}
	}
}
