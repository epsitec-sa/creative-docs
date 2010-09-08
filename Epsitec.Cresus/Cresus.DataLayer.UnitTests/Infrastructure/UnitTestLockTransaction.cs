using Epsitec.Common.UnitTesting;

using Epsitec.Cresus.Database;

using Epsitec.Cresus.DataLayer.Infrastructure;
using Epsitec.Cresus.DataLayer.UnitTests.Helpers;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.Collections.Generic;

using System.Linq;


namespace Epsitec.Cresus.DataLayer.UnitTests.Infrastructure
{


	[TestClass]
	public sealed class UnitTestLockTransaction
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
		public void LockTransactionConstructorArgumentCheck()
		{
			DbInfrastructure dbInfrastructure = DatabaseHelper.DbInfrastructure;
			List<string> lockNames = new List<string> ()
		    {
		        "myLock1",
		        "myLock2",
		        "myLock3",
		    };
			long connectionId = 0;

			ExceptionAssert.Throw<System.ArgumentNullException>
			(
				() => new LockTransaction (null, connectionId, lockNames)
			);

			ExceptionAssert.Throw<System.ArgumentNullException>
			(
				() => new LockTransaction (dbInfrastructure, connectionId, null)
			);

			ExceptionAssert.Throw<System.ArgumentException>
			(
				() => new LockTransaction (dbInfrastructure, connectionId, new List<string> () { "l1", "" })
			);

			ExceptionAssert.Throw<System.ArgumentException>
			(
				() => new LockTransaction (dbInfrastructure, connectionId, new List<string> () { "l1", null })
			);

			ExceptionAssert.Throw<System.ArgumentException>
			(
				() => new LockTransaction (dbInfrastructure, connectionId, new List<string> () { "l1", "l2", "l1" })
			);
		}


		//[TestMethod]
		//public void SimpleCase()
		//{
		//    DbInfrastructure dbInfrastructure = DatabaseHelper.DbInfrastructure;
			
		//    string userName = "myUser";


		//    //using (LockTransaction l1 = LockTransaction.TryCreateLockTransaction())
		//    //{
				
		//    //}

		//    using (var foo = this.Foo())
		//    {
		//        LockTransaction bar;

		//        LockTransaction.TryCreateLockTransaction (dbInfrastructure, new List<string> { "myLock" }, userName, out bar);

		//        foo = bar;
		//    }
		//}

		//public LockTransaction Foo()
		//{
		//    return null;
		//}


	}


}
