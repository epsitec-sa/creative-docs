using Epsitec.Common.UnitTesting;

using Epsitec.Cresus.Database;
using Epsitec.Cresus.Database.UnitTests.Helpers;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.Collections.Generic;
using System.Linq;


namespace Cresus.Database.UnitTests
{


	[TestClass]
	public class UnitTestDbConnexionManager
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
					() => new DbConnexionManager (System.TimeSpan.FromSeconds (1)).Attach (null, new DbTable ())
				);

				ExceptionAssert.Throw<System.ArgumentNullException>
				(
					() => new DbConnexionManager (System.TimeSpan.FromSeconds (1)).Attach (dbInfrastructure, null)
				);
			}
		}


		[TestMethod]
		public void AttachAndDetach()
		{
			using (DbInfrastructure dbInfrastructure = TestHelper.ConnectToDatabase ())
			{
				Assert.IsNotNull (dbInfrastructure.ConnexionManager);
			}
		}


		[TestMethod]
		public void OpenConnexionArgumentCheck()
		{
			using (DbInfrastructure dbInfrastructure = TestHelper.ConnectToDatabase ())
			{
				DbConnexionManager manager = dbInfrastructure.ConnexionManager;

				ExceptionAssert.Throw<System.ArgumentException>
				(
					() => manager.OpenConnexion (null)
				);

				ExceptionAssert.Throw<System.ArgumentException>
				(
					() => manager.OpenConnexion ("")
				);
			}
		}


		[TestMethod]
		public void CloseConnexionArgumentCheck()
		{
			using (DbInfrastructure dbInfrastructure = TestHelper.ConnectToDatabase ())
			{
				DbConnexionManager manager = dbInfrastructure.ConnexionManager;

				ExceptionAssert.Throw<System.ArgumentException>
				(
					() => manager.CloseConnexion (-1)
				);
			}
		}


		[TestMethod]
		public void GetConnexionIdentityArgumentCheck()
		{
			using (DbInfrastructure dbInfrastructure = TestHelper.ConnectToDatabase ())
			{
				DbConnexionManager manager = dbInfrastructure.ConnexionManager;

				ExceptionAssert.Throw<System.ArgumentException>
				(
					() => manager.GetConnexionIdentity (-1)
				);
			}
		}


		[TestMethod]
		public void GetConnexionStatusArgumentCheck()
		{
			using (DbInfrastructure dbInfrastructure = TestHelper.ConnectToDatabase ())
			{
				DbConnexionManager manager = dbInfrastructure.ConnexionManager;

				ExceptionAssert.Throw<System.ArgumentException>
				(
					() => manager.GetConnexionStatus (-1)
				);
			}
		}


		[TestMethod]
		public void OpenAndCloseConnexion()
		{
			using (DbInfrastructure dbInfrastructure = TestHelper.ConnectToDatabase ())
			{
				DbConnexionManager manager = dbInfrastructure.ConnexionManager;

				long connexionId = manager.OpenConnexion ("connexion");

				Assert.AreEqual ("connexion", manager.GetConnexionIdentity (connexionId));
				Assert.AreEqual (DbConnexionStatus.Opened, manager.GetConnexionStatus (connexionId));

				manager.CloseConnexion (connexionId);

				Assert.AreEqual ("connexion", manager.GetConnexionIdentity (connexionId));
				Assert.AreEqual (DbConnexionStatus.Closed, manager.GetConnexionStatus (connexionId));
			}
		}


		[TestMethod]
		public void KeepConnexionAliveArgumentCheck()
		{
			using (DbInfrastructure dbInfrastructure = TestHelper.ConnectToDatabase ())
			{
				DbConnexionManager manager = dbInfrastructure.ConnexionManager;

				ExceptionAssert.Throw<System.ArgumentException>
				(
					() => manager.KeepConnexionAlive (-1)
				);
			}
		}


		[TestMethod]
		public void KeepConnexionAlive()
		{
			using (DbInfrastructure dbInfrastructure = TestHelper.ConnectToDatabase ())
			{
				DbConnexionManager manager = dbInfrastructure.ConnexionManager;

				long connexionId = manager.OpenConnexion ("connexion");

				List<System.DateTime> lastSeenValues = new List<System.DateTime> ();
				
				lastSeenValues.Add (manager.GetConnexionLastSeen (connexionId));

				for (int i = 0; i < 10; i++)
				{
					System.Threading.Thread.Sleep (500);

					manager.KeepConnexionAlive (connexionId);

					lastSeenValues.Add (manager.GetConnexionLastSeen (connexionId));
				}

				Assert.IsTrue (lastSeenValues.First () == manager.GetConnexionSince (connexionId));

				for (int i = 0; i < lastSeenValues.Count - 1; i++)
				{
					Assert.IsTrue (lastSeenValues[i] < lastSeenValues[i + 1]);
				}
				
				manager.CloseConnexion (connexionId);
			}
		}


		[TestMethod]
		public void GetConnexionSinceArgumentCheck()
		{
			using (DbInfrastructure dbInfrastructure = TestHelper.ConnectToDatabase ())
			{
				DbConnexionManager manager = dbInfrastructure.ConnexionManager;

				ExceptionAssert.Throw<System.ArgumentException>
				(
					() => manager.GetConnexionSince (-1)
				);
			}
		}


		[TestMethod]
		public void GetConnexionLastSeenArgumentCheck()
		{
			using (DbInfrastructure dbInfrastructure = TestHelper.ConnectToDatabase ())
			{
				DbConnexionManager manager = dbInfrastructure.ConnexionManager;

				ExceptionAssert.Throw<System.ArgumentException>
				(
					() => manager.GetConnexionLastSeen (-1)
				);
			}
		}


		[TestMethod]
		public void InterruptDeadConnexions()
		{
			using (DbInfrastructure dbInfrastructure = TestHelper.ConnectToDatabase ())
			{
				DbConnexionManager manager = new DbConnexionManager (System.TimeSpan.FromSeconds (5));

				manager.Attach (dbInfrastructure, dbInfrastructure.ResolveDbTable (Tags.TableConnexion));

				long connexionId1 = manager.OpenConnexion ("connexion1");
				long connexionId2 = manager.OpenConnexion ("connexion2");
				long connexionId3 = manager.OpenConnexion ("connexion3");

				manager.CloseConnexion (connexionId1);

				for (int i = 0; i < 5; i++)
				{
					manager.KeepConnexionAlive (connexionId2);

					System.Threading.Thread.Sleep (1000);
				}

				System.Threading.Thread.Sleep (1000);

				manager.InterruptDeadConnexions ();

				Assert.AreEqual (DbConnexionStatus.Closed, manager.GetConnexionStatus (connexionId1));
				Assert.AreEqual (DbConnexionStatus.Opened, manager.GetConnexionStatus (connexionId2));
				Assert.AreEqual (DbConnexionStatus.Interrupted, manager.GetConnexionStatus (connexionId3));

				manager.CloseConnexion (connexionId2);

				manager.Detach ();
			}
		}


	}


}
