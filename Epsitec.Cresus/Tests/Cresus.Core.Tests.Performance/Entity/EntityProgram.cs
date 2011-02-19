//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Cresus.Database;
using Epsitec.Cresus.DataLayer;
using Epsitec.Cresus.PerformanceTests;

using System.Collections.Generic;
using System.Linq;


namespace Epsitec.Cresus.PerformanceTests.Entity
{


	public sealed class EntityProgram
	{


		public static void Main(string[] args)
		{
			TestHelper.Initialize ();

			bool buildDatabase = true;
			bool retreiveAllData = false;
			bool retreiveRequestedData = false;
			bool measureTime = false;
			bool userInput = false;

			using (var test = new TestPerformance (buildDatabase))
			{
				if (retreiveAllData)
				{
					test.RetrieveAllData ();
				}

				if (retreiveRequestedData)
				{
					test.RetrieveRequestedData ();
				}

				if (measureTime)
				{
					test.RetrieveNaturalPerson ();
					test.RetrieveLocation ();

					System.Console.ForegroundColor = System.ConsoleColor.Yellow;

					if (userInput)
					{
						System.Console.WriteLine ("Ready to run the performance test. Hit a key to start.");
						System.Console.ReadKey ();
					}

					System.Threading.Thread.Sleep (2*1000);
					System.Console.ResetColor ();

					EntityProgram.MeasureAndDisplayExecutionTime ("RetrieveNaturalPerson", 100, () => test.RetrieveNaturalPerson ());
					EntityProgram.MeasureAndDisplayExecutionTime ("RetrieveLocation", 100, () => test.RetrieveLocation ());
				}
			}

			if (userInput)
			{
				System.Console.WriteLine ("Done, press any key to continue...");
				System.Console.ReadKey ();
			}
		}


		private static void MeasureAndDisplayExecutionTime(string text, int count, System.Action action)
		{
			var time = EntityProgram.MeasureExecutionTimeInMilliseconds (count, action);
			System.Console.ForegroundColor = System.ConsoleColor.Green;
			System.Console.WriteLine ("{0}: {1}ms", text, time);
			System.Console.ResetColor ();
		}


		private static decimal MeasureExecutionTimeInMilliseconds(int count, System.Action action)
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
