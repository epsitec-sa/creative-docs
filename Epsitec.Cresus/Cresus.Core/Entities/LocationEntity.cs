//	Copyright � 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;
using Epsitec.Common.Support.EntityEngine;

using Epsitec.Cresus.Core.Helpers;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Entities
{
	public partial class LocationEntity
	{
		public override FormattedText GetSummary()
		{
			return TextFormatter.FormatText ("Pays: ", this.Country.Name, "\n", "Num�ro postal: ", this.PostalCode, "\n", "Ville: ", this.Name);
		}

		public override FormattedText GetCompactSummary()
		{
			return TextFormatter.FormatText (this.Country.Code, "-", this.PostalCode, " ", this.Name);
		}

		public override string[] GetEntityKeywords()
		{
			return new string[] { this.Country.Code, this.PostalCode.ToSimpleText (), this.Name.ToSimpleText () };
		}

		public override EntityStatus GetEntityStatus ()
		{
			//	We consider a location to be empty if it has neither postal code, nor
			//	location name; a location with just a coutry or region is still empty.
			using (var a = new EntityStatusAccumulator ())
			{
				a.Accumulate (this.PostalCode.GetEntityStatus ());
				a.Accumulate (this.Name.GetEntityStatus ());

				return a.EntityStatus;
			}
		}
	}
}
