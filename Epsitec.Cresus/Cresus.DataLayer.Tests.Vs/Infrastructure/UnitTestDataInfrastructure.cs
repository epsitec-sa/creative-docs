using Epsitec.Common.UnitTesting;

using Epsitec.Cresus.Database;

using Epsitec.Cresus.DataLayer.Infrastructure;
using Epsitec.Cresus.DataLayer.Tests.Vs.Helpers;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.Collections.Generic;

using System.Linq;


namespace Epsitec.Cresus.DataLayer.Tests.Vs.Infrastructure
{


	[TestClass]
	public sealed class UnitTestDataInfrastructure
	{


		// TODO This class does not contain all the test to check the behavior of DataInfrastructure
		// Some more test should probably be added.
		// Marc


		[ClassInitialize]
		public static void ClassInitialize(TestContext testContext)
		{
			TestHelper.Initialize ();
		}


		[TestInitialize]
		public void TestInitialize()
		{
			DbInfrastructureHelper.ResetTestDatabase ();
		}


		[TestMethod]
		[Ignore]
		public void CleanInactiveLocks()
		{
			// This test is ignored because it throws an exception at its end, because the
			// LockTransaction objects are not disposed.

			using (DbInfrastructure dbInfrastructure1 = DbInfrastructureHelper.ConnectToTestDatabase ())
			using (DbInfrastructure dbInfrastructure2 = DbInfrastructureHelper.ConnectToTestDatabase ())
			using (DbInfrastructure dbInfrastructure3 = DbInfrastructureHelper.ConnectToTestDatabase ())
			using (DbInfrastructure dbInfrastructure4 = DbInfrastructureHelper.ConnectToTestDatabase ())
			{
				using (DataInfrastructure dataInfrastructure1 = DataInfrastructureHelper.ConnectToTestDatabase (dbInfrastructure1))
				using (DataInfrastructure dataInfrastructure2 = DataInfrastructureHelper.ConnectToTestDatabase (dbInfrastructure2))
				using (DataInfrastructure dataInfrastructure3 = DataInfrastructureHelper.ConnectToTestDatabase (dbInfrastructure3))
				using (DataInfrastructure dataInfrastructure4 = DataInfrastructureHelper.ConnectToTestDatabase (dbInfrastructure4))
				{
					dataInfrastructure1.OpenConnection ("1");
					dataInfrastructure2.OpenConnection ("2");
					dataInfrastructure3.OpenConnection ("3");
					dataInfrastructure4.OpenConnection ("4");

					Assert.IsTrue (dataInfrastructure4.AreAllLocksAvailable (new List<string> { "1" }));
					Assert.IsTrue (dataInfrastructure4.AreAllLocksAvailable (new List<string> { "2" }));
					Assert.IsTrue (dataInfrastructure4.AreAllLocksAvailable (new List<string> { "3" }));

					LockTransaction t1 = dataInfrastructure1.CreateLockTransaction (new List<string> { "1" });
					LockTransaction t2 = dataInfrastructure2.CreateLockTransaction (new List<string> { "2" });
					LockTransaction t3 = dataInfrastructure3.CreateLockTransaction (new List<string> { "3" });

					t1.Lock ();
					t2.Lock ();
					t3.Lock ();

					Assert.IsFalse (dataInfrastructure4.AreAllLocksAvailable (new List<string> { "1" }));
					Assert.IsFalse (dataInfrastructure4.AreAllLocksAvailable (new List<string> { "2" }));
					Assert.IsFalse (dataInfrastructure4.AreAllLocksAvailable (new List<string> { "3" }));

					dataInfrastructure2.CloseConnection ();

					Assert.IsFalse (dataInfrastructure4.AreAllLocksAvailable (new List<string> { "1" }));
					Assert.IsFalse (dataInfrastructure4.AreAllLocksAvailable (new List<string> { "2" }));
					Assert.IsFalse (dataInfrastructure4.AreAllLocksAvailable (new List<string> { "3" }));

					for (int i = 0; i < 30; i++)
					{
						dataInfrastructure1.KeepConnectionAlive ();
						dataInfrastructure4.KeepConnectionAlive ();

						System.Threading.Thread.Sleep (1000);
					}

					System.Threading.Thread.Sleep (1000);

					dataInfrastructure4.KeepConnectionAlive ();

					Assert.IsFalse (dataInfrastructure4.AreAllLocksAvailable (new List<string> { "1" }));
					Assert.IsTrue (dataInfrastructure4.AreAllLocksAvailable (new List<string> { "2" }));
					Assert.IsTrue (dataInfrastructure4.AreAllLocksAvailable (new List<string> { "3" }));

					dataInfrastructure1.CloseConnection ();

					dataInfrastructure4.KeepConnectionAlive ();

					Assert.IsTrue (dataInfrastructure4.AreAllLocksAvailable (new List<string> { "1" }));
					Assert.IsTrue (dataInfrastructure4.AreAllLocksAvailable (new List<string> { "2" }));
					Assert.IsTrue (dataInfrastructure4.AreAllLocksAvailable (new List<string> { "3" }));

					dataInfrastructure4.CloseConnection ();

					// TODO Kind of dispose the lock transaction, otherwise an exception will be thrown.
				}
			}
		}


		[TestMethod]
		public void GetDatabaseInfoArgumentCheck()
		{
			using (DbInfrastructure dbInfrastructure = DbInfrastructureHelper.ConnectToTestDatabase ())
			using (DataInfrastructure dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase (dbInfrastructure))
			{
				ExceptionAssert.Throw<System.ArgumentException>
				(
					() => dataInfrastructure.GetDatabaseInfo (null)
				);

				ExceptionAssert.Throw<System.ArgumentException>
				(
					() => dataInfrastructure.GetDatabaseInfo ("")
				);
			}
		}


		[TestMethod]
		public void SetDatabaseInfoArgumentCheck()
		{
			using (DbInfrastructure dbInfrastructure = DbInfrastructureHelper.ConnectToTestDatabase ())
			using (DataInfrastructure dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase (dbInfrastructure))
			{
				ExceptionAssert.Throw<System.ArgumentException>
				(
					() => dataInfrastructure.SetDatabaseInfo (null, "test")
				);

				ExceptionAssert.Throw<System.ArgumentException>
				(
					() => dataInfrastructure.SetDatabaseInfo ("", "test")
				);
			}
		}


		[TestMethod]
		public void GetSetAndExistsInfo()
		{
			using (DbInfrastructure dbInfrastructure = DbInfrastructureHelper.ConnectToTestDatabase ())
			using (DataInfrastructure dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase (dbInfrastructure))
			{
				Dictionary<string, string> info = new Dictionary<string, string> ();

				for (int i = 0; i < 10; i++)
				{
					info[this.GetRandomString ()] = this.GetRandomString ();
				}

				foreach (string key in info.Keys)
				{
					Assert.IsNull (dataInfrastructure.GetDatabaseInfo (key));
				}

				for (int i = 0; i < 10; i++)
				{
					foreach (string key in info.Keys.ToList ())
					{
						dataInfrastructure.SetDatabaseInfo (key, info[key]);

						Assert.AreEqual (info[key], dataInfrastructure.GetDatabaseInfo (key));

						info[key] = this.GetRandomString ();
					}
				}

				foreach (string key in info.Keys)
				{
					dataInfrastructure.SetDatabaseInfo (key, null);
				}

				foreach (string key in info.Keys)
				{
					Assert.IsNull (dataInfrastructure.GetDatabaseInfo (key));
				}
			}
		}


		private string GetRandomString()
		{
			return this.dice.Next ().ToString ();
		}


		private System.Random dice = new System.Random ();


	}


}
