using Epsitec.Common.Support.EntityEngine;

using Epsitec.Common.Types;

using Epsitec.Cresus.Database;

using Epsitec.Cresus.DataLayer.Context;
using Epsitec.Cresus.DataLayer.ImportExport;
using Epsitec.Cresus.DataLayer.Infrastructure;
using Epsitec.Cresus.DataLayer.Tests.Vs.Entities;
using Epsitec.Cresus.DataLayer.Tests.Vs.Helpers;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.Collections.Generic;

using System.IO;


namespace Epsitec.Cresus.DataLayer.Tests.Vs.ImportExport
{


	[TestClass]
	public class UnitTestImportExportManager
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
		public void CompleteGraphPersistedEntities()
		{
			FileInfo file = new FileInfo ("test.xml");
			
			using (DB db = DB.ConnectToTestDatabase ())
			using (DataContext dataContext = db.DataInfrastructure.CreateDataContext (true))
			{
				NaturalPersonEntity person = dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1000000001)));
				UriContactEntity contact = dataContext.CreateEntity<UriContactEntity> ();

				contact.UriScheme = dataContext.ResolveEntity<UriSchemeEntity> (new DbKey (new DbId (1000000001)));
				contact.Uri = "new@uri.com";
				contact.NaturalPerson = person;
				person.Contacts.Add (contact);

				var x = person.Title;

				List<AbstractEntity> entities = new List<AbstractEntity> ()
				{
					person
				};

				ImportExportManager.Export (file, dataContext, entities, e => true, ExportationMode.PersistedEntities);
			}

			DatabaseCreator2.ResetEmptyTestDatabase ();
			
			using (DB db = DB.ConnectToTestDatabase ())
			{
				ImportExportManager.Import (file, db.DataInfrastructure);

				using (DataContext dataContext = DataContextHelper.ConnectToTestDatabase (db.DataInfrastructure))
				{
					NaturalPersonEntity alfred = dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1000000001)));

					Assert.IsTrue (DatabaseCreator2.CheckAlfred (alfred));

					Assert.AreSame (alfred, alfred.Contacts[0].NaturalPerson);
					Assert.AreSame (alfred, alfred.Contacts[1].NaturalPerson);
				}
			}
		}


		[TestMethod]
		public void CompleteGraphNonNullVirtualizedEntities()
		{
			FileInfo file = new FileInfo ("test.xml");
			
			using (DB db = DB.ConnectToTestDatabase ())
			using (DataContext dataContext = db.DataInfrastructure.CreateDataContext (true))
			{
				NaturalPersonEntity person = dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1000000001)));
				UriContactEntity contact = dataContext.CreateEntity<UriContactEntity> ();

				contact.UriScheme = dataContext.ResolveEntity<UriSchemeEntity> (new DbKey (new DbId (1000000001)));
				contact.Uri = "new@uri.com";
				contact.NaturalPerson = person;
				person.Contacts.Add (contact);

				var x = person.Title;

				List<AbstractEntity> entities = new List<AbstractEntity> ()
				{
					person
				};

				ImportExportManager.Export (file, dataContext, entities, e => true, ExportationMode.NonNullVirtualizedEntities);
			}

			DatabaseCreator2.ResetEmptyTestDatabase ();
			
			using (DB db = DB.ConnectToTestDatabase ())
			{
				ImportExportManager.Import (file, db.DataInfrastructure);

				using (DataContext dataContext = DataContextHelper.ConnectToTestDatabase (db.DataInfrastructure))
				{
					NaturalPersonEntity alfred = dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1000000001)));

					Assert.AreEqual ("Alfred", alfred.Firstname);
					Assert.AreEqual ("Dupond", alfred.Lastname);
					Assert.AreEqual (new Date (1950, 12, 31), alfred.BirthDate);
					Assert.IsNull (alfred.Title);
					Assert.IsNotNull (alfred.PreferredLanguage);
					Assert.AreEqual ("French", alfred.PreferredLanguage.Name);
					Assert.AreEqual ("Fr", alfred.PreferredLanguage.Code);
					Assert.IsNotNull (alfred.Gender);
					Assert.AreEqual ("Male", alfred.Gender.Name);
					Assert.AreEqual ("M", alfred.Gender.Code);
					Assert.AreEqual (3, alfred.Contacts.Count);
					Assert.AreEqual ("alfred@coucou.com", (alfred.Contacts[0] as UriContactEntity).Uri);
					Assert.AreEqual ("alfred@blabla.com", (alfred.Contacts[1] as UriContactEntity).Uri);
					Assert.AreEqual ("new@uri.com", (alfred.Contacts[2] as UriContactEntity).Uri);
					Assert.IsNotNull ((alfred.Contacts[0] as UriContactEntity).UriScheme);
					Assert.IsNotNull ((alfred.Contacts[1] as UriContactEntity).UriScheme);
					Assert.IsNotNull ((alfred.Contacts[2] as UriContactEntity).UriScheme);
					Assert.AreSame ((alfred.Contacts[0] as UriContactEntity).UriScheme, (alfred.Contacts[1] as UriContactEntity).UriScheme);
					Assert.AreSame ((alfred.Contacts[0] as UriContactEntity).UriScheme, (alfred.Contacts[2] as UriContactEntity).UriScheme);
					Assert.AreEqual ("mailto:", (alfred.Contacts[0] as UriContactEntity).UriScheme.Code);
					Assert.AreEqual ("email", (alfred.Contacts[0] as UriContactEntity).UriScheme.Name);
				}
			}
		}


		[TestMethod]
		public void PartialGraph()
		{
			FileInfo file = new FileInfo ("test.xml");
			
			using (DB db = DB.ConnectToTestDatabase ())
			using (DataContext dataContext = DataContextHelper.ConnectToTestDatabase (db.DataInfrastructure))
			{
				List<AbstractEntity> entities = new List<AbstractEntity> ()
				{
					dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1000000001)))
				};

				ImportExportManager.Export (file, dataContext, entities, e => e is NaturalPersonEntity || e is UriContactEntity, ExportationMode.PersistedEntities);
			}

			DatabaseCreator2.ResetEmptyTestDatabase ();

			using (DB db = DB.ConnectToTestDatabase ())
			{
				ImportExportManager.Import (file, db.DataInfrastructure);

				using (DataContext dataContext = DataContextHelper.ConnectToTestDatabase (db.DataInfrastructure))
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
			
			using (DB db = DB.ConnectToTestDatabase ())
			using (DataContext dataContext = DataContextHelper.ConnectToTestDatabase (db.DataInfrastructure))
			{
				List<AbstractEntity> entities = new List<AbstractEntity> ()
				{
					dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1000000001)))
				};

				ImportExportManager.Export (file, dataContext, entities, e => false, ExportationMode.PersistedEntities);
			}

			DatabaseCreator2.ResetEmptyTestDatabase ();
			
			using (DB db = DB.ConnectToTestDatabase ())
			{
				ImportExportManager.Import (file, db.DataInfrastructure);

				using (DataContext dataContext = DataContextHelper.ConnectToTestDatabase (db.DataInfrastructure))
				{
					NaturalPersonEntity alfred = dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1000000001)));

					Assert.IsNull (alfred);
				}
			}
		}


	}


}
