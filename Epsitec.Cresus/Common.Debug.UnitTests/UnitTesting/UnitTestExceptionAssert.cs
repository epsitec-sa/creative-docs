using Epsitec.Common.Debug.UnitTesting;

using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace Common.Debug.UnitTests
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
