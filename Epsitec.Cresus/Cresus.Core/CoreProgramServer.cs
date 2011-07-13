//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core
{
	internal static class CoreProgramServer
	{
		public static void ExecuteServer()
		{
			//	Pour Jonas : obtenir un "brick wall" à partir d'une entité, même vide :

			var customerSummaryWall = CoreSession.GetBrickWall (new Epsitec.Cresus.Core.Entities.CustomerEntity (), Controllers.ViewControllerMode.Summary);

			//	...et pour mesurer le temps pris, une fois que tout est "chaud" :
			
			var watch = new System.Diagnostics.Stopwatch ();

			for (int i = 0; i < 10; i++)
			{
				watch.Restart ();
				CoreSession.GetBrickWall (new Epsitec.Cresus.Core.Entities.CustomerEntity (), Controllers.ViewControllerMode.Edition);
				watch.Stop ();

				System.Diagnostics.Debug.WriteLine (string.Format ("Attempt {0}: fetching EditionController took {1} μs", i+1, 1000L*1000L * watch.ElapsedTicks / System.Diagnostics.Stopwatch.Frequency));
			}
		}
	}
}
