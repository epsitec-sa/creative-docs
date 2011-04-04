using Epsitec.Common.UnitTesting;

using Epsitec.Cresus.Database;

using Epsitec.Cresus.DataLayer.Infrastructure;
using Epsitec.Cresus.DataLayer.Tests.Vs.Helpers;

using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace Epsitec.Cresus.DataLayer.Tests.Vs.Infrastructure
{


	[TestClass]
	public sealed class UnitTestConnection
	{


		[ClassInitialize]
		public static void ClassInitialize(TestContext testContext)
		{
			TestHelper.Initialize ();
		}


		[TestMethod]
		public void ConstructorArgumentCheck()
		{
			ExceptionAssert.Throw<System.ArgumentException>
			(
				() => new Connection (DbId.Empty, "identity", ConnectionStatus.Interrupted, System.DateTime.Now, System.DateTime.Now)
			);

			ExceptionAssert.Throw<System.ArgumentException>
			(
				() => new Connection (new DbId(2), null, ConnectionStatus.Interrupted, System.DateTime.Now, System.DateTime.Now)
			);
		}


		[TestMethod]
		public void ConstructorAndPropertiesTest()
		{
			var id = new DbId (5432);
			var identity = "identity";
			var status = ConnectionStatus.Closed;
			var establishementTime = System.DateTime.Now + System.TimeSpan.FromMilliseconds (5433);
			var refreshTime = System.DateTime.Now + System.TimeSpan.FromMilliseconds (90867);

			var connection = new Connection (id, identity, status, establishementTime, refreshTime);

			Assert.AreEqual (id, connection.Id);
			Assert.AreEqual (identity, connection.Identity);
			Assert.AreEqual (status, connection.Status);
			Assert.AreEqual (establishementTime, connection.EstablishmentTime);
			Assert.AreEqual (refreshTime, connection.RefreshTime);
		}


	}


}
