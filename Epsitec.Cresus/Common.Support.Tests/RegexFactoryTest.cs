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
