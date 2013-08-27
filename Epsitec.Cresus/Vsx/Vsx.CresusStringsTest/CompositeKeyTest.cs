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
			var sk1 = Key.Create("A.B");

			var sk21 = Key.Create ("ab");

			var sk221 = Key.Split ("1.2");
			var sk222 = Key.Split ("3.4");
			var sk223 = Key.Create ("cd");
			var sk22 = new CompositeKey (sk221, sk222, sk223);
			var sk2 = new CompositeKey (sk21, sk22);

			var k1 = new CompositeKey (sk1, sk2);

			var a1 = k1.ToArray ();
		}
	}
}
