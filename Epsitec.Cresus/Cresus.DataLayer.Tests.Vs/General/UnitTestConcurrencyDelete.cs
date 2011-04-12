using Epsitec.Cresus.Database;

using Epsitec.Cresus.DataLayer.Context;
using Epsitec.Cresus.DataLayer.Infrastructure;
using Epsitec.Cresus.DataLayer.Schema;
using Epsitec.Cresus.DataLayer.Tests.Vs.Entities;
using Epsitec.Cresus.DataLayer.Tests.Vs.Helpers;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.Linq;


namespace Epsitec.Cresus.DataLayer.Tests.Vs.General
{


	[TestClass]
	public class UnitTestConcurrencyDelete
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
		public void DoubleDelete()
		{
			DbKey key = new DbKey (new DbId (1000000001));
					
			using (DataInfrastructure dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase ())
			{
				using (DataContext dataContext1 = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
				using (DataContext dataContext2 = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
				{
					NaturalPersonEntity person1 = dataContext1.ResolveEntity<NaturalPersonEntity> (key);
					NaturalPersonEntity person2 = dataContext2.ResolveEntity<NaturalPersonEntity> (key);

					Assert.IsNotNull (person1);
					Assert.IsNotNull (person2);

					dataContext1.DeleteEntity (person1);
					dataContext2.DeleteEntity (person2);

					dataContext1.SaveChanges ();
					dataContext2.SaveChanges ();
				}

				using (DataContext dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
				{
					NaturalPersonEntity person = dataContext.ResolveEntity<NaturalPersonEntity> (key);

					Assert.IsNull (person);
				}
			}
		}


		[TestMethod]
		public void DeleteAndUseAsTarget1()
		{
			DbKey keyPerson = new DbKey (new DbId (1000000001));
			DbKey keycontact = new DbKey (new DbId (1000000004));
			
			using (DataInfrastructure dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase ())
			{
				using (DataContext dataContext1 = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
				using (DataContext dataContext2 = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
				{
					NaturalPersonEntity person1 = dataContext1.ResolveEntity<NaturalPersonEntity> (keyPerson);
					NaturalPersonEntity person2 = dataContext2.ResolveEntity<NaturalPersonEntity> (keyPerson);
					AbstractContactEntity contact2 = dataContext2.ResolveEntity<AbstractContactEntity> (keycontact);

					Assert.IsNotNull (person1);
					Assert.IsNotNull (person2);
					Assert.IsNotNull (contact2);

					dataContext1.DeleteEntity (person1);
					contact2.NaturalPerson = person2;

					dataContext1.SaveChanges ();
					dataContext2.SaveChanges ();

					// TODO Here there is a row which is inserted in the table that stores
					// the relation between the contacts and the persons. Therefore in the
					// database, there is a relation from the contact to the person, even if
					// the person does not exist anymore. When the person of the contact is
					// resolved, no person with the given id exists, so it is as if there is
					// nothing in the row.
					// The behavior is fine, but we have now polluted the database with some
					// garbage stuff. Hopefully, when the field will be modified the next
					// time, this garbage will be trashed.
					// Marc
				}

				using (DataContext dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
				{
					NaturalPersonEntity person = dataContext.ResolveEntity<NaturalPersonEntity> (keyPerson);
					AbstractContactEntity contact = dataContext.ResolveEntity<AbstractContactEntity> (keycontact);

					Assert.IsNull (person);
					Assert.IsNull (contact.NaturalPerson);

				}
			}
		}


		[TestMethod]
		public void DeleteAndUseAsTarget2()
		{
			DbKey keyPerson = new DbKey (new DbId (1000000003));
			DbKey keycontact = new DbKey (new DbId (1000000001));
			
			using (DataInfrastructure dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase ())
			{
				using (DataContext dataContext1 = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
				using (DataContext dataContext2 = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
				{
					AbstractContactEntity contact1 = dataContext1.ResolveEntity<AbstractContactEntity> (keycontact);
					AbstractContactEntity contact2 = dataContext2.ResolveEntity<AbstractContactEntity> (keycontact);
					NaturalPersonEntity person2 = dataContext2.ResolveEntity<NaturalPersonEntity> (keyPerson);

					Assert.IsNotNull (contact1);
					Assert.IsNotNull (contact2);
					Assert.IsNotNull (person2);

					dataContext1.DeleteEntity (contact1);
					person2.Contacts.Add (contact2);

					dataContext1.SaveChanges ();
					dataContext2.SaveChanges ();

					// TODO Here there is a row which is inserted in the table that stores
					// the relation between the persons and the contacts. Therefore in the
					// database, there is a relation from the person to the contact, even if
					// the contact does not exist anymore. When the contacts of the person are
					// resolved, no contact with the given id exists, so it is as if there is
					// nothing in the row.
					// The behavior is fine, but we have now polluted the database with some
					// garbage stuff. Hopefully, when the field will be modified the next
					// time, this garbage will be trashed.
					// Marc
				}

				using (DataContext dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
				{
					NaturalPersonEntity person = dataContext.ResolveEntity<NaturalPersonEntity> (keyPerson);
					AbstractContactEntity contact = dataContext.ResolveEntity<AbstractContactEntity> (keycontact);

					Assert.IsNull (contact);
					Assert.IsFalse (person.Contacts.Any ());
				}
			}
		}


		[TestMethod]
		public void DeleteAndUseAsTarget3()
		{
			DbKey keyPerson = new DbKey (new DbId (1000000003));
			DbKey keycontactA = new DbKey (new DbId (1000000001));
			DbKey keycontactB = new DbKey (new DbId (1000000002));
			DbKey keycontactC = new DbKey (new DbId (1000000003));
			
			using (DataInfrastructure dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase ())
			{
				using (DataContext dataContext1 = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
				using (DataContext dataContext2 = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
				{
					AbstractContactEntity contactA1 = dataContext1.ResolveEntity<AbstractContactEntity> (keycontactA);
					AbstractContactEntity contactA2 = dataContext2.ResolveEntity<AbstractContactEntity> (keycontactA);
					AbstractContactEntity contactB2 = dataContext2.ResolveEntity<AbstractContactEntity> (keycontactB);
					AbstractContactEntity contactC2 = dataContext2.ResolveEntity<AbstractContactEntity> (keycontactC);
					NaturalPersonEntity person2 = dataContext2.ResolveEntity<NaturalPersonEntity> (keyPerson);

					Assert.IsNotNull (contactA1);
					Assert.IsNotNull (contactA2);
					Assert.IsNotNull (contactB2);
					Assert.IsNotNull (contactC2);
					Assert.IsNotNull (person2);

					dataContext1.DeleteEntity (contactA1);
					person2.Contacts.Add (contactB2);
					person2.Contacts.Add (contactA2);
					person2.Contacts.Add (contactC2);

					dataContext1.SaveChanges ();
					dataContext2.SaveChanges ();

					// TODO Here there is a row which is inserted in the table that stores
					// the relation between the persons and the contacts. Therefore in the
					// database, there is a relation from the person to the contact, even if
					// the contact does not exist anymore. When the contacts of the person are
					// resolved, no contact with the given id exists, so it is as if there is
					// nothing in the row.
					// The behavior is fine, but we have now polluted the database with some
					// garbage stuff. Hopefully, when the field will be modified the next
					// time, this garbage will be trashed.
					// Marc
				}

				using (DataContext dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
				{
					NaturalPersonEntity person = dataContext.ResolveEntity<NaturalPersonEntity> (keyPerson);
					AbstractContactEntity contactA = dataContext.ResolveEntity<AbstractContactEntity> (keycontactA);
					AbstractContactEntity contactB = dataContext.ResolveEntity<AbstractContactEntity> (keycontactB);
					AbstractContactEntity contactC = dataContext.ResolveEntity<AbstractContactEntity> (keycontactC);

					Assert.IsNull (contactA);
					Assert.IsTrue (person.Contacts.Count == 2);
					Assert.AreSame (contactB, person.Contacts[0]);
					Assert.AreSame (contactC, person.Contacts[1]);
				}
			}
		}

		[TestMethod]
		public void DeleteAndModify1()
		{
			DbKey keyPerson = new DbKey (new DbId (1000000001));
			
			using (DataInfrastructure dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase ())
			{
				using (DataContext dataContext1 = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
				using (DataContext dataContext2 = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
				{
					NaturalPersonEntity person1 = dataContext1.ResolveEntity<NaturalPersonEntity> (keyPerson);
					NaturalPersonEntity person2 = dataContext2.ResolveEntity<NaturalPersonEntity> (keyPerson);

					Assert.IsNotNull (person1);
					Assert.IsNotNull (person2);

					dataContext1.DeleteEntity (person1);
					person2.Firstname = "coucou";

					dataContext1.SaveChanges ();
					dataContext2.SaveChanges ();
				}

				using (DataContext dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
				{
					NaturalPersonEntity person = dataContext.ResolveEntity<NaturalPersonEntity> (keyPerson);

					Assert.IsNull (person);
				}
			}
		}


		[TestMethod]
		public void DeleteAndModify2()
		{
			DbKey keyPerson = new DbKey (new DbId (1000000001));
			DbKey keyTitle = new DbKey (new DbId (1000000001));
			
			using (DataInfrastructure dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase ())
			{
				using (DataContext dataContext1 = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
				using (DataContext dataContext2 = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
				{
					NaturalPersonEntity person1 = dataContext1.ResolveEntity<NaturalPersonEntity> (keyPerson);
					NaturalPersonEntity person2 = dataContext2.ResolveEntity<NaturalPersonEntity> (keyPerson);
					PersonTitleEntity title2 = dataContext2.ResolveEntity<PersonTitleEntity> (keyTitle);

					Assert.IsNotNull (person1);
					Assert.IsNotNull (person2);
					Assert.IsNotNull (title2);

					dataContext1.DeleteEntity (person1);
					person2.Title = title2;

					dataContext1.SaveChanges ();
					dataContext2.SaveChanges ();

					// TODO Here there is a row which is inserted in the table that stores
					// the relation between the persons and the titles. Therefore in the
					// database, there is a relation from the person to the title, even if
					// the person does not exist anymore.
					// The behavior is fine, but we have now polluted the database with some
					// garbage stuff, which is not that fine.
					// Marc
				}

				using (DataContext dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
				{
					NaturalPersonEntity person = dataContext.ResolveEntity<NaturalPersonEntity> (keyPerson);

					Assert.IsNull (person);
				}
			}
		}


		[TestMethod]
		public void DeleteAndModify3()
		{
			DbKey keyPerson = new DbKey (new DbId (1000000001));
			DbKey keyContact = new DbKey (new DbId (1000000004));
			
			using (DataInfrastructure dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase ())
			{
				using (DataContext dataContext1 = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
				using (DataContext dataContext2 = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
				{
					NaturalPersonEntity person1 = dataContext1.ResolveEntity<NaturalPersonEntity> (keyPerson);
					NaturalPersonEntity person2 = dataContext2.ResolveEntity<NaturalPersonEntity> (keyPerson);
					AbstractContactEntity contact2 = dataContext2.ResolveEntity<AbstractContactEntity> (keyContact);

					Assert.IsNotNull (person1);
					Assert.IsNotNull (person2);
					Assert.IsNotNull (contact2);

					dataContext1.DeleteEntity (person1);
					person2.Contacts.Add (contact2);

					dataContext1.SaveChanges ();
					dataContext2.SaveChanges ();

					// TODO Here there are three rows which are inserted in the table that
					// stores the relation between the persons and the contacts. Therefore
					// in the database, there is a relation from the person to the contacts,
					// even if the person does not exist anymore.
					// The behavior is fine, but we have now polluted the database with some
					// garbage stuff, which is not that fine.
					// Marc
				}

				using (DataContext dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
				{
					NaturalPersonEntity person = dataContext.ResolveEntity<NaturalPersonEntity> (keyPerson);

					Assert.IsNull (person);
				}
			}
		}


	}


}
