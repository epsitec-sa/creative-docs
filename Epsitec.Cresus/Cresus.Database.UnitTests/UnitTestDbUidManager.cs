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
					() => manager.CreateUidCounter (null, 0, 0)
				);

				ExceptionAssert.Throw<System.ArgumentException>
				(
					() => manager.CreateUidCounter ("", 0, 0)
				);

				ExceptionAssert.Throw<System.ArgumentException>
				(
					() => manager.CreateUidCounter ("test", -1, 0)
				);

				ExceptionAssert.Throw<System.ArgumentException>
				(
					() => manager.CreateUidCounter ("test", 0, -1)
				);
			}
		}


		[TestMethod]
		public void CreateUidCounter()
		{
			using (DbInfrastructure dbInfrastructure = TestHelper.ConnectToDatabase ())
			{
				DbUidManager manager = dbInfrastructure.UidManager;

				manager.CreateUidCounter ("myCounter", 0, 10);

				Assert.IsTrue (manager.ExistsUidCounter ("myCounter"));
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
					() => manager.DeleteUidCounter (null)
				);

				ExceptionAssert.Throw<System.ArgumentException>
				(
					() => manager.DeleteUidCounter ("")
				);
			}
		}


		[TestMethod]
		public void DeleteUidCounter()
		{
			using (DbInfrastructure dbInfrastructure = TestHelper.ConnectToDatabase ())
			{
				DbUidManager manager = dbInfrastructure.UidManager;

				manager.CreateUidCounter ("myCounter", 0, 10);

				Assert.IsTrue (manager.ExistsUidCounter ("myCounter"));

				manager.DeleteUidCounter ("myCounter");

				Assert.IsFalse (manager.ExistsUidCounter ("myCounter"));
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
					() => manager.ExistsUidCounter (null)
				);

				ExceptionAssert.Throw<System.ArgumentException>
				(
					() => manager.ExistsUidCounter ("")
				);
			}
		}


		[TestMethod]
		public void ExistsUidCounter()
		{
			using (DbInfrastructure dbInfrastructure = TestHelper.ConnectToDatabase ())
			{
				DbUidManager manager = dbInfrastructure.UidManager;

				Assert.IsFalse (manager.ExistsUidCounter ("myCounter"));

				manager.CreateUidCounter ("myCounter", 0, 10);

				Assert.IsTrue (manager.ExistsUidCounter ("myCounter"));

				manager.DeleteUidCounter ("myCounter");

				Assert.IsFalse (manager.ExistsUidCounter ("myCounter"));
			}
		}


		[TestMethod]
		public void GetUidCounterNames()
		{
			using (DbInfrastructure dbInfrastructure = TestHelper.ConnectToDatabase ())
			{
				DbUidManager manager = dbInfrastructure.UidManager;

				List<string> counterNames = manager.GetUidCounterNames ().ToList ();
				Assert.IsTrue (counterNames.Count == 0);

				manager.CreateUidCounter ("myCounter1", 0, 10);

				counterNames = manager.GetUidCounterNames ().ToList ();
				Assert.IsTrue (counterNames.Count == 1);
				Assert.IsTrue (counterNames.Contains ("myCounter1"));

				manager.CreateUidCounter ("myCounter2", 0, 10);

				counterNames = manager.GetUidCounterNames ().ToList ();
				Assert.IsTrue (counterNames.Count == 2);
				Assert.IsTrue (counterNames.Contains ("myCounter1"));
				Assert.IsTrue (counterNames.Contains ("myCounter2"));

				manager.CreateUidCounter ("myCounter3", 0, 10);

				counterNames = manager.GetUidCounterNames ().ToList ();
				Assert.IsTrue (counterNames.Count == 3);
				Assert.IsTrue (counterNames.Contains ("myCounter1"));
				Assert.IsTrue (counterNames.Contains ("myCounter2"));
				Assert.IsTrue (counterNames.Contains ("myCounter3"));

				manager.DeleteUidCounter ("myCounter1");

				counterNames = manager.GetUidCounterNames ().ToList ();
				Assert.IsTrue (counterNames.Count == 2);
				Assert.IsTrue (counterNames.Contains ("myCounter2"));
				Assert.IsTrue (counterNames.Contains ("myCounter3"));
				
				manager.DeleteUidCounter ("myCounter2");

				counterNames = manager.GetUidCounterNames ().ToList ();
				Assert.IsTrue (counterNames.Count == 1);
				Assert.IsTrue (counterNames.Contains ("myCounter3"));

				manager.DeleteUidCounter ("myCounter3");
				
				counterNames = manager.GetUidCounterNames ().ToList ();
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
					() => manager.GetUidCounterMin (null)
				);

				ExceptionAssert.Throw<System.ArgumentException>
				(
					() => manager.GetUidCounterMin ("")
				);

				ExceptionAssert.Throw<System.ArgumentException>
				(
					() => manager.SetUidCounterMin (null, 0)
				);

				ExceptionAssert.Throw<System.ArgumentException>
				(
					() => manager.SetUidCounterMin ("", 0)
				);

				ExceptionAssert.Throw<System.ArgumentException>
				(
					() => manager.SetUidCounterMin ("test", -1)
				);
			}
		}


		[TestMethod]
		public void GetSetUidCounterMin()
		{
			using (DbInfrastructure dbInfrastructure = TestHelper.ConnectToDatabase ())
			{
				DbUidManager manager = dbInfrastructure.UidManager;

				manager.CreateUidCounter ("myCounter", 0, 10);
				Assert.AreEqual (0, manager.GetUidCounterMin ("myCounter"));

				for (int i = 0; i < 10; i++)
				{
					manager.SetUidCounterMin ("myCounter", i);
					Assert.AreEqual (i, manager.GetUidCounterMin ("myCounter"));
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
					() => manager.GetUidCounterMax (null)
				);

				ExceptionAssert.Throw<System.ArgumentException>
				(
					() => manager.GetUidCounterMax ("")
				);

				ExceptionAssert.Throw<System.ArgumentException>
				(
					() => manager.SetUidCounterMax (null, 0)
				);

				ExceptionAssert.Throw<System.ArgumentException>
				(
					() => manager.SetUidCounterMax ("", 0)
				);

				ExceptionAssert.Throw<System.ArgumentException>
				(
					() => manager.SetUidCounterMax ("test", -1)
				);
			}
		}


		[TestMethod]
		public void GetSetUidCounterMax()
		{
			using (DbInfrastructure dbInfrastructure = TestHelper.ConnectToDatabase ())
			{
				DbUidManager manager = dbInfrastructure.UidManager;

				manager.CreateUidCounter ("myCounter", 0, 10);
				Assert.AreEqual (10, manager.GetUidCounterMax ("myCounter"));

				for (int i = 10; i < 20; i++)
				{
					manager.SetUidCounterMax ("myCounter", i);
					Assert.AreEqual (i, manager.GetUidCounterMax ("myCounter"));
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
					() => manager.GetUidCounterCurrent (null)
				);

				ExceptionAssert.Throw<System.ArgumentException>
				(
					() => manager.GetUidCounterCurrent ("")
				);

				ExceptionAssert.Throw<System.ArgumentException>
				(
					() => manager.SetUidCounterCurrent (null, 0)
				);

				ExceptionAssert.Throw<System.ArgumentException>
				(
					() => manager.SetUidCounterCurrent ("", 0)
				);

				ExceptionAssert.Throw<System.ArgumentException>
				(
					() => manager.SetUidCounterCurrent ("test", -1)
				);
			}
		}


		[TestMethod]
		public void GetSetUidCounterCurrent()
		{
			using (DbInfrastructure dbInfrastructure = TestHelper.ConnectToDatabase ())
			{
				DbUidManager manager = dbInfrastructure.UidManager;

				manager.CreateUidCounter ("myCounter", 0, 10);
				Assert.AreEqual (0, manager.GetUidCounterCurrent ("myCounter"));

				for (int i = 0; i < 10; i++)
				{
					manager.SetUidCounterCurrent ("myCounter", i);
					Assert.AreEqual (i, manager.GetUidCounterCurrent ("myCounter"));
				}
			}
		}


	}

}
