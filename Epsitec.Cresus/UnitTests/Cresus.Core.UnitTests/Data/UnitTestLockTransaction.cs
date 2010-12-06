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
		public void InvalidBehaviorTest()
		{
			using (DbInfrastructure dbInfrastructure = TestHelper.ConnectToDatabase ())
			{
				using (DataInfrastructure dataInfrastructure = new DataInfrastructure (dbInfrastructure))
				{
					dataInfrastructure.OpenConnection ("id");

					LockTransaction lt = new LockTransaction (dataInfrastructure, new List<string> () { "lock1", "lock2" });

					ExceptionAssert.Throw<System.InvalidOperationException>
					(
						() => { var x = lt.ForeignLockOwners; }
					);

					ExceptionAssert.Throw<System.InvalidOperationException>
					(
						() => { var x = lt.LockCreationTimes; }
					);
					
					Assert.IsTrue (lt.Acquire ());

					ExceptionAssert.Throw<System.InvalidOperationException>
					(
						() => { var x = lt.ForeignLockOwners; }
					);

					ExceptionAssert.Throw<System.InvalidOperationException>
					(
						() => { var x = lt.LockCreationTimes; }
					);

					lt.Dispose ();

					ExceptionAssert.Throw<System.InvalidOperationException>
					(
						() => lt.Acquire ()
					);

					ExceptionAssert.Throw<System.InvalidOperationException>
					(
						() => lt.Poll ()
					);

					ExceptionAssert.Throw<System.InvalidOperationException>
					(
						() => { var x = lt.ForeignLockOwners; }
					);

					ExceptionAssert.Throw<System.InvalidOperationException>
					(
						() => { var x = lt.LockCreationTimes; }
					);
				}
			}
		}


		[TestMethod]
		public void SimpleTest1()
		{
			using (DbInfrastructure dbInfrastructure = new DbInfrastructure ())
			{
				dbInfrastructure.AttachToDatabase (TestHelper.CreateDbAccess ());
				
				using (DataInfrastructure dataInfrastructure = new DataInfrastructure (dbInfrastructure))
				{
					dataInfrastructure.OpenConnection ("id");

					using (LockTransaction lt = new LockTransaction (dataInfrastructure, new List<string> { "lock", }))
					{
						Assert.IsTrue (lt.Poll ());
						Assert.IsTrue (lt.Acquire ());
					}
				}
			}
		}


		[TestMethod]
		public void SimpleTest2()
		{
			using (DbInfrastructure dbInfrastructure1 = new DbInfrastructure ())
			using (DbInfrastructure dbInfrastructure2 = new DbInfrastructure ())
			{
				dbInfrastructure1.AttachToDatabase (TestHelper.CreateDbAccess ());
				dbInfrastructure2.AttachToDatabase (TestHelper.CreateDbAccess ());

				using (DataInfrastructure dataInfrastructure1 = new DataInfrastructure (dbInfrastructure1))
				using (DataInfrastructure dataInfrastructure2 = new DataInfrastructure (dbInfrastructure2))
				{
					dataInfrastructure1.OpenConnection ("id1");
					dataInfrastructure2.OpenConnection ("id2");

					using (LockTransaction lt1 = new LockTransaction (dataInfrastructure1, new List<string> { "lock1", "lock2" }))
					using (LockTransaction lt2 = new LockTransaction (dataInfrastructure1, new List<string> { "lock2" }))
					using (LockTransaction lt3 = new LockTransaction (dataInfrastructure2, new List<string> { "lock2", "lock3" }))
					{
						Assert.IsTrue (lt1.Poll ());
						Assert.IsTrue (lt2.Poll ());
						Assert.IsTrue (lt3.Poll ());

						Assert.IsTrue (lt1.Acquire ());

						Assert.IsTrue (lt1.Poll ());
						Assert.IsTrue (lt2.Poll ());
						Assert.IsFalse (lt3.Poll ());

						Assert.IsTrue (lt2.Acquire ());
						Assert.IsFalse (lt3.Acquire ());

						Assert.IsTrue (lt1.Poll ());
						Assert.IsTrue (lt2.Poll ());
						Assert.IsFalse (lt3.Poll ());

						lt1.Dispose ();

						Assert.IsFalse (lt3.Acquire ());

						Assert.IsTrue (lt2.Poll ());
						Assert.IsFalse (lt3.Poll ());

						lt2.Dispose ();

						Assert.IsTrue (lt3.Poll ());
						Assert.IsTrue (lt3.Acquire ());

						Assert.IsTrue (lt3.Poll ());

						lt3.Dispose ();
					}
				}
			}
		}


		[TestMethod]
		public void GetLockOwnersAndCreationTimeTest()
		{
			using (DbInfrastructure dbInfrastructure1 = new DbInfrastructure ())
			using (DbInfrastructure dbInfrastructure2 = new DbInfrastructure ())
			using (DbInfrastructure dbInfrastructure3 = new DbInfrastructure ())
			using (DbInfrastructure dbInfrastructure4 = new DbInfrastructure ())
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

					using (LockTransaction lt1 = new LockTransaction (dataInfrastructure1, new List<string> { "lock1", "lock2" }))
					using (LockTransaction lt2 = new LockTransaction (dataInfrastructure2, new List<string> { "lock3" }))
					using (LockTransaction lt3 = new LockTransaction (dataInfrastructure3, new List<string> { "lock4" }))
					using (LockTransaction lt4 = new LockTransaction (dataInfrastructure4, new List<string> { "lock1", "lock2", "lock3" }))
					{
						Assert.IsTrue (lt1.Acquire ());
						Assert.IsTrue (lt2.Acquire ());
						Assert.IsTrue (lt3.Acquire ());
						Assert.IsFalse (lt4.Acquire ());

						lt1.Poll ();
						lt2.Poll ();
						lt3.Poll ();
						lt4.Poll ();

						System.DateTime t2 = dbInfrastructure1.GetDatabaseTime ();

						var expectedLockOwners1 = new Dictionary<string, string>
		                {
		                    { "lock1", "id1" },
		                    { "lock2", "id1" },
		                };

						var expectedLockOwners2 = new Dictionary<string, string>
		                {
		                    { "lock3", "id2" },
		                };

						var expectedLockOwners3 = new Dictionary<string, string>
		                {
		                    { "lock4", "id3" },
		                };

						var expectedLockOwners4 = new Dictionary<string, string>
		                {
		                    { "lock1", "id1" },
		                    { "lock2", "id1" },
		                    { "lock3", "id2" },
		                };

						Assert.IsTrue (expectedLockOwners1.SetEquals (lt1.ForeignLockOwners));
						Assert.IsTrue (expectedLockOwners2.SetEquals (lt2.ForeignLockOwners));
						Assert.IsTrue (expectedLockOwners3.SetEquals (lt3.ForeignLockOwners));
						Assert.IsTrue (expectedLockOwners4.SetEquals (lt4.ForeignLockOwners));

						Assert.AreEqual (2, lt1.LockCreationTimes.Count);
						Assert.IsTrue (lt1.LockCreationTimes.ContainsKey ("lock1"));
						Assert.IsTrue (lt1.LockCreationTimes.ContainsKey ("lock2"));

						Assert.AreEqual (1, lt2.LockCreationTimes.Count);
						Assert.IsTrue (lt2.LockCreationTimes.ContainsKey ("lock3"));

						Assert.AreEqual (1, lt3.LockCreationTimes.Count);
						Assert.IsTrue (lt3.LockCreationTimes.ContainsKey ("lock4"));

						Assert.AreEqual (3, lt4.LockCreationTimes.Count);
						Assert.IsTrue (lt4.LockCreationTimes.ContainsKey ("lock1"));
						Assert.IsTrue (lt4.LockCreationTimes.ContainsKey ("lock2"));
						Assert.IsTrue (lt4.LockCreationTimes.ContainsKey ("lock3"));

						Assert.AreEqual (lt1.LockCreationTimes["lock1"], lt4.LockCreationTimes["lock1"]);
						Assert.AreEqual (lt1.LockCreationTimes["lock2"], lt4.LockCreationTimes["lock2"]);

						Assert.AreEqual (lt2.LockCreationTimes["lock3"], lt4.LockCreationTimes["lock3"]);

						Assert.IsTrue (t1 <= lt4.LockCreationTimes["lock1"]);
						Assert.IsTrue (t1 <= lt4.LockCreationTimes["lock2"]);
						Assert.IsTrue (lt4.LockCreationTimes["lock1"] <= lt4.LockCreationTimes["lock3"]);
						Assert.IsTrue (lt4.LockCreationTimes["lock2"] <= lt4.LockCreationTimes["lock3"]);
						Assert.IsTrue (lt4.LockCreationTimes["lock3"] <= lt3.LockCreationTimes["lock4"]);
						Assert.IsTrue (lt3.LockCreationTimes["lock4"] <= t2);
					}
				}
			}
		}


	}


}
