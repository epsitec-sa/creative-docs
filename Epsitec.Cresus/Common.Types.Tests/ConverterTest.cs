using System;
using System.Globalization; 

using NUnit.Framework;

namespace Epsitec.Common.Types
{
	[TestFixture] public class ConverterTest
	{
		[Test] public void CheckIsNull()
		{
			object a = null;
			object b = System.DBNull.Value;
			
			Assertion.Assert (Converter.IsNull (a));
			Assertion.Assert (Converter.IsNull (b));
			Assertion.Assert (! Converter.IsNotNull (a));
			Assertion.Assert (! Converter.IsNotNull (b));
		}
		
		[Test] public void CheckToDateTime()
		{
			object a = null;
			object b = System.DBNull.Value;
			object c = "";
			object d = 632350746051230000;
			object e = new System.DateTime (2004, 11, 3, 10, 30, 5, 123);
			object f = "2004-11-03T10:30:05+1230000";
			object g = "2004-11-03 10:30:05Z";
			object h = "2004-11-03 10:30:05";
			
			System.DateTime result;
			
			Assertion.Assert (! Converter.Convert (a, out result));
			Assertion.Assert (! Converter.Convert (b, out result));
			Assertion.Assert (! Converter.Convert (c, out result));
			
			Assertion.Assert (Converter.Convert (d, out result));
			Assertion.AssertEquals (new System.DateTime (2004, 11, 3, 10, 30, 5, 123), result);
			Assertion.Assert (Converter.Convert (e, out result));
			Assertion.AssertEquals (new System.DateTime (2004, 11, 3, 10, 30, 5, 123), result);
			Assertion.Assert (Converter.Convert (f, out result));
			Assertion.AssertEquals (new System.DateTime (2004, 11, 3, 10, 30, 5, 123), result);
			Assertion.Assert (Converter.Convert (g, out result));
			Assertion.AssertEquals (new System.DateTime (2004, 11, 3, 10, 30, 5, 0), result);
			Assertion.Assert (Converter.Convert (h, out result));
			Assertion.AssertEquals (new System.DateTime (2004, 11, 3, 10, 30, 5, 0), result);
			
			System.DateTime tloc = System.DateTime.Now;
			System.DateTime tutc = System.DateTime.UtcNow;
			
			System.Console.WriteLine ("Now [u] : {0}, UTC {1}", tloc.ToString ("u"), tutc.ToString ("u"));
			System.Console.WriteLine ("Now [U] : {0}, UTC {1}", tloc.ToString ("U"), tutc.ToString ("U"));
			System.Console.WriteLine ("Now [r] : {0}, UTC {1}", tloc.ToString ("r"), tutc.ToString ("r"));
			System.Console.WriteLine ("Now [s] : {0}, UTC {1}", tloc.ToString ("s"), tutc.ToString ("s"));
		}
		
		[Test] public void CheckToDecimal()
		{
			object a = null;
			object b = System.DBNull.Value;
			object c = "";
			object d = 10;
			object e = 10.0;
			object f = 10.00M;
			object g = true;
			object h = "10";
			object i = "10.00";
			
			decimal result;
			
			Assertion.Assert (! Converter.Convert (a, out result));
			Assertion.Assert (! Converter.Convert (b, out result));
			Assertion.Assert (! Converter.Convert (c, out result));
			
			Assertion.Assert (Converter.Convert (d, out result));
			Assertion.Assert (result == 10);
			Assertion.Assert (Converter.Convert (e, out result));
			Assertion.Assert (result == 10);
			Assertion.Assert (Converter.Convert (f, out result));
			Assertion.Assert (result == 10);
			Assertion.Assert (Converter.Convert (g, out result));
			Assertion.Assert (result == 1);
			Assertion.Assert (Converter.Convert (h, out result));
			Assertion.Assert (result == 10);
			Assertion.Assert (Converter.Convert (i, out result));
			Assertion.Assert (result == 10);
		}
		
		[Test] [ExpectedException (typeof (System.FormatException))] public void CheckToDecimalEx1()
		{
			decimal result;
			Converter.Convert ("X", out result);
		}
		
		[Test] [ExpectedException (typeof (System.NotSupportedException))] public void CheckToDecimalEx2()
		{
			decimal result;
			Converter.Convert (new Drawing.Rectangle (10, 20, 30, 40), out result);
		}
		
		[Test] public void CheckToEnum1()
		{
			System.Enum v1, v2, v3, v;
			
			Assertion.Assert (Converter.Convert (MyEnum.First, typeof (MyEnum), out v1));
			Assertion.Assert (Converter.Convert ("Second",     typeof (MyEnum), out v2));
			Assertion.Assert (Converter.Convert (99,           typeof (MyEnum), out v3));
			
			Assertion.AssertEquals (MyEnum.First,  v1);
			Assertion.AssertEquals (MyEnum.Second, v2);
			Assertion.AssertEquals (MyEnum.Extra,  v3);
			
			Assertion.Assert (! Converter.Convert (0,   typeof (MyEnum), out v));
			Assertion.Assert (! Converter.Convert ("",  typeof (MyEnum), out v));
			Assertion.Assert (! Converter.Convert ("X", typeof (MyEnum), out v));
			Assertion.Assert (! Converter.Convert ("0", typeof (MyEnum), out v));
		}
		
		[Test] public void CheckToEnum2()
		{
			System.Enum v1, v2, v3, v;
			
			Assertion.Assert (Converter.Convert (MyFlags.Flag1,  typeof (MyFlags), out v1));
			Assertion.Assert (Converter.Convert ("Flag2, Flag4", typeof (MyFlags), out v2));
			Assertion.Assert (Converter.Convert (9,              typeof (MyFlags), out v3));
			
			Assertion.AssertEquals (MyFlags.Flag1,  v1);
			Assertion.AssertEquals (MyFlags.Flag2 | MyFlags.Flag4, v2);
			Assertion.AssertEquals (MyFlags.Flag1 | MyFlags.Flag4, v3);
			
			Assertion.Assert (! Converter.Convert (-1,   typeof (MyFlags), out v));
			Assertion.Assert (  Converter.Convert (0x0f, typeof (MyFlags), out v));
			Assertion.Assert (! Converter.Convert (0x1f, typeof (MyFlags), out v));
			Assertion.Assert (! Converter.Convert ("",   typeof (MyFlags), out v));
			Assertion.Assert (! Converter.Convert ("X",  typeof (MyFlags), out v));
			Assertion.Assert (! Converter.Convert ("-1", typeof (MyFlags), out v));
		}
		
		[Test] public void CheckToString()
		{
			object a = null;
			object b = System.DBNull.Value;
			object c = "test";
			object d = 10;
			object e = 10.0;
			object f = 10.00M;
			object g = true;
			object h = new Drawing.Color (0.1, 0.2, 0.3, 0.4);
			object i = new Drawing.Rectangle (10, 20, 30, 40);
			object j = new System.DateTime (2004, 11, 3, 10, 30, 5, 123);
			
			string result;
			
			Assertion.Assert (! Converter.Convert (a, out result));
			Assertion.Assert (! Converter.Convert (b, out result));
			
			Assertion.Assert (Converter.Convert (c, out result));
			Assertion.AssertEquals ("test", result);
			Assertion.Assert (Converter.Convert (d, out result));
			Assertion.AssertEquals ("10", result);
			Assertion.Assert (Converter.Convert (e, out result));
			Assertion.AssertEquals ("10", result);
			Assertion.Assert (Converter.Convert (f, out result));
			Assertion.AssertEquals ("10.00", result);
			Assertion.Assert (Converter.Convert (g, out result));
			Assertion.AssertEquals ("True", result);
			Assertion.Assert (Converter.Convert (h, out result));
			Assertion.AssertEquals ("0.1;0.2;0.3;0.4", result);
			Assertion.Assert (Converter.Convert (i, out result));
			Assertion.AssertEquals ("10;20;30;40", result);
			Assertion.Assert (Converter.Convert (i, out result));
			Assertion.AssertEquals ("10;20;30;40", result);
			Assertion.Assert (Converter.Convert (j, out result));
			Assertion.AssertEquals ("2004-11-03T10:30:05+1230000", result);
		}
		
		
		[Test] public void MicrosoftBug()
		{
			//	Only 'r' and 'u' do not provide the proper output, when compared to
			//	the documentation...
			
			DateTime dt = DateTime.Now;
			String[] format = {
								  "d", "D",
								  "f", "F",
								  "g", "G",
								  "m",
								  "r",
								  "s",
								  "t", "T",
								  "u", "U",
								  "y",
								  "dddd, MMMM dd yyyy",
								  "ddd, MMM d \"'\"yy",
								  "dddd, MMMM dd",
								  "M/yy",
								  "dd-MM-yy",
			};
			String date;
			for (int i = 0; i < format.Length; i++) 
			{
				date = dt.ToString(format[i], DateTimeFormatInfo.InvariantInfo);
				Console.WriteLine(String.Concat(format[i], " :" , date));
			}
		}
		
		private enum MyEnum
		{
			None	= -1,
			First	=  1,
			Second	=  2,
			Third	=  3,
			Extra	= 99
		}
		
		[System.Flags]
		private enum MyFlags
		{
			None	= 0,
			Flag1	= 1,
			Flag2	= 2,
			Flag3	= 4,
			Flag4	= 8
		}
	}
}
