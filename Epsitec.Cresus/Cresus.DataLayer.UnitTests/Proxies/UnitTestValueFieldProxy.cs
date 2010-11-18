﻿using Epsitec.Common.Support;
using Epsitec.Common.Types;

using Epsitec.Common.UnitTesting;

using Epsitec.Cresus.Database;

using Epsitec.Cresus.DataLayer.Context;
using Epsitec.Cresus.DataLayer.Infrastructure;
using Epsitec.Cresus.DataLayer.Proxies;
using Epsitec.Cresus.DataLayer.UnitTests.Entities;
using Epsitec.Cresus.DataLayer.UnitTests.Helpers;

using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace Epsitec.Cresus.DataLayer.UnitTests.Proxies
{


	[TestClass]
	public sealed class UnitTestValueFieldProxy
	{


		[ClassInitialize]
		public static void ClassInitialize(TestContext testContext)
		{
			TestHelper.Initialize ();

			DatabaseHelper.CreateAndConnectToDatabase ();

			DatabaseCreator2.PupulateDatabase ();
		}


		[ClassCleanup]
		public static void ClassCleanup()
		{
			DatabaseHelper.DisconnectFromDatabase ();
		}


		[TestMethod]
		public void ValueFieldProxyConstructorTest()
		{
			using (DataInfrastructure dataInfrastructure = new DataInfrastructure (DatabaseHelper.DbInfrastructure))
			{
				dataInfrastructure.OpenConnection ("id");

				using (DataContext dataContext = dataInfrastructure.CreateDataContext ())
				{
					NaturalPersonEntity person = dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1000000001)));
					Druid fieldId = Druid.Parse ("[L0AV]");

					var proxy = new ValueFieldProxy_Accessor (dataContext, person, fieldId);

					Assert.AreSame (dataContext, proxy.DataContext);
					Assert.AreSame (person, proxy.Entity);
					Assert.AreEqual (fieldId, proxy.FieldId);
				}
			}
		}


		[TestMethod]
		public void ValueFieldProxyConstructorArgumentCheck()
		{
			using (DataInfrastructure dataInfrastructure = new DataInfrastructure (DatabaseHelper.DbInfrastructure))
			{
				dataInfrastructure.OpenConnection ("id");

				using (DataContext dataContext1 = dataInfrastructure.CreateDataContext ())
				using (DataContext dataContext2 = dataInfrastructure.CreateDataContext ())
				{
					NaturalPersonEntity person = dataContext1.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1000000001)));
					Druid fieldId = Druid.Parse ("[L0AV]");

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
						() => new ValueFieldProxy (dataContext1, person, Druid.Parse ("[L0AN]"))
					);

					ExceptionAssert.Throw<System.ArgumentException>
					(
						() => new ValueFieldProxy (dataContext1, person, Druid.Parse ("[L0AD1]"))
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
			using (DataInfrastructure dataInfrastructure = new DataInfrastructure (DatabaseHelper.DbInfrastructure))
			{
				dataInfrastructure.OpenConnection ("id");

				using (DataContext dataContext = dataInfrastructure.CreateDataContext ())
				{
					NaturalPersonEntity person = dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1000000001)));

					var proxy1 = new ValueFieldProxy (dataContext, person, Druid.Parse ("[L0AV]"));
					var proxy2 = new ValueFieldProxy (dataContext, person, Druid.Parse ("[L0A01]"));
					var proxy3 = new ValueFieldProxy (dataContext, person, Druid.Parse ("[L0A61]"));

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


}
