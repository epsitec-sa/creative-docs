using Epsitec.Common.Support;
using Epsitec.Common.Types;

using Epsitec.Common.UnitTesting;

using Epsitec.Cresus.Database;

using Epsitec.Cresus.DataLayer.Context;
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
		public void KeyedCollectionFieldProxyConstructorTest()
		{
			using (DataContext dataContext = new DataContext (DatabaseHelper.DbInfrastructure))
			{
				NaturalPersonEntity person = dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1)));
				Druid fieldId = Druid.Parse ("[L0AS]");
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
			using (DataContext dataContext1 = new DataContext (DatabaseHelper.DbInfrastructure))
			{
				using (DataContext dataContext2 = new DataContext (DatabaseHelper.DbInfrastructure))
				{
					NaturalPersonEntity person = dataContext1.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1)));
					Druid fieldId = Druid.Parse ("[L0AS]");
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
						() => new KeyedCollectionFieldProxy (dataContext1, person, Druid.Parse ("[L0AN]"), targetKeys)
					);

					ExceptionAssert.Throw<System.ArgumentException>
					(
						() => new KeyedCollectionFieldProxy (dataContext1, person, Druid.Parse ("[L0AD1]"), targetKeys)
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
			using (DataContext dataContext = new DataContext (DatabaseHelper.DbInfrastructure))
			{
				NaturalPersonEntity person = dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1)));
				Druid fieldId = Druid.Parse ("[L0AS]");
				List<EntityKey> targetKeys = person.Contacts.Select (c => dataContext.GetNormalizedEntityKey (c).Value).ToList ();

				var proxy = new KeyedCollectionFieldProxy (dataContext, person, fieldId, targetKeys);

				object obj = new object ();

				Assert.IsFalse (proxy.DiscardWriteEntityValue (new TestStore (), "L0AS", ref obj));
			}
		}


		[TestMethod]
		public void GetReadEntityValueTest()
		{
			using (DataContext dataContext = new DataContext (DatabaseHelper.DbInfrastructure))
			{
				NaturalPersonEntity person = dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1)));
				IList contacts1 = person.Contacts as IList;

				Druid fieldId = Druid.Parse ("[L0AS]");
				List<EntityKey> targetKeys = person.Contacts.Select (c => dataContext.GetNormalizedEntityKey (c).Value).ToList ();

				var proxy = new KeyedCollectionFieldProxy (dataContext, person, fieldId, targetKeys);

				TestStore testStore = new TestStore ();

				IList contacts2 = proxy.GetReadEntityValue (testStore, "L0AS") as IList;
				IList contacts3 = testStore.GetValue ("L0AS") as IList;

				CollectionAssert.AreEqual (contacts1, contacts2);
				CollectionAssert.AreEqual (contacts1, contacts3);
			}
		}


		[TestMethod]
		public void GetWriteEntityValueTest()
		{
			using (DataContext dataContext = new DataContext (DatabaseHelper.DbInfrastructure))
			{
				NaturalPersonEntity person = dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1)));
				Druid fieldId = Druid.Parse ("[L0AS]");
				List<EntityKey> targetKeys = person.Contacts.Select (c => dataContext.GetNormalizedEntityKey (c).Value).ToList ();

				var proxy = new KeyedCollectionFieldProxy (dataContext, person, fieldId, targetKeys);
				object gender = proxy.GetWriteEntityValue (new TestStore (), "L0AS");

				Assert.AreSame (proxy, gender);
			}
		}


		[TestMethod]
		public void PromoteToRealInstanceTest1()
		{
			using (DataContext dataContext = new DataContext (DatabaseHelper.DbInfrastructure))
			{
				NaturalPersonEntity person = dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1)));
				IList contacts1 = person.Contacts as IList;
				List<EntityKey> targetKeys = person.Contacts.Select (c => dataContext.GetNormalizedEntityKey (c).Value).ToList ();

				Druid fieldId = Druid.Parse ("[L0AS]");

				var proxy = new KeyedCollectionFieldProxy (dataContext, person, fieldId, targetKeys);

				IList contacts2 = proxy.PromoteToRealInstance () as IList;

				CollectionAssert.AreEqual (contacts1, contacts2);
			}
		}


		[TestMethod]
		public void PromoteToRealInstanceTest2()
		{
			using (DataContext dataContext = new DataContext (DatabaseHelper.DbInfrastructure))
			{
				NaturalPersonEntity person = dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1)));
				Druid fieldId = Druid.Parse ("[L0AS]");
				List<EntityKey> targetKeys = new List<EntityKey> ()
				{
					new EntityKey (Druid.Parse("[L0AP]"), new DbKey (new DbId(1))),
					new EntityKey (Druid.Parse("[L0AP]"), new DbKey (new DbId(2))),
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
			using (DataContext dataContext = new DataContext (DatabaseHelper.DbInfrastructure))
			{
				NaturalPersonEntity person = dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (3)));
				Druid fieldId = Druid.Parse ("[L0AS]");
				List<EntityKey> targetKeys = new List<EntityKey> ()
				{
					new EntityKey (Druid.Parse("[L0AP]"), new DbKey (new DbId(5))),
					new EntityKey (Druid.Parse("[L0AP]"), new DbKey (new DbId(6))),
				};

				var proxy = new KeyedCollectionFieldProxy (dataContext, person, fieldId, targetKeys);

				object contacts = proxy.PromoteToRealInstance ();

				Assert.AreSame (UndefinedValue.Value, contacts);
			}
		}


	}


}
