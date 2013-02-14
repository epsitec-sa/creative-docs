using Epsitec.Common.Types;
using Epsitec.Common.UnitTesting;

using Epsitec.Cresus.Database;

using Epsitec.Cresus.DataLayer.Context;
using Epsitec.Cresus.DataLayer.Infrastructure;
using Epsitec.Cresus.DataLayer.Schema;
using Epsitec.Cresus.DataLayer.Tests.Vs.Helpers;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.Collections.Generic;


namespace Epsitec.Cresus.DataLayer.Tests.Vs.Schema
{


	[TestClass]
	public class UnitTestDataConverter
	{


		[ClassInitialize]
		public static void ClassInitialize(TestContext testContext)
		{
			TestHelper.Initialize ();

			DatabaseCreator2.ResetEmptyTestDatabase ();
		}


		[TestMethod]
		public void DataConverterConstructorTest()
		{
			using (DB db = DB.ConnectToTestDatabase ())
			using (DataContext dataContext = DataContextHelper.ConnectToTestDatabase (db.DataInfrastructure))
			{
				new DataConverter (dataContext);
			}
		}


		[TestMethod]
		public void DataConverterConstructorArgumentCheck()
		{
			ExceptionAssert.Throw<System.ArgumentNullException>
			(
				() => new DataConverter (null)
			);
		}


		[TestMethod]
		public void FromCresusToDatabaseTypeTest()
		{
			using (DB db = DB.ConnectToTestDatabase ())
			using (DataContext dataContext = DataContextHelper.ConnectToTestDatabase (db.DataInfrastructure))
			{
				DataConverter dataConverter = new DataConverter (dataContext);

				foreach (var sample in this.GetSampleRawTypes ())
				{
					DbRawType result1 = sample.Item2;
					DbRawType result2 = dataConverter.FromDotNetToDatabaseType (sample.Item1);

					Assert.AreEqual (result1, result2);
				}
			}
		}


		[TestMethod]
		public void FromCresusToDatabaseValueArgumentCheck()
		{
			using (DB db = DB.ConnectToTestDatabase ())
			using (DataContext dataContext = DataContextHelper.ConnectToTestDatabase (db.DataInfrastructure))
			{
				DataConverter dataConverter = new DataConverter (dataContext);

				DbRawType rawType = DbRawType.Boolean;
				DbSimpleType simpleType = DbSimpleType.Decimal;
				DbNumDef numDef = DbNumDef.FromRawType (DbRawType.Boolean);
				object value = true;

				ExceptionAssert.Throw<System.ArgumentException>
				(
					() => dataConverter.FromCresusToDatabaseValue (DbRawType.Null, simpleType, numDef, value)
				);

				ExceptionAssert.Throw<System.ArgumentException>
				(
					() => dataConverter.FromCresusToDatabaseValue (DbRawType.Unknown, simpleType, numDef, value)
				);

				ExceptionAssert.Throw<System.ArgumentException>
				(
					() => dataConverter.FromCresusToDatabaseValue (rawType, DbSimpleType.Null, numDef, value)
				);

				ExceptionAssert.Throw<System.ArgumentException>
				(
					() => dataConverter.FromCresusToDatabaseValue (rawType, DbSimpleType.Unknown, numDef, value)
				);

				ExceptionAssert.Throw<System.ArgumentNullException>
				(
					() => dataConverter.FromCresusToDatabaseValue (rawType, simpleType, numDef, null)
				);

				ExceptionAssert.Throw<System.ArgumentException>
				(
					() => dataConverter.FromCresusToDatabaseValue (rawType, simpleType, this.GetInvalidNumDef (), value)
				);
			}
		}


		[TestMethod]
		public void FromCresusToDatabaseValueTest()
		{
			using (DB db = DB.ConnectToTestDatabase ())
			using (DataContext dataContext = DataContextHelper.ConnectToTestDatabase (db.DataInfrastructure))
			{
				DataConverter dataConverter = new DataConverter (dataContext);

				foreach (var sample in this.GetSampleValues1 ())
				{
					object result1 = sample.Item5;
					object result2 = dataConverter.FromCresusToDatabaseValue (sample.Item1, sample.Item2, sample.Item3, sample.Item4);

					if (result1 is byte[] && result2 is byte[])
					{
						byte[] r1 = (byte[]) result1;
						byte[] r2 = (byte[]) result2;

						CollectionAssert.AreEquivalent (r1, r2);
					}
					else
					{
						Assert.AreEqual (result1, result2);
					}
				}
			}
		}


		[TestMethod]
		public void FromDatabaseToCresusValueArgumentCheck()
		{
			using (DB db = DB.ConnectToTestDatabase ())
			using (DataContext dataContext = DataContextHelper.ConnectToTestDatabase (db.DataInfrastructure))
			{
				DataConverter dataConverter = new DataConverter (dataContext);

				INamedType type = new BooleanType ();
				DbRawType rawType = DbRawType.Boolean;
				DbSimpleType simpleType = DbSimpleType.Decimal;
				DbNumDef numDef = DbNumDef.FromRawType (DbRawType.Boolean);
				object value = true;

				ExceptionAssert.Throw<System.ArgumentException>
				(
					() => dataConverter.FromDatabaseToCresusValue (null, rawType, simpleType, numDef, value)
				);

				ExceptionAssert.Throw<System.ArgumentException>
				(
					() => dataConverter.FromDatabaseToCresusValue (type, DbRawType.Null, simpleType, numDef, value)
				);

				ExceptionAssert.Throw<System.ArgumentException>
				(
					() => dataConverter.FromDatabaseToCresusValue (type, DbRawType.Unknown, simpleType, numDef, value)
				);

				ExceptionAssert.Throw<System.ArgumentException>
				(
					() => dataConverter.FromDatabaseToCresusValue (type, rawType, DbSimpleType.Null, numDef, value)
				);

				ExceptionAssert.Throw<System.ArgumentException>
				(
					() => dataConverter.FromDatabaseToCresusValue (type, rawType, DbSimpleType.Unknown, numDef, value)
				);

				ExceptionAssert.Throw<System.ArgumentNullException>
				(
					() => dataConverter.FromDatabaseToCresusValue (type, rawType, simpleType, numDef, null)
				);

				ExceptionAssert.Throw<System.ArgumentException>
				(
					() => dataConverter.FromDatabaseToCresusValue (type, rawType, simpleType, this.GetInvalidNumDef (), value)
				);
			}
		}


		[TestMethod]
		public void FromDatabaseToCresusValueTest()
		{
			using (DB db = DB.ConnectToTestDatabase ())
			using (DataContext dataContext = DataContextHelper.ConnectToTestDatabase (db.DataInfrastructure))
			{
				DataConverter dataConverter = new DataConverter (dataContext);

				foreach (var sample in this.GetSampleValues2 ())
				{
					object result1 = sample.Item6;
					object result2 = dataConverter.FromDatabaseToCresusValue (sample.Item1, sample.Item2, sample.Item3, sample.Item4, sample.Item5);

					if (result1 is byte[] && result2 is byte[])
					{
						byte[] r1 = (byte[]) result1;
						byte[] r2 = (byte[]) result2;

						CollectionAssert.AreEquivalent (r1, r2);
					}
					else
					{
						Assert.AreEqual (result1, result2);
					}
				}
			}
		}


		private IEnumerable<System.Tuple<DbRawType, DbRawType>> GetSampleRawTypes()
		{
			yield return System.Tuple.Create (DbRawType.Boolean,		DbRawType.Int16);
			yield return System.Tuple.Create (DbRawType.Int16,			DbRawType.Int16);
			yield return System.Tuple.Create (DbRawType.Int32,			DbRawType.Int32);
			yield return System.Tuple.Create (DbRawType.Int64,			DbRawType.Int64);
			yield return System.Tuple.Create (DbRawType.SmallDecimal,	DbRawType.SmallDecimal);
			yield return System.Tuple.Create (DbRawType.LargeDecimal,	DbRawType.LargeDecimal);
			yield return System.Tuple.Create (DbRawType.String,			DbRawType.String);
			yield return System.Tuple.Create (DbRawType.Time,			DbRawType.Time);
			yield return System.Tuple.Create (DbRawType.Date,			DbRawType.Date);
			yield return System.Tuple.Create (DbRawType.DateTime,		DbRawType.DateTime);
			yield return System.Tuple.Create (DbRawType.Guid,			DbRawType.String);
			yield return System.Tuple.Create (DbRawType.ByteArray,		DbRawType.ByteArray);
		}


		private IEnumerable<System.Tuple<DbRawType, DbSimpleType, DbNumDef, object, object>> GetSampleValues1()
		{
			yield return System.Tuple.Create (DbRawType.Boolean, DbSimpleType.Decimal, DbNumDef.FromRawType (DbRawType.Boolean), (object) System.DBNull.Value, (object) System.DBNull.Value);
			yield return System.Tuple.Create (DbRawType.Boolean, DbSimpleType.Decimal, DbNumDef.FromRawType (DbRawType.Boolean), (object) true, (object) (short) 1);
			yield return System.Tuple.Create (DbRawType.Boolean, DbSimpleType.Decimal, DbNumDef.FromRawType (DbRawType.Boolean), (object) false, (object) (short) 0);

			yield return System.Tuple.Create (DbRawType.Int16, DbSimpleType.Decimal, DbNumDef.FromRawType (DbRawType.Int16), (object) System.DBNull.Value, (object) System.DBNull.Value);
			yield return System.Tuple.Create (DbRawType.Int16, DbSimpleType.Decimal, DbNumDef.FromRawType (DbRawType.Int16), (object) (short) 0, (object) (short) 0);
			yield return System.Tuple.Create (DbRawType.Int16, DbSimpleType.Decimal, DbNumDef.FromRawType (DbRawType.Int16), (object) (short) 1, (object) (short) 1);
			yield return System.Tuple.Create (DbRawType.Int16, DbSimpleType.Decimal, DbNumDef.FromRawType (DbRawType.Int16), (object) (short) 2, (object) (short) 2);

			yield return System.Tuple.Create (DbRawType.Int32, DbSimpleType.Decimal, DbNumDef.FromRawType (DbRawType.Int32), (object) System.DBNull.Value, (object) System.DBNull.Value);
			yield return System.Tuple.Create (DbRawType.Int32, DbSimpleType.Decimal, DbNumDef.FromRawType (DbRawType.Int32), (object) (int) 0, (object) (int) 0);
			yield return System.Tuple.Create (DbRawType.Int32, DbSimpleType.Decimal, DbNumDef.FromRawType (DbRawType.Int32), (object) (int) 1, (object) (int) 1);
			yield return System.Tuple.Create (DbRawType.Int32, DbSimpleType.Decimal, DbNumDef.FromRawType (DbRawType.Int32), (object) (int) 2, (object) (int) 2);

			yield return System.Tuple.Create (DbRawType.Int64, DbSimpleType.Decimal, DbNumDef.FromRawType (DbRawType.Int64), (object) System.DBNull.Value, (object) System.DBNull.Value);
			yield return System.Tuple.Create (DbRawType.Int64, DbSimpleType.Decimal, DbNumDef.FromRawType (DbRawType.Int64), (object) (long) 0, (object) (long) 0);
			yield return System.Tuple.Create (DbRawType.Int64, DbSimpleType.Decimal, DbNumDef.FromRawType (DbRawType.Int64), (object) (long) 1, (object) (long) 1);
			yield return System.Tuple.Create (DbRawType.Int64, DbSimpleType.Decimal, DbNumDef.FromRawType (DbRawType.Int64), (object) (long) 2, (object) (long) 2);

			yield return System.Tuple.Create (DbRawType.SmallDecimal, DbSimpleType.Decimal, DbNumDef.FromRawType (DbRawType.SmallDecimal), (object) System.DBNull.Value, (object) System.DBNull.Value);
			yield return System.Tuple.Create (DbRawType.SmallDecimal, DbSimpleType.Decimal, DbNumDef.FromRawType (DbRawType.SmallDecimal), (object) (decimal) 0, (object) (decimal) 0);
			yield return System.Tuple.Create (DbRawType.SmallDecimal, DbSimpleType.Decimal, DbNumDef.FromRawType (DbRawType.SmallDecimal), (object) (decimal) 1, (object) (decimal) 1);
			yield return System.Tuple.Create (DbRawType.SmallDecimal, DbSimpleType.Decimal, DbNumDef.FromRawType (DbRawType.SmallDecimal), (object) (decimal) 2, (object) (decimal) 2);

			yield return System.Tuple.Create (DbRawType.LargeDecimal, DbSimpleType.Decimal, DbNumDef.FromRawType (DbRawType.LargeDecimal), (object) System.DBNull.Value, (object) System.DBNull.Value);
			yield return System.Tuple.Create (DbRawType.LargeDecimal, DbSimpleType.Decimal, DbNumDef.FromRawType (DbRawType.LargeDecimal), (object) (decimal) 0, (object) (decimal) 0);
			yield return System.Tuple.Create (DbRawType.LargeDecimal, DbSimpleType.Decimal, DbNumDef.FromRawType (DbRawType.LargeDecimal), (object) (decimal) 1, (object) (decimal) 1);
			yield return System.Tuple.Create (DbRawType.LargeDecimal, DbSimpleType.Decimal, DbNumDef.FromRawType (DbRawType.LargeDecimal), (object) (decimal) 2, (object) (decimal) 2);

			yield return System.Tuple.Create (DbRawType.String, DbSimpleType.String, DbNumDef.FromRawType (DbRawType.String), (object) System.DBNull.Value, (object) System.DBNull.Value);
			yield return System.Tuple.Create (DbRawType.String, DbSimpleType.String, DbNumDef.FromRawType (DbRawType.String), (object) "test", (object) "test");
			yield return System.Tuple.Create (DbRawType.String, DbSimpleType.String, DbNumDef.FromRawType (DbRawType.String), (object) "&<>;.!üä£ö", (object) "&<>;.!üä£ö");

			Epsitec.Common.Types.Time time1 = new Epsitec.Common.Types.Time (0);
			Epsitec.Common.Types.Time time2 = new Epsitec.Common.Types.Time (1);
			Epsitec.Common.Types.Time time3 = new Epsitec.Common.Types.Time (2);

			yield return System.Tuple.Create (DbRawType.Time, DbSimpleType.Time, DbNumDef.FromRawType (DbRawType.Time), (object) System.DBNull.Value, (object) System.DBNull.Value);
			yield return System.Tuple.Create (DbRawType.Time, DbSimpleType.Time, DbNumDef.FromRawType (DbRawType.Time), (object) time1, (object) time1.ToDateTime ());
			yield return System.Tuple.Create (DbRawType.Time, DbSimpleType.Time, DbNumDef.FromRawType (DbRawType.Time), (object) time2, (object) time2.ToDateTime ());
			yield return System.Tuple.Create (DbRawType.Time, DbSimpleType.Time, DbNumDef.FromRawType (DbRawType.Time), (object) time3, (object) time3.ToDateTime ());

			Epsitec.Common.Types.Date date1 = new Epsitec.Common.Types.Date (1950, 12, 12);
			Epsitec.Common.Types.Date date2 = new Epsitec.Common.Types.Date (1964, 7, 5);
			Epsitec.Common.Types.Date date3 = new Epsitec.Common.Types.Date (1998, 1, 4);

			yield return System.Tuple.Create (DbRawType.Date, DbSimpleType.Date, DbNumDef.FromRawType (DbRawType.Date), (object) System.DBNull.Value, (object) System.DBNull.Value);
			yield return System.Tuple.Create (DbRawType.Date, DbSimpleType.Date, DbNumDef.FromRawType (DbRawType.Date), (object) date1, (object) date1.ToDateTime ());
			yield return System.Tuple.Create (DbRawType.Date, DbSimpleType.Date, DbNumDef.FromRawType (DbRawType.Date), (object) date2, (object) date2.ToDateTime ());
			yield return System.Tuple.Create (DbRawType.Date, DbSimpleType.Date, DbNumDef.FromRawType (DbRawType.Date), (object) date3, (object) date3.ToDateTime ());

			System.DateTime dateTime1 = new System.DateTime (7348923);
			System.DateTime dateTime2 = new System.DateTime (5423523);
			System.DateTime dateTime3 = new System.DateTime (5423542);

			yield return System.Tuple.Create (DbRawType.DateTime, DbSimpleType.DateTime, DbNumDef.FromRawType (DbRawType.DateTime), (object) System.DBNull.Value, (object) System.DBNull.Value);
			yield return System.Tuple.Create (DbRawType.DateTime, DbSimpleType.DateTime, DbNumDef.FromRawType (DbRawType.DateTime), (object) dateTime1, (object) dateTime1);
			yield return System.Tuple.Create (DbRawType.DateTime, DbSimpleType.DateTime, DbNumDef.FromRawType (DbRawType.DateTime), (object) dateTime2, (object) dateTime2);
			yield return System.Tuple.Create (DbRawType.DateTime, DbSimpleType.DateTime, DbNumDef.FromRawType (DbRawType.DateTime), (object) dateTime3, (object) dateTime3);

			System.Guid guid1 = System.Guid.NewGuid ();
			System.Guid guid2 = System.Guid.NewGuid ();
			System.Guid guid3 = System.Guid.NewGuid ();

			yield return System.Tuple.Create (DbRawType.Guid, DbSimpleType.Guid, DbNumDef.FromRawType (DbRawType.Guid), (object) System.DBNull.Value, (object) System.DBNull.Value);
			yield return System.Tuple.Create (DbRawType.Guid, DbSimpleType.Guid, DbNumDef.FromRawType (DbRawType.Guid), (object) guid1, (object) guid1.ToString ("N"));
			yield return System.Tuple.Create (DbRawType.Guid, DbSimpleType.Guid, DbNumDef.FromRawType (DbRawType.Guid), (object) guid2, (object) guid2.ToString ("N"));
			yield return System.Tuple.Create (DbRawType.Guid, DbSimpleType.Guid, DbNumDef.FromRawType (DbRawType.Guid), (object) guid3, (object) guid3.ToString ("N"));

			byte[] byteArray1 = System.Guid.NewGuid ().ToByteArray ();
			byte[] byteArray2 = System.Guid.NewGuid ().ToByteArray ();
			byte[] byteArray3 = System.Guid.NewGuid ().ToByteArray ();

			yield return System.Tuple.Create (DbRawType.ByteArray, DbSimpleType.ByteArray, DbNumDef.FromRawType (DbRawType.ByteArray), (object) System.DBNull.Value, (object) System.DBNull.Value);
			yield return System.Tuple.Create (DbRawType.ByteArray, DbSimpleType.ByteArray, DbNumDef.FromRawType (DbRawType.ByteArray), (object) byteArray1, (object) byteArray1);
			yield return System.Tuple.Create (DbRawType.ByteArray, DbSimpleType.ByteArray, DbNumDef.FromRawType (DbRawType.ByteArray), (object) byteArray2, (object) byteArray2);
			yield return System.Tuple.Create (DbRawType.ByteArray, DbSimpleType.ByteArray, DbNumDef.FromRawType (DbRawType.ByteArray), (object) byteArray3, (object) byteArray3);
		}


		private IEnumerable<System.Tuple<INamedType, DbRawType, DbSimpleType, DbNumDef, object, object>> GetSampleValues2()
		{
			yield return System.Tuple.Create ((INamedType) new BooleanType (), DbRawType.Boolean, DbSimpleType.Decimal, DbNumDef.FromRawType (DbRawType.Boolean), (object) System.DBNull.Value, (object) System.DBNull.Value);
			yield return System.Tuple.Create ((INamedType) new BooleanType (), DbRawType.Boolean, DbSimpleType.Decimal, DbNumDef.FromRawType (DbRawType.Boolean), (object) (short) 1, (object) true);
			yield return System.Tuple.Create ((INamedType) new BooleanType (), DbRawType.Boolean, DbSimpleType.Decimal, DbNumDef.FromRawType (DbRawType.Boolean), (object) (short) 0, (object) false);

			yield return System.Tuple.Create ((INamedType) new IntegerType (), DbRawType.Int32, DbSimpleType.Decimal, DbNumDef.FromRawType (DbRawType.Int32), (object) System.DBNull.Value, (object) System.DBNull.Value);
			yield return System.Tuple.Create ((INamedType) new IntegerType (), DbRawType.Int32, DbSimpleType.Decimal, DbNumDef.FromRawType (DbRawType.Int32), (object) (int) 0, (object) (int) 0);
			yield return System.Tuple.Create ((INamedType) new IntegerType (), DbRawType.Int32, DbSimpleType.Decimal, DbNumDef.FromRawType (DbRawType.Int32), (object) (int) 1, (object) (int) 1);
			yield return System.Tuple.Create ((INamedType) new IntegerType (), DbRawType.Int32, DbSimpleType.Decimal, DbNumDef.FromRawType (DbRawType.Int32), (object) (int) 2, (object) (int) 2);

			yield return System.Tuple.Create ((INamedType) new LongIntegerType (), DbRawType.Int64, DbSimpleType.Decimal, DbNumDef.FromRawType (DbRawType.Int64), (object) System.DBNull.Value, (object) System.DBNull.Value);
			yield return System.Tuple.Create ((INamedType) new LongIntegerType (), DbRawType.Int64, DbSimpleType.Decimal, DbNumDef.FromRawType (DbRawType.Int64), (object) (long) 0, (object) (long) 0);
			yield return System.Tuple.Create ((INamedType) new LongIntegerType (), DbRawType.Int64, DbSimpleType.Decimal, DbNumDef.FromRawType (DbRawType.Int64), (object) (long) 1, (object) (long) 1);
			yield return System.Tuple.Create ((INamedType) new LongIntegerType (), DbRawType.Int64, DbSimpleType.Decimal, DbNumDef.FromRawType (DbRawType.Int64), (object) (long) 2, (object) (long) 2);

			yield return System.Tuple.Create ((INamedType) new DecimalType (), DbRawType.SmallDecimal, DbSimpleType.Decimal, DbNumDef.FromRawType (DbRawType.SmallDecimal), (object) System.DBNull.Value, (object) System.DBNull.Value);
			yield return System.Tuple.Create ((INamedType) new DecimalType (), DbRawType.SmallDecimal, DbSimpleType.Decimal, DbNumDef.FromRawType (DbRawType.SmallDecimal), (object) (decimal) 0, (object) (decimal) 0);
			yield return System.Tuple.Create ((INamedType) new DecimalType (), DbRawType.SmallDecimal, DbSimpleType.Decimal, DbNumDef.FromRawType (DbRawType.SmallDecimal), (object) (decimal) 1, (object) (decimal) 1);
			yield return System.Tuple.Create ((INamedType) new DecimalType (), DbRawType.SmallDecimal, DbSimpleType.Decimal, DbNumDef.FromRawType (DbRawType.SmallDecimal), (object) (decimal) 2, (object) (decimal) 2);

			yield return System.Tuple.Create ((INamedType) new DecimalType (), DbRawType.LargeDecimal, DbSimpleType.Decimal, DbNumDef.FromRawType (DbRawType.LargeDecimal), (object) System.DBNull.Value, (object) System.DBNull.Value);
			yield return System.Tuple.Create ((INamedType) new DecimalType (), DbRawType.LargeDecimal, DbSimpleType.Decimal, DbNumDef.FromRawType (DbRawType.LargeDecimal), (object) (decimal) 0, (object) (decimal) 0);
			yield return System.Tuple.Create ((INamedType) new DecimalType (), DbRawType.LargeDecimal, DbSimpleType.Decimal, DbNumDef.FromRawType (DbRawType.LargeDecimal), (object) (decimal) 1, (object) (decimal) 1);
			yield return System.Tuple.Create ((INamedType) new DecimalType (), DbRawType.LargeDecimal, DbSimpleType.Decimal, DbNumDef.FromRawType (DbRawType.LargeDecimal), (object) (decimal) 2, (object) (decimal) 2);

			yield return System.Tuple.Create ((INamedType) new StringType (), DbRawType.String, DbSimpleType.String, DbNumDef.FromRawType (DbRawType.String), (object) System.DBNull.Value, (object) System.DBNull.Value);
			yield return System.Tuple.Create ((INamedType) new StringType (), DbRawType.String, DbSimpleType.String, DbNumDef.FromRawType (DbRawType.String), (object) "test", (object) "test");
			yield return System.Tuple.Create ((INamedType) new StringType (), DbRawType.String, DbSimpleType.String, DbNumDef.FromRawType (DbRawType.String), (object) "&<>;.!üä£ö", (object) "&<>;.!üä£ö");

			Epsitec.Common.Types.Time time1 = new Epsitec.Common.Types.Time (0);
			Epsitec.Common.Types.Time time2 = new Epsitec.Common.Types.Time (1);
			Epsitec.Common.Types.Time time3 = new Epsitec.Common.Types.Time (2);

			yield return System.Tuple.Create ((INamedType) new TimeType (), DbRawType.Time, DbSimpleType.Time, DbNumDef.FromRawType (DbRawType.Time), (object) System.DBNull.Value, (object) System.DBNull.Value);
			yield return System.Tuple.Create ((INamedType) new TimeType (), DbRawType.Time, DbSimpleType.Time, DbNumDef.FromRawType (DbRawType.Time), (object) time1.ToDateTime (), (object) time1);
			yield return System.Tuple.Create ((INamedType) new TimeType (), DbRawType.Time, DbSimpleType.Time, DbNumDef.FromRawType (DbRawType.Time), (object) time2.ToDateTime (), (object) time2);
			yield return System.Tuple.Create ((INamedType) new TimeType (), DbRawType.Time, DbSimpleType.Time, DbNumDef.FromRawType (DbRawType.Time), (object) time3.ToDateTime (), (object) time3);

			Epsitec.Common.Types.Date date1 = new Epsitec.Common.Types.Date (1950, 12, 12);
			Epsitec.Common.Types.Date date2 = new Epsitec.Common.Types.Date (1964, 7, 5);
			Epsitec.Common.Types.Date date3 = new Epsitec.Common.Types.Date (1998, 1, 4);

			yield return System.Tuple.Create ((INamedType) new DateType (), DbRawType.Date, DbSimpleType.Date, DbNumDef.FromRawType (DbRawType.Date), (object) System.DBNull.Value, (object) System.DBNull.Value);
			yield return System.Tuple.Create ((INamedType) new DateType (), DbRawType.Date, DbSimpleType.Date, DbNumDef.FromRawType (DbRawType.Date), (object) date1.ToDateTime (), (object) date1);
			yield return System.Tuple.Create ((INamedType) new DateType (), DbRawType.Date, DbSimpleType.Date, DbNumDef.FromRawType (DbRawType.Date), (object) date2.ToDateTime (), (object) date2);
			yield return System.Tuple.Create ((INamedType) new DateType (), DbRawType.Date, DbSimpleType.Date, DbNumDef.FromRawType (DbRawType.Date), (object) date3.ToDateTime (), (object) date3);

			System.DateTime dateTime1 = new System.DateTime (7348923);
			System.DateTime dateTime2 = new System.DateTime (5423523);
			System.DateTime dateTime3 = new System.DateTime (5423542);

			yield return System.Tuple.Create ((INamedType) new DateTimeType (), DbRawType.DateTime, DbSimpleType.DateTime, DbNumDef.FromRawType (DbRawType.DateTime), (object) System.DBNull.Value, (object) System.DBNull.Value);
			yield return System.Tuple.Create ((INamedType) new DateTimeType (), DbRawType.DateTime, DbSimpleType.DateTime, DbNumDef.FromRawType (DbRawType.DateTime), (object) dateTime1, (object) dateTime1);
			yield return System.Tuple.Create ((INamedType) new DateTimeType (), DbRawType.DateTime, DbSimpleType.DateTime, DbNumDef.FromRawType (DbRawType.DateTime), (object) dateTime2, (object) dateTime2);
			yield return System.Tuple.Create ((INamedType) new DateTimeType (), DbRawType.DateTime, DbSimpleType.DateTime, DbNumDef.FromRawType (DbRawType.DateTime), (object) dateTime3, (object) dateTime3);

			byte[] byteArray1 = System.Guid.NewGuid ().ToByteArray ();
			byte[] byteArray2 = System.Guid.NewGuid ().ToByteArray ();
			byte[] byteArray3 = System.Guid.NewGuid ().ToByteArray ();

			yield return System.Tuple.Create ((INamedType) new BinaryType (), DbRawType.ByteArray, DbSimpleType.ByteArray, DbNumDef.FromRawType (DbRawType.ByteArray), (object) System.DBNull.Value, (object) System.DBNull.Value);
			yield return System.Tuple.Create ((INamedType) new BinaryType (), DbRawType.ByteArray, DbSimpleType.ByteArray, DbNumDef.FromRawType (DbRawType.ByteArray), (object) byteArray1, (object) byteArray1);
			yield return System.Tuple.Create ((INamedType) new BinaryType (), DbRawType.ByteArray, DbSimpleType.ByteArray, DbNumDef.FromRawType (DbRawType.ByteArray), (object) byteArray2, (object) byteArray2);
			yield return System.Tuple.Create ((INamedType) new BinaryType (), DbRawType.ByteArray, DbSimpleType.ByteArray, DbNumDef.FromRawType (DbRawType.ByteArray), (object) byteArray3, (object) byteArray3);

			yield break;
		}


		private DbNumDef GetInvalidNumDef()
		{
			return new DbNumDef (1, 0)
			{
				MinValue = -1,
				MaxValue = 99
			};
		}


	}


}
