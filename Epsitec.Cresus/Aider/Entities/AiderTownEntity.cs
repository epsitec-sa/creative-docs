﻿//	Copyright © 2012-2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Marc BETTEX, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;

using Epsitec.Common.Types;

using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Entities;

using System.Linq;
using Epsitec.Data.Platform;

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
		
		public static AiderTownEntity FindOrCreate(BusinessContext businessContext, AiderCountryEntity country, int zipCode, string name)
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

		public static AiderTownEntity FindOrCreate(BusinessContext businessContext, string zipCode, string name)
		{
			var aiderTown = AiderTownEntity.Find (businessContext, zipCode, name);

			if (aiderTown == null)
			{
				aiderTown = businessContext.CreateAndRegisterEntity<AiderTownEntity> ();

				aiderTown.ZipCode = zipCode;
				aiderTown.Name = name;
			}

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
	}
}
