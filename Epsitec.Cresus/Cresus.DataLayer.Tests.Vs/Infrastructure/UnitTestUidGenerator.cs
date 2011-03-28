using Epsitec.Common.UnitTesting;

using Epsitec.Cresus.Database;

using Epsitec.Cresus.DataLayer.Infrastructure;
using Epsitec.Cresus.DataLayer.Schema;
using Epsitec.Cresus.DataLayer.Tests.Vs.Helpers;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.Collections.Generic;

using System.Linq;


namespace Epsitec.Cresus.DataLayer.Tests.Vs.Infrastructure
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
			DbInfrastructureHelper.ResetTestDatabase ();
		}


		[TestMethod]
		public void CreateUidGeneratorArgumentCheck()
		{
			using (DbInfrastructure dbInfrastructure = DbInfrastructureHelper.ConnectToTestDatabase ())
			{
				string name = "test";
				List<UidSlot> slots = new List<UidSlot> ()
				{
					new UidSlot (0, 9),
					new UidSlot (20, 29),
					new UidSlot (10, 19),
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

				List<UidSlot> badSlots = new List<UidSlot> ();

				ExceptionAssert.Throw<System.ArgumentException>
				(
					() => UidGenerator.CreateUidGenerator (dbInfrastructure, name, badSlots)
				);

				badSlots = new List<UidSlot> ()
				{
					new UidSlot (0, 5),
					new UidSlot (5, 10),
				};

				ExceptionAssert.Throw<System.ArgumentException>
				(
					() => UidGenerator.CreateUidGenerator (dbInfrastructure, name, badSlots)
				);
			}
		}


		[TestMethod]
		public void CreateUidGeneratorTest()
		{
			using (DbInfrastructure dbInfrastructure = DbInfrastructureHelper.ConnectToTestDatabase ())
			{
				for (int i = 0; i < 10; i++)
				{
					string name = "myCounter" + i;

					List<UidSlot> slots = new List<UidSlot> ()
				{
					new UidSlot (10, 19),
					new UidSlot (20, 29),
					new UidSlot (0, 9),
				};

					Assert.IsFalse (UidGenerator.UidGeneratorExists (dbInfrastructure, name));

					UidGenerator.CreateUidGenerator (dbInfrastructure, name, slots);

					Assert.IsTrue (UidGenerator.UidGeneratorExists (dbInfrastructure, name));
				}
			}
		}


		[TestMethod]
		public void DeleteUidGeneratorArgumentCheck()
		{
			using (DbInfrastructure dbInfrastructure = DbInfrastructureHelper.ConnectToTestDatabase ())
			{
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
		}


		[TestMethod]
		public void DeleteUidGeneratorTest()
		{
			using (DbInfrastructure dbInfrastructure = DbInfrastructureHelper.ConnectToTestDatabase ())
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

					List<UidSlot> slots = new List<UidSlot> ()
					{
						new UidSlot (0, 9),
						new UidSlot (20, 29),
						new UidSlot (10, 19),
					};

					UidGenerator.CreateUidGenerator (dbInfrastructure, name + i, slots);

					countersCreated.Add (i);
					countersRemoved.Remove (i);

					foreach (int j in countersCreated)
					{
						Assert.IsTrue (UidGenerator.UidGeneratorExists (dbInfrastructure, name + j));
					}

					foreach (int j in countersRemoved)
					{
						Assert.IsFalse (UidGenerator.UidGeneratorExists (dbInfrastructure, name + j));
					}
				}

				for (int i = 0; i < 10; i++)
				{
					string name = "myCounter";

					UidGenerator.DeleteUidGenerator (dbInfrastructure, name + i);

					countersCreated.Remove (i);
					countersRemoved.Add (i);

					foreach (int j in countersCreated)
					{
						Assert.IsTrue (UidGenerator.UidGeneratorExists (dbInfrastructure, name + j));
					}

					foreach (int j in countersRemoved)
					{
						Assert.IsFalse (UidGenerator.UidGeneratorExists (dbInfrastructure, name + j));
					}
				}
			}
		}


		[TestMethod]
		public void UidGeneratorExistsArgumentCheck()
		{
			using (DbInfrastructure dbInfrastructure = DbInfrastructureHelper.ConnectToTestDatabase ())
			{
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
		}


		[TestMethod]
		public void GetUidGeneratorArgumentCheck()
		{
			using (DbInfrastructure dbInfrastructure = DbInfrastructureHelper.ConnectToTestDatabase ())
			{
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
		}


		[TestMethod]
		public void GetUidGeneratorTest()
		{
			using (DbInfrastructure dbInfrastructure = DbInfrastructureHelper.ConnectToTestDatabase ())
			{
				for (int i = 0; i < 10; i++)
				{
					string name = "myCounter" + i;

					List<UidSlot> slots = new List<UidSlot> ()
					{
						new UidSlot (20, 29),
						new UidSlot (0, 9),
						new UidSlot (10, 19),
					};

					Assert.IsFalse (UidGenerator.UidGeneratorExists (dbInfrastructure, name));

					UidGenerator.CreateUidGenerator (dbInfrastructure, name, slots);

					Assert.IsTrue (UidGenerator.UidGeneratorExists (dbInfrastructure, name));
				}

				for (int i = 0; i < 10; i++)
				{
					string name = "myCounter" + i;

					UidGenerator generator = UidGenerator.GetUidGenerator (dbInfrastructure, name);

					Assert.IsNotNull (generator);
					Assert.AreEqual (generator.Name, name);
					Assert.AreEqual (generator.Slots.Count (), 3);
					Assert.AreEqual (0, generator.Slots.ElementAt (0).MinValue);
					Assert.AreEqual (9, generator.Slots.ElementAt (0).MaxValue);
					Assert.AreEqual (10, generator.Slots.ElementAt (1).MinValue);
					Assert.AreEqual (19, generator.Slots.ElementAt (1).MaxValue);
					Assert.AreEqual (20, generator.Slots.ElementAt (2).MinValue);
					Assert.AreEqual (29, generator.Slots.ElementAt (2).MaxValue);
				}
			}
		}


		[TestMethod]
		public void GetNextUidTest1()
		{
			using (DbInfrastructure dbInfrastructure = DbInfrastructureHelper.ConnectToTestDatabase ())
			{
				string name = "myCounter";

				List<UidSlot> slots = new List<UidSlot> ()
				{
					new UidSlot (0, 9),
				};

				UidGenerator.CreateUidGenerator (dbInfrastructure, name, slots);

				UidGenerator generator = UidGenerator.GetUidGenerator (dbInfrastructure, name);

				for (int i = 0; i <= 9; i++)
				{
					Assert.AreEqual (i, generator.GetNextUid ());
				}

				ExceptionAssert.Throw<System.Exception> (() => generator.GetNextUid ());
			}
		}


		[TestMethod]
		public void GetNextUidTest2()
		{
			using (DbInfrastructure dbInfrastructure = DbInfrastructureHelper.ConnectToTestDatabase ())
			{
				string name = "myCounter";

				List<UidSlot> slots = new List<UidSlot> ()
				{
					new UidSlot (15, 15),
					new UidSlot (19, 34),
					new UidSlot (0, 9),
				};

				UidGenerator.CreateUidGenerator (dbInfrastructure, name, slots);

				UidGenerator generator = UidGenerator.GetUidGenerator (dbInfrastructure, name);

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
		}


		[TestMethod]
		public void GetNextUidInSlotArgumentCheck()
		{
			using (DbInfrastructure dbInfrastructure = DbInfrastructureHelper.ConnectToTestDatabase ())
			{
				string name = "myCounter";

				List<UidSlot> slots = new List<UidSlot> ()
				{
					new UidSlot (0, 9),
				};

				UidGenerator.CreateUidGenerator (dbInfrastructure, name, slots);

				UidGenerator generator = UidGenerator.GetUidGenerator (dbInfrastructure, name);

				ExceptionAssert.Throw<System.Exception>
				(
					() => generator.GetNextUidInSlot (-1)
				);

				ExceptionAssert.Throw<System.Exception>
				(
					() => generator.GetNextUidInSlot (2)
				);
			}
		}


		[TestMethod]
		public void GetNextUidInSlotTest()
		{
			using (DbInfrastructure dbInfrastructure = DbInfrastructureHelper.ConnectToTestDatabase ())
			{
				string name = "myCounter";

				List<UidSlot> slots = new List<UidSlot> ()
				{
					new UidSlot (12, 17),
					new UidSlot (0, 9),
					new UidSlot (19, 34),
				};

				UidGenerator.CreateUidGenerator (dbInfrastructure, name, slots);

				UidGenerator generator = UidGenerator.GetUidGenerator (dbInfrastructure, name);

				for (int i = 12; i <= 17; i++)
				{
					Assert.AreEqual (i, generator.GetNextUidInSlot (1));
				}

				ExceptionAssert.Throw<System.Exception> (() => generator.GetNextUidInSlot (1));
			}
		}


	}


}
