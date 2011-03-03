using Epsitec.Common.UnitTesting;

using Epsitec.Cresus.Database;
using Epsitec.Cresus.Database.Services;
using Epsitec.Cresus.Database.Tests.Vs.Helpers;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.Linq;


namespace Epsitec.Cresus.Database.Tests.Vs.Services
{


	[TestClass]
	public class UnitTestDbUidManager
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
		public void ConstructorArgumentCheck()
		{
			ExceptionAssert.Throw<System.ArgumentNullException>
			(
				() => new DbUidManager (null)
			);
		}


		[TestMethod]
		public void CreateUidCounterArgumentCheck()
		{
			using (DbInfrastructure dbInfrastructure = DbInfrastructureHelper.ConnectToTestDatabase ())
			{
				DbUidManager manager = dbInfrastructure.ServiceManager.UidManager;

				ExceptionAssert.Throw<System.ArgumentException>
				(
					() => manager.CreateUidCounter (null, 0, 0, 0)
				);

				ExceptionAssert.Throw<System.ArgumentException>
				(
					() => manager.CreateUidCounter ("", 0, 0, 0)
				);

				ExceptionAssert.Throw<System.ArgumentException>
				(
					() => manager.CreateUidCounter ("test", -1, 0, 0)
				);

				ExceptionAssert.Throw<System.ArgumentException>
				(
					() => manager.CreateUidCounter ("test", 0, -1, 0)
				);

				ExceptionAssert.Throw<System.ArgumentException>
				(
					() => manager.CreateUidCounter ("test", 0, 0, -1)
				);

				ExceptionAssert.Throw<System.InvalidOperationException>
				(
					() =>
					{
						manager.CreateUidCounter ("myCounter", 0, 0, 10);
						manager.CreateUidCounter ("myCounter", 0, 0, 10);
					}
				);
			}
		}


		[TestMethod]
		public void CreateAndExistsUidCounter()
		{
			using (DbInfrastructure dbInfrastructure = DbInfrastructureHelper.ConnectToTestDatabase ())
			{
				DbUidManager manager = dbInfrastructure.ServiceManager.UidManager;

				for (int i = 0; i < 10; i++)
				{
					manager.CreateUidCounter ("myCounter", i, 0, 10);
					Assert.IsTrue (manager.ExistsUidCounter ("myCounter", i));
				}
			}
		}


		[TestMethod]
		public void DeleteUidCounterArgumentCheck()
		{
			using (DbInfrastructure dbInfrastructure = DbInfrastructureHelper.ConnectToTestDatabase ())
			{
				DbUidManager manager = dbInfrastructure.ServiceManager.UidManager;

				ExceptionAssert.Throw<System.ArgumentException>
				(
					() => manager.DeleteUidCounter (null, 0)
				);

				ExceptionAssert.Throw<System.ArgumentException>
				(
					() => manager.DeleteUidCounter ("", 0)
				);

				ExceptionAssert.Throw<System.ArgumentException>
				(
					() => manager.DeleteUidCounter ("test", -1)
				);
			}
		}


		[TestMethod]
		public void ExistsUidCounterArgumentCheck()
		{
			using (DbInfrastructure dbInfrastructure = DbInfrastructureHelper.ConnectToTestDatabase ())
			{
				DbUidManager manager = dbInfrastructure.ServiceManager.UidManager;

				ExceptionAssert.Throw<System.ArgumentException>
				(
					() => manager.ExistsUidCounter (null, 0)
				);

				ExceptionAssert.Throw<System.ArgumentException>
				(
					() => manager.ExistsUidCounter ("", 0)
				);

				ExceptionAssert.Throw<System.ArgumentException>
				(
					() => manager.ExistsUidCounter ("test", -1)
				);
			}
		}


		[TestMethod]
		public void DeleteAndExistsUidCounter()
		{
			using (DbInfrastructure dbInfrastructure = DbInfrastructureHelper.ConnectToTestDatabase ())
			{
				DbUidManager manager = dbInfrastructure.ServiceManager.UidManager;

				for (int i = 0; i < 10; i++)
				{
					manager.CreateUidCounter ("myCounter", i, 0, 10);
					Assert.IsTrue (manager.ExistsUidCounter ("myCounter", i));

					manager.DeleteUidCounter ("myCounter", i);
					Assert.IsFalse (manager.ExistsUidCounter ("myCounter", i));
				}
			}
		}


		[TestMethod]
		public void GetUidCounterSlots()
		{
			using (DbInfrastructure dbInfrastructure = DbInfrastructureHelper.ConnectToTestDatabase ())
			{
				DbUidManager manager = dbInfrastructure.ServiceManager.UidManager;

				var slots = manager.GetUidCounterSlots ("myCounter1").ToList ();
				Assert.IsTrue (slots.Count == 0);
				slots = manager.GetUidCounterSlots ("myCounter2").ToList ();
				Assert.IsTrue (slots.Count == 0);

				manager.CreateUidCounter ("myCounter1", 0, 0, 10);

				slots = manager.GetUidCounterSlots ("myCounter1").ToList ();
				Assert.IsTrue (slots.Count == 1);
				Assert.IsTrue (slots.Any (s => s.SlotNumber == 0));
				slots = manager.GetUidCounterSlots ("myCounter2").ToList ();
				Assert.IsTrue (slots.Count == 0);

				manager.CreateUidCounter ("myCounter2", 0, 0, 10);

				slots = manager.GetUidCounterSlots ("myCounter1").ToList ();
				Assert.IsTrue (slots.Count == 1);
				Assert.IsTrue (slots.Any (s => s.SlotNumber == 0));
				slots = manager.GetUidCounterSlots ("myCounter2").ToList ();
				Assert.IsTrue (slots.Count == 1);
				Assert.IsTrue (slots.Any (s => s.SlotNumber == 0));

				manager.CreateUidCounter ("myCounter1", 1, 0, 10);

				slots = manager.GetUidCounterSlots ("myCounter1").ToList ();
				Assert.IsTrue (slots.Count == 2);
				Assert.IsTrue (slots.Any (s => s.SlotNumber == 0));
				Assert.IsTrue (slots.Any (s => s.SlotNumber == 1));
				slots = manager.GetUidCounterSlots ("myCounter2").ToList ();
				Assert.IsTrue (slots.Count == 1);
				Assert.IsTrue (slots.Any (s => s.SlotNumber == 0));

				manager.DeleteUidCounter ("myCounter1", 0);

				slots = manager.GetUidCounterSlots ("myCounter1").ToList ();
				Assert.IsTrue (slots.Count == 1);
				Assert.IsTrue (slots.Any (s => s.SlotNumber == 1));
				slots = manager.GetUidCounterSlots ("myCounter2").ToList ();
				Assert.IsTrue (slots.Count == 1);
				Assert.IsTrue (slots.Any (s => s.SlotNumber == 0));

				manager.DeleteUidCounter ("myCounter2", 0);

				slots = manager.GetUidCounterSlots ("myCounter1").ToList ();
				Assert.IsTrue (slots.Count == 1);
				Assert.IsTrue (slots.Any (s => s.SlotNumber == 1));
				slots = manager.GetUidCounterSlots ("myCounter2").ToList ();
				Assert.IsTrue (slots.Count == 0);

				manager.DeleteUidCounter ("myCounter1", 1);

				slots = manager.GetUidCounterSlots ("myCounter1").ToList ();
				Assert.IsTrue (slots.Count == 0);
				slots = manager.GetUidCounterSlots ("myCounter2").ToList ();
				Assert.IsTrue (slots.Count == 0);
			}
		}


		[TestMethod]
		public void GetUidCounterArgumentCheck()
		{
			using (DbInfrastructure dbInfrastructure = DbInfrastructureHelper.ConnectToTestDatabase ())
			{
				DbUidManager manager = dbInfrastructure.ServiceManager.UidManager;

				ExceptionAssert.Throw<System.ArgumentException>
				(
					() => manager.GetUidCounter (null, 0)
				);

				ExceptionAssert.Throw<System.ArgumentException>
				(
					() => manager.GetUidCounter ("", 0)
				);

				ExceptionAssert.Throw<System.ArgumentException>
				(
					() => manager.GetUidCounter ("test", -1)
				);
			}
		}


		[TestMethod]
		public void GetUidCounterMin()
		{
			using (DbInfrastructure dbInfrastructure = DbInfrastructureHelper.ConnectToTestDatabase ())
			{
				DbUidManager manager = dbInfrastructure.ServiceManager.UidManager;

				for (int i = 0; i < 10; i++)
				{
					manager.CreateUidCounter ("myCounter", i, i, i + 10);

					Assert.AreEqual (i, manager.GetUidCounter ("myCounter", i).MinValue);
					Assert.AreEqual (i + 10, manager.GetUidCounter ("myCounter", i).MaxValue);
				}
			}
		}


		[TestMethod]
		public void GetUidCounterNextValueArgumentCheck()
		{
			using (DbInfrastructure dbInfrastructure = DbInfrastructureHelper.ConnectToTestDatabase ())
			{
				DbUidManager manager = dbInfrastructure.ServiceManager.UidManager;

				ExceptionAssert.Throw<System.ArgumentException>
				(
					() => manager.GetUidCounterNextValue (null, 0)
				);

				ExceptionAssert.Throw<System.ArgumentException>
				(
					() => manager.GetUidCounterNextValue ("", 0)
				);

				ExceptionAssert.Throw<System.ArgumentException>
				(
					() => manager.GetUidCounterNextValue ("test", -1)
				);

				ExceptionAssert.Throw<System.InvalidOperationException>
				(
					() => manager.GetUidCounterNextValue ("myCounter2", 0)
				);
			}
		}


		[TestMethod]
		public void GetUidCounterNextValue()
		{
			using (DbInfrastructure dbInfrastructure = DbInfrastructureHelper.ConnectToTestDatabase ())
			{
				DbUidManager manager = dbInfrastructure.ServiceManager.UidManager;

				for (int i = 0; i < 10; i++)
				{
					manager.CreateUidCounter ("myCounter", i, 0, 10);
					
					for (int j = 0; j <= 10; j++)
					{
						Assert.AreEqual (j, manager.GetUidCounterNextValue ("myCounter", i));
					}

					Assert.AreEqual (null, manager.GetUidCounterNextValue ("myCounter", i));
				}
			}
		}


	}

}
