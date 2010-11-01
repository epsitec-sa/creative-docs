﻿using Epsitec.Common.UnitTesting;

using Epsitec.Cresus.Database;

using Epsitec.Cresus.DataLayer.Context;
using Epsitec.Cresus.DataLayer.Infrastructure;
using Epsitec.Cresus.DataLayer.UnitTests.Entities;
using Epsitec.Cresus.DataLayer.UnitTests.Helpers;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.Linq;


namespace Epsitec.Cresus.DataLayer.UnitTests.General
{


	[TestClass]
	public class UnitTestEmptyEntity
	{


		[ClassInitialize]
		public static void ClassInitialize(TestContext testContext)
		{
			TestHelper.Initialize ();
		}


		[ClassCleanup]
		public static void ClassCleanup()
		{
			DatabaseHelper.DisconnectFromDatabase ();
		}


		[TestInitialize]
		public void TestInitialize()
		{
			DatabaseHelper.CreateAndConnectToDatabase ();

			Assert.IsTrue (DatabaseHelper.DbInfrastructure.IsConnectionOpen);

			using (DataInfrastructure dataInfrastructure = new DataInfrastructure (DatabaseHelper.DbInfrastructure))
			{
				using (DataContext dataContext = dataInfrastructure.CreateDataContext ())
				{
					DatabaseCreator2.PupulateDatabase (dataContext);
				}
			}
		}


		[TestMethod]
		public void ArgumentCheck()
		{
			using (DataInfrastructure dataInfrastructure = new DataInfrastructure (DatabaseHelper.DbInfrastructure))
			{
				using (DataContext dataContext = dataInfrastructure.CreateDataContext ())
				{
					var alfred = dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1000000001)));

					ExceptionAssert.Throw<System.ArgumentException>
					(
						() => dataContext.RegisterEmptyEntity (alfred)
					);

					ExceptionAssert.Throw<System.ArgumentNullException>
					(
						() => dataContext.RegisterEmptyEntity (null)
					);

					ExceptionAssert.Throw<System.ArgumentNullException>
					(
						() => dataContext.UnregisterEmptyEntity (null)
					);

					ExceptionAssert.Throw<System.ArgumentNullException>
					(
						() => dataContext.UpdateEmptyEntityStatus (null, true)
					);
				}
			}
		}
		

		[TestMethod]
		public void RegistredEmptyEntitiesTest1()
		{
			using (DataInfrastructure dataInfrastructure = new DataInfrastructure (DatabaseHelper.DbInfrastructure))
			{
				using (DataContext dataContext = dataInfrastructure.CreateDataContext ())
				{
					var alfred = dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1000000001)));
					var contact = dataContext.CreateEntity<UriContactEntity> ();

					alfred.Contacts.Add (contact);

					dataContext.RegisterEmptyEntity (contact);

					dataContext.SaveChanges ();
				}

				using (DataContext dataContext = dataInfrastructure.CreateDataContext ())
				{
					var alfred = dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1000000001)));
					var contacts = dataContext.GetByExample (new AbstractContactEntity ());

					Assert.AreEqual (2, alfred.Contacts.Count);
					Assert.AreEqual (4, contacts.Count ());
				}
			}
		}


		[TestMethod]
		public void RegistredEmptyEntitiesTest2()
		{
			using (DataInfrastructure dataInfrastructure = new DataInfrastructure (DatabaseHelper.DbInfrastructure))
			{
				using (DataContext dataContext = dataInfrastructure.CreateDataContext ())
				{
					var alfred = dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1000000001)));
					var contact = dataContext.CreateEntity<UriContactEntity> ();

					dataContext.RegisterEmptyEntity (contact);

					alfred.Contacts.Add (contact);

					dataContext.SaveChanges ();
				}

				using (DataContext dataContext = dataInfrastructure.CreateDataContext ())
				{
					var alfred = dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1000000001)));
					var contacts = dataContext.GetByExample (new AbstractContactEntity ());

					Assert.AreEqual (2, alfred.Contacts.Count);
					Assert.AreEqual (4, contacts.Count ());
				}
			}
		}


		[TestMethod]
		public void RegistredEmptyEntitiesTest3()
		{
			using (DataInfrastructure dataInfrastructure = new DataInfrastructure (DatabaseHelper.DbInfrastructure))
			{
				using (DataContext dataContext = dataInfrastructure.CreateDataContext ())
				{
					var contact = dataContext.CreateEntity<UriContactEntity> ();

					dataContext.RegisterEmptyEntity (contact);

					dataContext.SaveChanges ();
				}

				using (DataContext dataContext = dataInfrastructure.CreateDataContext ())
				{
					var contacts = dataContext.GetByExample (new AbstractContactEntity ());

					Assert.AreEqual (4, contacts.Count ());
				}
			}
		}


		[TestMethod]
		public void UnregistredEmptyEntitiesTest1()
		{

			using (DataInfrastructure dataInfrastructure = new DataInfrastructure (DatabaseHelper.DbInfrastructure))
			{
				using (DataContext dataContext = dataInfrastructure.CreateDataContext ())
				{
					var alfred = dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1000000001)));
					var contact = dataContext.CreateEntity<UriContactEntity> ();

					dataContext.RegisterEmptyEntity (contact);
					dataContext.UnregisterEmptyEntity (contact);

					alfred.Contacts.Add (contact);

					dataContext.SaveChanges ();
				}

				using (DataContext dataContext = dataInfrastructure.CreateDataContext ())
				{
					var alfred = dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1000000001)));
					var contacts = dataContext.GetByExample (new AbstractContactEntity ());

					Assert.AreEqual (3, alfred.Contacts.Count);
					Assert.AreEqual (5, contacts.Count ());
				}
			}
		}


		[TestMethod]
		public void UnregistredEmptyEntitiesTest2()
		{
			using (DataInfrastructure dataInfrastructure = new DataInfrastructure (DatabaseHelper.DbInfrastructure))
			{
				using (DataContext dataContext = dataInfrastructure.CreateDataContext ())
				{
					var alfred = dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1000000001)));
					var contact = dataContext.CreateEntity<UriContactEntity> ();

					dataContext.RegisterEmptyEntity (contact);

					alfred.Contacts.Add (contact);

					dataContext.UnregisterEmptyEntity (contact);

					dataContext.SaveChanges ();
				}

				using (DataContext dataContext = dataInfrastructure.CreateDataContext ())
				{
					var alfred = dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1000000001)));
					var contacts = dataContext.GetByExample (new AbstractContactEntity ());

					Assert.AreEqual (3, alfred.Contacts.Count);
					Assert.AreEqual (5, contacts.Count ());
				}
			}
		}


		[TestMethod]
		public void UnregistredEmptyEntitiesTest3()
		{
			using (DataInfrastructure dataInfrastructure = new DataInfrastructure (DatabaseHelper.DbInfrastructure))
			{
				using (DataContext dataContext = dataInfrastructure.CreateDataContext ())
				{
					var alfred = dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1000000001)));
					var contact = dataContext.CreateEntity<UriContactEntity> ();

					alfred.Contacts.Add (contact);

					dataContext.RegisterEmptyEntity (contact);
					dataContext.UnregisterEmptyEntity (contact);

					dataContext.SaveChanges ();
				}

				using (DataContext dataContext = dataInfrastructure.CreateDataContext ())
				{
					var alfred = dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1000000001)));
					var contacts = dataContext.GetByExample (new AbstractContactEntity ());

					Assert.AreEqual (3, alfred.Contacts.Count);
					Assert.AreEqual (5, contacts.Count ());
				}
			}
		}


		[TestMethod]
		public void UnregistredEmptyEntitiesTest4()
		{
			using (DataInfrastructure dataInfrastructure = new DataInfrastructure (DatabaseHelper.DbInfrastructure))
			{
				using (DataContext dataContext = dataInfrastructure.CreateDataContext ())
				{
					var contact = dataContext.CreateEntity<UriContactEntity> ();

					dataContext.RegisterEmptyEntity (contact);
					dataContext.UnregisterEmptyEntity (contact);

					dataContext.SaveChanges ();
				}

				using (DataContext dataContext = dataInfrastructure.CreateDataContext ())
				{
					var contacts = dataContext.GetByExample (new AbstractContactEntity ());

					Assert.AreEqual (5, contacts.Count ());
				}
			}
		}


		[TestMethod]
		public void UnregistredEmptyEntitiesTest5()
		{
			using (DataInfrastructure dataInfrastructure = new DataInfrastructure (DatabaseHelper.DbInfrastructure))
			{
				using (DataContext dataContext = dataInfrastructure.CreateDataContext ())
				{
					var alfred = dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1000000001)));
					var contact = dataContext.CreateEntity<UriContactEntity> ();

					dataContext.RegisterEmptyEntity (contact);

					dataContext.SaveChanges ();

					dataContext.UnregisterEmptyEntity (contact);

					alfred.Contacts.Add (contact);

					dataContext.SaveChanges ();
				}

				using (DataContext dataContext = dataInfrastructure.CreateDataContext ())
				{
					var alfred = dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1000000001)));
					var contacts = dataContext.GetByExample (new AbstractContactEntity ());

					Assert.AreEqual (3, alfred.Contacts.Count);
					Assert.AreEqual (5, contacts.Count ());
				}
			}
		}


		[TestMethod]
		public void UnregistredEmptyEntitiesTest6()
		{
			using (DataInfrastructure dataInfrastructure = new DataInfrastructure (DatabaseHelper.DbInfrastructure))
			{
				using (DataContext dataContext = dataInfrastructure.CreateDataContext ())
				{
					var alfred = dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1000000001)));
					var contact = dataContext.CreateEntity<UriContactEntity> ();

					dataContext.RegisterEmptyEntity (contact);

					alfred.Contacts.Add (contact);

					dataContext.SaveChanges ();

					dataContext.UnregisterEmptyEntity (contact);

					dataContext.SaveChanges ();
				}

				using (DataContext dataContext = dataInfrastructure.CreateDataContext ())
				{
					var alfred = dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1000000001)));
					var contacts = dataContext.GetByExample (new AbstractContactEntity ());

					Assert.AreEqual (3, alfred.Contacts.Count);
					Assert.AreEqual (5, contacts.Count ());
				}
			}
		}


		[TestMethod]
		public void UnregistredEmptyEntitiesTest7()
		{
			using (DataInfrastructure dataInfrastructure = new DataInfrastructure (DatabaseHelper.DbInfrastructure))
			{
				using (DataContext dataContext = dataInfrastructure.CreateDataContext ())
				{
					var alfred = dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1000000001)));
					var contact = dataContext.CreateEntity<UriContactEntity> ();

					alfred.Contacts.Add (contact);

					dataContext.RegisterEmptyEntity (contact);

					dataContext.SaveChanges ();

					dataContext.UnregisterEmptyEntity (contact);

					dataContext.SaveChanges ();
				}

				using (DataContext dataContext = dataInfrastructure.CreateDataContext ())
				{
					var alfred = dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1000000001)));
					var contacts = dataContext.GetByExample (new AbstractContactEntity ());

					Assert.AreEqual (3, alfred.Contacts.Count);
					Assert.AreEqual (5, contacts.Count ());
				}
			}
		}


		[TestMethod]
		public void UnregistredEmptyEntitiesTest8()
		{
			using (DataInfrastructure dataInfrastructure = new DataInfrastructure (DatabaseHelper.DbInfrastructure))
			{
				using (DataContext dataContext = dataInfrastructure.CreateDataContext ())
				{
					var contact = dataContext.CreateEntity<UriContactEntity> ();

					dataContext.RegisterEmptyEntity (contact);

					dataContext.SaveChanges ();

					dataContext.UnregisterEmptyEntity (contact);

					dataContext.SaveChanges ();
				}

				using (DataContext dataContext = dataInfrastructure.CreateDataContext ())
				{
					var contacts = dataContext.GetByExample (new AbstractContactEntity ());

					Assert.AreEqual (5, contacts.Count ());
				}
			}
		}


		[TestMethod]
		public void UpdateEmptyEmptyEntitiesStatusTest1()
		{
			using (DataInfrastructure dataInfrastructure = new DataInfrastructure (DatabaseHelper.DbInfrastructure))
			{
				using (DataContext dataContext = dataInfrastructure.CreateDataContext ())
				{
					var contact = dataContext.CreateEntity<UriContactEntity> ();

					dataContext.UpdateEmptyEntityStatus (contact, true);

					dataContext.SaveChanges ();
				}

				using (DataContext dataContext = dataInfrastructure.CreateDataContext ())
				{
					var contacts = dataContext.GetByExample (new AbstractContactEntity ());

					Assert.AreEqual (4, contacts.Count ());
				}
			}
		}


		[TestMethod]
		public void UpdateEmptyEmptyEntitiesStatusTest2()
		{
			using (DataInfrastructure dataInfrastructure = new DataInfrastructure (DatabaseHelper.DbInfrastructure))
			{
				using (DataContext dataContext = dataInfrastructure.CreateDataContext ())
				{
					var contact = dataContext.CreateEntity<UriContactEntity> ();

					dataContext.RegisterEmptyEntity (contact);

					dataContext.SaveChanges ();

					dataContext.UpdateEmptyEntityStatus (contact, false);

					dataContext.SaveChanges ();
				}

				using (DataContext dataContext = dataInfrastructure.CreateDataContext ())
				{
					var contacts = dataContext.GetByExample (new AbstractContactEntity ());

					Assert.AreEqual (5, contacts.Count ());
				}
			}
		}


		[TestMethod]
		public void CreateEmptyEntityTest()
		{
			using (DataInfrastructure dataInfrastructure = new DataInfrastructure (DatabaseHelper.DbInfrastructure))
			{
				using (DataContext dataContext = dataInfrastructure.CreateDataContext ())
				{
					var contact = dataContext.CreateEntityAndRegisterAsEmpty<UriContactEntity> ();

					dataContext.SaveChanges ();
				}

				using (DataContext dataContext = dataInfrastructure.CreateDataContext ())
				{
					var contacts = dataContext.GetByExample (new AbstractContactEntity ());

					Assert.AreEqual (4, contacts.Count ());
				}
			}
		}


	}


}
