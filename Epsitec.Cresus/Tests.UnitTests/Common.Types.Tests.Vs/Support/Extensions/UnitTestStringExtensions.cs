using Epsitec.Common.Support.Extensions;

using Epsitec.Common.UnitTesting;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.Collections.Generic;



namespace Epsitec.Common.Types.UnitTests.Support.Extensions
{


	[TestClass]
	public class UnitTestStringExtensions
	{


		[TestMethod]
		public void IsAlphaNumericArgumentCheck()
		{
			ExceptionAssert.Throw<System.ArgumentNullException>
			(
				() => StringExtensions.IsAlphaNumeric (null)
			);
		}


		[TestMethod]
		public void IsAlphaNumericTest()
		{
			Assert.IsTrue (StringExtensions.IsAlphaNumeric (""));
			Assert.IsTrue (StringExtensions.IsAlphaNumeric ("1"));
			Assert.IsTrue (StringExtensions.IsAlphaNumeric ("a"));
			Assert.IsTrue (StringExtensions.IsAlphaNumeric ("A"));
			Assert.IsTrue (StringExtensions.IsAlphaNumeric ("1aB"));
			Assert.IsFalse (StringExtensions.IsAlphaNumeric ("_"));
			Assert.IsFalse (StringExtensions.IsAlphaNumeric (" "));
			Assert.IsFalse (StringExtensions.IsAlphaNumeric ("."));
			Assert.IsFalse (StringExtensions.IsAlphaNumeric ("é"));
			Assert.IsFalse (StringExtensions.IsAlphaNumeric ("1_a B"));
		}


		[TestMethod]
		public void SplitArgumentCheck()
		{
			ExceptionAssert.Throw<System.ArgumentNullException>
			(
				() => StringExtensions.Split (null, ";")
			);

			ExceptionAssert.Throw<System.ArgumentException>
			(
				() => StringExtensions.Split ("a", null)
			);

			ExceptionAssert.Throw<System.ArgumentException>
			(
			  () => StringExtensions.Split ("a", "")
			);
		}


		[TestMethod]
		public void SplitTest()
		{
			List<string> data = new List<string> ()
			{
				"",
				"a",
				";",
				"a;a",
				"a;b;c;",
				";;;",
				";;a;;a;;",
			};

			foreach (string s in data)
			{
				Assert.AreEqual (s, string.Join (";", StringExtensions.Split (s, ";")));
			}
		}


	}
}
