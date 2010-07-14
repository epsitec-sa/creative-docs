using Epsitec.Common.Support.EntityEngine;

using Epsitec.Cresus.Core;
using Epsitec.Cresus.Core.Entities;

using Epsitec.Cresus.DataLayer.Context;
using Epsitec.Cresus.DataLayer.Loader;

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

			this.CreateDatabaseHelper ();
		}

		private void CreateDatabaseHelper()
		{
			Database.CreateAndConnectToDatabase ();

			Assert.IsTrue (Database.DbInfrastructure.IsConnectionOpen);

			Database2.PupulateDatabase ();
		}


		[TestMethod]
		public void GetObjects()
		{
			TestHelper.PrintStartTest ("Get objects");

			using (DataContext dataContext = new DataContext(Database.DbInfrastructure))
			{
				NaturalPersonEntity[] persons = dataContext.GetByExample<NaturalPersonEntity> (new NaturalPersonEntity ()).ToArray ();

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

			using (DataContext dataContext = new DataContext(Database.DbInfrastructure))
			{
				NaturalPersonEntity[] persons = dataContext.GetByExample<AbstractPersonEntity> (new AbstractPersonEntity ()).Cast<NaturalPersonEntity> ().ToArray ();

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

			using (DataContext dataContext = new DataContext(Database.DbInfrastructure))
			{
				NaturalPersonEntity example = Database2.GetCorrectExample1 ();

				NaturalPersonEntity[] persons = dataContext.GetByExample<NaturalPersonEntity> (example).ToArray ();

				Assert.IsTrue (persons.Count () == 1);

				Assert.IsTrue (persons.Any (p => Database2.CheckAlfred (p)));
			}
		}


		[TestMethod]
		public void GetObjectsWithCorrectReferenceExample()
		{
			TestHelper.PrintStartTest ("Get objects with correct reference example");

			using (DataContext dataContext = new DataContext(Database.DbInfrastructure))
			{
				NaturalPersonEntity example = Database2.GetCorrectExample2 ();

				NaturalPersonEntity[] persons = dataContext.GetByExample<NaturalPersonEntity> (example).ToArray ();

				Assert.IsTrue (persons.Count () == 1);

				Assert.IsTrue (persons.Any (p => Database2.CheckAlfred (p)));
			}
		}


		[TestMethod]
		public void GetObjectsWithCorrectCollectionExample1()
		{
			TestHelper.PrintStartTest ("Get objects with correct collection example 1");

			using (DataContext dataContext = new DataContext(Database.DbInfrastructure))
			{
				NaturalPersonEntity example = Database2.GetCorrectExample3 ();

				NaturalPersonEntity[] persons = dataContext.GetByExample<NaturalPersonEntity> (example).ToArray ();

				Assert.IsTrue (persons.Length == 2);

				Assert.IsTrue (persons.Any (p => Database2.CheckAlfred (p)));
				Assert.IsTrue (persons.Any (p => Database2.CheckGertrude (p)));
			}
		}


		[TestMethod]
		public void GetObjectsWithCorrectCollectionExample2()
		{
			TestHelper.PrintStartTest ("Get objects with correct collection example 2");

			using (DataContext dataContext = new DataContext(Database.DbInfrastructure))
			{
				NaturalPersonEntity example = Database2.GetCorrectExample4 ();

				NaturalPersonEntity[] persons = dataContext.GetByExample<NaturalPersonEntity> (example).ToArray ();

				Assert.IsTrue (persons.Count () == 1);

				Assert.IsTrue (persons.Any (p => Database2.CheckAlfred (p)));
			}
		}


		[TestMethod]
		public void GetObjectsWithCorrectCollectionExample3()
		{
			TestHelper.PrintStartTest ("Get objects with correct collection example 3");

			using (DataContext dataContext = new DataContext(Database.DbInfrastructure))
			{
				NaturalPersonEntity example = Database2.GetCorrectExample5 ();

				NaturalPersonEntity[] persons = dataContext.GetByExample<NaturalPersonEntity> (example).ToArray ();

				Assert.IsTrue (persons.Count () == 1);

				Assert.IsTrue (persons.Any (p => Database2.CheckAlfred (p)));
			}
		}


		[TestMethod]
		public void GetObjectsWithIncorrectValueExample()
		{
			TestHelper.PrintStartTest ("Get objects with incorrect value example");

			using (DataContext dataContext = new DataContext(Database.DbInfrastructure))
			{
				NaturalPersonEntity example = Database2.GetIncorrectExample1 ();

				NaturalPersonEntity[] persons = dataContext.GetByExample<NaturalPersonEntity> (example).ToArray ();

				Assert.IsTrue (persons.Count () == 0);
			}
		}


		[TestMethod]
		public void GetObjectsWithIncorrectReferenceExample()
		{
			TestHelper.PrintStartTest ("Get objects with incorrect reference example");

			using (DataContext dataContext = new DataContext(Database.DbInfrastructure))
			{
				NaturalPersonEntity example = Database2.GetIncorrectExample2 ();

				NaturalPersonEntity[] persons = dataContext.GetByExample<NaturalPersonEntity> (example).ToArray ();

				Assert.IsTrue (persons.Count () == 0);
			}
		}


		[TestMethod]
		public void GetObjectsWithIncorrectCollectionExample1()
		{
			TestHelper.PrintStartTest ("Get objects with incorrect collection example 1");

			using (DataContext dataContext = new DataContext(Database.DbInfrastructure))
			{
				NaturalPersonEntity example = Database2.GetIncorrectExample3 ();

				NaturalPersonEntity[] persons = dataContext.GetByExample<NaturalPersonEntity> (example).ToArray ();

				Assert.IsTrue (persons.Count () == 0);
			}
		}


		[TestMethod]
		public void GetObjectsWithIncorrectCollectionExample2()
		{
			TestHelper.PrintStartTest ("Get objects with incorrect collection example 2");

			using (DataContext dataContext = new DataContext(Database.DbInfrastructure))
			{
				NaturalPersonEntity example = Database2.GetIncorrectExample4 ();

				NaturalPersonEntity[] persons = dataContext.GetByExample<NaturalPersonEntity> (example).ToArray ();

				Assert.IsTrue (persons.Count () == 0);
			}
		}


		[TestMethod]
		public void GetObjectsWithEntityEquality()
		{
			TestHelper.PrintStartTest ("Get objects with entity equality");

			using (DataContext dataContext = new DataContext(Database.DbInfrastructure))
			{
				NaturalPersonEntity example = Database2.GetCorrectExample1 ();

				NaturalPersonEntity[] persons1 = dataContext.GetByExample<NaturalPersonEntity> (example).ToArray ();
				NaturalPersonEntity[] persons2 = dataContext.GetByExample<NaturalPersonEntity> (example).ToArray ();

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

			using (DataContext dataContext = new DataContext(Database.DbInfrastructure))
			{
				NaturalPersonEntity freshPerson1 = dataContext.CreateEntity<NaturalPersonEntity> ();

				freshPerson1.Firstname = "Albert";
				freshPerson1.Lastname = "Levert";

				dataContext.SaveChanges ();

				NaturalPersonEntity example = new NaturalPersonEntity ()
				{
					Firstname = "Albert",
					Lastname = "Levert",
				};

				NaturalPersonEntity freshPerson2 = dataContext.GetByExample<NaturalPersonEntity> (example).First ();

				Assert.IsTrue (freshPerson1 == freshPerson2);

				dataContext.SaveChanges ();
			}

			this.CreateDatabaseHelper ();
		}


		[TestMethod]
		public void GetObjectByReferenceReference()
		{
			TestHelper.PrintStartTest ("Get objects by reference reference");

			using (DataContext dataContext = new DataContext(Database.DbInfrastructure))
			{
				NaturalPersonEntity alfredExample = Database2.GetCorrectExample1 ();
				NaturalPersonEntity alfred = dataContext.GetByExample<NaturalPersonEntity> (alfredExample).FirstOrDefault ();

				UriContactEntity contactExample = new UriContactEntity ()
				{
					NaturalPerson = alfred,
				};

				UriContactEntity[] contacts = dataContext.GetByExample<UriContactEntity> (contactExample).ToArray ();

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

			using (DataContext dataContext = new DataContext(Database.DbInfrastructure))
			{
				UriContactEntity contactExample = new UriContactEntity ()
				{
					Uri = "alfred@coucou.com",
				};

				UriContactEntity contact = dataContext.GetByExample (contactExample).FirstOrDefault ();

				NaturalPersonEntity alfredExample = new NaturalPersonEntity ();
				alfredExample.Contacts.Add (contact);

				NaturalPersonEntity alfred = dataContext.GetByExample (alfredExample).FirstOrDefault ();

				Assert.IsTrue (Database2.CheckAlfred (alfred));
			}
		}


		[TestMethod]
		public void GetReferencersReference()
		{
			TestHelper.PrintStartTest ("Get referencers reference");

			using (DataContext dataContext = new DataContext(Database.DbInfrastructure))
			{
				NaturalPersonEntity alfredExample = Database2.GetCorrectExample1 ();
				NaturalPersonEntity alfred = dataContext.GetByExample<NaturalPersonEntity> (alfredExample).FirstOrDefault ();

				AbstractEntity[] referencers = dataContext.GetReferencers (alfred).Select (r => r.Item1).ToArray ();

				Assert.IsTrue (referencers.Length == 2);

				Assert.IsTrue (referencers.OfType<UriContactEntity> ().Any (c => Database2.CheckUriContact (c, "alfred@coucou.com", "Alfred")));
				Assert.IsTrue (referencers.OfType<UriContactEntity> ().Any (c => Database2.CheckUriContact (c, "alfred@blabla.com", "Alfred")));
			}
		}


		[TestMethod]
		public void GetReferencersCollection()
		{
			TestHelper.PrintStartTest ("Get referencers collection");

			using (DataContext dataContext = new DataContext(Database.DbInfrastructure))
			{
				UriContactEntity contactExample = new UriContactEntity ()
				{
					Uri = "alfred@coucou.com",
				};

				UriContactEntity contact = dataContext.GetByExample (contactExample).FirstOrDefault ();

				AbstractEntity[] referencers = dataContext.GetReferencers (contact).Select (r => r.Item1).ToArray ();

				Assert.IsTrue (referencers.Length == 1);
				Assert.IsTrue (referencers.OfType<NaturalPersonEntity> ().Any (p => Database2.CheckAlfred (p)));
			}
		}
		

		[TestMethod]
		public void GetObjectsWithDeletedEntity()
		{
			TestHelper.PrintStartTest ("Get objects with deleted entity");

			using (DataContext dataContext = new DataContext(Database.DbInfrastructure))
			{
				NaturalPersonEntity example = Database2.GetCorrectExample1 ();

				NaturalPersonEntity[] persons = dataContext.GetByExample<NaturalPersonEntity> (example).ToArray ();

				Assert.IsTrue (persons.Count () == 1);
				Assert.IsTrue (persons.Any (p => Database2.CheckAlfred (p)));

				dataContext.DeleteEntity (persons[0]);

				dataContext.SaveChanges ();

				NaturalPersonEntity[] persons2 = dataContext.GetByExample<NaturalPersonEntity> (example).ToArray ();

				Assert.IsTrue (persons2.Count () == 0);
			}

			using (DataContext dataContext = new DataContext(Database.DbInfrastructure))
			{
				NaturalPersonEntity example = Database2.GetCorrectExample1 ();

				NaturalPersonEntity[] persons = dataContext.GetByExample<NaturalPersonEntity> (example).ToArray ();

				Assert.IsTrue (persons.Count () == 0);
			}

			this.CreateDatabaseHelper ();
		}


		[TestMethod]
		public void GetObjectsWithDeletedRelation()
		{
			TestHelper.PrintStartTest ("Get objects with deleted relation");

			using (DataContext dataContext = new DataContext(Database.DbInfrastructure))
			{
				NaturalPersonEntity example = Database2.GetCorrectExample1 ();

				NaturalPersonEntity[] persons = dataContext.GetByExample<NaturalPersonEntity> (example).ToArray ();

				Assert.IsTrue (persons.Count () == 1);
				Assert.IsTrue (persons.Any (p => Database2.CheckAlfred (p)));

				persons[0].Contacts.RemoveAt (0);
				persons[0].Contacts.RemoveAt (0);

				persons[0].Gender = null;

				dataContext.SaveChanges ();

				NaturalPersonEntity[] persons2 = dataContext.GetByExample<NaturalPersonEntity> (example).ToArray ();

				Assert.IsTrue (persons2.Count () == 1);
				Assert.IsTrue (persons2[0].Contacts.Count == 0);
				Assert.IsTrue (persons2[0].Gender == null);
			}

			using (DataContext dataContext = new DataContext(Database.DbInfrastructure))
			{
				NaturalPersonEntity example = Database2.GetCorrectExample1 ();

				NaturalPersonEntity[] persons = dataContext.GetByExample<NaturalPersonEntity> (example).ToArray ();

				Assert.IsTrue (persons.Count () == 1);
				Assert.IsTrue (persons[0].Contacts.Count == 0);
				Assert.IsTrue (persons[0].Gender == null);
			}

			this.CreateDatabaseHelper ();
		}


	}


}
