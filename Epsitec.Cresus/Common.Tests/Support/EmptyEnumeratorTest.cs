//	Copyright © 2004-2009, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types.Collections;

using NUnit.Framework;

namespace Epsitec.Common.Tests.Support
{
	[TestFixture] public class EmptyEnumeratorTest
	{
		[Test]
		public void CheckEnumeration()
		{
			TestClass t = new TestClass ();

			int n = 0;

			foreach (object x in t)
			{
				n++;
			}

			Assert.AreEqual (0, n);
		}

		[Test]
		public void CheckEnumerationGeneric()
		{
			TestClassGeneric t = new TestClassGeneric ();

			int n = 0;

			foreach (string x in t)
			{
				n++;
			}

			Assert.AreEqual (0, n);
		}

		public class TestClass : System.Collections.IEnumerable
		{
			#region IEnumerable Members
			
			public System.Collections.IEnumerator GetEnumerator()
			{
				return EmptyEnumerator<object>.Instance;
			}
			
			#endregion
		}

		public class TestClassGeneric : System.Collections.Generic.IEnumerable<string>
		{
			#region IEnumerable<string> Members
			
			public System.Collections.Generic.IEnumerator<string> GetEnumerator()
			{
				return EmptyEnumerator<string>.Instance;
			}
			
			#endregion

			#region IEnumerable Members

			System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
			{
				return EmptyEnumerator<string>.Instance;
			}

			#endregion
		}
	}
}
