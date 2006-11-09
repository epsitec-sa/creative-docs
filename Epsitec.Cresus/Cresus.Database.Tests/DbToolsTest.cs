using NUnit.Framework;

namespace Epsitec.Cresus.Database
{
	[TestFixture]
	public class DbToolsTest
	{
		[Test] public void CheckMakeCompositeName()
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
