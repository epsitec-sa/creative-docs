//	Copyright © 2011-2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Marc BETTEX, Maintainer: Marc BETTEX

using Epsitec.Common.Support.EntityEngine;

using Epsitec.Common.Types;

using Epsitec.Cresus.Core;
using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Entities;

using System.Linq;

namespace Epsitec.Aider.Entities
{
	public partial class AiderCountryEntity
	{
		public override FormattedText GetSummary()
		{
			return TextFormatter.FormatText (this.Name, "(~", this.IsoCode, "~)");
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
				a.Accumulate (this.IsoCode.GetEntityStatus ().TreatAsOptional ());
				
				return a.EntityStatus;
			}
		}

		public static AiderCountryEntity FindOrCreate(BusinessContext businessContext, string isoCode, string name)
		{
			var country = AiderCountryEntity.Find (businessContext, isoCode);

			if (country == null)
			{
				country = businessContext.CreateAndRegisterEntity<AiderCountryEntity> ();

				country.IsoCode = isoCode;
				country.Name = name;
			}

			return country;
		}

		public static AiderCountryEntity Find(BusinessContext businessContext, string isoCode)
		{
			var example = new AiderCountryEntity ()
			{
				IsoCode = isoCode,
			};

			return businessContext.DataContext
				.GetByExample<AiderCountryEntity> (example)
				.FirstOrDefault ();
		}
	}
}
