//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD


using Epsitec.Cresus.Database;

using Epsitec.Cresus.DataLayer.Context;
using Epsitec.Cresus.DataLayer.UnitTests.Entities;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.Linq;


namespace Epsitec.Cresus.DataLayer.UnitTests
{
	
	
	[TestClass]
	public class UnitTestDataContext
	{


		[ClassInitialize]
		public void Initialize(TestContext testContext)
		{
			TestHelper.Initialize ();

			Database2.CreateAndConnectToDatabase ();

			using (DataContext dataContext = new DataContext (Database.DbInfrastructure))
			{
				Database2.PupulateDatabase (dataContext);
			}
		}


		[ClassCleanup]
		public void Cleanup()
		{
			Database.DisconnectFromDatabase ();
		}

				
		[TestMethod]
		public void CreateDatabase()
		{
			TestHelper.PrintStartTest ("Create database");

			this.CreateDatabaseHelper ();
		}


		private void CreateDatabaseHelper()
		{
			Database2.CreateAndConnectToDatabase ();

			Assert.IsTrue (Database2.DbInfrastructure.IsConnectionOpen);

			using (DataContext dataContext = new DataContext (Database.DbInfrastructure))
			{
				Database2.PupulateDatabase (dataContext);
			}
		}


		[TestMethod]
		public void DiscardEmptyEntities()
		{
			TestHelper.PrintStartTest ("Discard empty entities");

			using (var dataContext = new DataContext(Database.DbInfrastructure))
			{
				var alfred = dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1)));

				var emptyContact1 = dataContext.CreateEntity<UriContactEntity> ();
				var emptyContact2 = dataContext.CreateEntity<UriContactEntity> ();

				alfred.Contacts.Add (emptyContact1);

				dataContext.RegisterEmptyEntity (emptyContact1);
				dataContext.RegisterEmptyEntity (emptyContact2);

				dataContext.SaveChanges ();
			}

			using (var dataContext = new DataContext(Database.DbInfrastructure))
			{
				var alfred = dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1)));
				var contacts = dataContext.GetByExample (new AbstractContactEntity ());

				Assert.AreEqual (2, alfred.Contacts.Count);
				Assert.AreEqual (4, contacts.Count ());
			}

			using (var dataContext = new DataContext(Database.DbInfrastructure))
			{
				var alfred = dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1)));

				var emptyContact1 = dataContext.CreateEntity<UriContactEntity> ();
				var emptyContact2 = dataContext.CreateEntity<UriContactEntity> ();

				alfred.Contacts.Add (emptyContact1);

				dataContext.RegisterEmptyEntity (emptyContact1);
				dataContext.RegisterEmptyEntity (emptyContact2);

				dataContext.SaveChanges ();

				dataContext.UnregisterEmptyEntity (emptyContact1);
				dataContext.UnregisterEmptyEntity (emptyContact2);

				dataContext.SaveChanges ();
			}

			using (var dataContext = new DataContext(Database.DbInfrastructure))
			{
				var alfred = dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1)));
				var contacts = dataContext.GetByExample (new AbstractContactEntity ());

				// TODO This behavior is some kind of a bug. Alfred was not saved because from its point
				// of view, it has not changed. I'm not sure if a solution can be made for that bug.
				// Marc
				Assert.AreEqual (2, alfred.Contacts.Count);

				Assert.AreEqual (6, contacts.Count ());
			}

			this.CreateDatabaseHelper ();
		}


		[TestMethod]
		public void SaveWithoutChanges1()
		{
			TestHelper.PrintStartTest ("Save without changes 1");

			using (DataContext dataContext = new DataContext(Database.DbInfrastructure))
			{
				Database2.DbInfrastructure.GetSourceReferences (new Common.Support.Druid ());
				dataContext.SaveChanges ();
			}
		}


		[TestMethod]
		public void SaveWithoutChanges2()
		{
			TestHelper.PrintStartTest ("Save without changes 2");

			using (DataContext dataContext = new DataContext(Database.DbInfrastructure))
			{
				UriContactEntity[] contacts = {
					dataContext.ResolveEntity<UriContactEntity> (new DbKey (new DbId (1))),
					dataContext.ResolveEntity<UriContactEntity> (new DbKey (new DbId (2))),
					dataContext.ResolveEntity<UriContactEntity> (new DbKey (new DbId (3))),
					dataContext.ResolveEntity<UriContactEntity> (new DbKey (new DbId (4))),
				};

				Assert.AreEqual (4, contacts.Length);
				
				Assert.IsTrue (contacts.Any (c => Database2.CheckUriContact (c, "alfred@coucou.com", "Alfred")));
				Assert.IsTrue (contacts.Any (c => Database2.CheckUriContact (c, "alfred@blabla.com", "Alfred")));
				Assert.IsTrue (contacts.Any (c => Database2.CheckUriContact (c, "gertrude@coucou.com", "Gertrude")));
				Assert.IsTrue (contacts.Any (c => Database2.CheckUriContact (c, "nobody@nowhere.com", null)));

				dataContext.SaveChanges ();
			}
		}


		[TestMethod]
		public void ResolveEntity()
		{
			TestHelper.PrintStartTest ("Resolve entity");

			using (DataContext dataContext = new DataContext(Database.DbInfrastructure))
			{
				NaturalPersonEntity alfred = dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1)));
				NaturalPersonEntity gertrude = dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (2)));
				NaturalPersonEntity hans = dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (3)));

				Assert.IsTrue (Database2.CheckAlfred (alfred));
				Assert.IsTrue (Database2.CheckGertrude (gertrude));
				Assert.IsTrue (Database2.CheckHans (hans));
			}
		}


		[TestMethod]
		public void InsertEntity()
		{
			TestHelper.PrintStartTest ("Insert entity");

			using (DataContext dataContext = new DataContext(Database.DbInfrastructure))
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

			using (DataContext dataContext = new DataContext (Database.DbInfrastructure))
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

			this.CreateDatabaseHelper ();
		}


		[TestMethod]
		public void InsertEntityValue()
		{
			TestHelper.PrintStartTest ("Insert entity value");

			using (DataContext dataContext = new DataContext (Database.DbInfrastructure))
			{
				NaturalPersonEntity albert = dataContext.CreateEntity<NaturalPersonEntity> ();

				albert.Firstname = "Albert";
				albert.Lastname = "Levert";

				dataContext.SaveChanges ();
			}

			using (DataContext dataContext = new DataContext (Database.DbInfrastructure))
			{
				NaturalPersonEntity albert = dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (4)));

				albert.BirthDate = new Common.Types.Date (1954, 12, 31);

				dataContext.SaveChanges ();
			}

			using (DataContext dataContext = new DataContext (Database.DbInfrastructure))
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

			this.CreateDatabaseHelper ();
		}


		[TestMethod]
		public void UpdateEntityValue()
		{
			TestHelper.PrintStartTest ("Insert entity reference");

			using (DataContext dataContext = new DataContext (Database.DbInfrastructure))
			{
				NaturalPersonEntity albert = dataContext.CreateEntity<NaturalPersonEntity> ();

				albert.Firstname = "Albert";
				albert.Lastname = "Levert";

				dataContext.SaveChanges ();
			}

			using (DataContext dataContext = new DataContext (Database.DbInfrastructure))
			{
				NaturalPersonEntity albert = dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (4)));

				albert.Lastname = "Lebleu";

				dataContext.SaveChanges ();
			}

			using (DataContext dataContext = new DataContext (Database.DbInfrastructure))
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

			this.CreateDatabaseHelper ();
		}


		[TestMethod]
		public void DeleteEntityValue()
		{
			TestHelper.PrintStartTest ("Delete entity value");

			using (DataContext dataContext = new DataContext (Database.DbInfrastructure))
			{
				NaturalPersonEntity albert = dataContext.CreateEntity<NaturalPersonEntity> ();

				albert.Firstname = "Albert";
				albert.Lastname = "Levert";
				albert.BirthDate = new Common.Types.Date (1954, 12, 31);

				dataContext.SaveChanges ();
			}

			using (DataContext dataContext = new DataContext (Database.DbInfrastructure))
			{
				NaturalPersonEntity albert = dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (4)));

				albert.BirthDate = null;

				dataContext.SaveChanges ();
			}

			using (DataContext dataContext = new DataContext (Database.DbInfrastructure))
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

			this.CreateDatabaseHelper ();
		}


		[TestMethod]
		public void InsertEntityReference()
		{
			TestHelper.PrintStartTest ("Insert entity reference");

			using (DataContext dataContext = new DataContext (Database.DbInfrastructure))
			{
				NaturalPersonEntity albert = dataContext.CreateEntity<NaturalPersonEntity> ();

				albert.Firstname = "Albert";
				albert.Lastname = "Levert";

				dataContext.SaveChanges ();
			}

			using (DataContext dataContext = new DataContext (Database.DbInfrastructure))
			{
				NaturalPersonEntity albert = dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (4)));
				PersonGenderEntity gender = dataContext.ResolveEntity<PersonGenderEntity> (new DbKey (new DbId (1)));

				albert.Gender = gender;

				dataContext.SaveChanges ();
			}

			using (DataContext dataContext = new DataContext (Database.DbInfrastructure))
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

			this.CreateDatabaseHelper ();
		}


		[TestMethod]
		public void UpdateEntityReference()
		{
			TestHelper.PrintStartTest ("Update entity reference");

			using (DataContext dataContext = new DataContext (Database.DbInfrastructure))
			{
				NaturalPersonEntity albert = dataContext.CreateEntity<NaturalPersonEntity> ();
				PersonGenderEntity gender = dataContext.ResolveEntity<PersonGenderEntity> (new DbKey (new DbId (1)));

				albert.Firstname = "Albert";
				albert.Lastname = "Levert";
				albert.Gender = gender;

				dataContext.SaveChanges ();
			}

			using (DataContext dataContext = new DataContext (Database.DbInfrastructure))
			{
				NaturalPersonEntity albert = dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (4)));
				PersonGenderEntity gender = dataContext.ResolveEntity<PersonGenderEntity> (new DbKey (new DbId (2)));

				albert.Gender = gender;

				dataContext.SaveChanges ();
			}

			using (DataContext dataContext = new DataContext (Database.DbInfrastructure))
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

			this.CreateDatabaseHelper ();
		}


		[TestMethod]
		public void RemoveEntityReference()
		{
			TestHelper.PrintStartTest ("Remove entity reference");



			using (DataContext dataContext = new DataContext (Database.DbInfrastructure))
			{
				NaturalPersonEntity albert = dataContext.CreateEntity<NaturalPersonEntity> ();
				PersonGenderEntity gender = dataContext.ResolveEntity<PersonGenderEntity> (new DbKey (new DbId (1)));

				albert.Firstname = "Albert";
				albert.Lastname = "Levert";
				albert.Gender = gender;

				dataContext.SaveChanges ();
			}

			using (DataContext dataContext = new DataContext (Database.DbInfrastructure))
			{
				NaturalPersonEntity albert = dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (4)));
				
				albert.Gender = null;

				dataContext.SaveChanges ();
			}

			using (DataContext dataContext = new DataContext (Database.DbInfrastructure))
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

			this.CreateDatabaseHelper ();
		}


		[TestMethod]
		public void CreateEntityCollection()
		{
			TestHelper.PrintStartTest ("Create entity collection");
			
			using (DataContext dataContext = new DataContext (Database.DbInfrastructure))
			{
				NaturalPersonEntity albert = dataContext.CreateEntity<NaturalPersonEntity> ();

				albert.Firstname = "Albert";
				albert.Lastname = "Levert";

				dataContext.SaveChanges ();
			}

			using (DataContext dataContext = new DataContext (Database.DbInfrastructure))
			{
				NaturalPersonEntity albert = dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (4)));
				AbstractContactEntity contact = dataContext.ResolveEntity<AbstractContactEntity> (new DbKey (new DbId (4)));

				albert.Contacts.Add (contact);

				dataContext.SaveChanges ();
			}

			using (DataContext dataContext = new DataContext (Database.DbInfrastructure))
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

			this.CreateDatabaseHelper ();
		}


		[TestMethod]
		public void AddEntityCollection()
		{
			TestHelper.PrintStartTest ("Add entity collection");

			using (DataContext dataContext = new DataContext (Database.DbInfrastructure))
			{
				NaturalPersonEntity albert = dataContext.CreateEntity<NaturalPersonEntity> ();
				AbstractContactEntity contact = dataContext.ResolveEntity<AbstractContactEntity> (new DbKey (new DbId (4)));

				albert.Firstname = "Albert";
				albert.Lastname = "Levert";
				albert.Contacts.Add (contact);

				dataContext.SaveChanges ();
			}

			using (DataContext dataContext = new DataContext (Database.DbInfrastructure))
			{
				NaturalPersonEntity albert = dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (4)));

				AbstractContactEntity contact = dataContext.ResolveEntity<AbstractContactEntity> (new DbKey (new DbId (3)));

				albert.Contacts.Add(contact);

				dataContext.SaveChanges ();
			}

			using (DataContext dataContext = new DataContext (Database.DbInfrastructure))
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

			this.CreateDatabaseHelper ();
		}


		[TestMethod]
		public void UpdateEntityCollection()
		{
			TestHelper.PrintStartTest ("Update entity collection");

			using (DataContext dataContext = new DataContext (Database.DbInfrastructure))
			{
				NaturalPersonEntity albert = dataContext.CreateEntity<NaturalPersonEntity> ();
				AbstractContactEntity contact = dataContext.ResolveEntity<AbstractContactEntity> (new DbKey (new DbId (4)));

				albert.Firstname = "Albert";
				albert.Lastname = "Levert";
				albert.Contacts.Add (contact);

				dataContext.SaveChanges ();
			}

			using (DataContext dataContext = new DataContext (Database.DbInfrastructure))
			{
				NaturalPersonEntity albert = dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (4)));

				AbstractContactEntity contact = dataContext.ResolveEntity<AbstractContactEntity> (new DbKey (new DbId (3)));

				albert.Contacts[0] = contact;

				dataContext.SaveChanges ();
			}

			using (DataContext dataContext = new DataContext (Database.DbInfrastructure))
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

			this.CreateDatabaseHelper ();
		}


		[TestMethod]
		public void ReorderEntityCollection()
		{
			using (DataContext dataContext = new DataContext (Database.DbInfrastructure))
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

			using (DataContext dataContext = new DataContext (Database.DbInfrastructure))
			{
				NaturalPersonEntity albert = dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (4)));

				AbstractContactEntity contact1 = dataContext.ResolveEntity<AbstractContactEntity> (new DbKey (new DbId (4)));
				AbstractContactEntity contact2 = dataContext.ResolveEntity<AbstractContactEntity> (new DbKey (new DbId (3)));

				albert.Contacts[0] = contact2;
				albert.Contacts[1] = contact1;

				dataContext.SaveChanges ();
			}

			using (DataContext dataContext = new DataContext (Database.DbInfrastructure))
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

			this.CreateDatabaseHelper ();
		}


		[TestMethod]
		public void RemoveEntityCollection()
		{
			using (DataContext dataContext = new DataContext (Database.DbInfrastructure))
			{
				NaturalPersonEntity albert = dataContext.CreateEntity<NaturalPersonEntity> ();
				AbstractContactEntity contact = dataContext.ResolveEntity<AbstractContactEntity> (new DbKey (new DbId (4)));

				albert.Firstname = "Albert";
				albert.Lastname = "Levert";
				albert.Contacts.Add (contact);

				dataContext.SaveChanges ();
			}

			using (DataContext dataContext = new DataContext (Database.DbInfrastructure))
			{
				NaturalPersonEntity albert = dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (4)));

				albert.Contacts.RemoveAt (0);

				dataContext.SaveChanges ();
			}

			using (DataContext dataContext = new DataContext (Database.DbInfrastructure))
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

			this.CreateDatabaseHelper ();
		}


		[TestMethod]
		public void DeleteEntityPresentInCollectionMemory()
		{
			TestHelper.PrintStartTest ("Delete entity present in collection (memory)");

			using (DataContext dataContext = new DataContext(Database.DbInfrastructure))
			{
				NaturalPersonEntity alfred = dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1)));

				Assert.IsTrue (Database2.CheckAlfred (alfred));

				dataContext.DeleteEntity (alfred.Contacts[0]);

				dataContext.SaveChanges ();

				Assert.IsTrue (alfred.Contacts.Count == 1);
				Assert.IsTrue (alfred.Contacts.Any (c => Database2.CheckUriContact (c as UriContactEntity, "alfred@blabla.com", "Alfred")));
			}

			using (DataContext dataContext = new DataContext(Database.DbInfrastructure))
			{
				NaturalPersonEntity alfred = dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1)));

				Assert.IsTrue (alfred.Contacts.Count == 1);
				Assert.IsTrue (alfred.Contacts.Any (c => Database2.CheckUriContact (c as UriContactEntity, "alfred@blabla.com", "Alfred")));
			}

			this.CreateDatabaseHelper ();
		}


		[TestMethod]
		public void DeleteEntityPresentInReferenceMemory()
		{
			TestHelper.PrintStartTest ("Delete entity present in reference (memory)");

			using (DataContext dataContext = new DataContext(Database.DbInfrastructure))
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

			using (DataContext dataContext = new DataContext(Database.DbInfrastructure))
			{
				UriContactEntity[] contacts = {
					dataContext.ResolveEntity<UriContactEntity> (new DbKey (new DbId (1))),
					dataContext.ResolveEntity<UriContactEntity> (new DbKey (new DbId (2))),
				};

				Assert.IsTrue (contacts.All (c => c.NaturalPerson == null));
			}

			this.CreateDatabaseHelper ();
		}


		[TestMethod]
		public void DeleteEntityPresentInCollectionDatabase()
		{
			TestHelper.PrintStartTest ("Delete entity present in collection (database)");

			using (DataContext dataContext = new DataContext(Database.DbInfrastructure))
			{
				UriContactEntity contact = dataContext.ResolveEntity<UriContactEntity> (new DbKey (new DbId (1)));
				dataContext.DeleteEntity (contact);

				dataContext.SaveChanges ();

				NaturalPersonEntity alfred = dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1)));

				Assert.IsTrue (alfred.Contacts.Count == 1);
				Assert.IsTrue (alfred.Contacts.Any (c => Database2.CheckUriContact (c as UriContactEntity, "alfred@blabla.com", "Alfred")));
			}

			using (DataContext dataContext = new DataContext(Database.DbInfrastructure))
			{
				NaturalPersonEntity alfred = dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1)));

				Assert.IsTrue (alfred.Contacts.Count == 1);
				Assert.IsTrue (alfred.Contacts.Any (c => Database2.CheckUriContact (c as UriContactEntity, "alfred@blabla.com", "Alfred")));
			}

			this.CreateDatabaseHelper ();
		}


		[TestMethod]
		public void DeleteEntityPresentInReferenceDatabase()
		{
			TestHelper.PrintStartTest ("Delete entity present in reference (database)");

			using (DataContext dataContext = new DataContext(Database.DbInfrastructure))
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

			using (DataContext dataContext = new DataContext(Database.DbInfrastructure))
			{
				UriContactEntity[] contacts = {
					dataContext.ResolveEntity<UriContactEntity> (new DbKey (new DbId (1))),
					dataContext.ResolveEntity<UriContactEntity> (new DbKey (new DbId (2))),
				};

				Assert.IsTrue (contacts.All (c => c.NaturalPerson == null));
			}

			this.CreateDatabaseHelper ();
		}


	}


}
