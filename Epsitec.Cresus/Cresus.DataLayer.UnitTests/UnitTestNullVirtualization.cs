using Epsitec.Common.Support.EntityEngine;

using Epsitec.Cresus.Database;

using Epsitec.Cresus.DataLayer.Context;
using Epsitec.Cresus.DataLayer.UnitTests.Entities;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;


namespace Epsitec.Cresus.DataLayer.UnitTests
{


    [TestClass]
	public class UnitTestNullVirtualization
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

			using (DataContext dataContext = this.CreateDataContext ())
			{
				Database2.PupulateDatabase (dataContext);
			}
		}


		[TestMethod]
		public void ModifyNullReferenceData1()
		{
			using (DataContext dataContext = this.CreateDataContext ())
			{
				NaturalPersonEntity gertrude = dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (2)));

				Assert.IsNotNull (gertrude);
				Assert.IsFalse (EntityNullReferenceVirtualizer.IsNullEntity (gertrude));
				Assert.IsTrue (EntityNullReferenceVirtualizer.IsPatchedEntity (gertrude));
				Assert.IsFalse (EntityNullReferenceVirtualizer.IsPatchedEntityStillUnchanged (gertrude));

				LanguageEntity language = gertrude.PreferredLanguage;

				Assert.IsNotNull (language);
				Assert.IsTrue (EntityNullReferenceVirtualizer.IsPatchedEntity (language));
				Assert.IsTrue (EntityNullReferenceVirtualizer.IsPatchedEntityStillUnchanged (language));
				Assert.IsTrue (EntityNullReferenceVirtualizer.IsNullEntity (language));

				language.Code = "1337";
				language.Name = "1337 5|*34|<";

				Assert.IsTrue (EntityNullReferenceVirtualizer.IsPatchedEntity (language));
				Assert.IsFalse (EntityNullReferenceVirtualizer.IsPatchedEntityStillUnchanged (language));
				Assert.IsFalse (EntityNullReferenceVirtualizer.IsNullEntity (language));

				dataContext.SaveChanges ();
			}

			using (DataContext dataContext = this.CreateDataContext ())
			{
				NaturalPersonEntity gertrude = dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (2)));

				Assert.IsNotNull (gertrude);
				Assert.IsFalse (EntityNullReferenceVirtualizer.IsNullEntity (gertrude));
				Assert.IsTrue (EntityNullReferenceVirtualizer.IsPatchedEntity (gertrude));
				Assert.IsFalse (EntityNullReferenceVirtualizer.IsPatchedEntityStillUnchanged (gertrude));
								
				LanguageEntity language = dataContext.ResolveEntity<LanguageEntity> (new DbKey (new DbId (3)));

				Assert.IsNotNull (language);
				Assert.IsFalse (EntityNullReferenceVirtualizer.IsNullEntity (language));
				Assert.IsTrue (EntityNullReferenceVirtualizer.IsPatchedEntity (language));
				Assert.IsFalse (EntityNullReferenceVirtualizer.IsPatchedEntityStillUnchanged (language));
				Assert.AreEqual ("1337", language.Code);
				Assert.AreEqual ("1337 5|*34|<", language.Name);

				Assert.AreSame (gertrude.PreferredLanguage, language);
			}

			this.CreateDatabaseHelper ();
		}


		[TestMethod]
		public void ModifyNullReferenceData2()
		{
			using (DataContext dataContext = this.CreateDataContext ())
			{
				UriContactEntity contact = dataContext.ResolveEntity<UriContactEntity> (new DbKey (new DbId (4)));
				
				Assert.IsNotNull (contact);
				Assert.IsFalse (EntityNullReferenceVirtualizer.IsNullEntity (contact));
				Assert.IsTrue (EntityNullReferenceVirtualizer.IsPatchedEntity (contact));
				Assert.IsFalse (EntityNullReferenceVirtualizer.IsPatchedEntityStillUnchanged (contact));

				NaturalPersonEntity person = contact.NaturalPerson;

				Assert.IsNotNull (person);
				Assert.IsTrue (EntityNullReferenceVirtualizer.IsPatchedEntity (person));
				Assert.IsTrue (EntityNullReferenceVirtualizer.IsPatchedEntityStillUnchanged (person));
				Assert.IsTrue (EntityNullReferenceVirtualizer.IsNullEntity (person));

				LanguageEntity language = person.PreferredLanguage;

				Assert.IsNotNull (person);
				Assert.IsTrue (EntityNullReferenceVirtualizer.IsPatchedEntity (person));
				Assert.IsTrue (EntityNullReferenceVirtualizer.IsPatchedEntityStillUnchanged (person));
				Assert.IsTrue (EntityNullReferenceVirtualizer.IsNullEntity (person));

				Assert.IsNotNull (language);
				Assert.IsTrue (EntityNullReferenceVirtualizer.IsPatchedEntity (language));
				Assert.IsTrue (EntityNullReferenceVirtualizer.IsPatchedEntityStillUnchanged (language));
				Assert.IsTrue (EntityNullReferenceVirtualizer.IsNullEntity (language));

				language.Code = "1337";
				language.Name = "1337 5|*34|<";

				Assert.IsTrue (EntityNullReferenceVirtualizer.IsPatchedEntity (person));
				Assert.IsFalse (EntityNullReferenceVirtualizer.IsPatchedEntityStillUnchanged (person));
				Assert.IsFalse (EntityNullReferenceVirtualizer.IsNullEntity (person));

				Assert.IsTrue (EntityNullReferenceVirtualizer.IsPatchedEntity (language));
				Assert.IsFalse (EntityNullReferenceVirtualizer.IsPatchedEntityStillUnchanged (language));
				Assert.IsFalse (EntityNullReferenceVirtualizer.IsNullEntity (language));

				dataContext.SaveChanges ();
			}

			using (DataContext dataContext = this.CreateDataContext ())
			{
				UriContactEntity contact = dataContext.ResolveEntity<UriContactEntity> (new DbKey (new DbId (4)));

				Assert.IsNotNull (contact);
				Assert.IsFalse (EntityNullReferenceVirtualizer.IsNullEntity (contact));
				Assert.IsTrue (EntityNullReferenceVirtualizer.IsPatchedEntity (contact));
				Assert.IsFalse (EntityNullReferenceVirtualizer.IsPatchedEntityStillUnchanged (contact));

				LanguageEntity language = dataContext.ResolveEntity<LanguageEntity> (new DbKey (new DbId (3)));

				Assert.IsNotNull (language);
				Assert.IsFalse (EntityNullReferenceVirtualizer.IsNullEntity (language));
				Assert.IsTrue (EntityNullReferenceVirtualizer.IsPatchedEntity (language));
				Assert.IsFalse (EntityNullReferenceVirtualizer.IsPatchedEntityStillUnchanged (language));
				Assert.AreEqual ("1337", language.Code);
				Assert.AreEqual ("1337 5|*34|<", language.Name);

				Assert.AreSame (contact.NaturalPerson.PreferredLanguage, language);
			}

			this.CreateDatabaseHelper ();
		}


		[TestMethod]
		public void ReplaceNullReferenceEntity1()
		{
			using (DataContext dataContext = this.CreateDataContext ())
			{
				NaturalPersonEntity gertrude = dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (2)));

				Assert.IsNotNull (gertrude);
				Assert.IsFalse (EntityNullReferenceVirtualizer.IsNullEntity (gertrude));
				Assert.IsTrue (EntityNullReferenceVirtualizer.IsPatchedEntity (gertrude));
				Assert.IsFalse (EntityNullReferenceVirtualizer.IsPatchedEntityStillUnchanged (gertrude));

				LanguageEntity language1 = gertrude.PreferredLanguage;

				Assert.IsNotNull (language1);
				Assert.IsTrue (EntityNullReferenceVirtualizer.IsPatchedEntity (language1));
				Assert.IsTrue (EntityNullReferenceVirtualizer.IsPatchedEntityStillUnchanged (language1));
				Assert.IsTrue (EntityNullReferenceVirtualizer.IsNullEntity (language1));

				LanguageEntity language2 = dataContext.CreateEntity<LanguageEntity> ();

				language2.Code = "1337";
				language2.Name = "1337 5|*34|<";

				Assert.IsNotNull (language2);
				Assert.IsFalse (EntityNullReferenceVirtualizer.IsNullEntity (language2));
				Assert.IsTrue (EntityNullReferenceVirtualizer.IsPatchedEntity (language2));
				Assert.IsFalse (EntityNullReferenceVirtualizer.IsPatchedEntityStillUnchanged (language2));

				gertrude.PreferredLanguage = language2;

				dataContext.SaveChanges ();
			}

			using (DataContext dataContext = this.CreateDataContext ())
			{
				NaturalPersonEntity gertrude = dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (2)));

				Assert.IsNotNull (gertrude);
				Assert.IsFalse (EntityNullReferenceVirtualizer.IsNullEntity (gertrude));
				Assert.IsTrue (EntityNullReferenceVirtualizer.IsPatchedEntity (gertrude));
				Assert.IsFalse (EntityNullReferenceVirtualizer.IsPatchedEntityStillUnchanged (gertrude));

				LanguageEntity language = dataContext.ResolveEntity<LanguageEntity> (new DbKey (new DbId (3)));

				Assert.IsNotNull (language);
				Assert.IsFalse (EntityNullReferenceVirtualizer.IsNullEntity (language));
				Assert.IsTrue (EntityNullReferenceVirtualizer.IsPatchedEntity (language));
				Assert.IsFalse (EntityNullReferenceVirtualizer.IsPatchedEntityStillUnchanged (language));
				Assert.AreEqual ("1337", language.Code);
				Assert.AreEqual ("1337 5|*34|<", language.Name);

				Assert.AreSame (gertrude.PreferredLanguage, language);
			}

			this.CreateDatabaseHelper ();
		}


		[TestMethod]
		public void ReplaceNullReferenceData2()
		{
			using (DataContext dataContext = this.CreateDataContext ())
			{
				UriContactEntity contact = dataContext.ResolveEntity<UriContactEntity> (new DbKey (new DbId (4)));

				Assert.IsNotNull (contact);
				Assert.IsFalse (EntityNullReferenceVirtualizer.IsNullEntity (contact));
				Assert.IsTrue (EntityNullReferenceVirtualizer.IsPatchedEntity (contact));
				Assert.IsFalse (EntityNullReferenceVirtualizer.IsPatchedEntityStillUnchanged (contact));

				NaturalPersonEntity person = contact.NaturalPerson;

				Assert.IsNotNull (person);
				Assert.IsTrue (EntityNullReferenceVirtualizer.IsNullEntity (person));
				Assert.IsTrue (EntityNullReferenceVirtualizer.IsPatchedEntity (person));
				Assert.IsTrue (EntityNullReferenceVirtualizer.IsPatchedEntityStillUnchanged (person));

				LanguageEntity language = person.PreferredLanguage;

				Assert.IsNotNull (person);
				Assert.IsTrue (EntityNullReferenceVirtualizer.IsNullEntity (person));
				Assert.IsTrue (EntityNullReferenceVirtualizer.IsPatchedEntity (person));
				Assert.IsTrue (EntityNullReferenceVirtualizer.IsPatchedEntityStillUnchanged (person));

				Assert.IsNotNull (language);
				Assert.IsTrue (EntityNullReferenceVirtualizer.IsNullEntity (language));
				Assert.IsTrue (EntityNullReferenceVirtualizer.IsPatchedEntity (language));
				Assert.IsTrue (EntityNullReferenceVirtualizer.IsPatchedEntityStillUnchanged (language));

				LanguageEntity language2 = dataContext.CreateEntity<LanguageEntity> ();

				language2.Code = "1337";
				language2.Name = "1337 5|*34|<";

				Assert.IsNotNull (language2);
				Assert.IsFalse (EntityNullReferenceVirtualizer.IsNullEntity (language2));
				Assert.IsTrue (EntityNullReferenceVirtualizer.IsPatchedEntity (language2));
				Assert.IsFalse (EntityNullReferenceVirtualizer.IsPatchedEntityStillUnchanged (language2));

				person.PreferredLanguage = language2;

				Assert.IsNotNull (person);
				Assert.IsFalse (EntityNullReferenceVirtualizer.IsNullEntity (person));
				Assert.IsTrue (EntityNullReferenceVirtualizer.IsPatchedEntity (person));
				Assert.IsFalse (EntityNullReferenceVirtualizer.IsPatchedEntityStillUnchanged (person));

				Assert.IsFalse (EntityNullReferenceVirtualizer.IsNullEntity (person));
				Assert.IsTrue (EntityNullReferenceVirtualizer.IsPatchedEntity (person));
				Assert.IsFalse (EntityNullReferenceVirtualizer.IsPatchedEntityStillUnchanged (person));

				dataContext.SaveChanges ();
			}

			using (DataContext dataContext = this.CreateDataContext ())
			{
				UriContactEntity contact = dataContext.ResolveEntity<UriContactEntity> (new DbKey (new DbId (4)));

				Assert.IsNotNull (contact);
				Assert.IsFalse (EntityNullReferenceVirtualizer.IsNullEntity (contact));
				Assert.IsTrue (EntityNullReferenceVirtualizer.IsPatchedEntity (contact));
				Assert.IsFalse (EntityNullReferenceVirtualizer.IsPatchedEntityStillUnchanged (contact));

				LanguageEntity language = dataContext.ResolveEntity<LanguageEntity> (new DbKey (new DbId (3)));

				Assert.IsNotNull (language);
				Assert.IsFalse (EntityNullReferenceVirtualizer.IsNullEntity (language));
				Assert.IsTrue (EntityNullReferenceVirtualizer.IsPatchedEntity (language));
				Assert.IsFalse (EntityNullReferenceVirtualizer.IsPatchedEntityStillUnchanged (language));
				Assert.AreEqual ("1337", language.Code);
				Assert.AreEqual ("1337 5|*34|<", language.Name);

				Assert.AreSame (contact.NaturalPerson.PreferredLanguage, language);
			}

			this.CreateDatabaseHelper ();
		}


		// TODO Add more test methods for the following cases :
		// - auto generated null reference virtualizer for collections.
		// - every method of the EntityNullReferenceVirtualizer.


		private DataContext CreateDataContext()
		{
			return new DataContext (Database.DbInfrastructure)
			{
				EnableNullVirtualization = true
			};
		}


	}


}
