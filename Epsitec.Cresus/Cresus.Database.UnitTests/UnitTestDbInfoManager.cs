using Epsitec.Common.UnitTesting;

using Epsitec.Cresus.Database;
using Epsitec.Cresus.Database.UnitTests.Helpers;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.Collections.Generic;

using System.Linq;


namespace Cresus.Database.UnitTests
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
			TestHelper.DeleteDatabase ();
			TestHelper.CreateDatabase ();
		}


		[TestMethod]
		public void AttachArgumentCheck()
		{
			using (DbInfrastructure dbInfrastructure = TestHelper.ConnectToDatabase ())
			{
				ExceptionAssert.Throw<System.ArgumentNullException>
				(
					() => new DbInfoManager ().Attach (null, new DbTable ())
				);

				ExceptionAssert.Throw<System.ArgumentNullException>
				(
					() => new DbInfoManager ().Attach (dbInfrastructure, null)
				);
			}
		}


		[TestMethod]
		public void AttachAndDetach()
		{
			using (DbInfrastructure dbInfrastructure = TestHelper.ConnectToDatabase ())
			{
				Assert.IsNotNull (dbInfrastructure.InfoManager);
			}
		}


		[TestMethod]
		public void ExistsInfoArgumentCheck()
		{
			using (DbInfrastructure dbInfrastructure = TestHelper.ConnectToDatabase ())
			{
				ExceptionAssert.Throw<System.ArgumentException>
				(
					() => dbInfrastructure.InfoManager.ExistsInfo (null)
				);

				ExceptionAssert.Throw<System.ArgumentException>
				(
					() => dbInfrastructure.InfoManager.ExistsInfo ("")
				);
			}
		}


		[TestMethod]
		public void SetInfoArgumentCheck()
		{
			using (DbInfrastructure dbInfrastructure = TestHelper.ConnectToDatabase ())
			{
				ExceptionAssert.Throw<System.ArgumentException>
				(
					() => dbInfrastructure.InfoManager.SetInfo (null, "test")
				);

				ExceptionAssert.Throw<System.ArgumentException>
				(
					() => dbInfrastructure.InfoManager.SetInfo ("", "test")
				);
			}
		}


		[TestMethod]
		public void GetInfoArgumentCheck()
		{
			using (DbInfrastructure dbInfrastructure = TestHelper.ConnectToDatabase ())
			{
				ExceptionAssert.Throw<System.ArgumentException>
				(
					() => dbInfrastructure.InfoManager.GetInfo (null)
				);

				ExceptionAssert.Throw<System.ArgumentException>
				(
					() => dbInfrastructure.InfoManager.GetInfo ("")
				);
			}
		}


		[TestMethod]
		public void GetSetAndExistsInfo()
		{
			using (DbInfrastructure dbInfrastructure = TestHelper.ConnectToDatabase ())
			{
				DbInfoManager manager = dbInfrastructure.InfoManager;

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
