using NUnit.Framework;

namespace Epsitec.Common.Types
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
			
			Assert.IsFalse (a == b);
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
			
			a = new string[] { "a", "b" };
			b = new string[] { "a", "b" };
			
			Assert.IsFalse (System.Object.Equals (a, b));
			Assert.IsTrue (Comparer.Equal (a, b));
			
			a = new string[2,2] { { "1", "2" }, { "3", "4" } };
			b = new string[2,2] { { "1", "2" }, { "3", "4" } };
			
			Assert.IsFalse (System.Object.Equals (a, b));
			Assert.IsTrue (Comparer.Equal (a, b));
			
			a = new string[2,2] { { "1", "2" }, { "3", "4" } };
			b = new string[2,3] { { "1", "2", "X" }, { "3", "4", "Y" } };
			
			Assert.IsFalse (Comparer.Equal (a, b));
		}
	}
}
