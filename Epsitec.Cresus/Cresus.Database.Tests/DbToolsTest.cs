using NUnit.Framework;

namespace Epsitec.Cresus.Database
{
	[TestFixture]
	public class DbToolsTest
	{
		[Test] public void CheckBuildCompositeName()
		{
			Assert.AreEqual ("", DbTools.BuildCompositeName ());
			Assert.AreEqual ("", DbTools.BuildCompositeName (""));
			Assert.AreEqual ("a", DbTools.BuildCompositeName ("a"));
			Assert.AreEqual ("a_b", DbTools.BuildCompositeName ("a", "b"));
			Assert.AreEqual ("a_b_c", DbTools.BuildCompositeName ("a", "b", "c"));
			Assert.AreEqual ("a__c", DbTools.BuildCompositeName ("a", "", "c"));
			Assert.AreEqual ("a_b", DbTools.BuildCompositeName ("a", "b", ""));
			Assert.AreEqual ("a", DbTools.BuildCompositeName ("a", "", ""));
		}
	}
}
