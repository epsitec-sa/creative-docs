//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;

using Epsitec.Cresus.Core.Library;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Entities
{
	partial class VatDefinitionEntity
	{
		public override FormattedText GetSummary()
		{
			string emptyReplacement = TextFormatter.Command.EmptyReplacement + ":—";

			return TextFormatter.FormatText (
				this.Name, "\n",
				this.Description, "\n",
				"Code ", this.VatCode, "\n",
				"Taux ", this.Rate * 100, "%\n",
				"Du", this.BeginDate, emptyReplacement, "au", this.EndDate, emptyReplacement);
		}
		
		public override FormattedText GetCompactSummary()
		{
			return TextFormatter.FormatText (this.Name, "(", this.VatCode, ") à ", this.Rate * 100, "%");
		}
	}
}
