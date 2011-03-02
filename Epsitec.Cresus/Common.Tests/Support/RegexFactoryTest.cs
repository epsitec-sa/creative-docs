using NUnit.Framework;
using System.Text.RegularExpressions;
using Epsitec.Common.Support;

namespace Epsitec.Common.Tests.Support
{
	[TestFixture]
	public class RegexFactoryTest
	{
		[Test]
		public void CheckFromSimpleJoker1()
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

		[Test]
		public void CheckFromSimpleJoker2()
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

		[Test]
		public void CheckFromSimpleJoker3()
		{
			Regex r1 = RegexFactory.FromSimpleJoker ("*a.b.c", RegexFactory.Options.Capture);
			Regex r2 = RegexFactory.FromSimpleJoker ("a.b.c*", RegexFactory.Options.Capture);
			Regex r3 = RegexFactory.FromSimpleJoker ("*a.b.c*", RegexFactory.Options.Capture);
			Regex r4 = RegexFactory.FromSimpleJoker ("*a.*.c*", RegexFactory.Options.Capture);
			Regex r5 = RegexFactory.FromSimpleJoker ("x\\*\\.\\\\x\\?x", RegexFactory.Options.Capture);

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
			Assert.AreEqual ("b", md.Groups[2].Value);
			Assert.AreEqual (".z", md.Groups[3].Value);

			Assert.IsTrue (r5.IsMatch ("x*.\\x?x"));
		}

		[Test]
		public void CheckFromSimpleJoker4()
		{
			Regex r1 = RegexFactory.FromSimpleJoker ("*.jpg|*.bmp|*.tiff", RegexFactory.Options.Capture | RegexFactory.Options.IgnoreCase);

			Match ma, mb, mc, md, me;

			ma = r1.Match ("foo.txt");
			mb = r1.Match ("bar.jpg");
			mc = r1.Match ("bar.jpg ");
			md = r1.Match ("bar.bmp");
			me = r1.Match ("bar.tiff");

			Assert.IsFalse (ma.Success);
			Assert.IsTrue (mb.Success);
			Assert.IsFalse (mc.Success);
			Assert.IsTrue (md.Success);
			Assert.IsTrue (me.Success);

			Assert.AreEqual ("bar", mb.Groups[1].Value);
			Assert.AreEqual ("bar", md.Groups[2].Value);
			Assert.AreEqual ("bar", me.Groups[3].Value);
		}

		[Test]
		[ExpectedException (typeof (System.ArgumentException))]
		public void CheckFromSimpleJokerEx1()
		{
			Regex r = RegexFactory.FromSimpleJoker (null);
		}

		[Test]
		[ExpectedException (typeof (System.ArgumentException))]
		public void CheckFromSimpleJokerEx2()
		{
			Regex r = RegexFactory.FromSimpleJoker ("  ");
		}


		[Test]
		public void CheckAlphaName()
		{
			Assert.AreEqual (true, RegexFactory.AlphaName.IsMatch ("aAb"));
			Assert.AreEqual (false, RegexFactory.AlphaName.IsMatch ("a_b"));
			Assert.AreEqual (false, RegexFactory.AlphaName.IsMatch ("a1b"));
			Assert.AreEqual (false, RegexFactory.AlphaName.IsMatch ("_ab"));
			Assert.AreEqual (false, RegexFactory.AlphaName.IsMatch ("1ab"));
			Assert.AreEqual (false, RegexFactory.AlphaName.IsMatch ("a b"));
			Assert.AreEqual (false, RegexFactory.AlphaName.IsMatch ("a.b"));
		}

		[Test]
		public void CheckAlphaNumName()
		{
			Assert.AreEqual (true, RegexFactory.AlphaNumName.IsMatch ("aAb"));
			Assert.AreEqual (true, RegexFactory.AlphaNumName.IsMatch ("a_b"));
			Assert.AreEqual (true, RegexFactory.AlphaNumName.IsMatch ("a1b"));
			Assert.AreEqual (true, RegexFactory.AlphaNumName.IsMatch ("_ab"));
			Assert.AreEqual (false, RegexFactory.AlphaNumName.IsMatch ("1ab"));
			Assert.AreEqual (false, RegexFactory.AlphaNumName.IsMatch ("a b"));
			Assert.AreEqual (false, RegexFactory.AlphaNumName.IsMatch ("a.b"));
		}

		[Test]
		public void CheckAlphaNumDotName()
		{
			Assert.AreEqual (true, RegexFactory.AlphaNumDotName1.IsMatch ("aAb"));
			Assert.AreEqual (true, RegexFactory.AlphaNumDotName1.IsMatch ("a_b"));
			Assert.AreEqual (true, RegexFactory.AlphaNumDotName1.IsMatch ("a1b"));
			Assert.AreEqual (true, RegexFactory.AlphaNumDotName1.IsMatch ("_ab"));
			Assert.AreEqual (false, RegexFactory.AlphaNumDotName1.IsMatch ("1ab"));
			Assert.AreEqual (true, RegexFactory.AlphaNumDotName1.IsMatch ("a.b"));
			Assert.AreEqual (false, RegexFactory.AlphaNumDotName1.IsMatch ("a.b."));
			Assert.AreEqual (false, RegexFactory.AlphaNumDotName1.IsMatch (".a.b"));
			Assert.AreEqual (false, RegexFactory.AlphaNumDotName1.IsMatch ("a#b"));
			Assert.AreEqual (false, RegexFactory.AlphaNumDotName1.IsMatch ("a b"));
		}

		[Test]
		public void CheckFileName()
		{
			Assert.AreEqual (true, RegexFactory.FileName.IsMatch ("aAb"));
			Assert.AreEqual (true, RegexFactory.FileName.IsMatch ("a_b"));
			Assert.AreEqual (true, RegexFactory.FileName.IsMatch ("a1b"));
			Assert.AreEqual (true, RegexFactory.FileName.IsMatch ("_ab"));
			Assert.AreEqual (true, RegexFactory.FileName.IsMatch ("1ab"));
			Assert.AreEqual (true, RegexFactory.FileName.IsMatch ("a.b"));
			Assert.AreEqual (false, RegexFactory.FileName.IsMatch ("a.b."));
			Assert.AreEqual (false, RegexFactory.FileName.IsMatch (".a.b"));
			Assert.AreEqual (false, RegexFactory.FileName.IsMatch ("ab "));
			Assert.AreEqual (false, RegexFactory.FileName.IsMatch ("a  b"));
			Assert.AreEqual (true, RegexFactory.FileName.IsMatch ("a b"));
			Assert.AreEqual (true, RegexFactory.FileName.IsMatch ("$(x-'\"y+z)!"));
			Assert.AreEqual (true, RegexFactory.FileName.IsMatch ("(x---z)!"));
			Assert.AreEqual (false, RegexFactory.FileName.IsMatch ("(x...z)!"));
			Assert.AreEqual (false, RegexFactory.FileName.IsMatch ("(x/y)"));
			Assert.AreEqual (false, RegexFactory.FileName.IsMatch ("(x\\y)"));
			Assert.AreEqual (false, RegexFactory.FileName.IsMatch ("(x#y)"));
		}

		[Test]
		public void CheckResourceFullName()
		{
			Assert.AreEqual (true, RegexFactory.ResourceFullName.IsMatch ("aAb"));
			Assert.AreEqual (true, RegexFactory.ResourceFullName.IsMatch ("a_b"));
			Assert.AreEqual (true, RegexFactory.ResourceFullName.IsMatch ("a1b"));
			Assert.AreEqual (true, RegexFactory.ResourceFullName.IsMatch ("_ab"));
			Assert.AreEqual (false, RegexFactory.ResourceFullName.IsMatch ("1ab"));
			Assert.AreEqual (true, RegexFactory.ResourceFullName.IsMatch ("a.b"));
			Assert.AreEqual (true, RegexFactory.ResourceFullName.IsMatch ("a.b.00"));
			Assert.AreEqual (true, RegexFactory.ResourceFullName.IsMatch ("a#b"));
			Assert.AreEqual (true, RegexFactory.ResourceFullName.IsMatch ("a#b[0]"));
			Assert.AreEqual (true, RegexFactory.ResourceFullName.IsMatch ("a.x#b[0]"));
			Assert.AreEqual (true, RegexFactory.ResourceFullName.IsMatch ("a#b.z[0]"));
			Assert.AreEqual (true, RegexFactory.ResourceFullName.IsMatch ("a.x#b.z.y[1234]"));
			Assert.AreEqual (false, RegexFactory.ResourceFullName.IsMatch ("a#b[x]"));
			Assert.AreEqual (false, RegexFactory.ResourceFullName.IsMatch ("a#b[0]]"));
			Assert.AreEqual (false, RegexFactory.ResourceFullName.IsMatch ("a.b."));
			Assert.AreEqual (false, RegexFactory.ResourceFullName.IsMatch (".a.b"));
			Assert.AreEqual (false, RegexFactory.ResourceFullName.IsMatch ("ab "));
			Assert.AreEqual (false, RegexFactory.ResourceFullName.IsMatch ("a  b"));
			Assert.AreEqual (false, RegexFactory.ResourceFullName.IsMatch ("a b"));
			Assert.AreEqual (false, RegexFactory.ResourceFullName.IsMatch ("$(x-'\"y+z)!"));
			Assert.AreEqual (false, RegexFactory.ResourceFullName.IsMatch ("(x---z)!"));
			Assert.AreEqual (false, RegexFactory.ResourceFullName.IsMatch ("(x...z)!"));
			Assert.AreEqual (false, RegexFactory.ResourceFullName.IsMatch ("(x/y)"));
			Assert.AreEqual (false, RegexFactory.ResourceFullName.IsMatch ("(x\\y)"));
			Assert.AreEqual (false, RegexFactory.ResourceFullName.IsMatch ("(x#y)"));
			Assert.AreEqual (false, RegexFactory.ResourceFullName.IsMatch ("x---z"));
			Assert.AreEqual (false, RegexFactory.ResourceFullName.IsMatch ("x...z"));
			Assert.AreEqual (false, RegexFactory.ResourceFullName.IsMatch ("x/y"));
			Assert.AreEqual (false, RegexFactory.ResourceFullName.IsMatch ("x\\y"));
			Assert.AreEqual (false, RegexFactory.ResourceFullName.IsMatch ("a.#b"));
			Assert.AreEqual (false, RegexFactory.ResourceFullName.IsMatch ("a#b."));
			Assert.AreEqual (false, RegexFactory.ResourceFullName.IsMatch ("a#b.[0]"));
			Assert.AreEqual (false, RegexFactory.ResourceFullName.IsMatch ("a[]"));
			Assert.AreEqual (false, RegexFactory.ResourceFullName.IsMatch ("a[12345]"));
			Assert.AreEqual (false, RegexFactory.ResourceFullName.IsMatch ("a[1]#b"));
		}

		[Test]
		public void CheckResourceBundleName()
		{
			Assert.AreEqual (true, RegexFactory.ResourceBundleName.IsMatch ("aAb"));
			Assert.AreEqual (true, RegexFactory.ResourceBundleName.IsMatch ("a_b"));
			Assert.AreEqual (true, RegexFactory.ResourceBundleName.IsMatch ("a1b"));
			Assert.AreEqual (true, RegexFactory.ResourceBundleName.IsMatch ("_ab"));
			Assert.AreEqual (false, RegexFactory.ResourceBundleName.IsMatch ("1ab"));
			Assert.AreEqual (true, RegexFactory.ResourceBundleName.IsMatch ("a.b"));
			Assert.AreEqual (true, RegexFactory.ResourceBundleName.IsMatch ("a.b.c"));
			Assert.AreEqual (true, RegexFactory.ResourceBundleName.IsMatch ("a.b.00"));
			Assert.AreEqual (false, RegexFactory.ResourceBundleName.IsMatch ("a#b"));
			Assert.AreEqual (false, RegexFactory.ResourceBundleName.IsMatch ("a[0]"));
			Assert.AreEqual (false, RegexFactory.ResourceBundleName.IsMatch ("a.b."));
			Assert.AreEqual (false, RegexFactory.ResourceBundleName.IsMatch (".a.b"));
		}

		[Test]
		public void CheckResourceFieldName()
		{
			Assert.AreEqual (true, RegexFactory.ResourceFieldName.IsMatch ("aAb"));
			Assert.AreEqual (true, RegexFactory.ResourceFieldName.IsMatch ("a_b"));
			Assert.AreEqual (true, RegexFactory.ResourceFieldName.IsMatch ("a1b"));
			Assert.AreEqual (true, RegexFactory.ResourceFieldName.IsMatch ("_ab"));
			Assert.AreEqual (false, RegexFactory.ResourceFieldName.IsMatch ("1ab"));
			Assert.AreEqual (true, RegexFactory.ResourceFieldName.IsMatch ("a.b"));
			Assert.AreEqual (true, RegexFactory.ResourceFieldName.IsMatch ("a.b.c"));
			Assert.AreEqual (true, RegexFactory.ResourceFieldName.IsMatch ("a.b.00"));
			Assert.AreEqual (false, RegexFactory.ResourceFieldName.IsMatch ("a#b"));
			Assert.AreEqual (false, RegexFactory.ResourceFieldName.IsMatch ("a[0]"));
			Assert.AreEqual (false, RegexFactory.ResourceFieldName.IsMatch ("a.b."));
			Assert.AreEqual (false, RegexFactory.ResourceFieldName.IsMatch (".a.b"));
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
