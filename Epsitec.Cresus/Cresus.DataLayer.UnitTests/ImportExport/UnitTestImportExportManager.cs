using Epsitec.Common.Types;

using Epsitec.Cresus.Database;

using Epsitec.Cresus.DataLayer.Context;
using Epsitec.Cresus.DataLayer.ImportExport;
using Epsitec.Cresus.DataLayer.Infrastructure;
using Epsitec.Cresus.DataLayer.UnitTests.Entities;
using Epsitec.Cresus.DataLayer.UnitTests.Helpers;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.IO;


namespace Epsitec.Cresus.DataLayer.UnitTests.ImportExport
{


	[TestClass]
	public class UnitTestImportExportManager
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

			using (DataInfrastructure dataInfrastructure = new DataInfrastructure (DatabaseHelper.DbInfrastructure))
			{
				using (DataContext dataContext = dataInfrastructure.CreateDataContext ())
				{
					DatabaseCreator2.PupulateDatabase (dataContext);
				}
			}
		}


		[TestMethod]
		public void CompleteGraph()
		{
			FileInfo file = new FileInfo ("test.xml");

			using (DataInfrastructure dataInfrastructure = new DataInfrastructure (DatabaseHelper.DbInfrastructure))
			{
				using (DataContext dataContext = dataInfrastructure.CreateDataContext ())
				{
					NaturalPersonEntity alfred = dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1000000001)));

					ImportExportManager.Export (file, dataContext, alfred, e => true);
				}
			}

			DatabaseHelper.CreateAndConnectToDatabase ();

			using (DataInfrastructure dataInfrastructure = new DataInfrastructure (DatabaseHelper.DbInfrastructure))
			{
				using (DataContext dataContext = dataInfrastructure.CreateDataContext ())
				{
					DatabaseCreator2.RegisterSchema (dataContext);
				}
				
				ImportExportManager.Import (file, dataInfrastructure);

				using (DataContext dataContext = dataInfrastructure.CreateDataContext ())
				{
					NaturalPersonEntity alfred = dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1000000001)));

					Assert.IsTrue (DatabaseCreator2.CheckAlfred (alfred));

					Assert.AreSame (alfred, alfred.Contacts[0].NaturalPerson);
					Assert.AreSame (alfred, alfred.Contacts[1].NaturalPerson);
				}
			}
		}


		[TestMethod]
		public void PartialGraph()
		{
			FileInfo file = new FileInfo ("test.xml");

			using (DataInfrastructure dataInfrastructure = new DataInfrastructure (DatabaseHelper.DbInfrastructure))
			{
				using (DataContext dataContext = dataInfrastructure.CreateDataContext ())
				{
					NaturalPersonEntity alfred = dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1000000001)));

					ImportExportManager.Export (file, dataContext, alfred, e => e is NaturalPersonEntity || e is UriContactEntity);
				}
			}

			DatabaseHelper.CreateAndConnectToDatabase ();

			using (DataInfrastructure dataInfrastructure = new DataInfrastructure (DatabaseHelper.DbInfrastructure))
			{
				using (DataContext dataContext = dataInfrastructure.CreateDataContext ())
				{
					DatabaseCreator2.RegisterSchema (dataContext);
				}
				
				ImportExportManager.Import (file, dataInfrastructure);

				using (DataContext dataContext = dataInfrastructure.CreateDataContext ())
				{
					NaturalPersonEntity alfred = dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1000000001)));

					Assert.AreEqual ("Alfred", alfred.Firstname);
					Assert.AreEqual ("Dupond", alfred.Lastname);
					Assert.AreEqual (new Date (1950, 12, 31), alfred.BirthDate);
					Assert.IsNull (alfred.PreferredLanguage);
					Assert.IsNull (alfred.Gender);
					Assert.IsNull (alfred.Title);
					Assert.AreEqual (2, alfred.Contacts.Count);

					UriContactEntity contact1 = (UriContactEntity) alfred.Contacts[0];
					UriContactEntity contact2 = (UriContactEntity) alfred.Contacts[1];
					
					Assert.AreEqual ("alfred@coucou.com", contact1.Uri);
					Assert.AreEqual ("alfred@blabla.com", contact2.Uri);

					Assert.AreEqual (alfred, contact1.NaturalPerson);
					Assert.AreEqual (alfred, contact2.NaturalPerson);

					Assert.IsNull (contact1.UriScheme);
					Assert.IsNull (contact2.UriScheme);
				}
			}
		}


		[TestMethod]
		public void EmptyGraph()
		{
			FileInfo file = new FileInfo ("test.xml");

			using (DataInfrastructure dataInfrastructure = new DataInfrastructure (DatabaseHelper.DbInfrastructure))
			{
				using (DataContext dataContext = dataInfrastructure.CreateDataContext ())
				{
					NaturalPersonEntity alfred = dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1000000001)));

					ImportExportManager.Export (file, dataContext, alfred, e => false);
				}
			}

			DatabaseHelper.CreateAndConnectToDatabase ();

			using (DataInfrastructure dataInfrastructure = new DataInfrastructure (DatabaseHelper.DbInfrastructure))
			{
				using (DataContext dataContext = dataInfrastructure.CreateDataContext ())
				{
					DatabaseCreator2.RegisterSchema (dataContext);
				}
				
				ImportExportManager.Import (file, dataInfrastructure);

				using (DataContext dataContext = dataInfrastructure.CreateDataContext ())
				{
					NaturalPersonEntity alfred = dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1000000001)));

					Assert.IsNull (alfred);
				}
			}
		}


	}


}
