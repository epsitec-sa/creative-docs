//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;
using Epsitec.Common.Support.EntityEngine;

using Epsitec.Cresus.Core.Helpers;

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
			return TextFormatter.FormatText (this.Street.StreetName, ", ", this.Location.Country.Code, "-", this.Location.PostalCode, this.Location.Name);
		}

		public override string[] GetEntityKeywords()
		{
			return new string[] { this.Street.StreetName.ToSimpleText (), this.Location.Country.Code, this.Location.PostalCode.ToSimpleText (), this.Location.Name.ToSimpleText () };
		}

		public override EntityStatus GetEntityStatus()
		{
			var s1 = this.Street.GetEntityStatus ();
			var s2 = this.PostBox.GetEntityStatus ().TreatAsOptional ();
			var s3 = this.Location.GetEntityStatus ();

			return Helpers.EntityStatusHelper.CombineStatus (StatusHelperCardinality.All, s1, s2, s3);
		}
	}
}
