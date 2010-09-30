//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;

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
			return TextFormatter.FormatText (this.PostalCode, " ", this.Name);
		}
	}
}
