using Epsitec.Common.UnitTesting;

using Epsitec.Cresus.Database;
using Epsitec.Cresus.Database.Services;
using Epsitec.Cresus.Database.UnitTests.Helpers;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.Collections.Generic;

using System.Linq;


namespace Epsitec.Cresus.Database.UnitTests.Services
{


	[TestClass]
	public sealed class UnitTestDbInfoManager
	{


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
		public void ConstructorArgumentCheck()
		{
			ExceptionAssert.Throw<System.ArgumentNullException>
			(
				() => new DbInfoManager (null)
			);
		}


		[TestMethod]
		public void ExistsInfoArgumentCheck()
		{
			using (DbInfrastructure dbInfrastructure = DbInfrastructureHelper.ConnectToTestDatabase ())
			{
				ExceptionAssert.Throw<System.ArgumentException>
				(
					() => dbInfrastructure.ServiceManager.InfoManager.ExistsInfo (null)
				);

				ExceptionAssert.Throw<System.ArgumentException>
				(
					() => dbInfrastructure.ServiceManager.InfoManager.ExistsInfo ("")
				);
			}
		}


		[TestMethod]
		public void SetInfoArgumentCheck()
		{
			using (DbInfrastructure dbInfrastructure = DbInfrastructureHelper.ConnectToTestDatabase ())
			{
				ExceptionAssert.Throw<System.ArgumentException>
				(
					() => dbInfrastructure.ServiceManager.InfoManager.SetInfo (null, "test")
				);

				ExceptionAssert.Throw<System.ArgumentException>
				(
					() => dbInfrastructure.ServiceManager.InfoManager.SetInfo ("", "test")
				);
			}
		}


		[TestMethod]
		public void GetInfoArgumentCheck()
		{
			using (DbInfrastructure dbInfrastructure = DbInfrastructureHelper.ConnectToTestDatabase ())
			{
				ExceptionAssert.Throw<System.ArgumentException>
				(
					() => dbInfrastructure.ServiceManager.InfoManager.GetInfo (null)
				);

				ExceptionAssert.Throw<System.ArgumentException>
				(
					() => dbInfrastructure.ServiceManager.InfoManager.GetInfo ("")
				);
			}
		}


		[TestMethod]
		public void GetSetAndExistsInfo()
		{
			using (DbInfrastructure dbInfrastructure = DbInfrastructureHelper.ConnectToTestDatabase ())
			{
				DbInfoManager manager = dbInfrastructure.ServiceManager.InfoManager;

				Dictionary<string, string> info = new Dictionary<string, string> ();

				for (int i = 0; i < 10; i++)
				{
					info[this.GetRandomString ()] = this.GetRandomString ();
				}

				foreach (string key in info.Keys)
				{
					Assert.IsFalse (manager.ExistsInfo (key));
				}

				for (int i = 0; i < 10; i++)
				{
					foreach (string key in info.Keys.ToList ())
					{
						manager.SetInfo (key, info[key]);

						Assert.IsTrue (manager.ExistsInfo (key));

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
					Assert.IsFalse (manager.ExistsInfo (key));
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
