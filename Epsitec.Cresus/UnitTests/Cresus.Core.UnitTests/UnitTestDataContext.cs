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

			Database.CreateAndConnectToDatabase ();

			Assert.IsTrue (Database.DbInfrastructure.IsConnectionOpen);
		}


		[TestMethod]
		public void PopulateDatabase()
		{
			TestHelper.PrintStartTest ("Populate database");

			Database2.PupulateDatabase ();
		}


		[TestMethod]
		public void SaveWithoutChanges1()
		{
			TestHelper.PrintStartTest ("Save without changes 1");

			using (DataContext dataContext = new DataContext (Database.DbInfrastructure))
			{
				Database2.DbInfrastructure.GetSourceReferences (new Common.Support.Druid ());
				dataContext.SaveChanges ();
			}
		}


		[TestMethod]
		public void SaveWithoutChanges2()
		{
			TestHelper.PrintStartTest ("Save without changes 2");

			using (DataContext dataContext = new DataContext (Database.DbInfrastructure))
			{
				Repository repository = new Repository (Database.DbInfrastructure, dataContext);

				UriContactEntity[] contacts = repository.GetEntitiesByExample<UriContactEntity> (new UriContactEntity ()).ToArray ();

				Assert.IsTrue (contacts.Length == 4);

				Assert.IsTrue (contacts.Any (c => Database2.CheckUriContact (c, "alfred@coucou.com", "Alfred")));
				Assert.IsTrue (contacts.Any (c => Database2.CheckUriContact (c, "alfred@blabla.com", "Alfred")));
				Assert.IsTrue (contacts.Any (c => Database2.CheckUriContact (c, "gertrude@coucou.com", "Gertrude")));
				Assert.IsTrue (contacts.Any (c => Database2.CheckUriContact (c, "nobody@nowhere.com", null)));

				dataContext.SaveChanges ();
			}
		}


		[TestMethod]
		public void ResolveWithoutBulkMode()
		{
			TestHelper.PrintStartTest ("Resolve without bulk mode");

			using (DataContext dataContext = new DataContext (Database.DbInfrastructure, false))
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
		public void ResolveWithBulkMode()
		{
			TestHelper.PrintStartTest ("Resolve with bulk mode");

			using (DataContext dataContext = new DataContext (Database.DbInfrastructure, true))
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
		public void DeleteRelation1()
		{
			TestHelper.PrintStartTest ("Delete Relation 1");

			using (DataContext dataContext = new DataContext (Database.DbInfrastructure))
			{
				NaturalPersonEntity alfred = dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1000000000001)));

				Assert.IsTrue (alfred.Gender != null);

				alfred.Gender = null;

				Assert.IsTrue (alfred.Gender == null);

				dataContext.SaveChanges ();
			}
		
			using (DataContext dataContext = new DataContext (Database.DbInfrastructure))
			{
				NaturalPersonEntity alfred = dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1000000000001)));

				Assert.IsTrue (alfred.Gender == null);
			}
		}


		[TestMethod]
		public void DeleteRelation2()
		{
			TestHelper.PrintStartTest ("Delete Relation 2");

			Database.CreateAndConnectToDatabase ();
			Database2.PupulateDatabase ();

			using (DataContext dataContext = new DataContext (Database.DbInfrastructure))
			{
				NaturalPersonEntity alfred = dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1000000000001)));

				Assert.IsTrue (Database2.CheckAlfred (alfred));

				alfred.Contacts.RemoveAt (0);

				Assert.IsTrue (alfred.Contacts.Count == 1);
				Assert.IsTrue (alfred.Contacts.Any (c => Database2.CheckUriContact (c as UriContactEntity, "alfred@blabla.com", "Alfred")));

				dataContext.SaveChanges ();
			}
		
			using (DataContext dataContext = new DataContext (Database.DbInfrastructure))
			{
				NaturalPersonEntity alfred = dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1000000000001)));

				Assert.IsTrue (alfred.Contacts.Count == 1);
				Assert.IsTrue (alfred.Contacts.Any (c => Database2.CheckUriContact (c as UriContactEntity, "alfred@blabla.com", "Alfred")));
			}
		}


		[TestMethod]
		public void DeleteEntity1()
		{
			TestHelper.PrintStartTest ("Delete Entity 1");

			Database.CreateAndConnectToDatabase ();
			Database2.PupulateDatabase ();

			using (DataContext dataContext = new DataContext (Database.DbInfrastructure))
			{
				NaturalPersonEntity alfred = dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1000000000001)));

				Assert.IsTrue (Database2.CheckAlfred (alfred));

				dataContext.DeleteEntity (alfred.Contacts[0] as UriContactEntity);

				dataContext.SaveChanges ();
			}

			using (DataContext dataContext = new DataContext (Database.DbInfrastructure))
			{
				NaturalPersonEntity alfred = dataContext.ResolveEntity<NaturalPersonEntity> (new DbKey (new DbId (1000000000001)));

				Assert.IsTrue (alfred.Contacts.Count == 1);
				Assert.IsTrue (alfred.Contacts.Any (c => Database2.CheckUriContact (c as UriContactEntity, "alfred@blabla.com", "Alfred")));
			}
		}


	}


}
