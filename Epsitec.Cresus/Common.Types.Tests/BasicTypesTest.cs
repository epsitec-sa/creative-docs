using NUnit.Framework;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization.Formatters.Soap;

namespace Epsitec.Common.Types
{
	[TestFixture] public class BasicTypesTest
	{
		[Test] public void CheckEnumType1()
		{
			System.Type type = typeof (MyEnum);
			
			EnumType et = new EnumType (type);
			
			Assert.AreEqual (5, et.Values.Length);
			Assert.IsFalse (et.IsCustomizable);
			Assert.IsFalse (et.IsDefinedAsFlags);
			
			Assert.AreEqual ("None",   et.Values[0].Name);
			Assert.AreEqual ("First",  et.Values[1].Name);
			Assert.AreEqual ("Second", et.Values[2].Name);
			Assert.AreEqual ("Third",  et.Values[3].Name);
			Assert.AreEqual ("Extra",  et.Values[4].Name);
			
			Assert.IsFalse (et.Values[0].IsHidden);
			Assert.IsTrue  (et.Values[4].IsHidden);
			
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
			
			Assert.IsTrue (et.CheckConstraint ("3"));
			Assert.IsTrue (et.CheckConstraint (-1));
			Assert.IsTrue (et.CheckConstraint ("Extra"));
			Assert.IsFalse (et.CheckConstraint (18));
			Assert.IsFalse (et.CheckConstraint ("{Other}"));
		}
		
		[Test] public void CheckEnumType2()
		{
			System.Type type = typeof (MyFlags);
			
			EnumType et = new EnumType (type);
			
			Assert.AreEqual (5, et.Values.Length);
			Assert.IsFalse (et.IsCustomizable);
			Assert.IsTrue (et.IsDefinedAsFlags);
			
			Assert.AreEqual ("None",   et.Values[0].Name);
			Assert.AreEqual ("Flag1",  et.Values[1].Name);
			Assert.AreEqual ("Flag2", et.Values[2].Name);
			Assert.AreEqual ("Flag3",  et.Values[3].Name);
			Assert.AreEqual ("Flag4",  et.Values[4].Name);
			
			Assert.AreEqual (0, et["None"] .Rank);
			Assert.AreEqual (1, et["Flag1"].Rank);
			Assert.AreEqual (2, et["Flag2"].Rank);
			Assert.AreEqual (4, et["Flag3"].Rank);
			Assert.AreEqual (8, et["Flag4"].Rank);
			
			Assert.AreEqual ("None",  et[0].Name);
			Assert.AreEqual ("Flag1", et[1].Name);
			Assert.AreEqual ("Flag2", et[2].Name);
			Assert.AreEqual ("Flag3", et[4].Name);
			Assert.AreEqual ("Flag4", et[8].Name);
			
			Assert.IsTrue (et.CheckConstraint ("0"));
			Assert.IsTrue (et.CheckConstraint (0xf));
			Assert.IsTrue (et.CheckConstraint ("Flag1, Flag2"));
			Assert.IsFalse (et.CheckConstraint (0x18));
			Assert.IsFalse (et.CheckConstraint ("{Other}"));
		}
		
		[Test] public void CheckEnumType3()
		{
			System.Type type = typeof (MyEnum);
			
			EnumType et = new EnumType (type);
			
			et.DefineTextsFromResources ("Enums#MyEnum");
			
			Assert.AreEqual ("[res:Enums#MyEnum.capt]", et.Caption);
			Assert.AreEqual ("[res:Enums#MyEnum.desc]", et.Description);
			
			Assert.AreEqual ("[res:Enums#MyEnum.None.capt]",   et["None"].Caption);
			Assert.AreEqual ("[res:Enums#MyEnum.First.capt]",  et["First"].Caption);
			Assert.AreEqual ("[res:Enums#MyEnum.Second.capt]", et["Second"].Caption);
			Assert.AreEqual ("[res:Enums#MyEnum.Third.capt]",  et["Third"].Caption);
			Assert.AreEqual ("[res:Enums#MyEnum.Extra.capt]",  et["Extra"].Caption);
			
			Assert.AreEqual ("[res:Enums#MyEnum.None.desc]",   et["None"].Description);
			Assert.AreEqual ("[res:Enums#MyEnum.First.desc]",  et["First"].Description);
			Assert.AreEqual ("[res:Enums#MyEnum.Second.desc]", et["Second"].Description);
			Assert.AreEqual ("[res:Enums#MyEnum.Third.desc]",  et["Third"].Description);
			Assert.AreEqual ("[res:Enums#MyEnum.Extra.desc]",  et["Extra"].Description);
		}
		
		[Test] public void CheckOpenEnumType()
		{
			EnumType et = new OpenEnumType (typeof (MyEnum));
			
			Assert.AreEqual (5, et.Values.Length);
			Assert.IsTrue (et.IsCustomizable);
			
			Assert.AreEqual ("None",   et.Values[0].Name);
			Assert.AreEqual ("First",  et.Values[1].Name);
			Assert.AreEqual ("Second", et.Values[2].Name);
			Assert.AreEqual ("Third",  et.Values[3].Name);
			Assert.AreEqual ("Extra",  et.Values[4].Name);
			
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
			
			Assert.IsTrue (et.CheckConstraint ("3"));
			Assert.IsTrue (et.CheckConstraint (-1));
			Assert.IsTrue (et.CheckConstraint ("Extra"));
			Assert.IsFalse (et.CheckConstraint (18));
			Assert.IsTrue (et.CheckConstraint ("{Other}"));
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

		private enum MyEnum
		{
			None	= -1,
			First	=  1,
			Second	=  2,
			Third	=  3,
			
			[Hide] Extra	= 99
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
