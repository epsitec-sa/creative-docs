using Epsitec.Cresus.Core.Data;
using Epsitec.Cresus.Core.Entities;

using Epsitec.Cresus.DataLayer;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.Linq;

namespace Epsitec.Cresus.Core
{


	[TestClass]
	public class UnitTestRepository
	{


		[ClassInitialize]
		public static void Initialize(TestContext testContext)
		{
			TestHelper.Initialize ();
		}


		[TestMethod]
		public void CreateDatabase()
		{
			TestHelper.PrintStartTest ("Create database");

			Database.CreateAndConnectToDatabase ();

			Assert.IsTrue (Database.DbInfrastructure.IsConnectionOpen);
		}


		[TestMethod]
		public void PopulateDatabase()
		{
			TestHelper.PrintStartTest ("Populate database");

			Database2.PupulateDatabase ();
		}


		[TestMethod]
		public void GetObjects1()
		{
			TestHelper.PrintStartTest ("Get objects 1");

			using (DataContext dataContext = new DataContext (Database.DbInfrastructure))
			{
				Repository repository = new Repository (Database.DbInfrastructure, dataContext);

				NaturalPersonEntity[] persons = repository.GetEntitiesByExample<NaturalPersonEntity> (new NaturalPersonEntity ()).ToArray ();

				Assert.IsTrue (persons.Length == 3);

				Assert.IsTrue (persons.Any (p => Database2.CheckAlfred (p)));
				Assert.IsTrue (persons.Any (p => Database2.CheckGertrude (p)));
				Assert.IsTrue (persons.Any (p => Database2.CheckHans (p)));

			}
		}


		[TestMethod]
		public void GetObjects2()
		{
			TestHelper.PrintStartTest ("Get objects 2");

			using (DataContext dataContext = new DataContext (Database.DbInfrastructure))
			{
				Repository repository = new Repository (Database.DbInfrastructure, dataContext);

				NaturalPersonEntity[] persons = repository.GetEntitiesByExample<AbstractPersonEntity> (new AbstractPersonEntity ()).Cast<NaturalPersonEntity> ().ToArray ();

				Assert.IsTrue (persons.Length == 3);

				Assert.IsTrue (persons.Any (p => Database2.CheckAlfred (p)));
				Assert.IsTrue (persons.Any (p => Database2.CheckGertrude (p)));
				Assert.IsTrue (persons.Any (p => Database2.CheckHans (p)));
			}
		}


		[TestMethod]
		public void GetObjects3()
		{
			TestHelper.PrintStartTest ("Get objects 3");

			using (DataContext dataContext = new DataContext (Database.DbInfrastructure))
			{
				Repository repository = new Repository (Database.DbInfrastructure, dataContext);

				NaturalPersonEntity example = Database2.GetCorrectExample1 ();

				NaturalPersonEntity[] persons = repository.GetEntitiesByExample<NaturalPersonEntity> (example).ToArray ();

				Assert.IsTrue (persons.Count () == 1);

				Assert.IsTrue (persons.Any (p => Database2.CheckAlfred (p)));
			}
		}


		[TestMethod]
		public void GetObjects4()
		{
			TestHelper.PrintStartTest ("Get objects 4");

			using (DataContext dataContext = new DataContext (Database.DbInfrastructure))
			{
				Repository repository = new Repository (Database.DbInfrastructure, dataContext);

				NaturalPersonEntity example = Database2.GetCorrectExample2 ();

				NaturalPersonEntity[] persons = repository.GetEntitiesByExample<NaturalPersonEntity> (example).ToArray ();

				Assert.IsTrue (persons.Count () == 1);

				Assert.IsTrue (persons.Any (p => Database2.CheckAlfred (p)));
			}
		}


		[TestMethod]
		public void GetObjects5()
		{
			TestHelper.PrintStartTest ("Get objects 5");

			using (DataContext dataContext = new DataContext (Database.DbInfrastructure))
			{
				Repository repository = new Repository (Database.DbInfrastructure, dataContext);

				NaturalPersonEntity example = Database2.GetCorrectExample3 ();

				NaturalPersonEntity[] persons = repository.GetEntitiesByExample<NaturalPersonEntity> (example).ToArray ();

				Assert.IsTrue (persons.Length == 2);

				Assert.IsTrue (persons.Any (p => Database2.CheckAlfred (p)));
				Assert.IsTrue (persons.Any (p => Database2.CheckGertrude (p)));
			}
		}


		[TestMethod]
		public void GetObjects6()
		{
			TestHelper.PrintStartTest ("Get objects 6");

			using (DataContext dataContext = new DataContext (Database.DbInfrastructure))
			{
				Repository repository = new Repository (Database.DbInfrastructure, dataContext);

				NaturalPersonEntity example = Database2.GetCorrectExample4 ();

				NaturalPersonEntity[] persons = repository.GetEntitiesByExample<NaturalPersonEntity> (example).ToArray ();

				Assert.IsTrue (persons.Count () == 1);

				Assert.IsTrue (persons.Any (p => Database2.CheckAlfred (p)));
			}
		}


		[TestMethod]
		public void GetObjects7()
		{
			TestHelper.PrintStartTest ("Get objects 7");

			using (DataContext dataContext = new DataContext (Database.DbInfrastructure))
			{
				Repository repository = new Repository (Database.DbInfrastructure, dataContext);

				NaturalPersonEntity example = Database2.GetIncorrectExample1 ();

				NaturalPersonEntity[] persons = repository.GetEntitiesByExample<NaturalPersonEntity> (example).ToArray ();

				Assert.IsTrue (persons.Count () == 0);
			}
		}


		[TestMethod]
		public void GetObjects8()
		{
			TestHelper.PrintStartTest ("Get objects 8");

			using (DataContext dataContext = new DataContext (Database.DbInfrastructure))
			{
				Repository repository = new Repository (Database.DbInfrastructure, dataContext);

				NaturalPersonEntity example = Database2.GetIncorrectExample2 ();

				NaturalPersonEntity[] persons = repository.GetEntitiesByExample<NaturalPersonEntity> (example).ToArray ();

				Assert.IsTrue (persons.Count () == 0);
			}
		}


		[TestMethod]
		public void GetObjects9()
		{
			TestHelper.PrintStartTest ("Get objects 9");

			using (DataContext dataContext = new DataContext (Database.DbInfrastructure))
			{
				Repository repository = new Repository (Database.DbInfrastructure, dataContext);

				NaturalPersonEntity example = Database2.GetIncorrectExample3 ();

				NaturalPersonEntity[] persons = repository.GetEntitiesByExample<NaturalPersonEntity> (example).ToArray ();

				Assert.IsTrue (persons.Count () == 0);
			}
		}


		[TestMethod]
		public void GetObjects10()
		{
			TestHelper.PrintStartTest ("Get objects 10");

			using (DataContext dataContext = new DataContext (Database.DbInfrastructure))
			{
				Repository repository = new Repository (Database.DbInfrastructure, dataContext);

				NaturalPersonEntity example = Database2.GetIncorrectExample4 ();

				NaturalPersonEntity[] persons = repository.GetEntitiesByExample<NaturalPersonEntity> (example).ToArray ();

				Assert.IsTrue (persons.Count () == 0);
			}
		}


		[TestMethod]
		public void GetObjects11()
		{
			TestHelper.PrintStartTest ("Get objects 11");

			using (DataContext dataContext = new DataContext (Database.DbInfrastructure))
			{
				Repository repository = new Repository (Database.DbInfrastructure, dataContext);

				NaturalPersonEntity example = Database2.GetCorrectExample1 ();

				NaturalPersonEntity[] persons1 = repository.GetEntitiesByExample<NaturalPersonEntity> (example).ToArray ();
				NaturalPersonEntity[] persons2 = repository.GetEntitiesByExample<NaturalPersonEntity> (example).ToArray ();

				Assert.IsTrue (persons1.Count () == 1);
				Assert.IsTrue (persons2.Count () == 1);

				Assert.IsTrue (persons1.Any (p => Database2.CheckAlfred (p)));
				Assert.IsTrue (persons2.Any (p => Database2.CheckAlfred (p)));

				Assert.IsTrue (object.ReferenceEquals (persons1[0], persons2[0]));

			}
		}


		[TestMethod]
		public void GetObjects12()
		{
			TestHelper.PrintStartTest ("Get objects 12");

			using (DataContext dataContext = new DataContext (Database.DbInfrastructure))
			{
				Repository repository = new Repository (Database.DbInfrastructure, dataContext);

				NaturalPersonEntity example = Database2.GetCorrectExample1 ();

				NaturalPersonEntity[] persons = repository.GetEntitiesByExample<NaturalPersonEntity> (example).ToArray ();

				Assert.IsTrue (persons.Count () == 1);
				Assert.IsTrue (persons.Any (p => Database2.CheckAlfred (p)));

				dataContext.DeleteEntity (persons[0]);

				dataContext.SaveChanges ();
			}
		
			using (DataContext dataContext = new DataContext (Database.DbInfrastructure))
			{
				Repository repository = new Repository (Database.DbInfrastructure, dataContext);

				NaturalPersonEntity example = Database2.GetCorrectExample1 ();

				NaturalPersonEntity[] persons = repository.GetEntitiesByExample<NaturalPersonEntity> (example).ToArray ();

				Assert.IsTrue (persons.Count () == 0);
			}
		}


		[TestMethod]
		public void GetObjects13()
		{
			TestHelper.PrintStartTest ("Get objects 13");

			Database.CreateAndConnectToDatabase ();
			Database2.PupulateDatabase ();

			using (DataContext dataContext = new DataContext (Database.DbInfrastructure))
			{
				Repository repository = new Repository (Database.DbInfrastructure, dataContext);

				NaturalPersonEntity example = Database2.GetCorrectExample1 ();

				NaturalPersonEntity[] persons = repository.GetEntitiesByExample<NaturalPersonEntity> (example).ToArray ();

				Assert.IsTrue (persons.Count () == 1);
				Assert.IsTrue (persons.Any (p => Database2.CheckAlfred (p)));

				persons[0].Contacts.RemoveAt (0);
				persons[0].Contacts.RemoveAt (0);

				persons[0].Gender = null;

				dataContext.SaveChanges ();
			}
		
			using (DataContext dataContext = new DataContext (Database.DbInfrastructure))
			{
				Repository repository = new Repository (Database.DbInfrastructure, dataContext);

				NaturalPersonEntity example = Database2.GetCorrectExample1 ();

				NaturalPersonEntity[] persons = repository.GetEntitiesByExample<NaturalPersonEntity> (example).ToArray ();

				Assert.IsTrue (persons.Count () == 1);
				Assert.IsTrue (persons[0].Contacts.Count == 0);
				Assert.IsTrue (persons[0].Gender == null);
			}
		}


	}


}
