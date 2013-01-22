using Epsitec.Common.Types;

using Epsitec.Cresus.Database;

using Epsitec.Cresus.DataLayer.Context;
using Epsitec.Cresus.DataLayer.Infrastructure;
using Epsitec.Cresus.DataLayer.Tests.Vs.Entities;
using Epsitec.Cresus.DataLayer.Tests.Vs.Helpers;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.Linq;


namespace Epsitec.Cresus.DataLayer.Tests.Vs.General
{


	[TestClass]
	public sealed class UnitTestEntityReload
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
		public void ReloadValueModifiedInMemory()
		{			
			using (DB db = DB.ConnectToTestDatabase ())
			using (DataContext dataContext = DataContextHelper.ConnectToTestDatabase (db.DataInfrastructure, enableReload: true))
			{
				NaturalPersonEntity alfred = dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1000000001)));

				Assert.AreEqual ("Alfred", alfred.Firstname);

				alfred.Firstname = "Albert";

				Assert.AreEqual ("Albert", alfred.Firstname);

				bool changes = dataContext.Reload ();
				Assert.IsFalse (changes);

				Assert.AreEqual ("Albert", alfred.Firstname);
				Assert.IsTrue (dataContext.GetEntitiesModified ().Contains (alfred));
			}
		}


		[TestMethod]
		public void ReloadValueModifiedInDatabase()
		{
			// Here we need two DataInfrastructures, otherwise both DataContext will be synchronized
			// and we don't want that. Therefore, we also require two DbInfrastructures, because they
			// are tightly coupled with the DataInfrastructures.
			// Marc

			using (DB db1 = DB.ConnectToTestDatabase ())
			using (DB db2 = DB.ConnectToTestDatabase ())
			using (DataContext dataContext1 = DataContextHelper.ConnectToTestDatabase (db1.DataInfrastructure, enableReload: true))
			using (DataContext dataContext2 = DataContextHelper.ConnectToTestDatabase (db2.DataInfrastructure, enableReload: true))
			{
				NaturalPersonEntity alfred1 = dataContext1.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1000000001)));
				NaturalPersonEntity alfred2 = dataContext2.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1000000001)));

				Assert.AreEqual ("Alfred", alfred1.Firstname);
				Assert.AreEqual ("Alfred", alfred2.Firstname);

				alfred1.Firstname = "Albert";

				Assert.AreEqual ("Albert", alfred1.Firstname);
				Assert.AreEqual ("Alfred", alfred2.Firstname);

				dataContext1.SaveChanges ();

				bool changes = dataContext2.Reload ();
				Assert.IsTrue (changes);

				Assert.AreEqual ("Albert", alfred1.Firstname);
				Assert.AreEqual ("Albert", alfred2.Firstname);
				Assert.IsFalse (dataContext1.GetEntitiesModified ().Contains (alfred1));
				Assert.IsFalse (dataContext2.GetEntitiesModified ().Contains (alfred2));
			}
		}


		[TestMethod]
		public void ReloadReferenceModifiedInMemory()
		{		
			using (DB db = DB.ConnectToTestDatabase ())
			using (DataContext dataContext = DataContextHelper.ConnectToTestDatabase (db.DataInfrastructure, enableReload: true))
			{
				NaturalPersonEntity alfred = dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1000000001)));
				LanguageEntity french = dataContext.ResolveEntity<LanguageEntity> (new DbKey (new DbId (1000000001)));
				LanguageEntity german = dataContext.ResolveEntity<LanguageEntity> (new DbKey (new DbId (1000000002)));

				Assert.AreSame (french, alfred.PreferredLanguage);

				alfred.PreferredLanguage = german;

				Assert.AreSame (german, alfred.PreferredLanguage);

				bool changes = dataContext.Reload ();
				Assert.IsFalse (changes);

				Assert.AreSame (german, alfred.PreferredLanguage);
				Assert.IsTrue (dataContext.GetEntitiesModified ().Contains (alfred));
			}
		}


		[TestMethod]
		public void ReloadReferenceModifiedInDatabase()
		{
			// Here we need two DataInfrastructures, otherwise both DataContext will be synchronized
			// and we don't want that. Therefore, we also require two DbInfrastructures, because they
			// are tightly coupled with the DataInfrastructures.
			// Marc

			using (DB db1 = DB.ConnectToTestDatabase ())
			using (DB db2 = DB.ConnectToTestDatabase ())
			using (DataContext dataContext1 = DataContextHelper.ConnectToTestDatabase (db1.DataInfrastructure, enableReload: true))
			using (DataContext dataContext2 = DataContextHelper.ConnectToTestDatabase (db2.DataInfrastructure, enableReload: true))
			{
				NaturalPersonEntity alfred1 = dataContext1.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1000000001)));
				LanguageEntity french1 = dataContext1.ResolveEntity<LanguageEntity> (new DbKey (new DbId (1000000001)));
				LanguageEntity german1 = dataContext1.ResolveEntity<LanguageEntity> (new DbKey (new DbId (1000000002)));

				NaturalPersonEntity alfred2 = dataContext2.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1000000001)));
				LanguageEntity french2 = dataContext2.ResolveEntity<LanguageEntity> (new DbKey (new DbId (1000000001)));
				LanguageEntity german2 = dataContext2.ResolveEntity<LanguageEntity> (new DbKey (new DbId (1000000002)));

				Assert.AreSame (french1, alfred1.PreferredLanguage);
				Assert.AreSame (french2, alfred2.PreferredLanguage);

				alfred1.PreferredLanguage = german1;

				Assert.AreSame (german1, alfred1.PreferredLanguage);
				Assert.AreSame (french2, alfred2.PreferredLanguage);

				dataContext1.SaveChanges ();

				bool changes = dataContext2.Reload ();
				Assert.IsTrue (changes);

				Assert.AreSame (german1, alfred1.PreferredLanguage);
				Assert.AreSame (german2, alfred2.PreferredLanguage);
				Assert.IsFalse (dataContext1.GetEntitiesModified ().Contains (alfred1));
				Assert.IsFalse (dataContext2.GetEntitiesModified ().Contains (alfred2));
			}
		}


		[TestMethod]
		public void ReloadCollectionModifiedInMemory()
		{		
			using (DB db = DB.ConnectToTestDatabase ())
			using (DataContext dataContext = DataContextHelper.ConnectToTestDatabase (db.DataInfrastructure, enableReload: true))
			{
				NaturalPersonEntity alfred = dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1000000001)));
				UriContactEntity contact1 = dataContext.ResolveEntity<UriContactEntity> (new DbKey (new DbId (1000000001)));
				UriContactEntity contact2 = dataContext.ResolveEntity<UriContactEntity> (new DbKey (new DbId (1000000002)));

				Assert.AreEqual (2, alfred.Contacts.Count);
				Assert.AreSame (contact1, alfred.Contacts[0]);
				Assert.AreSame (contact2, alfred.Contacts[1]);

				alfred.Contacts.Clear ();

				Assert.AreEqual (0, alfred.Contacts.Count);

				bool changes = dataContext.Reload ();
				Assert.IsFalse (changes);

				Assert.AreEqual (0, alfred.Contacts.Count);
				Assert.IsTrue (dataContext.GetEntitiesModified ().Contains (alfred));
			}
		}


		[TestMethod]
		public void ReloadCollectionModifiedInDatabase()
		{
			// Here we need two DataInfrastructures, otherwise both DataContext will be synchronized
			// and we don't want that. Therefore, we also require two DbInfrastructures, because they
			// are tightly coupled with the DataInfrastructures.
			// Marc

			using (DB db1 = DB.ConnectToTestDatabase ())
			using (DB db2 = DB.ConnectToTestDatabase ())
			using (DataContext dataContext1 = DataContextHelper.ConnectToTestDatabase (db1.DataInfrastructure, enableReload: true))
			using (DataContext dataContext2 = DataContextHelper.ConnectToTestDatabase (db2.DataInfrastructure, enableReload: true))
			{
				NaturalPersonEntity alfred1 = dataContext1.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1000000001)));
				UriContactEntity contact1A = dataContext1.ResolveEntity<UriContactEntity> (new DbKey (new DbId (1000000001)));
				UriContactEntity contact1B = dataContext1.ResolveEntity<UriContactEntity> (new DbKey (new DbId (1000000002)));

				NaturalPersonEntity alfred2 = dataContext2.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1000000001)));
				UriContactEntity contact2A = dataContext2.ResolveEntity<UriContactEntity> (new DbKey (new DbId (1000000001)));
				UriContactEntity contact2B = dataContext2.ResolveEntity<UriContactEntity> (new DbKey (new DbId (1000000002)));

				Assert.AreEqual (2, alfred1.Contacts.Count);
				Assert.AreSame (contact1A, alfred1.Contacts[0]);
				Assert.AreSame (contact1B, alfred1.Contacts[1]);

				Assert.AreEqual (2, alfred2.Contacts.Count);
				Assert.AreSame (contact2A, alfred2.Contacts[0]);
				Assert.AreSame (contact2B, alfred2.Contacts[1]);

				alfred1.Contacts.Clear ();

				Assert.AreEqual (0, alfred1.Contacts.Count);

				Assert.AreEqual (2, alfred2.Contacts.Count);
				Assert.AreSame (contact2A, alfred2.Contacts[0]);
				Assert.AreSame (contact2B, alfred2.Contacts[1]);

				dataContext1.SaveChanges ();

				bool changes = dataContext2.Reload ();
				Assert.IsTrue (changes);

				Assert.AreEqual (0, alfred1.Contacts.Count);
				Assert.AreEqual (0, alfred2.Contacts.Count);
				Assert.IsFalse (dataContext1.GetEntitiesModified ().Contains (alfred1));
				Assert.IsFalse (dataContext2.GetEntitiesModified ().Contains (alfred2));
			}
		}


		[TestMethod]
		public void ReloadEntityDeletedInMemory()
		{		
			using (DB db = DB.ConnectToTestDatabase ())
			using (DataContext dataContext = DataContextHelper.ConnectToTestDatabase (db.DataInfrastructure, enableReload: true))
			{
				NaturalPersonEntity alfred = dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1000000001)));

				dataContext.SaveChanges ();

				dataContext.DeleteEntity (alfred);

				Assert.IsTrue (dataContext.IsDeleted (alfred));

				bool changes = dataContext.Reload ();
				Assert.IsFalse (changes);

				Assert.IsTrue (dataContext.IsDeleted (alfred));
			}
		}


		[TestMethod]
		public void ReloadEntityDeletedInDatabase()
		{
			// Here we need two DataInfrastructures, otherwise both DataContext will be synchronized
			// and we don't want that. Therefore, we also require two DbInfrastructures, because they
			// are tightly coupled with the DataInfrastructures.
			// Marc

			using (DB db1 = DB.ConnectToTestDatabase ())
			using (DB db2 = DB.ConnectToTestDatabase ())
			using (DataContext dataContext1 = DataContextHelper.ConnectToTestDatabase (db1.DataInfrastructure, enableReload: true))
			using (DataContext dataContext2 = DataContextHelper.ConnectToTestDatabase (db2.DataInfrastructure, enableReload: true))
			{
				NaturalPersonEntity alfred1 = dataContext1.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1000000001)));
				NaturalPersonEntity alfred2 = dataContext2.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1000000001)));

				Assert.IsFalse (dataContext1.IsDeleted (alfred1));
				Assert.IsFalse (dataContext2.IsDeleted (alfred2));

				dataContext1.DeleteEntity (alfred1);
				dataContext1.SaveChanges ();

				Assert.IsTrue (dataContext1.IsDeleted (alfred1));
				Assert.IsFalse (dataContext2.IsDeleted (alfred2));

				bool changes = dataContext2.Reload ();
				Assert.IsTrue (changes);

				Assert.IsTrue (dataContext1.IsDeleted (alfred1));
				Assert.IsTrue (dataContext2.IsDeleted (alfred2));
			}
		}


		[TestMethod]
		public void ReloadNonPersistentEntities()
		{		
			using (DB db = DB.ConnectToTestDatabase ())
			using (DataContext dataContext = DataContextHelper.ConnectToTestDatabase (db.DataInfrastructure, enableReload: true))
			{

				NaturalPersonEntity albertLeVert = dataContext.CreateEntity<NaturalPersonEntity> ();

				albertLeVert.Firstname = "Albert";
				albertLeVert.Lastname = "Le Vert";

				Assert.IsTrue (dataContext.Contains (albertLeVert));
				Assert.AreEqual ("Albert", albertLeVert.Firstname);
				Assert.AreEqual ("Le Vert", albertLeVert.Lastname);

				bool changes = dataContext.Reload ();
				Assert.IsFalse (changes);

				Assert.IsTrue (dataContext.Contains (albertLeVert));
				Assert.AreEqual ("Albert", albertLeVert.Firstname);
				Assert.AreEqual ("Le Vert", albertLeVert.Lastname);
				Assert.IsTrue (dataContext.GetEntitiesModified ().Contains (albertLeVert));
			}
		}


		[TestMethod]
		public void SpecialCase()
		{
			// Here we need two DataInfrastructures, otherwise both DataContext will be synchronized
			// and we don't want that. Therefore, we also require two DbInfrastructures, because they
			// are tightly coupled with the DataInfrastructures.
			// Marc

			using (DB db1 = DB.ConnectToTestDatabase ())
			using (DB db2 = DB.ConnectToTestDatabase ())
			using (DataContext dataContext1 = DataContextHelper.ConnectToTestDatabase (db1.DataInfrastructure, enableReload: true))
			using (DataContext dataContext2 = DataContextHelper.ConnectToTestDatabase (db2.DataInfrastructure, enableReload: true))
			{
				NaturalPersonEntity alfred1 = dataContext1.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1000000001)));
				NaturalPersonEntity gertrude2 = dataContext2.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1000000002)));

				alfred1.Firstname = "Albert";
				dataContext1.SaveChanges ();

				NaturalPersonEntity alfred2 = dataContext2.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1000000001)));

				alfred2.Firstname = "Edgar";

				bool changes = dataContext2.Reload ();
				Assert.IsFalse (changes);

				Assert.AreEqual ("Edgar", alfred2.Firstname);
			}
		}


		[TestMethod]
		public void ComplexCase()
		{
			// Here we need two DataInfrastructures, otherwise both DataContext will be synchronized
			// and we don't want that. Therefore, we also require two DbInfrastructures, because they
			// are tightly coupled with the DataInfrastructures.
			// Marc

			using (DB db1 = DB.ConnectToTestDatabase ())
			using (DB db2 = DB.ConnectToTestDatabase ())
			using (DataContext dataContext1 = DataContextHelper.ConnectToTestDatabase (db1.DataInfrastructure, enableReload: true))
			using (DataContext dataContext2 = DataContextHelper.ConnectToTestDatabase (db2.DataInfrastructure, enableReload: true))
			{
				NaturalPersonEntity alfred1 = dataContext1.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1000000001)));
				NaturalPersonEntity gertrude2 = dataContext2.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1000000002)));
				NaturalPersonEntity hans1 = dataContext1.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1000000003)));
				NaturalPersonEntity hans2 = dataContext2.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1000000003)));

				alfred1.Firstname = "Albert";

				gertrude2.Firstname = "Germaine";

				hans1.BirthDate = new Date (2000, 1, 1);

				UriSchemeEntity uriScheme1 = dataContext1.ResolveEntity<UriSchemeEntity> (new DbKey (new DbId (1000000001)));
				UriContactEntity uriContact1 = dataContext1.CreateEntity<UriContactEntity> ();

				uriContact1.Uri = "hans@coucou.com";
				uriContact1.UriScheme = uriScheme1;

				hans1.Contacts.Add (uriContact1);

				Assert.AreEqual (0, hans2.Contacts.Count);

				dataContext1.SaveChanges ();

				NaturalPersonEntity alfred2 = dataContext2.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1000000001)));

				alfred2.Firstname = "Edgar";

				bool changes = dataContext2.Reload ();
				Assert.IsTrue (changes);

				Assert.AreEqual ("Edgar", alfred2.Firstname);
				Assert.AreEqual ("Germaine", gertrude2.Firstname);
				Assert.AreEqual (new Date (2000, 1, 1), hans2.BirthDate);
				Assert.AreEqual (1, hans2.Contacts.Count);
			}
		}


	}


}
