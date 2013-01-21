using Epsitec.Common.Support;

using Epsitec.Cresus.Database;

using Epsitec.Cresus.DataLayer.Schema;

using System.Collections.Generic;



namespace Epsitec.Cresus.DataLayer.Tests.Vs.Helpers
{


	internal static class EntityEngineHelper
	{


		public static EntityEngine ConnectToTestDatabase()
		{
			var entityIds = EntityEngineHelper.GetEntityTypeIds ();

			using (var dbInfrastructure = DbInfrastructureHelper.ConnectToTestDatabase ())
			{
				return EntityEngine.Connect (dbInfrastructure, entityIds);
			}
		}


		public static EntityEngine ConnectToTestDatabase(DbInfrastructure dbInfrastructure)
		{
			var entityIds = EntityEngineHelper.GetEntityTypeIds ();

			return EntityEngine.Connect (dbInfrastructure, entityIds);
		}


		public static IEnumerable<Druid> GetEntityTypeIds()
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
