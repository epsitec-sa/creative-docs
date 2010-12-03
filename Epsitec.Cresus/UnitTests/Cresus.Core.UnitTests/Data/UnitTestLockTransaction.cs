using Epsitec.Common.Support.Extensions;

using Epsitec.Common.UnitTesting;

using Epsitec.Cresus.Database;

using Epsitec.Cresus.DataLayer.Infrastructure;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.Collections.Generic;


namespace Epsitec.Cresus.Core.Data
{


	[TestClass]
	public class UnitTestLockTransaction
	{


		[ClassInitialize]
		public static void Initialize(TestContext testContext)
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
		public void GetLockOwnersAndCreationTimeInvalidOperation()
		{
			using (DbInfrastructure dbInfrastructure = TestHelper.ConnectToDatabase ())
			{
				using (DataInfrastructure dataInfrastructure = new DataInfrastructure (dbInfrastructure))
				{
					dataInfrastructure.OpenConnection ("id");

					LockTransaction lt = new LockTransaction (new List<string> () { "lock1", "lock2" });

					ExceptionAssert.Throw<System.InvalidOperationException>
					(
						() => { var x = lt.LockOwners; }
					);

					ExceptionAssert.Throw<System.InvalidOperationException>
					(
						() => { var x = lt.LockCreationTimes; }
					);
					
					Assert.IsTrue (lt.Acquire (dataInfrastructure));

					ExceptionAssert.Throw<System.InvalidOperationException>
					(
						() => { var x = lt.LockOwners; }
					);

					ExceptionAssert.Throw<System.InvalidOperationException>
					(
						() => { var x = lt.LockCreationTimes; }
					);

					lt.Dispose ();

					ExceptionAssert.Throw<System.InvalidOperationException>
					(
						() =>
						{
							var x = lt.LockOwners;
						}
					);

					ExceptionAssert.Throw<System.InvalidOperationException>
					(
						() =>
						{
							var x = lt.LockCreationTimes;
						}
					);
				}
			}
		}


		[TestMethod]
		public void GetLockOwnersAndCreationTimeTest()
		{
			using (DbInfrastructure dbInfrastructure1 = new DbInfrastructure())
			using (DbInfrastructure dbInfrastructure2 = new DbInfrastructure())
			using (DbInfrastructure dbInfrastructure3 = new DbInfrastructure())
			using (DbInfrastructure dbInfrastructure4 = new DbInfrastructure())
			{
				dbInfrastructure1.AttachToDatabase (TestHelper.CreateDbAccess ());
				dbInfrastructure2.AttachToDatabase (TestHelper.CreateDbAccess ());
				dbInfrastructure3.AttachToDatabase (TestHelper.CreateDbAccess ());
				dbInfrastructure4.AttachToDatabase (TestHelper.CreateDbAccess ());

				using (DataInfrastructure dataInfrastructure1 = new DataInfrastructure (dbInfrastructure1))
				using (DataInfrastructure dataInfrastructure2 = new DataInfrastructure (dbInfrastructure2))
				using (DataInfrastructure dataInfrastructure3 = new DataInfrastructure (dbInfrastructure3))
				using (DataInfrastructure dataInfrastructure4 = new DataInfrastructure (dbInfrastructure4))
				{
					dataInfrastructure1.OpenConnection ("id1");
					dataInfrastructure2.OpenConnection ("id2");
					dataInfrastructure3.OpenConnection ("id3");
					dataInfrastructure4.OpenConnection ("id4");

					System.DateTime t1 = dbInfrastructure1.GetDatabaseTime ();

					using (LockTransaction lt1 = new LockTransaction (new List<string> { "lock1", "lock2" }))
					using (LockTransaction lt2 = new LockTransaction (new List<string> { "lock3" }))
					using (LockTransaction lt3 = new LockTransaction (new List<string> { "lock4" }))
					using (LockTransaction lt4 = new LockTransaction (new List<string> { "lock1", "lock2", "lock3" }))
					{
						Assert.IsTrue (lt1.Acquire (dataInfrastructure1));
						Assert.IsTrue (lt2.Acquire (dataInfrastructure2));
						Assert.IsTrue (lt3.Acquire (dataInfrastructure3));
						Assert.IsFalse (lt4.Acquire (dataInfrastructure4));

						System.DateTime t2 = dbInfrastructure1.GetDatabaseTime ();

						var expectedLockOwners = new Dictionary<string, string>
						{
							{ "lock1", "id1" },
							{ "lock2", "id1" },
							{ "lock3", "id2" },
						};

						var actualLockOwners = lt4.LockOwners;
						var actualLockCreationTimes = lt4.LockCreationTimes;
						
						Assert.IsTrue (expectedLockOwners.SetEquals (actualLockOwners));

						Assert.AreEqual (3, actualLockCreationTimes.Count);
						Assert.IsTrue (actualLockCreationTimes.ContainsKey ("lock1"));
						Assert.IsTrue (actualLockCreationTimes.ContainsKey ("lock2"));
						Assert.IsTrue (actualLockCreationTimes.ContainsKey ("lock3"));

						Assert.IsTrue (t1 <= actualLockCreationTimes["lock1"]);
						Assert.IsTrue (t1 <= actualLockCreationTimes["lock2"]);
						Assert.IsTrue (actualLockCreationTimes["lock1"] <= actualLockCreationTimes["lock3"]);
						Assert.IsTrue (actualLockCreationTimes["lock2"] <= actualLockCreationTimes["lock3"]);
						Assert.IsTrue (actualLockCreationTimes["lock3"] <= t2);
					}
				}
			}
		}


	}


}
