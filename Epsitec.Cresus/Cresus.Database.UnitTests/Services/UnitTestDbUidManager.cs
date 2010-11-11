using Epsitec.Common.UnitTesting;

using Epsitec.Cresus.Database;
using Epsitec.Cresus.Database.Services;
using Epsitec.Cresus.Database.UnitTests.Helpers;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.Collections.Generic;

using System.Linq;


namespace Cresus.Database.UnitTests.Services
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
			TestHelper.DeleteDatabase();
			TestHelper.CreateDatabase ();
		}


		[TestMethod]
		public void AttachArgumentCheck()
		{
			using (DbInfrastructure dbInfrastructure = TestHelper.ConnectToDatabase ())
			{
				ExceptionAssert.Throw<System.ArgumentNullException>
				(
					() => new DbUidManager ().Attach (null, new DbTable ())
				);

				ExceptionAssert.Throw<System.ArgumentNullException>
				(
					() => new DbUidManager ().Attach (dbInfrastructure, null)
				);
			}
		}

		
		[TestMethod]
		public void AttachAndDetach()
		{
			using (DbInfrastructure dbInfrastructure = TestHelper.ConnectToDatabase ())
			{
				Assert.IsNotNull (dbInfrastructure.UidManager);
			}
		}


		[TestMethod]
		public void CreateUidCounterArgumentCheck()
		{
			using (DbInfrastructure dbInfrastructure = TestHelper.ConnectToDatabase ())
			{
				DbUidManager manager = dbInfrastructure.UidManager;

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
			using (DbInfrastructure dbInfrastructure = TestHelper.ConnectToDatabase ())
			{
				DbUidManager manager = dbInfrastructure.UidManager;

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
			using (DbInfrastructure dbInfrastructure = TestHelper.ConnectToDatabase ())
			{
				DbUidManager manager = dbInfrastructure.UidManager;

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

				ExceptionAssert.Throw<System.InvalidOperationException>
				(
					() => manager.DeleteUidCounter ("myCounter", 0)
				);
			}
		}


		[TestMethod]
		public void DeleteAndExistsUidCounter()
		{
			using (DbInfrastructure dbInfrastructure = TestHelper.ConnectToDatabase ())
			{
				DbUidManager manager = dbInfrastructure.UidManager;

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
		public void ExistsUidCounterArgumentCheck()
		{
			using (DbInfrastructure dbInfrastructure = TestHelper.ConnectToDatabase ())
			{
				DbUidManager manager = dbInfrastructure.UidManager;

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
		public void GetUidCounterSlots()
		{
			using (DbInfrastructure dbInfrastructure = TestHelper.ConnectToDatabase ())
			{
				DbUidManager manager = dbInfrastructure.UidManager;

				var slots = manager.GetUidCounterSlots ("myCounter1").ToList ();
				Assert.IsTrue (slots.Count == 0);
				slots = manager.GetUidCounterSlots ("myCounter2").ToList ();
				Assert.IsTrue (slots.Count == 0);

				manager.CreateUidCounter ("myCounter1", 0, 0, 10);

				slots = manager.GetUidCounterSlots ("myCounter1").ToList ();
				Assert.IsTrue (slots.Count == 1);
				Assert.IsTrue (slots.Any (s => s == 0));
				slots = manager.GetUidCounterSlots ("myCounter2").ToList ();
				Assert.IsTrue (slots.Count == 0);

				manager.CreateUidCounter ("myCounter2", 0, 0, 10);

				slots = manager.GetUidCounterSlots ("myCounter1").ToList ();
				Assert.IsTrue (slots.Count == 1);
				Assert.IsTrue (slots.Any (s => s == 0));
				slots = manager.GetUidCounterSlots ("myCounter2").ToList ();
				Assert.IsTrue (slots.Count == 1);
				Assert.IsTrue (slots.Any (s => s == 0));

				manager.CreateUidCounter ("myCounter1", 1, 0, 10);

				slots = manager.GetUidCounterSlots ("myCounter1").ToList ();
				Assert.IsTrue (slots.Count == 2);
				Assert.IsTrue (slots.Any (s => s == 0));
				Assert.IsTrue (slots.Any (s => s == 1));
				slots = manager.GetUidCounterSlots ("myCounter2").ToList ();
				Assert.IsTrue (slots.Count == 1);
				Assert.IsTrue (slots.Any (s => s == 0));

				manager.DeleteUidCounter ("myCounter1", 0);

				slots = manager.GetUidCounterSlots ("myCounter1").ToList ();
				Assert.IsTrue (slots.Count == 1);
				Assert.IsTrue (slots.Any (s => s == 1));
				slots = manager.GetUidCounterSlots ("myCounter2").ToList ();
				Assert.IsTrue (slots.Count == 1);
				Assert.IsTrue (slots.Any (s => s == 0));

				manager.DeleteUidCounter ("myCounter2", 0);

				slots = manager.GetUidCounterSlots ("myCounter1").ToList ();
				Assert.IsTrue (slots.Count == 1);
				Assert.IsTrue (slots.Any (s => s == 1));
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
		public void GetUidCounterMinArgumentCheck()
		{
			using (DbInfrastructure dbInfrastructure = TestHelper.ConnectToDatabase ())
			{
				DbUidManager manager = dbInfrastructure.UidManager;

				ExceptionAssert.Throw<System.ArgumentException>
				(
					() => manager.GetUidCounterMin (null, 0)
				);

				ExceptionAssert.Throw<System.ArgumentException>
				(
					() => manager.GetUidCounterMin ("", 0)
				);

				ExceptionAssert.Throw<System.ArgumentException>
				(
					() => manager.GetUidCounterMin ("test", -1)
				);

				ExceptionAssert.Throw<System.InvalidOperationException>
				(
					() => manager.GetUidCounterMin ("myCounter2", 0)
				);
			}
		}


		[TestMethod]
		public void GetUidCounterMin()
		{
			using (DbInfrastructure dbInfrastructure = TestHelper.ConnectToDatabase ())
			{
				DbUidManager manager = dbInfrastructure.UidManager;

				for (int i = 0; i < 10; i++)
				{
					manager.CreateUidCounter ("myCounter", i, i, i + 10);
					
					Assert.AreEqual (i, manager.GetUidCounterMin ("myCounter", i));
				}
			}
		}


		[TestMethod]
		public void GetUidCounterMaxArgumentCheck()
		{
			using (DbInfrastructure dbInfrastructure = TestHelper.ConnectToDatabase ())
			{
				DbUidManager manager = dbInfrastructure.UidManager;

				ExceptionAssert.Throw<System.ArgumentException>
				(
					() => manager.GetUidCounterMax (null, 0)
				);

				ExceptionAssert.Throw<System.ArgumentException>
				(
					() => manager.GetUidCounterMax ("", 0)
				);

				ExceptionAssert.Throw<System.ArgumentException>
				(
					() => manager.GetUidCounterMax ("test", -1)
				);

				ExceptionAssert.Throw<System.InvalidOperationException>
				(
					() => manager.GetUidCounterMax ("myCounter2", 0)
				);
			}
		}


		[TestMethod]
		public void GetUidCounterMax()
		{
			using (DbInfrastructure dbInfrastructure = TestHelper.ConnectToDatabase ())
			{
				DbUidManager manager = dbInfrastructure.UidManager;

				for (int i = 0; i < 10; i++)
				{
					manager.CreateUidCounter ("myCounter", i, 0, i + 1);

					Assert.AreEqual (i + 1, manager.GetUidCounterMax ("myCounter", i));
				}
			}
		}


		[TestMethod]
		public void GetUidCounterNextArgumentCheck()
		{
			using (DbInfrastructure dbInfrastructure = TestHelper.ConnectToDatabase ())
			{
				DbUidManager manager = dbInfrastructure.UidManager;

				ExceptionAssert.Throw<System.ArgumentException>
				(
					() => manager.GetUidCounterNext (null, 0)
				);

				ExceptionAssert.Throw<System.ArgumentException>
				(
					() => manager.GetUidCounterNext ("", 0)
				);

				ExceptionAssert.Throw<System.ArgumentException>
				(
					() => manager.GetUidCounterNext ("test", -1)
				);

				ExceptionAssert.Throw<System.InvalidOperationException>
				(
					() => manager.GetUidCounterNext ("myCounter2", 0)
				);
			}
		}


		[TestMethod]
		public void GetSetUidCounterNext()
		{
			using (DbInfrastructure dbInfrastructure = TestHelper.ConnectToDatabase ())
			{
				DbUidManager manager = dbInfrastructure.UidManager;

				for (int i = 0; i < 10; i++)
				{
					manager.CreateUidCounter ("myCounter", i, 0, 10);
					
					for (int j = 0; j <= 10; j++)
					{
						Assert.AreEqual (j, manager.GetUidCounterNext ("myCounter", i));
					}

					Assert.AreEqual (null, manager.GetUidCounterNext ("myCounter", i));
				}
			}
		}


	}

}
