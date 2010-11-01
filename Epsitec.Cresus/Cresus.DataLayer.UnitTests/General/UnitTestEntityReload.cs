using Epsitec.Common.Support;

using Epsitec.Cresus.Database;

using Epsitec.Cresus.DataLayer.Context;
using Epsitec.Cresus.DataLayer.Infrastructure;
using Epsitec.Cresus.DataLayer.UnitTests.Entities;
using Epsitec.Cresus.DataLayer.UnitTests.Helpers;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.Linq;


namespace Epsitec.Cresus.DataLayer.UnitTests.General
{


	[TestClass]
	public sealed class UnitTestEntityReload
	{


		[ClassInitialize]
		public static void ClassInitialize(TestContext testContext)
		{
			TestHelper.Initialize ();
			
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


		[ClassCleanup]
		public static void ClassCleanup()
		{
			DatabaseHelper.DisconnectFromDatabase ();
		}


		[TestMethod]
		public void ReloadValue()
		{
			using (DataInfrastructure dataInfrastructure = new DataInfrastructure (DatabaseHelper.DbInfrastructure))
			{
				using (DataContext dataContext = dataInfrastructure.CreateDataContext ())
				{
					NaturalPersonEntity alfred = dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1)));

					Assert.AreEqual ("Alfred", alfred.Firstname);

					alfred.Firstname = "Albert";

					Assert.AreEqual ("Albert", alfred.Firstname);

					dataContext.ReloadEntity (alfred);

					Assert.AreEqual ("Alfred", alfred.Firstname);
				}
			}
		}


		[TestMethod]
		public void ReloadReference()
		{
			using (DataInfrastructure dataInfrastructure = new DataInfrastructure (DatabaseHelper.DbInfrastructure))
			{
				using (DataContext dataContext = dataInfrastructure.CreateDataContext ())
				{
					NaturalPersonEntity alfred = dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1)));
					LanguageEntity french = dataContext.ResolveEntity<LanguageEntity> (new DbKey (new DbId (1)));
					LanguageEntity german = dataContext.ResolveEntity<LanguageEntity> (new DbKey (new DbId (1)));

					Assert.AreSame (french, alfred.PreferredLanguage);

					alfred.PreferredLanguage = german;

					Assert.AreSame (german, alfred.PreferredLanguage);

					dataContext.ReloadEntity (alfred);

					Assert.AreSame (french, alfred.PreferredLanguage);
				}
			}
		}


		[TestMethod]
		public void ReloadCollection()
		{
			using (DataInfrastructure dataInfrastructure = new DataInfrastructure (DatabaseHelper.DbInfrastructure))
			{
				using (DataContext dataContext = dataInfrastructure.CreateDataContext ())
				{
					NaturalPersonEntity alfred = dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1)));
					UriContactEntity contact1 = dataContext.ResolveEntity<UriContactEntity> (new DbKey (new DbId (1)));
					UriContactEntity contact2 = dataContext.ResolveEntity<UriContactEntity> (new DbKey (new DbId (2)));

					Assert.AreEqual (2, alfred.Contacts.Count);
					Assert.AreSame (contact1, alfred.Contacts[0]);
					Assert.AreSame (contact2, alfred.Contacts[1]);

					alfred.Contacts.Clear ();

					Assert.AreEqual (0, alfred.Contacts.Count);

					dataContext.ReloadEntity (alfred);
					
					Assert.AreEqual (2, alfred.Contacts.Count);
					Assert.AreSame (contact1, alfred.Contacts[0]);
					Assert.AreSame (contact2, alfred.Contacts[1]);
				}
			}
		}


		[TestMethod]
		public void ReloadDeletedEntity()
		{
			using (DbInfrastructure dbInfrastructure1 = new DbInfrastructure())
			using (DbInfrastructure dbInfrastructure2 = new DbInfrastructure ())
			{
				dbInfrastructure1.AttachToDatabase (TestHelper.CreateDbAccess ());
				dbInfrastructure2.AttachToDatabase (TestHelper.CreateDbAccess ());

				using (DataInfrastructure dataInfrastructure1 = new DataInfrastructure (dbInfrastructure1))
				using (DataInfrastructure dataInfrastructure2 = new DataInfrastructure (dbInfrastructure2))
				{
					using (DataContext dataContext1 = dataInfrastructure1.CreateDataContext ())
					using (DataContext dataContext2 = dataInfrastructure2.CreateDataContext ())
					{
						UriContactEntity contact1 = dataContext1.CreateEntity<UriContactEntity> ();

						contact1.Uri = "coucou@blabla.com";

						dataContext1.SaveChanges ();

						UriContactEntity contact2 = dataContext2.ResolveEntity<UriContactEntity> (new DbKey (new DbId (5)));

						Assert.IsFalse (dataContext1.IsDeleted (contact1));
						Assert.IsFalse (dataContext2.IsDeleted (contact2));

						dataContext1.DeleteEntity (contact1);
						dataContext1.SaveChanges ();

						Assert.IsTrue (dataContext1.IsDeleted (contact1));
						Assert.IsFalse (dataContext2.IsDeleted (contact2));

						dataContext2.ReloadEntity (contact2);

						Assert.IsTrue (dataContext1.IsDeleted (contact1));
						Assert.IsTrue (dataContext2.IsDeleted (contact2));
					}
				}
			}
		}


		[TestMethod]
		public void ReloadFieldValue()
		{
			using (DataInfrastructure dataInfrastructure = new DataInfrastructure (DatabaseHelper.DbInfrastructure))
			{
				using (DataContext dataContext = dataInfrastructure.CreateDataContext ())
				{
					NaturalPersonEntity alfred = dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1)));

					Assert.AreEqual ("Alfred", alfred.Firstname);

					alfred.Firstname = "Albert";

					Assert.AreEqual ("Albert", alfred.Firstname);

					dataContext.ReloadEntityField (alfred, Druid.Parse("[L0AV]"));

					Assert.AreEqual ("Alfred", alfred.Firstname);
				}
			}
		}


		[TestMethod]
		public void ReloadFieldReference()
		{
			using (DataInfrastructure dataInfrastructure = new DataInfrastructure (DatabaseHelper.DbInfrastructure))
			{
				using (DataContext dataContext = dataInfrastructure.CreateDataContext ())
				{
					NaturalPersonEntity alfred = dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1)));
					LanguageEntity french = dataContext.ResolveEntity<LanguageEntity> (new DbKey (new DbId (1)));
					LanguageEntity german = dataContext.ResolveEntity<LanguageEntity> (new DbKey (new DbId (1)));

					Assert.AreSame (french, alfred.PreferredLanguage);

					alfred.PreferredLanguage = german;

					Assert.AreSame (german, alfred.PreferredLanguage);

					dataContext.ReloadEntityField (alfred, Druid.Parse("[L0AD1]"));

					Assert.AreSame (french, alfred.PreferredLanguage);
				}
			}
		}


		[TestMethod]
		public void ReloadCollectionField()
		{
			using (DataInfrastructure dataInfrastructure = new DataInfrastructure (DatabaseHelper.DbInfrastructure))
			{
				using (DataContext dataContext = dataInfrastructure.CreateDataContext ())
				{
					NaturalPersonEntity alfred = dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1)));
					UriContactEntity contact1 = dataContext.ResolveEntity<UriContactEntity> (new DbKey (new DbId (1)));
					UriContactEntity contact2 = dataContext.ResolveEntity<UriContactEntity> (new DbKey (new DbId (2)));

					Assert.AreEqual (2, alfred.Contacts.Count);
					Assert.AreSame (contact1, alfred.Contacts[0]);
					Assert.AreSame (contact2, alfred.Contacts[1]);

					alfred.Contacts.Clear ();

					Assert.AreEqual (0, alfred.Contacts.Count);

					dataContext.ReloadEntityField (alfred, Druid.Parse("[L0AS]"));

					Assert.AreEqual (2, alfred.Contacts.Count);
					Assert.AreSame (contact1, alfred.Contacts[0]);
					Assert.AreSame (contact2, alfred.Contacts[1]);
				}
			}
		}


	}


}
