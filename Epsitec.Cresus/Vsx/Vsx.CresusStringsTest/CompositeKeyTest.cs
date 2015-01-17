using System;
using System.Diagnostics;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Epsitec.Cresus.Strings
{
	[TestClass]
	public class CompositeKeyTest
	{
		[TestMethod]
		public void Flattening()
		{
			var sk1 = "A.B";

			var sk21 = "ab";

			var sk221 = "1.2".Split('.');
			var sk222 = "3.4".Split ('.');
			var sk223 = Key.Create("cd");

			var sk22 = Key.Create (sk221, sk222, sk223);
			var sk2 = Key.Create (sk21, sk22);

			var k1 = Key.Create (sk1, sk2);

			var a1 = k1.ToArray ();
		}

		[TestMethod]
		public void Equality()
		{
			var k1 = Key.Create ("1", "2", "3");
			var k2 = Key.Create ("1.2.3".Split('.'));
			Assert.AreEqual (k1, k2);
		}
	}
}
