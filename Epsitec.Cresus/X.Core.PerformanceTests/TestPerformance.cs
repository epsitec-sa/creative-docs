//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Marc BETTEX, Maintainer: Marc BETTEX

using Epsitec.Cresus.Core.Data;
using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Database;
using Epsitec.Cresus.DataLayer;
using System.Linq;

namespace Epsitec.Cresus.Core
{
	public class TestPerformance : System.IDisposable
	{
		public TestPerformance(bool createAndPopulateDatabase)
		{
			TestSetup.Initialize ();

			if (createAndPopulateDatabase)
			{
				Database.CreateAndConnectToDatabase ();
				Database.PopulateDatabase ();
			}
			else
			{
				Database.ConnectToDatabase ();
			}

			this.dbInfrastructure = Database.DbInfrastructure;
		}

		//	Retrieve a natural person
		public void RetrieveNaturalPerson()
		{
			using (DataContext context =  new DataContext (this.dbInfrastructure))
			{
				System.Diagnostics.Debug.WriteLine ("About to retrieve a natural person entity");
				System.Diagnostics.Stopwatch watch = new System.Diagnostics.Stopwatch ();

				watch.Start ();

				var key    = new DbKey (new DbId (1000000000040));
				var person = context.ResolveEntity<NaturalPersonEntity> (key);

				watch.Stop ();
				System.Diagnostics.Debug.WriteLine ("Operation took " + watch.ElapsedMilliseconds + " ms");
			}
		}
	

		public void RetrieveData()
		{
			using (DataContext dataContext = new DataContext (this.dbInfrastructure))
			{
				Repository repository = new Repository (this.dbInfrastructure, dataContext);

				System.Diagnostics.Stopwatch watch = new System.Diagnostics.Stopwatch ();

				watch.Start ();
				repository.GetEntitiesByExample<CountryEntity> (new CountryEntity ()).Count ();
				watch.Stop ();

				System.Diagnostics.Debug.WriteLine ("Time elapsed: " + watch.ElapsedMilliseconds);

				watch.Restart ();
				repository.GetEntitiesByExample<TelecomContactEntity> (new TelecomContactEntity ()).Count ();
				watch.Stop ();

				System.Diagnostics.Debug.WriteLine ("Time elapsed: " + watch.ElapsedMilliseconds);

				watch.Restart ();
				repository.GetEntitiesByExample<TelecomContactEntity> (new TelecomContactEntity ()).Count ();
				watch.Stop ();

				System.Diagnostics.Debug.WriteLine ("Time elapsed: " + watch.ElapsedMilliseconds);
			}
		}

		#region IDisposable Members

		public void Dispose()
		{
			if (this.dbInfrastructure != null)
			{
				this.dbInfrastructure.Dispose ();
				this.dbInfrastructure = null;
			}
		}

		#endregion


		private DbInfrastructure dbInfrastructure;
	}
}
