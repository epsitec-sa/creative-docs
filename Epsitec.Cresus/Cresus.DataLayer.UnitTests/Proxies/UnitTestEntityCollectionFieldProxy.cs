using Epsitec.Common.Support;
using Epsitec.Common.Types;

using Epsitec.Cresus.Database;

using Epsitec.Cresus.DataLayer.Context;
using Epsitec.Cresus.DataLayer.Proxies;
using Epsitec.Cresus.DataLayer.UnitTests.Entities;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.Collections;


namespace Epsitec.Cresus.DataLayer.UnitTests
{


	[TestClass]
	public sealed class UnitTestEntityCollectionFieldProxy
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
		public void EntityCollectionFieldProxyConstructorTest1()
		{
			using (DataContext dataContext = new DataContext (DatabaseHelper.DbInfrastructure))
			{
				NaturalPersonEntity person = dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1)));
				Druid fieldId = Druid.Parse ("[L0AS]");

				var proxy = new EntityCollectionFieldProxy_Accessor (dataContext, person, fieldId);

				Assert.AreSame (dataContext, proxy.dataContext);
				Assert.AreSame (person, proxy.entity);
				Assert.AreEqual (fieldId, proxy.fieldId);
			}
		}


		[TestMethod]
		[DeploymentItem ("Cresus.DataLayer.dll")]
		[ExpectedException (typeof (System.ArgumentNullException))]
		public void EntityCollectionFieldProxyConstructorTest2()
		{
			using (DataContext dataContext = new DataContext (DatabaseHelper.DbInfrastructure))
			{
				NaturalPersonEntity person = dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1)));
				Druid fieldId = Druid.Parse ("[L0AS]");

				new EntityCollectionFieldProxy_Accessor (null, person, fieldId);
			}
		}


		[TestMethod]
		[DeploymentItem ("Cresus.DataLayer.dll")]
		[ExpectedException (typeof (System.ArgumentNullException))]
		public void EntityCollectionFieldProxyConstructorTest3()
		{
			using (DataContext dataContext = new DataContext (DatabaseHelper.DbInfrastructure))
			{
				Druid fieldId = Druid.Parse ("[L0AS]");

				new EntityCollectionFieldProxy_Accessor (dataContext, null, fieldId);
			}
		}


		[TestMethod]
		[DeploymentItem ("Cresus.DataLayer.dll")]
		[ExpectedException (typeof (System.ArgumentException))]
		public void EntityCollectionFieldProxyConstructorTest4()
		{
			using (DataContext dataContext = new DataContext (DatabaseHelper.DbInfrastructure))
			{
				NaturalPersonEntity person = dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1)));
				
				new EntityCollectionFieldProxy_Accessor (dataContext, person, Druid.Empty);
			}
		}


		[TestMethod]
		[DeploymentItem ("Cresus.DataLayer.dll")]
		public void DiscardWriteEntityValueTest()
		{
			using (DataContext dataContext = new DataContext (DatabaseHelper.DbInfrastructure))
			{
				NaturalPersonEntity person = dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1)));
				Druid fieldId = Druid.Parse ("[L0AS]");

				var proxy = new EntityCollectionFieldProxy_Accessor (dataContext, person, fieldId);

				object obj = new object ();

				Assert.IsFalse (proxy.DiscardWriteEntityValue (new TestStore (), "L0AS", ref obj));
			}
		}


		[TestMethod]
		[DeploymentItem ("Cresus.DataLayer.dll")]
		public void GetReadEntityValueTest()
		{
			using (DataContext dataContext = new DataContext (DatabaseHelper.DbInfrastructure))
			{
				NaturalPersonEntity person = dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1)));
				IList contacts1 = person.Contacts as IList;

				Druid fieldId = Druid.Parse ("[L0AS]");

				var proxy = new EntityCollectionFieldProxy_Accessor (dataContext, person, fieldId);

				TestStore testStore = new TestStore ();

				IList contacts2 = proxy.GetReadEntityValue(testStore, "L0AS") as IList;
				IList contacts3 = testStore.GetValue ("L0AS") as IList;

				CollectionAssert.AreEqual (contacts1, contacts2);
				CollectionAssert.AreEqual (contacts1, contacts3);
			}
		}


		[TestMethod]
		[DeploymentItem ("Cresus.DataLayer.dll")]
		public void GetWriteEntityValueTest()
		{
			using (DataContext dataContext = new DataContext (DatabaseHelper.DbInfrastructure))
			{
				NaturalPersonEntity person = dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1)));
				Druid fieldId = Druid.Parse ("[L0AS]");

				var proxy = new EntityCollectionFieldProxy_Accessor (dataContext, person, fieldId);
				object gender = proxy.GetWriteEntityValue (new TestStore (), "L0AS");


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
				IList contacts1 = person.Contacts as IList;
				
				Druid fieldId = Druid.Parse ("[L0AS]");
				
				var proxy = new EntityCollectionFieldProxy_Accessor (dataContext, person, fieldId);

				IList contacts2 = proxy.PromoteToRealInstance () as IList;

				CollectionAssert.AreEqual (contacts1, contacts2);
			}
		}


		[TestMethod]
		[DeploymentItem ("Cresus.DataLayer.dll")]
		public void PromoteToRealInstanceTest2()
		{
			using (DataContext dataContext = new DataContext (DatabaseHelper.DbInfrastructure))
			{
				NaturalPersonEntity person = dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (3)));
				Druid fieldId = Druid.Parse ("[L0AS]");

				var proxy = new EntityCollectionFieldProxy_Accessor (dataContext, person, fieldId);

				object contacts = proxy.PromoteToRealInstance ();

				Assert.AreSame (UndefinedValue.Value, contacts);
			}
		}


	}


}
