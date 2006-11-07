using NUnit.Framework;

namespace Epsitec.Cresus.Database
{
	[TestFixture]
	public class TypeConverterTest
	{
		[Test] public void CheckMapping()
		{
			DbRawType[] raw_array = new DbRawType[] { DbRawType.Boolean, DbRawType.Int16, DbRawType.Int32, DbRawType.Int64, DbRawType.SmallDecimal, DbRawType.LargeDecimal, DbRawType.String, DbRawType.Date, DbRawType.Time, DbRawType.DateTime, DbRawType.ByteArray, DbRawType.Guid, DbRawType.Null };
			
			foreach (DbRawType raw in raw_array)
			{
				DbNumDef num_def;
				DbSimpleType simple;
				
				simple = TypeConverter.GetSimpleType (raw, out num_def);
				
				Assert.AreEqual (raw, TypeConverter.GetRawType (simple, num_def), string.Format ("Cannot match {0}", raw.ToString ()));
			}
		}
		
		[Test] public void CheckIsCompatibleToSimpleType()
		{
			object[] objects = new object[] { true, (short)(10), (int)(20), (long)(30), (decimal)(40), Common.Types.Date.Today, Common.Types.Time.Now, System.DateTime.Now, "x", new byte[] { (byte)1 } };
			DbSimpleType[] types = new DbSimpleType[] { DbSimpleType.Decimal, DbSimpleType.Decimal, DbSimpleType.Decimal, DbSimpleType.Decimal, DbSimpleType.Decimal, DbSimpleType.Date, DbSimpleType.Time, DbSimpleType.DateTime, DbSimpleType.String, DbSimpleType.ByteArray };
			
			for (int i = 0; i < objects.Length; i++)
			{
				Assert.IsTrue (TypeConverter.IsCompatibleToSimpleType (objects[i].GetType (), types[i]));
			}
		}
		
		[Test] public void CheckNumericConversions()
		{
			DbRawType raw;
			DbNumDef def;
			object val;
			object obj;
			
			val = (short) 10;
			raw = DbRawType.Int16;
			def = DbNumDef.FromRawType (raw);
			obj = TypeConverter.ConvertToSimpleType (val, DbSimpleType.Decimal, def);
			
			val = (long) 10;
			raw = DbRawType.Int16;
			def = DbNumDef.FromRawType (raw);
			obj = TypeConverter.ConvertToSimpleType (val, DbSimpleType.Decimal, def);
			
			bool exception_thrown = false;
			
			try
			{
				val = (int) 100000;
				raw = DbRawType.Int16;
				def = DbNumDef.FromRawType (raw);
				obj = TypeConverter.ConvertToSimpleType (val, DbSimpleType.Decimal, def);
			}
			catch (Exceptions.FormatException ex)
			{
				if (ex.Message.StartsWith ("Incompatible"))
				{
					exception_thrown = true;
				}
			}
			
			Assert.IsTrue (exception_thrown, "ConverToSimpleType: 100000 to Int16 should fail");
		}
		
		[Test] public void CheckNumericConversionsWithNumDefConversion()
		{
			DbNumDef def;
			object val;
			object obj;
			object res;
			
			def = new DbNumDef (5, 3, 10.000M, 20.000M);
			val = def.ConvertToInt64 (12.345M);
			obj = TypeConverter.ConvertToSimpleType (val, DbSimpleType.Decimal, def);
			
			Assert.AreEqual (12.345M, obj);
			
			res = TypeConverter.ConvertFromSimpleType (obj, DbSimpleType.Decimal, def);
			
			Assert.AreEqual (res, val);
			
			bool exception_thrown = false;
			
			try
			{
				def = new DbNumDef (5, 3, 0.000M, 20.000M);
				val = new DbNumDef (6, 4, 0.000M, 20.000M).ConvertToInt64 (19.9999M);
				obj = TypeConverter.ConvertToSimpleType (val, DbSimpleType.Decimal, def);
			}
			catch (Exceptions.FormatException ex)
			{
				if (ex.Message.StartsWith ("Incompatible"))
				{
					exception_thrown = true;
				}
			}
			
			Assert.IsTrue (exception_thrown, "ConverToSimpleType: incompatible values should fail");
		}
		
		[Test] public void CheckMiscConversions()
		{
			DbNumDef def;
			object val;
			object obj;
			object res;
			
			val = System.DateTime.Today;
			def = null;
			obj = TypeConverter.ConvertToSimpleType (val, DbSimpleType.Date, def);
			res = TypeConverter.ConvertFromSimpleType (obj, DbSimpleType.Date, def);
			
			System.Console.Out.WriteLine ("Today is {0}", obj);
			
			Assert.AreEqual (new Common.Types.Date (val), new Common.Types.Date (res));
			
			val = System.DateTime.Now;
			def = null;
			obj = TypeConverter.ConvertToSimpleType (val, DbSimpleType.Time, def);
			res = TypeConverter.ConvertFromSimpleType (obj, DbSimpleType.Time, def);
			
			System.Console.Out.WriteLine ("Now is {0}", obj);
			
			Assert.AreEqual (new Common.Types.Time (val), new Common.Types.Time (res));
			
			val = "Hello";
			def = null;
			obj = TypeConverter.ConvertToSimpleType (val, DbSimpleType.String, def);
			res = TypeConverter.ConvertFromSimpleType (obj, DbSimpleType.String, def);
			
			Assert.AreEqual (val, res);
			
			val = new byte[] { 0, 1, 2, 4 };
			def = null;
			obj = TypeConverter.ConvertToSimpleType (val, DbSimpleType.ByteArray, def);
			res = TypeConverter.ConvertFromSimpleType (obj, DbSimpleType.ByteArray, def);
			
			Assert.AreEqual (val, res);
			
			val = System.Guid.NewGuid ();
			def = null;
			obj = TypeConverter.ConvertToSimpleType (val, DbSimpleType.Guid, def);
			res = TypeConverter.ConvertFromSimpleType (obj, DbSimpleType.Guid, def);
			
			Assert.AreEqual (val, res);
		}
		
		[Test] public void CheckInternalConversions()
		{
			DbInfrastructure infrastructure = DbInfrastructureTest.GetInfrastructureFromBase ("fiche", false);
			ITypeConverter   converter      = infrastructure.TypeConverter;
			
			object a = TypeConverter.ConvertToInternal (converter, "ABC", DbRawType.String);
			object b = TypeConverter.ConvertToInternal (converter, true, DbRawType.Boolean);
			
			Assert.AreEqual (typeof (string), a.GetType ());
			Assert.AreEqual (typeof (short), b.GetType ());
			
			object c = TypeConverter.ConvertFromInternal (converter, a, DbRawType.String);
			object d = TypeConverter.ConvertFromInternal (converter, b, DbRawType.Boolean);
			
			Assert.AreEqual ("ABC", c);
			Assert.AreEqual (true, d);
		}

		[Test]
		public void CheckGetSimpleType()
		{
			DbTypeDef type;

			type = new DbTypeDef (Res.Types.Num.KeyId);

			Assert.AreEqual (true, type.IsFixedLength);
			Assert.AreEqual (false, type.IsMultilingual);
			Assert.AreEqual (false, type.IsNullable);
			Assert.AreEqual (1, type.Length);
			Assert.AreEqual (DbKey.Empty, type.Key);
			Assert.AreEqual ("Num.KeyId", type.Name);
			Assert.AreEqual (DbRawType.Int64, type.RawType);
			Assert.AreEqual (DbSimpleType.Decimal, type.SimpleType);
			Assert.AreEqual (Res.Types.Num.KeyId.CaptionId, type.TypeId);
			Assert.AreEqual (false, type.NumDef.IsConversionNeeded);

			type = new DbTypeDef (Res.Types.Num.NullableKeyId);

			Assert.AreEqual (true, type.IsFixedLength);
			Assert.AreEqual (false, type.IsMultilingual);
			Assert.AreEqual (true, type.IsNullable);
			Assert.AreEqual (1, type.Length);
			Assert.AreEqual (DbKey.Empty, type.Key);
			Assert.AreEqual ("Num.NullableKeyId", type.Name);
			Assert.AreEqual (DbRawType.Int64, type.RawType);
			Assert.AreEqual (DbSimpleType.Decimal, type.SimpleType);
			Assert.AreEqual (Res.Types.Num.NullableKeyId.CaptionId, type.TypeId);
			Assert.AreEqual (false, type.NumDef.IsConversionNeeded);

			type = new DbTypeDef (Res.Types.Num.KeyStatus);

			Assert.AreEqual (true, type.IsFixedLength);
			Assert.AreEqual (false, type.IsMultilingual);
			Assert.AreEqual (false, type.IsNullable);
			Assert.AreEqual (1, type.Length);
			Assert.AreEqual (DbKey.Empty, type.Key);
			Assert.AreEqual ("Num.KeyStatus", type.Name);
			Assert.AreEqual (DbRawType.Int16, type.RawType);
			Assert.AreEqual (DbSimpleType.Decimal, type.SimpleType);
			Assert.AreEqual (Res.Types.Num.KeyStatus.CaptionId, type.TypeId);
			Assert.AreEqual (false, type.NumDef.IsConversionNeeded);

			type = new DbTypeDef (Res.Types.Num.ReqExecState);

			Assert.AreEqual (true, type.IsFixedLength);
			Assert.AreEqual (false, type.IsMultilingual);
			Assert.AreEqual (false, type.IsNullable);
			Assert.AreEqual (1, type.Length);
			Assert.AreEqual (DbKey.Empty, type.Key);
			Assert.AreEqual ("Num.ReqExecState", type.Name);
			Assert.AreEqual (DbRawType.Int16, type.RawType);
			Assert.AreEqual (DbSimpleType.Decimal, type.SimpleType);
			Assert.AreEqual (Res.Types.Num.ReqExecState.CaptionId, type.TypeId);
			Assert.AreEqual (false, type.NumDef.IsConversionNeeded);

			type = new DbTypeDef (Res.Types.Other.DateTime);

			Assert.AreEqual (true, type.IsFixedLength);
			Assert.AreEqual (false, type.IsMultilingual);
			Assert.AreEqual (false, type.IsNullable);
			Assert.AreEqual (1, type.Length);
			Assert.AreEqual (DbKey.Empty, type.Key);
			Assert.AreEqual ("Other.DateTime", type.Name);
			Assert.AreEqual (DbRawType.DateTime, type.RawType);
			Assert.AreEqual (DbSimpleType.DateTime, type.SimpleType);
			Assert.AreEqual (Res.Types.Other.DateTime.CaptionId, type.TypeId);
			Assert.AreEqual (null, type.NumDef);

			type = new DbTypeDef (Res.Types.Other.ReqData);

			Assert.AreEqual (true, type.IsFixedLength);
			Assert.AreEqual (false, type.IsMultilingual);
			Assert.AreEqual (false, type.IsNullable);
			Assert.AreEqual (1, type.Length);
			Assert.AreEqual (DbKey.Empty, type.Key);
			Assert.AreEqual ("Other.ReqData", type.Name);
			Assert.AreEqual (DbRawType.ByteArray, type.RawType);
			Assert.AreEqual (DbSimpleType.ByteArray, type.SimpleType);
			Assert.AreEqual (Res.Types.Other.ReqData.CaptionId, type.TypeId);
			Assert.AreEqual (null, type.NumDef);
		}
	}
}
