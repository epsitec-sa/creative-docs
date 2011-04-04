using Epsitec.Cresus.DataLayer.Infrastructure;
using Epsitec.Cresus.DataLayer.Schema;


namespace Epsitec.Cresus.DataLayer.Tests.Vs.Helpers
{


	internal static class DataInfrastructureHelper
	{


		public static DataInfrastructure ConnectToTestDatabase()
		{
			var entityEngine = EntityEngineHelper.ConnectToTestDatabase ();

			return DataInfrastructureHelper.ConnectToTestDatabase (entityEngine);
		}


		public static DataInfrastructure ConnectToTestDatabase(EntityEngine entityEngine)
		{
			var access = DbInfrastructureHelper.GetDbAccessForTestDatabase ();

			var dataInfrastructure = new DataInfrastructure (access, entityEngine);
				
			dataInfrastructure.OpenConnection ("id");

			return dataInfrastructure;
		}
		

	}


}
