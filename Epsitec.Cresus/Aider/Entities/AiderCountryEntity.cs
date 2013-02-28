//	Copyright © 2011-2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Marc BETTEX, Maintainer: Marc BETTEX

using Epsitec.Aider.Enumerations;

using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Support.Extensions;

using Epsitec.Common.Types;

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

		public static AiderCountryEntity Create(BusinessContext businessContext, string isoCode, string name, Mutability mutability)
		{
			var country = businessContext.CreateAndRegisterEntity<AiderCountryEntity> ();

			country.IsoCode = isoCode;
			country.Name = name;
			country.Mutability = mutability;

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

		public static bool IsValidIsoCode(string isoCode)
		{
			return isoCode != null
				&& isoCode.Length == 2
				&& isoCode.IsAllUpperCase ()
				&& isoCode.IsAlpha ();
		}
	}
}
