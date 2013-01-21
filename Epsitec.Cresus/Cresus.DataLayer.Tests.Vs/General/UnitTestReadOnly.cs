using Epsitec.Common.Support.EntityEngine;

using Epsitec.Common.Types.Exceptions;

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
	public sealed class UnitTestReadOnly
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
		public void ReadOnlyDataContextEntities1()
		{			
			using (DB db = DB.ConnectToTestDatabase ())
			using (DataContext dataContext = DataContextHelper.ConnectToTestDatabase (db.DataInfrastructure, readOnly:true))
			{
				NaturalPersonEntity alfred = dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1000000001)));

				Assert.IsTrue (alfred.IsReadOnly);
				Assert.IsTrue (alfred.PreferredLanguage.IsReadOnly);
				Assert.IsTrue (alfred.Gender.IsReadOnly);
				Assert.IsTrue (alfred.Contacts[0].IsReadOnly);
				Assert.IsTrue (alfred.Contacts[1].IsReadOnly);
				Assert.IsTrue (((UriContactEntity) alfred.Contacts[0]).UriScheme.IsReadOnly);
				Assert.IsTrue (((UriContactEntity) alfred.Contacts[1]).UriScheme.IsReadOnly);
			}
		}


		[TestMethod]
		public void ReadOnlyDataContextEntities2()
		{			
			using (DB db = DB.ConnectToTestDatabase ())
			using (DataContext dataContext1 = DataContextHelper.ConnectToTestDatabase (db.DataInfrastructure, readOnly: true))
			using (DataContext dataContext2 = DataContextHelper.ConnectToTestDatabase (db.DataInfrastructure, readOnly: true))
			{
				NaturalPersonEntity alfred1 = dataContext1.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1000000001)));

				NaturalPersonEntity alfred2 = DataContext.CopyEntity (dataContext1, alfred1, dataContext2);

				Assert.IsTrue (alfred2.IsReadOnly);
			}
		}


		[TestMethod]
		public void ReadOnlyDataContextExceptions1()
		{
			using (DB db = DB.ConnectToTestDatabase ())
			using (DataContext dataContext = DataContextHelper.ConnectToTestDatabase (db.DataInfrastructure, readOnly: true))
			{
				ExceptionAssert.Throw<ReadOnlyException>
				(
					() => dataContext.CreateEntity<NaturalPersonEntity> ()
				);

				ExceptionAssert.Throw<ReadOnlyException>
				(
					() => dataContext.CreateEntity (EntityInfo<NaturalPersonEntity>.GetTypeId ())
				);

				ExceptionAssert.Throw<ReadOnlyException>
				(
					() => dataContext.CreateEntityAndRegisterAsEmpty<NaturalPersonEntity> ()
				);

				ExceptionAssert.Throw<ReadOnlyException>
				(
					() => dataContext.SaveChanges ()
				);
			}
		}


		[TestMethod]
		public void ReadOnlyDataContextExceptions2()
		{			
			using (DB db = DB.ConnectToTestDatabase ())
			using (DataContext dataContext = DataContextHelper.ConnectToTestDatabase (db.DataInfrastructure, readOnly: true))
			{
				NaturalPersonEntity alfred = dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1000000001)));

				ExceptionAssert.Throw<ReadOnlyException>
				(
					() => dataContext.RegisterEmptyEntity (alfred)
				);

				ExceptionAssert.Throw<ReadOnlyException>
				(
					() => dataContext.UnregisterEmptyEntity (alfred)
				);

				ExceptionAssert.Throw<ReadOnlyException>
				(
					() => dataContext.DeleteEntity (alfred)
				);
			}
		}


		[TestMethod]
		public void ReadOnlyDataContextExceptions3()
		{			
			using (DB db = DB.ConnectToTestDatabase ())
			using (DataContext dataContext1 = DataContextHelper.ConnectToTestDatabase (db.DataInfrastructure, readOnly: false))
			using (DataContext dataContext2 = DataContextHelper.ConnectToTestDatabase (db.DataInfrastructure, readOnly: true))
			{
				NaturalPersonEntity alfred = dataContext1.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1000000001)));

				ExceptionAssert.Throw<ReadOnlyException>
				(
					() => DataContext.CopyEntity (dataContext1, alfred, dataContext2)
				);
			}
		}


		[TestMethod]
		public void ReadOnlyDataContextExceptions4()
		{			
			using (DB db = DB.ConnectToTestDatabase ())
			using (DataContext dataContext = DataContextHelper.ConnectToTestDatabase (db.DataInfrastructure, readOnly: false))
			{
				NaturalPersonEntity alfred = dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1000000001)));
				NaturalPersonEntity albert = dataContext.CreateEntity<NaturalPersonEntity> ();

				alfred.Freeze ();
				albert.Freeze ();

				ExceptionAssert.Throw<ReadOnlyException>
				(
					() => dataContext.RegisterEmptyEntity (albert)
				);

				ExceptionAssert.Throw<ReadOnlyException>
				(
					() => dataContext.UnregisterEmptyEntity (albert)
				);

				ExceptionAssert.Throw<ReadOnlyException>
				(
					() => dataContext.UpdateEmptyEntityStatus (albert, true)
				);

				ExceptionAssert.Throw<ReadOnlyException>
				(
					() => dataContext.DeleteEntity (alfred)
				);
			}
		}


		[TestMethod]
		public void ValueSynchronizationBackDoor()
		{			
			using (DB db = DB.ConnectToTestDatabase ())
			using (DataContext dataContext1 = DataContextHelper.ConnectToTestDatabase (db.DataInfrastructure, readOnly: false))
			using (DataContext dataContext2 = DataContextHelper.ConnectToTestDatabase (db.DataInfrastructure, readOnly: true))
			{
				NaturalPersonEntity alfred1 = dataContext1.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1000000001)));
				NaturalPersonEntity alfred2 = dataContext2.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1000000001)));

				const string oldLastName = "Dupond";
				const string newLastName = "New last name";
				
				Assert.AreEqual (oldLastName, alfred1.Lastname);
				Assert.AreEqual (oldLastName, alfred2.Lastname);

				alfred1.Lastname = newLastName;

				Assert.AreEqual (newLastName, alfred1.Lastname);
				Assert.AreEqual (oldLastName, alfred2.Lastname);

				dataContext1.SaveChanges ();

				Assert.AreEqual (newLastName, alfred1.Lastname);
				Assert.AreEqual (newLastName, alfred2.Lastname);
			}
		}


		[TestMethod]
		public void ReferenceSynchronizationBackDoor()
		{			
			using (DB db = DB.ConnectToTestDatabase ())
			using (DataContext dataContext1 = DataContextHelper.ConnectToTestDatabase (db.DataInfrastructure, readOnly: false))
			using (DataContext dataContext2 = DataContextHelper.ConnectToTestDatabase (db.DataInfrastructure, readOnly: true))
			{
				NaturalPersonEntity alfred1 = dataContext1.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1000000001)));
				NaturalPersonEntity alfred2 = dataContext2.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1000000001)));

				LanguageEntity french1 = dataContext1.ResolveEntity<LanguageEntity> (new DbKey (new DbId (1000000001)));
				LanguageEntity french2 = dataContext2.ResolveEntity<LanguageEntity> (new DbKey (new DbId (1000000001)));

				LanguageEntity german1 = dataContext1.ResolveEntity<LanguageEntity> (new DbKey (new DbId (1000000002)));
				LanguageEntity german2 = dataContext2.ResolveEntity<LanguageEntity> (new DbKey (new DbId (1000000002)));

				Assert.AreEqual (french1, alfred1.PreferredLanguage);
				Assert.AreEqual (french2, alfred2.PreferredLanguage);

				alfred1.PreferredLanguage = german1;

				Assert.AreEqual (german1, alfred1.PreferredLanguage);
				Assert.AreEqual (french2, alfred2.PreferredLanguage);

				dataContext1.SaveChanges ();

				Assert.AreEqual (german1, alfred1.PreferredLanguage);
				Assert.AreEqual (german2, alfred2.PreferredLanguage);
			}
		}


		[TestMethod]
		public void CollectionSynchronizationBackDoor()
		{			
			using (DB db = DB.ConnectToTestDatabase ())
			using (DataContext dataContext1 = DataContextHelper.ConnectToTestDatabase (db.DataInfrastructure, readOnly: false))
			using (DataContext dataContext2 = DataContextHelper.ConnectToTestDatabase (db.DataInfrastructure, readOnly: true))
			{
				NaturalPersonEntity alfred1 = dataContext1.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1000000001)));
				NaturalPersonEntity alfred2 = dataContext2.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1000000001)));

				AbstractContactEntity contactA1 = dataContext1.ResolveEntity<AbstractContactEntity> (new DbKey (new DbId (1000000001)));
				AbstractContactEntity contactA2 = dataContext2.ResolveEntity<AbstractContactEntity> (new DbKey (new DbId (1000000001)));

				AbstractContactEntity contactB1 = dataContext1.ResolveEntity<AbstractContactEntity> (new DbKey (new DbId (1000000002)));
				AbstractContactEntity contactB2 = dataContext2.ResolveEntity<AbstractContactEntity> (new DbKey (new DbId (1000000002)));

				Assert.AreEqual (2, alfred1.Contacts.Count);
				Assert.AreEqual (contactA1, alfred1.Contacts[0]);
				Assert.AreEqual (contactB1, alfred1.Contacts[1]);

				Assert.AreEqual (2, alfred2.Contacts.Count);
				Assert.AreEqual (contactA2, alfred2.Contacts[0]);
				Assert.AreEqual (contactB2, alfred2.Contacts[1]);

				alfred1.Contacts[0] = contactB1;
				alfred1.Contacts[1] = contactA1;

				Assert.AreEqual (2, alfred1.Contacts.Count);
				Assert.AreEqual (contactB1, alfred1.Contacts[0]);
				Assert.AreEqual (contactA1, alfred1.Contacts[1]);

				Assert.AreEqual (2, alfred2.Contacts.Count);
				Assert.AreEqual (contactA2, alfred2.Contacts[0]);
				Assert.AreEqual (contactB2, alfred2.Contacts[1]);

				dataContext1.SaveChanges ();

				Assert.AreEqual (2, alfred1.Contacts.Count);
				Assert.AreEqual (contactB1, alfred1.Contacts[0]);
				Assert.AreEqual (contactA1, alfred1.Contacts[1]);

				Assert.AreEqual (2, alfred2.Contacts.Count);
				Assert.AreEqual (contactB2, alfred2.Contacts[0]);
				Assert.AreEqual (contactA2, alfred2.Contacts[1]);
			}
		}


		[TestMethod]
		public void DeleteSynchronizationBackDoor()
		{			
			using (DB db = DB.ConnectToTestDatabase ())
			using (DataContext dataContext1 = DataContextHelper.ConnectToTestDatabase (db.DataInfrastructure, readOnly: false))
			using (DataContext dataContext2 = DataContextHelper.ConnectToTestDatabase (db.DataInfrastructure, readOnly: true))
			{
				NaturalPersonEntity alfred1 = dataContext1.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1000000001)));
				NaturalPersonEntity alfred2 = dataContext2.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1000000001)));

				AbstractContactEntity contact1 = alfred1.Contacts[0];
				AbstractContactEntity contact2 = alfred2.Contacts[0];

				Assert.IsFalse (dataContext1.IsDeleted (alfred1));
				Assert.IsFalse (dataContext2.IsDeleted (alfred2));

				Assert.AreEqual (alfred1, contact1.NaturalPerson);
				Assert.AreEqual (alfred2, contact2.NaturalPerson);

				dataContext1.DeleteEntity (alfred1);

				Assert.IsTrue (dataContext1.IsDeleted (alfred1));
				Assert.IsFalse (dataContext2.IsDeleted (alfred2));

				Assert.AreEqual (alfred1, contact1.NaturalPerson);
				Assert.AreEqual (alfred2, contact2.NaturalPerson);

				dataContext1.SaveChanges ();

				Assert.IsTrue (dataContext1.IsDeleted (alfred1));
				Assert.IsTrue (dataContext2.IsDeleted (alfred2));

				Assert.IsNull (contact1.NaturalPerson);
				Assert.IsNull (contact2.NaturalPerson);
			}
		}


		[TestMethod]
		public void DeletePropagationBackDoor()
		{			
			using (DB db = DB.ConnectToTestDatabase ())
			using (DataContext dataContext = DataContextHelper.ConnectToTestDatabase (db.DataInfrastructure, readOnly: false))
			{
				NaturalPersonEntity alfred = dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1000000001)));
				AbstractContactEntity contact = alfred.Contacts[0];

				contact.Freeze ();

				Assert.IsFalse (dataContext.IsDeleted (alfred));
				Assert.AreEqual (alfred, contact.NaturalPerson);

				dataContext.DeleteEntity (alfred);

				Assert.IsTrue (dataContext.IsDeleted (alfred));
				Assert.AreEqual (alfred, contact.NaturalPerson);

				dataContext.SaveChanges ();

				Assert.IsTrue (dataContext.IsDeleted (alfred));
				Assert.IsNull (contact.NaturalPerson);
			}
		}


		[TestMethod]
		public void ReloadBackDoor()
		{
			using (DB db1 = DB.ConnectToTestDatabase ())
			using (DB db2 = DB.ConnectToTestDatabase ())
			using (DataContext dataContext1 = DataContextHelper.ConnectToTestDatabase (db1.DataInfrastructure, readOnly: false))
			using (DataContext dataContext2 = DataContextHelper.ConnectToTestDatabase (db2.DataInfrastructure, readOnly: true))
			{
				NaturalPersonEntity alfred1 = dataContext1.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1000000001)));
				NaturalPersonEntity alfred2 = dataContext2.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1000000001)));

				const string name1 = "Dupond";
				const string name2 = "New last name";
				
				Assert.AreEqual (name1, alfred1.Lastname);
				Assert.AreEqual (name1, alfred2.Lastname);

				alfred1.Lastname = name2;

				Assert.AreEqual (name2, alfred1.Lastname);
				Assert.AreEqual (name1, alfred2.Lastname);

				alfred2.Freeze ();

				Assert.AreEqual (name2, alfred1.Lastname);
				Assert.AreEqual (name1, alfred2.Lastname);

				dataContext1.SaveChanges ();

				Assert.AreEqual (name2, alfred1.Lastname);
				Assert.AreEqual (name1, alfred2.Lastname);

				dataContext2.Reload ();

				Assert.AreEqual (name2, alfred1.Lastname);
				Assert.AreEqual (name2, alfred2.Lastname);
			}
		}


	}


}

