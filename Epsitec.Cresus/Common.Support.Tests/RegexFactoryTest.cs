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
			
			Assertion.Assert (ma.Success == true);
			Assertion.Assert (mb.Success == false);
			Assertion.Assert (mc.Success == false);
			
			ma = r2.Match ("a.b.c");
			mb = r2.Match ("A.B.C");
			mc = r2.Match ("x.a.b.c.z");
			
			Assertion.Assert (ma.Success == true);
			Assertion.Assert (mb.Success == false);
			Assertion.Assert (mc.Success == false);
			
			ma = r3.Match ("a.b.c");
			mb = r3.Match ("A.B.C");
			mc = r3.Match ("x.a.b.c.z");
			
			Assertion.Assert (ma.Success == true);
			Assertion.Assert (mb.Success == true);
			Assertion.Assert (mc.Success == false);
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
			
			Assertion.Assert (ma.Success == true);
			Assertion.Assert (mb.Success == true);
			Assertion.Assert (mc.Success == false);
			Assertion.Assert (md.Success == false);
			
			ma = r2.Match ("a.b.c");
			mb = r2.Match ("x.a.b.c");
			mc = r2.Match ("a.b.c.z");
			md = r2.Match ("x.a.b.c.z");
			
			Assertion.Assert (ma.Success == true);
			Assertion.Assert (mb.Success == false);
			Assertion.Assert (mc.Success == true);
			Assertion.Assert (md.Success == false);
			
			ma = r3.Match ("a.b.c");
			mb = r3.Match ("x.a.b.c");
			mc = r3.Match ("a.b.c.z");
			md = r3.Match ("x.a.b.c.z");
			
			Assertion.Assert (ma.Success == true);
			Assertion.Assert (mb.Success == true);
			Assertion.Assert (mc.Success == true);
			Assertion.Assert (md.Success == true);
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
			
			Assertion.Assert (ma.Success == true);
			Assertion.Assert (mb.Success == true);
			Assertion.Assert (mc.Success == false);
			Assertion.Assert (md.Success == false);
			
			Assertion.AssertEquals (2, ma.Groups.Count);
			Assertion.AssertEquals ("a.b.c", ma.Groups[0].Value);
			Assertion.AssertEquals ("", ma.Groups[1].Value);
			
			Assertion.AssertEquals (2, mb.Groups.Count);
			Assertion.AssertEquals ("x.a.b.c", mb.Groups[0].Value);
			Assertion.AssertEquals ("x.", mb.Groups[1].Value);
			
			ma = r2.Match ("a.b.c");
			mb = r2.Match ("x.a.b.c");
			mc = r2.Match ("a.b.c.z");
			md = r2.Match ("x.a.b.c.z");
			
			Assertion.Assert (ma.Success == true);
			Assertion.Assert (mb.Success == false);
			Assertion.Assert (mc.Success == true);
			Assertion.Assert (md.Success == false);
			
			ma = r3.Match ("a.b.c");
			mb = r3.Match ("x.a.b.c");
			mc = r3.Match ("a.b.c.z");
			md = r3.Match ("x.a.b.c.z");
			
			Assertion.Assert (ma.Success == true);
			Assertion.Assert (mb.Success == true);
			Assertion.Assert (mc.Success == true);
			Assertion.Assert (md.Success == true);
			
			Assertion.AssertEquals (3, mb.Groups.Count);
			Assertion.AssertEquals ("x.a.b.c.z", md.Groups[0].Value);
			Assertion.AssertEquals ("x.", md.Groups[1].Value);
			Assertion.AssertEquals (".z", md.Groups[2].Value);
			
			ma = r4.Match ("a.b.c");
			mb = r4.Match ("x.a.b.c");
			mc = r4.Match ("a.b.c.z");
			md = r4.Match ("x.a.b.c.z");
			
			Assertion.Assert (ma.Success == true);
			Assertion.Assert (mb.Success == true);
			Assertion.Assert (mc.Success == true);
			Assertion.Assert (md.Success == true);
			
			Assertion.AssertEquals (4, mb.Groups.Count);
			Assertion.AssertEquals ("x.a.b.c.z", md.Groups[0].Value);
			Assertion.AssertEquals ("x.", md.Groups[1].Value);
			Assertion.AssertEquals ("b",  md.Groups[2].Value);
			Assertion.AssertEquals (".z", md.Groups[3].Value);
		}
		
		
		[Test] public void CheckAlphaName()
		{
			Assertion.AssertEquals (true,  RegexFactory.AlphaName.IsMatch ("aAb"));
			Assertion.AssertEquals (false, RegexFactory.AlphaName.IsMatch ("a_b"));
			Assertion.AssertEquals (false, RegexFactory.AlphaName.IsMatch ("a1b"));
			Assertion.AssertEquals (false, RegexFactory.AlphaName.IsMatch ("_ab"));
			Assertion.AssertEquals (false, RegexFactory.AlphaName.IsMatch ("1ab"));
			Assertion.AssertEquals (false, RegexFactory.AlphaName.IsMatch ("a b"));
			Assertion.AssertEquals (false, RegexFactory.AlphaName.IsMatch ("a.b"));
		}
		
		[Test] public void CheckAlphaNumName()
		{
			Assertion.AssertEquals (true,  RegexFactory.AlphaNumName.IsMatch ("aAb"));
			Assertion.AssertEquals (true,  RegexFactory.AlphaNumName.IsMatch ("a_b"));
			Assertion.AssertEquals (true,  RegexFactory.AlphaNumName.IsMatch ("a1b"));
			Assertion.AssertEquals (true,  RegexFactory.AlphaNumName.IsMatch ("_ab"));
			Assertion.AssertEquals (false, RegexFactory.AlphaNumName.IsMatch ("1ab"));
			Assertion.AssertEquals (false, RegexFactory.AlphaNumName.IsMatch ("a b"));
			Assertion.AssertEquals (false, RegexFactory.AlphaNumName.IsMatch ("a.b"));
		}
		
		[Test] public void CheckAlphaDotName()
		{
			Assertion.AssertEquals (true,  RegexFactory.AlphaDotName.IsMatch ("aAb"));
			Assertion.AssertEquals (true,  RegexFactory.AlphaDotName.IsMatch ("a_b"));
			Assertion.AssertEquals (true,  RegexFactory.AlphaDotName.IsMatch ("a1b"));
			Assertion.AssertEquals (true,  RegexFactory.AlphaDotName.IsMatch ("_ab"));
			Assertion.AssertEquals (false, RegexFactory.AlphaDotName.IsMatch ("1ab"));
			Assertion.AssertEquals (true,  RegexFactory.AlphaDotName.IsMatch ("a.b"));
			Assertion.AssertEquals (false, RegexFactory.AlphaDotName.IsMatch ("a.b."));
			Assertion.AssertEquals (false, RegexFactory.AlphaDotName.IsMatch (".a.b"));
			Assertion.AssertEquals (false, RegexFactory.AlphaDotName.IsMatch ("a#b"));
			Assertion.AssertEquals (false, RegexFactory.AlphaDotName.IsMatch ("a b"));
		}
		
		[Test] public void CheckFileName()
		{
			Assertion.AssertEquals (true,  RegexFactory.FileName.IsMatch ("aAb"));
			Assertion.AssertEquals (true,  RegexFactory.FileName.IsMatch ("a_b"));
			Assertion.AssertEquals (true,  RegexFactory.FileName.IsMatch ("a1b"));
			Assertion.AssertEquals (true,  RegexFactory.FileName.IsMatch ("_ab"));
			Assertion.AssertEquals (true,  RegexFactory.FileName.IsMatch ("1ab"));
			Assertion.AssertEquals (true,  RegexFactory.FileName.IsMatch ("a.b"));
			Assertion.AssertEquals (false, RegexFactory.FileName.IsMatch ("a.b."));
			Assertion.AssertEquals (false, RegexFactory.FileName.IsMatch (".a.b"));
			Assertion.AssertEquals (false, RegexFactory.FileName.IsMatch ("ab "));
			Assertion.AssertEquals (false, RegexFactory.FileName.IsMatch ("a  b"));
			Assertion.AssertEquals (true,  RegexFactory.FileName.IsMatch ("a b"));
			Assertion.AssertEquals (true,  RegexFactory.FileName.IsMatch ("$(x-'\"y+z)!"));
			Assertion.AssertEquals (true,  RegexFactory.FileName.IsMatch ("(x---z)!"));
			Assertion.AssertEquals (false, RegexFactory.FileName.IsMatch ("(x...z)!"));
			Assertion.AssertEquals (false, RegexFactory.FileName.IsMatch ("(x/y)"));
			Assertion.AssertEquals (false, RegexFactory.FileName.IsMatch ("(x\\y)"));
			Assertion.AssertEquals (false, RegexFactory.FileName.IsMatch ("(x#y)"));
		}
		
		[Test] public void CheckResourceName()
		{
			Assertion.AssertEquals (true,  RegexFactory.ResourceName.IsMatch ("aAb"));
			Assertion.AssertEquals (true,  RegexFactory.ResourceName.IsMatch ("a_b"));
			Assertion.AssertEquals (true,  RegexFactory.ResourceName.IsMatch ("a1b"));
			Assertion.AssertEquals (true,  RegexFactory.ResourceName.IsMatch ("_ab"));
			Assertion.AssertEquals (false, RegexFactory.ResourceName.IsMatch ("1ab"));
			Assertion.AssertEquals (true,  RegexFactory.ResourceName.IsMatch ("a.b"));
			Assertion.AssertEquals (true,  RegexFactory.ResourceName.IsMatch ("a#b"));
			Assertion.AssertEquals (true,  RegexFactory.ResourceName.IsMatch ("a#b[0]"));
			Assertion.AssertEquals (false, RegexFactory.ResourceName.IsMatch ("a#b[x]"));
			Assertion.AssertEquals (false, RegexFactory.ResourceName.IsMatch ("a#b[0]]"));
			Assertion.AssertEquals (false, RegexFactory.ResourceName.IsMatch ("a.b."));
			Assertion.AssertEquals (false, RegexFactory.ResourceName.IsMatch (".a.b"));
			Assertion.AssertEquals (false, RegexFactory.ResourceName.IsMatch ("ab "));
			Assertion.AssertEquals (false, RegexFactory.ResourceName.IsMatch ("a  b"));
			Assertion.AssertEquals (false, RegexFactory.ResourceName.IsMatch ("a b"));
			Assertion.AssertEquals (false, RegexFactory.ResourceName.IsMatch ("$(x-'\"y+z)!"));
			Assertion.AssertEquals (false, RegexFactory.ResourceName.IsMatch ("(x---z)!"));
			Assertion.AssertEquals (false, RegexFactory.ResourceName.IsMatch ("(x...z)!"));
			Assertion.AssertEquals (false, RegexFactory.ResourceName.IsMatch ("(x/y)"));
			Assertion.AssertEquals (false, RegexFactory.ResourceName.IsMatch ("(x\\y)"));
			Assertion.AssertEquals (false, RegexFactory.ResourceName.IsMatch ("(x#y)"));
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
