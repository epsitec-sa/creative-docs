using Epsitec.Common.Support;

using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace Epsitec.Common.Tests.Vs.Support
{


	[TestClass]
	public class UnitTestStringUtils
	{


		[TestMethod]
		public void RemoveDiacritics()
		{
			Assert.AreEqual ("a", StringUtils.RemoveDiacritics ("ä"));
			Assert.AreEqual ("e", StringUtils.RemoveDiacritics ("é"));
			Assert.AreEqual ("i", StringUtils.RemoveDiacritics ("ì"));
			Assert.AreEqual ("o", StringUtils.RemoveDiacritics ("ô"));
			Assert.AreEqual ("U", StringUtils.RemoveDiacritics ("Ü"));
			Assert.AreEqual ("n", StringUtils.RemoveDiacritics ("ñ"));
			Assert.AreEqual ("abcd", StringUtils.RemoveDiacritics ("abcd"));
			Assert.AreEqual ("aebeced", StringUtils.RemoveDiacritics ("aébécéd"));
			Assert.AreEqual (null, StringUtils.RemoveDiacritics (null));
			Assert.AreEqual ("", StringUtils.RemoveDiacritics (""));
		}


	}


}
