using Epsitec.Common.Types;

using Epsitec.Cresus.Core.Data;
using Epsitec.Cresus.Core.Entities;

using Epsitec.Cresus.Database;
using Epsitec.Cresus.DataLayer;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.Collections.Generic;
using System.Linq;
using System.Collections;

namespace Epsitec.Cresus.Core
{


	[TestClass]
	public class UnitTestRepository
	{


		[ClassInitialize]
		public static void Initialize(TestContext testContext)
		{
			TestSetup.Initialize ();
		}


		[TestMethod]
		public void CreateDatabase()
		{
			this.StartTest ("Create database");

			Database.CreateAndConnectToDatabase ();

			Assert.IsTrue (Database.DbInfrastructure.IsConnectionOpen);
		}


		[TestMethod]
		public void PopulateDatabase()
		{
			this.StartTest ("Populate database");

			Database2.PupulateDatabase ();
		}

		
		[TestMethod]
		public void SaveWithoutChanges1()
		{
			this.StartTest ("Save without changes 1");

			using (DataContext dataContext = new DataContext (Database.DbInfrastructure))
			{
				dataContext.SaveChanges ();
			}
		}


		[TestMethod]
		public void SaveWithoutChanges2()
		{
			this.StartTest ("Save without changes 2");

			using (DataContext dataContext = new DataContext (Database.DbInfrastructure))
			{
				Repository repository = new Repository (Database.DbInfrastructure, dataContext);

				UriContactEntity[] contacts = repository.GetEntitiesByExample<UriContactEntity> (new UriContactEntity ()).ToArray ();

				Assert.IsTrue (contacts.Length == 4);

				Database2.CheckUriContacts (contacts);

				dataContext.SaveChanges ();
			}
		}


		[TestMethod]
		public void GetObjects1()
		{
			this.StartTest ("Get objects 1");

			using (DataContext dataContext = new DataContext (Database.DbInfrastructure))
			{
				Repository repository = new Repository (Database.DbInfrastructure, dataContext);

				NaturalPersonEntity[] persons = repository.GetEntitiesByExample<NaturalPersonEntity> (new NaturalPersonEntity ()).ToArray ();

				Assert.IsTrue (persons.Length == 3);

				Database2.CheckAlfred (persons);
				Database2.CheckGertrude (persons);
				Database2.CheckHans (persons);

			}
		}


		[TestMethod]
		public void GetObjects2()
		{
			this.StartTest ("Get objects 2");

			using (DataContext dataContext = new DataContext (Database.DbInfrastructure))
			{
				Repository repository = new Repository (Database.DbInfrastructure, dataContext);

				NaturalPersonEntity[] persons = repository.GetEntitiesByExample<AbstractPersonEntity> (new AbstractPersonEntity ()).Cast<NaturalPersonEntity> ().ToArray ();

				Assert.IsTrue (persons.Length == 3);

				Database2.CheckAlfred (persons);
				Database2.CheckGertrude (persons);
				Database2.CheckHans (persons);
			}
		}


		[TestMethod]
		public void GetObjects3()
		{
			this.StartTest ("Get objects 3");

			using (DataContext dataContext = new DataContext (Database.DbInfrastructure))
			{
				Repository repository = new Repository (Database.DbInfrastructure, dataContext);

				NaturalPersonEntity example = Database2.GetCorrectExample1 ();

				NaturalPersonEntity[] persons = repository.GetEntitiesByExample<NaturalPersonEntity> (example).ToArray ();

				Assert.IsTrue (persons.Count () == 1);

				Database2.CheckAlfred (persons);
			}
		}


		[TestMethod]
		public void GetObjects4()
		{
			this.StartTest ("Get objects 4");

			using (DataContext dataContext = new DataContext (Database.DbInfrastructure))
			{
				Repository repository = new Repository (Database.DbInfrastructure, dataContext);

				NaturalPersonEntity example = Database2.GetCorrectExample2 ();

				NaturalPersonEntity[] persons = repository.GetEntitiesByExample<NaturalPersonEntity> (example).ToArray ();

				Assert.IsTrue (persons.Count () == 1);

				Database2.CheckAlfred (persons);
			}
		}


		[TestMethod]
		public void GetObjects5()
		{
			this.StartTest ("Get objects 5");

			using (DataContext dataContext = new DataContext (Database.DbInfrastructure))
			{
				Repository repository = new Repository (Database.DbInfrastructure, dataContext);

				NaturalPersonEntity example = Database2.GetCorrectExample3 ();

				NaturalPersonEntity[] persons = repository.GetEntitiesByExample<NaturalPersonEntity> (example).ToArray ();

				Assert.IsTrue (persons.Length == 2);

				Database2.CheckAlfred (persons);
				Database2.CheckGertrude (persons);
			}
		}


		[TestMethod]
		public void GetObjects6()
		{
			this.StartTest ("Get objects 6");

			using (DataContext dataContext = new DataContext (Database.DbInfrastructure))
			{
				Repository repository = new Repository (Database.DbInfrastructure, dataContext);

				NaturalPersonEntity example = Database2.GetCorrectExample4 ();

				NaturalPersonEntity[] persons = repository.GetEntitiesByExample<NaturalPersonEntity> (example).ToArray ();

				Assert.IsTrue (persons.Count () == 1);

				Database2.CheckAlfred (persons);
			}
		}


		[TestMethod]
		public void GetObjects7()
		{
			this.StartTest ("Get objects 7");

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
			this.StartTest ("Get objects 8");

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
			this.StartTest ("Get objects 9");

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
			this.StartTest ("Get objects 10");

			using (DataContext dataContext = new DataContext (Database.DbInfrastructure))
			{
				Repository repository = new Repository (Database.DbInfrastructure, dataContext);

				NaturalPersonEntity example = Database2.GetIncorrectExample4 ();

				NaturalPersonEntity[] persons = repository.GetEntitiesByExample<NaturalPersonEntity> (example).ToArray ();

				Assert.IsTrue (persons.Count () == 0);
			}
		}


		private void StartTest(string name)
		{
			System.Diagnostics.Debug.WriteLine ("===========================================================================================================================================================");
			System.Diagnostics.Debug.WriteLine ("Starting test: " + name);
			System.Diagnostics.Debug.WriteLine ("===========================================================================================================================================================");
		}


	}


}
