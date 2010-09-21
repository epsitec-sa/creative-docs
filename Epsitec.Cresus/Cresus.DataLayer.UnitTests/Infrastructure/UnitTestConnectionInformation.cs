using Epsitec.Common.UnitTesting;

using Epsitec.Cresus.Database;

using Epsitec.Cresus.DataLayer.Infrastructure;
using Epsitec.Cresus.DataLayer.UnitTests.Helpers;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.Collections.Generic;


namespace Epsitec.Cresus.DataLayer.UnitTests.Infrastructure
{


	[TestClass]
	public sealed class UnitTestConnectionInformation
	{


		[ClassInitialize]
		public static void ClassInitialize(TestContext testContext)
		{
			TestHelper.Initialize ();
		}


		[TestInitialize]
		public void TestInitialize()
		{
			DatabaseHelper.CreateAndConnectToDatabase ();
		}


		[TestCleanup]
		public static void TestCleanup()
		{
			DatabaseHelper.DisconnectFromDatabase ();
		}


		[TestMethod]
		public void ConnectionInformationConstructorArgumentCheck()
		{
			DbInfrastructure dbInfrastructure = DatabaseHelper.DbInfrastructure;
			string connectionIdentity = "connexion";

			ExceptionAssert.Throw<System.ArgumentNullException>
			(
				() => new ConnectionInformation (null, connectionIdentity)
			);

			ExceptionAssert.Throw<System.ArgumentException>
			(
				() => new ConnectionInformation (dbInfrastructure, null)
			);

			ExceptionAssert.Throw<System.ArgumentException>
			(
				() => new ConnectionInformation (dbInfrastructure, "")
			);
		}


		[TestMethod]
		public void SimpleCase()
		{
			DbInfrastructure dbInfrastructure = DatabaseHelper.DbInfrastructure;

			ConnectionInformation connection = new ConnectionInformation (dbInfrastructure, "connexion");

			Assert.AreEqual (ConnectionStatus.NotYetOpen, connection.Status);
			connection.RefreshStatus ();
			Assert.AreEqual (ConnectionStatus.NotYetOpen, connection.Status);

			connection.Open ();

			Assert.AreEqual (ConnectionStatus.Open, connection.Status);
			connection.RefreshStatus ();
			Assert.AreEqual (ConnectionStatus.Open, connection.Status);

			connection.Close ();

			Assert.AreEqual (ConnectionStatus.Closed, connection.Status);
			connection.RefreshStatus ();
			Assert.AreEqual (ConnectionStatus.Closed, connection.Status);
		}


		[TestMethod]
		public void InterruptionCase()
		{
			DbInfrastructure dbInfrastructure = DatabaseHelper.DbInfrastructure;

			ConnectionInformation connection = new ConnectionInformation (dbInfrastructure, "connexion");
			Assert.AreEqual (ConnectionStatus.NotYetOpen, connection.Status);

			connection.Open ();
			Assert.AreEqual (ConnectionStatus.Open, connection.Status);

			System.Threading.Thread.Sleep (3000);

			connection.RefreshStatus ();
			Assert.AreEqual (ConnectionStatus.Open, connection.Status);

			ConnectionInformation.InterruptDeadConnections (dbInfrastructure, System.TimeSpan.FromSeconds (2));
			Assert.AreEqual (ConnectionStatus.Open, connection.Status);
			
			connection.RefreshStatus ();
			Assert.AreEqual (ConnectionStatus.Interrupted, connection.Status);
		}


	}


}
