//	Copyright © 2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Michael WALZ

using NUnit.Framework;

namespace Epsitec.Common.Text.Exchange
{
	[TestFixture] 
	public class RosettaTest
	{
		[Test] public void TestCtmlToHtmlConversion()
		{
			Rosetta rosetta = new Rosetta ();
			
			//	...
			
			string ctml = "<run>Hello, </run><run><rundef><b/></rundef>world</run>.";
			string html = "Hello, <b>world</b>.";
			
			Assert.AreEqual (rosetta.ConvertCtmlToHtml (ctml), html);
		}
	}
}
