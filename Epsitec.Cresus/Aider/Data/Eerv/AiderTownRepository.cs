using Epsitec.Aider.Entities;

using Epsitec.Cresus.Core.Business;

using System;

using System.Collections.Generic;


namespace Epsitec.Aider.Data.Eerv
{


	/// <summary>
	/// The AiderTownRepository class is useful to manage towns that might exist or not. You ask it
	/// for at town and it creates it if it does not exist yet. It you ask for the same town again
	/// it will give back the same town that it created before and not a new one.
	/// </summary>
	internal sealed class AiderTownRepository
	{


		public AiderTownRepository(BusinessContext businessContext)
		{
			this.businessContext = businessContext;

			this.aiderTowns = new Dictionary<Tuple<string, string>, AiderTownEntity> ();
			this.switzerland = AiderCountryEntity.Find (this.businessContext, "CH", "Suisse");
		}


		public AiderTownEntity GetTown(EervAddress address)
		{
			var key = Tuple.Create (address.ZipCode, address.Town);

			AiderTownEntity town;

			if (!this.aiderTowns.TryGetValue (key, out town))
			{
				town = this.GetOrCreate (address);

				this.aiderTowns[key] = town;
			}

			return town;
		}


		private AiderTownEntity GetOrCreate(EervAddress address)
		{
			var name = address.Town;

			if (address.IsInSwitzerland ())
			{
				var zipCode = int.Parse (address.ZipCode);

				return AiderTownEntity.FindOrCreate (this.businessContext, this.switzerland, zipCode, name);
			}
			else
			{
				var zipCode = address.ZipCode;

				return AiderTownEntity.FindOrCreate (this.businessContext, zipCode, name);
			}
		}


		private readonly BusinessContext businessContext;


		private readonly AiderCountryEntity switzerland;


		private readonly Dictionary<Tuple<string, string>, AiderTownEntity> aiderTowns;


	}


}
