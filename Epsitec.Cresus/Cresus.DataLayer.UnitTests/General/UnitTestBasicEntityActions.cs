//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD


using Epsitec.Cresus.Database;

using Epsitec.Cresus.DataLayer.Context;
using Epsitec.Cresus.DataLayer.UnitTests.Entities;
using Epsitec.Cresus.DataLayer.UnitTests.Helpers;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.Linq;


namespace Epsitec.Cresus.DataLayer.UnitTests.General
{
	
	
	[TestClass]
	public sealed class UnitTestBasicEntityActions
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

			using (DataContext dataContext = new DataContext (DatabaseHelper.DbInfrastructure))
			{
				DatabaseCreator2.PupulateDatabase (dataContext);
			}
		}


		[TestMethod]
		public void SaveWithoutChanges1()
		{
			using (DataContext dataContext = new DataContext(DatabaseHelper.DbInfrastructure))
			{
				dataContext.SaveChanges ();
			}
		}


		[TestMethod]
		public void SaveWithoutChanges2()
		{
			using (DataContext dataContext = new DataContext(DatabaseHelper.DbInfrastructure))
			{
				UriContactEntity[] contacts = {
					dataContext.ResolveEntity<UriContactEntity> (new DbKey (new DbId (1))),
					dataContext.ResolveEntity<UriContactEntity> (new DbKey (new DbId (2))),
					dataContext.ResolveEntity<UriContactEntity> (new DbKey (new DbId (3))),
					dataContext.ResolveEntity<UriContactEntity> (new DbKey (new DbId (4))),
				};

				Assert.AreEqual (4, contacts.Length);
				
				Assert.IsTrue (contacts.Any (c => DatabaseCreator2.CheckUriContact (c, "alfred@coucou.com", "Alfred")));
				Assert.IsTrue (contacts.Any (c => DatabaseCreator2.CheckUriContact (c, "alfred@blabla.com", "Alfred")));
				Assert.IsTrue (contacts.Any (c => DatabaseCreator2.CheckUriContact (c, "gertrude@coucou.com", "Gertrude")));
				Assert.IsTrue (contacts.Any (c => DatabaseCreator2.CheckUriContact (c, "nobody@nowhere.com", null)));

				dataContext.SaveChanges ();
			}
		}


		[TestMethod]
		public void SaveNewEntityWithoutChanges()
		{
			using (DataContext dataContext = new DataContext (DatabaseHelper.DbInfrastructure))
			{
				NaturalPersonEntity person = dataContext.CreateEntity<NaturalPersonEntity> ();

				dataContext.SaveChanges ();
			}

			using (DataContext dataContext = new DataContext (DatabaseHelper.DbInfrastructure))
			{
				NaturalPersonEntity person = dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (4)));

				Assert.IsNotNull (person);
			}
		}


		[TestMethod]
		public void ResolveEntity()
		{
			using (DataContext dataContext = new DataContext(DatabaseHelper.DbInfrastructure))
			{
				NaturalPersonEntity alfred = dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1)));
				NaturalPersonEntity gertrude = dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (2)));
				NaturalPersonEntity hans = dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (3)));

				Assert.IsTrue (DatabaseCreator2.CheckAlfred (alfred));
				Assert.IsTrue (DatabaseCreator2.CheckGertrude (gertrude));
				Assert.IsTrue (DatabaseCreator2.CheckHans (hans));
			}
		}


		[TestMethod]
		public void InsertEntity()
		{
			using (DataContext dataContext = new DataContext(DatabaseHelper.DbInfrastructure))
			{
				NaturalPersonEntity albert1 = dataContext.CreateEntity<NaturalPersonEntity> ();
				PersonGenderEntity gender = dataContext.ResolveEntity<PersonGenderEntity> (new DbKey (new DbId (1)));

				albert1.Firstname = "Albert";
				albert1.Lastname = "Levert";

				albert1.Gender = gender;

				dataContext.SaveChanges ();

				NaturalPersonEntity albert2 = dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (4)));

				Assert.AreSame (albert1, albert2);
			}

			using (DataContext dataContext = new DataContext (DatabaseHelper.DbInfrastructure))
			{
				NaturalPersonEntity albert = dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (4)));
				PersonGenderEntity gender = dataContext.ResolveEntity<PersonGenderEntity> (new DbKey (new DbId (1)));

				Assert.AreEqual ("Albert", albert.Firstname);
				Assert.AreEqual ("Levert", albert.Lastname);
				Assert.IsNull (albert.BirthDate);
				Assert.IsNotNull (albert.Gender);
				Assert.AreEqual (gender, albert.Gender);
				Assert.IsNull (albert.Title);
				Assert.IsNull (albert.PreferredLanguage);
				Assert.AreEqual (0, albert.Contacts.Count);
			}
		}


		[TestMethod]
		public void InsertEntityValue()
		{
			using (DataContext dataContext = new DataContext (DatabaseHelper.DbInfrastructure))
			{
				NaturalPersonEntity albert = dataContext.CreateEntity<NaturalPersonEntity> ();

				albert.Firstname = "Albert";
				albert.Lastname = "Levert";

				dataContext.SaveChanges ();
			}

			using (DataContext dataContext = new DataContext (DatabaseHelper.DbInfrastructure))
			{
				NaturalPersonEntity albert = dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (4)));

				albert.BirthDate = new Common.Types.Date (1954, 12, 31);

				dataContext.SaveChanges ();
			}

			using (DataContext dataContext = new DataContext (DatabaseHelper.DbInfrastructure))
			{
				NaturalPersonEntity albert = dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (4)));

				Assert.AreEqual ("Albert", albert.Firstname);
				Assert.AreEqual ("Levert", albert.Lastname);
				Assert.IsNotNull (albert.BirthDate);
				Assert.AreEqual (new Common.Types.Date (1954, 12, 31), albert.BirthDate);
				Assert.IsNull (albert.Gender);
				Assert.IsNull (albert.Title);
				Assert.IsNull (albert.PreferredLanguage);
				Assert.AreEqual (0, albert.Contacts.Count);
			}
		}


		[TestMethod]
		public void UpdateEntityValue()
		{
			using (DataContext dataContext = new DataContext (DatabaseHelper.DbInfrastructure))
			{
				NaturalPersonEntity albert = dataContext.CreateEntity<NaturalPersonEntity> ();

				albert.Firstname = "Albert";
				albert.Lastname = "Levert";

				dataContext.SaveChanges ();
			}

			using (DataContext dataContext = new DataContext (DatabaseHelper.DbInfrastructure))
			{
				NaturalPersonEntity albert = dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (4)));

				albert.Lastname = "Lebleu";

				dataContext.SaveChanges ();
			}

			using (DataContext dataContext = new DataContext (DatabaseHelper.DbInfrastructure))
			{
				NaturalPersonEntity albert = dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (4)));

				Assert.AreEqual ("Albert", albert.Firstname);
				Assert.AreEqual ("Lebleu", albert.Lastname);
				Assert.IsNull (albert.BirthDate);
				Assert.IsNull (albert.Gender);
				Assert.IsNull (albert.Title);
				Assert.IsNull (albert.PreferredLanguage);
				Assert.AreEqual (0, albert.Contacts.Count);
			}
		}


		[TestMethod]
		public void DeleteEntityValue()
		{
			using (DataContext dataContext = new DataContext (DatabaseHelper.DbInfrastructure))
			{
				NaturalPersonEntity albert = dataContext.CreateEntity<NaturalPersonEntity> ();

				albert.Firstname = "Albert";
				albert.Lastname = "Levert";
				albert.BirthDate = new Common.Types.Date (1954, 12, 31);

				dataContext.SaveChanges ();
			}

			using (DataContext dataContext = new DataContext (DatabaseHelper.DbInfrastructure))
			{
				NaturalPersonEntity albert = dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (4)));

				albert.BirthDate = null;

				dataContext.SaveChanges ();
			}

			using (DataContext dataContext = new DataContext (DatabaseHelper.DbInfrastructure))
			{
				NaturalPersonEntity albert = dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (4)));

				Assert.AreEqual ("Albert", albert.Firstname);
				Assert.AreEqual ("Levert", albert.Lastname);
				Assert.IsNull (albert.BirthDate);
				Assert.IsNull (albert.Gender);
				Assert.IsNull (albert.Title);
				Assert.IsNull (albert.PreferredLanguage);
				Assert.AreEqual (0, albert.Contacts.Count);
			}
		}


		[TestMethod]
		public void InsertEntityReference()
		{
			using (DataContext dataContext = new DataContext (DatabaseHelper.DbInfrastructure))
			{
				NaturalPersonEntity albert = dataContext.CreateEntity<NaturalPersonEntity> ();

				albert.Firstname = "Albert";
				albert.Lastname = "Levert";

				dataContext.SaveChanges ();
			}

			using (DataContext dataContext = new DataContext (DatabaseHelper.DbInfrastructure))
			{
				NaturalPersonEntity albert = dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (4)));
				PersonGenderEntity gender = dataContext.ResolveEntity<PersonGenderEntity> (new DbKey (new DbId (1)));

				albert.Gender = gender;

				dataContext.SaveChanges ();
			}

			using (DataContext dataContext = new DataContext (DatabaseHelper.DbInfrastructure))
			{
				NaturalPersonEntity albert = dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (4)));
				PersonGenderEntity gender = dataContext.ResolveEntity<PersonGenderEntity> (new DbKey (new DbId (1)));

				Assert.AreEqual ("Albert", albert.Firstname);
				Assert.AreEqual ("Levert", albert.Lastname);
				Assert.IsNull (albert.BirthDate);
				Assert.IsNotNull (albert.Gender);
				Assert.AreSame (gender, albert.Gender);
				Assert.IsNull (albert.Title);
				Assert.IsNull (albert.PreferredLanguage);
				Assert.AreEqual (0, albert.Contacts.Count);
			}
		}


		[TestMethod]
		public void UpdateEntityReference()
		{
			using (DataContext dataContext = new DataContext (DatabaseHelper.DbInfrastructure))
			{
				NaturalPersonEntity albert = dataContext.CreateEntity<NaturalPersonEntity> ();
				PersonGenderEntity gender = dataContext.ResolveEntity<PersonGenderEntity> (new DbKey (new DbId (1)));

				albert.Firstname = "Albert";
				albert.Lastname = "Levert";
				albert.Gender = gender;

				dataContext.SaveChanges ();
			}

			using (DataContext dataContext = new DataContext (DatabaseHelper.DbInfrastructure))
			{
				NaturalPersonEntity albert = dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (4)));
				PersonGenderEntity gender = dataContext.ResolveEntity<PersonGenderEntity> (new DbKey (new DbId (2)));

				albert.Gender = gender;

				dataContext.SaveChanges ();
			}

			using (DataContext dataContext = new DataContext (DatabaseHelper.DbInfrastructure))
			{
				NaturalPersonEntity albert = dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (4)));
				PersonGenderEntity gender = dataContext.ResolveEntity<PersonGenderEntity> (new DbKey (new DbId (2)));

				Assert.AreEqual ("Albert", albert.Firstname);
				Assert.AreEqual ("Levert", albert.Lastname);
				Assert.IsNull (albert.BirthDate);
				Assert.IsNotNull (albert.Gender);
				Assert.AreSame (gender, albert.Gender);
				Assert.IsNull (albert.Title);
				Assert.IsNull (albert.PreferredLanguage);
				Assert.AreEqual (0, albert.Contacts.Count);
			}
		}


		[TestMethod]
		public void RemoveEntityReference()
		{
			using (DataContext dataContext = new DataContext (DatabaseHelper.DbInfrastructure))
			{
				NaturalPersonEntity albert = dataContext.CreateEntity<NaturalPersonEntity> ();
				PersonGenderEntity gender = dataContext.ResolveEntity<PersonGenderEntity> (new DbKey (new DbId (1)));

				albert.Firstname = "Albert";
				albert.Lastname = "Levert";
				albert.Gender = gender;

				dataContext.SaveChanges ();
			}

			using (DataContext dataContext = new DataContext (DatabaseHelper.DbInfrastructure))
			{
				NaturalPersonEntity albert = dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (4)));
				
				albert.Gender = null;

				dataContext.SaveChanges ();
			}

			using (DataContext dataContext = new DataContext (DatabaseHelper.DbInfrastructure))
			{
				NaturalPersonEntity albert = dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (4)));

				Assert.AreEqual ("Albert", albert.Firstname);
				Assert.AreEqual ("Levert", albert.Lastname);
				Assert.IsNull (albert.BirthDate);
				Assert.IsNull (albert.Gender);
				Assert.IsNull (albert.Title);
				Assert.IsNull (albert.PreferredLanguage);
				Assert.AreEqual (0, albert.Contacts.Count);
			}
		}


		[TestMethod]
		public void CreateEntityCollection()
		{
			using (DataContext dataContext = new DataContext (DatabaseHelper.DbInfrastructure))
			{
				NaturalPersonEntity albert = dataContext.CreateEntity<NaturalPersonEntity> ();

				albert.Firstname = "Albert";
				albert.Lastname = "Levert";

				dataContext.SaveChanges ();
			}

			using (DataContext dataContext = new DataContext (DatabaseHelper.DbInfrastructure))
			{
				NaturalPersonEntity albert = dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (4)));
				AbstractContactEntity contact = dataContext.ResolveEntity<AbstractContactEntity> (new DbKey (new DbId (4)));

				albert.Contacts.Add (contact);

				dataContext.SaveChanges ();
			}

			using (DataContext dataContext = new DataContext (DatabaseHelper.DbInfrastructure))
			{
				NaturalPersonEntity albert = dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (4)));
				AbstractContactEntity contact = dataContext.ResolveEntity<AbstractContactEntity> (new DbKey (new DbId (4)));

				Assert.AreEqual ("Albert", albert.Firstname);
				Assert.AreEqual ("Levert", albert.Lastname);
				Assert.IsNull (albert.BirthDate);
				Assert.IsNull (albert.Gender);
				Assert.IsNull (albert.Title);
				Assert.IsNull (albert.PreferredLanguage);
				Assert.AreEqual (1, albert.Contacts.Count);
				Assert.AreSame (contact, albert.Contacts[0]);
			}
		}


		[TestMethod]
		public void AddEntityCollection()
		{
			using (DataContext dataContext = new DataContext (DatabaseHelper.DbInfrastructure))
			{
				NaturalPersonEntity albert = dataContext.CreateEntity<NaturalPersonEntity> ();
				AbstractContactEntity contact = dataContext.ResolveEntity<AbstractContactEntity> (new DbKey (new DbId (4)));

				albert.Firstname = "Albert";
				albert.Lastname = "Levert";
				albert.Contacts.Add (contact);

				dataContext.SaveChanges ();
			}

			using (DataContext dataContext = new DataContext (DatabaseHelper.DbInfrastructure))
			{
				NaturalPersonEntity albert = dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (4)));

				AbstractContactEntity contact = dataContext.ResolveEntity<AbstractContactEntity> (new DbKey (new DbId (3)));

				albert.Contacts.Add(contact);

				dataContext.SaveChanges ();
			}

			using (DataContext dataContext = new DataContext (DatabaseHelper.DbInfrastructure))
			{
				NaturalPersonEntity albert = dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (4)));
				AbstractContactEntity contact1 = dataContext.ResolveEntity<AbstractContactEntity> (new DbKey (new DbId (4)));
				AbstractContactEntity contact2 = dataContext.ResolveEntity<AbstractContactEntity> (new DbKey (new DbId (3)));

				Assert.AreEqual ("Albert", albert.Firstname);
				Assert.AreEqual ("Levert", albert.Lastname);
				Assert.IsNull (albert.BirthDate);
				Assert.IsNull (albert.Gender);
				Assert.IsNull (albert.Title);
				Assert.IsNull (albert.PreferredLanguage);
				Assert.AreEqual (2, albert.Contacts.Count);
				Assert.AreEqual (contact1, albert.Contacts[0]);
				Assert.AreEqual (contact2, albert.Contacts[1]);
			}
		}


		[TestMethod]
		public void UpdateEntityCollection()
		{
			using (DataContext dataContext = new DataContext (DatabaseHelper.DbInfrastructure))
			{
				NaturalPersonEntity albert = dataContext.CreateEntity<NaturalPersonEntity> ();
				AbstractContactEntity contact = dataContext.ResolveEntity<AbstractContactEntity> (new DbKey (new DbId (4)));

				albert.Firstname = "Albert";
				albert.Lastname = "Levert";
				albert.Contacts.Add (contact);

				dataContext.SaveChanges ();
			}

			using (DataContext dataContext = new DataContext (DatabaseHelper.DbInfrastructure))
			{
				NaturalPersonEntity albert = dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (4)));

				AbstractContactEntity contact = dataContext.ResolveEntity<AbstractContactEntity> (new DbKey (new DbId (3)));

				albert.Contacts[0] = contact;

				dataContext.SaveChanges ();
			}

			using (DataContext dataContext = new DataContext (DatabaseHelper.DbInfrastructure))
			{
				NaturalPersonEntity albert = dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (4)));
				AbstractContactEntity contact = dataContext.ResolveEntity<AbstractContactEntity> (new DbKey (new DbId (3)));

				Assert.AreEqual ("Albert", albert.Firstname);
				Assert.AreEqual ("Levert", albert.Lastname);
				Assert.IsNull (albert.BirthDate);
				Assert.IsNull (albert.Gender);
				Assert.IsNull (albert.Title);
				Assert.IsNull (albert.PreferredLanguage);
				Assert.AreEqual (1, albert.Contacts.Count);
				Assert.AreEqual (contact, albert.Contacts[0]);
			}
		}


		[TestMethod]
		public void ReorderEntityCollection()
		{
			using (DataContext dataContext = new DataContext (DatabaseHelper.DbInfrastructure))
			{
				NaturalPersonEntity albert = dataContext.CreateEntity<NaturalPersonEntity> ();
				AbstractContactEntity contact1 = dataContext.ResolveEntity<AbstractContactEntity> (new DbKey (new DbId (4)));
				AbstractContactEntity contact2 = dataContext.ResolveEntity<AbstractContactEntity> (new DbKey (new DbId (3)));

				albert.Firstname = "Albert";
				albert.Lastname = "Levert";
				albert.Contacts.Add (contact1);
				albert.Contacts.Add (contact2);

				dataContext.SaveChanges ();
			}

			using (DataContext dataContext = new DataContext (DatabaseHelper.DbInfrastructure))
			{
				NaturalPersonEntity albert = dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (4)));

				AbstractContactEntity contact1 = dataContext.ResolveEntity<AbstractContactEntity> (new DbKey (new DbId (4)));
				AbstractContactEntity contact2 = dataContext.ResolveEntity<AbstractContactEntity> (new DbKey (new DbId (3)));

				albert.Contacts[0] = contact2;
				albert.Contacts[1] = contact1;

				dataContext.SaveChanges ();
			}

			using (DataContext dataContext = new DataContext (DatabaseHelper.DbInfrastructure))
			{
				NaturalPersonEntity albert = dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (4)));
				AbstractContactEntity contact1 = dataContext.ResolveEntity<AbstractContactEntity> (new DbKey (new DbId (4)));
				AbstractContactEntity contact2 = dataContext.ResolveEntity<AbstractContactEntity> (new DbKey (new DbId (3)));

				Assert.AreEqual ("Albert", albert.Firstname);
				Assert.AreEqual ("Levert", albert.Lastname);
				Assert.IsNull (albert.BirthDate);
				Assert.IsNull (albert.Gender);
				Assert.IsNull (albert.Title);
				Assert.IsNull (albert.PreferredLanguage);
				Assert.AreEqual (2, albert.Contacts.Count);
				Assert.AreEqual (contact2, albert.Contacts[0]);
				Assert.AreEqual (contact1, albert.Contacts[1]);
			}
		}


		[TestMethod]
		public void RemoveEntityCollection()
		{
			using (DataContext dataContext = new DataContext (DatabaseHelper.DbInfrastructure))
			{
				NaturalPersonEntity albert = dataContext.CreateEntity<NaturalPersonEntity> ();
				AbstractContactEntity contact = dataContext.ResolveEntity<AbstractContactEntity> (new DbKey (new DbId (4)));

				albert.Firstname = "Albert";
				albert.Lastname = "Levert";
				albert.Contacts.Add (contact);

				dataContext.SaveChanges ();
			}

			using (DataContext dataContext = new DataContext (DatabaseHelper.DbInfrastructure))
			{
				NaturalPersonEntity albert = dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (4)));

				albert.Contacts.RemoveAt (0);

				dataContext.SaveChanges ();
			}

			using (DataContext dataContext = new DataContext (DatabaseHelper.DbInfrastructure))
			{
				NaturalPersonEntity albert = dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (4)));

				Assert.AreEqual ("Albert", albert.Firstname);
				Assert.AreEqual ("Levert", albert.Lastname);
				Assert.IsNull (albert.BirthDate);
				Assert.IsNull (albert.Gender);
				Assert.IsNull (albert.Title);
				Assert.IsNull (albert.PreferredLanguage);
				Assert.AreEqual (0, albert.Contacts.Count);
			}
		}


		[TestMethod]
		public void NullElementInEntityCollection()
		{using (DataContext dataContext = new DataContext (DatabaseHelper.DbInfrastructure))
			{
				NaturalPersonEntity albert = dataContext.CreateEntity<NaturalPersonEntity> ();
				AbstractContactEntity contact = dataContext.ResolveEntity<AbstractContactEntity> (new DbKey (new DbId (4)));

				albert.Firstname = "Albert";
				albert.Lastname = "Levert";
				albert.Contacts.Add (contact);

				dataContext.SaveChanges ();
			}

			using (DataContext dataContext = new DataContext (DatabaseHelper.DbInfrastructure))
			{
				NaturalPersonEntity albert = dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (4)));

				albert.Contacts.Add (null);

				dataContext.SaveChanges ();
			}

			using (DataContext dataContext = new DataContext (DatabaseHelper.DbInfrastructure))
			{
				NaturalPersonEntity albert = dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (4)));
				AbstractContactEntity contact = dataContext.ResolveEntity<AbstractContactEntity> (new DbKey (new DbId (4)));

				Assert.AreEqual ("Albert", albert.Firstname);
				Assert.AreEqual ("Levert", albert.Lastname);
				Assert.IsNull (albert.BirthDate);
				Assert.IsNull (albert.Gender);
				Assert.IsNull (albert.Title);
				Assert.IsNull (albert.PreferredLanguage);
				Assert.AreEqual (1, albert.Contacts.Count);
				Assert.AreSame (contact, albert.Contacts.First ());
			}
		}


		[TestMethod]
		public void DeleteEntityPresentInCollectionMemory()
		{
			using (DataContext dataContext = new DataContext(DatabaseHelper.DbInfrastructure))
			{
				NaturalPersonEntity alfred = dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1)));

				Assert.IsTrue (DatabaseCreator2.CheckAlfred (alfred));

				dataContext.DeleteEntity (alfred.Contacts[0]);

				dataContext.SaveChanges ();

				Assert.IsTrue (alfred.Contacts.Count == 1);
				Assert.IsTrue (alfred.Contacts.Any (c => DatabaseCreator2.CheckUriContact (c as UriContactEntity, "alfred@blabla.com", "Alfred")));
			}

			using (DataContext dataContext = new DataContext(DatabaseHelper.DbInfrastructure))
			{
				NaturalPersonEntity alfred = dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1)));

				Assert.IsTrue (alfred.Contacts.Count == 1);
				Assert.IsTrue (alfred.Contacts.Any (c => DatabaseCreator2.CheckUriContact (c as UriContactEntity, "alfred@blabla.com", "Alfred")));
			}
		}


		[TestMethod]
		public void DeleteEntityPresentInReferenceMemory()
		{
			using (DataContext dataContext = new DataContext(DatabaseHelper.DbInfrastructure))
			{
				UriContactEntity[] contacts = {
					dataContext.ResolveEntity<UriContactEntity> (new DbKey (new DbId (1))),
					dataContext.ResolveEntity<UriContactEntity> (new DbKey (new DbId (2))),
				};

				NaturalPersonEntity alfred = dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1)));
				dataContext.DeleteEntity (alfred);

				Assert.IsTrue (contacts.All (c => c.NaturalPerson == alfred));

				dataContext.SaveChanges ();

				Assert.IsTrue (contacts.All (c => c.NaturalPerson == null));
			}

			using (DataContext dataContext = new DataContext(DatabaseHelper.DbInfrastructure))
			{
				UriContactEntity[] contacts = {
					dataContext.ResolveEntity<UriContactEntity> (new DbKey (new DbId (1))),
					dataContext.ResolveEntity<UriContactEntity> (new DbKey (new DbId (2))),
				};

				Assert.IsTrue (contacts.All (c => c.NaturalPerson == null));
			}
		}


		[TestMethod]
		public void DeleteEntityPresentInCollectionDatabase()
		{
			using (DataContext dataContext = new DataContext(DatabaseHelper.DbInfrastructure))
			{
				UriContactEntity contact = dataContext.ResolveEntity<UriContactEntity> (new DbKey (new DbId (1)));
				dataContext.DeleteEntity (contact);

				dataContext.SaveChanges ();

				NaturalPersonEntity alfred = dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1)));

				Assert.IsTrue (alfred.Contacts.Count == 1);
				Assert.IsTrue (alfred.Contacts.Any (c => DatabaseCreator2.CheckUriContact (c as UriContactEntity, "alfred@blabla.com", "Alfred")));
			}

			using (DataContext dataContext = new DataContext(DatabaseHelper.DbInfrastructure))
			{
				NaturalPersonEntity alfred = dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1)));

				Assert.IsTrue (alfred.Contacts.Count == 1);
				Assert.IsTrue (alfred.Contacts.Any (c => DatabaseCreator2.CheckUriContact (c as UriContactEntity, "alfred@blabla.com", "Alfred")));
			}
		}


		[TestMethod]
		public void DeleteEntityPresentInReferenceDatabase()
		{
			using (DataContext dataContext = new DataContext(DatabaseHelper.DbInfrastructure))
			{
				NaturalPersonEntity alfred = dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1)));
				dataContext.DeleteEntity (alfred);

				dataContext.SaveChanges ();

				UriContactEntity[] contacts = {
					dataContext.ResolveEntity<UriContactEntity> (new DbKey (new DbId (1))),
					dataContext.ResolveEntity<UriContactEntity> (new DbKey (new DbId (2))),
				};

				Assert.IsTrue (contacts.All (c => c.NaturalPerson == null));
			}

			using (DataContext dataContext = new DataContext(DatabaseHelper.DbInfrastructure))
			{
				UriContactEntity[] contacts = {
					dataContext.ResolveEntity<UriContactEntity> (new DbKey (new DbId (1))),
					dataContext.ResolveEntity<UriContactEntity> (new DbKey (new DbId (2))),
				};

				Assert.IsTrue (contacts.All (c => c.NaturalPerson == null));
			}
		}


		[TestMethod]
		public void RevertModifiedEntity()
		{
			string firstName1 = "Alfred";
			string firstName2 = "Albert";
			
			using (DataContext dataContext = new DataContext (DatabaseHelper.DbInfrastructure))
			{
				NaturalPersonEntity alfred = dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1)));
				Assert.AreEqual (firstName1, alfred.Firstname);

				alfred.Firstname = firstName2;
				Assert.AreEqual (firstName2, alfred.Firstname);

				dataContext.SaveChanges ();
				Assert.AreEqual (firstName2, alfred.Firstname);

				alfred.Firstname = firstName1;
				Assert.AreEqual (firstName1, alfred.Firstname);

				dataContext.SaveChanges ();
				Assert.AreEqual (firstName1, alfred.Firstname);
			}

			using (DataContext dataContext = new DataContext (DatabaseHelper.DbInfrastructure))
			{
				NaturalPersonEntity alfred = dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1)));
				Assert.AreEqual (firstName1, alfred.Firstname);
			}
		}


	}


}
