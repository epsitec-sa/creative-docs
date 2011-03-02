//	Copyright © 2006-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Responsable: Pierre ARNAUD

using NUnit.Framework;
using Epsitec.Common.Support;

namespace Epsitec.Common.Tests.Support
{
	[TestFixture] public class DruidTest
	{
		[Test]
		public void CheckEscape()
		{
			Assert.AreEqual ("abc", Druid.Escape ("abc"));
			Assert.AreEqual ("123", Druid.Escape ("123"));
			Assert.AreEqual ("x[x]", Druid.Escape ("x[x]"));
			Assert.AreEqual ("][x]", Druid.Escape ("[x]"));
			Assert.AreEqual ("]_x", Druid.Escape ("_x"));
			Assert.AreEqual ("]$x", Druid.Escape ("$x"));
			
			Assert.AreEqual ("x[x]", Druid.Unescape ("x[x]"));
			Assert.AreEqual ("[x]", Druid.Unescape ("][x]"));
			Assert.AreEqual ("_x", Druid.Unescape ("]_x"));
			Assert.AreEqual ("$x", Druid.Unescape ("]$x"));
		}
		
		[Test]
		public void CheckFromFullString()
		{
			Assert.AreEqual (0x0000000000000000L, Druid.FromFullString ("0"));
			Assert.AreEqual (0x0000100002000003L, Druid.FromFullString ("1023"));
			Assert.AreEqual (0x0000100002000100L, Druid.FromFullString ("10208"));
			Assert.AreEqual (0x0000100011000100L, Druid.FromFullString ("10H08"));
			Assert.AreEqual (0x000010001f0003ffL, Druid.FromFullString ("10VVV"));
			Assert.AreEqual (0x000010001f000400L, Druid.FromFullString ("10V00001"));
			Assert.AreEqual (0x0000100042000400L, Druid.FromFullString ("10200201"));
			Assert.AreEqual (0x0040100002000003L, Druid.FromFullString ("1023001"));
			Assert.AreEqual (0x4000100002000003L, Druid.FromFullString ("1023000008"));
			Assert.AreEqual (0x40001F8002F00003L, Druid.FromFullString ("10230000080VF"));

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
			Assert.AreEqual ("10V00001", Druid.ToFullString (0x000010001f000400));
			Assert.AreEqual ("10200201", Druid.ToFullString (0x0000100042000400));
			Assert.AreEqual ("1023001", Druid.ToFullString (0x0040100002000003));
			Assert.AreEqual ("1023000008", Druid.ToFullString (0x4000100002000003));
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
		public void CheckMiscConversions()
		{
			Druid druid = Druid.Parse ("[1023000008]");

			Assert.AreEqual (2, druid.DeveloperAndPatchLevel);
			Assert.AreEqual (3, druid.Local);
			Assert.AreEqual (0x40001, druid.Module);
			
			Assert.AreEqual (0x4000100002000003L, druid.ToLong ());
			Assert.AreEqual (0x0000000002000003L, druid.ToFieldId ());
			Assert.AreEqual ("[1023000008]", druid.ToResourceId ());
			Assert.AreEqual ("$23", druid.ToFieldName ());

			Assert.AreEqual (DruidType.Full, druid.Type);
			Assert.AreEqual (DruidType.ModuleRelative, new Druid (2, 3).Type);
			Assert.AreEqual (DruidType.Invalid, new Druid ().Type);
			Assert.AreEqual (DruidType.Full, new Druid (druid).Type);

			Assert.AreEqual ("$23", new Druid (2, 3).ToFieldName ());
			Assert.AreEqual ("[1023]", new Druid (new Druid (1, 2, 3)).ToResourceId ());
		}

		[Test]
		public void CheckParse()
		{
			Assert.AreEqual (0x0000000000000000L, Druid.Parse ("[0]").ToLong ());
			Assert.AreEqual (0x0000100002000003L, Druid.Parse ("[1023]").ToLong ());
			Assert.AreEqual (0x0000100002000100L, Druid.Parse ("[10208]").ToLong ());
			Assert.AreEqual (0x0000100011000100L, Druid.Parse ("[10H08]").ToLong ());
			Assert.AreEqual (0x000010001f0003ffL, Druid.Parse ("[10VVV]").ToLong ());
			Assert.AreEqual (0x000010001f000400L, Druid.Parse ("[10V00001]").ToLong ());
			Assert.AreEqual (0x0000100042000400L, Druid.Parse ("[10200201]").ToLong ());
			Assert.AreEqual (0x0040100002000003L, Druid.Parse ("[1023001]").ToLong ());
			Assert.AreEqual (0x4000100002000003L, Druid.Parse ("[1023000008]").ToLong ());

			Assert.AreEqual (0x0000000002000003L, Druid.Parse ("23").ToLong ());
			Assert.AreEqual (0x000010001f0003ffL, Druid.Parse ("_10VVV").ToLong ());

			Assert.AreEqual (0x00000000000L, Druid.Parse ("$0").ToLong ());
			Assert.AreEqual (0x00002000003L, Druid.Parse ("$23").ToLong ());
			Assert.AreEqual (0x00002000100L, Druid.Parse ("$208").ToLong ());
			Assert.AreEqual (0x00011000100L, Druid.Parse ("$H08").ToLong ());
			Assert.AreEqual (0x0001f0003ffL, Druid.Parse ("$VVV").ToLong ());
			Assert.AreEqual (0x0001f000400L, Druid.Parse ("$V0001").ToLong ());
			Assert.AreEqual (0x00042000400L, Druid.Parse ("$20021").ToLong ());
			
			Assert.AreEqual (0x00011000100L, Druid.Parse ("H08").ToLong ());
		}

		[Test]
		[ExpectedException (typeof (System.FormatException))]
		public void CheckParseEx1()
		{
			Druid.Parse ("$23000000008");
		}

		[Test]
		public void CheckTemporaryDruids()
		{
			Druid id1 = Druid.CreateTemporaryDruid ();
			Druid id2 = Druid.CreateTemporaryDruid ();

			System.Console.Out.WriteLine (id1.ToString ());

			Assert.IsTrue (id1.IsTemporary);
			Assert.AreEqual (id1.Module, id2.Module);
			Assert.AreEqual (id1.Local+1, id2.Local);
		}

		[Test]
		public void CheckTryParse()
		{
			Druid druid;
			
			Assert.IsTrue (Druid.TryParse ("[0]", out druid));
			Assert.AreEqual (0x0000000000000000L, druid.ToLong ());

			Assert.IsTrue (Druid.TryParse ("[1023000008]", out druid));
			Assert.AreEqual (0x4000100002000003L, druid.ToLong ());

			Assert.IsTrue (Druid.TryParse ("_1023000008", out druid));
			Assert.AreEqual (0x4000100002000003L, druid.ToLong ());

			Assert.IsTrue (Druid.TryParse ("$VVV", out druid));
			Assert.AreEqual (0x0001f0003ffL, druid.ToLong ());

			Assert.IsFalse (Druid.TryParse ("$XYZ", out druid));
			Assert.IsFalse (Druid.TryParse ("[X]", out druid));
			Assert.IsFalse (Druid.TryParse ("_X", out druid));
			Assert.IsFalse (Druid.TryParse ("1234", out druid));
			Assert.IsFalse (Druid.TryParse ("xyz", out druid));
			Assert.IsFalse (Druid.TryParse ("][", out druid));
			Assert.IsFalse (Druid.TryParse ("[]", out druid));
		}
	}
}
