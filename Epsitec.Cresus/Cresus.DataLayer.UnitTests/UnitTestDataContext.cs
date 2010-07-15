//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD


using Epsitec.Cresus.Database;

using Epsitec.Cresus.Core;
using Epsitec.Cresus.Core.Entities;

using Epsitec.Cresus.DataLayer.Context;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.Linq;

namespace Epsitec.Cresus.DataLayer
{
	
	
	// TODO Create a test which updates entities for the values, reference and collections.
	// Marc


	[TestClass]
	public class UnitTestDataContext
	{


		[ClassInitialize]
		public void Initialize(TestContext testContext)
		{
			TestHelper.Initialize ();
		}

		
		[TestMethod]
		public void CreateDatabase()
		{
			TestHelper.PrintStartTest ("Create database");

			this.CreateDatabaseHelper ();
		}


		private void CreateDatabaseHelper()
		{
			Database.CreateAndConnectToDatabase ();

			Assert.IsTrue (Database.DbInfrastructure.IsConnectionOpen);

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

				Assert.IsTrue (alfred.Contacts.Count == 2);
				Assert.IsTrue (contacts.Count () == 4);
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

				Assert.IsTrue (alfred.Contacts.Count == 2);
				Assert.IsTrue (contacts.Count () == 6);
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

				Assert.IsTrue (contacts.Length == 4);

				Assert.IsTrue (contacts.Any (c => Database2.CheckUriContact (c, "alfred@coucou.com", "Alfred")));
				Assert.IsTrue (contacts.Any (c => Database2.CheckUriContact (c, "alfred@blabla.com", "Alfred")));
				Assert.IsTrue (contacts.Any (c => Database2.CheckUriContact (c, "gertrude@coucou.com", "Gertrude")));
				Assert.IsTrue (contacts.Any (c => Database2.CheckUriContact (c, "nobody@nowhere.com", null)));

				dataContext.SaveChanges ();
			}
		}


		[TestMethod]
		public void Resolve()
		{
			TestHelper.PrintStartTest ("Resolve");

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
		public void GetFreshObject()
		{
			TestHelper.PrintStartTest ("Get fresh object");

			using (DataContext dataContext = new DataContext(Database.DbInfrastructure))
			{
				NaturalPersonEntity freshPerson1 = dataContext.CreateEntity<NaturalPersonEntity> ();

				freshPerson1.Firstname = "Albert";
				freshPerson1.Lastname = "Levert";

				dataContext.SaveChanges ();

				NaturalPersonEntity freshPerson2 = dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (4)));

				Assert.IsTrue (freshPerson1 == freshPerson2);

				dataContext.SaveChanges ();
			}

			this.CreateDatabaseHelper ();
		}


		[TestMethod]
		public void DeleteRelationReference()
		{
			TestHelper.PrintStartTest ("Delete Relation Reference");

			using (DataContext dataContext = new DataContext(Database.DbInfrastructure))
			{
				NaturalPersonEntity alfred = dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1)));

				Assert.IsTrue (alfred.Gender != null);

				alfred.Gender = null;

				Assert.IsTrue (alfred.Gender == null);

				dataContext.SaveChanges ();
			}

			using (DataContext dataContext = new DataContext(Database.DbInfrastructure))
			{
				NaturalPersonEntity alfred = dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1)));

				Assert.IsTrue (alfred.Gender == null);
			}

			this.CreateDatabaseHelper ();
		}


		[TestMethod]
		public void DeleteRelationCollection()
		{
			TestHelper.PrintStartTest ("Delete Relation Collection");

			using (DataContext dataContext = new DataContext(Database.DbInfrastructure))
			{
				NaturalPersonEntity alfred = dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1)));

				Assert.IsTrue (Database2.CheckAlfred (alfred));

				alfred.Contacts.RemoveAt (0);

				Assert.IsTrue (alfred.Contacts.Count == 1);
				Assert.IsTrue (alfred.Contacts.Any (c => Database2.CheckUriContact (c as UriContactEntity, "alfred@blabla.com", "Alfred")));

				dataContext.SaveChanges ();
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
		public void DeleteEntityCollectionTargetInMemory()
		{
			TestHelper.PrintStartTest ("Delete Entity Collection Target In Memory");

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
		public void DeleteEntityReferenceTargetInMemory()
		{
			TestHelper.PrintStartTest ("Delete Entity Reference Target In Memory");

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
		public void DeleteEntityCollectionTargetInDatabase()
		{
			TestHelper.PrintStartTest ("Delete Entity Collection Target In Database");

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
		public void DeleteEntityReferenceTargetInDatabase()
		{
			TestHelper.PrintStartTest ("Delete Entity Reference Target In Database");

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
