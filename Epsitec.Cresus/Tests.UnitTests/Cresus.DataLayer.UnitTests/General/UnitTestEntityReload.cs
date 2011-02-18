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
			
			DatabaseCreator2.ResetPopulatedTestDatabase ();
		}


		[TestMethod]
		public void ReloadValue()
		{
			using (DbInfrastructure dbInfrastructure = DbInfrastructureHelper.ConnectToTestDatabase ())
			using (DataInfrastructure dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase (dbInfrastructure))
			using (DataContext dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
			{
				NaturalPersonEntity alfred = dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1000000001)));

				Assert.AreEqual ("Alfred", alfred.Firstname);

				alfred.Firstname = "Albert";

				Assert.AreEqual ("Albert", alfred.Firstname);

				dataContext.ReloadEntity (alfred);

				Assert.AreEqual ("Alfred", alfred.Firstname);
			}
		}


		[TestMethod]
		public void ReloadReference()
		{
			using (DbInfrastructure dbInfrastructure = DbInfrastructureHelper.ConnectToTestDatabase ())
			using (DataInfrastructure dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase (dbInfrastructure))
			using (DataContext dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
			{
				NaturalPersonEntity alfred = dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1000000001)));
				LanguageEntity french = dataContext.ResolveEntity<LanguageEntity> (new DbKey (new DbId (1000000001)));
				LanguageEntity german = dataContext.ResolveEntity<LanguageEntity> (new DbKey (new DbId (1000000001)));

				Assert.AreSame (french, alfred.PreferredLanguage);

				alfred.PreferredLanguage = german;

				Assert.AreSame (german, alfred.PreferredLanguage);

				dataContext.ReloadEntity (alfred);

				Assert.AreSame (french, alfred.PreferredLanguage);
			}
		}


		[TestMethod]
		public void ReloadCollection()
		{
			using (DbInfrastructure dbInfrastructure = DbInfrastructureHelper.ConnectToTestDatabase ())
			using (DataInfrastructure dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase (dbInfrastructure))
			using (DataContext dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
			{
				NaturalPersonEntity alfred = dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1000000001)));
				UriContactEntity contact1 = dataContext.ResolveEntity<UriContactEntity> (new DbKey (new DbId (1000000001)));
				UriContactEntity contact2 = dataContext.ResolveEntity<UriContactEntity> (new DbKey (new DbId (1000000002)));

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


		[TestMethod]
		public void ReloadDeletedEntity()
		{
			// Here we need two DataInfrastructures, otherwise both DataContext will be synchronized
			// and we don't want that. Therefore, we also require two DbInfrastructures, because they
			// are tightly coupled with the DataInfrastructures.
			// Marc

			using (DbInfrastructure dbInfrastructure1 = DbInfrastructureHelper.ConnectToTestDatabase ())
			using (DbInfrastructure dbInfrastructure2 = DbInfrastructureHelper.ConnectToTestDatabase ())
			using (DataInfrastructure dataInfrastructure1 = DataInfrastructureHelper.ConnectToTestDatabase (dbInfrastructure1))
			using (DataInfrastructure dataInfrastructure2 = DataInfrastructureHelper.ConnectToTestDatabase (dbInfrastructure2))
			using (DataContext dataContext1 = DataContextHelper.ConnectToTestDatabase (dataInfrastructure1))
			using (DataContext dataContext2 = DataContextHelper.ConnectToTestDatabase (dataInfrastructure2))
			{
				UriContactEntity contact1 = dataContext1.CreateEntity<UriContactEntity> ();

				contact1.Uri = "coucou@blabla.com";

				dataContext1.SaveChanges ();

				UriContactEntity contact2 = dataContext2.ResolveEntity<UriContactEntity> (new DbKey (new DbId (1000000005)));

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


		[TestMethod]
		public void ReloadFieldValue()
		{
			using (DbInfrastructure dbInfrastructure = DbInfrastructureHelper.ConnectToTestDatabase ())
			using (DataInfrastructure dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase (dbInfrastructure))
			using (DataContext dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
			{
				NaturalPersonEntity alfred = dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1000000001)));

				Assert.AreEqual ("Alfred", alfred.Firstname);

				alfred.Firstname = "Albert";

				Assert.AreEqual ("Albert", alfred.Firstname);

				dataContext.ReloadEntityField (alfred, Druid.Parse ("[J1AL1]"));

				Assert.AreEqual ("Alfred", alfred.Firstname);
			}
		}


		[TestMethod]
		public void ReloadFieldReference()
		{
			using (DbInfrastructure dbInfrastructure = DbInfrastructureHelper.ConnectToTestDatabase ())
			using (DataInfrastructure dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase (dbInfrastructure))
			using (DataContext dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
			{
				NaturalPersonEntity alfred = dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1000000001)));
				LanguageEntity french = dataContext.ResolveEntity<LanguageEntity> (new DbKey (new DbId (1000000001)));
				LanguageEntity german = dataContext.ResolveEntity<LanguageEntity> (new DbKey (new DbId (1000000001)));

				Assert.AreSame (french, alfred.PreferredLanguage);

				alfred.PreferredLanguage = german;

				Assert.AreSame (german, alfred.PreferredLanguage);

				dataContext.ReloadEntityField (alfred, Druid.Parse ("[J1AD1]"));

				Assert.AreSame (french, alfred.PreferredLanguage);
			}
		}


		[TestMethod]
		public void ReloadCollectionField()
		{
			using (DbInfrastructure dbInfrastructure = DbInfrastructureHelper.ConnectToTestDatabase ())
			using (DataInfrastructure dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase (dbInfrastructure))
			using (DataContext dataContext = DataContextHelper.ConnectToTestDatabase (dataInfrastructure))
			{
				NaturalPersonEntity alfred = dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1000000001)));
				UriContactEntity contact1 = dataContext.ResolveEntity<UriContactEntity> (new DbKey (new DbId (1000000001)));
				UriContactEntity contact2 = dataContext.ResolveEntity<UriContactEntity> (new DbKey (new DbId (1000000002)));

				Assert.AreEqual (2, alfred.Contacts.Count);
				Assert.AreSame (contact1, alfred.Contacts[0]);
				Assert.AreSame (contact2, alfred.Contacts[1]);

				alfred.Contacts.Clear ();

				Assert.AreEqual (0, alfred.Contacts.Count);

				dataContext.ReloadEntityField (alfred, Druid.Parse ("[J1AC1]"));

				Assert.AreEqual (2, alfred.Contacts.Count);
				Assert.AreSame (contact1, alfred.Contacts[0]);
				Assert.AreSame (contact2, alfred.Contacts[1]);
			}
		}


	}


}
