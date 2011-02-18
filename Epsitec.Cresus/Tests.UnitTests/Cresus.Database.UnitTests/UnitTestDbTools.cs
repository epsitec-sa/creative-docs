using Epsitec.Cresus.Database.UnitTests.Helpers;

using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace Epsitec.Cresus.Database.UnitTests
{


	[TestClass]
	public sealed class UnitTestDbTools
	{


		[ClassInitialize]
		public static void ClassInitialize(TestContext testContext)
		{
			TestHelper.Initialize ();
		}
		
		
		[TestMethod]
		public void CheckMakeCompositeName()
		{
			Assert.AreEqual ("", DbTools.MakeCompositeName ());
			Assert.AreEqual ("", DbTools.MakeCompositeName (""));
			Assert.AreEqual ("a", DbTools.MakeCompositeName ("a"));
			Assert.AreEqual ("a_b", DbTools.MakeCompositeName ("a", "b"));
			Assert.AreEqual ("a_b_c", DbTools.MakeCompositeName ("a", "b", "c"));
			Assert.AreEqual ("a__c", DbTools.MakeCompositeName ("a", "", "c"));
			Assert.AreEqual ("a_b", DbTools.MakeCompositeName ("a", "b", ""));
			Assert.AreEqual ("a", DbTools.MakeCompositeName ("a", "", ""));
		}


	}


}
