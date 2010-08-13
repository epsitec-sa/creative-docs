using Epsitec.Common.UnitTesting;

using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace Common.UnitTesting.UnitTests
{


	[TestClass]
	public class UnitTestExceptionAssert
	{
		

		[TestMethod]
		[ExpectedException (typeof (AssertFailedException))]
		public void ThrowNoException1()
		{
			ExceptionAssert.Throw<System.Exception> (() => { });
		}


		[TestMethod]
		public void ThrowExcpectedException()
		{



		}


		[TestMethod]
		public void ThrowUnexpectedException()
		{



		}


	}


}
