using NUnit.Framework;

namespace Epsitec.Common.Converters
{
	[TestFixture] public class ConverterTest
	{
		[Test] public void CheckIsNull()
		{
			object a = null;
			object b = System.DBNull.Value;
			
			Assert.IsTrue (Converter.IsNull (a));
			Assert.IsTrue (Converter.IsNull (b));
			Assert.IsTrue (! Converter.IsNotNull (a));
			Assert.IsTrue (! Converter.IsNotNull (b));
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
			
			Assert.IsTrue (! Converter.Convert (a, out result));
			Assert.IsTrue (! Converter.Convert (b, out result));
			
			Assert.IsTrue (Converter.Convert (c, out result));
			Assert.AreEqual ("test", result);
			Assert.IsTrue (Converter.Convert (d, out result));
			Assert.AreEqual ("10", result);
			Assert.IsTrue (Converter.Convert (e, out result));
			Assert.AreEqual ("10", result);
			Assert.IsTrue (Converter.Convert (f, out result));
			Assert.AreEqual ("10.00", result);
			Assert.IsTrue (Converter.Convert (g, out result));
			Assert.AreEqual ("True", result);
			Assert.IsTrue (Converter.Convert (h, out result));
			Assert.AreEqual ("0.1;0.2;0.3;0.4", result);
			Assert.IsTrue (Converter.Convert (i, out result));
			Assert.AreEqual ("10;20;30;40", result);
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
			
			Assert.IsTrue (! Converter.Convert (a, out result));
			Assert.IsTrue (! Converter.Convert (b, out result));
			Assert.IsTrue (! Converter.Convert (c, out result));
			
			Assert.IsTrue (Converter.Convert (d, out result));
			Assert.IsTrue (result == 10);
			Assert.IsTrue (Converter.Convert (e, out result));
			Assert.IsTrue (result == 10);
			Assert.IsTrue (Converter.Convert (f, out result));
			Assert.IsTrue (result == 10);
			Assert.IsTrue (Converter.Convert (g, out result));
			Assert.IsTrue (result == 1);
			Assert.IsTrue (Converter.Convert (h, out result));
			Assert.IsTrue (result == 10);
			Assert.IsTrue (Converter.Convert (i, out result));
			Assert.IsTrue (result == 10);
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
			
			Assert.IsTrue (Converter.Convert (MyEnum.First, typeof (MyEnum), out v1));
			Assert.IsTrue (Converter.Convert ("Second",     typeof (MyEnum), out v2));
			Assert.IsTrue (Converter.Convert (99,           typeof (MyEnum), out v3));
			
			Assert.AreEqual (MyEnum.First,  v1);
			Assert.AreEqual (MyEnum.Second, v2);
			Assert.AreEqual (MyEnum.Extra,  v3);
			
			Assert.IsTrue (Converter.Convert (0,   typeof (MyEnum), out v) == false);
			Assert.IsTrue (Converter.Convert ("",  typeof (MyEnum), out v) == false);
			Assert.IsTrue (Converter.Convert ("0", typeof (MyEnum), out v) == false);
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
