using Epsitec.Common.UnitTesting;

using Epsitec.Cresus.Database;
using Epsitec.Cresus.Database.UnitTests.Helpers;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.Collections.Generic;

using System.Linq;


namespace Cresus.Database.UnitTests
{


	[TestClass]
	public class UnitTestDbLockManager
	{


		[ClassInitialize]
		public static void ClassInitialize(TestContext testContext)
		{
			TestHelper.Initialize ();
		}


		[TestInitialize]
		public void TestInitialize()
		{
			TestHelper.DeleteDatabase ();
			TestHelper.CreateDatabase ();
		}


		[TestMethod]
		public void AttachArgumentCheck()
		{
			using (DbInfrastructure dbInfrastructure = TestHelper.ConnectToDatabase ())
			{
				ExceptionAssert.Throw<System.ArgumentNullException>
				(
					() => new DbLockManager ().Attach (null, new DbTable ())
				);

				ExceptionAssert.Throw<System.ArgumentNullException>
				(
					() => new DbLockManager ().Attach (dbInfrastructure, null)
				);
			}
		}


		[TestMethod]
		public void AttachAndDetach()
		{
			using (DbInfrastructure dbInfrastructure = TestHelper.ConnectToDatabase ())
			{
				Assert.IsNotNull (dbInfrastructure.LockManager);
			}
		}


		[TestMethod]
		public void InsertLockArgumentCheck()
		{
			using (DbInfrastructure dbInfrastructure = TestHelper.ConnectToDatabase ())
			{
				DbLockManager manager = dbInfrastructure.LockManager;

				ExceptionAssert.Throw<System.ArgumentException>
				(
					() => manager.InsertLock (null, "test")
				);

				ExceptionAssert.Throw<System.ArgumentException>
				(
					() => manager.InsertLock ("", "test")
				);

				ExceptionAssert.Throw<System.ArgumentException>
				(
					() => manager.InsertLock ("test", null)
				);

				ExceptionAssert.Throw<System.ArgumentException>
				(
					() => manager.InsertLock ("test", "")
				);
			}
		}


		[TestMethod]
		public void InsertAndExistsLock()
		{
			using (DbInfrastructure dbInfrastructure = TestHelper.ConnectToDatabase ())
			{
				DbLockManager manager = dbInfrastructure.LockManager;

				for (int i = 0; i < 10; i++)
				{
					manager.InsertLock ("myLock" + i, "test");
					Assert.IsTrue (manager.ExistsLock ("myLock" + i));
				}
			}
		}


		[TestMethod]
		public void DeleteLockArgumentCheck()
		{
			using (DbInfrastructure dbInfrastructure = TestHelper.ConnectToDatabase ())
			{
				DbLockManager manager = dbInfrastructure.LockManager;

				ExceptionAssert.Throw<System.ArgumentException>
				(
					() => manager.RemoveLock (null)
				);

				ExceptionAssert.Throw<System.ArgumentException>
				(
					() => manager.RemoveLock ("")
				);
			}
		}


		[TestMethod]
		public void RemoveAndExistsLock()
		{
			using (DbInfrastructure dbInfrastructure = TestHelper.ConnectToDatabase ())
			{
				DbLockManager manager = dbInfrastructure.LockManager;

				for (int i = 0; i < 10; i++)
				{
					manager.InsertLock ("myLock" + i, "test");
					Assert.IsTrue (manager.ExistsLock ("myLock" + i));

					manager.RemoveLock ("myLock" + i);
					Assert.IsFalse (manager.ExistsLock ("myLock" + i));
				}
			}
		}


		[TestMethod]
		public void ExistsLockArgumentCheck()
		{
			using (DbInfrastructure dbInfrastructure = TestHelper.ConnectToDatabase ())
			{
				DbLockManager manager = dbInfrastructure.LockManager;

				ExceptionAssert.Throw<System.ArgumentException>
				(
					() => manager.ExistsLock (null)
				);

				ExceptionAssert.Throw<System.ArgumentException>
				(
					() => manager.ExistsLock ("")
				);
			}
		}


		[TestMethod]
		public void UpdateLockDateTimeArgumentCheck()
		{
			using (DbInfrastructure dbInfrastructure = TestHelper.ConnectToDatabase ())
			{
				DbLockManager manager = dbInfrastructure.LockManager;

				ExceptionAssert.Throw<System.ArgumentException>
				(
					() => manager.UpdateLockDateTime (null)
				);

				ExceptionAssert.Throw<System.ArgumentException>
				(
					() => manager.UpdateLockDateTime ("")
				);
			}
		}


		[TestMethod]
		public void GetLockTimeSpanArgumentCheck()
		{
			using (DbInfrastructure dbInfrastructure = TestHelper.ConnectToDatabase ())
			{
				DbLockManager manager = dbInfrastructure.LockManager;

				ExceptionAssert.Throw<System.ArgumentException>
				(
					() => manager.GetLockTimeSpan (null)
				);

				ExceptionAssert.Throw<System.ArgumentException>
				(
					() => manager.GetLockTimeSpan ("")
				);
			}
		}


		[TestMethod]
		public void GetAndUpdateLockDateTime()
		{
			using (DbInfrastructure dbInfrastructure = TestHelper.ConnectToDatabase ())
			{
				DbLockManager manager = dbInfrastructure.LockManager;

				manager.InsertLock ("myLock1", "test1");
				Assert.IsTrue (manager.ExistsLock ("myLock1"));

				manager.InsertLock ("myLock2", "test1");
				Assert.IsTrue (manager.ExistsLock ("myLock2"));

				manager.InsertLock ("myLock3", "test2");
				Assert.IsTrue (manager.ExistsLock ("myLock3"));

				List<List<System.TimeSpan>> timeSpans1 = new List<List<System.TimeSpan>> ()
				{
					new List<System.TimeSpan> (),
					new List<System.TimeSpan> (),
					new List<System.TimeSpan> (),
				};

				for (int i = 0; i < 5; i++)
				{
					System.Threading.Thread.Sleep (500);

					for (int j = 0; j < 3; j++)
					{
						timeSpans1[j].Add (manager.GetLockTimeSpan ("myLock" + (j + 1)));
					}
				}

				for (int i = 0; i < 3; i++)
				{
					for (int j = 0; j < timeSpans1.Count - 1; j++)
					{
						Assert.IsTrue (timeSpans1[i][j] < timeSpans1[i][j + 1]);
					}
				}

				manager.UpdateLockDateTime ("test1");

				List<List<System.TimeSpan>> timeSpans2 = new List<List<System.TimeSpan>> ()
				{
					new List<System.TimeSpan> (),
					new List<System.TimeSpan> (),
					new List<System.TimeSpan> (),
				};

				for (int i = 0; i < 5; i++)
				{
					System.Threading.Thread.Sleep (500);

					for (int j = 0; j < 3; j++)
					{
						timeSpans2[j].Add (manager.GetLockTimeSpan ("myLock" + (j + 1)));
					}
				}

				Assert.IsTrue (timeSpans1[0].Last () > timeSpans2[0].First ());
				Assert.IsTrue (timeSpans1[1].Last () > timeSpans2[1].First ());
				Assert.IsTrue (timeSpans1[2].Last () < timeSpans2[2].First ());

				for (int i = 0; i < 3; i++)
				{
					for (int j = 0; j < timeSpans2.Count - 1; j++)
					{
						Assert.IsTrue (timeSpans2[i][j] < timeSpans2[i][j + 1]);
					}
				}
			}
		}


		[TestMethod]
		public void GetLockUserNameArgumentCheck()
		{
			using (DbInfrastructure dbInfrastructure = TestHelper.ConnectToDatabase ())
			{
				DbLockManager manager = dbInfrastructure.LockManager;

				ExceptionAssert.Throw<System.ArgumentException>
				(
					() => manager.GetLockUserName (null)
				);

				ExceptionAssert.Throw<System.ArgumentException>
				(
					() => manager.GetLockUserName ("")
				);
			}
		}


		[TestMethod]
		public void GetLockUserName()
		{
			using (DbInfrastructure dbInfrastructure = TestHelper.ConnectToDatabase ())
			{
				DbLockManager manager = dbInfrastructure.LockManager;

				manager.InsertLock ("myLock1", "test1");
				Assert.IsTrue (manager.ExistsLock ("myLock1"));

				manager.InsertLock ("myLock2", "test1");
				Assert.IsTrue (manager.ExistsLock ("myLock2"));

				manager.InsertLock ("myLock3", "test2");
				Assert.IsTrue (manager.ExistsLock ("myLock3"));

				Assert.AreEqual ("test1", manager.GetLockUserName ("myLock1"));
				Assert.AreEqual ("test1", manager.GetLockUserName ("myLock2"));
				Assert.AreEqual ("test2", manager.GetLockUserName ("myLock3"));
			}
		}


	}


}
