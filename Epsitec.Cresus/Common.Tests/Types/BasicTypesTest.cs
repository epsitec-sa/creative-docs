//	Copyright © 2003-2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using NUnit.Framework;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization.Formatters.Soap;
using Epsitec.Common.Types;
using Epsitec.Common.Support;

namespace Epsitec.Common.Tests.Types
{
	[TestFixture] public class BasicTypesTest
	{
		[Test]
		public void CheckBinaryType()
		{
			BinaryType type = new BinaryType ();

			type.DefineMimeType ("image/jpeg;image/tiff");

			System.Console.Out.WriteLine (type.Caption.SerializeToString ());
		}
		
		[Test]
		[ExpectedException (typeof (System.ArgumentException))]
		public void CheckBinaryTypeEx1()
		{
			BinaryType type = new BinaryType ();

			type.DefineMimeType ("; ;");
		}

		[Test]
		public void CheckDefaultTypes()
		{
			Assert.IsNotNull (StringType.NativeDefault);
			Assert.AreEqual ("Default.StringNative", StringType.NativeDefault.Name);
			Assert.IsTrue (StringType.NativeDefault.IsNullable);
			
			Assert.IsNotNull (IntegerType.Default);
			Assert.AreEqual ("Default.Integer", IntegerType.Default.Name);
			Assert.IsFalse (IntegerType.Default.IsNullable);

			Assert.IsNotNull (DateType.Default);
			Assert.IsNotNull (DateTimeType.Default);
			Assert.IsNotNull (TimeType.Default);

			Assert.IsNotNull (Epsitec.Common.Types.DruidType.Default);
			Assert.AreEqual ("Default.Druid", Epsitec.Common.Types.DruidType.Default.Name);
			Assert.IsFalse (Epsitec.Common.Types.DruidType.Default.IsNullable);
			Assert.AreEqual (typeof (Druid), Epsitec.Common.Types.DruidType.Default.SystemType);
		}

		[Test]
		public void CheckStringType()
		{
			StringType typeString = Epsitec.Common.Types.Res.Types.Default.String;
			StringType typeStringMulti = Epsitec.Common.Types.Res.Types.Default.StringMultiline;
			StringType typeText = Epsitec.Common.Types.Res.Types.Default.Text;
			StringType typeTextMulti = Epsitec.Common.Types.Res.Types.Default.TextMultiline;

			Assert.AreEqual (false, typeString.UseFormattedText);
			Assert.AreEqual (null, typeString.DefaultControllerParameters);

			Assert.AreEqual (false, typeStringMulti.UseFormattedText);
			Assert.AreEqual ("Mode=Multiline", typeStringMulti.DefaultControllerParameters);

			Assert.AreEqual (true, typeText.UseFormattedText);
			Assert.AreEqual (null, typeText.DefaultControllerParameters);

			Assert.AreEqual (true, typeTextMulti.UseFormattedText);
			Assert.AreEqual ("Mode=Multiline", typeTextMulti.DefaultControllerParameters);

			Assert.AreEqual (true,  typeString.IsValidValue (null));
			Assert.AreEqual (true,  typeString.IsValidValue (""));
			Assert.AreEqual (true,  typeString.IsValidValue ("Abc"));
			Assert.AreEqual (false, typeString.IsValidValue (new FormattedText ("Hello")));

			Assert.AreEqual (true,  typeText.IsValidValue (null));
			Assert.AreEqual (false, typeText.IsValidValue (""));
			Assert.AreEqual (false, typeText.IsValidValue ("Abc"));
			Assert.AreEqual (true, typeText.IsValidValue (new FormattedText ("Hello")));

		}
		
		[Test]
		public void CheckEnumType1()
		{
			System.Type type = typeof (MyEnum);
			
			EnumType et = new EnumType (type);
			EnumValue[] ev = Collection.ToArray<EnumValue> (et.Values);

			Assert.AreEqual ("Enumeration MyEnum", et.Name);
			Assert.AreEqual (Druid.Empty, et.CaptionId);
			
			Assert.AreEqual (5, ev.Length);
			Assert.IsFalse (et.IsCustomizable);
			Assert.IsFalse (et.IsDefinedAsFlags);

			Assert.AreEqual ("None",   ev[0].Name);
			Assert.AreEqual ("First",  ev[1].Name);
			Assert.AreEqual ("Second", ev[2].Name);
			Assert.AreEqual ("Third",  ev[3].Name);
			Assert.AreEqual ("Extra",  ev[4].Name);

			Assert.IsFalse (ev[0].IsHidden);
			Assert.IsTrue (ev[4].IsHidden);
			
			Assert.AreEqual (-1, et["None"]  .Rank);
			Assert.AreEqual ( 1, et["First"] .Rank);
			Assert.AreEqual ( 2, et["Second"].Rank);
			Assert.AreEqual ( 3, et["Third"] .Rank);
			Assert.AreEqual (99, et["Extra"] .Rank);

			Assert.AreEqual (MyEnum.None, (et["None"].Value));
			Assert.AreEqual (MyEnum.First, (et["First"].Value));
			Assert.AreEqual (MyEnum.Second, (et["Second"].Value));
			Assert.AreEqual (MyEnum.Third, (et["Third"].Value));
			Assert.AreEqual (MyEnum.Extra, (et["Extra"].Value));

			Assert.AreEqual (-1, et[MyEnum.None].Rank);
			Assert.AreEqual (1, et[MyEnum.First].Rank);
			Assert.AreEqual (2, et[MyEnum.Second].Rank);
			Assert.AreEqual (3, et[MyEnum.Third].Rank);
			Assert.AreEqual (99, et[MyEnum.Extra].Rank);
			
			Assert.AreEqual ("None", et[-1].Name);
			Assert.AreEqual ("First" , et[ 1].Name);
			Assert.AreEqual ("Second", et[ 2].Name);
			Assert.AreEqual ("Third" , et[ 3].Name);
			Assert.AreEqual ("Extra" , et[99].Name);
			
			Assert.IsTrue (et.IsValidValue ("3"));
			Assert.IsTrue (et.IsValidValue (-1));
			Assert.IsTrue (et.IsValidValue ("Extra"));
			Assert.IsFalse (et.IsValidValue (18));
			Assert.IsFalse (et.IsValidValue ("{Other}"));
		}
		
		[Test]
		public void CheckEnumType2()
		{
			System.Type type = typeof (MyFlags);
			
			EnumType et = new EnumType (type);
			EnumValue[] ev = Collection.ToArray<EnumValue> (et.Values);
			
			Assert.AreEqual (5, ev.Length);
			Assert.IsFalse (et.IsCustomizable);
			Assert.IsTrue (et.IsDefinedAsFlags);
			
			Assert.AreEqual ("None",  ev[0].Name);
			Assert.AreEqual ("Flag1", ev[1].Name);
			Assert.AreEqual ("Flag2", ev[2].Name);
			Assert.AreEqual ("Flag3", ev[3].Name);
			Assert.AreEqual ("Flag4", ev[4].Name);

			Assert.AreEqual (0, et["None"].Rank);
			Assert.AreEqual (1, et["Flag1"].Rank);
			Assert.AreEqual (2, et["Flag2"].Rank);
			Assert.AreEqual (4, et["Flag3"].Rank);
			Assert.AreEqual (8, et["Flag4"].Rank);

			Assert.AreEqual (MyFlags.None, (et["None"].Value));
			Assert.AreEqual (MyFlags.Flag1, (et["Flag1"].Value));
			Assert.AreEqual (MyFlags.Flag2, (et["Flag2"].Value));
			Assert.AreEqual (MyFlags.Flag3, (et["Flag3"].Value));
			Assert.AreEqual (MyFlags.Flag4, (et["Flag4"].Value));
			
			Assert.AreEqual ("None",  et[0].Name);
			Assert.AreEqual ("Flag1", et[1].Name);
			Assert.AreEqual ("Flag2", et[2].Name);
			Assert.AreEqual ("Flag3", et[4].Name);
			Assert.AreEqual ("Flag4", et[8].Name);
			
			Assert.IsTrue (et.IsValidValue ("0"));
			Assert.IsTrue (et.IsValidValue (0xf));
			Assert.IsTrue (et.IsValidValue ("Flag1, Flag2"));
			Assert.IsFalse (et.IsValidValue (0x18));
			Assert.IsFalse (et.IsValidValue ("{Other}"));
		}

		[Test]
		public void CheckEnumType3()
		{
			System.Type type = typeof (MyEnum2);

			EnumType et = new EnumType (type);
			EnumValue[] ev = Collection.ToArray<EnumValue> (et.Values);

			Assert.AreEqual (4, ev.Length);
			Assert.IsFalse (et.IsCustomizable);
			Assert.IsFalse (et.IsDefinedAsFlags);

			Assert.AreEqual ("ValueA", ev[0].Name);
			Assert.AreEqual ("ValueB", ev[1].Name);
			Assert.AreEqual ("ValueC", ev[2].Name);
			Assert.AreEqual ("None", ev[3].Name);

			Assert.AreEqual (5, et["None"].Rank);
			Assert.AreEqual (1, et["ValueA"].Rank);
			Assert.AreEqual (1, et["ValueB"].Rank);
			Assert.AreEqual (2, et["ValueC"].Rank);

			Assert.AreEqual (MyEnum2.None, (et["None"].Value));
			Assert.AreEqual (MyEnum2.ValueA, (et["ValueA"].Value));
			Assert.AreEqual (MyEnum2.ValueB, (et["ValueB"].Value));
			Assert.AreEqual (MyEnum2.ValueC, (et["ValueC"].Value));
		}

		[Test]
		public void CheckEnumType4()
		{
			System.ComponentModel.EnumConverter converter = new System.ComponentModel.EnumConverter (typeof (MyEnum));

			//	The converter cannot be used to convert a MyEnum object to an int.

			System.Enum value1 = MyEnum.First;
			MyEnum      value2 = MyEnum.First;
			object      value3 = System.Enum.ToObject (typeof (MyEnum), 1);

			//	Casting to int only works with the real MyEnum typed object. The others,
			//	even if they are equal, won't accept the cast at compile time :
			
			System.Console.Out.WriteLine ("System.Enum --> {0} ---", value1.GetType ().FullName/*, (int) value1*/);
			System.Console.Out.WriteLine ("MyEnum -------> {0} {1}", value2.GetType ().FullName, (int) value2);
			System.Console.Out.WriteLine ("object -------> {0} ---", value3.GetType ().FullName/*, (int) value3*/);

			Assert.AreEqual (value1, value2);
			Assert.AreEqual (value1, value3);
		}
		
		[Test] public void CheckOpenEnumType()
		{
			EnumType et = new OpenEnumType (typeof (MyEnum));
			EnumValue[] ev = Collection.ToArray<EnumValue> (et.Values);

			Assert.AreEqual ("Enumeration MyEnum", et.Name);
			Assert.AreEqual (Druid.Empty, et.CaptionId);
			
			Assert.AreEqual (5, ev.Length);
			Assert.IsTrue (et.IsCustomizable);

			Assert.AreEqual ("None",   ev[0].Name);
			Assert.AreEqual ("First",  ev[1].Name);
			Assert.AreEqual ("Second", ev[2].Name);
			Assert.AreEqual ("Third",  ev[3].Name);
			Assert.AreEqual ("Extra",  ev[4].Name);
			
			Assert.AreEqual (-1, et["None"]  .Rank);
			Assert.AreEqual ( 1, et["First"] .Rank);
			Assert.AreEqual ( 2, et["Second"].Rank);
			Assert.AreEqual ( 3, et["Third"] .Rank);
			Assert.AreEqual (99, et["Extra"] .Rank);
			
			Assert.AreEqual ("None"  , et[-1].Name);
			Assert.AreEqual ("First" , et[ 1].Name);
			Assert.AreEqual ("Second", et[ 2].Name);
			Assert.AreEqual ("Third" , et[ 3].Name);
			Assert.AreEqual ("Extra" , et[99].Name);
			
			Assert.IsTrue (et.IsValidValue ("3"));
			Assert.IsTrue (et.IsValidValue (-1));
			Assert.IsTrue (et.IsValidValue ("Extra"));
			Assert.IsFalse (et.IsValidValue (18));
			Assert.IsTrue (et.IsValidValue ("{Other}"));
		}

		[Test]
		public void CheckDate()
		{
			System.ComponentModel.TypeConverter converter = System.ComponentModel.TypeDescriptor.GetConverter (typeof (Date));

			Date datePast = new Date (2006, 2, 15);
			Date dateNoDay = new Date (2006, 2, 0);
			Date dateNoMonth = new Date (2006, 0, 0);
			Date dateOnlyDayMonth = new Date (0, 2, 15);
			Date dateNow = new Date (System.DateTime.Now);
			Date dateNull = Date.FromObject (null);

			Assert.AreEqual (732356 * Time.TicksPerDay, datePast.Ticks);
			Assert.IsTrue (datePast < dateNow);
			Assert.IsTrue (datePast <= dateNow);
			Assert.IsFalse (datePast > dateNow);
			Assert.IsFalse (datePast >= dateNow);
			Assert.IsTrue (datePast.HasDay);
			Assert.IsTrue (datePast.HasMonth);
			Assert.IsTrue (datePast.HasYear);
			Assert.IsFalse (dateNoDay.HasDay);
			Assert.IsFalse (dateNoMonth.HasMonth);
			Assert.IsFalse (dateOnlyDayMonth.HasYear);

			Assert.AreEqual ("B2CC47", converter.ConvertToInvariantString (datePast));
			Assert.AreEqual ("000000", converter.ConvertToInvariantString (dateNull));
			Assert.AreEqual (datePast, converter.ConvertFromInvariantString ("B2CC47"));

			Assert.AreEqual (new Date (2006, 2, 1).Ticks, dateNoDay.Ticks);
			Assert.AreEqual ("B2CB66", converter.ConvertToInvariantString (dateNoDay));
			
			Assert.IsTrue (dateNoDay < new Date (2006, 2, 1));
			Assert.IsTrue (dateNoDay > new Date (2006, 1, 31));
			Assert.IsTrue (dateNoMonth < new Date (2006, 1, 1));
			Assert.IsTrue (dateNoMonth > new Date (2005, 12, 31));
			
			Assert.IsTrue (dateNull.IsNull);

			Assert.AreEqual (0, new Date (2006, 8, 28).ComputeAge (new Date (2006, 8, 28)));
			Assert.AreEqual (0, new Date (2006, 8, 28).ComputeAge (new Date (2007, 8, 27)));
			Assert.AreEqual (6, new Date (2006, 8, 28).ComputeAge (new Date (2013, 1, 24)));
			Assert.AreEqual (6, new Date (2006, 8, 28).ComputeAge (new Date (2013, 8, 27)));
			Assert.AreEqual (7, new Date (2006, 8, 28).ComputeAge (new Date (2013, 8, 28)));
			Assert.AreEqual (69, new Date (1943, 1, 3).ComputeAge (new Date (2013, 1, 2)));
			Assert.AreEqual (70, new Date (1943, 1, 3).ComputeAge (new Date (2013, 2, 1)));
		}

		[Test]
		[ExpectedException (typeof (System.ArgumentOutOfRangeException))]
		public void CheckDateInvalid1()
		{
			Date date = new Date (2010, 9, 31);
		}

		[Test]
		[ExpectedException (typeof (System.ArgumentOutOfRangeException))]
		public void CheckDateInvalid2()
		{
			Date date = new Date (2010, 13, 1);
		}

		[Test]
		public void CheckDateSerialization()
		{
			Date date_1 = Date.Today;
			Date date_2;

			DateUser dt_1 = new DateUser ();
			DateUser dt_2;

			using (System.IO.Stream stream = System.IO.File.Open ("test.bin", System.IO.FileMode.Create))
			{
				BinaryFormatter formatter = new BinaryFormatter ();
				formatter.Serialize (stream, date_1);
			}

			using (System.IO.Stream stream = System.IO.File.Open ("test.bin", System.IO.FileMode.Open))
			{
				BinaryFormatter formatter = new BinaryFormatter ();
				date_2 = (Date) formatter.Deserialize (stream);
			}

			using (System.IO.Stream stream = System.IO.File.Open ("test.bin", System.IO.FileMode.Create))
			{
				BinaryFormatter formatter = new BinaryFormatter ();
				formatter.Serialize (stream, dt_1);
			}

			using (System.IO.Stream stream = System.IO.File.Open ("test.bin", System.IO.FileMode.Open))
			{
				BinaryFormatter formatter = new BinaryFormatter ();
				dt_2 = (DateUser) formatter.Deserialize (stream);
			}

			Assert.IsNotNull (dt_2);

			System.IO.File.Delete ("test.bin");

			using (System.IO.Stream stream = System.IO.File.Open ("test.soap", System.IO.FileMode.Create))
			{
				SoapFormatter formatter = new SoapFormatter ();
				formatter.Serialize (stream, date_1);
			}

			using (System.IO.Stream stream = System.IO.File.Open ("test.soap", System.IO.FileMode.Open))
			{
				System.Text.UTF8Encoding encoding = new System.Text.UTF8Encoding ();
				byte[] buffer = new byte[stream.Length];
				stream.Read (buffer, 0, (int) stream.Length);
				System.Console.Out.WriteLine ("{0}", encoding.GetString (buffer));
			}

			using (System.IO.Stream stream = System.IO.File.Open ("test.soap", System.IO.FileMode.Open))
			{
				SoapFormatter formatter = new SoapFormatter ();
				date_2 = (Date) formatter.Deserialize (stream);
			}

			using (System.IO.Stream stream = System.IO.File.Open ("test.soap", System.IO.FileMode.Create))
			{
				SoapFormatter formatter = new SoapFormatter ();
				formatter.Serialize (stream, dt_1);
			}

			using (System.IO.Stream stream = System.IO.File.Open ("test.soap", System.IO.FileMode.Open))
			{
				System.Text.UTF8Encoding encoding = new System.Text.UTF8Encoding ();
				byte[] buffer = new byte[stream.Length];
				stream.Read (buffer, 0, (int) stream.Length);
				System.Console.Out.WriteLine ("{0}", encoding.GetString (buffer));
			}

			using (System.IO.Stream stream = System.IO.File.Open ("test.soap", System.IO.FileMode.Open))
			{
				SoapFormatter formatter = new SoapFormatter ();
				dt_2 = (DateUser) formatter.Deserialize (stream);
			}

			System.IO.File.Delete ("test.soap");
		}

		[Test]
		public void CheckDateType()
		{
			DateType type = new DateType ();

			Assert.AreEqual (typeof (Date), type.SystemType);
			Assert.AreEqual (TimeResolution.Default, type.Resolution);
			Assert.AreEqual (Time.Null, type.MinimumTime);
			Assert.AreEqual (Time.Null, type.MaximumTime);

			Date date = new Date (2006, 11, 2);

			Assert.IsTrue (type.IsValidValue (date));
			Assert.IsTrue (type.IsNullValue (null));
			Assert.IsTrue (type.IsNullValue (Date.Null));
			Assert.IsFalse (type.IsValidValue (null));
			Assert.IsFalse (type.IsValidValue (Date.Null));
			Assert.IsFalse (type.IsValidValue ("x"));

			type.DefineMinimumDate (new Date (2000, 1, 1));
			type.DefineMaximumDate (new Date (2010, 12, 31));

			Assert.IsTrue (type.IsValidValue (new Date (2006, 11, 2)));
			Assert.IsFalse (type.IsValidValue (new Date (1999, 12, 31)));
			Assert.IsFalse (type.IsValidValue (new Date (2032, 2, 11)));

			type.DefineResolution (TimeResolution.Weeks);

			string xml = type.Caption.SerializeToString ();

			Caption caption = new Caption ();
			caption.DeserializeFromString (xml);

			DateType copy = new DateType (caption);

			Assert.AreEqual (type.MinimumDate, copy.MinimumDate);
			Assert.AreEqual (type.MinimumTime, copy.MinimumTime);
			Assert.AreEqual (type.MaximumDate, copy.MaximumDate);
			Assert.AreEqual (type.MaximumTime, copy.MaximumTime);
			Assert.AreEqual (type.Resolution, copy.Resolution);
		}

		[Test]
		public void CheckTime()
		{
			System.ComponentModel.TypeConverter converter = System.ComponentModel.TypeDescriptor.GetConverter (typeof (Time));

			Time time1 = new Time (1, 10, 30);
			Time time2 = new Time (new System.DateTime (2006, 2, 15, 1, 10, 30));
			Time time3 = new Time (1, 10, 31);
			Time timeNull = Time.FromObject (null);

			Assert.AreEqual (time1.Ticks, time2.Ticks);
			Assert.IsTrue (time1 == time2);
			Assert.IsTrue (time1 < time3);
			Assert.IsTrue (time3 > time1);
			Assert.IsTrue (time1 <= time3);
			Assert.IsTrue (time3 >= time1);

			Assert.AreEqual ("4230000", converter.ConvertToInvariantString (time1));
			Assert.AreEqual ("4231000", converter.ConvertToInvariantString (time3));

			Assert.IsTrue (timeNull.IsNull);
		}

		[Test]
		public void CheckTimeType()
		{
			TimeType type = new TimeType ();

			Assert.AreEqual (typeof (Time), type.SystemType);
			Assert.AreEqual (TimeResolution.Default, type.Resolution);
			Assert.AreEqual (Time.Null, type.MinimumTime);
			Assert.AreEqual (Time.Null, type.MaximumTime);

			Time time = new Time (9, 44, 13);

			Assert.IsTrue (type.IsValidValue (time));
			Assert.IsTrue (type.IsNullValue (null));
			Assert.IsTrue (type.IsNullValue (Time.Null));
			Assert.IsFalse (type.IsValidValue (null));
			Assert.IsFalse (type.IsValidValue (Time.Null));
			Assert.IsFalse (type.IsValidValue ("x"));

			type.DefineMinimumTime (new Time (6, 0, 0));
			type.DefineMaximumTime (new Time (16, 59, 59, 999));

			Assert.IsTrue (type.IsValidValue (new Time (8, 30, 17)));
			Assert.IsFalse (type.IsValidValue (new Time (17, 34, 0)));
			Assert.IsFalse (type.IsValidValue (new Time (1, 10, 0)));

			type.DefineResolution (TimeResolution.Seconds);

			string xml = type.Caption.SerializeToString ();

			Caption caption = new Caption ();
			caption.DeserializeFromString (xml);

			TimeType copy = new TimeType (caption);

			Assert.AreEqual (type.MinimumDate, copy.MinimumDate);
			Assert.AreEqual (type.MinimumTime, copy.MinimumTime);
			Assert.AreEqual (type.MaximumDate, copy.MaximumDate);
			Assert.AreEqual (type.MaximumTime, copy.MaximumTime);
			Assert.AreEqual (type.Resolution, copy.Resolution);
		}

		[Test]
		public void CheckDateTimeType()
		{
			DateTimeType type = new DateTimeType ();

			Assert.AreEqual (typeof (System.DateTime), type.SystemType);
			Assert.AreEqual (TimeResolution.Default, type.Resolution);
			Assert.AreEqual (Time.Null, type.MinimumTime);
			Assert.AreEqual (Time.Null, type.MaximumTime);

			System.DateTime time = new System.DateTime (2006, 11, 2, 10, 52, 34, 942);

			Assert.IsTrue (type.IsValidValue (time));
			Assert.IsTrue (type.IsNullValue (null));
			Assert.IsFalse (type.IsValidValue (null));
			Assert.IsFalse (type.IsValidValue ("x"));

			type.DefineMinimumTime (new Time (6, 0, 0));
			type.DefineMaximumTime (new Time (16, 59, 59, 999));

			type.DefineMinimumDate (new Date (2000, 1, 1));
			type.DefineMaximumDate (new Date (2010, 12, 31));
			
			Assert.IsTrue (type.IsValidValue (new System.DateTime (2006, 11, 2, 8, 30, 17)));
			Assert.IsFalse (type.IsValidValue (new System.DateTime (2006, 11, 2, 17, 34, 0)));
			Assert.IsFalse (type.IsValidValue (new System.DateTime (2006, 11, 2, 1, 10, 0)));
			Assert.IsFalse (type.IsValidValue (new System.DateTime (2032, 2, 11, 8, 30, 17)));
			Assert.IsFalse (type.IsValidValue (new System.DateTime (1999, 12, 31, 8, 30, 17)));

			type.DefineResolution (TimeResolution.Minutes);
			type.DefineTimeStep (new System.TimeSpan (0, 0, 30));
			type.DefineDateStep (new DateSpan (7));

			string xml = type.Caption.SerializeToString ();

			System.Console.Out.WriteLine (xml);

			Caption caption = new Caption ();
			caption.DeserializeFromString (xml);

			DateTimeType copy = new DateTimeType (caption);

			Assert.AreEqual (type.MinimumDate, copy.MinimumDate);
			Assert.AreEqual (type.MinimumTime, copy.MinimumTime);
			Assert.AreEqual (type.MaximumDate, copy.MaximumDate);
			Assert.AreEqual (type.MaximumTime, copy.MaximumTime);
			Assert.AreEqual (type.Resolution, copy.Resolution);
		}

		[Test]
		public void CheckNullableTypes()
		{
			int? valueOne = 1;
			int? valueNull = null;

			IntegerType type1 = new IntegerType (0, 100);
			IntegerType type2 = new IntegerType (0, 100);
			
			type1.DefineIsNullable (true);

			Assert.IsTrue (type1.IsNullable);
			Assert.IsFalse (type1.IsNullValue (valueOne));
			Assert.IsTrue (type1.IsNullValue (valueNull));
			Assert.IsTrue (type1.IsValidValue (valueOne));
			Assert.IsTrue (type1.IsValidValue (valueNull));
			
			Assert.IsFalse (type2.IsNullable);
			Assert.IsFalse (type2.IsNullValue (valueOne));
			Assert.IsTrue (type2.IsNullValue (valueNull));
			Assert.IsTrue (type2.IsValidValue (valueOne));
			Assert.IsFalse (type2.IsValidValue (valueNull));
		}

		[Test]
		public void CheckNumericTypes()
		{
			bool num1 = true;
			short num16 = 1;
			int num32 = 1;
			long num64 = 1;
			decimal numDecimal = 1;

			BooleanType type1 = new BooleanType ();
			IntegerType type32 = new IntegerType ();
			LongIntegerType type64 = new LongIntegerType ();
			DecimalType typeDecimal = new DecimalType ();

			Assert.IsTrue (type32.IsValidValue (num32));
			Assert.IsTrue (type64.IsValidValue (num64));
			Assert.IsTrue (typeDecimal.IsValidValue (numDecimal));

			Assert.IsFalse (type32.IsValidValue (num16));
			Assert.IsFalse (type64.IsValidValue (num16));
			Assert.IsFalse (typeDecimal.IsValidValue (num16));

			Assert.IsFalse (type64.IsValidValue (num32));
			Assert.IsFalse (typeDecimal.IsValidValue (num32));

			Assert.IsFalse (type32.IsValidValue (num64));
			Assert.IsFalse (typeDecimal.IsValidValue (num64));

			Assert.IsFalse (type32.IsValidValue (numDecimal));
			Assert.IsFalse (type64.IsValidValue (numDecimal));

			Assert.IsTrue (type1.IsValidValue (num1));
			Assert.IsTrue (type1.IsValidValue (!num1));
			Assert.IsFalse (type1.IsValidValue (0));
			Assert.IsFalse (type1.IsValidValue (1));

			IntegerType limit1 = new IntegerType (0, 99999);

			Assert.IsTrue (limit1.IsValidValue (0));
			Assert.IsTrue (limit1.IsValidValue (1000));
			Assert.IsTrue (limit1.IsValidValue (99999));
			
			Assert.IsFalse (limit1.IsValidValue (99999+1));
			Assert.IsFalse (limit1.IsValidValue (-1));

			DecimalType limit2 = new DecimalType (0, 10, 0.1M);

			limit2.DefineName ("Rating");

			Assert.IsTrue (limit2.IsValidValue (0.0M));
			Assert.IsTrue (limit2.IsValidValue (0.1M));
			Assert.IsTrue (limit2.IsValidValue (9.9M));
			Assert.IsTrue (limit2.IsValidValue (10.0M));
			Assert.IsTrue (limit2.IsValidValue (9.90M));
			Assert.IsFalse (limit2.IsValidValue (9.95M));

			Assert.AreEqual ("Boolean", type1.Name);
			Assert.AreEqual ("Integer", type32.Name);
			Assert.AreEqual ("LongInteger", type64.Name);
			Assert.AreEqual ("Decimal", typeDecimal.Name);
			Assert.AreEqual ("Rating", limit2.Name);
			Assert.AreEqual (typeof (bool), type1.SystemType);
			Assert.AreEqual (typeof (int), type32.SystemType);
			Assert.AreEqual (typeof (long), type64.SystemType);
			Assert.AreEqual (typeof (decimal), typeDecimal.SystemType);
			Assert.AreEqual (typeof (decimal), limit2.SystemType);
		}

		[Test]
		public void CheckOtherType()
		{
			OtherType type = new OtherType ();
			Caption caption = new Caption ();
			AbstractType.SetSystemType (caption, typeof (DecimalRange));
			type.DefineCaption (caption);

			Assert.IsTrue (type.IsValidValue (new DecimalRange ()));
			Assert.IsFalse (type.IsValidValue (null));
			Assert.IsFalse (type.IsValidValue (10));
		}

		[Test]
		public void CheckSupportTypes()
		{
//-			Assert.AreSame (StringType.Default, Epsitec.Common.Support.Res.Types.StringCollection.ItemType);

			Assert.IsNotNull (Epsitec.Common.Support.Res.Types.Field);
//-			Assert.IsNotNull (Epsitec.Common.Support.Res.Types.FieldCollection);
//-			Assert.IsNotNull (Epsitec.Common.Support.Res.Types.FieldCollection.ItemType);
			
//-			Assert.AreEqual (Epsitec.Common.Support.Res.Types.Field.CaptionId, Epsitec.Common.Support.Res.Types.FieldCollection.ItemType.CaptionId);
		}

		[Test]
		public void CheckTypeCreation()
		{
			Caption caption = new Caption ();

			caption.Name = "Nom";
			AbstractType.SetSystemType (caption, typeof (string).FullName);

			StringType strType = TypeRosetta.CreateTypeObject (caption) as StringType;

			Assert.IsNotNull (strType);
			Assert.AreEqual ("Nom", strType.Name);
			Assert.AreEqual (0, strType.MinimumLength);
			Assert.AreEqual (1000000, strType.MaximumLength);

			caption = new Caption ();
			caption.Name = "Xxx";
			
			AbstractType.SetSystemType (caption, typeof (bool));
			Assert.IsNotNull (TypeRosetta.CreateTypeObject (caption) as BooleanType);

			AbstractType.SetSystemType (caption, typeof (decimal));
			Assert.IsNotNull (TypeRosetta.CreateTypeObject (caption) as DecimalType);

			AbstractType.SetSystemType (caption, typeof (double));
			Assert.IsNotNull (TypeRosetta.CreateTypeObject (caption) as DoubleType);

			AbstractType.SetSystemType (caption, typeof (int));
			Assert.IsNotNull (TypeRosetta.CreateTypeObject (caption) as IntegerType);

			AbstractType.SetSystemType (caption, typeof (long));
			Assert.IsNotNull (TypeRosetta.CreateTypeObject (caption) as LongIntegerType);

			AbstractType.SetSystemType (caption, typeof (void));
			Assert.IsNotNull (TypeRosetta.CreateTypeObject (caption) as OtherType);
			
			AbstractType.SetSystemType (caption, typeof (DecimalRange));
			Assert.IsNotNull (TypeRosetta.CreateTypeObject (caption) as OtherType);
		}

		[Test]
		public void CheckTypeSerialization()
		{
			Caption caption;
			string intSerial;
			string strSerial;

			caption = TypeRosetta.GetTypeObject ("Default.Integer").Caption;
			intSerial = caption.SerializeToString ();

			System.Console.WriteLine ("Integer caption ID: {0}", caption.Id.ToString ());

			Assert.AreEqual ("Default.Integer", TypeRosetta.GetTypeObject ("Default.Integer").Name);
			Assert.AreEqual ("Default.Integer", caption.Name);
			Assert.AreEqual ("V:"+typeof (int).FullName, caption.GetValue (AbstractType.SytemTypeProperty));

			caption = TypeRosetta.GetTypeObject ("Default.String").Caption;
			strSerial = caption.SerializeToString ();

			System.Console.WriteLine ("String caption ID: {0}", caption.Id.ToString ());

			System.Console.WriteLine ("Integer: {0}", intSerial);
			System.Console.WriteLine ("String: {0}", strSerial);

			AbstractType type = TypeRosetta.CreateTypeObject (TypeRosetta.GetTypeObject ("Default.Integer").Caption);

			Assert.IsNotNull (type);
			Assert.AreEqual ("Default.Integer", type.Name);
			Assert.AreEqual (typeof (int), type.SystemType);
		}
		
		[System.Serializable]
		class DateUser : System.Runtime.Serialization.ISerializable
		{
			public DateUser()
			{
			}
			
			protected DateUser(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) : this()
			{
				this.time = new Time (info.GetInt64 ("time"));
				this.date = new Date (info.GetInt64 ("date"));
			}
			
			#region ISerializable Members

			public void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
			{
				info.AddValue ("time", this.time.Ticks);
				info.AddValue ("date", this.date.Ticks);
			}
			#endregion
			
			private Date			date = Date.Today;
			private Time			time = Time.Now;
		}

		private enum MyEnum : short
		{
			None	= -1,
			First	=  1,
			Second	=  2,
			Third	=  3,
			
			[Hidden] Extra	= 99
		}

		private enum MyEnum2 : long
		{
			[Rank (5)] None,
			[Rank (1)] ValueB,
			[Rank (1)] ValueA,
			[Rank (2)] ValueC
		}
		
		[System.Flags]
		private enum MyFlags : byte
		{
			None	= 0,
			Flag1	= 1,
			Flag2	= 2,
			Flag3	= 4,
			Flag4	= 8
		}
	}
}
