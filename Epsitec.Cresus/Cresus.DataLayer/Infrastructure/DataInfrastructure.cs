using Epsitec.Cresus.Database;


using System.Collections.Generic;


namespace Epsitec.Cresus.DataLayer.Infrastructure
{


	public sealed class DataInfrastructure
	{


		public DataInfrastructure(DbInfrastructure dbInfrastructure)
		{
			this.DbInfrastructure = dbInfrastructure;
		}


		public DbInfrastructure DbInfrastructure
		{
			get;
			private set;
		}


		public void CreateUidGenerator(string name, IEnumerable<System.Tuple<long, long>> slots)
		{
			UidGenerator.CreateUidGenerator (this.DbInfrastructure, name, slots);
		}


		public void DeleteUidGenerator(string name)
		{
			UidGenerator.DeleteUidGenerator (this.DbInfrastructure, name);
		}


		public bool UidGeneratorExists(string name)
		{
			return UidGenerator.UidGeneratorExists (this.DbInfrastructure, name);
		}


		public UidGenerator GetUidGenerator(string name)
		{
			return UidGenerator.GetUidGenerator (this.DbInfrastructure, name);
		}
	
	
	}


}
