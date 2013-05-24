//	Copyright © 2012-2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Marc BETTEX, Maintainer: Pierre ARNAUD

using Epsitec.Aider.Enumerations;

using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;

using Epsitec.Common.Types;

using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Entities;

using Epsitec.Data.Platform;

using System;

using System.Linq;
using System.Collections.Generic;

namespace Epsitec.Aider.Entities
{
	public partial class AiderTownEntity
	{
		public override FormattedText GetSummary()
		{
			var text = StringUtils.Join
			(
				", ",
				StringUtils.Join
				(
					" ",
					StringUtils.Join ("-", this.Country.IsoCode, this.ZipCode),
					this.Name
				),
				this.Country.Name
			);

			return TextFormatter.FormatText (text);
		}

		public override FormattedText GetCompactSummary()
		{
			return TextFormatter.FormatText (this.ZipCode, this.Name);
		}

		public bool IsInVaudCounty()
		{
			return this.SwissCantonCode == "VD";
		}

		public override EntityStatus GetEntityStatus()
		{
			using (var a = new EntityStatusAccumulator ())
			{
				a.Accumulate (this.Name.GetEntityStatus ());
				a.Accumulate (this.SwissCantonCode.GetEntityStatus ().TreatAsOptional ());
				a.Accumulate (this.ZipCode.GetEntityStatus ().TreatAsOptional ());
				a.Accumulate (this.Country);

				return a.EntityStatus;
			}
		}

		public static AiderTownEntity FindOrCreate(BusinessContext businessContext, AiderCountryEntity country, string zipCode, string name, Mutability mutability)
		{
			return country.IsSwitzerland ()
				? AiderTownEntity.FindOrCreateSwissTown (businessContext, country, zipCode, name, mutability)
				: AiderTownEntity.FindOrCreateForeignTown (businessContext, country, zipCode, name, mutability);
		}

		private static AiderTownEntity FindOrCreateSwissTown(BusinessContext businessContext, AiderCountryEntity country, string zipCode, string name, Mutability mutability)
		{
			var swissZipCode = InvariantConverter.ParseInt (zipCode);

			var zipMatch = SwissPostZipRepository.Current
				.FindZips (swissZipCode, name)
				.FirstOrDefault ();

			if (zipMatch == null)
			{
				throw new ArgumentException ();
			}

			zipCode = InvariantConverter.ToString (zipMatch.ZipCode);
			name = zipMatch.LongName;

			var aiderTown = AiderTownEntity.Find (businessContext, country, zipCode, name);

			if (aiderTown == null)
			{
				aiderTown = AiderTownEntity.Create (businessContext, country, zipCode, name, mutability);
				aiderTown.SwissZipCode = zipMatch.ZipCode;
				aiderTown.SwissZipCodeId = zipMatch.OnrpCode;
				aiderTown.SwissZipType = zipMatch.ZipType;
				aiderTown.SwissCantonCode = zipMatch.Canton;
			}

			return aiderTown;
		}

		private static AiderTownEntity FindOrCreateForeignTown(BusinessContext businessContext, AiderCountryEntity country, string zipCode, string name, Mutability mutability)
		{
			return AiderTownEntity.Find (businessContext, country, zipCode, name)
				?? AiderTownEntity.Create (businessContext, country, zipCode, name, mutability);
		}

		private static AiderTownEntity Find(BusinessContext businessContext, AiderCountryEntity country, string zipCode, string name)
		{
			var example = new AiderTownEntity ()
			{
				Country = country,
				ZipCode = zipCode,
				Name = name,
			};

			return businessContext.DataContext
				.GetByExample<AiderTownEntity> (example)
				.FirstOrDefault ();
		}

		public static AiderTownEntity Create(BusinessContext businessContext, AiderCountryEntity country, string zipCode, string name, Mutability mutability)
		{
			var aiderTown = businessContext.CreateAndRegisterEntity<AiderTownEntity> ();

			aiderTown.Country = country;
			aiderTown.ZipCode = zipCode;
			aiderTown.Name = name;
			aiderTown.Mutability = mutability;

			return aiderTown;
		}

		partial void GetSwissZipCodeAddOn(ref string value)
		{
			var onrp = this.SwissZipCodeId.GetValueOrDefault ();
			var info = SwissPostZipRepository.Current.FindByOnrpCode (onrp);

			if (info == null)
			{
				value = null;
			}
			else
			{
				value = string.Format ("{0:00}", info.ZipComplement);
			}
		}

		partial void SetSwissZipCodeAddOn(string value)
		{
			throw new System.NotImplementedException ();
		}

        public static List<AiderTownEntity> GetTownFavoritesByUserScope(BusinessContext businessContext,AiderUserEntity user)
        {
            var townRepository = businessContext.Data.GetRepository<AiderTownEntity>();

            var scope = user.PreferredScope;
            if (scope.IsNull () || string.IsNullOrEmpty(scope.GroupPath))
            {
                var example = new AiderTownEntity
                {
                    SwissCantonCode = "VD"
                };
                return townRepository.GetByExample(example).ToList();
            }
            else
            {
                //TODO FIND TOWN BY SCOPE
                //ParishAddressRepository parishRepo = ParishAddressRepository.Current;
                //var parish = parishRepo.GetDetails(user.Parish.Name);
                //Replacement code:
                var example = new AiderTownEntity
                {
                    SwissCantonCode = "VD"
                };
                return townRepository.GetByExample(example).ToList();
            }
        }
	}
}
