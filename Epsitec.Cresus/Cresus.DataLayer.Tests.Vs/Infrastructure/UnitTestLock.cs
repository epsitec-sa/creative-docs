using Epsitec.Common.UnitTesting;

using Epsitec.Cresus.Database;

using Epsitec.Cresus.DataLayer.Infrastructure;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Epsitec.Cresus.DataLayer.Tests.Vs.Infrastructure
{


	[TestClass]
	public sealed class UnitTestLock
	{


		[TestMethod]
		public void ConstructorArgumentCheck()
		{
			Connection owner = new Connection (new DbId (1), "identity", ConnectionStatus.Open, System.DateTime.Now, System.DateTime.Now);

			ExceptionAssert.Throw<System.ArgumentException>
			(
				() => new Lock (null, "lock name", System.DateTime.Now)
			);

			ExceptionAssert.Throw<System.ArgumentException>
			(
				() => new Lock (owner, null, System.DateTime.Now)
			);

			ExceptionAssert.Throw<System.ArgumentException>
			(
				() => new Lock (owner, "", System.DateTime.Now)
			);
		}


		[TestMethod]
		public void ConstructorAndPropertiesTest()
		{
			var owner = new Connection (new DbId (1), "identity", ConnectionStatus.Open, System.DateTime.Now, System.DateTime.Now);
			var name = "lock name";
			var creationTime = System.DateTime.Now;

			var l = new Lock (owner, name, creationTime);

			Assert.AreEqual (owner, l.Owner);
			Assert.AreEqual (name, l.Name);
			Assert.AreEqual (creationTime, l.CreationTime);
		}


	}


}
