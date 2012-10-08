//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;

namespace Epsitec.Cresus.Core.Entities
{
	partial class TaxSettingsEntity
	{
		public override FormattedText GetSummary()
		{
			return TextFormatter.FormatText ("N°", this.VatNumber);
		}

		public override FormattedText GetCompactSummary()
		{
			return TextFormatter.FormatText ("N°", this.VatNumber);
		}
	}
}
