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
	public sealed class UnitTestInfoManager
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
			using (DbInfrastructure dbInfrastructure = DbInfrastructureHelper.ConnectToTestDatabase ())
			{
				EntityEngine entityEngine = EntityEngineHelper.ConnectToTestDatabase (dbInfrastructure);

				ExceptionAssert.Throw<System.ArgumentNullException>
				(
					() => new InfoManager (null, entityEngine.ServiceSchemaEngine)
				);

				ExceptionAssert.Throw<System.ArgumentNullException>
				(
					() => new InfoManager (dbInfrastructure, null)
				);
			}
		}


		[TestMethod]
		public void DoesInfoExistsArgumentCheck()
		{
			using (DbInfrastructure dbInfrastructure = DbInfrastructureHelper.ConnectToTestDatabase ())
			{
				EntityEngine entityEngine = EntityEngineHelper.ConnectToTestDatabase (dbInfrastructure);

				InfoManager manager = new InfoManager (dbInfrastructure, entityEngine.ServiceSchemaEngine);

				ExceptionAssert.Throw<System.ArgumentException>
				(
					() => manager.DoesInfoExists (null)
				);

				ExceptionAssert.Throw<System.ArgumentException>
				(
					() => manager.DoesInfoExists ("")
				);
			}
		}


		[TestMethod]
		public void SetInfoArgumentCheck()
		{
			using (DbInfrastructure dbInfrastructure = DbInfrastructureHelper.ConnectToTestDatabase ())
			{
				EntityEngine entityEngine = EntityEngineHelper.ConnectToTestDatabase (dbInfrastructure);

				InfoManager manager = new InfoManager (dbInfrastructure, entityEngine.ServiceSchemaEngine);

				ExceptionAssert.Throw<System.ArgumentException>
				(
					() => manager.SetInfo (null, "test")
				);

				ExceptionAssert.Throw<System.ArgumentException>
				(
					() => manager.SetInfo ("", "test")
				);
			}
		}


		[TestMethod]
		public void GetInfoArgumentCheck()
		{
			using (DbInfrastructure dbInfrastructure = DbInfrastructureHelper.ConnectToTestDatabase ())
			{
				EntityEngine entityEngine = EntityEngineHelper.ConnectToTestDatabase (dbInfrastructure);

				InfoManager manager = new InfoManager (dbInfrastructure, entityEngine.ServiceSchemaEngine);

				ExceptionAssert.Throw<System.ArgumentException>
				(
					() => manager.GetInfo (null)
				);

				ExceptionAssert.Throw<System.ArgumentException>
				(
					() => manager.GetInfo ("")
				);
			}
		}


		[TestMethod]
		public void GetSetAndExistsInfo()
		{
			using (DbInfrastructure dbInfrastructure = DbInfrastructureHelper.ConnectToTestDatabase ())
			{
				EntityEngine entityEngine = EntityEngineHelper.ConnectToTestDatabase (dbInfrastructure);

				InfoManager manager = new InfoManager (dbInfrastructure, entityEngine.ServiceSchemaEngine);

				Dictionary<string, string> info = new Dictionary<string, string> ();

				for (int i = 0; i < 10; i++)
				{
					info[this.GetRandomString ()] = this.GetRandomString ();
				}

				foreach (string key in info.Keys)
				{
					Assert.IsFalse (manager.DoesInfoExists (key));
				}

				for (int i = 0; i < 10; i++)
				{
					foreach (string key in info.Keys.ToList ())
					{
						manager.SetInfo (key, info[key]);

						Assert.IsTrue (manager.DoesInfoExists (key));

						Assert.AreEqual (info[key], manager.GetInfo (key));

						info[key] = this.GetRandomString ();
					}
				}

				foreach (string key in info.Keys)
				{
					manager.SetInfo (key, null);
				}

				foreach (string key in info.Keys)
				{
					Assert.IsFalse (manager.DoesInfoExists (key));
				}
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

			var infoManagers = dbInfrastructures
				.Select (i => new InfoManager (i, entityEngine.ServiceSchemaEngine))
				.ToList ();

			try
			{
				System.DateTime time = System.DateTime.Now;
				var keys = Enumerable.Range (0, 50).Select (i => i.ToString ()).ToList ();

				var threads = infoManagers.Select (i => new System.Threading.Thread (() =>
				{
					var dice = new System.Random (System.Threading.Thread.CurrentThread.ManagedThreadId);

					var infoManager = i;

					while (System.DateTime.Now - time <= System.TimeSpan.FromSeconds (15))
					{
						var key1 = keys[dice.Next (0, keys.Count)];

						if (dice.NextDouble () > 0.2)
						{
							infoManager.SetInfo (key1, System.Guid.NewGuid ().ToString ());
						}
						else
						{
							infoManager.SetInfo (key1, null);
						}

						var key2 = keys[dice.Next (0, keys.Count)];

						infoManager.GetInfo (key2);

						var key3 = keys[dice.Next (0, keys.Count)];

						infoManager.DoesInfoExists (key3);
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


		private string GetRandomString()
		{
			return this.dice.Next ().ToString ();
		}


		private System.Random dice = new System.Random ();

	}


}
