using NUnit.Framework;

namespace Epsitec.Common.Converters
{
	[TestFixture] public class ComparerTest
	{
		[Test] public void CheckCompareBoxed()
		{
			object a = 1;
			object b = 1;
			
			Assertion.Assert (a != b);
			Assertion.Assert (a.Equals (b));
			
			a = "hello";
			b = "hello";
			
			Assertion.Assert (a == b);
			Assertion.Assert (a.Equals (b));
			
			System.Text.StringBuilder buffer = new System.Text.StringBuilder ();
			
			buffer.Append ("hell");
			buffer.Append ("o");
			
			b = buffer.ToString ();
			
			Assertion.Assert (a != b);
			Assertion.Assert ((string)a == (string)b);
			Assertion.Assert (a.Equals (b));
		}
		
		[Test] public void CheckEqual()
		{
			object a = 1;
			object b = 1;
			
			Assertion.Assert (Comparer.Equal (a, b));
			
			a = "hello";
			b = "hello";
			
			Assertion.Assert (Comparer.Equal (a, b));
			
			System.Text.StringBuilder buffer = new System.Text.StringBuilder ();
			
			buffer.Append ("hell");
			buffer.Append ("o");
			
			b = buffer.ToString ();
			
			Assertion.Assert (Comparer.Equal (a, b));
		}
	}
}
