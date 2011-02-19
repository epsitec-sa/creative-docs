using Epsitec.Common.UnitTesting;

using Epsitec.Cresus.Database.Logging;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.Collections.Generic;

using System;


namespace Epsitec.Cresus.Database.UnitTests.Logging
{


	[TestClass]
	public sealed class UnitTestParameter
	{


		[TestMethod]
		public void ConstructorArgumentCheck()
		{
			ExceptionAssert.Throw<ArgumentNullException>
			(
				() => new Parameter (null, new object ())
			);
		}


		[TestMethod]
		public void ConstructorAndGettersTest()
		{
			var samples = new List<Tuple<string, object>> ()
			{
				new Tuple<string, object> ("p1", null),
				new Tuple<string, object> ("p2", "Blupi"),
				new Tuple<string, object> ("p3", 3),
				new Tuple<string, object> ("p4", DateTime.Now),
				new Tuple<string, object> ("p5", new object ()),
			};

			foreach (var sample in samples)
			{
				Parameter parameter = new Parameter (sample.Item1, sample.Item2);

				Assert.AreEqual (parameter.Name, sample.Item1);
				Assert.AreEqual (parameter.Value, sample.Item2);
			}
		}


	}


}
