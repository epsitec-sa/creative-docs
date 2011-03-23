using Epsitec.Common.Support;

using Epsitec.Cresus.Database;

using Epsitec.Cresus.DataLayer.Infrastructure;

using System.Collections.Generic;


namespace Epsitec.Cresus.DataLayer.Tests.Vs.Helpers
{


	internal static class DataInfrastructureHelper
	{


		public static DataInfrastructure ConnectToTestDatabase(DbInfrastructure dbInfrastructure)
		{
			DataInfrastructure dataInfrastructure = new DataInfrastructure (dbInfrastructure, DataInfrastructureHelper.GetEntityIds ());
				
			dataInfrastructure.OpenConnection ("id");

			return dataInfrastructure;
		}


		public static IEnumerable<Druid> GetEntityIds()
		{
			yield return new Druid ("[J1A4]");
			yield return new Druid ("[J1A6]");
			yield return new Druid ("[J1A9]");
			yield return new Druid ("[J1AE]");
			yield return new Druid ("[J1AG]");
			yield return new Druid ("[J1AJ]");
			yield return new Druid ("[J1AN]");
			yield return new Druid ("[J1AQ]");
			yield return new Druid ("[J1AT]");
			yield return new Druid ("[J1AV]");
			yield return new Druid ("[J1A11]");
			yield return new Druid ("[J1A41]");
			yield return new Druid ("[J1A61]");
			yield return new Druid ("[J1A81]");
			yield return new Druid ("[J1AA1]");
			yield return new Druid ("[J1AB1]");
			yield return new Druid ("[J1AE1]");
			yield return new Druid ("[J1AJ1]");
			yield return new Druid ("[J1AT1]");
			yield return new Druid ("[J1A02]");
			yield return new Druid ("[J1A42]");
			yield return new Druid ("[J1A72]");
		}
		

	}


}
