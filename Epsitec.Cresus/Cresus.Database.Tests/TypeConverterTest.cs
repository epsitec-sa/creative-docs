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
				
				simple = TypeConverter.MapToSimpleType (raw, out num_def);
				
				Assert.AreEqual (raw, TypeConverter.MapToRawType (simple, num_def), string.Format ("Cannot match {0}", raw.ToString ()));
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
			catch (DbFormatException ex)
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
			catch (DbFormatException ex)
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
	}
}
