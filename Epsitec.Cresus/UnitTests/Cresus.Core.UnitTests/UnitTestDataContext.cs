//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support.EntityEngine;

using Epsitec.Cresus.Database;
using Epsitec.Cresus.DataLayer;
using Epsitec.Cresus.Core.Data;
using Epsitec.Cresus.Core.Entities;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core
{


	[TestClass]
	public class UnitTestDataContext
	{


		[ClassInitialize]
		public static void Initialize(TestContext testContext)
		{
			TestHelper.Initialize ();
		}

		
		[TestMethod]
		public void CreateDatabase()
		{
			TestHelper.PrintStartTest ("Create database");

			this.CreateDatabase (false);
			this.CreateDatabase (true);
		}


		public void CreateDatabase(bool bulkMode)
		{
			Database.CreateAndConnectToDatabase ();

			Assert.IsTrue (Database.DbInfrastructure.IsConnectionOpen);

			Database2.PupulateDatabase (bulkMode);
		}


		[TestMethod]
		public void SaveWithoutChanges1()
		{
			TestHelper.PrintStartTest ("Save without changes 1");

			this.SaveWithoutChanges1 (false);
			this.SaveWithoutChanges1 (true);
		}


		public void SaveWithoutChanges1(bool bulkMode)
		{
			using (DataContext dataContext = new DataContext (Database.DbInfrastructure, bulkMode))
			{
				Database2.DbInfrastructure.GetSourceReferences (new Common.Support.Druid ());
				dataContext.SaveChanges ();
			}
		}


		[TestMethod]
		public void SaveWithoutChanges2()
		{
			TestHelper.PrintStartTest ("Save without changes 2");

			this.SaveWithoutChanges2 (false);
			this.SaveWithoutChanges2 (true);
		}


		public void SaveWithoutChanges2(bool bulkMode)
		{
			using (DataContext dataContext = new DataContext (Database.DbInfrastructure, bulkMode))
			{
				UriContactEntity[] contacts = {
					dataContext.ResolveEntity<UriContactEntity> (new DbKey (new DbId (1000000000001))),
					dataContext.ResolveEntity<UriContactEntity> (new DbKey (new DbId (1000000000002))),
					dataContext.ResolveEntity<UriContactEntity> (new DbKey (new DbId (1000000000003))),
					dataContext.ResolveEntity<UriContactEntity> (new DbKey (new DbId (1000000000004))),
				};

				Assert.IsTrue (contacts.Length == 4);

				Assert.IsTrue (contacts.Any (c => Database2.CheckUriContact (c, "alfred@coucou.com", "Alfred")));
				Assert.IsTrue (contacts.Any (c => Database2.CheckUriContact (c, "alfred@blabla.com", "Alfred")));
				Assert.IsTrue (contacts.Any (c => Database2.CheckUriContact (c, "gertrude@coucou.com", "Gertrude")));
				Assert.IsTrue (contacts.Any (c => Database2.CheckUriContact (c, "nobody@nowhere.com", null)));

				dataContext.SaveChanges ();
			}
		}


		[TestMethod]
		public void Resolve()
		{
			TestHelper.PrintStartTest ("Resolve");

			this.Resolve (false);
			this.Resolve (true);
		}


		public void Resolve(bool bulkMode)
		{
			using (DataContext dataContext = new DataContext (Database.DbInfrastructure, bulkMode))
			{
				NaturalPersonEntity alfred = dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1000000000001)));
				NaturalPersonEntity gertrude = dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1000000000002)));
				NaturalPersonEntity hans = dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1000000000003)));

				Assert.IsTrue (Database2.CheckAlfred (alfred));
				Assert.IsTrue (Database2.CheckGertrude (gertrude));
				Assert.IsTrue (Database2.CheckHans (hans));
			}
		}


		[TestMethod]
		public void GetFreshObject()
		{
			TestHelper.PrintStartTest ("Get fresh object");

			this.GetFreshObject (false);
			this.GetFreshObject (true);
		}


		public void GetFreshObject(bool bulkMode)
		{
			using (DataContext dataContext = new DataContext (Database.DbInfrastructure, bulkMode))
			{
				NaturalPersonEntity freshPerson1 = dataContext.CreateEmptyEntity<NaturalPersonEntity> ();

				freshPerson1.Firstname = "Albert";
				freshPerson1.Lastname = "Levert";

				dataContext.SaveChanges ();

				NaturalPersonEntity freshPerson2 = dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1000000000004)));

				Assert.IsTrue (freshPerson1 == freshPerson2);

				dataContext.SaveChanges ();
			}

			this.CreateDatabase (false);
		}


		[TestMethod]
		public void DeleteRelationReference()
		{
			TestHelper.PrintStartTest ("Delete Relation Reference");

			this.DeleteRelationReference (false);
			this.DeleteRelationReference (true);
		}


		public void DeleteRelationReference(bool bulkMode)
		{
			using (DataContext dataContext = new DataContext (Database.DbInfrastructure, bulkMode))
			{
				NaturalPersonEntity alfred = dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1000000000001)));

				Assert.IsTrue (alfred.Gender != null);

				alfred.Gender = null;

				Assert.IsTrue (alfred.Gender == null);

				dataContext.SaveChanges ();
			}

			using (DataContext dataContext = new DataContext (Database.DbInfrastructure, bulkMode))
			{
				NaturalPersonEntity alfred = dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1000000000001)));

				Assert.IsTrue (alfred.Gender == null);
			}

			this.CreateDatabase (bulkMode);
		}


		[TestMethod]
		public void DeleteRelationCollection()
		{
			TestHelper.PrintStartTest ("Delete Relation Collection");

			this.DeleteRelationCollection (false);
			this.DeleteRelationCollection (true);
		}


		public void DeleteRelationCollection(bool bulkMode)
		{
			using (DataContext dataContext = new DataContext (Database.DbInfrastructure, bulkMode))
			{
				NaturalPersonEntity alfred = dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1000000000001)));

				Assert.IsTrue (Database2.CheckAlfred (alfred));

				alfred.Contacts.RemoveAt (0);

				Assert.IsTrue (alfred.Contacts.Count == 1);
				Assert.IsTrue (alfred.Contacts.Any (c => Database2.CheckUriContact (c as UriContactEntity, "alfred@blabla.com", "Alfred")));

				dataContext.SaveChanges ();
			}

			using (DataContext dataContext = new DataContext (Database.DbInfrastructure, bulkMode))
			{
				NaturalPersonEntity alfred = dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1000000000001)));

				Assert.IsTrue (alfred.Contacts.Count == 1);
				Assert.IsTrue (alfred.Contacts.Any (c => Database2.CheckUriContact (c as UriContactEntity, "alfred@blabla.com", "Alfred")));
			}

			this.CreateDatabase (bulkMode);
		}


		[TestMethod]
		public void DeleteEntityCollectionTargetInMemory()
		{
			TestHelper.PrintStartTest ("Delete Entity Collection Target In Memory");

			this.DeleteEntityCollectionTargetInMemory (false);
			this.DeleteEntityCollectionTargetInMemory (true);
		}


		public void DeleteEntityCollectionTargetInMemory(bool bulkMode)
		{
			using (DataContext dataContext = new DataContext (Database.DbInfrastructure, bulkMode))
			{
				NaturalPersonEntity alfred = dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1000000000001)));

				Assert.IsTrue (Database2.CheckAlfred (alfred));

				dataContext.DeleteEntity (alfred.Contacts[0]);

				dataContext.SaveChanges ();

				Assert.IsTrue (alfred.Contacts.Count == 1);
				Assert.IsTrue (alfred.Contacts.Any (c => Database2.CheckUriContact (c as UriContactEntity, "alfred@blabla.com", "Alfred")));
			}

			using (DataContext dataContext = new DataContext (Database.DbInfrastructure, bulkMode))
			{
				NaturalPersonEntity alfred = dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1000000000001)));

				Assert.IsTrue (alfred.Contacts.Count == 1);
				Assert.IsTrue (alfred.Contacts.Any (c => Database2.CheckUriContact (c as UriContactEntity, "alfred@blabla.com", "Alfred")));
			}

			this.CreateDatabase (bulkMode);
		}


		[TestMethod]
		public void DeleteEntityReferenceTargetInMemory()
		{
			TestHelper.PrintStartTest ("Delete Entity Reference Target In Memory");

			this.DeleteEntityReferenceTargetInMemory (false);
			this.DeleteEntityReferenceTargetInMemory (true);
		}


		public void DeleteEntityReferenceTargetInMemory(bool bulkMode)
		{
			using (DataContext dataContext = new DataContext (Database.DbInfrastructure, bulkMode))
			{
				UriContactEntity[] contacts = {
					dataContext.ResolveEntity<UriContactEntity> (new DbKey (new DbId (1000000000001))),
					dataContext.ResolveEntity<UriContactEntity> (new DbKey (new DbId (1000000000002))),
				};

				NaturalPersonEntity alfred = dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1000000000001)));
				dataContext.DeleteEntity (alfred);

				Assert.IsTrue (contacts.All (c => c.NaturalPerson == alfred));

				dataContext.SaveChanges ();

				Assert.IsTrue (contacts.All (c => c.NaturalPerson == null));
			}

			using (DataContext dataContext = new DataContext (Database.DbInfrastructure, bulkMode))
			{
				UriContactEntity[] contacts = {
					dataContext.ResolveEntity<UriContactEntity> (new DbKey (new DbId (1000000000001))),
					dataContext.ResolveEntity<UriContactEntity> (new DbKey (new DbId (1000000000002))),
				};

				Assert.IsTrue (contacts.All (c => c.NaturalPerson == null));
			}

			this.CreateDatabase (bulkMode);
		}


		[TestMethod]
		public void DeleteEntityCollectionTargetInDatabase()
		{
			TestHelper.PrintStartTest ("Delete Entity Collection Target In Database");

			this.DeleteEntityCollectionTargetInDatabase (false);
			this.DeleteEntityCollectionTargetInDatabase (true);
		}


		public void DeleteEntityCollectionTargetInDatabase(bool bulkMode)
		{
			using (DataContext dataContext = new DataContext (Database.DbInfrastructure, bulkMode))
			{
				UriContactEntity contact = dataContext.ResolveEntity<UriContactEntity> (new DbKey (new DbId (1000000000001)));
				dataContext.DeleteEntity (contact);

				dataContext.SaveChanges ();

				NaturalPersonEntity alfred = dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1000000000001)));

				Assert.IsTrue (alfred.Contacts.Count == 1);
				Assert.IsTrue (alfred.Contacts.Any (c => Database2.CheckUriContact (c as UriContactEntity, "alfred@blabla.com", "Alfred")));
			}

			using (DataContext dataContext = new DataContext (Database.DbInfrastructure, bulkMode))
			{
				NaturalPersonEntity alfred = dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1000000000001)));

				Assert.IsTrue (alfred.Contacts.Count == 1);
				Assert.IsTrue (alfred.Contacts.Any (c => Database2.CheckUriContact (c as UriContactEntity, "alfred@blabla.com", "Alfred")));
			}

			this.CreateDatabase (bulkMode);
		}


		[TestMethod]
		public void DeleteEntityReferenceTargetInDatabase()
		{
			TestHelper.PrintStartTest ("Delete Entity Reference Target In Database");

			this.DeleteEntityReferenceTargetInDatabase (false);
			this.DeleteEntityReferenceTargetInDatabase (true);
		}


		public void DeleteEntityReferenceTargetInDatabase(bool bulkMode)
		{
			using (DataContext dataContext = new DataContext (Database.DbInfrastructure, bulkMode))
			{
				NaturalPersonEntity alfred = dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1000000000001)));
				dataContext.DeleteEntity (alfred);

				dataContext.SaveChanges ();

				UriContactEntity[] contacts = {
					dataContext.ResolveEntity<UriContactEntity> (new DbKey (new DbId (1000000000001))),
					dataContext.ResolveEntity<UriContactEntity> (new DbKey (new DbId (1000000000002))),
				};

				Assert.IsTrue (contacts.All (c => c.NaturalPerson == null));
			}

			using (DataContext dataContext = new DataContext (Database.DbInfrastructure, bulkMode))
			{
				UriContactEntity[] contacts = {
					dataContext.ResolveEntity<UriContactEntity> (new DbKey (new DbId (1000000000001))),
					dataContext.ResolveEntity<UriContactEntity> (new DbKey (new DbId (1000000000002))),
				};

				Assert.IsTrue (contacts.All (c => c.NaturalPerson == null));
			}

			this.CreateDatabase (bulkMode);
		}


	}


}
