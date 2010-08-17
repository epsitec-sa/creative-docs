using Epsitec.Common.Support;
using Epsitec.Common.Types;

using Epsitec.Common.UnitTesting;

using Epsitec.Cresus.Database;

using Epsitec.Cresus.DataLayer.Context;
using Epsitec.Cresus.DataLayer.Proxies;
using Epsitec.Cresus.DataLayer.UnitTests.Entities;
using Epsitec.Cresus.DataLayer.UnitTests.Helpers;

using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace Epsitec.Cresus.DataLayer.UnitTests.Proxies
{


	[TestClass]
	public sealed class UnitTestKeyedReferenceFieldProxy
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
		public void KeyedReferenceFieldProxyConstructorTest()
		{
			using (DataContext dataContext = new DataContext (DatabaseHelper.DbInfrastructure))
			{
				NaturalPersonEntity person = dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1)));
				Druid fieldId = Druid.Parse ("[L0A11]");
				EntityKey targetKey = dataContext.GetEntityKey (person.Gender).Value;

				var proxy = new KeyedReferenceFieldProxy_Accessor (dataContext, person, fieldId, targetKey);

				Assert.AreSame (dataContext, proxy.DataContext);
				Assert.AreSame (person, proxy.Entity);
				Assert.AreEqual (fieldId, proxy.FieldId);
				Assert.AreEqual (targetKey, proxy.targetKey);
			}
		}


		[TestMethod]
		public void KeyedReferenceFieldProxyConstructorArgumentCheck()
		{
			using (DataContext dataContext1 = new DataContext (DatabaseHelper.DbInfrastructure))
			{
				using (DataContext dataContext2 = new DataContext (DatabaseHelper.DbInfrastructure))
				{
					NaturalPersonEntity person = dataContext1.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1)));
					Druid fieldId = Druid.Parse ("[L0A11]");
					EntityKey targetKey = dataContext1.GetEntityKey (person.Gender).Value;

					ExceptionAssert.Throw<System.ArgumentNullException>
					(
						() => new KeyedReferenceFieldProxy (null, person, fieldId, targetKey)
					);

					ExceptionAssert.Throw<System.ArgumentNullException>
					(
						() => new KeyedReferenceFieldProxy (dataContext1, null, fieldId, targetKey)
					);

					ExceptionAssert.Throw<System.ArgumentException>
					(
						() => new KeyedReferenceFieldProxy (dataContext1, person, Druid.Empty, targetKey)
					);

					ExceptionAssert.Throw<System.ArgumentException>
					(
						() => new KeyedReferenceFieldProxy (dataContext1, person, Druid.Parse ("[L0AN]"), targetKey)
					);

					ExceptionAssert.Throw<System.ArgumentException>
					(
						() => new KeyedReferenceFieldProxy (dataContext1, person, Druid.Parse ("[L0AS]"), targetKey)
					);

					ExceptionAssert.Throw<System.ArgumentException>
					(
						() => new KeyedReferenceFieldProxy (dataContext2, person, fieldId, targetKey)
					);

					ExceptionAssert.Throw<System.ArgumentException>
					(
						() => new KeyedReferenceFieldProxy (dataContext2, person, fieldId, EntityKey.Empty)
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
				Druid fieldId = Druid.Parse ("[L0A11]");
				EntityKey targetKey = dataContext.GetEntityKey (person.Gender).Value;

				var proxy = new KeyedReferenceFieldProxy (dataContext, person, fieldId, targetKey);

				object obj = new object ();

				Assert.IsFalse (proxy.DiscardWriteEntityValue (new TestStore (), "L0A11", ref obj));
			}
		}


		[TestMethod]
		public void GetReadEntityValueTest()
		{
			using (DataContext dataContext = new DataContext (DatabaseHelper.DbInfrastructure))
			{
				NaturalPersonEntity person = dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1)));
				PersonGenderEntity gender1 = dataContext.ResolveEntity<PersonGenderEntity> (new DbKey (new DbId (1)));

				Druid fieldId = Druid.Parse ("[L0A11]");
				EntityKey targetKey = dataContext.GetEntityKey (person.Gender).Value;

				var proxy = new KeyedReferenceFieldProxy (dataContext, person, fieldId, targetKey);

				TestStore testStore = new TestStore ();

				object gender2 = proxy.GetReadEntityValue (testStore, "L0A11");
				object gender3 = testStore.GetValue ("L0A11");

				Assert.AreSame (gender1, gender2);
				Assert.AreSame (gender1, gender3);
			}
		}


		[TestMethod]
		public void GetWriteEntityValueTest()
		{
			using (DataContext dataContext = new DataContext (DatabaseHelper.DbInfrastructure))
			{
				NaturalPersonEntity person = dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1)));
				Druid fieldId = Druid.Parse ("[L0A11]");
				EntityKey targetKey = dataContext.GetEntityKey (person.Gender).Value;

				var proxy = new KeyedReferenceFieldProxy (dataContext, person, fieldId, targetKey);
				object gender = proxy.GetWriteEntityValue (new TestStore (), "L0A11");
				
				Assert.AreSame (proxy, gender);
			}
		}


		[TestMethod]
		public void PromoteToRealInstanceTest1()
		{
			using (DataContext dataContext = new DataContext (DatabaseHelper.DbInfrastructure))
			{
				NaturalPersonEntity person = dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1)));
				PersonGenderEntity gender1 = dataContext.ResolveEntity<PersonGenderEntity> (new DbKey (new DbId (1)));

				Druid fieldId = Druid.Parse ("[L0A11]");
				EntityKey targetKey = dataContext.GetEntityKey (person.Gender).Value;

				var proxy = new KeyedReferenceFieldProxy (dataContext, person, fieldId, targetKey);

				object gender2 = proxy.PromoteToRealInstance ();

				Assert.AreSame (gender1, gender2);
			}
		}


		[TestMethod]
		public void PromoteToRealInstanceTest2()
		{
			using (DataContext dataContext = new DataContext (DatabaseHelper.DbInfrastructure))
			{
				NaturalPersonEntity person = dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1)));
				
				Druid fieldId = Druid.Parse ("[L0A11]");
				EntityKey targetKey = new EntityKey (Druid.Parse ("[L0AA1]"), new DbKey (new DbId (1)));

				var proxy = new KeyedReferenceFieldProxy (dataContext, person, fieldId, targetKey);

				object gender2 = proxy.PromoteToRealInstance ();
				PersonGenderEntity gender1 = dataContext.ResolveEntity<PersonGenderEntity> (new DbKey (new DbId (1)));

				Assert.AreSame (gender1, gender2);
			}
		}


		[TestMethod]
		public void PromoteToRealInstanceTest3()
		{
			using (DataContext dataContext = new DataContext (DatabaseHelper.DbInfrastructure))
			{
				NaturalPersonEntity person = dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (3)));

				Druid fieldId = Druid.Parse ("[L0A11]");
				EntityKey targetKey = new EntityKey (Druid.Parse ("[L0AA1]"), new DbKey (new DbId (3)));

				var proxy = new KeyedReferenceFieldProxy (dataContext, person, fieldId, targetKey);

				object gender = proxy.PromoteToRealInstance ();

				Assert.AreSame (UndefinedValue.Value, gender);
			}
		}


	}


}
