using Epsitec.Common.Support.Extensions;

using Epsitec.Common.UnitTesting;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.Collections.Generic;

using System.Linq;


namespace Epsitec.Common.Tests.Vs.Support.Extensions
{


	[TestClass]
	public sealed class UnitTestDictionaryExtensions
	{


		[TestMethod]
		public void AsReadOnlyDictionaryArgumentCheck()
		{
			ExceptionAssert.Throw<System.ArgumentNullException>
			(
				() => ((Dictionary<int, int>) null).AsReadOnlyDictionary ()
			);
		}


		[TestMethod]
		public void AsReadOnlyDictionaryTest()
		{
			System.Random dice = new System.Random ();

			for (int i = 0; i < 25; i++)
			{
				var dictionary = Enumerable.Range (0, 25).ToDictionary (e => e, e => dice.Next ());

				var readOnlyDictionary = dictionary.AsReadOnlyDictionary ();

				Assert.IsTrue (readOnlyDictionary.IsReadOnly);
				Assert.IsTrue (dictionary.Keys.SetEquals (readOnlyDictionary.Keys));

				foreach (var key in dictionary.Keys)
				{
					Assert.AreEqual (dictionary[key], readOnlyDictionary[key]);
				}
			}
		}


		[TestMethod]
		public void AsEntriesArgumentCheck()
		{
			ExceptionAssert.Throw<System.ArgumentNullException>
			(
				() => ((Dictionary<int, int>) null).AsEntries ()
			);
		}


		[TestMethod]
		public void AsEntriesTest()
		{
			var dice = new System.Random ();

			for (int i = 0; i < 25; i++)
			{
				var dictionary = Enumerable
					.Range (0, 25)
					.ToDictionary (e => e, e => dice.Next ());

				var expectedKeys = dictionary.Keys;
				var actualKeys = dictionary.AsEntries ().Select (e => e.Key).Cast<int> ();
				
				Assert.IsTrue (expectedKeys.SetEquals (actualKeys));

				foreach (var entry in dictionary.AsEntries ())
				{
					Assert.AreEqual (dictionary[(int) entry.Key], entry.Value);
				}
			}
		}


	}


}
