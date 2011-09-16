//	Copyright © 2010-2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;
using Epsitec.Common.Support.EntityEngine;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Entities
{
	public partial class AddressEntity
	{
		public override FormattedText GetSummary()
		{
			return TextFormatter.FormatText
				(
					this.Street.StreetName, "\n",
					this.Location.PostalCode, this.Location.Name
				);
		}

		public override FormattedText GetCompactSummary()
		{
			return TextFormatter.FormatText (this.Street.StreetName, ", ", this.Location.Country.CountryCode, "-", this.Location.PostalCode, this.Location.Name);
		}

		public override string[] GetEntityKeywords()
		{
			return new string[] { this.Street.StreetName.ToSimpleText (), this.Location.Country.CountryCode, this.Location.PostalCode.ToSimpleText (), this.Location.Name.ToSimpleText () };
		}

		public override EntityStatus GetEntityStatus()
		{
			using (var a = new EntityStatusAccumulator ())
			{
				a.Accumulate (this.Street.GetEntityStatus ());
				a.Accumulate (this.PostBox.GetEntityStatus ().TreatAsOptional ());
				a.Accumulate (this.Location.GetEntityStatus ());

				return a.EntityStatus;
			}
		}
	}
}
