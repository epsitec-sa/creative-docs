using NUnit.Framework;

namespace Epsitec.Common.Converters
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
		[Test] public void CheckToEnum()
		{
			System.Enum v1, v2, v3, v;
			
			Assertion.Assert (Converter.Convert (MyEnum.First, typeof (MyEnum), out v1));
			Assertion.Assert (Converter.Convert ("Second",     typeof (MyEnum), out v2));
			Assertion.Assert (Converter.Convert (99,           typeof (MyEnum), out v3));
			
			Assertion.AssertEquals (MyEnum.First,  v1);
			Assertion.AssertEquals (MyEnum.Second, v2);
			Assertion.AssertEquals (MyEnum.Extra,  v3);
			
			Assertion.Assert (Converter.Convert (0,   typeof (MyEnum), out v) == false);
			Assertion.Assert (Converter.Convert ("",  typeof (MyEnum), out v) == false);
			Assertion.Assert (Converter.Convert ("0", typeof (MyEnum), out v) == false);
		}
		
		private enum MyEnum
		{
			None	= -1,
			First	=  1,
			Second	=  2,
			Third	=  3,
			Extra	= 99
		}
	}
}
