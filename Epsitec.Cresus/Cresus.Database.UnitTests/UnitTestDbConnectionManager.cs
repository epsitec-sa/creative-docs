using Epsitec.Common.UnitTesting;

using Epsitec.Cresus.Database;
using Epsitec.Cresus.Database.UnitTests.Helpers;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.Collections.Generic;
using System.Linq;


namespace Cresus.Database.UnitTests
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
					() => new DbConnectionManager (System.TimeSpan.FromSeconds (1)).Attach (null, new DbTable ())
				);

				ExceptionAssert.Throw<System.ArgumentNullException>
				(
					() => new DbConnectionManager (System.TimeSpan.FromSeconds (1)).Attach (dbInfrastructure, null)
				);
			}
		}


		[TestMethod]
		public void AttachAndDetach()
		{
			using (DbInfrastructure dbInfrastructure = TestHelper.ConnectToDatabase ())
			{
				Assert.IsNotNull (dbInfrastructure.ConnectionManager);
			}
		}


		[TestMethod]
		public void OpenConnectionArgumentCheck()
		{
			using (DbInfrastructure dbInfrastructure = TestHelper.ConnectToDatabase ())
			{
				DbConnectionManager manager = dbInfrastructure.ConnectionManager;

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
			using (DbInfrastructure dbInfrastructure = TestHelper.ConnectToDatabase ())
			{
				DbConnectionManager manager = dbInfrastructure.ConnectionManager;

				ExceptionAssert.Throw<System.ArgumentException>
				(
					() => manager.CloseConnection (-1)
				);
			}
		}


		[TestMethod]
		public void CloseConnectionInvalidBehavior()
		{
			using (DbInfrastructure dbInfrastructure = TestHelper.ConnectToDatabase ())
			{
				DbConnectionManager manager = new DbConnectionManager (System.TimeSpan.FromSeconds (2));

				manager.Attach (dbInfrastructure, dbInfrastructure.ResolveDbTable (Tags.TableConnection));

				long connectionId1 = manager.OpenConnection ("connection1");

				manager.CloseConnection (connectionId1);

				ExceptionAssert.Throw<System.InvalidOperationException>
				(
					() => manager.CloseConnection (connectionId1)
				);

				ExceptionAssert.Throw<System.InvalidOperationException>
				(
					() => manager.CloseConnection (1)
				);

				long connectionId2 = manager.OpenConnection ("connection2");

				System.Threading.Thread.Sleep (3000);

				manager.InterruptDeadConnections ();

				ExceptionAssert.Throw<System.InvalidOperationException>
				(
					() => manager.CloseConnection (connectionId2)
				);

				manager.Detach ();
			}
		}


		[TestMethod]
		public void GetConnectionIdentityArgumentCheck()
		{
			using (DbInfrastructure dbInfrastructure = TestHelper.ConnectToDatabase ())
			{
				DbConnectionManager manager = dbInfrastructure.ConnectionManager;

				ExceptionAssert.Throw<System.ArgumentException>
				(
					() => manager.GetConnectionIdentity (-1)
				);
			}
		}


		[TestMethod]
		public void GetConnectionIdentityInvalidBehavior()
		{
			using (DbInfrastructure dbInfrastructure = TestHelper.ConnectToDatabase ())
			{
				DbConnectionManager manager = dbInfrastructure.ConnectionManager;

				ExceptionAssert.Throw<System.InvalidOperationException>
				(
					() => manager.GetConnectionIdentity (1)
				);
			}
		}


		[TestMethod]
		public void GetConnectionStatusArgumentCheck()
		{
			using (DbInfrastructure dbInfrastructure = TestHelper.ConnectToDatabase ())
			{
				DbConnectionManager manager = dbInfrastructure.ConnectionManager;

				ExceptionAssert.Throw<System.ArgumentException>
				(
					() => manager.GetConnectionStatus (-1)
				);
			}
		}


		[TestMethod]
		public void GetConnectionStatusInvalidBehavior()
		{
			using (DbInfrastructure dbInfrastructure = TestHelper.ConnectToDatabase ())
			{
				DbConnectionManager manager = dbInfrastructure.ConnectionManager;

				ExceptionAssert.Throw<System.InvalidOperationException>
				(
					() => manager.GetConnectionStatus (1)
				);
			}
		}


		[TestMethod]
		public void OpenAndCloseConnection()
		{
			using (DbInfrastructure dbInfrastructure = TestHelper.ConnectToDatabase ())
			{
				DbConnectionManager manager = dbInfrastructure.ConnectionManager;

				long connectionId = manager.OpenConnection ("connection");

				Assert.AreEqual ("connection", manager.GetConnectionIdentity (connectionId));
				Assert.AreEqual (DbConnectionStatus.Opened, manager.GetConnectionStatus (connectionId));

				manager.CloseConnection (connectionId);

				Assert.AreEqual ("connection", manager.GetConnectionIdentity (connectionId));
				Assert.AreEqual (DbConnectionStatus.Closed, manager.GetConnectionStatus (connectionId));
			}
		}


		[TestMethod]
		public void ConnectionExistsArgumentCheck()
		{
			using (DbInfrastructure dbInfrastructure = TestHelper.ConnectToDatabase ())
			{
				DbConnectionManager manager = dbInfrastructure.ConnectionManager;

				ExceptionAssert.Throw<System.ArgumentException>
				(
					() => manager.ConnectionExists (-1)
				);
			}
		}


		[TestMethod]
		public void ConnectionExists()
		{
			using (DbInfrastructure dbInfrastructure = TestHelper.ConnectToDatabase ())
			{
				DbConnectionManager manager = dbInfrastructure.ConnectionManager;

				Assert.IsFalse (manager.ConnectionExists (0));

				long connectionId = manager.OpenConnection ("connection");

				Assert.IsTrue (manager.ConnectionExists (connectionId));

				manager.CloseConnection (connectionId);

				Assert.IsTrue (manager.ConnectionExists (connectionId));
			}
		}


		[TestMethod]
		public void KeepConnectionAliveArgumentCheck()
		{
			using (DbInfrastructure dbInfrastructure = TestHelper.ConnectToDatabase ())
			{
				DbConnectionManager manager = dbInfrastructure.ConnectionManager;

				ExceptionAssert.Throw<System.ArgumentException>
				(
					() => manager.KeepConnectionAlive (-1)
				);
			}
		}


		[TestMethod]
		public void KeepConnectionAliveInvalidBehavior()
		{
			using (DbInfrastructure dbInfrastructure = TestHelper.ConnectToDatabase ())
			{
				DbConnectionManager manager = new DbConnectionManager (System.TimeSpan.FromSeconds (2));

				manager.Attach (dbInfrastructure, dbInfrastructure.ResolveDbTable (Tags.TableConnection));

				long connectionId1 = manager.OpenConnection ("connection1");

				manager.CloseConnection (connectionId1);

				ExceptionAssert.Throw<System.InvalidOperationException>
				(
					() => manager.KeepConnectionAlive (connectionId1)
				);

				ExceptionAssert.Throw<System.InvalidOperationException>
				(
					() => manager.KeepConnectionAlive (1)
				);
				long connectionId2 = manager.OpenConnection ("connection2");

				System.Threading.Thread.Sleep (3000);

				manager.InterruptDeadConnections ();

				ExceptionAssert.Throw<System.InvalidOperationException>
				(
					() => manager.KeepConnectionAlive (connectionId2)
				);

				manager.Detach ();
			}
		}


		[TestMethod]
		public void KeepConnectionAlive()
		{
			using (DbInfrastructure dbInfrastructure = TestHelper.ConnectToDatabase ())
			{
				DbConnectionManager manager = dbInfrastructure.ConnectionManager;

				long connectionId = manager.OpenConnection ("connection");

				List<System.DateTime> lastSeenValues = new List<System.DateTime> ();
				
				lastSeenValues.Add (manager.GetConnectionLastSeen (connectionId));

				for (int i = 0; i < 10; i++)
				{
					System.Threading.Thread.Sleep (500);

					manager.KeepConnectionAlive (connectionId);

					lastSeenValues.Add (manager.GetConnectionLastSeen (connectionId));
				}

				Assert.IsTrue (lastSeenValues.First () == manager.GetConnectionSince (connectionId));

				for (int i = 0; i < lastSeenValues.Count - 1; i++)
				{
					Assert.IsTrue (lastSeenValues[i] < lastSeenValues[i + 1]);
				}
				
				manager.CloseConnection (connectionId);
			}
		}


		[TestMethod]
		public void GetConnectionSinceArgumentCheck()
		{
			using (DbInfrastructure dbInfrastructure = TestHelper.ConnectToDatabase ())
			{
				DbConnectionManager manager = dbInfrastructure.ConnectionManager;

				ExceptionAssert.Throw<System.ArgumentException>
				(
					() => manager.GetConnectionSince (-1)
				);
			}
		}


		[TestMethod]
		public void GetConnectionSinceInvalidBehavior()
		{
			using (DbInfrastructure dbInfrastructure = TestHelper.ConnectToDatabase ())
			{
				DbConnectionManager manager = dbInfrastructure.ConnectionManager;

				ExceptionAssert.Throw<System.InvalidOperationException>
				(
					() => manager.GetConnectionSince (1)
				);
			}
		}


		[TestMethod]
		public void GetConnectionLastSeenArgumentCheck()
		{
			using (DbInfrastructure dbInfrastructure = TestHelper.ConnectToDatabase ())
			{
				DbConnectionManager manager = dbInfrastructure.ConnectionManager;

				ExceptionAssert.Throw<System.ArgumentException>
				(
					() => manager.GetConnectionLastSeen (-1)
				);
			}
		}


		[TestMethod]
		public void GetConnectionLastSeenInvalidBehavior()
		{
			using (DbInfrastructure dbInfrastructure = TestHelper.ConnectToDatabase ())
			{
				DbConnectionManager manager = dbInfrastructure.ConnectionManager;

				ExceptionAssert.Throw<System.InvalidOperationException>
				(
					() => manager.GetConnectionLastSeen (1)
				);
			}
		}


		[TestMethod]
		public void InterruptDeadConnections()
		{
			using (DbInfrastructure dbInfrastructure = TestHelper.ConnectToDatabase ())
			{
				DbConnectionManager manager = new DbConnectionManager (System.TimeSpan.FromSeconds (5));

				manager.Attach (dbInfrastructure, dbInfrastructure.ResolveDbTable (Tags.TableConnection));

				long connectionId1 = manager.OpenConnection ("connection1");
				long connectionId2 = manager.OpenConnection ("connection2");
				long connectionId3 = manager.OpenConnection ("connection3");

				manager.CloseConnection (connectionId1);

				for (int i = 0; i < 5; i++)
				{
					manager.KeepConnectionAlive (connectionId2);

					System.Threading.Thread.Sleep (1000);
				}

				System.Threading.Thread.Sleep (1000);

				Assert.AreEqual (true, manager.InterruptDeadConnections ());

				Assert.AreEqual (DbConnectionStatus.Closed, manager.GetConnectionStatus (connectionId1));
				Assert.AreEqual (DbConnectionStatus.Opened, manager.GetConnectionStatus (connectionId2));
				Assert.AreEqual (DbConnectionStatus.Interrupted, manager.GetConnectionStatus (connectionId3));

				manager.CloseConnection (connectionId2);

				manager.Detach ();
			}
		}


	}


}
