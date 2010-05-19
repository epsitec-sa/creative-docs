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
			TestSetup.Initialize ();

			if (UnitTestPerformance.createAndPopulateDatabase)
			{
				Database.CreateAndConnectToDatabase ();
				Database.PopulateDatabase (UnitTestPerformance.bigDatabase);
			}
			else
			{
				Database.ConnectToDatabase ();
			}
		}

		[TestMethod]
		public void Check03RetrieveData()
		{
			using (DataContext dataContext = new DataContext (Database.DbInfrastructure))
			{
				dataContext.BulkMode = true;
				
				Repository repository = new Repository (Database.DbInfrastructure, dataContext);

				System.Diagnostics.Stopwatch watch = new System.Diagnostics.Stopwatch ();

				watch.Start ();
				repository.GetEntitiesByExample<CountryEntity> (new CountryEntity ()).Count ();
				watch.Stop ();

				System.Diagnostics.Debug.WriteLine ("Time elapsed: " + watch.ElapsedMilliseconds);

				watch.Restart ();
				repository.GetEntitiesByExample<NaturalPersonEntity> (new NaturalPersonEntity ()).Count ();
				watch.Stop ();

				System.Diagnostics.Debug.WriteLine ("Time elapsed: " + watch.ElapsedMilliseconds);

				watch.Restart ();
				repository.GetEntitiesByExample<NaturalPersonEntity> (new NaturalPersonEntity ()).Count ();
				watch.Stop ();

				System.Diagnostics.Debug.WriteLine ("Time elapsed: " + watch.ElapsedMilliseconds);
			}
		}


		private static bool createAndPopulateDatabase = true;
		private static bool bigDatabase = true;
	}
}
