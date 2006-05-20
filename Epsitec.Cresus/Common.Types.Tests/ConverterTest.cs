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
			
			Assert.IsTrue (Converter.IsNull (a));
			Assert.IsTrue (Converter.IsNull (b));
			Assert.IsTrue (! Converter.IsNotNull (a));
			Assert.IsTrue (! Converter.IsNotNull (b));
		}
		
		[Test] public void CheckToDateTime()
		{
			object a = null;
			object b = System.DBNull.Value;
			object c = "";
			object d = 632350746051230000;
			object e = new System.DateTime (2004, 11, 3, 10, 30, 5, 123);
			object f = "2004-11-03T10:30:05.1230000";
			object g = "2004-11-03 10:30:05Z";
			object h = "2004-11-03 10:30:05";
			
			System.DateTime result;
			
			Assert.IsTrue (! Converter.Convert (a, out result));
			Assert.IsTrue (! Converter.Convert (b, out result));
			Assert.IsTrue (! Converter.Convert (c, out result));
			
			Assert.IsTrue (Converter.Convert (d, out result));
			Assert.AreEqual (new System.DateTime (2004, 11, 3, 10, 30, 5, 123), result);
			Assert.IsTrue (Converter.Convert (e, out result));
			Assert.AreEqual (new System.DateTime (2004, 11, 3, 10, 30, 5, 123), result);
			Assert.IsTrue (Converter.Convert (f, out result));
			Assert.AreEqual (new System.DateTime (2004, 11, 3, 10, 30, 5, 123), result);
			Assert.IsTrue (Converter.Convert (g, out result));
			Assert.AreEqual (new System.DateTime (2004, 11, 3, 10, 30, 5, 0), result);
			Assert.IsTrue (Converter.Convert (h, out result));
			Assert.AreEqual (new System.DateTime (2004, 11, 3, 10, 30, 5, 0), result);
			
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
		
		[Test] public void CheckToEnum1()
		{
			System.Enum v1, v2, v3, v;
			
			Assert.IsTrue (Converter.Convert (MyEnum.First, typeof (MyEnum), out v1));
			Assert.IsTrue (Converter.Convert ("Second",     typeof (MyEnum), out v2));
			Assert.IsTrue (Converter.Convert (99,           typeof (MyEnum), out v3));
			
			Assert.AreEqual (MyEnum.First,  v1);
			Assert.AreEqual (MyEnum.Second, v2);
			Assert.AreEqual (MyEnum.Extra,  v3);
			
			Assert.IsTrue (! Converter.Convert (0,   typeof (MyEnum), out v));
			Assert.IsTrue (! Converter.Convert ("",  typeof (MyEnum), out v));
			Assert.IsTrue (! Converter.Convert ("X", typeof (MyEnum), out v));
			Assert.IsTrue (! Converter.Convert ("0", typeof (MyEnum), out v));
		}
		
		[Test] public void CheckToEnum2()
		{
			System.Enum v1, v2, v3, v;
			
			Assert.IsTrue (Converter.Convert (MyFlags.Flag1,  typeof (MyFlags), out v1));
			Assert.IsTrue (Converter.Convert ("Flag2, Flag4", typeof (MyFlags), out v2));
			Assert.IsTrue (Converter.Convert (9,              typeof (MyFlags), out v3));
			
			Assert.AreEqual (MyFlags.Flag1,  v1);
			Assert.AreEqual (MyFlags.Flag2 | MyFlags.Flag4, v2);
			Assert.AreEqual (MyFlags.Flag1 | MyFlags.Flag4, v3);
			
			Assert.IsTrue (! Converter.Convert (-1,   typeof (MyFlags), out v));
			Assert.IsTrue (  Converter.Convert (0x0f, typeof (MyFlags), out v));
			Assert.IsTrue (! Converter.Convert (0x1f, typeof (MyFlags), out v));
			Assert.IsTrue (! Converter.Convert ("",   typeof (MyFlags), out v));
			Assert.IsTrue (! Converter.Convert ("X",  typeof (MyFlags), out v));
			Assert.IsTrue (! Converter.Convert ("-1", typeof (MyFlags), out v));
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
			Assert.AreEqual ("#199A;#3;#4CCD;#6", result);
			Assert.IsTrue (Converter.Convert (i, out result));
			Assert.AreEqual ("10;20;30;40", result);
			Assert.IsTrue (Converter.Convert (i, out result));
			Assert.AreEqual ("10;20;30;40", result);
			Assert.IsTrue (Converter.Convert (j, out result));
			Assert.AreEqual ("2004-11-03T10:30:05.1230000Z", result);
		}

		[Test]
		public void CheckTypeConverter()
		{
			ISerializationConverter convC = Converter.GetSerializationConverter (typeof (string));
			ISerializationConverter convD = Converter.GetSerializationConverter (typeof (int));
			ISerializationConverter convE = Converter.GetSerializationConverter (typeof (double));
			ISerializationConverter convF = Converter.GetSerializationConverter (typeof (decimal));
			ISerializationConverter convG = Converter.GetSerializationConverter (typeof (bool));
			ISerializationConverter convH = Converter.GetSerializationConverter (typeof (Drawing.Color));
			ISerializationConverter convI = Converter.GetSerializationConverter (typeof (Drawing.Rectangle));
			ISerializationConverter convJ = Converter.GetSerializationConverter (typeof (System.DateTime));

			object c = "test";
			object d = 10;
			object e = 10.0;
			object f = 10.00M;
			object g = true;
			object h = new Drawing.Color (0.1, 0.2, 0.3, 0.4);
			object i = new Drawing.Rectangle (10, 20, 30, 40);
			object j = new System.DateTime (2004, 11, 3, 10, 30, 5, 123);
			
			Assert.AreEqual ("test",	convC.ConvertToString (c, null));
			Assert.AreEqual ("10",		convD.ConvertToString (d, null));
			Assert.AreEqual ("10",		convE.ConvertToString (e, null));
			Assert.AreEqual ("10.00",	convF.ConvertToString (f, null));
			Assert.AreEqual ("True",	convG.ConvertToString (g, null));
			
			Assert.AreEqual ("#199A;#3;#4CCD;#6",	convH.ConvertToString (h, null));
			Assert.AreEqual ("10;20;30;40",			convI.ConvertToString (i, null));
			Assert.AreEqual ("11/03/2004 10:30:05", convJ.ConvertToString (j, null));
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
