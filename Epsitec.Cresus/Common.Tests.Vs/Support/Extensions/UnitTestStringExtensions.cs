//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Marc BETTEX, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support.Extensions;
using Epsitec.Common.UnitTesting;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Common.Tests.Vs.Support.Extensions
{
	[TestClass]
	public class UnitTestStringExtensions
	{
		[TestMethod]
		public void IsAlphaNumericTest()
		{
			ExceptionAssert.Throw<System.ArgumentNullException> (() => StringExtensions.IsAlphaNumeric (null));
			
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
		public void ContainsAtPositionTest()
		{
			ExceptionAssert.Throw<System.ArgumentNullException> (() => StringExtensions.ContainsAtPosition (null, "", 0));
			ExceptionAssert.Throw<System.ArgumentNullException> (() => StringExtensions.ContainsAtPosition ("x", null, 0));
			
			string a = "abcdef";

			Assert.IsTrue (StringExtensions.ContainsAtPosition (a, "", 0));
			Assert.IsTrue (StringExtensions.ContainsAtPosition (a, "a", 0));
			Assert.IsTrue (StringExtensions.ContainsAtPosition (a, "abcdef", 0));
			Assert.IsTrue (StringExtensions.ContainsAtPosition (a, "b", 1));
			Assert.IsTrue (StringExtensions.ContainsAtPosition (a, "f", 5));
			Assert.IsTrue (StringExtensions.ContainsAtPosition (a, "", 6));
			Assert.IsFalse (StringExtensions.ContainsAtPosition (a, "x", 6));
			Assert.IsFalse (StringExtensions.ContainsAtPosition (a, "", 7));
			Assert.IsFalse (StringExtensions.ContainsAtPosition (a, "", -1));
			Assert.IsFalse (StringExtensions.ContainsAtPosition (a, "a", 1));
			Assert.IsFalse (StringExtensions.ContainsAtPosition (a, "abcdex", 0));
		}

		[TestMethod]
		public void SplitTest()
		{
			ExceptionAssert.Throw<System.ArgumentNullException> (() => StringExtensions.Split (null, ";"));
			ExceptionAssert.Throw<System.ArgumentException> (() => StringExtensions.Split ("a", null));
			ExceptionAssert.Throw<System.ArgumentException> (() => StringExtensions.Split ("a", ""));

			var data = new List<string> ()
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

		[TestMethod]
		public void ContainsAnyWordsTest()
		{
			Assert.IsTrue ("case postale 123".ContainsAnyWords ("cp", "case postale"));
			Assert.IsTrue ("ma case postale 123".ContainsAnyWords ("cp", "case postale"));
			Assert.IsTrue ("ma case postale".ContainsAnyWords ("cp", "case postale"));
			Assert.IsTrue ("ma case POSTALE 123".ContainsAnyWords ("cp", "case postale"));
			Assert.IsTrue ("ma case postale 123".ContainsAnyWords ("cp", "CASE postale"));
			Assert.IsTrue ("ma\tcase\npostale\r123".ContainsAnyWords ("cp", "case\u00a0postale"));
			
			Assert.IsFalse ("ma case postal".ContainsAnyWords ("cp", "case postale"));
			Assert.IsFalse ("ma case postale".ContainsAnyWords ("cp", "case postal"));
		}

		[TestMethod]
		public void TruncateTest()
		{
			string nullString = null;

			Assert.AreEqual ("abc", "abcdef".Truncate (3));
			Assert.AreEqual ("abc", "abc".Truncate (5));
			Assert.AreEqual ("", "abc".Truncate (0));
			
			Assert.IsNull (nullString.Truncate (3));
		}

		[TestMethod]
		public void TruncateAndAddEllipsisTest()
		{
			string nullString = null;

			Assert.AreEqual ("ab…", "abcdef".TruncateAndAddEllipsis (3));
			Assert.AreEqual ("abc", "abc".TruncateAndAddEllipsis (5));
			Assert.AreEqual (null, nullString.TruncateAndAddEllipsis (3));
			Assert.AreEqual ("ab...", "abcdef".TruncateAndAddEllipsis (5, "..."));

			ExceptionAssert.Throw<System.ArgumentException>	(() => "abcdef".TruncateAndAddEllipsis (2, "..."));
		}

		[TestMethod]
		public void JoinTest()
		{
			Assert.AreEqual ("abc", new List<string> () { "a", "b", "c" }.Join (""));
			Assert.AreEqual ("a b c", new List<string> () { "a", "b", "c"  }.Join (" "));
			Assert.AreEqual ("a-b-c", new List<string> () { "a", "b", "c" }.Join ("-"));
			Assert.AreEqual ("", new List<string> () { null }.Join (""));
			Assert.AreEqual ("", new List<string> () { "" }.Join ("-"));
			Assert.AreEqual ("a", new List<string> () { "a" }.Join (""));


			ExceptionAssert.Throw<System.ArgumentNullException> (() => StringExtensions.Join (null, " "));
			ExceptionAssert.Throw<System.ArgumentNullException> (() => new List<string> ().Join (null));
		}

		[TestMethod]
		public void CountOccurencesTest()
		{
			Assert.AreEqual (1, "a".CountOccurences ("a"));
			Assert.AreEqual (0, "b".CountOccurences ("a"));
			Assert.AreEqual (1, "abcd".CountOccurences ("c"));
			Assert.AreEqual (1, "abcd".CountOccurences ("ab"));
			Assert.AreEqual (1, "abcd".CountOccurences ("bc"));
			Assert.AreEqual (1, "abcd".CountOccurences ("cd"));
			Assert.AreEqual (0, "a".CountOccurences ("ac"));
			Assert.AreEqual (3, "aaa".CountOccurences ("a"));
			Assert.AreEqual (3, "abababa".CountOccurences ("ab"));

			ExceptionAssert.Throw<System.ArgumentException> (() => "".CountOccurences (null));
			ExceptionAssert.Throw<System.ArgumentException> (() => "".CountOccurences (""));
		}

		[TestMethod]
		public void IsAllUpperCaseTest()
		{
			Assert.IsTrue ("".IsAllUpperCase ());
			Assert.IsTrue ("A".IsAllUpperCase ());
			Assert.IsTrue ("A B C".IsAllUpperCase ());
			Assert.IsTrue ("A-B".IsAllUpperCase ());
			Assert.IsTrue ("A'B".IsAllUpperCase ());
			Assert.IsTrue ("A!B".IsAllUpperCase ());

			Assert.IsFalse ("a".IsAllUpperCase ());
			Assert.IsFalse ("A-b".IsAllUpperCase ());
			Assert.IsFalse ("b!A".IsAllUpperCase ());
		}
	}
}
