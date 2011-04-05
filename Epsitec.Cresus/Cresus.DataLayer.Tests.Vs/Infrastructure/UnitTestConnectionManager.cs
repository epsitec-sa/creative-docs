using Epsitec.Common.Support.Extensions;

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
    public sealed class UnitTestConnectionManager
    {


		[ClassInitialize]
		public static void ClassInitialize(TestContext testContext)
		{
			TestHelper.Initialize ();
		}


		[TestInitialize]
		public void TestInitialize()
		{
			DatabaseCreator2.ResetEmptyTestDatabase ();
		}


		[TestMethod]
		public void ConstructorArgumentCheck()
		{
			using (DbInfrastructure dbinfrastructure = DbInfrastructureHelper.ConnectToTestDatabase ())
			{
				EntityEngine entityEngine = EntityEngineHelper.ConnectToTestDatabase ();

				ExceptionAssert.Throw<System.ArgumentNullException>
				(
					() => new ConnectionManager (null, entityEngine.ServiceSchemaEngine)
				);

				ExceptionAssert.Throw<System.ArgumentNullException>
				(
					() => new ConnectionManager (dbinfrastructure, null)
				);
			}
		}


		[TestMethod]
		public void OpenConnectionArgumentCheck()
		{
			using (DbInfrastructure dbinfrastructure = DbInfrastructureHelper.ConnectToTestDatabase ())
			{
				EntityEngine entityEngine = EntityEngineHelper.ConnectToTestDatabase ();

				ConnectionManager manager = new ConnectionManager (dbinfrastructure, entityEngine.ServiceSchemaEngine);

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
			using (DbInfrastructure dbinfrastructure = DbInfrastructureHelper.ConnectToTestDatabase ())
			{
				EntityEngine entityEngine = EntityEngineHelper.ConnectToTestDatabase ();

				ConnectionManager manager = new ConnectionManager (dbinfrastructure, entityEngine.ServiceSchemaEngine);

				ExceptionAssert.Throw<System.ArgumentException>
				(
					() => manager.CloseConnection (DbId.Empty)
				);
			}
		}


		[TestMethod]
		public void GetConnectionArgumentCheck()
		{
			using (DbInfrastructure dbinfrastructure = DbInfrastructureHelper.ConnectToTestDatabase ())
			{
				EntityEngine entityEngine = EntityEngineHelper.ConnectToTestDatabase ();

				ConnectionManager manager = new ConnectionManager (dbinfrastructure, entityEngine.ServiceSchemaEngine);

				ExceptionAssert.Throw<System.ArgumentException>
				(
					() => manager.GetConnection (DbId.Empty)
				);
			}
		}


		[TestMethod]
		public void KeepConnectionAliveArgumentCheck()
		{
			using (DbInfrastructure dbinfrastructure = DbInfrastructureHelper.ConnectToTestDatabase ())
			{
				EntityEngine entityEngine = EntityEngineHelper.ConnectToTestDatabase ();

				ConnectionManager manager = new ConnectionManager (dbinfrastructure, entityEngine.ServiceSchemaEngine);

				ExceptionAssert.Throw<System.ArgumentException>
				(
					() => manager.KeepConnectionAlive (DbId.Empty)
				);
			}
		}


		[TestMethod]
		public void CloseConnectionInvalidBehavior()
		{
			using (DbInfrastructure dbinfrastructure = DbInfrastructureHelper.ConnectToTestDatabase ())
			{
				EntityEngine entityEngine = EntityEngineHelper.ConnectToTestDatabase ();

				ConnectionManager manager = new ConnectionManager (dbinfrastructure, entityEngine.ServiceSchemaEngine);

				DbId connectionId1 = manager.OpenConnection ("connection1").Id;

				manager.CloseConnection (connectionId1);

				ExceptionAssert.Throw<System.InvalidOperationException>
				(
					() => manager.CloseConnection (connectionId1)
				);

				ExceptionAssert.Throw<System.InvalidOperationException>
				(
					() => manager.CloseConnection (new DbId (2))
				);

				DbId connectionId2 = manager.OpenConnection ("connection2").Id;

				System.Threading.Thread.Sleep (3000);

				Assert.IsTrue (manager.KillDeadConnections (System.TimeSpan.FromSeconds (2)));

				ExceptionAssert.Throw<System.InvalidOperationException>
				(
					() => manager.CloseConnection (connectionId2)
				);
			}
		}


		[TestMethod]
		public void OpenAndCloseConnection()
		{
			using (DbInfrastructure dbinfrastructure = DbInfrastructureHelper.ConnectToTestDatabase ())
			{
				EntityEngine entityEngine = EntityEngineHelper.ConnectToTestDatabase ();

				ConnectionManager manager = new ConnectionManager (dbinfrastructure, entityEngine.ServiceSchemaEngine);

				DbId connectionId = manager.OpenConnection ("connection").Id;

				Assert.AreEqual ("connection", manager.GetConnection (connectionId).Identity);
				Assert.AreEqual (ConnectionStatus.Open, manager.GetConnection (connectionId).Status);

				manager.CloseConnection (connectionId);

				Assert.AreEqual ("connection", manager.GetConnection (connectionId).Identity);
				Assert.AreEqual (ConnectionStatus.Closed, manager.GetConnection (connectionId).Status);
			}
		}


		[TestMethod]
		public void KeepConnectionAliveInvalidBehavior()
		{
			using (DbInfrastructure dbinfrastructure = DbInfrastructureHelper.ConnectToTestDatabase ())
			{
				EntityEngine entityEngine = EntityEngineHelper.ConnectToTestDatabase ();

				ConnectionManager manager = new ConnectionManager (dbinfrastructure, entityEngine.ServiceSchemaEngine);

				DbId connectionId1 = manager.OpenConnection ("connection1").Id;

				manager.CloseConnection (connectionId1);

				ExceptionAssert.Throw<System.InvalidOperationException>
				(
					() => manager.KeepConnectionAlive (connectionId1)
				);

				ExceptionAssert.Throw<System.InvalidOperationException>
				(
					() => manager.KeepConnectionAlive (new DbId (2))
				);

				DbId connectionId2 = manager.OpenConnection ("connection2").Id;

				System.Threading.Thread.Sleep (3000);

				Assert.IsTrue (manager.KillDeadConnections (System.TimeSpan.FromSeconds (2)));

				ExceptionAssert.Throw<System.InvalidOperationException>
				(
					() => manager.KeepConnectionAlive (connectionId2)
				);
			}
		}


		[TestMethod]
		public void KeepConnectionAlive()
		{
			using (DbInfrastructure dbinfrastructure = DbInfrastructureHelper.ConnectToTestDatabase ())
			{
				EntityEngine entityEngine = EntityEngineHelper.ConnectToTestDatabase ();

				ConnectionManager manager = new ConnectionManager (dbinfrastructure, entityEngine.ServiceSchemaEngine);

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
		public void KillDeadConnections()
		{
			using (DbInfrastructure dbinfrastructure = DbInfrastructureHelper.ConnectToTestDatabase ())
			{
				EntityEngine entityEngine = EntityEngineHelper.ConnectToTestDatabase ();

				ConnectionManager manager = new ConnectionManager (dbinfrastructure, entityEngine.ServiceSchemaEngine);

				DbId connectionId1 = manager.OpenConnection ("connection1").Id;
				DbId connectionId2 = manager.OpenConnection ("connection2").Id;
				DbId connectionId3 = manager.OpenConnection ("connection3").Id;

				manager.CloseConnection (connectionId1);

				for (int i = 0; i < 3; i++)
				{
					manager.KeepConnectionAlive (connectionId2);

					System.Threading.Thread.Sleep (1000);

					Assert.AreEqual (false, manager.KillDeadConnections (System.TimeSpan.FromSeconds (5)));
				}

				Assert.AreEqual (ConnectionStatus.Closed, manager.GetConnection (connectionId1).Status);
				Assert.AreEqual (ConnectionStatus.Open, manager.GetConnection (connectionId2).Status);
				Assert.AreEqual (ConnectionStatus.Open, manager.GetConnection (connectionId3).Status);

				System.Threading.Thread.Sleep (2000);

				Assert.AreEqual (true, manager.KillDeadConnections (System.TimeSpan.FromSeconds (5)));

				Assert.AreEqual (ConnectionStatus.Closed, manager.GetConnection (connectionId1).Status);
				Assert.AreEqual (ConnectionStatus.Open, manager.GetConnection (connectionId2).Status);
				Assert.AreEqual (ConnectionStatus.Interrupted, manager.GetConnection (connectionId3).Status);

				manager.CloseConnection (connectionId2);
			}
		}


		[TestMethod]
		public void GetOpenConnections()
		{
			using (DbInfrastructure dbinfrastructure = DbInfrastructureHelper.ConnectToTestDatabase ())
			{
				EntityEngine entityEngine = EntityEngineHelper.ConnectToTestDatabase ();

				ConnectionManager manager = new ConnectionManager (dbinfrastructure, entityEngine.ServiceSchemaEngine);

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

				Assert.AreEqual (true, manager.KillDeadConnections (System.TimeSpan.FromSeconds (5)));
				connectionIds.Remove (connectionId3);
				Assert.IsTrue (connectionIds.SetEquals (manager.GetOpenConnections ().Select (c => c.Id)));

				Assert.AreEqual (ConnectionStatus.Closed, manager.GetConnection (connectionId1).Status);
				Assert.AreEqual (ConnectionStatus.Open, manager.GetConnection (connectionId2).Status);
				Assert.AreEqual (ConnectionStatus.Interrupted, manager.GetConnection (connectionId3).Status);

				manager.CloseConnection (connectionId2);
				connectionIds.Remove (connectionId2);
				Assert.IsTrue (connectionIds.SetEquals (manager.GetOpenConnections ().Select (c => c.Id)));
			}
		}


		[TestMethod]
		public void Concurrency()
		{
			int nbThreads = 100;

			var entityEngine = EntityEngineHelper.ConnectToTestDatabase ();

			var dbInfrastructures = Enumerable.Range (0, nbThreads)
				.Select (i => DbInfrastructureHelper.ConnectToTestDatabase ())
				.ToList ();

			try
			{
				System.DateTime time = System.DateTime.Now;

				var threads = dbInfrastructures.Select (d => new System.Threading.Thread (() =>
				{
					var dice = new System.Random (System.Threading.Thread.CurrentThread.ManagedThreadId);

					var connectionManager = new ConnectionManager (d, entityEngine.ServiceSchemaEngine);

					while (System.DateTime.Now - time <= System.TimeSpan.FromSeconds (15))
					{
						var connection = connectionManager.OpenConnection (System.Guid.NewGuid ().ToString ());

						if (dice.NextDouble () > 0.5)
						{
							connectionManager.KeepConnectionAlive (connection.Id);
						}

						if (dice.NextDouble () > 0.5)
						{
							connectionManager.CloseConnection (connection.Id);
						}

						if (dice.NextDouble () > 0.9)
						{
							connectionManager.KillDeadConnections (System.TimeSpan.FromSeconds (1));
						}
					}
				})).ToList ();

				foreach (var thread in threads)
				{
					thread.Start ();
				}

				foreach (var thread in threads)
				{
					thread.Join ();
				}
			}
			finally
			{
				foreach (var dbInfrastructure in dbInfrastructures)
				{
					dbInfrastructure.Dispose ();
				}
			}
		}


	}


}
