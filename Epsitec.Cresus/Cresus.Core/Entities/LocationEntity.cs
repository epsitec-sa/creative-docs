//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
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
			return TextFormatter.FormatText ("Pays: ", this.Country.Name, "\n", "Numéro postal: ", this.PostalCode, "\n", "Ville: ", this.Name);
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
			var s1 = this.PostalCode.GetEntityStatus ();
			var s2 = this.Name.GetEntityStatus ();

			return EntityStatusHelper.CombineStatus (StatusHelperCardinality.All, s1, s2);
		}
	}
}
