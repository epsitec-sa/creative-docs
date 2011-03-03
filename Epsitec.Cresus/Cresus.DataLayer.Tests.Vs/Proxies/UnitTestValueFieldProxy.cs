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
	public sealed class UnitTestValueFieldProxy
	{


		[ClassInitialize]
		public static void ClassInitialize(TestContext testContext)
		{
			TestHelper.Initialize ();

			DatabaseCreator2.ResetPopulatedTestDatabase ();
		}


		[TestMethod]
		public void ValueFieldProxyConstructorTest()
		{
			using (DbInfrastructure dbInfrastructure = DbInfrastructureHelper.ConnectToTestDatabase ())
			using (DataInfrastructure dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase (dbInfrastructure))
			using (DataContext dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
			{
				NaturalPersonEntity person = dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1000000001)));
				Druid fieldId = Druid.Parse ("[J1AL1]");

				var proxy = new ValueFieldProxy_Accessor (dataContext, person, fieldId);

				Assert.AreSame (dataContext, proxy.DataContext);
				Assert.AreSame (person, proxy.Entity);
				Assert.AreEqual (fieldId, proxy.FieldId);
			}
		}


		[TestMethod]
		public void ValueFieldProxyConstructorArgumentCheck()
		{
			using (DbInfrastructure dbInfrastructure = DbInfrastructureHelper.ConnectToTestDatabase ())
			using (DataInfrastructure dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase (dbInfrastructure))
			{
				using (DataContext dataContext1 = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
				using (DataContext dataContext2 = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
				{
					NaturalPersonEntity person = dataContext1.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1000000001)));
					Druid fieldId = Druid.Parse ("[J1AL1]");

					ExceptionAssert.Throw<System.ArgumentNullException>
					(
						() => new ValueFieldProxy (null, person, fieldId)
					);

					ExceptionAssert.Throw<System.ArgumentNullException>
					(
						() => new ValueFieldProxy (dataContext1, null, fieldId)
					);

					ExceptionAssert.Throw<System.ArgumentException>
					(
						() => new ValueFieldProxy (dataContext1, person, Druid.Empty)
					);

					ExceptionAssert.Throw<System.ArgumentException>
					(
						() => new ValueFieldProxy (dataContext1, person, Druid.Parse ("[J1AJ1]"))
					);

					ExceptionAssert.Throw<System.ArgumentException>
					(
						() => new ValueFieldProxy (dataContext1, person, Druid.Parse ("[J1AD1]"))
					);

					ExceptionAssert.Throw<System.ArgumentException>
					(
						() => new ValueFieldProxy (dataContext2, person, fieldId)
					);
				}
			}
		}


		[TestMethod]
		public void GetValueTest()
		{
			using (DbInfrastructure dbInfrastructure = DbInfrastructureHelper.ConnectToTestDatabase ())
			using (DataInfrastructure dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase (dbInfrastructure))
			using (DataContext dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
			{
				NaturalPersonEntity person = dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1000000001)));

				var proxy1 = new ValueFieldProxy (dataContext, person, Druid.Parse ("[J1AL1]"));
				var proxy2 = new ValueFieldProxy (dataContext, person, Druid.Parse ("[J1AM1]"));
				var proxy3 = new ValueFieldProxy (dataContext, person, Druid.Parse ("[J1AO1]"));

				object value1 = proxy1.GetValue ();
				object value2 = proxy2.GetValue ();
				object value3 = proxy3.GetValue ();

				Assert.AreEqual ("Alfred", value1);
				Assert.AreEqual ("Dupond", value2);
				Assert.AreEqual (new Date (1950, 12, 31), value3);
			}
		}


	}


}
