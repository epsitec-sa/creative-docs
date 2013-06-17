//	Copyright © 2012-2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Marc BETTEX, Maintainer: Pierre ARNAUD

using Epsitec.Aider.Data.Eerv;
using Epsitec.Aider.Entities;
using Epsitec.Aider.Enumerations;

using Epsitec.Common.Types;

using Epsitec.Cresus.Core.Business;

using Epsitec.Data.Platform;

using System.Collections.Generic;

namespace Epsitec.Aider.Data.Common
{
	/// <summary>
	/// The <c>AiderTownRepository</c> class is useful to manage towns that might exist or not.
	/// You ask it for at town and it creates it if it does not exist yet. It you ask for the
	/// same town again it will give back the same town that it created before and not a new one.
	/// </summary>
	internal sealed class AiderTownRepository
	{
		public AiderTownRepository(BusinessContext businessContext)
		{
			this.businessContext = businessContext;

			this.aiderTowns = new Dictionary<System.Tuple<string, string>, AiderTownEntity> ();
			this.switzerland = AiderCountryEntity.Find (this.businessContext, "CH");
		}

		
		public void AddMissingSwissTowns()
		{
			this.FillCache ();

			foreach (var zip in SwissPostZipRepository.Current.FindAll ())
			{
				this.GetTown (zip.OnrpCode, z => z.ZipType != SwissPostZipType.Internal);
			}
		}

		public AiderTownEntity GetTown(string zipCode, string name, string countryCode)
		{
			var key = System.Tuple.Create (zipCode, name);

			AiderTownEntity town;

			if (!this.aiderTowns.TryGetValue (key, out town))
			{
				if (countryCode == "CH")
				{
					town = this.GetOrCreateSwissTown (zipCode, name, Mutability.Customizable);
				}
				else
				{
					town = this.GetOrCreateForeignTown (zipCode, name, countryCode);
				}

				this.aiderTowns[key] = town;
			}

			return town;
		}

		public AiderTownEntity GetTown(int zipOnrp, System.Predicate<SwissPostZipInformation> filter = null)
		{
			var zip = SwissPostZipRepository.Current.FindByOnrpCode (zipOnrp);

			if (zip == null)
			{
				return null;
			}

			if ((filter != null) &&
				(filter (zip) == false))
			{
				return null;
			}

			var name = zip.LongName;
			var key  = System.Tuple.Create (InvariantConverter.ToString (zip.ZipCode), name);

			AiderTownEntity town;

			if (!this.aiderTowns.TryGetValue (key, out town))
			{
				town = this.GetOrCreateSwissTown (InvariantConverter.ToString(zip.ZipCode), name, Mutability.SystemDefined);
				this.aiderTowns[key] = town;
			}

			return town;
		}


		private void FillCache()
		{
			var example = new AiderTownEntity ()
			{
				Country = this.switzerland
			};

			var towns = this.businessContext.DataContext.GetByExample<AiderTownEntity> (example);

			foreach (var town in towns)
			{
				var key = System.Tuple.Create (town.ZipCode, town.Name);

				if (this.aiderTowns.ContainsKey (key) == false)
				{
					this.aiderTowns[key] = town;
				}
			}
		}
		
		private AiderTownEntity GetOrCreateSwissTown(string zipCode, string name, Mutability mutability)
		{
			return AiderTownEntity.FindOrCreate (this.businessContext, this.switzerland, zipCode, name, mutability);
		}

		private AiderTownEntity GetOrCreateForeignTown(string zipCode, string name, string countryCode)
		{
			var country = AiderCountryEntity.Find (this.businessContext, countryCode);

			return AiderTownEntity.FindOrCreate (this.businessContext, country, zipCode, name, Mutability.Customizable);
		}


		private readonly BusinessContext		businessContext;

		private readonly AiderCountryEntity		switzerland;

		private readonly Dictionary<System.Tuple<string, string>, AiderTownEntity> aiderTowns;
	}
}
