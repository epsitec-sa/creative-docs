using NUnit.Framework;

namespace Epsitec.Cresus.Database
{
	[TestFixture]
	public class FbTypeConverterTest
	{
		[Test] public void CheckFindTypeConverter()
		{
			IDbAbstractionFactory dbf = DbFactoryTest.CreateDbAbstractionFactory ();
			ITypeConverter  converter = dbf.TypeConverter;
			
			Assert.IsNotNull (converter);
		}
		
		[Test] public void CheckNativeSupport()
		{
			IDbAbstractionFactory dbf = DbFactoryTest.CreateDbAbstractionFactory ();
			ITypeConverter  converter = dbf.TypeConverter;
			
			Assert.IsTrue (converter.CheckNativeSupport (DbRawType.Int16));
			Assert.IsTrue (converter.CheckNativeSupport (DbRawType.Int32));
			Assert.IsTrue (converter.CheckNativeSupport (DbRawType.Int64));
			Assert.IsTrue (converter.CheckNativeSupport (DbRawType.SmallDecimal));
			Assert.IsTrue (converter.CheckNativeSupport (DbRawType.LargeDecimal));
			Assert.IsTrue (converter.CheckNativeSupport (DbRawType.Date));
			Assert.IsTrue (converter.CheckNativeSupport (DbRawType.Time));
			Assert.IsTrue (converter.CheckNativeSupport (DbRawType.DateTime));
			Assert.IsTrue (converter.CheckNativeSupport (DbRawType.String));
			Assert.IsTrue (converter.CheckNativeSupport (DbRawType.ByteArray));
			
			Assert.IsTrue (converter.CheckNativeSupport (DbRawType.Guid) == false);
			Assert.IsTrue (converter.CheckNativeSupport (DbRawType.Boolean) == false);
		}
		
		[Test] public void CheckRawTypeConverter()
		{
			IDbAbstractionFactory dbf = DbFactoryTest.CreateDbAbstractionFactory ();
			ITypeConverter  converter = dbf.TypeConverter;
			IRawTypeConverter raw_conv;
			
			object oorg;
			object oint;
			object oext;
			
			Assert.IsTrue (converter.GetRawTypeConverter (DbRawType.Boolean, out raw_conv));
			oorg = true;
			oint = raw_conv.ConvertToInternalType (oorg);
			oext = raw_conv.ConvertFromInternalType (oint);
			Assert.AreEqual (oorg, oext);
			System.Console.Out.WriteLine ("{0} maps to {1}", oorg, oint);
			
			Assert.IsTrue (converter.GetRawTypeConverter (DbRawType.Guid, out raw_conv));
			oorg = System.Guid.NewGuid ();
			oint = raw_conv.ConvertToInternalType (oorg);
			oext = raw_conv.ConvertFromInternalType (oint);
			Assert.AreEqual (oorg, oext);
			System.Console.Out.WriteLine ("{0} maps to {1}", oorg, oint);
		}
	}
}
