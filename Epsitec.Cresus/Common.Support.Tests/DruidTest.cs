//	Copyright © 2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using NUnit.Framework;

namespace Epsitec.Common.Support
{
	[TestFixture] public class DruidTest
	{
		[Test]
		public void CheckToFullString()
		{
			Assert.AreEqual ("0", Druid.ToFullString (0));
			Assert.AreEqual ("1023", Druid.ToFullString (0x0000100002000003));
			Assert.AreEqual ("10208", Druid.ToFullString (0x0000100002000100));
			Assert.AreEqual ("10H08", Druid.ToFullString (0x0000100011000100));
			Assert.AreEqual ("10VVV", Druid.ToFullString (0x000010001f0003ff));
			Assert.AreEqual ("10V0001", Druid.ToFullString (0x000010001f000400));
			Assert.AreEqual ("1020021", Druid.ToFullString (0x0000100042000400));
			Assert.AreEqual ("1023000001", Druid.ToFullString (0x0040100002000003));
			Assert.AreEqual ("1023000000008", Druid.ToFullString (0x4000100002000003));
		}
		
		[Test]
		public void CheckFromFullString()
		{
			Assert.AreEqual (0x0000000000000000L, Druid.FromFullString ("0"));
			Assert.AreEqual (0x0000100002000003L, Druid.FromFullString ("1023"));
			Assert.AreEqual (0x0000100002000100L, Druid.FromFullString ("10208"));
			Assert.AreEqual (0x0000100011000100L, Druid.FromFullString ("10H08"));
			Assert.AreEqual (0x000010001f0003ffL, Druid.FromFullString ("10VVV"));
			Assert.AreEqual (0x000010001f000400L, Druid.FromFullString ("10V0001"));
			Assert.AreEqual (0x0000100042000400L, Druid.FromFullString ("1020021"));
			Assert.AreEqual (0x0040100002000003L, Druid.FromFullString ("1023000001"));
			Assert.AreEqual (0x4000100002000003L, Druid.FromFullString ("1023000000008"));
		}
	}
}
