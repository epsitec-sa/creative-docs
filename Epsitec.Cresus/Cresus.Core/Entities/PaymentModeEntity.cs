//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Entities
{
	public partial class PaymentModeEntity
	{
		public FormattedText GetSummary()
		{
			return TextFormatter.FormatText
				(
					"Code: ",        this.Code,        "\n",
					"Résumé: ",      this.Name,        "\n",
					"Description: ", this.Description, "\n",
					"Compte: ",      this.BookAccount
				);
		}

		public FormattedText GetCompactSummary()
		{
			return TextFormatter.FormatText (this.Name);
		}
	}
}
