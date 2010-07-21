using Epsitec.Common.Support.Extensions;

using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace Common.Support.UnitTests
{


	[TestClass]
	public class UnitTestExceptionThrower
	{
				

		[TestMethod]
		public void ThrowIfNullTest1()
		{
			object element = new object ();

			element.ThrowIfNull ("element");
		}


		[TestMethod]
		[ExpectedException (typeof (System.ArgumentNullException))]
		public void ThrowIfNullTest2()
		{
			object element = null;

			element.ThrowIfNull ("element");
		}
		

		[TestMethod]
		public void ThrowIfNullOrEmptyTest1()
		{
			string string1 = "coucou";
			string string2 = " ";
			string string3 = "\t";
			string string4 = "\n";
			
			string1.ThrowIfNullOrEmpty ("string1");
			string2.ThrowIfNullOrEmpty ("string2");
			string3.ThrowIfNullOrEmpty ("string3");
			string4.ThrowIfNullOrEmpty ("string4");
		}


		[TestMethod]
		[ExpectedException (typeof (System.ArgumentException))]
		public void ThrowIfNullOrEmptyTest2()
		{
			string element = null;

			element.ThrowIfNullOrEmpty ("element");
		}


		[TestMethod]
		[ExpectedException (typeof (System.ArgumentException))]
		public void ThrowIfNullOrEmptyTest3()
		{
			string element = "";

			element.ThrowIfNullOrEmpty ("element");
		}


	}


}
