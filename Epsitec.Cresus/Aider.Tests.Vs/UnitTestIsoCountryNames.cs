using Epsitec.Aider.Tools;

using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace Aider.Tests.Vs
{


	[TestClass]
	public class UnitTestIsoCountryNames
	{


		[TestMethod]
		public void Test()
		{
			Assert.AreEqual ("Suisse", IsoCountryNames.Instance["CH"]);
			Assert.AreEqual ("France", IsoCountryNames.Instance["FR"]);
			Assert.AreEqual ("Allemagne", IsoCountryNames.Instance["DE"]);
			Assert.AreEqual ("Italie", IsoCountryNames.Instance["IT"]);
		}


	}


}
