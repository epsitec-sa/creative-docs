﻿//	Copyright © 2011-2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
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
		public void CheckIsAlphaNumeric()
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
		public void CheckIsInteger()
		{
			ExceptionAssert.Throw<System.ArgumentNullException> (() => StringExtensions.IsInteger (null));

			Assert.IsFalse (StringExtensions.IsInteger (""));
			Assert.IsFalse (StringExtensions.IsInteger ("+"));
			Assert.IsFalse (StringExtensions.IsInteger ("-"));
			Assert.IsFalse (StringExtensions.IsInteger ("0+0"));
			Assert.IsFalse (StringExtensions.IsInteger ("0-0"));
			Assert.IsFalse (StringExtensions.IsInteger ("--1"));
			Assert.IsFalse (StringExtensions.IsInteger ("++1"));
			Assert.IsFalse (StringExtensions.IsInteger ("-+1"));
			Assert.IsFalse (StringExtensions.IsInteger ("1.1"));
			Assert.IsFalse (StringExtensions.IsInteger ("12345x"));
			Assert.IsFalse (StringExtensions.IsInteger ("12345 "));

			Assert.IsTrue (StringExtensions.IsInteger ("0"));
			Assert.IsTrue (StringExtensions.IsInteger ("1234567890"));
			Assert.IsTrue (StringExtensions.IsInteger ("-123456789"));
		}

		[TestMethod]
		public void CheckIsDecimal()
		{
			ExceptionAssert.Throw<System.ArgumentNullException> (() => StringExtensions.IsDecimal (null));

			Assert.IsFalse (StringExtensions.IsDecimal (""));
			Assert.IsFalse (StringExtensions.IsDecimal ("+"));
			Assert.IsFalse (StringExtensions.IsDecimal ("-"));
			Assert.IsFalse (StringExtensions.IsDecimal ("0+0"));
			Assert.IsFalse (StringExtensions.IsDecimal ("0-0"));
			Assert.IsFalse (StringExtensions.IsDecimal ("--1"));
			Assert.IsFalse (StringExtensions.IsDecimal ("++1"));
			Assert.IsFalse (StringExtensions.IsDecimal ("-+1"));
			Assert.IsFalse (StringExtensions.IsDecimal ("."));
			Assert.IsFalse (StringExtensions.IsDecimal ("0.1.2"));
			Assert.IsFalse (StringExtensions.IsDecimal ("1.x"));
			Assert.IsFalse (StringExtensions.IsDecimal ("1. "));

			Assert.IsTrue (StringExtensions.IsDecimal ("0"));
			Assert.IsTrue (StringExtensions.IsDecimal ("1234567890"));
			Assert.IsTrue (StringExtensions.IsDecimal ("-123456789"));
			Assert.IsTrue (StringExtensions.IsDecimal ("1."));
			Assert.IsTrue (StringExtensions.IsDecimal (".1"));
			Assert.IsTrue (StringExtensions.IsDecimal ("0.0"));
		}

		[TestMethod]
		public void CheckContainsAtPosition()
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
		public void CheckSplit()
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
		public void CheckContainsAnyWords()
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
		public void CheckTruncate()
		{
			string nullString = null;

			Assert.AreEqual ("abc", "abcdef".Truncate (3));
			Assert.AreEqual ("abc", "abc".Truncate (5));
			Assert.AreEqual ("", "abc".Truncate (0));
			
			Assert.IsNull (nullString.Truncate (3));
		}

		[TestMethod]
		public void CheckTruncateAndAddEllipsis()
		{
			string nullString = null;

			Assert.AreEqual ("ab…", "abcdef".TruncateAndAddEllipsis (3));
			Assert.AreEqual ("abc", "abc".TruncateAndAddEllipsis (5));
			Assert.AreEqual (null, nullString.TruncateAndAddEllipsis (3));
			Assert.AreEqual ("ab...", "abcdef".TruncateAndAddEllipsis (5, "..."));

			ExceptionAssert.Throw<System.ArgumentException>	(() => "abcdef".TruncateAndAddEllipsis (2, "..."));
		}

		[TestMethod]
		public void CheckJoin()
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
		public void CheckCountOccurences()
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
		public void CheckIsAllUpperCase()
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

		[TestMethod]
		public void CheckTrimSpacesAndDashes()
		{
			Assert.AreEqual ("abc", "abc".TrimSpacesAndDashes ());
			Assert.AreEqual ("abc", " abc ".TrimSpacesAndDashes ());
			Assert.AreEqual ("abc", "  abc  ".TrimSpacesAndDashes ());
			Assert.AreEqual ("abc", "-abc-".TrimSpacesAndDashes ());
			Assert.AreEqual ("abc", "--abc--".TrimSpacesAndDashes ());
			Assert.AreEqual ("abc", "- - abc - -".TrimSpacesAndDashes ());
			Assert.AreEqual ("abc", " - - abc - - ".TrimSpacesAndDashes ());
			Assert.AreEqual ("a-b", "a-b".TrimSpacesAndDashes ());
			Assert.AreEqual ("a-b", "a- b".TrimSpacesAndDashes ());
			Assert.AreEqual ("a-b", "a -b".TrimSpacesAndDashes ());
			Assert.AreEqual ("a-b", "a - b".TrimSpacesAndDashes ());
			Assert.AreEqual ("a-b", "a---b".TrimSpacesAndDashes ());
			Assert.AreEqual ("", "".TrimSpacesAndDashes ());
			Assert.AreEqual ("", " ".TrimSpacesAndDashes ());
			Assert.AreEqual ("", "-".TrimSpacesAndDashes ());
			Assert.AreEqual ("", " - - - ".TrimSpacesAndDashes ());
		}
	}
}
