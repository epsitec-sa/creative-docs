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
using System.Collections.Generic;

using System.Linq;


namespace Epsitec.Cresus.DataLayer.UnitTests.Proxies
{


	[TestClass]
	public sealed class UnitTestKeyedCollectionFieldProxy
	{


		[ClassInitialize]
		public static void ClassInitialize(TestContext testContext)
		{
			TestHelper.Initialize ();

			DatabaseCreator2.ResetPopulatedTestDatabase ();
		}


		[TestMethod]
		public void KeyedCollectionFieldProxyConstructorTest()
		{
			using (DbInfrastructure dbInfrastructure = DbInfrastructureHelper.ConnectToTestDatabase ())
			using (DataInfrastructure dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase (dbInfrastructure))
			using (DataContext dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
			{
				NaturalPersonEntity person = dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1000000001)));
				Druid fieldId = Druid.Parse ("[J1AC1]");
				List<EntityKey> targetKeys = person.Contacts.Select (c => dataContext.GetNormalizedEntityKey (c).Value).ToList ();

				var proxy = new KeyedCollectionFieldProxy_Accessor (dataContext, person, fieldId, targetKeys);

				Assert.AreSame (dataContext, proxy.DataContext);
				Assert.AreSame (person, proxy.Entity);
				Assert.AreEqual (fieldId, proxy.FieldId);
				Assert.IsTrue (targetKeys.SequenceEqual (proxy.targetKeys));
			}
		}


		[TestMethod]
		public void KeyedCollectionFieldProxyConstructorArgumentCheck()
		{
			using (DbInfrastructure dbInfrastructure = DbInfrastructureHelper.ConnectToTestDatabase ())
			using (DataInfrastructure dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase (dbInfrastructure))
			{
				using (DataContext dataContext1 = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
				using (DataContext dataContext2 = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
				{
					NaturalPersonEntity person = dataContext1.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1000000001)));
					Druid fieldId = Druid.Parse ("[J1AC1]");
					List<EntityKey> targetKeys = person.Contacts.Select (c => dataContext1.GetNormalizedEntityKey (c).Value).ToList ();

					ExceptionAssert.Throw<System.ArgumentNullException>
					(
						() => new KeyedCollectionFieldProxy (null, person, fieldId, targetKeys)
					);

					ExceptionAssert.Throw<System.ArgumentNullException>
					(
						() => new KeyedCollectionFieldProxy (dataContext1, null, fieldId, targetKeys)
					);

					ExceptionAssert.Throw<System.ArgumentException>
					(
						() => new KeyedCollectionFieldProxy (dataContext1, person, Druid.Empty, targetKeys)

					);

					ExceptionAssert.Throw<System.ArgumentException>
					(
						() => new KeyedCollectionFieldProxy (dataContext1, person, Druid.Parse ("[J1AJ1]"), targetKeys)
					);

					ExceptionAssert.Throw<System.ArgumentException>
					(
						() => new KeyedCollectionFieldProxy (dataContext1, person, Druid.Parse ("[J1AD1]"), targetKeys)
					);

					ExceptionAssert.Throw<System.ArgumentException>
					(
						() => new KeyedCollectionFieldProxy (dataContext2, person, fieldId, targetKeys)
					);

					ExceptionAssert.Throw<System.ArgumentNullException>
					(
						() => new KeyedCollectionFieldProxy (dataContext1, person, fieldId, null)
					);

					ExceptionAssert.Throw<System.ArgumentException>
					(
						() => new KeyedCollectionFieldProxy (dataContext1, person, fieldId, new List<EntityKey> () { EntityKey.Empty })
					);
				}
			}
		}


		[TestMethod]
		public void DiscardWriteEntityValueTest()
		{
			using (DbInfrastructure dbInfrastructure = DbInfrastructureHelper.ConnectToTestDatabase ())
			using (DataInfrastructure dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase (dbInfrastructure))
			using (DataContext dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
			{
				NaturalPersonEntity person = dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1000000001)));
				Druid fieldId = Druid.Parse ("[J1AC1]");
				List<EntityKey> targetKeys = person.Contacts.Select (c => dataContext.GetNormalizedEntityKey (c).Value).ToList ();

				var proxy = new KeyedCollectionFieldProxy (dataContext, person, fieldId, targetKeys);

				object obj = new object ();

				Assert.IsFalse (proxy.DiscardWriteEntityValue (new TestStore (), "J1AC1", ref obj));
			}
		}


		[TestMethod]
		public void GetReadEntityValueTest()
		{
			using (DbInfrastructure dbInfrastructure = DbInfrastructureHelper.ConnectToTestDatabase ())
			using (DataInfrastructure dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase (dbInfrastructure))
			using (DataContext dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
			{
				NaturalPersonEntity person = dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1000000001)));
				IList contacts1 = person.Contacts as IList;

				Druid fieldId = Druid.Parse ("[J1AC1]");
				List<EntityKey> targetKeys = person.Contacts.Select (c => dataContext.GetNormalizedEntityKey (c).Value).ToList ();

				var proxy = new KeyedCollectionFieldProxy (dataContext, person, fieldId, targetKeys);

				TestStore testStore = new TestStore ();

				IList contacts2 = proxy.GetReadEntityValue (testStore, "J1AC1") as IList;
				IList contacts3 = testStore.GetValue ("J1AC1") as IList;

				CollectionAssert.AreEqual (contacts1, contacts2);
				CollectionAssert.AreEqual (contacts1, contacts3);
			}
		}


		[TestMethod]
		public void GetWriteEntityValueTest()
		{
			using (DbInfrastructure dbInfrastructure = DbInfrastructureHelper.ConnectToTestDatabase ())
			using (DataInfrastructure dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase (dbInfrastructure))
			using (DataContext dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
			{
				NaturalPersonEntity person = dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1000000001)));
				Druid fieldId = Druid.Parse ("[J1AC1]");
				List<EntityKey> targetKeys = person.Contacts.Select (c => dataContext.GetNormalizedEntityKey (c).Value).ToList ();

				var proxy = new KeyedCollectionFieldProxy (dataContext, person, fieldId, targetKeys);
				object gender = proxy.GetWriteEntityValue (new TestStore (), "J1AC1");

				Assert.AreSame (proxy, gender);
			}
		}


		[TestMethod]
		public void PromoteToRealInstanceTest1()
		{
			using (DbInfrastructure dbInfrastructure = DbInfrastructureHelper.ConnectToTestDatabase ())
			using (DataInfrastructure dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase (dbInfrastructure))
			using (DataContext dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
			{
				NaturalPersonEntity person = dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1000000001)));
				IList contacts1 = person.Contacts as IList;
				List<EntityKey> targetKeys = person.Contacts.Select (c => dataContext.GetNormalizedEntityKey (c).Value).ToList ();

				Druid fieldId = Druid.Parse ("[J1AC1]");

				var proxy = new KeyedCollectionFieldProxy (dataContext, person, fieldId, targetKeys);

				IList contacts2 = proxy.PromoteToRealInstance () as IList;

				CollectionAssert.AreEqual (contacts1, contacts2);
			}
		}


		[TestMethod]
		public void PromoteToRealInstanceTest2()
		{
			using (DbInfrastructure dbInfrastructure = DbInfrastructureHelper.ConnectToTestDatabase ())
			using (DataInfrastructure dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase (dbInfrastructure))
			using (DataContext dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
			{
				NaturalPersonEntity person = dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1000000001)));
				Druid fieldId = Druid.Parse ("[J1AC1]");
				List<EntityKey> targetKeys = new List<EntityKey> ()
					{
						new EntityKey (Druid.Parse("[J1AA1]"), new DbKey (new DbId(1))),
						new EntityKey (Druid.Parse("[J1AA1]"), new DbKey (new DbId(2))),
					};

				var proxy = new KeyedCollectionFieldProxy (dataContext, person, fieldId, targetKeys);

				IList contacts2 = proxy.PromoteToRealInstance () as IList;
				IList contacts1 = person.Contacts as IList;

				CollectionAssert.AreEqual (contacts1, contacts2);
			}
		}


		[TestMethod]
		public void PromoteToRealInstanceTest3()
		{
			using (DbInfrastructure dbInfrastructure = DbInfrastructureHelper.ConnectToTestDatabase ())
			using (DataInfrastructure dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase (dbInfrastructure))
			using (DataContext dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
			{
				NaturalPersonEntity person = dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1000000003)));
				Druid fieldId = Druid.Parse ("[J1AC1]");
				List<EntityKey> targetKeys = new List<EntityKey> ()
					{
						new EntityKey (Druid.Parse("[J1AA1]"), new DbKey (new DbId(5))),
						new EntityKey (Druid.Parse("[J1AA1]"), new DbKey (new DbId(6))),
					};

				var proxy = new KeyedCollectionFieldProxy (dataContext, person, fieldId, targetKeys);

				object contacts = proxy.PromoteToRealInstance ();

				Assert.AreSame (UndefinedValue.Value, contacts);
			}
		}


	}


}
