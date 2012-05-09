using Epsitec.Common.Support;
using Epsitec.Common.Types;

using Epsitec.Common.UnitTesting;

using Epsitec.Cresus.Database;

using Epsitec.Cresus.DataLayer.Context;
using Epsitec.Cresus.DataLayer.Infrastructure;
using Epsitec.Cresus.DataLayer.Proxies;
using Epsitec.Cresus.DataLayer.Tests.Vs.Entities;
using Epsitec.Cresus.DataLayer.Tests.Vs.Helpers;

using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace Epsitec.Cresus.DataLayer.Tests.Vs.Proxies
{


	[TestClass]
	public sealed class UnitTestReferenceFieldProxy
	{


		[ClassInitialize]
		public static void ClassInitialize(TestContext testContext)
		{
			TestHelper.Initialize ();

			DatabaseCreator2.ResetPopulatedTestDatabase ();
		}


		[TestMethod]
		public void ReferenceFieldProxyConstructorTest()
		{			
			using (DataInfrastructure dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase ())
			using (DataContext dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
			{
				NaturalPersonEntity person = dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1000000001)));
				Druid fieldId = Druid.Parse ("[J1AN1]");

				var proxy = new ReferenceFieldProxy_Accessor (dataContext, person, fieldId);

				Assert.AreSame (dataContext, proxy.DataContext);
				Assert.AreSame (person, proxy.Entity);
				Assert.AreEqual (fieldId, proxy.FieldId);
			}
		}


		[TestMethod]
		public void ReferenceFieldProxyConstructorArgumentCheck()
		{			
			using (DataInfrastructure dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase ())
			{
				using (DataContext dataContext1 = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
				using (DataContext dataContext2 = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
				{
					NaturalPersonEntity person = dataContext1.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1000000001)));
					Druid fieldId = Druid.Parse ("[J1AN1]");

					ExceptionAssert.Throw<System.ArgumentNullException>
					(
						() => new ReferenceFieldProxy (null, person, fieldId)
					);

					ExceptionAssert.Throw<System.ArgumentNullException>
					(
						() => new ReferenceFieldProxy (dataContext1, null, fieldId)
					);

					ExceptionAssert.Throw<System.ArgumentException>
					(
						() => new ReferenceFieldProxy (dataContext1, person, Druid.Empty)
					);

					ExceptionAssert.Throw<System.ArgumentException>
					(
						() => new ReferenceFieldProxy (dataContext1, person, Druid.Parse ("[J1AJ1]"))
					);

					ExceptionAssert.Throw<System.ArgumentException>
					(
						() => new ReferenceFieldProxy (dataContext1, person, Druid.Parse ("[J1AC1]"))
					);

					ExceptionAssert.Throw<System.ArgumentException>
					(
						() => new ReferenceFieldProxy (dataContext2, person, fieldId)
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

				var proxy = new ReferenceFieldProxy (dataContext, person, fieldId);

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

				var proxy = new ReferenceFieldProxy (dataContext, person, fieldId);

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

				var proxy = new ReferenceFieldProxy (dataContext, person, fieldId);
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

				var proxy = new ReferenceFieldProxy (dataContext, person, fieldId);

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
				NaturalPersonEntity person = dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1000000003)));

				Druid fieldId = Druid.Parse ("[J1AN1]");

				var proxy = new ReferenceFieldProxy (dataContext, person, fieldId);

				object gender = proxy.PromoteToRealInstance ();

				Assert.AreSame (UndefinedValue.Value, gender);
			}
		}


		[TestMethod]
		public void PromoteToRealInstanceTest3()
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

				var proxy = new ReferenceFieldProxy (dataContext2, person2, fieldId);

				object gender = proxy.PromoteToRealInstance ();

				Assert.AreSame (UndefinedValue.Value, gender);
			}
		}


	}


}
