using NUnit.Framework;

namespace Epsitec.Cresus.Database
{
	[TestFixture]
	public class FbTypeConverterTest
	{
		[SetUp] public void LoadAssemblies()
		{
			DbFactory.Initialise ();
		}
		
		[Test] public void CheckFindTypeConverter()
		{
			IDbAbstractionFactory dbf = this.CreateDbAbstractionFactory ();
			ITypeConverter  converter = dbf.TypeConverter;
			
			Assertion.AssertNotNull (converter);
		}
		
		[Test] public void CheckNativeSupport()
		{
			IDbAbstractionFactory dbf = this.CreateDbAbstractionFactory ();
			ITypeConverter  converter = dbf.TypeConverter;
			
			Assertion.Assert (converter.CheckNativeSupport (DbRawType.Int16));
			Assertion.Assert (converter.CheckNativeSupport (DbRawType.Int32));
			Assertion.Assert (converter.CheckNativeSupport (DbRawType.Int64));
			Assertion.Assert (converter.CheckNativeSupport (DbRawType.SmallDecimal));
			Assertion.Assert (converter.CheckNativeSupport (DbRawType.LargeDecimal));
			Assertion.Assert (converter.CheckNativeSupport (DbRawType.Date));
			Assertion.Assert (converter.CheckNativeSupport (DbRawType.Time));
			Assertion.Assert (converter.CheckNativeSupport (DbRawType.DateTime));
			Assertion.Assert (converter.CheckNativeSupport (DbRawType.String));
			Assertion.Assert (converter.CheckNativeSupport (DbRawType.ByteArray));
			
			Assertion.Assert (converter.CheckNativeSupport (DbRawType.Guid) == false);
			Assertion.Assert (converter.CheckNativeSupport (DbRawType.Boolean) == false);
		}
		
		[Test] public void CheckRawTypeConverter()
		{
			IDbAbstractionFactory dbf = this.CreateDbAbstractionFactory ();
			ITypeConverter  converter = dbf.TypeConverter;
			IRawTypeConverter raw_conv;
			
			object oorg;
			object oint;
			object oext;
			
			Assertion.Assert (converter.GetRawTypeConverter (DbRawType.Boolean, out raw_conv));
			oorg = true;
			oint = raw_conv.ConvertToInternalType (oorg);
			oext = raw_conv.ConvertFromInternalType (oint);
			Assertion.AssertEquals (oorg, oext);
			System.Console.Out.WriteLine ("{0} maps to {1}", oorg, oint);
			
			Assertion.Assert (converter.GetRawTypeConverter (DbRawType.Guid, out raw_conv));
			oorg = System.Guid.NewGuid ();
			oint = raw_conv.ConvertToInternalType (oorg);
			oext = raw_conv.ConvertFromInternalType (oint);
			Assertion.AssertEquals (oorg, oext);
			System.Console.Out.WriteLine ("{0} maps to {1}", oorg, oint);
		}
		
		
		protected IDbAbstractionFactory CreateDbAbstractionFactory()
		{
			return DbFactory.FindDbAbstractionFactory ("Firebird");
		}
	}
}
