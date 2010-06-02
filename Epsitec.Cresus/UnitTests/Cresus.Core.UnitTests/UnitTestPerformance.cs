//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Marc BETTEX, Maintainer: Marc BETTEX

using Epsitec.Common.Support;
using Epsitec.Common.Types;

using Epsitec.Cresus.Core.Data;
using Epsitec.Cresus.Core.Entities;

using Epsitec.Cresus.Database;
using Epsitec.Cresus.DataLayer;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.Collections.Generic;
using System.Linq;


namespace Epsitec.Cresus.Core
{


	[TestClass]
	public class UnitTestPerformance
	{


		[ClassInitialize]
		public static void Initialize(TestContext testContext)
		{
			TestHelper.Initialize ();
		}


		[TestMethod]
		public void CreateAndPopulateDatabase()
		{
			if (UnitTestPerformance.createDatabase)
			{
				TestHelper.PrintStartTest ("Database creation");
							
				Database.CreateAndConnectToDatabase ();

				TestHelper.MeasureAndDisplayTime(
					"database population",
					() => Database1.PopulateDatabase (UnitTestPerformance.databaseSize)
				);
			}
			else
			{
				TestHelper.PrintStartTest ("Database connection");

				Database.ConnectToDatabase ();
			}
		}


		[TestMethod]
		public void RetrieveTelecomDataWithoutWarmup()
		{
			TestHelper.PrintStartTest ("Retrieve telecom without warmup");

			using (DataContext dataContext = new DataContext (Database.DbInfrastructure))
			{
				Repository repository = new Repository (Database.DbInfrastructure, dataContext);
				repository.GetEntitiesByExample<CountryEntity> (new CountryEntity ()).Count ();
			}

			TestHelper.MeasureAndDisplayTime (
				"retrieval",
				() =>
				{
					using (DataContext dataContext = new DataContext (Database.DbInfrastructure))
					{
						Repository repository = new Repository (Database.DbInfrastructure, dataContext);
						repository.GetEntitiesByExample<TelecomContactEntity> (new TelecomContactEntity ()).Count ();
					}
				},
				10
			);
		}


		[TestMethod]
		public void RetrieveTelecomDataWithWarmup()
		{
			TestHelper.PrintStartTest ("Retrieve telecom with warmup");

			using (DataContext dataContext = new DataContext (Database.DbInfrastructure))
			{
				Repository repository = new Repository (Database.DbInfrastructure, dataContext);
				repository.GetEntitiesByExample<TelecomContactEntity> (new TelecomContactEntity ()).Count ();

				TestHelper.MeasureAndDisplayTime (
					"retrieval",
					() => repository.GetEntitiesByExample<TelecomContactEntity> (new TelecomContactEntity ()).Count (),
					10
				);
			}
		}


		private static bool createDatabase = true;


		private static DatabaseSize databaseSize = DatabaseSize.Small;


	}
}
