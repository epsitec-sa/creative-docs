using Epsitec.Common.Support.Extensions;

using Epsitec.Common.UnitTesting;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using System;

using System.Reflection;


namespace Epsitec.Common.Tests.Vs.Support.Extensions
{


	[TestClass]
	public class UnitTestAssemblyExtensions
	{


		[TestMethod]
		public void GetResourceTextTest()
		{
			var assembly = Assembly.GetExecutingAssembly ();
			var resourceName = "Epsitec.Common.Tests.Vs.Resources.TextResource.txt";
			var expectedvalue = "I am a super text resource and I am waiting to be read by this\r\nwonderfull method called AssemblyExtensions.GetResourceDataAsString() !";
			var actualValue = assembly.GetResourceText (resourceName);

			Assert.AreEqual (expectedvalue, actualValue);
		}


		[TestMethod]
		public void GetResourceTextArgumentCheck()
		{
			var assembly = Assembly.GetExecutingAssembly ();
			var resourceName = "Epsitec.Common.Tests.Vs.Resources.TextResource.txt";

			ExceptionAssert.Throw<ArgumentNullException>
			(
				() => ((Assembly) null).GetResourceText (resourceName)
			);

			ExceptionAssert.Throw<ArgumentException>
			(
				() => assembly.GetResourceText (null)
			);

			ExceptionAssert.Throw<ArgumentException>
			(
				() => assembly.GetResourceText ("")
			);
		}


	}


}
