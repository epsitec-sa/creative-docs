//	Copyright © 2012-2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Marc BETTEX, Maintainer: Pierre ARNAUD

using Epsitec.Aider.Entities;
using Epsitec.Aider.Enumerations;

using Epsitec.Common.Types;

using Epsitec.Cresus.Core.Business;

using Epsitec.Data.Platform;

using System.Collections.Generic;

namespace Epsitec.Aider.Data.Eerv
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

		public AiderTownEntity GetTown(EervAddress address)
		{
			var key = System.Tuple.Create (address.ZipCode, address.Town);

			AiderTownEntity town;

			if (!this.aiderTowns.TryGetValue (key, out town))
			{
				if (address.IsInSwitzerland ())
				{
					town = this.GetOrCreateSwissTown (InvariantConverter.ToInt (address.ZipCode), address.Town, Mutability.Customizable);
				}
				else
				{
					town = this.GetOrCreateForeignTown (address);
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
				town = this.GetOrCreateSwissTown (zip.ZipCode, name, Mutability.SystemDefined);
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
		
		private AiderTownEntity GetOrCreateSwissTown(int zipCode, string name, Mutability mutability)
		{
			return AiderTownEntity.FindOrCreate (this.businessContext, this.switzerland, zipCode, name, mutability);
		}

		private AiderTownEntity GetOrCreateForeignTown(EervAddress address)
		{
			var name = address.Town;
			var zipCode = address.ZipCode;

			return AiderTownEntity.FindOrCreate (this.businessContext, zipCode, name, Mutability.Customizable);
		}


		private readonly BusinessContext		businessContext;

		private readonly AiderCountryEntity		switzerland;

		private readonly Dictionary<System.Tuple<string, string>, AiderTownEntity> aiderTowns;
	}
}
