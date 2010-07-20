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
	public sealed class UnitTestValueFieldProxy
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
				Druid fieldId = Druid.Parse ("[L0AV]");

				var proxy = new ValueFieldProxy_Accessor (dataContext, person, fieldId);

				Assert.AreSame (dataContext, proxy.dataContext);
				Assert.AreSame (person, proxy.entity);
				Assert.AreEqual (fieldId, proxy.fieldId);
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
				Druid fieldId = Druid.Parse ("[L0AV]");

				new ValueFieldProxy_Accessor (null, person, fieldId);
			}
		}


		[TestMethod]
		[DeploymentItem ("Cresus.DataLayer.dll")]
		[ExpectedException (typeof (System.ArgumentNullException))]
		public void ValueFieldProxyConstructorTest3()
		{
			using (DataContext dataContext = new DataContext (DatabaseHelper.DbInfrastructure))
			{
				Druid fieldId = Druid.Parse ("[L0AV]");

				new ValueFieldProxy_Accessor (dataContext, null, fieldId);
			}
		}


		[TestMethod]
		[DeploymentItem ("Cresus.DataLayer.dll")]
		[ExpectedException (typeof (System.ArgumentException))]
		public void ValueFieldProxyConstructorTest4()
		{
			using (DataContext dataContext = new DataContext (DatabaseHelper.DbInfrastructure))
			{
				NaturalPersonEntity person = dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1)));

				new ValueFieldProxy_Accessor (dataContext, person, Druid.Empty);
			}
		}


		[TestMethod]
		[DeploymentItem ("Cresus.DataLayer.dll")]
		public void GetValueTest()
		{
			using (DataContext dataContext = new DataContext (DatabaseHelper.DbInfrastructure))
			{
				NaturalPersonEntity person = dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1)));

				var proxy1 = new ValueFieldProxy_Accessor (dataContext, person, Druid.Parse ("[L0AV]"));
				var proxy2 = new ValueFieldProxy_Accessor (dataContext, person, Druid.Parse ("[L0A01]"));
				var proxy3 = new ValueFieldProxy_Accessor (dataContext, person, Druid.Parse ("[L0A61]"));

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
