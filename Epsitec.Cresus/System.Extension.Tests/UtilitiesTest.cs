using NUnit.Framework;

namespace System
{
	[TestFixture]
	public class UtilitiesTest
	{
		[Test] public void CheckCheckForDuplicates1()
		{
			string[] data = new string[] { "A", "B", "C", "C", "D" };
			
			Assertion.Assert (System.Utilities.CheckForDuplicates (data));
			Assertion.Assert (System.Utilities.CheckForDuplicates (data, false));
		}
		
		[Test] public void CheckCheckForDuplicates2()
		{
			string[] data = new string[] { "C", "B", "A", "C", "D" };
			
			Assertion.AssertEquals (true,  System.Utilities.CheckForDuplicates (data));
			Assertion.AssertEquals (false, System.Utilities.CheckForDuplicates (data, false));
		}
		
		[Test] public void CheckCheckForDuplicates3()
		{
			object[] data1 = new object[] { new SpecialData(1, "A"), new SpecialData (2, "B"), new SpecialData (3, "C") };
			object[] data2 = new object[] { new SpecialData(1, "C"), new SpecialData (2, "B"), new SpecialData (3, "C") };
			object[] data3 = new object[] { new SpecialData(3, "A"), new SpecialData (2, "B"), new SpecialData (3, "C") };
			
			Assertion.AssertEquals (false, System.Utilities.CheckForDuplicates (data1, SpecialData.NumComparer));
			Assertion.AssertEquals (false, System.Utilities.CheckForDuplicates (data1, SpecialData.TextComparer));
			
			Assertion.AssertEquals (false, System.Utilities.CheckForDuplicates (data2, SpecialData.NumComparer));
			Assertion.AssertEquals (true,  System.Utilities.CheckForDuplicates (data2, SpecialData.TextComparer));
			
			Assertion.AssertEquals (true,  System.Utilities.CheckForDuplicates (data3, SpecialData.NumComparer));
			Assertion.AssertEquals (false, System.Utilities.CheckForDuplicates (data3, SpecialData.TextComparer));
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
			
			Assertion.AssertEquals (3, args.Length);
			Assertion.AssertEquals ("a", args[0]);
			Assertion.AssertEquals ("b", args[1]);
			Assertion.AssertEquals ("c", args[2]);
			
			args = Utilities.Split ("'a;b';c", ';');
			
			Assertion.AssertEquals (2, args.Length);
			Assertion.AssertEquals ("'a;b'", args[0]);
			Assertion.AssertEquals ("c", args[1]);
			
			args = Utilities.Split ("'a;\"b';\"c\"", ';');
			
			Assertion.AssertEquals (2, args.Length);
			Assertion.AssertEquals ("'a;\"b'", args[0]);
			Assertion.AssertEquals ("\"c\"", args[1]);
			
			args = Utilities.Split ("a;<b;c>;d", ';');
			
			Assertion.AssertEquals (3, args.Length);
			Assertion.AssertEquals ("a", args[0]);
			Assertion.AssertEquals ("<b;c>", args[1]);
			Assertion.AssertEquals ("d", args[2]);
			
			args = Utilities.Split ("a;<x arg='1;2'/>;d", ';');
			
			Assertion.AssertEquals (3, args.Length);
			Assertion.AssertEquals ("a", args[0]);
			Assertion.AssertEquals ("<x arg='1;2'/>", args[1]);
			Assertion.AssertEquals ("d", args[2]);
		}
		
		[Test] public void CheckStringSimplify()
		{
			string s1 = @"'xyz'";
			string s2 = @"'ab''cd'";
			string s3 = @"'abcd'''";
			string s4 = @"""xyz'abc""";
			string s5 = @"<abc>";
			string s6 = @"abc";
			
			Assertion.AssertEquals (true,  Utilities.StringSimplify (ref s1));
			Assertion.AssertEquals (true,  Utilities.StringSimplify (ref s2));
			Assertion.AssertEquals (true,  Utilities.StringSimplify (ref s3));
			Assertion.AssertEquals (true,  Utilities.StringSimplify (ref s4));
			Assertion.AssertEquals (true,  Utilities.StringSimplify (ref s5, '<', '>'));
			Assertion.AssertEquals (false, Utilities.StringSimplify (ref s6));
			
			Assertion.AssertEquals ("xyz", s1);
			Assertion.AssertEquals ("ab'cd", s2);
			Assertion.AssertEquals ("abcd'", s3);
			Assertion.AssertEquals ("xyz'abc", s4);
			Assertion.AssertEquals ("abc", s5);
			Assertion.AssertEquals ("abc", s6);
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
			Assertion.AssertEquals ("&lt;#&apos;&quot;&gt;\u00A0\u2014x", Utilities.TextToXml ("<#'\">\u00A0\u2014x"));
		}
		
		[Test] public void CheckXmlToText()
		{
			Assertion.AssertEquals ("<#'\">\u00A0\u2014x", Utilities.XmlToText ("&lt;#&apos;&quot;&gt;&#160;&#8212;x"));
		}
		
		[Test] [ExpectedException (typeof (System.FormatException))] public void CheckXmlToTextEx1()
		{
			Utilities.XmlToText ("&nbsp;");
		}
	}
}
