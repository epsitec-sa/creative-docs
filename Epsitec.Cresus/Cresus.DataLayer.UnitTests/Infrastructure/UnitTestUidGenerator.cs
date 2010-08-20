using Epsitec.Common.UnitTesting;

using Epsitec.Cresus.Database;

using Epsitec.Cresus.DataLayer.Infrastructure;
using Epsitec.Cresus.DataLayer.UnitTests.Helpers;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.Collections.Generic;

using System.Linq;


namespace Epsitec.Cresus.DataLayer.UnitTests.Infrastructure
{


	[TestClass]
	public sealed class UnitTestUidGenerator
	{


		[ClassInitialize]
		public static void ClassInitialize(TestContext testContext)
		{
			TestHelper.Initialize ();
		}


		[TestInitialize]
		public void TestInitialize()
		{
			DatabaseHelper.CreateAndConnectToDatabase ();
		}


		[TestCleanup]
		public static void TestCleanup()
		{
			DatabaseHelper.DisconnectFromDatabase ();
		}


		[TestMethod]
		public void CreateUidGeneratorArgumentCheck()
		{
			DbInfrastructure dbInfrastructure = DatabaseHelper.DbInfrastructure;
			string name = "test";
			List<System.Tuple<long, long>> slots = new List<System.Tuple<long, long>> ()
			{
				System.Tuple.Create ((long)  0, (long)  9),
				System.Tuple.Create ((long) 20, (long) 29),
				System.Tuple.Create ((long) 10, (long) 19),
			};

			ExceptionAssert.Throw<System.ArgumentNullException>
			(
				() => UidGenerator.CreateUidGenerator (null, name, slots)
			);

			ExceptionAssert.Throw<System.ArgumentException>
			(
				() => UidGenerator.CreateUidGenerator (dbInfrastructure, null, slots)
			);

			ExceptionAssert.Throw<System.ArgumentException>
			(
				() => UidGenerator.CreateUidGenerator (dbInfrastructure, "", slots)
			);

			ExceptionAssert.Throw<System.ArgumentNullException>
			(
				() => UidGenerator.CreateUidGenerator (dbInfrastructure, name, null)
			);

			List<System.Tuple<long,long>> badSlots = new List<System.Tuple<long, long>> ();

			ExceptionAssert.Throw<System.ArgumentException>
			(
				() => UidGenerator.CreateUidGenerator (dbInfrastructure, name, badSlots)
			);

			badSlots = new List<System.Tuple<long, long>> ()
			{
				System.Tuple.Create ((long) -1, (long) 0),
			};

			ExceptionAssert.Throw<System.ArgumentException>
			(
				() => UidGenerator.CreateUidGenerator (dbInfrastructure, name, badSlots)
			);

			badSlots = new List<System.Tuple<long, long>> ()
			{
				System.Tuple.Create ((long) 0, (long) -1),
			};

			ExceptionAssert.Throw<System.ArgumentException>
			(
				() => UidGenerator.CreateUidGenerator (dbInfrastructure, name, badSlots)
			);

			badSlots = new List<System.Tuple<long, long>> ()
			{
				System.Tuple.Create ((long) 10, (long) 5),
			};

			ExceptionAssert.Throw<System.ArgumentException>
			(
				() => UidGenerator.CreateUidGenerator (dbInfrastructure, name, badSlots)
			);

			badSlots = new List<System.Tuple<long, long>> ()
			{
				System.Tuple.Create ((long) 0, (long) 5),
				System.Tuple.Create ((long) 5, (long) 10),
			};

			ExceptionAssert.Throw<System.ArgumentException>
			(
				() => UidGenerator.CreateUidGenerator (dbInfrastructure, name, badSlots)
			);
		}


		[TestMethod]
		public void CreateUidGeneratorTest()
		{
			for (int i = 0; i < 10; i++)
			{
				string name = "myCounter" + i;
				
				List<System.Tuple<long, long>> slots = new List<System.Tuple<long, long>> ()
				{
					System.Tuple.Create ((long) 10, (long) 19),
					System.Tuple.Create ((long) 20, (long) 29),
					System.Tuple.Create ((long)  0, (long)  9),
				};

				Assert.IsFalse (UidGenerator.UidGeneratorExists (DatabaseHelper.DbInfrastructure, name));

				UidGenerator.CreateUidGenerator (DatabaseHelper.DbInfrastructure, name, slots);

				Assert.IsTrue (UidGenerator.UidGeneratorExists (DatabaseHelper.DbInfrastructure, name));	
			}
		}


		[TestMethod]
		public void DeleteUidGeneratorArgumentCheck()
		{
			DbInfrastructure dbInfrastructure = DatabaseHelper.DbInfrastructure;
			string name = "test";

			ExceptionAssert.Throw<System.ArgumentException>
			(
				() => UidGenerator.DeleteUidGenerator (null, name)
			);

			ExceptionAssert.Throw<System.ArgumentException>
			(
				() => UidGenerator.DeleteUidGenerator (dbInfrastructure, null)
			);

			ExceptionAssert.Throw<System.ArgumentException>
			(
				() => UidGenerator.DeleteUidGenerator (dbInfrastructure, "")
			);
		}


		[TestMethod]
		public void DeleteUidGeneratorTest()
		{
			List<int> countersRemoved = new List<int> ();
			List<int> countersCreated = new List<int> ();

			for (int i = 0; i < 10; i++)
			{
				countersRemoved.Add (i);
			}

			for (int i = 0; i < 10; i++)
			{
				string name = "myCounter";

				List<System.Tuple<long, long>> slots = new List<System.Tuple<long, long>> ()
				{
					System.Tuple.Create ((long)  0, (long)  9),
					System.Tuple.Create ((long) 20, (long) 29),
					System.Tuple.Create ((long) 10, (long) 19),
				};

				UidGenerator.CreateUidGenerator (DatabaseHelper.DbInfrastructure, name + i, slots);

				countersCreated.Add (i);
				countersRemoved.Remove (i);

				foreach (int j in countersCreated)
				{
					Assert.IsTrue (UidGenerator.UidGeneratorExists (DatabaseHelper.DbInfrastructure, name + j));
				}

				foreach (int j in countersRemoved)
				{
					Assert.IsFalse (UidGenerator.UidGeneratorExists (DatabaseHelper.DbInfrastructure, name + j));
				}
			}

			for (int i = 0; i < 10; i++)
			{
				string name = "myCounter";

				UidGenerator.DeleteUidGenerator (DatabaseHelper.DbInfrastructure, name + i);

				countersCreated.Remove (i);
				countersRemoved.Add (i);

				foreach (int j in countersCreated)
				{
					Assert.IsTrue (UidGenerator.UidGeneratorExists (DatabaseHelper.DbInfrastructure, name + j));
				}

				foreach (int j in countersRemoved)
				{
					Assert.IsFalse (UidGenerator.UidGeneratorExists (DatabaseHelper.DbInfrastructure, name + j));
				}
			}
		}


		[TestMethod]
		public void UidGeneratorExistsArgumentCheck()
		{
			DbInfrastructure dbInfrastructure = DatabaseHelper.DbInfrastructure;
			string name = "test";

			ExceptionAssert.Throw<System.ArgumentException>
			(
				() => UidGenerator.UidGeneratorExists (null, name)
			);

			ExceptionAssert.Throw<System.ArgumentException>
			(
				() => UidGenerator.UidGeneratorExists (dbInfrastructure, null)
			);

			ExceptionAssert.Throw<System.ArgumentException>
			(
				() => UidGenerator.UidGeneratorExists (dbInfrastructure, "")
			);
		}


		[TestMethod]
		public void GetUidGeneratorArgumentCheck()
		{
			DbInfrastructure dbInfrastructure = DatabaseHelper.DbInfrastructure;
			string name = "test";

			ExceptionAssert.Throw<System.ArgumentException>
			(
				() => UidGenerator.GetUidGenerator (null, name)
			);

			ExceptionAssert.Throw<System.ArgumentException>
			(
				() => UidGenerator.GetUidGenerator (dbInfrastructure, null)
			);

			ExceptionAssert.Throw<System.ArgumentException>
			(
				() => UidGenerator.GetUidGenerator (dbInfrastructure, "")
			);
		}


		[TestMethod]
		public void GetUidGeneratorTest()
		{
			for (int i = 0; i < 10; i++)
			{
				string name = "myCounter" + i;

				List<System.Tuple<long, long>> slots = new List<System.Tuple<long, long>> ()
				{
					System.Tuple.Create ((long) 20, (long) 29),
					System.Tuple.Create ((long) 0, (long) 9),
					System.Tuple.Create ((long) 10, (long) 19),
				};

				Assert.IsFalse (UidGenerator.UidGeneratorExists (DatabaseHelper.DbInfrastructure, name));

				UidGenerator.CreateUidGenerator (DatabaseHelper.DbInfrastructure, name, slots);

				Assert.IsTrue (UidGenerator.UidGeneratorExists (DatabaseHelper.DbInfrastructure, name));
			}

			for (int i = 0; i < 10; i++)
			{
				string name = "myCounter" + i;

				UidGenerator generator = UidGenerator.GetUidGenerator (DatabaseHelper.DbInfrastructure, name);

				Assert.IsNotNull (generator);
				Assert.AreEqual (generator.Name, name);
				Assert.AreEqual (generator.Slots.Count (), 3);
				Assert.IsTrue (generator.Slots.Any (s => s.Item1 == 0 && s.Item2 == 9));
				Assert.IsTrue (generator.Slots.Any (s => s.Item1 == 10 && s.Item2 == 19));
				Assert.IsTrue (generator.Slots.Any (s => s.Item1 == 20 && s.Item2 == 29));
			}
		}


		[TestMethod]
		public void GetNextUidTest1()
		{
			string name = "myCounter";

			List<System.Tuple<long, long>> slots = new List<System.Tuple<long, long>> ()
			{
				System.Tuple.Create ((long) 0, (long) 9),
			};

			UidGenerator.CreateUidGenerator (DatabaseHelper.DbInfrastructure, name, slots);

			UidGenerator generator = UidGenerator.GetUidGenerator (DatabaseHelper.DbInfrastructure, name);

			for (int i = 0; i <= 9; i++)
			{
				Assert.AreEqual (i, generator.GetNextUid ());
			}

			ExceptionAssert.Throw<System.Exception> (() => generator.GetNextUid ());
		}


		[TestMethod]
		public void GetNextUidTest2()
		{
			string name = "myCounter";

			List<System.Tuple<long, long>> slots = new List<System.Tuple<long, long>> ()
			{
				System.Tuple.Create ((long) 15, (long) 15),
				System.Tuple.Create ((long) 19, (long) 34),
				System.Tuple.Create ((long) 0, (long) 9),
			};

			UidGenerator.CreateUidGenerator (DatabaseHelper.DbInfrastructure, name, slots);

			UidGenerator generator = UidGenerator.GetUidGenerator (DatabaseHelper.DbInfrastructure, name);

			for (int i = 0; i <= 9; i++)
			{
				Assert.AreEqual (i, generator.GetNextUid ());
			}

			for (int i = 15; i <= 15; i++)
			{
				Assert.AreEqual (i, generator.GetNextUid ());
			}

			for (int i = 19; i <= 34; i++)
			{
				Assert.AreEqual (i, generator.GetNextUid ());
			}

			ExceptionAssert.Throw<System.Exception> (() => generator.GetNextUid ());
		}


		[TestMethod]
		public void GetNextUidInSlotArgumentCheck()
		{
			string name = "myCounter";

			List<System.Tuple<long, long>> slots = new List<System.Tuple<long, long>> ()
			{
				System.Tuple.Create ((long) 0, (long) 9),
			};

			UidGenerator.CreateUidGenerator (DatabaseHelper.DbInfrastructure, name, slots);

			UidGenerator generator = UidGenerator.GetUidGenerator (DatabaseHelper.DbInfrastructure, name);

			ExceptionAssert.Throw<System.Exception>
			(
				() => generator.GetNextUidInSlot (-1)
			);

			ExceptionAssert.Throw<System.Exception>
			(
				() => generator.GetNextUidInSlot (2)
			);
		}


		[TestMethod]
		public void GetNextUidInSlotTest()
		{
			string name = "myCounter";

			List<System.Tuple<long, long>> slots = new List<System.Tuple<long, long>> ()
			{
				System.Tuple.Create ((long) 12, (long) 17),
				System.Tuple.Create ((long) 0, (long) 9),
				System.Tuple.Create ((long) 19, (long) 34),
			};

			UidGenerator.CreateUidGenerator (DatabaseHelper.DbInfrastructure, name, slots);

			UidGenerator generator = UidGenerator.GetUidGenerator (DatabaseHelper.DbInfrastructure, name);

			for (int i = 12; i <= 17; i++)
			{
				Assert.AreEqual (i, generator.GetNextUidInSlot (1));
			}

			ExceptionAssert.Throw<System.Exception> (() => generator.GetNextUidInSlot (1));
		}


	}


}
