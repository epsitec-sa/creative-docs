using Epsitec.Common.Support.EntityEngine;

using Epsitec.Cresus.Database;

using Epsitec.Cresus.DataLayer.Context;
using Epsitec.Cresus.DataLayer.Infrastructure;
using Epsitec.Cresus.DataLayer.Schema;
using Epsitec.Cresus.DataLayer.Tests.Vs.Entities;
using Epsitec.Cresus.DataLayer.Tests.Vs.Helpers;

using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace Epsitec.Cresus.DataLayer.Tests.Vs.General
{


    [TestClass]
	public sealed class UnitTestNullVirtualization
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
		public void ModifyNullReferenceData1()
		{
			
			using (DataInfrastructure dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase ())
			{
				using (DataContext dataContext = DataContextHelper.ConnectToTestDatabase(dataInfrastructure, enableNullVirtualization:true))
				{
					NaturalPersonEntity gertrude = dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1000000002)));

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
					Assert.AreEqual ("1337", language.Code);
					Assert.AreEqual ("1337 5|*34|<", language.Name);

					dataContext.SaveChanges ();
				}

				using (DataContext dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure, enableNullVirtualization: true))
				{
					NaturalPersonEntity gertrude = dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1000000002)));

					Assert.IsNotNull (gertrude);
					Assert.IsFalse (EntityNullReferenceVirtualizer.IsNullEntity (gertrude));
					Assert.IsTrue (EntityNullReferenceVirtualizer.IsPatchedEntity (gertrude));
					Assert.IsFalse (EntityNullReferenceVirtualizer.IsPatchedEntityStillUnchanged (gertrude));

					LanguageEntity language = dataContext.ResolveEntity<LanguageEntity> (new DbKey (new DbId (1000000003)));

					Assert.IsNotNull (language);
					Assert.IsFalse (EntityNullReferenceVirtualizer.IsNullEntity (language));
					Assert.IsTrue (EntityNullReferenceVirtualizer.IsPatchedEntity (language));
					Assert.IsFalse (EntityNullReferenceVirtualizer.IsPatchedEntityStillUnchanged (language));
					Assert.AreEqual ("1337", language.Code);
					Assert.AreEqual ("1337 5|*34|<", language.Name);

					Assert.AreSame (gertrude.PreferredLanguage, language);
				}
			}
		}


		[TestMethod]
		public void ModifyNullReferenceData2()
		{
			
			using (DataInfrastructure dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase ())
			{
				using (DataContext dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure, enableNullVirtualization: true))
				{
					UriContactEntity contact = dataContext.ResolveEntity<UriContactEntity> (new DbKey (new DbId (1000000004)));

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
					Assert.AreEqual ("1337", language.Code);
					Assert.AreEqual ("1337 5|*34|<", language.Name);

					dataContext.SaveChanges ();
				}

				using (DataContext dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure, enableNullVirtualization: true))
				{
					UriContactEntity contact = dataContext.ResolveEntity<UriContactEntity> (new DbKey (new DbId (1000000004)));

					Assert.IsNotNull (contact);
					Assert.IsFalse (EntityNullReferenceVirtualizer.IsNullEntity (contact));
					Assert.IsTrue (EntityNullReferenceVirtualizer.IsPatchedEntity (contact));
					Assert.IsFalse (EntityNullReferenceVirtualizer.IsPatchedEntityStillUnchanged (contact));

					LanguageEntity language = dataContext.ResolveEntity<LanguageEntity> (new DbKey (new DbId (1000000003)));

					Assert.IsNotNull (language);
					Assert.IsFalse (EntityNullReferenceVirtualizer.IsNullEntity (language));
					Assert.IsTrue (EntityNullReferenceVirtualizer.IsPatchedEntity (language));
					Assert.IsFalse (EntityNullReferenceVirtualizer.IsPatchedEntityStillUnchanged (language));
					Assert.AreEqual ("1337", language.Code);
					Assert.AreEqual ("1337 5|*34|<", language.Name);

					Assert.AreSame (contact.NaturalPerson.PreferredLanguage, language);
				}
			}
		}


		[TestMethod]
		public void ReplaceNullReferenceEntity1()
		{
			
			using (DataInfrastructure dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase ())
			{
				using (DataContext dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure, enableNullVirtualization: true))
				{
					NaturalPersonEntity gertrude = dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1000000002)));

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
					Assert.AreEqual ("1337", language2.Code);
					Assert.AreEqual ("1337 5|*34|<", language2.Name);

					gertrude.PreferredLanguage = language2;

					dataContext.SaveChanges ();
				}

				using (DataContext dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure, enableNullVirtualization: true))
				{
					NaturalPersonEntity gertrude = dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1000000002)));

					Assert.IsNotNull (gertrude);
					Assert.IsFalse (EntityNullReferenceVirtualizer.IsNullEntity (gertrude));
					Assert.IsTrue (EntityNullReferenceVirtualizer.IsPatchedEntity (gertrude));
					Assert.IsFalse (EntityNullReferenceVirtualizer.IsPatchedEntityStillUnchanged (gertrude));

					LanguageEntity language = dataContext.ResolveEntity<LanguageEntity> (new DbKey (new DbId (1000000003)));

					Assert.IsNotNull (language);
					Assert.IsFalse (EntityNullReferenceVirtualizer.IsNullEntity (language));
					Assert.IsTrue (EntityNullReferenceVirtualizer.IsPatchedEntity (language));
					Assert.IsFalse (EntityNullReferenceVirtualizer.IsPatchedEntityStillUnchanged (language));
					Assert.AreEqual ("1337", language.Code);
					Assert.AreEqual ("1337 5|*34|<", language.Name);

					Assert.AreSame (gertrude.PreferredLanguage, language);
				}
			}
		}


		[TestMethod]
		public void ReplaceNullReferenceData2()
		{
			
			using (DataInfrastructure dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase ())
			{
				using (DataContext dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure, enableNullVirtualization: true))
				{
					UriContactEntity contact = dataContext.ResolveEntity<UriContactEntity> (new DbKey (new DbId (1000000004)));

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
					Assert.AreEqual ("1337", language2.Code);
					Assert.AreEqual ("1337 5|*34|<", language2.Name);

					person.PreferredLanguage = language2;

					Assert.IsNotNull (person);
					Assert.IsFalse (EntityNullReferenceVirtualizer.IsNullEntity (person));
					Assert.IsTrue (EntityNullReferenceVirtualizer.IsPatchedEntity (person));
					Assert.IsFalse (EntityNullReferenceVirtualizer.IsPatchedEntityStillUnchanged (person));

					Assert.IsFalse (EntityNullReferenceVirtualizer.IsNullEntity (language2));
					Assert.IsTrue (EntityNullReferenceVirtualizer.IsPatchedEntity (language2));
					Assert.IsFalse (EntityNullReferenceVirtualizer.IsPatchedEntityStillUnchanged (language2));

					dataContext.SaveChanges ();
				}

				using (DataContext dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure, enableNullVirtualization: true))
				{
					UriContactEntity contact = dataContext.ResolveEntity<UriContactEntity> (new DbKey (new DbId (1000000004)));

					Assert.IsNotNull (contact);
					Assert.IsFalse (EntityNullReferenceVirtualizer.IsNullEntity (contact));
					Assert.IsTrue (EntityNullReferenceVirtualizer.IsPatchedEntity (contact));
					Assert.IsFalse (EntityNullReferenceVirtualizer.IsPatchedEntityStillUnchanged (contact));

					LanguageEntity language = dataContext.ResolveEntity<LanguageEntity> (new DbKey (new DbId (1000000003)));

					Assert.IsNotNull (language);
					Assert.IsFalse (EntityNullReferenceVirtualizer.IsNullEntity (language));
					Assert.IsTrue (EntityNullReferenceVirtualizer.IsPatchedEntity (language));
					Assert.IsFalse (EntityNullReferenceVirtualizer.IsPatchedEntityStillUnchanged (language));
					Assert.AreEqual ("1337", language.Code);
					Assert.AreEqual ("1337 5|*34|<", language.Name);

					Assert.AreSame (contact.NaturalPerson.PreferredLanguage, language);
				}
			}
		}


	}


}
