using Epsitec.Cresus.Database;

using Epsitec.Cresus.DataLayer.Infrastructure;


namespace Epsitec.Cresus.DataLayer.Tests.Vs.Helpers
{


	internal static class DataInfrastructureHelper
	{


		public static DataInfrastructure ConnectToTestDatabase(DbInfrastructure dbInfrastructure)
		{
			DataInfrastructure dataInfrastructure = new DataInfrastructure (dbInfrastructure);
				
			dataInfrastructure.OpenConnection ("id");

			return dataInfrastructure;
		}
		

	}


}
