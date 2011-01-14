//	Copyright © 2003-2008, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD


using Epsitec.Common.UnitTesting;

using Epsitec.Cresus.Database.UnitTests.Helpers;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.Collections.Generic;


namespace Epsitec.Cresus.Database.UnitTests
{


	[TestClass]
	public sealed class UnitTestTypeConverter
	{


		[ClassInitialize]
		public static void ClassInitialize(TestContext testContext)
		{
			TestHelper.Initialize ();

			DbInfrastructureHelper.ResetTestDatabase ();
		}


		[TestMethod]
		public void GetRawTypeTest()
		{
			DbRawType[] rawTypes = new DbRawType[]
			{
				DbRawType.Boolean,
				DbRawType.Int16,
				DbRawType.Int32, 
				DbRawType.Int64, 
				DbRawType.SmallDecimal, 
				DbRawType.LargeDecimal, 
				DbRawType.String, 
				DbRawType.Date, 
				DbRawType.Time,
				DbRawType.DateTime,
				DbRawType.ByteArray,
				DbRawType.Guid, 
				DbRawType.Null
			};
			
			foreach (DbRawType rawType in rawTypes)
			{
				DbNumDef numDef;
				DbSimpleType simpleType;
				
				simpleType = TypeConverter.GetSimpleType (rawType, out numDef);

				Assert.AreEqual (rawType, TypeConverter.GetRawType (simpleType, numDef));
			}
		}


		[TestMethod]
		public void IsCompatibleToSimpleTypeTest()
		{
			Dictionary<object, DbSimpleType> data = new Dictionary<object, DbSimpleType>
			{ 
				{ true, DbSimpleType.Decimal },
				{ (short) 10, DbSimpleType.Decimal },
				{ (int) 20, DbSimpleType.Decimal },
				{ (long) 30, DbSimpleType.Decimal },
				{ (decimal) 40, DbSimpleType.Decimal },
				{ Common.Types.Date.Today, DbSimpleType.Date },
				{ Common.Types.Time.Now, DbSimpleType.Time },
				{ System.DateTime.Now, DbSimpleType.DateTime },
				{ "x", DbSimpleType.String },
				{ new byte[] { (byte) 1 }, DbSimpleType.ByteArray },
			};

			foreach (var item in data)
			{
				Assert.IsTrue (TypeConverter.IsCompatibleToSimpleType (item.Key.GetType (), item.Value));
			}
		}


		[TestMethod]
		public void TextConversionsTest()
		{
			object value;
			object result;
			
			value = "Hello";
			result = TypeConverter.ConvertToSimpleType (value, DbRawType.String);

			Assert.AreEqual ("Hello", result);

			value = "Hello";
			result = TypeConverter.ConvertFromSimpleType (value, DbSimpleType.String, null);

			Assert.AreEqual ("Hello", result);
		}


		[TestMethod]
		public void NumericConversionsTest()
		{
			DbRawType rawType;
			DbNumDef numDef;
			object value;
			object result;
			
			value = (short) 10;
			rawType = DbRawType.Int16;
			numDef = DbNumDef.FromRawType (rawType);
			result = TypeConverter.ConvertToSimpleType (value, DbSimpleType.Decimal, numDef);
			Assert.AreEqual ((short) value, (decimal) result);


			value = (long) 10;
			rawType = DbRawType.Int16;
			numDef = DbNumDef.FromRawType (rawType);
			result = TypeConverter.ConvertToSimpleType (value, DbSimpleType.Decimal, numDef);
			Assert.AreEqual ((long) value, (decimal) result);

			value = (int) 100000;
			rawType = DbRawType.Int16;
			numDef = DbNumDef.FromRawType (rawType);

			ExceptionAssert.Throw<Exceptions.FormatException>
			(
				() => TypeConverter.ConvertToSimpleType (value, DbSimpleType.Decimal, numDef)
			);
		}


		[TestMethod]
		public void NumericConversionsWithNumDefConversionTest()
		{
			DbNumDef numDef;
			object value;
			object result;
			
			numDef = new DbNumDef (5, 3, 10.000M, 20.000M);
			value = numDef.ConvertToInt64 (12.345M);
			result = TypeConverter.ConvertToSimpleType (value, DbSimpleType.Decimal, numDef);
			Assert.AreEqual (12.345M, result);

			result = TypeConverter.ConvertFromSimpleType (result, DbSimpleType.Decimal, numDef);
			Assert.AreEqual ((short) result, (long) value);
			
			numDef = new DbNumDef (5, 3, 0.000M, 20.000M);
			value = new DbNumDef (6, 4, 0.000M, 20.000M).ConvertToInt64 (19.9999M);

			ExceptionAssert.Throw<Exceptions.FormatException>
			(
				() => TypeConverter.ConvertToSimpleType (value, DbSimpleType.Decimal, numDef)
			);
		}


		[TestMethod]
		public void MiscConversionsTest()
		{
			DbNumDef numDef;
			object value;
			object result;
			
			value = System.DateTime.Today;
			numDef = null;
			result = TypeConverter.ConvertToSimpleType (value, DbSimpleType.Date, numDef);
			result = TypeConverter.ConvertFromSimpleType (result, DbSimpleType.Date, numDef);
			
			Assert.AreEqual (Common.Types.Date.FromObject (value), Common.Types.Date.FromObject (result));
			
			value = System.DateTime.Now;
			numDef = null;
			result = TypeConverter.ConvertToSimpleType (value, DbSimpleType.Time, numDef);
			result = TypeConverter.ConvertFromSimpleType (result, DbSimpleType.Time, numDef);
			
			Assert.AreEqual (Common.Types.Time.FromObject (value), Common.Types.Time.FromObject (result));
			
			value = "Hello";
			numDef = null;
			result = TypeConverter.ConvertToSimpleType (value, DbSimpleType.String, numDef);
			result = TypeConverter.ConvertFromSimpleType (result, DbSimpleType.String, numDef);
			
			Assert.AreEqual (value, result);
			
			value = new byte[] { 0, 1, 2, 4 };
			numDef = null;
			result = TypeConverter.ConvertToSimpleType (value, DbSimpleType.ByteArray, numDef);
			result = TypeConverter.ConvertFromSimpleType (result, DbSimpleType.ByteArray, numDef);
			
			Assert.AreEqual (value, result);
			
			value = System.Guid.NewGuid ();
			numDef = null;
			result = TypeConverter.ConvertToSimpleType (value, DbSimpleType.Guid, numDef);
			result = TypeConverter.ConvertFromSimpleType (result, DbSimpleType.Guid, numDef);
			
			Assert.AreEqual (value, result);
		}

		
		[TestMethod]
		public void InternalConversionsTest()
		{
			using (DbInfrastructure dbInfrastructure = DbInfrastructureHelper.ConnectToTestDatabase ())
			{
				ITypeConverter converter = dbInfrastructure.Converter;

				object a = TypeConverter.ConvertToInternal (converter, "ABC", DbRawType.String);
				object b = TypeConverter.ConvertToInternal (converter, true, DbRawType.Boolean);

				Assert.AreEqual (typeof (string), a.GetType ());
				Assert.AreEqual (typeof (short), b.GetType ());

				object c = TypeConverter.ConvertFromInternal (converter, a, DbRawType.String);
				object d = TypeConverter.ConvertFromInternal (converter, b, DbRawType.Boolean);

				Assert.AreEqual ("ABC", c);
				Assert.AreEqual (true, d);
			}
		}


		
		[TestMethod]
		public void DbColumnConversionsTest()
		{
			using (DbInfrastructure dbInfrastructure = DbInfrastructureHelper.ConnectToTestDatabase ())
			{
				ITypeConverter converter = dbInfrastructure.Converter;

				DbColumn columnGuid = new DbColumn ("unique id", new DbTypeDef ("GUID", DbSimpleType.Guid, null, 0, true, DbNullability.Yes));
				DbColumn columnDate = new DbColumn ("date", new DbTypeDef (Epsitec.Common.Types.Res.Types.Default.Date));

				Assert.AreEqual (System.DBNull.Value, columnGuid.ConvertAdoToInternal (converter, System.DBNull.Value));
				Assert.AreEqual (System.DBNull.Value, columnGuid.ConvertInternalToAdo (converter, System.DBNull.Value));
				Assert.AreEqual (System.DBNull.Value, columnGuid.ConvertAdoToSimple (System.DBNull.Value));
				Assert.AreEqual (System.DBNull.Value, columnGuid.ConvertSimpleToAdo (System.DBNull.Value));

				Assert.AreEqual (System.DBNull.Value, columnDate.ConvertAdoToInternal (converter, System.DBNull.Value));
				Assert.AreEqual (System.DBNull.Value, columnDate.ConvertInternalToAdo (converter, System.DBNull.Value));
				Assert.AreEqual (System.DBNull.Value, columnDate.ConvertAdoToSimple (System.DBNull.Value));
				Assert.AreEqual (System.DBNull.Value, columnDate.ConvertSimpleToAdo (System.DBNull.Value));

				System.Guid guid = System.Guid.NewGuid ();
				Epsitec.Common.Types.Date date = new Epsitec.Common.Types.Date (2007, 12, 31);

				object valueGuid = columnGuid.ConvertAdoToInternal (converter, columnGuid.ConvertSimpleToAdo (guid));
				object valueDate = columnDate.ConvertAdoToInternal (converter, columnDate.ConvertSimpleToAdo (date));

				System.Console.Out.WriteLine ("Guid: Converted {0} to {1}, type={2}", guid, valueGuid, valueGuid.GetType ().FullName);
				System.Console.Out.WriteLine ("Date: Converted {0} to {1}, type={2}", date, valueDate, valueDate.GetType ().FullName);

				object resultGuid = columnGuid.ConvertAdoToSimple (columnGuid.ConvertInternalToAdo (converter, valueGuid));
				object resultDate = columnDate.ConvertAdoToSimple (columnDate.ConvertInternalToAdo (converter, valueDate));

				Assert.AreEqual (resultGuid, guid);
				Assert.AreEqual (resultDate, date);
			}
		}


		[TestMethod]
		public void GetSimpleTypeTest()
		{
			DbTypeDef type;

			type = new DbTypeDef (Res.Types.Num.KeyId);

			Assert.AreEqual (true, type.IsFixedLength);
			Assert.AreEqual (false, type.IsNullable);
			Assert.AreEqual (1, type.Length);
			Assert.AreEqual (DbKey.Empty, type.Key);
			Assert.AreEqual (Tags.TypeKeyId, type.Name);
			Assert.AreEqual (DbRawType.Int64, type.RawType);
			Assert.AreEqual (DbSimpleType.Decimal, type.SimpleType);
			Assert.AreEqual (Res.Types.Num.KeyId.CaptionId, type.TypeId);
			Assert.AreEqual (false, type.NumDef.IsConversionNeeded);

			type = new DbTypeDef (Res.Types.Num.NullableKeyId);

			Assert.AreEqual (true, type.IsFixedLength);
			Assert.AreEqual (true, type.IsNullable);
			Assert.AreEqual (1, type.Length);
			Assert.AreEqual (DbKey.Empty, type.Key);
			Assert.AreEqual (Tags.TypeNullableKeyId, type.Name);
			Assert.AreEqual (DbRawType.Int64, type.RawType);
			Assert.AreEqual (DbSimpleType.Decimal, type.SimpleType);
			Assert.AreEqual (Res.Types.Num.NullableKeyId.CaptionId, type.TypeId);
			Assert.AreEqual (false, type.NumDef.IsConversionNeeded);

			type = new DbTypeDef (Res.Types.Num.KeyStatus);

			Assert.AreEqual (true, type.IsFixedLength);
			Assert.AreEqual (false, type.IsNullable);
			Assert.AreEqual (1, type.Length);
			Assert.AreEqual (DbKey.Empty, type.Key);
			Assert.AreEqual (Tags.TypeKeyStatus, type.Name);
			Assert.AreEqual (DbRawType.Int16, type.RawType);
			Assert.AreEqual (DbSimpleType.Decimal, type.SimpleType);
			Assert.AreEqual (Res.Types.Num.KeyStatus.CaptionId, type.TypeId);
			Assert.AreEqual (false, type.NumDef.IsConversionNeeded);

			type = new DbTypeDef (Res.Types.Num.ReqExecState);

			Assert.AreEqual (true, type.IsFixedLength);
			Assert.AreEqual (false, type.IsNullable);
			Assert.AreEqual (1, type.Length);
			Assert.AreEqual (DbKey.Empty, type.Key);
			Assert.AreEqual (Tags.TypeReqExState, type.Name);
			Assert.AreEqual (DbRawType.Int16, type.RawType);
			Assert.AreEqual (DbSimpleType.Decimal, type.SimpleType);
			Assert.AreEqual (Res.Types.Num.ReqExecState.CaptionId, type.TypeId);
			Assert.AreEqual (false, type.NumDef.IsConversionNeeded);

			type = new DbTypeDef (Res.Types.Other.DateTime);

			Assert.AreEqual (true, type.IsFixedLength);
			Assert.AreEqual (false, type.IsNullable);
			Assert.AreEqual (1, type.Length);
			Assert.AreEqual (DbKey.Empty, type.Key);
			Assert.AreEqual (Tags.TypeDateTime, type.Name);
			Assert.AreEqual (DbRawType.DateTime, type.RawType);
			Assert.AreEqual (DbSimpleType.DateTime, type.SimpleType);
			Assert.AreEqual (Res.Types.Other.DateTime.CaptionId, type.TypeId);
			Assert.AreEqual (null, type.NumDef);

			type = new DbTypeDef (Res.Types.Other.ReqData);

			Assert.AreEqual (true, type.IsFixedLength);
			Assert.AreEqual (false, type.IsNullable);
			Assert.AreEqual (1, type.Length);
			Assert.AreEqual (DbKey.Empty, type.Key);
			Assert.AreEqual (Tags.TypeReqData, type.Name);
			Assert.AreEqual (DbRawType.ByteArray, type.RawType);
			Assert.AreEqual (DbSimpleType.ByteArray, type.SimpleType);
			Assert.AreEqual (Res.Types.Other.ReqData.CaptionId, type.TypeId);
			Assert.AreEqual (null, type.NumDef);
		}


	}


}
