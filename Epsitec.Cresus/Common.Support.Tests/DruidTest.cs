//	Copyright © 2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using NUnit.Framework;

namespace Epsitec.Common.Support
{
	[TestFixture] public class DruidTest
	{
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

			Assert.AreEqual (1, Druid.GetModuleId (0x0000100002000003L));
			Assert.AreEqual (2, Druid.GetDevId (0x0000100002000003L));
			Assert.AreEqual (3, Druid.GetLocalId (0x0000100002000003L));
		}
		
		[Test]
		public void CheckFromModuleString()
		{
			Assert.AreEqual (0x0000000000000000L, Druid.FromModuleString ("0", 0));
			Assert.AreEqual (0x0000100002000003L, Druid.FromModuleString ("23", 1));
			Assert.AreEqual (0x0000100002000100L, Druid.FromModuleString ("208", 1));
			Assert.AreEqual (0x0000100011000100L, Druid.FromModuleString ("H08", 1));
			Assert.AreEqual (0x000010001f0003ffL, Druid.FromModuleString ("VVV", 1));
			Assert.AreEqual (0x000010001f000400L, Druid.FromModuleString ("V0001", 1));
			Assert.AreEqual (0x0000100042000400L, Druid.FromModuleString ("20021", 1));
			Assert.AreEqual (0x0040100002000003L, Druid.FromModuleString ("23", 0x401));
			Assert.AreEqual (0x4000100002000003L, Druid.FromModuleString ("23", 0x40001));
		}

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
		public void CheckToModuleString()
		{
			Assert.AreEqual ("0", Druid.ToModuleString (0));
			Assert.AreEqual ("23", Druid.ToModuleString (0x0000100002000003));
			Assert.AreEqual ("208", Druid.ToModuleString (0x0000100002000100));
			Assert.AreEqual ("H08", Druid.ToModuleString (0x0000100011000100));
			Assert.AreEqual ("VVV", Druid.ToModuleString (0x000010001f0003ff));
			Assert.AreEqual ("V0001", Druid.ToModuleString (0x000010001f000400));
			Assert.AreEqual ("20021", Druid.ToModuleString (0x0000100042000400));
			Assert.AreEqual ("23", Druid.ToModuleString (0x0040100002000003));
			Assert.AreEqual ("23", Druid.ToModuleString (0x4000100002000003));
		}

		[Test]
		public void CheckIsValid()
		{
			Assert.IsTrue (Druid.IsValidFullString ("1023000000008"));
			Assert.IsTrue (Druid.IsValidModuleString ("VVV"));
			Assert.IsTrue (Druid.IsValidModuleString ("VVV000000"));
			Assert.IsFalse (Druid.IsValidFullString ("10230000000081"));
			Assert.IsFalse (Druid.IsValidModuleString ("VVV0000001"));
			Assert.IsFalse (Druid.IsValidModuleString (""));
			Assert.IsFalse (Druid.IsValidModuleString (null));
			Assert.IsFalse (Druid.IsValidModuleString ("abc"));
		}

		[Test]
		public void CheckParse()
		{
			Assert.AreEqual (0x0000000000000000L, Druid.Parse ("[0]").ToLong ());
			Assert.AreEqual (0x0000100002000003L, Druid.Parse ("[1023]").ToLong ());
			Assert.AreEqual (0x0000100002000100L, Druid.Parse ("[10208]").ToLong ());
			Assert.AreEqual (0x0000100011000100L, Druid.Parse ("[10H08]").ToLong ());
			Assert.AreEqual (0x000010001f0003ffL, Druid.Parse ("[10VVV]").ToLong ());
			Assert.AreEqual (0x000010001f000400L, Druid.Parse ("[10V0001]").ToLong ());
			Assert.AreEqual (0x0000100042000400L, Druid.Parse ("[1020021]").ToLong ());
			Assert.AreEqual (0x0040100002000003L, Druid.Parse ("[1023000001]").ToLong ());
			Assert.AreEqual (0x4000100002000003L, Druid.Parse ("[1023000000008]").ToLong ());

		}
	}
}
