using Epsitec.Common.UnitTesting;

using Epsitec.Cresus.Database;
using Epsitec.Cresus.Database.UnitTests.Helpers;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.Collections.Generic;

using System.Linq;


namespace Cresus.Database.UnitTests
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
		public void GetUidCounterNames()
		{
			using (DbInfrastructure dbInfrastructure = TestHelper.ConnectToDatabase ())
			{
				DbUidManager manager = dbInfrastructure.UidManager;

				var counterNames = manager.GetUidCounterNamesAndSlots ().ToList ();
				Assert.IsTrue (counterNames.Count == 0);

				manager.CreateUidCounter ("myCounter1", 0, 0, 10);

				counterNames = manager.GetUidCounterNamesAndSlots ().ToList ();
				Assert.IsTrue (counterNames.Count == 1);
				Assert.IsTrue (counterNames.Any (t => t.Item1 == "myCounter1" && t.Item2 == 0));

				manager.CreateUidCounter ("myCounter2", 0, 0, 10);

				counterNames = manager.GetUidCounterNamesAndSlots ().ToList ();
				Assert.IsTrue (counterNames.Count == 2);
				Assert.IsTrue (counterNames.Any (t => t.Item1 == "myCounter1" && t.Item2 == 0));
				Assert.IsTrue (counterNames.Any (t => t.Item1 == "myCounter2" && t.Item2 == 0));

				manager.CreateUidCounter ("myCounter1", 1, 0, 10);

				counterNames = manager.GetUidCounterNamesAndSlots ().ToList ();
				Assert.IsTrue (counterNames.Count == 3);
				Assert.IsTrue (counterNames.Any (t => t.Item1 == "myCounter1" && t.Item2 == 0));
				Assert.IsTrue (counterNames.Any (t => t.Item1 == "myCounter2" && t.Item2 == 0));
				Assert.IsTrue (counterNames.Any (t => t.Item1 == "myCounter1" && t.Item2 == 1));

				manager.DeleteUidCounter ("myCounter1", 0);

				counterNames = manager.GetUidCounterNamesAndSlots ().ToList ();
				Assert.IsTrue (counterNames.Count == 2);
				Assert.IsTrue (counterNames.Any (t => t.Item1 == "myCounter2" && t.Item2 == 0));
				Assert.IsTrue (counterNames.Any (t => t.Item1 == "myCounter1" && t.Item2 == 1));
				
				manager.DeleteUidCounter ("myCounter2", 0);

				counterNames = manager.GetUidCounterNamesAndSlots ().ToList ();
				Assert.IsTrue (counterNames.Count == 1);
				Assert.IsTrue (counterNames.Any (t => t.Item1 == "myCounter1" && t.Item2 == 1));

				manager.DeleteUidCounter ("myCounter1", 1);
				
				counterNames = manager.GetUidCounterNamesAndSlots ().ToList ();
				Assert.IsTrue (counterNames.Count == 0);
			}
		}


		[TestMethod]
		public void GetSetUidCounterMinArgumentCheck()
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

				ExceptionAssert.Throw<System.ArgumentException>
				(
					() => manager.SetUidCounterMin (null, 0, 0)
				);

				ExceptionAssert.Throw<System.ArgumentException>
				(
					() => manager.SetUidCounterMin ("", 0, 0)
				);

				ExceptionAssert.Throw<System.ArgumentException>
				(
					() => manager.SetUidCounterMin ("test", -1, 0)
				);

				ExceptionAssert.Throw<System.ArgumentException>
				(
					() => manager.SetUidCounterMin ("test", 0, -1)
				);
			}
		}


		[TestMethod]
		public void GetSetUidCounterMin()
		{
			using (DbInfrastructure dbInfrastructure = TestHelper.ConnectToDatabase ())
			{
				DbUidManager manager = dbInfrastructure.UidManager;

				for (int i = 0; i < 10; i++)
				{
					manager.CreateUidCounter ("myCounter", i, 0, 10);
					Assert.AreEqual (0, manager.GetUidCounterMin ("myCounter", i));

					for (int j = 0; j < 10; j++)
					{
						manager.SetUidCounterMin ("myCounter", i, j);
						Assert.AreEqual (j, manager.GetUidCounterMin ("myCounter", i));
					}
				}
			}
		}


		[TestMethod]
		public void GetSetUidCounterMaxArgumentCheck()
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

				ExceptionAssert.Throw<System.ArgumentException>
				(
					() => manager.SetUidCounterMax (null, 0, 0)
				);

				ExceptionAssert.Throw<System.ArgumentException>
				(
					() => manager.SetUidCounterMax ("", 0, 0)
				);

				ExceptionAssert.Throw<System.ArgumentException>
				(
					() => manager.SetUidCounterMax ("test", -1, 0)
				);

				ExceptionAssert.Throw<System.ArgumentException>
				(
					() => manager.SetUidCounterMax ("test", 0, -1)
				);
			}
		}


		[TestMethod]
		public void GetSetUidCounterMax()
		{
			using (DbInfrastructure dbInfrastructure = TestHelper.ConnectToDatabase ())
			{
				DbUidManager manager = dbInfrastructure.UidManager;

				for (int i = 0; i < 10; i++)
				{
					manager.CreateUidCounter ("myCounter", i, 0, 10);
					Assert.AreEqual (10, manager.GetUidCounterMax ("myCounter", i));

					for (int j = 10; j < 20; j++)
					{
						manager.SetUidCounterMax ("myCounter", i, j);
						Assert.AreEqual (j, manager.GetUidCounterMax ("myCounter", i));
					}
				}
			}
		}


		[TestMethod]
		public void GetSetUidCounterCurrentArgumentCheck()
		{
			using (DbInfrastructure dbInfrastructure = TestHelper.ConnectToDatabase ())
			{
				DbUidManager manager = dbInfrastructure.UidManager;

				ExceptionAssert.Throw<System.ArgumentException>
				(
					() => manager.GetUidCounterCurrent (null, 0)
				);

				ExceptionAssert.Throw<System.ArgumentException>
				(
					() => manager.GetUidCounterCurrent ("", 0)
				);

				ExceptionAssert.Throw<System.ArgumentException>
				(
					() => manager.GetUidCounterCurrent ("test", -1)
				);

				ExceptionAssert.Throw<System.ArgumentException>
				(
					() => manager.SetUidCounterCurrent (null, 0, 0)
				);

				ExceptionAssert.Throw<System.ArgumentException>
				(
					() => manager.SetUidCounterCurrent ("", 0, 0)
				);

				ExceptionAssert.Throw<System.ArgumentException>
				(
					() => manager.SetUidCounterCurrent ("test", -1, 0)
				);

				ExceptionAssert.Throw<System.ArgumentException>
				(
					() => manager.SetUidCounterCurrent ("test", 0, -1)
				);
			}
		}


		[TestMethod]
		public void GetSetUidCounterCurrent()
		{
			using (DbInfrastructure dbInfrastructure = TestHelper.ConnectToDatabase ())
			{
				DbUidManager manager = dbInfrastructure.UidManager;

				for (int i = 0; i < 10; i++)
				{
					manager.CreateUidCounter ("myCounter", i, 0, 10);
					Assert.AreEqual (0, manager.GetUidCounterCurrent ("myCounter", i));

					for (int j = 0; j < 10; j++)
					{
						manager.SetUidCounterCurrent ("myCounter", i, j);
						Assert.AreEqual (j, manager.GetUidCounterCurrent ("myCounter", i));
					}	
				}
			}
		}


	}

}
