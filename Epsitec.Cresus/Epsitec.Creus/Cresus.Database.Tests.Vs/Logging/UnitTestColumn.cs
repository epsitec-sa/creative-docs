using Epsitec.Common.UnitTesting;

using Epsitec.Cresus.Database.Logging;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.Collections.Generic;


namespace Epsitec.Cresus.Database.Tests.Vs.Logging
{


	[TestClass]
	public sealed class UnitTestColumn
	{


		[TestMethod]
		public void ConstructorArgumentCheck()
		{
			ExceptionAssert.Throw<System.ArgumentNullException>
			(
				() => new Column (null)
			);
		}


		[TestMethod]
		public void ConstructorAndNameCheck()
		{
			List<string> samples = new List<string> () { "", "1", "toto", "coucou", "blupi", };

			foreach (string sample in samples)
			{
				Column column = new Column (sample);

				Assert.AreEqual (sample, column.Name);
			}
		}


	}


}
