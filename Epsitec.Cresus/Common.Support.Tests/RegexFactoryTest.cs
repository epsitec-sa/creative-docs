using NUnit.Framework;
using System.Text.RegularExpressions;

namespace Epsitec.Common.Support
{
	[TestFixture] public class RegexFactoryTest
	{
		[Test] public void CheckFromSimpleJoker1()
		{
			Regex r1 = RegexFactory.FromSimpleJoker ("a.b.c", RegexFactory.Options.None);
			Regex r2 = RegexFactory.FromSimpleJoker ("a.b.c", RegexFactory.Options.Compiled);
			Regex r3 = RegexFactory.FromSimpleJoker ("a.b.c", RegexFactory.Options.IgnoreCase);
			
			Match ma, mb, mc;
			
			ma = r1.Match ("a.b.c");
			mb = r1.Match ("A.B.C");
			mc = r1.Match ("x.a.b.c.z");
			
			Assert.IsTrue (ma.Success == true);
			Assert.IsTrue (mb.Success == false);
			Assert.IsTrue (mc.Success == false);
			
			ma = r2.Match ("a.b.c");
			mb = r2.Match ("A.B.C");
			mc = r2.Match ("x.a.b.c.z");
			
			Assert.IsTrue (ma.Success == true);
			Assert.IsTrue (mb.Success == false);
			Assert.IsTrue (mc.Success == false);
			
			ma = r3.Match ("a.b.c");
			mb = r3.Match ("A.B.C");
			mc = r3.Match ("x.a.b.c.z");
			
			Assert.IsTrue (ma.Success == true);
			Assert.IsTrue (mb.Success == true);
			Assert.IsTrue (mc.Success == false);
		}
		
		[Test] public void CheckFromSimpleJoker2()
		{
			Regex r1 = RegexFactory.FromSimpleJoker ("*a.b.c", RegexFactory.Options.None);
			Regex r2 = RegexFactory.FromSimpleJoker ("a.b.c*", RegexFactory.Options.None);
			Regex r3 = RegexFactory.FromSimpleJoker ("*a.b.c*", RegexFactory.Options.None);
			
			Match ma, mb, mc, md;
			
			ma = r1.Match ("a.b.c");
			mb = r1.Match ("x.a.b.c");
			mc = r1.Match ("a.b.c.z");
			md = r1.Match ("x.a.b.c.z");
			
			Assert.IsTrue (ma.Success == true);
			Assert.IsTrue (mb.Success == true);
			Assert.IsTrue (mc.Success == false);
			Assert.IsTrue (md.Success == false);
			
			ma = r2.Match ("a.b.c");
			mb = r2.Match ("x.a.b.c");
			mc = r2.Match ("a.b.c.z");
			md = r2.Match ("x.a.b.c.z");
			
			Assert.IsTrue (ma.Success == true);
			Assert.IsTrue (mb.Success == false);
			Assert.IsTrue (mc.Success == true);
			Assert.IsTrue (md.Success == false);
			
			ma = r3.Match ("a.b.c");
			mb = r3.Match ("x.a.b.c");
			mc = r3.Match ("a.b.c.z");
			md = r3.Match ("x.a.b.c.z");
			
			Assert.IsTrue (ma.Success == true);
			Assert.IsTrue (mb.Success == true);
			Assert.IsTrue (mc.Success == true);
			Assert.IsTrue (md.Success == true);
		}
		
		[Test] public void CheckFromSimpleJoker3()
		{
			Regex r1 = RegexFactory.FromSimpleJoker ("*a.b.c", RegexFactory.Options.Capture);
			Regex r2 = RegexFactory.FromSimpleJoker ("a.b.c*", RegexFactory.Options.Capture);
			Regex r3 = RegexFactory.FromSimpleJoker ("*a.b.c*", RegexFactory.Options.Capture);
			Regex r4 = RegexFactory.FromSimpleJoker ("*a.*.c*", RegexFactory.Options.Capture);
			
			Match ma, mb, mc, md;
			
			ma = r1.Match ("a.b.c");
			mb = r1.Match ("x.a.b.c");
			mc = r1.Match ("a.b.c.z");
			md = r1.Match ("x.a.b.c.z");
			
			Assert.IsTrue (ma.Success == true);
			Assert.IsTrue (mb.Success == true);
			Assert.IsTrue (mc.Success == false);
			Assert.IsTrue (md.Success == false);
			
			Assert.AreEqual (2, ma.Groups.Count);
			Assert.AreEqual ("a.b.c", ma.Groups[0].Value);
			Assert.AreEqual ("", ma.Groups[1].Value);
			
			Assert.AreEqual (2, mb.Groups.Count);
			Assert.AreEqual ("x.a.b.c", mb.Groups[0].Value);
			Assert.AreEqual ("x.", mb.Groups[1].Value);
			
			ma = r2.Match ("a.b.c");
			mb = r2.Match ("x.a.b.c");
			mc = r2.Match ("a.b.c.z");
			md = r2.Match ("x.a.b.c.z");
			
			Assert.IsTrue (ma.Success == true);
			Assert.IsTrue (mb.Success == false);
			Assert.IsTrue (mc.Success == true);
			Assert.IsTrue (md.Success == false);
			
			ma = r3.Match ("a.b.c");
			mb = r3.Match ("x.a.b.c");
			mc = r3.Match ("a.b.c.z");
			md = r3.Match ("x.a.b.c.z");
			
			Assert.IsTrue (ma.Success == true);
			Assert.IsTrue (mb.Success == true);
			Assert.IsTrue (mc.Success == true);
			Assert.IsTrue (md.Success == true);
			
			Assert.AreEqual (3, mb.Groups.Count);
			Assert.AreEqual ("x.a.b.c.z", md.Groups[0].Value);
			Assert.AreEqual ("x.", md.Groups[1].Value);
			Assert.AreEqual (".z", md.Groups[2].Value);
			
			ma = r4.Match ("a.b.c");
			mb = r4.Match ("x.a.b.c");
			mc = r4.Match ("a.b.c.z");
			md = r4.Match ("x.a.b.c.z");
			
			Assert.IsTrue (ma.Success == true);
			Assert.IsTrue (mb.Success == true);
			Assert.IsTrue (mc.Success == true);
			Assert.IsTrue (md.Success == true);
			
			Assert.AreEqual (4, mb.Groups.Count);
			Assert.AreEqual ("x.a.b.c.z", md.Groups[0].Value);
			Assert.AreEqual ("x.", md.Groups[1].Value);
			Assert.AreEqual ("b",  md.Groups[2].Value);
			Assert.AreEqual (".z", md.Groups[3].Value);
		}
		
		
		[Test] public void CheckAlphaName()
		{
			Assert.AreEqual (true,  RegexFactory.AlphaName.IsMatch ("aAb"));
			Assert.AreEqual (false, RegexFactory.AlphaName.IsMatch ("a_b"));
			Assert.AreEqual (false, RegexFactory.AlphaName.IsMatch ("a1b"));
			Assert.AreEqual (false, RegexFactory.AlphaName.IsMatch ("_ab"));
			Assert.AreEqual (false, RegexFactory.AlphaName.IsMatch ("1ab"));
			Assert.AreEqual (false, RegexFactory.AlphaName.IsMatch ("a b"));
			Assert.AreEqual (false, RegexFactory.AlphaName.IsMatch ("a.b"));
		}
		
		[Test] public void CheckAlphaNumName()
		{
			Assert.AreEqual (true,  RegexFactory.AlphaNumName.IsMatch ("aAb"));
			Assert.AreEqual (true,  RegexFactory.AlphaNumName.IsMatch ("a_b"));
			Assert.AreEqual (true,  RegexFactory.AlphaNumName.IsMatch ("a1b"));
			Assert.AreEqual (true,  RegexFactory.AlphaNumName.IsMatch ("_ab"));
			Assert.AreEqual (false, RegexFactory.AlphaNumName.IsMatch ("1ab"));
			Assert.AreEqual (false, RegexFactory.AlphaNumName.IsMatch ("a b"));
			Assert.AreEqual (false, RegexFactory.AlphaNumName.IsMatch ("a.b"));
		}
		
		[Test] public void CheckAlphaNumDotName()
		{
			Assert.AreEqual (true,  RegexFactory.AlphaNumDotName.IsMatch ("aAb"));
			Assert.AreEqual (true,  RegexFactory.AlphaNumDotName.IsMatch ("a_b"));
			Assert.AreEqual (true,  RegexFactory.AlphaNumDotName.IsMatch ("a1b"));
			Assert.AreEqual (true,  RegexFactory.AlphaNumDotName.IsMatch ("_ab"));
			Assert.AreEqual (false, RegexFactory.AlphaNumDotName.IsMatch ("1ab"));
			Assert.AreEqual (true,  RegexFactory.AlphaNumDotName.IsMatch ("a.b"));
			Assert.AreEqual (false, RegexFactory.AlphaNumDotName.IsMatch ("a.b."));
			Assert.AreEqual (false, RegexFactory.AlphaNumDotName.IsMatch (".a.b"));
			Assert.AreEqual (false, RegexFactory.AlphaNumDotName.IsMatch ("a#b"));
			Assert.AreEqual (false, RegexFactory.AlphaNumDotName.IsMatch ("a b"));
		}
		
		[Test] public void CheckFileName()
		{
			Assert.AreEqual (true,  RegexFactory.FileName.IsMatch ("aAb"));
			Assert.AreEqual (true,  RegexFactory.FileName.IsMatch ("a_b"));
			Assert.AreEqual (true,  RegexFactory.FileName.IsMatch ("a1b"));
			Assert.AreEqual (true,  RegexFactory.FileName.IsMatch ("_ab"));
			Assert.AreEqual (true,  RegexFactory.FileName.IsMatch ("1ab"));
			Assert.AreEqual (true,  RegexFactory.FileName.IsMatch ("a.b"));
			Assert.AreEqual (false, RegexFactory.FileName.IsMatch ("a.b."));
			Assert.AreEqual (false, RegexFactory.FileName.IsMatch (".a.b"));
			Assert.AreEqual (false, RegexFactory.FileName.IsMatch ("ab "));
			Assert.AreEqual (false, RegexFactory.FileName.IsMatch ("a  b"));
			Assert.AreEqual (true,  RegexFactory.FileName.IsMatch ("a b"));
			Assert.AreEqual (true,  RegexFactory.FileName.IsMatch ("$(x-'\"y+z)!"));
			Assert.AreEqual (true,  RegexFactory.FileName.IsMatch ("(x---z)!"));
			Assert.AreEqual (false, RegexFactory.FileName.IsMatch ("(x...z)!"));
			Assert.AreEqual (false, RegexFactory.FileName.IsMatch ("(x/y)"));
			Assert.AreEqual (false, RegexFactory.FileName.IsMatch ("(x\\y)"));
			Assert.AreEqual (false, RegexFactory.FileName.IsMatch ("(x#y)"));
		}
		
		[Test] public void CheckResourceName()
		{
			Assert.AreEqual (true,  RegexFactory.ResourceName.IsMatch ("aAb"));
			Assert.AreEqual (true,  RegexFactory.ResourceName.IsMatch ("a_b"));
			Assert.AreEqual (true,  RegexFactory.ResourceName.IsMatch ("a1b"));
			Assert.AreEqual (true,  RegexFactory.ResourceName.IsMatch ("_ab"));
			Assert.AreEqual (false, RegexFactory.ResourceName.IsMatch ("1ab"));
			Assert.AreEqual (true,  RegexFactory.ResourceName.IsMatch ("a.b"));
			Assert.AreEqual (true,  RegexFactory.ResourceName.IsMatch ("a#b"));
			Assert.AreEqual (true,  RegexFactory.ResourceName.IsMatch ("a#b[0]"));
			Assert.AreEqual (false, RegexFactory.ResourceName.IsMatch ("a#b[x]"));
			Assert.AreEqual (false, RegexFactory.ResourceName.IsMatch ("a#b[0]]"));
			Assert.AreEqual (false, RegexFactory.ResourceName.IsMatch ("a.b."));
			Assert.AreEqual (false, RegexFactory.ResourceName.IsMatch (".a.b"));
			Assert.AreEqual (false, RegexFactory.ResourceName.IsMatch ("ab "));
			Assert.AreEqual (false, RegexFactory.ResourceName.IsMatch ("a  b"));
			Assert.AreEqual (false, RegexFactory.ResourceName.IsMatch ("a b"));
			Assert.AreEqual (false, RegexFactory.ResourceName.IsMatch ("$(x-'\"y+z)!"));
			Assert.AreEqual (false, RegexFactory.ResourceName.IsMatch ("(x---z)!"));
			Assert.AreEqual (false, RegexFactory.ResourceName.IsMatch ("(x...z)!"));
			Assert.AreEqual (false, RegexFactory.ResourceName.IsMatch ("(x/y)"));
			Assert.AreEqual (false, RegexFactory.ResourceName.IsMatch ("(x\\y)"));
			Assert.AreEqual (false, RegexFactory.ResourceName.IsMatch ("(x#y)"));
		}
		
		
		protected void DumpGroups(GroupCollection groups)
		{
			System.Console.Out.WriteLine ("Group has {0} elements : ", groups.Count);
			
			foreach (Group group in groups)
			{
				if (group.Success)
				{
					System.Console.Out.WriteLine ("  group: {0}", group.Value);
				}
			}
		}
	}
}
