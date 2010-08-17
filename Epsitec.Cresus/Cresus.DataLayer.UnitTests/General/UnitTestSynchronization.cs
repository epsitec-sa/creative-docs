﻿using Epsitec.Common.Types;

using Epsitec.Cresus.Database;

using Epsitec.Cresus.DataLayer.Context;
using Epsitec.Cresus.DataLayer.UnitTests.Entities;
using Epsitec.Cresus.DataLayer.UnitTests.Helpers;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.Collections.Generic;
using System.Linq;


namespace Epsitec.Cresus.DataLayer.UnitTests.General
{


	[TestClass]
	public sealed class UnitTestSynchronization
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

			using (DataContext dataContext = new DataContext (DatabaseHelper.DbInfrastructure))
			{
				DatabaseCreator2.PupulateDatabase (dataContext);
			}
		}


		[TestMethod]
		public void TestUpdateValue()
		{
			int nbDataContexts = 10;

			List<DataContext> dataContexts = new List<DataContext> ();

			for (int i = 0; i < nbDataContexts; i++)
			{
				DataContext dataContext = new DataContext (DatabaseHelper.DbInfrastructure);

				dataContexts.Add (dataContext);
				DataContextPool.Instance.Add (dataContext);
			}

			DbKey dbKey = new DbKey (new DbId (1));

			List<NaturalPersonEntity> naturalPersons = new List<NaturalPersonEntity> ();

			for (int i = 0; i < nbDataContexts; i++)
			{
				naturalPersons.Add (dataContexts[i].ResolveEntity<NaturalPersonEntity> (dbKey));
			}

			string firstName1 = "Alfred";
			string firstName2 = "Albert";

			for (int i = 0; i < nbDataContexts; i++)
			{
				Assert.AreEqual (firstName1, naturalPersons[i].Firstname);
			}

			naturalPersons.First ().Firstname = firstName2;
			dataContexts.First ().SaveChanges ();

			for (int i = 0; i < nbDataContexts; i++)
			{
				Assert.AreEqual (firstName2, naturalPersons[i].Firstname);
			}

			for (int i = 0; i < nbDataContexts; i++)
			{
				DataContext dataContext = dataContexts[i];

				DataContextPool.Instance.Remove (dataContext);
				dataContext.Dispose ();
			}
		}


		[TestMethod]
		public void TestUpdateReference()
		{
			int nbDataContexts = 10;

			List<DataContext> dataContexts = new List<DataContext> ();

			for (int i = 0; i < nbDataContexts; i++)
			{
				DataContext dataContext = new DataContext (DatabaseHelper.DbInfrastructure);

				dataContexts.Add (dataContext);
				DataContextPool.Instance.Add (dataContext);
			}

			DbKey dbKey = new DbKey (new DbId (1));

			List<NaturalPersonEntity> naturalPersons = new List<NaturalPersonEntity> ();

			for (int i = 0; i < nbDataContexts; i++)
			{
				naturalPersons.Add (dataContexts[i].ResolveEntity<NaturalPersonEntity> (dbKey));
			}

			string gender1 = "Male";
			string gender2 = "Female";

			for (int i = 0; i < nbDataContexts; i++)
			{
				Assert.AreEqual (gender1, naturalPersons[i].Gender.Name);
			}

			DbKey newGenderKey = new DbKey (new DbId (2));
			PersonGenderEntity newGender = dataContexts.First ().ResolveEntity<PersonGenderEntity> (newGenderKey);

			naturalPersons.First ().Gender = newGender;
			dataContexts.First ().SaveChanges ();

			for (int i = 0; i < nbDataContexts; i++)
			{
				Assert.AreEqual (gender2, naturalPersons[i].Gender.Name);
			}

			for (int i = 0; i < nbDataContexts; i++)
			{
				DataContext dataContext = dataContexts[i];

				DataContextPool.Instance.Remove (dataContext);
				dataContext.Dispose ();
			}
		}


		[TestMethod]
		public void TestUpdateCollection()
		{
			int nbDataContexts = 10;

			List<DataContext> dataContexts = new List<DataContext> ();

			for (int i = 0; i < nbDataContexts; i++)
			{
				DataContext dataContext = new DataContext (DatabaseHelper.DbInfrastructure);

				dataContexts.Add (dataContext);
				DataContextPool.Instance.Add (dataContext);
			}

			DbKey dbKey = new DbKey (new DbId (1));

			List<NaturalPersonEntity> naturalPersons = new List<NaturalPersonEntity> ();

			for (int i = 0; i < nbDataContexts; i++)
			{
				naturalPersons.Add (dataContexts[i].ResolveEntity<NaturalPersonEntity> (dbKey));
			}

			string contact1 = "alfred@coucou.com";
			string contact2 = "alfred@blabla.com";

			for (int i = 0; i < nbDataContexts; i++)
			{
				Assert.AreEqual (contact1, ((UriContactEntity) naturalPersons[i].Contacts[0]).Uri);
				Assert.AreEqual (contact2, ((UriContactEntity) naturalPersons[i].Contacts[1]).Uri);
			}

			NaturalPersonEntity naturalPerson1 = naturalPersons.First ();
			UriContactEntity tmpContact = ((UriContactEntity) naturalPerson1.Contacts[0]);
			naturalPerson1.Contacts[0] = naturalPerson1.Contacts[1];
			naturalPerson1.Contacts[1] = tmpContact;

			dataContexts.First ().SaveChanges ();

			for (int i = 0; i < nbDataContexts; i++)
			{
				Assert.AreEqual (contact1, ((UriContactEntity) naturalPersons[i].Contacts[1]).Uri);
				Assert.AreEqual (contact2, ((UriContactEntity) naturalPersons[i].Contacts[0]).Uri);
			}

			for (int i = 0; i < nbDataContexts; i++)
			{
				DataContext dataContext = dataContexts[i];

				DataContextPool.Instance.Remove (dataContext);
				dataContext.Dispose ();
			}
		}


		[TestMethod]
		public void TestDeleteEntityReference()
		{
			int nbDataContexts = 10;

			List<DataContext> dataContexts = new List<DataContext> ();

			for (int i = 0; i < nbDataContexts; i++)
			{
				DataContext dataContext = new DataContext (DatabaseHelper.DbInfrastructure);

				dataContexts.Add (dataContext);
				DataContextPool.Instance.Add (dataContext);
			}

			DbKey dbKey = new DbKey (new DbId (1));

			List<NaturalPersonEntity> naturalPersons = new List<NaturalPersonEntity> ();

			for (int i = 0; i < nbDataContexts; i++)
			{
				naturalPersons.Add (dataContexts[i].ResolveEntity<NaturalPersonEntity> (dbKey));
			}

			for (int i = 0; i < nbDataContexts; i++)
			{
				Assert.AreEqual ("Male", naturalPersons[i].Gender.Name);
			}

			dataContexts.First ().DeleteEntity (naturalPersons.First ().Gender);
			dataContexts.First ().SaveChanges ();

			for (int i = 0; i < nbDataContexts; i++)
			{
				Assert.IsNull (naturalPersons[i].Gender);
			}

			for (int i = 0; i < nbDataContexts; i++)
			{
				DataContext dataContext = dataContexts[i];

				DataContextPool.Instance.Remove (dataContext);
				dataContext.Dispose ();
			}
		}


		[TestMethod]
		public void TestDeleteEntityCollection()
		{
			int nbDataContexts = 10;

			List<DataContext> dataContexts = new List<DataContext> ();

			for (int i = 0; i < nbDataContexts; i++)
			{
				DataContext dataContext = new DataContext (DatabaseHelper.DbInfrastructure);

				dataContexts.Add (dataContext);
				DataContextPool.Instance.Add (dataContext);
			}

			DbKey dbKey = new DbKey (new DbId (1));

			List<NaturalPersonEntity> naturalPersons = new List<NaturalPersonEntity> ();

			for (int i = 0; i < nbDataContexts; i++)
			{
				naturalPersons.Add (dataContexts[i].ResolveEntity<NaturalPersonEntity> (dbKey));
			}

			for (int i = 0; i < nbDataContexts; i++)
			{
				Assert.AreEqual (2, naturalPersons[i].Contacts.Count);

				if (i < 5)
				{
					string tmp = ((UriContactEntity) naturalPersons[i].Contacts[0]).Uri;
				}
			}

			dataContexts.First ().DeleteEntity (naturalPersons.First ().Contacts.First ());
			dataContexts.First ().SaveChanges ();

			for (int i = 0; i < nbDataContexts; i++)
			{
				Assert.AreEqual (1, naturalPersons[i].Contacts.Count);
			}

			for (int i = 0; i < nbDataContexts; i++)
			{
				DataContext dataContext = dataContexts[i];

				DataContextPool.Instance.Remove (dataContext);
				dataContext.Dispose ();
			}
		}


		[TestMethod]
		public void TestRemoveValue()
		{
			int nbDataContexts = 10;

			List<DataContext> dataContexts = new List<DataContext> ();

			for (int i = 0; i < nbDataContexts; i++)
			{
				DataContext dataContext = new DataContext (DatabaseHelper.DbInfrastructure);

				dataContexts.Add (dataContext);
				DataContextPool.Instance.Add (dataContext);
			}

			DbKey dbKey = new DbKey (new DbId (1));

			List<NaturalPersonEntity> naturalPersons = new List<NaturalPersonEntity> ();

			for (int i = 0; i < nbDataContexts; i++)
			{
				naturalPersons.Add (dataContexts[i].ResolveEntity<NaturalPersonEntity> (dbKey));
			}

			for (int i = 0; i < nbDataContexts; i++)
			{
				Assert.AreEqual (new Date (1950, 12, 31), naturalPersons[i].BirthDate);
			}

			naturalPersons.First ().BirthDate = null;
			dataContexts.First ().SaveChanges ();

			for (int i = 0; i < nbDataContexts; i++)
			{
				Assert.IsNull (naturalPersons[i].BirthDate);
			}

			for (int i = 0; i < nbDataContexts; i++)
			{
				DataContext dataContext = dataContexts[i];

				DataContextPool.Instance.Remove (dataContext);
				dataContext.Dispose ();
			}
		}


		[TestMethod]
		public void TestRemoveReference()
		{
			int nbDataContexts = 10;

			List<DataContext> dataContexts = new List<DataContext> ();

			for (int i = 0; i < nbDataContexts; i++)
			{
				DataContext dataContext = new DataContext (DatabaseHelper.DbInfrastructure);

				dataContexts.Add (dataContext);
				DataContextPool.Instance.Add (dataContext);
			}

			DbKey dbKey = new DbKey (new DbId (1));

			List<NaturalPersonEntity> naturalPersons = new List<NaturalPersonEntity> ();

			for (int i = 0; i < nbDataContexts; i++)
			{
				naturalPersons.Add (dataContexts[i].ResolveEntity<NaturalPersonEntity> (dbKey));
			}

			for (int i = 0; i < nbDataContexts; i++)
			{
				Assert.AreEqual ("Male", naturalPersons[i].Gender.Name);
			}

			naturalPersons.First ().Gender = null;
			dataContexts.First ().SaveChanges ();

			for (int i = 0; i < nbDataContexts; i++)
			{
				Assert.IsNull (naturalPersons[i].Gender);
			}

			for (int i = 0; i < nbDataContexts; i++)
			{
				DataContext dataContext = dataContexts[i];

				DataContextPool.Instance.Remove (dataContext);
				dataContext.Dispose ();
			}
		}


		[TestMethod]
		public void TestRemoveCollectionItem()
		{
			int nbDataContexts = 10;

			List<DataContext> dataContexts = new List<DataContext> ();

			for (int i = 0; i < nbDataContexts; i++)
			{
				DataContext dataContext = new DataContext (DatabaseHelper.DbInfrastructure);

				dataContexts.Add (dataContext);
				DataContextPool.Instance.Add (dataContext);
			}

			DbKey dbKey = new DbKey (new DbId (1));

			List<NaturalPersonEntity> naturalPersons = new List<NaturalPersonEntity> ();

			for (int i = 0; i < nbDataContexts; i++)
			{
				naturalPersons.Add (dataContexts[i].ResolveEntity<NaturalPersonEntity> (dbKey));
			}

			for (int i = 0; i < nbDataContexts; i++)
			{
				Assert.AreEqual (2, naturalPersons[i].Contacts.Count);
			}

			naturalPersons.First ().Contacts.RemoveAt (1);
			dataContexts.First ().SaveChanges ();

			for (int i = 0; i < nbDataContexts; i++)
			{
				Assert.AreEqual (1, naturalPersons[i].Contacts.Count);
			}

			for (int i = 0; i < nbDataContexts; i++)
			{
				DataContext dataContext = dataContexts[i];

				DataContextPool.Instance.Remove (dataContext);
				dataContext.Dispose ();
			}
		}


		[TestMethod]
		public void TestUpdateReferenceWithNewEntity()
		{
			int nbDataContexts = 10;

			List<DataContext> dataContexts = new List<DataContext> ();

			for (int i = 0; i < nbDataContexts; i++)
			{
				DataContext dataContext = new DataContext (DatabaseHelper.DbInfrastructure);

				dataContexts.Add (dataContext);
				DataContextPool.Instance.Add (dataContext);
			}

			DbKey dbKey = new DbKey (new DbId (1));

			List<NaturalPersonEntity> naturalPersons = new List<NaturalPersonEntity> ();

			for (int i = 0; i < nbDataContexts; i++)
			{
				naturalPersons.Add (dataContexts[i].ResolveEntity<NaturalPersonEntity> (dbKey));
			}

			for (int i = 0; i < nbDataContexts; i++)
			{
				Assert.AreEqual ("Male", naturalPersons[i].Gender.Name);
			}

			PersonGenderEntity newGender = dataContexts.First ().CreateEntity<PersonGenderEntity> ();
			newGender.Name = "E.T.";

			naturalPersons.First ().Gender = newGender;
			dataContexts.First ().SaveChanges ();

			for (int i = 0; i < nbDataContexts; i++)
			{
				Assert.AreEqual ("E.T.", naturalPersons[i].Gender.Name);
			}

			for (int i = 0; i < nbDataContexts; i++)
			{
				DataContext dataContext = dataContexts[i];

				DataContextPool.Instance.Remove (dataContext);
				dataContext.Dispose ();
			}
		}


		[TestMethod]
		public void TestUpdateCollectionWithNewEntity()
		{
			int nbDataContexts = 10;

			List<DataContext> dataContexts = new List<DataContext> ();

			for (int i = 0; i < nbDataContexts; i++)
			{
				DataContext dataContext = new DataContext (DatabaseHelper.DbInfrastructure);

				dataContexts.Add (dataContext);
				DataContextPool.Instance.Add (dataContext);
			}

			DbKey dbKey = new DbKey (new DbId (1));

			List<NaturalPersonEntity> naturalPersons = new List<NaturalPersonEntity> ();

			for (int i = 0; i < nbDataContexts; i++)
			{
				naturalPersons.Add (dataContexts[i].ResolveEntity<NaturalPersonEntity> (dbKey));
			}

			for (int i = 0; i < nbDataContexts; i++)
			{
				Assert.AreEqual (2, naturalPersons[i].Contacts.Count);
			}

			UriContactEntity newContact = dataContexts.First ().CreateEntity<UriContactEntity> ();
			newContact.Uri = "new@uri.com";

			naturalPersons.First ().Contacts.Add (newContact);
			dataContexts.First ().SaveChanges ();

			for (int i = 0; i < nbDataContexts; i++)
			{
				Assert.AreEqual ("new@uri.com", ((UriContactEntity) naturalPersons[i].Contacts[2]).Uri);
			}

			for (int i = 0; i < nbDataContexts; i++)
			{
				DataContext dataContext = dataContexts[i];

				DataContextPool.Instance.Remove (dataContext);
				dataContext.Dispose ();
			}
		}


	}


}
