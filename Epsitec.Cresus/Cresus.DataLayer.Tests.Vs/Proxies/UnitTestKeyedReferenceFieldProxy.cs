using Epsitec.Common.Support;
using Epsitec.Common.Types;

using Epsitec.Common.UnitTesting;

using Epsitec.Cresus.Database;

using Epsitec.Cresus.DataLayer.Context;
using Epsitec.Cresus.DataLayer.Infrastructure;
using Epsitec.Cresus.DataLayer.Proxies;
using Epsitec.Cresus.DataLayer.Schema;
using Epsitec.Cresus.DataLayer.Tests.Vs.Entities;
using Epsitec.Cresus.DataLayer.Tests.Vs.Helpers;


using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace Epsitec.Cresus.DataLayer.Tests.Vs.Proxies
{


	[TestClass]
	public sealed class UnitTestKeyedReferenceFieldProxy
	{


		[ClassInitialize]
		public static void ClassInitialize(TestContext testContext)
		{
			TestHelper.Initialize ();

			DatabaseCreator2.ResetPopulatedTestDatabase ();
		}


		[TestMethod]
		public void KeyedReferenceFieldProxyConstructorTest()
		{			
			using (DataInfrastructure dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase ())
			using (DataContext dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
			{
				NaturalPersonEntity person = dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1000000001)));
				Druid fieldId = Druid.Parse ("[J1AN1]");
				EntityKey targetKey = dataContext.GetNormalizedEntityKey (person.Gender).Value;

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
			using (DataInfrastructure dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase ())
			{
				using (DataContext dataContext1 = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
				using (DataContext dataContext2 = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
				{
					NaturalPersonEntity person = dataContext1.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1000000001)));
					Druid fieldId = Druid.Parse ("[J1AN1]");
					EntityKey targetKey = dataContext1.GetNormalizedEntityKey (person.Gender).Value;

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
						() => new KeyedReferenceFieldProxy (dataContext1, person, Druid.Parse ("[J1AJ1]"), targetKey)
					);

					ExceptionAssert.Throw<System.ArgumentException>
					(
						() => new KeyedReferenceFieldProxy (dataContext1, person, Druid.Parse ("[J1AC1]"), targetKey)
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
			using (DataInfrastructure dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase ())
			using (DataContext dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
			{
				NaturalPersonEntity person = dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1000000001)));
				Druid fieldId = Druid.Parse ("[J1AN1]");
				EntityKey targetKey = dataContext.GetNormalizedEntityKey (person.Gender).Value;

				var proxy = new KeyedReferenceFieldProxy (dataContext, person, fieldId, targetKey);

				object obj = new object ();

				Assert.IsFalse (proxy.DiscardWriteEntityValue (new TestStore (), "J1AN1", ref obj));
			}
		}


		[TestMethod]
		public void GetReadEntityValueTest()
		{			
			using (DataInfrastructure dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase ())
			using (DataContext dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
			{
				NaturalPersonEntity person = dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1000000001)));
				PersonGenderEntity gender1 = dataContext.ResolveEntity<PersonGenderEntity> (new DbKey (new DbId (1000000001)));

				Druid fieldId = Druid.Parse ("[J1AN1]");
				EntityKey targetKey = dataContext.GetNormalizedEntityKey (person.Gender).Value;

				var proxy = new KeyedReferenceFieldProxy (dataContext, person, fieldId, targetKey);

				TestStore testStore = new TestStore ();

				object gender2 = proxy.GetReadEntityValue (testStore, "J1AN1");
				object gender3 = testStore.GetValue ("J1AN1");

				Assert.AreSame (gender1, gender2);
				Assert.AreSame (gender1, gender3);
			}
		}


		[TestMethod]
		public void GetWriteEntityValueTest()
		{			
			using (DataInfrastructure dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase ())
			using (DataContext dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
			{
				NaturalPersonEntity person = dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1000000001)));
				Druid fieldId = Druid.Parse ("[J1AN1]");
				EntityKey targetKey = dataContext.GetNormalizedEntityKey (person.Gender).Value;

				var proxy = new KeyedReferenceFieldProxy (dataContext, person, fieldId, targetKey);
				object gender = proxy.GetWriteEntityValue (new TestStore (), "J1AN1");

				Assert.AreSame (proxy, gender);
			}
		}


		[TestMethod]
		public void PromoteToRealInstanceTest1()
		{		
			using (DataInfrastructure dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase ())
			using (DataContext dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
			{
				NaturalPersonEntity person = dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1000000001)));
				PersonGenderEntity gender1 = dataContext.ResolveEntity<PersonGenderEntity> (new DbKey (new DbId (1000000001)));

				Druid fieldId = Druid.Parse ("[J1AN1]");
				EntityKey targetKey = dataContext.GetNormalizedEntityKey (person.Gender).Value;

				var proxy = new KeyedReferenceFieldProxy (dataContext, person, fieldId, targetKey);

				object gender2 = proxy.PromoteToRealInstance ();

				Assert.AreSame (gender1, gender2);
			}
		}


		[TestMethod]
		public void PromoteToRealInstanceTest2()
		{			
			using (DataInfrastructure dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase ())
			using (DataContext dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
			{
				NaturalPersonEntity person = dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1000000001)));

				Druid fieldId = Druid.Parse ("[J1AN1]");
				EntityKey targetKey = new EntityKey (Druid.Parse ("[J1AQ]"), new DbKey (new DbId (1000000001)));

				var proxy = new KeyedReferenceFieldProxy (dataContext, person, fieldId, targetKey);

				object gender2 = proxy.PromoteToRealInstance ();
				PersonGenderEntity gender1 = dataContext.ResolveEntity<PersonGenderEntity> (new DbKey (new DbId (1000000001)));

				Assert.AreSame (gender1, gender2);
			}
		}


		[TestMethod]
		public void PromoteToRealInstanceTest3()
		{		
			using (DataInfrastructure dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase ())
			using (DataContext dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
			{
				NaturalPersonEntity person = dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1000000001)));

				Druid fieldId = Druid.Parse ("[J1AN1]");
				EntityKey targetKey = new EntityKey (Druid.Parse ("[J1AQ]"), new DbKey (new DbId (1000000003)));

				var proxy = new KeyedReferenceFieldProxy (dataContext, person, fieldId, targetKey);

				object gender2 = proxy.PromoteToRealInstance ();
				PersonGenderEntity gender1 = dataContext.ResolveEntity<PersonGenderEntity> (new DbKey (new DbId (1000000001)));

				Assert.AreSame (gender1, gender2);
			}
		}


		[TestMethod]
		public void PromoteToRealInstanceTest4()
		{			
			using (DataInfrastructure dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase ())
			using (DataContext dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
			{
				NaturalPersonEntity person = dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1000000003)));

				Druid fieldId = Druid.Parse ("[J1AN1]");
				EntityKey targetKey = new EntityKey (Druid.Parse ("[J1AQ]"), new DbKey (new DbId (1000000003)));

				var proxy = new KeyedReferenceFieldProxy (dataContext, person, fieldId, targetKey);

				object gender = proxy.PromoteToRealInstance ();

				Assert.AreSame (UndefinedValue.Value, gender);
			}
		}


		[TestMethod]
		public void PromoteToRealInstanceTest5()
		{
			using (DataInfrastructure dataInfrastructure1 = DataInfrastructureHelper.ConnectToTestDatabase ())
			using (DataInfrastructure dataInfrastructure2 = DataInfrastructureHelper.ConnectToTestDatabase ())
			using (DataContext dataContext1 = DataContextHelper.ConnectToTestDatabase (dataInfrastructure1))
			using (DataContext dataContext2 = DataContextHelper.ConnectToTestDatabase (dataInfrastructure2))
			{
				NaturalPersonEntity person1 = dataContext1.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1000000001)));
				NaturalPersonEntity person2 = dataContext2.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1000000001)));

				dataContext1.DeleteEntity (person1);
				dataContext1.SaveChanges ();

				Druid fieldId = Druid.Parse ("[J1AN1]");
				EntityKey targetKey = new EntityKey (Druid.Parse ("[J1AQ]"), new DbKey (new DbId (1000000001)));

				var proxy = new KeyedReferenceFieldProxy (dataContext2, person2, fieldId, targetKey);

				object gender = proxy.PromoteToRealInstance ();

				Assert.AreSame (UndefinedValue.Value, gender);
			}
		}


	}


}
