using Epsitec.Common.Types;

using Epsitec.Cresus.Database;

using Epsitec.Cresus.DataLayer.Context;
using Epsitec.Cresus.DataLayer.Infrastructure;
using Epsitec.Cresus.DataLayer.Schema;
using Epsitec.Cresus.DataLayer.Tests.Vs.Entities;
using Epsitec.Cresus.DataLayer.Tests.Vs.Helpers;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.Collections.Generic;
using System.Linq;


namespace Epsitec.Cresus.DataLayer.Tests.Vs.General
{


	[TestClass]
	public sealed class UnitTestSynchronization
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
		public void TestUpdateValue()
		{
			
			using (DataInfrastructure dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase ())
			{
				int nbDataContexts = 10;

				List<DataContext> dataContexts = new List<DataContext> ();

				for (int i = 0; i < nbDataContexts; i++)
				{
					DataContext dataContext = dataInfrastructure.CreateDataContext ();

					dataContexts.Add (dataContext);
				}

				DbKey dbKey = new DbKey (new DbId (1000000001));

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
			}
		}


		[TestMethod]
		public void TestUpdateReference()
		{
			
			using (DataInfrastructure dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase ())
			{
				int nbDataContexts = 10;

				List<DataContext> dataContexts = new List<DataContext> ();

				for (int i = 0; i < nbDataContexts; i++)
				{
					DataContext dataContext = dataInfrastructure.CreateDataContext ();

					dataContexts.Add (dataContext);
				}

				DbKey dbKey = new DbKey (new DbId (1000000001));

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

				DbKey newGenderKey = new DbKey (new DbId (1000000002));
				PersonGenderEntity newGender = dataContexts.First ().ResolveEntity<PersonGenderEntity> (newGenderKey);

				naturalPersons.First ().Gender = newGender;
				dataContexts.First ().SaveChanges ();

				for (int i = 0; i < nbDataContexts; i++)
				{
					Assert.AreEqual (gender2, naturalPersons[i].Gender.Name);
				}
			}
		}


		[TestMethod]
		public void TestUpdateCollection()
		{
			
			using (DataInfrastructure dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase ())
			{
				int nbDataContexts = 10;

				List<DataContext> dataContexts = new List<DataContext> ();

				for (int i = 0; i < nbDataContexts; i++)
				{
					DataContext dataContext = dataInfrastructure.CreateDataContext ();

					dataContexts.Add (dataContext);
				}

				DbKey dbKey = new DbKey (new DbId (1000000001));

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
			}
		}


		[TestMethod]
		public void TestDeleteEntityReference()
		{
			
			using (DataInfrastructure dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase ())
			{
				int nbDataContexts = 10;

				List<DataContext> dataContexts = new List<DataContext> ();

				for (int i = 0; i < nbDataContexts; i++)
				{
					DataContext dataContext = dataInfrastructure.CreateDataContext ();

					dataContexts.Add (dataContext);
				}

				DbKey dbKey = new DbKey (new DbId (1000000001));

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
			}
		}


		[TestMethod]
		public void TestDeleteEntityCollection()
		{
			
			using (DataInfrastructure dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase ())
			{
				int nbDataContexts = 10;

				List<DataContext> dataContexts = new List<DataContext> ();

				for (int i = 0; i < nbDataContexts; i++)
				{
					DataContext dataContext = dataInfrastructure.CreateDataContext ();

					dataContexts.Add (dataContext);
				}

				DbKey dbKey = new DbKey (new DbId (1000000001));

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
			}
		}


		[TestMethod]
		public void TestRemoveValue()
		{
			
			using (DataInfrastructure dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase ())
			{
				int nbDataContexts = 10;

				List<DataContext> dataContexts = new List<DataContext> ();

				for (int i = 0; i < nbDataContexts; i++)
				{
					DataContext dataContext = dataInfrastructure.CreateDataContext ();

					dataContexts.Add (dataContext);
				}

				DbKey dbKey = new DbKey (new DbId (1000000001));

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
			}
		}


		[TestMethod]
		public void TestRemoveReference()
		{
			
			using (DataInfrastructure dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase ())
			{
				int nbDataContexts = 10;

				List<DataContext> dataContexts = new List<DataContext> ();

				for (int i = 0; i < nbDataContexts; i++)
				{
					DataContext dataContext = dataInfrastructure.CreateDataContext ();

					dataContexts.Add (dataContext);
				}

				DbKey dbKey = new DbKey (new DbId (1000000001));

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
			}
		}


		[TestMethod]
		public void TestRemoveCollectionItem()
		{
			
			using (DataInfrastructure dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase ())
			{
				int nbDataContexts = 10;

				List<DataContext> dataContexts = new List<DataContext> ();

				for (int i = 0; i < nbDataContexts; i++)
				{
					DataContext dataContext = dataInfrastructure.CreateDataContext ();

					dataContexts.Add (dataContext);
				}

				DbKey dbKey = new DbKey (new DbId (1000000001));

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
			}
		}


		[TestMethod]
		public void TestRemoveAllCollectionItems()
		{
			
			using (DataInfrastructure dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase ())
			{
				int nbDataContexts = 10;

				List<DataContext> dataContexts = new List<DataContext> ();

				for (int i = 0; i < nbDataContexts; i++)
				{
					DataContext dataContext = dataInfrastructure.CreateDataContext ();

					dataContexts.Add (dataContext);
				}

				DbKey dbKey = new DbKey (new DbId (1000000001));

				List<NaturalPersonEntity> naturalPersons = new List<NaturalPersonEntity> ();

				for (int i = 0; i < nbDataContexts; i++)
				{
					naturalPersons.Add (dataContexts[i].ResolveEntity<NaturalPersonEntity> (dbKey));
				}

				for (int i = 0; i < nbDataContexts; i++)
				{
					Assert.AreEqual (2, naturalPersons[i].Contacts.Count);
				}

				naturalPersons.First ().Contacts.Clear ();
				dataContexts.First ().SaveChanges ();

				for (int i = 0; i < nbDataContexts; i++)
				{
					Assert.AreEqual (0, naturalPersons[i].Contacts.Count);
				}
			}
		}


		[TestMethod]
		public void TestUpdateReferenceWithNewEntity()
		{
			
			using (DataInfrastructure dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase ())
			{
				int nbDataContexts = 10;

				List<DataContext> dataContexts = new List<DataContext> ();

				for (int i = 0; i < nbDataContexts; i++)
				{
					DataContext dataContext = dataInfrastructure.CreateDataContext ();

					dataContexts.Add (dataContext);
				}

				DbKey dbKey = new DbKey (new DbId (1000000001));

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
			}
		}


		[TestMethod]
		public void TestUpdateCollectionWithNewEntity()
		{
			
			using (DataInfrastructure dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase ())
			{
				int nbDataContexts = 10;

				List<DataContext> dataContexts = new List<DataContext> ();

				for (int i = 0; i < nbDataContexts; i++)
				{
					DataContext dataContext = dataInfrastructure.CreateDataContext ();

					dataContexts.Add (dataContext);
				}

				DbKey dbKey = new DbKey (new DbId (1000000001));

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
			}
		}


		[TestMethod]
		public void DeleteEntity()
		{
			
			using (DataInfrastructure dataInfrastructure = DataInfrastructureHelper.ConnectToTestDatabase ())
			{
				int nbDataContexts = 10;

				List<DataContext> dataContexts = new List<DataContext> ();

				for (int i = 0; i < nbDataContexts; i++)
				{
					DataContext dataContext = dataInfrastructure.CreateDataContext ();

					dataContexts.Add (dataContext);
				}

				DbKey dbKey = new DbKey (new DbId (1000000001));

				List<NaturalPersonEntity> naturalPersons = new List<NaturalPersonEntity> ();

				for (int i = 0; i < nbDataContexts; i++)
				{
					naturalPersons.Add (dataContexts[i].ResolveEntity<NaturalPersonEntity> (dbKey));
				}

				dataContexts.First ().DeleteEntity (naturalPersons.First ());
				dataContexts.First ().SaveChanges ();

				for (int i = 0; i < nbDataContexts; i++)
				{
					Assert.IsTrue (dataContexts[i].IsDeleted (naturalPersons[i]));
				}
			}
		}


	}


}
