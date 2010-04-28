//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support.EntityEngine;

using Epsitec.Cresus.Database;
using Epsitec.Cresus.DataLayer;

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
			TestSetup.Initialize ();
			UnitTestDataContext.infrastructure = TestSetup.CreateDbInfrastructure ();
		}
		
		[TestMethod]
		public void Test01DatabaseConnection()
		{
			Assert.IsTrue (UnitTestDataContext.infrastructure.IsConnectionOpen);
		}

		[TestMethod]
		public void Test02DatabaseSchemaCreation()
		{
			EntityContext entityContext = EntityContext.Current;
			DataContext   dataContext   = new DataContext (UnitTestDataContext.infrastructure);

			Assert.AreEqual (entityContext, dataContext.EntityContext);
			
			Assert.IsTrue (dataContext.CreateSchema<Entities.AddressEntity> ());
			Assert.IsFalse (dataContext.CreateSchema<Entities.LocationEntity> ());

			dataContext.Dispose ();
		}

		[TestMethod]
		public void Test03DatabaseCreateCountryEntity()
		{
			EntityContext entityContext = EntityContext.Current;
			DataContext   dataContext   = new DataContext (UnitTestDataContext.infrastructure);

			var country1 = dataContext.CreateEmptyEntity<Entities.CountryEntity> ();
			var country2 = dataContext.CreateEmptyEntity<Entities.CountryEntity> ();

			country1.Code = "CH";
			country1.Name = "Suisse";

			country2.Code = "FR";
			country2.Name = "France";

			dataContext.SaveChanges ();
			dataContext.Dispose ();
		}

		[TestMethod]
		public void Test04DatabaseReadBackCountryEntity()
		{
			EntityContext entityContext = EntityContext.Current;
			DataContext   dataContext   = new DataContext (UnitTestDataContext.infrastructure);

			Assert.AreEqual (1, this.FindCountryEntities ("CH").Count ());
			Assert.AreEqual (1, this.FindCountryEntities ("FR").Count ());
			Assert.AreEqual (0, this.FindCountryEntities ("DE").Count ());
			
			var countryKey = this.FindCountryEntities ("CH").First ();
			var country = dataContext.ResolveEntity<Entities.CountryEntity> (countryKey);

			Assert.AreEqual ("CH", country.Code);
			Assert.AreEqual ("Suisse", country.Name);
			
			dataContext.Dispose ();
		}

		[TestMethod]
		public void Test05DatabaseCreateAddressEntity()
		{
			EntityContext entityContext = EntityContext.Current;
			DataContext   dataContext   = new DataContext (UnitTestDataContext.infrastructure);

			var address1 = dataContext.CreateEntity<Entities.AddressEntity> ();
			var address2 = dataContext.CreateEntity<Entities.AddressEntity> ();
			var location = dataContext.CreateEntity<Entities.LocationEntity> ();
			var street1  = dataContext.CreateEntity<Entities.StreetEntity> ();
			var street2  = dataContext.CreateEntity<Entities.StreetEntity> ();
			var postBox  = dataContext.CreateEntity<Entities.PostBoxEntity> ();

			var countryKey = this.FindCountryEntities ("CH").First ();
			var country    = dataContext.ResolveEntity<Entities.CountryEntity> (countryKey);

			address1.Location   = location;
			address1.Street     = street1;
			address1.PostBox    = postBox;

			address2.Location   = location;
			address2.Street     = street2;

			location.Country    = country;
			location.Name       = "Yverdon-les-Bains";
			location.PostalCode = "1400";

			street1.StreetName  = "Ch. du Fontenay 6";
			street2.StreetName  = "Rue d'Orbe 28";

			postBox.Number      = "Case postale 1234";
			
			dataContext.SaveChanges ();
			dataContext.Dispose ();
		}


		private IEnumerable<DbKey> FindCountryEntities(string code)
		{
			DataBrowser browser = new DataBrowser (UnitTestDataContext.infrastructure, new DataContext (UnitTestDataContext.infrastructure));
			DataQuery query = new DataQuery ();

			var entity = new Entities.CountryEntity ();

			entity.Code = code;
			query.Columns.Add (new DataQueryColumn (EntityFieldPath.Parse ("[L0A2]")));

			using (DbTransaction transaction = browser.DbInfrastructure.BeginTransaction (DbTransactionMode.ReadOnly))
			{
				// The DataBrowser has been significantly modified. This method does not exist
				// anymore but it could be implemented again, this is why I leave the test unchanged,
				// except for the three lines below.

				//foreach (DataBrowserRow row in browser.QueryByExample (transaction, entity, query))
				//{
				//    yield return row.Keys[0];
				//}

				throw new System.NotImplementedException ("See comments above");

				transaction.Commit ();
			}
		}


		private static DbInfrastructure infrastructure;
	}
}
