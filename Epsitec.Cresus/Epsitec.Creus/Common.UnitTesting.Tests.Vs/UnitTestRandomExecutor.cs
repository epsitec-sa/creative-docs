using Epsitec.Common.UnitTesting;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using System;

using System.Collections.Generic;

using System.Linq;


namespace Epsitec.Common.UnitTesting.UnitTests.Vs
{

	
	[TestClass]
	public sealed class UnitTestRandomExecutor
	{
		
		
		[TestMethod]
		[ExpectedException (typeof (ArgumentException))]
		public void ArgumentCheck1()
		{
			Action[] actions = new Action[] { () => { } };

			RandomExecutor.ExecuteRandomly (0, actions);
		}
		
		
		[TestMethod]
		[ExpectedException (typeof (ArgumentNullException))]
		public void ArgumentCheck2()
		{
			Action[] actions = null;
			
			RandomExecutor.ExecuteRandomly (1, actions);
		}


		[TestMethod]
		[ExpectedException (typeof (ArgumentException))]
		public void ArgumentCheck3()
		{
			Action[] actions = new Action[0];

			RandomExecutor.ExecuteRandomly (1, actions);
		}


		[TestMethod]
		[ExpectedException (typeof (ArgumentException))]
		public void ArgumentCheck4()
		{
			Action[] actions = new Action[] { null };

			RandomExecutor.ExecuteRandomly (1, actions);
		}


		[TestMethod]
		public void SimpleTest()
		{
			for (int nbActions = 1; nbActions < 10; nbActions++)
			{
				var results = Enumerable.Range (0, nbActions).Select (_ => 0).ToArray ();
				var actions = Enumerable.Range (0, nbActions).Select<int, Action> (n => () => results[n] = results[n] + 1).ToArray ();

				var nbExecutions = 1000000;
				var expectedResult = nbExecutions / nbActions;
				var tolerance = expectedResult / 100;
				var lowerBound = expectedResult - tolerance;
				var upperBound = expectedResult + tolerance;
				
				RandomExecutor.ExecuteRandomly (nbExecutions, actions);

				for (int j = 0; j < nbActions; j++)
				{
					Assert.IsTrue (results[j] > lowerBound);
					Assert.IsTrue (results[j] < upperBound);
				}

				Assert.AreEqual (nbExecutions, results.Sum ());
			}
		}


	}


}
