using Epsitec.Common.Support.Extensions;

using Epsitec.Common.UnitTesting;

using Epsitec.Cresus.Database;
using Epsitec.Cresus.Database.Services;
using Epsitec.Cresus.Database.UnitTests.Helpers;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.Collections.Generic;
using System.Linq;


namespace Epsitec.Cresus.Database.UnitTests.Services
{


	[TestClass]
	public sealed class UnitTestDbConnectionManager
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
				() => new DbConnectionManager (null)
			);
		}


		[TestMethod]
		public void OpenConnectionArgumentCheck()
		{
			using (DbInfrastructure dbInfrastructure = DbInfrastructureHelper.ConnectToTestDatabase ())
			{
				DbConnectionManager manager = dbInfrastructure.ServiceManager.ConnectionManager;

				ExceptionAssert.Throw<System.ArgumentException>
				(
					() => manager.OpenConnection (null)
				);

				ExceptionAssert.Throw<System.ArgumentException>
				(
					() => manager.OpenConnection ("")
				);
			}
		}


		[TestMethod]
		public void CloseConnectionArgumentCheck()
		{
			using (DbInfrastructure dbInfrastructure = DbInfrastructureHelper.ConnectToTestDatabase ())
			{
				DbConnectionManager manager = dbInfrastructure.ServiceManager.ConnectionManager;

				ExceptionAssert.Throw<System.ArgumentException>
				(
					() => manager.CloseConnection (-1)
				);
			}
		}


		[TestMethod]
		public void CloseConnectionInvalidBehavior()
		{
			using (DbInfrastructure dbInfrastructure = DbInfrastructureHelper.ConnectToTestDatabase ())
			{
				DbConnectionManager manager = new DbConnectionManager (dbInfrastructure);
				manager.TurnOn ();

				DbId connectionId1 = manager.OpenConnection ("connection1").Id;

				manager.CloseConnection (connectionId1);

				ExceptionAssert.Throw<System.InvalidOperationException>
				(
					() => manager.CloseConnection (connectionId1)
				);

				ExceptionAssert.Throw<System.InvalidOperationException>
				(
					() => manager.CloseConnection (1)
				);

				DbId connectionId2 = manager.OpenConnection ("connection2").Id;

				System.Threading.Thread.Sleep (3000);

				manager.InterruptDeadConnections (System.TimeSpan.FromSeconds (2));

				ExceptionAssert.Throw<System.InvalidOperationException>
				(
					() => manager.CloseConnection (connectionId2)
				);
			}
		}


		[TestMethod]
		public void OpenAndCloseConnection()
		{
			using (DbInfrastructure dbInfrastructure = DbInfrastructureHelper.ConnectToTestDatabase ())
			{
				DbConnectionManager manager = dbInfrastructure.ServiceManager.ConnectionManager;

				DbId connectionId = manager.OpenConnection ("connection").Id;

				Assert.AreEqual ("connection", manager.GetConnection (connectionId).Identity);
				Assert.AreEqual (DbConnectionStatus.Open, manager.GetConnection (connectionId).Status);

				manager.CloseConnection (connectionId);

				Assert.AreEqual ("connection", manager.GetConnection (connectionId).Identity);
				Assert.AreEqual (DbConnectionStatus.Closed, manager.GetConnection (connectionId).Status);
			}
		}


		[TestMethod]
		public void ConnectionExistsArgumentCheck()
		{
			using (DbInfrastructure dbInfrastructure = DbInfrastructureHelper.ConnectToTestDatabase ())
			{
				DbConnectionManager manager = dbInfrastructure.ServiceManager.ConnectionManager;

				ExceptionAssert.Throw<System.ArgumentException>
				(
					() => manager.ConnectionExists (-1)
				);
			}
		}


		[TestMethod]
		public void ConnectionExists()
		{
			using (DbInfrastructure dbInfrastructure = DbInfrastructureHelper.ConnectToTestDatabase ())
			{
				DbConnectionManager manager = dbInfrastructure.ServiceManager.ConnectionManager;

				Assert.IsFalse (manager.ConnectionExists (0));

				DbId connectionId = manager.OpenConnection ("connection").Id;

				Assert.IsTrue (manager.ConnectionExists (connectionId));

				manager.CloseConnection (connectionId);

				Assert.IsTrue (manager.ConnectionExists (connectionId));
			}
		}


		[TestMethod]
		public void KeepConnectionAliveArgumentCheck()
		{
			using (DbInfrastructure dbInfrastructure = DbInfrastructureHelper.ConnectToTestDatabase ())
			{
				DbConnectionManager manager = dbInfrastructure.ServiceManager.ConnectionManager;

				ExceptionAssert.Throw<System.ArgumentException>
				(
					() => manager.KeepConnectionAlive (-1)
				);
			}
		}


		[TestMethod]
		public void KeepConnectionAliveInvalidBehavior()
		{
			using (DbInfrastructure dbInfrastructure = DbInfrastructureHelper.ConnectToTestDatabase ())
			{
				DbConnectionManager manager = new DbConnectionManager (dbInfrastructure);
				manager.TurnOn ();

				DbId connectionId1 = manager.OpenConnection ("connection1").Id;

				manager.CloseConnection (connectionId1);

				ExceptionAssert.Throw<System.InvalidOperationException>
				(
					() => manager.KeepConnectionAlive (connectionId1)
				);

				ExceptionAssert.Throw<System.InvalidOperationException>
				(
					() => manager.KeepConnectionAlive (1)
				);
				DbId connectionId2 = manager.OpenConnection ("connection2").Id;

				System.Threading.Thread.Sleep (3000);

				manager.InterruptDeadConnections (System.TimeSpan.FromSeconds (2));

				ExceptionAssert.Throw<System.InvalidOperationException>
				(
					() => manager.KeepConnectionAlive (connectionId2)
				);
			}
		}


		[TestMethod]
		public void KeepConnectionAlive()
		{
			using (DbInfrastructure dbInfrastructure = DbInfrastructureHelper.ConnectToTestDatabase ())
			{
				DbConnectionManager manager = dbInfrastructure.ServiceManager.ConnectionManager;

				DbId connectionId = manager.OpenConnection ("connection").Id;

				List<System.DateTime> refreshTimes = new List<System.DateTime> ();

				refreshTimes.Add (manager.GetConnection (connectionId).RefreshTime);

				for (int i = 0; i < 10; i++)
				{
					System.Threading.Thread.Sleep (500);

					manager.KeepConnectionAlive (connectionId);

					refreshTimes.Add (manager.GetConnection (connectionId).RefreshTime);
				}

				Assert.IsTrue (refreshTimes.First () == manager.GetConnection (connectionId).EstablishmentTime);

				for (int i = 0; i < refreshTimes.Count - 1; i++)
				{
					Assert.IsTrue (refreshTimes[i] < refreshTimes[i + 1]);
				}
				
				manager.CloseConnection (connectionId);
			}
		}


		[TestMethod]
		public void InterruptDeadConnections()
		{
			using (DbInfrastructure dbInfrastructure = DbInfrastructureHelper.ConnectToTestDatabase ())
			{
				DbConnectionManager manager = new DbConnectionManager (dbInfrastructure);
				manager.TurnOn ();

				DbId connectionId1 = manager.OpenConnection ("connection1").Id;
				DbId connectionId2 = manager.OpenConnection ("connection2").Id;
				DbId connectionId3 = manager.OpenConnection ("connection3").Id;

				manager.CloseConnection (connectionId1);

				for (int i = 0; i < 5; i++)
				{
					manager.KeepConnectionAlive (connectionId2);

					System.Threading.Thread.Sleep (1000);
				}

				System.Threading.Thread.Sleep (1000);

				Assert.AreEqual (true, manager.InterruptDeadConnections (System.TimeSpan.FromSeconds (5)));

				Assert.AreEqual (DbConnectionStatus.Closed, manager.GetConnection (connectionId1).Status);
				Assert.AreEqual (DbConnectionStatus.Open, manager.GetConnection (connectionId2).Status);
				Assert.AreEqual (DbConnectionStatus.Interrupted, manager.GetConnection (connectionId3).Status);

				manager.CloseConnection (connectionId2);
			}
		}


		[TestMethod]
		public void GetOpenedConnections()
		{
			using (DbInfrastructure dbInfrastructure = DbInfrastructureHelper.ConnectToTestDatabase ())
			{
				DbConnectionManager manager = new DbConnectionManager (dbInfrastructure);
				manager.TurnOn ();

				List<DbId> connectionIds = new List<DbId> ();

				Assert.IsTrue (connectionIds.SetEquals (manager.GetOpenConnections ().Select (c => c.Id)));

				DbId connectionId1 = manager.OpenConnection ("connection1").Id;
				connectionIds.Add (connectionId1);
				Assert.IsTrue (connectionIds.SetEquals (manager.GetOpenConnections ().Select (c => c.Id)));

				DbId connectionId2 = manager.OpenConnection ("connection2").Id;
				connectionIds.Add (connectionId2);
				Assert.IsTrue (connectionIds.SetEquals (manager.GetOpenConnections ().Select (c => c.Id)));

				DbId connectionId3 = manager.OpenConnection ("connection3").Id;
				connectionIds.Add (connectionId3);
				Assert.IsTrue (connectionIds.SetEquals (manager.GetOpenConnections ().Select (c => c.Id)));

				manager.CloseConnection (connectionId1);
				connectionIds.Remove (connectionId1);
				Assert.IsTrue (connectionIds.SetEquals (manager.GetOpenConnections ().Select (c => c.Id)));

				for (int i = 0; i < 5; i++)
				{
					manager.KeepConnectionAlive (connectionId2);

					System.Threading.Thread.Sleep (1000);
				}

				System.Threading.Thread.Sleep (1000);

				Assert.AreEqual (true, manager.InterruptDeadConnections (System.TimeSpan.FromSeconds (5)));
				connectionIds.Remove (connectionId3);
				Assert.IsTrue (connectionIds.SetEquals (manager.GetOpenConnections ().Select (c => c.Id)));

				Assert.AreEqual (DbConnectionStatus.Closed, manager.GetConnection (connectionId1).Status);
				Assert.AreEqual (DbConnectionStatus.Open, manager.GetConnection (connectionId2).Status);
				Assert.AreEqual (DbConnectionStatus.Interrupted, manager.GetConnection (connectionId3).Status);

				manager.CloseConnection (connectionId2);
				connectionIds.Remove (connectionId2);
				Assert.IsTrue (connectionIds.SetEquals (manager.GetOpenConnections ().Select (c => c.Id)));
			}
		}


		[TestMethod]
		public void GetLockOwnersArgumengCheck()
		{
			using (DbInfrastructure dbInfrastructure = DbInfrastructureHelper.ConnectToTestDatabase ())
			{
				DbConnectionManager manager = new DbConnectionManager (dbInfrastructure);

				ExceptionAssert.Throw<System.ArgumentNullException>
				(
					() => manager.GetLockOwners (null)
				);
			}
		}


		[TestMethod]
		public void GetLockOwnersTest()
		{
			using (DbInfrastructure dbInfrastructure = DbInfrastructureHelper.ConnectToTestDatabase ())
			{
				DbConnectionManager connectionManager = new DbConnectionManager (dbInfrastructure);
				DbLockManager lockManager = new DbLockManager (dbInfrastructure);

				connectionManager.TurnOn ();
				lockManager.TurnOn ();

				DbId connectionId1 = connectionManager.OpenConnection ("connection1").Id;
				DbId connectionId2 = connectionManager.OpenConnection ("connection2").Id;
				DbId connectionId3 = connectionManager.OpenConnection ("connection3").Id;
				DbId connectionId4 = connectionManager.OpenConnection ("connection4").Id;

				lockManager.RequestLock ("lock1", connectionId1);
				lockManager.RequestLock ("lock2", connectionId1);
				lockManager.RequestLock ("lock3", connectionId2);
				lockManager.RequestLock ("lock3", connectionId2);
				lockManager.RequestLock ("lock4", connectionId3);

				var expectedResult1 = new Dictionary<DbId, List<string>> ()
				{
					{ connectionId1, new List<string>() { "lock1", "lock2", } },
					{ connectionId2, new List<string>() { "lock3", } },
					{ connectionId3, new List<string>() { "lock4", } },
				};
				var actualResult1 = connectionManager.GetLockOwners (new List<string> () { "lock1", "lock2", "lock3", "lock4", });
				this.CheckResult (expectedResult1, actualResult1);

				var expectedResult2 = new Dictionary<DbId, List<string>> ();
				var actualResult2 = connectionManager.GetLockOwners (new List<string> ());
				this.CheckResult (expectedResult2, actualResult2);

				var expectedResult3 = new Dictionary<DbId, List<string>> ()
				{
					{ connectionId1, new List<string>() { "lock1", "lock2", } },
				};
				var actualResult3 = connectionManager.GetLockOwners (new List<string> () { "lock1", "lock2", });
				this.CheckResult (expectedResult3, actualResult3);

				var expectedResult4 = new Dictionary<DbId, List<string>> ()
				{
					{ connectionId2, new List<string>() { "lock3", } },
				};
				var actualResult4 = connectionManager.GetLockOwners (new List<string> () { "lock3", });
				this.CheckResult (expectedResult4, actualResult4);

				var expectedResult5 = new Dictionary<DbId, List<string>> ()
				{
					{ connectionId3, new List<string>() { "lock4", } },
				};
				var actualResult5 = connectionManager.GetLockOwners (new List<string> () { "lock4", });

				this.CheckResult (expectedResult5, actualResult5);

				var expectedResult6 = new Dictionary<DbId, List<string>> ();
				var actualResult6 = connectionManager.GetLockOwners (new List<string> () { "coucou" });
				this.CheckResult (expectedResult6, actualResult6);
			}
		}


		private void CheckResult(Dictionary<DbId, List<string>> expected, IEnumerable<System.Tuple<DbConnection, DbLock>> actual)
		{
			var actualAsList = actual.Select (t => System.Tuple.Create (t.Item1.Id, t.Item2.Name)).ToList ();

			Assert.AreEqual (expected.Values.Aggregate (0, (a, l) => a + l.Count), actualAsList.Count);

			foreach (var item in expected)
			{
				foreach (var name in item.Value)
				{
					Assert.IsNotNull (actualAsList.Single (t => t.Item1 == item.Key && t.Item2 == name));
				}
			}
		}


	}


}
