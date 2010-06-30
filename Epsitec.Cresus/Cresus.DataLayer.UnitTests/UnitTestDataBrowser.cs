using Epsitec.Common.Support.EntityEngine;

using Epsitec.Cresus.Core;
using Epsitec.Cresus.Core.Entities;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.Linq;


namespace Epsitec.Cresus.DataLayer
{


	[TestClass]
	public class UnitTestDataBrowser
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

			this.CreateDatabase (false);
			this.CreateDatabase (true);
		}


		public void CreateDatabase(bool bulkMode)
		{
			Database.CreateAndConnectToDatabase ();

			Assert.IsTrue (Database.DbInfrastructure.IsConnectionOpen);

			Database2.PupulateDatabase (bulkMode);
		}


		[TestMethod]
		public void GetObjects()
		{
			TestHelper.PrintStartTest ("Get objects");

			this.GetObjects (false);
			this.GetObjects (true);
		}


		public void GetObjects(bool bulkMode)
		{
			using (DataContext dataContext = new DataContext (Database.DbInfrastructure, bulkMode))
			{
				DataBrowser dataBrowser = new DataBrowser (dataContext);

				NaturalPersonEntity[] persons = dataBrowser.GetByExample<NaturalPersonEntity> (new NaturalPersonEntity ()).ToArray ();

				Assert.IsTrue (persons.Length == 3);

				Assert.IsTrue (persons.Any (p => Database2.CheckAlfred (p)));
				Assert.IsTrue (persons.Any (p => Database2.CheckGertrude (p)));
				Assert.IsTrue (persons.Any (p => Database2.CheckHans (p)));

			}
		}


		[TestMethod]
		public void GetObjectsWithCast()
		{
			TestHelper.PrintStartTest ("Get objects with cast");

			this.GetObjectsWithCast (false);
			this.GetObjectsWithCast (true);
		}


		public void GetObjectsWithCast(bool bulkMode)
		{
			using (DataContext dataContext = new DataContext (Database.DbInfrastructure, bulkMode))
			{
				DataBrowser dataBrowser = new DataBrowser (dataContext);

				NaturalPersonEntity[] persons = dataBrowser.GetByExample<AbstractPersonEntity> (new AbstractPersonEntity ()).Cast<NaturalPersonEntity> ().ToArray ();

				Assert.IsTrue (persons.Length == 3);

				Assert.IsTrue (persons.Any (p => Database2.CheckAlfred (p)));
				Assert.IsTrue (persons.Any (p => Database2.CheckGertrude (p)));
				Assert.IsTrue (persons.Any (p => Database2.CheckHans (p)));
			}
		}


		[TestMethod]
		public void GetObjectsWithCorrectValueExample()
		{
			TestHelper.PrintStartTest ("Get objects with correct value example");

			this.GetObjectsWithCorrectValueExample (false);
			this.GetObjectsWithCorrectValueExample (true);
		}


		public void GetObjectsWithCorrectValueExample(bool bulkMode)
		{
			using (DataContext dataContext = new DataContext (Database.DbInfrastructure, bulkMode))
			{
				DataBrowser dataBrowser = new DataBrowser (dataContext);

				NaturalPersonEntity example = Database2.GetCorrectExample1 ();

				NaturalPersonEntity[] persons = dataBrowser.GetByExample<NaturalPersonEntity> (example).ToArray ();

				Assert.IsTrue (persons.Count () == 1);

				Assert.IsTrue (persons.Any (p => Database2.CheckAlfred (p)));
			}
		}


		[TestMethod]
		public void GetObjectsWithCorrectReferenceExample()
		{
			TestHelper.PrintStartTest ("Get objects with correct reference example");

			this.GetObjectsWithCorrectReferenceExample (false);
			this.GetObjectsWithCorrectReferenceExample (true);
		}


		public void GetObjectsWithCorrectReferenceExample(bool bulkMode)
		{
			using (DataContext dataContext = new DataContext (Database.DbInfrastructure, bulkMode))
			{
				DataBrowser dataBrowser = new DataBrowser (dataContext);

				NaturalPersonEntity example = Database2.GetCorrectExample2 ();

				NaturalPersonEntity[] persons = dataBrowser.GetByExample<NaturalPersonEntity> (example).ToArray ();

				Assert.IsTrue (persons.Count () == 1);

				Assert.IsTrue (persons.Any (p => Database2.CheckAlfred (p)));
			}
		}


		[TestMethod]
		public void GetObjectsWithCorrectCollectionExample1()
		{
			TestHelper.PrintStartTest ("Get objects with correct collection example 1");

			this.GetObjectsWithCorrectCollectionExample1 (false);
			this.GetObjectsWithCorrectCollectionExample1 (true);
		}


		public void GetObjectsWithCorrectCollectionExample1(bool bulkMode)
		{
			using (DataContext dataContext = new DataContext (Database.DbInfrastructure, bulkMode))
			{
				DataBrowser dataBrowser = new DataBrowser (dataContext);

				NaturalPersonEntity example = Database2.GetCorrectExample3 ();

				NaturalPersonEntity[] persons = dataBrowser.GetByExample<NaturalPersonEntity> (example).ToArray ();

				Assert.IsTrue (persons.Length == 2);

				Assert.IsTrue (persons.Any (p => Database2.CheckAlfred (p)));
				Assert.IsTrue (persons.Any (p => Database2.CheckGertrude (p)));
			}
		}


		[TestMethod]
		public void GetObjectsWithCorrectCollectionExample2()
		{
			TestHelper.PrintStartTest ("Get objects with correct collection example 2");


			this.GetObjectsWithCorrectCollectionExample2 (false);
			this.GetObjectsWithCorrectCollectionExample2 (true);
		}


		public void GetObjectsWithCorrectCollectionExample2(bool bulkMode)
		{
			using (DataContext dataContext = new DataContext (Database.DbInfrastructure, bulkMode))
			{
				DataBrowser dataBrowser = new DataBrowser (dataContext);

				NaturalPersonEntity example = Database2.GetCorrectExample4 ();

				NaturalPersonEntity[] persons = dataBrowser.GetByExample<NaturalPersonEntity> (example).ToArray ();

				Assert.IsTrue (persons.Count () == 1);

				Assert.IsTrue (persons.Any (p => Database2.CheckAlfred (p)));
			}
		}


		[TestMethod]
		public void GetObjectsWithIncorrectValueExample()
		{
			TestHelper.PrintStartTest ("Get objects with incorrect value example");

			this.GetObjectsWithIncorrectValueExample (false);
			this.GetObjectsWithIncorrectValueExample (true);
		}


		public void GetObjectsWithIncorrectValueExample(bool bulkMode)
		{
			using (DataContext dataContext = new DataContext (Database.DbInfrastructure, bulkMode))
			{
				DataBrowser dataBrowser = new DataBrowser (dataContext);

				NaturalPersonEntity example = Database2.GetIncorrectExample1 ();

				NaturalPersonEntity[] persons = dataBrowser.GetByExample<NaturalPersonEntity> (example).ToArray ();

				Assert.IsTrue (persons.Count () == 0);
			}
		}


		[TestMethod]
		public void GetObjectsWithIncorrectReferenceExample()
		{
			TestHelper.PrintStartTest ("Get objects with incorrect reference example");

			this.GetObjectsWithIncorrectReferenceExample (false);
			this.GetObjectsWithIncorrectReferenceExample (true);
		}


		public void GetObjectsWithIncorrectReferenceExample(bool bulkMode)
		{
			using (DataContext dataContext = new DataContext (Database.DbInfrastructure, bulkMode))
			{
				DataBrowser dataBrowser = new DataBrowser (dataContext);

				NaturalPersonEntity example = Database2.GetIncorrectExample2 ();

				NaturalPersonEntity[] persons = dataBrowser.GetByExample<NaturalPersonEntity> (example).ToArray ();

				Assert.IsTrue (persons.Count () == 0);
			}
		}


		[TestMethod]
		public void GetObjectsWithIncorrectCollectionExample1()
		{
			TestHelper.PrintStartTest ("Get objects with incorrect collection example 1");

			this.GetObjectsWithIncorrectCollectionExample1 (false);
			this.GetObjectsWithIncorrectCollectionExample1 (true);
		}

		public void GetObjectsWithIncorrectCollectionExample1(bool bulkMode)
		{
			using (DataContext dataContext = new DataContext (Database.DbInfrastructure, bulkMode))
			{
				DataBrowser dataBrowser = new DataBrowser (dataContext);

				NaturalPersonEntity example = Database2.GetIncorrectExample3 ();

				NaturalPersonEntity[] persons = dataBrowser.GetByExample<NaturalPersonEntity> (example).ToArray ();

				Assert.IsTrue (persons.Count () == 0);
			}
		}


		[TestMethod]
		public void GetObjectsWithIncorrectCollectionExample2()
		{
			TestHelper.PrintStartTest ("Get objects with incorrect collection example 2");

			this.GetObjectsWithIncorrectCollectionExample2 (false);
			this.GetObjectsWithIncorrectCollectionExample2 (true);
		}


		public void GetObjectsWithIncorrectCollectionExample2(bool bulkMode)
		{
			using (DataContext dataContext = new DataContext (Database.DbInfrastructure, bulkMode))
			{
				DataBrowser dataBrowser = new DataBrowser (dataContext);

				NaturalPersonEntity example = Database2.GetIncorrectExample4 ();

				NaturalPersonEntity[] persons = dataBrowser.GetByExample<NaturalPersonEntity> (example).ToArray ();

				Assert.IsTrue (persons.Count () == 0);
			}
		}


		[TestMethod]
		public void GetObjectsWithEntityEquality()
		{
			TestHelper.PrintStartTest ("Get objects with entity equality");

			this.GetObjectsWithEntityEquality (false);
			this.GetObjectsWithEntityEquality (true);
		}


		public void GetObjectsWithEntityEquality(bool bulkMode)
		{
			using (DataContext dataContext = new DataContext (Database.DbInfrastructure, bulkMode))
			{
				DataBrowser dataBrowser = new DataBrowser (dataContext);

				NaturalPersonEntity example = Database2.GetCorrectExample1 ();

				NaturalPersonEntity[] persons1 = dataBrowser.GetByExample<NaturalPersonEntity> (example).ToArray ();
				NaturalPersonEntity[] persons2 = dataBrowser.GetByExample<NaturalPersonEntity> (example).ToArray ();

				Assert.IsTrue (persons1.Count () == 1);
				Assert.IsTrue (persons2.Count () == 1);

				Assert.IsTrue (persons1.Any (p => Database2.CheckAlfred (p)));
				Assert.IsTrue (persons2.Any (p => Database2.CheckAlfred (p)));

				Assert.IsTrue (object.ReferenceEquals (persons1[0], persons2[0]));
			}
		}


		[TestMethod]
		public void GetFreshObject()
		{
			TestHelper.PrintStartTest ("Get fresh object");

			this.GetFreshObject (false);
			this.GetFreshObject (true);
		}


		public void GetFreshObject(bool bulkMode)
		{
			using (DataContext dataContext = new DataContext (Database.DbInfrastructure, bulkMode))
			{
				NaturalPersonEntity freshPerson1 = dataContext.CreateEmptyEntity<NaturalPersonEntity> ();

				freshPerson1.Firstname = "Albert";
				freshPerson1.Lastname = "Levert";

				dataContext.SaveChanges ();

				DataBrowser dataBrowser = new DataBrowser (dataContext);

				NaturalPersonEntity example = new NaturalPersonEntity ()
				{
					Firstname = "Albert",
					Lastname = "Levert",
				};

				NaturalPersonEntity freshPerson2 = dataBrowser.GetByExample<NaturalPersonEntity> (example).First ();

				Assert.IsTrue (freshPerson1 == freshPerson2);

				dataContext.SaveChanges ();
			}

			this.CreateDatabase (false);
		}


		[TestMethod]
		public void GetObjectByReferenceReference()
		{
			TestHelper.PrintStartTest ("Get objects by reference reference");

			this.GetObjectByReferenceReference (false);
			this.GetObjectByReferenceReference (true);
		}


		public void GetObjectByReferenceReference(bool bulkMode)
		{
			using (DataContext dataContext = new DataContext (Database.DbInfrastructure, bulkMode))
			{
				DataBrowser dataBrowser = new DataBrowser (dataContext);

				NaturalPersonEntity alfredExample = Database2.GetCorrectExample1 ();
				NaturalPersonEntity alfred = dataBrowser.GetByExample<NaturalPersonEntity> (alfredExample).FirstOrDefault ();

				UriContactEntity contactExample = new UriContactEntity ()
				{
					NaturalPerson = alfred,
				};

				UriContactEntity[] contacts = dataBrowser.GetByExample<UriContactEntity> (contactExample).ToArray ();

				Assert.IsTrue (contacts.Length == 2);

				Assert.IsTrue (contacts.Any (c => Database2.CheckUriContact (c, "alfred@coucou.com", "Alfred")));
				Assert.IsTrue (contacts.Any (c => Database2.CheckUriContact (c, "alfred@blabla.com", "Alfred")));

				Assert.IsTrue (contacts.All (c => c.NaturalPerson == alfred));
			}
		}


		[TestMethod]
		public void GetObjectByCollectionReference()
		{
			TestHelper.PrintStartTest ("Get objects by collection reference");

			this.GetObjectByCollectionReference (false);
			this.GetObjectByCollectionReference (true);
		}


		public void GetObjectByCollectionReference(bool bulkMode)
		{
			using (DataContext dataContext = new DataContext (Database.DbInfrastructure, bulkMode))
			{
				DataBrowser dataBrowser = new DataBrowser (dataContext);

				UriContactEntity contactExample = new UriContactEntity ()
				{
					Uri = "alfred@coucou.com",
				};

				UriContactEntity contact = dataBrowser.GetByExample (contactExample).FirstOrDefault ();

				NaturalPersonEntity alfredExample = new NaturalPersonEntity ();
				alfredExample.Contacts.Add (contact);

				NaturalPersonEntity alfred = dataBrowser.GetByExample (alfredExample).FirstOrDefault ();

				Assert.IsTrue (Database2.CheckAlfred (alfred));
			}
		}


		[TestMethod]
		public void GetReferencersReference()
		{
			TestHelper.PrintStartTest ("Get referencers reference");

			this.GetReferencersReference (false);
			this.GetReferencersReference (true);
		}


		public void GetReferencersReference(bool bulkMode)
		{
			using (DataContext dataContext = new DataContext (Database.DbInfrastructure, bulkMode))
			{
				DataBrowser dataBrowser = new DataBrowser (dataContext);

				NaturalPersonEntity alfredExample = Database2.GetCorrectExample1 ();
				NaturalPersonEntity alfred = dataBrowser.GetByExample<NaturalPersonEntity> (alfredExample).FirstOrDefault ();

				AbstractEntity[] referencers = dataBrowser.GetReferencers (alfred).Select (r => r.Item1).ToArray ();

				Assert.IsTrue (referencers.Length == 2);

				Assert.IsTrue (referencers.OfType<UriContactEntity> ().Any (c => Database2.CheckUriContact (c, "alfred@coucou.com", "Alfred")));
				Assert.IsTrue (referencers.OfType<UriContactEntity> ().Any (c => Database2.CheckUriContact (c, "alfred@blabla.com", "Alfred")));
			}
		}


		[TestMethod]
		public void GetReferencersCollection()
		{
			TestHelper.PrintStartTest ("Get referencers collection");

			this.GetReferencersCollection (false);
			this.GetReferencersCollection (true);
		}


		public void GetReferencersCollection(bool bulkMode)
		{
			using (DataContext dataContext = new DataContext (Database.DbInfrastructure, bulkMode))
			{
				DataBrowser dataBrowser = new DataBrowser (dataContext);

				UriContactEntity contactExample = new UriContactEntity ()
				{
					Uri = "alfred@coucou.com",
				};

				UriContactEntity contact = dataBrowser.GetByExample (contactExample).FirstOrDefault ();

				AbstractEntity[] referencers = dataBrowser.GetReferencers (contact).Select (r => r.Item1).ToArray ();

				Assert.IsTrue (referencers.Length == 1);
				Assert.IsTrue (referencers.OfType<NaturalPersonEntity> ().Any (p => Database2.CheckAlfred (p)));
			}
		}
		

		[TestMethod]
		public void GetObjectsWithDeletedEntity()
		{
			TestHelper.PrintStartTest ("Get objects with deleted entity");

			this.GetObjectsWithDeletedEntity (false);
			this.GetObjectsWithDeletedEntity (true);
		}


		public void GetObjectsWithDeletedEntity(bool bulkMode)
		{
			using (DataContext dataContext = new DataContext (Database.DbInfrastructure, bulkMode))
			{
				DataBrowser dataBrowser = new DataBrowser (dataContext);

				NaturalPersonEntity example = Database2.GetCorrectExample1 ();

				NaturalPersonEntity[] persons = dataBrowser.GetByExample<NaturalPersonEntity> (example).ToArray ();

				Assert.IsTrue (persons.Count () == 1);
				Assert.IsTrue (persons.Any (p => Database2.CheckAlfred (p)));

				dataContext.DeleteEntity (persons[0]);

				dataContext.SaveChanges ();

				NaturalPersonEntity[] persons2 = dataBrowser.GetByExample<NaturalPersonEntity> (example).ToArray ();

				Assert.IsTrue (persons2.Count () == 0);
			}

			using (DataContext dataContext = new DataContext (Database.DbInfrastructure, bulkMode))
			{
				DataBrowser dataBrowser = new DataBrowser (dataContext);

				NaturalPersonEntity example = Database2.GetCorrectExample1 ();

				NaturalPersonEntity[] persons = dataBrowser.GetByExample<NaturalPersonEntity> (example).ToArray ();

				Assert.IsTrue (persons.Count () == 0);
			}

			this.CreateDatabase (bulkMode);
		}


		[TestMethod]
		public void GetObjectsWithDeletedRelation()
		{
			TestHelper.PrintStartTest ("Get objects with deleted relation");

			this.GetObjectsWithDeletedRelation (false);
			this.GetObjectsWithDeletedRelation (true);
		}


		public void GetObjectsWithDeletedRelation(bool bulkMode)
		{
			using (DataContext dataContext = new DataContext (Database.DbInfrastructure, bulkMode))
			{
				DataBrowser dataBrowser = new DataBrowser (dataContext);

				NaturalPersonEntity example = Database2.GetCorrectExample1 ();

				NaturalPersonEntity[] persons = dataBrowser.GetByExample<NaturalPersonEntity> (example).ToArray ();

				Assert.IsTrue (persons.Count () == 1);
				Assert.IsTrue (persons.Any (p => Database2.CheckAlfred (p)));

				persons[0].Contacts.RemoveAt (0);
				persons[0].Contacts.RemoveAt (0);

				persons[0].Gender = null;

				dataContext.SaveChanges ();

				NaturalPersonEntity[] persons2 = dataBrowser.GetByExample<NaturalPersonEntity> (example).ToArray ();

				Assert.IsTrue (persons2.Count () == 1);
				Assert.IsTrue (persons2[0].Contacts.Count == 0);
				Assert.IsTrue (persons2[0].Gender == null);
			}

			using (DataContext dataContext = new DataContext (Database.DbInfrastructure, bulkMode))
			{
				DataBrowser dataBrowser = new DataBrowser (dataContext);

				NaturalPersonEntity example = Database2.GetCorrectExample1 ();

				NaturalPersonEntity[] persons = dataBrowser.GetByExample<NaturalPersonEntity> (example).ToArray ();

				Assert.IsTrue (persons.Count () == 1);
				Assert.IsTrue (persons[0].Contacts.Count == 0);
				Assert.IsTrue (persons[0].Gender == null);
			}

			this.CreateDatabase (bulkMode);
		}


	}


}
