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
					() => new DbConnectionManager ().Attach (null, new DbTable ())
				);

				ExceptionAssert.Throw<System.ArgumentNullException>
				(
					() => new DbConnectionManager ().Attach (dbInfrastructure, null)
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
				DbConnectionManager manager = new DbConnectionManager ();

				manager.Attach (dbInfrastructure, dbInfrastructure.ResolveDbTable (Tags.TableConnection));

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

				manager.Detach ();
			}
		}


		[TestMethod]
		public void OpenAndCloseConnection()
		{
			using (DbInfrastructure dbInfrastructure = TestHelper.ConnectToDatabase ())
			{
				DbConnectionManager manager = dbInfrastructure.ConnectionManager;

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

				DbId connectionId = manager.OpenConnection ("connection").Id;

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
				DbConnectionManager manager = new DbConnectionManager ();

				manager.Attach (dbInfrastructure, dbInfrastructure.ResolveDbTable (Tags.TableConnection));

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

				manager.Detach ();
			}
		}


		[TestMethod]
		public void KeepConnectionAlive()
		{
			using (DbInfrastructure dbInfrastructure = TestHelper.ConnectToDatabase ())
			{
				DbConnectionManager manager = dbInfrastructure.ConnectionManager;

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
			using (DbInfrastructure dbInfrastructure = TestHelper.ConnectToDatabase ())
			{
				DbConnectionManager manager = new DbConnectionManager ();

				manager.Attach (dbInfrastructure, dbInfrastructure.ResolveDbTable (Tags.TableConnection));

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

				manager.Detach ();
			}
		}


		[TestMethod]
		public void GetOpenedConnections()
		{
			using (DbInfrastructure dbInfrastructure = TestHelper.ConnectToDatabase ())
			{
				DbConnectionManager manager = new DbConnectionManager ();

				manager.Attach (dbInfrastructure, dbInfrastructure.ResolveDbTable (Tags.TableConnection));

				List<DbId> connectionIds = new List<DbId> ();

				this.CheckSetAreSame (connectionIds, manager.GetOpenConnections ().Select (c => c.Id).ToList ());

				DbId connectionId1 = manager.OpenConnection ("connection1").Id;
				connectionIds.Add (connectionId1);
				this.CheckSetAreSame (connectionIds, manager.GetOpenConnections ().Select (c => c.Id).ToList ());

				DbId connectionId2 = manager.OpenConnection ("connection2").Id;
				connectionIds.Add (connectionId2);
				this.CheckSetAreSame (connectionIds, manager.GetOpenConnections ().Select (c => c.Id).ToList ());

				DbId connectionId3 = manager.OpenConnection ("connection3").Id;
				connectionIds.Add (connectionId3);
				this.CheckSetAreSame (connectionIds, manager.GetOpenConnections ().Select (c => c.Id).ToList ());

				manager.CloseConnection (connectionId1);
				connectionIds.Remove (connectionId1);
				this.CheckSetAreSame (connectionIds, manager.GetOpenConnections ().Select (c => c.Id).ToList ());

				for (int i = 0; i < 5; i++)
				{
					manager.KeepConnectionAlive (connectionId2);

					System.Threading.Thread.Sleep (1000);
				}

				System.Threading.Thread.Sleep (1000);

				Assert.AreEqual (true, manager.InterruptDeadConnections (System.TimeSpan.FromSeconds (5)));
				connectionIds.Remove (connectionId3);
				this.CheckSetAreSame (connectionIds, manager.GetOpenConnections ().Select (c => c.Id).ToList ());

				Assert.AreEqual (DbConnectionStatus.Closed, manager.GetConnection (connectionId1).Status);
				Assert.AreEqual (DbConnectionStatus.Open, manager.GetConnection (connectionId2).Status);
				Assert.AreEqual (DbConnectionStatus.Interrupted, manager.GetConnection (connectionId3).Status);

				manager.CloseConnection (connectionId2);
				connectionIds.Remove (connectionId2);
				this.CheckSetAreSame (connectionIds, manager.GetOpenConnections ().Select (c => c.Id).ToList ());

				manager.Detach ();
			}
		}


		private void CheckSetAreSame(List<DbId> expected, List<DbId> actual)
		{
			Assert.AreEqual (expected.Count, actual.Count);

			foreach (long l in expected)
			{
				CollectionAssert.Contains (actual, l);
			}
		}


	}


}
