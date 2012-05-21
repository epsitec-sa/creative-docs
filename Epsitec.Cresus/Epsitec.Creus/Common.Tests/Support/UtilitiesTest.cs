using NUnit.Framework;
using Epsitec.Common.Support;

namespace Epsitec.Common.Tests.Support
{
	[TestFixture]
	public class UtilitiesTest
	{
		[Test] public void CheckCheckForDuplicates1()
		{
			string[] data = new string[] { "A", "B", "C", "C", "D" };

			Assert.IsTrue (Utilities.CheckForDuplicates (data));
			Assert.IsTrue (Utilities.CheckForDuplicates (data, false));
		}
		
		[Test] public void CheckCheckForDuplicates2()
		{
			string[] data = new string[] { "C", "B", "A", "C", "D" };

			Assert.AreEqual (true, Utilities.CheckForDuplicates (data));
			Assert.AreEqual (false, Utilities.CheckForDuplicates (data, false));
		}
		
		[Test] public void CheckCheckForDuplicates3()
		{
			object[] data1 = new object[] { new SpecialData(1, "A"), new SpecialData (2, "B"), new SpecialData (3, "C") };
			object[] data2 = new object[] { new SpecialData(1, "C"), new SpecialData (2, "B"), new SpecialData (3, "C") };
			object[] data3 = new object[] { new SpecialData(3, "A"), new SpecialData (2, "B"), new SpecialData (3, "C") };

			Assert.AreEqual (false, Utilities.CheckForDuplicates (data1, SpecialData.NumComparer));
			Assert.AreEqual (false, Utilities.CheckForDuplicates (data1, SpecialData.TextComparer));

			Assert.AreEqual (false, Utilities.CheckForDuplicates (data2, SpecialData.NumComparer));
			Assert.AreEqual (true, Utilities.CheckForDuplicates (data2, SpecialData.TextComparer));

			Assert.AreEqual (true, Utilities.CheckForDuplicates (data3, SpecialData.NumComparer));
			Assert.AreEqual (false, Utilities.CheckForDuplicates (data3, SpecialData.TextComparer));
		}
		
		class SpecialData
		{
			public SpecialData(int num, string text)
			{
				this.num  = num;
				this.text = text;
			}
			
			
			private int		num;
			private string	text;
			
			public static System.Collections.IComparer NumComparer { get { return new SpecialCompare1 (); } }
			public static System.Collections.IComparer TextComparer { get { return new SpecialCompare2 (); } }
			
			class SpecialCompare1 : System.Collections.IComparer
			{
				#region IComparer Members

				public int Compare(object x, object y)
				{
					SpecialData ox = x as SpecialData;
					SpecialData oy = y as SpecialData;
					if (ox == oy) return 0;
					if (ox == null) return -1;
					if (oy == null) return 1;
					
					if (ox.num < oy.num) return -1;
					if (ox.num > oy.num) return 1;
					return 0;
				}

				#endregion
			}
			class SpecialCompare2 : System.Collections.IComparer
			{
				#region IComparer Members

				public int Compare(object x, object y)
				{
					SpecialData ox = x as SpecialData;
					SpecialData oy = y as SpecialData;
					if (ox == oy) return 0;
					if (ox == null) return -1;
					if (oy == null) return 1;
					return System.String.Compare (ox.text, oy.text);
				}

				#endregion
			}
		}
		
		
		[Test] public void CheckSplit()
		{
			string[] args;
			
			args = Utilities.Split ("a;b;c", ';');
			
			Assert.AreEqual (3, args.Length);
			Assert.AreEqual ("a", args[0]);
			Assert.AreEqual ("b", args[1]);
			Assert.AreEqual ("c", args[2]);
			
			args = Utilities.Split ("'a;b';c", ';');
			
			Assert.AreEqual (2, args.Length);
			Assert.AreEqual ("'a;b'", args[0]);
			Assert.AreEqual ("c", args[1]);
			
			args = Utilities.Split ("'a;\"b';\"c\"", ';');
			
			Assert.AreEqual (2, args.Length);
			Assert.AreEqual ("'a;\"b'", args[0]);
			Assert.AreEqual ("\"c\"", args[1]);
			
			args = Utilities.Split ("a;<b;c>;d", ';');
			
			Assert.AreEqual (3, args.Length);
			Assert.AreEqual ("a", args[0]);
			Assert.AreEqual ("<b;c>", args[1]);
			Assert.AreEqual ("d", args[2]);
			
			args = Utilities.Split ("a;<x arg='1;2'/>;d", ';');
			
			Assert.AreEqual (3, args.Length);
			Assert.AreEqual ("a", args[0]);
			Assert.AreEqual ("<x arg='1;2'/>", args[1]);
			Assert.AreEqual ("d", args[2]);

			args = Utilities.Split ("a;- ;-;<x arg='1;-;2'/>;-;-;d", ";-;");
			
			Assert.AreEqual (3, args.Length);
			Assert.AreEqual ("a;- ", args[0]);
			Assert.AreEqual ("<x arg='1;-;2'/>", args[1]);
			Assert.AreEqual ("-;d", args[2]);
		}
		
		[Test] public void CheckStringSimplify()
		{
			string s1 = @"'xyz'";
			string s2 = @"'ab''cd'";
			string s3 = @"'abcd'''";
			string s4 = @"""xyz'abc""";
			string s5 = @"<abc>";
			string s6 = @"abc";
			
			Assert.AreEqual (true,  Utilities.StringSimplify (ref s1));
			Assert.AreEqual (true,  Utilities.StringSimplify (ref s2));
			Assert.AreEqual (true,  Utilities.StringSimplify (ref s3));
			Assert.AreEqual (true,  Utilities.StringSimplify (ref s4));
			Assert.AreEqual (true,  Utilities.StringSimplify (ref s5, '<', '>'));
			Assert.AreEqual (false, Utilities.StringSimplify (ref s6));
			
			Assert.AreEqual ("xyz", s1);
			Assert.AreEqual ("ab'cd", s2);
			Assert.AreEqual ("abcd'", s3);
			Assert.AreEqual ("xyz'abc", s4);
			Assert.AreEqual ("abc", s5);
			Assert.AreEqual ("abc", s6);
		}
		
		[Test] [ExpectedException (typeof (System.Exception))] public void CheckStringSimplifyEx1()
		{
			string s1 = @"'xy'z'";
			Utilities.StringSimplify (ref s1);
		}
		
		[Test] [ExpectedException (typeof (System.Exception))] public void CheckStringSimplifyEx2()
		{
			string s1 = @"'xyz";
			Utilities.StringSimplify (ref s1);
		}
		
		[Test] [ExpectedException (typeof (System.Exception))] public void CheckStringSimplifyEx3()
		{
			string s1 = @"<xyz<";
			Utilities.StringSimplify (ref s1, '<', '>');
		}
		
		[Test] public void CheckTextToXml()
		{
			Assert.AreEqual ("&lt;#&apos;&quot;&gt;\u00A0\u2014x\n", Utilities.TextToXml ("<#'\">\u00A0\u2014x\n"));
		}
		
		[Test] public void CheckXmlToText()
		{
			Assert.AreEqual ("<#'\">\u00A0\u2014x\n", Utilities.XmlToText ("&lt;#&apos;&quot;&gt;&#160;&#8212;x\n"));
		}
		
		[Test] public void CheckTextToXmlBreak()
		{
			Assert.AreEqual ("&lt;#&apos;&quot;&gt;\u00A0\u2014x<br/>", Utilities.TextToXmlBreak ("<#'\">\u00A0\u2014x\n"));
		}
		
		[Test] public void CheckXmlBreakToText()
		{
			Assert.AreEqual ("<#'\">\u00A0\u2014x\n", Utilities.XmlBreakToText ("&lt;#&apos;&quot;&gt;&#160;&#8212;x<br/>"));
		}
		
		[Test] [ExpectedException (typeof (System.FormatException))] public void CheckXmlToTextEx1()
		{
			Utilities.XmlToText ("&nbsp;");
		}
	}
}
