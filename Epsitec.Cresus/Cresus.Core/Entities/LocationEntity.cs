//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;
using Epsitec.Common.Support.EntityEngine;

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

		public override EntityStatus EntityStatus
		{
			get
			{
				//	We consider a location to be empty if it has neither postal code, nor
				//	location name; a location with just a coutry or region is still empty.
				bool ok1 = !this.PostalCode.IsNullOrWhiteSpace;
				bool ok2 = !this.Name.IsNullOrWhiteSpace;

				if (!ok1 && !ok2)
				{
					return EntityStatus.Empty;
				}

				if (ok1 && ok2)
				{
					return EntityStatus.Valid;
				}

				return EntityStatus.Invalid;
			}
		}
	}
}
