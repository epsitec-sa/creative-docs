using Epsitec.Common.UnitTesting;

using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace Common.UnitTesting.UnitTests
{


	[TestClass]
	public class UnitTestExceptionAssert
	{


		[TestMethod]
		[ExpectedException (typeof (System.ArgumentNullException))]
		public void ArgumentCheck1()
		{
			ExceptionAssert.Throw (null);
		}


		[TestMethod]
		[ExpectedException (typeof (System.ArgumentNullException))]
		public void ArgumentCheck2()
		{
			ExceptionAssert.Throw<System.ArgumentNullException> (null);
		}


		[TestMethod]
		[ExpectedException (typeof (AssertFailedException))]
		public void ThrowNoException1()
		{
			ExceptionAssert.Throw (() => { });
		}
		

		[TestMethod]
		[ExpectedException (typeof (AssertFailedException))]
		public void ThrowNoException2()
		{
			ExceptionAssert.Throw<System.ArgumentNullException> (() => { });
		}


		[TestMethod]
		public void ThrowExcpectedException1()
		{
			ExceptionAssert.Throw<System.ArgumentNullException> (() =>
			{
				throw new System.ArgumentNullException ();
			});
		}


		[TestMethod]
		public void ThrowExcpectedException2()
		{
			ExceptionAssert.Throw (() =>
			{
				throw new System.Exception ();
			});
		}


		[TestMethod]
		[ExpectedException (typeof (AssertFailedException))]
		public void ThrowUnexpectedException()
		{
			ExceptionAssert.Throw<System.ArgumentNullException> (() =>
			{
				throw new System.Exception ();
			});
		}


	}


}
