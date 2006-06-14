using NUnit.Framework;
using System.Text.RegularExpressions;

namespace Epsitec.Common.Support
{
	[TestFixture] public class RegexConstraintTest
	{
		[Test] public void CheckConstraint()
		{
			RegexConstraint c1 = new RegexConstraint ("a*", RegexFactory.Options.IgnoreCase);
			RegexConstraint c2 = new RegexConstraint (RegexFactory.LocalizedDecimalNum);
			RegexConstraint c3 = new RegexConstraint (PredefinedRegex.AlphaNum);
			
			Assert.IsTrue (c1.IsValidValue ("abc"));
			Assert.IsTrue (c2.IsValidValue ((10.5M).ToString ()));
			Assert.IsTrue (c3.IsValidValue ("abc"));
			
			Assert.IsFalse (c1.IsValidValue ("xyz"));
			Assert.IsFalse (c2.IsValidValue ("abc"));
			Assert.IsFalse (c3.IsValidValue ("10"));
		}
		
		[Test] public void CheckToStringFromString()
		{
			RegexConstraint c1 = new RegexConstraint ("a*", RegexFactory.Options.IgnoreCase);
			RegexConstraint c2 = new RegexConstraint (RegexFactory.LocalizedDecimalNum);
			RegexConstraint c3 = new RegexConstraint (PredefinedRegex.AlphaNum);
			
			string s1 = c1.ToString ();
			string s2 = c2.ToString ();
			string s3 = c3.ToString ();
			
			c1 = RegexConstraint.FromString (s1);
			c2 = RegexConstraint.FromString (s2);
			c3 = RegexConstraint.FromString (s3);
			
			Assert.IsTrue (c1.IsValidValue ("abc"));
			Assert.IsTrue (c2.IsValidValue ((10.5M).ToString ()));
			Assert.IsTrue (c3.IsValidValue ("abc"));
			
			Assert.IsFalse (c1.IsValidValue ("xyz"));
			Assert.IsFalse (c2.IsValidValue ("abc"));
			Assert.IsFalse (c3.IsValidValue ("10"));
			
			Assert.AreEqual ("a*", c1.Pattern);
			Assert.AreEqual (RegexFactory.Options.IgnoreCase, c1.PatternOptions);
			Assert.AreEqual (RegexFactory.LocalizedDecimalNum.ToString (), c2.Regex.ToString ());
			Assert.AreEqual (PredefinedRegex.AlphaNum, c3.PredefinedRegex);
		}
	}
}
