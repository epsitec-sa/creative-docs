using NUnit.Framework;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization.Formatters.Soap;

namespace Epsitec.Common.Types
{
	[TestFixture] public class BasicTypesTest
	{
		[Test]
		public void CheckDefaultTypes()
		{
			Assert.IsNotNull (StringType.Default);
			Assert.AreEqual ("Default.String", StringType.Default.Name);
			Assert.IsTrue (StringType.Default.IsNullable);
			
			Assert.IsNotNull (IntegerType.Default);
			Assert.AreEqual ("Default.Integer", IntegerType.Default.Name);
			Assert.IsFalse (IntegerType.Default.IsNullable);
		}
		
		[Test]
		public void CheckEnumType1()
		{
			System.Type type = typeof (MyEnum);
			
			EnumType et = new EnumType (type);
			EnumValue[] ev = Collection.ToArray<EnumValue> (et.Values);

			Assert.AreEqual ("Enumeration MyEnum", et.Name);
			Assert.AreEqual (Support.Druid.Empty, et.CaptionId);
			
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
		[Ignore ("No caption defined for enum")]
		public void CheckEnumType4()
		{
			System.Type type = typeof (MyEnum);
			
			EnumType et = new EnumType (type);

			Assert.AreEqual ("[res:Enums#MyEnum.capt]", et.CaptionId);
			
			Assert.AreEqual ("[res:Enums#MyEnum.None.capt]",   et["None"].CaptionId);
			Assert.AreEqual ("[res:Enums#MyEnum.First.capt]",  et["First"].CaptionId);
			Assert.AreEqual ("[res:Enums#MyEnum.Second.capt]", et["Second"].CaptionId);
			Assert.AreEqual ("[res:Enums#MyEnum.Third.capt]",  et["Third"].CaptionId);
			Assert.AreEqual ("[res:Enums#MyEnum.Extra.capt]",  et["Extra"].CaptionId);
		}

		[Test]
		public void CheckEnumType5()
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
			Assert.AreEqual (Support.Druid.Empty, et.CaptionId);
			
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
			Date dateNow = new Date (System.DateTime.Now);
			Date dateNull = new Date (null);

			Assert.AreEqual (732356 * Time.TicksPerDay, datePast.Ticks);
			Assert.IsTrue (datePast < dateNow);
			Assert.IsTrue (datePast <= dateNow);
			Assert.IsFalse (datePast > dateNow);
			Assert.IsFalse (datePast >= dateNow);

			Assert.AreEqual ("732356", converter.ConvertToInvariantString (datePast));
			
			Assert.IsTrue (dateNull.IsNull);
		}

		[Test]
		public void CheckTime()
		{
			System.ComponentModel.TypeConverter converter = System.ComponentModel.TypeDescriptor.GetConverter (typeof (Time));

			Time time1 = new Time (1, 10, 30);
			Time time2 = new Time (new System.DateTime (2006, 2, 15, 1, 10, 30));
			Time time3 = new Time (1, 10, 31);
			Time timeNull = new Time (null);

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
		public void CheckTypeCreation()
		{
			Caption caption = new Caption ();

			caption.Name = "Nom";
			AbstractType.SetSystemType (caption, typeof (string).FullName);

			StringType strType = TypeRosetta.CreateTypeObject (caption) as StringType;

			Assert.IsNotNull (strType);
			Assert.AreEqual ("Nom", strType.Name);
			Assert.AreEqual (0, strType.MinimumLength);
			Assert.AreEqual (100*1000, strType.MaximumLength);

			caption = new Caption ();
			caption.Name = "Xxx";
			
			AbstractType.SetSystemType (caption, typeof (bool).FullName);
			Assert.IsNotNull (TypeRosetta.CreateTypeObject (caption) as BooleanType);

			AbstractType.SetSystemType (caption, typeof (decimal).FullName);
			Assert.IsNotNull (TypeRosetta.CreateTypeObject (caption) as DecimalType);

			AbstractType.SetSystemType (caption, typeof (double).FullName);
			Assert.IsNotNull (TypeRosetta.CreateTypeObject (caption) as DoubleType);

			AbstractType.SetSystemType (caption, typeof (int).FullName);
			Assert.IsNotNull (TypeRosetta.CreateTypeObject (caption) as IntegerType);

			AbstractType.SetSystemType (caption, typeof (long).FullName);
			Assert.IsNotNull (TypeRosetta.CreateTypeObject (caption) as LongIntegerType);

			AbstractType.SetSystemType (caption, typeof (void).FullName);
			Assert.IsNotNull (TypeRosetta.CreateTypeObject (caption) as VoidType);
		}

		[Test]
		public void CheckTypeSerialization()
		{
			Caption caption;
			string intSerial;
			string strSerial;

			caption = TypeRosetta.GetTypeObject ("Default.Integer").Caption;
			intSerial = caption.SerializeToString ();

			System.Console.WriteLine ("Integer caption ID: {0}", caption.Druid.ToString ());

			Assert.AreEqual ("Default.Integer", TypeRosetta.GetTypeObject ("Default.Integer").Name);
			Assert.AreEqual ("Default.Integer", caption.Name);
			Assert.AreEqual ("V:"+typeof (int).FullName, caption.GetValue (AbstractType.SytemTypeProperty));

			caption = TypeRosetta.GetTypeObject ("Default.String").Caption;
			strSerial = caption.SerializeToString ();

			System.Console.WriteLine ("String caption ID: {0}", caption.Druid.ToString ());

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
				this.time = new Types.Time (info.GetInt64 ("time"));
				this.date = new Types.Date (info.GetInt64 ("date"));
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
