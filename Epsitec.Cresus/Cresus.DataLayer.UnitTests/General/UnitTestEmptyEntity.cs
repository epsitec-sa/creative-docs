using Epsitec.Common.UnitTesting;

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


		[TestInitialize]
		public void TestInitialize()
		{
			DatabaseCreator2.ResetPopulatedTestDatabase ();
		}


		[TestMethod]
		public void ArgumentCheck()
		{
			using (DbInfrastructure dbInfrastructure = DbInfrastructureHelper.ConnectToTestDatabase ())
			using (DataInfrastructure dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase (dbInfrastructure))
			using (DataContext dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
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


		[TestMethod]
		public void RegistredEmptyEntitiesTest1()
		{
			using (DbInfrastructure dbInfrastructure = DbInfrastructureHelper.ConnectToTestDatabase ())
			using (DataInfrastructure dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase (dbInfrastructure))
			{
				using (DataContext dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
				{
					var alfred = dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1000000001)));
					var contact = dataContext.CreateEntity<UriContactEntity> ();

					alfred.Contacts.Add (contact);

					dataContext.RegisterEmptyEntity (contact);

					dataContext.SaveChanges ();
				}

				using (DataContext dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
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
			using (DbInfrastructure dbInfrastructure = DbInfrastructureHelper.ConnectToTestDatabase ())
			using (DataInfrastructure dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase (dbInfrastructure))
			{
				using (DataContext dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
				{
					var alfred = dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1000000001)));
					var contact = dataContext.CreateEntity<UriContactEntity> ();

					dataContext.RegisterEmptyEntity (contact);

					alfred.Contacts.Add (contact);

					dataContext.SaveChanges ();
				}

				using (DataContext dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
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
			using (DbInfrastructure dbInfrastructure = DbInfrastructureHelper.ConnectToTestDatabase ())
			using (DataInfrastructure dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase (dbInfrastructure))
			{
				using (DataContext dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
				{
					var contact = dataContext.CreateEntity<UriContactEntity> ();

					dataContext.RegisterEmptyEntity (contact);

					dataContext.SaveChanges ();
				}

				using (DataContext dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
				{
					var contacts = dataContext.GetByExample (new AbstractContactEntity ());

					Assert.AreEqual (4, contacts.Count ());
				}
			}
		}


		[TestMethod]
		public void UnregistredEmptyEntitiesTest1()
		{
			using (DbInfrastructure dbInfrastructure = DbInfrastructureHelper.ConnectToTestDatabase ())
			using (DataInfrastructure dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase (dbInfrastructure))
			{
				using (DataContext dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
				{
					var alfred = dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1000000001)));
					var contact = dataContext.CreateEntity<UriContactEntity> ();

					dataContext.RegisterEmptyEntity (contact);
					dataContext.UnregisterEmptyEntity (contact);

					alfred.Contacts.Add (contact);

					dataContext.SaveChanges ();
				}

				using (DataContext dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
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
			using (DbInfrastructure dbInfrastructure = DbInfrastructureHelper.ConnectToTestDatabase ())
			using (DataInfrastructure dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase (dbInfrastructure))
			{
				using (DataContext dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
				{
					var alfred = dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1000000001)));
					var contact = dataContext.CreateEntity<UriContactEntity> ();

					dataContext.RegisterEmptyEntity (contact);

					alfred.Contacts.Add (contact);

					dataContext.UnregisterEmptyEntity (contact);

					dataContext.SaveChanges ();
				}

				using (DataContext dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
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
			using (DbInfrastructure dbInfrastructure = DbInfrastructureHelper.ConnectToTestDatabase ())
			using (DataInfrastructure dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase (dbInfrastructure))
			{
				using (DataContext dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
				{
					var alfred = dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1000000001)));
					var contact = dataContext.CreateEntity<UriContactEntity> ();

					alfred.Contacts.Add (contact);

					dataContext.RegisterEmptyEntity (contact);
					dataContext.UnregisterEmptyEntity (contact);

					dataContext.SaveChanges ();
				}

				using (DataContext dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
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
			using (DbInfrastructure dbInfrastructure = DbInfrastructureHelper.ConnectToTestDatabase ())
			using (DataInfrastructure dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase (dbInfrastructure))
			{
				using (DataContext dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
				{
					var contact = dataContext.CreateEntity<UriContactEntity> ();

					dataContext.RegisterEmptyEntity (contact);
					dataContext.UnregisterEmptyEntity (contact);

					dataContext.SaveChanges ();
				}

				using (DataContext dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
				{
					var contacts = dataContext.GetByExample (new AbstractContactEntity ());

					Assert.AreEqual (5, contacts.Count ());
				}
			}
		}


		[TestMethod]
		public void UnregistredEmptyEntitiesTest5()
		{
			using (DbInfrastructure dbInfrastructure = DbInfrastructureHelper.ConnectToTestDatabase ())
			using (DataInfrastructure dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase (dbInfrastructure))
			{
				using (DataContext dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
				{
					var alfred = dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1000000001)));
					var contact = dataContext.CreateEntity<UriContactEntity> ();

					dataContext.RegisterEmptyEntity (contact);

					dataContext.SaveChanges ();

					dataContext.UnregisterEmptyEntity (contact);

					alfred.Contacts.Add (contact);

					dataContext.SaveChanges ();
				}

				using (DataContext dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
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
			using (DbInfrastructure dbInfrastructure = DbInfrastructureHelper.ConnectToTestDatabase ())
			using (DataInfrastructure dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase (dbInfrastructure))
			{
				using (DataContext dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
				{
					var alfred = dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1000000001)));
					var contact = dataContext.CreateEntity<UriContactEntity> ();

					dataContext.RegisterEmptyEntity (contact);

					alfred.Contacts.Add (contact);

					dataContext.SaveChanges ();

					dataContext.UnregisterEmptyEntity (contact);

					dataContext.SaveChanges ();
				}

				using (DataContext dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
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
			using (DbInfrastructure dbInfrastructure = DbInfrastructureHelper.ConnectToTestDatabase ())
			using (DataInfrastructure dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase (dbInfrastructure))
			{
				using (DataContext dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
				{
					var alfred = dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1000000001)));
					var contact = dataContext.CreateEntity<UriContactEntity> ();

					alfred.Contacts.Add (contact);

					dataContext.RegisterEmptyEntity (contact);

					dataContext.SaveChanges ();

					dataContext.UnregisterEmptyEntity (contact);

					dataContext.SaveChanges ();
				}

				using (DataContext dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
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
			using (DbInfrastructure dbInfrastructure = DbInfrastructureHelper.ConnectToTestDatabase ())
			using (DataInfrastructure dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase (dbInfrastructure))
			{
				using (DataContext dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
				{
					var contact = dataContext.CreateEntity<UriContactEntity> ();

					dataContext.RegisterEmptyEntity (contact);

					dataContext.SaveChanges ();

					dataContext.UnregisterEmptyEntity (contact);

					dataContext.SaveChanges ();
				}

				using (DataContext dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
				{
					var contacts = dataContext.GetByExample (new AbstractContactEntity ());

					Assert.AreEqual (5, contacts.Count ());
				}
			}
		}


		[TestMethod]
		public void UpdateEmptyEmptyEntitiesStatusTest1()
		{
			using (DbInfrastructure dbInfrastructure = DbInfrastructureHelper.ConnectToTestDatabase ())
			using (DataInfrastructure dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase (dbInfrastructure))
			{
				using (DataContext dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
				{
					var contact = dataContext.CreateEntity<UriContactEntity> ();

					dataContext.UpdateEmptyEntityStatus (contact, true);

					dataContext.SaveChanges ();
				}

				using (DataContext dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
				{
					var contacts = dataContext.GetByExample (new AbstractContactEntity ());

					Assert.AreEqual (4, contacts.Count ());
				}
			}
		}


		[TestMethod]
		public void UpdateEmptyEmptyEntitiesStatusTest2()
		{
			using (DbInfrastructure dbInfrastructure = DbInfrastructureHelper.ConnectToTestDatabase ())
			using (DataInfrastructure dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase (dbInfrastructure))
			{
				using (DataContext dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
				{
					var contact = dataContext.CreateEntity<UriContactEntity> ();

					dataContext.RegisterEmptyEntity (contact);

					dataContext.SaveChanges ();

					dataContext.UpdateEmptyEntityStatus (contact, false);

					dataContext.SaveChanges ();
				}

				using (DataContext dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
				{
					var contacts = dataContext.GetByExample (new AbstractContactEntity ());

					Assert.AreEqual (5, contacts.Count ());
				}
			}
		}


		[TestMethod]
		public void CreateEmptyEntityTest()
		{
			using (DbInfrastructure dbInfrastructure = DbInfrastructureHelper.ConnectToTestDatabase ())
			using (DataInfrastructure dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase (dbInfrastructure))
			{
				using (DataContext dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
				{
					var contact = dataContext.CreateEntityAndRegisterAsEmpty<UriContactEntity> ();

					dataContext.SaveChanges ();
				}

				using (DataContext dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
				{
					var contacts = dataContext.GetByExample (new AbstractContactEntity ());

					Assert.AreEqual (4, contacts.Count ());
				}
			}
		}


	}


}
