using NUnit.Framework;

namespace Epsitec.Cresus.DataLayer.Tests
{
	[TestFixture]
	public class DbToolsTest
	{
		[Test] public void CheckBuildCompositeName()
		{
			Assertion.AssertEquals ("", DbTools.BuildCompositeName ());
			Assertion.AssertEquals ("", DbTools.BuildCompositeName (""));
			Assertion.AssertEquals ("a", DbTools.BuildCompositeName ("a"));
			Assertion.AssertEquals ("a_b", DbTools.BuildCompositeName ("a", "b"));
			Assertion.AssertEquals ("a_b_c", DbTools.BuildCompositeName ("a", "b", "c"));
			Assertion.AssertEquals ("a__c", DbTools.BuildCompositeName ("a", "", "c"));
			Assertion.AssertEquals ("a_b", DbTools.BuildCompositeName ("a", "b", ""));
			Assertion.AssertEquals ("a", DbTools.BuildCompositeName ("a", "", ""));
		}
	}
}
