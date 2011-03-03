using Epsitec.Common.UnitTesting;

using Epsitec.Cresus.Database.Logging;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.Collections.Generic;


namespace Epsitec.Cresus.Database.Tests.Vs.Logging
{


	[TestClass]
	public sealed class UnitTestRow
	{


		[TestMethod]
		public void ConstructorArgumentCheck()
		{
			ExceptionAssert.Throw<System.ArgumentNullException>
			(
				() => new Row ((IEnumerable<object>) null)
			);
		}


		[TestMethod]
		public void ConstructorAndValuesTest()
		{
			List<List<object>> samples = new List<List<object>> ()
			{
				new List<object> (),
				new List<object> () { 0, 1, 2 },
				new List<object> () { null, 1, "2", System.TimeSpan.FromDays (3), new object (), },
			};
						
			foreach (List<object> sample in samples)
			{
				Row row = new Row (sample);

				CollectionAssert.AreEqual (sample, row.Values);
			}
		}


	}


}
