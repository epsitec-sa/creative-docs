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
			using (DbInfrastructure dbinfrastructure = DbInfrastructureHelper.ConnectToTestDatabase ())
			{
				EntityEngine entityEngine = EntityEngineHelper.ConnectToTestDatabase ();

				ExceptionAssert.Throw<System.ArgumentNullException>
				(
					() => new InfoManager (null, entityEngine.ServiceSchemaEngine)
				);

				ExceptionAssert.Throw<System.ArgumentNullException>
				(
					() => new InfoManager (dbinfrastructure, null)
				);
			}
		}


		[TestMethod]
		public void DoesInfoExistsArgumentCheck()
		{
			using (DbInfrastructure dbInfrastructure = DbInfrastructureHelper.ConnectToTestDatabase ())
			{
				EntityEngine entityEngine = EntityEngineHelper.ConnectToTestDatabase ();

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
				EntityEngine entityEngine = EntityEngineHelper.ConnectToTestDatabase ();

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
				EntityEngine entityEngine = EntityEngineHelper.ConnectToTestDatabase ();

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
				EntityEngine entityEngine = EntityEngineHelper.ConnectToTestDatabase ();

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


		private string GetRandomString()
		{
			return this.dice.Next ().ToString ();
		}


		private System.Random dice = new System.Random ();

	}


}
