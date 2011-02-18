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

			DatabaseCreator2.ResetPopulatedTestDatabase ();
		}


		[TestMethod]
		public void CollectionFieldProxyConstructorTest()
		{
			using (DbInfrastructure dbInfrastructure = DbInfrastructureHelper.ConnectToTestDatabase ())
			using (DataInfrastructure dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase (dbInfrastructure))
			using (DataContext dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
			{
				NaturalPersonEntity person = dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1000000001)));
				Druid fieldId = Druid.Parse ("[J1AC1]");

				var proxy = new CollectionFieldProxy_Accessor (dataContext, person, fieldId);

				Assert.AreSame (dataContext, proxy.DataContext);
				Assert.AreSame (person, proxy.Entity);
				Assert.AreEqual (fieldId, proxy.FieldId);
			}
		}


		[TestMethod]
		public void CollectionFieldProxyConstructorArgumentCheck()
		{
			using (DbInfrastructure dbInfrastructure = DbInfrastructureHelper.ConnectToTestDatabase ())
			using (DataInfrastructure dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase (dbInfrastructure))
			{
				using (DataContext dataContext1 = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
				using (DataContext dataContext2 = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
				{
					NaturalPersonEntity person = dataContext1.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1000000001)));
					Druid fieldId = Druid.Parse ("[J1AC1]");

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
						() => new CollectionFieldProxy (dataContext1, person, Druid.Parse ("[J1AJ1]"))
					);

					ExceptionAssert.Throw<System.ArgumentException>
					(
						() => new CollectionFieldProxy (dataContext1, person, Druid.Parse ("[J1AD1]"))
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
			using (DbInfrastructure dbInfrastructure = DbInfrastructureHelper.ConnectToTestDatabase ())
			using (DataInfrastructure dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase (dbInfrastructure))
			using (DataContext dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
			{
				NaturalPersonEntity person = dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1000000001)));
				Druid fieldId = Druid.Parse ("[J1AC1]");

				var proxy = new CollectionFieldProxy (dataContext, person, fieldId);

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

				var proxy = new CollectionFieldProxy (dataContext, person, fieldId);

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

				var proxy = new CollectionFieldProxy (dataContext, person, fieldId);
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

				Druid fieldId = Druid.Parse ("[J1AC1]");

				var proxy = new CollectionFieldProxy (dataContext, person, fieldId);

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
				NaturalPersonEntity person = dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1000000003)));
				Druid fieldId = Druid.Parse ("[J1AC1]");

				var proxy = new CollectionFieldProxy (dataContext, person, fieldId);

				object contacts = proxy.PromoteToRealInstance ();

				Assert.AreSame (UndefinedValue.Value, contacts);
			}
		}


	}


}
