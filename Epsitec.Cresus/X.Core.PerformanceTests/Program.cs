using System.Linq;

using Epsitec.Cresus.Core.Data;
using Epsitec.Cresus.Core.Entities;

using Epsitec.Cresus.Database;
using Epsitec.Cresus.DataLayer;
using Epsitec.Common.Support;



namespace Cresus.Core
{
	class Program
	{
		static void Main(string[] args)
		{
			ResourceManagerPool.Default = new ResourceManagerPool ("default");
			ResourceManagerPool.Default.AddResourceProbingPath (@"S:\Epsitec.Cresus\Cresus.Core");

			new UnitTestPerformance ().Check03RetreiveData ();
		}
	}


	public class UnitTestPerformance
	{

		public void Check03RetreiveData()
		{
			UnitTestPerformance.dbInfrastructure = new DbInfrastructure ();
			UnitTestPerformance.dbInfrastructure.AttachToDatabase (DbInfrastructure.CreateDatabaseAccess ("CORETEST"));
			using (DataContext dataContext = new DataContext (UnitTestPerformance.dbInfrastructure))
			{
				Repository repository = new Repository (UnitTestPerformance.dbInfrastructure, dataContext);

				System.Diagnostics.Stopwatch watch = new System.Diagnostics.Stopwatch ();
				watch.Start ();
				repository.GetEntitiesByExample<CountryEntity> (new CountryEntity ()).Count ();
				watch.Stop ();
				System.Diagnostics.Debug.WriteLine (watch.ElapsedMilliseconds);

				watch.Start ();
				repository.GetEntitiesByExample<TelecomContactEntity> (new TelecomContactEntity ()).Count ();
				watch.Stop ();
				System.Diagnostics.Debug.WriteLine (watch.ElapsedMilliseconds);
				watch.Start ();
				repository.GetEntitiesByExample<TelecomContactEntity> (new TelecomContactEntity ()).Count ();
				watch.Stop ();
				System.Diagnostics.Debug.WriteLine (watch.ElapsedMilliseconds);
			}
		}

		private static DbInfrastructure dbInfrastructure;


	}


}
