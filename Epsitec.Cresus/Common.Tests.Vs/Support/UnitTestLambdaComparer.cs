using Epsitec.Common.Support;

using Epsitec.Common.UnitTesting;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.Collections.Generic;


namespace Epsitec.Common.Tests.Vs.Support
{


	[TestClass]
	public sealed class UnitTestLambdaComparer
	{


		[TestMethod]
		public void ArgumentCheckConstructur()
		{
			ExceptionAssert.Throw<System.ArgumentNullException>
			(
				() => new LambdaComparer<object> (null)
			);

			ExceptionAssert.Throw<System.ArgumentNullException>
			(
				() => new LambdaComparer<object> (null, o => o.GetHashCode ())
			);

			ExceptionAssert.Throw<System.ArgumentNullException>
			(
				() => new LambdaComparer<object> (object.Equals, null)
			);
		}


		[TestMethod]
		public void TestConstructor1()
		{
			System.Func<int, int, bool> compareFunction = (e1, e2) => e1 == e2;
			System.Func<int, int> hashFunction = EqualityComparer<int>.Default.GetHashCode;

			LambdaComparer<int> comparer = new LambdaComparer<int> (compareFunction);

			Assert.AreEqual (compareFunction, comparer.CompareFunction);
			Assert.AreEqual (hashFunction, comparer.HashFunction);
		}


		[TestMethod]
		public void TestConstructor2()
		{
			System.Func<int, int, bool> compareFunction = (e1, e2) => e1 == e2;
			System.Func<int, int> hashFunction = e => e.GetHashCode ();

			LambdaComparer<int> comparer = new LambdaComparer<int> (compareFunction, hashFunction);

			Assert.AreEqual (compareFunction, comparer.CompareFunction);
			Assert.AreEqual (hashFunction, comparer.HashFunction);
		}


		[TestMethod]
		public void TestEquals()
		{
			System.Func<int, int, bool> compareFunction = (e1, e2) => e1 == 3;
			System.Func<int, int> hashFunction = e => e.GetHashCode ();

			LambdaComparer<int> comparer = new LambdaComparer<int> (compareFunction, hashFunction);

			for (int i = 0; i < 10; i++)
			{
				for (int j = 0; j < 10; j++)
				{
					Assert.AreEqual (compareFunction (i, j), comparer.Equals (i, j));
				}
			}
		}


		[TestMethod]
		public void TestHashCode()
		{
			System.Func<int, int, bool> compareFunction = (e1, e2) => e1 == e2;
			System.Func<int, int> hashFunction = e => e % 3;

			LambdaComparer<int> comparer = new LambdaComparer<int> (compareFunction, hashFunction);

			for (int i = 0; i < 10; i++)
			{
				Assert.AreEqual (hashFunction (i), comparer.GetHashCode (i));
			}
		}


	}


}
