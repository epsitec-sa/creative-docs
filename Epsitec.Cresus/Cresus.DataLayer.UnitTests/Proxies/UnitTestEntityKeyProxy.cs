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
	public sealed class UnitTestEntityKeyProxy
	{


		[ClassInitialize]
		public static void Initialize(TestContext testContext)
		{
			TestHelper.Initialize ();

			DatabaseHelper.CreateAndConnectToDatabase ();

			using (DataContext dataContext = new DataContext (DatabaseHelper.DbInfrastructure))
			{
				DatabaseCreator2.PupulateDatabase (dataContext);
			}
		}


		[ClassCleanup]
		public static void Cleanup()
		{
			DatabaseHelper.DisconnectFromDatabase ();
		}


		[TestMethod]
		[DeploymentItem ("Cresus.DataLayer.dll")]
		public void ValueFieldProxyConstructorTest1()
		{
			using (DataContext dataContext = new DataContext (DatabaseHelper.DbInfrastructure))
			{
				NaturalPersonEntity person = dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1)));
				EntityKey entityKey = new EntityKey (person, new DbKey (new DbId (1)));

				var proxy = new EntityKeyProxy_Accessor (dataContext, entityKey);

				Assert.AreSame (dataContext, proxy.dataContext);
				Assert.AreEqual (entityKey, proxy.entityKey);
			}
		}


		[TestMethod]
		[DeploymentItem ("Cresus.DataLayer.dll")]
		[ExpectedException (typeof (System.ArgumentNullException))]
		public void ValueFieldProxyConstructorTest2()
		{
			using (DataContext dataContext = new DataContext (DatabaseHelper.DbInfrastructure))
			{
				NaturalPersonEntity person = dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1)));
				EntityKey entityKey = new EntityKey (person, new DbKey (new DbId (1)));

				new EntityKeyProxy_Accessor (null, entityKey);
			}
		}


		[TestMethod]
		[DeploymentItem ("Cresus.DataLayer.dll")]
		[ExpectedException (typeof (System.ArgumentException))]
		public void ValueFieldProxyConstructorTest3()
		{
			using (DataContext dataContext = new DataContext (DatabaseHelper.DbInfrastructure))
			{
				new EntityKeyProxy_Accessor (dataContext, EntityKey.Empty);
			}
		}


		[TestMethod]
		[DeploymentItem ("Cresus.DataLayer.dll")]
		public void DiscardWriteEntityValueTest()
		{
			using (DataContext dataContext = new DataContext (DatabaseHelper.DbInfrastructure))
			{
				NaturalPersonEntity person = dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1)));
				EntityKey entityKey = new EntityKey (person, new DbKey (new DbId (1)));

				var proxy = new EntityKeyProxy_Accessor (dataContext, entityKey);
				object obj = new object ();

				Assert.IsFalse (proxy.DiscardWriteEntityValue (new TestStore (), "L0A11", ref obj));
			}
		}


		[TestMethod]
		[DeploymentItem ("Cresus.DataLayer.dll")]
		public void GetReadEntityValueTest()
		{
			using (DataContext dataContext = new DataContext (DatabaseHelper.DbInfrastructure))
			{
				NaturalPersonEntity person1 = dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1)));
				EntityKey entityKey = new EntityKey (person1, new DbKey (new DbId (1)));

				var proxy = new EntityKeyProxy_Accessor (dataContext, entityKey);

				TestStore testStore = new TestStore ();

				object person2 = proxy.GetReadEntityValue (testStore, "L0A11");
				object person3 = testStore.GetValue ("L0A11");

				Assert.AreSame (person1, person2);
				Assert.AreSame (person1, person3);
			}
		}


		[TestMethod]
		[DeploymentItem ("Cresus.DataLayer.dll")]
		public void GetWriteEntityValueTest()
		{
			using (DataContext dataContext = new DataContext (DatabaseHelper.DbInfrastructure))
			{
				NaturalPersonEntity person1 = dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1)));
				EntityKey entityKey = new EntityKey (person1, new DbKey (new DbId (1)));

				var proxy = new EntityKeyProxy_Accessor (dataContext, entityKey);
				object obj = new object ();

				object person2 = proxy.GetWriteEntityValue (new TestStore (), "");

				Assert.AreSame (proxy.Target, person2);
			}
		}


		[TestMethod]
		[DeploymentItem ("Cresus.DataLayer.dll")]
		public void PromoteToRealInstanceTest1()
		{
			using (DataContext dataContext = new DataContext (DatabaseHelper.DbInfrastructure))
			{
				NaturalPersonEntity person1 = dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1)));
				EntityKey entityKey = new EntityKey (person1, new DbKey (new DbId (1)));

				var proxy = new EntityKeyProxy_Accessor (dataContext, entityKey);

				object person2 = proxy.PromoteToRealInstance ();

				Assert.AreSame (person1, person2);
			}
		}


		[TestMethod]
		[DeploymentItem ("Cresus.DataLayer.dll")]
		public void PromoteToRealInstanceTest2()
		{
			using (DataContext dataContext = new DataContext (DatabaseHelper.DbInfrastructure))
			{
				EntityKey entityKey = new EntityKey (Druid.Parse("[L0AM]"), new DbKey (new DbId (5)));

				var proxy = new EntityKeyProxy_Accessor (dataContext, entityKey);

				object person2 = proxy.PromoteToRealInstance ();

				Assert.AreSame (UndefinedValue.Value, person2);
			}
		}


	}


}
