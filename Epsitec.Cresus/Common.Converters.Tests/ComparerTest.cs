using NUnit.Framework;

namespace Epsitec.Common.Converters
{
	[TestFixture] public class ComparerTest
	{
		[Test] public void CheckCompareBoxed()
		{
			object a = 1;
			object b = 1;
			
			Assert.IsTrue (a != b);
			Assert.IsTrue (a.Equals (b));
			
			a = "hello";
			b = "hello";
			
			Assert.IsTrue (a == b);
			Assert.IsTrue (a.Equals (b));
			
			System.Text.StringBuilder buffer = new System.Text.StringBuilder ();
			
			buffer.Append ("hell");
			buffer.Append ("o");
			
			b = buffer.ToString ();
			
			Assert.IsTrue (a != b);
			Assert.IsTrue ((string)a == (string)b);
			Assert.IsTrue (a.Equals (b));
		}
		
		[Test] public void CheckEqual()
		{
			object a = 1;
			object b = 1;
			
			Assert.IsTrue (Comparer.Equal (a, b));
			
			a = "hello";
			b = "hello";
			
			Assert.IsTrue (Comparer.Equal (a, b));
			
			System.Text.StringBuilder buffer = new System.Text.StringBuilder ();
			
			buffer.Append ("hell");
			buffer.Append ("o");
			
			b = buffer.ToString ();
			
			Assert.IsTrue (Comparer.Equal (a, b));
		}
	}
}
