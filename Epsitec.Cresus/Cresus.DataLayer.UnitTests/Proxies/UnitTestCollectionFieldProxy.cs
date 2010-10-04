using Epsitec.Common.Support;
using Epsitec.Common.Types;

using Epsitec.Common.UnitTesting;

using Epsitec.Cresus.Database;

using Epsitec.Cresus.DataLayer.Context;
using Epsitec.Cresus.DataLayer.Infrastructure;
using Epsitec.Cresus.DataLayer.Proxies;
using Epsitec.Cresus.DataLayer.UnitTests.Entities;
using Epsitec.Cresus.DataLayer.UnitTests.Helpers;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.Collections;


namespace Epsitec.Cresus.DataLayer.UnitTests.Proxies
{


	[TestClass]
	public sealed class UnitTestCollectionFieldProxy
	{


		[ClassInitialize]
		public static void ClassInitialize(TestContext testContext)
		{
			TestHelper.Initialize ();

			DatabaseHelper.CreateAndConnectToDatabase ();

			using (DataInfrastructure dataInfrastructure = new DataInfrastructure (DatabaseHelper.DbInfrastructure))
			{
				using (DataContext dataContext = dataInfrastructure.CreateDataContext ())
				{
					DatabaseCreator2.PupulateDatabase (dataContext);
				}
			}
		}


		[ClassCleanup]
		public static void ClassCleanup()
		{
			DatabaseHelper.DisconnectFromDatabase ();
		}


		[TestMethod]
		public void CollectionFieldProxyConstructorTest()
		{
			using (DataInfrastructure dataInfrastructure = new DataInfrastructure (DatabaseHelper.DbInfrastructure))
			{
				using (DataContext dataContext = dataInfrastructure.CreateDataContext ())
				{
					NaturalPersonEntity person = dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1)));
					Druid fieldId = Druid.Parse ("[L0AS]");

					var proxy = new CollectionFieldProxy_Accessor (dataContext, person, fieldId);

					Assert.AreSame (dataContext, proxy.DataContext);
					Assert.AreSame (person, proxy.Entity);
					Assert.AreEqual (fieldId, proxy.FieldId);
				}
			}
		}


		[TestMethod]
		public void CollectionFieldProxyConstructorArgumentCheck()
		{
			using (DataInfrastructure dataInfrastructure = new DataInfrastructure (DatabaseHelper.DbInfrastructure))
			{
				using (DataContext dataContext1 = dataInfrastructure.CreateDataContext ())
				using (DataContext dataContext2 = dataInfrastructure.CreateDataContext ())
				{
					NaturalPersonEntity person = dataContext1.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1)));
					Druid fieldId = Druid.Parse ("[L0AS]");

					ExceptionAssert.Throw<System.ArgumentNullException>
					(
						() => new CollectionFieldProxy (null, person, fieldId)
					);

					ExceptionAssert.Throw<System.ArgumentNullException>
					(
						() => new CollectionFieldProxy (dataContext1, null, fieldId)
					);

					ExceptionAssert.Throw<System.ArgumentException>
					(
						() => new CollectionFieldProxy (dataContext1, person, Druid.Empty)

					);

					ExceptionAssert.Throw<System.ArgumentException>
					(
						() => new CollectionFieldProxy (dataContext1, person, Druid.Parse ("[L0AN]"))
					);

					ExceptionAssert.Throw<System.ArgumentException>
					(
						() => new CollectionFieldProxy (dataContext1, person, Druid.Parse ("[L0AD1]"))
					);

					ExceptionAssert.Throw<System.ArgumentException>
					(
						() => new CollectionFieldProxy (dataContext2, person, fieldId)
					);
				}
			}
		}


		[TestMethod]
		public void DiscardWriteEntityValueTest()
		{
			using (DataInfrastructure dataInfrastructure = new DataInfrastructure (DatabaseHelper.DbInfrastructure))
			{
				using (DataContext dataContext = dataInfrastructure.CreateDataContext ())
				{
					NaturalPersonEntity person = dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1)));
					Druid fieldId = Druid.Parse ("[L0AS]");

					var proxy = new CollectionFieldProxy (dataContext, person, fieldId);

					object obj = new object ();

					Assert.IsFalse (proxy.DiscardWriteEntityValue (new TestStore (), "L0AS", ref obj));
				}
			}
		}


		[TestMethod]
		public void GetReadEntityValueTest()
		{
			using (DataInfrastructure dataInfrastructure = new DataInfrastructure (DatabaseHelper.DbInfrastructure))
			{
				using (DataContext dataContext = dataInfrastructure.CreateDataContext ())
				{
					NaturalPersonEntity person = dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1)));
					IList contacts1 = person.Contacts as IList;

					Druid fieldId = Druid.Parse ("[L0AS]");

					var proxy = new CollectionFieldProxy (dataContext, person, fieldId);

					TestStore testStore = new TestStore ();

					IList contacts2 = proxy.GetReadEntityValue (testStore, "L0AS") as IList;
					IList contacts3 = testStore.GetValue ("L0AS") as IList;

					CollectionAssert.AreEqual (contacts1, contacts2);
					CollectionAssert.AreEqual (contacts1, contacts3);
				}
			}
		}


		[TestMethod]
		public void GetWriteEntityValueTest()
		{
			using (DataInfrastructure dataInfrastructure = new DataInfrastructure (DatabaseHelper.DbInfrastructure))
			{
				using (DataContext dataContext = dataInfrastructure.CreateDataContext ())
				{
					NaturalPersonEntity person = dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1)));
					Druid fieldId = Druid.Parse ("[L0AS]");

					var proxy = new CollectionFieldProxy (dataContext, person, fieldId);
					object gender = proxy.GetWriteEntityValue (new TestStore (), "L0AS");

					Assert.AreSame (proxy, gender);
				}
			}
		}


		[TestMethod]
		public void PromoteToRealInstanceTest1()
		{
			using (DataInfrastructure dataInfrastructure = new DataInfrastructure (DatabaseHelper.DbInfrastructure))
			{
				using (DataContext dataContext = dataInfrastructure.CreateDataContext ())
				{
					NaturalPersonEntity person = dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1)));
					IList contacts1 = person.Contacts as IList;

					Druid fieldId = Druid.Parse ("[L0AS]");

					var proxy = new CollectionFieldProxy (dataContext, person, fieldId);

					IList contacts2 = proxy.PromoteToRealInstance () as IList;

					CollectionAssert.AreEqual (contacts1, contacts2);
				}
			}
		}


		[TestMethod]
		public void PromoteToRealInstanceTest2()
		{
			using (DataInfrastructure dataInfrastructure = new DataInfrastructure (DatabaseHelper.DbInfrastructure))
			{
				using (DataContext dataContext = dataInfrastructure.CreateDataContext ())
				{
					NaturalPersonEntity person = dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (3)));
					Druid fieldId = Druid.Parse ("[L0AS]");

					var proxy = new CollectionFieldProxy (dataContext, person, fieldId);

					object contacts = proxy.PromoteToRealInstance ();

					Assert.AreSame (UndefinedValue.Value, contacts);
				}
			}
		}


	}


}
