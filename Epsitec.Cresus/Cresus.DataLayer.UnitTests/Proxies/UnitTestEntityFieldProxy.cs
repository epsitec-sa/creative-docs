using Epsitec.Common.Support;
using Epsitec.Common.Types;

using Epsitec.Cresus.Database;

using Epsitec.Cresus.DataLayer.Context;
using Epsitec.Cresus.DataLayer.Proxies;
using Epsitec.Cresus.DataLayer.UnitTests.Entities;

using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace Epsitec.Cresus.DataLayer.UnitTests
{


	[TestClass]
	public sealed class UnitTestEntityFieldProxy
	{


		[ClassInitialize]
		public static void ClassInitialize(TestContext testContext)
		{
			TestHelper.Initialize ();

			DatabaseHelper.CreateAndConnectToDatabase ();

			using (DataContext dataContext = new DataContext (DatabaseHelper.DbInfrastructure))
			{
				DatabaseCreator2.PupulateDatabase (dataContext);
			}
		}


		[ClassCleanup]
		public static void ClassCleanup()
		{
			DatabaseHelper.DisconnectFromDatabase ();
		}


		[TestMethod]
		[DeploymentItem ("Cresus.DataLayer.dll")]
		public void EntityFieldProxyConstructorTest()
		{
			using (DataContext dataContext = new DataContext (DatabaseHelper.DbInfrastructure))
			{
				NaturalPersonEntity person = dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1)));
				Druid fieldId = Druid.Parse ("[L0A11]");

				var proxy = new EntityFieldProxy_Accessor (dataContext, person, fieldId);

				Assert.AreSame (dataContext, proxy.DataContext);
				Assert.AreSame (person, proxy.Entity);
				Assert.AreEqual (fieldId, proxy.FieldId);
			}
		}


		[TestMethod]
		[DeploymentItem ("Cresus.DataLayer.dll")]
		[ExpectedException (typeof (System.ArgumentNullException))]
		public void EntityFieldProxyConstructorTest2()
		{
			using (DataContext dataContext = new DataContext (DatabaseHelper.DbInfrastructure))
			{
				NaturalPersonEntity person = dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1)));
				Druid fieldId = Druid.Parse ("[L0A11]");

				new EntityFieldProxy_Accessor (null, person, fieldId);
			}
		}


		[TestMethod]
		[DeploymentItem ("Cresus.DataLayer.dll")]
		[ExpectedException (typeof (System.ArgumentNullException))]
		public void EntityFieldProxyConstructorTest3()
		{
			using (DataContext dataContext = new DataContext (DatabaseHelper.DbInfrastructure))
			{
				Druid fieldId = Druid.Parse ("[L0A11]");

				new EntityFieldProxy_Accessor (dataContext, null, fieldId);
			}
		}


		[TestMethod]
		[DeploymentItem ("Cresus.DataLayer.dll")]
		[ExpectedException (typeof (System.ArgumentException))]
		public void EntityFieldProxyConstructorTest4()
		{
			using (DataContext dataContext = new DataContext (DatabaseHelper.DbInfrastructure))
			{
				NaturalPersonEntity person = dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1)));

				new EntityFieldProxy_Accessor (dataContext, person, Druid.Empty);
			}
		}


		[TestMethod]
		[DeploymentItem ("Cresus.DataLayer.dll")]
		[ExpectedException (typeof (System.ArgumentException))]
		public void EntityFieldProxyonstructorTest5()
		{
			using (DataContext dataContext = new DataContext (DatabaseHelper.DbInfrastructure))
			{
				NaturalPersonEntity person = dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1)));

				new EntityFieldProxy_Accessor (dataContext, person, Druid.Parse ("[L0AN]"));
			}
		}


		[TestMethod]
		[DeploymentItem ("Cresus.DataLayer.dll")]
		[ExpectedException (typeof (System.ArgumentException))]
		public void EntityFieldProxyConstructorTest6()
		{
			using (DataContext dataContext = new DataContext (DatabaseHelper.DbInfrastructure))
			{
				NaturalPersonEntity person = dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1)));

				new EntityFieldProxy_Accessor (dataContext, person, Druid.Parse ("[L0AS]"));
			}
		}


		[TestMethod]
		[DeploymentItem ("Cresus.DataLayer.dll")]
		[ExpectedException (typeof (System.ArgumentException))]
		public void EntityFieldProxyConstructorTest7()
		{
			using (DataContext dataContext1 = new DataContext (DatabaseHelper.DbInfrastructure))
			{
				using (DataContext dataContext2 = new DataContext (DatabaseHelper.DbInfrastructure))
				{
					NaturalPersonEntity person = dataContext1.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1)));
					Druid fieldId = Druid.Parse ("[L0A11]");

					new EntityFieldProxy_Accessor (dataContext2, person, fieldId);
				}
			}
		}


		[TestMethod]
		[DeploymentItem ("Cresus.DataLayer.dll")]
		public void DiscardWriteEntityValueTest()
		{
			using (DataContext dataContext = new DataContext (DatabaseHelper.DbInfrastructure))
			{
				NaturalPersonEntity person = dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1)));
				Druid fieldId = Druid.Parse ("[L0A11]");

				var proxy = new EntityFieldProxy_Accessor (dataContext, person, fieldId);

				object obj = new object ();

				Assert.IsFalse (proxy.DiscardWriteEntityValue (new TestStore (), "L0A11", ref obj));
			}
		}

		[TestMethod ]
		[DeploymentItem ("Cresus.DataLayer.dll")]
		public void GetReadEntityValueTest()
		{
			using (DataContext dataContext = new DataContext (DatabaseHelper.DbInfrastructure))
			{
				NaturalPersonEntity person = dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1)));
				PersonGenderEntity gender1 = dataContext.ResolveEntity<PersonGenderEntity> (new DbKey (new DbId (1)));

				Druid fieldId = Druid.Parse ("[L0A11]");

				var proxy = new EntityFieldProxy_Accessor (dataContext, person, fieldId);

				TestStore testStore = new TestStore ();

				object gender2 = proxy.GetReadEntityValue (testStore, "L0A11");
				object gender3 = testStore.GetValue ("L0A11");

				Assert.AreSame (gender1, gender2);
				Assert.AreSame (gender1, gender3);
			}
		}


		[TestMethod ]
		[DeploymentItem ("Cresus.DataLayer.dll")]
		public void GetWriteEntityValueTest()
		{
			using (DataContext dataContext = new DataContext (DatabaseHelper.DbInfrastructure))
			{
				NaturalPersonEntity person = dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1)));
				Druid fieldId = Druid.Parse ("[L0A11]");

				var proxy = new EntityFieldProxy_Accessor (dataContext, person, fieldId);
				object gender = proxy.GetWriteEntityValue (new TestStore (), "L0A11");


				Assert.AreSame (proxy.Target, gender);
			}
		}


		[TestMethod]
		[DeploymentItem ("Cresus.DataLayer.dll")]
		public void PromoteToRealInstanceTest1()
		{
			using (DataContext dataContext = new DataContext (DatabaseHelper.DbInfrastructure))
			{
				NaturalPersonEntity person = dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1)));
				PersonGenderEntity gender1 = dataContext.ResolveEntity<PersonGenderEntity> (new DbKey (new DbId (1)));

				Druid fieldId = Druid.Parse ("[L0A11]");

				var proxy = new EntityFieldProxy_Accessor (dataContext, person, fieldId);

				object gender2 = proxy.PromoteToRealInstance ();

				Assert.AreSame (gender1, gender2);
			}
		}


		[TestMethod]
		[DeploymentItem ("Cresus.DataLayer.dll")]
		public void PromoteToRealInstanceTest2()
		{
			using (DataContext dataContext = new DataContext (DatabaseHelper.DbInfrastructure))
			{
				NaturalPersonEntity person = dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (3)));

				Druid fieldId = Druid.Parse ("[L0A11]");

				var proxy = new EntityFieldProxy_Accessor (dataContext, person, fieldId);

				object gender = proxy.PromoteToRealInstance ();

				Assert.AreSame (UndefinedValue.Value, gender);
			}
		}


	}


}
