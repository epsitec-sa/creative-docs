using Epsitec.Common.Support.EntityEngine;

using Epsitec.Common.UnitTesting;

using Epsitec.Cresus.Database;

using Epsitec.Cresus.DataLayer.Context;
using Epsitec.Cresus.DataLayer.Infrastructure;
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
		public void CreationWithoutModificationDepth1()
		{
			using (DB db = DB.ConnectToTestDatabase ())
			{
				for (int i = 0; i < 2; i++)
				{
					using (DataContext dataContext = DataContextHelper.ConnectToTestDatabase (db.DataInfrastructure, enableNullVirtualization: true))
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

						if (i == 1)
						{
							dataContext.SaveChanges ();
						}
					}
				}
			}
		}


		[TestMethod]
		public void CreationWithoutModificationDepth2()
		{
			using (DB db = DB.ConnectToTestDatabase ())
			{
				for (int i = 0; i < 2; i++)
				{
					using (DataContext dataContext = DataContextHelper.ConnectToTestDatabase (db.DataInfrastructure, enableNullVirtualization: true))
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

						if (i == 1)
						{
							dataContext.SaveChanges ();
						}
					}
				}
			}
		}


		[TestMethod]
		public void CreationWIthModificationDepth1()
		{
			using (DB db = DB.ConnectToTestDatabase ())
			{
				using (DataContext dataContext = DataContextHelper.ConnectToTestDatabase (db.DataInfrastructure, enableNullVirtualization: true))
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

				using (DataContext dataContext = DataContextHelper.ConnectToTestDatabase (db.DataInfrastructure, enableNullVirtualization: true))
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
		public void CreationWithModificationDepth2()
		{
			using (DB db = DB.ConnectToTestDatabase ())
			{
				using (DataContext dataContext = DataContextHelper.ConnectToTestDatabase (db.DataInfrastructure, enableNullVirtualization: true))
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

				using (DataContext dataContext = DataContextHelper.ConnectToTestDatabase (db.DataInfrastructure, enableNullVirtualization: true))
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
		public void CreationAndReplacementDepth1()
		{
			using (DB db = DB.ConnectToTestDatabase ())
			{
				using (DataContext dataContext = DataContextHelper.ConnectToTestDatabase (db.DataInfrastructure, enableNullVirtualization: true))
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

				using (DataContext dataContext = DataContextHelper.ConnectToTestDatabase (db.DataInfrastructure, enableNullVirtualization: true))
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
		public void CreationAndReplacementDepth2()
		{
			using (DB db = DB.ConnectToTestDatabase ())
			{
				using (DataContext dataContext = DataContextHelper.ConnectToTestDatabase (db.DataInfrastructure, enableNullVirtualization: true))
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

				using (DataContext dataContext = DataContextHelper.ConnectToTestDatabase (db.DataInfrastructure, enableNullVirtualization: true))
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
		public void CreationModificationAndAssignation()
		{
			using (DB db = DB.ConnectToTestDatabase ())
			{
				using (DataContext dataContext = DataContextHelper.ConnectToTestDatabase (db.DataInfrastructure, enableNullVirtualization: true))
				{
					NaturalPersonEntity albert = dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1000000001)));
					NaturalPersonEntity gertrude = dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1000000002)));

					Assert.IsNotNull (albert);
					Assert.IsFalse (EntityNullReferenceVirtualizer.IsNullEntity (albert));
					Assert.IsTrue (EntityNullReferenceVirtualizer.IsPatchedEntity (albert));
					Assert.IsFalse (EntityNullReferenceVirtualizer.IsPatchedEntityStillUnchanged (albert));

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

					Assert.IsNotNull (language);
					Assert.IsFalse (EntityNullReferenceVirtualizer.IsNullEntity (language));
					Assert.IsTrue (EntityNullReferenceVirtualizer.IsPatchedEntity (language));
					Assert.IsFalse (EntityNullReferenceVirtualizer.IsPatchedEntityStillUnchanged (language));
					Assert.AreEqual ("1337", language.Code);
					Assert.AreEqual ("1337 5|*34|<", language.Name);

					albert.PreferredLanguage = language;

					dataContext.SaveChanges ();
				}

				using (DataContext dataContext = DataContextHelper.ConnectToTestDatabase (db.DataInfrastructure, enableNullVirtualization: true))
				{
					NaturalPersonEntity albert = dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1000000001)));
					NaturalPersonEntity gertrude = dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1000000002)));

					Assert.IsNotNull (albert);
					Assert.IsFalse (EntityNullReferenceVirtualizer.IsNullEntity (albert));
					Assert.IsTrue (EntityNullReferenceVirtualizer.IsPatchedEntity (albert));
					Assert.IsFalse (EntityNullReferenceVirtualizer.IsPatchedEntityStillUnchanged (albert));

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

					Assert.AreSame (albert.PreferredLanguage, language);
					Assert.AreSame (gertrude.PreferredLanguage, language);
				}
			}
		}


		[TestMethod]
		public void CreationAndRemoval()
		{
			using (DB db = DB.ConnectToTestDatabase ())
			{
				using (DataContext dataContext = DataContextHelper.ConnectToTestDatabase (db.DataInfrastructure, enableNullVirtualization: true))
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

					gertrude.PreferredLanguage = null;

					Assert.IsNull (gertrude.PreferredLanguage);

					dataContext.SaveChanges ();

					Assert.IsNull (gertrude.PreferredLanguage);
				}

				using (DataContext dataContext = DataContextHelper.ConnectToTestDatabase (db.DataInfrastructure, enableNullVirtualization: true))
				{
					NaturalPersonEntity gertrude = dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1000000002)));

					Assert.IsNotNull (gertrude);
					Assert.IsFalse (EntityNullReferenceVirtualizer.IsNullEntity (gertrude));
					Assert.IsTrue (EntityNullReferenceVirtualizer.IsPatchedEntity (gertrude));
					Assert.IsFalse (EntityNullReferenceVirtualizer.IsPatchedEntityStillUnchanged (gertrude));

					LanguageEntity languageGertrude = gertrude.PreferredLanguage;

					Assert.IsNotNull (languageGertrude);
					Assert.IsTrue (EntityNullReferenceVirtualizer.IsPatchedEntity (languageGertrude));
					Assert.IsTrue (EntityNullReferenceVirtualizer.IsPatchedEntityStillUnchanged (languageGertrude));
					Assert.IsTrue (EntityNullReferenceVirtualizer.IsNullEntity (languageGertrude));
				}
			}
		}


		[TestMethod]
		public void SetToNull()
		{
			using (DB db = DB.ConnectToTestDatabase ())
			{
				using (DataContext dataContext = DataContextHelper.ConnectToTestDatabase (db.DataInfrastructure, enableNullVirtualization: true))
				{
					NaturalPersonEntity gertrude = dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1000000002)));

					Assert.IsNotNull (gertrude);
					Assert.IsFalse (EntityNullReferenceVirtualizer.IsNullEntity (gertrude));
					Assert.IsTrue (EntityNullReferenceVirtualizer.IsPatchedEntity (gertrude));
					Assert.IsFalse (EntityNullReferenceVirtualizer.IsPatchedEntityStillUnchanged (gertrude));

					// NOTE Here everything seems to happen fine, but under the hood, when we set
					// the value to null, we make a call to get the current value, so that we can
					// trigger the event with the old value. So when we get the current value, as
					// there are no value for the language, we create a "null" language, which is
					// immediately overwritten by the null that we put in the modified value store.
					// But this "null" language is still present in the original value stores. I'm
					// not sure if this is a problem (for the correctness or for the performance),
					// but I don't see how we could correct this behavior without modifying the code
					// generated for the entities.
					// Marc

					gertrude.PreferredLanguage = null;

					Assert.IsNull (gertrude.PreferredLanguage);

					dataContext.SaveChanges ();

					Assert.IsNull (gertrude.PreferredLanguage);
				}

				using (DataContext dataContext = DataContextHelper.ConnectToTestDatabase (db.DataInfrastructure, enableNullVirtualization: true))
				{
					NaturalPersonEntity gertrude = dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1000000002)));

					Assert.IsNotNull (gertrude);
					Assert.IsFalse (EntityNullReferenceVirtualizer.IsNullEntity (gertrude));
					Assert.IsTrue (EntityNullReferenceVirtualizer.IsPatchedEntity (gertrude));
					Assert.IsFalse (EntityNullReferenceVirtualizer.IsPatchedEntityStillUnchanged (gertrude));

					LanguageEntity languageGertrude = gertrude.PreferredLanguage;

					Assert.IsNotNull (languageGertrude);
					Assert.IsTrue (EntityNullReferenceVirtualizer.IsPatchedEntity (languageGertrude));
					Assert.IsTrue (EntityNullReferenceVirtualizer.IsPatchedEntityStillUnchanged (languageGertrude));
					Assert.IsTrue (EntityNullReferenceVirtualizer.IsNullEntity (languageGertrude));
				}
			}
		}


		[TestMethod]
		public void CreationRemovalAndModification()
		{
			using (DB db = DB.ConnectToTestDatabase ())
			{
				using (DataContext dataContext = DataContextHelper.ConnectToTestDatabase (db.DataInfrastructure, enableNullVirtualization: true))
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

					gertrude.PreferredLanguage = null;

					// NOTE Here what happens is that we set the preferred language of Gertrude to
					// null in the modified value store, but there is still the reference to the
					// "null" leet speak language in its original value store. And the value store of
					// the "null" leet speak language still thinks that Gertrude is its parent
					// entity. So when we assign something to a field of the leet speak language,
					// this call goes all the way up to the value store of Gertrude in order to tell
					// that store that the leet speak language is not "null" anymore so that it can
					// remove it from its list of "null" entities and assign it to the real write
					// store. Of course, this is wrong because the leet speak language is not the
					// language of Gertrude anymore.
					// Marc

					Assert.IsNull (gertrude.PreferredLanguage);

					language.Code = "1337";

					Assert.IsNull (gertrude.PreferredLanguage);

					language.Name = "1337 5|*34|<";

					Assert.IsNotNull (language);
					Assert.IsFalse (EntityNullReferenceVirtualizer.IsNullEntity (language));
					Assert.IsTrue (EntityNullReferenceVirtualizer.IsPatchedEntity (language));
					Assert.IsFalse (EntityNullReferenceVirtualizer.IsPatchedEntityStillUnchanged (language));
					Assert.AreEqual ("1337", language.Code);
					Assert.AreEqual ("1337 5|*34|<", language.Name);

					Assert.IsNull (gertrude.PreferredLanguage);

					dataContext.SaveChanges ();

					Assert.IsNull (gertrude.PreferredLanguage);
				}

				using (DataContext dataContext = DataContextHelper.ConnectToTestDatabase (db.DataInfrastructure, enableNullVirtualization: true))
				{
					NaturalPersonEntity gertrude = dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1000000002)));

					Assert.IsNotNull (gertrude);
					Assert.IsFalse (EntityNullReferenceVirtualizer.IsNullEntity (gertrude));
					Assert.IsTrue (EntityNullReferenceVirtualizer.IsPatchedEntity (gertrude));
					Assert.IsFalse (EntityNullReferenceVirtualizer.IsPatchedEntityStillUnchanged (gertrude));

					LanguageEntity languageGertrude = gertrude.PreferredLanguage;

					Assert.IsNotNull (languageGertrude);
					Assert.IsTrue (EntityNullReferenceVirtualizer.IsPatchedEntity (languageGertrude));
					Assert.IsTrue (EntityNullReferenceVirtualizer.IsPatchedEntityStillUnchanged (languageGertrude));
					Assert.IsTrue (EntityNullReferenceVirtualizer.IsNullEntity (languageGertrude));

					Assert.IsNull (languageGertrude.Code);
					Assert.IsNull (languageGertrude.Name);
				}
			}
		}


		[TestMethod]
		public void CreationAssignationAndModification()
		{
			using (DB db = DB.ConnectToTestDatabase ())
			{
				using (DataContext dataContext = DataContextHelper.ConnectToTestDatabase (db.DataInfrastructure, enableNullVirtualization: true))
				{
					NaturalPersonEntity albert = dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1000000001)));
					NaturalPersonEntity gertrude = dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1000000002)));

					Assert.IsNotNull (albert);
					Assert.IsFalse (EntityNullReferenceVirtualizer.IsNullEntity (albert));
					Assert.IsTrue (EntityNullReferenceVirtualizer.IsPatchedEntity (albert));
					Assert.IsFalse (EntityNullReferenceVirtualizer.IsPatchedEntityStillUnchanged (albert));

					Assert.IsNotNull (gertrude);
					Assert.IsFalse (EntityNullReferenceVirtualizer.IsNullEntity (gertrude));
					Assert.IsTrue (EntityNullReferenceVirtualizer.IsPatchedEntity (gertrude));
					Assert.IsFalse (EntityNullReferenceVirtualizer.IsPatchedEntityStillUnchanged (gertrude));

					LanguageEntity language = gertrude.PreferredLanguage;

					Assert.IsNotNull (language);
					Assert.IsTrue (EntityNullReferenceVirtualizer.IsPatchedEntity (language));
					Assert.IsTrue (EntityNullReferenceVirtualizer.IsPatchedEntityStillUnchanged (language));
					Assert.IsTrue (EntityNullReferenceVirtualizer.IsNullEntity (language));

					albert.PreferredLanguage = language;

					language.Code = "1337";
					language.Name = "1337 5|*34|<";

					Assert.IsNotNull (language);
					Assert.IsFalse (EntityNullReferenceVirtualizer.IsNullEntity (language));
					Assert.IsTrue (EntityNullReferenceVirtualizer.IsPatchedEntity (language));
					Assert.IsFalse (EntityNullReferenceVirtualizer.IsPatchedEntityStillUnchanged (language));
					Assert.AreEqual ("1337", language.Code);
					Assert.AreEqual ("1337 5|*34|<", language.Name);

					dataContext.SaveChanges ();
				}

				using (DataContext dataContext = DataContextHelper.ConnectToTestDatabase (db.DataInfrastructure, enableNullVirtualization: true))
				{
					NaturalPersonEntity albert = dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1000000001)));
					NaturalPersonEntity gertrude = dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1000000002)));

					Assert.IsNotNull (albert);
					Assert.IsFalse (EntityNullReferenceVirtualizer.IsNullEntity (albert));
					Assert.IsTrue (EntityNullReferenceVirtualizer.IsPatchedEntity (albert));
					Assert.IsFalse (EntityNullReferenceVirtualizer.IsPatchedEntityStillUnchanged (albert));

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

					Assert.AreSame (albert.PreferredLanguage, language);
					Assert.AreSame (gertrude.PreferredLanguage, language);
				}
			}
		}


		[TestMethod]
		public void CreationAssignationRemovalAndModification()
		{
			using (DB db = DB.ConnectToTestDatabase ())
			{
				using (DataContext dataContext = DataContextHelper.ConnectToTestDatabase (db.DataInfrastructure, enableNullVirtualization: true))
				{
					NaturalPersonEntity albert = dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1000000001)));
					NaturalPersonEntity gertrude = dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1000000002)));

					Assert.IsNotNull (albert);
					Assert.IsFalse (EntityNullReferenceVirtualizer.IsNullEntity (albert));
					Assert.IsTrue (EntityNullReferenceVirtualizer.IsPatchedEntity (albert));
					Assert.IsFalse (EntityNullReferenceVirtualizer.IsPatchedEntityStillUnchanged (albert));

					Assert.IsNotNull (gertrude);
					Assert.IsFalse (EntityNullReferenceVirtualizer.IsNullEntity (gertrude));
					Assert.IsTrue (EntityNullReferenceVirtualizer.IsPatchedEntity (gertrude));
					Assert.IsFalse (EntityNullReferenceVirtualizer.IsPatchedEntityStillUnchanged (gertrude));

					LanguageEntity language = gertrude.PreferredLanguage;

					Assert.IsNotNull (language);
					Assert.IsTrue (EntityNullReferenceVirtualizer.IsPatchedEntity (language));
					Assert.IsTrue (EntityNullReferenceVirtualizer.IsPatchedEntityStillUnchanged (language));
					Assert.IsTrue (EntityNullReferenceVirtualizer.IsNullEntity (language));

					albert.PreferredLanguage = language;

					gertrude.PreferredLanguage = null;

					// NOTE Here what happens is that we set the preferred language of Gertrude to
					// null in the modified value store, but there is still the reference to the
					// "null" leet speak language in its original value store. And the value store of
					// the "null" leet speak language still thinks that Gertrude is its parent
					// entity. So when we assign something to a field of the leet speak language,
					// this call goes all the way up to the value store of Gertrude in order to tell
					// that store that the leet speak language is not "null" anymore so that it can
					// remove it from its list of "null" entities and assign it to the real write
					// store. Of course, this is wrong because the leet speak language is not the
					// language of Gertrude anymore.
					// Marc

					Assert.IsNull (gertrude.PreferredLanguage);

					language.Code = "1337";

					Assert.IsNull (gertrude.PreferredLanguage);

					language.Name = "1337 5|*34|<";

					Assert.IsNotNull (language);
					Assert.IsFalse (EntityNullReferenceVirtualizer.IsNullEntity (language));
					Assert.IsTrue (EntityNullReferenceVirtualizer.IsPatchedEntity (language));
					Assert.IsFalse (EntityNullReferenceVirtualizer.IsPatchedEntityStillUnchanged (language));
					Assert.AreEqual ("1337", language.Code);
					Assert.AreEqual ("1337 5|*34|<", language.Name);

					Assert.IsNull (gertrude.PreferredLanguage);

					dataContext.SaveChanges ();

					Assert.IsNull (gertrude.PreferredLanguage);
				}

				using (DataContext dataContext = DataContextHelper.ConnectToTestDatabase (db.DataInfrastructure, enableNullVirtualization: true))
				{
					NaturalPersonEntity albert = dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1000000001)));
					NaturalPersonEntity gertrude = dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1000000002)));

					Assert.IsNotNull (albert);
					Assert.IsFalse (EntityNullReferenceVirtualizer.IsNullEntity (albert));
					Assert.IsTrue (EntityNullReferenceVirtualizer.IsPatchedEntity (albert));
					Assert.IsFalse (EntityNullReferenceVirtualizer.IsPatchedEntityStillUnchanged (albert));

					Assert.IsNotNull (gertrude);
					Assert.IsFalse (EntityNullReferenceVirtualizer.IsNullEntity (gertrude));
					Assert.IsTrue (EntityNullReferenceVirtualizer.IsPatchedEntity (gertrude));
					Assert.IsFalse (EntityNullReferenceVirtualizer.IsPatchedEntityStillUnchanged (gertrude));

					LanguageEntity languageAlbert = dataContext.ResolveEntity<LanguageEntity> (new DbKey (new DbId (1000000003)));

					Assert.IsNotNull (languageAlbert);
					Assert.IsFalse (EntityNullReferenceVirtualizer.IsNullEntity (languageAlbert));
					Assert.IsTrue (EntityNullReferenceVirtualizer.IsPatchedEntity (languageAlbert));
					Assert.IsFalse (EntityNullReferenceVirtualizer.IsPatchedEntityStillUnchanged (languageAlbert));
					Assert.AreEqual ("1337", languageAlbert.Code);
					Assert.AreEqual ("1337 5|*34|<", languageAlbert.Name);

					Assert.AreSame (albert.PreferredLanguage, languageAlbert);

					LanguageEntity languageGertrude = gertrude.PreferredLanguage;

					Assert.IsNotNull (languageGertrude);
					Assert.IsTrue (EntityNullReferenceVirtualizer.IsPatchedEntity (languageGertrude));
					Assert.IsTrue (EntityNullReferenceVirtualizer.IsPatchedEntityStillUnchanged (languageGertrude));
					Assert.IsTrue (EntityNullReferenceVirtualizer.IsNullEntity (languageGertrude));

					Assert.IsNull (languageGertrude.Code);
					Assert.IsNull (languageGertrude.Name);
				}
			}
		}


		[TestMethod]
		public void UnwrapNullEntityTest()
		{
			using (DB db = DB.ConnectToTestDatabase ())
			{
				using (DataContext dataContext = DataContextHelper.ConnectToTestDatabase (db.DataInfrastructure, enableNullVirtualization: true))
				{
					NaturalPersonEntity gertrude = dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1000000002)));
					LanguageEntity language = gertrude.PreferredLanguage;
					NaturalPersonEntity noOne = null;

					Assert.AreSame (gertrude, dataContext.UnwrapNullEntity (gertrude));
					Assert.IsNull (dataContext.UnwrapNullEntity (language));
					Assert.IsNull (dataContext.UnwrapNullEntity (noOne));

					language.Code = "1337";

					Assert.AreSame (language, dataContext.UnwrapNullEntity (language));
				}
			}
		}


		[TestMethod]
		public void WrapNullEntityTest()
		{
			using (DB db = DB.ConnectToTestDatabase ())
			{
				using (DataContext dataContext = DataContextHelper.ConnectToTestDatabase (db.DataInfrastructure, enableNullVirtualization: true))
				{
					NaturalPersonEntity gertrude = dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1000000002)));
					LanguageEntity language = gertrude.PreferredLanguage;
					NaturalPersonEntity noOne = null;

					NaturalPersonEntity wrapedGertrude = dataContext.WrapNullEntity (gertrude);
					LanguageEntity wrapedLanguage = dataContext.WrapNullEntity (language);
					NaturalPersonEntity wrapedNoOne = dataContext.WrapNullEntity (noOne);

					Assert.AreSame (gertrude, wrapedGertrude);
					Assert.AreSame (language, wrapedLanguage);

					Assert.IsNotNull (wrapedNoOne);
					Assert.IsTrue (EntityNullReferenceVirtualizer.IsPatchedEntity (wrapedNoOne));
					Assert.IsTrue (EntityNullReferenceVirtualizer.IsPatchedEntityStillUnchanged (wrapedNoOne));
					Assert.IsTrue (EntityNullReferenceVirtualizer.IsNullEntity (wrapedNoOne));

					wrapedNoOne.Firstname = "Blupi";
					wrapedNoOne.Lastname = "Mania";

					dataContext.SaveChanges ();
				}

				using (DataContext dataContext = DataContextHelper.ConnectToTestDatabase (db.DataInfrastructure, enableNullVirtualization: true))
				{
					NaturalPersonEntity person = dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1000000004)));

					Assert.IsNotNull (person);
					Assert.IsTrue (EntityNullReferenceVirtualizer.IsPatchedEntity (person));
					Assert.IsFalse (EntityNullReferenceVirtualizer.IsPatchedEntityStillUnchanged (person));
					Assert.IsFalse (EntityNullReferenceVirtualizer.IsNullEntity (person));

					Assert.AreEqual ("Blupi", person.Firstname);
					Assert.AreEqual ("Mania", person.Lastname);
				}
			}
		}


		[TestMethod]
		public void CreateNullEntityAndModify()
		{
			using (DB db = DB.ConnectToTestDatabase ())
			{
				using (DataContext dataContext = DataContextHelper.ConnectToTestDatabase (db.DataInfrastructure, enableNullVirtualization: true))
				{
					NaturalPersonEntity person = dataContext.CreateNullEntity<NaturalPersonEntity> (freeze: false);

					Assert.IsNotNull (person);
					Assert.IsTrue (EntityNullReferenceVirtualizer.IsPatchedEntity (person));
					Assert.IsTrue (EntityNullReferenceVirtualizer.IsPatchedEntityStillUnchanged (person));
					Assert.IsTrue (EntityNullReferenceVirtualizer.IsNullEntity (person));

					person.Firstname = "Blupi";
					person.Lastname = "Mania";

					Assert.IsTrue (EntityNullReferenceVirtualizer.IsPatchedEntity (person));
					Assert.IsFalse (EntityNullReferenceVirtualizer.IsPatchedEntityStillUnchanged (person));
					Assert.IsFalse (EntityNullReferenceVirtualizer.IsNullEntity (person));

					dataContext.SaveChanges ();
				}

				using (DataContext dataContext = DataContextHelper.ConnectToTestDatabase (db.DataInfrastructure, enableNullVirtualization: true))
				{
					NaturalPersonEntity person = dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1000000004)));

					Assert.IsNotNull (person);
					Assert.IsTrue (EntityNullReferenceVirtualizer.IsPatchedEntity (person));
					Assert.IsFalse (EntityNullReferenceVirtualizer.IsPatchedEntityStillUnchanged (person));
					Assert.IsFalse (EntityNullReferenceVirtualizer.IsNullEntity (person));

					Assert.AreEqual ("Blupi", person.Firstname);
					Assert.AreEqual ("Mania", person.Lastname);
				}
			}
		}


		[TestMethod]
		public void CreateNullEntityAndDiscard()
		{
			using (DB db = DB.ConnectToTestDatabase ())
			{
				using (DataContext dataContext = DataContextHelper.ConnectToTestDatabase (db.DataInfrastructure, enableNullVirtualization: true))
				{
					NaturalPersonEntity person = dataContext.CreateNullEntity<NaturalPersonEntity> (freeze: false);

					Assert.IsNotNull (person);
					Assert.IsTrue (EntityNullReferenceVirtualizer.IsPatchedEntity (person));
					Assert.IsTrue (EntityNullReferenceVirtualizer.IsPatchedEntityStillUnchanged (person));
					Assert.IsTrue (EntityNullReferenceVirtualizer.IsNullEntity (person));

					dataContext.SaveChanges ();
				}

				using (DataContext dataContext = DataContextHelper.ConnectToTestDatabase (db.DataInfrastructure, enableNullVirtualization: true))
				{
					NaturalPersonEntity person = dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1000000004)));

					Assert.IsNull (person);
				}
			}
		}


		[TestMethod]
		public void CreateReadOnlyNullEntity()
		{
			using (DB db = DB.ConnectToTestDatabase ())
			{
				using (DataContext dataContext = DataContextHelper.ConnectToTestDatabase (db.DataInfrastructure, enableNullVirtualization: true))
				{
					NaturalPersonEntity person = dataContext.CreateNullEntity<NaturalPersonEntity> (freeze: true);

					Assert.IsNotNull (person);
					Assert.IsTrue (EntityNullReferenceVirtualizer.IsPatchedEntity (person));
					Assert.IsTrue (EntityNullReferenceVirtualizer.IsPatchedEntityStillUnchanged (person));
					Assert.IsTrue (EntityNullReferenceVirtualizer.IsNullEntity (person));

					Assert.IsTrue (person.IsReadOnly);

					ExceptionAssert.Throw<Epsitec.Common.Types.Exceptions.ReadOnlyException>
					(
						() => person.Firstname = "coucou"
					);

					dataContext.SaveChanges ();
				}

				using (DataContext dataContext = DataContextHelper.ConnectToTestDatabase (db.DataInfrastructure, enableNullVirtualization: true))
				{
					NaturalPersonEntity person = dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1000000004)));

					Assert.IsNull (person);
				}
			}
		}


	}


}
