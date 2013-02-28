//	Copyright © 2012-2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Marc BETTEX, Maintainer: Pierre ARNAUD

using Epsitec.Aider.Enumerations;

using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;

using Epsitec.Common.Types;

using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Entities;

using Epsitec.Data.Platform;

using System.Linq;

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
			return TextFormatter.FormatText (this.Name);
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

		public static AiderTownEntity FindOrCreate(BusinessContext businessContext, AiderCountryEntity country, int zipCode, string name, Mutability mutability)
		{
			var aiderTown = AiderTownEntity.Find (businessContext, country, zipCode, name);

			if (aiderTown == null)
			{
				aiderTown = businessContext.CreateAndRegisterEntity<AiderTownEntity> ();

				int? zipOnrp = null;
				var zipMatch = SwissPostZipRepository.Current.FindZips (zipCode, name).FirstOrDefault ();
				string canton = null;
				SwissPostZipType? zipType = null;

				if (zipMatch != null)
				{
					zipOnrp = zipMatch.OnrpCode;
					zipType = zipMatch.ZipType;
					name    = zipMatch.LongName;
					canton  = zipMatch.Canton;
				}

				aiderTown.ZipCode         = InvariantConverter.ToString (zipCode);
				aiderTown.SwissZipCode    = zipCode;
				aiderTown.SwissZipCodeId  = zipOnrp;
				aiderTown.SwissZipType    = zipType;
				aiderTown.SwissCantonCode = canton;
				aiderTown.Name            = name;
				aiderTown.Country         = country;
				aiderTown.Mutability      = mutability;
			}

			return aiderTown;
		}

		public static AiderTownEntity Find(BusinessContext businessContext, AiderCountryEntity country, int zipCode, string name)
		{
			var example = new AiderTownEntity ()
			{
				Country = country,
				SwissZipCode = zipCode,
				Name = name,
			};

			return businessContext.DataContext
				.GetByExample<AiderTownEntity> (example)
				.FirstOrDefault ();
		}

		public static AiderTownEntity FindOrCreate(BusinessContext businessContext, string zipCode, string name, Mutability mutability)
		{
			var aiderTown = AiderTownEntity.Find (businessContext, zipCode, name);

			if (aiderTown == null)
			{
				aiderTown = AiderTownEntity.Create (businessContext, null, zipCode, name, mutability);
			}

			return aiderTown;
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


		public static AiderTownEntity Find(BusinessContext businessContext, string zipCode, string name)
		{
			var example = new AiderTownEntity ()
			{
				ZipCode = zipCode,
				Name = name,
			};

			return businessContext.DataContext
				.GetByExample<AiderTownEntity> (example)
				.FirstOrDefault ();
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
	}
}
