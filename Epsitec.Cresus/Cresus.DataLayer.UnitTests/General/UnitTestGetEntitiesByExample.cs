using Epsitec.Cresus.DataLayer.Context;
using Epsitec.Cresus.DataLayer.Infrastructure;
using Epsitec.Cresus.DataLayer.UnitTests.Entities;
using Epsitec.Cresus.DataLayer.UnitTests.Helpers;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.Linq;


namespace Epsitec.Cresus.DataLayer.UnitTests.General
{


	[TestClass]
	public sealed class UnitTestGetEntitiesByExample
	{


		[ClassInitialize]
		public static void ClassInitialize(TestContext testContext)
		{
			TestHelper.Initialize ();

			DatabaseHelper.CreateAndConnectToDatabase ();

			using (DataInfrastructure dataInfrastructure = new DataInfrastructure (DatabaseHelper.DbInfrastructure))
			{
				dataInfrastructure.OpenConnection ("id");

				using (DataContext dataContext = dataInfrastructure.CreateDataContext ())
				{
					DatabaseCreator2.PupulateDatabase (dataContext);
				}
			}
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
				dataInfrastructure.OpenConnection ("id");

				using (DataContext dataContext = dataInfrastructure.CreateDataContext ())
				{
					DatabaseCreator2.PupulateDatabase (dataContext);
				}
			}
		}


		[TestMethod]
		public void GetObjects()
		{
			using (DataInfrastructure dataInfrastructure = new DataInfrastructure (DatabaseHelper.DbInfrastructure))
			{
				dataInfrastructure.OpenConnection ("id");

				using (DataContext dataContext = dataInfrastructure.CreateDataContext ())
				{
					NaturalPersonEntity[] persons = dataContext.GetByExample<NaturalPersonEntity> (new NaturalPersonEntity ()).ToArray ();

					Assert.IsTrue (persons.Length == 3);

					Assert.IsTrue (persons.Any (p => DatabaseCreator2.CheckAlfred (p)));
					Assert.IsTrue (persons.Any (p => DatabaseCreator2.CheckGertrude (p)));
					Assert.IsTrue (persons.Any (p => DatabaseCreator2.CheckHans (p)));

				}
			}
		}


		[TestMethod]
		public void GetObjectsWithCast()
		{
			using (DataInfrastructure dataInfrastructure = new DataInfrastructure (DatabaseHelper.DbInfrastructure))
			{
				dataInfrastructure.OpenConnection ("id");

				using (DataContext dataContext = dataInfrastructure.CreateDataContext ())
				{
					NaturalPersonEntity[] persons = dataContext.GetByExample<AbstractPersonEntity> (new AbstractPersonEntity ()).Cast<NaturalPersonEntity> ().ToArray ();

					Assert.IsTrue (persons.Length == 3);

					Assert.IsTrue (persons.Any (p => DatabaseCreator2.CheckAlfred (p)));
					Assert.IsTrue (persons.Any (p => DatabaseCreator2.CheckGertrude (p)));
					Assert.IsTrue (persons.Any (p => DatabaseCreator2.CheckHans (p)));
				}
			}
		}


		[TestMethod]
		public void GetObjectsWithCorrectValueExample()
		{
			using (DataInfrastructure dataInfrastructure = new DataInfrastructure (DatabaseHelper.DbInfrastructure))
			{
				dataInfrastructure.OpenConnection ("id");

				using (DataContext dataContext = dataInfrastructure.CreateDataContext ())
				{
					NaturalPersonEntity example = DatabaseCreator2.GetCorrectExample1 ();

					NaturalPersonEntity[] persons = dataContext.GetByExample<NaturalPersonEntity> (example).ToArray ();

					Assert.IsTrue (persons.Count () == 1);

					Assert.IsTrue (persons.Any (p => DatabaseCreator2.CheckAlfred (p)));
				}
			}
		}


		[TestMethod]
		public void GetObjectsWithCorrectReferenceExample()
		{
			using (DataInfrastructure dataInfrastructure = new DataInfrastructure (DatabaseHelper.DbInfrastructure))
			{
				dataInfrastructure.OpenConnection ("id");

				using (DataContext dataContext = dataInfrastructure.CreateDataContext ())
				{
					NaturalPersonEntity example = DatabaseCreator2.GetCorrectExample2 ();

					NaturalPersonEntity[] persons = dataContext.GetByExample<NaturalPersonEntity> (example).ToArray ();

					Assert.IsTrue (persons.Count () == 1);

					Assert.IsTrue (persons.Any (p => DatabaseCreator2.CheckAlfred (p)));
				}
			}
		}


		[TestMethod]
		public void GetObjectsWithCorrectCollectionExample1()
		{
			using (DataInfrastructure dataInfrastructure = new DataInfrastructure (DatabaseHelper.DbInfrastructure))
			{
				dataInfrastructure.OpenConnection ("id");

				using (DataContext dataContext = dataInfrastructure.CreateDataContext ())
				{
					NaturalPersonEntity example = DatabaseCreator2.GetCorrectExample3 ();

					NaturalPersonEntity[] persons = dataContext.GetByExample<NaturalPersonEntity> (example).ToArray ();

					Assert.IsTrue (persons.Length == 2);

					Assert.IsTrue (persons.Any (p => DatabaseCreator2.CheckAlfred (p)));
					Assert.IsTrue (persons.Any (p => DatabaseCreator2.CheckGertrude (p)));
				}
			}
		}


		[TestMethod]
		public void GetObjectsWithCorrectCollectionExample2()
		{
			using (DataInfrastructure dataInfrastructure = new DataInfrastructure (DatabaseHelper.DbInfrastructure))
			{
				dataInfrastructure.OpenConnection ("id");

				using (DataContext dataContext = dataInfrastructure.CreateDataContext ())
				{
					NaturalPersonEntity example = DatabaseCreator2.GetCorrectExample4 ();

					NaturalPersonEntity[] persons = dataContext.GetByExample<NaturalPersonEntity> (example).ToArray ();

					Assert.IsTrue (persons.Count () == 1);

					Assert.IsTrue (persons.Any (p => DatabaseCreator2.CheckAlfred (p)));
				}
			}
		}


		[TestMethod]
		public void GetObjectsWithCorrectCollectionExample3()
		{
			using (DataInfrastructure dataInfrastructure = new DataInfrastructure (DatabaseHelper.DbInfrastructure))
			{
				dataInfrastructure.OpenConnection ("id");

				using (DataContext dataContext = dataInfrastructure.CreateDataContext ())
				{
					NaturalPersonEntity example = DatabaseCreator2.GetCorrectExample5 ();

					NaturalPersonEntity[] persons = dataContext.GetByExample<NaturalPersonEntity> (example).ToArray ();

					Assert.IsTrue (persons.Count () == 1);

					Assert.IsTrue (persons.Any (p => DatabaseCreator2.CheckAlfred (p)));
				}
			}
		}


		[TestMethod]
		public void GetObjectsWithIncorrectValueExample()
		{
			using (DataInfrastructure dataInfrastructure = new DataInfrastructure (DatabaseHelper.DbInfrastructure))
			{
				dataInfrastructure.OpenConnection ("id");

				using (DataContext dataContext = dataInfrastructure.CreateDataContext ())
				{
					NaturalPersonEntity example = DatabaseCreator2.GetIncorrectExample1 ();

					NaturalPersonEntity[] persons = dataContext.GetByExample<NaturalPersonEntity> (example).ToArray ();

					Assert.IsTrue (persons.Count () == 0);
				}
			}
		}


		[TestMethod]
		public void GetObjectsWithIncorrectReferenceExample()
		{
			using (DataInfrastructure dataInfrastructure = new DataInfrastructure (DatabaseHelper.DbInfrastructure))
			{
				dataInfrastructure.OpenConnection ("id");

				using (DataContext dataContext = dataInfrastructure.CreateDataContext ())
				{
					NaturalPersonEntity example = DatabaseCreator2.GetIncorrectExample2 ();

					NaturalPersonEntity[] persons = dataContext.GetByExample<NaturalPersonEntity> (example).ToArray ();

					Assert.IsTrue (persons.Count () == 0);
				}
			}
		}


		[TestMethod]
		public void GetObjectsWithIncorrectCollectionExample1()
		{
			using (DataInfrastructure dataInfrastructure = new DataInfrastructure (DatabaseHelper.DbInfrastructure))
			{
				dataInfrastructure.OpenConnection ("id");

				using (DataContext dataContext = dataInfrastructure.CreateDataContext ())
				{
					NaturalPersonEntity example = DatabaseCreator2.GetIncorrectExample3 ();

					NaturalPersonEntity[] persons = dataContext.GetByExample<NaturalPersonEntity> (example).ToArray ();

					Assert.IsTrue (persons.Count () == 0);
				}
			}
		}


		[TestMethod]
		public void GetObjectsWithIncorrectCollectionExample2()
		{
			using (DataInfrastructure dataInfrastructure = new DataInfrastructure (DatabaseHelper.DbInfrastructure))
			{
				dataInfrastructure.OpenConnection ("id");

				using (DataContext dataContext = dataInfrastructure.CreateDataContext ())
				{
					NaturalPersonEntity example = DatabaseCreator2.GetIncorrectExample4 ();

					NaturalPersonEntity[] persons = dataContext.GetByExample<NaturalPersonEntity> (example).ToArray ();

					Assert.IsTrue (persons.Count () == 0);
				}
			}
		}


		[TestMethod]
		public void GetObjectsWithEntityEquality()
		{
			using (DataInfrastructure dataInfrastructure = new DataInfrastructure (DatabaseHelper.DbInfrastructure))
			{
				dataInfrastructure.OpenConnection ("id");

				using (DataContext dataContext = dataInfrastructure.CreateDataContext ())
				{
					NaturalPersonEntity example = DatabaseCreator2.GetCorrectExample1 ();

					NaturalPersonEntity[] persons1 = dataContext.GetByExample<NaturalPersonEntity> (example).ToArray ();
					NaturalPersonEntity[] persons2 = dataContext.GetByExample<NaturalPersonEntity> (example).ToArray ();

					Assert.IsTrue (persons1.Count () == 1);
					Assert.IsTrue (persons2.Count () == 1);

					Assert.IsTrue (persons1.Any (p => DatabaseCreator2.CheckAlfred (p)));
					Assert.IsTrue (persons2.Any (p => DatabaseCreator2.CheckAlfred (p)));

					Assert.IsTrue (object.ReferenceEquals (persons1[0], persons2[0]));
				}
			}
		}


		[TestMethod]
		public void GetFreshObject()
		{
			using (DataInfrastructure dataInfrastructure = new DataInfrastructure (DatabaseHelper.DbInfrastructure))
			{
				dataInfrastructure.OpenConnection ("id");

				using (DataContext dataContext = dataInfrastructure.CreateDataContext ())
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
			}
		}


		[TestMethod]
		public void GetObjectByReferenceReference()
		{
			using (DataInfrastructure dataInfrastructure = new DataInfrastructure (DatabaseHelper.DbInfrastructure))
			{
				dataInfrastructure.OpenConnection ("id");

				using (DataContext dataContext = dataInfrastructure.CreateDataContext ())
				{
					NaturalPersonEntity alfredExample = DatabaseCreator2.GetCorrectExample1 ();
					NaturalPersonEntity alfred = dataContext.GetByExample<NaturalPersonEntity> (alfredExample).FirstOrDefault ();

					UriContactEntity contactExample = new UriContactEntity ()
					{
						NaturalPerson = alfred,
					};

					UriContactEntity[] contacts = dataContext.GetByExample<UriContactEntity> (contactExample).ToArray ();

					Assert.IsTrue (contacts.Length == 2);

					Assert.IsTrue (contacts.Any (c => DatabaseCreator2.CheckUriContact (c, "alfred@coucou.com", "Alfred")));
					Assert.IsTrue (contacts.Any (c => DatabaseCreator2.CheckUriContact (c, "alfred@blabla.com", "Alfred")));

					Assert.IsTrue (contacts.All (c => c.NaturalPerson == alfred));
				}
			}
		}


		[TestMethod]
		public void GetObjectByCollectionReference()
		{
			using (DataInfrastructure dataInfrastructure = new DataInfrastructure (DatabaseHelper.DbInfrastructure))
			{
				dataInfrastructure.OpenConnection ("id");

				using (DataContext dataContext = dataInfrastructure.CreateDataContext ())
				{
					UriContactEntity contactExample = new UriContactEntity ()
						{
							Uri = "alfred@coucou.com",
						};

					UriContactEntity contact = dataContext.GetByExample (contactExample).FirstOrDefault ();

					NaturalPersonEntity alfredExample = new NaturalPersonEntity ();
					alfredExample.Contacts.Add (contact);

					NaturalPersonEntity alfred = dataContext.GetByExample (alfredExample).FirstOrDefault ();

					Assert.IsTrue (DatabaseCreator2.CheckAlfred (alfred));
				}
			}
		}
		

		[TestMethod]
		public void GetObjectsWithDeletedEntity()
		{
			using (DataInfrastructure dataInfrastructure = new DataInfrastructure (DatabaseHelper.DbInfrastructure))
			{
				dataInfrastructure.OpenConnection ("id");

				using (DataContext dataContext = dataInfrastructure.CreateDataContext ())
				{
					NaturalPersonEntity example = DatabaseCreator2.GetCorrectExample1 ();

					NaturalPersonEntity[] persons = dataContext.GetByExample<NaturalPersonEntity> (example).ToArray ();

					Assert.IsTrue (persons.Count () == 1);
					Assert.IsTrue (persons.Any (p => DatabaseCreator2.CheckAlfred (p)));

					dataContext.DeleteEntity (persons[0]);

					dataContext.SaveChanges ();

					NaturalPersonEntity[] persons2 = dataContext.GetByExample<NaturalPersonEntity> (example).ToArray ();

					Assert.IsTrue (persons2.Count () == 0);
				}

				using (DataContext dataContext = dataInfrastructure.CreateDataContext ())
				{
					NaturalPersonEntity example = DatabaseCreator2.GetCorrectExample1 ();

					NaturalPersonEntity[] persons = dataContext.GetByExample<NaturalPersonEntity> (example).ToArray ();

					Assert.IsTrue (persons.Count () == 0);
				}
			}
		}


		[TestMethod]
		public void GetObjectsWithDeletedRelation()
		{
			using (DataInfrastructure dataInfrastructure = new DataInfrastructure (DatabaseHelper.DbInfrastructure))
			{
				dataInfrastructure.OpenConnection ("id");

				using (DataContext dataContext = dataInfrastructure.CreateDataContext ())
				{
					NaturalPersonEntity example = DatabaseCreator2.GetCorrectExample1 ();

					NaturalPersonEntity[] persons = dataContext.GetByExample<NaturalPersonEntity> (example).ToArray ();

					Assert.IsTrue (persons.Count () == 1);
					Assert.IsTrue (persons.Any (p => DatabaseCreator2.CheckAlfred (p)));

					persons[0].Contacts.RemoveAt (0);
					persons[0].Contacts.RemoveAt (0);

					persons[0].Gender = null;

					dataContext.SaveChanges ();

					NaturalPersonEntity[] persons2 = dataContext.GetByExample<NaturalPersonEntity> (example).ToArray ();

					Assert.IsTrue (persons2.Count () == 1);
					Assert.IsTrue (persons2[0].Contacts.Count == 0);
					Assert.IsTrue (persons2[0].Gender == null);
				}

				using (DataContext dataContext = dataInfrastructure.CreateDataContext ())
				{
					NaturalPersonEntity example = DatabaseCreator2.GetCorrectExample1 ();

					NaturalPersonEntity[] persons = dataContext.GetByExample<NaturalPersonEntity> (example).ToArray ();

					Assert.IsTrue (persons.Count () == 1);
					Assert.IsTrue (persons[0].Contacts.Count == 0);
					Assert.IsTrue (persons[0].Gender == null);
				}
			}
		}


	}


}
