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

			Program.MeasureAndDisplayExecutionTime ("System.Type.GetType(...)", 1000,
				delegate
				{
					System.Type.GetType ("System.String");
					System.Type.GetType ("System.String");
					System.Type.GetType ("System.String");
					System.Type.GetType ("System.String");
					System.Type.GetType ("System.String");
					System.Type.GetType ("System.Boolean");
					System.Type.GetType ("System.Boolean");
					System.Type.GetType ("System.Boolean");
					System.Type.GetType ("System.Boolean");
					System.Type.GetType ("System.Boolean");
					System.Type.GetType ("System.Boolean");
					System.Type.GetType ("System.Boolean");
					System.Type.GetType ("System.Boolean");
					System.Type.GetType ("System.Boolean");
					System.Type.GetType ("System.Boolean");
					System.Type.GetType ("System.Int32");
					System.Type.GetType ("System.Int32");
					System.Type.GetType ("System.Int32");
					System.Type.GetType ("System.Int32");
					System.Type.GetType ("System.Int32");
				});

			using (var test = new TestPerformance (false))
			{
				var schemaEngine = new SchemaEngine (test.DbInfrastructure);
				SchemaEngine.SetSchemaEngine (test.DbInfrastructure, schemaEngine);

				test.RetrieveNaturalPerson ();

				System.Console.ForegroundColor = System.ConsoleColor.Green;
				System.Console.WriteLine ("Ready to run the performance test. Hit a key to start.");
				System.Console.ReadKey ();
				System.Console.ResetColor ();

				Program.MeasureAndDisplayExecutionTime ("RetrieveNaturalPerson", 100, () => test.RetrieveNaturalPerson ());
			}
		}

		static void MeasureAndDisplayExecutionTime(string text, int count, System.Action action)
		{
			var time = Program.MeasureMilliseconds (count, action);
			System.Console.ForegroundColor = System.ConsoleColor.Green;
			System.Console.WriteLine ("{0}: {1}ms", text, time);
			System.Console.ResetColor ();
		}

		static decimal MeasureMilliseconds(int count, System.Action action)
		{
			//	Warm-up first...
			action ();

			//	Performance measurement:
			var watch = new System.Diagnostics.Stopwatch ();
			watch.Start ();
			
			for (int i = 0; i < count; i++)
			{
				action ();
			}
			
			watch.Stop ();
			return ((decimal)(watch.ElapsedMilliseconds * 1000 / count)) / 1000M;
		}
	}
}
