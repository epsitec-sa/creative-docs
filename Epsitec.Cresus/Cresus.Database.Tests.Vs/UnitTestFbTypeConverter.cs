using Epsitec.Cresus.Database.Tests.Vs.Helpers;

using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace Epsitec.Cresus.Database.Tests.Vs
{


	[TestClass]
	public sealed class UnitTestFbTypeConverter
	{


		[ClassInitialize]
		public static void ClassInitialize(TestContext testContext)
		{
			TestHelper.Initialize ();
		}


		[TestMethod]
		public void FindTypeConverterTest()
		{
			IDbAbstractionFactory dbf = UnitTestFbTypeConverter.CreateDbAbstractionFactory ();
			ITypeConverter converter = dbf.TypeConverter;

			Assert.IsNotNull (converter);
		}


		[TestMethod]
		public void NativeSupportTest()
		{
			IDbAbstractionFactory dbf = UnitTestFbTypeConverter.CreateDbAbstractionFactory ();
			ITypeConverter converter = dbf.TypeConverter;

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

			Assert.IsFalse (converter.CheckNativeSupport (DbRawType.Guid));
			Assert.IsFalse (converter.CheckNativeSupport (DbRawType.Boolean));
		}


		[TestMethod]
		public void RawTypeConverterBoolTest()
		{
			IDbAbstractionFactory dbf = UnitTestFbTypeConverter.CreateDbAbstractionFactory ();
			ITypeConverter  converter = dbf.TypeConverter;
			IRawTypeConverter rawTypeConverter;

			Assert.IsTrue (converter.GetRawTypeConverter (DbRawType.Boolean, out rawTypeConverter));
			
			object objectOriginal = true;
			object objectInternal = rawTypeConverter.ConvertToInternalType (objectOriginal);
			object objectExternal = rawTypeConverter.ConvertFromInternalType (objectInternal);			
			
			Assert.AreEqual (objectOriginal, objectExternal);
		}


		[TestMethod]
		public void RawTypeConverterGuidTest()
		{
			IDbAbstractionFactory dbf = UnitTestFbTypeConverter.CreateDbAbstractionFactory ();
			ITypeConverter  converter = dbf.TypeConverter;
			IRawTypeConverter rawTypeConverter;

			Assert.IsTrue (converter.GetRawTypeConverter (DbRawType.Guid, out rawTypeConverter));
			
			object objectOriginal = System.Guid.NewGuid ();
			object objectInternal = rawTypeConverter.ConvertToInternalType (objectOriginal);
			object objectExternal = rawTypeConverter.ConvertFromInternalType (objectInternal);
			
			Assert.AreEqual (objectOriginal, objectExternal);
		}


		private static IDbAbstractionFactory CreateDbAbstractionFactory()
		{
			return DbFactory.FindDatabaseAbstractionFactory ("Firebird");
		}


	}


}
