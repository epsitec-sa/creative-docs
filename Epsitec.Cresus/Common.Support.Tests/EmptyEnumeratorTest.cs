using NUnit.Framework;

namespace Epsitec.Common.Support
{
	[TestFixture] public class EmptyEnumeratorTest
	{
		[Test] public void CheckEnumeration()
		{
			TestClass t = new TestClass ();
			
			int n = 0;
			
			foreach (object x in t)
			{
				Assertion.AssertNull (x);
				
				n++;
			}
			
			Assertion.AssertEquals (0, n);
		}
		
		public class TestClass : System.Collections.IEnumerable
		{
			#region IEnumerable Members
			public System.Collections.IEnumerator GetEnumerator()
			{
				return new EmptyEnumerator ();
			}
			#endregion
		}
	}
}
